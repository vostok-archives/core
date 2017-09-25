using System;

namespace Vostok.Metrics
{
    public interface IMeterScope : IDisposable
    {
        void Register<T>(IMeter<T> meter, IRecorder<T> recorder, TimeSpan period);
    }
}