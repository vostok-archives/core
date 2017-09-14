using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using Kontur.Clusterclient.Core.Model;
using Kontur.Net.Http;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Xunit;

namespace Kontur.Clusterclient.Transport.Http
{
    [TestFixture]
    internal class RequestConverter_Tests
    {
        private RequestConverter converter;

        public RequestConverter_Tests()
        {
            converter = new RequestConverter();
        }

        [TestCase(RequestMethods.Get, HttpMethod.GET)]
        [TestCase(RequestMethods.Delete, HttpMethod.DELETE)]
        [TestCase(RequestMethods.Head, HttpMethod.HEAD)]
        [TestCase(RequestMethods.Options, HttpMethod.OPTIONS)]
        [TestCase(RequestMethods.Patch, HttpMethod.PATCH)]
        [TestCase(RequestMethods.Post, HttpMethod.POST)]
        [TestCase(RequestMethods.Put, HttpMethod.PUT)]
        [TestCase(RequestMethods.Trace, HttpMethod.TRACE)]
        [TestCase("BULLSHIT", HttpMethod.Unknown)]
        public void Should_correctly_convert_request_method(string method, HttpMethod expected)
        {
            var request = new Request(method, new Uri("http://foo/bar"));

            var convertedRequest = converter.Convert(request);

            convertedRequest.Method.Should().Be(expected);
        }

        [Fact]
        public void Should_not_modify_request_url()
        {
            var request = Request.Get(new Uri("http://foo/bar"));

            var convertedRequest = converter.Convert(request);

            convertedRequest.AbsoluteUri.Should().BeSameAs(request.Url);
        }

        [Fact]
        public void Should_convert_null_headers_to_null_or_empty_headers()
        {
            var request = Request.Get(new Uri("http://foo/bar"));

            var convertedRequest = converter.Convert(request);

            convertedRequest.Headers.Should().BeEmpty();
        }

        [Fact]
        public void Should_convert_empty_headers_to_null_or_empty_headers()
        {
            var request = new Request(RequestMethods.Get, new Uri("http://foo/bar"), headers: Headers.Empty);

            var convertedRequest = converter.Convert(request);

            convertedRequest.Headers.Should().BeEmpty();
        }

        [Fact]
        public void Should_convert_custom_headers()
        {
            var request = Request.Get("http://foo/bar")
                .WithHeader("h1", "v1")
                .WithHeader("h2", "v2")
                .WithHeader("h3", "v3");

            var convertedHeaders = converter.Convert(request).Headers;

            convertedHeaders.Should().HaveCount(3);
            convertedHeaders["h1"].Should().Be("v1");
            convertedHeaders["h2"].Should().Be("v2");
            convertedHeaders["h3"].Should().Be("v3");
        }

        [Fact]
        public void Should_not_convert_content_length_header()
        {
            var request = Request.Get("http://foo/bar").WithHeader(HeaderNames.ContentLength, 123);

            converter.Convert(request).Headers.Should().BeEmpty();
        }

        [Fact]
        public void Should_not_convert_content_type_header()
        {
            var request = Request.Post("http://foo/bar").WithContentTypeHeader("application/json");

            converter.Convert(request).Headers.Should().BeEmpty();
        }

        [Fact]
        public void Should_not_convert_content_range_header()
        {
            var request = Request.Post("http://foo/bar").WithContentRangeHeader(1, 10, 100);

            converter.Convert(request).Headers.Should().BeEmpty();
        }

        [Fact]
        public void Should_correctly_convert_accept_header()
        {
            var request = Request.Get("http://foo/bar").WithAcceptHeader("application/json");

            converter.Convert(request).Headers.Accept.Should().Be("application/json");
        }

        [Fact]
        public void Should_correctly_convert_accept_charset_header()
        {
            var request = Request.Get("http://foo/bar").WithAcceptCharsetHeader("utf-8");

            converter.Convert(request).Headers.AcceptCharset.WebName.Should().Be("utf-8");
        }

        [Fact]
        public void Should_correctly_convert_authorization_header()
        {
            var request = Request.Get("http://foo/bar").WithAuthorizationHeader("oauth 123");

            converter.Convert(request).Headers.Authorization.Scheme.Should().Be("oauth");
            converter.Convert(request).Headers.Authorization.Parameter.Should().Be("123");
        }

