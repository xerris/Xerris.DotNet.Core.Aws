using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.IoC;
using Xerris.DotNet.Core.Cache;

namespace Xerris.AWS.Services
{
    public sealed class AppStartup : IAppStartup
    {
        public IConfiguration StartUp(IServiceCollection collection)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            
            var builder = new ApplicationConfigurationBuilder<ApplicationConfig>();
            var appConfig = builder.Build();
            
            collection.AddSingleton<IApplicationConfig>(appConfig);
            collection.AddSecretProvider(appConfig.SecretConfigurations);
            collection.AddSingleton<ICache>(new WaitToFinishMemoryCache(2,10));
            collection.AutoRegister(GetType().Assembly);
            
            return builder.Configuration;
        }

        public void InitializeLogging(IConfiguration configuration, Action<IConfiguration> defaultConfig)
        {
            defaultConfig(configuration);
        }
    }
}