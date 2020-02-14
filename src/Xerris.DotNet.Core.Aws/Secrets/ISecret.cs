using System.Threading.Tasks;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public interface ISecret
    {
        Task<string> GetSecretAsync();
    }
}