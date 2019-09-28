using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeInjection
{
    public class InjectedPipeline : IInjectedPipeline
    {
        private readonly LinkedList<IInjectedLogic> _registeredLogics = new LinkedList<IInjectedLogic>();

        public void Add(IInjectedLogic injectedLogic)
        {
            if (injectedLogic == null) throw new ArgumentNullException(nameof(injectedLogic));

            _registeredLogics.AddLast(injectedLogic);
        }

        public void ExecutePreCondition(
            object target, 
            MethodInfo invokedMethodInfo, 
            object[] arguments)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (invokedMethodInfo == null) throw new ArgumentNullException(nameof(invokedMethodInfo));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            var logic = _registeredLogics.First;
            while (logic != null)
            {
                var nextLogic = logic.Next;
                var logicToExecute = logic.Value.BeforeExecute(target, invokedMethodInfo, arguments, nextLogic?.Value);
                if (logicToExecute == null) break;
                nextLogic = _registeredLogics.Find(logicToExecute);
                logic = nextLogic;
            }
        }

        public void ExecutePostCondition(
            object target, 
            MethodInfo invokedMethodInfo,
            object[] arguments)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (invokedMethodInfo == null) throw new ArgumentNullException(nameof(invokedMethodInfo));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            var logic = _registeredLogics.Last;
            while (logic != null)
            {
                var prevLogic = logic.Previous;
                var logicToExecute = logic.Value.AfterExecute(target, invokedMethodInfo, arguments, prevLogic?.Value);
                if (logicToExecute == null) break;
                prevLogic = _registeredLogics.Find(logicToExecute);
                logic = prevLogic;
            }
        }
    }
}