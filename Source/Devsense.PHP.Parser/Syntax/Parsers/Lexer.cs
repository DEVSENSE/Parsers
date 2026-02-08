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
using System.Diagnostics;
using System.Text;
using System.Globalization;

using Devsense.PHP.Text;
using Devsense.PHP.Errors;
using System.Collections.Generic;
using Devsense.PHP.Utilities;
using Devsense.PHP.Ast.DocBlock;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// PHP lexer generated according to the PhpLexer.l grammar.
    /// </summary>
    public partial class Lexer : ITokenProvider<SemanticValueType, Span>, IDisposable
    {
        /// <summary>
        /// Gets value indicating short open tags are enabled.
        /// </summary>
        protected bool EnableShortTags => (_features & LanguageFeatures.ShortOpenTags) != 0;

        /// <summary>
        /// Gets value indicating Unicode codepoints (\u{[0-9A-Fa-f]+}) in double quoted strings are enabled.
        /// </summary>
        protected bool EnableUtfCodepoints => (_features & LanguageFeatures.Php70Set) == LanguageFeatures.Php70Set;

        readonly LanguageFeatures _features;

        /// <summary>
        /// Semantic value of the actual token
        /// </summary>
        SemanticValueType _tokenSemantics;

        /// <summary>
        /// Position of the actual token
        /// </summary>
        private Span _tokenPosition;

        /// <summary>
        /// Token string, updated lazily by <see cref="TokenText"/>.
        /// </summary>
        private string _tokenText;

        /// <summary>
        /// Enxoding used to converted between single-byte strings and Unicode strings.
        /// </summary>
        private readonly Encoding/*!*/ _encoding;

        /// <summary>
        /// Sink used to report lexical errors.
        /// </summary>
        readonly IErrorSink<Span> _errors;

        /// <summary>
        /// Factory of documentary comment blocks.
        /// </summary>
        readonly IDocBlockFactory<Span> _docblockFactory;

        private IStringTable _strings;

        private StringTable _private_strings;

        /// <summary>
        /// Token position
        /// </summary>
        private int _charOffset = 0;

        /// <summary>
        /// Information about the expected heredoc end token.
        /// </summary>
        internal HereDocTokenValue _hereDocValue = null;

        /// <summary>
        /// Gets current doc comment.
        /// </summary>
        public IDocBlock DocComment { get; set; }

        void SetDocBlock() => DocComment = _docblockFactory.CreateDocBlock(new Span(_charOffset, this.TokenLength), GetTokenString(intern: false));    // TokenPosition is not updated yet at this point
        void ResetDocBlock() => DocComment = null;

        /// <summary>
        /// Lexer constructor that initializes all the necessary members
        /// </summary>
        /// <param name="source">Source code.</param>
        /// <param name="encoding">Source file encoding to convert UTF characters.</param>
        /// <param name="errors">Error sink used to report lexical error.</param>
        /// <param name="features">Allow or disable short oppening tags for PHP.</param>
        /// <param name="docBlockFactory">Factory for documentary comment semantic elements.</param>
        /// <param name="positionShift">Starting position of the first token, used during custom restart.</param>
        /// <param name="initialState">Initial state of the lexer, used during custom restart.</param>
        /// <param name="strings">Optionally string table to be used for strings interning. If not specified, temporary instance of <see cref="StringTable"/> will be used.</param>
        public Lexer(
            ReadOnlyMemory<char> source,
            Encoding encoding,
            IErrorSink<Span> errors = null,
            LanguageFeatures features = LanguageFeatures.Basic,
            IDocBlockFactory<Span> docBlockFactory = null,
            int positionShift = 0,
            LexicalStates initialState = LexicalStates.INITIAL,
            IStringTable strings = null)
        {
            _encoding = encoding ?? Encoding.UTF8;
            _errors = errors ?? new EmptyErrorSink<Span>();
            _docblockFactory = docBlockFactory ?? DefaultDocBlockFactory.Instance;
            _charOffset = positionShift;
            _features = features;
            _strings = strings ?? (_private_strings = StringTable.GetInstance());
            _processDoubleQuotedString = ProcessDoubleQuotedString;

            Initialize(source, initialState);
        }

        /// <summary>
        /// Override of <c>Initialize</c> resetting <see cref="_charOffset"/> so <see cref="TokenPosition"/> is correctly shifted.
        /// </summary>
        public void Initialize(ReadOnlyMemory<char> source, LexicalStates lexicalState, bool atBol, int positionShift)
        {
            Initialize(source, lexicalState, atBol);
            _charOffset = positionShift;
        }

        public void Dispose()
        {
            _strings = null;
            _private_strings?.Free();
            _private_strings = null;
        }

        /// <summary>
        /// Updates <see cref="_charOffset"/> and <see cref="_tokenPosition"/>.
        /// </summary>
        public void UpdateTokenPosition()
        {
            int tokenLength = this.TokenLength;

            // update token position info:
            _tokenPosition = new Span(_charOffset, tokenLength);
            _charOffset += tokenLength;
        }

        void ITokenProvider<SemanticValueType, Span>.ReportError(string[] expectedTerminals) => _errors.SyntaxError(GetTokenString(intern: false), _tokenPosition, expectedTerminals);

        int ITokenProvider<SemanticValueType, Span>.GetNextToken() => (int)GetNextToken();

        /// <summary>
        /// Gets next token and updates position within the source.
        /// </summary>
        public Tokens GetNextToken()
        {
            _tokenText = null;

            Tokens token = NextToken();
            UpdateTokenPosition();
            return token;
        }

        public LexicalStates PreviousLexicalState => stateStack.Count != 0 ? yy_top_state() : (LexicalStates)(-1);

        protected void _yymore() { yymore(); }

        private void _yyless(int count)
        {
            lookahead_index = (token_end -= count);
        }

        #region Token Buffer Interpretation

        protected ReadOnlyMemory<char> Source => source_string;

        protected int BufferTokenStart { get { return token_start; } }

        public string Intern(char[] array, int start, int length) => Intern(array.AsSpan(start, length));

        public string Intern(ReadOnlySpan<char> text) => StringInterns.TryIntern(text) ?? _strings.GetOrAdd(text);

        public ReadOnlySpan<char> TokenTextSpan => TokenSource.Span;

        public ReadOnlyMemory<char> TokenSource => source_string.Slice(token_start, TokenLength);

        public string GetText(ReadOnlySpan<char> text, bool intern)
        {
            return
                StringInterns.TryIntern(text) // always try most frequent strings fast
                ?? (intern
                    ? _strings.GetOrAdd(text)
                    : text.ToString()
                );
        }

        public string GetText(int offset, int length, bool intern) => GetText(buffer.Slice(offset, length), intern);

        protected char GetTokenChar(int index) => buffer[token_start + index];

        protected string GetTokenString(bool intern = true) => GetText(TokenTextSpan, intern);

        protected string GetTokenChunkString(bool intern = true)
        {
            return GetText(token_chunk_start, token_end - token_chunk_start, intern);
        }

        protected string GetTokenSubstring(int startIndex, bool intern = true) => GetText(TokenTextSpan.Slice(startIndex), intern);

        protected string GetTokenSubstring(int startIndex, int length, bool intern = true) => GetText(TokenTextSpan.Slice(startIndex, length), intern);

        protected char GetTokenAsEscapedCharacter(int startIndex)
        {
            Debug.Assert(GetTokenChar(startIndex) == '\\');
            char c;
            switch (c = GetTokenChar(startIndex + 1))
            {
                case 'n': return '\n';
                case 't': return '\t';
                case 'r': return '\r';
                default: return c;
            }
        }

        ///// <summary>
        ///// Checks whether {LNUM} fits to integer, long or double 
        ///// and returns appropriate value from Tokens enum.
        ///// </summary>
        //protected Tokens GetIntegerTokenType(int startIndex)
        //{
        //    int i = token_start + startIndex;
        //    while (i < token_end && buffer[i] == '0') i++;

        //    int number_length = token_end - i;
        //    if (i != token_start + startIndex)
        //    {
        //        // starts with zero - octal
        //        // similar to GetHexIntegerTokenType code
        //        if (number_length < 22)
        //            return Tokens.T_LNUMBER;
        //        return Tokens.T_DNUMBER;
        //    }
        //    else
        //    {
        //        // can't easily check for numbers of different length
        //        SemanticValueType val = default(SemanticValueType);
        //        return GetTokenAsDecimalNumber(startIndex, 10, ref val);
        //    }
        //}

        ///// <summary>
        ///// Checks whether {HNUM} fits to integer, long or double 
        ///// and returns appropriate value from Tokens enum.
        ///// </summary>
        //protected Tokens GetHexIntegerTokenType(int startIndex)
        //{
        //    // 0xffffffff no
        //    // 0x7fffffff yes
        //    int i = token_start + startIndex;
        //    while (i < token_end && buffer[i] == '0') i++;

        //    // returns T_LNUMBER when: length without zeros is less than 16
        //    // or equals 16 and first non-zero character is less than '8'
        //    if ((token_end - i < 16) || ((token_end - i == 16) && buffer[i] >= '0' && buffer[i] < '8'))
        //        return Tokens.T_LNUMBER;

        //    return Tokens.T_DNUMBER;
        //}

        //// base == 10: [0-9]*
        //// base == 16: [0-9A-Fa-f]*
        //// assuming result < max int
        //protected int GetTokenAsInteger(int startIndex, int @base)
        //{
        //    int result = 0;
        //    int buffer_pos = token_start + startIndex;

        //    for (; ; )
        //    {
        //        int digit = Convert.AlphaNumericToDigit(buffer[buffer_pos]);
        //        if (digit >= @base) break;

        //        result = result * @base + digit;
        //        buffer_pos++;
        //    }

        //    return result;
        //}

        #region nested struct: DigitsEnumerator // helper enumerator of digits within string, ignores '_'

        ref struct DigitsEnumerator
        {
            readonly ReadOnlySpan<char> buffer;
            readonly int @base;

            int buffer_pos;
            int digit;

            public DigitsEnumerator(char[] buffer, int buffer_pos, int @base) : this(buffer.AsSpan(buffer_pos), @base) { }

            public DigitsEnumerator(ReadOnlySpan<char> buffer, int @base)
            {
                this.buffer = buffer;
                this.buffer_pos = 0;
                this.@base = @base;
                this.digit = 0;
            }

            public bool MoveNext()
            {
                for (; ; )
                {
                    if (buffer_pos >= buffer.Length)
                    {
                        return false;
                    }

                    var ch = buffer[buffer_pos++];
                    if (ch != '_')
                    {
                        return (digit = Convert.AlphaNumericToDigit(ch)) < @base;
                    }
                }
            }

            public int Current => digit;
        }

        #endregion

        /// <summary>
        /// Reads token as a number (accepts tokens with any reasonable base [0-9a-zA-Z]*).
        /// Parsed value is stored in <paramref name="val"/> as integer (when value is less than MaxInt),
        /// as Long (when value is less then MaxLong) or as double.
        /// </summary>
        /// <param name="startIndex">Starting read position of the token.</param>
        /// <param name="base">The base of the number.</param>
        /// <param name="val">Parsed value is stored in this union</param>
        /// <returns>Returns one of T_LNUMBER (int), T_L64NUMBER (long) or T_DNUMBER (double)</returns>
        protected Tokens GetTokenAsDecimalNumber(int startIndex, int @base, ref SemanticValueType val)
        {
            long lresult = 0;
            double dresult;

            // helper enumerator of digits, ignores '_'
            var digits = new DigitsEnumerator(buffer.Slice(token_start + startIndex), @base);

            // try parse Int32
            // most literals fit 32 bit number

            while (lresult < Int32.MaxValue)
            {
                if (digits.MoveNext())
                {
                    lresult = lresult * @base + digits.Current;
                }
                else
                {
                    // NOTE: we don't use val.Integer, all numbers are either Long or Double
                    val.Long = lresult;
                    return Tokens.T_LNUMBER;
                }
            }

            // parse rest of the number
            // (Int64 or overflow to Double)
            while (digits.MoveNext())
            {
                var tmp = lresult;
                lresult = unchecked(lresult * @base + digits.Current);

                // overflow?
                if (lresult < tmp)
                {
                    dresult = (double)tmp * @base + digits.Current;

                    // overflow:
                    while (digits.MoveNext())
                    {
                        dresult = dresult * @base + digits.Current;
                    }

                    val.Double = dresult;
                    return Tokens.T_DNUMBER;
                }
            }

            // did not overflow:
            val.Long = lresult;
            return Tokens.T_LNUMBER;
        }

        // [0-9]*[.][0-9]+
        // [0-9]+[.][0-9]*
        // [0-9]*[.][0-9]+[eE][+-]?[0-9]+
        // [0-9]+[.][0-9]*[eE][+-]?[0-9]+
        // [0-9]+[eE][+-]?[0-9]+
        protected double GetTokenAsDouble(int startIndex)
        {
            // TODO: ReadOnlySpan<char>

            string str = GetTokenSubstring(startIndex, intern: false);

            if (str.IndexOf('_') >= 0) str = str.Replace("_", ""); // NOTE: NumberStyles.AllowThousands is insufficient so we just remove '_' from the source string

            try
            {
                return double.Parse(
                    str,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                    CultureInfo.InvariantCulture);
            }
            catch (OverflowException)
            {
                _errors.Error(CurrentTokenSpan, Warnings.TooBigDouble, GetTokenString());

                //
                return (str.Length > 0 && str[0] == '-') ? double.NegativeInfinity : double.PositiveInfinity;
            }
        }

        /// <summary>
        /// Delegate used for <see cref="ProcessStringText(ReadOnlySpan{char}, ProcessStringDelegate, bool)"/> that handles escaped string sequences.
        /// </summary>
        /// <param name="buffer">Source text buffer.</param>
        /// <param name="index">Index of the escaped character right after '\'.</param>
        /// <param name="builder">Output string builder.</param>
        /// <returns>Position of the last processed character. In case the escaped character is not handled, gets <c>-1</c>.</returns>
        delegate int ProcessStringDelegate(ReadOnlySpan<char> buffer, int index, PhpStringBuilder builder);

        readonly ProcessStringDelegate _processSingleQuotedString = (buffer, pos, result) =>
        {
            var c = buffer[pos];
            if (c == '\\' || c == '\'')
            {
                result.Append(c);
                return pos;
            }

            return -1;
        };

        readonly ProcessStringDelegate _processDoubleQuotedString;

        int ProcessDoubleQuotedString(ReadOnlySpan<char> buffer, int pos, PhpStringBuilder result)
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
                    if (CurrentLexicalState == LexicalStates.ST_IN_SHELL)
                    {
                        result.Append(buffer[pos]);
                        return pos;
                    }
                    break;

                case 'u':
                    if (EnableUtfCodepoints && TryParseCodePoint(buffer, ref pos, out var value))
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
        /// Parses string literal text, processes escaped sequences.
        /// </summary>
        /// <param name="buffer">Characters buffer to read from.</param>
        /// <param name="tryprocess">Callback that handles escaped character sequences. Returns new buffer position in case the sequence was processed, otherwise <c>-1</c>.</param>
        /// <param name="binary">Whether to force a binary string output.</param>
        /// <returns>Parsed text, either <see cref="string"/> or <see cref="byte"/>[].</returns>
        object ProcessStringText(ReadOnlySpan<char> buffer, ProcessStringDelegate tryprocess, bool binary = false)
        {
            Debug.Assert(tryprocess != null);

            if (buffer.IsEmpty)
            {
                return string.Empty;
            }

            int to = buffer.Length;

            PhpStringBuilder lazyBuilder = null;

            // find first backslash quickly
            int index = 0;
            while (index < to && buffer[index] != '\\')
            {
                index++;
            }

            //
            for (; index < to; index++)
            {
                var c = buffer[index];
                if (c == '\\' && index + 1 < to)
                {
                    // ensure string builder lazily
                    if (lazyBuilder == null)
                    {
                        lazyBuilder = new PhpStringBuilder(_encoding, binary, buffer.Length);
                        lazyBuilder.Append(buffer.Slice(0, index));
                    }

                    int processed = tryprocess(buffer, index + 1, lazyBuilder);
                    if (processed > 0)
                    {
                        Debug.Assert(processed >= index + 1);
                        index = processed;
                        continue;
                    }
                }

                if (lazyBuilder != null)
                {
                    lazyBuilder.Append(c);
                }
            }

            //
            return lazyBuilder == null
                ? Intern(buffer)
                : lazyBuilder.Result; // .StringResult;
        }

        object GetTokenAsQuotedString(ProcessStringDelegate tryprocess, char quote)
        {
            // 1/ "..."
            // 2/ b"..."
            // 3/ ...

            var text = this.TokenTextSpan;

            if (text.IsEmpty)
            {
                return string.Empty;
            }

            var binary = false;
            var start = 0;
            var end = text.Length;

            if (text[0] == 'b')
            {
                binary = true;
                start++;
            }

            if (end - start >= 2 && text[start] == quote && text[end - 1] == quote)
            {
                // quoted
                start++;
                end--;
            }
            else
            {
                if (binary) // rollback
                {
                    binary = false;
                    start--;
                }
            }

            return ProcessStringText(text.Slice(start, end - start), tryprocess, binary);
        }

        protected object ProcessEscapedStringWithEnding(ReadOnlySpan<char> buffer, char ending)
        {
            var length = buffer.Length;
            char c2 = (length >= 2) ? buffer[length - 2] : '\0';
            object output;

            // ends with "{END" or "$END" ?
            if (buffer[length - 1] == ending && (c2 == '$' || c2 == '{'))
            {
                output = ProcessStringText(buffer.Slice(0, length - 1), _processDoubleQuotedString);
                _yyless(1);
            }
            else
            {
                output = ProcessStringText(buffer, _processDoubleQuotedString);
            }

            return output.ToString(); // TODO: handle 8bit values
        }

        /// <summary>
        /// Parse code point enclosed in braces.
        /// 'pos' points before '{'
        /// </summary>
        bool TryParseCodePoint(ReadOnlySpan<char> buffer, ref int pos, out string value)
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
                            _errors.Error(
                                new Span(_charOffset + (pos - token_start) + 1, index - pos),
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

        #endregion

        private char Map(char c)
        {
            return (c > SByte.MaxValue) ? 'a' : c;
        }

        public SemanticValueType TokenValue => _tokenSemantics;

        /// <summary>
        /// Gets span of the current token.
        /// </summary>
        public Span TokenSpan => TokenPosition;

        /// <summary>
        /// Alias to <see cref="TokenSpan"/>.
        /// </summary>
        public Span TokenPosition => _tokenPosition;

        /// <summary>
        /// Gets source text of the current token.
        /// </summary>
        public string TokenText => _tokenText ??= GetTokenString();

        /// <summary>
        /// gets the current token position before the <see cref="_tokenPosition"/> gets updated properly.
        /// </summary>
        Span CurrentTokenSpan => new Span(_charOffset, TokenLength);

        /// <summary>Store the token text into the <see cref="_tokenSemantics"/>.</summary>
        Tokens Identifier(Tokens t)
        {
            _tokenSemantics.Token = t;
            _tokenSemantics.String = TokenText;
            return t;
        }

        Tokens ProcessBinaryNumber()
        {
            // parse binary number value
            Tokens token = GetTokenAsDecimalNumber(2, 2, ref _tokenSemantics);

            if (token == Tokens.T_DNUMBER)
            {
                // conversion to double causes data loss
                _errors.Error(CurrentTokenSpan, Warnings.TooBigIntegerConversion, GetTokenString());
            }
            return token;
        }

        Tokens ProcessOctalNumber()
        {
            // parse octal number value // 0oXXXX
            Tokens token = GetTokenAsDecimalNumber(2, 8, ref _tokenSemantics);

            if (token == Tokens.T_DNUMBER)
            {
                // conversion to double causes data loss
                _errors.Error(CurrentTokenSpan, Warnings.TooBigIntegerConversion, GetTokenString());
            }
            return token;
        }

        Tokens ProcessDecimalNumber()
        {
            // [0-9]* - value is either in octal or in decimal
            Tokens token;

            if (GetTokenChar(0) == '0')
                token = GetTokenAsDecimalNumber(1, 8, ref _tokenSemantics);
            else
                token = GetTokenAsDecimalNumber(0, 10, ref _tokenSemantics);

            if (token == Tokens.T_DNUMBER)
            {
                // conversion to double causes data loss
                _errors.Error(CurrentTokenSpan, Warnings.TooBigIntegerConversion, GetTokenString());
            }
            return token;
        }

        Tokens ProcessHexadecimalNumber()
        {
            // parse hexadecimal value
            Tokens token = GetTokenAsDecimalNumber(2, 16, ref _tokenSemantics);

            if (token == Tokens.T_DNUMBER)
            {
                // conversion to double causes data loss
                _errors.Error(CurrentTokenSpan, Warnings.TooBigIntegerConversion, GetTokenString());
            }
            return token;
        }

        Tokens ProcessRealNumber()
        {
            _tokenSemantics.Double = GetTokenAsDouble(0);
            return Tokens.T_DNUMBER;
        }

        Tokens ProcessSingleQuotedString()
        {
            _tokenSemantics.Object = GetTokenAsQuotedString(_processSingleQuotedString, '\'');
            _tokenSemantics.QuoteToken = Tokens.T_SINGLE_QUOTES;
            return Tokens.T_CONSTANT_ENCAPSED_STRING;
        }

        Tokens ProcessDoubleQuotedString()
        {
            _tokenSemantics.Object = GetTokenAsQuotedString(_processDoubleQuotedString, '\"');
            _tokenSemantics.QuoteToken = Tokens.T_DOUBLE_QUOTES;

            return Tokens.T_CONSTANT_ENCAPSED_STRING;
        }

        Tokens ProcessLabel()
        {
            _tokenSemantics.Object = GetTokenString();
            return Tokens.T_STRING;
        }

        Tokens ProcessVariable()
        {
            _tokenSemantics.Object = GetTokenSubstring(1, intern: true);
            return Tokens.T_VARIABLE;
        }

        Tokens ProcessVariableOffsetNumber()
        {
            Tokens token = GetTokenAsDecimalNumber(0, 10, ref _tokenSemantics);
            if (token == Tokens.T_DNUMBER)
            {
                _tokenSemantics.Long = (long)_tokenSemantics.Double;    // we always treat T_NUM_STRING as Long
                _tokenSemantics.Object = GetTokenString();
            }
            return (Tokens.T_NUM_STRING);
        }

        Tokens ProcessVariableOffsetString()
        {
            _tokenSemantics.Object = GetTokenString();
            return (Tokens.T_NUM_STRING);
        }

        bool VerifyEndLabel(ReadOnlySpan<char> chars)
        {
            return
                _hereDocValue != null &&
                chars.LastWord().StartsWith(_hereDocValue.Label.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Resolves closing label indentation (whitespace preceeding the T_END_HEREDOC which is at the last line),
        /// and trims the last line off</summary>
        ReadOnlySpan<char> ResolveHeredocIndentation(ref ReadOnlySpan<char> content)
        {
            int from = content.Length;

            // the last line specifies the indentation
            for (; from > 0;)
            {
                var ch = content[from - 1];
                if (ch == ' ' || ch == '\t')
                {
                    from--;
                }
                else
                {
                    break;
                }
            }

            var indent = content.Slice(from);

            //// scan line by line and check the line indentation

            //if (indent.Length != 0)
            //{
            //    foreach (var line in content.EnumerateLines(false))
            //    {
            //        if (!line.StartsWith(indent) && line.Length != 0)
            //        {
            //            // note: {line.Start} refers to the index in {buffer}
            //            // id {line} is empty, then it is CharSpan.Empty (line.Start will be 0)
            //            var linespan = new Span(_charOffset + (line.Start - token_start), line.Length);
            //            _errors.Error(linespan, FatalErrors.HeredocIndentError);
            //            break;
            //        }
            //    }
            //}

            // trim last line
            content = content.Slice(0, from);

            // \n
            if (content.LastChar() == '\n') content = content.Slice(0, content.Length - 1);
            // \r\n
            // \r
            if (content.LastChar() == '\r') content = content.Slice(0, content.Length - 1);

            //
            return indent;
        }

        ///// <summary>
        ///// Removes line indentation from the content.
        ///// </summary>
        //static string RemoveHeredocIndentation(string content, CharSpan indentation)
        //{
        //    if (indentation.Length == 0 || content.Length == 0)
        //    {
        //        // no indentation
        //        return content;
        //    }

        //    // scan line by line and trim N characters
        //    int linestart = 0;
        //    var result = StringUtils.GetStringBuilder(content.Length);

        //    for (int i = 0; i < content.Length;)
        //    {
        //        var eol = TextUtils.LengthOfLineBreak(content, i);
        //        if (eol != 0)
        //        {
        //            i += eol;

        //            // take [linestart .. i] characters
        //            // without the indentation
        //            var wholeline = content.AsSpan(linestart, i - linestart);
        //            if (wholeline.Length >= indentation.Length && wholeline.StartsWith(indentation))
        //            {
        //                result.Append(content, linestart + indentation.Length, i - linestart - indentation.Length);
        //            }
        //            else
        //            {
        //                // invalid indentation
        //                // add the whole line, syntax error will be reported later
        //                result.Append(content, linestart, i - linestart);
        //            }

        //            linestart = i;
        //        }
        //        else
        //        {
        //            i++;
        //        }
        //    }

        //    var count = content.Length - linestart - indentation.Length;
        //    if (count > 0)
        //    {
        //        var wholeline = content.AsSpan(linestart);
        //        if (wholeline.Length >= indentation.Length && wholeline.StartsWith(indentation))
        //        {
        //            result.Append(content, linestart + indentation.Length, count);
        //        }
        //        else
        //        {
        //            // invalid indentation
        //            // add the whole line, syntax error will be reported later
        //            result.Append(content, linestart, content.Length - linestart);
        //        }
        //    }

        //    //
        //    return StringUtils.ReturnStringBuilder(result);
        //}

        bool ProcessEndNowDoc(ProcessStringDelegate tryprocess)
        {
            BEGIN(LexicalStates.ST_END_HEREDOC);

            var content = TrimNowDocEnd(TokenTextSpan, _hereDocValue.Label); // trim label and whitespaces from the heredoc end
            var lookbackfix = TokenLength - content.Length;
            var indentation = ResolveHeredocIndentation(ref content);
            var sourcetext = content.ToString();

            string text = tryprocess != null
                ? ProcessStringText(content, tryprocess).ToString() // TODO: handle 8bit values
                : sourcetext;

            // text = RemoveHeredocIndentation(text, indentation);

            // move back at the end of the heredoc label - yyless does not work properly (requires additional condition for the optional ';')
            lookahead_index = token_end = lookahead_index - lookbackfix;

            _tokenSemantics.Strings = new StringPair(text, sourcetext);

            // remember the expected indentation
            _hereDocValue = _hereDocValue.WithIndentation(indentation);

            //
            return text.Length != 0;
        }

        /// <summary>
        /// Annotates <see cref="Tokens.T_START_HEREDOC"/> and <see cref="Tokens.T_END_HEREDOC"/> tokens semantic value.
        /// </summary>
        public class HereDocTokenValue : IEquatable<HereDocTokenValue> // SemanticValueType.Object
        {
            public string Label { get; }

            public string Indentation { get; }

            public HereDocTokenValue(string label, string indentation = null)
            {
                Debug.Assert(string.IsNullOrWhiteSpace(indentation));

                this.Label = label ?? throw new ArgumentNullException(nameof(label));
                this.Indentation = indentation ?? string.Empty;
            }

            public HereDocTokenValue Clone() => new HereDocTokenValue(Label, Indentation);

            public HereDocTokenValue WithIndentation(ReadOnlySpan<char> indentation)
            {
                return indentation.Equals(this.Indentation.AsSpan(), StringComparison.Ordinal)
                    ? this
                    : new HereDocTokenValue(Label, indentation.ToString())
                    ;
            }

            public bool Equals(HereDocTokenValue other)
            {
                return other != null && other.Label == Label && other.Indentation == Indentation;
            }

            public override int GetHashCode()
            {
                return StringComparer.Ordinal.GetHashCode(Label) ^ StringComparer.Ordinal.GetHashCode(Indentation);
            }
        }

        /// <summary>
        /// Removes closing label from the heredoc content.
        /// </summary>
        static ReadOnlySpan<char> TrimNowDocEnd(ReadOnlySpan<char> content, string label)
        {
            Debug.Assert(label != null);

            // content: {TEXT}\n[WHITESPACE]*label[WHITESPACE;]*

            // trim whitespace suffix
            // spaces, line separators, paragraph separators, tabs
            for (; content.Length != 0; )
            {
                var ch = content[content.Length - 1];
                if (char.IsWhiteSpace(ch) || ch == ';')
                {
                    content = content.Slice(0, content.Length - 1);
                }
                else
                {
                    break;
                }
            }

            // trim label
            Debug.Assert(content.EndsWith(label.AsSpan(), StringComparison.Ordinal));

            content = content.Slice(0, content.Length - label.Length);

            //
            return content;
        }

        int LabelTrailLength()
        {
            int length = 0;
            var buffer = this.buffer;
            for (int i = token_end - 1; i >= token_start; i--)
            {
                if (char.IsWhiteSpace(buffer[i]) || // spaces, line separators, paragraph separators, tabs
                    buffer[i] == ';')
                {
                    length++;
                }
                else
                {
                    break;
                }
            }
            return length;
        }

        Tokens ProcessStringEOF()
        {
            if (TokenLength > 1 && GetTokenChar(0) == '"')
            {
                _yyless(TokenLength - 1);
                return Tokens.T_DOUBLE_QUOTES;
            }
            else
            {
                _tokenSemantics.Strings = new StringPair(
                    text: (string)ProcessEscapedStringWithEnding(TokenTextSpan, '"'),
                    sourcecode: GetTokenString()
                );
                return Tokens.T_ENCAPSED_AND_WHITESPACE;
            }
        }

        bool ProcessPreOpenTag()
        {
            var text = this.TokenTextSpan; // GetTokenString(intern: false);
            int pos = text.LastIndexOf('<');
            if (pos != 0)
            {
                _yyless(Math.Abs(pos - text.Length));
                _tokenSemantics.Object = text.Slice(0, pos).ToString();
                return true;
            }
            return false;
        }

        Tokens ProcessEof(Tokens token)
        {
            if (TokenLength > 0)
            {
                var text = GetTokenString(intern: false);

                if (token == Tokens.T_ENCAPSED_AND_WHITESPACE)
                {
                    _tokenSemantics.Strings = new StringPair(text, text);
                }
                else
                {
                    _tokenSemantics.String = text;
                }

                return token;
            }
            return Tokens.EOF;
        }

        Tokens WithTokenString(Tokens token)
        {
            _tokenSemantics.Object = GetTokenString();
            return token;
        }

        bool ProcessString(int count, out Tokens token)
        {
            var quote = GetTokenChar(0) == 'b' && TokenLength >= 2 // binary string format
                ? GetTokenChar(1)
                : GetTokenChar(0);

            if (count == 1 && TokenLength > 1 && quote == '"' && GetTokenChar(TokenLength - 1) == '"')
            {
                BEGIN(LexicalStates.ST_IN_SCRIPTING);
                token = ProcessDoubleQuotedString();
                return true;
            }
            else if (TokenLength > 1 && quote == '"')
            {
                _yyless(TokenLength - 1);
                token = Tokens.T_DOUBLE_QUOTES;
                return true;
            }
            else
            {
                return ProcessText(count, LexicalStates.ST_IN_STRING, '"', out token);
            }
        }

        bool ProcessShell(int count, out Tokens token) => ProcessText(count, LexicalStates.ST_IN_SHELL, '`', out token);

        bool ProcessHeredoc(int count, out Tokens token) => ProcessText(count, LexicalStates.ST_IN_HEREDOC, '\0', out token);

        bool ProcessText(int count, LexicalStates newState, char ending, out Tokens token)
        {
            _yyless(count);
            token = Tokens.T_ENCAPSED_AND_WHITESPACE;
            yy_push_state(newState);
            if (TokenLength > 0)
            {
                _tokenSemantics.Strings = new StringPair(
                    text: (string)ProcessEscapedStringWithEnding(TokenTextSpan, ending),
                    sourcecode: GetTokenString()
                );
                return true;
            }
            else
            {
                yymore();
                return false;
            }
        }

        #region Compressed State

        public struct CompressedState : IEquatable<CompressedState>
        {
            internal HereDocTokenValue HereDocValue => _hereDocValue != null ? _hereDocValue.Clone() : null;
            private readonly HereDocTokenValue _hereDocValue;

            internal LexicalStates CurrentState => _currentState;
            private readonly LexicalStates _currentState;

            private readonly LexicalStates[]/*!*/ _stateStack;

            private IDocBlock _phpDoc;
            public IDocBlock PhpDoc => _phpDoc;

            public CompressedState(Lexer lexer)
            {
                this._hereDocValue = lexer._hereDocValue != null ? lexer._hereDocValue.Clone() : null;
                this._currentState = lexer.CurrentLexicalState;
                this._stateStack = lexer.stateStack.Count == 0 ? EmptyArray<LexicalStates>.Instance : lexer.stateStack.ToArray();
                this._phpDoc = lexer.DocComment;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = _hereDocValue != null ? _hereDocValue.GetHashCode() : 0x2312347;
                    for (int i = 0; i < _stateStack.Length; i++)
                        result ^= (int)_stateStack[i] << i;
                    return result ^ ((int)_currentState << 7);
                }
            }

            public bool Equals(CompressedState other)
            {
                if (_stateStack.Length != other._stateStack.Length) return false;
                if (_hereDocValue != null && !_hereDocValue.Equals(other._hereDocValue)) return false;
                if (_hereDocValue == null && other._hereDocValue != null) return false;

                for (int i = 0; i < _stateStack.Length; i++)
                {
                    if (_stateStack[i] != other._stateStack[i])
                        return false;
                }

                return true;
            }

            public int PushStateStack(Stack<LexicalStates> stack)
            {
                var array = _stateStack;
                for (int i = 0; i < _stateStack.Length; i++)
                {
                    stack.Push(array[i]);
                }

                //
                return array.Length;
            }
        }

        public CompressedState GetCompressedState()
        {
            return new CompressedState(this);
        }

        public void RestoreCompressedState(CompressedState state)
        {
            _hereDocValue = state.HereDocValue;
            CurrentLexicalState = state.CurrentState;
            DocComment = state.PhpDoc;

            //
            stateStack.Clear();
            state.PushStateStack(stateStack);
        }

        #endregion
    }
}
