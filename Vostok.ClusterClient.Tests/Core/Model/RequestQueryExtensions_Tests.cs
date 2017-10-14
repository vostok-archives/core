using FluentAssertions;
using Vostok.Clusterclient.Model;
using Xunit;

namespace Vostok.Clusterclient.Core.Model
{
    public class RequestQueryExtensions_Tests
    {
        private readonly Request request;

        public RequestQueryExtensions_Tests()
        {
            request = Request.Get("foo/bar?a=1");
        }

        [Fact]
        public void WithAdditionalQueryParameter_should_correctly_add_parameter_with_string_value()
        {
            request.WithAdditionalQueryParameter("b", "2").Url.ToString().Should().Be("foo/bar?a=1&b=2");
        }

        [Fact]
        public void WithAdditionalQueryParameter_should_correctly_add_parameter_with_typed_object_value()
        {
            request.WithAdditionalQueryParameter("b", 2).Url.ToString().Should().Be("foo/bar?a=1&b=2");
        }
    }
}
