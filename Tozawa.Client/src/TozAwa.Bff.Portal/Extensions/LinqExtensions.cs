using System;
using System.Collections.Generic;
using System.Linq;

namespace Tozawa.Bff.Portal.extension
{
    public static class LinqExtensions
    {
        [Obsolete("Method is included in .net6")]
        public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
    public static class EnumerableExtensions
    {
        public static bool ContainsAtLeastOne<T>(this IEnumerable<T> list, IEnumerable<T> secondList)
        {
            return list.Any(secondList.Contains);
        }

        public static IEnumerable<T> Matching<T>(this IEnumerable<T> list, IEnumerable<T> secondList)
        {
            return list.Where(secondList.Contains);
        }

        public static bool AllMatching<T>(this IEnumerable<T> list, IEnumerable<T> secondList)
        {
            return list.All(secondList.Contains);
        }

        public static bool CompleteMatch<T>(this IEnumerable<T> list, IEnumerable<T> secondList)
        {
            var first = list as T[] ?? list.ToArray();
            var second = secondList as T[] ?? secondList.ToArray();
            return first.Length == second.Length && first.AllMatching(second);
        }
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>
            (this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct =
                new[] { Enumerable.Empty<T>() };
            IEnumerable<IEnumerable<T>> result = emptyProduct;
            foreach (IEnumerable<T> sequence in sequences)
            {
                result = from accseq in result from item in sequence select accseq.Concat(new[] { item });
            }
            return result;
        }

        public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}