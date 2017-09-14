using System;
using System.Collections.Specialized;
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
    internal class ResponseConverter_Tests
    {
        private ResponseConverter converter;
        
        public ResponseConverter_Tests()
        {
            converter = new ResponseConverter();
        }

        [Fact]
        public void Should_correctly_convert_all_response_codes()
        {
            foreach (var code in Enum.GetValues(typeof(HttpResponseCode)).Cast<HttpResponseCode>())
            {
                var convertedCode = converter.Convert(new HttpResponse(code)).Code;
                var convertedCodeNumeric = (int) convertedCode;

                var expectedCode = (ResponseCode) Enum.Parse(typeof (ResponseCode), code.ToString(), true);
                var expectedCodeNumeric = (int) expectedCode;

                convertedCodeNumeric.Should().Be(expectedCodeNumeric);
            }
        }

        [Fact]
        public void Should_convert_empty_body_to_empty_content()
        {
            var response = new HttpResponse(HttpResponseCode.Ok);

            var convertedResponse = converter.Convert(response);

            convertedResponse.Content.Should().BeSameAs(Content.Empty);
        }

        [Fact]
        public void Should_correctly_convert_non_empty_body()
        {
            var buffer = Guid.NewGuid().ToByteArray();

            var response = new HttpResponse(HttpResponseCode.Ok, new ByteArrayContent(buffer, 5, 10));

            var convertedResponseContent = converter.Convert(response).Content;

            convertedResponseContent.Buffer.Should().BeSameAs(buffer);
            convertedResponseContent.Offset.Should().Be(5);
            convertedResponseContent.Length.Should().Be(10);
        }

        [Fact]
        public void Should_convert_empty_headers_to_empty_headers()
        {
            var response = new HttpResponse(HttpResponseCode.Ok);

            var convertedResponse = converter.Convert(response);

            convertedResponse.Headers.Should().BeSameAs(Headers.Empty);
        }

        [Fact]
        public void Should_correctly_convert_non_empty_headers()
        {
            var originalHeaders = new HttpResponseHeaders(new NameValueCollection
            {
                { HeaderNames.Authorization, "oauth 3534dfg3sdfgdfg345" },
                { HeaderNames.ContentType, "application/json" },
                { HeaderNames.ContentLength, "123" },
                { HeaderNames.Server, "Apache" }
            });

            var convertedHeaders = converter.Convert(new HttpResponse(HttpResponseCode.Ok, originalHeaders)).Headers;

            convertedHeaders.Should().HaveCount(originalHeaders.Count);

            foreach (var key in originalHeaders.Keys)
            {
                convertedHeaders[key].Should().Be(originalHeaders[key]);
            }
        }
    }
}