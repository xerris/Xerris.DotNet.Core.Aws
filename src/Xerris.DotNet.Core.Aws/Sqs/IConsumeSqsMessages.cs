using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using Serilog;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Core.Time;

namespace Xerris.DotNet.Core.Aws.Sqs
{
    public interface IConsumeSqsMessages<T> where T : class
    {
        Task Process(IEnumerable<SQSEvent.SQSMessage> messages);
    }

    public abstract class SqsConsumer<T> : IConsumeSqsMessages<T> where T : class
    {
        private readonly IAmazonSQS sqsClient;
        private string SqsQueueUrl { get; }
        private readonly bool deleteSuccessfulMessages;

        protected SqsConsumer(IAmazonSQS sqsClient, string sqsUrl, bool deleteSuccessfulMessages = false)
        {
            this.sqsClient = sqsClient;
            this.deleteSuccessfulMessages = deleteSuccessfulMessages;
            SqsQueueUrl = sqsUrl;
        }

        protected abstract Func<T, Task<bool>> ExecuteAsync { get; }

        public async Task Process(IEnumerable<SQSEvent.SQSMessage> messages)
        {
            foreach (var eachMessage in messages)
            {
                if (eachMessage.IsKeepWarm())
                {
                    Log.Debug("keep-warm invoked at: {Now}", Clock.Utc.Now.ToLongDateString());
                    continue;
                } 
                
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