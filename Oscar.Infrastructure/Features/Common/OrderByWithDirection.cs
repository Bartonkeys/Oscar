using System;
namespace Oscar.Infrastructure.Features.Common
{
    public static class ExtensionMethods
    {
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
        (this IEnumerable<TSource> source,
         Func<TSource, TKey> keySelector,
         bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                              : source.OrderBy(keySelector);
        }
    }
}


