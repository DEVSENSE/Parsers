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
					// #line 616
					{ 
						return Tokens.T_INLINE_HTML; 
					}
					break;
					
				case 3:
					// #line 633
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
					// #line 808
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 7:
					// #line 680
					{
						return ProcessLabel();
					}
					break;
					
				case 8:
					// #line 235
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 9:
					// #line 519
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 10:
					// #line 564
					{
						return ProcessDecimalNumber();
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
						return Tokens.T_LBRACE;
					}
					break;
					
				case 13:
					// #line 536
					{
						RESET_DOC_COMMENT();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return Tokens.T_RBRACE;
					}
					break;
					
				case 14:
					// #line 753
					{
						BEGIN(LexicalStates.ST_BACKQUOTE); 
						return Tokens.T_BACKQUOTE; 
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
					// #line 720
					{
						BEGIN(LexicalStates.ST_DOUBLE_QUOTES);
						return (Tokens.T_DOUBLE_QUOTES);
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
						return (Tokens.T_PAAMAYIM_NEKUDOTAYIM);
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
					// #line 507
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 52:
					// #line 98
					{
						return (Tokens.T_TRY);
					}
					break;
					
				case 53:
					// #line 73
					{
						return (Tokens.T_EXIT);
					}
					break;
					
				case 54:
					// #line 142
					{
						return (Tokens.T_FOR);
					}
					break;
					
				case 55:
					// #line 335
					{
						return (Tokens.T_USE);
					}
					break;
					
				case 56:
					// #line 271
					{
						return (Tokens.T_NEW);
					}
					break;
					
				case 57:
					// #line 503
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 58:
					// #line 475
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 59:
					// #line 263
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 60:
					// #line 279
					{
						return (Tokens.T_VAR);
					}
					break;
					
				case 61:
					// #line 411
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 62:
					// #line 415
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 63:
					// #line 427
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 64:
					// #line 471
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 65:
					// #line 455
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 66:
					// #line 568
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 67:
					// #line 560
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 68:
					// #line 69
					{ 
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 69:
					// #line 202
					{
						return (Tokens.T_ECHO);
					}
					break;
					
				case 70:
					// #line 126
					{
						return (Tokens.T_ELSE);
					}
					break;
					
				case 71:
					// #line 311
					{
						return (Tokens.T_EVAL);
					}
					break;
					
				case 72:
					// #line 182
					{
						return (Tokens.T_CASE);
					}
					break;
					
				case 73:
					// #line 391
					{
						return (Tokens.T_LIST);
					}
					break;
					
				case 74:
					// #line 198
					{
						return (Tokens.T_GOTO);
					}
					break;
					
				case 75:
					// #line 696
					{ BEGIN(LexicalStates.ST_DOC_COMMENT); yymore(); RESET_DOC_COMMENT(); break; }
					break;
					
				case 76:
					// #line 122
					{
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 77:
					// #line 351
					{
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 78:
					// #line 347
					{
						return (Tokens.T_ISSET);
					}
					break;
					
				case 79:
					// #line 218
					{
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 80:
					// #line 110
					{
						return (Tokens.T_THROW);
					}
					break;
					
				case 81:
					// #line 367
					{
						return (Tokens.T_FINAL);
					}
					break;
					
				case 82:
					// #line 383
					{
						return (Tokens.T_UNSET);
					}
					break;
					
				case 83:
					// #line 81
					{
						return (Tokens.T_CONST);
					}
					break;
					
				case 84:
					// #line 275
					{
						return (Tokens.T_CLONE);
					}
					break;
					
				case 85:
					// #line 210
					{
						return (Tokens.T_CLASS);
					}
					break;
					
				case 86:
					// #line 102
					{
						return (Tokens.T_CATCH);
					}
					break;
					
				case 87:
					// #line 94
					{
						return (Tokens.T_YIELD);
					}
					break;
					
				case 88:
					// #line 395
					{
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 89:
					// #line 130
					{
						return (Tokens.T_WHILE);
					}
					break;
					
				case 90:
					// #line 190
					{
						return (Tokens.T_BREAK);
					}
					break;
					
				case 91:
					// #line 206
					{
						return (Tokens.T_PRINT);
					}
					break;
					
				case 92:
					// #line 283
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 93:
					// #line 726
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
					    hereDocLabel = GetTokenSubstring(s, length);
					    return (Tokens.T_START_HEREDOC);
					}
					break;
					
				case 94:
					// #line 146
					{
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 95:
					// #line 118
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 96:
					// #line 359
					{
						return (Tokens.T_STATIC);
					}
					break;
					
				case 97:
					// #line 174
					{
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 98:
					// #line 85
					{
						return (Tokens.T_RETURN);
					}
					break;
					
				case 99:
					// #line 343
					{
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 100:
					// #line 379
					{
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 101:
					// #line 287
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 102:
					// #line 303
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 103:
					// #line 222
					{
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 104:
					// #line 315
					{
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 105:
					// #line 186
					{
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 106:
					// #line 158
					{
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 107:
					// #line 106
					{
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 108:
					// #line 150
					{
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 109:
					// #line 323
					{
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 110:
					// #line 371
					{
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 111:
					// #line 307
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 112:
					// #line 295
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 113:
					// #line 608
					{
						return (Tokens.T_DIR);
					}
					break;
					
				case 114:
					// #line 134
					{
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 115:
					// #line 77
					{
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 116:
					// #line 194
					{
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 117:
					// #line 399
					{
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 118:
					// #line 363
					{
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 119:
					// #line 299
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 120:
					// #line 291
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 121:
					// #line 604
					{
						return (Tokens.T_FILE);
					}
					break;
					
				case 122:
					// #line 600
					{
						return (Tokens.T_LINE);
					}
					break;
					
				case 123:
					// #line 178
					{
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 124:
					// #line 214
					{
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 125:
					// #line 339
					{
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 126:
					// #line 331
					{
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 127:
					// #line 375
					{
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 128:
					// #line 588
					{
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 129:
					// #line 584
					{
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 130:
					// #line 162
					{
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 131:
					// #line 154
					{
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 132:
					// #line 166
					{
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 133:
					// #line 226
					{
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 134:
					// #line 89
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 135:
					// #line 596
					{
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 136:
					// #line 319
					{
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 137:
					// #line 327
					{
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 138:
					// #line 592
					{
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 139:
					// #line 612
					{
						return (Tokens.T_NS_C);
					}
					break;
					
				case 140:
					// #line 355
					{
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 141:
					// #line 792
					{
					    tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this.sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 142:
					// #line 774
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 143:
					// #line 765
					{
						//Z_LVAL_P(zendlval) = (zend_long) '{';
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 144:
					// #line 530
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 145:
					// #line 653
					{
						yyless(TokenLength - 1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 146:
					// #line 647
					{
						yyless(TokenLength - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 147:
					// #line 716
					{ yymore(); break; }
					break;
					
				case 148:
					// #line 717
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 149:
					// #line 797
					{
					    tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this.sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 150:
					// #line 779
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 151:
					// #line 802
					{
					    tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this.sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 152:
					// #line 784
					{
						BEGIN(LexicalStates.ST_END_HEREDOC);
						tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 153:
					// #line 249
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 154:
					// #line 244
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 155:
					// #line 240
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 156:
					// #line 553
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 157:
					// #line 544
					{
						yyless(TokenLength - 1);
						tokenSemantics.Object = GetTokenString();
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 158:
					// #line 697
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 699
					{ yymore(); break; }
					break;
					
				case 160:
					// #line 698
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); SET_DOC_COMMENT(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 161:
					// #line 692
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 694
					{ yymore(); break; }
					break;
					
				case 163:
					// #line 693
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return Tokens.T_COMMENT; }
					break;
					
				case 164:
					// #line 815
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 816
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return Tokens.T_COMMENT; }
					break;
					
				case 166:
					// #line 814
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 818
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
					
				case 168:
					// #line 673
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 169:
					// #line 668
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 170:
					// #line 572
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 171:
					// #line 663
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 172:
					// #line 576
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 173:
					// #line 759
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 174:
					// #line 790
					{ yymore(); break; }
					break;
					
				case 176: goto case 1;
				case 178: goto case 5;
				case 179: goto case 7;
				case 180: goto case 9;
				case 181: goto case 10;
				case 182: goto case 27;
				case 183: goto case 30;
				case 184: goto case 36;
				case 185: goto case 49;
				case 186: goto case 93;
				case 187: goto case 141;
				case 188: goto case 149;
				case 189: goto case 151;
				case 190: goto case 152;
				case 191: goto case 153;
				case 192: goto case 156;
				case 193: goto case 165;
				case 194: goto case 166;
				case 195: goto case 169;
				case 196: goto case 170;
				case 197: goto case 172;
				case 200: goto case 7;
				case 201: goto case 9;
				case 202: goto case 30;
				case 203: goto case 172;
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
				case 245: goto case 9;
				case 247: goto case 7;
				case 248: goto case 9;
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
				case 362: goto case 7;
				case 364: goto case 7;
				case 366: goto case 7;
				case 384: goto case 7;
				case 395: goto case 7;
				case 398: goto case 7;
				case 400: goto case 7;
				case 402: goto case 7;
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
				case 594: goto case 7;
				case 595: goto case 7;
				case 596: goto case 7;
				case 597: goto case 7;
				case 598: goto case 7;
				case 599: goto case 7;
				case 600: goto case 7;
				case 601: goto case 7;
				case 602: goto case 7;
				case 603: goto case 7;
				case 604: goto case 7;
				case 605: goto case 7;
				case 606: goto case 7;
				case 607: goto case 7;
				case 608: goto case 7;
				case 609: goto case 7;
				case 610: goto case 7;
				case 611: goto case 7;
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
			AcceptConditions.AcceptOnStart, // 152
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
			AcceptConditions.Accept, // 167
			AcceptConditions.Accept, // 168
			AcceptConditions.Accept, // 169
			AcceptConditions.Accept, // 170
			AcceptConditions.Accept, // 171
			AcceptConditions.Accept, // 172
			AcceptConditions.Accept, // 173
			AcceptConditions.Accept, // 174
			AcceptConditions.NotAccept, // 175
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
			AcceptConditions.AcceptOnStart, // 190
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
			AcceptConditions.Accept, // 201
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
			AcceptConditions.Accept, // 245
			AcceptConditions.NotAccept, // 246
			AcceptConditions.Accept, // 247
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
			AcceptConditions.Accept, // 362
			AcceptConditions.NotAccept, // 363
			AcceptConditions.Accept, // 364
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
			AcceptConditions.NotAccept, // 377
			AcceptConditions.NotAccept, // 378
			AcceptConditions.NotAccept, // 379
			AcceptConditions.NotAccept, // 380
			AcceptConditions.NotAccept, // 381
			AcceptConditions.NotAccept, // 382
			AcceptConditions.NotAccept, // 383
			AcceptConditions.Accept, // 384
			AcceptConditions.NotAccept, // 385
			AcceptConditions.NotAccept, // 386
			AcceptConditions.NotAccept, // 387
			AcceptConditions.NotAccept, // 388
			AcceptConditions.NotAccept, // 389
			AcceptConditions.NotAccept, // 390
			AcceptConditions.NotAccept, // 391
			AcceptConditions.NotAccept, // 392
			AcceptConditions.NotAccept, // 393
			AcceptConditions.NotAccept, // 394
			AcceptConditions.Accept, // 395
			AcceptConditions.NotAccept, // 396
			AcceptConditions.NotAccept, // 397
			AcceptConditions.Accept, // 398
			AcceptConditions.NotAccept, // 399
			AcceptConditions.Accept, // 400
			AcceptConditions.NotAccept, // 401
			AcceptConditions.Accept, // 402
			AcceptConditions.NotAccept, // 403
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
		};
		
		private static int[] colMap = new int[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 35, 15, 0, 0, 57, 0, 0, 
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			35, 42, 62, 61, 52, 46, 47, 60, 34, 36, 44, 41, 50, 24, 31, 45, 
			55, 56, 27, 27, 27, 27, 27, 27, 27, 27, 29, 63, 43, 40, 25, 32, 
			50, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 28, 54, 30, 58, 49, 38, 
			59, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 28, 51, 48, 53, 50, 0, 
			64, 26
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 2, 3, 2, 2, 2, 4, 5, 6, 7, 2, 2, 2, 2, 8, 
			2, 2, 9, 9, 9, 9, 2, 2, 2, 10, 2, 11, 2, 2, 12, 2, 
			2, 13, 2, 2, 14, 15, 16, 2, 17, 2, 18, 2, 2, 2, 2, 2, 
			2, 19, 2, 9, 9, 9, 20, 9, 9, 9, 2, 2, 9, 2, 2, 2, 
			2, 2, 21, 22, 9, 9, 23, 9, 9, 9, 9, 24, 9, 9, 9, 9, 
			9, 25, 9, 9, 9, 9, 9, 26, 9, 9, 9, 9, 2, 2, 27, 9, 
			9, 9, 9, 9, 9, 2, 2, 9, 28, 9, 9, 9, 9, 29, 9, 2, 
			2, 9, 9, 9, 9, 9, 9, 2, 2, 9, 9, 9, 9, 9, 9, 9, 
			9, 9, 9, 9, 9, 9, 2, 9, 9, 9, 9, 9, 9, 30, 2, 2, 
			2, 2, 2, 31, 2, 32, 2, 33, 2, 2, 34, 2, 2, 2, 35, 36, 
			2, 37, 38, 2, 39, 2, 2, 2, 2, 2, 40, 2, 41, 2, 2, 42, 
			43, 44, 45, 46, 47, 48, 49, 2, 2, 50, 51, 52, 53, 54, 55, 56, 
			57, 58, 59, 60, 61, 62, 63, 57, 64, 65, 66, 67, 68, 69, 70, 71, 
			72, 73, 74, 75, 76, 77, 78, 2, 79, 80, 81, 82, 83, 84, 85, 86, 
			87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 
			103, 104, 105, 106, 107, 108, 76, 109, 60, 21, 110, 22, 111, 8, 112, 113, 
			114, 49, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 
			129, 24, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 
			144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 
			176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 
			192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 
			208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 
			224, 225, 226, 227, 228, 229, 230, 57, 231, 232, 233, 234, 62, 67, 235, 236, 
			237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 
			253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 
			269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 
			285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 
			301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 
			317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 
			333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 
			349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 
			365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 
			381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 
			397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 
			413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 
			429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 
			445, 446, 447, 448, 449, 450, 451, 452, 9, 453, 454, 455, 456, 457, 458, 459, 
			460, 461, 462, 463
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 176, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 175, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 405, 600, 600, 600, 600, 600, 406, 407, 600, 600, 600, 600, 408, -1, 409, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 410, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 22, 23, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 216, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, -1, 253, 253, 253, 255, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 50, 253, 253, 253, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 58, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 216, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 202, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, 271, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 273, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, 49, 49, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, 49, -1, -1, -1, -1, -1, -1, -1, 49 },
			{ -1, 587, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 66, -1, -1, -1, 66, 66, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, 66, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, 66, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 290, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 310, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 313, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, 313, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, 313, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 591, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 516, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 608, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 2, 187, 187, 187, 345, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 347, 349, 187, 187, 187, 187, 187, 187, 187, 187, 187, 142, 187, 2 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1, 147, 147, 147, 359, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1, 147, 147, 147, -1 },
			{ 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 2, 188, 188, 188, 361, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 363, 349, 188, 188, 188, 188, 188, 188, 150, 188, 188, 188, 188, 2 },
			{ 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 2, 189, 189, 189, 367, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 368, 349, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 177 },
			{ -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1, 154, 154, 154, 154, 154, 154, 154, 154, -1, -1, 154, 154, 154, -1, -1, -1, -1, 154, -1, -1, -1, 154, 154, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 154, 154, -1, -1, -1, -1, -1, -1, -1, 154 },
			{ 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 160, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 163, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, -1, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, 170, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, 172, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, 1, 1, 1, 1, 1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1 },
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1 },
			{ -1, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, -1, 370, 370, 370, 370, 370, 370, 370, 370, -1, -1, 370, -1, 370, -1, -1, -1, -1, 370, -1, -1, -1, 370, 370, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 370 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 18, 600, 411, 600, 600, 412, 600, 600, 600, -1, 534, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 216, 249, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 251, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, 182, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, -1, 185, 185, 185, 185, 185, 185, 185, 185, 353, -1, 185, 185, 185, -1, -1, -1, -1, 185, -1, -1, -1, 185, 185, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 145, 185, 185, -1, -1, -1, -1, -1, -1, -1, 185 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 345, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 351, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, -1 },
			{ 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 361, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 365, -1, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, -1 },
			{ 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 367, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 369, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 152, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, -1, 375, 375, 375, 375, 375, 375, 375, 375, -1, -1, 375, 375, 375, -1, -1, -1, -1, 375, -1, -1, -1, 375, 375, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, 157, 375, 375, -1, -1, -1, -1, -1, -1, -1, 375 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, -1, 49, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49 },
			{ -1, -1, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 381, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, 172, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 197, -1, -1, -1, 197, 197, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, 197, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 414, 600, 220, 600, 600, 600, 600, 600, 600, 19, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, 203, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 20, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 232, 600, 600, 21, 536, 600, 600, -1, 600, 600, 600, 600, 563, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 6, 7, 384, 179, 395, 200, 398, 400, 402, 533, 205, 562, 581, 592, 598, 8, 600, 208, 600, 602, 211, 600, 603, 604, 9, 180, 600, 10, 600, 201, 11, 206, 209, 404, 212, 8, 215, 600, 605, 600, 218, 221, 224, 227, 230, 233, 236, 239, 242, 245, 215, 12, 248, 13, 215, 181, 10, 8, 215, 14, 15, 16, 17, 215, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 540, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, 213, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, 15, -1, 17, -1, 600 },
			{ -1, -1, -1, 222, -1, 225, 228, 386, -1, -1, 231, 234, 237, -1, -1, -1, -1, 240, -1, -1, 243, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 51, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, -1, -1, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, 182, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 52, 600, -1, 600, 434, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 53, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 387, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 54, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 259, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 55, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 184, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 261, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 56, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 265, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 57, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, 42, 16, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 388, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 60, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 267, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 68, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 396, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 69, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 269, -1, -1, -1, -1, -1, -1, 389, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 70, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 71, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 72, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 73, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 74, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, -1, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, -1 },
			{ -1, 600, 600, 600, 600, 600, 76, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 77, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, 277, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 78, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 390, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 79, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 279, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 80, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 391, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 81, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 283, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 82, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 287, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 83, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, -1, 289, 289, 289, 289, 289, 289, 289, 289, -1, -1, 289, -1, 289, -1, -1, -1, -1, 289, -1, 271, -1, 289, 289, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, 393, -1, 289 },
			{ -1, 84, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 85, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, 92, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 86, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 397, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 87, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 394, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 88, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, 301, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 89, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 90, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 91, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 399, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 94, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 93, 289, 289, 289, 289, 289, 289, 289, 289, -1, -1, 289, 289, 289, -1, -1, -1, -1, 289, -1, -1, -1, 289, 289, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 289, 289, 186, -1, -1, -1, -1, -1, -1, 289 },
			{ -1, 600, 600, 600, 600, 600, 95, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, -1, 309, 309, 309, 309, 309, 309, 309, 309, -1, -1, 309, -1, 309, -1, -1, -1, -1, 309, -1, -1, -1, 309, 309, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 96, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 97, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, 92, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 98, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 99, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 100, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 103, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 303, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 104, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 105, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 106, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, -1, 309, 309, 309, 309, 309, 309, 309, 309, -1, -1, 309, 309, 309, -1, -1, -1, -1, 309, -1, -1, -1, 309, 309, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309, 309, -1, -1, -1, 333, -1, -1, -1, 309 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 107, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, -1, 311, 311, 311, 311, 311, 311, 311, 311, -1, -1, 311, 311, 311, -1, -1, -1, -1, 311, -1, -1, -1, 311, 311, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 311, 311, -1, -1, -1, -1, -1, 333, -1, 311 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 108, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, 401, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 109, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 403, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 110, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 113, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, 111, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 114, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 115, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 116, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 117, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 118, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 121, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 122, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 123, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 335, 119, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 124, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, 120, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 125, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 126, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 127, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 134, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 128, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 129, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 143, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1 },
			{ -1, 130, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, -1, 185, 185, 185, 185, 185, 185, 185, 185, -1, -1, 185, -1, 185, -1, -1, -1, -1, 185, -1, -1, -1, 185, 185, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 144, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 131, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1 },
			{ -1, 600, 600, 600, 600, 600, 132, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 133, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, -1, 146, 146, 146, 146, 146, 146, 146, 146, -1, -1, 146, -1, 146, -1, -1, -1, -1, 146, -1, -1, -1, 146, 146, 146, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 146 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 135, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 2, 147, 147, 147, 359, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 148, 147, 147, 147, 2 },
			{ -1, 136, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1 },
			{ -1, 137, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, 188, 188, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 138, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 143, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 139, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 140, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 189, 189, 189, -1 },
			{ 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 143, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1 },
			{ 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1 },
			{ -1, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 370, 152, 370, 370, 370, 370, 370, 370, 370, 370, -1, -1, 370, 370, 370, -1, -1, -1, -1, 370, -1, -1, -1, 370, 370, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 370, 370, 190, -1, -1, -1, -1, -1, 371, 370 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 152, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 190, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2 },
			{ 153, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 8, 154, 154, 154, 154, 154, 154, 154, 154, 191, 153, 154, 153, 154, 153, 153, 153, 153, 154, 153, 8, 153, 154, 154, 154, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 8, 153, 153, 153, 153, 153, 153, 154 },
			{ 156, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 156, 192, 192, 192, 192, 192, 192, 192, 192, 156, 156, 199, 156, 192, 156, 156, 156, 156, 192, 156, 156, 156, 192, 192, 192, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 199 },
			{ 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 2, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 159, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 2 },
			{ 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 2, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 162, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 2 },
			{ 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 165, 164, 164, 164, 164, 164, 164, 164, 164, 164, 166, 2, 164, 164, 164, 164, 164, 194, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 194, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 193, 164, 164, 164, 164, 164, 164, 2 },
			{ 6, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 168, 600, 600, 600, 600, 600, 600, 600, 600, 169, 169, 600, 170, 600, 169, 168, 169, 169, 600, 169, 168, 169, 600, 600, 600, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 195, 169, 169, 196, 170, 168, 171, 169, 168, 168, 169, 169, 600 },
			{ 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 2, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 2 },
			{ 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 2, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 177 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 214, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 271, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 275, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 392, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, -1, 311, 311, 311, 311, 311, 311, 311, 311, -1, -1, 311, -1, 311, -1, -1, -1, -1, 311, -1, -1, -1, 311, 311, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 311 },
			{ -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 217, 600, 600, -1, 600, 600, 413, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 285, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 415, 600, 600, 600, 537, 600, 600, 223, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 535, 600, 600, 226, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 229, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 416, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 235, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 238, 565, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 429, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 241, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 244, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 430, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 247, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 582, 600, 600, 600, 600, 431, 600, 432, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 433, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 435, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 538, 600, 600, 566, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 436, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 593, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 440, 600, 600, 600, 600, -1, 600, 441, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 442, 600, 600, 600, 600, 600, 600, 250, 600, 600, 610, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 543, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 567, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 443, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 544, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 444, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 252, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 254, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 541, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 583, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 448, 600, 600, 600, 600, 600, 600, 595, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 449, 450, 451, 600, 452, 594, 600, 600, 600, 600, 546, -1, 453, 600, 547, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 256, 600, 548, 455, 600, 600, 600, 600, 456, 600, 600, 600, -1, 600, 600, 600, 457, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 258, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 568, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 458, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 260, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 262, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 264, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 266, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 459, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 268, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 570, 600, 600, 600, 600, 600, 600, 270, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 272, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 274, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 276, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 463, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 278, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 280, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 282, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 284, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 286, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 599, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 601, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 466, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 467, 600, 600, 600, 549, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 468, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 550, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 470, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 288, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 472, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 554, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 552, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 475, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 575, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 479, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 292, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 294, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 296, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 298, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 300, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 483, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 484, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 556, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 485, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 302, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 488, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 558, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 490, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 304, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 492, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 306, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 308, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 312, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 559, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 495, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 314, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 316, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 318, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 497, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 577, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 499, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 500, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 579, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 320, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 502, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 503, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 504, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 322, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 324, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 326, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 328, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 330, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 332, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 512, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 513, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 334, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 336, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 338, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 517, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 518, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 340, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 342, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 344, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 611, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 519, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 346, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 520, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 560, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 348, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 350, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 521, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 352, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 354, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 522, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 356, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 524, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 527, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 528, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 358, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 360, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 362, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 529, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 530, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 364, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 531, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 532, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 366, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 564, 600, 600, 600, 417, -1, 600, 418, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 542, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 438, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 445, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 437, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 585, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 446, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 447, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 464, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 572, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 461, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 586, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 473, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 573, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 551, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 471, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 609, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 486, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 487, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 491, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 578, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 489, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 494, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 557, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 510, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 501, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 506, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 523, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 525, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 419, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 420, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 584, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 439, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 454, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 571, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 462, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 474, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 574, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 555, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 477, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 606, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 576, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 496, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 493, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 498, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 511, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 507, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 514, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 526, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 421, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 545, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 465, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 569, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 476, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 481, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 478, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 553, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 505, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 508, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 515, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 422, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 460, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 469, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 588, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 480, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 509, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 423, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 482, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 607, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 539, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 424, 600, 600, 600, 425, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 426, 600, 600, 600, 600, 427, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 428, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 589, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 590, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 561, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 597, 600, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 600, 596, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 },
			{ -1, 600, 600, 600, 600, 600, 600, 600, 600, 600, 580, 600, 600, 600, 600, -1, 600, 600, 600, 600, 600, 600, 600, 600, -1, -1, 600, 600, 600, -1, -1, -1, -1, 600, -1, -1, -1, 600, 600, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 600, 600, -1, -1, -1, -1, -1, -1, -1, 600 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  210,
			  141,
			  357,
			  149,
			  151,
			  372,
			  373,
			  374,
			  376,
			  377,
			  378,
			  379,
			  382,
			  383
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 612);
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

