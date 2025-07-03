// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Hashtable = System.Collections.Generic.Dictionary<object, object>;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Provides set of keyed properties.
    /// </summary>
    public interface IPropertyCollection
    {
        /// <summary>
        /// Sets property into collection.
        /// </summary>
        /// <param name="key">Key to the property, cannot be <c>null</c>.</param>
        /// <param name="value">Value.</param>
        void SetProperty(object key, object value);

        /// <summary>
        /// Sets property into collection under the key <c>typeof(T)</c>.
        /// </summary>
        /// <typeparam name="T">Type of the value and property key.</typeparam>
        /// <param name="value">Value.</param>
        void SetProperty<T>(T value);

        /// <summary>
        /// Gets property from the collection.
        /// </summary>
        /// <param name="key">Key to the property, cannot be <c>null</c>.</param>
        /// <returns>Property value or <c>null</c> if property does not exist.</returns>
        object GetProperty(object key);

        /// <summary>
        /// Gets property of type <typeparamref name="T"/> from the collection.
        /// </summary>
        /// <typeparam name="T">Type and key of the property.</typeparam>
        /// <returns>Property value.</returns>
        T GetProperty<T>();

        /// <summary>
        /// Gets property of type <typeparamref name="T"/> from the collection.
        /// </summary>
        /// <typeparam name="T">Type and key of the property.</typeparam>
        /// <returns>Property value.</returns>
        T GetPropertyOfType<T>() where T: class;

        /// <summary>
        /// Tries to get property from the container.
        /// </summary>
        bool TryGetProperty(object key, out object value);

        /// <summary>
        /// Tries to get property from the container.
        /// </summary>
        bool TryGetProperty<T>(out T value);

        /// <summary>
        /// Removes property from the collection.
        /// </summary>
        /// <param name="key">Key to the property.</param>
        /// <returns><c>True</c> if property was found and removed, otherwise <c>false</c>.</returns>
        bool RemoveProperty(object key);

        /// <summary>
        /// Removes property from the collection.
        /// </summary>
        /// <typeparam name="T">Key to the property.</typeparam>
        /// <returns><c>True</c> if property was found and removed, otherwise <c>false</c>.</returns>
        bool RemoveProperty<T>();

        /// <summary>
        /// Clear the collection of properties.
        /// </summary>
        void ClearProperties();

        /// <summary>
        /// Gets or sets property.
        /// </summary>
        /// <param name="key">Property key, cannot be <c>null</c>.</param>
        /// <returns>Property value or <c>null</c> if property does not exist.</returns>
        object this[object key] { get; set; }
    }

    /// <summary>
    /// Manages list of properties, organized by a key.
    /// </summary>
    public struct PropertyCollection : IPropertyCollection, IEnumerable<KeyValuePair<object, object>>
    {
        #region Fields & Properties

        /// <summary>
        /// Reference to actual collection of properties or single property itself.
        /// </summary>
        /// <remarks>
        /// This mechanism saves memory for small property sets.
        /// The Type of this object depends on the number of properties in the set.
        /// </remarks>
        private object _obj; // object|DictionaryNode|Hashtable

        /// <summary>
        /// If the number of properties exceeds this number, hashtable will be used instead of an array.
        /// </summary>
        const int MaxListSize = 8;

        /// <summary>
        /// Gets value indicating the collection is empty.
        /// </summary>
        public bool IsEmpty => ReferenceEquals(_obj, null);

        #endregion

        #region IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<object, object>> GetEnumerator() => Enumerable.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Enumerates items in this collection.
        /// </summary>
        /// <returns>Enumerable object of the collection. Cannot be <c>null</c>.</returns>
        IEnumerable<KeyValuePair<object, object>>/*!*/Enumerable
        {
            get
            {
                var o = _obj;
                if (ReferenceEquals(o, null))
                {
                    return EmptyArray<KeyValuePair<object, object>>.Instance;
                }
                else
                {
                    // non-empty container
                    if (o is DictionaryNode list) return list;
                    if (o is Hashtable dict) return dict;

                    // single item keyed by own type
                    return new[] { new KeyValuePair<object, object>(o.GetType(), o) };
                }
            }
        }

        #endregion

        #region Nested class: DictionaryNode

        sealed class DictionaryNode : IEnumerable<KeyValuePair<object, object>>
        {
            public object Key { get; }
            public object Value { get; set; }
            public DictionaryNode Next { get; set; }

            public DictionaryNode(object key, object value, DictionaryNode next)
            {
                this.Key = key;
                this.Value = value;
                this.Next = next;
            }

            /// <summary>
            /// Items count in this node.
            /// </summary>
            public int Count => CountItems(this);

            #region IEnumerable

            public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
            {
                for (var node = this; node != null; node = node.Next)
                {
                    yield return new KeyValuePair<object, object>(node.Key, node.Value);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets property into the container.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void SetProperty(object key, object value)
        {
            CheckKey(key);

            //
            object o = _obj;

            // empty list
            if (ReferenceEquals(o, null) || Equals(o.GetType(), key))
            {
                if (value != null && Equals(value.GetType(), key))
                {
                    // special case,
                    // single item keyed by own type
                    _obj = value;
                }
                else
                {
                    // create list
                    _obj = new DictionaryNode(key, value, null);
                }
            }
            // linked list
            else if (o is DictionaryNode list)
            {
                // replaces value if key already in collection,
                // counts items
                int count = 0;
                for (var node = list; node != null; node = node.Next)
                {
                    if (Equals(node.Key, key))
                    {
                        node.Value = value;
                        return;
                    }
                    count++;
                }

                // add new item
                if (count < MaxListSize)
                {
                    _obj = new DictionaryNode(key, value, list);
                }
                else
                {
                    // upgrade to hashtable
                    var dict = ToHashtable(list);
                    dict[key] = value;

                    _obj = dict;
                }
            }
            // hashtable
            else if (o is Hashtable dict)
            {
                dict[key] = value;
            }
            // one item list,
            // upgrade to linked list
            else
            {
                _obj = new DictionaryNode(
                    key: key,
                    value: value,
                    next: new DictionaryNode(
                        key: o.GetType(),
                        value: o,
                        next: null
                    )
                );
            }
        }

        /// <summary>
        /// Sets property into the container.
        /// </summary>
        public void SetProperty<T>(T value) => SetProperty(typeof(T), value);

        /// <summary>
        /// Tries to get property from the container.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Out. Value of the property.</param>
        /// <returns><c>true</c> if the property was found, otherwise <c>false</c>.</returns>
        public bool TryGetProperty(object key, out object value)
        {
            CheckKey(key);

            var o = _obj;

            // empty container
            if (!ReferenceEquals(o, null))
            {
                if (Equals(o.GetType(), key))
                {
                    // special case,
                    // single item keyed by own type
                    value = o;
                    return true;
                }
                else if (o is DictionaryNode list)
                {
                    for (; list != null; list = list.Next)
                    {
                        if (Equals(list.Key, key))
                        {
                            value = list.Value;
                            return true;
                        }
                    }
                }
                else if (o is Hashtable dict)
                {
                    return dict.TryGetValue(key, out value);
                }
            }

            // nothing found
            value = default(object);
            return false;
        }

        /// <summary>
        /// Quick check for a property of type <typeparamref name="T"/>, in case there is none or one entry.  
        /// </summary>
        internal bool TryGetPropertyOfTypeFast<T>(out T value) where T: class => (value = _obj as T) != null;

        /// <summary>
        /// Tries to get property from the container.
        /// </summary>
        /// <param name="value">Out. Value of the property.</param>
        /// <returns><c>true</c> if the property was found, otherwise <c>false</c>.</returns>
        public bool TryGetPropertyOfType<T>(out T value)
        {
            var o = _obj;

            if (!ReferenceEquals(o, null))
            {
                if (o is T)
                {
                    value = (T)o;
                    return true;
                }
                else if (o is DictionaryNode list)
                {
                    for (; list != null; list = list.Next)
                    {
                        if (list.Value is T)
                        {
                            value = (T)list.Value;
                            return true;
                        }
                    }
                }
                else if (o is Hashtable dict)
                {
                    foreach (var pair in dict)
                    {
                        if (pair.Value is T)
                        {
                            value = (T)pair.Value;
                            return true;
                        }
                    }
                }
            }

            // nothing found
            value = default(T);
            return false;
        }

        /// <summary>
        /// Tries to get property from the container.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns><c>null</c> or property value.</returns>
        public object GetProperty(object key) => TryGetProperty(key, out var value) ? value : null;

        /// <summary>
        /// Tries to get property from the container.
        /// </summary>
        public T GetProperty<T>() => TryGetProperty(typeof(T), out var value) ? (T)value : default(T);

        /// <summary>
        /// Tries to get a property of given type from the container.
        /// </summary>
        public T GetPropertyOfType<T>() where T: class => TryGetPropertyOfType<T>(out var value) ? (T)value : default(T);

        /// <summary>
        /// Tries to get property from the container.
        /// </summary>
        public bool TryGetProperty<T>(out T value)
        {
            if (TryGetProperty(typeof(T), out var tmp))
            {
                value = (T)tmp;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Removes property from the container.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns><c>True</c> if property was found and removed. otherwise <c>false</c>.</returns>
        public bool RemoveProperty(object key)
        {
            CheckKey(key);

            var o = _obj;

            if (!ReferenceEquals(o, null))
            {
                if (Equals(o.GetType(), key))
                {
                    // single item keyed by own type
                    _obj = null;
                    return true;
                }
                else if (o is DictionaryNode node)
                {
                    DictionaryNode prev = null;
                    for (; node != null; node = node.Next)
                    {
                        if (Equals(node.Key, key))
                        {
                            if (prev == null)
                            {
                                _obj = node.Next;
                            }
                            else
                            {
                                prev.Next = node.Next;
                            }

                            return true;
                        }

                        //
                        prev = node;
                    }
                }
                else if (o is Hashtable dict)
                {
                    if (dict.Remove(key))
                    {
                        if (dict.Count < MaxListSize)
                        {
                            // downgrade to list
                            _obj = ToList(dict);
                        }

                        return true;
                    }
                }
            }

            //
            return false;
        }

        /// <summary>
        /// Removes property from the container.
        /// </summary>
        public bool RemoveProperty<T>() => RemoveProperty(typeof(T));

        /// <summary>
        /// Clears the container.
        /// </summary>
        public void ClearProperties()
        {
            _obj = null;
        }

        /// <summary>
        /// Gets amount of properties in the container.
        /// </summary>
        public int Count
        {
            get
            {
                var o = _obj;
                if (ReferenceEquals(o, null)) return 0;
                if (o is DictionaryNode list) return CountItems(list);
                if (o is Hashtable dict) return dict.Count;
                else return 1; // {o} is value itself
            }
        }

        /// <summary>
        /// Gets or sets named property.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>Property value or <c>null</c>.</returns>
        public object this[object key]
        {
            get => GetProperty(key);
            set => SetProperty(key, value);
        }

        #endregion

        #region Helper functions

        static Exception KeyNullException()
        {
            return new ArgumentNullException("key");
        }

        static Exception KeyNotAllowedException()
        {
            return new ArgumentException(string.Empty, "key");
        }

        static void CheckKey(object key)
        {
            if (key == null)
            {
                throw KeyNullException();
            }

            if (key == (object)typeof(Hashtable))
            {
                // key cannot be Hashtable,
                // reserved,
                // would corrupt the data structure
                throw KeyNotAllowedException();
            }
        }

        /// <summary>
        /// Counts items in the linked list.
        /// </summary>
        private static int CountItems(DictionaryNode head)
        {
            int count = 0;
            for (var p = head; p != null; p = p.Next)
            {
                count++;
            }
            return count;
        }

        private static Hashtable/*!*/ToHashtable(DictionaryNode/*!*/node)
        {
            var hashtable = new Hashtable(13);

            for (var p = node; p != null; p = p.Next)
            {
                hashtable[p.Key] = p.Value;
            }

            return hashtable;
        }
        private static DictionaryNode ToList(Hashtable/*!*/hashtable)
        {
            DictionaryNode list = null;
            foreach (var p in hashtable)
            {
                list = new DictionaryNode(p.Key, p.Value, list);
            }
            return list;
        }

        #endregion
    }

    /// <summary>
    /// Helper reference object implementing <see cref="IPropertyCollection"/>
    /// </summary>
    [DebuggerDisplay("Count = {_properties.Count}")]
    public class PropertyCollectionClass : IPropertyCollection, IEnumerable<KeyValuePair<object, object>>
    {
        /// <summary>
        /// Internal collection (struct).
        /// </summary>
        private PropertyCollection _properties = new PropertyCollection();

        public object this[object key]
        {
            get
            {
                return _properties.GetProperty(key);
            }

            set
            {
                _properties.SetProperty(key, value);
            }
        }

        public void ClearProperties()
        {
            if (_properties.IsEmpty == false)
            {
                lock (this)
                {
                    _properties.ClearProperties();
                }
            }
        }

        public object GetProperty(object key)
        {
            if (_properties.IsEmpty)
            {
                return null;
            }
            
            lock (this)
            {
                return _properties.GetProperty(key);
            }
        }

        public T GetProperty<T>()
        {
            if (_properties.IsEmpty)
            {
                return default(T);
            }
            
            lock (this)
            {
                return _properties.GetProperty<T>();
            }
        }

        public T GetPropertyOfType<T>() where T: class
        {
            if (_properties.IsEmpty)
            {
                return default(T);
            }
            
            if (_properties.TryGetPropertyOfTypeFast<T>(out var value) == false)
            {
                lock (this)
                {
                    value = _properties.GetPropertyOfType<T>();
                }
            }
            
            //
            return value;
        }

        public bool RemoveProperty(object key)
        {
            if (_properties.IsEmpty)
            {
                return false;
            }
            
            lock (this)
            {
                return _properties.RemoveProperty(key);
            }
        }

        public bool RemoveProperty<T>()
        {
            if (_properties.IsEmpty)
            {
                return false;
            }
            
            lock (this)
            {
                return _properties.RemoveProperty<T>();
            }
        }

        public void SetProperty(object key, object value)
        {
            lock (this)
            {
                _properties.SetProperty(key, value);
            }
        }

        public void SetProperty<T>(T value)
        {
            lock (this)
            {
                _properties.SetProperty<T>(value);
            }
        }

        public bool TryGetProperty(object key, out object value)
        {
            if (_properties.IsEmpty)
            {
                value = null;
                return false;
            }
            
            lock (this)
            {
                return _properties.TryGetProperty(key, out value);
            }
        }

        public bool TryGetProperty<T>(out T value)
        {
            if (_properties.IsEmpty)
            {
                value = default(T);
                return false;
            }
            
            lock (this)
            {
                return _properties.TryGetProperty<T>(out value);
            }
        }

        /// <summary>
        /// Gets value from collection. If value is not set yet, it is created using provided factory.
        /// </summary>
        public T GetOrCreateProperty<T>(Func<T>/*!*/factory)
        {
            if (this.TryGetProperty<T>(out var value) == false)
            {
                this.SetProperty<T>(value = factory());
            }

            return value;
        }

        /// <summary>
        /// Gets number of properties in the container.
        /// </summary>
        public int Count
        {
            get
            {
                if (_properties.IsEmpty)
                {
                    return 0;
                }
                
                lock (this)
                {
                    return _properties.Count;
                }
            }
        }

        /// <summary>
        /// Enumerates items in the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            lock (this)
            {
                return _properties.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
