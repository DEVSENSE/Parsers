﻿using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Devsense.PHP.Text;

namespace Devsense.PHP.Utilities
{
    /// <summary>
    /// This is basically a lossy cache of strings that is searchable by
    /// strings, string sub ranges, character array ranges or string-builder.
    /// </summary>
    public class StringTable : IStringTable
    {
        // entry in the caches
        [DebuggerDisplay("{Text}")]
        private struct Entry
        {
            // hash code of the entry
            public int HashCode;

            // full text of the item
            public string Text;
        }

        // TODO: Need to tweak the size with more scenarios.
        //       for now this is what works well enough with 
        //       Roslyn C# compiler project

        // Size of local cache.
        private const int LocalSizeBits = 8;
        private const int LocalSize = (1 << LocalSizeBits);
        private const int LocalSizeMask = LocalSize - 1;

        // max size of shared cache.
        private const int SharedSizeBits = 8;
        private const int SharedSize = (1 << SharedSizeBits);
        private const int SharedSizeMask = SharedSize - 1;

        // size of bucket in shared cache. (local cache has bucket size 1).
        private const int SharedBucketBits = 4;
        private const int SharedBucketSize = (1 << SharedBucketBits);
        private const int SharedBucketSizeMask = SharedBucketSize - 1;

        // local (L1) cache
        // simple fast and not threadsafe cache 
        // with limited size and "last add wins" expiration policy
        //
        // The main purpose of the local cache is to use in long lived
        // single threaded operations with lots of locality (like parsing).
        // Local cache is smaller (and thus faster) and is not affected
        // by cache misses on other threads.
        private readonly Entry[] _localTable = new Entry[LocalSize];

        // shared (L2) threadsafe cache
        // slightly slower than local cache
        // we read this cache when having a miss in local cache
        // writes to local cache will update shared cache as well.
        private static readonly Entry[] s_sharedTable = new Entry[SharedSize];

        // essentially a random number 
        // the usage pattern will randomly use and increment this
        // the counter is not static to avoid interlocked operations and cross-thread traffic
        private int _localRandom = Environment.TickCount;

        // same as above but for users that go directly with unbuffered shared cache.
        private static int s_sharedRandom = Environment.TickCount;

        //internal StringTable() :
        //    this(null)
        //{
        //}

        // implement Poolable object pattern
        #region "Poolable"

        private StringTable(ObjectPool<StringTable> pool)
        {
            _pool = pool;
        }

        private readonly ObjectPool<StringTable> _pool;
        private static readonly ObjectPool<StringTable> s_staticPool = CreatePool();

        private static ObjectPool<StringTable> CreatePool()
        {
            ObjectPool<StringTable> pool = null;
            pool = new ObjectPool<StringTable>(() => new StringTable(pool), Environment.ProcessorCount * 2);
            return pool;
        }

        public static StringTable GetInstance()
        {
            return s_staticPool.Allocate();
        }

        public void Free()
        {
            // leave cache content in the cache, just return it to the pool
            // Array.Clear(this.localTable, 0, this.localTable.Length);
            // Array.Clear(sharedTable, 0, sharedTable.Length);

            _pool.Free(this);
        }

        #endregion // Poolable

        string IStringTable.GetOrAdd(ReadOnlySpan<char> text) => Add(text);

        public string Add(ReadOnlySpan<char> chars)
        {
            if (chars.IsEmpty)
            {
                return string.Empty;
            }

            var hashCode = Hash.GetFNVHashCode(chars);

            // capture array to avoid extra range checks
            var arr = _localTable;
            var idx = LocalIdxFromHash(hashCode);

            var text = arr[idx].Text;

            if (text != null && arr[idx].HashCode == hashCode)
            {
                var result = arr[idx].Text;
                if (StringTable.TextEquals(result, chars))
                {
                    return result;
                }
            }

            string shared = FindSharedEntry(chars, hashCode);
            if (shared != null)
            {
                // PERF: the following code does element-wise assignment of a struct
                //       because current JIT produces better code compared to
                //       arr[idx] = new Entry(...)
                arr[idx].HashCode = hashCode;
                arr[idx].Text = shared;

                return shared;
            }

            return AddItem(chars, hashCode);
        }

