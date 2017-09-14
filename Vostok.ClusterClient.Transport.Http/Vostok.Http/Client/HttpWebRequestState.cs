﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
	// ReSharper disable EmptyGeneralCatchClause
	internal class HttpWebRequestState : IDisposable
	{
		public HttpWebRequestState(TimeSpan timeout)
		{
			timeBudget = RequestTimeBudget.StartNew(timeout, TimeSpan.FromMilliseconds(5));
		}

		public void Reset()
		{
			Request = null;
			Response = null;
			RequestStream = null;
			ResponseStream = null;
			BodyStream = null;
		}

		public void CancelRequest()
		{
		    Interlocked.Exchange(ref cancellationState, 1);

			CancelRequestAttempt();
		}

		public void CancelRequestAttempt()
		{
			if (Request != null)
				try
				{
					Request.Abort();
				}
				catch { }
		}

		public void Dispose()
		{
			CloseRequestStream();
			CloseResponseStream();
		}

		public void CloseRequestStream()
		{
			if (RequestStream != null)
				try
				{
					RequestStream.Close();
				}
				catch { }
				finally
				{
					RequestStream = null;
				}
		}

		public void CloseResponseStream()
		{
			if (ResponseStream != null)
				try
				{
					ResponseStream.Close();
				}
				catch { }
				finally
				{
					ResponseStream = null;
				}
		}

		public HttpWebRequest Request { get; set; }
		public HttpWebResponse Response { get; set; }

		public Stream RequestStream { get; set; }
		public Stream ResponseStream { get; set; }

		public int ConnectionAttempt { get; set; }

	    public byte[] BodyBuffer { get; set; }
	    public MemoryStream BodyStream { get; set; }

		public TimeSpan TimeRemaining => timeBudget.Remaining;
		public bool RequestCancelled => cancellationState > 0;

	    private readonly IRequestTimeBudget timeBudget;
	    private int cancellationState;
	}
	// ReSharper restore EmptyGeneralCatchClause
}