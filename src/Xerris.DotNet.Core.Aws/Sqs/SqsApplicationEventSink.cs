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

    public class ApplicationEventSink : SqsMessageProcessor<ApplicationEvent>, IEventSink
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
                Log.Information("Sending ApplicationEvent [{identifier}]", applicationEvent.Identifier);
                await SendMessageAsync(applicationEvent).ConfigureAwait(false);
                Log.Information("Sent ApplicationEvent [{identifier}]", applicationEvent.Identifier);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to send '{body}' to sqs.", applicationEvent.ToJson());
            }
        }

        public async Task SendAsync(IEnumerable<ApplicationEvent> applicationEvents)
        {
            var identifiersToBeSent = applicationEvents.Select(i => i.Identifier).ToArray();
            try
            {
                
                Log.Information("Sent ApplicationEventd [{identifier}]", identifiersToBeSent);
                await SendMessagesAsync(applicationEvents).ConfigureAwait(false);
                Log.Information("Sent ApplicationEvents [{identifier}]", identifiersToBeSent);

            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to send '{number}' events to sqs.", identifiersToBeSent.Length);
            }
        }
    }
}