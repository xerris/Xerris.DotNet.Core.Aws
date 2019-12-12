using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.Api
{
    public static class ResponseBuilder
    {
        private static readonly IDictionary<string,string> Headers = new Dictionary<string, string>
        {
            {"Access-Control-Allow-Origin", "*"},
            {"Access-Control-Allow-Credentials", "true"},
            {"Access-Control-Allow-Headers", "Content-Type,X-Api-Key"},
            {"Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE, HEAD"},
            {"Content-type", "application/json; charset=UTF-8"}
        };

        public static APIGatewayProxyResponse Ok(this string payload)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.OK,
                Body = payload,
                Headers = Headers
            };
        }

        public static APIGatewayProxyResponse Ok<T>(this T payload)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.OK,
                Body = payload.ToJson(),
                Headers = Headers
            };
        }

        public static APIGatewayProxyResponse Created<T>(this T payload)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.Created,
                Body = payload.ToJson(),
                Headers = Headers
            };
        }

        public static APIGatewayProxyResponse Accepted<T>(this T payload)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.Accepted,
                Body = payload.ToJson(),
                Headers = Headers
            };
        }
        
        public static APIGatewayProxyResponse Error(this string payload)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.InternalServerError,
                Body = payload,
                Headers = Headers
            };
        }

        public static APIGatewayProxyResponse BadRequest(this string payload)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.BadRequest,
                Body = payload,
                Headers = Headers
            };
        }

        public static APIGatewayProxyResponse BadRequest<T>(this T payload)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.BadRequest,
                Body = payload.ToJson(),
                Headers = Headers
            };
        }

        public static APIGatewayProxyResponse UnAuthorized(this object item)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.Unauthorized,
                Headers = Headers
            };
        }
    }
}