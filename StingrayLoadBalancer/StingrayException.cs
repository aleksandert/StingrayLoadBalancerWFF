using System;
using System.Collections.Generic;
using System.Text;

namespace StingrayLoadBalancer
{
    class StingrayException : Exception
    {
        public StingrayException(string message)
            : base(message)
        { }

        public StingrayException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }
}
