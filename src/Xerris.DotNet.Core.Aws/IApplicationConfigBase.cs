using Amazon.Extensions.NETCore.Setup;

namespace Xerris.DotNet.Core.Aws
{
    public class IApplicationConfigBase : Core.IApplicationConfigBase
    {
        public AWSOptions AwsOptions { get; set; }
    }
}