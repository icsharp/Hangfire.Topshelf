using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Samples.Framework;
using HF.Samples.GoodsService;
using HF.Samples.OrderService;
using HF.Samples.StorageService;

namespace HF.Samples.Console
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			if (env.IsDevelopment())
			{
				// This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
				builder.AddApplicationInsightsSettings(developerMode: true);
			}
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddApplicationInsightsTelemetry(Configuration);

			services.AddMvc();

			services.AddHangfire(x =>
			{
				var connectionString = Configuration.GetConnectionString("Hangfire");
				x.UseSqlServerStorage(connectionString);
			});

			return RegisterAutofac(services);
		}
		private IServiceProvider RegisterAutofac(IServiceCollection services)
		{
			var builder = new ContainerBuilder();

			builder.Populate(services);

			builder.RegisterModule(new DelegateModule(() => new Assembly[]
			{
				typeof(IProductService).GetTypeInfo().Assembly,
				typeof(IOrderService).GetTypeInfo().Assembly,
				typeof(IInventoryService).GetTypeInfo().Assembly
			}));

			var container = builder.Build();

			GlobalConfiguration.Configuration.UseAutofacActivator(container);

			return new AutofacServiceProvider(container);
		}
		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseApplicationInsightsRequestTelemetry();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseApplicationInsightsExceptionTelemetry();

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseHangfireDashboard();
		}
	}
}
