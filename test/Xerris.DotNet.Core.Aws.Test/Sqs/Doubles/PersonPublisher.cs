using Amazon.SQS;
using Xerris.DotNet.Core.Aws.Sqs;

namespace Xerris.DotNet.Core.Aws.Test.Sqs.Doubles
{
    public interface IPersonPublisher : IPublishSqsMessages<PersonMessage> 
    {
    }
    
    public class PersonPublisher : SqsPublisher<PersonMessage>, IPersonPublisher
    {
        public PersonPublisher(IAmazonSQS sqsClient, string sqsUrl) : base(sqsClient, sqsUrl)
        {
        }
    }
}