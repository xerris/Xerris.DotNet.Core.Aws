using Amazon.Lambda.APIGatewayEvents;
using Xerris.DotNet.Core.Aws.Api;

namespace Xerris.AWS.Hello
{
    public static class ApiGatewayProxyRequestExtensions
    {
        public const string KeepWarmHeaderName = "x-keep-warm";
        public static bool IsKeepWarm(this APIGatewayProxyRequest request)
        {
            try
            {
                return request.Headers.TryGetValue(KeepWarmHeaderName, out var value) && bool.Parse(value);
            }
            catch
            {
                return false;
            }
        }

        public static string GetSourceContext(this APIGatewayProxyRequest request)
        {
            return request.GetQueryString("context");
        }
    }
}