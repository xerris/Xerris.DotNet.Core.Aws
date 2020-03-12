using Amazon.Extensions.NETCore.Setup;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.Secrets;

namespace Xerris.Lambda.Api
{
    public interface IApplicationConfig : IApplicationConfigBase
    {
        SecretConfigCollection SecretConfigurations { get; }
    }

    public class ApplicationConfig : IApplicationConfig
    {
        public AWSOptions AwsOptions { get; set; }
        public SecretConfigCollection SecretConfigurations { get; set; }
    }
}