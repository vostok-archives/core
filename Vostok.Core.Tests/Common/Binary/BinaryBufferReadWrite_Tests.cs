using System;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Binary;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Utilities;

namespace Vostok.Common.Binary
{
    [TestFixture]
    public class BinaryBufferReadWrite_Tests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Should_correctly_read_and_write_boolean_values(bool item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadBool());
        }

        [TestCase(byte.MinValue)]
        [TestCase(byte.MaxValue)]
        [TestCase(50)]
        public void Should_correctly_read_and_write_byte_values(byte item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadByte());
        }

        [TestCase(short.MinValue)]
        [TestCase(short.MinValue + 1)]
        [TestCase(short.MaxValue)]
        [TestCase(short.MaxValue - 1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public void Should_correctly_read_and_write_int16_values(short item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadInt16());
        }

        [TestCase(ushort.MinValue)]
        [TestCase(ushort.MaxValue)]
        public void Should_correctly_read_and_write_uint16_values(ushort item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadUInt16());
        }

        [TestCase(int.MinValue)]
        [TestCase(int.MinValue + 1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MaxValue - 1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public void Should_correctly_read_and_write_int32_values(int item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadInt32());
        }

        [TestCase(uint.MinValue)]
        [TestCase(uint.MinValue + 1)]
        [TestCase(uint.MaxValue)]
        [TestCase(uint.MaxValue - 1)]
        public void Should_correctly_read_and_write_uint32_values(uint item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadUInt32());
        }

        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(0L)]
        [TestCase(1L)]
        [TestCase(-1L)]
        public void Should_correctly_read_and_write_int64_values(long item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadInt64());
        }

        [TestCase(ulong.MinValue)]
        [TestCase(ulong.MinValue + 1)]
        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MaxValue - 1)]
        public void Should_correctly_read_and_write_uint64_values(ulong item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadUInt64());
        }

        [TestCase(0f)]
        [TestCase(0.0f)]
        [TestCase(float.MaxValue)]
        [TestCase(float.MinValue)]
        [TestCase(float.NegativeInfinity)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NaN)]
        [TestCase(float.Epsilon)]
        [TestCase(-1.1111111f)]
        [TestCase(0.43353543f)]
        [TestCase(1/3f)]
        [TestCase(-1/10f)]
        public void Should_correctly_read_and_write_float_values(float item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadFloat());
        }

        [TestCase(0d)]
        [TestCase(0.0d)]
        [TestCase(double.MaxValue)]
        [TestCase(double.MinValue)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NaN)]
        [TestCase(double.Epsilon)]
        [TestCase(-1.1111111d)]
        [TestCase(0.43353543d)]
        [TestCase(1/3d)]
        [TestCase(-1/10d)]
        public void Should_correctly_read_and_write_double_values(double item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadDouble());
        }

        [TestCase("abcdxyz")]
        [TestCase("ABCDXYZ")]
        [TestCase("0123456789")]
        [TestCase("-_?:;&()*^:%@!<>[]{}=+-")]
        [TestCase("//\\        $#'`~\"")]
        [TestCase("àáâãäå¸æçýþÿ")]
        public void Should_correctly_read_and_write_string_values(string item)
        {
            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadString());
        }

        [Test]
        public void Should_correctly_read_and_write_guid_values()
        {
            Test(Guid.NewGuid(), (value, writer) => writer.Write(value), reader => reader.ReadGuid());
        }

        [Test]
        [Repeat(10)]
        public void Should_correctly_read_and_write_random_strings()
        {
            Test(Encoding.UTF8.GetString(ThreadSafeRandom.NextBytes(4.Kilobytes())), (value, writer) => writer.Write(value), reader => reader.ReadString());
        }

        [Test]
        [Repeat(10)]
        public void Should_correctly_read_and_write_random_byte_arrays()
        {
            var item = ThreadSafeRandom.NextBytes(ThreadSafeRandom.Next(100));

            Test(item, (value, writer) => writer.Write(value), reader => reader.ReadByteArray());
        }

        [Test]
        public void Should_correctly_read_and_write_strings_without_length_prefix()
        {
            Test("Hello!", (value, writer) => writer.WriteWithoutLengthPrefix(value), reader => reader.ReadString(6));
        }

        [Test]
        public void Should_correctly_read_and_write_byte_arrays_without_length_prefix()
        {
            Test(new byte[] {1, 2, 3, 4, 5, 6}, (value, writer) => writer.WriteWithoutLengthPrefix(value), reader => reader.ReadByteArray(6));
        }

        private static void Test<T>(T item, Action<T, IBinaryWriter> write, Func<IBinaryReader, T> read)
        {
            var writer = new BinaryBufferWriter(1);

            write(item, writer);

            var reader = new BinaryBufferReader(writer.Buffer, 0);

            var readItem = read(reader);

            readItem.ShouldBeEquivalentTo(item);
        }
    }
}
