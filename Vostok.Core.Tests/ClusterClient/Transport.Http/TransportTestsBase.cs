using System;
using System.Threading;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Threading;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Clusterclient.Transport.Http
{
    public abstract class TransportTestsBase
    {
        protected readonly VostokHttpTransport transport;
        protected readonly ILog log;

        protected TransportTestsBase()
        {
            log = new ConsoleLog();
            transport = new VostokHttpTransport(log);

            ThreadPoolUtility.Setup(log);
        }

        protected Response Send(Request request, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return transport.SendAsync(request, timeout ?? TimeSpan.FromMinutes(1), cancellationToken).GetAwaiter().GetResult();
        }
    }
}