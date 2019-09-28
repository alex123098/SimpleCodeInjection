using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeInjection
{
    public class ProxyFactory : IProxyFactory
    {
        private readonly ModuleBuilder _moduleBuilder;

        public ProxyFactory()
        {
            var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _moduleBuilder = assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
        }

        public Type CreateProxyType<T>(T realInstance, IInjectedPipeline injectedPipeline)
        {
            if (realInstance == null) throw new ArgumentNullException(nameof(realInstance));
            if (injectedPipeline == null) throw new ArgumentNullException(nameof(injectedPipeline));

            var typeAttributes = CreateProxyTypeAttributes();
            var typeName = GenerateTypeName<T>();
            var parentType = typeof(object);
            var interfaces = GetAllTypeInterfaces<T>();
            // create a type
            var typeBuilder = _moduleBuilder.DefineType(typeName, typeAttributes, parentType, interfaces);

            // define a field for a pipeline
            var pipelineFieldName = GenerateFieldName("_pipeline");
            var pipelineFieldType = typeof(IInjectedPipeline);
            var pipelineField = typeBuilder.DefineField(
                pipelineFieldName,
                pipelineFieldType,
                FieldAttributes.Private | FieldAttributes.InitOnly);

            // define field for the decorated object instance
            var realInstanceFieldName = GenerateFieldName("_realInstance");
            var realInstanceFieldType = typeof(T);
            var realInstanceField = typeBuilder.DefineField(
                realInstanceFieldName,
                realInstanceFieldType,
                FieldAttributes.Private | FieldAttributes.InitOnly);

            // create constructor method
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { pipelineFieldType, realInstanceFieldType });
            EmitConstructor(constructorBuilder.GetILGenerator(), pipelineField, realInstanceField);

            // create implementations for all interfaces
            foreach (var @interface in interfaces)
                EmitInterfaceImplementation(@interface, typeBuilder, pipelineField, realInstanceField);

            return typeBuilder.CreateTypeInfo();
        }

        #region Emit methods

        private void EmitInterfaceImplementation(
            Type @interface,
            TypeBuilder typeBuilder,
            FieldBuilder pipelineField,
            FieldBuilder realInstanceField)
        {
            if (@interface.FullName == null) 
                throw new NotSupportedException("Interface without FullName is not supported");
            
            var interfaceMethods = @interface.GetMethods();
            for (var i = 0; i < interfaceMethods.Length; i++)
            {
                var method = interfaceMethods[i];
                // get types of parameters of current method
                var parameterTypes = method.GetParameters()
                    .Select(pi => pi.ParameterType)
                    .ToArray();
                // create method builder
                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    method.ReturnType,
                    parameterTypes);
                MethodMetadataContainer.AddMethods(@interface);
                EmitMethodDefinition(@interface.FullName, i, methodBuilder.GetILGenerator(), pipelineField,
                    realInstanceField, method);
            }

            // generate the same for all base interfaces
            foreach (var parentInterface in @interface.GetInterfaces())
                EmitInterfaceImplementation(parentInterface, typeBuilder, pipelineField, realInstanceField);
        }

        private void EmitMethodDefinition(
            string interfaceName,
            int methodIndex,
            ILGenerator il,
            FieldInfo pipelineField,
            FieldInfo realInstanceField,
            MethodInfo methodInfo)
        {
            // declare array of parameters for a method
            var methodParameters = methodInfo.GetParameters();
            il.DeclareLocal(typeof(object[]));

            // declare a local variable for a method result if it returns value
            var returnType = methodInfo.ReturnType;
            if (returnType != typeof(void)) il.DeclareLocal(returnType);

            // create array of parameters
            il.Emit(OpCodes.Ldc_I4, methodParameters.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc_0);

            // store all parameters
            for (var i = 0; i < methodParameters.Length; i++)
            {
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i + 1);
                // box if it is a value type
                if (methodParameters[i].ParameterType.IsValueType)
                    il.Emit(OpCodes.Box, methodParameters[i].ParameterType);
                il.Emit(OpCodes.Stelem_Ref);
            }

            // call the PreExecute method
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, pipelineField);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, realInstanceField);
            // call MethodMetadataContainer.GetMethodInfo. Unfortunately this is the only way to load method info to stack :(
            il.Emit(OpCodes.Ldstr, interfaceName);
            il.Emit(OpCodes.Ldc_I4, methodIndex);
            il.Emit(OpCodes.Call, typeof(MethodMetadataContainer).GetMethod("GetMethodInfo"));
            // load arguments of current method
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Callvirt, typeof(IInjectedPipeline).GetMethod("ExecutePreCondition"));

            // call actual method
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, realInstanceField);
            for (var i = 1; i <= methodParameters.Length; i++) il.Emit(OpCodes.Ldarg, i);
            il.Emit(OpCodes.Callvirt, methodInfo);

            // store return value
            if (methodInfo.ReturnType != typeof(void)) il.Emit(OpCodes.Stloc_1);

            // call PostExecute method
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, pipelineField);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, realInstanceField);
            // call MethodMetadataContainer.GetMethodInfo. Unfortunately this is the only way to load method info to stack :(
            il.Emit(OpCodes.Ldstr, interfaceName);
            il.Emit(OpCodes.Ldc_I4, methodIndex);
            il.Emit(OpCodes.Call, typeof(MethodMetadataContainer).GetMethod("GetMethodInfo"));
            // load arguments of current method
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Callvirt, typeof(IInjectedPipeline).GetMethod("ExecutePostCondition"));

            // push return value
            if (methodInfo.ReturnType != typeof(void)) il.Emit(OpCodes.Ldloc_1);
            // return from the method
            il.Emit(OpCodes.Ret);
        }

        private void EmitConstructor(
            ILGenerator il,
            FieldInfo pipelineField,
            FieldInfo realInstanceField)
        {
            var parentConstructor = typeof(object).GetConstructor(Type.EmptyTypes);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, parentConstructor);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, pipelineField);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, realInstanceField);

            il.Emit(OpCodes.Ret);
        }

        #endregion

        #region Misc methods

        private string GenerateFieldName(string prefix)
        {
            return string.Concat(prefix, Guid.NewGuid().ToString("N"));
        }

        private Type[] GetAllTypeInterfaces<T>()
        {
            var type = typeof(T);
            return type.IsInterface ? new[] { type } : type.GetInterfaces();
        }

        private string GenerateTypeName<T>()
        {
            return string.Concat(typeof(T).FullName, Guid.NewGuid().ToString("N"), "Proxy");
        }

        private TypeAttributes CreateProxyTypeAttributes()
        {
            return TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;
        }

        #endregion
    }

    public static class MethodMetadataContainer
    {
        private static readonly ConcurrentDictionary<string, Type> Types = new ConcurrentDictionary<string, Type>();

        public static void AddMethods(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (type.FullName == null) throw new NotSupportedException("Type without FullName is not supported");

            Types.TryAdd(type.FullName, type);
        }
        
        public static MethodInfo? GetMethodInfo(string interfaceName, int methodIndex)
        {
            if (!Types.TryGetValue(interfaceName, out var type)) return null;
            
            var methods = type.GetMethods();
            return methods.Length > methodIndex ? methods[methodIndex] : null;
        }
    }
}