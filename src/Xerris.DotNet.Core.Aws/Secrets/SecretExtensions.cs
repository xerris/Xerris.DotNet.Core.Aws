using System.Collections.Generic;
using System.Threading.Tasks;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public static class SecretExtensions
    {
        public static async Task<string> GetSecretAsync(this ISecret secret, string key)
        {
            var pairs = await secret.GetSecretAsync();
            var dictionary = pairs.FromJson<Dictionary<string,string>>();
            return dictionary[key];
        }
    }
}