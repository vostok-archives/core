using System;
using System.IO;
using System.Threading.Tasks;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Synchronization;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
	/// <summary>
	/// <para>Осуществляет передачу данных из входного потока. Требует наличия промежуточного буфера для копирования (он может быть передан через конструктор или создан автоматически).</para>
	/// </summary>
	public class StreamContent : HttpContentBase
	{
		public StreamContent(Stream sourceStream, long length, byte[] buffer)
		{
			Preconditions.EnsureNotNull(sourceStream, "sourceStream");
			Preconditions.EnsureNotNull(buffer, "buffer");
			Preconditions.EnsureCondition(sourceStream.CanRead, "sourceStream", "Source stream is not readable.");
			Preconditions.EnsureCondition(sourceStream.CanSeek, "sourceStream", "Source stream is not seekable.");

			this.sourceStream = sourceStream;
			startPosition = sourceStream.Position;
			this.length = Math.Min(length, sourceStream.Length - startPosition);
			this.buffer = buffer;
            locker = new AsyncLock();
		}

		public StreamContent(Stream sourceStream, long length = long.MaxValue, int bufferSize = DefaultBufferSize)
			: this (sourceStream, length, new byte[bufferSize]) { }

		public override void CopyTo(Stream outputStream)
		{
            using (locker.LockAsync().GetAwaiter().GetResult())
            {
                SeekToStart();
                long totalBytesWritten = 0;
                while (true)
                {
                    if (totalBytesWritten >= length)
                        return;
                    var bytesRead = sourceStream.Read(buffer, 0, (int)Math.Min(buffer.Length, length - totalBytesWritten));
                    if (bytesRead <= 0)
                        return;
                    outputStream.Write(buffer, 0, bytesRead);
                    totalBytesWritten += bytesRead;
                }
            }
		}

		public override async Task CopyToAsync(Stream outputStream)
		{
            using (await locker.LockAsync().ConfigureAwait(false))
            {
                SeekToStart();
                long totalBytesWritten = 0;
                while (true)
                {
                    if (totalBytesWritten >= length)
                        return;
                    int bytesRead = sourceStream.Read(buffer, 0, (int) Math.Min(buffer.Length, length - totalBytesWritten));
                    if (bytesRead <= 0)
                        return;
                    await outputStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                    totalBytesWritten += bytesRead;
                }
            }
		}

		public override string ToString()
		{
			return string.Format("StreamContent: Length = {0}.", length);
		}

		public Stream SourceStream => sourceStream;

	    public override long Length => length;

	    private void SeekToStart()
		{
			sourceStream.Seek(startPosition, SeekOrigin.Begin);
		}

		private readonly Stream sourceStream;
		private readonly long startPosition;
		private readonly long length;
		private readonly byte[] buffer;
	    private readonly IAsyncLock locker;
	    private const int DefaultBufferSize = 16 * 1024;
	}
}