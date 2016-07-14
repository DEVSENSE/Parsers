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
		
		protected Lexer(System.IO.TextReader reader)
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
				case 2:
					// #line 616
					{ 
						return Tokens.T_INLINE_HTML; 
					}
					break;
					
				case 3:
					// #line 633
					{
						if (this._allowShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							yymore(); break;//return Tokens.T_INLINE_HTML;
						}
					}
					break;
					
				case 4:
					// #line 620
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 5:
					// #line 626
					{
						//HANDLE_NEWLINE(yytext[yyleng-1]);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 6:
					// #line 680
					{
						return ProcessLabel();
					}
					break;
					
				case 7:
					// #line 235
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 8:
					// #line 519
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 564
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 10:
					// #line 820
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 11:
					// #line 259
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 12:
					// #line 524
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens)'{';
					}
					break;
					
				case 13:
					// #line 536
					{
						ResetDocComment();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens)'}';
					}
					break;
					
				case 14:
					// #line 756
					{
						BEGIN(LexicalStates.ST_BACKQUOTE); 
						return (Tokens)'`';
					}
					break;
					
				case 15:
					// #line 708
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
					// #line 685
					{
						BEGIN(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 17:
					// #line 723
					{
						BEGIN(LexicalStates.ST_DOUBLE_QUOTES);
						return (Tokens)'"';
					}
					break;
					
				case 18:
					// #line 114
					{
						return (Tokens.T_IF);
					}
					break;
					
				case 19:
					// #line 138
					{
						return (Tokens.T_DO);
					}
					break;
					
				case 20:
					// #line 499
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 21:
					// #line 170
					{
						return (Tokens.T_AS);
					}
					break;
					
				case 22:
					// #line 407
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 23:
					// #line 230
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 24:
					// #line 443
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 25:
					// #line 515
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 26:
					// #line 435
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 27:
					// #line 580
					{
						return ProcessRealNumber();
					}
					break;
					
				case 28:
					// #line 255
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 29:
					// #line 463
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 30:
					// #line 701
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 31:
					// #line 267
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 32:
					// #line 387
					{
						return (Tokens.T_DOUBLE_ARROW);
					}
					break;
					
				case 33:
					// #line 419
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 34:
					// #line 439
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 35:
					// #line 403
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 36:
					// #line 423
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 37:
					// #line 431
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 38:
					// #line 511
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 39:
					// #line 447
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 40:
					// #line 451
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 41:
					// #line 459
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 42:
					// #line 691
					{ BEGIN(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 43:
					// #line 467
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 44:
					// #line 479
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 45:
					// #line 495
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 46:
					// #line 483
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 47:
					// #line 491
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 48:
					// #line 487
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 49:
					// #line 659
					{
						return ProcessVariable();
					}
					break;
					
				case 50:
					// #line 706
					{ return ProcessSingleQuotedString(); }
					break;
					
				case 51:
					// #line 719
					{
						return ProcessDoubleQuotedString();
					}
					break;
					
				case 52:
					// #line 507
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 53:
					// #line 98
					{
						return (Tokens.T_TRY);
					}
					break;
					
				case 54:
					// #line 73
					{
						return (Tokens.T_EXIT);
					}
					break;
					
				case 55:
					// #line 142
					{
						return (Tokens.T_FOR);
					}
					break;
					
				case 56:
					// #line 335
					{
						return (Tokens.T_USE);
					}
					break;
					
				case 57:
					// #line 271
					{
						return (Tokens.T_NEW);
					}
					break;
					
				case 58:
					// #line 503
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 59:
					// #line 475
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 60:
					// #line 263
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 61:
					// #line 279
					{
						return (Tokens.T_VAR);
					}
					break;
					
				case 62:
					// #line 411
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 63:
					// #line 415
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 64:
					// #line 427
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 65:
					// #line 471
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 66:
					// #line 455
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 67:
					// #line 568
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 68:
					// #line 560
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 69:
					// #line 69
					{ 
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 70:
					// #line 202
					{
						return (Tokens.T_ECHO);
					}
					break;
					
				case 71:
					// #line 126
					{
						return (Tokens.T_ELSE);
					}
					break;
					
				case 72:
					// #line 311
					{
						return (Tokens.T_EVAL);
					}
					break;
					
				case 73:
					// #line 182
					{
						return (Tokens.T_CASE);
					}
					break;
					
				case 74:
					// #line 391
					{
						return (Tokens.T_LIST);
					}
					break;
					
				case 75:
					// #line 198
					{
						return (Tokens.T_GOTO);
					}
					break;
					
				case 76:
					// #line 696
					{ BEGIN(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocComment(); break; }
					break;
					
				case 77:
					// #line 122
					{
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 78:
					// #line 351
					{
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 79:
					// #line 347
					{
						return (Tokens.T_ISSET);
					}
					break;
					
				case 80:
					// #line 218
					{
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 81:
					// #line 110
					{
						return (Tokens.T_THROW);
					}
					break;
					
				case 82:
					// #line 367
					{
						return (Tokens.T_FINAL);
					}
					break;
					
				case 83:
					// #line 383
					{
						return (Tokens.T_UNSET);
					}
					break;
					
				case 84:
					// #line 81
					{
						return (Tokens.T_CONST);
					}
					break;
					
				case 85:
					// #line 275
					{
						return (Tokens.T_CLONE);
					}
					break;
					
				case 86:
					// #line 210
					{
						return (Tokens.T_CLASS);
					}
					break;
					
				case 87:
					// #line 102
					{
						return (Tokens.T_CATCH);
					}
					break;
					
				case 88:
					// #line 94
					{
						return (Tokens.T_YIELD);
					}
					break;
					
				case 89:
					// #line 395
					{
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 90:
					// #line 130
					{
						return (Tokens.T_WHILE);
					}
					break;
					
				case 91:
					// #line 190
					{
						return (Tokens.T_BREAK);
					}
					break;
					
				case 92:
					// #line 206
					{
						return (Tokens.T_PRINT);
					}
					break;
					
				case 93:
					// #line 283
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 94:
					// #line 729
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
					    this._hereDocLabel = GetTokenSubstring(s, length);
					    return (Tokens.T_START_HEREDOC);
					}
					break;
					
				case 95:
					// #line 146
					{
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 96:
					// #line 118
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 97:
					// #line 359
					{
						return (Tokens.T_STATIC);
					}
					break;
					
				case 98:
					// #line 174
					{
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 99:
					// #line 85
					{
						return (Tokens.T_RETURN);
					}
					break;
					
				case 100:
					// #line 343
					{
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 101:
					// #line 379
					{
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 102:
					// #line 287
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 103:
					// #line 303
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 104:
					// #line 222
					{
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 105:
					// #line 315
					{
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 106:
					// #line 186
					{
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 107:
					// #line 158
					{
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 108:
					// #line 106
					{
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 109:
					// #line 150
					{
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 110:
					// #line 323
					{
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 111:
					// #line 371
					{
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 112:
					// #line 307
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 113:
					// #line 295
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 114:
					// #line 608
					{
						return (Tokens.T_DIR);
					}
					break;
					
				case 115:
					// #line 134
					{
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 116:
					// #line 77
					{
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 117:
					// #line 194
					{
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 118:
					// #line 399
					{
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 119:
					// #line 363
					{
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 120:
					// #line 299
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 121:
					// #line 291
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 122:
					// #line 604
					{
						return (Tokens.T_FILE);
					}
					break;
					
				case 123:
					// #line 600
					{
						return (Tokens.T_LINE);
					}
					break;
					
				case 124:
					// #line 178
					{
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 125:
					// #line 214
					{
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 126:
					// #line 339
					{
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 127:
					// #line 331
					{
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 128:
					// #line 375
					{
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 129:
					// #line 588
					{
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 130:
					// #line 584
					{
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 131:
					// #line 162
					{
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 132:
					// #line 154
					{
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 133:
					// #line 166
					{
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 134:
					// #line 226
					{
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 135:
					// #line 89
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 136:
					// #line 596
					{
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 137:
					// #line 319
					{
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 138:
					// #line 327
					{
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 139:
					// #line 592
					{
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 140:
					// #line 612
					{
						return (Tokens.T_NS_C);
					}
					break;
					
				case 141:
					// #line 355
					{
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 142:
					// #line 802
					{
					    this._tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this._sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 143:
					// #line 778
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens)'"';
					}
					break;
					
				case 144:
					// #line 769
					{
						//Z_LVAL_P(zendlval) = (zend_long) '{';
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 145:
					// #line 530
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 146:
					// #line 653
					{
						yyless(TokenLength - 1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 147:
					// #line 647
					{
						yyless(TokenLength - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 148:
					// #line 716
					{ yymore(); break; }
					break;
					
				case 149:
					// #line 717
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 150:
					// #line 807
					{
					    this._tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this._sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 151:
					// #line 783
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens)'`';
					}
					break;
					
				case 152:
					// #line 814
					{
					    this._tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this._sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 153:
					// #line 812
					{ yymore(); break; }
					break;
					
				case 154:
					// #line 794
					{
					    if(GetTokenString().Contains(this._hereDocLabel))
							return ProcessEndNowDoc(s => (string)ProcessEscapedString(s, this._sourceUnit.Encoding, false));
					    yymore(); break;
					}
					break;
					
				case 155:
					// #line 244
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 156:
					// #line 249
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 157:
					// #line 240
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 158:
					// #line 553
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 159:
					// #line 544
					{
						yyless(TokenLength - 1);
						this._tokenSemantics.Object = GetTokenString();
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 160:
					// #line 697
					{ yymore(); break; }
					break;
					
				case 161:
					// #line 699
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 698
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); SetDocComment(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 163:
					// #line 692
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 694
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 693
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return Tokens.T_COMMENT; }
					break;
					
				case 166:
					// #line 827
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 828
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return Tokens.T_COMMENT; }
					break;
					
				case 168:
					// #line 826
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 830
					{ yymore(); break; }
					break;
					
				case 170:
					// #line 673
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 171:
					// #line 668
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 172:
					// #line 572
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 173:
					// #line 663
					{
						yy_pop_state();
						return (Tokens)']';
					}
					break;
					
				case 174:
					// #line 576
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 175:
					// #line 762
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						this._tokenSemantics.Object = this._hereDocLabel;
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 176:
					// #line 800
					{ yymore(); break; }
					break;
					
				case 177:
					// #line 788
					{
					    if(GetTokenString().Contains(this._hereDocLabel))
							return ProcessEndNowDoc(s => s);
					    yymore(); break;
					}
					break;
					
				case 180: goto case 2;
				case 181: goto case 5;
				case 182: goto case 6;
				case 183: goto case 8;
				case 184: goto case 9;
				case 185: goto case 27;
				case 186: goto case 30;
				case 187: goto case 36;
				case 188: goto case 49;
				case 189: goto case 94;
				case 190: goto case 142;
				case 191: goto case 150;
				case 192: goto case 152;
				case 193: goto case 153;
				case 194: goto case 154;
				case 195: goto case 156;
				case 196: goto case 158;
				case 197: goto case 167;
				case 198: goto case 168;
				case 199: goto case 171;
				case 200: goto case 172;
				case 201: goto case 174;
				case 202: goto case 177;
				case 204: goto case 6;
				case 205: goto case 8;
				case 206: goto case 30;
				case 207: goto case 174;
				case 209: goto case 6;
				case 210: goto case 8;
				case 212: goto case 6;
				case 213: goto case 8;
				case 215: goto case 6;
				case 216: goto case 8;
				case 218: goto case 6;
				case 219: goto case 8;
				case 221: goto case 6;
				case 222: goto case 8;
				case 224: goto case 6;
				case 225: goto case 8;
				case 227: goto case 6;
				case 228: goto case 8;
				case 230: goto case 6;
				case 231: goto case 8;
				case 233: goto case 6;
				case 234: goto case 8;
				case 236: goto case 6;
				case 237: goto case 8;
				case 239: goto case 6;
				case 240: goto case 8;
				case 242: goto case 6;
				case 243: goto case 8;
				case 245: goto case 6;
				case 246: goto case 8;
				case 248: goto case 6;
				case 249: goto case 8;
				case 251: goto case 6;
				case 252: goto case 8;
				case 254: goto case 6;
				case 256: goto case 6;
				case 258: goto case 6;
				case 260: goto case 6;
				case 262: goto case 6;
				case 264: goto case 6;
				case 266: goto case 6;
				case 268: goto case 6;
				case 270: goto case 6;
				case 272: goto case 6;
				case 274: goto case 6;
				case 276: goto case 6;
				case 278: goto case 6;
				case 280: goto case 6;
				case 282: goto case 6;
				case 284: goto case 6;
				case 286: goto case 6;
				case 288: goto case 6;
				case 290: goto case 6;
				case 292: goto case 6;
				case 294: goto case 6;
				case 296: goto case 6;
				case 298: goto case 6;
				case 300: goto case 6;
				case 302: goto case 6;
				case 304: goto case 6;
				case 306: goto case 6;
				case 308: goto case 6;
				case 310: goto case 6;
				case 312: goto case 6;
				case 314: goto case 6;
				case 316: goto case 6;
				case 318: goto case 6;
				case 320: goto case 6;
				case 322: goto case 6;
				case 324: goto case 6;
				case 326: goto case 6;
				case 328: goto case 6;
				case 330: goto case 6;
				case 332: goto case 6;
				case 334: goto case 6;
				case 336: goto case 6;
				case 338: goto case 6;
				case 340: goto case 6;
				case 342: goto case 6;
				case 344: goto case 6;
				case 346: goto case 6;
				case 348: goto case 6;
				case 350: goto case 6;
				case 352: goto case 6;
				case 354: goto case 6;
				case 356: goto case 6;
				case 358: goto case 6;
				case 360: goto case 6;
				case 362: goto case 6;
				case 364: goto case 6;
				case 366: goto case 6;
				case 368: goto case 6;
				case 370: goto case 6;
				case 393: goto case 6;
				case 405: goto case 6;
				case 408: goto case 6;
				case 410: goto case 6;
				case 412: goto case 6;
				case 414: goto case 6;
				case 415: goto case 6;
				case 416: goto case 6;
				case 417: goto case 6;
				case 418: goto case 6;
				case 419: goto case 6;
				case 420: goto case 6;
				case 421: goto case 6;
				case 422: goto case 6;
				case 423: goto case 6;
				case 424: goto case 6;
				case 425: goto case 6;
				case 426: goto case 6;
				case 427: goto case 6;
				case 428: goto case 6;
				case 429: goto case 6;
				case 430: goto case 6;
				case 431: goto case 6;
				case 432: goto case 6;
				case 433: goto case 6;
				case 434: goto case 6;
				case 435: goto case 6;
				case 436: goto case 6;
				case 437: goto case 6;
				case 438: goto case 6;
				case 439: goto case 6;
				case 440: goto case 6;
				case 441: goto case 6;
				case 442: goto case 6;
				case 443: goto case 6;
				case 444: goto case 6;
				case 445: goto case 6;
				case 446: goto case 6;
				case 447: goto case 6;
				case 448: goto case 6;
				case 449: goto case 6;
				case 450: goto case 6;
				case 451: goto case 6;
				case 452: goto case 6;
				case 453: goto case 6;
				case 454: goto case 6;
				case 455: goto case 6;
				case 456: goto case 6;
				case 457: goto case 6;
				case 458: goto case 6;
				case 459: goto case 6;
				case 460: goto case 6;
				case 461: goto case 6;
				case 462: goto case 6;
				case 463: goto case 6;
				case 464: goto case 6;
				case 465: goto case 6;
				case 466: goto case 6;
				case 467: goto case 6;
				case 468: goto case 6;
				case 469: goto case 6;
				case 470: goto case 6;
				case 471: goto case 6;
				case 472: goto case 6;
				case 473: goto case 6;
				case 474: goto case 6;
				case 475: goto case 6;
				case 476: goto case 6;
				case 477: goto case 6;
				case 478: goto case 6;
				case 479: goto case 6;
				case 480: goto case 6;
				case 481: goto case 6;
				case 482: goto case 6;
				case 483: goto case 6;
				case 484: goto case 6;
				case 485: goto case 6;
				case 486: goto case 6;
				case 487: goto case 6;
				case 488: goto case 6;
				case 489: goto case 6;
				case 490: goto case 6;
				case 491: goto case 6;
				case 492: goto case 6;
				case 493: goto case 6;
				case 494: goto case 6;
				case 495: goto case 6;
				case 496: goto case 6;
				case 497: goto case 6;
				case 498: goto case 6;
				case 499: goto case 6;
				case 500: goto case 6;
				case 501: goto case 6;
				case 502: goto case 6;
				case 503: goto case 6;
				case 504: goto case 6;
				case 505: goto case 6;
				case 506: goto case 6;
				case 507: goto case 6;
				case 508: goto case 6;
				case 509: goto case 6;
				case 510: goto case 6;
				case 511: goto case 6;
				case 512: goto case 6;
				case 513: goto case 6;
				case 514: goto case 6;
				case 515: goto case 6;
				case 516: goto case 6;
				case 517: goto case 6;
				case 518: goto case 6;
				case 519: goto case 6;
				case 520: goto case 6;
				case 521: goto case 6;
				case 522: goto case 6;
				case 523: goto case 6;
				case 524: goto case 6;
				case 525: goto case 6;
				case 526: goto case 6;
				case 527: goto case 6;
				case 528: goto case 6;
				case 529: goto case 6;
				case 530: goto case 6;
				case 531: goto case 6;
				case 532: goto case 6;
				case 533: goto case 6;
				case 534: goto case 6;
				case 535: goto case 6;
				case 536: goto case 6;
				case 537: goto case 6;
				case 538: goto case 6;
				case 539: goto case 6;
				case 540: goto case 6;
				case 541: goto case 6;
				case 542: goto case 6;
				case 543: goto case 6;
				case 544: goto case 6;
				case 545: goto case 6;
				case 546: goto case 6;
				case 547: goto case 6;
				case 548: goto case 6;
				case 549: goto case 6;
				case 550: goto case 6;
				case 551: goto case 6;
				case 552: goto case 6;
				case 553: goto case 6;
				case 554: goto case 6;
				case 555: goto case 6;
				case 556: goto case 6;
				case 557: goto case 6;
				case 558: goto case 6;
				case 559: goto case 6;
				case 560: goto case 6;
				case 561: goto case 6;
				case 562: goto case 6;
				case 563: goto case 6;
				case 564: goto case 6;
				case 565: goto case 6;
				case 566: goto case 6;
				case 567: goto case 6;
				case 568: goto case 6;
				case 569: goto case 6;
				case 570: goto case 6;
				case 571: goto case 6;
				case 572: goto case 6;
				case 573: goto case 6;
				case 574: goto case 6;
				case 575: goto case 6;
				case 576: goto case 6;
				case 577: goto case 6;
				case 578: goto case 6;
				case 579: goto case 6;
				case 580: goto case 6;
				case 581: goto case 6;
				case 582: goto case 6;
				case 583: goto case 6;
				case 584: goto case 6;
				case 585: goto case 6;
				case 586: goto case 6;
				case 587: goto case 6;
				case 588: goto case 6;
				case 589: goto case 6;
				case 590: goto case 6;
				case 591: goto case 6;
				case 592: goto case 6;
				case 593: goto case 6;
				case 594: goto case 6;
				case 595: goto case 6;
				case 596: goto case 6;
				case 597: goto case 6;
				case 598: goto case 6;
				case 599: goto case 6;
				case 600: goto case 6;
				case 601: goto case 6;
				case 602: goto case 6;
				case 603: goto case 6;
				case 604: goto case 6;
				case 605: goto case 6;
				case 606: goto case 6;
				case 607: goto case 6;
				case 608: goto case 6;
				case 609: goto case 6;
				case 610: goto case 6;
				case 611: goto case 6;
				case 612: goto case 6;
				case 613: goto case 6;
				case 614: goto case 6;
				case 615: goto case 6;
				case 616: goto case 6;
				case 617: goto case 6;
				case 618: goto case 6;
				case 619: goto case 6;
				case 620: goto case 6;
				case 621: goto case 6;
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
			AcceptConditions.AcceptOnStart, // 154
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
			AcceptConditions.Accept, // 167
			AcceptConditions.Accept, // 168
			AcceptConditions.Accept, // 169
			AcceptConditions.Accept, // 170
			AcceptConditions.Accept, // 171
			AcceptConditions.Accept, // 172
			AcceptConditions.Accept, // 173
			AcceptConditions.Accept, // 174
			AcceptConditions.Accept, // 175
			AcceptConditions.Accept, // 176
			AcceptConditions.AcceptOnStart, // 177
			AcceptConditions.NotAccept, // 178
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
			AcceptConditions.Accept, // 190
			AcceptConditions.Accept, // 191
			AcceptConditions.Accept, // 192
			AcceptConditions.Accept, // 193
			AcceptConditions.AcceptOnStart, // 194
			AcceptConditions.Accept, // 195
			AcceptConditions.Accept, // 196
			AcceptConditions.Accept, // 197
			AcceptConditions.Accept, // 198
			AcceptConditions.Accept, // 199
			AcceptConditions.Accept, // 200
			AcceptConditions.Accept, // 201
			AcceptConditions.AcceptOnStart, // 202
			AcceptConditions.NotAccept, // 203
			AcceptConditions.Accept, // 204
			AcceptConditions.Accept, // 205
			AcceptConditions.Accept, // 206
			AcceptConditions.Accept, // 207
			AcceptConditions.NotAccept, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.Accept, // 210
			AcceptConditions.NotAccept, // 211
			AcceptConditions.Accept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.NotAccept, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.Accept, // 216
			AcceptConditions.NotAccept, // 217
			AcceptConditions.Accept, // 218
			AcceptConditions.Accept, // 219
			AcceptConditions.NotAccept, // 220
			AcceptConditions.Accept, // 221
			AcceptConditions.Accept, // 222
			AcceptConditions.NotAccept, // 223
			AcceptConditions.Accept, // 224
			AcceptConditions.Accept, // 225
			AcceptConditions.NotAccept, // 226
			AcceptConditions.Accept, // 227
			AcceptConditions.Accept, // 228
			AcceptConditions.NotAccept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.NotAccept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.Accept, // 234
			AcceptConditions.NotAccept, // 235
			AcceptConditions.Accept, // 236
			AcceptConditions.Accept, // 237
			AcceptConditions.NotAccept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.Accept, // 240
			AcceptConditions.NotAccept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.NotAccept, // 244
			AcceptConditions.Accept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.NotAccept, // 247
			AcceptConditions.Accept, // 248
			AcceptConditions.Accept, // 249
			AcceptConditions.NotAccept, // 250
			AcceptConditions.Accept, // 251
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
			AcceptConditions.Accept, // 362
			AcceptConditions.NotAccept, // 363
			AcceptConditions.Accept, // 364
			AcceptConditions.NotAccept, // 365
			AcceptConditions.Accept, // 366
			AcceptConditions.NotAccept, // 367
			AcceptConditions.Accept, // 368
			AcceptConditions.NotAccept, // 369
			AcceptConditions.Accept, // 370
			AcceptConditions.NotAccept, // 371
			AcceptConditions.NotAccept, // 372
			AcceptConditions.NotAccept, // 373
			AcceptConditions.NotAccept, // 374
			AcceptConditions.NotAccept, // 375
			AcceptConditions.NotAccept, // 376
			AcceptConditions.NotAccept, // 377
			AcceptConditions.NotAccept, // 378
			AcceptConditions.NotAccept, // 379
			AcceptConditions.NotAccept, // 380
			AcceptConditions.NotAccept, // 381
			AcceptConditions.NotAccept, // 382
			AcceptConditions.NotAccept, // 383
			AcceptConditions.NotAccept, // 384
			AcceptConditions.NotAccept, // 385
			AcceptConditions.NotAccept, // 386
			AcceptConditions.NotAccept, // 387
			AcceptConditions.NotAccept, // 388
			AcceptConditions.NotAccept, // 389
			AcceptConditions.NotAccept, // 390
			AcceptConditions.NotAccept, // 391
			AcceptConditions.NotAccept, // 392
			AcceptConditions.Accept, // 393
			AcceptConditions.Accept, // 394
			AcceptConditions.NotAccept, // 395
			AcceptConditions.NotAccept, // 396
			AcceptConditions.NotAccept, // 397
			AcceptConditions.NotAccept, // 398
			AcceptConditions.NotAccept, // 399
			AcceptConditions.NotAccept, // 400
			AcceptConditions.NotAccept, // 401
			AcceptConditions.NotAccept, // 402
			AcceptConditions.NotAccept, // 403
			AcceptConditions.NotAccept, // 404
			AcceptConditions.Accept, // 405
			AcceptConditions.NotAccept, // 406
			AcceptConditions.NotAccept, // 407
			AcceptConditions.Accept, // 408
			AcceptConditions.NotAccept, // 409
			AcceptConditions.Accept, // 410
			AcceptConditions.NotAccept, // 411
			AcceptConditions.Accept, // 412
			AcceptConditions.NotAccept, // 413
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
			AcceptConditions.Accept, // 594
			AcceptConditions.Accept, // 595
			AcceptConditions.Accept, // 596
			AcceptConditions.Accept, // 597
			AcceptConditions.Accept, // 598
			AcceptConditions.Accept, // 599
			AcceptConditions.Accept, // 600
			AcceptConditions.Accept, // 601
			AcceptConditions.Accept, // 602
			AcceptConditions.Accept, // 603
			AcceptConditions.Accept, // 604
			AcceptConditions.Accept, // 605
			AcceptConditions.Accept, // 606
			AcceptConditions.Accept, // 607
			AcceptConditions.Accept, // 608
			AcceptConditions.Accept, // 609
			AcceptConditions.Accept, // 610
			AcceptConditions.Accept, // 611
			AcceptConditions.Accept, // 612
			AcceptConditions.Accept, // 613
			AcceptConditions.Accept, // 614
			AcceptConditions.Accept, // 615
			AcceptConditions.Accept, // 616
			AcceptConditions.Accept, // 617
			AcceptConditions.Accept, // 618
			AcceptConditions.Accept, // 619
			AcceptConditions.Accept, // 620
			AcceptConditions.Accept, // 621
		};
		
		private static int[] colMap = new int[]
		{
			28, 28, 28, 28, 28, 28, 28, 28, 28, 35, 15, 28, 28, 57, 28, 28, 
			28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 
			35, 42, 63, 62, 52, 46, 47, 61, 34, 36, 44, 41, 50, 24, 31, 45, 
			55, 56, 27, 27, 27, 27, 27, 27, 58, 58, 29, 64, 43, 40, 25, 32, 
			50, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 26, 54, 30, 59, 49, 38, 
			60, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 26, 51, 48, 53, 50, 28, 
			65, 0
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 2, 3, 1, 1, 4, 5, 6, 7, 1, 1, 1, 1, 1, 8, 
			1, 9, 10, 10, 10, 10, 1, 1, 1, 11, 1, 12, 1, 1, 13, 1, 
			1, 14, 1, 1, 15, 16, 17, 1, 18, 1, 19, 1, 1, 1, 1, 1, 
			1, 20, 1, 1, 10, 10, 10, 21, 10, 10, 10, 1, 1, 10, 1, 1, 
			1, 1, 1, 22, 23, 10, 10, 24, 10, 10, 10, 10, 25, 10, 10, 10, 
			10, 10, 26, 10, 10, 10, 10, 10, 27, 10, 10, 10, 10, 1, 1, 28, 
			10, 10, 10, 10, 10, 10, 1, 1, 10, 29, 10, 10, 10, 10, 30, 10, 
			1, 1, 10, 10, 10, 10, 10, 10, 1, 1, 10, 10, 10, 10, 10, 10, 
			10, 10, 10, 10, 10, 10, 10, 1, 10, 10, 10, 10, 10, 10, 31, 1, 
			1, 1, 1, 1, 32, 1, 33, 1, 34, 1, 1, 35, 36, 1, 37, 1, 
			38, 39, 1, 40, 41, 1, 42, 1, 1, 1, 1, 1, 43, 1, 44, 1, 
			1, 1, 45, 46, 47, 48, 49, 50, 51, 52, 1, 1, 53, 54, 55, 56, 
			57, 58, 59, 1, 1, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 
			71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 1, 82, 83, 84, 85, 
			86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 
			102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 79, 112, 62, 22, 113, 23, 
			114, 8, 115, 116, 117, 9, 118, 119, 120, 121, 122, 52, 123, 124, 125, 126, 
			127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 25, 138, 139, 140, 141, 
			142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 
			158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 
			174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 
			190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 
			206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 
			222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 37, 236, 
			237, 238, 239, 64, 70, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 
			251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 
			267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 
			283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 
			299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 
			315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 
			331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 
			347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 
			363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 
			379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 
			395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 
			411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 
			427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 
			443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 
			459, 460, 10, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 180, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 178, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 415, 610, 610, 610, 610, 610, 416, 417, 610, 610, 610, 610, 418, -1, 419, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 420, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 22, 23, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 9, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 9, 9, -1, 9, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 259, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 50, 257, 257, 257, -1 },
			{ -1, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 263, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 265, -1, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 51, 261, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, 27, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 206, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 283, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, 49, -1, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, 49, -1, 49, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 597, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 67, -1, -1, -1, 67, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, 67, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, 68, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 294, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 314, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 323, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, 323, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, 323, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 601, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 526, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 618, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 355, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 357, 359, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 143, 190, 1 },
			{ -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 369, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1, 148, 148, 148, -1 },
			{ 1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 371, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 372, 359, 191, 191, 191, 191, 191, 191, 191, 151, 191, 191, 191, 191, 1 },
			{ 1, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 153, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 374, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 375, 359, 192, 192, 192, 192, 193, -1, 192, 192, 192, 192, 192, 192, 179 },
			{ -1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, -1, 155, 155, 155, 155, 155, 155, 155, 155, -1, -1, 155, 155, -1, -1, -1, -1, -1, 155, -1, -1, -1, 155, 155, 155, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, 155, -1, 155, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, -1, 382, 382, 382, 382, 382, 382, 382, 382, -1, -1, 382, 382, -1, -1, -1, -1, -1, 382, -1, -1, -1, 382, 382, 382, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 159, 159, 382, 382, -1, 382, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 162, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, 172, -1, 172, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, -1, 174, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1 },
			{ -1, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, -1, 377, 377, 377, 377, 377, 377, 377, 377, -1, -1, 377, -1, -1, -1, -1, -1, -1, 377, -1, -1, -1, 377, 377, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 18, 610, 421, 610, 610, 422, 610, 610, 610, -1, 544, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 220, 253, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, 9, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 9, 9, -1, 9, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185, 185, -1, 185, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, 188, 188, 188, 188, 363, -1, 188, 188, -1, -1, -1, -1, -1, 188, -1, -1, -1, 188, 188, 188, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 146, 188, 188, -1, 188, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 355, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 361, -1, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, -1, 190, -1 },
			{ -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 371, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 373, -1, 191, 191, 191, 191, 191, 191, 191, -1, 191, 191, 191, 191, -1 },
			{ -1, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 153, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 374, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 376, -1, 192, 192, 192, 192, 193, -1, 192, 192, 192, 192, 192, 192, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 153, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 387, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 388, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, -1, 174, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 201, -1, -1, -1, 201, 201, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, 201, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, 201, -1, 201, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 424, 610, 224, 610, 610, 610, 610, 610, 610, 19, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 20, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, 223, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, 27, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 236, 610, 610, 21, 546, 610, 610, -1, 610, 610, 610, 610, 573, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 6, 393, 182, 405, 204, 408, 410, 412, 543, 209, 572, 591, 602, 608, 7, 610, 212, 610, 612, 215, 610, 613, 614, 8, 183, 610, 9, 10, 205, 11, 210, 213, 414, 216, 7, 219, 610, 615, 610, 222, 225, 228, 231, 234, 237, 240, 243, 246, 249, 219, 12, 252, 13, 219, 184, 9, 7, 9, 219, 14, 15, 16, 17, 219, 1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 550, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, 15, -1, 17, -1, -1 },
			{ -1, -1, -1, 226, -1, 229, 232, 396, -1, -1, 235, 238, 241, -1, -1, -1, -1, 244, -1, -1, 247, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 250, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 395, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 52, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 267, -1, -1, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 267, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185, 185, -1, 185, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 53, 610, -1, 610, 444, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 54, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 397, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 55, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 269, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 56, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 271, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 57, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 275, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 58, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, 42, 16, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 398, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 61, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 277, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 69, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 406, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 70, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 279, -1, -1, -1, -1, -1, -1, 399, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 71, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 72, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 73, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 74, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 75, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, 257, -1 },
			{ -1, 610, 610, 610, 610, 610, 77, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 78, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, -1, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, -1, 261, 261, 261, 261, 261, 261, 261, -1 },
			{ -1, 610, 610, 610, 79, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, -1, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, 261, -1 },
			{ -1, 610, 610, 610, 80, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 81, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 287, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 82, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 400, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 83, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 84, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 401, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 85, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 86, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 87, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, -1, 299, 299, 299, 299, 299, 299, 299, 299, -1, -1, 299, -1, -1, -1, -1, -1, -1, 299, -1, 281, -1, 299, 299, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 301, -1, 403, -1, -1 },
			{ -1, 610, 610, 610, 610, 88, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 89, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 90, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 407, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 91, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 404, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 92, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 95, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 96, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 97, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 409, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 98, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 299, 94, 299, 299, 299, 299, 299, 299, 299, 299, -1, -1, 299, 299, -1, -1, -1, -1, -1, 299, -1, -1, -1, 299, 299, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 299, 299, 189, 299, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 99, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, -1, 319, 319, 319, 319, 319, 319, 319, 319, -1, -1, 319, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, 319, 319, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 100, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 101, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 104, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 105, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 106, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 107, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 108, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 109, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 110, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, 319, -1, 319, 319, 319, 319, 319, 319, 319, 319, -1, -1, 319, 319, -1, -1, -1, -1, -1, 319, -1, -1, -1, 319, 319, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, 319, -1, 319, -1, -1, 343, -1, -1, -1, -1 },
			{ -1, 111, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, -1, 321, 321, 321, 321, 321, 321, 321, 321, -1, -1, 321, 321, -1, -1, -1, -1, -1, 321, -1, -1, -1, 321, 321, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, 321, -1, 321, -1, -1, -1, -1, 343, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 114, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 411, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 115, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 413, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 116, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 117, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 118, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 119, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 122, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 335, 113, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 123, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 124, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 125, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 126, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 127, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, 120, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 128, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 129, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 130, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 131, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 135, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 132, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, -1, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, -1, 190, 190, 190, 190, 190, 190, 190, -1 },
			{ -1, 610, 610, 610, 610, 610, 133, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 144, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 134, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, 188, 188, 188, 188, -1, -1, 188, -1, -1, -1, -1, -1, -1, 188, -1, -1, -1, 188, 188, 188, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 145, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 136, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, -1, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, -1 },
			{ -1, 137, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 138, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1, 147, 147, 147, 147, 147, 147, 147, 147, -1, -1, 147, -1, -1, -1, -1, -1, -1, 147, -1, -1, -1, 147, 147, 147, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 147 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 139, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 369, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 149, 148, 148, 148, 1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 140, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 141, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1, 191, 191, 191, 191, 191, 191, 191, -1 },
			{ -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 144, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1 },
			{ -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1 },
			{ -1, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, -1, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, -1, 192, 192, 192, 192, 192, 192, 192, -1 },
			{ -1, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 144, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, -1 },
			{ -1, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, -1, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, -1 },
			{ -1, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 377, 154, 377, 377, 377, 377, 377, 377, 377, 377, -1, -1, 377, 377, -1, -1, -1, -1, -1, 377, -1, -1, -1, 377, 377, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 377, 377, 194, 377, -1, -1, -1, -1, -1, 378, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1 },
			{ 1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 7, 155, 155, 155, 155, 155, 155, 155, 155, 156, 195, 155, 195, 195, 195, 195, 195, 195, 155, 195, 7, 195, 155, 155, 155, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 7, 195, 195, 195, 195, 195, 195, 195, 1 },
			{ 1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 196, 158, 158, 158, 158, 158, 158, 158, 158, 196, 196, 158, 196, 196, 196, 196, 196, 196, 158, 196, 196, 196, 158, 158, 158, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 1 },
			{ 1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 161, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 1 },
			{ 1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 164, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 1 },
			{ 1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 167, 166, 166, 166, 166, 166, 166, 166, 166, 166, 168, 166, 166, 166, 166, 166, 166, 198, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 198, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 197, 166, 166, 166, 166, 166, 166, 166, 1 },
			{ 1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 170, 610, 610, 610, 610, 610, 610, 610, 610, 171, 171, 610, 172, 10, 171, 170, 171, 171, 610, 171, 170, 171, 610, 610, 610, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 199, 171, 171, 200, 172, 170, 172, 173, 171, 170, 170, 171, 171, 1 },
			{ 1, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 1 },
			{ 1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 394 },
			{ -1, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 177, 391, 391, 391, 391, 391, 391, 391, 391, -1, -1, 391, 391, -1, -1, -1, -1, -1, 391, -1, -1, -1, 391, 391, 391, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 391, 391, 202, 391, -1, -1, -1, -1, -1, 392, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 202, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 218, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, -1, 391, 391, 391, 391, 391, 391, 391, 391, -1, -1, 391, -1, -1, -1, -1, -1, -1, 391, -1, -1, -1, 391, 391, 391, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 273, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 285, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 402, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, 321, -1, 321, 321, 321, 321, 321, 321, 321, 321, -1, -1, 321, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, 321, 321, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 221, 610, 610, -1, 610, 610, 423, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 425, 610, 610, 610, 547, 610, 610, 227, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 545, 610, 610, 230, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 233, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 426, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 239, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 242, 575, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 439, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 245, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 248, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 440, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 251, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 592, 610, 610, 610, 610, 441, 610, 442, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 443, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 445, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 548, 610, 610, 576, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 446, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 603, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 450, 610, 610, 610, 610, -1, 610, 451, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 452, 610, 610, 610, 610, 610, 610, 254, 610, 610, 620, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 553, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 577, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 453, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 554, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 454, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 256, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 258, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 551, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 593, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 458, 610, 610, 610, 610, 610, 610, 605, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 459, 460, 461, 610, 462, 604, 610, 610, 610, 610, 556, -1, 463, 610, 557, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 260, 610, 558, 465, 610, 610, 610, 610, 466, 610, 610, 610, -1, 610, 610, 610, 467, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 262, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 578, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 468, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 264, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 266, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 268, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 270, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 469, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 272, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 580, 610, 610, 610, 610, 610, 610, 274, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 276, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 278, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 280, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 473, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 282, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 284, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 286, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 288, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 290, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 609, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 611, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 476, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 477, 610, 610, 610, 559, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 478, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 560, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 480, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 292, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 482, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 564, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 562, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 485, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 585, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 489, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 296, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 298, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 300, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 302, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 304, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 493, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 494, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 566, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 495, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 306, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 498, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 568, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 500, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 308, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 502, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 310, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 312, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 316, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 569, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 505, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 318, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 320, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 322, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 507, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 587, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 509, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 510, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 589, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 324, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 512, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 513, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 514, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 326, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 328, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 330, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 332, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 334, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 336, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 522, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 523, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 338, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 340, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 342, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 527, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 528, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 344, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 346, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 348, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 621, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 529, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 350, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 530, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 570, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 352, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 354, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 531, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 356, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 358, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 532, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 360, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 534, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 537, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 538, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 362, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 364, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 366, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 539, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 540, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 368, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 541, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 542, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 370, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 574, 610, 610, 610, 427, -1, 610, 428, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 552, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 448, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 455, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 447, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 595, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 456, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 457, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 474, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 582, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 471, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 596, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 483, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 583, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 561, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 481, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 619, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 496, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 497, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 501, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 588, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 499, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 504, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 567, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 520, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 511, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 516, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 533, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 535, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 429, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 430, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 594, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 449, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 464, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 581, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 472, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 484, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 584, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 565, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 487, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 616, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 586, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 506, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 503, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 508, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 521, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 517, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 524, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 536, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 431, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 555, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 475, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 579, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 486, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 491, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 488, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 563, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 515, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 518, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 525, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 432, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 470, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 479, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 598, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 490, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 519, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 433, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 492, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 617, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 549, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 434, 610, 610, 610, 435, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 436, 610, 610, 610, 610, 437, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 438, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 599, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 600, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 571, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 607, 610, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 610, 606, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 610, 610, 610, 610, 610, 610, 610, 610, 610, 590, 610, 610, 610, 610, -1, 610, 610, 610, 610, 610, 610, 610, 610, -1, -1, 610, 610, -1, -1, -1, -1, -1, 610, -1, -1, -1, 610, 610, 610, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 610, 610, -1, 610, -1, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  214,
			  142,
			  367,
			  150,
			  152,
			  379,
			  380,
			  381,
			  383,
			  384,
			  385,
			  386,
			  389,
			  390
		};
		
		#endregion
		
		public Tokens NextToken()
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 622);
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
		} // end of NextToken
	}
}

