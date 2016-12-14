using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Hangfire.Samples.Framework.Logging;

namespace Hangfire.Samples.Framework
{
	public class DelegateModule : Autofac.Module
	{
		private Func<IEnumerable<Assembly>> _assemblyProvider;
		public DelegateModule(Func<IEnumerable<Assembly>> assemblyProvider)
		{
			if (assemblyProvider == null) throw new ArgumentNullException(nameof(assemblyProvider));

			_assemblyProvider = assemblyProvider;
		}
		protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
		{
			base.AttachToComponentRegistration(componentRegistry, registration);

			// Handle constructor parameters.
			registration.Preparing += OnComponentPreparing;

			// Handle properties.
			registration.Activated += (sender, e) => InjectLoggerProperties(e.Instance);
		}

		private void InjectLoggerProperties(object instance)
		{
			var instanceType = instance.GetType();

			// Get all the injectable properties to set.
			// If you wanted to ensure the properties were only UNSET properties,
			// here's where you'd do it.
			var properties = instanceType
				.GetTypeInfo()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.PropertyType == typeof(ILog) && p.CanWrite && p.GetIndexParameters().Length == 0);

			// Set the properties located.
			foreach (var propToSet in properties)
			{
				propToSet.SetValue(instance, LogProvider.GetLogger(instanceType), null);
			}
		}

		private void OnComponentPreparing(object sender, PreparingEventArgs e)
		{
			e.Parameters = e.Parameters.Union(new[]
				 {
					new ResolvedParameter(
						(p, i) => p.ParameterType == typeof(ILog),
						(p, i) => LogProvider.GetLogger(p.Member.DeclaringType)
					),
				 });
		}
		/// <summary>
		/// Auto register
		/// </summary>
		/// <param name="builder"></param>
		protected override void Load(ContainerBuilder builder)
		{
			var assemblies = _assemblyProvider().ToArray();

			//register all implemented interfaces
			builder.RegisterAssemblyTypes(assemblies)
				.Where(t =>
						typeof(IDependency).GetTypeInfo().IsAssignableFrom(t)
						&& t != typeof(IDependency)
						&& !t.GetTypeInfo().IsInterface)
				.AsImplementedInterfaces();
		}

	}
}
