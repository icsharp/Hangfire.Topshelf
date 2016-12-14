using System;
using Hangfire.Topshelf.Core;
using Serilog;
using Topshelf;

namespace Hangfire.Topshelf
{
	class Program
	{
		static int Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.LiterateConsole()
				.WriteTo.RollingFile("logs\\log-{Date}.txt")
				.CreateLogger();

			return (int)HostFactory.Run(x =>
			{
				x.RunAsLocalSystem();

				x.SetServiceName(HangfireSettings.Instance.ServiceName);
				x.SetDisplayName(HangfireSettings.Instance.ServiceDisplayName);
				x.SetDescription(HangfireSettings.Instance.ServiceDescription);

				x.UseOwin(baseAddress: HangfireSettings.Instance.ServiceAddress);

				x.SetStartTimeout(TimeSpan.FromMinutes(5));
				//https://github.com/Topshelf/Topshelf/issues/165
				x.SetStopTimeout(TimeSpan.FromMinutes(35));

				x.EnableServiceRecovery(r => { r.RestartService(1); });
			});
		}
	}
}
