using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transforms;

namespace Vostok.ClusterClient.Tests.Core.Transforms
{
    public class DefaultHeadersTransform_Tests
    {
        private Request request;

        [SetUp]
        public void SetUp()
        {
            request = Request.Get("foo/bar");
        }

        [Test]
        public void Ctor_should_correctly_handle_null_input()
        {
            var transform = new DefaultHeadersTransform();

            transform.DefaultHeaders.Should().BeEmpty();
        }

        [Test]
        public void Ctor_should_correctly_handle_input_headers()
        {
            var transform = new DefaultHeadersTransform(
                new[]
                {
                    new Header("k1", "v1"),
                    new Header("k2", "v2"),
                    new Header("k3", "v3")
                });

            transform.DefaultHeaders.Count.Should().Be(3);
            transform.DefaultHeaders["k1"].Should().Be("v1");
            transform.DefaultHeaders["k2"].Should().Be("v2");
            transform.DefaultHeaders["k3"].Should().Be("v3");
        }

        [Test]
        public void Collection_initializer_syntax_with_header_instances_should_correctly_populate_default_headers()
        {
            var transform = new DefaultHeadersTransform
            {
                new Header("k1", "v1"),
                new Header("k2", "v2"),
                new Header("k3", "v3")
            };

            transform.DefaultHeaders.Count.Should().Be(3);
            transform.DefaultHeaders["k1"].Should().Be("v1");
            transform.DefaultHeaders["k2"].Should().Be("v2");
            transform.DefaultHeaders["k3"].Should().Be("v3");
        }

        [Test]
        public void Collection_initializer_syntax_with_name_and_value_pairs_should_correctly_populate_default_headers()
        {
            var transform = new DefaultHeadersTransform
            {
                {"k1", "v1"},
                {"k2", "v2"},
                {"k3", "v3"}
            };

            transform.DefaultHeaders.Count.Should().Be(3);
            transform.DefaultHeaders["k1"].Should().Be("v1");
            transform.DefaultHeaders["k2"].Should().Be("v2");
            transform.DefaultHeaders["k3"].Should().Be("v3");
        }

        [Test]
        public void Transform_should_return_same_request_if_there_are_no_default_headers()
        {
            var transform = new DefaultHeadersTransform();

            transform.Transform(request).Should().BeSameAs(request);
        }

        [Test]
        public void Transform_should_reuse_default_headers_instance_when_request_headers_are_null()
        {
            var transform = new DefaultHeadersTransform
            {
                {"k1", "v1"}
            };

            transform.Transform(request).Headers.Should().BeSameAs(transform.DefaultHeaders);
        }

        [Test]
        public void Transform_should_merge_request_headers_with_default_headers()
        {
            var transform = new DefaultHeadersTransform
            {
                {"k1", "v1"},
                {"k2", "v2"}
            };

            request = request
                .WithHeader("k3", "v3")
                .WithHeader("k4", "v4");

            var transformedRequest = transform.Transform(request);

            Debug.Assert(transformedRequest.Headers != null, "transformedRequest.Headers != null");
            transformedRequest.Headers.Count.Should().Be(4);
            transformedRequest.Headers["k1"].Should().Be("v1");
            transformedRequest.Headers["k2"].Should().Be("v2");
            transformedRequest.Headers["k3"].Should().Be("v3");
            transformedRequest.Headers["k4"].Should().Be("v4");
        }

        [Test]
        public void Transform_should_give_priority_to_request_headers_when_merging()
        {
            var transform = new DefaultHeadersTransform
            {
                {"k1", "v1"},
                {"k2", "v2"}
            };

            request = request
                .WithHeader("k2", "override")
                .WithHeader("k3", "v3");

            var transformedRequest = transform.Transform(request);

            Debug.Assert(transformedRequest.Headers != null, "transformedRequest.Headers != null");
            transformedRequest.Headers.Count.Should().Be(3);
            transformedRequest.Headers["k1"].Should().Be("v1");
            transformedRequest.Headers["k2"].Should().Be("override");
            transformedRequest.Headers["k3"].Should().Be("v3");
        }

        [Test]
        public void Transform_should_preserve_request_method()
        {
            var transform = new DefaultHeadersTransform
            {
                {"k1", "v1"},
                {"k2", "v2"}
            };

            transform.Transform(request).Method.Should().BeSameAs(request.Method);
        }

        [Test]
        public void Transform_should_preserve_request_url()
        {
            var transform = new DefaultHeadersTransform
            {
                {"k1", "v1"},
                {"k2", "v2"}
            };

            transform.Transform(request).Url.Should().BeSameAs(request.Url);
        }

        [Test]
        public void Transform_should_preserve_request_content()
        {
            var transform = new DefaultHeadersTransform
            {
                {"k1", "v1"},
                {"k2", "v2"}
            };

            request = request.WithContent(new byte[8]);

            transform.Transform(request).Content.Should().BeSameAs(request.Content);
        }
    }
}
