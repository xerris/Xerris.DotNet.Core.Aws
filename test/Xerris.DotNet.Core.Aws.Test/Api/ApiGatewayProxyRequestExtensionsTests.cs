using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;
using Xerris.DotNet.Core.Aws.Api;
using Xerris.DotNet.Core.Aws.Test.Model;
using Xerris.DotNet.Core.Extensions;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Api
{
    public class ApiGatewayProxyRequestExtensionsTests
    {
        [Fact]
        public void Parse()
        {
            var foo = new Foo {FirstName = "first", LastName = "last"};
            var request = new APIGatewayProxyRequest {Body = foo.ToJson() };

            request.Parse<Foo>().Should().BeEquivalentTo(foo);
        }
        
        [Fact]
        public void Parse_NotAFoo()
        {
            const string foo = "hi, I'm not a foo";
            var request = new APIGatewayProxyRequest {Body = foo };

            request.Parse<Foo>().Should().BeEquivalentTo(new Foo());
        }
        
        [Fact]
        public void Authorization_Found()
        {
            var headers = new Dictionary<string, string> {{ApiGatewayProxyRequestExtensions.Authorization, "auth"}};
            var request = new APIGatewayProxyRequest {Headers = headers};
            request.GetAuthorization().Should().Be("auth");
        }
        
        [Fact]
        public void GetHeader()
        {
            var headers = new Dictionary<string, string> {{"com.xerris.header", "Hi there"}};
            var request = new APIGatewayProxyRequest {Headers = headers};
            request.GetHeader("com.xerris.header").Should().Be("Hi there");
        }
        
        [Fact]
        public void GetHeader_KeyNotFound()
        {
            var headers = new Dictionary<string, string> {{"Hi there", "com.xerris.header"}};
            var request = new APIGatewayProxyRequest {Headers = headers};
            request.GetHeader("kaka").Should().BeNull();
        }
        
        [Fact]
        public void GetHeader_NoHeaders()
        {
            var request = new APIGatewayProxyRequest {Headers = null};
            request.GetHeader("kaka").Should().BeNull();
        }
        
        [Fact]
        public void Authorization_NoHeaders()
        {
            var request = new APIGatewayProxyRequest();
            request.GetAuthorization().Should().BeNullOrEmpty();
        }
        
        [Fact]
        public void Authorization_NoAuthorizationHeader()
        {
            var headers = new Dictionary<string, string> {{"A Different Header", "boo!"}};
            var request = new APIGatewayProxyRequest {Headers = headers};
            request.GetAuthorization().Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetQueryString()
        {
            var queryStrings = new Dictionary<string, string> {{"key", "value"}};
            var request = new APIGatewayProxyRequest {QueryStringParameters = queryStrings};
            request.GetQueryString("key").Should().Be("value");
        }

        [Fact]
        public void GetQueryString_NotFound()
        {
            var queryStrings = new Dictionary<string, string> {{"key", "value"}};
            var request = new APIGatewayProxyRequest {QueryStringParameters = queryStrings};
            request.GetQueryString("kaka").Should().BeNull();
        }

        [Fact]
        public void GetQueryString_NoQueryStringParameters()
        {
            var request = new APIGatewayProxyRequest {QueryStringParameters = null};
            request.GetQueryString("kaka").Should().BeNull();
        }

        [Fact]
        public void GetPathParameter()
        {
            var pathParameters = new Dictionary<string, string> {{"key", "value"}};
            var request = new APIGatewayProxyRequest {PathParameters = pathParameters};
            request.GetPathParameter("key").Should().Be("value");
        }

        [Fact]
        public void GetPathParameter_WrongKey()
        {
            var pathParameters = new Dictionary<string, string> {{"key", "value"}};
            var request = new APIGatewayProxyRequest {PathParameters = pathParameters};
            request.GetPathParameter("kaka").Should().BeNull();
        }

        [Theory]
        [InlineData("x-keep-warm", "true", true)]
        [InlineData("x-keep-warm", "True", true)]
        [InlineData("x-keep-warm", "False", false)]
        [InlineData("x-keep-warm", "false", false)]
        [InlineData("x-kaka", "True", false)]
        [InlineData("x-kaka", "true", false)]
        [InlineData("", "true", false)]
        public void IsKeepWarm(string key, string value, bool isWarm)
        {
            var headers = new Dictionary<string, string> {{key, value}};
            var request = new APIGatewayProxyRequest {Headers = headers};
            request.IsKeepWarm().Should().Be(isWarm);
        }
    }
}