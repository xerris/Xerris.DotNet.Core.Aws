using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xerris.DotNet.Core.Aws.Secrets;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Secrets
{
    public class CachingSecretTest : IDisposable
    {
        private readonly MockRepository repository = new MockRepository(MockBehavior.Default);
        private readonly Mock<ISecret> secret;
        private readonly CachingSecret systemUnderTest;

        public CachingSecretTest()
        {
            secret = repository.Create<ISecret>();
            systemUnderTest = new CachingSecret(secret.Object);
        }

        [Fact]
        public async Task ShouldGetSecret()
        {
            const string expected = "shh this is a secret";
            secret.Setup(s => s.GetSecretAsync()).ReturnsAsync(expected);
            var actual = await systemUnderTest.GetSecretAsync();
            actual.Should().Be(expected);
        }
        
        [Fact]
        public async Task ShouldGetSecretFromCachedValue()
        {
            const string expected = "shh this is a secret";
            secret.Setup(s => s.GetSecretAsync()).ReturnsAsync(expected);
            for(var i = 0; i < new Random().Next(1,20); i++)
            {
                var actual = await systemUnderTest.GetSecretAsync();
                actual.Should().Be(expected);
            }
        }

        public void Dispose()
        {
            secret.Verify(s => s.GetSecretAsync(), Times.Once);
            repository.VerifyAll();
        }
    }
}