namespace LinqFramework.Extention
{
    public static class Linq
    {
        public static T[] Where<T>(this T[] source , Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach ( var item in source )
            {
                if ( predicate(item) )
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

        
    }
}
