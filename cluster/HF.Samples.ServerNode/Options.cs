using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace HF.Samples.ServerNode
{
	public interface IOptions
	{
		[Value(0, MetaName = "identifier", Required = true, HelpText = "The hangfire server specified node identifier name.")]
		string Identifier { get; set; }

		[Option('q', "queues", Default = "default", HelpText = "The assigned queues to hangfire server node.")]
		string Queues { get; set; }

		[Option('w', "workercount", Default = 20, HelpText = "The assigned worker count to hangfire server node.")]
		int WorkerCount { get; set; }
	}

	[Verb("node", HelpText = "The hangfire server node configuration.")]
	class NodeOptions : IOptions
	{
		public string Identifier { get; set; }

		public string Queues { get; set; }

		public int WorkerCount { get; set; }

		[Usage(ApplicationAlias = "HF.Samples.ServerNode.dll")]
		public static IEnumerable<Example> Examples
		{
			get
			{
				yield return new Example("Normal Scenario", new NodeOptions { Identifier = Guid.NewGuid().ToString(), Queues = "default,jobs" });
				yield return new Example("More details as below", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new NodeOptions { Identifier = Guid.NewGuid().ToString(), Queues = "default,jobs", WorkerCount = 32 });
			}
		}
	}
}
