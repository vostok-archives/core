using System.Collections.Generic;

namespace Vostok.Airlock
{
    internal interface IDataBatchesFactory
    {
        IEnumerable<IDataBatch> CreateBatches();
    }
}