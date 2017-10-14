using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal class AirlockWriteStream : Stream
    {
        private readonly IBinaryWriter writer;

        public AirlockWriteStream(IBinaryWriter writer)
        {
            this.writer = writer;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override bool CanTimeout => false;

        public override void Write(byte[] buffer, int offset, int count)
        {
            writer.WriteWithoutLengthPrefix(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            writer.Write(value);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Write(buffer, offset, count);

            return Task.CompletedTask;
        }

        public override void Flush()
        {
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #region Not supported

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
