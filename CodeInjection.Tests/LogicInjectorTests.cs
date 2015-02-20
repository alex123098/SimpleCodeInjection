using System.Reflection;
using System.Text;
using Moq;
using Xunit;

namespace CodeInjection.Tests
{
	public class LogicInjectorTests
	{
		[Fact]
		public void CreateProxy_DoesNotThrow() {
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);

			instance.TestVoidMethod();
		}

		[Fact]
		public void CreateProxy_WillCall_ProxyFactory() {
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			var proxyFactoryMock = new Mock<IProxyFactory>();
			proxyFactoryMock
				.Setup(pf => pf.CreateProxyType(It.IsAny<IInjectionTest>(), pipelineMock.Object))
				.Returns(typeof(ProxyInjectionTest))
				.Verifiable();
			injector.ProxyFactory = proxyFactoryMock.Object;

			injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);

			proxyFactoryMock.Verify(
				pf => pf.CreateProxyType(testInstance.Object, pipelineMock.Object),
				Times.Once);
		}

		[Fact]
		public void CreateProxy_CreatedProxyWillActivatePreExecutePipeline() {
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			pipelineMock
				.Setup(p => p.ExecutePreCondition(It.IsAny<object>(), It.IsAny<MethodInfo>(), It.Is<object[]>(a => a.Length == 0)))
				.Verifiable();

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);

			instance.TestVoidMethod();

			pipelineMock.Verify();
		}

		[Fact]
		public void CreateProxy_CreatedProxyWillActivatePostExecutePipeline() {
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			pipelineMock
				.Setup(p => p.ExecutePostCondition(It.IsAny<object>(), It.IsAny<MethodInfo>(), It.Is<object[]>(a => a.Length == 0)))
				.Verifiable();

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);

			instance.TestVoidMethod();

			pipelineMock.Verify();
		}

		[Fact]
		public void CreateProxy_CreatedProxyWillCallActualMethod() {
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			testInstance
				.Setup(i => i.TestVoidMethod())
				.Verifiable();

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);

			instance.TestVoidMethod();

			testInstance.Verify();
		}

		[Fact]
		public void CreateProxy_InvokationOfMethodWithArgs_DoesNotThrow() {
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);

			instance.TestVoidWithArgs(string.Empty, 0, SimpleEnum.One);
		}

		[Fact]
		public void CreateProxy_MethodWillPreserveObjectToReturn() {
			const string expected = "testString";
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			testInstance
				.Setup(i => i.TestReturnsStringBuilder())
				.Returns(new StringBuilder(expected));

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);
			var resut = instance.TestReturnsStringBuilder();

			Assert.Equal(expected, resut.ToString());
		}

		[Fact]
		public void CreateProxy_MethodWillPreserveStringToReturn() {
			const string expected = "testString";
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			testInstance
				.Setup(i => i.TestReturnsString())
				.Returns(expected);

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);
			var resut = instance.TestReturnsString();

			Assert.Equal(expected, resut);
		}

		[Fact]
		public void CreateProxy_MethodWillPreserveIntToReturn() {
			const int expected = 2;
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			testInstance
				.Setup(i => i.TestReturnsInt())
				.Returns(expected);

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);
			var resut = instance.TestReturnsInt();

			Assert.Equal(expected, resut);
		}

		[Fact]
		public void CreateProxy_MethodWillPreserveEnumToReturn() {
			const SimpleEnum expected = SimpleEnum.Two;
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			testInstance
				.Setup(i => i.TestReturnsEnum())
				.Returns(expected);

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);
			var resut = instance.TestReturnsEnum();

			Assert.Equal(expected, resut);
		}

		[Fact]
		public void CreateProxy_MethodWillPreserveBoolToReturn() {
			const bool expected = true;
			ILogicInjector injector = new LogicInjector();
			var testInstance = new Mock<IInjectionTest>();
			var pipelineMock = new Mock<IInjectedPipeline>();
			testInstance
				.Setup(i => i.TestReturnsBool())
				.Returns(expected);

			IInjectionTest instance = injector.CreateProxyFor(testInstance.Object, pipelineMock.Object);
			var resut = instance.TestReturnsBool();

			Assert.Equal(expected, resut);
		}
	}


	public enum SimpleEnum
	{
		One,
		Two
	}

	public interface IInjectionTest
	{
		void TestVoidMethod();

		void TestVoidWithArgs(string stArg, int intArg, SimpleEnum enumArg);

		StringBuilder TestReturnsStringBuilder();

		string TestReturnsString();

		int TestReturnsInt();

		SimpleEnum TestReturnsEnum();

		bool TestReturnsBool();
	}

	public class ProxyInjectionTest : IInjectionTest
	{
		public ProxyInjectionTest(IInjectedPipeline p, IInjectionTest t) { }

		public void TestVoidMethod() {
			
		}

		public void TestVoidWithArgs(string stArg, int intArg, SimpleEnum enumArg) {
		}

		public StringBuilder TestReturnsStringBuilder() {
			return new StringBuilder();
		}

		public string TestReturnsString() {
			return string.Empty;
		}

		public int TestReturnsInt() {
			return 0;
		}

		public SimpleEnum TestReturnsEnum() {
			return SimpleEnum.One;
		}

		public bool TestReturnsBool() {
			return false;
		}
	}
}
