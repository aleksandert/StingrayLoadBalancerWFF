﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Farm;
using System.Net;

namespace StingrayLoadBalancer
{
	[WebFarmOperationProvider]
	public class StingrayRemoveServerOperationProvider : WebFarmOperationProvider
	{
		public override string Name
		{
			get
			{
				return Constants.ProviderNamePrefix + RemoveServerOperationProvider.ProviderName;
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

			serverContext.TraceInfo("Removing node '{0}' from pool '{1}'.", node, config.PoolName);
					   
			LoadBalancer.RemoveNode(config, node);

			serverContext.TraceInfo("Node '{0}' removed from pool '{1}'.", node, config.PoolName);

			return null;
		}
	}
}
