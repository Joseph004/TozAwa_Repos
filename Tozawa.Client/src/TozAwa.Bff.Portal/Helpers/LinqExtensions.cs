using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Tozawa.Bff.Portal.Helpers
{
    public static class LinqExtensions
    {
        public static IOrderedEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            var enumerable = source.ToList();
            int max = GetMax(enumerable, selector);
            return enumerable.OrderBy(i => AlphaNumericComparer(selector, i, max));
        }

        public static IOrderedEnumerable<T> ThenByAlphaNumeric<T>(this IOrderedEnumerable<T> source, Func<T, string> selector)
        {
            int max = GetMax(source, selector);
            return source.ThenBy(i => AlphaNumericComparer(selector, i, max));
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };

            IEnumerable<IEnumerable<T>> result = emptyProduct;
            foreach (IEnumerable<T> sequence in sequences)
            {
                result = from accseq in result from item in sequence select accseq.Concat(new[] { item });
            }
            return result;
        }

        private static int GetMax<T>(IEnumerable<T> source, Func<T, string> selector)
        {
            return source
            .SelectMany(i => Regex.Matches(selector(i), @"\d+").Select(m => (int?)m.Value.Length))
            .Max() ?? 0;
        }

        private static string AlphaNumericComparer<T>(Func<T, string> selector, T i, int max)
        {
            return Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0'));
        }
    }

}
