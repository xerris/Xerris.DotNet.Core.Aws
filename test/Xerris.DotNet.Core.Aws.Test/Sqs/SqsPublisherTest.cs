using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using Xerris.DotNet.Core.Aws.Test.Sqs.Doubles;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Core.Validations;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Sqs
{
    public class SqsPublisherTest : IDisposable
    {
        private readonly MockRepository mocks;
        private readonly Mock<IAmazonSQS> sqsClient;
        private readonly PersonPublisher publisher;

        public SqsPublisherTest()
        {
            mocks = new MockRepository(MockBehavior.Strict);
            sqsClient = mocks.Create<IAmazonSQS>();
            publisher = new PersonPublisher(sqsClient.Object);
        }

        [Fact]
        public async Task CanSendMessage()
        {
            var elvis = new PersonMessage {Id = Guid.NewGuid(), Name = "Elvis"};
            var response = new SendMessageResponse();
            sqsClient.Setup(x => x.SendMessageAsync(It.Is<SendMessageRequest>(m => Matches(m, elvis)), 
                It.IsNotNull<CancellationToken>()))
                     .ReturnsAsync(response);

            await publisher.SendMessageAsync(elvis);
        }

        [Fact]
        public async Task CanSendMessages()
        {
            var elvis = new PersonMessage {Id = Guid.NewGuid(), Name = "Elvis"};
            var response = new SendMessageBatchResponse();
            sqsClient.Setup(x => x.SendMessageBatchAsync(It.IsNotNull<SendMessageBatchRequest>(), It.IsNotNull<CancellationToken>()))
                .ReturnsAsync(response);

            await publisher.SendMessagesAsync(new[] {elvis});
        }
        
        private static bool Matches(SendMessageRequest actual, PersonMessage expected)
        {
            Validate.Begin()
                .IsNotNull(actual, "actual").Check()
                .IsNotNull(expected, "expected").Check()
                .IsNotNull(actual.MessageBody, "messageBody")
                .Check();

            var person = actual.MessageBody.FromJson<PersonMessage>();
            
            return Validate.Begin()
                .IsNotNull(person, "person").Check()
                .IsEqual(person.Id, expected.Id, "id")
                .IsEqual(person.Name, expected.Name, "Name")
                .Check()
                .IsValid();
        }

        public void Dispose()
        {
            mocks.VerifyAll();
        }
    }
}