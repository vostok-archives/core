using System;
using System.Collections.Generic;

namespace Vostok.RetriableCall
{
    public class ExceptionComparer : IComparer<Exception>
    {
        public int Compare(Exception e1, Exception e2)
        {
            var cmp = string.CompareOrdinal(e1.Message, e2.Message);
            return cmp != 0 ? cmp : string.CompareOrdinal(e1.StackTrace, e2.StackTrace);
        }
    }
}