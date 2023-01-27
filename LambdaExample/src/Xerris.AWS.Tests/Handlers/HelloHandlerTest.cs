using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Moq;
using Xerris.AWS.Hello.Handlers;
using Xerris.AWS.Services.Services;
using Xunit;

namespace Xerris.AWS.Tests.Handlers
{
    public class HelloHandlerTest : IDisposable
    {
        private readonly MockRepository mocks;
        private readonly Mock<IHelloService> service;
        private readonly APIGatewayProxyRequest request = new APIGatewayProxyRequest
        {
            Headers = new Dictionary<string, string>{{"Authorization", "jwt"}}
        };

        private readonly HelloHandler handler;

        public HelloHandlerTest()
        {
            mocks = new MockRepository(MockBehavior.Strict);
        }

        [Fact]
        public  Task GetCustomerList()
        { 
            return Task.CompletedTask;
        }

    
        public void Dispose()
        {
            mocks.VerifyAll();
        }
    }
}