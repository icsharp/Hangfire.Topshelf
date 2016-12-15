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

		public InventoryService(IProductService productService)
		{
			_productService = productService;	
		}

		public void Reduce(string productId)
		{
			if (!_productService.Exists(productId))
				throw new Exception($"The product {productId} is not exists.");

			int quantity = int.Parse(RedisHelper.Instance.Database.StringGet(Constants.QuantityStringKey));

			if (quantity <= 0)
				throw new Exception("Quantity is not available.");

			Thread.Sleep(5);

			quantity--;

			RedisHelper.Instance.Database.StringSet(Constants.QuantityStringKey, quantity);

			Logger.InfoFormat("Reducing inventory successfully, quantity: {quantity}", quantity);
		}
	}
}
