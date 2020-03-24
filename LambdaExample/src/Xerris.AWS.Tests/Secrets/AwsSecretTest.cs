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

namespace Xerris.AWS.Tests.Secrets
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
        public void ShouldThrowSecretException()
        {
            client.Setup(c =>
                    c.GetSecretValueAsync(It.Is<GetSecretValueRequest>(r => r.SecretId == SecretId),
                        CancellationToken.None))
                .Throws(new ApplicationException("bad things happen"));

            Func<Task> act = async () => await systemUnderTest.GetSecretAsync();
            act.Should().Throw<SecretException>()
                .Where(e => e.Message.Contains(SecretId, StringComparison.Ordinal)
                            && e.Message.Contains(RegionEndpoint.USEast2.DisplayName, StringComparison.Ordinal))
                .WithInnerException<ApplicationException>().WithMessage("bad things happen");
        }

        [Fact]
        public async Task ShouldGetSecretByKey()
        {
            var response = new GetSecretValueResponse {SecretString = "{\"NutrienReports\":\"correct\",\"derp\":\"derpy\"}"};

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