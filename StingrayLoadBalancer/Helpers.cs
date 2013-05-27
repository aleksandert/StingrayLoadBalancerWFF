using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StingrayLoadBalancer
{
    class Helpers
    {
        public static WebFarmSettings GetConfigSettings(string webFarm)
        {
            return Properties.Settings.Default.StingrayConfiguration
                .Where(x => x.WebFarmName.Equals(webFarm, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }

        public static string GetNodeName(string server, int port)
        {
            return string.Format("{0}:{1}", server, port);
        }
    }
}
