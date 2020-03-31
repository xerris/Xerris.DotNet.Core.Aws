using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;

namespace Xerris.DotNet.Core.Aws.Sqs
{
    public interface IProcessSqsMessages<T> where T : class
    {
        Task Process(IEnumerable<SQSEvent.SQSMessage> messages);
    }
}