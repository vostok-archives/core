namespace Vostok.Clusterclient.Model
{
    public static class ResponseCodeExtensions
    {
        /// <summary>
        /// Determines whether given code belongs to informational range (1xx).
        /// </summary>
        public static bool IsInformational(this ResponseCode code)
        {
            var numericCode = (int) code;

            return numericCode >= 100 && numericCode < 200;
        }

        /// <summary>
        /// Determines whether given code belongs to successful range (2xx).
        /// </summary>
        public static bool IsSuccessful(this ResponseCode code)
        {
            var numericCode = (int) code;

            return numericCode >= 200 && numericCode < 300;
        }

        /// <summary>
        /// Determines whether given code belongs to redirection range (3xx).
        /// </summary>
        public static bool IsRedirection(this ResponseCode code)
        {
            var numericCode = (int) code;

            return numericCode >= 300 && numericCode < 400;
        }

        /// <summary>
        /// Determines whether given code belongs to client errors range (4xx).
        /// </summary>
        public static bool IsClientError(this ResponseCode code)
        {
            var numericCode = (int) code;

            return numericCode >= 400 && numericCode < 500;
        }

        /// <summary>
        /// Determines whether given code belongs to server errors range (4xx).
        /// </summary>
        public static bool IsServerError(this ResponseCode code)
        {
            var numericCode = (int) code;

            return numericCode >= 500 && numericCode < 600;
        }

        /// <summary>
        /// Determines whether given code represents a possible network-related failure (connection loss, request timeout, data transmission error).
        /// </summary>
        public static bool IsNetworkError(this ResponseCode code)
        {
            switch (code)
            {
                case ResponseCode.RequestTimeout:
                case ResponseCode.ConnectFailure:
                case ResponseCode.ReceiveFailure:
                case ResponseCode.SendFailure:
                    return true;

                default:
                    return false;
            }
        }
    }
}
