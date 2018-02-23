using Microsoft.Web.Farm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StingrayLoadBalancer
{
	internal static class Extensions
	{
		public static void Trace(this ServerContext context, System.Diagnostics.TraceLevel level, string format, params object[] args)
		{
			context.TraceMessage(new TraceMessage(level, string.Format(format, args)));
		}

		public static void TraceVerbose(this ServerContext context, string format, params object[] args)
		{
			context.Trace(TraceLevel.Verbose, format, args);
		}

		public static void TraceInfo(this ServerContext context, string format, params object[] args)
		{
			context.Trace(TraceLevel.Info, format, args);
		}

		public static void TraceWarning(this ServerContext context, string format, params object[] args)
		{
			context.Trace(TraceLevel.Warning, format, args);
		}

		public static void TraceError(this ServerContext context, string format, params object[] args)
		{
			context.Trace(TraceLevel.Error, format, args);
		}

		public static void Trace(this WebFarmContext context, System.Diagnostics.TraceLevel level, string format, params object[] args)
		{
			context.TraceMessage(new TraceMessage(level, string.Format(format, args)));
		}

		public static void TraceVerbose(this WebFarmContext context, string format, params object[] args)
		{
			context.Trace(TraceLevel.Verbose, format, args);
		}

		public static void TraceInfo(this WebFarmContext context, string format, params object[] args)
		{
			context.Trace(TraceLevel.Info, format, args);
		}

		public static void TraceWarning(this WebFarmContext context, string format, params object[] args)
		{
			context.Trace(TraceLevel.Warning, format, args);
		}

		public static void TraceError(this WebFarmContext context, string format, params object[] args)
		{
			context.Trace(TraceLevel.Error, format, args);
		}

		

	}
}
