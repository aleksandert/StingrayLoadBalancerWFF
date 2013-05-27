using System;
using System.Collections.Generic;
using System.Text;

namespace StingrayLoadBalancer
{
    public class Node
    {
        public string Name
        {
            get;
            set;
        }

        public NodeStatus Status
        {
            get;
            set;
        }

        public int LastUsed
        {
            get;
            set;
        }

        public int Connections
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Status: {1}, LastUsed: {2}, Connections: {3}", this.Name, this.Status, this.LastUsed, this.Connections);
        }
    }

    public enum NodeStatus
    { 
        Enabled,
        Disabled,
        Draining,
        
    }
}
