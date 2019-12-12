using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;
using Xerris.DotNet.Core.Aws.Api;
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
            var foo = "hi, I'm not a foo";
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
        
    }
}