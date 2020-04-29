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

    public class SqsApplicationEventSink : SqsPublisher<ApplicationEvent>, IEventSink
    {
        public SqsApplicationEventSink(IAmazonSQS sqsClient, string sqsQueueUrl) : base(sqsClient, sqsQueueUrl)
        {
        }

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