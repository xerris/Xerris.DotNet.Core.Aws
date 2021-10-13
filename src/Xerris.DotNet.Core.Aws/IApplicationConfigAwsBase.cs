using Amazon.Extensions.NETCore.Setup;

namespace Xerris.DotNet.Core.Aws
{
    public class IApplicationConfigAwsBase : IApplicationConfigBase
    {
        public AWSOptions AwsOptions { get; set; }
    }
}