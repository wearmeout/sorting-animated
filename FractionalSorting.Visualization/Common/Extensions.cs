using System.Collections.Generic;

namespace FractionalSorting.Visualization.Common
{
    public static class Extensions
    {
        public static int FindIndex<T>(this IReadOnlyList<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
                if (object.Equals(list[i], item))
                    return i;

            return -1;
        }
    }
}
