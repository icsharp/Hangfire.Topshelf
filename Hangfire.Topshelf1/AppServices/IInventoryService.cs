using System.ComponentModel;

namespace Hangfire.Topshelf.AppServices
{
	public interface IInventoryService : IAppService
	{
		/// <summary>
		/// Reducing inventory when order created
		/// </summary>
		/// <param name="orderId"></param>
		[AutomaticRetry(Attempts = 3)]
		[DisplayName("Reducing inventory when order created, orderId:{0}")]
		[Queue("apis")]
		void Reduce(int orderId);
	}
}
