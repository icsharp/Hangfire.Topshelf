using System;
using System.Web.Http;
using Hangfire.Dashboard;
using Hangfire.Console;
using Hangfire.Topshelf.Jobs;
using Owin;
using Swashbuckle.Application;

namespace Hangfire.Topshelf.Core
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			// Configure Web API for self-host. 
			HttpConfiguration config = new HttpConfiguration();

			config.MapHttpAttributeRoutes();

			config.EnableSwagger(c =>
			{
				c.SingleApiVersion("v1", "Hangfire Topshelf Apis");
				c.IncludeXmlComments($@"{typeof(Startup).Assembly.GetName().Name}.xml");
			})
			.EnableSwaggerUi();

			var container = app.UseAutofac(config);

			app.UseWebApi(config);

			var queues = new[] { "default", "apis", "jobs" };

			app.UseDatabaseStorage(HangfireSettings.HangfireDbConnectionString)
			   .UseMsmq(@".\private$\hangfire-{0}", queues)
			   .UseConsole();

			//global hangfire filters
			app.UseHangfireFilters(new AutomaticRetryAttribute { Attempts = 0 });

			app.UseHangfireServer(new BackgroundJobServerOptions
			{
				//wait all jobs performed when BackgroundJobServer shutdown.
				ShutdownTimeout = TimeSpan.FromMinutes(30),
				Queues = queues,
				WorkerCount = Math.Max(Environment.ProcessorCount, 20)
			});
			var options = new DashboardOptions
			{
				AppPath = HangfireSettings.AppWebSite,
				AuthorizationFilters = new[]
				{
					new BasicAuthAuthorizationFilter ( new BasicAuthAuthorizationFilterOptions
					{
						SslRedirect = false,
						RequireSsl = false,
						LoginCaseSensitive = true,
						Users = new[]
						{
							new BasicAuthAuthorizationUser
							{
								Login = HangfireSettings.LoginUser,
								// Password as plain text
								PasswordClear = HangfireSettings.LoginPwd
							}

						}
					} )
				}
			};
			app.UseHangfireDashboard("", options);

			app.UseDashboardMetric();

			app.UseRecurringJob(typeof(RecurringJobService));

			app.UseRecurringJob(container);

			app.UseRecurringJob("recurringjob.json");
		}

	}
}
