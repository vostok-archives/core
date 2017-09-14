namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers
{
    public struct HttpRequestHeader
    {
        public HttpRequestHeader(string name, string value) 
            : this()
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }

        #region Overrides of ValueType

        public override string ToString()
        {
            return string.Concat(Name ?? string.Empty, ": ", Value ?? string.Empty);
        }

        #endregion
    }
}