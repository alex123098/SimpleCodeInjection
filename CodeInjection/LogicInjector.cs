using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeInjection
{
	public class LogicInjector : ILogicInjector
	{
		private readonly ModuleBuilder _moduleBuilder;

		private IProxyFactory _proxyFactory;

		public IProxyFactory ProxyFactory {
			[Pure]
			get {
				Contract.Ensures(Contract.Result<IProxyFactory>() != null);
				return _proxyFactory;
			}
			set {
				_proxyFactory = value;
			}
		}

		public LogicInjector() {
			Contract.Ensures(ProxyFactory != null);

			var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			_moduleBuilder = assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
			SetDefaultProxyFactory();
		}

		public T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline) {
			var proxyType = ProxyFactory.CreateProxyType(realInstance, injectedPipeline);
			return (T) Activator.CreateInstance(proxyType, injectedPipeline, realInstance);
		}

		private void SetDefaultProxyFactory() {
			_proxyFactory = new CacheableProxyFactory(
				new ProxyFactory(_moduleBuilder));
		}

		[ContractInvariantMethod]
		private void ContractInvariants() {
			Contract.Invariant(_moduleBuilder != null);
			Contract.Invariant(ProxyFactory != null);
		}
	}
}
