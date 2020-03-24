using FluentAssertions;
using Xerris.AWS.Services;
using Xerris.AWS.Services.Services;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.Secrets;
using Xerris.DotNet.Core.Cache;
using Xunit;

namespace Xerris.AWS.Tests.Services
{
    public class IoCTests 
    {
        public IoCTests()
        {
            IoC.Resolve<IApplicationConfig>();
        }
        
        [Fact]
        public void AppConfig() { IoC.Resolve<IApplicationConfig>().Should().NotBeNull(); }
        
        [Fact]
        public void SecretProvider() { IoC.Resolve<ISecretProvider>().Should().NotBeNull(); }
        
        [Fact]
        public void Cache() { IoC.Resolve<ICache>().Should().NotBeNull(); }
        
        [Fact]
        public void HelloService() { IoC.Resolve<IHelloService>().Should().NotBeNull(); }
    }
}