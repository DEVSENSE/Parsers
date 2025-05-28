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

namespace Devsense.PHP.Text
{
    #region TextUtils

    public static class TextUtils
    {
        // CR, LF, CRLF
        const char CR = '\r';
        const char LF = '\n';

        // unicode line separators:
        const char NEL = '\u0085';
        const char LS = '\u2028';
        const char PS = '\u2029';

        /// <summary>
        /// Gets length of line break character sequence if any.
        /// </summary>
        /// <param name="text">Document text.</param>
        /// <param name="position">Index of character within <paramref name="text"/> to look at.</param>
        /// <returns>Length of line break character sequence at <paramref name="position"/>. In case of no line break, <c>0</c> is returned.</returns>
        public static int LengthOfLineBreak(string text, int position) => LengthOfLineBreak(text.AsSpan(position));

        /// <summary>
        /// Gets length of line break character sequence if any.
        /// </summary>
        /// <remarks>See <see cref="LengthOfLineBreak(string, int)"/>.</remarks>
        public static int LengthOfLineBreak(ReadOnlySpan<char> text, int position) => LengthOfLineBreak(text.Slice(position));

        /// <summary>
        /// Gets length of line break character sequence if any.
        /// </summary>
        /// <remarks>See <see cref="LengthOfLineBreak(string, int)"/>.</remarks>
        public static int LengthOfLineBreak(ReadOnlySpan<char> text)
        {
            var c = text[0];
            if (c == CR)
            {
                // \r\n
                if (text.Length > 1 && text[1] == LF)
                {
                    return 2;
                }

                // \r
                return 1;
            }
            else
            {
                // \n
                // unicode line breaks
                if (c == LF || c == NEL || c == LS || c == PS)
                    return 1;

                //
                return 0;
            }
        }

        /// <summary>
        /// Gets position of CR/LF/CRLF line break.
        /// </summary>
        /// <param name="text">Source text.</param>
        /// <param name="eol_length">Length of the line break.</param>
        /// <returns>Index of the line break or <c>-1</c> if there is no line break.</returns>
        public static int IndexOfLineBreak(ReadOnlySpan<char> text, out int eol_length)
        {
            var idx = text.IndexOfAny(CR, LF);
            if (idx >= 0)
            {
                // \n
                var c0 = text[idx];
                if (c0 == LF)
                {
                    eol_length = 1;
                    return idx;
                }
                else //if (c0 == CR)
                {
                    if (idx + 1 < text.Length && text[idx + 1] == LF)
                    {
                        // \r\n
                        eol_length = 2;
                    }
                    else
                    {
                        // \r
                        eol_length = 1;
                    }

                    return idx;
                }
            }

            //
            eol_length = 0;
            return -1;
        }

        internal static bool EndsWithEol(ReadOnlySpan<char> text)
        {
            return text.Length != 0 && (LengthOfLineBreak(text.Slice(text.Length - 1)) != 0);
        }

        /// <summary>
        /// Gets <see cref="Span"/> of whole <paramref name="line"/>.
        /// </summary>
        /// <param name="lineBreaks">Information about line breaks in the document. Cannot be <c>null</c>.</param>
        /// <param name="line">Line number.</param>
        /// <returns><see cref="Span"/> of line specified by parameter <paramref name="line"/> within the document <paramref name="lineBreaks"/>.</returns>
        public static Span GetLineSpan(this ILineBreaks/*!*/lineBreaks, int line)
        {
            if (lineBreaks == null)
                throw new ArgumentNullException(nameof(lineBreaks));

            if (line < 0 || line > lineBreaks.Count)
                throw new ArgumentException(nameof(line));

            int start = (line != 0) ? lineBreaks.EndOfLineBreak(line - 1) : 0;
            int end = (line < lineBreaks.Count) ? lineBreaks.EndOfLineBreak(line) : lineBreaks.TextLength;

            return Span.FromBounds(start, end);
        }

        /// <summary>
        /// Gets line number from <paramref name="position"/> within document.
        /// </summary>
        /// <param name="lines">Reference to line breaks object.</param>
        /// <param name="position">Position within document.</param>
        /// <returns>Line number.</returns>
        /// <exception cref="ArgumentOutOfRangeException">In case <paramref name="position"/> is out of line number range.</exception>
        public static int GetLineFromPosition(this ILineBreaks lines, int position) => lines.TryGetLineAtPosition(position, out var line)
            ? line
            : throw new ArgumentOutOfRangeException();

        /// <summary>
        /// Gets line and column from position number.
        /// </summary>
        /// <param name="lines">Reference to line breaks object.</param>
        /// <param name="position">Position with the document.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        public static void GetLineColumnFromPosition(this ILineBreaks lines, int position, out int line, out int column)
        {
            if (lines.TryGetLineAtPosition(position, out line))
            {
                column = line > 0
                    ? position - lines.EndOfLineBreak(line - 1)
                    : position;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets line and its span at given character index.
        /// </summary>
        /// <param name="lines">Text lines.</param>
        /// <param name="position">Character index.</param>
        /// <param name="line">Line index, zero based.</param>
        /// <param name="span">Line span.</param>
        /// <returns>Value indicating the position is valid within the document.</returns>
        public static bool TryGetLineAtPosition(this ILineBreaks lines, int position, out int line, out Span span)
        {
            if (lines.TryGetLineAtPosition(position, out line))
            {
                span = GetLineSpan(lines, line);
                return true;
            }

            //
            span = Span.Invalid;
            return false;
        }

        /// <summary>Gets number as string. Numbers in range (0,9) don't cause a new string allocation.</summary>
        public static string GetNumeral(this int number)
        {
            switch (number)
            {
                case 0: return "0";
                case 1: return "1";
                case 2: return "2";
                case 3: return "3";
                case 4: return "4";
                case 5: return "5";
                case 6: return "6";
                case 7: return "7";
                case 8: return "8";
                case 9: return "9";
                default: return number.ToString();
            }
        }

        public static string Substring(this string str, Span span)
        {
            return str.Substring(span.Start, span.Length);
        }

        public static IEnumerable<Span> EnumerateLines(this string text, bool includeEOL)
        {
            int linestart = 0;

            for (int i = 0; i < text.Length;)
            {
                var eol = TextUtils.LengthOfLineBreak(text, i);
                if (eol != 0)
                {
                    yield return new Span(linestart, i - linestart + (includeEOL ? eol : 0));

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

            yield return new Span(linestart, text.Length - linestart);
        }

        /// <summary>
        /// Enumerates spans of text separated by split character.
        /// </summary>
        public static IEnumerable<Span> SplitEnumerator(this string text, char splitChar, bool ignoreEmpty = false)
        {
            int start = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == splitChar)
                {
                    if (!ignoreEmpty || start < i)
                    {
                        yield return new Span(start, i - start);
                    }

                    start = i + 1;
                }
            }

            if (!ignoreEmpty || start < text.Length)
            {
                yield return new Span(start, text.Length - start);
            }
        }

        public static ReadOnlySpan<char> AsSpan(this string text, Span ourspan) => (ourspan.Length <= 0)
            ? ReadOnlySpan<char>.Empty
            : text.AsSpan(ourspan.Start, ourspan.Length);
    }

    #endregion
}
