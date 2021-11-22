using System;
using System.Collections.Generic;
using MultiMergeShared;

namespace MultiMerge
{
    static class Extensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// AsDynamic will let you get and set private properties and fields of an Instance
        /// </summary>
        /// <param name="o">Instance</param>
        /// <returns></returns>
        public static dynamic AsDynamic(this object o)
        {
            return PrivateDynamicObject.WrapObjectIfNeeded(o);
        }

        /// <summary>
        /// AsDynamic will let you get and set private/internal static properties and fields of an Instance
        /// </summary>
        /// <param name="o">Instance</param>
        /// <returns></returns>
        public static dynamic AsStaticDynamic(this object o)
        {
            return PrivateDynamicObject.WrapObjectIfNeeded(null, o.GetType());
        }
    }
}
