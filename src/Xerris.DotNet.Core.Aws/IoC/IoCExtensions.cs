using Microsoft.Extensions.DependencyInjection;

namespace Xerris.DotNet.Core.Aws.IoC
{
    public static class IoCExtensions
    {
        public static IServiceCollection AddLazyProviderFor<T>(this IServiceCollection collection)
        {
            collection.AddSingleton<ILazyProvider<T>>(provider => new LazyProvider<T>(provider.GetService<T>));
            return collection;
        }
    }
}