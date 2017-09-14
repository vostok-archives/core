using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
	/// <summary>
	/// <para>Содержит части содранного из исходников .NET 4.0 кода, связанного с <see cref="System.Web.HttpUtility"/></para>
	/// <para>Причина: начиная с 4.0 в контексте веб-приложения HttpUtility.UrlEncode может просто без причины падать из-за отсутствия HttpContext.</para>
	/// </summary>
	public static class UrlEncodingUtility
	{
		#region Url encode
		public static string UrlEncode(string str)
		{
            return UrlEncode(str, EncodingFactory.GetDefault());
		}

	    public static string UrlEncode(string str, Encoding e)
		{
            if (str == null || HttpEncoderUtility.IsUrlSafeString(str))
                return str;

            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
		}

	    public static byte[] UrlEncodeToBytes(string str, Encoding e)
		{
			if (str == null)
				return null;
			var bytes = e.GetBytes(str);
			return HttpEncoderClone.Default.UrlEncode(bytes, 0, bytes.Length, false);
		}

	    #endregion

		#region HttpEncoderClone (partially stripped from .NET 4.0)
		// ReSharper disable MemberHidesStaticFromOuterClass
		private sealed class HttpEncoderClone
		{
		    public static HttpEncoderClone Default { get; } = new HttpEncoderClone();

		    internal byte[] UrlEncode(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
			{
				var encoded = UrlEncode(bytes, offset, count);

				return (alwaysCreateNewReturnValue && (encoded != null) && (encoded == bytes))
					? (byte[])encoded.Clone()
					: encoded;
			}

			private byte[] UrlEncode(byte[] bytes, int offset, int count)
			{
				if (!ValidateUrlEncodingParameters(bytes, offset, count))
				{
					return null;
				}

				var cSpaces = 0;
				var cUnsafe = 0;

				// count them first 
				for (var i = 0; i < count; i++)
				{
					var ch = (char)bytes[offset + i];

					if (ch == ' ')
						cSpaces++;
					else if (!HttpEncoderUtility.IsUrlSafeChar(ch))
						cUnsafe++;
				}

				// nothing to expand? 
				if (cSpaces == 0 && cUnsafe == 0)
					return bytes;

				// expand not 'safe' characters into %XX, spaces to +s
				var expandedBytes = new byte[count + cUnsafe * 2];
				var pos = 0;

				for (var i = 0; i < count; i++)
				{
					var b = bytes[offset + i];
					var ch = (char)b;

					if (HttpEncoderUtility.IsUrlSafeChar(ch))
					{
						expandedBytes[pos++] = b;
					}
					else if (ch == ' ')
					{
						expandedBytes[pos++] = (byte)'+';
					}
					else
					{
						expandedBytes[pos++] = (byte)'%';
						expandedBytes[pos++] = (byte)HttpEncoderUtility.IntToHex((b >> 4) & 0xf);
						expandedBytes[pos++] = (byte)HttpEncoderUtility.IntToHex(b & 0x0f);
					}
				}

				return expandedBytes;
			}

			//  Helper to encode the non-ASCII url characters only

		    private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
			{
				if (bytes == null && count == 0)
					return false;
				if (bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				if (offset < 0 || offset > bytes.Length)
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				if (count < 0 || offset + count > bytes.Length)
				{
					throw new ArgumentOutOfRangeException("count");
				}

				return true;
			}
		}
		// ReSharper restore MemberHidesStaticFromOuterClass
		#endregion

		#region HttpEncoderUtility (stripped from .NET 4.0)
		private static class HttpEncoderUtility
		{
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static char IntToHex(int n)
			{
				if (n <= 9)
					return (char)(n + '0');
				return (char)(n - 10 + 'a');
			}

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsUrlSafeString(string str)
		    {
		        foreach (var c in str)
		        {
		            if (!IsUrlSafeChar(c))
		                return false;
		        }

		        return true;
		    }

			// Set of safe chars, from RFC 1738.4 minus '+' 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool IsUrlSafeChar(char ch)
			{
				if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9')
					return true;

				switch (ch)
				{
					case '-':
					case '_':
					case '.':
					case '!':
					case '*':
					case '(':
					case ')':
						return true;
				}

				return false;
			}
		}
		#endregion
	}
}