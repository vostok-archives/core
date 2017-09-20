using System;
using System.Collections.Generic;
using System.Linq;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
	internal static class ResponseCodeUtilities
	{
	    static ResponseCodeUtilities()
	    {
            allowedValues = new HashSet<int>(Enum.GetValues(typeof(ResponseCode)).Cast<int>());
	    }

		public static ResponseCode Convert(int code, ILog log)
		{
			if (allowedValues.Contains(code))
				return (ResponseCode) code;

			// (iloktionov): Напрямую скастовать не удалось. Попытаемся привести к x00-коду семейства в соответствии с RFC.
			if (code < 100 || code >= 600)
			{
				LogUnknownResponseCode(log, code);
				return ResponseCode.Unknown;
			}
			LogConvertResponseCode(log, code);
			return Convert(code - code % 100, log);
		}

	    private static readonly HashSet<int> allowedValues;

		#region Logging
		private static void LogUnknownResponseCode(ILog log, int code)
		{
			log.Warn("Received unknown response code {0}.", code);
		}

		private static void LogConvertResponseCode(ILog log, int code)
		{
			log.Warn("Received unknown response code {0}. Will convert it to code {1}.", code, code - code % 100);
		} 
		#endregion
	}
}