using System;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SecretsManager;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Xerris.DotNet.Core.Aws.Secrets;
using Xerris.DotNet.Core.Aws.Sqs;
using Xerris.DotNet.Core.Utilities.ApplicationEvents;

namespace Xerris.DotNet.Core.Aws.IoC
{
    public static class IoCExtensions
    {
        public static IServiceCollection AddLazyProviderFor<T>(this IServiceCollection collection)
        {
            collection.AddSingleton<ILazyProvider<T>>(provider => new LazyProvider<T>(provider.GetService<T>));
            return collection;
        }

        public static IServiceCollection AddSecretProvider(this IServiceCollection collection, SecretConfigCollection secretConfigCollection)
        {
            collection.AddAWSService<IAmazonSecretsManager>();
            collection.AddSingleton<ISecretProvider>(provider => 
                new SecretProvider(secretConfigCollection, provider.GetService<IAmazonSecretsManager>()));
            return collection;
        }
        
        public static IServiceCollection AddLazySecretProvider(this IServiceCollection collection, SecretConfigCollection secretConfigCollection)
        {
            collection.AddAWSService<IAmazonSecretsManager>();
            collection.AddSingleton<ILazyProvider<ISecretProvider>>(provider =>
                new LazyProvider<ISecretProvider>(() =>
                    new SecretProvider(secretConfigCollection, provider.GetService<IAmazonSecretsManager>())));
            return collection;
        }

        public static IServiceCollection AddApplicationMonitoringWithSqsSink(this IServiceCollection collection, string sqsUrl)
        {
            return AddApplicationMonitoringWithSqsSink(collection, () => sqsUrl);
        }
        
        public static IServiceCollection AddApplicationMonitoringWithSqsSink(this IServiceCollection collection, Func<string> sqsUrlFunc)
        {
            collection.AddAWSService<IAmazonSQS>();
            collection.AddSingleton<IEventSink>(
                services => new SqsApplicationEventSink(sqsUrlFunc(),services.GetService<IAmazonSQS>()));
            collection.AddSingleton<IMonitorBuilder, MonitorBuilder>();
            return collection;
        }
    }
}