        public string Add(char[] chars, int start, int len)
        {
            var hashCode = Hash.GetFNVHashCode(chars, start, len);

            // capture array to avoid extra range checks
            var arr = _localTable;
            var idx = LocalIdxFromHash(hashCode);

            var text = arr[idx].Text;

            if (text != null && arr[idx].HashCode == hashCode)
            {
                var result = arr[idx].Text;
                if (StringTable.TextEquals(result, chars, start, len))
                {
                    return result;
                }
            }

            string shared = FindSharedEntry(chars, start, len, hashCode);
            if (shared != null)
            {
                // PERF: the following code does element-wise assignment of a struct
                //       because current JIT produces better code compared to
                //       arr[idx] = new Entry(...)
                arr[idx].HashCode = hashCode;
                arr[idx].Text = shared;

                return shared;
            }

            return AddItem(chars, start, len, hashCode);
        }

        public string Add(string chars, int start, int len)
        {
            var hashCode = Hash.GetFNVHashCode(chars, start, len);

            // capture array to avoid extra range checks
            var arr = _localTable;
            var idx = LocalIdxFromHash(hashCode);

            var text = arr[idx].Text;

            if (text != null && arr[idx].HashCode == hashCode)
            {
                var result = arr[idx].Text;
                if (StringTable.TextEquals(result, chars, start, len))
                {
                    return result;
                }
            }

            string shared = FindSharedEntry(chars, start, len, hashCode);
            if (shared != null)
            {
                // PERF: the following code does element-wise assignment of a struct
                //       because current JIT produces better code compared to
                //       arr[idx] = new Entry(...)
                arr[idx].HashCode = hashCode;
                arr[idx].Text = shared;

                return shared;
            }

            return AddItem(chars, start, len, hashCode);
        }

        public string Add(char chars)
        {
            var hashCode = Hash.GetFNVHashCode(chars);

            // capture array to avoid extra range checks
            var arr = _localTable;
            var idx = LocalIdxFromHash(hashCode);

            var text = arr[idx].Text;

            if (text != null)
            {
                var result = arr[idx].Text;
                if (text.Length == 1 && text[0] == chars)
                {
                    return result;
                }
            }

            string shared = FindSharedEntry(chars, hashCode);
            if (shared != null)
            {
                // PERF: the following code does element-wise assignment of a struct
                //       because current JIT produces better code compared to
                //       arr[idx] = new Entry(...)
                arr[idx].HashCode = hashCode;
                arr[idx].Text = shared;

                return shared;
            }

            return AddItem(chars, hashCode);
        }

        public string Add(StringBuilder chars)
        {
            var hashCode = Hash.GetFNVHashCode(chars);

            // capture array to avoid extra range checks
            var arr = _localTable;
            var idx = LocalIdxFromHash(hashCode);

            var text = arr[idx].Text;

            if (text != null && arr[idx].HashCode == hashCode)
            {
                var result = arr[idx].Text;
                if (StringTable.TextEquals(result, chars))
                {
                    return result;
                }
            }

            string shared = FindSharedEntry(chars, hashCode);
            if (shared != null)
            {
                // PERF: the following code does element-wise assignment of a struct
                //       because current JIT produces better code compared to
                //       arr[idx] = new Entry(...)
                arr[idx].HashCode = hashCode;
                arr[idx].Text = shared;

                return shared;
            }

            return AddItem(chars, hashCode);
        }

        public string Add(string chars)
        {
            var hashCode = Hash.GetFNVHashCode(chars);

            // capture array to avoid extra range checks
            var arr = _localTable;
            var idx = LocalIdxFromHash(hashCode);

            var text = arr[idx].Text;

            if (text != null && arr[idx].HashCode == hashCode)
            {
                var result = arr[idx].Text;
                if (result == chars)
                {
                    return result;
                }
            }

            string shared = FindSharedEntry(chars, hashCode);
            if (shared != null)
            {
                // PERF: the following code does element-wise assignment of a struct
                //       because current JIT produces better code compared to
                //       arr[idx] = new Entry(...)
                arr[idx].HashCode = hashCode;
                arr[idx].Text = shared;

                return shared;
            }

            AddCore(chars, hashCode);
            return chars;
        }

        private static string FindSharedEntry(char[] chars, int start, int len, int hashCode) => FindSharedEntry(chars.AsSpan(start, len), hashCode);

