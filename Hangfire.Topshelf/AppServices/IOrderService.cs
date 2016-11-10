using System.ComponentModel;

namespace Hangfire.Topshelf.AppServices
{
	public interface IOrderService : IAppService
	{
		/// <summary>
		/// Creating order from product.
		/// </summary>
		/// <param name="productId"></param>
		[AutomaticRetry(Attempts = 3)]
		[DisplayName("Creating order from product, productId:{0}")]
		[Queue("apis")]
		void CreateOrder(int productId);
	}
}
