using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Utilities
{
    static class StackObjectPool<T>
    {
        static readonly ObjectPool<Stack<T>> s_pool = new ObjectPool<Stack<T>>(
            () => new Stack<T>(),
            stack => stack.Clear()
        );

        public static Stack<T> Allocate() => s_pool.Allocate();

        public static void Free(Stack<T> value)
        {
            if (value != null)
            {
                s_pool.Free(value);
            }
        }
    }
}
