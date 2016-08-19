using System;
using System.Diagnostics;
using System.Text;
using System.Globalization;

using Devsense.PHP.Text;
using Devsense.PHP.Errors;
using System.Collections.Generic;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// PHP lexer generated according to the PhpLexer.l grammar.
    /// </summary>
    public partial class Lexer : ITokenProvider<SemanticValueType, Span>
    {
        /// <summary>
        /// Allow short opening tags
        /// </summary>
        protected bool _allowShortTags = true;

        /// <summary>
        /// Semantic value of the actual token
        /// </summary>
        SemanticValueType _tokenSemantics;

        /// <summary>
        /// Position of the actual token
        /// </summary>
        private Span _tokenPosition;

        /// <summary>
        /// Enxoding used to converted between single-byte strings and Unicode strings.
        /// </summary>
        private readonly Encoding/*!*/ _encoding;

        /// <summary>
        /// Sink used to report lexical errors.
        /// </summary>
        IErrorSink<Span> _errors;

        /// <summary>
        /// Token postition
        /// </summary>
        private int _charOffset = 0;

        /// <summary>
        /// Last encountered heredoc or nowdoc label
        /// </summary>
        protected string _hereDocLabel = null;

        /// <summary>
        /// Flag for handling unicode strings (currently always false, may be eliminated)
        /// </summary>
        private bool _inUnicodeString = false;

        /// <summary>
        /// Get actual doc comment.
        /// </summary>
        public PHPDocBlock DocBlock { get; set; }

        void SetDocBlock() => DocBlock = new PHPDocBlock(GetTokenString(), _tokenPosition);
        void ResetDocBlock() => DocBlock = null;

        /// <summary>
        /// Lexer constructor that initializes all the necessary members
        /// </summary>
        /// <param name="reader">Text reader containing the source code.</param>
        /// <param name="encoding">Source file encoding to convert UTF characters.</param>
        /// <param name="errors">Error sink used to report lexical error.</param>
        /// <param name="features">Allow or disable short oppening tags for PHP.</param>
        /// <param name="positionShift">Starting position of the first token, used during custom restart.</param>
        /// <param name="initialState">Initial state of the lexer, used during custom restart.</param>
        public Lexer(
            System.IO.TextReader reader,
            Encoding encoding,
            IErrorSink<Span> errors = null,
            LanguageFeatures features = LanguageFeatures.Basic,
            int positionShift = 0,
            LexicalStates initialState = LexicalStates.INITIAL)
        {
            _encoding = encoding ?? Encoding.UTF8;
            _errors = errors ?? new EmptyErrorSink<Span>();
            _charOffset = positionShift;
            _allowShortTags = (features & LanguageFeatures.ShortOpenTags) != 0;

            Initialize(reader, initialState);
        }

        /// <summary>
        /// Updates <see cref="charOffset"/> and <see cref="_tokenPosition"/>.
        /// </summary>
        public void UpdateTokenPosition()
        {
            int tokenLength = this.TokenLength;

            // update token position info:
            _tokenPosition = new Span(_charOffset, tokenLength);
            _charOffset += tokenLength;
        }

        void ITokenProvider<SemanticValueType, Span>.ReportError(string[] expectedTerminals)
        {
            // TODO (expected tokens....)
            _errors.Error(_tokenPosition, FatalErrors.SyntaxError, CoreResources.GetString("unexpected_token", GetTokenString()));
        }

        /// <summary>
        /// Get next token, store its actual position in the source unit and call the <see cref="NextTokenEvent"/>.
        /// </summary>
        /// <returns>Next token.</returns>
        virtual public int GetNextToken()
        {
            Tokens token = NextToken();
            UpdateTokenPosition();
            return (int)token;
        }

        protected void _yymore() { yymore(); }

        #region Token Buffer Interpretation

        public int GetTokenByteLength(Encoding/*!*/ encoding)
        {
            return encoding.GetByteCount(buffer, token_start, token_end - token_start);
        }

        protected char[] Buffer { get { return buffer; } }
        protected int BufferTokenStart { get { return token_start; } }

        protected char GetTokenChar(int index)
        {
            return buffer[token_start + index];
        }

        protected string GetTokenString()
        {
            return new String(buffer, token_start, token_end - token_start);
        }

        protected string GetTokenChunkString()
        {
            return new String(buffer, token_chunk_start, token_end - token_chunk_start);
        }

        protected string GetTokenSubstring(int startIndex)
        {
            return new String(buffer, token_start + startIndex, token_end - token_start - startIndex);
        }

        protected string GetTokenSubstring(int startIndex, int length)
        {
            return new String(buffer, token_start + startIndex, length);
        }

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

        /// <summary>
        /// Checks whether {LNUM} fits to integer, long or double 
        /// and returns appropriate value from Tokens enum.
        /// </summary>
        protected Tokens GetIntegerTokenType(int startIndex)
        {
            int i = token_start + startIndex;
            while (i < token_end && buffer[i] == '0') i++;

            int number_length = token_end - i;
            if (i != token_start + startIndex)
            {
                // starts with zero - octal
                // similar to GetHexIntegerTokenType code
                if (number_length < 22)
                    return Tokens.T_LNUMBER;
                return Tokens.T_DNUMBER;
            }
            else
            {
                // can't easily check for numbers of different length
                SemanticValueType val = default(SemanticValueType);
                return GetTokenAsDecimalNumber(startIndex, 10, ref val);
            }
        }

        /// <summary>
        /// Checks whether {HNUM} fits to integer, long or double 
        /// and returns appropriate value from Tokens enum.
        /// </summary>
        protected Tokens GetHexIntegerTokenType(int startIndex)
        {
            // 0xffffffff no
            // 0x7fffffff yes
            int i = token_start + startIndex;
            while (i < token_end && buffer[i] == '0') i++;

            // returns T_LNUMBER when: length without zeros is less than 16
            // or equals 16 and first non-zero character is less than '8'
            if ((token_end - i < 16) || ((token_end - i == 16) && buffer[i] >= '0' && buffer[i] < '8'))
                return Tokens.T_LNUMBER;

            return Tokens.T_DNUMBER;
        }

        // base == 10: [0-9]*
        // base == 16: [0-9A-Fa-f]*
        // assuming result < max int
        protected int GetTokenAsInteger(int startIndex, int @base)
        {
            int result = 0;
            int buffer_pos = token_start + startIndex;

            for (;;)
            {
                int digit = Convert.AlphaNumericToDigit(buffer[buffer_pos]);
                if (digit >= @base) break;

                result = result * @base + digit;
                buffer_pos++;
            }

            return result;
        }


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
            double dresult = 0;

            int digit;
            int buffer_pos = token_start + startIndex;

            // try to parse INT value
            // most of the literals are parsed using the following loop
            while (buffer_pos < buffer.Length && (digit = Convert.AlphaNumericToDigit(buffer[buffer_pos])) < @base && lresult <= long.MaxValue)
            {
                lresult = lresult * @base + digit;
                buffer_pos++;
            }
            // try to parse LONG value (check for the overflow and if it occurs converts data to double)
            bool longOverflow = false;
            while (buffer_pos < buffer.Length && (digit = Convert.AlphaNumericToDigit(buffer[buffer_pos])) < @base)
            {
                try
                {
                    lresult = checked(lresult * @base + digit);
                }
                catch (OverflowException)
                {
                    longOverflow = true; break;
                }
                buffer_pos++;
            }

            if (longOverflow)
            {
                // too big for LONG - use double
                dresult = (double)lresult;
                while (buffer_pos < buffer.Length && (digit = Convert.AlphaNumericToDigit(buffer[buffer_pos])) < @base)
                {
                    dresult = dresult * @base + digit;
                    buffer_pos++;
                }
                val.Double = dresult;
                return Tokens.T_DNUMBER;
            }
            else
            {
                val.Long = lresult;
                return Tokens.T_LNUMBER;
            }
        }


        // [0-9]*[.][0-9]+
        // [0-9]+[.][0-9]*
        // [0-9]*[.][0-9]+[eE][+-]?[0-9]+
        // [0-9]+[.][0-9]*[eE][+-]?[0-9]+
        // [0-9]+[eE][+-]?[0-9]+
        protected double GetTokenAsDouble(int startIndex)
        {
            string str = new string(buffer, token_start, token_end - token_start);

            try
            {
                return double.Parse(
                    str,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                    CultureInfo.InvariantCulture);
            }
            catch (OverflowException)
            {
                return (str.Length > 0 && str[0] == '-') ? double.NegativeInfinity : double.PositiveInfinity;
            }
        }

        #region GetTokenAs*QuotedString

        protected object GetTokenAsDoublyQuotedString(int startIndex, Encoding/*!*/ encoding, bool forceBinaryString)
        {
            PhpStringBuilder result = new PhpStringBuilder(encoding, forceBinaryString, TokenLength);

            int buffer_pos = token_start + startIndex + 1;

            // the following loops expect the token ending by "
            Debug.Assert(buffer[buffer_pos - 1] == '"' && buffer[token_end - 1] == '"');

            //StringBuilder result = new StringBuilder(TokenLength);

            char c;
            while ((c = buffer[buffer_pos++]) != '"')
            {
                if (c == '\\')
                {
                    switch (c = buffer[buffer_pos++])
                    {
                        case 'n':
                            result.Append('\n');
                            break;

                        case 'r':
                            result.Append('\r');
                            break;

                        case 't':
                            result.Append('\t');
                            break;

                        case '\\':
                        case '$':
                        case '"':
                            result.Append(c);
                            break;

                        case 'C':
                            if (!_inUnicodeString) goto default;
                            result.Append(ParseCodePointName(ref buffer_pos));
                            break;

                        case 'u':
                        case 'U':
                            if (!_inUnicodeString) goto default;
                            result.Append(ParseCodePoint(c == 'u' ? 4 : 6, ref buffer_pos));
                            break;

                        case 'x':
                            {
                                int digit;
                                if ((digit = Convert.AlphaNumericToDigit(buffer[buffer_pos])) < 16)
                                {
                                    int hex_code = digit;
                                    buffer_pos++;
                                    if ((digit = Convert.AlphaNumericToDigit(buffer[buffer_pos])) < 16)
                                    {
                                        buffer_pos++;
                                        hex_code = (hex_code << 4) + digit;
                                    }

                                    //encodeBytes[0] = (byte)hex_code;
                                    //result.Append(encodeChars, 0, encoding.GetChars(encodeBytes, 0, 1, encodeChars, 0));
                                    result.Append((byte)hex_code);
                                }
                                else
                                {
                                    result.Append('\\');
                                    result.Append('x');
                                }
                                break;
                            }

                        default:
                            {
                                int digit;
                                if ((digit = Convert.NumericToDigit(c)) < 8)
                                {
                                    int octal_code = digit;

                                    if ((digit = Convert.NumericToDigit(buffer[buffer_pos])) < 8)
                                    {
                                        octal_code = (octal_code << 3) + digit;
                                        buffer_pos++;

                                        if ((digit = Convert.NumericToDigit(buffer[buffer_pos])) < 8)
                                        {
                                            buffer_pos++;
                                            octal_code = (octal_code << 3) + digit;
                                        }
                                    }
                                    //encodeBytes[0] = (byte)octal_code;
                                    //result.Append(encodeChars, 0, encoding.GetChars(encodeBytes, 0, 1, encodeChars, 0));
                                    result.Append((byte)octal_code);
                                }
                                else
                                {
                                    result.Append('\\');
                                    result.Append(c);
                                }
                                break;
                            }
                    }
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.Result;
        }

        protected object ProcessEscapedString(string text, Encoding/*!*/ encoding, bool forceBinaryString)
        {
            PhpStringBuilder result = new PhpStringBuilder(encoding, forceBinaryString, text.Length);

            int pos = 0;

            //StringBuilder result = new StringBuilder(TokenLength);

            char c;
            while (pos < text.Length)
            {
                c = text[pos++];
                if (c == '\\')
                {
                    switch (c = text[pos++])
                    {
                        case 'n':
                            result.Append('\n');
                            break;

                        case 'r':
                            result.Append('\r');
                            break;

                        case 't':
                            result.Append('\t');
                            break;

                        case '\\':
                        case '$':
                        case '"':
                            result.Append(c);
                            break;

                        case 'C':
                            //if (!inUnicodeString) goto default;
                            result.Append(ParseCodePointName(ref pos));
                            break;

                        case 'u':
                        case 'U':
                            //if (!inUnicodeString) goto default;
                            result.Append(ParseCodePoint(c == 'u' ? 4 : 6, ref pos));
                            break;

                        case 'x':
                            {
                                int digit;
                                if ((digit = Convert.AlphaNumericToDigit(text[pos])) < 16)
                                {
                                    int hex_code = digit;
                                    pos++;
                                    if ((digit = Convert.AlphaNumericToDigit(text[pos])) < 16)
                                    {
                                        pos++;
                                        hex_code = (hex_code << 4) + digit;
                                    }

                                    //encodeBytes[0] = (byte)hex_code;
                                    //result.Append(encodeChars, 0, encoding.GetChars(encodeBytes, 0, 1, encodeChars, 0));
                                    result.Append((byte)hex_code);
                                }
                                else
                                {
                                    result.Append('\\');
                                    result.Append('x');
                                }
                                break;
                            }

                        default:
                            {
                                int digit;
                                if ((digit = Convert.NumericToDigit(c)) < 8)
                                {
                                    int octal_code = digit;

                                    if ((digit = Convert.NumericToDigit(text[pos])) < 8)
                                    {
                                        octal_code = (octal_code << 3) + digit;
                                        pos++;

                                        if ((digit = Convert.NumericToDigit(text[pos])) < 8)
                                        {
                                            pos++;
                                            octal_code = (octal_code << 3) + digit;
                                        }
                                    }
                                    //encodeBytes[0] = (byte)octal_code;
                                    //result.Append(encodeChars, 0, encoding.GetChars(encodeBytes, 0, 1, encodeChars, 0));
                                    result.Append((byte)octal_code);
                                }
                                else
                                {
                                    result.Append('\\');
                                    result.Append(c);
                                }
                                break;
                            }
                    }
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.Result;
        }

        protected object GetTokenAsSinglyQuotedString(int startIndex, Encoding/*!*/ encoding, bool forceBinaryString)
        {
            PhpStringBuilder result = new PhpStringBuilder(encoding, forceBinaryString, TokenLength);

            int buffer_pos = token_start + startIndex + 1;

            // the following loops expect the token ending by '
            Debug.Assert(buffer[buffer_pos - 1] == '\'' && buffer[token_end - 1] == '\'');

            //StringBuilder result = new StringBuilder(TokenLength);
            char c;

            while ((c = buffer[buffer_pos++]) != '\'')
            {
                if (c == '\\')
                {
                    switch (c = buffer[buffer_pos++])
                    {
                        case '\\':
                        case '\'':
                            result.Append(c);
                            break;

                        // ??? will cause many problems ... but PHP allows this
                        //case 'C':
                        //  if (!inUnicodeString) goto default;
                        //  result.Append(ParseCodePointName(ref buffer_pos));
                        //  break;

                        //case 'u':
                        //case 'U':
                        //  if (!inUnicodeString) goto default;
                        //  result.Append(ParseCodePoint( c == 'u' ? 4 : 6, ref buffer_pos));
                        //  break;

                        default:
                            result.Append('\\');
                            result.Append(c);
                            break;
                    }
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.Result;
        }

        #endregion

        private string ParseCodePointName(ref int pos)
        {
            if (buffer[pos] == '{')
            {
                int start = ++pos;
                while (pos < token_end && buffer[pos] != '}') pos++;

                if (pos < token_end)
                {
                    string name = new String(buffer, start, pos - start);

                    // TODO: name look-up
                    // return ...[name];

                    // skip '}'
                    pos++;
                }
            }

            //errors.Add(Errors.InvalidCodePointName, sourceFile, );

            return "?";
        }

        private string ParseCodePoint(int maxLength, ref int pos)
        {
            int digit;
            int code_point = 0;
            while (maxLength > 0 && (digit = Convert.NumericToDigit(buffer[pos])) < 16)
            {
                code_point = code_point << 4 + digit;
                pos++;
                maxLength--;
            }

            if (maxLength != 0)
            {
                // TODO: warning
            }

            try
            {
                if ((code_point < 0 || code_point > 0x10ffff) || (code_point >= 0xd800 && code_point <= 0xdfff))
                {
                    // TODO: errors.Add(Errors.InvalidCodePoint, sourceFile, tokenPosition.Short, GetTokenString());
                    return "?";
                }
                else
                {
                    return StringUtils.Utf32ToString(code_point);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // TODO: errors.Add(Errors.InvalidCodePoint, sourceFile, tokenPosition.Short, GetTokenString());
                return "?";
            }
        }

        #endregion

        private char Map(char c)
        {
            return (c > SByte.MaxValue) ? 'a' : c;
        }

        public SemanticValueType TokenValue
        {
            get
            {
                return _tokenSemantics;
            }
        }

        public Span TokenPosition
        {
            get
            {
                return _tokenPosition;
            }
        }

        Tokens ProcessBinaryNumber()
        {
            // parse binary number value
            Tokens token = GetTokenAsDecimalNumber(2, 2, ref _tokenSemantics);

            if (token == Tokens.T_DNUMBER)
            {
                // conversion to double causes data loss
                _errors.Error(_tokenPosition, Warnings.TooBigIntegerConversion, GetTokenString());
            }
            return token;
        }

        Tokens ProcessDecimalNumber()
        {
            // [0-9]* - value is either in octal or in decimal
            Tokens token = Tokens.END;
            if (GetTokenChar(0) == '0')
                token = GetTokenAsDecimalNumber(1, 8, ref _tokenSemantics);
            else
                token = GetTokenAsDecimalNumber(0, 10, ref _tokenSemantics);

            if (token == Tokens.T_DNUMBER)
            {
                // conversion to double causes data loss
                _errors.Error(_tokenPosition, Warnings.TooBigIntegerConversion, GetTokenString());
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
                _errors.Error(_tokenPosition, Warnings.TooBigIntegerConversion, GetTokenString());
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
            bool forceBinaryString = GetTokenChar(0) == 'b';

            _tokenSemantics.Object = GetTokenAsSinglyQuotedString(forceBinaryString ? 1 : 0, _encoding, forceBinaryString);
            return Tokens.T_CONSTANT_ENCAPSED_STRING;
        }

        Tokens ProcessDoubleQuotedString()
        {
            bool forceBinaryString = GetTokenChar(0) == 'b';

            _tokenSemantics.Object = GetTokenAsDoublyQuotedString(forceBinaryString ? 1 : 0, _encoding, forceBinaryString);
            return Tokens.T_CONSTANT_ENCAPSED_STRING;
        }

        Tokens ProcessLabel()
        {
            _tokenSemantics.Object = GetTokenString();
            return Tokens.T_STRING;
        }

        Tokens ProcessVariable()
        {
            _tokenSemantics.Object = GetTokenSubstring(1);
            return Tokens.T_VARIABLE;
        }

        Tokens ProcessVariableOffsetNumber()
        {
            Tokens token = GetTokenAsDecimalNumber(0, 10, ref _tokenSemantics);
            if (token == Tokens.T_DNUMBER)
            {
                _tokenSemantics.Double = 0;
                _tokenSemantics.Object = GetTokenString();
            }
            return (Tokens.T_NUM_STRING);
        }

        Tokens ProcessVariableOffsetString()
        {
            _tokenSemantics.Object = GetTokenString();
            return (Tokens.T_NUM_STRING);
        }

        Tokens ProcessEndNowDoc(Func<string, string> f)
        {
            BEGIN(LexicalStates.ST_END_HEREDOC);
            string label = GetTokenString();
            label = label.TrimEnd(new char[] { '\r', '\n', ';' });
            // move back at the end of the heredoc label - yyless does not work properly (requires additional condition for the optional ';')
            lookahead_index = token_end = lookahead_index - (TokenLength - label.Length) - 1;
            _tokenSemantics.Object = f(label.Substring(0, label.Length - _hereDocLabel.Length));
            return (Tokens.T_ENCAPSED_AND_WHITESPACE);
        }

        #region Compressed State

        public struct CompressedState : IEquatable<CompressedState>
        {
            internal string HereDocLabel => _hereDocLabel;
            private readonly string _hereDocLabel;

            internal LexicalStates CurrentState => _currentState;
            private readonly LexicalStates _currentState;

            private readonly LexicalStates[]/*!*/ _stateStack;
            
            private PHPDocBlock _phpDoc;
            public PHPDocBlock PhpDoc => _phpDoc;

            public CompressedState(Lexer lexer)
            {
                this._hereDocLabel = lexer._hereDocLabel;
                this._currentState = lexer.CurrentLexicalState;
                this._stateStack = lexer.stateStack.ToArray();
                this._phpDoc = lexer.DocBlock;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = (_hereDocLabel != null) ? _hereDocLabel.GetHashCode() : 0x2312347;
                    for (int i = 0; i < _stateStack.Length; i++)
                        result ^= (int)_stateStack[i] << i;
                    return result ^ ((int)_currentState << 7);
                }
            }

            public bool Equals(CompressedState other)
            {
                if (_hereDocLabel != other._hereDocLabel) return false;
                if (_stateStack.Length != other._stateStack.Length) return false;

                for (int i = 0; i < _stateStack.Length; i++)
                {
                    if (_stateStack[i] != other._stateStack[i])
                        return false;
                }

                return true;
            }

            public Stack<LexicalStates> GetStateStack()
            {
                return new Stack<LexicalStates>(_stateStack);
            }
        }

        public CompressedState GetCompressedState()
        {
            return new CompressedState(this);
        }

        public void RestoreCompressedState(CompressedState state)
        {
            _hereDocLabel = state.HereDocLabel;
            stateStack = state.GetStateStack();
            CurrentLexicalState = state.CurrentState;
            DocBlock = state.PhpDoc;
        }

        #endregion
    }
}
