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

        /*readonly*/
        struct Chunk
        {
            /// <summary>
            /// The underlying value, either string or byte[] or char[].
            /// </summary>
            public object RawValue => _value;
            object _value;

            // TODO: byte, char as a value type

            /// <summary>
            /// Chunk is constructed from UTF-16 string.
            /// </summary>
            public bool IsUnicode => RawValue is string || RawValue is char[];

            /// <summary>
            /// Chunk is constructed from byte array.
            /// </summary>
            public bool Is8Bit => RawValue is byte[];

            /// <summary>
            /// Chunk is empty.
            /// </summary>
            public bool IsEmpty => _value == null || (_value is string str && str.Length == 0) || (_value is byte[] b && b.Length == 0) || (_value is char[] c && c.Length == 0);

            Chunk(object value)
            {
                Debug.Assert(value == null || value is string || value is byte[] || value is char[]);
                _value = value ?? string.Empty;
            }

            public Chunk(byte[] value) : this((value != null && value.Length != 0) ? (object)value : null)
            {
            }

            public Chunk(string value) : this((value != null && value.Length != 0) ? (object)value : null)
            {
            }

            public Chunk(char[] value) : this((value != null && value.Length != 0) ? (object)value : null)
            {
            }

            public Chunk(Span<char> value) : this(value.Length != 0 ? (object)value.ToArray() : null)
            {
            }

            public Chunk(char c) : this(c.ToString())
            {
            }

            public Chunk(byte b) : this(new byte[] { b })
            {
            }

            /// <summary>
            /// If both given chunks are the same type, merge them.
            /// </summary>
            public static bool TryMerge(Chunk a, Chunk b, out Chunk merged)
            {
                if (a.IsEmpty)
                {
                    merged = b;
                    return true;
                }
                else if (b.IsEmpty)
                {
                    merged = a;
                    return true;
                }
                else if (a.RawValue is string stra && b.RawValue is string strb)
                {
                    merged = new Chunk(stra + strb);
                    return true;
                }
                else if (a.RawValue is byte[] ba && b.RawValue is byte[] bb)
                {
                    merged = new Chunk(ArrayUtils.Concat(ba, bb));
                    return true;
                }
                else if (a.RawValue is char[] ca && b.RawValue is char[] cb)
                {
                    merged = new Chunk(ArrayUtils.Concat(ca, cb));
                    return true;
                }
                else
                {
                    merged = default(Chunk);
                    return false;
                }
            }

            public string ToString(Encoding encoding) => RawValue switch
            {
                null => string.Empty,
                string str => str,
                char[] chars => new string(chars),
                byte[] bytes => bytes.Length == 1 ? ((char)bytes[0]).ToString() : encoding.GetString(bytes, 0, bytes.Length),
                _ => throw new InvalidOperationException(),
            };

            public byte[] ToBytes(Encoding encoding) => RawValue switch
            {
                null => EmptyArray<byte>.Instance,
                string str => encoding.GetBytes(str),
                char[] chars => encoding.GetBytes(chars),
                byte[] bytes => bytes,
                _ => throw new InvalidOperationException(),
            };
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

            public void Append(Chunk chunk)
            {
                if (chunk.IsEmpty) return;

                if (ChunksCount != 0 && Chunk.TryMerge(Chunks[ChunksCount - 1], chunk, out var merged))
                {
                    Chunks[ChunksCount - 1] = merged;
                }
                else
                {
                    EnsureCapacity(ChunksCount + 1);
                    Chunks[ChunksCount++] = chunk;
                }
            }

            public void Append(byte b) => Append(new Chunk(new byte[] { b }));

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

                if (ChunksCount == 1 && Chunks[0].RawValue is string str)
                {
                    return str;
                }

                var builder = new StringBuilder(ChunksCount * 2);

                for (int i = 0; i < ChunksCount; i++)
                {
                    builder.Append(Chunks[i].ToString(encoding));
                }

                return builder.ToString();
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
                    result.AddRange(Chunks[i].ToBytes(encoding));
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
                    switch (chunk.RawValue)
                    {
                        case null:
                            break;

                        case string str:
                            yield return str;
                            break;

                        case byte[] bytes:
                            yield return bytes;
                            break;

                        case char[] chars:
                            yield return new string(chars);
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

        private void Append(Chunk chunk) => _chunks.Append(chunk);

        public void Append(string str, Span span)
        {
            Append(span);
            Append(str);
        }

        public void Append(string str) => Append(new Chunk(str));

        public void Append(char[] buffer, int start, int length)
        {
            if (length == 0)
            {
                return;
            }

            Append(new Chunk(buffer.AsSpan(start, length)));
        }

        public void Append(char c, Span span)
        {
            Append(span);
            Append(c);
        }

        public void Append(char c) => Append(new Chunk(c));

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
