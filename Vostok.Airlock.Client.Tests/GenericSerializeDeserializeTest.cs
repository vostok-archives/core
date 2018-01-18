using System;
using System.IO;
using FluentAssertions;
using Vostok.Commons.Binary;

namespace Vostok.Airlock.Client.Tests
{
    internal static class GenericSerializeDeserializeTest
    {
        private class TestAirlockSource : IAirlockSource
        {
            public Stream ReadStream => throw new NotImplementedException();
            public IBinaryReader Reader { get; set; }
        }

        private class TestAirlockSink : IAirlockSink
        {
            public Stream WriteStream => throw new NotImplementedException();
            public IBinaryWriter Writer { get; set; }
        }

        public static void RunTest<T,TSerializer>(T obj) where TSerializer : IAirlockSerializer<T>, IAirlockDeserializer<T>, new()
        {
            var serializer = new TSerializer();
            var bufferWriter = new BinaryBufferWriter(10000);
            var airlockSink = new TestAirlockSink
            {
                Writer = bufferWriter
            };
            serializer.Serialize(obj, airlockSink);

            var airlockSource = new TestAirlockSource
            {
                Reader = new BinaryBufferReader(bufferWriter.Buffer, 0)
            };

            var deserialized = serializer.Deserialize(airlockSource);
            deserialized.ShouldBeEquivalentTo(obj);
        }
    }
}