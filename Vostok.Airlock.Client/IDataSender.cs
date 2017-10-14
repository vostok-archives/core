using System.Threading.Tasks;

namespace Vostok.Airlock
{
    internal interface IDataSender
    {
        Task<DataSendResult> SendAsync();
    }
}