using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Farm;
using System.Threading;

namespace StingrayLoadBalancer
{
	internal abstract class BaseAsyncResult : IAsyncResult, IDisposable
	{
		private AsyncCallback _callback;
		private bool _isComplete;
		private object _state;
		private ManualResetEvent _waitHandle = new ManualResetEvent(false);

		public object AsyncState { get { return this._state; } }
		public WaitHandle AsyncWaitHandle { get { return this._waitHandle; } }
		public bool CompletedSynchronously { get { return false; } }
		public bool IsCompleted { get { return this._isComplete; } }

		public BaseAsyncResult(AsyncCallback callback, object state)
		{
			this._callback = callback;
			this._state = state;
		}

		public void Dispose()
		{
			try
			{
				this.Dispose(true);
			}
			finally
			{
				
				GC.SuppressFinalize(this);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._waitHandle != null)
			{
				this._waitHandle.Close();
				this._waitHandle = null;
			}
		}

		protected void SetComplete()
		{
			this._isComplete = true;
			this._waitHandle.Set();
			if (this._callback != null)
			{
				this._callback(this);
			}
		}
	}
}
