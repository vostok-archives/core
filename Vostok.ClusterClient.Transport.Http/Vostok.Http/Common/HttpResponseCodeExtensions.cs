namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common
{
	public static class HttpResponseCodeExtensions
	{
		public static bool IsNetworkFailureCode(this HttpResponseCode code)
		{
			switch (code)
			{
				case HttpResponseCode.ConnectFailure:
				case HttpResponseCode.ReceiveFailure:
				case HttpResponseCode.SendFailure:
				case HttpResponseCode.RequestTimeout:
				case HttpResponseCode.ProxyTimeout:
					return true;
				default:
					return false;
			}
		}

		public static bool IsInformational(this HttpResponseCode code)
		{
			return code >= HttpResponseCode.Continue && code <= HttpResponseCode.SwitchingProtocols;
		}

		public static bool IsSuccessful(this HttpResponseCode code)
		{
			return code >= HttpResponseCode.Ok && code <= HttpResponseCode.PartialContent;
		}

		public static bool IsRedirection(this HttpResponseCode code)
		{
			return code >= HttpResponseCode.MultipleChoices && code <= HttpResponseCode.TemporaryRedirect;
		}

		public static bool IsClientError(this HttpResponseCode code)
		{
			return code >= HttpResponseCode.BadRequest && code <= HttpResponseCode.UnknownFailure;
		}

		public static bool IsServerError(this HttpResponseCode code)
		{
			return code >= HttpResponseCode.InternalServerError && code <= HttpResponseCode.HttpVersionNotSupported;
		}
	}
}