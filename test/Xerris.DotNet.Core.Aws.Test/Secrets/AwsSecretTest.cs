using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using FluentAssertions;
using Moq;
using Xerris.DotNet.Core.Aws.Secrets;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Secrets
{
    public class AwsSecretTest : IDisposable
    {
        private const string SecretId = "secretId";
        private readonly MockRepository repository = new MockRepository(MockBehavior.Strict);
        private readonly Mock<IAmazonSecretsManager> client;
        private readonly AwsSecret systemUnderTest;

        public AwsSecretTest()
        {
            client = repository.Create<IAmazonSecretsManager>();
            systemUnderTest = new AwsSecret(SecretId, RegionEndpoint.USEast2, client.Object);
        }

        [Fact]
        public async Task ShouldGetStringSecret()
        {
            var response = new GetSecretValueResponse {SecretString = "this is my secret"};

            client.Setup(c =>
                    c.GetSecretValueAsync(It.Is<GetSecretValueRequest>(r => r.SecretId == SecretId),
                        CancellationToken.None))
                .ReturnsAsync(response);

            var actual = await systemUnderTest.GetSecretAsync();
            actual.Should().Be(response.SecretString);
        }
        
        [Fact]
        public async Task ShouldGetBinarySecret()
        {
            const string expected = "this is another secret";

            using var stream = new MemoryStream();
            var s = Convert.ToBase64String(Encoding.UTF8.GetBytes(expected));
            stream.Write(Encoding.UTF8.GetBytes(s));
            stream.Flush();
            stream.Position = 0;

            var response = new GetSecretValueResponse {SecretBinary = stream};

            client.Setup(c =>
                    c.GetSecretValueAsync(It.Is<GetSecretValueRequest>(r => r.SecretId == SecretId),
                        CancellationToken.None))
                .ReturnsAsync(response);

            var actual = await systemUnderTest.GetSecretAsync();
            actual.Should().Be(expected);
        }

        [Fact]
        public async void ShouldThrowSecretException()
        {
            client.Setup(c =>
                    c.GetSecretValueAsync(It.Is<GetSecretValueRequest>(r => r.SecretId == SecretId),
                        CancellationToken.None))
                .Throws(new ApplicationException("bad things happen"));

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
            var response = new GetSecretValueResponse {SecretString = "{\"NutrienReports\":\"correct\",\"de-rp\":\"der-py\"}"};

            client.Setup(c =>
                    c.GetSecretValueAsync(It.Is<GetSecretValueRequest>(r => r.SecretId == SecretId),
                        CancellationToken.None))
                .ReturnsAsync(response);

            var actual = await systemUnderTest.GetSecretAsync("NutrienReports");
            actual.Should().Be("correct");
        }

        public void Dispose()
        {
            repository.VerifyAll();
        }
    }
}