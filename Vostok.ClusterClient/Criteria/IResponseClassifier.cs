using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    internal interface IResponseClassifier
    {
        [Pure]
        ResponseVerdict Decide([NotNull] Response response, [NotNull] IList<IResponseCriterion> criteria);
    }
}
