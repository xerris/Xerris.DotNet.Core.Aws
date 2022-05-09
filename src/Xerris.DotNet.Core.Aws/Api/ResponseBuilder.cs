using System;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Xerris.DotNet.Core.Extensions;

namespace Xerris.DotNet.Core.Aws.Api
{
    public static class ResponseBuilder
    {
        private static readonly IDictionary<string,string> Headers = new Dictionary<string, string>
        {
            {"Access-Control-Allow-Origin", "*"},
            {"Access-Control-Allow-Credentials", "true"},
            {"Access-Control-Allow-Headers", "Content-Type,X-Api-Key,Authorization,X-Api-Key,X-Amz-Security-Token"},
            {"Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE, HEAD"},
            {"Content-type", "application/json; charset=UTF-8"}
        };

        private static APIGatewayProxyResponse CreateResponse(this string payload, HttpStatusCode statusCode)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)statusCode,
                Body = payload,
                Headers = Headers
            };
        }

        public static APIGatewayProxyResponse Ok(this string payload)
        {
            return CreateResponse(payload, HttpStatusCode.OK);
        }

        public static APIGatewayProxyResponse Ok<T>(this T payload, JsonSerializerSettings settings = null)
        {
            return CreateResponse(payload.ToJson(settings), HttpStatusCode.OK);
        }

        public static APIGatewayProxyResponse Created<T>(this T payload, JsonSerializerSettings settings = null)
        {
            return CreateResponse(payload.ToJson(settings), HttpStatusCode.Created);
        }

        public static APIGatewayProxyResponse Accepted<T>(this T payload, JsonSerializerSettings settings = null)
        {
            return CreateResponse(payload.ToJson(settings), HttpStatusCode.Accepted);
        }
        
        public static APIGatewayProxyResponse Error(this string payload)
        {
            return CreateResponse(payload, HttpStatusCode.InternalServerError);
        }
        
        public static APIGatewayProxyResponse Error<T>(this T payload, JsonSerializerSettings settings = null)
        {
            return CreateResponse(payload.ToJson(settings), HttpStatusCode.InternalServerError);
        }

        public static APIGatewayProxyResponse NotFound(this string payload)
        {
            return CreateResponse(payload, HttpStatusCode.NotFound);
        }

        public static APIGatewayProxyResponse NotFound<T>(this T payload, JsonSerializerSettings settings = null)
        {
            return CreateResponse(payload.ToJson(settings), HttpStatusCode.NotFound);
        }

        public static APIGatewayProxyResponse BadRequest<T>(this T payload, JsonSerializerSettings settings = null)
        {
            return CreateResponse(payload.ToJson(settings), HttpStatusCode.BadRequest);
        }

        public static APIGatewayProxyResponse BadRequest(this string payload)
        {
            return CreateResponse(payload, HttpStatusCode.BadRequest);
        }

        public static APIGatewayProxyResponse UnAuthorized<T>(this T payload, JsonSerializerSettings settings = null)
        {
            return CreateResponse(payload.ToJson(settings), HttpStatusCode.Unauthorized);
        }
        
        public static APIGatewayProxyResponse UnAuthorized(this string message)
        {
            return CreateResponse(message, HttpStatusCode.Unauthorized);
        }

        public static APIGatewayProxyResponse Warmed(this object called)
        {
            return Ok(new Ack());
        }
        
        private class Ack
        {
            public Ack()
            {
                WarmedAt = DateTime.UtcNow.ToString("s");
            }
            
            public string WarmedAt { get; set; }
        }
    }
}