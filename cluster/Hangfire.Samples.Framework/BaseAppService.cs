using Hangfire.Samples.Framework.Logging;

namespace Hangfire.Samples.Framework
{
	public abstract class BaseAppService : IAppService
	{
		public virtual ILog Logger { get; set; } = new NullLogger();
	}
}
