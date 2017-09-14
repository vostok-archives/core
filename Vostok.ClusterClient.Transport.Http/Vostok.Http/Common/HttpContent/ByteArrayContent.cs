using System;
using System.IO;
using System.Threading.Tasks;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
	public class ByteArrayContent : HttpContentBase
	{
		public ByteArrayContent(byte[] buffer, int offset, int length)
		{
			Preconditions.EnsureNotNull(buffer, "buffer");
			Preconditions.EnsureArgumentRange(offset >= 0 && offset <= buffer.Length, "offset", "Incorrect offset = {0}. Buffer length = {1}.", offset, buffer.Length);
			Preconditions.EnsureArgumentRange(length >= 0 && offset + length <= buffer.Length, "length", "Incorrect length = {0}. Offset = {1}. Buffer length = {2}.", length, offset, buffer.Length);
			this.buffer = buffer;
			this.offset = offset;
			this.length = length;
		}

		public ByteArrayContent(byte[] buffer)
		{
			Preconditions.EnsureNotNull(buffer, "buffer");
			this.buffer = buffer;
			offset = 0;
			length = buffer.Length;
		}

	    public ByteArrayContent(ArraySegment<byte> segment)
            : this (segment.Array, segment.Offset, segment.Count)
	    {
	    }

		public override void CopyTo(Stream outputStream)
		{
			outputStream.Write(buffer, offset, length);
		}

		public override Task CopyToAsync(Stream outputStream)
		{
			return outputStream.WriteAsync(buffer, offset, length);
		}

		public byte[] ToArray()
		{
			if (offset == 0 && length == buffer.Length)
				return buffer;
			var array = new byte[length];
			System.Buffer.BlockCopy(buffer, offset, array, 0, length);
			return array;
		}

	    public ArraySegment<byte> ToArraySegment()
	    {
	        return new ArraySegment<byte>(buffer, offset, length);
	    }

	    public MemoryStream ToMemoryStream()
		{
			return new MemoryStream(buffer, offset, length, false, true);
		}

	    public override string ToString()
		{
			return (Charset ?? EncodingFactory.GetDefault()).GetString(buffer, offset, length);
		}

		public byte[] Buffer
		{
			get { return buffer; }
		}

		public int Offset
		{
			get { return offset; }
		}

		public override long Length
		{
			get { return length; }
		}

		public static readonly ByteArrayContent Empty = new ByteArrayContent(new byte[0]);

		protected readonly byte[] buffer;
		protected readonly int offset;
		protected readonly int length;
	}
}