using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CodeInjection.SampleApp
{
    internal class Program
    {
        private static void Main()
        {
            var sampleService = new SampleService();
            var codeInjector = new LogicInjector();
            var pipeline = new InjectedPipeline();
            //codeInjector.ProxyFactory = new ProxyFactory();
            pipeline.Add(new LogToConsoleInjectionLogic());
            //codeInjector.ActivatorFactory = new ReflectionActivatorFactory();

            Console.WriteLine("Press \"S\" for simple test.");
            Console.WriteLine("Press \"T\" for performance test.");
            Console.WriteLine("Press \"Q\" to exit.");

            do
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.S:
                        StandartInvokation(codeInjector, sampleService, pipeline);
                        break;
                    case ConsoleKey.T:
                        StartLoadTest(codeInjector, sampleService, pipeline);
                        break;
                    case ConsoleKey.Q:
                        return;
                }
            } while (true);
        }

        private static void StartLoadTest(ILogicInjector codeInjector, ISampleService sampleService,
            IInjectedPipeline pipeline)
        {
            var oldOut = Console.Out;
            Console.SetOut(new StringWriter());
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 1000; i++) CallService(codeInjector, sampleService, pipeline, "test");
            stopWatch.Stop();
            Console.SetOut(oldOut);

            Console.WriteLine();
            Console.WriteLine("1000 calls of dynamic decorator with constructor: {0}", stopWatch.Elapsed);
            GC.Collect();

            Console.SetOut(new StringWriter());
            stopWatch.Reset();
            stopWatch.Start();
            for (var i = 0; i < 1000; i++)
            {
                var dec = new HardCodedDecorator(sampleService);
                dec.DoSomethingUseful("test");
            }

            stopWatch.Stop();
            Console.SetOut(oldOut);

            Console.WriteLine("1000 calls of classic decorator with constructor: {0}", stopWatch.Elapsed);
        }

        private static void CallService(ILogicInjector codeInjector, ISampleService sampleService,
            IInjectedPipeline pipeline, string clientName)
        {
            var decoratedService = codeInjector.CreateProxyFor(sampleService, pipeline);
            decoratedService.DoSomethingUseful(clientName);
        }

        private static void StandartInvokation(ILogicInjector codeInjector, ISampleService sampleService,
            IInjectedPipeline pipeline)
        {
            Console.WriteLine();
            Console.Write("Enter client name: ");
            var clientName = Console.ReadLine();

            CallService(codeInjector, sampleService, pipeline, clientName);
        }
    }

    public class HardCodedDecorator : ISampleService
    {
        private readonly MethodInfo _doSomethingUsefulMethodInfo;
        private readonly ISampleService _service;

        public HardCodedDecorator(ISampleService service)
        {
            _service = service;
            _doSomethingUsefulMethodInfo = _service.GetType().GetMethod("DoSomethingUseful");
        }

        public void DoSomethingUseful(string clientName)
        {
            var targetType = _service.GetType();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[Before execute] Starts For:");
            Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, _service.GetHashCode());
            Console.WriteLine("Will execute method [{0}]", _doSomethingUsefulMethodInfo.Name);
            Console.WriteLine();
            Console.ResetColor();

            _service.DoSomethingUseful(clientName);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("[Before execute] Ends For:");
            Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, _service.GetHashCode());
            Console.WriteLine("Method [{0}] execution finished", _doSomethingUsefulMethodInfo.Name);
            Console.ResetColor();
        }
    }
}