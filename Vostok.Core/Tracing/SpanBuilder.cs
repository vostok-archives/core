using System;
using System.Diagnostics;
using Vstk.Commons.Collections;
using Vstk.Flow;

namespace Vstk.Tracing
{
    internal class SpanBuilder : ISpanBuilder
    {
        private const string spanContextKey = "Vostok.Tracing.CurrentSpan";

        private readonly ITraceReporter traceReporter;
        private readonly PoolHandle<Span> spanHandle;
        private readonly TraceContextScope contextScope;
        private readonly ITraceConfiguration configuration;
        private readonly Stopwatch stopwatch;
        private readonly Span parentSpan;
        private readonly IDisposable spanContextScope;

        public SpanBuilder(
            ITraceReporter traceReporter,
            PoolHandle<Span> spanHandle,
            TraceContextScope contextScope,
            ITraceConfiguration configuration)
        {
            this.traceReporter = traceReporter;
            this.spanHandle = spanHandle;
            this.contextScope = contextScope;
            this.configuration = configuration;

            stopwatch = Stopwatch.StartNew();
            parentSpan = Context.Properties.Get<Span>(spanContextKey);
            spanContextScope = Context.Properties.Use(spanContextKey, Span);

            InitializeSpan();
            EnrichSpanWithInheritedFields();
            EnrichSpanWithContext();
        }

        public bool IsCanceled { get; set; } = false;

        public bool IsEndless { get; set; } = false;

        private Span Span => spanHandle;

        public void SetAnnotation<TValue>(string key, TValue value)
        {
            Span.Annotations[key] = value?.ToString() ?? string.Empty;
        }

        public void SetBeginTimestamp(DateTimeOffset timestamp)
        {
            Span.BeginTimestamp = timestamp;
        }

        public void SetEndTimestamp(DateTimeOffset timestamp)
        {
            Span.EndTimestamp = timestamp;
        }

        // TODO(iloktionov): Тут есть неявное предположение о том, что реализация эйрлока не использует спан после завершения Push().
        // TODO(iloktionov): Его надо как-то получше обосновать, потому что иначе использовать пул просто небезопасно.
        public void Dispose()
        {
            try
            {
                if (!IsCanceled)
                {
                    FinalizeSpan();
                    traceReporter.SendSpan(Span);
                }
            }
            finally
            {
                CleanupSpan();
                spanHandle.Dispose();
                spanContextScope.Dispose();
                contextScope.Dispose();
            }
        }

        private void InitializeSpan()
        {
            Span.TraceId = contextScope.Current.TraceId;
            Span.SpanId = contextScope.Current.SpanId;
            Span.ParentSpanId = contextScope.Parent?.SpanId;
            Span.BeginTimestamp = DateTimeOffset.UtcNow; // TODO(iloktionov): придумать что-нибудь поточнее
        }

        private void EnrichSpanWithInheritedFields()
        {
            if (parentSpan == null)
                return;

            foreach (var field in configuration.InheritedFieldsWhitelist)
            {
                if (parentSpan.Annotations.TryGetValue(field, out var value))
                    SetAnnotation(field, value);
            }
        }

        private void EnrichSpanWithContext()
        {
            foreach (var pair in Context.Properties.Current)
            {
                if (configuration.ContextFieldsWhitelist.Contains(pair.Key))
                    SetAnnotation(pair.Key, pair.Value);
            }
        }

        private void FinalizeSpan()
        {
            Span.EndTimestamp = IsEndless ? null as DateTimeOffset? : Span.BeginTimestamp + stopwatch.Elapsed;
        }

        private void CleanupSpan()
        {
            Span.Annotations.Clear();
        }
    }
}
