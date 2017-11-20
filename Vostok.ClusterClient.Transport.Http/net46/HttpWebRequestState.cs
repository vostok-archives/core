using System;
using System.IO;
using System.Net;
using System.Threading;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport.Http
{
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
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
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
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
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
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
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

        private readonly RequestTimeBudget timeBudget;
        private int cancellationState;
    }
}