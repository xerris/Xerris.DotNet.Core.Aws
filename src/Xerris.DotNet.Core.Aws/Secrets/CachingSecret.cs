using System.Threading.Tasks;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public class CachingSecret : ISecret
    {
        private ISecret inner;
        private string savedSecret;

        public CachingSecret(ISecret inner)
        {
            this.inner = inner;
        }
        
        public async Task<string> GetSecretAsync()
        {
            if (string.IsNullOrEmpty(savedSecret))
            {
                savedSecret = await inner.GetSecretAsync();
                inner = null; // dont need inner anymore let's free up some memory
            }

            return savedSecret;
        }
    }
}