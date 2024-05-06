using System;
using System.Collections.Generic;

namespace MittaUI.Runtime.Extension
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> sourceT, Action<T> action)
        {
            foreach (var st in sourceT) action(st);
        }
    }
}