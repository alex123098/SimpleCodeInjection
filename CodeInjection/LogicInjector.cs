using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeInjection
{
	public class LogicInjector : ILogicInjector
	{
		private AssemblyBuilder _assemblyBuilder;
		private ModuleBuilder _moduleBuilder;

		public T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline) {
			var moduleBuilder = GetCurrentModuleBuilder();
			var proxyBuilder = new ProxyBuilder<T>(realInstance, injectedPipeline);

			var proxyType = proxyBuilder.BuildProxyType(moduleBuilder);
			return (T) Activator.CreateInstance(proxyType, injectedPipeline, realInstance);
		}

		private ModuleBuilder GetCurrentModuleBuilder() {
			if (_moduleBuilder != null) {
				return _moduleBuilder;
			}
			if (_assemblyBuilder == null) {
				var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
				_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			}
			_moduleBuilder = _assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
			return _moduleBuilder;
		}
	}
}
