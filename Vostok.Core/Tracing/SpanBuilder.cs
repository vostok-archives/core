﻿using System;
using System.Diagnostics;
using Vostok.Airlock;
using Vostok.Commons.Collections;
using Vostok.Flow;

namespace Vostok.Tracing
{
    internal class SpanBuilder : ISpanBuilder
    {
        private const string SpanContextName = "Vostok.Tracing.Span";
        private readonly string airlockRoutingKey;
        private readonly IAirlock airlock;
        private readonly PoolHandle<Span> spanHandle;
        private readonly TraceContextScope contextScope;
        private readonly TraceConfiguration configuration;
        private readonly Stopwatch stopwatch;
        private readonly Span parentSpan;

        public SpanBuilder(
            string operationName,
            string airlockRoutingKey,
            IAirlock airlock,
            PoolHandle<Span> spanHandle,
            TraceContextScope contextScope,
            TraceConfiguration configuration)
        {
            this.airlockRoutingKey = airlockRoutingKey;
            this.airlock = airlock;
            this.spanHandle = spanHandle;
            this.contextScope = contextScope;
            this.configuration = configuration;

            stopwatch = Stopwatch.StartNew();

            parentSpan = Context.Properties.Get<Span>(SpanContextName);

            InitializeSpan(operationName ?? parentSpan?.OperationName);
            EnrichSpanWithContext();
            EnrichSpanFromParentSpan();
            Context.Properties.Set(SpanContextName, Span);
        }

        private void EnrichSpanFromParentSpan()
        {
            if (parentSpan == null)
            {
                return;
            }

            foreach (var inheritableProperty in configuration.InheritableProperties)
            {
                if (!parentSpan.Annotations.ContainsKey(inheritableProperty))
                    continue;

                var annotation = parentSpan.Annotations[inheritableProperty];
                SetAnnotation(inheritableProperty, annotation);
            }
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
                    airlock.Push(airlockRoutingKey, Span);
                }
            }
            finally
            {
                CleanupSpan();
                spanHandle.Dispose();
                contextScope.Dispose();
                Context.Properties.Set(SpanContextName, parentSpan);
            }
        }

        private void InitializeSpan(string operationName)
        {
            Span.OperationName = operationName;
            Span.TraceId = contextScope.Current.TraceId;
            Span.SpanId = contextScope.Current.SpanId;
            Span.ParentSpanId = contextScope.Parent?.SpanId;
            Span.BeginTimestamp = DateTimeOffset.UtcNow; // TODO(iloktionov): придумать что-нибудь поточнее
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
            Span.OperationName = null;
            Span.Annotations.Clear();
        }
    }
}
