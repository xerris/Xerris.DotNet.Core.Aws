using System;
using System.Threading.Tasks;
using Moq;
using Xerris.AWS.Services.Services;
using Xunit;

namespace Xerris.AWS.Tests.Services
{
    public class HelloServiceTest : IDisposable
    {
        private readonly MockRepository mocks;

        private readonly HelloService service;

        public HelloServiceTest()
        {
            mocks = new MockRepository(MockBehavior.Strict);
            // service = new HelloService(client.Object, new GetCustomerRequestValidator());
        }

        [Fact]
        public Task GetCustomerList()
        {
            return Task.CompletedTask;
        }


        public void Dispose()
        {
            mocks.VerifyAll();
        }
    }
}
