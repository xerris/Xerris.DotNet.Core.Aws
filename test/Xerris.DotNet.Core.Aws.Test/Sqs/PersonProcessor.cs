using System;
using System.Threading.Tasks;
using Amazon.SQS;
using Xerris.DotNet.Core.Aws.Sqs;

namespace Xerris.DotNet.Core.Aws.Test.Sqs
{
    public class PersonProcessor : SqsMessageProcessor<PersonMessage>
    {
        public PersonProcessor(IAmazonSQS sqsClient, Func<PersonMessage, Task<bool>> action, bool deleteSuccessfulMessages = true) 
                : base(sqsClient, deleteSuccessfulMessages)
        {
            ExecuteAsync = action;
        }

        protected override Func<PersonMessage, Task<bool>> ExecuteAsync { get; }
        protected override string SqsQueueUrl => "http://test-queue.fifo";
    }
}