using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.IoC;
using Xerris.DotNet.Core.Aws.Secrets;

namespace Xerris.Lambda.Api
{
    public class SecretConsumer
    {
        private readonly ILazyProvider<ISecretProvider> provider;

        public SecretConsumer() : this(IoC.Resolve<ILazyProvider<ISecretProvider>>()) 
        {
        }

        public SecretConsumer(ILazyProvider<ISecretProvider> provider)
        {
            this.provider = provider;
        }

        public async Task<string> GetSecret(string id, string key)
        {
            var value = await provider.Create().GetAwsSecret(id).GetSecretAsync(key);
            return value;
        }
    }
}