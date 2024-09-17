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
            List<T> result = new List<T>();

            foreach (var item in source)
            {
                if (item == null)
                {
                    // Handle null values
                    if (typeof(T).IsClass || Nullable.GetUnderlyingType(typeof(T)) != null)
                    {
                        result.Add(default);
                    }
                }
                else if (typeof(T) == typeof(object))
                {
                    // Handle objects (exclude arrays)
                    if (item.GetType() == typeof(object) && !(item is Array))
                    {
                        result.Add((T)item);
                    }
                }
                else if (typeof(T) == typeof(Array))
                {
                    // Handle arrays
                    if (item is Array)
                    {
                        result.Add((T)item);
                    }
                }
                else if (item is T)
                {
                    // Directly add items that are of the expected type
                    result.Add((T)item);
                }
            }

            return result.ToArray();
        }

        // Helper method to compare values without IComparable
        private static int CompareValues<T, TKey>(T a, T b, Func<T, TKey> keySelector, bool isDescending = false)
        {
            var aValue = keySelector(a);
            var bValue = keySelector(b);

            if (aValue == null && bValue == null) return 0;
            if (aValue == null) return isDescending ? 1 : -1;
            if (bValue == null) return isDescending ? -1 : 1;

            var comparison = Comparer<TKey>.Default.Compare(aValue, bValue);
            return isDescending ? -comparison : comparison;
        }

        public static List<T> OrderBy<T, TKey>(this List<T> list, Func<T, TKey> keySelector)
        {
            list.Sort((a, b) => CompareValues(a, b, keySelector));
            return list;
        }

        public static List<T> OrderByDescending<T, TKey>(this List<T> list, Func<T, TKey> keySelector)
        {
            list.Sort((a, b) => CompareValues(a, b, keySelector, true));
            return list;
        }

        // something is wrong
        public static List<T> ThenBy<T, TKey>(this List<T> list, Func<T, TKey> keySelector)
        {
            list.Sort((a, b) =>
            {
                int result = CompareValues(a, b, keySelector);
                return result != 0 ? result : CompareValues(a, b, keySelector);
            });
            return list;
        }

        // something is wrong
        public static List<T> ThenByDescending<T, TKey>(this List<T> list, Func<T, TKey> keySelector)
        {
            list.Sort((a, b) =>
            {
                int result = CompareValues(a, b, keySelector, true);
                return result != 0 ? result : CompareValues(a, b, keySelector);
            });
            return list;
        }


        public static Dictionary<TKey, List<T>> GroupBy<T, TKey>(this List<T> source, Func<T, TKey> keySelector)
        {
            var result = new Dictionary<TKey, List<T>>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                if (!result.ContainsKey(key))
                {
                    result[key] = new List<T>();
                }
                result[key].Add(item);
            }
            return result;
        }

        public static Dictionary<TKey, List<TElement>> ToLookup<TElement, TKey>(this List<TElement> source, Func<TElement, TKey> keySelector)
        {
            var lookup = new Dictionary<TKey, List<TElement>>();

            foreach (var element in source)
            {
                var key = keySelector(element);

                if (!lookup.ContainsKey(key))
                {
                    lookup[key] = new List<TElement>();
                }

                lookup[key].Add(element);
            }

            return lookup;
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

    }
}
