using System;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager.Extensions.Caching;
using FluentAssertions;
using Moq;
using Xerris.DotNet.Core.Aws.Secrets;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Secrets
{
    public class CachedSecretTest : IDisposable
    {
        private const string SecretId = "secretId";
        private readonly MockRepository repository = new MockRepository(MockBehavior.Strict);
        private readonly Mock<ISecretsManagerCache> cache;
        private readonly CachedSecret systemUnderTest;

        public CachedSecretTest()
        {
            cache = repository.Create<ISecretsManagerCache>();
            systemUnderTest = new CachedSecret(SecretId, RegionEndpoint.USEast2, cache.Object);
        }

        [Fact]
        public async Task ShouldGetStringSecret()
        {
            const string expected = "this is my secret";

            cache.Setup(c => c.GetSecretString(SecretId, default)).ReturnsAsync(expected);
            var actual = await systemUnderTest.GetSecretAsync();
            actual.Should().Be(expected);
        }
        
        [Fact]
        public async Task ShouldGetBinarySecret()
        {
            const string expected = "this is another secret";

            cache.Setup(c => c.GetSecretString(SecretId, default)).ReturnsAsync((string)null);
            cache.Setup(c => c.GetSecretBinary(SecretId, default)).ReturnsAsync(Encoding.UTF8.GetBytes(expected));

            var actual = await systemUnderTest.GetSecretAsync();
            actual.Should().Be(expected);
        }

        [Fact]
        public async void ShouldThrowSecretException()
        {
            cache.Setup(c => c.GetSecretString(SecretId, default)).ReturnsAsync((string)null);
            cache.Setup(c => c.GetSecretBinary(SecretId, default)).Throws<ApplicationException>();

            try
            {
                await systemUnderTest.GetSecretAsync();
                throw new Exception("Should throw SecretException");
            }
            catch (SecretException ex)
            {
                ex.Message.Should().Contain(SecretId);
                ex.Message.Should().Contain(RegionEndpoint.USEast2.DisplayName);
            }
        }

        [Fact]
        public async Task ShouldGetSecretByKey()
        {
            cache.Setup(c => c.GetSecretString(SecretId, default))
                .ReturnsAsync("{\"NutrienReports\":\"correct\",\"de-rp\":\"der-py\"}");

            var actual = await systemUnderTest.GetSecretAsync("NutrienReports");
            actual.Should().Be("correct");
        }

        public void Dispose()
        {
            repository.VerifyAll();
        }
    }
}