using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Hangfire.Logging;
using Hangfire.Topshelf.AppServices;
using Hangfire.Topshelf.Core;

namespace Hangfire.Topshelf.Apis
{
	/// <summary>
	/// Restful Apis to process request and add it to background job from apps/micro-services.
	/// </summary>
	public class RPCController : ApiController
	{
		private static ILog logger = LogProvider.GetLogger(typeof(RPCController));

		public IProductService ProductService { get; private set; }

		public RPCController(IProductService productService)
		{
			ProductService = productService;
		}

		/// <summary>
		/// Test apis
		/// </summary>
		/// <returns></returns>
		[Route("api/test")]
		[HttpGet]
		public HttpResponseMessage TestSimpleJob()
		{
			BackgroundJob.Enqueue<ISampleService>(x => x.SimpleJob(PerformContextToken.Null));

			return Request.CreateResponse(HttpStatusCode.OK, "SimpleJob enqueued.");
		}

		/// <summary>
		/// Create order
		/// </summary>
		/// <param name="productId">Product Id</param>
		/// <returns></returns>
		[Route("api/order/create")]
		[HttpPost]
		public HttpResponseMessage CreateOrder(int productId)
		{
			if (!(ProductService.Exists(productId)))
				throw new Exception("Product not exists.");

			BackgroundJob.Enqueue<IOrderService>(x => x.CreateOrder(productId));

			return Request.CreateResponse(HttpStatusCode.OK, "Order Creating...");
		}
	}
}
