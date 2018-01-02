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
        #region Fields & Properties
        /// <summary>
        /// Currently used encoding.
        /// </summary>
        private readonly Encoding/*!*/encoding;

        private byte[] BytesBuffer
        {
            get
            {
                if (_encodeBytes == null) _encodeBytes = new byte[4];
                return _encodeBytes;
            }
        }
        private byte[] _encodeBytes;

        private char[] CharsBuffer
        {
            get
            {
                if (_encodeChars == null) _encodeChars = new char[5];
                return _encodeChars;
            }
        }
        private char[] _encodeChars;

        private StringBuilder _unicodeBuilder;
        private List<byte> _binaryBuilder;

        private bool IsUnicode { get { return !IsBinary; } }
        private bool IsBinary { get { return _binaryBuilder != null; } }

        private Span span;

        /// <summary>
        /// Length of contained data (string or byte[]).
        /// </summary>
        public int Length
        {
            get
            {
                if (_unicodeBuilder != null)
                    return _unicodeBuilder.Length;
                else if (_binaryBuilder != null)
                    return _binaryBuilder.Count;
                else
                    return 0;
            }
        }

        private StringBuilder UnicodeBuilder
        {
            get
            {
                if (_unicodeBuilder == null)
                {
                    if (_binaryBuilder != null && _binaryBuilder.Count > 0)
                    {
                        byte[] bytes = _binaryBuilder.ToArray();
                        _unicodeBuilder = new StringBuilder(encoding.GetString(bytes, 0, bytes.Length));
                    }
                    else
                    {
                        _unicodeBuilder = new StringBuilder();
                    }
                    _binaryBuilder = null;
                }

                return _unicodeBuilder;
            }
        }

        private List<byte> BinaryBuilder
        {
            get
            {
                if (_binaryBuilder == null)
                {
                    if (_unicodeBuilder != null && _unicodeBuilder.Length > 0)
                    {
                        _binaryBuilder = new List<byte>(encoding.GetBytes(_unicodeBuilder.ToString()));
                    }
                    else
                    {
                        _binaryBuilder = new List<byte>();
                    }
                    _unicodeBuilder = null;
                }

                return _binaryBuilder;
            }
        }

        #endregion

        #region Results

        /// <summary>
        /// The result of builder: String or byte[].
        /// </summary>
        public object Result
        {
            get
            {
                if (IsBinary)
                    return Length != 0 ? BinaryBuilder.ToArray() : EmptyArray<byte>.Instance;
                else
                    return Length != 0 ? UnicodeBuilder.ToString() : string.Empty;
            }
        }

        public Literal CreateLiteral()
        {
            if (IsBinary)
                return new BinaryStringLiteral(span, BinaryBuilder.ToArray());
            else
                return new StringLiteral(span, UnicodeBuilder.ToString());
        }

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

            this.encoding = encoding;
            this.span = Span.Invalid;

            //if (binary)
            //    _binaryBuilder = new List<byte>(initialLength);
            //else
            _unicodeBuilder = new StringBuilder(maxLength, int.MaxValue);
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
                    this.span = Span.Combine(this.span, span);
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
            if (IsUnicode)
            {
                UnicodeBuilder.Append(str);
            }
            else
            {
                BinaryBuilder.AddRange(encoding.GetBytes(str));
            }
        }

        public void Append(char[] buffer, int start, int length)
        {
            if (IsUnicode)
            {
                UnicodeBuilder.Append(buffer, start, length);
            }
            else
            {
                BinaryBuilder.AddRange(encoding.GetBytes(buffer, start, length));
            }
        }

        public void Append(char c, Span span)
        {
            Append(span);
            Append(c);
        }
        public void Append(char c)
        {
            if (IsUnicode)
            {
                UnicodeBuilder.Append(c);
            }
            else
            {
                var encodeBytes = BytesBuffer;
                var encodeChars = CharsBuffer;

                encodeChars[0] = c;
                int count = encoding.GetBytes(encodeChars, 0, 1, encodeBytes, 0);
                for (int i = 0; i < count; ++i)
                    BinaryBuilder.Add(encodeBytes[i]);
            }
        }

        public void Append(byte b, Span span)
        {
            Append(span);
            Append(b);
        }
        public void Append(byte b)
        {
            // force binary string

            if (IsUnicode)
            {
                var encodeBytes = BytesBuffer;
                var encodeChars = CharsBuffer;

                encodeBytes[0] = b;
                UnicodeBuilder.Append(encodeChars, 0, encoding.GetChars(encodeBytes, 0, 1, encodeChars, 0));
            }
            else
                BinaryBuilder.Add(b);
        }

        public void Append(int c, Span span)
        {
            Append(span);
            Append(c);
        }
        public void Append(int c)
        {
            Debug.Assert(c >= 0);

            //if (c <= 0xff)
            if (IsBinary)
                BinaryBuilder.Add((byte)c);
            else
                UnicodeBuilder.Append((char)c);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Trim ending /r/n or /n characters. Assuming the string ends with /n.
        /// </summary>
        internal void TrimEoln()
        {
            if (IsUnicode)
            {
                if (UnicodeBuilder.Length > 0)
                {
                    if (UnicodeBuilder.Length >= 2 && UnicodeBuilder[UnicodeBuilder.Length - 2] == '\r')
                    {
                        // trim ending \r\n:
                        UnicodeBuilder.Length -= 2;
                    }
                    else
                    {
                        // trim ending \n:
                        UnicodeBuilder.Length -= 1;
                    }
                }
            }
            else
            {
                if (BinaryBuilder.Count > 0)
                {
                    if (BinaryBuilder.Count >= 2 && BinaryBuilder[BinaryBuilder.Count - 2] == (byte)'\r')
                    {
                        BinaryBuilder.RemoveRange(BinaryBuilder.Count - 2, 2);
                    }
                    else
                    {
                        BinaryBuilder.RemoveAt(BinaryBuilder.Count - 1);
                    }
                }
            }

        }

        #endregion
    }
}
