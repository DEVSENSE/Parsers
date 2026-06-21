using Devsense.PHP.Errors;
using Devsense.PHP.Text;
using Devsense.PHP.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Methods for decoding double quoted and single quoted source string values.
    /// </summary>
    internal static class PhpStringSyntax
    {
        /// <summary>
        /// Delegate used for <see cref="ProcessStringText"/> that handles escaped string sequences.
        /// </summary>
        /// <param name="source">Source text buffer.</param>
        /// <param name="index">Index of the escaped character right after '\'.</param>
        /// <param name="builder">Output string builder.</param>
        /// <returns>Position of the last processed character. In case the escaped character is not handled, gets <c>-1</c>.</returns>
        internal delegate int ProcessStringDelegate(ReadOnlySpan<char> source, int index, PhpStringBuilder builder);

        /// <summary>
        /// <see cref="ProcessStringDelegate"/> for single quoted strings.
        /// </summary>
        internal readonly static ProcessStringDelegate s_processSingleQuotedString = (ReadOnlySpan<char> buffer, int pos, PhpStringBuilder result) =>
        {
            var c = buffer[pos];
            if (c == '\\' || c == '\'')
            {
                result.Append(c);
                return pos;
            }

            return -1;
        };

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="pos"></param>
        /// <param name="result"></param>
        /// <param name="token_start">For error reporting.</param>
        /// <param name="errors">Optional. For error reporting.</param>
        /// <param name="ST_IN_SHELL">If it's backquoted string.</param>
        /// <param name="UTF_CODEPOINTS">If UTF code points are enabled.</param>
        /// <returns></returns>
        internal static int ProcessDoubleQuotedStringImpl(ReadOnlySpan<char> buffer, int pos, PhpStringBuilder result, int token_start, IErrorSink<Span> errors, bool ST_IN_SHELL, bool UTF_CODEPOINTS)
        {
            switch (buffer[pos])
            {
                case 'n':
                    result.Append('\n');
                    return pos;

                case 'r':
                    result.Append('\r');
                    return pos;

                case 't':
                    result.Append('\t');
                    return pos;

                case 'v':
                    result.Append('\v');
                    return pos;

                case 'e':
                    result.Append((char)0x1B);
                    return pos;

                case 'f':
                    result.Append('\f');
                    return pos;

                case '\\':
                case '$':
                case '"':
                    result.Append(buffer[pos]);
                    return pos;

                case '`':
                    if (ST_IN_SHELL)
                    {
                        result.Append(buffer[pos]);
                        return pos;
                    }
                    break;

                case 'u':
                    if (UTF_CODEPOINTS && TryParseCodePoint(buffer, ref pos, out var value, token_start, errors))
                    {
                        result.Append(value);
                        return pos;
                    }
                    break;

                case 'x':
                    {
                        // \x[0-9A-Fa-f]{1,2}
                        int digit;
                        if (++pos < buffer.Length && (digit = Convert.AlphaNumericToDigit(buffer[pos])) < 16)
                        {
                            int hex_code = digit;
                            if (++pos < buffer.Length && (digit = Convert.AlphaNumericToDigit(buffer[pos])) < 16)
                            {
                                hex_code = (hex_code << 4) + digit;
                            }
                            else
                            {
                                pos--;  // rollback
                            }

                            result.Append((byte)hex_code);
                            return pos;
                        }

                        break;
                    }

                default:
                    {
                        // \[0-7]{1,3}
                        int digit;
                        if ((digit = Convert.NumericToDigit(buffer[pos])) < 8)  // 1
                        {
                            int octal_code = digit;

                            if (++pos < buffer.Length && (digit = Convert.NumericToDigit(buffer[pos])) < 8)    // 2
                            {
                                octal_code = (octal_code << 3) + digit;

                                if (++pos < buffer.Length && (digit = Convert.NumericToDigit(buffer[pos])) < 8)    // 3
                                {
                                    octal_code = (octal_code << 3) + digit;
                                }
                                else
                                {
                                    pos--; // rollback
                                }
                            }
                            else
                            {
                                pos--; // rollback
                            }

                            result.Append((byte)octal_code);
                            return pos;
                        }

                        break;
                    }
            }

            // not handled:
            return -1;
        }

        /// <summary>
        /// Parse code point enclosed in braces.
        /// 'pos' points before '{'
        /// </summary>
        static bool TryParseCodePoint(ReadOnlySpan<char> buffer, ref int pos, out string value, int token_start, IErrorSink<Span> errors)
        {
            var index = pos + 1; //
            int code_point = 0;
            int digit;

            // {
            if (index < buffer.Length && buffer[index] == '{')
            {
                int ndigits = 0;

                // [0-9A-Fa-f]+}
                while (++index < buffer.Length)
                {
                    var ch = buffer[index];
                    if (ch == '}')
                    {
                        if (ndigits == 0)
                        {
                            // \u{}
                            break;
                        }

                        if ((code_point < 0 || code_point > 0x10ffff) || (code_point >= 0xd800 && code_point <= 0xdfff))
                        {
                            errors.Error(
                                new Span(token_start + pos + 1, index - pos),
                                Errors.Errors.InvalidCodePoint,
                                code_point.ToString("x"));

                            break;
                        }

                        pos = index;
                        value = StringUtils.Utf32ToString(code_point);
                        return true;
                    }
                    else if ((digit = Convert.AlphaNumericToDigit(ch)) < 16)
                    {
                        code_point = (code_point << 4) + digit;
                        ndigits++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //
            value = default;
            return false;
        }

        /// <summary>
        /// Parses string literal source text, processes escaped sequences.
        /// </summary>
        internal static object ProcessStringText(ReadOnlySpan<char> buffer, ProcessStringDelegate tryprocess, bool binary, Encoding encoding, IStringTable strings)
        {
            Debug.Assert(tryprocess != null);

            if (buffer.IsEmpty)
            {
                return string.Empty;
            }

            PhpStringBuilder builder = null;

            // decode the string only if it contains a backslash
            var index = buffer.IndexOf('\\');
            if (index >= 0)
            {
                builder = new PhpStringBuilder(encoding ?? Encoding.UTF8, binary, buffer.Length);
                builder.Append(buffer.Slice(0, index));

                for (; index < buffer.Length; index++)
                {
                    var c = buffer[index];
                    if (c == '\\' && index + 1 < buffer.Length)
                    {
                        int newindex = tryprocess(buffer, index + 1, builder);
                        if (newindex > 0)
                        {
                            Debug.Assert(newindex >= index + 1);
                            index = newindex;
                            continue;
                        }
                    }

                    //
                    builder.Append(c);
                }
            }

            //
            return builder == null
                ? StringInterns.TryIntern(buffer) ?? strings?.GetOrAdd(buffer) ?? buffer.ToString()
                : builder.Result; // .StringResult;
        }

        static object ProcessBackquoteString(ReadOnlySpan<char> buffer, Span span, LanguageFeatures features, IErrorSink<Span> errors, Encoding encoding = null, IStringTable strings = null) =>
            ProcessStringText(
                buffer,
                (buffer, pos, result) => ProcessDoubleQuotedStringImpl(buffer, pos, result, span.Start, errors,
                    ST_IN_SHELL: true,
                    UTF_CODEPOINTS: features.HasUtfCodepoints()
                    ),
                binary: false,
                encoding: encoding,
                strings: strings
            );

        static object ProcessDoublequoteString(ReadOnlySpan<char> buffer, Span span, LanguageFeatures features, IErrorSink<Span> errors, bool binary = false, Encoding encoding = null, IStringTable strings = null) =>
            ProcessStringText(
                buffer,
                (buffer, pos, result) => ProcessDoubleQuotedStringImpl(buffer, pos, result, span.Start, errors,
                    ST_IN_SHELL: false,
                    UTF_CODEPOINTS: features.HasUtfCodepoints()
                    ),
                binary: binary,
                encoding: encoding,
                strings: strings
            );

        static object ProcessSinglequoteString(ReadOnlySpan<char> buffer, Span span, bool binary = false, Encoding encoding = null, IStringTable strings = null) =>
            ProcessStringText(
                buffer,
                s_processSingleQuotedString,
                binary: binary,
                encoding: encoding,
                strings: strings
            );

        internal static object ProcessString(ReadOnlySpan<char> buffer, Span span, LanguageFeatures features, IErrorSink<Span> errors, Tokens quote, bool binary = false, Encoding encoding = null, IStringTable strings = null)
        {
            switch (quote)
            {
                case Tokens.END: // HEREDOC without quotes
                case Tokens.T_DOUBLE_QUOTES:
                    return ProcessDoublequoteString(buffer, span, features, errors, binary, encoding, strings);
                case Tokens.T_SINGLE_QUOTES:
                    return ProcessSinglequoteString(buffer, span, binary, encoding, strings);
                case Tokens.T_BACKQUOTE:
                    return ProcessBackquoteString(buffer, span, features, errors, encoding, strings);
                default:
                    throw new ArgumentOutOfRangeException(nameof(quote));
            }
        }
    }
}
