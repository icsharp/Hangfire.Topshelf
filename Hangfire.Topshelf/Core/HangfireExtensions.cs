using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Web.Http;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Integration.WebApi;
using Hangfire.Common;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using Hangfire.SqlServer;
using Owin;
using Topshelf;
using Topshelf.HostConfigurators;
using Hangfire.Samples.Framework;
using Hangfire.Samples.Framework.Logging;

namespace Hangfire.Topshelf.Core
{
	public static class HangfireExtensions
	{
		public static void Console(this ILog logger, string message, PerformContext context, bool loggerEnabled = true)
		{
			if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
			if (context == null) throw new ArgumentNullException(nameof(context));

			context.WriteLine(message);

			if (loggerEnabled) logger.Info(message);
		}

		public static HostConfigurator UseOwin(this HostConfigurator configurator, string baseAddress)
		{
			if (string.IsNullOrEmpty(baseAddress)) throw new ArgumentNullException(nameof(baseAddress));

			configurator.Service(() => new Bootstrap { Address = baseAddress });

			return configurator;
		}

		public static IGlobalConfiguration<TStorage> UseStorage<TStorage>(this IAppBuilder app, TStorage storage) where TStorage : JobStorage
		{
			if (storage == null) throw new ArgumentNullException(nameof(storage));

			return GlobalConfiguration.Configuration.UseStorage(storage);
		}

		public static IGlobalConfiguration<SqlServerStorage> UseMsmq(this IGlobalConfiguration<SqlServerStorage> configuration, string pathPattern, params string[] queues)
		{
			if (string.IsNullOrEmpty(pathPattern)) throw new ArgumentNullException(nameof(pathPattern));
			if (queues == null) throw new ArgumentNullException(nameof(queues));

			foreach (var queueName in queues)
			{
				var path = string.Format(pathPattern, queueName);

				if (!MessageQueue.Exists(path))
					using (var queue = MessageQueue.Create(path, transactional: true))
						queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
			}
			return configuration.UseMsmqQueues(pathPattern, queues);
		}

		public static IAppBuilder UseDashboardMetric(this IAppBuilder app)
		{
			GlobalConfiguration.Configuration
				.UseDashboardMetric(DashboardMetrics.ServerCount)
				.UseDashboardMetric(SqlServerStorage.ActiveConnections)
				.UseDashboardMetric(SqlServerStorage.TotalConnections)
				.UseDashboardMetric(DashboardMetrics.RecurringJobCount)
				.UseDashboardMetric(DashboardMetrics.RetriesCount)
				.UseDashboardMetric(DashboardMetrics.AwaitingCount)
				.UseDashboardMetric(DashboardMetrics.EnqueuedAndQueueCount)
				.UseDashboardMetric(DashboardMetrics.ScheduledCount)
				.UseDashboardMetric(DashboardMetrics.ProcessingCount)
				.UseDashboardMetric(DashboardMetrics.SucceededCount)
				.UseDashboardMetric(DashboardMetrics.FailedCount)
				.UseDashboardMetric(DashboardMetrics.DeletedCount);

			return app;
		}

		public static IAppBuilder UseHangfireFilters(this IAppBuilder app, params JobFilterAttribute[] filters)
		{
			if (filters == null) throw new ArgumentNullException(nameof(filters));

			foreach (var filter in filters)
				GlobalConfiguration.Configuration.UseFilter(filter);

			return app;
		}

		public static IContainer UseAutofac(this IAppBuilder app, HttpConfiguration config)
		{
			if (config == null) throw new ArgumentNullException(nameof(config));

			var builder = new ContainerBuilder();

			var assembly = typeof(Startup).Assembly;

			builder.RegisterAssemblyModules(assembly);

			builder.RegisterApiControllers(assembly);

			var container = builder.Build();

			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			GlobalConfiguration.Configuration.UseAutofacActivator(container);

			return container;
		}

		public static IGlobalConfiguration UseRecurringJob(this IGlobalConfiguration configuration, IContainer container)
		{
			if (container == null) throw new ArgumentNullException(nameof(container));

			var interfaceTypes = container.ComponentRegistry
				.RegistrationsFor(new TypedService(typeof(IDependency)))
				.Select(x => x.Activator)
				.OfType<ReflectionActivator>()
				.Select(x => x.LimitType.GetInterface($"I{x.LimitType.Name}"));

			return GlobalConfiguration.Configuration.UseRecurringJob(() => interfaceTypes);
		}

		public static IAppBuilder UseRecurringJob(this IAppBuilder app, IContainer container)
		{
			if (container == null) throw new ArgumentNullException(nameof(container));

			GlobalConfiguration.Configuration.UseRecurringJob(container);

			return app;
		}
		public static IAppBuilder UseRecurringJob(this IAppBuilder app, params Type[] types)
		{
			if (types == null) throw new ArgumentNullException(nameof(types));

			GlobalConfiguration.Configuration.UseRecurringJob(types);

			return app;
		}
		public static IAppBuilder UseRecurringJob(this IAppBuilder app, Func<IEnumerable<Type>> typesProvider)
		{
			if (typesProvider == null) throw new ArgumentNullException(nameof(typesProvider));

			GlobalConfiguration.Configuration.UseRecurringJob(typesProvider);

			return app;
		}

		public static IAppBuilder UseRecurringJob(this IAppBuilder app, string jsonFile)
		{
			if (string.IsNullOrEmpty(jsonFile)) throw new ArgumentNullException(nameof(jsonFile));

			GlobalConfiguration.Configuration.UseRecurringJob(jsonFile);

			return app;
		}
	}
}
