using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Devsense.PHP.Syntax;

namespace Devsense.PHP.Text
{
    /// <summary>
    /// Helper class that represents a span of characters within given buffer.
    /// Basic functionality of <c>ReadOnlySpan&lt;char&gt;</c>.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct CharSpan
    {
        /// <summary>
        /// Empty span of characters.
        /// </summary>
        public static CharSpan Empty => new CharSpan(EmptyArray<char>.Instance, 0, 0);

        public readonly char[] Buffer;
        public readonly int Start, Length;

        private string DebugString => new string(Buffer, Start, Length);

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

        /// <summary>
        /// Gets char at given index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Index is less than <c>0</c> or greater or equal to <see cref="Length"/>.</exception>
        public char this[int idx] => (idx >= 0 && idx < Length)
            ? Buffer[Start + idx]
            : throw new ArgumentOutOfRangeException();
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
