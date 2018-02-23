using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StingrayLoadBalancer
{
    public class StingrayException : Exception
    {
        public StingrayException(string message)
            : base(message)
        { }

		public StingrayException(string format, params object[] args)
			: this(string.Format(CultureInfo.CurrentCulture, format, args))
		{ }

		public StingrayException(string message, Exception innerException)
            : base(message, innerException)
        { }

		public StingrayException(Exception innerException, string format, params object[] args)
			: this(string.Format(CultureInfo.CurrentCulture, format, args), innerException)
		{ }

	}
}
