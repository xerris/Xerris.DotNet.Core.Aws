using System;
using Amazon.Lambda.APIGatewayEvents;
using Serilog;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.Api
{
    public static class ApiGatewayProxyRequestExtensions
    {
        public const string Authorization = "Authorization";
        
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
                
        public static string GetAuthorization(this APIGatewayProxyRequest request)
        {
            if (request.Headers == null) return null; 
            request.Headers.TryGetValue(Authorization, out var value);
            return value;
        }
    }
}