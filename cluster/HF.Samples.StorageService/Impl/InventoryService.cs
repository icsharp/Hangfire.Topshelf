using System;
using System.Threading;
using Hangfire.Samples.Framework;
using Hangfire.Samples.Framework.Logging;
using HF.Samples.GoodsService;

namespace HF.Samples.StorageService.Impl
{
	public class InventoryService : BaseAppService, IInventoryService
	{
		private IProductService _productService;

		private static int quantity = 100;

		public InventoryService(IProductService productService)
		{
			_productService = productService;
		}
		public void Reduce(string productId)
		{
			if (!_productService.Exists(productId))
				throw new Exception($"The product {productId} is not exists.");

			if (quantity <= 0)
				throw new Exception("Quantity is not available.");

			Thread.Sleep(2000);

			quantity--;

			Logger.InfoFormat("Reducing inventory successfully, quantity: {quantity}", quantity);
		}
	}
}
