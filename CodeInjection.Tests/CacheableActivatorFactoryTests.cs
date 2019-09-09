using Moq;
using Xunit;

namespace CodeInjection.Tests
{
    public class CacheableActivatorFactoryTests
    {
        [Fact]
        public void CreateActivatorOf_WillRequestForActivatorOnce()
        {
            var realFactoryMock = new Mock<IActivatorFactory>();
            realFactoryMock
                .Setup(factory => factory.CreateActivatorOf<IInjectionTest>(typeof(ProxyInjectionTest)))
                .Returns((pipeline, instance) => new ProxyInjectionTest(pipeline, instance))
                .Verifiable();
            IActivatorFactory cacheableFactory = new CacheableActivatorFactory(realFactoryMock.Object);

            cacheableFactory.CreateActivatorOf<IInjectionTest>(typeof(ProxyInjectionTest));
            cacheableFactory.CreateActivatorOf<IInjectionTest>(typeof(ProxyInjectionTest));

            realFactoryMock.Verify(
                factory => factory.CreateActivatorOf<IInjectionTest>(typeof(ProxyInjectionTest)),
                Times.Once);
        }
    }
}