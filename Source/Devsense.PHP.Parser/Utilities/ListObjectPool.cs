using System;
using System.Collections.Generic;
using System.Text;
using Devsense.PHP.Syntax;

namespace Devsense.PHP.Utilities
{
    static class ListObjectPool<T>
    {
        static readonly ObjectPool<List<T>> _pool = new ObjectPool<List<T>>(
            () => new List<T>(),
            list => list.Clear()
            );

        public static List<T> Allocate()
        {
            return _pool.Allocate();
        }

        public static void Free(List<T> value)
        {
            if (value != null)
            {
                _pool.Free(value);
            }
        }

        /// <summary>
        /// Gets new array of items in the list and frees the list into the pool.
        /// </summary>
        /// <param name="value">List to free, can be null.</param>
        /// <returns>Array of items in the list. Cannot be <c>null</c>.</returns>
        public static T[] GetArrayAndFree(List<T> value)
        {
            T[] array;

            if (value != null && value.Count != 0)
            {
                array = value.ToArray();
            }
            else
            {
                array = EmptyArray<T>.Instance;
            }

            Free(value);

            return array;
        }
    }
}
