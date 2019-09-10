using CodeInjection.Activators;
using Moq;
using Xunit;

namespace CodeInjection.Tests
{
    public class DynamicActivatorFactoryTests
    {
        [Fact]
        public void CreateActivatorOf_DoesNotThrow()
        {
            IActivatorFactory activatorFactory = new DynamicActivatorFactory();

            activatorFactory.CreateActivatorOf<IInjectionTest>(typeof(ProxyInjectionTest));
        }

        [Fact]
        public void CreateActivatorOf_ReturnsActivatorMethod()
        {
            IActivatorFactory activatorFactory = new DynamicActivatorFactory();
            var factory = activatorFactory.CreateActivatorOf<IInjectionTest>(typeof(ProxyInjectionTest));
            var pipelineDummy = new Mock<IInjectedPipeline>();
            var instanceDummy = new Mock<IInjectionTest>();

            var activationResult = factory(pipelineDummy.Object, instanceDummy.Object);

            Assert.NotNull(activationResult);
        }
    }
}