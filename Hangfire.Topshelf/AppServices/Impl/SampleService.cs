using System;
using System.Linq;
using Hangfire.Server;
using Hangfire.Console;

namespace Hangfire.Topshelf.AppServices.Impl
{
	public class SampleService : BaseAppService, ISampleService
	{
		public void SimpleJob(PerformContext context)
		{
			context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} SimpleJob Running ...");

			var progressBar = context.WriteProgressBar();

			foreach (var i in Enumerable.Range(1, 50).ToList().WithProgress(progressBar))
			{
				System.Threading.Thread.Sleep(1000);
			}
		}
	}
}
