using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;
using System.Globalization;

namespace StingrayLoadBalancer
{
    public static class LoadBalancer
    {
        static LoadBalancer()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static T CreateProxy<T>(WebFarmSettings config) where T : System.Web.Services.Protocols.SoapHttpClientProtocol, new()
        {
            var proxy = new T();

            proxy.Url = config.ControlApiUrl;
            proxy.Credentials = new NetworkCredential(config.ControlApiUsername, config.ControlApiPassword);

            return proxy;
        }

        private static void EnsurePool(WebFarmSettings config)
        {
            var pools = GetPoolNames(config);

            bool poolExists = false;

            foreach (var pool in pools)
            {
                if (pool.Equals(config.PoolName, StringComparison.Ordinal))
                {
                    poolExists = true;
                    break;
                }
            }

            if (!poolExists)
            {
                throw new StingrayException(string.Format("Pool {0} doesn't exists.", config.PoolName));
            }
        }

        private static void EnsureNode(WebFarmSettings config, string nodeAddress)
        {
            bool nodeExists = false;

            foreach (string node in GetAllNodes(config))
            {
                if (node.Equals(nodeAddress, StringComparison.Ordinal))
                {
                    nodeExists = true;
                    break;
                }
            }

            if (!nodeExists)
            {
                throw new StingrayException(string.Format("Pool '{0}' doesn't contain node '{1}'.", config.PoolName, nodeAddress));
            }

        }

        public static void AddNode(WebFarmSettings config, string nodeAddress)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                string poolName = config.PoolName;

