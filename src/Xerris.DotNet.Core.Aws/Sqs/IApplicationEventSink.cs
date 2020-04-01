using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Serilog;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Core.Utilities.ApplicationEvents;

namespace Xerris.DotNet.Core.Aws.Sqs
{
    public interface IApplicationEventSink : IEventSink
    {
        
    }

    public class ApplicationEventSink : SqsMessageProcessor<ApplicationEvent>, IApplicationEventSink
    {
        public ApplicationEventSink(string sqsQueueUrl, IAmazonSQS sqsClient) : base(sqsClient, false)
        {
            SqsQueueUrl = sqsQueueUrl;
        }

        protected override Func<ApplicationEvent, Task<bool>> ExecuteAsync => throw new NotSupportedException("ApplicationEventSink does not support reading messages");

        public async Task SendAsync(ApplicationEvent applicationEvent)
        {
            try
            {
                await SendMessageAsync(applicationEvent).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to send '{body}' to sqs.", applicationEvent.ToJson());
            }
        }

        public async Task SendAsync(IEnumerable<ApplicationEvent> applicationEvents)
        {
            try
            {
                await SendMessagesAsync(applicationEvents).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to send '{number}' events to sqs.", applicationEvents.Count());
            }
        }
    }
}