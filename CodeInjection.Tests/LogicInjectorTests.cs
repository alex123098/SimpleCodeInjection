﻿using System.Reflection;
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
	}

	public class ProxyInjectionTest : IInjectionTest
	{
		public ProxyInjectionTest(IInjectedPipeline p, IInjectionTest t) { }

		public void TestVoidMethod() {
			
		}

		public void TestVoidWithArgs(string stArg, int intArg, SimpleEnum enumArg) {
		}
	}
}
