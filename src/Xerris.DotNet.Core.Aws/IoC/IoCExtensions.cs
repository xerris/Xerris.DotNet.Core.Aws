using Amazon.Runtime;
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
    }
}