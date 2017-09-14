namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
	internal static class MethodUtilities
	{
		public static bool MethodAllowsBody(HttpMethod method)
		{
			switch(method)
			{
				case HttpMethod.POST:
				case HttpMethod.PUT:
				case HttpMethod.PATCH:
				case HttpMethod.DELETE:
					return true;
				default:
					return false;
			}
		}
	
		public static HttpMethod ParseMethod(string input)
		{
			switch (input)
			{
				case "HEAD": return HttpMethod.HEAD;
				case "GET": return HttpMethod.GET;
				case "POST": return HttpMethod.POST;
				case "PUT": return HttpMethod.PUT;
				case "PATCH": return HttpMethod.PATCH;
				case "DELETE": return HttpMethod.DELETE;
				case "TRACE": return HttpMethod.TRACE;
				case "OPTIONS": return HttpMethod.OPTIONS;
				case "STOP": return HttpMethod.STOP;
				default: return HttpMethod.Unknown;
			}
		}

	    public static string GetString(HttpMethod method)
	    {
	        switch (method)
	        {
	            case HttpMethod.HEAD: return "HEAD";
	            case HttpMethod.GET: return "GET";
                case HttpMethod.POST: return "POST";
                case HttpMethod.PUT: return "PUT";
                case HttpMethod.PATCH: return "PATCH";
                case HttpMethod.DELETE: return "DELETE";
                case HttpMethod.TRACE: return "TRACE";
                case HttpMethod.OPTIONS: return "OPTIONS";
                case HttpMethod.STOP: return "STOP";
                default: return method.ToString();
	        }
	    }
	}
}