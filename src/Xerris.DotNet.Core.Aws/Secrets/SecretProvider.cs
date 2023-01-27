using System;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public class SecretProvider : ISecretProvider, IDisposable
    {
        private readonly SecretConfigCollection collection;
        private readonly IAmazonSecretsManager manager;
        private readonly ISecretsManagerCache cache;

        public SecretProvider(SecretConfigCollection collection, IAmazonSecretsManager manager)
        {
            this.collection = collection;
            this.manager = manager;
            cache = new SecretsManagerCache(manager,
                new SecretCacheConfiguration { MaxCacheSize = 128, CacheItemTTL = 600000 });
        }
        
        public ISecret GetAwsSecret(string name)
        {
            var config = GetConfig(name);
            return new CachedSecret(config.SecretId, config.Region, cache);
        }

        private SecretConfig GetConfig(string name)
        {
            try
            {
                return collection.GetConfig(name);
            }
            catch (Exception e)
            {
                throw new SecretException($"Configuration does not exist for '{name}'", e);
            }
        }

        public void Dispose()
        {
            manager?.Dispose();
            cache?.Dispose();
            collection.Items = null;
            GC.SuppressFinalize(this);
        }
    }
}