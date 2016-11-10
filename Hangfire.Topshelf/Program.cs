using System;
using Hangfire.Topshelf.Core;
using Topshelf;

namespace Hangfire.Topshelf
{
	class Program
	{
		static int Main(string[] args)
		{
			log4net.Config.XmlConfigurator.Configure();

			return (int)HostFactory.Run(x =>
			{
				x.RunAsLocalSystem();

				x.SetServiceName(HangfireSettings.ServiceName);
				x.SetDisplayName(HangfireSettings.ServiceDisplayName);
				x.SetDescription(HangfireSettings.ServiceDescription);

				x.UseOwin(baseAddress: HangfireSettings.ServiceAddress);

				x.SetStartTimeout(TimeSpan.FromMinutes(5));
				//https://github.com/Topshelf/Topshelf/issues/165
				x.SetStopTimeout(TimeSpan.FromMinutes(35));

				x.EnableServiceRecovery(r => { r.RestartService(1); });
			});
		}
	}
}
