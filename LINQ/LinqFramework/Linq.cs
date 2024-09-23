using System.Collections;

namespace LinqFramework
{
    public static class Linq
    {
        public static T[] Where<T>(this T[] source, Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result.ToArray();
        }

        public static int Count<T>(this T[] source, Func<T, bool> predicate = null)
        {
            var result = predicate != null ? source.Where(predicate).Length : source.Length;
            return result;
        }

        public static T First<T>(this T[] source, Func<T, bool> predicate = null)
        {
            if (predicate != null)
            {
                foreach (var item in source)
                {
                    if (predicate(item)) return item;
                }
                return default;
            }
            return source.Length > 0 ? source[0] : default;
        }

        public static T[] Select<TSource, T>(this TSource[] source, Func<TSource, T> selector)
        {
            var result = new List<T>();
            foreach (var item in source)
            {
                result.Add(selector(item));
            }
            return result.ToArray();
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
            var innerLookup = new Dictionary<TKey, List<TInner>>();

            // Tạo lookup cho danh sách inner
            foreach (var innerItem in inner)
            {
                var key = innerKeySelector(innerItem);
                if (!innerLookup.ContainsKey(key))
                {
                    innerLookup[key] = new List<TInner>();
                }
                innerLookup[key].Add(innerItem);
            }

            var results = new List<TResult>();

            // Duyệt qua danh sách outer và lấy kết quả từ lookup
            foreach (var outerItem in outer)
            {
                var outerKey = outerKeySelector(outerItem);
                if (innerLookup.TryGetValue(outerKey, out var matchingInners))
                {
                    foreach (var innerItem in matchingInners)
                    {
                        results.Add(resultSelector(outerItem, innerItem));
                    }
                }
            }

            return results;
        }

    }
}
