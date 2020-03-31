using System;
using System.Collections.Generic;
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
        protected abstract string SqsQueueUrl { get; }

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