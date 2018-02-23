using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Farm;

namespace StingrayLoadBalancer
{
	[WebFarmOperationProvider]
	public class StingrayEnableLoadBalancingOperationProvider : WebFarmOperationProvider
	{
		public override string Name
		{
			get
			{
				return Constants.ProviderNamePrefix + EnableLoadBalancingOperationProvider.ProviderName;
			}
		}

		public override IEnumerable<OperationParameter> Parameters
		{
			get
			{
				yield return new OperationParameter("Server", "Server address", typeof(ServerContext));
			}
		}

		public override ProviderDisposition Disposition
		{
			get
			{
				return ProviderDisposition.Hidden;
			}
		}

		public override object RunOperation(WebFarmOperationContext operationContext)
		{
			if (operationContext == null) throw new ArgumentNullException(nameof(operationContext));

			ServerContext serverContext = (ServerContext)operationContext.Options.Parameters[0].Value;

			var config = Helpers.GetConfigSettings(serverContext.WebFarm.Name);

			var node = Helpers.GetNodeName(serverContext.Address, config.TCPPort);

			serverContext.TraceInfo("Enabling node {0} in pool {1}.", node, config.PoolName);

			LoadBalancer.EnableNode(config, node);

			serverContext.TraceInfo("Node {0} enabled in pool {1}.", node, config.PoolName);

			return null;
		}
	}
}
