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

        public static APIGatewayProxyRequest IncludeBody<T>(this APIGatewayProxyRequest request, T body)
        {
            request.Body = body.ToJson();
            return request;
        }
        
        public static T Parse<T>(this APIGatewayProxyRequest request) where T : class, new()
        {
            try
            {
                return request.Body.FromJson<T>();
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to deserialize {typeof(T).Name} from:{@Body}", request.Body);
                return new T();
            }
        }
        
        public static T Parse<T>(this APIGatewayProxyResponse response) where T : class, new()
        {
            try
            {
                return response.Body.FromJson<T>();
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to deserialize {typeof(T).Name} from:{@Body}", response.Body);
                return new T();
            }
        }

        public static string GetQueryString(this APIGatewayProxyRequest request, string key)
        {
            if (request.QueryStringParameters.IsNullOrEmpty()) return null;
            Validate.Begin().IsNotEmpty(key, $"query string key: {key}").Check();
            request.QueryStringParameters.TryGetValue(key, out var value);
            return value;
        }

        public static string GetPathParameter(this APIGatewayProxyRequest request, string key)
        {
            if (request.PathParameters.IsNullOrEmpty()) return null;
            Validate.Begin().IsNotEmpty(key, $"path parameter: {key}").Check();
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
        
        public static bool IsKeepWarm(this APIGatewayProxyRequest input)
        {
            try
            {
                return input.Headers.TryGetValue("x-keep-warm", out var str) && bool.Parse(str);
            }
            catch
            {
                return false;
            }
        }
    }
}