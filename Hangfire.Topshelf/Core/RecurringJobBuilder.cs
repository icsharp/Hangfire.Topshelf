using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hangfire.Topshelf.Core
{
	public class RecurringJobBuilder : IRecurringJobBuilder
	{
		public IRecurringJobRegistry Registry { get; private set; }
		public RecurringJobBuilder(IRecurringJobRegistry registry)
		{
			Registry = registry;
		}

		public void Build(Func<IEnumerable<Type>> typesProvider)
		{
			if (typesProvider == null) throw new ArgumentNullException(nameof(typesProvider));

			foreach (var type in typesProvider())
			{
				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
				{
					if (!method.IsDefined(typeof(RecurringJobAttribute), false)) continue;

					var attribute = method.GetCustomAttribute<RecurringJobAttribute>(false);

					if (attribute == null || !attribute.Enabled) continue;

					Registry.Register(method, attribute.Cron, attribute.TimeZone, attribute.Queue);
				}
			}
		}
	}
}
