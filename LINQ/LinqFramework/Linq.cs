using System.Collections;

namespace LinqFramework
{
    public static class Linq
    {
        public static List<T> Where<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static int Count<T>(this List<T> source, Func<T, bool> predicate = null)
        {
            var result = predicate != null ? source.Where(predicate).Count : source.Count;
            return result;
        }

        public static List<T> First<T>(this List<T> source, Func<T, bool> predicate = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var result = new List<T>();

            if (predicate != null)
            {
                foreach (var item in source)
                    if (predicate(item))
                    {
                        result.Add(item);
                        break; 
                    }
            }
            else if (source.Count > 0)
                result.Add(source[0]);

            return result.Count > 0 ? result : new List<T>();
        }

        public static List<T> Select<TSource, T>(this List<TSource> source, Func<TSource, T> selector)
        {
            var result = new List<T>();
            foreach (var item in source)
            {
                result.Add(selector(item));
            }
            return result;
        }

        public static T[] OfType<T>(this object[] source)
        {
            ArgumentNullException.ThrowIfNull(source);

            List<T> result = [];

            foreach (var item in source)
            {
                if (item is T output)
                {
                    result.Add(output);
                }
            }
            return result.ToArray();
        }


        public static List<T> OrderBy<T, TKey>(this List<T> source, Func<T, TKey> keySelector)
        {
            return Sort(source, keySelector, ascending: true);
        }

        public static List<T> OrderByDescending<T, TKey>(this List<T> source, Func<T, TKey> keySelector)
        {
            return Sort(source, keySelector, ascending: false);
        }

        public static List<T> ThenBy<T, TKey>(this List<T> source, Func<T, TKey> keySelector)
        {
            return ThenBy(source, keySelector, ascending: true);
        }

        public static List<T> ThenByDescending<T, TKey>(this List<T> source, Func<T, TKey> keySelector)
        {
            return ThenBy(source, keySelector, ascending: false);
        }

        private static List<T> Sort<T, TKey>(List<T> source, Func<T, TKey> keySelector, bool ascending)
        {
            var sortedList = new List<T>(source);
            sortedList.Sort((x, y) =>
            {
                int comparison = Comparer<TKey>.Default.Compare(keySelector(x), keySelector(y));
                return ascending ? comparison : -comparison;
            });
            return sortedList;
        }

        private static List<T> ThenBy<T, TKey>(List<T> source, Func<T, TKey> keySelector, bool ascending)
        {
            var sortedList = new List<T>(source);
            sortedList.Sort((x, y) =>
            {
                int primaryComparison = Comparer<TKey>.Default.Compare(keySelector(x), keySelector(y));
                if (primaryComparison != 0)
                {
                    return primaryComparison;
                }

                return 0;
            });

            return sortedList;
        }


        public static Dictionary<TKey, List<TSource>> GroupBy<TSource, TKey>(this List<TSource> source, Func<TSource, TKey> keySelector)
        {
            var result = new Dictionary<TKey, List<TSource>>();

            foreach (var item in source)
            {
                var key = keySelector(item);
                if (!result.ContainsKey(key))
                {
                    result[key] = new List<TSource>();
                }
                result[key].Add(item);
            }
            return result;
        }

        public static Dictionary<TKey, List<TSource>> ToLookup<TSource, TKey>(this List<TSource> source, Func<TSource, TKey> keySelector)
        {
            return GroupBy(source, keySelector);
        }

        public static List<T> DistinctBy<T, TKey>(this List<T> source, Func<T, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            var distinctList = new List<T>();

            foreach (var item in source)
            {
                var key = keySelector(item);

                if (seenKeys.Add(key))
                {
                    distinctList.Add(item);
                }
            }

            return distinctList;
        }

        public static int Sum<T>(this List<T> list, Func<T, int> selector)
        {
            int sum = 0;
            foreach (var item in list)
            {
                sum += selector(item);
            }
            return sum;
        }

        public static int Min<T>(this List<T> list, Func<T, int> selector)
        {
            int min = int.MaxValue;
            foreach (var item in list)
            {
                int value = selector(item);
                if (value < min)
                {
                    min = value;
                }
            }
            return min;
        }

        public static int Max<T>(this List<T> list, Func<T, int> selector)
        {
            int max = int.MinValue;
            foreach (var item in list)
            {
                int value = selector(item);
                if (value > max)
                {
                    max = value;
                }
            }
            return max;
        }

        public static double Average<T>(this List<T> list, Func<T, int> selector)
        {
            return (double)list.Sum(selector) / list.Count;
        }

        public static bool All<T>(this List<T> list, Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Any<T>(this List<T> list, Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool contains<T>(this List<T> list, T item)
        {
            foreach (var element in list)
            {
                if (EqualityComparer<T>.Default.Equals(element, item))
                {
                    return true;
                }
            }
            return false;
        }

        public static T ElementAt<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return list[index];
        }

        public static T ElementAtOrDefault<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
            {
                return default;
            }
            return list[index];
        }

        public static T FirstOrDefault<T>(this List<T> list, Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default;
        }

        public static T LastOrDefault<T>(this List<T> list, Func<T, bool> predicate)
        {
            T lastMatch = default;
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    lastMatch = item;
                }
            }
            return lastMatch;
        }

        public static T Last<T>(this List<T> list, Func<T, bool> predicate = null)
        {
            T lastMatch = default;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (predicate == null || predicate(list[i]))
                {
                    return list[i];
                }
            }
            return lastMatch;
        }

        public static List<TResult> InnerJoin<TOuter, TInner, TKey, TResult>(
            this List<TOuter> outer,
            List<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            var innerLookup = inner.ToLookup(innerKeySelector);
            var results = new List<TResult>();

            foreach (var outerItem in outer)
            {
                var outerKey = outerKeySelector(outerItem);
                if (innerLookup.TryGetValue(outerKey, out var innerItems))
                {
                    var selectedResults = innerItems.Select(innerItem => resultSelector(outerItem, innerItem));
                    results.AddRange(selectedResults);
                }
            }

            return results;
        }

        public static List<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
            this List<TOuter> outer,
            List<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            var innerLookup = inner.ToLookup(innerKeySelector);
            var results = new List<TResult>();

            foreach (var outerItem in outer)
            {
                var outerKey = outerKeySelector(outerItem);
                if (innerLookup.TryGetValue(outerKey, out var innerItems))
                {
                    var selectedResults = innerItems.Select(innerItem => resultSelector(outerItem, innerItem));
                    results.AddRange(selectedResults);
                }
                else
                {
                    results.Add(resultSelector(outerItem, default!)); // No matching inner, add default inner
                }
            }
            return results;
        }

    }
}
