using Xerris.DotNet.Core.Aws.Sqs;

namespace Xerris.DotNet.Core.Aws.Test.Sqs.Doubles
{
    public class PersonProcessor : SqsMessageProcessor<PersonMessage>
    {
        public PersonProcessor(IConsumeSqsMessages<PersonMessage> consumer, IPublishSqsMessages<PersonMessage> publisher, bool isFifo = false) 
                : base(consumer, publisher)
        {
            IsFifo = isFifo;
        }
    }
}