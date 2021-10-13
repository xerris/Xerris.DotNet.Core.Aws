using Microsoft.Extensions.Configuration;

namespace Xerris.DotNet.Core.Aws
{
    public class ApplicationConfigurationAwsBuilder<T> : ApplicationConfigurationBuilder<T> where T : IApplicationConfigAwsBase, new()
    {
        protected override T Build(IConfiguration config, T appConfig)
        {
            appConfig.AwsOptions = config.GetAWSOptions();
            return appConfig;
        }
    }
}