using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Devsense.PHP.Syntax;

namespace Devsense.PHP.Text
{
    /// <summary>
    /// Helper class that represents a span of characters within given buffer.
    /// Basic functionality of <c>ReadOnlySpan&lt;char&gt;</c>.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct CharSpan : IEquatable<string>
    {
        /// <summary>
        /// Empty span of characters.
        /// </summary>
        public static CharSpan Empty => new CharSpan(EmptyArray<char>.Instance, 0, 0);

        public readonly char[] Buffer;
        public readonly int Start, Length;

        private string DebugString => ToString();

        public CharSpan(char[] buffer)
            : this(buffer, 0, buffer.Length)
        { }

        public CharSpan(char[] buffer, int start)
            : this(buffer, start, buffer.Length - start)
        { }

        public CharSpan(char[] buffer, int start, int length)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(start >= 0 && start <= buffer.Length);
            Debug.Assert(length >= 0 && length <= buffer.Length - start);

            Buffer = buffer;
            Start = start;
            Length = length;
        }

        public override string ToString()
        {
            return Length != 0
                ? new string(Buffer, Start, Length)
                : string.Empty;
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj is string str) return Equals(str);
            if (obj is CharSpan span) return Equals(span);

            return false;
        }

        public bool Equals(CharSpan other)
        {
            if (other != null && other.Length == Length)
            {
                for (int i = 0; i < Length; i++)
                {
                    if (other[i] != this[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public bool Equals(string other)
        {
            if (other != null && other.Length == Length)
            {
                for (int i = 0; i < other.Length; i++)
                {
                    if (other[i] != Buffer[Start + i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets char at given index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Index is less than <c>0</c> or greater or equal to <see cref="Length"/>.</exception>
        public char this[int idx] => (idx >= 0 && idx < Length)
            ? Buffer[Start + idx]
            : throw new ArgumentOutOfRangeException();

        public static bool operator ==(CharSpan left, string right) => left.Equals(right);

        public static bool operator !=(CharSpan left, string right) => !left.Equals(right);

        public static implicit operator ReadOnlySpan<char>(CharSpan span) => new ReadOnlySpan<char>(span.Buffer, span.Start, span.Length);
    }

    /// <summary>
    /// Extension methods to <see cref="CharSpan"/>.
    /// </summary>
    public static class CharSpanExtension
    {
        public static CharSpan TrimLeft(this CharSpan chars)
        {
            int skip = 0;
            while (skip < chars.Length && char.IsWhiteSpace(chars[skip]))
            {
                skip++;
            }

            return chars.Substring(skip);
        }

        public static CharSpan TrimRight(this CharSpan chars)
        {
            int length = chars.Length;
            while (length != 0 && char.IsWhiteSpace(chars[length - 1]))
            {
                length--;
            }

            return chars.Substring(0, length);
        }

        public static CharSpan Trim(this CharSpan chars) => chars.TrimLeft().TrimRight();

        /// <summary>
        /// Finds last whitespace character and returns substring that follows.
        /// </summary>
        public static CharSpan LastWord(this CharSpan chars)
        {
            for (int i = chars.Length - 1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(chars[i]))
                {
                    return Substring(chars, i + 1);
                }
            }

            return chars;
        }

        public static IEnumerable<CharSpan> EnumerateLines(this CharSpan text, bool includeEOL)
        {
            int linestart = 0;

            for (int i = 0; i < text.Length;)
            {
                var eol = TextUtils.LengthOfLineBreak(text.Buffer, text.Start + i);
                if (eol != 0)
                {
                    yield return text.Substring(linestart, i - linestart + (includeEOL ? eol : 0));

                    i += eol;
                    linestart = i;

                    if (linestart >= text.Length)
                    {
                        yield break;
                    }
                }
                else
                {
                    i++;
                }
            }

            yield return text.Substring(linestart);
        }

        /// <summary>
        /// Gets last character of the span or '\0' if span is empty.
        /// </summary>
        public static char LastChar(this CharSpan chars)
        {
            return chars.Length != 0 ? chars[chars.Length - 1] : '\0';
        }

        public static CharSpan Substring(this CharSpan chars, int index)
            => index == 0
            ? chars
            : index >= chars.Length
                ? CharSpan.Empty
                : new CharSpan(chars.Buffer, chars.Start + index, chars.Length - index);

        public static CharSpan Substring(this CharSpan chars, int index, int length)
        {
            if (length == 0) return CharSpan.Empty;
            if (length > chars.Length - index) throw new ArgumentOutOfRangeException();

            return new CharSpan(chars.Buffer, chars.Start + index, length);
        }

        public static bool StartsWith(this CharSpan chars, string text)
        {
            if (text.Length > chars.Length)
            {
                return false;
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != chars[i])
                    return false;
            }

            return true;
        }

        public static bool StartsWith(this CharSpan chars, CharSpan text)
        {
            if (text.Length > chars.Length)
            {
                return false;
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != chars[i])
                    return false;
            }

            return true;
        }

        public static int LastIndexOf(this CharSpan chars, char ch)
        {
            int i = chars.Length - 1;

            while (i >= 0 && chars[i] != ch)
            {
                i--;
            }

            return i;
        }
    }
}
