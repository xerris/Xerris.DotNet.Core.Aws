namespace Xerris.DotNet.Core.Aws.Secrets
{
    public interface ISecretProvider
    {
        ISecret GetAwsSecret(string name);
    }
}