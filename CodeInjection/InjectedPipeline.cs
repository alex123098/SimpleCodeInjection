using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace CodeInjection
{
	public class InjectedPipeline : IInjectedPipeline
	{
		private readonly LinkedList<IInjectedLogic> _registeredLogics = new LinkedList<IInjectedLogic>();

		public void Add(IInjectedLogic injectedLogic) {
			_registeredLogics.AddLast(injectedLogic);
		}

		public void ExecutePreCondition(object target, MethodInfo invokedMethodInfo, object[] arguments) {
			var logic = _registeredLogics.First;
			while (logic != null) {
				var nextLogic = logic.Next;
				logic.Value.BeforeExecute(target, invokedMethodInfo, arguments, nextLogic == null ? null : nextLogic.Value);
				logic = nextLogic;
			}
		}

		public void ExecutePostCondition(object target, MethodInfo invokedMethodInfo, object[] arguments) {
			var logic = _registeredLogics.Last;
			while (logic != null) {
				var prevLogic = logic.Previous;
				logic.Value.AfterExecute(target, invokedMethodInfo, arguments, prevLogic == null ? null : prevLogic.Value);
				logic = prevLogic;
			}
		}

		[ContractInvariantMethod]
		private void ContractInvariant() {
			Contract.Invariant(_registeredLogics != null);
		}
	}
}
