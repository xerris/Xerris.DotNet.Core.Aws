using Amazon.Lambda.APIGatewayEvents;

namespace Xerris.DotNet.Core.Aws.Api.Gateway
{
    public static class LambdaContextExtensions
    {
        public const string Authorization = "Authorization";
        
        public static string GetAuthorization(this APIGatewayProxyRequest request)
        {
            if (request.Headers == null) return null; 
            request.Headers.TryGetValue(Authorization, out var value);
            return value;
        }
    }
}