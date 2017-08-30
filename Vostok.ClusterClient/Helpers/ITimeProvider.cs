using System;

namespace Vostok.Clusterclient.Helpers
{
    internal interface ITimeProvider
    {
        DateTime GetCurrentTime();
    }
}
