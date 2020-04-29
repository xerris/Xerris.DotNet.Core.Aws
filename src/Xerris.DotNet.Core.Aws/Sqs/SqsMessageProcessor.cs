using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;

namespace Xerris.DotNet.Core.Aws.Sqs
{
    public abstract class SqsMessageProcessor<T> : IProcessSqsMessages<T> where T : class
    {
        private readonly IConsumeSqsMessages<T> consumer;
        private readonly IPublishSqsMessages<T> publisher;
        private bool isFifo;
        
        protected bool IsFifo
        {
            get => isFifo;
            set { isFifo = value; publisher.IsFifo = value; }
        }

        protected SqsMessageProcessor(IConsumeSqsMessages<T> consumer, IPublishSqsMessages<T> publisher)
        {
            this.consumer = consumer;
            this.publisher = publisher;
        }

        public async Task Process(IEnumerable<SQSEvent.SQSMessage> messages)
        {
            await consumer.Process(messages);
        }

        public async Task SendMessageAsync(T message)
        {
            await publisher.SendMessageAsync(message);
        }

        public async Task SendMessagesAsync(IEnumerable<T> messages)
        {
            await publisher.SendMessagesAsync(messages);
        }   
    }
}