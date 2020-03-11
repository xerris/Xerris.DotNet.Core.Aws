using System;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
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

        public static IServiceCollection AddSecretProvider(this IServiceCollection collection, SecretConfigCollection secretConfigCollection,
            AWSCredentials credentials)
        {
            var provider = new SecretProvider(secretConfigCollection, credentials);
            collection.AddSingleton<ISecretProvider>(provider);
            return collection;
        }
        
        public static IServiceCollection AddSecretProvider(this IServiceCollection collection, SecretConfigCollection secretConfigCollection,
            AWSOptions awsOptions)
        {
            var chain = new CredentialProfileStoreChain();
            if (chain.TryGetAWSCredentials(awsOptions.Profile, out var awsCredentials))
            {
                throw new Exception("Error establishing your AWS Credentials");
            }
            awsOptions.Credentials = awsCredentials;
            return AddSecretProvider(collection, secretConfigCollection, awsCredentials);
        }
    }
}