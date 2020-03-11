using System;
using Amazon.Runtime;
using FluentAssertions;
using Moq;
using Xerris.DotNet.Core.Aws.Secrets;
using Xunit;

namespace Xerris.DotNet.Core.Aws.Test.Secrets
{
    public class SecretProviderTest
    {
        private readonly MockRepository mocks;

        private Mock<AWSCredentials> credentials;

        public SecretProviderTest()
        {
            mocks = new MockRepository(MockBehavior.Loose);
            credentials = mocks.Create<AWSCredentials>();
        }
        
        [Fact]
        public void ShouldGetSecret()
        {
            const string configName = "test";
            var collection = new SecretConfigCollection
            {
                Items = new []{ new SecretConfig {Name = configName, Region = "us-east-2", SecretId = "a secret"}}
            };

            var systemUnderTest = new SecretProvider(collection, credentials.Object);
            var actual = systemUnderTest.GetAwsSecret(configName);
            actual.Should().NotBeNull();
            actual.Should().BeAssignableTo<CachingSecret>();
        }
        
        [Fact]
        public void ShouldNotFindConfigInCollection()
        {
            var collection = new SecretConfigCollection
            {
                Items = new []{ new SecretConfig {Name = "test", Region = "us-east-2", SecretId = "a secret"}}
            };

            var systemUnderTest = new SecretProvider(collection, credentials.Object);
            Action act = () => systemUnderTest.GetAwsSecret("fail");
            act.Should().Throw<SecretException>();
        }
    }
}