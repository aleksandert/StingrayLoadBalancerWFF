using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Farm;
using System.Threading;

namespace StingrayLoadBalancer
{
    internal class DrainNodeGateContext
    {
        private const int timerPeriod = 60000;

        private static object sm_lock = new object();
        
        private WebFarmSettings _config;
        private List<DrainingNode> _drainingNodes = new List<DrainingNode>();
        private Timer _drainTimer;
        private object _lock = new object();
        
        public DrainNodeGateContext(WebFarmSettings config)
        {
            this._config = config;

            this._drainTimer = new Timer(CheckDrainingState, null, timerPeriod, timerPeriod);
        }

        public static void DrainNode(ServerContext serverContext, WaitCallback callback)
        {
            GetDrainNodeGateContext(serverContext.WebFarm).Wait(serverContext, callback);
        }

        private static DrainNodeGateContext GetDrainNodeGateContext(WebFarmContext webFarmContext)
        {
            lock (sm_lock)
            {
                DrainNodeGateContext state = webFarmContext.StateTable.GetState<DrainNodeGateContext>();

                if (state == null)
                {
                    var config = Helpers.GetConfigSettings(webFarmContext.Name);

                    state = new DrainNodeGateContext(config);
                    
                    webFarmContext.StateTable.SetState(state);
                }

                return state;
            }
        }

        public void Wait(ServerContext serverContext, WaitCallback callback)
        {
            var node = Helpers.GetNodeName(serverContext.Address, _config.TCPPort);

            //start node draining
            LoadBalancer.DrainNode(this._config, node);

            serverContext.TraceMessage(new TraceMessage(System.Diagnostics.TraceLevel.Info, string.Format("Draining started for node {0} in pool {1}. Timeout: {2}.", node, _config.PoolName, _config.DrainingPeriod)));

            var drainingNode = new DrainingNode() { Server = serverContext, Callback = callback };

            //add server to waiting list
            lock (_lock)
            {
                _drainingNodes.Add(drainingNode);
            }
        }

        public bool IsDrained(DrainingNode node)
        {
            var nodeName = Helpers.GetNodeName(node.Server.Address, _config.TCPPort);
            
            var lastUsed = LoadBalancer.GetNodeLastUsed(_config, nodeName);

            var isDrained = (lastUsed > _config.DrainingPeriod);

            if (isDrained)
            {
                node.Server.TraceMessage(new TraceMessage(System.Diagnostics.TraceLevel.Info, string.Format("Draining finished for node {0} in pool {1}.Timeout: {2} secs,  Since Last Used: {3} secs.", nodeName, _config.PoolName, _config.DrainingPeriod, lastUsed)));
            }
            else
            {
                node.Server.TraceMessage(new TraceMessage(System.Diagnostics.TraceLevel.Info, string.Format("Still draining node {0} in pool {1}. Timeout: {2} secs,  Since Last Used: {3} secs.", nodeName, _config.PoolName, _config.DrainingPeriod, lastUsed)));
            }

            return isDrained;
        }

        public void Release(DrainingNode node)
        {
            //remove server from waiting list
            lock (_lock)
            {
                _drainingNodes.Remove(node);
            }

            ThreadPool.QueueUserWorkItem(node.Callback);
        }
        
        private void CheckDrainingState(object unused)
        {
            foreach (var node in _drainingNodes.ToList())
            {
                if (IsDrained(node))
                {
                    Release(node);
                }
            }
        }

        public class DrainingNode
        {
            public ServerContext Server
            {
                get;
                set;
            }

            public WaitCallback Callback
            {
                get;
                set;
            }
        }
    }
}
