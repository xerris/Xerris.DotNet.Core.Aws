using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;

namespace Xerris.DotNet.Core.Aws.Sqs
{
    public abstract class SqsMessageProcessor<T> : IProcessSqsMessages<T> where T : class
    {
        private readonly IConsumeSqsMessages<T> consumer;
        private readonly IPublishSqsMessages<T> publisher;

        protected SqsMessageProcessor(IConsumeSqsMessages<T> consumer, IPublishSqsMessages<T> publisher)
        {
            this.consumer = consumer;
            this.publisher = publisher;
        }

        public async Task Process(IEnumerable<SQSEvent.SQSMessage> messages)
        {
            await consumer.Process(messages);
        }

        public async Task<bool> SendMessageAsync(T message)
        {
            var success = await publisher.SendMessageAsync(message);
            return success;
        }

        public async Task<bool> SendMessagesAsync(IEnumerable<T> messages)
        {
            var success = await publisher.SendMessagesAsync(messages);
            return success;
        }   
    }
}