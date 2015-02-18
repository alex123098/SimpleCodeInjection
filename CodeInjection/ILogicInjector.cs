using System;
using System.Diagnostics.Contracts;

namespace CodeInjection
{
	[ContractClass(typeof(LogicInjectorContract))]
	public interface ILogicInjector
	{
		T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline);
	}

	[ContractClassFor(typeof(ILogicInjector))]
	public abstract class LogicInjectorContract : ILogicInjector
	{
		public T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline) {
			Contract.Requires(realInstance != null);
			Contract.Requires(injectedPipeline != null);
			throw new NotImplementedException();
		}
	}
}
