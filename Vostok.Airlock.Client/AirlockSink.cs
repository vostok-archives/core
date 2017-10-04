using System.IO;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal class AirlockSink : IAirlockSink
    {
        private readonly BinaryBufferWriter writer;
        private readonly AirlockWriteStream stream;

        public AirlockSink(BinaryBufferWriter writer)
        {
            this.writer = writer;

            stream = new AirlockWriteStream(writer);
        }

        public IBinaryWriter Writer => writer;

        public Stream WriteStream => stream;
    }
}
