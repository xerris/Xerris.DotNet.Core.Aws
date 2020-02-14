using System;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public class SecretException : ApplicationException
    {
        public SecretException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
        public SecretException(string secretId, string region, Exception innerException) 
            : base($"Unable to obtain secret '{secretId}' in region {region}", innerException)
        {
        }
    }
}