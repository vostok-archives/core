using System;

namespace Vostok.Clusterclient.Transport.Http
{
    public class VostokHttpTransportSettings
    {
        public int ConnectionAttempts { get; set; } = 1;

        public TimeSpan? ConnectionTimeout = TimeSpan.FromMilliseconds(500);
    }
}