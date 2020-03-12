using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.IoC;

namespace Xerris.Lambda.Api
{
    public class AppStartup : IAppStartup
    {
        public IConfiguration StartUp(IServiceCollection collection)
        {
            
            var builder = new ApplicationConfigurationBuilder<ApplicationConfig>();
            var appConfig = builder.Build();
            
            collection.AddSingleton<IApplicationConfig>(appConfig);
            collection.AddDefaultAWSOptions(appConfig.AwsOptions);
            collection.AddLazySecretProvider(appConfig.SecretConfigurations, appConfig.AwsOptions);

            return builder.Configuration;
        }
    }
}