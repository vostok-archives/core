using System;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Sending;
using Vostok.Logging;
using Xunit;

namespace Vostok.Clusterclient.Core.Sending
{
    public class RequestConverter_Tests
    {
        private readonly RequestConverter converter;

        public RequestConverter_Tests()
        {
            converter = new RequestConverter(new ConsoleLog());
        }

        [Theory]
        [InlineData("foo/bar")]
        [InlineData("/foo/bar")]
        [InlineData("/")]
        [InlineData("")]
        public void Should_return_null_when_replica_url_is_not_absolute(string replicaUrl)
        {
            converter.TryConvertToAbsolute(Request.Get("request/path"), new Uri(replicaUrl, UriKind.Relative)).Should().BeNull();
        }

        [Fact]
        public void Should_return_null_when_replica_url_contains_query_parameters()
        {
            converter.TryConvertToAbsolute(Request.Get("request/path"), new Uri("http://replica?a=b")).Should().BeNull();
        }

        [Fact]
        public void Should_return_null_when_request_url_is_absolute()
        {
            converter.TryConvertToAbsolute(Request.Get("http://host/request/path"), new Uri("http://replica")).Should().BeNull();
        }

        [Theory]
        [InlineData("http://replica:123/foo", "bar/baz", "http://replica:123/foo/bar/baz")]
        [InlineData("http://replica:123/foo", "bar/baz?k=v", "http://replica:123/foo/bar/baz?k=v")]
        [InlineData("http://replica:123/foo/", "bar/baz?k=v", "http://replica:123/foo/bar/baz?k=v")]
        [InlineData("http://replica:123/foo", "/bar/baz?k=v", "http://replica:123/foo/bar/baz?k=v")]
        [InlineData("http://replica:123/foo/", "/bar/baz?k=v", "http://replica:123/foo/bar/baz?k=v")]
        [InlineData("http://replica:123", "/bar/baz?k=v", "http://replica:123/bar/baz?k=v")]
        [InlineData("http://replica:123", "/?k=v", "http://replica:123/?k=v")]
        [InlineData("http://replica:123/", "/?k=v", "http://replica:123/?k=v")]
        [InlineData("http://replica:123", "?k=v", "http://replica:123/?k=v")]
        public void Should_return_request_with_correct_merged_url(string replicaUrl, string requestUrl, string expectedUrl)
        {
            var convertedRequest = converter.TryConvertToAbsolute(Request.Get(requestUrl), new Uri(replicaUrl));

            convertedRequest.Should().NotBeNull();

            convertedRequest.Url.IsAbsoluteUri.Should().BeTrue();

            convertedRequest.Url.OriginalString.Should().Be(expectedUrl);
        }
    }
}
