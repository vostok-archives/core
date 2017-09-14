using System;
using System.Net;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
	internal static class ExceptionUtility
	{
		public static Exception UnwrapWebException(WebException exception)
		{
			if (exception.InnerException != null)
				return exception.InnerException;
			return exception;
		}
	}
}