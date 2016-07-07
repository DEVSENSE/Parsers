namespace PhpParser.Parser
{
	#region User Code
	
	/*
 Copyright (c) 2016 Michal Brabec
 The use and distribution terms for this software are contained in the file named License.txt, 
 which can be found in the root of the Phalanger distribution. By using this software 
 in any fashion, you are agreeing to be bound by the terms of this license.
 You must not remove this notice from this software.
*/
using System;
using PHP.Core;
using PHP.Syntax;
using PHP.Core.Text;
using System.Collections.Generic;
#endregion
	
	
	public partial class Lexer
	{
		public enum LexicalStates
		{
			INITIAL = 0,
			ST_IN_SCRIPTING = 1,
			ST_DOUBLE_QUOTES = 2,
			ST_SINGLE_QUOTES = 3,
			ST_BACKQUOTE = 4,
			ST_HEREDOC = 5,
			ST_NEWDOC = 6,
			ST_LOOKING_FOR_PROPERTY = 7,
			ST_LOOKING_FOR_VARNAME = 8,
			ST_DOC_COMMENT = 9,
			ST_COMMENT = 10,
			ST_ONE_LINE_COMMENT = 11,
			ST_VAR_OFFSET = 12,
			ST_END_HEREDOC = 13,
			ST_NOWDOC = 14,
		}
		
		[Flags]
		private enum AcceptConditions : byte
		{
			NotAccept = 0,
			AcceptOnStart = 1,
			AcceptOnEnd = 2,
			Accept = 4
		}
		
		public struct Position
		{
			public int Char;
			public Position(int ch)
			{
				this.Char = ch;
			}
		}
		private const int NoState = -1;
		private const char BOL = (char)128;
		private const char EOF = (char)129;
		
		private Tokens yyreturn;
		
		private System.IO.TextReader reader;
		private char[] buffer = new char[512];
		
		// whether the currently parsed token is being expanded (yymore has been called):
		private bool expanding_token;
		
		// offset in buffer where the currently parsed token starts:
		private int token_start;
		
		// offset in buffer where the currently parsed token chunk starts:
		private int token_chunk_start;
		
		// offset in buffer one char behind the currently parsed token (chunk) ending character:
		private int token_end;
		
		// offset of the lookahead character (number of characters parsed):
		private int lookahead_index;
		
		// number of characters read into the buffer:
		private int chars_read;
		
		// parsed token start position (wrt beginning of the stream):
		protected Position token_start_pos;
		
		// parsed token end position (wrt beginning of the stream):
		protected Position token_end_pos;
		
		private bool yy_at_bol = false;
		
		public LexicalStates CurrentLexicalState { get { return current_lexical_state; } set { current_lexical_state = value; } } 
		private LexicalStates current_lexical_state;
		
		public Lexer(System.IO.TextReader reader)
		{
			Initialize(reader, LexicalStates.INITIAL);
		}
		
		public void Initialize(System.IO.TextReader reader, LexicalStates lexicalState, bool atBol)
		{
			this.expanding_token = false;
			this.token_start = 0;
			this.chars_read = 0;
			this.lookahead_index = 0;
			this.token_chunk_start = 0;
			this.token_end = 0;
			this.token_end_pos = new Position(0);
			this.reader = reader;
			this.yy_at_bol = atBol;
			this.current_lexical_state = lexicalState;
		}
		
		public void Initialize(System.IO.TextReader reader, LexicalStates lexicalState)
		{
			Initialize(reader, lexicalState, false);
		}
		
		#region Accept
		
		#pragma warning disable 162
		
		
		Tokens Accept0(int state,out bool accepted)
		{
			accepted = true;
			
			switch(state)
			{
				case 1:
					// #line 638
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 3:
					// #line 629
					{
						if (AllowShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							return Tokens.T_INLINE_HTML;
						}
					}
					break;
					
				case 4:
					// #line 616
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 5:
					// #line 622
					{
						//HANDLE_NEWLINE(yytext[yyleng-1]);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 6:
					// #line 1021
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 7:
					// #line 675
					{
						return ProcessLabel();
					}
					break;
					
				case 8:
					// #line 233
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 9:
					// #line 517
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 10:
					// #line 562
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 11:
					// #line 257
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 12:
					// #line 522
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return Tokens.T_LBRACE;
					}
					break;
					
				case 13:
					// #line 534
					{
						RESET_DOC_COMMENT();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return Tokens.T_RBRACE;
					}
					break;
					
				case 14:
					// #line 775
					{
						BEGIN(LexicalStates.ST_BACKQUOTE); 
						return Tokens.T_BACKQUOTE; 
					}
					break;
					
				case 15:
					// #line 719
					{ 
						// Gets here only in the case of unterminated singly-quoted string. That leads usually to an error token,
						// however when the source code is parsed per-line (as in Visual Studio colorizer) it is important to remember
						// that we are in the singly-quoted string at the end of the line.
						BEGIN(LexicalStates.ST_SINGLE_QUOTES); 
						yymore(); 
						break; 
					}
					break;
					
				case 16:
					// #line 680
					{
						BEGIN(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 17:
					// #line 731
					{
						BEGIN(LexicalStates.ST_DOUBLE_QUOTES);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 18:
					// #line 112
					{
						return (Tokens.T_IF);
					}
					break;
					
				case 19:
					// #line 136
					{
						return (Tokens.T_DO);
					}
					break;
					
				case 20:
					// #line 497
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 21:
					// #line 168
					{
						return (Tokens.T_AS);
					}
					break;
					
				case 22:
					// #line 405
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 23:
					// #line 228
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 24:
					// #line 441
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 25:
					// #line 513
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 26:
					// #line 433
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 27:
					// #line 579
					{
						return ProcessRealNumber();
					}
					break;
					
				case 28:
					// #line 253
					{
						return (Tokens.T_PAAMAYIM_NEKUDOTAYIM);
					}
					break;
					
				case 29:
					// #line 461
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 30:
					// #line 712
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 31:
					// #line 265
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 32:
					// #line 385
					{
						return (Tokens.T_DOUBLE_ARROW);
					}
					break;
					
				case 33:
					// #line 417
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 34:
					// #line 437
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 35:
					// #line 401
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 36:
					// #line 421
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 37:
					// #line 429
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 38:
					// #line 509
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 39:
					// #line 445
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 40:
					// #line 449
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 41:
					// #line 457
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 42:
					// #line 686
					{
					    bool doc_com = false;
						if (TokenLength > 2) {
							doc_com = true;
							RESET_DOC_COMMENT();
						}
						while (lookahead_index < chars_read) {
							if (buffer[lookahead_index++] == '*' && buffer[lookahead_index] == '/') {
								break;
							}
						}
						if (lookahead_index < chars_read) {
					        lookahead_index++;
						} else {
							//zend_error(E_COMPILE_WARNING, "Unterminated comment starting line %d", CG(zend_lineno));
						}
					    MarkTokenEnd();
					    //HANDLE_NEWLINES(yytext, yyleng);
					    if (doc_com) {
					        _docBlock = new PHPDocBlock(GetTokenString(), new Span(charOffset, TokenLength));
					        return (Tokens.T_DOC_COMMENT);
						}
						return (Tokens.T_COMMENT);
					}
					break;
					
				case 43:
					// #line 465
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 44:
					// #line 477
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 45:
					// #line 493
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 46:
					// #line 481
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 47:
					// #line 489
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 48:
					// #line 485
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 49:
					// #line 654
					{
						return ProcessVariable();
					}
					break;
					
				case 50:
					// #line 717
					{ return ProcessSingleQuotedString(); }
					break;
					
				case 51:
					// #line 505
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 52:
					// #line 96
					{
						return (Tokens.T_TRY);
					}
					break;
					
				case 53:
					// #line 71
					{
						return (Tokens.T_EXIT);
					}
					break;
					
				case 54:
					// #line 140
					{
						return (Tokens.T_FOR);
					}
					break;
					
				case 55:
					// #line 333
					{
						return (Tokens.T_USE);
					}
					break;
					
				case 56:
					// #line 269
					{
						return (Tokens.T_NEW);
					}
					break;
					
				case 57:
					// #line 501
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 58:
					// #line 473
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 59:
					// #line 261
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 60:
					// #line 277
					{
						return (Tokens.T_VAR);
					}
					break;
					
				case 61:
					// #line 409
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 62:
					// #line 413
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 63:
					// #line 425
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 64:
					// #line 469
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 65:
					// #line 453
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 66:
					// #line 566
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 67:
					// #line 558
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 68:
					// #line 67
					{ 
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 69:
					// #line 200
					{
						return (Tokens.T_ECHO);
					}
					break;
					
				case 70:
					// #line 124
					{
						return (Tokens.T_ELSE);
					}
					break;
					
				case 71:
					// #line 309
					{
						return (Tokens.T_EVAL);
					}
					break;
					
				case 72:
					// #line 180
					{
						return (Tokens.T_CASE);
					}
					break;
					
				case 73:
					// #line 389
					{
						return (Tokens.T_LIST);
					}
					break;
					
				case 74:
					// #line 196
					{
						return (Tokens.T_GOTO);
					}
					break;
					
				case 75:
					// #line 120
					{
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 76:
					// #line 349
					{
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 77:
					// #line 345
					{
						return (Tokens.T_ISSET);
					}
					break;
					
				case 78:
					// #line 216
					{
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 79:
					// #line 108
					{
						return (Tokens.T_THROW);
					}
					break;
					
				case 80:
					// #line 365
					{
						return (Tokens.T_FINAL);
					}
					break;
					
				case 81:
					// #line 381
					{
						return (Tokens.T_UNSET);
					}
					break;
					
				case 82:
					// #line 79
					{
						return (Tokens.T_CONST);
					}
					break;
					
				case 83:
					// #line 273
					{
						return (Tokens.T_CLONE);
					}
					break;
					
				case 84:
					// #line 208
					{
						return (Tokens.T_CLASS);
					}
					break;
					
				case 85:
					// #line 100
					{
						return (Tokens.T_CATCH);
					}
					break;
					
				case 86:
					// #line 92
					{
						return (Tokens.T_YIELD);
					}
					break;
					
				case 87:
					// #line 393
					{
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 88:
					// #line 128
					{
						return (Tokens.T_WHILE);
					}
					break;
					
				case 89:
					// #line 188
					{
						return (Tokens.T_BREAK);
					}
					break;
					
				case 90:
					// #line 204
					{
						return (Tokens.T_PRINT);
					}
					break;
					
				case 91:
					// #line 281
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 92:
					// #line 737
					{
						int bprefix = (GetTokenChar(0) != '<') ? 1 : 0;
						int s = bprefix + 3;
					    int length = TokenLength - bprefix - 3 - 1 - (GetTokenChar(TokenLength-2) == '\r' ? 1 : 0);
					    string tokenString = GetTokenString();
					    while ((tokenString[s] == ' ') || (tokenString[s] == '\t')) {
							s++;
					        length--;
					    }
						if (tokenString[s] == '\'') {
							s++;
					        length -= 2;
					        BEGIN(LexicalStates.ST_NOWDOC);
						} else {
							if (tokenString[s] == '"') {
								s++;
					            length -= 2;
					        }
							BEGIN(LexicalStates.ST_HEREDOC);
						}
					    string label = GetTokenSubstring(s, length);
					    /* Check for ending label on the next line */
					    //if (heredoc_label->length < YYLIMIT - YYCURSOR && !memcmp(YYCURSOR, s, heredoc_label->length)) {
					    //	YYCTYPE *end = YYCURSOR + heredoc_label->length;
					    //	if (*end == ';') {
					    //		end++;
					    //	}
					    //	if (*end == '\n' || *end == '\r') {
					    //		BEGIN(ST_END_HEREDOC);
					    //	}
					    //}
					    _docLabelStack.Push(label);
					    return (Tokens.T_START_HEREDOC);
					}
					break;
					
				case 93:
					// #line 144
					{
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 94:
					// #line 116
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 95:
					// #line 357
					{
						return (Tokens.T_STATIC);
					}
					break;
					
				case 96:
					// #line 172
					{
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 97:
					// #line 83
					{
						return (Tokens.T_RETURN);
					}
					break;
					
				case 98:
					// #line 341
					{
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 99:
					// #line 377
					{
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 100:
					// #line 285
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 101:
					// #line 301
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 102:
					// #line 220
					{
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 103:
					// #line 313
					{
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 104:
					// #line 184
					{
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 105:
					// #line 156
					{
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 106:
					// #line 104
					{
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 107:
					// #line 148
					{
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 108:
					// #line 321
					{
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 109:
					// #line 369
					{
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 110:
					// #line 305
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 111:
					// #line 293
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 112:
					// #line 607
					{
						return (Tokens.T_DIR);
					}
					break;
					
				case 113:
					// #line 132
					{
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 114:
					// #line 75
					{
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 115:
					// #line 192
					{
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 116:
					// #line 397
					{
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 117:
					// #line 361
					{
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 118:
					// #line 297
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 119:
					// #line 289
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 120:
					// #line 603
					{
						return (Tokens.T_FILE);
					}
					break;
					
				case 121:
					// #line 599
					{
						return (Tokens.T_LINE);
					}
					break;
					
				case 122:
					// #line 176
					{
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 123:
					// #line 212
					{
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 124:
					// #line 337
					{
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 125:
					// #line 329
					{
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 126:
					// #line 373
					{
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 127:
					// #line 587
					{
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 128:
					// #line 583
					{
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 129:
					// #line 160
					{
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 130:
					// #line 152
					{
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 131:
					// #line 164
					{
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 132:
					// #line 224
					{
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 133:
					// #line 87
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 134:
					// #line 595
					{
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 135:
					// #line 317
					{
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 136:
					// #line 325
					{
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 137:
					// #line 591
					{
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 138:
					// #line 611
					{
						return (Tokens.T_NS_C);
					}
					break;
					
				case 139:
					// #line 353
					{
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 140:
					// #line 814
					{
						if (lookahead_index > chars_read) {
							return (Tokens.END);
						}
						if (GetTokenChar(0) == '\\' && lookahead_index < chars_read) {
					        lookahead_index++;
						}
						while (lookahead_index < chars_read) {
							switch (buffer[lookahead_index++]) {
								case '"':
									break;
								case '$':
									if (IS_LABEL_START(buffer[lookahead_index]) || buffer[lookahead_index] == '{') {
										break;
									}
									continue;
								case '{':
									if (buffer[lookahead_index] == '$') {
										break;
									}
									continue;
								case '\\':
									if (lookahead_index < chars_read) {
					                    lookahead_index++;
					                }
					                continue;
					            default:
									continue;
							}
					        lookahead_index--;
							break;
						}
					    MarkTokenEnd();
					    tokenSemantics.Object = ProcessEscapedString(0, TokenLength, this.sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 141:
					// #line 803
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 142:
					// #line 795
					{
						//Z_LVAL_P(zendlval) = (zend_long) '{';
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 143:
					// #line 528
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 144:
					// #line 648
					{
						yyless(TokenLength - 1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 145:
					// #line 642
					{
						yyless(TokenLength - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 146:
					// #line 727
					{ yymore(); break; }
					break;
					
				case 147:
					// #line 728
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 148:
					// #line 852
					{
						if (lookahead_index > chars_read) {
							return (Tokens.END);
						}
						if (GetTokenChar(0) == '\\' && lookahead_index < chars_read) {
							lookahead_index++;
						}
						while (lookahead_index < chars_read) {
							switch (buffer[lookahead_index++]) {
								case '"':
									break;
								case '$':
									if (IS_LABEL_START(buffer[lookahead_index]) || buffer[lookahead_index] == '{') {
										break;
									}
									continue;
								case '{':
									if (buffer[lookahead_index] == '$') {
										break;
									}
									continue;
								case '\\':
									if (lookahead_index < chars_read) {
										lookahead_index++;
									}
									continue;
								default:
									continue;
							}
							lookahead_index--;
							break;
						}
						MarkTokenEnd();
						tokenSemantics.Object = ProcessEscapedString(0, TokenLength, this.sourceUnit.Encoding, false);
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 149:
					// #line 808
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 150:
					// #line 890
					{
						int newline = 0;
						zend_heredoc_label *heredoc_label = zend_ptr_stack_top(&SCNG(heredoc_label_stack));
						if (YYCURSOR > YYLIMIT) {
							return (Tokens.END);
						}
						YYCURSOR--;
						while (YYCURSOR < YYLIMIT) {
							switch (*YYCURSOR++) {
								case '\r':
									if (*YYCURSOR == '\n') {
										YYCURSOR++;
									}
									/* fall through */
								case '\n':
									/* Check for ending label on the next line */
									if (IS_LABEL_START(*YYCURSOR) && heredoc_label->length < YYLIMIT - YYCURSOR && !memcmp(YYCURSOR, heredoc_label->label, heredoc_label->length)) {
										YYCTYPE *end = YYCURSOR + heredoc_label->length;
										if (*end == ';') {
											end++;
										}
										if (*end == '\n' || *end == '\r') {
											/* newline before label will be subtracted from returned text, but
											 * yyleng/yytext will include it, for zend_highlight/strip, tokenizer, etc. */
											if (YYCURSOR[-2] == '\r' && YYCURSOR[-1] == '\n') {
												newline = 2; /* Windows newline */
											} else {
												newline = 1;
											}
											CG(increment_lineno) = 1; /* For newline before label */
											BEGIN(ST_END_HEREDOC);
											goto heredoc_scan_done;
										}
									}
									continue;
								case '$':
									if (IS_LABEL_START(*YYCURSOR) || *YYCURSOR == '{') {
										break;
									}
									continue;
								case '{':
									if (*YYCURSOR == '$') {
										break;
									}
									continue;
								case '\\':
									if (YYCURSOR < YYLIMIT && *YYCURSOR != '\n' && *YYCURSOR != '\r') {
										YYCURSOR++;
									}
									/* fall through */
								default:
									continue;
							}
							YYCURSOR--;
							break;
						}
					heredoc_scan_done:
						yyleng = YYCURSOR - SCNG(yy_text);
						zend_scan_escape_string(zendlval, yytext, yyleng - newline, 0);
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 151:
					// #line 247
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 152:
					// #line 242
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 153:
					// #line 238
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 154:
					// #line 551
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 155:
					// #line 542
					{
						yyless(TokenLength - 1);
						tokenSemantics.Object = GetTokenString();
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 156:
					// #line 1028
					{ yymore(); break; }
					break;
					
				case 157:
					// #line 1029
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return Tokens.T_COMMENT; }
					break;
					
				case 158:
					// #line 1027
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 1031
					{ 
					  if (AllowAspTags || GetTokenChar(TokenLength - 2) != '%') 
					  { 
							yyless(0);
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return Tokens.T_COMMENT;
						} 
						else 
						{
							yymore();
							break;
						}
					}
					break;
					
				case 160:
					// #line 668
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 161:
					// #line 663
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 162:
					// #line 570
					{ /* Offset could be treated as a long */
						return ProcessVariableOffset();
					}
					break;
					
				case 163:
					// #line 658
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 164:
					// #line 574
					{ /* Offset must be treated as a string */
						ZVAL_STRINGL(zendlval, yytext, yyleng);
						return (Tokens.T_NUM_STRING);
					}
					break;
					
				case 165:
					// #line 781
					{
						zend_heredoc_label *heredoc_label = zend_ptr_stack_pop(&SCNG(heredoc_label_stack));
						YYCURSOR += heredoc_label->length - 1;
						yyleng = heredoc_label->length;
						heredoc_label_dtor(heredoc_label);
						efree(heredoc_label);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 166:
					// #line 964
					{
						int newline = 0;
						zend_heredoc_label *heredoc_label = zend_ptr_stack_top(&SCNG(heredoc_label_stack));
						if (YYCURSOR > YYLIMIT) {
							return (Tokens.END);
						}
						YYCURSOR--;
						while (YYCURSOR < YYLIMIT) {
							switch (*YYCURSOR++) {
								case '\r':
									if (*YYCURSOR == '\n') {
										YYCURSOR++;
									}
									/* fall through */
								case '\n':
									/* Check for ending label on the next line */
									if (IS_LABEL_START(*YYCURSOR) && heredoc_label->length < YYLIMIT - YYCURSOR && !memcmp(YYCURSOR, heredoc_label->label, heredoc_label->length)) {
										YYCTYPE *end = YYCURSOR + heredoc_label->length;
										if (*end == ';') {
											end++;
										}
										if (*end == '\n' || *end == '\r') {
											/* newline before label will be subtracted from returned text, but
											 * yyleng/yytext will include it, for zend_highlight/strip, tokenizer, etc. */
											if (YYCURSOR[-2] == '\r' && YYCURSOR[-1] == '\n') {
												newline = 2; /* Windows newline */
											} else {
												newline = 1;
											}
											CG(increment_lineno) = 1; /* For newline before label */
											BEGIN(ST_END_HEREDOC);
											goto nowdoc_scan_done;
										}
									}
									/* fall through */
								default:
									continue;
							}
						}
					nowdoc_scan_done:
						yyleng = YYCURSOR - SCNG(yy_text);
						zend_copy_value(zendlval, yytext, yyleng - newline);
						//HANDLE_NEWLINES(yytext, yyleng - newline);
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 168: goto case 1;
				case 170: goto case 5;
				case 171: goto case 7;
				case 172: goto case 9;
				case 173: goto case 10;
				case 174: goto case 27;
				case 175: goto case 30;
				case 176: goto case 36;
				case 177: goto case 42;
				case 178: goto case 49;
				case 179: goto case 92;
				case 180: goto case 140;
				case 181: goto case 148;
				case 182: goto case 150;
				case 183: goto case 151;
				case 184: goto case 154;
				case 185: goto case 157;
				case 186: goto case 158;
				case 187: goto case 161;
				case 188: goto case 162;
				case 189: goto case 164;
				case 191: goto case 7;
				case 192: goto case 9;
				case 193: goto case 30;
				case 194: goto case 140;
				case 195: goto case 148;
				case 196: goto case 150;
				case 197: goto case 164;
				case 199: goto case 7;
				case 200: goto case 9;
				case 202: goto case 7;
				case 203: goto case 9;
				case 205: goto case 7;
				case 206: goto case 9;
				case 208: goto case 7;
				case 209: goto case 9;
				case 211: goto case 7;
				case 212: goto case 9;
				case 214: goto case 7;
				case 215: goto case 9;
				case 217: goto case 7;
				case 218: goto case 9;
				case 220: goto case 7;
				case 221: goto case 9;
				case 223: goto case 7;
				case 224: goto case 9;
				case 226: goto case 7;
				case 227: goto case 9;
				case 229: goto case 7;
				case 230: goto case 9;
				case 232: goto case 7;
				case 233: goto case 9;
				case 235: goto case 7;
				case 236: goto case 9;
				case 238: goto case 7;
				case 239: goto case 9;
				case 241: goto case 7;
				case 242: goto case 9;
				case 244: goto case 7;
				case 246: goto case 7;
				case 248: goto case 7;
				case 250: goto case 7;
				case 252: goto case 7;
				case 254: goto case 7;
				case 256: goto case 7;
				case 258: goto case 7;
				case 260: goto case 7;
				case 262: goto case 7;
				case 264: goto case 7;
				case 266: goto case 7;
				case 268: goto case 7;
				case 270: goto case 7;
				case 272: goto case 7;
				case 274: goto case 7;
				case 276: goto case 7;
				case 278: goto case 7;
				case 280: goto case 7;
				case 282: goto case 7;
				case 284: goto case 7;
				case 286: goto case 7;
				case 288: goto case 7;
				case 290: goto case 7;
				case 292: goto case 7;
				case 294: goto case 7;
				case 296: goto case 7;
				case 298: goto case 7;
				case 300: goto case 7;
				case 302: goto case 7;
				case 304: goto case 7;
				case 306: goto case 7;
				case 308: goto case 7;
				case 310: goto case 7;
				case 312: goto case 7;
				case 314: goto case 7;
				case 316: goto case 7;
				case 318: goto case 7;
				case 320: goto case 7;
				case 322: goto case 7;
				case 324: goto case 7;
				case 326: goto case 7;
				case 328: goto case 7;
				case 330: goto case 7;
				case 332: goto case 7;
				case 334: goto case 7;
				case 336: goto case 7;
				case 338: goto case 7;
				case 340: goto case 7;
				case 342: goto case 7;
				case 344: goto case 7;
				case 346: goto case 7;
				case 348: goto case 7;
				case 350: goto case 7;
				case 352: goto case 7;
				case 354: goto case 7;
				case 356: goto case 7;
				case 358: goto case 7;
				case 360: goto case 7;
				case 366: goto case 7;
				case 377: goto case 7;
				case 380: goto case 7;
				case 382: goto case 7;
				case 384: goto case 7;
				case 386: goto case 7;
				case 387: goto case 7;
				case 388: goto case 7;
				case 389: goto case 7;
				case 390: goto case 7;
				case 391: goto case 7;
				case 392: goto case 7;
				case 393: goto case 7;
				case 394: goto case 7;
				case 395: goto case 7;
				case 396: goto case 7;
				case 397: goto case 7;
				case 398: goto case 7;
				case 399: goto case 7;
				case 400: goto case 7;
				case 401: goto case 7;
				case 402: goto case 7;
				case 403: goto case 7;
				case 404: goto case 7;
				case 405: goto case 7;
				case 406: goto case 7;
				case 407: goto case 7;
				case 408: goto case 7;
				case 409: goto case 7;
				case 410: goto case 7;
				case 411: goto case 7;
				case 412: goto case 7;
				case 413: goto case 7;
				case 414: goto case 7;
				case 415: goto case 7;
				case 416: goto case 7;
				case 417: goto case 7;
				case 418: goto case 7;
				case 419: goto case 7;
				case 420: goto case 7;
				case 421: goto case 7;
				case 422: goto case 7;
				case 423: goto case 7;
				case 424: goto case 7;
				case 425: goto case 7;
				case 426: goto case 7;
				case 427: goto case 7;
				case 428: goto case 7;
				case 429: goto case 7;
				case 430: goto case 7;
				case 431: goto case 7;
				case 432: goto case 7;
				case 433: goto case 7;
				case 434: goto case 7;
				case 435: goto case 7;
				case 436: goto case 7;
				case 437: goto case 7;
				case 438: goto case 7;
				case 439: goto case 7;
				case 440: goto case 7;
				case 441: goto case 7;
				case 442: goto case 7;
				case 443: goto case 7;
				case 444: goto case 7;
				case 445: goto case 7;
				case 446: goto case 7;
				case 447: goto case 7;
				case 448: goto case 7;
				case 449: goto case 7;
				case 450: goto case 7;
				case 451: goto case 7;
				case 452: goto case 7;
				case 453: goto case 7;
				case 454: goto case 7;
				case 455: goto case 7;
				case 456: goto case 7;
				case 457: goto case 7;
				case 458: goto case 7;
				case 459: goto case 7;
				case 460: goto case 7;
				case 461: goto case 7;
				case 462: goto case 7;
				case 463: goto case 7;
				case 464: goto case 7;
				case 465: goto case 7;
				case 466: goto case 7;
				case 467: goto case 7;
				case 468: goto case 7;
				case 469: goto case 7;
				case 470: goto case 7;
				case 471: goto case 7;
				case 472: goto case 7;
				case 473: goto case 7;
				case 474: goto case 7;
				case 475: goto case 7;
				case 476: goto case 7;
				case 477: goto case 7;
				case 478: goto case 7;
				case 479: goto case 7;
				case 480: goto case 7;
				case 481: goto case 7;
				case 482: goto case 7;
				case 483: goto case 7;
				case 484: goto case 7;
				case 485: goto case 7;
				case 486: goto case 7;
				case 487: goto case 7;
				case 488: goto case 7;
				case 489: goto case 7;
				case 490: goto case 7;
				case 491: goto case 7;
				case 492: goto case 7;
				case 493: goto case 7;
				case 494: goto case 7;
				case 495: goto case 7;
				case 496: goto case 7;
				case 497: goto case 7;
				case 498: goto case 7;
				case 499: goto case 7;
				case 500: goto case 7;
				case 501: goto case 7;
				case 502: goto case 7;
				case 503: goto case 7;
				case 504: goto case 7;
				case 505: goto case 7;
				case 506: goto case 7;
				case 507: goto case 7;
				case 508: goto case 7;
				case 509: goto case 7;
				case 510: goto case 7;
				case 511: goto case 7;
				case 512: goto case 7;
				case 513: goto case 7;
				case 514: goto case 7;
				case 515: goto case 7;
				case 516: goto case 7;
				case 517: goto case 7;
				case 518: goto case 7;
				case 519: goto case 7;
				case 520: goto case 7;
				case 521: goto case 7;
				case 522: goto case 7;
				case 523: goto case 7;
				case 524: goto case 7;
				case 525: goto case 7;
				case 526: goto case 7;
				case 527: goto case 7;
				case 528: goto case 7;
				case 529: goto case 7;
				case 530: goto case 7;
				case 531: goto case 7;
				case 532: goto case 7;
				case 533: goto case 7;
				case 534: goto case 7;
				case 535: goto case 7;
				case 536: goto case 7;
				case 537: goto case 7;
				case 538: goto case 7;
				case 539: goto case 7;
				case 540: goto case 7;
				case 541: goto case 7;
				case 542: goto case 7;
				case 543: goto case 7;
				case 544: goto case 7;
				case 545: goto case 7;
				case 546: goto case 7;
				case 547: goto case 7;
				case 548: goto case 7;
				case 549: goto case 7;
				case 550: goto case 7;
				case 551: goto case 7;
				case 552: goto case 7;
				case 553: goto case 7;
				case 554: goto case 7;
				case 555: goto case 7;
				case 556: goto case 7;
				case 557: goto case 7;
				case 558: goto case 7;
				case 559: goto case 7;
				case 560: goto case 7;
				case 561: goto case 7;
				case 562: goto case 7;
				case 563: goto case 7;
				case 564: goto case 7;
				case 565: goto case 7;
				case 566: goto case 7;
				case 567: goto case 7;
				case 568: goto case 7;
				case 569: goto case 7;
				case 570: goto case 7;
				case 571: goto case 7;
				case 572: goto case 7;
				case 573: goto case 7;
				case 574: goto case 7;
				case 575: goto case 7;
				case 576: goto case 7;
				case 577: goto case 7;
				case 578: goto case 7;
				case 579: goto case 7;
				case 580: goto case 7;
				case 581: goto case 7;
				case 582: goto case 7;
				case 583: goto case 7;
				case 584: goto case 7;
				case 585: goto case 7;
				case 586: goto case 7;
				case 587: goto case 7;
				case 588: goto case 7;
				case 589: goto case 7;
				case 590: goto case 7;
				case 591: goto case 7;
				case 592: goto case 7;
				case 593: goto case 7;
			}
			accepted = false;
			return yyreturn;
		}
		
		#pragma warning restore 162
		
		
		#endregion
		private void BEGIN(LexicalStates state)
		{
			current_lexical_state = state;
		}
		
		private char Advance()
		{
			if (lookahead_index >= chars_read)
			{
				if (token_start > 0)
				{
					// shift buffer left:
					int length = chars_read - token_start;
					System.Buffer.BlockCopy(buffer, token_start << 1, buffer, 0, length << 1);
					token_end -= token_start;
					token_chunk_start -= token_start;
					token_start = 0;
					chars_read = lookahead_index = length;
					
					// populate the remaining bytes:
					int count = reader.Read(buffer, chars_read, buffer.Length - chars_read);
					if (count <= 0) return EOF;
					
					chars_read += count;
				}
				
				while (lookahead_index >= chars_read)
				{
					if (lookahead_index >= buffer.Length)
						buffer = ResizeBuffer(buffer);
					
					int count = reader.Read(buffer, chars_read, buffer.Length - chars_read);
					if (count <= 0) return EOF;
					chars_read += count;
				}
			}
			
			return Map(buffer[lookahead_index++]);
		}
		
		private char[] ResizeBuffer(char[] buf)
		{
			char[] result = new char[buf.Length << 1];
			System.Buffer.BlockCopy(buf, 0, result, 0, buf.Length << 1);
			return result;
		}
		
		private void AdvanceEndPosition(int from, int to)
		{
			token_end_pos.Char += to - from;
		}
		
		protected static bool IsNewLineCharacter(char ch)
		{
		    return ch == '\r' || ch == '\n' || ch == (char)0x2028 || ch == (char)0x2029;
		}
		private void TrimTokenEnd()
		{
			if (token_end > token_chunk_start && buffer[token_end - 1] == '\n')
				token_end--;
			if (token_end > token_chunk_start && buffer[token_end - 1] == '\r')
				token_end--;
			}
		
		private void MarkTokenChunkStart()
		{
			token_chunk_start = lookahead_index;
		}
		
		private void MarkTokenEnd()
		{
			token_end = lookahead_index;
		}
		
		private void MoveToTokenEnd()
		{
			lookahead_index = token_end;
			yy_at_bol = (token_end > token_chunk_start) && (buffer[token_end - 1] == '\r' || buffer[token_end - 1] == '\n');
		}
		
		public int TokenLength
		{
			get { return token_end - token_start; }
		}
		
		public int TokenChunkLength
		{
			get { return token_end - token_chunk_start; }
		}
		
		private void yymore()
		{
			if (!expanding_token)
			{
				token_start = token_chunk_start;
				expanding_token = true;
			}
		}
		
		private void yyless(int count)
		{
			lookahead_index = token_end = token_chunk_start + count;
		}
		
		private Stack<LexicalStates> stateStack = new Stack<LexicalStates>(20);
		
		private void yy_push_state(LexicalStates state)
		{
			stateStack.Push(current_lexical_state);
			current_lexical_state = state;
		}
		
		private bool yy_pop_state()
		{
			if (stateStack.Count == 0) return false;
			current_lexical_state = stateStack.Pop();
			return true;
		}
		
		private LexicalStates yy_top_state()
		{
			return stateStack.Peek();
		}
		
		#region Tables
		
		private static AcceptConditions[] acceptCondition = new AcceptConditions[]
		{
			AcceptConditions.NotAccept, // 0
			AcceptConditions.Accept, // 1
			AcceptConditions.Accept, // 2
			AcceptConditions.Accept, // 3
			AcceptConditions.Accept, // 4
			AcceptConditions.Accept, // 5
			AcceptConditions.Accept, // 6
			AcceptConditions.Accept, // 7
			AcceptConditions.Accept, // 8
			AcceptConditions.Accept, // 9
			AcceptConditions.Accept, // 10
			AcceptConditions.Accept, // 11
			AcceptConditions.Accept, // 12
			AcceptConditions.Accept, // 13
			AcceptConditions.Accept, // 14
			AcceptConditions.Accept, // 15
			AcceptConditions.Accept, // 16
			AcceptConditions.Accept, // 17
			AcceptConditions.Accept, // 18
			AcceptConditions.Accept, // 19
			AcceptConditions.Accept, // 20
			AcceptConditions.Accept, // 21
			AcceptConditions.Accept, // 22
			AcceptConditions.Accept, // 23
			AcceptConditions.Accept, // 24
			AcceptConditions.Accept, // 25
			AcceptConditions.Accept, // 26
			AcceptConditions.Accept, // 27
			AcceptConditions.Accept, // 28
			AcceptConditions.Accept, // 29
			AcceptConditions.Accept, // 30
			AcceptConditions.Accept, // 31
			AcceptConditions.Accept, // 32
			AcceptConditions.Accept, // 33
			AcceptConditions.Accept, // 34
			AcceptConditions.Accept, // 35
			AcceptConditions.Accept, // 36
			AcceptConditions.Accept, // 37
			AcceptConditions.Accept, // 38
			AcceptConditions.Accept, // 39
			AcceptConditions.Accept, // 40
			AcceptConditions.Accept, // 41
			AcceptConditions.Accept, // 42
			AcceptConditions.Accept, // 43
			AcceptConditions.Accept, // 44
			AcceptConditions.Accept, // 45
			AcceptConditions.Accept, // 46
			AcceptConditions.Accept, // 47
			AcceptConditions.Accept, // 48
			AcceptConditions.Accept, // 49
			AcceptConditions.Accept, // 50
			AcceptConditions.Accept, // 51
			AcceptConditions.Accept, // 52
			AcceptConditions.Accept, // 53
			AcceptConditions.Accept, // 54
			AcceptConditions.Accept, // 55
			AcceptConditions.Accept, // 56
			AcceptConditions.Accept, // 57
			AcceptConditions.Accept, // 58
			AcceptConditions.Accept, // 59
			AcceptConditions.Accept, // 60
			AcceptConditions.Accept, // 61
			AcceptConditions.Accept, // 62
			AcceptConditions.Accept, // 63
			AcceptConditions.Accept, // 64
			AcceptConditions.Accept, // 65
			AcceptConditions.Accept, // 66
			AcceptConditions.Accept, // 67
			AcceptConditions.Accept, // 68
			AcceptConditions.Accept, // 69
			AcceptConditions.Accept, // 70
			AcceptConditions.Accept, // 71
			AcceptConditions.Accept, // 72
			AcceptConditions.Accept, // 73
			AcceptConditions.Accept, // 74
			AcceptConditions.Accept, // 75
			AcceptConditions.Accept, // 76
			AcceptConditions.Accept, // 77
			AcceptConditions.Accept, // 78
			AcceptConditions.Accept, // 79
			AcceptConditions.Accept, // 80
			AcceptConditions.Accept, // 81
			AcceptConditions.Accept, // 82
			AcceptConditions.Accept, // 83
			AcceptConditions.Accept, // 84
			AcceptConditions.Accept, // 85
			AcceptConditions.Accept, // 86
			AcceptConditions.Accept, // 87
			AcceptConditions.Accept, // 88
			AcceptConditions.Accept, // 89
			AcceptConditions.Accept, // 90
			AcceptConditions.Accept, // 91
			AcceptConditions.Accept, // 92
			AcceptConditions.Accept, // 93
			AcceptConditions.Accept, // 94
			AcceptConditions.Accept, // 95
			AcceptConditions.Accept, // 96
			AcceptConditions.Accept, // 97
			AcceptConditions.Accept, // 98
			AcceptConditions.Accept, // 99
			AcceptConditions.Accept, // 100
			AcceptConditions.Accept, // 101
			AcceptConditions.Accept, // 102
			AcceptConditions.Accept, // 103
			AcceptConditions.Accept, // 104
			AcceptConditions.Accept, // 105
			AcceptConditions.Accept, // 106
			AcceptConditions.Accept, // 107
			AcceptConditions.Accept, // 108
			AcceptConditions.Accept, // 109
			AcceptConditions.Accept, // 110
			AcceptConditions.Accept, // 111
			AcceptConditions.Accept, // 112
			AcceptConditions.Accept, // 113
			AcceptConditions.Accept, // 114
			AcceptConditions.Accept, // 115
			AcceptConditions.Accept, // 116
			AcceptConditions.Accept, // 117
			AcceptConditions.Accept, // 118
			AcceptConditions.Accept, // 119
			AcceptConditions.Accept, // 120
			AcceptConditions.Accept, // 121
			AcceptConditions.Accept, // 122
			AcceptConditions.Accept, // 123
			AcceptConditions.Accept, // 124
			AcceptConditions.Accept, // 125
			AcceptConditions.Accept, // 126
			AcceptConditions.Accept, // 127
			AcceptConditions.Accept, // 128
			AcceptConditions.Accept, // 129
			AcceptConditions.Accept, // 130
			AcceptConditions.Accept, // 131
			AcceptConditions.Accept, // 132
			AcceptConditions.Accept, // 133
			AcceptConditions.Accept, // 134
			AcceptConditions.Accept, // 135
			AcceptConditions.Accept, // 136
			AcceptConditions.Accept, // 137
			AcceptConditions.Accept, // 138
			AcceptConditions.Accept, // 139
			AcceptConditions.Accept, // 140
			AcceptConditions.Accept, // 141
			AcceptConditions.Accept, // 142
			AcceptConditions.Accept, // 143
			AcceptConditions.Accept, // 144
			AcceptConditions.Accept, // 145
			AcceptConditions.Accept, // 146
			AcceptConditions.Accept, // 147
			AcceptConditions.Accept, // 148
			AcceptConditions.Accept, // 149
			AcceptConditions.Accept, // 150
			AcceptConditions.Accept, // 151
			AcceptConditions.Accept, // 152
			AcceptConditions.Accept, // 153
			AcceptConditions.Accept, // 154
			AcceptConditions.Accept, // 155
			AcceptConditions.Accept, // 156
			AcceptConditions.Accept, // 157
			AcceptConditions.Accept, // 158
			AcceptConditions.Accept, // 159
			AcceptConditions.Accept, // 160
			AcceptConditions.Accept, // 161
			AcceptConditions.Accept, // 162
			AcceptConditions.Accept, // 163
			AcceptConditions.Accept, // 164
			AcceptConditions.Accept, // 165
			AcceptConditions.Accept, // 166
			AcceptConditions.NotAccept, // 167
			AcceptConditions.Accept, // 168
			AcceptConditions.Accept, // 169
			AcceptConditions.Accept, // 170
			AcceptConditions.Accept, // 171
			AcceptConditions.Accept, // 172
			AcceptConditions.Accept, // 173
			AcceptConditions.Accept, // 174
			AcceptConditions.Accept, // 175
			AcceptConditions.Accept, // 176
			AcceptConditions.Accept, // 177
			AcceptConditions.Accept, // 178
			AcceptConditions.Accept, // 179
			AcceptConditions.Accept, // 180
			AcceptConditions.Accept, // 181
			AcceptConditions.Accept, // 182
			AcceptConditions.Accept, // 183
			AcceptConditions.Accept, // 184
			AcceptConditions.Accept, // 185
			AcceptConditions.Accept, // 186
			AcceptConditions.Accept, // 187
			AcceptConditions.Accept, // 188
			AcceptConditions.Accept, // 189
			AcceptConditions.NotAccept, // 190
			AcceptConditions.Accept, // 191
			AcceptConditions.Accept, // 192
			AcceptConditions.Accept, // 193
			AcceptConditions.Accept, // 194
			AcceptConditions.Accept, // 195
			AcceptConditions.Accept, // 196
			AcceptConditions.Accept, // 197
			AcceptConditions.NotAccept, // 198
			AcceptConditions.Accept, // 199
			AcceptConditions.Accept, // 200
			AcceptConditions.NotAccept, // 201
			AcceptConditions.Accept, // 202
			AcceptConditions.Accept, // 203
			AcceptConditions.NotAccept, // 204
			AcceptConditions.Accept, // 205
			AcceptConditions.Accept, // 206
			AcceptConditions.NotAccept, // 207
			AcceptConditions.Accept, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.NotAccept, // 210
			AcceptConditions.Accept, // 211
			AcceptConditions.Accept, // 212
			AcceptConditions.NotAccept, // 213
			AcceptConditions.Accept, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.NotAccept, // 216
			AcceptConditions.Accept, // 217
			AcceptConditions.Accept, // 218
			AcceptConditions.NotAccept, // 219
			AcceptConditions.Accept, // 220
			AcceptConditions.Accept, // 221
			AcceptConditions.NotAccept, // 222
			AcceptConditions.Accept, // 223
			AcceptConditions.Accept, // 224
			AcceptConditions.NotAccept, // 225
			AcceptConditions.Accept, // 226
			AcceptConditions.Accept, // 227
			AcceptConditions.NotAccept, // 228
			AcceptConditions.Accept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.NotAccept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.NotAccept, // 234
			AcceptConditions.Accept, // 235
			AcceptConditions.Accept, // 236
			AcceptConditions.NotAccept, // 237
			AcceptConditions.Accept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.NotAccept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.NotAccept, // 243
			AcceptConditions.Accept, // 244
			AcceptConditions.NotAccept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.NotAccept, // 247
			AcceptConditions.Accept, // 248
			AcceptConditions.NotAccept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.NotAccept, // 251
			AcceptConditions.Accept, // 252
			AcceptConditions.NotAccept, // 253
			AcceptConditions.Accept, // 254
			AcceptConditions.NotAccept, // 255
			AcceptConditions.Accept, // 256
			AcceptConditions.NotAccept, // 257
			AcceptConditions.Accept, // 258
			AcceptConditions.NotAccept, // 259
			AcceptConditions.Accept, // 260
			AcceptConditions.NotAccept, // 261
			AcceptConditions.Accept, // 262
			AcceptConditions.NotAccept, // 263
			AcceptConditions.Accept, // 264
			AcceptConditions.NotAccept, // 265
			AcceptConditions.Accept, // 266
			AcceptConditions.NotAccept, // 267
			AcceptConditions.Accept, // 268
			AcceptConditions.NotAccept, // 269
			AcceptConditions.Accept, // 270
			AcceptConditions.NotAccept, // 271
			AcceptConditions.Accept, // 272
			AcceptConditions.NotAccept, // 273
			AcceptConditions.Accept, // 274
			AcceptConditions.NotAccept, // 275
			AcceptConditions.Accept, // 276
			AcceptConditions.NotAccept, // 277
			AcceptConditions.Accept, // 278
			AcceptConditions.NotAccept, // 279
			AcceptConditions.Accept, // 280
			AcceptConditions.NotAccept, // 281
			AcceptConditions.Accept, // 282
			AcceptConditions.NotAccept, // 283
			AcceptConditions.Accept, // 284
			AcceptConditions.NotAccept, // 285
			AcceptConditions.Accept, // 286
			AcceptConditions.NotAccept, // 287
			AcceptConditions.Accept, // 288
			AcceptConditions.NotAccept, // 289
			AcceptConditions.Accept, // 290
			AcceptConditions.NotAccept, // 291
			AcceptConditions.Accept, // 292
			AcceptConditions.NotAccept, // 293
			AcceptConditions.Accept, // 294
			AcceptConditions.NotAccept, // 295
			AcceptConditions.Accept, // 296
			AcceptConditions.NotAccept, // 297
			AcceptConditions.Accept, // 298
			AcceptConditions.NotAccept, // 299
			AcceptConditions.Accept, // 300
			AcceptConditions.NotAccept, // 301
			AcceptConditions.Accept, // 302
			AcceptConditions.NotAccept, // 303
			AcceptConditions.Accept, // 304
			AcceptConditions.NotAccept, // 305
			AcceptConditions.Accept, // 306
			AcceptConditions.NotAccept, // 307
			AcceptConditions.Accept, // 308
			AcceptConditions.NotAccept, // 309
			AcceptConditions.Accept, // 310
			AcceptConditions.NotAccept, // 311
			AcceptConditions.Accept, // 312
			AcceptConditions.NotAccept, // 313
			AcceptConditions.Accept, // 314
			AcceptConditions.NotAccept, // 315
			AcceptConditions.Accept, // 316
			AcceptConditions.NotAccept, // 317
			AcceptConditions.Accept, // 318
			AcceptConditions.NotAccept, // 319
			AcceptConditions.Accept, // 320
			AcceptConditions.NotAccept, // 321
			AcceptConditions.Accept, // 322
			AcceptConditions.NotAccept, // 323
			AcceptConditions.Accept, // 324
			AcceptConditions.NotAccept, // 325
			AcceptConditions.Accept, // 326
			AcceptConditions.NotAccept, // 327
			AcceptConditions.Accept, // 328
			AcceptConditions.NotAccept, // 329
			AcceptConditions.Accept, // 330
			AcceptConditions.NotAccept, // 331
			AcceptConditions.Accept, // 332
			AcceptConditions.NotAccept, // 333
			AcceptConditions.Accept, // 334
			AcceptConditions.NotAccept, // 335
			AcceptConditions.Accept, // 336
			AcceptConditions.NotAccept, // 337
			AcceptConditions.Accept, // 338
			AcceptConditions.NotAccept, // 339
			AcceptConditions.Accept, // 340
			AcceptConditions.NotAccept, // 341
			AcceptConditions.Accept, // 342
			AcceptConditions.NotAccept, // 343
			AcceptConditions.Accept, // 344
			AcceptConditions.NotAccept, // 345
			AcceptConditions.Accept, // 346
			AcceptConditions.NotAccept, // 347
			AcceptConditions.Accept, // 348
			AcceptConditions.NotAccept, // 349
			AcceptConditions.Accept, // 350
			AcceptConditions.NotAccept, // 351
			AcceptConditions.Accept, // 352
			AcceptConditions.NotAccept, // 353
			AcceptConditions.Accept, // 354
			AcceptConditions.NotAccept, // 355
			AcceptConditions.Accept, // 356
			AcceptConditions.NotAccept, // 357
			AcceptConditions.Accept, // 358
			AcceptConditions.NotAccept, // 359
			AcceptConditions.Accept, // 360
			AcceptConditions.NotAccept, // 361
			AcceptConditions.NotAccept, // 362
			AcceptConditions.NotAccept, // 363
			AcceptConditions.NotAccept, // 364
			AcceptConditions.NotAccept, // 365
			AcceptConditions.Accept, // 366
			AcceptConditions.NotAccept, // 367
			AcceptConditions.NotAccept, // 368
			AcceptConditions.NotAccept, // 369
			AcceptConditions.NotAccept, // 370
			AcceptConditions.NotAccept, // 371
			AcceptConditions.NotAccept, // 372
			AcceptConditions.NotAccept, // 373
			AcceptConditions.NotAccept, // 374
			AcceptConditions.NotAccept, // 375
			AcceptConditions.NotAccept, // 376
			AcceptConditions.Accept, // 377
			AcceptConditions.NotAccept, // 378
			AcceptConditions.NotAccept, // 379
			AcceptConditions.Accept, // 380
			AcceptConditions.NotAccept, // 381
			AcceptConditions.Accept, // 382
			AcceptConditions.NotAccept, // 383
			AcceptConditions.Accept, // 384
			AcceptConditions.NotAccept, // 385
			AcceptConditions.Accept, // 386
			AcceptConditions.Accept, // 387
			AcceptConditions.Accept, // 388
			AcceptConditions.Accept, // 389
			AcceptConditions.Accept, // 390
			AcceptConditions.Accept, // 391
			AcceptConditions.Accept, // 392
			AcceptConditions.Accept, // 393
			AcceptConditions.Accept, // 394
			AcceptConditions.Accept, // 395
			AcceptConditions.Accept, // 396
			AcceptConditions.Accept, // 397
			AcceptConditions.Accept, // 398
			AcceptConditions.Accept, // 399
			AcceptConditions.Accept, // 400
			AcceptConditions.Accept, // 401
			AcceptConditions.Accept, // 402
			AcceptConditions.Accept, // 403
			AcceptConditions.Accept, // 404
			AcceptConditions.Accept, // 405
			AcceptConditions.Accept, // 406
			AcceptConditions.Accept, // 407
			AcceptConditions.Accept, // 408
			AcceptConditions.Accept, // 409
			AcceptConditions.Accept, // 410
			AcceptConditions.Accept, // 411
			AcceptConditions.Accept, // 412
			AcceptConditions.Accept, // 413
			AcceptConditions.Accept, // 414
			AcceptConditions.Accept, // 415
			AcceptConditions.Accept, // 416
			AcceptConditions.Accept, // 417
			AcceptConditions.Accept, // 418
			AcceptConditions.Accept, // 419
			AcceptConditions.Accept, // 420
			AcceptConditions.Accept, // 421
			AcceptConditions.Accept, // 422
			AcceptConditions.Accept, // 423
			AcceptConditions.Accept, // 424
			AcceptConditions.Accept, // 425
			AcceptConditions.Accept, // 426
			AcceptConditions.Accept, // 427
			AcceptConditions.Accept, // 428
			AcceptConditions.Accept, // 429
			AcceptConditions.Accept, // 430
			AcceptConditions.Accept, // 431
			AcceptConditions.Accept, // 432
			AcceptConditions.Accept, // 433
			AcceptConditions.Accept, // 434
			AcceptConditions.Accept, // 435
			AcceptConditions.Accept, // 436
			AcceptConditions.Accept, // 437
			AcceptConditions.Accept, // 438
			AcceptConditions.Accept, // 439
			AcceptConditions.Accept, // 440
			AcceptConditions.Accept, // 441
			AcceptConditions.Accept, // 442
			AcceptConditions.Accept, // 443
			AcceptConditions.Accept, // 444
			AcceptConditions.Accept, // 445
			AcceptConditions.Accept, // 446
			AcceptConditions.Accept, // 447
			AcceptConditions.Accept, // 448
			AcceptConditions.Accept, // 449
			AcceptConditions.Accept, // 450
			AcceptConditions.Accept, // 451
			AcceptConditions.Accept, // 452
			AcceptConditions.Accept, // 453
			AcceptConditions.Accept, // 454
			AcceptConditions.Accept, // 455
			AcceptConditions.Accept, // 456
			AcceptConditions.Accept, // 457
			AcceptConditions.Accept, // 458
			AcceptConditions.Accept, // 459
			AcceptConditions.Accept, // 460
			AcceptConditions.Accept, // 461
			AcceptConditions.Accept, // 462
			AcceptConditions.Accept, // 463
			AcceptConditions.Accept, // 464
			AcceptConditions.Accept, // 465
			AcceptConditions.Accept, // 466
			AcceptConditions.Accept, // 467
			AcceptConditions.Accept, // 468
			AcceptConditions.Accept, // 469
			AcceptConditions.Accept, // 470
			AcceptConditions.Accept, // 471
			AcceptConditions.Accept, // 472
			AcceptConditions.Accept, // 473
			AcceptConditions.Accept, // 474
			AcceptConditions.Accept, // 475
			AcceptConditions.Accept, // 476
			AcceptConditions.Accept, // 477
			AcceptConditions.Accept, // 478
			AcceptConditions.Accept, // 479
			AcceptConditions.Accept, // 480
			AcceptConditions.Accept, // 481
			AcceptConditions.Accept, // 482
			AcceptConditions.Accept, // 483
			AcceptConditions.Accept, // 484
			AcceptConditions.Accept, // 485
			AcceptConditions.Accept, // 486
			AcceptConditions.Accept, // 487
			AcceptConditions.Accept, // 488
			AcceptConditions.Accept, // 489
			AcceptConditions.Accept, // 490
			AcceptConditions.Accept, // 491
			AcceptConditions.Accept, // 492
			AcceptConditions.Accept, // 493
			AcceptConditions.Accept, // 494
			AcceptConditions.Accept, // 495
			AcceptConditions.Accept, // 496
			AcceptConditions.Accept, // 497
			AcceptConditions.Accept, // 498
			AcceptConditions.Accept, // 499
			AcceptConditions.Accept, // 500
			AcceptConditions.Accept, // 501
			AcceptConditions.Accept, // 502
			AcceptConditions.Accept, // 503
			AcceptConditions.Accept, // 504
			AcceptConditions.Accept, // 505
			AcceptConditions.Accept, // 506
			AcceptConditions.Accept, // 507
			AcceptConditions.Accept, // 508
			AcceptConditions.Accept, // 509
			AcceptConditions.Accept, // 510
			AcceptConditions.Accept, // 511
			AcceptConditions.Accept, // 512
			AcceptConditions.Accept, // 513
			AcceptConditions.Accept, // 514
			AcceptConditions.Accept, // 515
			AcceptConditions.Accept, // 516
			AcceptConditions.Accept, // 517
			AcceptConditions.Accept, // 518
			AcceptConditions.Accept, // 519
			AcceptConditions.Accept, // 520
			AcceptConditions.Accept, // 521
			AcceptConditions.Accept, // 522
			AcceptConditions.Accept, // 523
			AcceptConditions.Accept, // 524
			AcceptConditions.Accept, // 525
			AcceptConditions.Accept, // 526
			AcceptConditions.Accept, // 527
			AcceptConditions.Accept, // 528
			AcceptConditions.Accept, // 529
			AcceptConditions.Accept, // 530
			AcceptConditions.Accept, // 531
			AcceptConditions.Accept, // 532
			AcceptConditions.Accept, // 533
			AcceptConditions.Accept, // 534
			AcceptConditions.Accept, // 535
			AcceptConditions.Accept, // 536
			AcceptConditions.Accept, // 537
			AcceptConditions.Accept, // 538
			AcceptConditions.Accept, // 539
			AcceptConditions.Accept, // 540
			AcceptConditions.Accept, // 541
			AcceptConditions.Accept, // 542
			AcceptConditions.Accept, // 543
			AcceptConditions.Accept, // 544
			AcceptConditions.Accept, // 545
			AcceptConditions.Accept, // 546
			AcceptConditions.Accept, // 547
			AcceptConditions.Accept, // 548
			AcceptConditions.Accept, // 549
			AcceptConditions.Accept, // 550
			AcceptConditions.Accept, // 551
			AcceptConditions.Accept, // 552
			AcceptConditions.Accept, // 553
			AcceptConditions.Accept, // 554
			AcceptConditions.Accept, // 555
			AcceptConditions.Accept, // 556
			AcceptConditions.Accept, // 557
			AcceptConditions.Accept, // 558
			AcceptConditions.Accept, // 559
			AcceptConditions.Accept, // 560
			AcceptConditions.Accept, // 561
			AcceptConditions.Accept, // 562
			AcceptConditions.Accept, // 563
			AcceptConditions.Accept, // 564
			AcceptConditions.Accept, // 565
			AcceptConditions.Accept, // 566
			AcceptConditions.Accept, // 567
			AcceptConditions.Accept, // 568
			AcceptConditions.Accept, // 569
			AcceptConditions.Accept, // 570
			AcceptConditions.Accept, // 571
			AcceptConditions.Accept, // 572
			AcceptConditions.Accept, // 573
			AcceptConditions.Accept, // 574
			AcceptConditions.Accept, // 575
			AcceptConditions.Accept, // 576
			AcceptConditions.Accept, // 577
			AcceptConditions.Accept, // 578
			AcceptConditions.Accept, // 579
			AcceptConditions.Accept, // 580
			AcceptConditions.Accept, // 581
			AcceptConditions.Accept, // 582
			AcceptConditions.Accept, // 583
			AcceptConditions.Accept, // 584
			AcceptConditions.Accept, // 585
			AcceptConditions.Accept, // 586
			AcceptConditions.Accept, // 587
			AcceptConditions.Accept, // 588
			AcceptConditions.Accept, // 589
			AcceptConditions.Accept, // 590
			AcceptConditions.Accept, // 591
			AcceptConditions.Accept, // 592
			AcceptConditions.Accept, // 593
		};
		
		private static int[] colMap = new int[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 35, 15, 0, 0, 57, 0, 0, 
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			35, 42, 62, 61, 52, 46, 47, 60, 34, 36, 44, 41, 50, 24, 31, 45, 
			55, 56, 27, 27, 27, 27, 27, 27, 27, 27, 29, 50, 43, 40, 25, 32, 
			50, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 28, 54, 30, 58, 49, 38, 
			59, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 28, 51, 48, 53, 50, 0, 
			26, 26
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 2, 1, 1, 1, 3, 4, 5, 6, 1, 1, 1, 1, 7, 
			1, 1, 8, 8, 8, 8, 1, 1, 1, 9, 1, 10, 1, 1, 11, 1, 
			1, 12, 1, 1, 13, 14, 15, 1, 16, 1, 17, 1, 1, 1, 1, 1, 
			1, 18, 1, 8, 8, 8, 19, 8, 8, 8, 1, 1, 8, 1, 1, 1, 
			1, 1, 20, 21, 8, 8, 22, 8, 8, 8, 8, 8, 8, 8, 8, 8, 
			23, 8, 8, 8, 8, 8, 24, 8, 8, 8, 8, 1, 1, 25, 8, 8, 
			8, 8, 8, 8, 1, 1, 8, 26, 8, 8, 8, 8, 27, 8, 1, 1, 
			8, 8, 8, 8, 8, 8, 1, 1, 8, 8, 8, 8, 8, 8, 8, 8, 
			8, 8, 8, 8, 8, 1, 8, 8, 8, 8, 8, 8, 1, 1, 1, 1, 
			1, 1, 28, 1, 1, 1, 1, 1, 29, 1, 1, 1, 30, 1, 1, 1, 
			1, 1, 31, 1, 32, 1, 1, 33, 34, 35, 36, 37, 38, 39, 40, 1, 
			1, 41, 42, 43, 44, 44, 44, 45, 35, 46, 47, 48, 49, 50, 51, 52, 
			53, 54, 55, 55, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 
			67, 1, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 
			82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 65, 95, 96, 
			20, 97, 48, 21, 98, 7, 99, 100, 101, 40, 102, 103, 104, 105, 106, 107, 
			108, 109, 110, 111, 112, 113, 114, 115, 116, 41, 117, 118, 119, 120, 121, 122, 
			123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 
			139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 
			155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 
			171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 
			187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 
			203, 204, 205, 206, 207, 35, 208, 209, 210, 211, 50, 56, 212, 213, 214, 215, 
			216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 
			232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 
			248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 
			264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 
			280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 
			296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 
			312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 
			328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 
			344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 
			360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 
			376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 
			392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 
			408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 
			424, 425, 426, 427, 428, 429, 8, 430, 431, 432, 433, 434, 435, 436, 437, 438, 
			439, 440
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 168, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 387, 582, 582, 582, 582, 582, 388, 389, 582, 582, 582, 582, 390, -1, 391, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 392, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 22, 23, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1, -1 },
			{ 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, -1, 245, 245, 245, 247, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 50, 245, 245 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 58, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 175, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 265, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, 49, 49, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, 49, -1, -1, -1, -1, -1, -1 },
			{ -1, 569, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 66, -1, -1, -1, 66, 66, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, 66, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, 66, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 284, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 304, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 305, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, 305, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, 305, -1, -1, -1, -1, -1 },
			{ -1, 573, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 498, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 590, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, -1, 146, 146, 146, 345, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, -1, 146, 146 },
			{ -1, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, -1, 152, 152, 152, 152, 152, 152, 152, 152, -1, -1, 152, 152, 152, -1, -1, -1, -1, 152, -1, -1, -1, 152, 152, 152, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 152, 152, -1, -1, -1, -1, -1, -1 },
			{ 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1, -1, 156, 156, 156, 156, 156, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1, 156, 156, 156, 156, 156 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 162, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 162, 162, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 164, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 164, 164, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 190, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, -1, 357, 357, 357, 357, 357, 357, 357, 357, -1, -1, 357, 357, 357, -1, -1, -1, -1, 357, -1, -1, -1, 357, 357, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, 155, 357, 357, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 18, 582, 393, 582, 582, 394, 582, 582, 582, -1, 516, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 207, 240, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 243, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1 },
			{ -1, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, -1, 178, 178, 178, 178, 178, 178, 178, 178, 339, -1, 178, 178, 178, -1, -1, -1, -1, 178, -1, -1, -1, 178, 178, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 144, 178, 178, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 92, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 142, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 153, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 159, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, -1, 49, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, 164, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 164, 164, -1, -1, -1, -1, -1, -1 },
			{ -1, 189, -1, -1, -1, 189, 189, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, 189, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, 189, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 396, 582, 214, 582, 582, 582, 582, 582, 582, 19, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 175, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, -1, 178, 178, 178, 178, 178, 178, 178, 178, -1, -1, 178, -1, 178, -1, -1, -1, -1, 178, -1, -1, -1, 178, 178, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 143, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 20, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, -1, -1, -1, -1, -1 },
			{ 6, 7, 366, 171, 377, 191, 380, 382, 384, 515, 199, 544, 563, 574, 580, 8, 582, 202, 582, 584, 205, 582, 585, 586, 9, 172, 582, 10, 582, 192, 11, 200, 203, 386, 206, 8, 209, 582, 587, 582, 212, 215, 218, 221, 224, 227, 230, 233, 236, 239, 209, 12, 242, 13, 209, 173, 10, 8, 209, 14, 15, 16, 17 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 226, 582, 582, 21, 518, 582, 582, -1, 582, 582, 582, 582, 545, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 522, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, 204, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, 15, -1, 17 },
			{ -1, -1, -1, 213, -1, 216, 219, 368, -1, -1, 222, 225, 228, -1, -1, -1, -1, 231, -1, -1, 234, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 51, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 52, 582, -1, 582, 416, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 369, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 53, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 251, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 54, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 253, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 55, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 56, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 57, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, 42, 16, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 259, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 60, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 68, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 261, -1, -1, -1, -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 69, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 70, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 71, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 72, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 73, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, -1, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245, 245 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 74, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 75, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 269, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 76, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 77, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 271, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 78, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 79, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 275, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 80, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 279, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 81, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, -1, 281, 281, 281, 281, 281, 281, 281, 281, -1, -1, 281, -1, 281, -1, -1, -1, -1, 281, -1, 263, -1, 281, 281, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 283, -1, 375 },
			{ -1, 582, 582, 582, 82, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 83, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 285, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 287, 91, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 84, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 85, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 86, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 87, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 88, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 89, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 381, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 90, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 281, 92, 281, 281, 281, 281, 281, 281, 281, 281, -1, -1, 281, 281, 281, -1, -1, -1, -1, 281, -1, -1, -1, 281, 281, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 281, 281, 179, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 93, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, -1, 301, 301, 301, 301, 301, 301, 301, 301, -1, -1, 301, -1, 301, -1, -1, -1, -1, 301, -1, -1, -1, 301, 301, 301, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 94, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 95, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 287, 91, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 96, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 97, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 98, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 99, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, 100, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 102, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 103, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 104, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, -1, 301, 301, 301, 301, 301, 301, 301, 301, -1, -1, 301, 301, 301, -1, -1, -1, -1, 301, -1, -1, -1, 301, 301, 301, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 301, 301, -1, -1, -1, 325, -1, -1 },
			{ -1, 105, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, -1, 303, 303, 303, 303, 303, 303, 303, 303, -1, -1, 303, 303, 303, -1, -1, -1, -1, 303, -1, -1, -1, 303, 303, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 303, 303, -1, -1, -1, -1, -1, 325 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 106, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 383, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 107, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 108, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 109, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 311, 110, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 112, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 113, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 114, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, 111, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 115, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 116, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 117, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 120, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 92, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 179, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 121, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, 118, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 122, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, 119, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 123, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 124, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 125, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 133, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 126, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 2, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 140, 180, 194, 140, 140, 140, 140, 140, 140, 140, 140, 140, 141 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 127, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 128, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, -1, 145, 145, 145, 145, 145, 145, 145, 145, -1, -1, 145, -1, 145, -1, -1, -1, -1, 145, -1, -1, -1, 145, 145, 145, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 129, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 2, 146, 146, 146, 345, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 147, 146, 146 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 130, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, -1, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146 },
			{ -1, 582, 582, 582, 582, 582, 131, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 2, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 181, 195, 148, 148, 148, 148, 148, 148, 149, 148, 148, 148 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 132, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 2, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 182, 196, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 134, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 135, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 151, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 8, 152, 152, 152, 152, 152, 152, 152, 152, 183, 151, 152, 151, 152, 151, 151, 151, 151, 152, 151, 8, 151, 152, 152, 152, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 8, 151, 151, 151, 151, 151 },
			{ -1, 136, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 154, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 154, 184, 184, 184, 184, 184, 184, 184, 184, 154, 154, 169, 154, 184, 154, 154, 154, 154, 184, 154, 154, 154, 184, 184, 184, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 137, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 138, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 157, 156, 156, 156, 156, 156, 156, 156, 156, 156, 158, 2, 156, 156, 156, 156, 156, 186, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 186, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 185, 156, 156, 156, 156, 156 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 139, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ 6, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 160, 582, 582, 582, 582, 582, 582, 582, 582, 161, 161, 582, 162, 582, 161, 160, 161, 161, 582, 161, 160, 161, 582, 582, 582, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 187, 161, 161, 188, 162, 160, 163, 161, 160, 160, 161 },
			{ 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 2, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165 },
			{ 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 2, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 208, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 267, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 273, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 291, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, 303, -1, 303, 303, 303, 303, 303, 303, 303, 303, -1, -1, 303, -1, 303, -1, -1, -1, -1, 303, -1, -1, -1, 303, 303, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 211, 582, 582, -1, 582, 582, 395, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 277, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 397, 582, 582, 582, 519, 582, 582, 217, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 517, 582, 582, 220, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 223, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 398, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 287, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 229, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 232, 547, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 411, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 235, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 238, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 412, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 241, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 564, 582, 582, 582, 582, 413, 582, 414, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 415, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 417, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 520, 582, 582, 548, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 418, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 575, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 422, 582, 582, 582, 582, -1, 582, 423, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 424, 582, 582, 582, 582, 582, 582, 244, 582, 582, 592, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 525, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 549, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 425, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 526, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 426, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 246, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 248, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 523, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 565, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 430, 582, 582, 582, 582, 582, 582, 577, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 431, 432, 433, 582, 434, 576, 582, 582, 582, 582, 528, -1, 435, 582, 529, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 250, 582, 530, 437, 582, 582, 582, 582, 438, 582, 582, 582, -1, 582, 582, 582, 439, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 252, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 550, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 440, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 254, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 256, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 258, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 260, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 441, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 262, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 552, 582, 582, 582, 582, 582, 582, 264, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 266, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 268, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 270, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 445, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 272, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 274, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 276, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 278, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 280, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 581, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 583, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 448, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 449, 582, 582, 582, 531, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 450, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 532, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 452, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 282, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 454, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 536, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 534, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 457, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 557, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 461, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 286, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 288, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 290, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 292, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 294, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 465, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 466, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 538, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 467, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 296, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 470, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 540, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 472, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 298, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 474, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 300, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 302, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 306, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 541, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 477, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 308, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 310, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 312, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 479, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 559, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 481, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 482, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 561, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 314, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 484, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 485, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 486, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 316, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 318, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 320, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 322, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 324, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 326, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 494, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 495, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 328, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 330, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 332, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 499, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 500, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 334, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 336, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 338, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 593, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 501, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 340, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 502, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 542, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 342, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 344, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 503, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 346, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 348, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 504, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 350, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 506, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 509, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 510, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 352, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 354, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 356, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 511, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 512, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 358, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 513, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 514, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 360, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 546, 582, 582, 582, 399, -1, 582, 400, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 524, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 420, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 427, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 419, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 567, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 428, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 429, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 446, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 554, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 443, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 568, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 455, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 555, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 533, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 453, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 591, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 468, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 469, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 473, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 560, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 471, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 476, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 539, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 492, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 483, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 488, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 505, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 507, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 401, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 402, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 566, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 421, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 436, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 553, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 444, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 456, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 556, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 537, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 459, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 588, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 558, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 478, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 475, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 480, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 493, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 489, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 496, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 508, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 403, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 527, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 447, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 551, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 458, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 463, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 460, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 535, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 487, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 490, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 497, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 404, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 442, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 451, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 570, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 462, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 491, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 405, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 464, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 589, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 521, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 406, 582, 582, 582, 407, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 408, 582, 582, 582, 582, 409, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 410, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 571, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 572, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 543, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 579, 582, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 582, 578, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 },
			{ -1, 582, 582, 582, 582, 582, 582, 582, 582, 582, 562, 582, 582, 582, 582, -1, 582, 582, 582, 582, 582, 582, 582, 582, -1, -1, 582, 582, 582, -1, -1, -1, -1, 582, -1, -1, -1, 582, 582, 582, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 582, 582, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  201,
			  337,
			  343,
			  347,
			  349,
			  351,
			  353,
			  355,
			  351,
			  351,
			  359,
			  361,
			  364,
			  365
		};
		
		#endregion
		
		public Tokens GetNextToken()
		{
			int current_state = yy_state_dtrans[(int)current_lexical_state];
			int last_accept_state = NoState;
			bool is_initial_state = true;
			
			MarkTokenChunkStart();
			token_start = token_chunk_start;
			expanding_token = false;
			AdvanceEndPosition((token_end > 0) ? token_end - 1 : 0, token_start);
			
			// capture token start position:
			token_start_pos.Char = token_end_pos.Char;
			
			if (acceptCondition[current_state] != AcceptConditions.NotAccept)
			{
				last_accept_state = current_state;
				MarkTokenEnd();
			}
			
			while (true)
			{
				char lookahead = (is_initial_state && yy_at_bol) ? BOL : Advance();
				int next_state = nextState[rowMap[current_state], colMap[lookahead]];
				
				if (lookahead == EOF && is_initial_state)
				{
					return Tokens.END;
				}
				if (next_state != -1)
				{
					current_state = next_state;
					is_initial_state = false;
					
					if (acceptCondition[current_state] != AcceptConditions.NotAccept)
					{
						last_accept_state = current_state;
						MarkTokenEnd();
					}
				}
				else
				{
					if (last_accept_state == NoState)
					{
						return Tokens.T_ERROR;
					}
					else
					{
						if ((acceptCondition[last_accept_state] & AcceptConditions.AcceptOnEnd) != 0)
							TrimTokenEnd();
						MoveToTokenEnd();
						
						if (last_accept_state < 0)
						{
							System.Diagnostics.Debug.Assert(last_accept_state >= 594);
						}
						else
						{
							bool accepted = false;
							yyreturn = Accept0(last_accept_state, out accepted);
							if (accepted)
							{
								AdvanceEndPosition(token_start, token_end - 1);
								return yyreturn;
							}
						}
						
						// token ignored:
						is_initial_state = true;
						current_state = yy_state_dtrans[(int)current_lexical_state];
						last_accept_state = NoState;
						MarkTokenChunkStart();
						if (acceptCondition[current_state] != AcceptConditions.NotAccept)
						{
							last_accept_state = current_state;
							MarkTokenEnd();
						}
					}
				}
			}
		} // end of GetNextToken
	}
}

