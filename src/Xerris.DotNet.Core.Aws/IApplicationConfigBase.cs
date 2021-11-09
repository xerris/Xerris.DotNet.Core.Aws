using Amazon.Extensions.NETCore.Setup;

namespace Xerris.DotNet.Core.Aws
{
    public interface IApplicationConfigBase : Core.IApplicationConfigBase
    {
        AWSOptions AwsOptions { get; set; }
    }
}