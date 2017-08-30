using System;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Sending
{
    internal interface IRequestConverter
    {
        [CanBeNull]
        Request TryConvertToAbsolute([NotNull] Request relativeRequest, [NotNull] Uri replica);
    }
}
