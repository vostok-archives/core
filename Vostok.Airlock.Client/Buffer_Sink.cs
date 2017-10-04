using System.IO;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal partial class Buffer
    {
        private volatile AirlockWriteStream writeStream;

        public Stream WriteStream => writeStream ?? (writeStream = new AirlockWriteStream(this));

        public IBinaryWriter Writer => this;
    }
}
