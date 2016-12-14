using System;
using System.Threading;
using Hangfire.Logging;

namespace Hangfire.Topshelf.AppServices.Impl
{
	public class OrderService : BaseAppService, IOrderService
	{
		public IProductService ProductService { get; private set; }

		public OrderService(IProductService productService)
		{
			ProductService = productService;
		}
		public void CreateOrder(int productId)
		{
			Thread.Sleep(1000);

			int orderId = new Random().Next();

			Logger.Info("create order successfully.");

			BackgroundJob.Enqueue<IInventoryService>(x => x.Reduce(orderId));
		}
	}
}
