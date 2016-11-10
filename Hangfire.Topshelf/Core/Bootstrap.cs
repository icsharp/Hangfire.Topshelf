using System;
using Microsoft.Owin.Hosting;
using Topshelf;
using Topshelf.Logging;

namespace Hangfire.Topshelf.Core
{
	/// <summary>
	/// OWIN host
	/// </summary>
	public class Bootstrap : ServiceControl
	{
		private readonly LogWriter _logger = HostLogger.Get(typeof(Bootstrap));
		private IDisposable webApp;
		public string Address { get; set; }
		public bool Start(HostControl hostControl)
		{
			try
			{
				webApp = WebApp.Start<Startup>(Address);
				return true;
			}
			catch (Exception ex)
			{
				_logger.Error($"Topshelf starting occured errors:{ex.ToString()}");
				return false;
			}

		}

		public bool Stop(HostControl hostControl)
		{
			try
			{
				webApp?.Dispose();
				return true;
			}
			catch (Exception ex)
			{
				_logger.Error($"Topshelf stopping occured errors:{ex.ToString()}");
				return false;
			}

		}
	}
}
