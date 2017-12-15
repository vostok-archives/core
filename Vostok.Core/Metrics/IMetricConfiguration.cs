using System;
using System.Collections.Generic;
using Vostok.Flow;

namespace Vostok.Metrics
{
    public interface IMetricConfiguration
    {
        /// <summary>
        /// Fields to be added as tags from current <see cref="Context"/>
        /// </summary>
        ISet<string> ContextFieldsWhitelist { get; }

        IMetricEventReporter Reporter { get; set; }

        TimeSpan DefaultInterval { get; set; } 
    }
}