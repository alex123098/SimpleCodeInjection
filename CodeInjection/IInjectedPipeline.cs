using System.Diagnostics.Contracts;
using System.Reflection;

namespace CodeInjection
{
	[ContractClass(typeof(InjectedPipelineContract))]
	public interface IInjectedPipeline
	{
		void Add(IInjectedLogic injectedLogic);
		void ExecutePreCondition(object target, MethodInfo invokedMethodInfo, object[] arguments);
		void ExecutePostCondition(object target, MethodInfo invokedMethodInfo, object[] arguments);
	}

	[ContractClassFor(typeof(IInjectedPipeline))]
	public abstract class InjectedPipelineContract : IInjectedPipeline
	{
		public void Add(IInjectedLogic injectedLogic) {
			Contract.Requires(injectedLogic != null);
			throw new System.NotImplementedException();
		}

		public void ExecutePreCondition(object target, MethodInfo invokedMethodInfo, object[] arguments) {
			Contract.Requires(target != null);
			Contract.Requires(invokedMethodInfo != null);
			Contract.Requires(arguments != null);
			throw new System.NotImplementedException();
		}

		public void ExecutePostCondition(object target, MethodInfo invokedMethodInfo, object[] arguments) {
			Contract.Requires(target != null);
			Contract.Requires(invokedMethodInfo != null);
			Contract.Requires(arguments != null);
			throw new System.NotImplementedException();
		}
	}
}