        [Fact]
        public void Should_correctly_convert_host_header()
        {
            var request = Request.Get("http://foo/bar").WithHeader(HeaderNames.Host, "google.ru");

            converter.Convert(request).Headers.Host.Should().Be("google.ru");
        }

        [Fact]
        public void Should_correctly_convert_if_match_header()
        {
            var request = Request.Get("http://foo/bar").WithIfMatchHeader("\"foo\"");

            converter.Convert(request).Headers.IfMatch.Tag.Should().Be("\"foo\"");
        }

        [Fact]
        public void Should_correctly_convert_if_modified_since_header()
        {
            var now = DateTime.UtcNow;

            now -= now.Millisecond.Milliseconds();

            var request = Request.Get("http://foo/bar").WithIfModifiedSinceHeader(now);

            converter.Convert(request).Headers.IfModifiedSince.Should().BeCloseTo(now, 1000);
        }

        [Fact]
        public void Should_correctly_convert_range_header()
        {
            var request = Request.Get("http://foo/bar").WithRangeHeader(5, 10);

            converter.Convert(request).Headers.Range.Unit.Should().Be("bytes");
            converter.Convert(request).Headers.Range.Ranges.Single().From.Should().Be(5);
            converter.Convert(request).Headers.Range.Ranges.Single().To.Should().Be(10);
        }

        [Fact]
        public void Should_correctly_convert_referer_header()
        {
            var request = Request.Get("http://foo/bar").WithHeader(HeaderNames.Referer, "foo/bar");

            converter.Convert(request).Headers.Referer.Should().Be("foo/bar");
        }

        [Fact]
        public void Should_correctly_convert_user_agent_header()
        {
            var request = Request.Get("http://foo/bar").WithHeader(HeaderNames.UserAgent, "foo/bar");

            converter.Convert(request).Headers.UserAgent.Should().Be("foo/bar");
        }

        [Fact]
        public void Should_convert_null_body_to_null_when_there_are_no_content_type_or_content_range_headers()
        {
            var request = Request.Get("http://foo/bar");

            var convertedBody = converter.Convert(request).Body;

            convertedBody.Should().BeNull();
        }

        [Fact]
        public void Should_convert_null_body_to_empty_when_there_is_content_type_header()
        {
            var request = Request.Post("http://foo/bar").WithContentTypeHeader("application/json");

            var convertedBody = converter.Convert(request).Body;

            convertedBody.Should().NotBeNull();
            convertedBody.Length.Should().Be(0);
            convertedBody.ContentType.Type.Should().Be("application/json");
        }

        [Fact]
        public void Should_convert_null_body_to_empty_when_there_is_content_range_header()
        {
            var request = Request.Post("http://foo/bar").WithContentRangeHeader(5, 9, 100);

            var convertedBody = converter.Convert(request).Body;

            convertedBody.Should().NotBeNull();
            convertedBody.Length.Should().Be(0);
            convertedBody.ContentRange.From.Should().Be(5);
            convertedBody.ContentRange.To.Should().Be(9);
            convertedBody.ContentRange.Length.Should().Be(100);
        }

        [Fact]
        public void Should_correctly_convert_non_empty_body()
        {
            var buffer = Guid.NewGuid().ToByteArray();

            var request = Request.Post("http://foo/bar").WithContent(buffer, 5, 10);

            var convertedBody = converter.Convert(request).Body.Should().BeOfType<ByteArrayContent>().Which;

            convertedBody.Buffer.Should().BeSameAs(buffer);
            convertedBody.Offset.Should().Be(5);
            convertedBody.Length.Should().Be(10);
        }

        [Fact]
        public void Should_correctly_convert_non_empty_body_with_content_type_header()
        {
            var request = Request.Post("http://foo/bar")
                .WithContent(Guid.NewGuid().ToByteArray())
                .WithContentTypeHeader("application/json");

            var convertedBody = converter.Convert(request).Body;

            convertedBody.ContentType.Type.Should().Be("application/json");
        }

        [Fact]
        public void Should_correctly_convert_non_empty_body_with_content_range_header()
        {
            var request = Request.Post("http://foo/bar")
                .WithContent(Guid.NewGuid().ToByteArray())
                .WithContentRangeHeader(5, 9, 100);

            var convertedBody = converter.Convert(request).Body;

            convertedBody.ContentRange.From.Should().Be(5);
            convertedBody.ContentRange.To.Should().Be(9);
            convertedBody.ContentRange.Length.Should().Be(100);
        }
    }
}