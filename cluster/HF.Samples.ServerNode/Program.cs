using System;
using CommandLine;
using Hangfire;
using Serilog;
using Hangfire.Samples.Framework.Logging;

namespace HF.Samples.ServerNode
{
	public class Program
	{
		private static ILog _logger = LogProvider.For<Program>();

		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.LiterateConsole()
				.WriteTo.RollingFile("logs\\log-{Date}.txt")
				.CreateLogger();

			Parser.Default.ParseArguments<NodeOptions>(args)
				.WithNotParsed(_ => Console.WriteLine("Arguments configuration is error, you can view command line by type: --help"))
				.WithParsed(opts =>
				{
					_logger.InfoFormat("Accepted args: {@opts}", opts);

					//start hangfire server here
					UseHangfireServer(opts);
				});

			_logger.Info("Press Enter to exit...");
			Console.ReadLine();
		}

		private static void UseHangfireServer(NodeOptions opts)
		{
			var options = new BackgroundJobServerOptions
			{
				ServerName = opts.Identifier,
				WorkerCount = opts.WorkerCount,
				Queues = opts.Queues.Split(',')
			};

			GlobalConfiguration.Configuration.UseSqlServerStorage(@"Server=.\sqlexpress;Database=Hangfire;Trusted_Connection=True;");

			using (new BackgroundJobServer(options))
			{
				while (true)
				{
					var command = Console.ReadLine();
					if (command == null || command.Equals("stop", StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
				}
			}

		}
	}
}
