using System;
using System.Threading.Tasks;

namespace Vostok.Airlock
{
    public interface IRequestSender
    {
        Task<RequestSendResult> SendAsync(ArraySegment<byte> serializedMessage);
    }
}