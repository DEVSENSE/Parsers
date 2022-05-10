using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Ast.DocBlock
{
    /// <summary>
    /// Enumerator of <see cref="IDocEntry"/> list.
    /// </summary>
    public struct DocBlockEntriesEnumerator<T> where T : IDocEntry
    {
        IDocEntry Next { get; set; }

        /// <summary>
        /// Gets the current enumerator entry. Cannot be <c>null</c>.
        /// </summary>
        public T/*!*/Current { get; private set; }

        //public int Index { get; private set; }

        //readonly Func<IDocEntry, bool> _predicate;

        public bool MoveNext()
        {
            for (var item = this.Next; item != null; item = item.Next)
            {
                if (item is T)
                {
                    this.Current = (T)item;
                    this.Next = item.Next;
                    return true;
                }
            }

            //
            return false;
        }

        public DocBlockEntriesEnumerator(IDocEntry head/*, Func<IDocEntry, bool> predicate*/)
        {
            this.Current = default(T);
            this.Next = head;
            //_predicate = predicate;
        }
    }
}
