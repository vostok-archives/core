using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Model
{
    public class RequestUrlBuilder_Tests
    {
        [Test]
        public void Should_produce_correct_uri_without_query_parameters()
        {
            var builder = new RequestUrlBuilder
            {
                "foo/",
                "bar/",
                "baz"
            };

            builder.Build().OriginalString.Should().Be("foo/bar/baz");
        }

        [Test]
        public void Should_produce_correct_uri_with_query_parameters()
        {
            var builder = new RequestUrlBuilder
            {
                "foo/",
                "bar/",
                "baz",
                {"a", 1},
                {"b", 2}
            };

            builder.Build().OriginalString.Should().Be("foo/bar/baz?a=1&b=2");
        }

        [Test]
        public void Should_produce_correct_uri_when_starting_from_non_empty_url_with_path()
        {
            var builder = new RequestUrlBuilder("initial/segments/")
            {
                "foo/",
                "bar/",
                "baz"
            };

            builder.Build().OriginalString.Should().Be("initial/segments/foo/bar/baz");
        }

        [Test]
        public void Should_produce_correct_uri_when_starting_from_non_empty_url_with_path_and_query()
        {
            var builder = new RequestUrlBuilder("foo/bar/baz?a=1")
            {
                {"b", 2},
                {"c", 3}
            };

            builder.Build().OriginalString.Should().Be("foo/bar/baz?a=1&b=2&c=3");
        }

        [Test]
        public void Should_encode_query_parameter_names_and_values()
        {
            var builder = new RequestUrlBuilder
            {
                "foo/",
                "bar/",
                "baz",
                {"a?a", "b?b"},
            };

            builder.Build().OriginalString.Should().Be("foo/bar/baz?a%3fa=b%3fb");
        }

        [Test]
        public void Should_not_be_usable_after_producing_an_url()
        {
            var builder = new RequestUrlBuilder
            {
                "foo/",
                "bar/",
                "baz"
            };

            builder.Build();

            Action pathAppend = () => builder.AppendToPath("123");
            Action queryAppend = () => builder.AppendToQuery("123", "456");

            pathAppend.ShouldThrow<ObjectDisposedException>();
            queryAppend.ShouldThrow<ObjectDisposedException>();
        }

        [Test]
        public void Should_produce_repetable_results()
        {
            for (var i = 0; i < 10; i++)
            {
                var url = new RequestUrlBuilder()
                    .AppendToPath("foo/")
                    .AppendToPath("bar")
                    .Build();

                url.OriginalString.Should().Be("foo/bar");
            }
        }

        [Test]
        public void Should_not_allow_to_append_to_path_after_appending_to_query()
        {
            var builder = new RequestUrlBuilder();

            builder.AppendToPath("foo/");

            builder.AppendToQuery("key", "value");

            Action action = () => builder.AppendToPath("bar");

            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Should_produce_correct_uri_when_appending_path_segments_without_slashes()
        {
            var builder = new RequestUrlBuilder
            {
                "1",
                "2",
                "3",
                "4"
            };

            builder.Build().OriginalString.Should().Be("1/2/3/4");
        }

        [Test]
        public void Should_produce_correct_uri_when_appending_path_segments_with_excess_slashes()
        {
            var builder = new RequestUrlBuilder
            {
                "11/",
                "/22/",
                "/33/",
                "/44"
            };

            builder.Build().OriginalString.Should().Be("11/22/33/44");
        }

        [Test]
        public void Should_safely_handle_multiple_calls_to_build_method()
        {
            var builder = new RequestUrlBuilder
            {
                "foo",
                "bar",
                "baz"
            };

            var url1 = builder.Build();
            var url2 = builder.Build();

            url2.Should().BeSameAs(url1);
        }
    }
}
