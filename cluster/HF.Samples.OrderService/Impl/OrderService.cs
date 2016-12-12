using System;
using System.Threading;
using Hangfire.Samples.Framework;
using Hangfire.Samples.Framework.Logging;
using HF.Samples.GoodsService;

namespace HF.Samples.OrderService.Impl
{
	public class OrderService : BaseAppService, IOrderService
	{
		private IProductService _productService;
		public OrderService(IProductService productService)
		{
			_productService = productService;
		}

		public void CreateOrder(string productId)
		{
			if (!_productService.Exists(productId))
				throw new Exception($"The product {productId} is not exists.");

			Thread.Sleep(1000);

			int orderId = new Random().Next();

			Logger.Info("create order successfully.");
		}
	}
}
