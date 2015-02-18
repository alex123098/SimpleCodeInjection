using System;
using System.Reflection;
using Xunit;
using Moq;

namespace CodeInjection.Tests
{
	public class InjectedPipelineTests
	{
		[Fact]
		public void AddLogic_WillNotThrowForLogicItem() {
			IInjectedPipeline pipeline = new InjectedPipeline();
			var logic = new Mock<IInjectedLogic>();

			pipeline.Add(logic.Object);
		}

		[Fact]
		public void ExecutePreCondition_WillExecuteTheDefinedAction() {
			IInjectedPipeline pipeline = new InjectedPipeline();
			var logic = new Mock<IInjectedLogic>();
			logic.Setup(l => l.BeforeExecute(It.IsAny<object>(), It.IsAny<MethodInfo>(), It.IsAny<object[]>(), It.IsAny<IInjectedLogic>()))
				.Returns((IInjectedLogic) null)
				.Verifiable();
			pipeline.Add(logic.Object);

			pipeline.ExecutePreCondition(new object(), typeof(object).GetMethod("ToString"), new object[0]);

			logic.Verify();
		}

		[Fact]
		public void ExecutePreCondition_WillProvideTheNextLogicValue() {
			IInjectedPipeline pipeline = new InjectedPipeline();
			var firstLogic = new Mock<IInjectedLogic>();
			var lastLogic = new Mock<IInjectedLogic>();
			firstLogic
				.Setup(
					l => l.BeforeExecute(
						It.IsAny<object>(), 
						It.IsAny<MethodInfo>(), 
						It.IsAny<object[]>(), 
						It.Is<IInjectedLogic>(logic => logic != null)))
				.Returns(lastLogic.Object)
				.Verifiable();
			lastLogic.Setup(
				l => l.BeforeExecute(
					It.IsAny<object>(), 
					It.IsAny<MethodInfo>(), 
					It.IsAny<object[]>(), 
					It.IsAny<IInjectedLogic>()))
				.Returns((IInjectedLogic)null)
				.Verifiable();

			pipeline.Add(firstLogic.Object);
			pipeline.Add(lastLogic.Object);

			pipeline.ExecutePreCondition(new object(), typeof(object).GetMethod("ToString"), new object[0]);

			firstLogic.Verify();
			lastLogic.Verify();
		}

		[Fact]
		public void ExecutePostCondition_WillExecuteTheDefinedAction() {
			IInjectedPipeline pipeline = new InjectedPipeline();
			var logic = new Mock<IInjectedLogic>();
			logic.Setup(l => l.AfterExecute(It.IsAny<object>(), It.IsAny<MethodInfo>(), It.IsAny<object[]>(), It.IsAny<IInjectedLogic>()))
				.Returns((IInjectedLogic) null)
				.Verifiable();
			pipeline.Add(logic.Object);

			pipeline.ExecutePostCondition(new object(), typeof(object).GetMethod("ToString"), new object[0]);

			logic.Verify();
		}

		[Fact]
		public void ExecutePostCondition_WillProvideTheNextLogicValue() {
			IInjectedPipeline pipeline = new InjectedPipeline();
			var firstLogic = new Mock<IInjectedLogic>();
			var lastLogic = new Mock<IInjectedLogic>();
			lastLogic
				.Setup(
					l => l.AfterExecute(
						It.IsAny<object>(),
						It.IsAny<MethodInfo>(),
						It.IsAny<object[]>(),
						It.Is<IInjectedLogic>(logic => logic != null)))
				.Returns(firstLogic.Object)
				.Verifiable();
			firstLogic.Setup(
				l => l.AfterExecute(
					It.IsAny<object>(),
					It.IsAny<MethodInfo>(),
					It.IsAny<object[]>(),
					It.IsAny<IInjectedLogic>()))
				.Returns((IInjectedLogic) null)
				.Verifiable();

			pipeline.Add(firstLogic.Object);
			pipeline.Add(lastLogic.Object);

			pipeline.ExecutePostCondition(new object(), typeof(object).GetMethod("ToString"), new object[0]);

			firstLogic.Verify();
			lastLogic.Verify();
		}

		[Fact]
		public void ExecutePreCondition_DoesNotThrow_WithoutLogic() {
			IInjectedPipeline pipeline = new InjectedPipeline();

			pipeline.ExecutePreCondition(new object(), typeof (object).GetMethod("ToString"), new object[0]);
		}

		[Fact]
		public void ExecutePostCondition_DoesNotThrow_WithoutLogic() {
			IInjectedPipeline pipeline = new InjectedPipeline();

			pipeline.ExecutePostCondition(new object(), typeof(object).GetMethod("ToString"), new object[0]);
		}

		[Fact]
		public void ExecutePreCondition_ThrowIfLogicThrows() {
			IInjectedPipeline pipeline = new InjectedPipeline();
			var logic = new Mock<IInjectedLogic>();
			logic.Setup(
				l => l.BeforeExecute(
					It.IsAny<object>(),
					It.IsAny<MethodInfo>(),
					It.IsAny<object[]>(),
					It.IsAny<IInjectedLogic>()))
				.Throws<InvalidOperationException>();
			pipeline.Add(logic.Object);

			Assert.Throws<InvalidOperationException>(
				() => pipeline.ExecutePreCondition(new object(), typeof (object).GetMethod("ToString"), new object[0]));
		}

		[Fact]
		public void ExecutePostCondition_ThrowIfLogicThrows() {
			IInjectedPipeline pipeline = new InjectedPipeline();
			var logic = new Mock<IInjectedLogic>();
			logic.Setup(
				l => l.AfterExecute(
					It.IsAny<object>(),
					It.IsAny<MethodInfo>(),
					It.IsAny<object[]>(),
					It.IsAny<IInjectedLogic>()))
				.Throws<InvalidOperationException>();
			pipeline.Add(logic.Object);

			Assert.Throws<InvalidOperationException>(
				() => pipeline.ExecutePostCondition(new object(), typeof(object).GetMethod("ToString"), new object[0]));
		}
	}
}
