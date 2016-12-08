using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HF.Samples.OrderService
{
	public class OrderService : IOrderService
	{
		private IProductService _productService;
		public OrderService(IProductService productService)
		{
			_productService = productService;
		}

		public void CreateOrder(int productId)
		{
			Thread.Sleep(1000);

			int orderId = new Random().Next();

			//Logger.Info("create order successfully.");

			//BackgroundJob.Enqueue<IInventoryService>(x => x.Reduce(orderId));

		}
	}
}
