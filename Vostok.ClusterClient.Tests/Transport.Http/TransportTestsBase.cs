using System;
using System.Threading;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Threading;
using Vostok.Logging;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class TransportTestsBase
    {
        protected readonly VostokHttpTransport transport;
        protected readonly ILog log;

        public TransportTestsBase(ITestOutputHelper outputHelper)
        {
            log = new TestOutputLog(outputHelper);
            transport = new VostokHttpTransport(log);

            ThreadPoolUtility.Setup(log);
        }

        protected Response Send(Request request, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return transport.SendAsync(request, timeout ?? TimeSpan.FromMinutes(1), cancellationToken).GetAwaiter().GetResult();
        }
    }
}