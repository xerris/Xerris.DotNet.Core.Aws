using System;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Moq;
using Xerris.DotNet.Core.Aws.Sqs;
using Xerris.DotNet.Core.Aws.Test.Sqs.Doubles;
using Xerris.DotNet.Core.Extensions;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Sqs
{
    public class SqsMessageProcessorTest : IDisposable
    {
        private readonly Mock<IConsumeSqsMessages<PersonMessage>> consumer;
        private readonly Mock<IPublishSqsMessages<PersonMessage>> publisher;
        private readonly MockRepository mocks;
        private readonly PersonProcessor processor;

        public SqsMessageProcessorTest()
        {
            mocks = new MockRepository(MockBehavior.Strict);
            consumer = mocks.Create<IConsumeSqsMessages<PersonMessage>>();
            publisher = mocks.Create<IPublishSqsMessages<PersonMessage>>();

            processor = new PersonProcessor(consumer.Object, publisher.Object);
        }

        [Fact]
        public void CanProcessMessage()
        {
            var elvis = new PersonMessage {Id = Guid.NewGuid(), Name = "Elvis"};
            var message = new SQSEvent.SQSMessage {Body = elvis.ToJson()};
            consumer.Setup(x => x.Process(new[] {message}))
                .Returns(Task.CompletedTask);
        
            processor.Process(new[] {message});
        }

        [Fact]
        public void CanSendMessage()
        {
            var elvis = new PersonMessage {Id = Guid.NewGuid(), Name = "Elvis"};
            publisher.Setup(x => x.SendMessageAsync(elvis, false))
                .ReturnsAsync(true);
        
            processor.SendMessageAsync(elvis);
        }

        [Fact]
        public void CanSendMessages()
        {
            var elvis = new PersonMessage {Id = Guid.NewGuid(), Name = "Elvis"};
            publisher.Setup(x => x.SendMessagesAsync(new[] {elvis}))
                .ReturnsAsync(true);
        
            processor.SendMessagesAsync(new[] {elvis});
        }

        public void Dispose()
        {
            mocks.VerifyAll();
        }
    }
}