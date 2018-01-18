using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Sending;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Sending
{
    public class RequestConverter_Tests
    {
        private RequestConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new RequestConverter(new ConsoleLog());
        }

        
        [TestCase("foo/bar")]
        [TestCase("/foo/bar")]
        [TestCase("/")]
        [TestCase("")]
        public void Should_return_null_when_replica_url_is_not_absolute(string replicaUrl)
        {
            converter.TryConvertToAbsolute(Request.Get("request/path"), new Uri(replicaUrl, UriKind.Relative)).Should().BeNull();
        }

        [Test]
        public void Should_return_null_when_replica_url_contains_query_parameters()
        {
            converter.TryConvertToAbsolute(Request.Get("request/path"), new Uri("http://replica?a=b")).Should().BeNull();
        }

        [Test]
        public void Should_return_null_when_request_url_is_absolute()
        {
            converter.TryConvertToAbsolute(Request.Get("http://host/request/path"), new Uri("http://replica")).Should().BeNull();
        }

        
        [TestCase("http://replica:123/foo", "bar/baz", "http://replica:123/foo/bar/baz")]
        [TestCase("http://replica:123/foo", "bar/baz?k=v", "http://replica:123/foo/bar/baz?k=v")]
        [TestCase("http://replica:123/foo/", "bar/baz?k=v", "http://replica:123/foo/bar/baz?k=v")]
        [TestCase("http://replica:123/foo", "/bar/baz?k=v", "http://replica:123/foo/bar/baz?k=v")]
        [TestCase("http://replica:123/foo/", "/bar/baz?k=v", "http://replica:123/foo/bar/baz?k=v")]
        [TestCase("http://replica:123", "/bar/baz?k=v", "http://replica:123/bar/baz?k=v")]
        [TestCase("http://replica:123", "/?k=v", "http://replica:123/?k=v")]
        [TestCase("http://replica:123/", "/?k=v", "http://replica:123/?k=v")]
        [TestCase("http://replica:123", "?k=v", "http://replica:123/?k=v")]
        public void Should_return_request_with_correct_merged_url(string replicaUrl, string requestUrl, string expectedUrl)
        {
            var convertedRequest = converter.TryConvertToAbsolute(Request.Get(requestUrl), new Uri(replicaUrl));

            convertedRequest.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
            convertedRequest.Url.IsAbsoluteUri.Should().BeTrue();

            convertedRequest.Url.OriginalString.Should().Be(expectedUrl);
        }
    }
}
