using System;

namespace Vostok.Airlock
{
    internal interface IDataSenderDaemon : IDisposable
    {
        void Start();
    }
}