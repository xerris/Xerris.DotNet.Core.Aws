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
    public interface IConsumeSqsMessages<T> where T : class
    {
        Task Process(IEnumerable<SQSEvent.SQSMessage> messages);
    }

    public abstract class SqsConsumer<T> : IConsumeSqsMessages<T> where T : class
    {
        private readonly IAmazonSQS sqsClient;
        private readonly bool deleteSuccessfulMessages;

        protected SqsConsumer(IAmazonSQS sqsClient, bool deleteSuccessfulMessages = true)
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
                return;
            
            Log.Information("Cannot remove SQS Status: {StatusCode} handle: {ReceiptHandle} ", 
                string.Join(response.HttpStatusCode.ToString(), Environment.NewLine), 
                string.Join(Environment.NewLine, message.ReceiptHandle));
        } 
    }
}