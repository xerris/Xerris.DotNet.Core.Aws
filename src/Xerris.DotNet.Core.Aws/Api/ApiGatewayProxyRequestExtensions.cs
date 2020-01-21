using System;
using Amazon.Lambda.APIGatewayEvents;
using Serilog;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Core.Validations;

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

        public static string GetQueryString(this APIGatewayProxyRequest request, string key)
        {
            if (request.QueryStringParameters.IsNullOrEmpty()) return null;
            Validate.Begin().IsNotEmpty(key, "query string key").Check();
            request.QueryStringParameters.TryGetValue(key, out var value);
            return value;
        }

        public static string GetPathParameter(this APIGatewayProxyRequest request, string key)
        {
            if (request.PathParameters.IsNullOrEmpty()) return null;
            Validate.Begin().IsNotEmpty(key, "path parameter").Check();
            request.PathParameters.TryGetValue(key, out var value);
            return value;
        }
                
        public static string GetAuthorization(this APIGatewayProxyRequest request)
        {
            return GetHeader(request, Authorization);
        }
                
        public static string GetHeader(this APIGatewayProxyRequest request, string key)
        {
            if (request.Headers == null) return null; 
            request.Headers.TryGetValue(key, out var value);
            return value;
        }
    }
}