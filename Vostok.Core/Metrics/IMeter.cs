using System.Collections.Generic;

namespace Vostok.Metrics
{
    public interface IMeter<TResult>
    {
        TResult Reset();
    }
}