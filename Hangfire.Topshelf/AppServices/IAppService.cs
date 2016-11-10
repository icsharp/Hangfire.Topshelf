using System;
using Hangfire.Logging;
using Hangfire.Topshelf.Core;

namespace Hangfire.Topshelf.AppServices
{
	public class NullLogger : ILog
	{
		public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
		{
			return true;
		}
	}

	public interface IAppService : IDependency
	{
		ILog Logger { get; set; }
	}
	public abstract class BaseAppService : IAppService
	{
		public virtual ILog Logger { get; set; } = new NullLogger();
	}
}
