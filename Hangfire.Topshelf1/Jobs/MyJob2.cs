using System;
using Hangfire.Common;
using Hangfire.Console;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using System.ComponentModel;

namespace Hangfire.Topshelf.Jobs
{
	public class MyJob2 : IRecurringJob
	{
		class SimpleObject
		{
			public string Name { get; set; }
			public int Age { get; set; }
		}

		[DisplayName("MyJob2 Test")]
		public void Execute(PerformContext context)
		{
			context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} MyJob2 Running ...");

			var intVal = context.GetJobData<int>("IntVal");

			var stringVal = context.GetJobData<string>("StringVal");

			var booleanVal = context.GetJobData<bool>("BooleanVal");

			var simpleObject = context.GetJobData<SimpleObject>("SimpleObject");

			context.WriteLine($"IntVal:{intVal},StringVal:{stringVal},BooleanVal:{booleanVal},simpleObject:{JobHelper.ToJson(simpleObject)}");
		}
	}
}
