using System.IO;
using System.Threading.Tasks;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
	/// <summary>
	/// <para>Осуществляет передачу данных из файла. Требует наличия промежуточного буфера для копирования (он может быть передан через конструктор или создан автоматически).</para>
	/// </summary>
	public class FileContent : StreamContent
	{
		public FileContent(string filePath, byte[] buffer)
			: base (OpenStream(filePath), long.MaxValue, buffer) { }

		public FileContent(string filePath)
			: base (OpenStream(filePath), long.MaxValue) { }

		public override async Task CopyToAsync(Stream outputStream)
		{
			using (SourceStream)
				await base.CopyToAsync(outputStream).ConfigureAwait(false);
		}

		public override void CopyTo(Stream outputStream)
		{
			using (SourceStream)
				base.CopyTo(outputStream);
		}

		private static FileStream OpenStream(string filePath)
		{
			return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
	}
}