using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Farm;
using System.Net;
using System.Threading;

namespace StingrayLoadBalancer
{
    [WebFarmOperationProvider]
    public class StingrayAddServerOperationProvider : WebFarmOperationProvider
    {
        public override string Name
        {
            get
            {
                return Constants.ProviderNamePrefix + AddServerOperationProvider.ProviderName;
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
            ServerContext serverContext = (ServerContext)operationContext.Options.Parameters[0].Value;

            var config = Helpers.GetConfigSettings(serverContext.WebFarm.Name);

            var node = Helpers.GetNodeName(serverContext.Address, config.TCPPort);

            serverContext.TraceInfo("Adding node {0} to pool {1}.", node, config.PoolName);

            LoadBalancer.AddNode(config, node);

            serverContext.TraceInfo("Node {0} added to pool {1}.", node, config.PoolName);

            return null;
        }    
    }
}
