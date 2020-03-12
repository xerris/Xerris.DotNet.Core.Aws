using FluentAssertions;
using Xerris.DotNet.Core;
using Xunit;

namespace Xerris.Lambda.Api.Tests
{
    public class IoCTest
    {
        [Fact]
        public void SecretConsumer()
        {
            IoC.Resolve<IApplicationConfig>().Should().NotBeNull();
            var consumer = new SecretConsumer(); //forces the lazySecretProvider to be pulled in.
            consumer.Should().NotBeNull();
        }
    }
}