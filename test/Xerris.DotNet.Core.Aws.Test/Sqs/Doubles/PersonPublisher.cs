using Amazon.SQS;
using Xerris.DotNet.Core.Aws.Sqs;

namespace Xerris.DotNet.Core.Aws.Test.Sqs.Doubles
{
    public class PersonPublisher : SqsPublisher<PersonMessage>
    {
        public PersonPublisher(IAmazonSQS sqsClient) : base(sqsClient)
        {
        }
    }
}