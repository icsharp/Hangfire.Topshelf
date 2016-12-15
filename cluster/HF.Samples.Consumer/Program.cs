using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using Hangfire.Samples.Framework.Logging;
using Hangfire.Samples.Framework;

namespace HF.Samples.Consumer
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

			InitializeQty(10);

			var uri = @"http://localhost:27541/api/order/create";

			Publish(uri, 10)
				.Result.ForEach(x => _logger.InfoFormat("Returns :{@x}", x));

			Console.ReadKey();
		}
		public static async Task<List<string>> Publish(string requestUri, int requestCount)
		{
			_logger.Info("Pls enter request count:");

			int.TryParse(Console.ReadLine(), out requestCount);

			_logger.InfoFormat("Begin requesting[requestUri: {0},requestCount: {1}]...", requestUri, requestCount);

			var ret = new List<string>();

			using (var client = new HttpClient())
			{
				for (int i = 0; i < requestCount; i++)
				{
					var res = await client.PostAsJsonAsync(requestUri, i);

					ret.Add(await res.Content.ReadAsStringAsync());
				}
			}
			return ret;
		}

		public static void InitializeQty(int defaultQty)
		{
			_logger.Info("Pls enter initial qty :");

			int.TryParse(Console.ReadLine(), out defaultQty);

			RedisHelper.Instance.Database.StringSet(Constants.QuantityStringKey, defaultQty);
		}
	}
}
