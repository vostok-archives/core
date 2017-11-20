using System.Collections.Generic;
using System.ComponentModel;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport.Http
{
    internal static class WinHttpErrorsHandler
    {
        // ReSharper disable InconsistentNaming
        private const int ERROR_WINHTTP_AUTODETECTION_FAILED = 12180;
        private const int ERROR_WINHTTP_CANNOT_CONNECT = 12029;
        private const int ERROR_WINHTTP_CONNECTION_ERROR = 12030;
        private const int ERROR_WINHTTP_NAME_NOT_RESOLVED = 12007;
        private const int ERROR_WINHTTP_OPERATION_CANCELLED = 12017;
        private const int ERROR_WINHTTP_SECURE_CHANNEL_ERROR = 12157;
        private const int ERROR_WINHTTP_SECURE_FAILURE = 12175;
        private const int ERROR_WINHTTP_TIMEOUT = 12002;
        // ReSharper restore InconsistentNaming

        private static readonly Dictionary<int, ResponseCode> mapping = new Dictionary<int, ResponseCode>
        {
            { ERROR_WINHTTP_AUTODETECTION_FAILED, ResponseCode.ConnectFailure },
            { ERROR_WINHTTP_CANNOT_CONNECT, ResponseCode.ConnectFailure },
            { ERROR_WINHTTP_CONNECTION_ERROR, ResponseCode.ConnectFailure },
            { ERROR_WINHTTP_NAME_NOT_RESOLVED, ResponseCode.ConnectFailure },
            { ERROR_WINHTTP_OPERATION_CANCELLED, ResponseCode.ConnectFailure },
            { ERROR_WINHTTP_SECURE_CHANNEL_ERROR, ResponseCode.ConnectFailure },
            { ERROR_WINHTTP_SECURE_FAILURE, ResponseCode.ConnectFailure },
            { ERROR_WINHTTP_TIMEOUT, ResponseCode.ConnectFailure }
        };

        public static Response Handle(Win32Exception error)
        {
            return mapping.TryGetValue(error.NativeErrorCode, out var code) 
                ? new Response(code) 
                : new Response(ResponseCode.UnknownFailure); 
        }
    }
}