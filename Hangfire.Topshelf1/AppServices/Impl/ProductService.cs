
namespace Hangfire.Topshelf.AppServices.Impl
{
	public class ProductService : BaseAppService, IProductService
	{
		public bool Exists(int productId)
		{
			return true;
		}
	}
}
