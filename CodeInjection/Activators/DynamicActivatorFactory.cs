using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeInjection.Activators
{
    public class DynamicActivatorFactory : IActivatorFactory
    {
        public FactoryMethodDelegate<T> CreateActivatorOf<T>(Type exactType)
        {
            if (exactType == null) throw new ArgumentNullException(nameof(exactType));

            var ctor = exactType.GetConstructor(new[] { typeof(IInjectedPipeline), typeof(T) });
            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"Unable to create activator ot type {typeof(T)}. " +
                    "The given type doesn't have an appropriate constructor.'");
            }

            var factoryMethodBuilder = new DynamicMethod(typeof(T).Name + Guid.NewGuid().ToString("N"),
                exactType,
                new[] { typeof(IInjectedPipeline), typeof(T) });
            EmitFactoryBody(factoryMethodBuilder.GetILGenerator(), ctor);

            return (FactoryMethodDelegate<T>) factoryMethodBuilder.CreateDelegate(typeof(FactoryMethodDelegate<T>));
        }

        private void EmitFactoryBody(ILGenerator il, ConstructorInfo constructor)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);
        }
    }
}