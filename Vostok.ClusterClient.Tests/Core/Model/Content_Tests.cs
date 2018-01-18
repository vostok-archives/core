using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable ObjectCreationAsStatement

namespace Vostok.ClusterClient.Tests.Core.Model
{
    public class Content_Tests
    {
        [Test]
        public void Ctor_with_single_buffer_should_throw_if_that_buffer_is_null()
        {
            Action action = () => new Content(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Ctor_with_single_buffer_should_properly_initialize_content()
        {
            var buffer = new byte[10];

            var content = new Content(buffer);

            content.Buffer.Should().BeSameAs(buffer);
            content.Offset.Should().Be(0);
            content.Length.Should().Be(10);
        }

        [Test]
        public void Ctor_with_array_segment_should_properly_initialize_content()
        {
            var buffer = new byte[10];

            var segment = new ArraySegment<byte>(buffer, 3, 5);

            var content = new Content(segment);

            content.Buffer.Should().BeSameAs(buffer);
            content.Offset.Should().Be(3);
            content.Length.Should().Be(5);
        }

        [Test]
        public void Ctor_should_throw_if_buffer_is_null()
        {
            Action action = () => new Content(null, 0, 1);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_throw_if_offset_is_negative()
        {
            Action action = () => new Content(new byte[10], -1, 1);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_should_throw_if_offset_is_out_of_array_bounds()
        {
            Action action = () => new Content(new byte[10], 11, 0);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_should_throw_if_length_is_negative()
        {
            Action action = () => new Content(new byte[10], 0, -1);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_should_throw_if_length_and_offset_do_not_fit_into_array_bounds()
        {
            Action action = () => new Content(new byte[10], 5, 6);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Ctor_should_properly_initialize_content()
        {
            var buffer = new byte[10];

            var content = new Content(buffer, 3, 7);

            content.Buffer.Should().BeSameAs(buffer);
            content.Offset.Should().Be(3);
            content.Length.Should().Be(7);
        }

        [Test]
        public void ToArray_should_return_original_array_if_content_spans_entire_buffer()
        {
            var buffer = new byte[10];

            var content = new Content(buffer);

            content.ToArray().Should().BeSameAs(buffer);
        }

        [Test]
        public void ToArray_should_copy_subarray_if_content_spans_only_a_slice_of_buffer()
        {
            var buffer = Guid.NewGuid().ToByteArray();

            var content = new Content(buffer, 5, 10);

            content.ToArray().Should().NotBeSameAs(buffer).And.Equal(buffer.Skip(5).Take(10));
        }

        [Test]
        public void ToArraySegment_should_return_correct_segment()
        {
            var content = new Content(Guid.NewGuid().ToByteArray(), 5, 10);

            var segment = content.ToArraySegment();

            segment.Array.Should().BeSameAs(content.Buffer);
            segment.Offset.Should().Be(5);
            segment.Count.Should().Be(10);
        }

        [Test]
        public void ToMemoryStream_should_return_stream_with_public_buffer_and_correct_data()
        {
            var content = new Content(Guid.NewGuid().ToByteArray(), 5, 10);

            var stream = content.ToMemoryStream();

            stream.GetBuffer().Should().BeSameAs(content.Buffer);
            stream.Length.Should().Be(10);
            stream.ToArray().Should().Equal(content.ToArray());
        }

        [Test]
        public void ToString_should_convert_bytes_to_utf8_string_by_default()
        {
            var buffer = Guid.NewGuid()
                .ToByteArray()
                .Concat(Encoding.UTF8.GetBytes("Hello!"))
                .Concat(Guid.NewGuid().ToByteArray())
                .ToArray();

            var content = new Content(buffer, 16, 6);

            content.ToString().Should().Be("Hello!");
        }
    }
}
