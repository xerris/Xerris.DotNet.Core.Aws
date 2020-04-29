using System;
using Amazon.SQS.Model;

namespace Xerris.DotNet.Core.Aws.Sqs
{
    public static class SqsExtensions
    {
        public static SendMessageRequest ApplyFifo(this SendMessageRequest request, bool isFifo)
        {
            if (isFifo)
            {
                var id = Guid.NewGuid().ToString("D");
                request.MessageGroupId = id;
                request.MessageDeduplicationId = id;
            }
            return request;
        }
    }
}