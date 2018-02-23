using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StingrayLoadBalancer.StingrayAPI 
{
    public partial class Pool
    {
        public string[] getEnabledNodes(string poolName)
        {
            return this.getNodes(new string[] { poolName }).FirstOrDefault();
        }

        public string[] getDisabledNodes(string poolName)
        {
            return this.getDisabledNodes(new string[] { poolName }).FirstOrDefault();
        }

        public string[] getDrainingNodes(string poolName)
        {
            return this.getDrainingNodes(new string[] { poolName }).FirstOrDefault();
        }

        
    }
}
