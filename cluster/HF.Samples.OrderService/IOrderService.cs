using System.ComponentModel;

namespace HF.Samples.OrderService
{
    public interface IOrderService
	{
		[DisplayName("Creating order from product, productId:{0}")]
		void CreateOrder(int productId);
	}
}
