using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Web.Farm;
using System.Threading;

namespace StingrayLoadBalancer
{
   

    [WebFarmOperationProvider]
    public class StingrayDisableLoadBalancingOperationProvider : WebFarmOperationProvider
    {
        public override string Name
        {
            get
            {
                return Constants.ProviderNamePrefix + DisableLoadBalancingOperationProvider.ProviderName;
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

        public override IAsyncResult BeginRunOperation(WebFarmOperationContext operationContext, AsyncCallback callback, object state)
        {
            operationContext.WebFarm.TraceMessage(new TraceMessage(System.Diagnostics.TraceLevel.Verbose, string.Format("StingrayDisableLoadBalancingOperationProvider.BeginRunOperationEx()")));

            SimpleAsyncResult result = new SimpleAsyncResult(operationContext, callback, state);
            result.BeginDisableLoadBalancing();
            return result;
        }

        public override object EndRunOperation(IAsyncResult result)
        {
            using (SimpleAsyncResult res = result as SimpleAsyncResult)
            {
                if (res == null)
                {
                    throw new ArgumentException("AsyncResult empty!", "result");
                }
                res.EndDisableLoadBalancing();

                return null;
            }
        }

        private enum OperationState
        { 
            Draining,
            Disabled
        }
       
        private class SimpleAsyncResult : BaseAsyncResult
        {
            private WebFarmOperationContext _operationContext;
            private ServerContext _serverContext;
            private Exception _lastException;

            public SimpleAsyncResult(WebFarmOperationContext operationContext, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this._operationContext = operationContext;
            }

            public void BeginDisableLoadBalancing()
            {
                try
                {
                    this._operationContext.RetryTime = TimeSpan.Zero;

                    this._serverContext = (ServerContext)_operationContext.Options.Parameters[0].Value;

                    var config = Helpers.GetConfigSettings(this._serverContext.WebFarm.Name);
                    var node = Helpers.GetNodeName(this._serverContext.Address, config.TCPPort);

                    //if node is already disabled, complete the task
                    if (LoadBalancer.GetDisabledNodes(config).Contains(node))
                    {
                        this._serverContext.TraceMessage(new TraceMessage(System.Diagnostics.TraceLevel.Warning, string.Format("Node {0} in pool {1} is already disabled.", node, config.PoolName)));
                        base.SetComplete();
                    }
                    //if is forced stop or draining is disabled, complete the task
                    else if (_operationContext.Options.Force || !(config.DrainingPeriod > 0))
                    {
                        this._serverContext.TraceMessage(new TraceMessage(System.Diagnostics.TraceLevel.Warning, string.Format("Draining skipped for node {0} in pool {1}. (Force={2}, DrainingPeriod={3})", node, config.PoolName, _operationContext.Options.Force, config.DrainingPeriod)));

                        this.DrainNodeCompleteCallback(null);
                    }
                    else if (!LoadBalancer.GetDrainingNodes(config).Contains(node))
                    {
                        //start draining
                        DrainNodeGateContext.DrainNode(this._serverContext, new WaitCallback(this.DrainNodeCompleteCallback));
                    }
                }
                catch (Exception exception)
                {
                    this._lastException = exception;
                    base.SetComplete();
                }

            }

            private void DrainNodeCompleteCallback(object unused)
            {
                try
                {
                    var config = Helpers.GetConfigSettings(this._serverContext.WebFarm.Name);
                    var node = Helpers.GetNodeName(this._serverContext.Address, config.TCPPort);

                    this._serverContext.TraceMessage(new TraceMessage(System.Diagnostics.TraceLevel.Info, string.Format("Disabling node {0} in pool {1}.", node, config.PoolName)));

                    LoadBalancer.DisableNode(config, node);

                    this._operationContext.SetCurrentState(OperationState.Disabled);

                    this._serverContext.TraceMessage(new TraceMessage(System.Diagnostics.TraceLevel.Info, string.Format("Node {0} in pool {1} disabled.", node, config.PoolName)));
                }
                catch (Exception exception)
                {
                    this._lastException = exception;
                }

                base.SetComplete();
            }

            public object EndDisableLoadBalancing()
            {
                if (this._lastException != null)
                {
                    throw this._lastException;
                }
                return null;
            }

        }
    }
}
