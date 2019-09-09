using System;
using System.Linq;

namespace CodeInjection.SampleApp
{
    public interface ISampleService
    {
        void DoSomethingUseful();
    }

    public class SampleService : ISampleService
    {
        private readonly Random _random = new Random();
        private readonly int[] _values;

        public SampleService()
        {
            _values = Enumerable.Repeat(0, 10_000_000).Select(_ => _random.Next()).ToArray();
        }
        
        public void DoSomethingUseful()
        {
            Array.Sort(_values);
        }
    }
}