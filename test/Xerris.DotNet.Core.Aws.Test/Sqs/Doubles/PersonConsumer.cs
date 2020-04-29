using System;
using System.Threading.Tasks;
using Amazon.SQS;
using Xerris.DotNet.Core.Aws.Sqs;

namespace Xerris.DotNet.Core.Aws.Test.Sqs.Doubles
{
    public class PersonConsumer : SqsConsumer<PersonMessage>
    {
        public PersonConsumer(IAmazonSQS sqsClient,string sqsUrl, Func<PersonMessage, Task<bool>> action,
                              bool deleteSuccessfulMessages = true) : 
            base(sqsClient, sqsUrl, deleteSuccessfulMessages)
        {
            ExecuteAsync = action;
        }

        protected override Func<PersonMessage, Task<bool>> ExecuteAsync { get; }
    }
}