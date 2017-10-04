using System;
using System.Threading.Tasks;

namespace Vostok.Airlock
{
    internal interface IDataSenderDaemon : IDisposable
    {
        void Start();

        Task FlushAsync();
    }
}