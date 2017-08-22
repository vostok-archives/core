using System;
using Vostok.Flow;

namespace Vostok.Tracing
{
    public class TraceContext
    {
        public const string TraceIdContextName = "Vostok.Tracing.TraceId";
        public const string SpanIdContextName = "Vostok.Tracing.SpanId";

        static TraceContext()
        {
            Context.Configuration.DistributedProperties.Add(TraceIdContextName);
            Context.Configuration.DistributedProperties.Add(SpanIdContextName);
        }

        public static IDisposable Use(TraceContext context)
        {
            return new TraceContextScope(context);
        }

        public static IDisposable Disable()
        {
            return Use(null);
        }

        public TraceContext(Guid traceId, Guid spanId)
        {
            TraceId = traceId;
            SpanId = spanId;
        }

        public Guid TraceId { get; }
        public Guid SpanId { get; }

        public static TraceContext Current
        {
            get
            {
                var properties = Context.Properties;

                var traceId = properties.Get<Guid>(TraceIdContextName);
                if (traceId == default(Guid))
                    return null;

                var spanId = properties.Get<Guid>(SpanIdContextName);
                if (spanId == default(Guid))
                    return null;

                return new TraceContext(traceId, spanId);
            }
            set
            {
                var properties = Context.Properties;

                if (value == null)
                {
                    properties.RemoveProperty(TraceIdContextName);
                    properties.RemoveProperty(SpanIdContextName);
                }
                else
                {
                    properties.Set(TraceIdContextName, value.TraceId);
                    properties.Set(SpanIdContextName, value.SpanId);
                }
            }
        }
    }
}
