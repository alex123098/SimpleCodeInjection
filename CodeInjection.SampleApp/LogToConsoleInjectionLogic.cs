using System;
using System.Reflection;

namespace CodeInjection.SampleApp
{
    internal class LogToConsoleInjectionLogic : IInjectedLogic
    {
        public IInjectedLogic BeforeExecute(object target, MethodInfo invokedMethod, object[] parameters,
            IInjectedLogic next)
        {
            var targetType = target.GetType();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[Before execute] Starts For:");
            Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, target.GetHashCode());
            Console.WriteLine("Will execute method [{0}]", invokedMethod.Name);
            Console.WriteLine();
            Console.ResetColor();

            return next;
        }

        public IInjectedLogic AfterExecute(object target, MethodInfo invokedMethod, object[] parameters,
            IInjectedLogic previous)
        {
            var targetType = target.GetType();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("[Before execute] Ends For:");
            Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, target.GetHashCode());
            Console.WriteLine("Method [{0}] execution finished", invokedMethod.Name);
            Console.ResetColor();

            return previous;
        }
    }
}