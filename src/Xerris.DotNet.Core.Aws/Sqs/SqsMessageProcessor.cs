using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using Serilog;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.Sqs
{
    public abstract class SqsMessageProcessor<T> : IProcessSqsMessages<T> where T : class
    {
        private readonly IAmazonSQS sqsClient;
        private readonly bool deleteSuccessfulMessages;

        protected SqsMessageProcessor(IAmazonSQS sqsClient, bool deleteSuccessfulMessages = true)
        {
            this.sqsClient = sqsClient;
            this.deleteSuccessfulMessages = deleteSuccessfulMessages;
        }

        protected abstract Func<T, Task<bool>> ExecuteAsync { get; }
        protected string SqsQueueUrl { get; set; }

        public async Task Process(IEnumerable<SQSEvent.SQSMessage> messages)
        {
            foreach (var eachMessage in messages)
            {
                var body = eachMessage.Body.FromJson<T>();
                var success = await ExecuteAsync(body);
                if (success)
                    await RemoveSqsMessage(eachMessage);
            }
        }

        public async Task<bool> SendMessageAsync(T message)
        {
            var request = new SendMessageRequest(SqsQueueUrl, message.ToJson());
            var response = await sqsClient.SendMessageAsync(request).ConfigureAwait(false);
            
            var successful = response.HttpStatusCode == HttpStatusCode.OK;
            if (!successful)
            {
                Log.Error("Sqs send failure: '{failureText}'", message.ToJson());
            }

            return successful;
        }

        public async Task<bool> SendMessagesAsync(IEnumerable<T> messages)
        {
            var batchRequestEntries = messages.Select((m, i) => new SendMessageBatchRequestEntry(i.ToString(), m.ToJson())).ToList();
            var request = new SendMessageBatchRequest(SqsQueueUrl, batchRequestEntries);
            var response = await sqsClient.SendMessageBatchAsync(request).ConfigureAwait(false);
            var successful = response.HttpStatusCode == HttpStatusCode.OK;
            
            if (!successful)
            {
                var failureText = response.Failed.Select(f => $"Unable to send '{batchRequestEntries[int.Parse(f.Id)].MessageBody}' because {f.Message}");
                Log.Error("Sqs batch send failure: {failureText}", string.Join(Environment.NewLine, failureText));
            }

            return successful;
        }
        
        private async Task RemoveSqsMessage(SQSEvent.SQSMessage message)
        {
            if (!deleteSuccessfulMessages) return;
            
            var deleteRequest = new DeleteMessageRequest
            {
                QueueUrl = SqsQueueUrl,
                ReceiptHandle = message.ReceiptHandle
            };
            var response = await sqsClient.DeleteMessageAsync(deleteRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                Log.Information($"Cannot remove SQS message for handle: { message.ReceiptHandle}");
        }      
    }
}