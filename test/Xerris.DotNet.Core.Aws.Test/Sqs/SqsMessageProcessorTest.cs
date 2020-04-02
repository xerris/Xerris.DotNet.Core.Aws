using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using Xerris.DotNet.Core.Extensions;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Sqs
{
    public class SqsMessageProcessorTest : IDisposable
    {
        private readonly Mock<IAmazonSQS> sqsClient;
        private readonly MockRepository mocks;
        
        public SqsMessageProcessorTest()
        {
            mocks = new MockRepository(MockBehavior.Strict);
            sqsClient = mocks.Create<IAmazonSQS>();
        }

        [Fact]
        public void CanProcess_RemovedSqsMessage()
        {
            var elvis = new PersonMessage {Id = Guid.NewGuid(), Name = "Elvis"};
            var processor = new PersonProcessor(sqsClient.Object, x =>
            {
                x.Done = true;
                return Task.FromResult(true);
            });

            sqsClient.Setup(x => x.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), new CancellationToken()));
            
            var message = new SQSEvent.SQSMessage {Body = elvis.ToJson()}; 
            var result = processor.Process(new[] {message});
        }

        [Fact]
        public async Task CanProcess_DoesNotRemoveSqsMessage()
        {
            var elvis = new PersonMessage {Id = Guid.NewGuid(), Name = "Elvis"};
            var processor = new PersonProcessor(sqsClient.Object, x =>
            {
                x.Done = true;
                return Task.FromResult(true);
            }, false);
            
            var message = new SQSEvent.SQSMessage {Body = elvis.ToJson()};
            await processor.Process(new[] {message});
            
            sqsClient.Verify(x => x.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), new CancellationToken()), Times.Never);
        }

        [Fact]
        public async Task CanProcess_ActionUnsuccesful_DoesNotRemoveSqsMessage()
        {
            var elvis = new PersonMessage {Id = Guid.NewGuid(), Name = "Elvis"};
            var processor = new PersonProcessor(sqsClient.Object, x => Task.FromResult(false));
            
            var message = new SQSEvent.SQSMessage {Body = elvis.ToJson()};
            await processor.Process(new[] {message});

            sqsClient.Verify(x => x.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), new CancellationToken()), Times.Never);
        }

        public void Dispose()
        {
            mocks.VerifyAll();
        }
    }
}