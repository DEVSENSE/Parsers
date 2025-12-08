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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// The PHP-semantic string builder. Binary or Unicode string builder.
    /// </summary>
    internal class PhpStringBuilder
    {
        #region Nested struct: Chunk

        struct Chunk
        {
            /// <summary>
            /// The underlying value: byte[] or char[].
            /// </summary>
            public Array RawArray => _array;
            Array _array;

            /// <summary>
            /// Count of used items in <see cref="RawArray"/>.
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            /// Underlying buffer capacity.
            /// </summary>
            public int Capacity => _array != null ? _array.Length : 0;

            /// <summary>
            /// Chunk is constructed from UTF-16 string.
            /// </summary>
            public bool IsUnicode => _array is char[];

            /// <summary>
            /// Chunk is constructed from byte array.
            /// </summary>
            public bool Is8Bit => _array is byte[];

            /// <summary>
            /// Chunk is empty.
            /// </summary>
            public bool IsEmpty => Length == 0;

            public void ToString(StringBuilder output, Encoding encoding)
            {
                if (_array == null || Length == 0)
                {
                    // nothing
                }
                else if (_array is char[] chars)
                {
                    // utf chars
                    output.Append(chars, 0, Length);
                }
                else if (_array is byte[] bytes)
                {
                    // 8bit chars
                    if (Length == 1)
                    {
                        output.Append((char)bytes[0]);
                    }
                    else
                    {
                        output.Append(encoding.GetString(bytes, 0, Length));
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            public void ToBytes(List<byte> output, Encoding encoding)
            {
                if (_array == null || Length == 0)
                {
                    // nothing
                }
                else if (_array is char[] chars)
                {
                    // utf chars
                    output.AddRange(encoding.GetBytes(chars, 0, Length));
                }
                else if (_array is byte[] bytes)
                {
                    // 8bit chars
                    output.AddRange(bytes.TakeArray(0, Length));
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            public static Chunk Create<T>() => new Chunk { _array = EmptyArray<T>.Instance, };

            static bool EnsureCapacity<T>(ref Array array, int capacity)
            {
                if (capacity < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (array == null)
                {
                    array = new T[capacity];
                    return true;
                }

                if (array is T[] typedArray)
                {
                    if (typedArray.Length < capacity)
                    {
                        var newsize = Math.Max(capacity, typedArray.Length * 2);

                        Array.Resize(ref typedArray, newsize);
                        array = typedArray;
                    }

                    return true;
                }

                return false;
            }

            public static bool EnsureCapacity<T>(ref Chunk chunk, int capacity) => EnsureCapacity<T>(ref chunk._array, capacity);
        }

        #endregion

        #region Nested struct: ChunkList

        struct ChunkList
        {
            /// <summary>
            /// Raw chunks of string data, either UTF-16 or bytes,
            /// </summary>
            public Chunk[] Chunks;

            /// <summary>
            /// Number of items in <see cref="_chunks"/>.
            /// </summary>
            public int ChunksCount;

            /// <summary>
            /// Gets value indicating the list is empty.
            /// </summary>
            public bool IsEmpty => ChunksCount == 0;

            /// <summary>
            /// Gets value indicating the string contains one or more 8bit characters that should not be encoded into UTF-16.
            /// </summary>
            public bool Contains8bitText
            {
                get
                {
                    for (int i = 0; i < ChunksCount; i++)
                    {
                        if (Chunks[i].Is8Bit) return true;
                    }

                    return false;
                }
            }

            public static ChunkList Create() => new ChunkList()
            {
                Chunks = EmptyArray<Chunk>.Instance,
                ChunksCount = 0,
            };

            Span<T> GetAppendBuffer<T>(int length)
            {
                if (length <= 0)
                {
                    return Span<T>.Empty;
                }

                if (ChunksCount != 0)
                {
                    ref var lastchunk = ref Chunks[ChunksCount - 1];
                    if (Chunk.EnsureCapacity<T>(ref lastchunk, lastchunk.Length + length))
                    {
                        var indexFrom = lastchunk.Length;
                        lastchunk.Length += length;

                        return ((T[])lastchunk.RawArray).AsSpan(indexFrom, length);
                    }
                }

                // add new chunk
                EnsureCapacity(++ChunksCount);
                Chunks[ChunksCount - 1] = Chunk.Create<T>();
                return GetAppendBuffer<T>(length);
            }

            public void Append(ReadOnlySpan<char> chars)
            {
                if (chars.Length > 0)
                {
                    var span = GetAppendBuffer<char>(chars.Length);
                    Debug.Assert(span.Length == chars.Length);
                    chars.CopyTo(span);
                }
            }

            public void Append(string str)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var span = GetAppendBuffer<char>(str.Length);
                    Debug.Assert(span.Length == str.Length);
                    str.AsSpan().CopyTo(span);
                }
            }

            public void Append(char c)
            {
                var span = GetAppendBuffer<char>(1);
                span[0] = c;
            }

            public void Append(byte b)
            {
                var span = GetAppendBuffer<byte>(1);
                span[0] = b;
            }

            void EnsureCapacity(int capacity)
            {
                if (Chunks.Length < capacity)
                {
                    Array.Resize(ref Chunks, capacity + 4);
                }
            }

            public string ToString(Encoding encoding)
            {
                if (encoding == null)
                    throw new ArgumentNullException(nameof(encoding));

                if (IsEmpty)
                {
                    return string.Empty;
                }

                var result = StringUtils.GetStringBuilder(ChunksCount * 2);

                for (int i = 0; i < ChunksCount; i++)
                {
                    Chunks[i].ToString(result, encoding);
                }

                return StringUtils.ReturnStringBuilder(result);
            }

            public byte[] ToBytes(Encoding encoding)
            {
                if (encoding == null)
                    throw new ArgumentNullException(nameof(encoding));

                if (IsEmpty)
                {
                    return EmptyArray<byte>.Instance;
                }

                var result = new List<byte>();

                for (int i = 0; i < ChunksCount; i++)
                {
                    Chunks[i].ToBytes(result, encoding);
                }

                return result.ToArray();
            }

            /// <summary>
            /// Gets enumeration of underlying chunks of <see cref="System.String"/> or <see cref="byte"/>[].
            /// </summary>
            public IEnumerable<object> EnumerateChunks()
            {
                for (int i = 0; i < ChunksCount; i++)
                {
                    var chunk = Chunks[i];
                    switch (chunk.RawArray)
                    {
                        case null:
                            break;

                        case byte[] bytes:
                            yield return bytes.TakeArray(0, chunk.Length);
                            break;

                        case char[] chars:
                            yield return new string(chars, 0, chunk.Length);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
        }

        #endregion

        #region Nested class: StringLiteralValue

        sealed class StringLiteralValue : IStringLiteralValue
        {
            readonly ChunkList Chunks;

            public Encoding Encoding { get; }

            public StringLiteralValue(ChunkList chunks, Encoding encoding)
            {
                Chunks = chunks;
                Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            }

            public bool Contains8bitText => Chunks.Contains8bitText;

            public override string ToString() => Chunks.ToString(Encoding);

            public byte[] ToBytes() => Chunks.ToBytes(Encoding);

            /// <summary>
            /// Gets enumeration of underlying chunks of <see cref="System.String"/> or <see cref="byte"/>[].
            /// </summary>
            public IEnumerable<object> EnumerateChunks() => Chunks.EnumerateChunks();
        }

        #endregion

        #region Fields & Properties

        /// <summary>
        /// File encoding.
        /// </summary>
        readonly Encoding/*!*/_encoding;

        ChunkList _chunks = ChunkList.Create();

        private Span span;

        #endregion

        #region Results

        public string StringResult => _chunks.ToString(_encoding);

        public IStringLiteralValue Result => new StringLiteralValue(_chunks, _encoding);

        #endregion

        #region Construct

        /// <summary>
        /// Initialize the PhpStringBuilder.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="binary"></param>
        /// <param name="maxLength"></param>
        public PhpStringBuilder(Encoding/*!*/encoding, bool binary, int maxLength)
        {
            Debug.Assert(encoding != null);
            Debug.Assert(maxLength != 0);

            this._encoding = encoding;
            this.span = Span.Invalid;

            //if (binary)
            //    _binaryBuilder = new List<byte>(initialLength);
            //else
            //    _unicodeBuilder = new StringBuilder(maxLength, int.MaxValue);
        }

        public PhpStringBuilder(Encoding/*!*/encoding, string/*!*/value, Span span)
            : this(encoding, false, value.Length)
        {
            Append(value, span);
        }

        #endregion

        #region Append

        private void Append(Span span)
        {
            if (this.span.IsValid)
            {
                if (span.IsValid)
                {
                    this.span = Span.Combine(this.span, span);
                }
            }
            else
            {
                this.span = span;
            }
        }

        public void Append(string str, Span span)
        {
            Append(span);
            Append(str);
        }

        public void Append(string str)
        {
            _chunks.Append(str);
        }

        public void Append(char[] buffer, int start, int length)
        {
            Append(buffer.AsSpan(start, length));
        }

        public void Append(ReadOnlySpan<char> buffer)
        {
            _chunks.Append(buffer);
        }

        public void Append(char c, Span span)
        {
            Append(span);
            Append(c);
        }

        public void Append(char c)
        {
            _chunks.Append(c);
        }

        public void Append(byte b, Span span)
        {
            Append(span);
            Append(b);
        }

        public void Append(byte b)
        {
            if (b <= 0x7f)
            {
                Append((char)b);
            }
            else
            {
                _chunks.Append(b);
            }
        }

        #endregion
    }
}
