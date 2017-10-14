using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// <para>Represents a criterion used to determine the quality of replica responses.</para>
    /// <para>Criteria form a chain which is traversed in order until some criterion returns an <see cref="ResponseVerdict.Accept"/> or <see cref="ResponseVerdict.Reject"/> verdict.</para>
    /// </summary>
    public interface IResponseCriterion
    {
        /// <summary>
        /// <para>Makes a decision about given response quality. See <see cref="ResponseVerdict"/> for more details.</para>
        /// <para>A criterion may return a <see cref="ResponseVerdict.DontKnow"/> verdict when given response is not in it's competence. Next criterion will be used in this case.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        [Pure]
        ResponseVerdict Decide([NotNull] Response response);
    }
}
