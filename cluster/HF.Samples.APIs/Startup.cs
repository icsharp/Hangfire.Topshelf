using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.Swagger.Model;
using Hangfire;

namespace HF.Samples.APIs
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsEnvironment("Development"))
			{
				// This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
				builder.AddApplicationInsightsSettings(developerMode: true);
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddApplicationInsightsTelemetry(Configuration);

			services.AddMvc();

			var connectionString = Configuration.GetConnectionString("Hangfire");
			GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

			//services.AddHangfire(x =>
			//{
			//	var connectionString = Configuration.GetConnectionString("Hangfire");
			//	x.UseSqlServerStorage(connectionString);
			//});

			services.AddSwaggerGen();

			var xmlPath = GetXmlCommentsPath();

			services.ConfigureSwaggerGen(options =>
			{
				options.SingleApiVersion(new Info
				{
					Version = "v1",
					Title = "Hangfire Samples APIs",
					Description = "The unified entry for hangfire invocation to add background job to queues.",
					TermsOfService = "None",
					Contact = new Contact { Name = "icsharp", Url = "https://github.com/icsharp/Hangfire.Topshelf" }
				});

				options.IncludeXmlComments(xmlPath);

				//options.DescribeAllEnumsAsStrings();
			});
		}
		private string GetXmlCommentsPath()
		{
			var app = PlatformServices.Default.Application;
			return Path.Combine(app.ApplicationBasePath, "HF.Samples.APIs.xml");
		}
		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseApplicationInsightsRequestTelemetry();

			app.UseApplicationInsightsExceptionTelemetry();

			app.UseMvc();

			app.UseSwagger();

			app.UseSwaggerUi();
		}
	}
}
