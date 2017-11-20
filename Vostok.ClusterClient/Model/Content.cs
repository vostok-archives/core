using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// Represents a buffered in-memory content transferred to/from server during request execution.
    /// </summary>
    public class Content
    {
        public static readonly Content Empty = new Content(new byte[0]);

        public Content([NotNull] byte[] buffer, int offset, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset), $"Offset {offset} is incorrect. Buffer size = {buffer.Length}.");

            if (length < 0 || length > buffer.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(length), $"Length {length} is incorrect. Buffer size = {buffer.Length}. Offset = {offset}.");

            Buffer = buffer;
            Offset = offset;
            Length = length;
        }

        public Content([NotNull] byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            Buffer = buffer;
            Offset = 0;
            Length = buffer.Length;
        }

        public Content(ArraySegment<byte> segment)
            : this(segment.Array, segment.Offset, segment.Count)
        {
        }

        /// <summary>
        /// Returns the byte array which contains the data.
        /// </summary>
        [NotNull]
        public byte[] Buffer { get; }

        /// <summary>
        /// Returns the offset of data in <see cref="Buffer"/>.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Returns the length of data in <see cref="Buffer"/>, starting from <see cref="Offset"/>.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// <para>Returns the data as a single byte array.</para>
        /// <para>If <see cref="Offset"/> == 0 and <see cref="Length"/> == <c>Buffer.Length</c>, <see cref="Buffer"/> is returned directly.</para>
        /// <para>If this condition does not hold, a copy operation takes place.</para>
        /// </summary>
        public byte[] ToArray()
        {
            if (Offset == 0 && Length == Buffer.Length)
                return Buffer;

            var array = new byte[Length];
            System.Buffer.BlockCopy(Buffer, Offset, array, 0, Length);
            return array;
        }

        /// <summary>
        /// Returns the data as byte array segment.
        /// </summary>
        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(Buffer, Offset, Length);
        }

        /// <summary>
        /// Wraps the data in a memory stream with public buffer.
        /// </summary>
        public MemoryStream ToMemoryStream()
        {
            return new MemoryStream(Buffer, Offset, Length, false, true);
        }

        /// <summary>
        /// Converts the data to a string using <see cref="UTF8Encoding"/>.
        /// </summary>
        public override string ToString()
        {
            return ToString(Encoding.UTF8);
        }

        /// <summary>
        /// Converts the data to a string using given <paramref name="encoding"></paramref>.
        /// </summary>
        public string ToString(Encoding encoding)
        {
            return encoding.GetString(Buffer, Offset, Length);
        }
    }
}
