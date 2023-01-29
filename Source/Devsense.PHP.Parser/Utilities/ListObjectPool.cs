using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
