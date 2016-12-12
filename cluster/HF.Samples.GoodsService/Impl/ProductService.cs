using Hangfire.Samples.Framework;

namespace HF.Samples.GoodsService.Impl
{
	public class ProductService : BaseAppService, IProductService
	{
		public bool Exists(string productId)
		{
			return true;
		}
	}
}
