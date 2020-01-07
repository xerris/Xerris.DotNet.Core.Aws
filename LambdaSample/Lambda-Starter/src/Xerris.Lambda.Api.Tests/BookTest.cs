using System;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FluentAssertions;
using Moq;
using Xerris.DotNet.Core.Extensions;
using Xerris.Lambda.Api.Handlers;
using Xerris.Lambda.Api.Request;
using Xunit;

namespace Xerris.Lambda.Api.Tests
{
    public class FunctionTest : IDisposable
    {
        private readonly MockRepository repository = new MockRepository(MockBehavior.Strict);


        [Fact]
        public void ShouldGetAllBooks()
        {
            var expected = new[]
            {
                new Book { ISBN ="abc-123", Title = "Slaughter House Five",  Author = "Kurt Vonnegut", Pages = 200 },
                new Book { ISBN ="abc-456", Title = "Bluebeard", Author = "Kurt Vonnegut", Pages = 150 }
            }.ToJson();

            var handler = new BookHandler();
            var actual = handler.GetBooks(new APIGatewayProxyRequest(), repository.Create<ILambdaContext>().Object);

            actual.Body.Should().Be(expected);
            actual.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        public void Dispose()
        {
            repository.VerifyAll();
        }
    }
}
