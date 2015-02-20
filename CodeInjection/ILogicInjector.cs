using System;
using System.Diagnostics.Contracts;

namespace CodeInjection
{
	[ContractClass(typeof(LogicInjectorContract))]
	public interface ILogicInjector
	{
		T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline);

		IProxyFactory ProxyFactory { get; set; }
	}

	[ContractClassFor(typeof(ILogicInjector))]
	public abstract class LogicInjectorContract : ILogicInjector
	{
		public T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline) {
			Contract.Requires(realInstance != null);
			Contract.Requires(injectedPipeline != null);
			throw new NotImplementedException();
		}

		public IProxyFactory ProxyFactory {
			[Pure] get {
				Contract.Ensures(Contract.Result<IProxyFactory>() != null);
				throw new NotImplementedException();
			}
			set {
				Contract.Requires(value != null);
				throw new NotImplementedException();
			}
		}
	}
}
