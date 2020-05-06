using System;
using Amazon.SQS.Model;
using Amazon.Lambda.SQSEvents;

namespace Xerris.DotNet.Core.Aws.Sqs
{
    public static class SqsExtensions
    {
        public static string KeepWarm => "Xerris.DotNet.Aws.keep-warm";
        
        public static bool IsKeepWarm(this SQSEvent.SQSMessage message)
        {
            return message?.Body != null && message.Body.Contains(KeepWarm, StringComparison.InvariantCulture);
        }
        
        public static SendMessageRequest ApplyFifo(this SendMessageRequest request, string url)
        {
            if (!url.EndsWith(".fifo", StringComparison.InvariantCulture)) 
                return request;
            
            var id = Guid.NewGuid().ToString("D");
            request.MessageGroupId = id;
            request.MessageDeduplicationId = id;
            return request;
        }

        public static SendMessageBatchRequestEntry ApplyFifo(this SendMessageBatchRequestEntry request, string url)
        {
            if (!url.EndsWith(".fifo", StringComparison.InvariantCulture))
                return request;
            
            var id = Guid.NewGuid().ToString("D");
            request.MessageGroupId = id;
            request.MessageDeduplicationId = id;
            return request;
        }
    }
}