using System.ComponentModel;
using Hangfire;
using Hangfire.Samples.Framework;

namespace HF.Samples.StorageService
{
	public interface IInventoryService : IAppService
	{
		/// <summary>
		/// Reducing inventory when order created
		/// </summary>
		/// <param name="productId"></param>	
		[DisplayName("Reducing inventory when order created, orderId:{0}")]
		[Queue("storage")]
		[DisableConcurrentExecution(300)]
		[AutomaticRetry(Attempts = 0)]
		void Reduce(string productId);
	}
}
