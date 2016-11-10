using Hangfire.Server;
using Hangfire.Topshelf.Core;
using System.ComponentModel;

namespace Hangfire.Topshelf.AppServices
{
	public interface ISampleService : IAppService
	{
		/// <summary>
		/// simple job test
		/// </summary>
		/// <param name="context"></param>
		[RecurringJob("0 4 1 * *")]
		[AutomaticRetry(Attempts = 3)]
		[DisplayName("SimpleJobTest")]
		[Queue("jobs")]
		void SimpleJob(PerformContext context);
	}
}
