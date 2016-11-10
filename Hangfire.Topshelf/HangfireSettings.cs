using System.Configuration;

namespace Hangfire.Topshelf
{
	public class HangfireSettings
	{
		/// <summary>
		/// Windows ServiceName
		/// </summary>
		public static string ServiceName => ConfigurationManager.AppSettings["hangfire.server.serviceName"];
		/// <summary>
		/// Windows ServiceDisplayName
		/// </summary>
		public static string ServiceDisplayName => ConfigurationManager.AppSettings["hangfire.server.serviceDisplayName"];
		/// <summary>
		/// Windows ServiceDescription
		/// </summary>
		public static string ServiceDescription => ConfigurationManager.AppSettings["hangfire.server.serviceDescription"];
		/// <summary>
		/// Windows ServiceAddress
		/// </summary>
		public static string ServiceAddress => ConfigurationManager.AppSettings["hangfire.server.serviceAddress"];

		/// <summary>
		/// App WebSite
		/// </summary>
		public static string AppWebSite => ConfigurationManager.AppSettings["hangfire.server.website"];

		/// <summary>
		/// hangfire login user
		/// </summary>
		public static string LoginUser => ConfigurationManager.AppSettings["hangfire.login.user"];

		/// <summary>
		/// hangfire login pwd
		/// </summary>
		public static string LoginPwd => ConfigurationManager.AppSettings["hangfire.login.pwd"];

		/// <summary>
		/// Hangfire Db ConnectionString
		/// </summary>
		public static string HangfireDbConnectionString => ConfigurationManager.ConnectionStrings["hangfiredb"].ConnectionString;

	}
}
