using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public class AwsSecret : ISecret
    {
        private readonly string secretId;
        private readonly RegionEndpoint region;
        private readonly IAmazonSecretsManager client;

        public AwsSecret(string secretId, string region, AWSCredentials credentials)
        {
            this.secretId = secretId;
            this.region = RegionEndpoint.GetBySystemName(region);
            client = new AmazonSecretsManagerClient(credentials, this.region);
        }

        public AwsSecret(string secretId, RegionEndpoint region, IAmazonSecretsManager client)
        {
            this.secretId = secretId;
            this.region = region;
            this.client = client;
        }

        public async Task<string> GetSecretAsync()
        {
            try
            {
                var request = new GetSecretValueRequest {SecretId = secretId};
                var response = await client.GetSecretValueAsync(request);

                // Decrypts secret using the associated KMS CMK.
                // Depending on whether the secret is a string or binary, one of these fields will be populated.
                if (!string.IsNullOrEmpty(response.SecretString))
                {
                    return response.SecretString;
                }

                using (var reader = new StreamReader(response.SecretBinary))
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                }
            }
            catch (Exception e)
            {
                throw new SecretException(secretId, region.ToString(), e) ;
            }
        }
    }
}