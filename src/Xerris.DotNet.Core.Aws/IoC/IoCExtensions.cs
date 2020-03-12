using System;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SecretsManager;
using Microsoft.Extensions.DependencyInjection;
using Xerris.DotNet.Core.Aws.Secrets;

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
    }
}