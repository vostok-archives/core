namespace Vostok.Clusterclient.Transport.Http
{
    internal enum HttpActionStatus
    {
        Success,
        ConnectionFailure,
        SendFailure,
        ReceiveFailure,
        Timeout,
        RequestCanceled,
        ProtocolError,
        UnknownFailure
    }
}