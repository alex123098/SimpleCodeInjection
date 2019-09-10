﻿using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace CodeInjection.SampleApp
{
    public class Cases
    {
        private readonly ISampleService _manuallyDecorated;
        private readonly ISampleService _injectorDecorated;

        public Cases()
        {
            var sampleService = new SampleService();
            var defaultInjector = new LogicInjector();
            var pipeline = new InjectedPipeline();
            pipeline.Add(new LogToConsoleInjectionLogic());
            _manuallyDecorated = new HardCodedDecorator(sampleService);
            _injectorDecorated = defaultInjector.CreateProxyFor<SampleService, ISampleService>(sampleService, pipeline);
        }

        [Benchmark]
        public void ClassicDecorator()
        {
            _manuallyDecorated.DoSomethingUseful();
        }

        [Benchmark]
        public void DefaultInjector()
        {
            _injectorDecorated.DoSomethingUseful();
        }
        
        [Benchmark]
        public void ClassicDecoratorError()
        {
            try
            {
                _manuallyDecorated.ThrowError();
            }
            catch 
            {
                //
            }
        }

        [Benchmark]
        public void DefaultInjectorError()
        {
            try
            {
                _injectorDecorated.ThrowError();
            }
            catch 
            {
                //
            }
        }
    }
    
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<Cases>();
        }
    }

    public class HardCodedDecorator : ISampleService
    {
        private readonly MethodInfo _doSomethingUsefulMethodInfo;
        private readonly MethodInfo _throwErrorMethodInfo;
        private readonly ISampleService _service;

        public HardCodedDecorator(ISampleService service)
        {
            _service = service;
            _doSomethingUsefulMethodInfo = _service.GetType().GetMethod(nameof(DoSomethingUseful));
            _throwErrorMethodInfo = _service.GetType().GetMethod(nameof(ThrowError));
        }

        public void DoSomethingUseful()
        {
            var targetType = _service.GetType();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[Before execute] Starts For:");
            Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, _service.GetHashCode());
            Console.WriteLine("Will execute method [{0}]", _doSomethingUsefulMethodInfo.Name);
            Console.WriteLine();
            Console.ResetColor();

            _service.DoSomethingUseful();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("[Before execute] Ends For:");
            Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, _service.GetHashCode());
            Console.WriteLine("Method [{0}] execution finished", _doSomethingUsefulMethodInfo.Name);
            Console.ResetColor();
        }

        public void ThrowError()
        {
            var targetType = _service.GetType();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[Before execute] Starts For:");
            Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, _service.GetHashCode());
            Console.WriteLine("Will execute method [{0}]", _doSomethingUsefulMethodInfo.Name);
            Console.WriteLine();
            Console.ResetColor();

            try
            {
                _service.ThrowError();
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Exception] Starts For:");
                Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, _service.GetHashCode());
                Console.WriteLine("Execution failed [{0}]", _throwErrorMethodInfo.Name);
                Console.WriteLine();
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("[Before execute] Ends For:");
            Console.WriteLine("Type [{0}], instance hash: {{{1}}}", targetType.FullName, _service.GetHashCode());
            Console.WriteLine("Method [{0}] execution finished", _doSomethingUsefulMethodInfo.Name);
            Console.ResetColor();
        }
    }
}