                try
                {
                    EnsurePool(config);

                    foreach (string node in GetAllNodes(config))
                    {
                        if (node.Equals(nodeAddress, StringComparison.Ordinal))
                        {
                            Trace.TraceWarning("Pool '{0}' already contains node '{1}'. No action needed.", poolName, nodeAddress);
                            return;
                        }
                    }

                    proxy.addNodes(new string[] { poolName },
                                    new string[][] { new string[] { nodeAddress } });

                    DisableNode(config, nodeAddress);
                }
                catch (Exception e)
                {
                    throw new StingrayException(string.Format(CultureInfo.CurrentCulture, "Error adding server {0} to pool {1}.", nodeAddress, poolName), e);
                }
            }
        }

        public static void RemoveNode(WebFarmSettings config, string nodeAddress)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

            using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                string poolName = config.PoolName;

                try
                {
                    EnsurePool(config);

                    bool nodeExists = false;

                    foreach (string node in GetAllNodes(config))
                    {
                        if (node.Equals(nodeAddress, StringComparison.Ordinal))
                        {
                            nodeExists = true;
                            break;
                        }
                    }

                    if (!nodeExists)
                    {
                        Trace.TraceWarning("Pool '{0}' doesn't contain node '{1}'.", poolName, nodeAddress);
                        return;
                    }

                    proxy.removeNodes(new string[] { poolName },
                                    new string[][] { new string[] { nodeAddress } });

                }
                catch (Exception e)
                {
                    throw new StingrayException(string.Format("Error removing node {0} from pool {1}.", nodeAddress, poolName), e);
                }
            }
        }

        public static void EnableNode(WebFarmSettings config, string nodeAddress)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                string poolName = config.PoolName;

                try
                {
                    EnsurePool(config);

                    EnsureNode(config, nodeAddress);

                    if (proxy.getEnabledNodes(poolName).Contains(nodeAddress))
                    {
                        Trace.TraceWarning("Node {0} is already enabled in pool {1}.", nodeAddress, poolName);
                        return;
                    }
                    
                    proxy.enableNodes(new string[] { poolName },
                                new string[][] { new string[] { nodeAddress } });
                }
                catch (Exception e)
                {
                    throw new StingrayException(string.Format("Error enabling node {0} in pool {1}.", nodeAddress, poolName), e);
                }


            }
        }

        public static void DisableNode(WebFarmSettings config, string nodeAddress)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                string poolName = config.PoolName;

                try
                {
                    EnsurePool(config);

                    EnsureNode(config, nodeAddress);

                    if (proxy.getDisabledNodes(config.PoolName).Contains(nodeAddress))
                    {
                        Trace.TraceWarning("Node {0} is already disabled in pool {1}.", nodeAddress, poolName);
                        return;
                    }

                    proxy.disableNodes(new string[] { poolName },
                            new string[][] { new string[] { nodeAddress } });
                }
                catch (Exception e)
                {
                    throw new StingrayException(string.Format("Error disabling node {0} in pool {1}.", nodeAddress, poolName), e);
                }
            }
        }

        public static void DrainNode(WebFarmSettings config, string nodeAddress)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                string poolName = config.PoolName;

                try
                {

                    EnsurePool(config);

                    EnsureNode(config, nodeAddress);

                    if (!proxy.getEnabledNodes(config.PoolName).Contains(nodeAddress))
                    {
                        Trace.TraceWarning("Node {0} is disabled or already draining in pool {1}.", nodeAddress, poolName);
                        return;
                    }

                    proxy.setDrainingNodes(new string[] { poolName },
                           new string[][] { new string[] { nodeAddress } });
                }
                catch (Exception e)
                {
                    throw new StingrayException(string.Format("Error draining node {0} in pool {1}.", nodeAddress, poolName), e);
                }
            }
        }

        public static int GetNodeLastUsed(WebFarmSettings config, string nodeAddress)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                try
                {
                    var sinceLastUsed = proxy.getNodesLastUsed(new string[] { nodeAddress });

                    if (sinceLastUsed.Length == 0)
                    {
                        throw new StingrayException(string.Format("Node {0} doesn't exists.", nodeAddress));
                    }

                    return sinceLastUsed[0];
                }
                catch (Exception e)
                {
                    throw new StingrayException(string.Format("Error getting NodeLastUsed for node {0}.", nodeAddress), e);
                }
            }
        }

        public static int GetConnections(WebFarmSettings config, string nodeAddress)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                try
                {
                    var connections = proxy.getNodesConnectionCounts(new string[] { nodeAddress });

                    if (connections.Length == 0)
                    {
                        throw new StingrayException(string.Format("Node {0} doesn't exists.", nodeAddress));
                    }

                    return connections[0];
                }
                catch (Exception e)
                {
                    throw new StingrayException(string.Format("Error getting NodeConnectionCount for node {0}.", nodeAddress), e);
                    throw;
                }
            }
        }

        public static string[] GetEnabledNodes(WebFarmSettings config)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                return proxy.getEnabledNodes(config.PoolName);
            }
        }

        public static string[] GetDisabledNodes(WebFarmSettings config)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                return proxy.getDisabledNodes(config.PoolName);
            }
        }

        public static string[] GetDrainingNodes(WebFarmSettings config)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                return proxy.getDrainingNodes(config.PoolName);
            } 
        }

        public static IEnumerable<Node> GetNodes(WebFarmSettings config)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                string poolName = config.PoolName;

                var pool = new string[] { poolName };

                var nodes = proxy.getNodes(pool).FirstOrDefault().Select(x => new Node() { Name = x, Status = NodeStatus.Enabled })
                            .Union(proxy.getDisabledNodes(pool).FirstOrDefault().Select(x => new Node { Name = x, Status = NodeStatus.Disabled }))
                            .Union(proxy.getDrainingNodes(pool).FirstOrDefault().Select(x => new Node { Name = x, Status = NodeStatus.Draining }))
                            .ToArray();

                var arrNodes = nodes.Select(x => x.Name).ToArray();

                var lastUsed = proxy.getNodesLastUsed(arrNodes);
                var connections = proxy.getNodesConnectionCounts(arrNodes);

                for (int i = 0; i < arrNodes.Length; i++)
                {
                    var node = nodes.Where(x => x.Name == arrNodes[i]).First();

                    node.LastUsed = lastUsed[i];
                    node.Connections = connections[i];
                }

                return nodes;
            }
        }

        public static string[] GetPoolNames(WebFarmSettings config)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                return proxy.getPoolNames();
            }
        }

        public static string[] GetAllNodes(WebFarmSettings config)
        {
			if (config == null) throw new ArgumentNullException(nameof(config));

			using (var proxy = CreateProxy<StingrayAPI.Pool>(config))
            {
                var pool = new string[] { config.PoolName };

                var enabled = proxy.getNodes(pool);
                var disabled = proxy.getDisabledNodes(pool);
                var draining = proxy.getDrainingNodes(pool);
                
                var length = enabled[0].Length + disabled[0].Length + draining[0].Length;
                var dstIndex = 0;

                string[] nodes = new string[length];

                Array.Copy(enabled[0], 0, nodes, dstIndex, enabled[0].Length);
                dstIndex += enabled[0].Length;
                Array.Copy(disabled[0], 0, nodes, dstIndex, disabled[0].Length);
                dstIndex += disabled[0].Length;
                Array.Copy(draining[0], 0, nodes, dstIndex, draining[0].Length);
                
                return nodes;
            }
        }
    }
}



