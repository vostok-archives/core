using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Vostok.Helpers
{
    internal class TestServer : IDisposable
    {
        private readonly HttpListener listener;
        private readonly int port;
        private ReceivedRequest lastRequest;

        public TestServer()
        {
            port = FreeTcpPortFinder.GetFreePort();
            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{port}/");
        }

        public ReceivedRequest LastRequest => lastRequest;

        public Uri Url => new Uri($"http://localhost:{port}/");

        public static TestServer StartNew(Action<HttpListenerContext> handle)
        {
            var server = new TestServer();

            server.Start(handle);

            return server;
        }

        public void Start(Action<HttpListenerContext> handle)
        {
            listener.Start();

            Task.Run(
                async () =>
                {
                    while (true)
                    {
                        var context = await listener.GetContextAsync();

#pragma warning disable 4014
                        Task.Run(
#pragma warning restore 4014
                            () =>
                            {
                                Interlocked.Exchange(ref lastRequest, DescribeReceivedRequest(context.Request));

                                handle(context);

                                context.Response.Close();
                            });
                    }
                    // ReSharper disable once FunctionNeverReturns
                });
        }

        public void Dispose()
        {
            listener.Stop();
            listener.Close();
        }

        private static ReceivedRequest DescribeReceivedRequest(HttpListenerRequest request)
        {
            var bodyStream = new MemoryStream(Math.Max(4, (int) request.ContentLength64));

            request.InputStream.CopyTo(bodyStream);

            return new ReceivedRequest
            {
                Url = request.Url,
                Method = request.HttpMethod,
                Headers = request.Headers,
                Query = request.QueryString,
                Body = bodyStream.ToArray()
            };
        }
    }
}
