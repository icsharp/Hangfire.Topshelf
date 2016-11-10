using Hangfire.Server;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Hangfire.Topshelf.Core
{
	public class RecurringJobRegistry : IRecurringJobRegistry
	{
		public void Register(MethodInfo method, string cron, TimeZoneInfo timeZone, string queue)
		{
			if (method == null) throw new ArgumentNullException(nameof(method));
			if (cron == null) throw new ArgumentNullException(nameof(cron));
			if (timeZone == null) throw new ArgumentNullException(nameof(timeZone));
			if (queue == null) throw new ArgumentNullException(nameof(queue));

			var parameters = method.GetParameters();

			Expression[] args = new Expression[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				var parameter = parameters[i];

				if (parameter.ParameterType == typeof(PerformContext))
				{
					args[i] = Expression.Constant(null, typeof(PerformContext));
				}
				else
				{
					args[i] = Expression.Parameter(parameter.ParameterType);
				}
			}

			var x = Expression.Parameter(method.DeclaringType, "x");

			var methodCall = method.IsStatic ? Expression.Call(method, args) : Expression.Call(x, method, args);

			var addOrUpdate = Expression.Call(
				typeof(RecurringJob),
				nameof(RecurringJob.AddOrUpdate),
				new Type[] { method.DeclaringType },
				new Expression[]
				{
							Expression.Lambda(methodCall, x),
							Expression.Constant(cron),
							Expression.Constant(timeZone),
							Expression.Constant(queue)
				});

			Expression.Lambda(addOrUpdate).Compile().DynamicInvoke();
		}
	}
}
