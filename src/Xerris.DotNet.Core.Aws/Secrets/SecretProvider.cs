using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.SecretsManager;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public class SecretProvider : ISecretProvider
    {
        private readonly Dictionary<string, ISecret> cache = new Dictionary<string, ISecret>();

        private readonly SecretConfigCollection collection;
        private readonly IAmazonSecretsManager manager;

        public SecretProvider(SecretConfigCollection collection, IAmazonSecretsManager manager)
        {
            this.collection = collection;
            this.manager = manager;
        }
        
        public ISecret GetAwsSecret(string name)
        {
            var config = GetConfig(name);
            lock (cache)
            {
                if (cache.TryGetValue(name, out var secret)) return secret;
                secret = new CachingSecret(new AwsSecret(config.SecretId, config.Region, manager));
                cache[name] = secret;

                return secret;
            }
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
    }
}