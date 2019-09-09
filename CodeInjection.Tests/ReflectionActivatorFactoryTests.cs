using Moq;
using Xunit;

namespace CodeInjection.Tests
{
    public class ReflectionActivatorFactoryTests
    {
        [Fact]
        public void CreateActivatorOf_DoesNotThrow()
        {
            IActivatorFactory activatorFactory = new ReflectionActivatorFactory();

            activatorFactory.CreateActivatorOf<IInjectionTest>(typeof(ProxyInjectionTest));
        }

        [Fact]
        public void CreateActivatorOf_ReturnsActivatorMethod()
        {
            IActivatorFactory activatorFactory = new ReflectionActivatorFactory();
            var factory = activatorFactory.CreateActivatorOf<IInjectionTest>(typeof(ProxyInjectionTest));
            var pipelineDummy = new Mock<IInjectedPipeline>();
            var instanceDummy = new Mock<IInjectionTest>();

            var activationResult = factory(pipelineDummy.Object, instanceDummy.Object);

            Assert.NotNull(activationResult);
        }
    }
}