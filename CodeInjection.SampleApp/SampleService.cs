using System;

namespace CodeInjection.SampleApp
{
	public interface ISampleService
	{
		void DoSomethingUseful(string clientName);
	}

	public class SampleService : ISampleService
	{
		public void DoSomethingUseful(string clientName) {
			Console.WriteLine("Did something useful for {0}", clientName);
		}
	}
}
