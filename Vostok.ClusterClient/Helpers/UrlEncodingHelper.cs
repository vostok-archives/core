using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vostok.Clusterclient.Helpers
{
    internal static class UrlEncodingHelper
    {
        #region HttpEncoderClone (partially stripped from .NET 4.0)

        // ReSharper disable MemberHidesStaticFromOuterClass
        private sealed class HttpEncoderClone
        {
            public static HttpEncoderClone Default { get; } = new HttpEncoderClone();

            internal byte[] UrlEncode(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
            {
                var encoded = UrlEncode(bytes, offset, count);

                return alwaysCreateNewReturnValue && encoded != null && encoded == bytes
                    ? (byte[]) encoded.Clone()
                    : encoded;
            }

            internal string UrlEncodeUnicode(string value, bool ignoreAscii)
            {
                if (value == null)
                {
                    return null;
                }

                var l = value.Length;
                var sb = new StringBuilder(l);

                for (var i = 0; i < l; i++)
                {
                    var ch = value[i];

                    if ((ch & 0xff80) == 0)
                    {
                        // 7 bit? 
                        if (ignoreAscii || HttpEncoderUtility.IsUrlSafeChar(ch))
                        {
                            sb.Append(ch);
                        }
                        else if (ch == ' ')
                        {
                            sb.Append('+');
                        }
                        else
                        {
                            sb.Append('%');
                            sb.Append(HttpEncoderUtility.IntToHex((ch >> 4) & 0xf));
                            sb.Append(HttpEncoderUtility.IntToHex(ch & 0xf));
                        }
                    }
                    else
                    {
                        // arbitrary Unicode?
                        sb.Append("%u");
                        sb.Append(HttpEncoderUtility.IntToHex((ch >> 12) & 0xf));
                        sb.Append(HttpEncoderUtility.IntToHex((ch >> 8) & 0xf));
                        sb.Append(HttpEncoderUtility.IntToHex((ch >> 4) & 0xf));
                        sb.Append(HttpEncoderUtility.IntToHex(ch & 0xf));
                    }
                }

                return sb.ToString();
            }

            internal string UrlPathEncode(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return value;
                }

                // recurse in case there is a query string 
                var i = value.IndexOf('?');
                if (i >= 0)
                    return UrlPathEncode(value.Substring(0, i)) + value.Substring(i);

                // encode DBCS characters and spaces only
                return HttpEncoderUtility.UrlEncodeSpaces(UrlEncodeNonAscii(value, Encoding.UTF8));
            }

            private static bool IsNonAsciiByte(byte b)
            {
                return b >= 0x7F || b < 0x20;
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
                    var ch = (char) bytes[offset + i];

                    if (ch == ' ')
                        cSpaces++;
                    else if (!HttpEncoderUtility.IsUrlSafeChar(ch))
                        cUnsafe++;
                }

                // nothing to expand? 
                if (cSpaces == 0 && cUnsafe == 0)
                    return bytes;

                // expand not 'safe' characters into %XX, spaces to +s
                var expandedBytes = new byte[count + cUnsafe*2];
                var pos = 0;

                for (var i = 0; i < count; i++)
                {
                    var b = bytes[offset + i];
                    var ch = (char) b;

                    if (HttpEncoderUtility.IsUrlSafeChar(ch))
                    {
                        expandedBytes[pos++] = b;
                    }
                    else if (ch == ' ')
                    {
                        expandedBytes[pos++] = (byte) '+';
                    }
                    else
                    {
                        expandedBytes[pos++] = (byte) '%';
                        expandedBytes[pos++] = (byte) HttpEncoderUtility.IntToHex((b >> 4) & 0xf);
                        expandedBytes[pos++] = (byte) HttpEncoderUtility.IntToHex(b & 0x0f);
                    }
                }

                return expandedBytes;
            }

            //  Helper to encode the non-ASCII url characters only
            private string UrlEncodeNonAscii(string str, Encoding e)
            {
                if (string.IsNullOrEmpty(str))
                    return str;
                if (e == null)
                    e = Encoding.UTF8;
                var bytes = e.GetBytes(str);
                var encodedBytes = UrlEncodeNonAscii(bytes, 0, bytes.Length, false /* alwaysCreateNewReturnValue */);
                return Encoding.ASCII.GetString(encodedBytes);
            }

            private byte[] UrlEncodeNonAscii(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
            {
                if (!ValidateUrlEncodingParameters(bytes, offset, count))
                {
                    return null;
                }

                var cNonAscii = 0;

                // count them first
                for (var i = 0; i < count; i++)
                {
                    if (IsNonAsciiByte(bytes[offset + i]))
                        cNonAscii++;
                }

                // nothing to expand?
                if (!alwaysCreateNewReturnValue && cNonAscii == 0)
                    return bytes;

                // expand not 'safe' characters into %XX, spaces to +s 
                var expandedBytes = new byte[count + cNonAscii*2];
                var pos = 0;

                for (var i = 0; i < count; i++)
                {
                    var b = bytes[offset + i];

                    if (IsNonAsciiByte(b))
                    {
                        expandedBytes[pos++] = (byte) '%';
                        expandedBytes[pos++] = (byte) HttpEncoderUtility.IntToHex((b >> 4) & 0xf);
                        expandedBytes[pos++] = (byte) HttpEncoderUtility.IntToHex(b & 0x0f);
                    }
                    else
                    {
                        expandedBytes[pos++] = b;
                    }
                }

                return expandedBytes;
            }

            private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
            {
                if (bytes == null && count == 0)
                    return false;
                if (bytes == null)
                {
                    throw new ArgumentNullException(nameof(bytes));
                }
                if (offset < 0 || offset > bytes.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(offset));
                }
                if (count < 0 || offset + count > bytes.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(count));
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
                    return (char) (n + '0');
                return (char) (n - 10 + 'a');
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

            //  Helper to encode spaces only
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static string UrlEncodeSpaces(string str)
            {
                if (str != null && str.IndexOf(' ') >= 0)
                    str = str.Replace(" ", "%20");
                return str;
            }
        }

        #endregion

        #region Url encode

        public static string UrlEncode(string str)
        {
            return UrlEncode(str, Encoding.UTF8);
        }

        public static string UrlPathEncode(string str)
        {
            return HttpEncoderClone.Default.UrlPathEncode(str);
        }

        public static string UrlEncode(string str, Encoding e)
        {
            if (str == null || HttpEncoderUtility.IsUrlSafeString(str))
                return str;

            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        public static string UrlEncode(byte[] bytes)
        {
            return bytes == null ? null : Encoding.ASCII.GetString(UrlEncodeToBytes(bytes));
        }

        public static string UrlEncode(byte[] bytes, int offset, int count)
        {
            return bytes == null ? null : Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, offset, count));
        }

        public static byte[] UrlEncodeToBytes(string str)
        {
            return str == null ? null : UrlEncodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;
            var bytes = e.GetBytes(str);
            return HttpEncoderClone.Default.UrlEncode(bytes, 0, bytes.Length, false);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            return bytes == null ? null : UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            return HttpEncoderClone.Default.UrlEncode(bytes, offset, count, true);
        }

        public static string UrlEncodeUnicode(string str)
        {
            return HttpEncoderClone.Default.UrlEncodeUnicode(str, false);
        }

        public static byte[] UrlEncodeUnicodeToBytes(string str)
        {
            return str == null ? null : Encoding.ASCII.GetBytes(UrlEncodeUnicode(str));
        }

        #endregion
    }
}
