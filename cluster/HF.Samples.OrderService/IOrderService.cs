using System.ComponentModel;
using Hangfire;
using Hangfire.Samples.Framework;

namespace HF.Samples.OrderService
{
	public interface IOrderService : IAppService
	{
		/// <summary>
		/// Creating order from product.
		/// </summary>
		/// <param name="productId"></param>
		[DisplayName("Creating order from product, productId:{0}")]
		[Queue("order")]
		void CreateOrder(string productId);
	}
}
