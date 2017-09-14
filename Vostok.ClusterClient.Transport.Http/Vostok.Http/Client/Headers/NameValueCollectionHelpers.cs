using System.Collections.Specialized;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers
{
    // todo: extract to core
    internal static class NameValueCollectionHelpers
    {
        // todo: test it!
        public static int ElementwiseHash(this NameValueCollection nameValueCollection)
        {
            unchecked
            {
                var current = nameValueCollection.Count;
                for (int i = 0; i < nameValueCollection.Count; i++)
                {
                    current = (current*397) ^ nameValueCollection[i].GetHashCode();
                }
                return current;
            }
        }

        // todo: test it!
        public static bool ElementwiseEquals(this NameValueCollection one, NameValueCollection other)
        {
            if (ReferenceEquals(null, other))
                return ReferenceEquals(null, one);
            if (ReferenceEquals(null, one))
                return false;
            if (ReferenceEquals(one, other))
                return true;
            if (one.Count != other.Count)
                return false;

            for (int i = 0; i < one.Count; i++)
            {
                var key = one.GetKey(i);
                var value = one[i];
                var otherValue = other[key];
                if (!Equals(value, otherValue))
                    return false;
            }

            return true;
        }
    }
}