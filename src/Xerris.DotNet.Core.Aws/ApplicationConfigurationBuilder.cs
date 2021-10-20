using Microsoft.Extensions.Configuration;

namespace Xerris.DotNet.Core.Aws
{
    public class ApplicationConfigurationBuilder<T> : Core.ApplicationConfigurationBuilder<T> where T : IApplicationConfigBase, new()
    {
        protected override T Build(IConfiguration config, T appConfig)
        {
            appConfig.AwsOptions = config.GetAWSOptions();
            return appConfig;
        }
    }
}