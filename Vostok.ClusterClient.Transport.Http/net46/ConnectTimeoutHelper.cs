using System;
using System.Linq.Expressions;
using System.Net;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    internal static class ConnectTimeoutHelper
    {
        public static bool CanCheckSocket => canCheckSocket;

        public static bool IsSocketConnected(HttpWebRequest request, ILog log)
        {
            Initialize(log);

            if (!canCheckSocket)
                return true;

            try
            {
                return isSocketConnected(request);
            }
            catch (Exception error)
            {
                canCheckSocket = false;

                log.Error("Failed to check socket connection", error);
            }

            return true;
        }

        private static void Initialize(ILog log)
        {
            if (isSocketConnected != null || !canCheckSocket)
                return;

            Exception savedError = null;

            lock (sync)
            {
                if (isSocketConnected != null || !canCheckSocket)
                    return;

                try
                {
                    isSocketConnected = BuildSocketConnectedChecker();
                }
                catch (Exception error)
                {
                    canCheckSocket = false;
                    savedError = error;
                }
            }

            if (savedError != null)
                log.Error("Failed to build connection checker lambda", savedError);
        }

        /// <summary>
        /// Builds the following lambda:
        /// (HttpWebRequest request) => request._SubmitWriteStream != null && request._SubmitWriteStream.InternalSocket != null && request._SubmitWriteStream.InternalSocket.Connected
        /// </summary>
        private static Func<HttpWebRequest, bool> BuildSocketConnectedChecker()
        {
            var request = Expression.Parameter(typeof(HttpWebRequest));

            var stream = Expression.Field(request, "_SubmitWriteStream");
            var socket = Expression.Property(stream, "InternalSocket");
            var isConnected = Expression.Property(socket, "Connected");

            var body = Expression.AndAlso(
                    Expression.ReferenceNotEqual(stream, Expression.Constant(null)),
                    Expression.AndAlso(
                        Expression.ReferenceNotEqual(socket, Expression.Constant(null)),
                        isConnected));

            return Expression.Lambda<Func<HttpWebRequest, bool>>(body, request).Compile();
        }

        private static volatile bool canCheckSocket = true;

        private static readonly object sync = new object();

        private static Func<HttpWebRequest, bool> isSocketConnected;
    }
}
