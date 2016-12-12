using Hangfire.Samples.Framework.Logging;

namespace Hangfire.Samples.Framework
{
	public interface IAppService : IDependency
	{
		ILog Logger { get; set; }
	}
}
