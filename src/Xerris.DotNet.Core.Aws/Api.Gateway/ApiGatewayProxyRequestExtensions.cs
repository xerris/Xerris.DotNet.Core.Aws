using System;
using Amazon.Lambda.APIGatewayEvents;
using Serilog;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.Api.Gateway
{
    public static class ApiGatewayProxyRequestExtensions
    {
        public static T Parse<T>(this APIGatewayProxyRequest request) where T : class, new()
        {
            try
            {
                return request.Body.FromJson<T>();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Unable to deserialize {typeof(T).Name} from: {request.Body}");
                return new T();
            }
        }
    }
}