        private static string FindSharedEntry(ReadOnlySpan<char> chars, int hashCode)
        {
            var arr = s_sharedTable;
            int idx = SharedIdxFromHash(hashCode);

            string e = null;
            // we use quadratic probing here
            // bucket positions are (n^2 + n)/2 relative to the masked hashcode
            for (int i = 1; i < SharedBucketSize + 1; i++)
            {
                e = arr[idx].Text;
                int hash = arr[idx].HashCode;

                if (e != null)
                {
                    if (hash == hashCode && TextEquals(e, chars))
                    {
                        break;
                    }

                    // this is not e we are looking for
                    e = null;
                }
                else
                {
                    // once we see unfilled entry, the rest of the bucket will be empty
                    break;
                }

                idx = (idx + i) & SharedSizeMask;
            }

            return e;
        }

        private static string FindSharedEntry(string chars, int start, int len, int hashCode) => FindSharedEntry(chars.AsSpan(start, len), hashCode);

        private static string FindSharedEntry(char chars, int hashCode)
        {
            var arr = s_sharedTable;
            int idx = SharedIdxFromHash(hashCode);

            string e = null;
            // we use quadratic probing here
            // bucket positions are (n^2 + n)/2 relative to the masked hashcode
            for (int i = 1; i < SharedBucketSize + 1; i++)
            {
                e = arr[idx].Text;

                if (e != null)
                {
                    if (e.Length == 1 && e[0] == chars)
                    {
                        break;
                    }

                    // this is not e we are looking for
                    e = null;
                }
                else
                {
                    // once we see unfilled entry, the rest of the bucket will be empty
                    break;
                }

                idx = (idx + i) & SharedSizeMask;
            }

            return e;
        }

        private static string FindSharedEntry(StringBuilder chars, int hashCode)
        {
            var arr = s_sharedTable;
            int idx = SharedIdxFromHash(hashCode);

            string e = null;
            // we use quadratic probing here
            // bucket positions are (n^2 + n)/2 relative to the masked hashcode
            for (int i = 1; i < SharedBucketSize + 1; i++)
            {
                e = arr[idx].Text;
                int hash = arr[idx].HashCode;

                if (e != null)
                {
                    if (hash == hashCode && TextEquals(e, chars))
                    {
                        break;
                    }

                    // this is not e we are looking for
                    e = null;
                }
                else
                {
                    // once we see unfilled entry, the rest of the bucket will be empty
                    break;
                }

                idx = (idx + i) & SharedSizeMask;
            }

            return e;
        }

        private static string FindSharedEntry(string chars, int hashCode)
        {
            var arr = s_sharedTable;
            int idx = SharedIdxFromHash(hashCode);

            string e = null;
            // we use quadratic probing here
            // bucket positions are (n^2 + n)/2 relative to the masked hashcode
            for (int i = 1; i < SharedBucketSize + 1; i++)
            {
                e = arr[idx].Text;
                int hash = arr[idx].HashCode;

                if (e != null)
                {
                    if (hash == hashCode && e == chars)
                    {
                        break;
                    }

                    // this is not e we are looking for
                    e = null;
                }
                else
                {
                    // once we see unfilled entry, the rest of the bucket will be empty
                    break;
                }

                idx = (idx + i) & SharedSizeMask;
            }

            return e;
        }


        private string AddItem(char[] chars, int start, int len, int hashCode) => AddItem(chars.AsSpan(start, len), hashCode);

        private string AddItem(ReadOnlySpan<char> chars, int hashCode)
        {
            var text = chars.ToString();
            AddCore(text, hashCode);
            return text;
        }

        private string AddItem(string chars, int start, int len, int hashCode) => AddItem(chars.AsSpan(start, len), hashCode);

        private string AddItem(char chars, int hashCode)
        {
            var text = new String(chars, 1);
            AddCore(text, hashCode);
            return text;
        }

        private string AddItem(StringBuilder chars, int hashCode)
        {
            var text = chars.ToString();
            AddCore(text, hashCode);
            return text;
        }

        private void AddCore(string chars, int hashCode)
        {
            // add to the shared table first (in case someone looks for same item)
            AddSharedEntry(hashCode, chars);

            // add to the local table too
            var arr = _localTable;
            var idx = LocalIdxFromHash(hashCode);
            arr[idx].HashCode = hashCode;
            arr[idx].Text = chars;
        }

