using System;
using System.Diagnostics.Contracts;

namespace CodeInjection
{
	public delegate object FactoryMethodDelegate<in T>(IInjectedPipeline pipeline, T realInstance);

	[ContractClass(typeof(ActivatorFactoryContract))]
	public interface IActivatorFactory
	{
		FactoryMethodDelegate<T> CreateActivatorOf<T>(Type exactType);
	}

	[ContractClassFor(typeof(IActivatorFactory))]
	public abstract class ActivatorFactoryContract :IActivatorFactory{
		public FactoryMethodDelegate<T> CreateActivatorOf<T>(Type exactType) {
			Contract.Requires(exactType != null);
			Contract.Requires(typeof(T).IsAssignableFrom(exactType));
			Contract.Ensures(Contract.Result<FactoryMethodDelegate<T>>() != null);
			throw new NotImplementedException();
		}
	}
}
