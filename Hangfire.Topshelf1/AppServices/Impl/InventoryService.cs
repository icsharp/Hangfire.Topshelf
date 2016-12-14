using System.Threading;
using Hangfire.Logging;

namespace Hangfire.Topshelf.AppServices.Impl
{
	public class InventoryService : BaseAppService, IInventoryService
	{
		public IProductService ProductService { get; private set; }

		public InventoryService(IProductService productService)
		{
			ProductService = productService;
		}

		public void Reduce(int orderId)
		{
			Thread.Sleep(1000);

			Logger.Info("reduce inventory successfully.");
		}
	}
}
