using CodeInjection.Caching;
using Moq;
using Xunit;

namespace CodeInjection.Tests
{
    public class CacheableProxyFactoryTests
    {
        [Fact]
        public void CreateProxyType_WillNotRecreateType()
        {
            var realInstanceMock = new Mock<IInjectionTest>();
            var pipelineMock = new Mock<IInjectedPipeline>();
            var decoratedProxyFactoryMock = new Mock<IProxyFactory>();
            decoratedProxyFactoryMock
                .Setup(pf => pf.CreateProxyType(It.IsAny<IInjectionTest>(), pipelineMock.Object))
                .Returns(typeof(IInjectionTest))
                .Verifiable();
            var proxyFactory = new CacheableProxyFactory(decoratedProxyFactoryMock.Object);

            proxyFactory.CreateProxyType(realInstanceMock.Object, pipelineMock.Object);
            proxyFactory.CreateProxyType(realInstanceMock.Object, pipelineMock.Object);

            decoratedProxyFactoryMock.Verify(
                pf => pf.CreateProxyType(realInstanceMock.Object, pipelineMock.Object),
                Times.Once);
        }
    }
}