        private void AddSharedEntry(int hashCode, string text)
        {
            var arr = s_sharedTable;
            int idx = SharedIdxFromHash(hashCode);

            // try finding an empty spot in the bucket
            // we use quadratic probing here
            // bucket positions are (n^2 + n)/2 relative to the masked hashcode
            int curIdx = idx;
            for (int i = 1; i < SharedBucketSize + 1; i++)
            {
                if (arr[curIdx].Text == null)
                {
                    idx = curIdx;
                    goto foundIdx;
                }

                curIdx = (curIdx + i) & SharedSizeMask;
            }

            // or pick a random victim within the bucket range
            // and replace with new entry
            var i1 = LocalNextRandom() & SharedBucketSizeMask;
            idx = (idx + ((i1 * i1 + i1) / 2)) & SharedSizeMask;

        foundIdx:
            arr[idx].HashCode = hashCode;
            Volatile.Write(ref arr[idx].Text, text);
        }

        public static string AddShared(StringBuilder chars)
        {
            var hashCode = Hash.GetFNVHashCode(chars);

            return FindSharedEntry(chars, hashCode) ?? AddSharedSlow(hashCode, chars);
        }

        public static string AddShared(ReadOnlySpan<char> chars)
        {
            var hashCode = Hash.GetFNVHashCode(chars);

            return FindSharedEntry(chars, hashCode) ?? AddSharedSlow(hashCode, chars.ToString());
        }

        private static string AddSharedSlow(int hashCode, StringBuilder builder)
        {
            return AddSharedSlow(hashCode, builder.ToString());
        }

        private static string AddSharedSlow(int hashCode, string text)
        {
            var arr = s_sharedTable;
            int idx = SharedIdxFromHash(hashCode);

            // try finding an empty spot in the bucket
            // we use quadratic probing here
            // bucket positions are (n^2 + n)/2 relative to the masked hashcode
            int curIdx = idx;
            for (int i = 1; i < SharedBucketSize + 1; i++)
            {
                if (arr[curIdx].Text == null)
                {
                    idx = curIdx;
                    goto foundIdx;
                }

                curIdx = (curIdx + i) & SharedSizeMask;
            }

            // or pick a random victim within the bucket range
            // and replace with new entry
            var i1 = SharedNextRandom() & SharedBucketSizeMask;
            idx = (idx + ((i1 * i1 + i1) / 2)) & SharedSizeMask;

        foundIdx:
            arr[idx].HashCode = hashCode;
            Volatile.Write(ref arr[idx].Text, text);

            //
            return text;
        }

        private static int LocalIdxFromHash(int hash)
        {
            return hash & LocalSizeMask;
        }

        private static int SharedIdxFromHash(int hash)
        {
            // we can afford to mix some more hash bits here
            return (hash ^ (hash >> LocalSizeBits)) & SharedSizeMask;
        }

        private int LocalNextRandom()
        {
            return _localRandom++;
        }

        private static int SharedNextRandom()
        {
            return Interlocked.Increment(ref StringTable.s_sharedRandom);
        }

        public static bool TextEquals(string array, string text, int start, int length) => TextEquals(array, text.AsSpan(start, length));

        public static bool TextEquals(string array, StringBuilder text)
        {
            if (array.Length != text.Length)
            {
                return false;
            }

            // interestingly, stringbuilder holds the list of chunks by the tail
            // so accessing positions at the beginning may cost more than those at the end.
            for (var i = array.Length - 1; i >= 0; i--)
            {
                if (array[i] != text[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool TextEquals(string array, char[] text, int start, int length) => TextEquals(array, text.AsSpan(start, length));

        public static bool TextEquals(string array, ReadOnlySpan<char> text)
        {
            return array.Length == text.Length && TextEqualsCore(array, text);
        }

        private static bool TextEqualsCore(string array, char[] text, int start) => TextEqualsCore(array, text.AsSpan(start));

        private static bool TextEqualsCore(string array, ReadOnlySpan<char> text)
        {
            // use array.Length to eliminate the range check
            int s = 0;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] != text[s])
                {
                    return false;
                }
                s++;
            }

            return true;
        }
    }
}