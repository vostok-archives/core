using System;
using Vostok.Airlock;

namespace Vostok.Tracing
{
    internal class SpanAirlockSerializer : IAirlockSerializer<Span>
    {
        public void Serialize(Span item, IAirlockSink sink)
        {
            throw new NotImplementedException();
        }
    }
}