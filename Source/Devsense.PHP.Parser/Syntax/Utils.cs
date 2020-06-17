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
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Devsense.PHP.Syntax
{
    #region Spans

    /// <summary>
    /// Utilities for span manipulation.
    /// </summary>
    internal static class SpanUtils
    {
        public static Text.Span SafeSpan(int start, int length) => start < 0 || length < 0 ? Text.Span.Invalid : new Text.Span(start, length);

        public static Text.Span SafeCombineSpan(Text.Span previous, Text.Span next)
        {
            if (!previous.IsValid)
            {
                return next;
            }
            else if (!next.IsValid)
            {
                return previous;
            }
            else
            {
                return previous.StartOrInvalid <= next.End ? Text.Span.Combine(previous, next) : Text.Span.Invalid;
            }
        }

        public static Text.Span CombineValid(Text.Span s1, Text.Span s2, Text.Span s3 = default(Text.Span))
        {
            var result = s1;

            if (s2.Length > 0)
            {
                result = result.IsValid ? Text.Span.Combine(result, s2) : s2;
            }

            if (s3.Length > 0)
            {
                result = result.IsValid ? Text.Span.Combine(result, s3) : s3;
            }

            return result;
        }

        public static Text.Span SpanIntermission(Text.Span previous, Text.Span next) =>
            previous.IsValid && next.IsValid && previous.End <= next.Start ? Text.Span.FromBounds(previous.End, next.Start) : Text.Span.Invalid;

        public static Text.Span SpanIntermission(int previous, Text.Span next) =>
            previous >= 0 && next.IsValid && previous <= next.Start ? Text.Span.FromBounds(previous, next.Start) : Text.Span.Invalid;

        public static Text.Span SpanIntermission(Text.Span previous, int next) =>
            previous.IsValid && next >= 0 && previous.End <= next ? Text.Span.FromBounds(previous.End, next) : Text.Span.Invalid;

        public static Text.Span ItemSpan(this Ast.Item item) => SafeCombineSpan(
            item.Index != null ? item.Index.Span : Text.Span.Invalid,
            item.Value.Span);

        public static Text.Span ItemsSpan(this IList<Ast.Item> items)
        {
            Ast.Item first = null;
            Ast.Item last = null;

            for (int i = 0; i < items.Count; i++)
            {
                if ((first = items[i]) != null)
                {
                    for (int j = items.Count - 1; j >= i; j--)
                    {
                        if ((last = items[j]) != null)
                        {
                            return SafeCombineSpan(first.ItemSpan(), last.ItemSpan());
                        }
                    }
                }
            }

            return Text.Span.Invalid;
        }
    }

    #endregion

    #region Strings

    /// <summary>
    /// Unicode category group.
    /// </summary>
    public enum UnicodeCategoryGroup
    {
        None,
        Separators,
        Symbols,
        Punctuations,
        Numbers,
        Marks,
        Letters,
        OtherCharacters
    }

    /// <summary>
    /// Utilities manipulating strings.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Increments a string in a manner of Perl (and also PHP). 
        /// </summary>
        /// <param name="str">The string to be incremented.</param>
        /// <returns>The incremented string. </returns>
        /// <remarks>
        /// <para>Single characters are incremented such that '0' changes to '1', ..., '8' to '9' and '9' to '0' 
        /// with a carry, 'a' changes to 'b', ..., 'z' to 'a' (with a carry) and finally 'A' to 'B', ..., 'Z' to 'A' 
        /// (with a carry). Other characters remain unchanged and generate no carry.</para>
        /// <para>Characters of the <paramref name="str"/> string are incremented starting from the last one
        /// advancing to the beginning of the string and ending when there is no carry generated or no character 
        /// available (i.e. we proceeded the first character of the <paramref name="str"/> string). In latter 
        /// case appropriate character ('1', 'a' or 'A') is prepended before the result.</para>
        /// </remarks>
        public static string Increment(string str)
        {
            if (str == null) return "0";

            int length = str.Length;

            // make a copy of a string and allocate one more character to handler "overflow":
            StringBuilder result = new StringBuilder(str, 0, length, length + 1);

            // if length of the string is zero, then '1' will be returned:
            char c = '9';

            // while there is a carry flag and not all characters of the string 
            // are processed do increment the current character:
            for (int i = length - 1; i >= 0; i--)
            {
                c = str[i];
                if (c >= '0' && c <= '8' || c >= 'a' && c <= 'y' || c >= 'A' && c <= 'Y')
                {
                    result[i] = (char)((int)c + 1);
                    break;
                }
                switch (c)
                {
                    case '9': result[i] = '0'; continue;
                    case 'z': result[i] = 'a'; continue;
                    case 'Z': result[i] = 'A'; continue;
                }
                break;
            }

            // if the last incremented character is '9', 'z' or 'Z' then we must be at the beginning of the string;
            // the string is shifted to the right by one and the first charater is set:
            switch (c)
            {
                case '9': result.Insert(0, new char[] { '1' }); break;
                case 'z': result.Insert(0, new char[] { 'a' }); break;
                case 'Z': result.Insert(0, new char[] { 'A' }); break;
            }

            return result.ToString();
        }

        public static string/*!*/ AddCSlashes(string/*!*/ str)
        {
            return AddCSlashes(str, true, true, true);
        }

        public static string/*!*/ AddCSlashes(string/*!*/ str, bool singleQuotes, bool doubleQuotes)
        {
            return AddCSlashes(str, singleQuotes, doubleQuotes, true);
        }

        /// <summary>
        /// Adds slashes before characters '\\', '\0', '\'' and '"'.
        /// </summary>
        /// <param name="str">The string to add slashes in.</param>
        /// <param name="doubleQuotes">Whether to slash double quotes.</param>
        /// <param name="singleQuotes">Whether to slash single quotes.</param>
        /// <param name="nul">Whether to slash '\0' character.</param>
        /// <returns>The slashed string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is a <B>null</B> reference.</exception>
        public static string/*!*/ AddCSlashes(string/*!*/ str, bool singleQuotes, bool doubleQuotes, bool nul)
        {
            if (str == null) throw new ArgumentNullException("str");

            StringBuilder result = new StringBuilder(str.Length);

            string double_quotes = doubleQuotes ? "\\\"" : "\"";
            string single_quotes = singleQuotes ? @"\'" : "'";
            string slashed_nul = nul ? "\\0" : "\0";

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '\\': result.Append(@"\\"); break;
                    case '\0': result.Append(slashed_nul); break;
                    case '\'': result.Append(single_quotes); break;
                    case '"': result.Append(double_quotes); break;
                    default: result.Append(c); break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Escape characters in toExcape in given string with given escape character.
        /// </summary>
        /// <param name="str">String to escape</param>
        /// <param name="toEscape">Characters to be escaped</param>
        /// <param name="escape">Escape character</param>
        /// <returns>Escaped string.</returns>
        public static string/*!*/ EscapeStringCustom(string/*!*/str, char[]/*!*/toEscape, char escape)
        {
            if (str == null) throw new ArgumentNullException("str");
            if (toEscape == null) throw new ArgumentNullException("toEscape");

            StringBuilder result = new StringBuilder(str.Length);

            Dictionary<char, bool> charsToEscape = new Dictionary<char, bool>(toEscape.Length);
            foreach (char c in toEscape) charsToEscape.Add(c, true);

            foreach (char c in str)
            {
                if (charsToEscape.ContainsKey(c)) result.Append(escape);

                result.Append(c);
            }

            return result.ToString();
        }

        /// <summary>
        /// Strips slashes from a string.
        /// </summary>
        /// <param name="str">String.</param>
        /// <returns>
        /// String where slashes are striped away.
        /// Slashed characters with special meaning ("\0") are replaced with their special value.
        /// </returns>
        public static string/*!*/ StripCSlashes(string/*!*/ str)
        {
            if (str == null) throw new ArgumentNullException("str");
            if (str == "") return "";

            StringBuilder result = new StringBuilder(str.Length);

            int i = 0;
            while (i < str.Length - 1)
            {
                if (str[i] == '\\')
                {
                    if (str[i + 1] == '0')
                        result.Append('\0');
                    else
                        result.Append(str[i + 1]); // PHP strips all slashes, not only quotes and slash

                    i += 2;
                }
                else
                {
                    result.Append(str[i]);
                    i++;
                }
            }
            if (i < str.Length && str[i] != '\\')
                result.Append(str[i]);

            return result.ToString();
        }

        /// <summary>
        /// Adds slash before '\0' character and duplicates apostrophes.
        /// </summary>
        /// <param name="str">The string to add slashes in.</param>
        /// <returns>The slashed string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is a <B>null</B> reference.</exception>
        public static string/*!*/ AddDbSlashes(string/*!*/ str)
        {
            if (str == null) throw new ArgumentNullException("str");

            StringBuilder result = new StringBuilder(str.Length);

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '\0': result.Append('\\'); result.Append('0'); break;
                    case '\'': result.Append('\''); result.Append('\''); break;
                    default: result.Append(c); break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Replaces slashed 0 with null character ('\0') and double apostrophe with single apostrophe. 
        /// </summary>
        /// <param name="str">String.</param>
        /// <returns>String with replaced characters.</returns>
        public static string/*!*/ StripDbSlashes(string/*!*/ str)
        {
            if (str == null) throw new ArgumentNullException("str");

            StringBuilder result = new StringBuilder(str.Length);

            int i = 0;
            while (i < str.Length - 1)
            {
                if (str[i] == '\\' && str[i + 1] == '0')
                {
                    result.Append('\0');
                    i += 2;
                }
                else if (str[i] == '\'' && str[i + 1] == '\'')
                {
                    result.Append('\'');
                    i += 2;
                }
                else
                {
                    result.Append(str[i]);
                    i++;
                }
            }
            if (i < str.Length)
                result.Append(str[i]);

            return result.ToString();
        }

        /// <summary>
        /// Converts a string of bytes into hexadecimal representation.
        /// </summary>
        /// <param name="bytes">The string of bytes.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>Concatenation of hexadecimal values of bytes of <paramref name="bytes"/> separated by <paramref name="separator"/>.</returns>
        public static string BinToHex(byte[] bytes, string separator)
        {
            if (bytes == null) return null;
            if (bytes.Length == 0) return String.Empty;
            if (separator == null) separator = String.Empty;

            int c;
            int length = bytes.Length;
            int sep_length = separator.Length;
            int res_length = length * (2 + sep_length);

            const string hex_digs = "0123456789abcdef";

            // prepares characters which will be appended to the result for each byte:
            char[] chars = new char[2 + sep_length];
            separator.CopyTo(0, chars, 2, sep_length);

            // prepares the result:
            StringBuilder result = new StringBuilder(res_length, res_length);

            // appends characters to the result for each byte:
            for (int i = 0; i < length - 1; i++)
            {
                c = (int)bytes[i];
                chars[0] = hex_digs[(c & 0xf0) >> 4];
                chars[1] = hex_digs[(c & 0x0f)];
                result.Append(chars);
            }

            // the last byte:
            c = (int)bytes[length - 1];
            result.Append(hex_digs[(c & 0xf0) >> 4]);
            result.Append(hex_digs[(c & 0x0f)]);

            return result.ToString();
        }

        /// <summary>
        /// Replaces specified characters in a string with another ones.
        /// </summary>
        /// <param name="str">A string where to do the replacement.</param>
        /// <param name="from">Characters to be replaced.</param>
        /// <param name="to">Characters to replace those in <paramref name="from"/> with.</param>
        /// <remarks>Replaces characters one by one.</remarks>
        internal static string Replace(string str, string from, string to)
        {
            Debug.Assert(from != null && to != null && from.Length == to.Length);

            StringBuilder result = new StringBuilder(str);

            for (int i = 0; i < from.Length; i++)
                result.Replace(from[i], to[i]);

            return result.ToString();
        }

        /// <summary>
        /// Finds an index of the first character in which two specified strings differs.
        /// </summary>
        /// <param name="str1">The first string.</param>
        /// <param name="str2">The second string.</param>
        /// <param name="ignoreCase">Whether to ignore case.</param>
        /// <returns>The index of the character or the length of the shortest string one is substring of the other.</returns>
        public static int FirstDifferent(string str1, string str2, bool ignoreCase)
        {
            // GENERICS: replace where used for StartsWith
            return ignoreCase ? FirstDifferentIgnoreCase(str1, str2) : FirstDifferentCaseSensitive(str1, str2);
        }

        private static int FirstDifferentIgnoreCase(string str1, string str2)
        {
            int length = Math.Min(str1.Length, str2.Length);
            char c1, c2;
            for (int i = 0; i < length; i++)
            {
                // check the characters case insensitively first, ToLower() is expensive
                // initialize the currentCulture lazily, CultureInfo.CurrentCulture is expensive too

                if ((c1 = str1[i]) != (c2 = str2[i]) && (char.ToLowerInvariant(c1) != char.ToLowerInvariant(c2)))
                {
                    return i;
                }
            }
            return length;
        }
        private static int FirstDifferentCaseSensitive(string str1, string str2)
        {
            int length = Math.Min(str1.Length, str2.Length);
            for (int i = 0; i < length; i++)
            {
                if (str1[i] != str2[i])
                {
                    return i;
                }
            }
            return length;
        }

        public static int FirstDifferent(char[] str1, int startIndex1, string str2, int startIndex2, bool ignoreCase)
        {
            int i = startIndex1;
            int j = startIndex2;
            int result = 0;
            int limit = Math.Min(str1.Length - startIndex1, str2.Length - startIndex2);
            if (ignoreCase)
            {
                while (result < limit)
                {
                    if (Char.ToLower(str1[i++]) != Char.ToLower(str2[j++])) return result;
                    result++;
                }
            }
            else
            {
                while (result < limit)
                {
                    if (str1[i++] != str2[j++]) return result;
                    result++;
                }
            }
            return result;
        }

        //internal static void StringBuilderAppend(PHP.Syntax.PhpStringBuilder/*!*/ dst, StringBuilder/*!*/ src, int startIndex, int length, Text.Span span)
        //{
        //    dst.Append(src.ToString(startIndex, length), span);
        //}

        public static bool IsAsciiString(string/*!*/ str)
        {
            return IsAsciiString(str, 0, str.Length);
        }

        public static bool IsAsciiString(string/*!*/ str, int start, int length)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (start < 0)
                throw new ArgumentOutOfRangeException("start");
            if (length < 0 || length > str.Length - start)
                throw new ArgumentOutOfRangeException("length");

            for (int i = start; i < start + length; i++)
            {
                if (str[i] > (char)127)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Adds characters of a Unicode designation parsed from the specified string.
        /// </summary>
        /// <param name="str">String containing the property.</param>
        /// <param name="pos">Position where the property code starts.</param>
        /// <param name="group">Group.</param>
        /// <param name="category">Category.</param>
        /// <returns>Length of the parsed property code (0 to 2).</returns>
        public static int ParseUnicodeDesignation(string/*!*/ str, int pos, out UnicodeCategoryGroup group,
          out UnicodeCategory category)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (pos < 0)
                throw new ArgumentOutOfRangeException("pos");

            category = 0;
            group = (UnicodeCategoryGroup)'\0';

            if (pos == str.Length) return 0;

            switch (str[pos])
            {
                case 'C': // Other 
                    if (pos + 1 < str.Length)
                    {
                        switch (str[pos + 1])
                        {
                            case 'c': category = UnicodeCategory.Control; return 2;
                            case 'f': category = UnicodeCategory.Format; return 2;
                            case 'n': category = UnicodeCategory.OtherNotAssigned; return 2;
                            case 'o': category = UnicodeCategory.PrivateUse; return 2;
                            case 's': category = UnicodeCategory.Surrogate; return 2;
                        }
                    }
                    group = UnicodeCategoryGroup.OtherCharacters;
                    return 1;

                case 'L': // Letter 
                    if (pos + 1 < str.Length)
                    {
                        switch (str[pos + 1])
                        {
                            case 'l': category = UnicodeCategory.LowercaseLetter; return 2;
                            case 'm': category = UnicodeCategory.ModifierLetter; return 2;
                            case 'o': category = UnicodeCategory.OtherLetter; return 2;
                            case 't': category = UnicodeCategory.TitlecaseLetter; return 2;
                            case 'u': category = UnicodeCategory.UppercaseLetter; return 2;
                        }
                    }
                    group = UnicodeCategoryGroup.Letters;
                    return 1;

                case 'M': // Mark 
                    if (pos + 1 < str.Length)
                    {
                        switch (str[pos + 1])
                        {
                            case 'c': category = UnicodeCategory.SpacingCombiningMark; return 2;
                            case 'e': category = UnicodeCategory.EnclosingMark; return 2;
                            case 'n': category = UnicodeCategory.NonSpacingMark; return 2;
                        }
                    }
                    group = UnicodeCategoryGroup.Marks;
                    return 1;

                case 'N': // Number 
                    if (pos + 1 < str.Length)
                    {
                        switch (str[pos + 1])
                        {
                            case 'd': category = UnicodeCategory.DecimalDigitNumber; return 2;
                            case 'l': category = UnicodeCategory.LetterNumber; return 2;
                            case 'o': category = UnicodeCategory.OtherNumber; return 2;
                        }
                    }
                    group = UnicodeCategoryGroup.Numbers;
                    return 1;

                case 'P': // Punctuation 
                    if (pos + 1 < str.Length)
                    {
                        switch (str[pos + 1])
                        {
                            case 'c': category = UnicodeCategory.ConnectorPunctuation; return 2;
                            case 'd': category = UnicodeCategory.DashPunctuation; return 2;
                            case 'e': category = UnicodeCategory.ClosePunctuation; return 2;
                            case 'f': category = UnicodeCategory.FinalQuotePunctuation; return 2;
                            case 'i': category = UnicodeCategory.InitialQuotePunctuation; return 2;
                            case 'o': category = UnicodeCategory.OtherPunctuation; return 2;
                            case 's': category = UnicodeCategory.OpenPunctuation; return 2;
                        }
                    }
                    group = UnicodeCategoryGroup.Punctuations;
                    return 1;

                case 'S': // Symbol 
                    if (pos + 1 < str.Length)
                    {
                        switch (str[pos + 1])
                        {
                            case 'c': category = UnicodeCategory.CurrencySymbol; return 2;
                            case 'k': category = UnicodeCategory.ModifierSymbol; return 2;
                            case 'm': category = UnicodeCategory.MathSymbol; return 2;
                            case 'o': category = UnicodeCategory.OtherSymbol; return 2;
                        }
                    }
                    group = UnicodeCategoryGroup.Symbols;
                    return 1;

                case 'Z': // Separator 
                    if (pos + 1 < str.Length)
                    {
                        switch (str[pos + 1])
                        {
                            case 'l': category = UnicodeCategory.LineSeparator; return 2;
                            case 'p': category = UnicodeCategory.ParagraphSeparator; return 2;
                            case 's': category = UnicodeCategory.SpaceSeparator; return 2;
                        }
                    }
                    group = UnicodeCategoryGroup.Separators;
                    return 1;
            }
            return 0;
        }

        internal static bool IsWhitespace(string/*!*/ str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsWhiteSpace(str[i]))
                    return false;
            }
            return true;
        }

        internal static bool IsWhiteSpace(byte b)
        {
            return b == ' ' || (b >= '\t' && b <= '\r') || b == '\u00a0' || b == '\u0085';
        }

        /// <summary>
        /// Array of characters representing a new line.
        /// </summary>
        public static readonly char[] s_NewlineCharacters = new[] { '\r', '\n', (char)0x2028, (char)0x2029 };

        /// <summary>
        /// Checks whether given character is considered as a new line character.
        /// </summary>
        public static bool IsNewLineCharacter(char ch)
        {
            return ch == '\r' || ch == '\n' || ch == (char)0x2028 || ch == (char)0x2029;
        }

        /// <summary>
        /// Checks if binary data can be converted a number
        /// </summary>
        /// <param name="bytes">The bytes to checked.</param>
        /// <returns>Returns true if bytes can be converted to a number.</returns>
        internal static bool IsConvertableToNumber(byte[] bytes)
        {
            int state = 0;
            byte b;

            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i];

                switch (state)
                {
                    case 0: // expecting whitespaces to be skipped
                        {
                            if (!IsWhiteSpace(b))
                            {
                                state = 1;
                                goto case 1;
                            }
                            break;
                        }

                    case 1:
                        {

                            if (b >= '0' && b <= '9')
                            {
                                //it's a number
                                return true;
                            }

                            if (b == '-')//sign
                            {
                                state = 2;
                                break;
                            }

                            if (b == '+')//sign
                            {
                                state = 2;
                                break;
                            }

                            // switch to decimals in next turn:
                            if (b == '.')
                            {
                                state = 2;
                                break;
                            }

                            //it's not a valid number
                            return false;

                        }

                    case 2:
                        {
                            if (b >= '0' && b <= '9')
                            {
                                //it's a number
                                return true;
                            }

                            //it's not a valid number
                            return false;
                        }
                }
            }

            return false;
        }

        /// <summary>
        /// <see cref="Char.ConvertFromUtf32"/> is missing from Mono so we must implement it by ourselves.
        /// </summary>
        internal static string Utf32ToString(int codePoint)
        {
            return char.ConvertFromUtf32(codePoint);

            //// MONO BUG:
            //// replace by Char.ConvertFromUtf32 when implemented in Mono

            //if (codePoint < 0x10000)
            //    return char.ToString((char)((ushort)codePoint));

            //codePoint -= 0x10000;
            //return new string(new char[] { (char)((codePoint / 0x400) + 0xd800), (char)((codePoint % 0x400) + 0xdc00) });
        }

        #region Uniform Wrappers

        // NOTE: It is not multibyte safe to work with these wrappers.

        public abstract class UniformWrapper
        {
            public abstract char this[int index] { get; }
            public abstract int Length { get; }
            public abstract object/*!*/ Value { get; }

            public virtual bool HasBuilder { get { return false; } }

            public virtual UniformBuilder GetBuilder()
            {
                throw new NotSupportedException();
            }

            public virtual UniformBuilder GetBuilder(int capacity)
            {
                throw new NotSupportedException();
            }

            #region Implicit Casts

            public static implicit operator UniformWrapper(string str)
            {
                return (str != null) ? new StringWrapper(str) : null;
            }

            public static implicit operator UniformWrapper(byte[] bytes)
            {
                return (bytes != null) ? new BytesWrapper(bytes) : null;
            }

            public static implicit operator UniformWrapper(char[] chars)
            {
                return (chars != null) ? new CharsWrapper(chars) : null;
            }

            public static implicit operator UniformWrapper(StringBuilder builder)
            {
                return (builder != null) ? new StringBuilderWrapper(builder) : null;
            }

            public static implicit operator string(UniformWrapper wrapper)
            {
                return (wrapper != null) ? (string)wrapper.Value : null;
            }

            public static implicit operator byte[] (UniformWrapper wrapper)
            {
                return (wrapper != null) ? (byte[])wrapper.Value : null;
            }

            public static implicit operator char[] (UniformWrapper wrapper)
            {
                return (wrapper != null) ? (char[])wrapper.Value : null;
            }

            public static implicit operator StringBuilder(UniformWrapper wrapper)
            {
                return (wrapper != null) ? (StringBuilder)wrapper.Value : null;
            }

            #endregion
        }

        public abstract class UniformBuilder
        {
            public abstract UniformWrapper ToWrapper();
            public abstract void Append(char c);

            public void Append(string s)
            {
                foreach (char c in s)
                    Append(c);
            }
        }

        public sealed class BytesWrapper : UniformWrapper
        {
            private byte[]/*!*/ bytes;

            public override object Value
            {
                get { return bytes; }
            }

            public override char this[int index]
            {
                get { return (char)bytes[index]; }
            }

            public override int Length
            {
                get { return bytes.Length; }
            }

            public override bool HasBuilder { get { return true; } }

            public BytesWrapper(byte[]/*!*/ bytes)
            {
                Debug.Assert(bytes != null);
                this.bytes = bytes;
            }

            public override UniformBuilder GetBuilder()
            {
                return new Builder(new MemoryStream());
            }

            public override UniformBuilder GetBuilder(int capacity)
            {
                return new Builder(new MemoryStream(capacity));
            }

            #region Nested Class: Builder

            public sealed class Builder : UniformBuilder
            {
                private MemoryStream stream;

                public Builder(MemoryStream stream)
                {
                    this.stream = stream;
                }

                public override void Append(char c)
                {
                    stream.WriteByte((byte)c);
                }

                public override UniformWrapper ToWrapper()
                {
                    return new BytesWrapper(stream.ToArray());
                }
            }

            #endregion
        }

        public sealed class CharsWrapper : UniformWrapper
        {
            private char[]/*!*/ chars;

            public override object Value
            {
                get { return chars; }
            }

            public override char this[int index] { get { return chars[index]; } }

            public override int Length { get { return chars.Length; } }

            public CharsWrapper(char[]/*!*/ chars)
            {
                Debug.Assert(chars != null);
                this.chars = chars;
            }
        }

        public sealed class StringWrapper : UniformWrapper
        {
            private string/*!*/ str;

            public override char this[int index] { get { return str[index]; } }

            public override int Length { get { return str.Length; } }

            public override bool HasBuilder { get { return true; } }

            public override object Value
            {
                get { return str; }
            }

            public StringWrapper(string str)
            {
                Debug.Assert(str != null);
                this.str = str;
            }

            public override UniformBuilder GetBuilder()
            {
                return new Builder(new StringBuilder());
            }

            public override UniformBuilder GetBuilder(int capacity)
            {
                return new Builder(new StringBuilder(capacity));
            }

            #region Nested Class: Builder

            public sealed class Builder : UniformBuilder
            {
                private System.Text.StringBuilder builder;

                public Builder(System.Text.StringBuilder builder)
                {
                    this.builder = builder;
                }

                public override void Append(char c)
                {
                    builder.Append(c);
                }

                public override UniformWrapper ToWrapper()
                {
                    return new StringWrapper(builder.ToString());
                }
            }

            #endregion
        }

        public sealed class StringBuilderWrapper : UniformWrapper
        {
            private StringBuilder/*!*/ builder;

            public override char this[int index] { get { return builder[index]; } }

            public override int Length { get { return builder.Length; } }

            public StringBuilderWrapper(StringBuilder builder)
            {
                Debug.Assert(builder != null);
                this.builder = builder;
            }

            public override object Value
            {
                get { return builder; }
            }
        }

        #endregion

        internal static string ToClsCompliantIdentifier(string/*!*/ name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            StringBuilder result = new StringBuilder(name.Length);

            if (name[0] >= 'a' && name[0] <= 'z' || name[0] >= 'A' && name[0] <= 'Z' || name[0] == '_')
                result.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                if (name[i] >= '0' && name[i] <= '9' || name[i] >= 'a' && name[i] <= 'z' || name[i] >= 'A' && name[i] <= 'Z'
                    || name[i] == '_')
                {
                    result.Append(name[i]);
                }
            }

            return (result.Length > 0) ? result.ToString() : "_";
        }

        /// <summary>
        /// Compare two strings ordinally (which is ok for ascii strings) case insensitively.
        /// </summary>
        /// <param name="self">First string.</param>
        /// <param name="str">Second string.</param>
        /// <returns>True iff two given strings are equals when using <see cref="StringComparison.OrdinalIgnoreCase"/>.</returns>
        public static bool EqualsOrdinalIgnoreCase(this string self, string str)
        {
            return string.Equals(self, str, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Converts the specified character to uppercase.
        /// Changes only characters between 'a' and 'z'.
        /// </summary>
        public static char ToUpperAsciiInvariant(this char c)
        {
            if ('a' <= c && c <= 'z')
            {
                c = (char)((int)c & -33);
            }
            return c;
        }

        /// <summary>
        /// Returns last character of string or -1 if empty
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Last character of string or -1 if empty</returns>
        public static int LastCharacter(this string/*!*/ str)
        {
            return str.Length == 0 ? -1 : str[str.Length - 1];
        }

        /// <summary>
        /// Counts characters within the string.
        /// </summary>
        public static int CharsCount(this string str, char c)
        {
            if (str == null)
                return 0;

            int count = 0;
            for (int i = 0; i < str.Length; i++)
                if (str[i] == c)
                    count++;

            return count;
        }

        /// <summary>
        /// Replaces specified portion of <see cref="StringBuilder"/> by <paramref name="replacement"/>.
        /// </summary>
        /// <param name="str"><see cref="StringBuilder"/> instance.</param>
        /// <param name="startIndex">Where to start replacing.</param>
        /// <param name="length">Amount of characters to replace.</param>
        /// <param name="replacement">Replacement.</param>
        /// <returns>Reference to <paramref name="str"/>.</returns>
        public static StringBuilder/*!*/Replace(this StringBuilder/*!*/str, int startIndex, int length, string replacement)
        {
            if (string.IsNullOrEmpty(replacement))
            {
                str.Remove(startIndex, length);
            }
            else if (length == 1 && replacement.Length == 1)
            {
                str[startIndex] = replacement[0];
            }
            else
            {
                str.Remove(startIndex, length);
                str.Insert(startIndex, replacement);
            }

            return str;
        }
    }

    #endregion

    #region CollectionUtils, ListUtils

    /// <summary>
    /// A few useful methods working with collections.
    /// </summary>
    public static class CollectionUtils
    {
        /// <summary>
        /// Determines whether a specified collection of strings contains a string.
        /// </summary>
        /// <param name="collection">The enumerable collection of strings.</param>
        /// <param name="str">The string to search for.</param>
        /// <param name="ignoreCase">Whether to compare case-insensitively.</param>
        /// <returns>Whether the collection contains <paramref name="str"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is a <B>null</B> reference.</exception>
        /// <exception cref="InvalidCastException"><paramref name="collection"/> contains a non-string.</exception>
        public static bool ContainsString(IEnumerable/*!*/ collection, string str, bool ignoreCase)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (string item in collection)
            {
                if (String.Compare(item, str, StringComparison.CurrentCultureIgnoreCase) == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an index of the last set bit in a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to search in.</param>
        /// <returns>The index of the last bit which is set.</returns>
        public static int GetLastSet(BitArray bitmap)
        {
            int result = bitmap.Length - 1;
            while (result >= 0 && !bitmap[result]) result--;
            return result;
        }

        public static int IncrementValue<TKey>(Dictionary<TKey, int>/*!*/ dictionary, TKey key, int amount)
        {
            int value = 0;
            dictionary.TryGetValue(key, out value);
            dictionary[key] = value + 1;
            return value;
        }

        /// <summary>
        /// Creates dictionary from two enumerators.
        /// </summary>
        public static IDictionary<T, K> JoinDictionary<T, K>(IEnumerable<T> keys, IEnumerable<K> values)
        {
            Dictionary<T, K> ret = new Dictionary<T, K>();
            IEnumerator<T> ek = keys.GetEnumerator();
            IEnumerator<K> ev = values.GetEnumerator();

            bool en, vn;
            while ((en = ek.MoveNext()) == (vn = ev.MoveNext()))
            {
                if (!en) return ret;
                ret.Add(ek.Current, ev.Current);
            }
            throw new InvalidOperationException("Joining collections of incompatible size!");
        }

        /// <summary>
        /// Creates dictionary with all items from the <paramref name="values"/> collection. 
        /// The key of returned dictionary is list of values of type <typeparamref name="K"/>.
        /// </summary>
        public static IDictionary<T, IList<K>> BuildListDictionary<T, K>(IEnumerable<T> keys, IEnumerable<K> values)
        {
            Dictionary<T, IList<K>> ret = new Dictionary<T, IList<K>>();
            IEnumerator<T> ek = keys.GetEnumerator();
            IEnumerator<K> ev = values.GetEnumerator();

            bool en, vn;
            while ((en = ek.MoveNext()) == (vn = ev.MoveNext()))
            {
                if (!en) return ret;

                IList<K> tmp;
                if (!ret.TryGetValue(ek.Current, out tmp))
                    ret.Add(ek.Current, tmp = new List<K>());
                tmp.Add(ev.Current);
            }
            throw new InvalidOperationException("Joining collections of incompatible size!");
        }

        /// <summary>
        /// Filters a collection <paramref name="en"/> using a function <paramref name="f"/>
        /// </summary>
        public static IEnumerable<T> Filter<T>(IEnumerable<T> en, Func<T, bool> f)
        {
            foreach (T el in en) if (f(el)) yield return el;
        }

        /// <summary>
        /// Determines whether the collection is not empty.
        /// </summary>
        public static bool Any<T>(this ICollection<T> list)
        {
            return list != null && list.Count != 0;
        }

        /// <summary>
        /// Calls <paramref name="action"/> for each item in <paramref name="enumerable"/>.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="enumerable">Enumeration of items.</param>
        /// <param name="action">Action to be called for each item.</param>
        public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var x in enumerable)
            {
                action(x);
            }
        }

        /// <summary>
        /// Adds <paramref name="entries"/> into <paramref name="target"/>.
        /// </summary>
        /// <typeparam name="K">Dictionary key type.</typeparam>
        /// <typeparam name="V">Dictionary value type.</typeparam>
        /// <param name="target">Target dictionary.</param>
        /// <param name="entries">Entries to be added.</param>
        public static void Add<K,V>(this IDictionary<K,V> target, IDictionary<K, V> entries)
        {
            entries.Foreach(target.Add);
        }
    }

    /// <summary>
    /// Extension methods for <see cref="IList"/>.
    /// </summary>
    public static class ListUtils
    {
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence
        /// within the entire <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">List to search in.</param>
        /// <param name="item">Item to search for.</param>
        /// <param name="comparer">Comparer to be used.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the entire <paramref name="list"/>, if found; otherwise, �1.</returns>
        public static int IndexOf<T>(this IList<T>/*!*/list, T item, IEqualityComparer<T>/*!*/comparer)
        {
            Debug.Assert(list != null);
            Debug.Assert(comparer != null);

            for (int i = 0; i < list.Count; i++)
                if (comparer.Equals(list[i], item))
                    return i;

            return -1;
        }

        /// <summary>
        /// Removes the last item from the list.
        /// </summary>
        /// <typeparam name="T">Type of list items.</typeparam>
        /// <param name="list">LIst to remove from.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="list"/> has no items.</exception>
        public static void RemoveLast<T>(this IList<T>/*!*/list)
        {
            list.RemoveAt(list.Count - 1);
        }

        /// <summary>
        /// Gets the last element of given list.
        /// </summary>
        /// <typeparam name="T">Type of the list elements.</typeparam>
        /// <param name="list">List.</param>
        /// <returns>Last element of given list.</returns>
        public static T Last<T>(this IList<T>/*!*/list)
        {
            return list[list.Count - 1];
        }

        /// <summary>
        /// Determines whether the list is not empty.
        /// </summary>
        public static bool Any<T>(this List<T> list)
        {
            return list != null && list.Count != 0;
        }

        /// <summary>
        /// Copies entries into new array, or gets empty array if the collection is empty.
        /// </summary>
        public static T[]/*!*/AsArray<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is IList<T> list)
            {
                return AsArray(list);
            }
            else if (enumerable is ICollection collection)
            {
                if (collection.Count != 0)
                {
                    var array = new T[collection.Count];
                    collection.CopyTo(array, 0);
                    return array;
                }
                else
                {
                    return EmptyArray<T>.Instance;
                }
            }
            else if (enumerable == null)
            {
                return EmptyArray<T>.Instance;
            }
            else
            {
                return enumerable.ToArray();
            }
        }

        /// <summary>
        /// Copies entries into new array, or gets empty array if the collection is empty.
        /// </summary>
        public static T[]/*!*/AsArray<T>(this IList<T> list)
        {
            T[] result = list as T[];

            if (result == null)
            {
                if (list != null && list.Count != 0)
                {
                    result = new T[list.Count];
                    list.CopyTo(result, 0);
                }
                else
                {
                    result = EmptyArray<T>.Instance;
                }
            }

            return result;
        }

        /// <summary>
        /// Calls <paramref name="action"/> for each item in <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="list">Enumeration of items.</param>
        /// <param name="action">Action to be called for each item.</param>
        public static void Foreach<T>(this IList<T> list, Action<T> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                action(list[i]);
            }
        }

        public static TTarget[] CastToArray<TTarget>(this IEnumerable enumerable)
        {
            if (enumerable is TTarget[] array)
            {
                return array;
            }
            else if (enumerable is ICollection collection)
            {
                return CastToArray<TTarget>(collection);
            }
            else if (enumerable == null)
            {
                return EmptyArray<TTarget>.Instance;
            }
            else
            {
                return enumerable.Cast<TTarget>().ToArray();
            }
        }

        public static TTarget[] CastToArray<TTarget>(this ICollection collection)
        {
            var count = collection.Count;
            if (count != 0)
            {
                var result = new TTarget[count];
                collection.CopyTo(result, 0);
                return result;
            }
            else
            {
                return EmptyArray<TTarget>.Instance;
            }
        }
    }

    #endregion

    #region Arrays

    /// <summary>
    /// Helper for an empty array instance.
    /// </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
    public static class EmptyArray<T>
    {
        /// <summary>
        /// Singleton instance of empty array of <typeparamref name="T"/>.
        /// </summary>
        public static T[]/*!*/Instance
        {
            get
            {
                if (_instance == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _instance, new T[0], null);
                }

                return _instance;
            }
        }
        private static volatile T[] _instance;
    }

    /// <summary>
    /// Utilities manipulating arrays.
    /// </summary>
    [DebuggerNonUserCode]
    public static class ArrayUtils
    {
        /// <summary>
        /// Gets empty array singleton of <typeparamref name="T"/>.
        /// Cannot be <c>null</c>.
        /// </summary>
        /// <remarks>To be used on platforms where we don't have <c>Array.Empty&lt;T&gt;()</c>.</remarks>
        public static T[] Empty<T>() => EmptyArray<T>.Instance;

        /// <summary>
        /// Empty int array.
        /// </summary>
        public static int[] EmptyIntegers { get { return EmptyArray<int>.Instance; } }

        /// <summary>
        /// Empty ushort array.
        /// </summary>
        public static ushort[] EmptyUShorts { get { return EmptyArray<ushort>.Instance; } }

        /// <summary>
        /// Empty object array.
        /// </summary>
        public static object[] EmptyObjects { get { return EmptyArray<object>.Instance; } }

        /// <summary>
        /// Empty byte array.
        /// </summary>
        public static byte[] EmptyBytes { get { return EmptyArray<byte>.Instance; } }

        /// <summary>
        /// Empty <see cref="string"/> array.
        /// </summary>
        public static string[] EmptyStrings { get { return EmptyArray<string>.Instance; } }

        /// <summary>
        /// Converts a <see cref="IList"/> to an array of strings.
        /// </summary>
        /// <param name="list">The list of strings.</param>
        /// <returns>The array of strings.</returns>
        /// <exception cref="InvalidCastException">An item of <paramref name="list"/> is not a string.</exception>
        public static string[] ToStringArray(IList list)
        {
            if (list == null || list.Count == 0) return ArrayUtils.EmptyStrings;

            string[] result = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
                result[i] = (string)list[i];

            return result;
        }

        /// <summary>
        /// Searches for specified character in sorted array of characters.
        /// </summary>
        /// <param name="array">The array to search in.</param>
        /// <param name="c">The character to search for.</param>
        /// <returns>The position of the <paramref name="c"/> in <paramref name="array"/> or -1 if not found.</returns>
        public static int BinarySearch(char[] array, char c)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            int i = 0;
            int j = array.Length - 1;
            while (i < j)
            {
                int m = (i + j) >> 1;
                char cm = array[m];
                if (c == cm) return m;

                if (c > cm)
                {
                    i = m + 1;
                }
                else
                {
                    j = m - 1;
                }
            }
            return (array[i] == c) ? i : -1;
        }

        /// <summary>
        /// Create copy of given array without the last item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static T[] RemoveLast<T>(T[] array)
        {
            T[] array2 = new T[array.Length - 1];
            Array.Copy(array, 0, array2, 0, array2.Length);
            return array2;
        }

        /// <summary>
        /// Compares two IEquatable objects. They can be null, the method will safely checks the references first.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool Equals<T>(T x, T y) where T : IEquatable<T>
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (object.ReferenceEquals(x, null) ^ object.ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        /// <summary>
        /// Compares two arrays of objects of type T. The method returns true if array are the same reference or they have the same length and single values matches.
        /// </summary>
        /// <typeparam name="T">Type of elements of arrays.</typeparam>
        /// <param name="x">First array</param>
        /// <param name="y">Second array.</param>
        /// <returns>True if arrays contains same objects, compared using <c>IEquatable.Equals</c>.</returns>
        public static bool Equals<T>(T[] x, T[] y) where T : IEquatable<T>
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (object.ReferenceEquals(x, null) ^ object.ReferenceEquals(y, null))
                return false;

            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; ++i)
                if (!Equals<T>(x[i], y[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Compare arrays of <see cref="Type"/> reference.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> references are equal or single elements matches.</returns>
        public static bool Equals(Type[] x, Type[] y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (object.ReferenceEquals(x, null) ^ object.ReferenceEquals(y, null))
                return false;

            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; ++i)
                if (x[i] != y[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Returns an array of indices of items in an array which are equal to or not equal to the specified value.
        /// </summary>
        /// <param name="bytes">The array of values. Assumes that length of the array is less or equal to 256.</param>
        /// <param name="value">The filtered value.</param>
        /// <param name="inequality">Determines whether to compare for inequality or equality.</param>
        /// <returns>The array of indices.</returns>
        internal static byte[] GetValueIndices(int[] bytes, int value, bool inequality)
        {
            Debug.Assert(bytes.Length <= 256);
            Debug.Assert(bytes != null);

            int length = bytes.Length;

            // computes new array's length:
            int count = 0;
            for (int i = 0; i < length; i++)
                if (bytes[i] == value) count++;
            if (inequality) count = length - count;

            // creates new array:
            byte[] result = new byte[count];

            // fills new array:
            if (!inequality)
            {
                for (int i = 0, j = 0; i < length; i++)
                    if (bytes[i] == value)
                        result[j++] = (byte)i;

            }
            else
            {
                for (int i = 0, j = 0; i < length; i++)
                    if (bytes[i] != value)
                        result[j++] = (byte)i;
            }

            return result;
        }

        /// <summary>
        /// Concats two arrays of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="x">The first array of <typeparamref name="T"/> to be concatenated.</param>
        /// <param name="y">The second array of <typeparamref name="T"/> to be concatenated.</param>
        /// <returns>The concatenation of <paramref name="x"/> and <paramref name="y"/>.</returns>
        public static T[]/*!*/ Concat<T>(T[]/*!*/ x, T[]/*!*/ y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");

            T[] result = new T[x.Length + y.Length];

            Array.Copy(x, 0, result, 0, x.Length);
            Array.Copy(y, 0, result, x.Length, y.Length);

            return result;
        }

        /// <summary>
        /// Concats array of <typeparamref name="T"/> with single <typeparamref name="T"/> element.
        /// </summary>
        /// <param name="x">The array of <typeparamref name="T"/> to be concatenated.</param>
        /// <param name="y">The element of <typeparamref name="T"/> to be appended.</param>
        /// <returns>The concatenation of <paramref name="x"/> and <paramref name="y"/>.</returns>
        public static T[]/*!*/ Concat<T>(T[] x, T y)
        {
            if (x == null || x.Length == 0)
                return new T[] { y };

            if (x.Length == 1)
                return new T[] { x[0], y };

            //
            T[] result = new T[x.Length + 1];

            Array.Copy(x, 0, result, 0, x.Length);
            result[x.Length] = y;

            return result;
        }

        /// <summary>
        /// Concats array of <typeparamref name="T"/> with single <typeparamref name="T"/> element.
        /// </summary>
        public static T[]/*!*/Concat<T>(T x, T[] y)
        {
            T[] result;

            if (y.Any())
            {
                result = new T[1 + y.Length];
                result[0] = x;
                Array.Copy(y, 0, result, 1, y.Length);
            }
            else
            {
                result = new T[] { x };
            }

            return result;
        }

        /// <summary>
        /// Concats two arrays of bytes.
        /// </summary>
        /// <param name="x">The first array of bytes to be concatenated.</param>
        /// <param name="y">The second array of bytes to be concatenated.</param>
        /// <returns>The concatenation of <paramref name="x"/> and <paramref name="y"/>.</returns>
        public static byte[]/*!*/ Concat(byte[]/*!*/ x, byte[]/*!*/ y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");

            byte[] result = new byte[x.Length + y.Length];

            Buffer.BlockCopy(x, 0, result, 0, x.Length);
            Buffer.BlockCopy(y, 0, result, x.Length, y.Length);

            return result;
        }

        internal static T[]/*!*/ Filter<T>(T[]/*!*/ srcArray, T[]/*!*/ dstArray, T removedValue)
            where T : class
        {
            int j = 0;
            for (int i = 0; i < srcArray.Length; i++)
            {
                if (!ReferenceEquals(srcArray[i], removedValue))
                    dstArray[j++] = srcArray[i];
            }

            return dstArray;
        }

        internal static int IndexOfNull<T>(ref T[]/*!*/ array, int start)
            where T : class
        {
            while (start < array.Length && array[start] != null) start++;

            if (start == array.Length)
                Array.Resize(ref array, (array.Length + 1) * 2);

            return start;
        }

        public static string/*!*/ ToList<T>(IEnumerable<T> enumerable, Action<StringBuilder, T>/*!*/ appendItem)
        {
            if (appendItem == null)
                throw new ArgumentNullException("appendItem");

            if (enumerable == null) return "";

            StringBuilder result = new StringBuilder();

            bool first = true;
            foreach (T item in enumerable)
            {
                if (!first) result.Append(',');
                first = false;

                appendItem(result, item);
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns number of starting elements satisfying given predicate.
        /// </summary>
        /// <param name="args">Array of objects.</param>
        /// <param name="predicate">Condition.</param>
        /// <returns>Amount of elements.</returns>
        public static int TakeWhileCount(object[]/*!*/args, Predicate<object> predicate)
        {
            Debug.Assert(args != null);

            int i = 0;

            for (; i < args.Length; ++i)
                if (!predicate(args[i]))
                    return i;

            return i;
        }

        /// <summary>
        /// Creates list without duplicities from given <c>items</c>.
        /// </summary>
        /// <typeparam name="T">Type of single items in the list.</typeparam>
        /// <param name="items">Items to check for duplicities.</param>
        /// <returns>New list of unique items. Cannot return null.</returns>
        public static ICollection<T>/*!*/Unique<T>(IList<T> items)
        {
            if (items == null || items.Count == 0)
                return EmptyArray<T>.Instance;

            return new HashSet<T>(items);
        }

        /// <summary>
        /// Creates new or resuses given <paramref name="items"/>, returned array contains only unique items.
        /// </summary>
        /// <typeparam name="T">Type of array element.</typeparam>
        /// <param name="items">Array of elements.</param>
        /// <returns>Unique array of element. Cannot be null.</returns>
        public static T[]/*!*/EnsureUnique<T>(T[] items)
        {
            if (items == null) return EmptyArray<T>.Instance;
            if (items.Length == 0) return items;

            var set = new HashSet<T>(items);
            if (set.Count == items.Length)
                return items;
            else
                return System.Linq.Enumerable.ToArray(set);
        }

        /// <summary>
        /// Read all the bytes from input stream to byte array.
        /// </summary>
        /// <param name="input">Input stream.</param>
        /// <returns>Array of bytes read from the input stream.</returns>
        public static byte[] ReadAllBytes(Stream input)
        {
            if (input == null)
                return null;

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Determines whether the array is not empty.
        /// </summary>
        public static bool Any<T>(this T[] arr)
        {
            return arr != null && arr.Length != 0;
        }

        /// <summary>
        /// Determines whether the array is empty or <c>null</c> reference.
        /// </summary>
        [Obsolete("Use IsEmpty<T>() instead.")]
        public static bool Empty<T>(this T[] arr)
        {
            return !Any<T>(arr);
        }

        /// <summary>
        /// Determines whether the array is empty or <c>null</c> reference.
        /// </summary>
        public static bool IsEmpty<T>(this T[] arr) => arr == null || arr.Length == 0;

        /// <summary>
        /// Copies a part of given array into a new one. If the result array would be the same size as the original one, reference to the original one is returned directly.
        /// </summary>
        public static T[] TakeArray<T>(this T[] arr, int from, int count)
        {
            if (arr == null)
                throw new ArgumentNullException();

            if (count == 0)
                return EmptyArray<T>.Instance;

            if (from == 0 && count == arr.Length)
                return arr;

            if (from < 0 || from + count > arr.Length)
                throw new ArgumentOutOfRangeException();

            //
            T[] result = new T[count];
            Array.Copy(arr, from, result, 0, count);
            return result;
        }
    }

    #endregion

    #region CoreResources

    internal static class CoreResources
    {
        internal static string GetString(string messageId)
        {
            return Errors.Strings.ResourceManager.GetString(messageId);
        }

        internal static string GetString(string messageId, params string[] args)
        {
            return string.Format(Errors.Strings.ResourceManager.GetString(messageId), args);
        }
    }

    #endregion
}
