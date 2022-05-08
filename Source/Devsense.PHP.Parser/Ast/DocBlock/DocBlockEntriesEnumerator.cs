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
        IDocEntry _next;

        public IDocEntry Current { get; private set; }

        //public int Index { get; private set; }

        //readonly Func<IDocEntry, bool> _predicate;

        public bool MoveNext()
        {
            if (_next != null)
            {
                _next = (this.Current = _next).Next;
                return Current is T ? true : MoveNext();
            }
            else
            {
                return false;
            }
        }

        public DocBlockEntriesEnumerator(IDocEntry head/*, Func<IDocEntry, bool> predicate*/)
        {
            this.Current = null;
            _next = head;
            //_predicate = predicate;
        }
    }
}
