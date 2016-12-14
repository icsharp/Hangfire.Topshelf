using System;
using System.Reflection;
using Autofac;
using CommandLine;
using Serilog;
using Hangfire;
using Hangfire.Samples.Framework;
using Hangfire.Samples.Framework.Logging;
using HF.Samples.GoodsService;
using HF.Samples.OrderService;
using HF.Samples.StorageService;

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
				.WithNotParsed(_ => _logger.Info("Arguments configuration is empty, you can view command line by type: --help"))
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

			UseAutofac();

			GlobalConfiguration.Configuration.UseRedisStorage();

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

		private static void UseAutofac()
		{
			var builder = new ContainerBuilder();

			builder.RegisterModule(new DelegateModule(() => new Assembly[]
			{
				typeof(IProductService).GetTypeInfo().Assembly,
				typeof(IOrderService).GetTypeInfo().Assembly,
				typeof(IInventoryService).GetTypeInfo().Assembly
			}));

			var container = builder.Build();

			GlobalConfiguration.Configuration.UseAutofacActivator(container);
		}
	}
}
