using Hangfire.Samples.Framework;

namespace HF.Samples.GoodsService
{
	public interface IProductService : IAppService
	{
		bool Exists(string productId);
	}
}
