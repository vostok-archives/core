using System.Globalization;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
	// (iloktionov): Код невозбранно украден из System.dll версии 4.0.
	internal static class HeadersParseUtility
	{
		public static string GetAttributeFromHeader(string headerValue, string attrName)
		{
			if (headerValue == null)
				return null;
			int length1 = headerValue.Length;
			int length2 = attrName.Length;
			int startIndex1 = 1;
			while (startIndex1 < length1)
			{
				startIndex1 = CultureInfo.InvariantCulture.CompareInfo.IndexOf(headerValue, attrName, startIndex1, CompareOptions.IgnoreCase);
				if (startIndex1 >= 0 && startIndex1 + length2 < length1)
				{
					char c1 = headerValue[startIndex1 - 1];
					char c2 = headerValue[startIndex1 + length2];
					if (c1 != 59 && c1 != 44 && !char.IsWhiteSpace(c1) || c2 != 61 && !char.IsWhiteSpace(c2))
						startIndex1 += length2;
					else
						break;
				}
				else
					break;
			}
			if (startIndex1 < 0 || startIndex1 >= length1)
				return null;
			int index1 = startIndex1 + length2;
			while (index1 < length1 && char.IsWhiteSpace(headerValue[index1]))
				++index1;
			if (index1 >= length1 || (int)headerValue[index1] != 61)
				return null;
			int startIndex2 = index1 + 1;
			while (startIndex2 < length1 && char.IsWhiteSpace(headerValue[startIndex2]))
				++startIndex2;
			if (startIndex2 >= length1)
				return null;
			string str;
			if (startIndex2 < length1 && (int)headerValue[startIndex2] == 34)
			{
				if (startIndex2 == length1 - 1)
					return null;
				int num = headerValue.IndexOf('"', startIndex2 + 1);
				if (num < 0 || num == startIndex2 + 1)
					return null;
				str = headerValue.Substring(startIndex2 + 1, num - startIndex2 - 1).Trim();
			}
			else
			{
				int index2 = startIndex2;
				while (index2 < length1 && (headerValue[index2] != 32 && headerValue[index2] != 44))
					++index2;
				if (index2 == startIndex2)
					return null;
				str = headerValue.Substring(startIndex2, index2 - startIndex2).Trim();
			}
			return str;
		}
	}
}