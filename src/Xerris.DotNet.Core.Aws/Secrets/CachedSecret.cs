using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager.Extensions.Caching;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public class CachedSecret : ISecret
    {
        private readonly ISecretsManagerCache cache;
        private readonly RegionEndpoint region;
        private readonly string secretId;

        public CachedSecret(string secretId, string region, ISecretsManagerCache manager) : this(secretId, RegionEndpoint.GetBySystemName(region), manager)
        {
        }
        
        public CachedSecret(string secretId, RegionEndpoint region, ISecretsManagerCache cache)
        {
            this.secretId = secretId;
            this.cache = cache;
            this.region = region;
        }

        public async Task<string> GetSecretAsync()
        {
            try
            {
                var item = await cache.GetSecretString(secretId);
                
                if (!string.IsNullOrEmpty(item))
                {
                    return item;
                }
                
                return Encoding.UTF8.GetString(await cache.GetSecretBinary(secretId));
            }
            catch (Exception e)
            {
                throw new SecretException(secretId, region.ToString(), e) ;
            }
        }
    }
}