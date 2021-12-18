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
        /// <summary>
        /// Gets length of line break character sequence if any.
        /// </summary>
        /// <param name="text">Document text.</param>
        /// <param name="position">Index of character within <paramref name="text"/> to look at.</param>
        /// <returns>Length of line break character sequence at <paramref name="position"/>. In case of no line break, <c>0</c> is returned.</returns>
        public static int LengthOfLineBreak(string text, int position) => LengthOfLineBreak(text.AsSpan(), position);

        /// <summary>
        /// Gets length of line break character sequence if any.
        /// </summary>
        /// <remarks>See <see cref="LengthOfLineBreak(string, int)"/>.</remarks>
        public static int LengthOfLineBreak(ReadOnlySpan<char> text, int position)
        {
            var c = text[position];
            if (c == '\r')
            {
                // \r
                if (++position >= text.Length || text[position] != '\n')
                    return 1;

                // \r\n
                return 2;
            }
            else
            {
                // \n
                // unicode line breaks
                if (c == '\n' || c == '\u0085' || c == '\u2028' || c == '\u2029')
                    return 1;

                return 0;
            }
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
                throw new ArgumentNullException("lineBreaks");

            if (line < 0 || line > lineBreaks.Count)
                throw new ArgumentException("line");

            int start = (line != 0) ? lineBreaks.EndOfLineBreak(line - 1) : 0;
            int end = (line < lineBreaks.Count) ? lineBreaks.EndOfLineBreak(line) : lineBreaks.TextLength;

            return Span.FromBounds(start, end);
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
    }

    #endregion
}
