using System.Collections.Generic;
using Vostok.Airlock;

namespace Vostok.Tracing
{
    public interface ITraceConfiguration
    {
        ISet<string> ContextFieldsWhitelist { get; }

        IAirlock Airlock { get; set; }
    }
}