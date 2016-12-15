using System;
using StackExchange.Redis;

namespace Hangfire.Samples.Framework
{
	public class RedisHelper
	{
		private static Lazy<RedisHelper> _instance = new Lazy<RedisHelper>(() => new RedisHelper());
		public static RedisHelper Instance => _instance.Value;

		private RedisHelper()
		{
			Connection = ConnectionMultiplexer.Connect("localhost");
			Database = Connection.GetDatabase();
		}
		public ConnectionMultiplexer Connection { get; private set; }
		public IDatabase Database { get; private set; }
	}
}
