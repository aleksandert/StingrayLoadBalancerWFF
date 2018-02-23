using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StingrayLoadBalancer.Test
{
	enum ExitCode : int
	{
		Success = 0,
		NoWebFarmName = 1,
		InvalidWebFarmName = 2,
		UnknownError = 10
	}

	class Program
	{
		static int Main(string[] args)
		{
			var webFarmName = args.FirstOrDefault();

			if (string.IsNullOrEmpty(webFarmName))
			{
				Console.WriteLine("Please specify web farm name:");
				webFarmName = Console.ReadLine();
			}

			var config = StingrayLoadBalancer.Properties.Settings.Default.StingrayConfiguration
											.Where(x => x.WebFarmName == webFarmName)
											.FirstOrDefault();

			if (config == null)
			{
				
				Console.Error.WriteLine("Configuration not found for web farm name '{0}'.", webFarmName);
				return (int)ExitCode.InvalidWebFarmName;
			}

			var cmd = "list";

			while (!string.IsNullOrEmpty(cmd))
			{
				try
				{
					if (!string.IsNullOrEmpty(cmd))
					{
						var pars = cmd.Split(' ');

						switch (pars[0])
						{
							case "add":
								LoadBalancer.AddNode(config, pars[1]);
								break;
							case "remove":
								LoadBalancer.RemoveNode(config, pars[1]);
								break;
							case "enable":
								LoadBalancer.EnableNode(config, pars[1]);
								break;
							case "disable":
								LoadBalancer.DisableNode(config, pars[1]);
								break;
							case "drain":
								LoadBalancer.DrainNode(config, pars[1]);
								break;
							case "list":
								{
									Console.WriteLine("Listing nodes in pool {0}.", config.PoolName);

									var nodes = StingrayLoadBalancer.LoadBalancer.GetNodes(config).OrderBy(x => x.Name);

									Console.WriteLine("{0} nodes in pool {1}.", nodes.Count(), config.PoolName);

									foreach (var node in nodes)
									{
										Console.WriteLine(node.ToString());
									}
								}
								break;
							default:
								Console.Error.WriteLine("Unknown command.");
								break;
						}
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("Command failed! ({0})", e.Message);
				}

				Console.WriteLine("\nEnter command (exit, add <nodename>, remove <nodename>, enable <nodename>, disable <nodename>, drain <nodename>, list): ");

				cmd = Console.ReadLine();
			}

			return (int)ExitCode.Success;
		}
	}
}
