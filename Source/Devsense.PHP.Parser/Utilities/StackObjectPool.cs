using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Utilities
{
    static class StackObjectPool<T>
    {
        static readonly ObjectPool<Stack<T>> _pool = new ObjectPool<Stack<T>>(
            () => new Stack<T>(),
            stack => stack.Clear());

        public static Stack<T> Allocate() => _pool.Allocate();

        public static void Free(Stack<T> value)
        {
            if (value != null)
            {
                _pool.Free(value);
            }
        }
    }
}
