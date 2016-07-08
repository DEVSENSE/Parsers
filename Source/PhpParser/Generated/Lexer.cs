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
					// #line 639
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 3:
					// #line 630
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
					// #line 617
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 5:
					// #line 623
					{
						//HANDLE_NEWLINE(yytext[yyleng-1]);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 6:
					// #line 804
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 7:
					// #line 676
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
					// #line 749
					{
						BEGIN(LexicalStates.ST_BACKQUOTE); 
						return Tokens.T_BACKQUOTE; 
					}
					break;
					
				case 15:
					// #line 704
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
					// #line 681
					{
						BEGIN(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 17:
					// #line 716
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
					// #line 697
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
					// #line 687
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
					// #line 655
					{
						return ProcessVariable();
					}
					break;
					
				case 50:
					// #line 702
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
					// #line 692
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
					// #line 722
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
					// #line 788
					{
					    tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this.sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 142:
					// #line 770
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 143:
					// #line 761
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
					// #line 649
					{
						yyless(TokenLength - 1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 146:
					// #line 643
					{
						yyless(TokenLength - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 147:
					// #line 712
					{ yymore(); break; }
					break;
					
				case 148:
					// #line 713
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 149:
					// #line 775
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 150:
					// #line 793
					{
					    tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this.sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 151:
					// #line 798
					{
					    tokenSemantics.Object = ProcessEscapedString(GetTokenString(), this.sourceUnit.Encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 152:
					// #line 780
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
					// #line 693
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 695
					{ yymore(); break; }
					break;
					
				case 160:
					// #line 694
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); SET_DOC_COMMENT(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 161:
					// #line 688
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 690
					{ yymore(); break; }
					break;
					
				case 163:
					// #line 689
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return Tokens.T_COMMENT; }
					break;
					
				case 164:
					// #line 811
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 812
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return Tokens.T_COMMENT; }
					break;
					
				case 166:
					// #line 810
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 814
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
					// #line 669
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 169:
					// #line 664
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
					// #line 659
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
					// #line 755
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 174:
					// #line 786
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
				case 188: goto case 152;
				case 189: goto case 153;
				case 190: goto case 156;
				case 191: goto case 165;
				case 192: goto case 166;
				case 193: goto case 169;
				case 194: goto case 170;
				case 195: goto case 172;
				case 198: goto case 7;
				case 199: goto case 9;
				case 200: goto case 30;
				case 201: goto case 141;
				case 202: goto case 172;
				case 204: goto case 7;
				case 205: goto case 9;
				case 206: goto case 141;
				case 208: goto case 7;
				case 209: goto case 9;
				case 210: goto case 141;
				case 212: goto case 7;
				case 213: goto case 9;
				case 214: goto case 141;
				case 216: goto case 7;
				case 217: goto case 9;
				case 219: goto case 7;
				case 220: goto case 9;
				case 222: goto case 7;
				case 223: goto case 9;
				case 225: goto case 7;
				case 226: goto case 9;
				case 228: goto case 7;
				case 229: goto case 9;
				case 231: goto case 7;
				case 232: goto case 9;
				case 234: goto case 7;
				case 235: goto case 9;
				case 237: goto case 7;
				case 238: goto case 9;
				case 240: goto case 7;
				case 241: goto case 9;
				case 243: goto case 7;
				case 244: goto case 9;
				case 246: goto case 7;
				case 247: goto case 9;
				case 249: goto case 7;
				case 250: goto case 9;
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
				case 368: goto case 7;
				case 389: goto case 7;
				case 400: goto case 7;
				case 403: goto case 7;
				case 405: goto case 7;
				case 407: goto case 7;
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
				case 612: goto case 7;
				case 613: goto case 7;
				case 614: goto case 7;
				case 615: goto case 7;
				case 616: goto case 7;
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
			AcceptConditions.AcceptOnStart, // 188
			AcceptConditions.Accept, // 189
			AcceptConditions.Accept, // 190
			AcceptConditions.Accept, // 191
			AcceptConditions.Accept, // 192
			AcceptConditions.Accept, // 193
			AcceptConditions.Accept, // 194
			AcceptConditions.Accept, // 195
			AcceptConditions.NotAccept, // 196
			AcceptConditions.Accept, // 197
			AcceptConditions.Accept, // 198
			AcceptConditions.Accept, // 199
			AcceptConditions.Accept, // 200
			AcceptConditions.Accept, // 201
			AcceptConditions.Accept, // 202
			AcceptConditions.NotAccept, // 203
			AcceptConditions.Accept, // 204
			AcceptConditions.Accept, // 205
			AcceptConditions.Accept, // 206
			AcceptConditions.NotAccept, // 207
			AcceptConditions.Accept, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.Accept, // 210
			AcceptConditions.NotAccept, // 211
			AcceptConditions.Accept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.Accept, // 214
			AcceptConditions.NotAccept, // 215
			AcceptConditions.Accept, // 216
			AcceptConditions.Accept, // 217
			AcceptConditions.NotAccept, // 218
			AcceptConditions.Accept, // 219
			AcceptConditions.Accept, // 220
			AcceptConditions.NotAccept, // 221
			AcceptConditions.Accept, // 222
			AcceptConditions.Accept, // 223
			AcceptConditions.NotAccept, // 224
			AcceptConditions.Accept, // 225
			AcceptConditions.Accept, // 226
			AcceptConditions.NotAccept, // 227
			AcceptConditions.Accept, // 228
			AcceptConditions.Accept, // 229
			AcceptConditions.NotAccept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.NotAccept, // 233
			AcceptConditions.Accept, // 234
			AcceptConditions.Accept, // 235
			AcceptConditions.NotAccept, // 236
			AcceptConditions.Accept, // 237
			AcceptConditions.Accept, // 238
			AcceptConditions.NotAccept, // 239
			AcceptConditions.Accept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.NotAccept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.Accept, // 244
			AcceptConditions.NotAccept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.NotAccept, // 248
			AcceptConditions.Accept, // 249
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
			AcceptConditions.Accept, // 368
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
			AcceptConditions.NotAccept, // 384
			AcceptConditions.NotAccept, // 385
			AcceptConditions.NotAccept, // 386
			AcceptConditions.NotAccept, // 387
			AcceptConditions.NotAccept, // 388
			AcceptConditions.Accept, // 389
			AcceptConditions.NotAccept, // 390
			AcceptConditions.NotAccept, // 391
			AcceptConditions.NotAccept, // 392
			AcceptConditions.NotAccept, // 393
			AcceptConditions.NotAccept, // 394
			AcceptConditions.NotAccept, // 395
			AcceptConditions.NotAccept, // 396
			AcceptConditions.NotAccept, // 397
			AcceptConditions.NotAccept, // 398
			AcceptConditions.NotAccept, // 399
			AcceptConditions.Accept, // 400
			AcceptConditions.NotAccept, // 401
			AcceptConditions.NotAccept, // 402
			AcceptConditions.Accept, // 403
			AcceptConditions.NotAccept, // 404
			AcceptConditions.Accept, // 405
			AcceptConditions.NotAccept, // 406
			AcceptConditions.Accept, // 407
			AcceptConditions.NotAccept, // 408
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
			AcceptConditions.Accept, // 612
			AcceptConditions.Accept, // 613
			AcceptConditions.Accept, // 614
			AcceptConditions.Accept, // 615
			AcceptConditions.Accept, // 616
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
			0, 1, 1, 2, 1, 1, 1, 3, 4, 5, 6, 1, 1, 1, 1, 7, 
			1, 1, 8, 8, 8, 8, 1, 1, 1, 9, 1, 10, 1, 1, 11, 1, 
			1, 12, 1, 1, 13, 14, 15, 1, 16, 1, 17, 1, 1, 1, 1, 1, 
			1, 18, 1, 8, 8, 8, 19, 8, 8, 8, 1, 1, 8, 1, 1, 1, 
			1, 1, 20, 21, 8, 8, 22, 8, 8, 8, 8, 23, 8, 8, 8, 8, 
			8, 24, 8, 8, 8, 8, 8, 25, 8, 8, 8, 8, 1, 1, 26, 8, 
			8, 8, 8, 8, 8, 1, 1, 8, 27, 8, 8, 8, 8, 28, 8, 1, 
			1, 8, 8, 8, 8, 8, 8, 1, 1, 8, 8, 8, 8, 8, 8, 8, 
			8, 8, 8, 8, 8, 8, 1, 8, 8, 8, 8, 8, 8, 29, 1, 1, 
			1, 1, 1, 30, 1, 31, 32, 33, 1, 1, 34, 1, 1, 1, 35, 36, 
			1, 37, 38, 1, 39, 1, 1, 1, 1, 1, 40, 1, 41, 1, 1, 42, 
			43, 44, 45, 46, 47, 48, 49, 1, 1, 50, 51, 31, 52, 53, 54, 55, 
			56, 57, 58, 59, 60, 54, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 
			71, 72, 73, 74, 75, 76, 77, 78, 79, 1, 80, 81, 82, 83, 84, 85, 
			86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 
			102, 103, 104, 105, 106, 76, 107, 108, 20, 109, 57, 21, 110, 7, 111, 112, 
			113, 49, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 
			128, 23, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 
			143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 
			159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 
			175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 
			191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 
			207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 
			223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 54, 235, 236, 237, 
			238, 59, 65, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 
			252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 
			268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 
			284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 
			300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 
			316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 
			332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 
			348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 
			364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 
			380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 
			396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 
			412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 
			428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 
			444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 8, 457, 458, 
			459, 460, 461, 462, 463, 464, 465, 466, 467
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 176, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 175, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 410, 605, 605, 605, 605, 605, 411, 412, 605, 605, 605, 605, 413, -1, 414, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 415, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 22, 23, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 215, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, -1, 253, 253, 253, 255, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 50, 253, 253, 253, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 58, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 215, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, 271, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 273, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, 49, 49, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, 49, -1, -1, -1, -1, -1, -1, -1, 49 },
			{ -1, 592, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 66, -1, -1, -1, 66, 66, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, 66, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, 66, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 292, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 312, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 313, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, 313, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, 313, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 596, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 521, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 613, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 2, 187, 187, 187, 345, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 347, 349, 187, 187, 187, 187, 187, 187, 187, 187, 187, 142, 187, 2 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1, 147, 147, 147, 359, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1, 147, 147, 147, -1 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 345, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 351, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, -1 },
			{ 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 367, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 369, -1, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, -1 },
			{ 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 373, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 374, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1 },
			{ -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1, 154, 154, 154, 154, 154, 154, 154, 154, -1, -1, 154, 154, 154, -1, -1, -1, -1, 154, -1, -1, -1, 154, 154, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 154, 154, -1, -1, -1, -1, -1, -1, -1, 154 },
			{ 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 160, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 163, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, -1, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, 170, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, 172, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, -1, 375, 375, 375, 375, 375, 375, 375, 375, -1, -1, 375, -1, 375, -1, -1, -1, -1, 375, -1, -1, -1, 375, 375, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 375 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 18, 605, 416, 605, 605, 417, 605, 605, 605, -1, 539, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 215, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 251, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, 182, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, -1, 185, 185, 185, 185, 185, 185, 185, 185, 353, -1, 185, 185, 185, -1, -1, -1, -1, 185, -1, -1, -1, 185, 185, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 145, 185, 185, -1, -1, -1, -1, -1, -1, -1, 185 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 152, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, -1, 380, 380, 380, 380, 380, 380, 380, 380, -1, -1, 380, 380, 380, -1, -1, -1, -1, 380, -1, -1, -1, 380, 380, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, 157, 380, 380, -1, -1, -1, -1, -1, -1, -1, 380 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, -1, 49, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49 },
			{ -1, -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 386, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, 172, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 195, -1, -1, -1, 195, 195, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, 195, -1, -1, 195, -1, -1, -1, -1, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, 195, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 419, 605, 222, 605, 605, 605, 605, 605, 605, 19, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 2, 206, 206, 206, 361, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 363, 349, 206, 206, 206, 206, 206, 206, 149, 206, 206, 150, 206, 2 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 202, 202, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 20, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, 218, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 361, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 365, -1, 206, 206, 206, 206, 206, 206, 187, 206, 206, 150, 206, -1 },
			{ 6, 7, 389, 179, 400, 198, 403, 405, 407, 538, 204, 567, 586, 597, 603, 8, 605, 208, 605, 607, 212, 605, 608, 609, 9, 180, 605, 10, 605, 199, 11, 205, 209, 409, 213, 8, 217, 605, 610, 605, 220, 223, 226, 229, 232, 235, 238, 241, 244, 247, 217, 12, 250, 13, 217, 181, 10, 8, 217, 14, 15, 16, 17, 217, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 234, 605, 605, 21, 541, 605, 605, -1, 605, 605, 605, 605, 568, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 2, 214, 214, 214, 370, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 371, 349, 214, 214, 214, 214, 214, 214, 214, 214, 214, 151, 214, 177 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 390, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 545, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, 211, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, 15, -1, 17, -1, 605 },
			{ -1, -1, -1, 221, -1, 224, 227, 391, -1, -1, 230, 233, 236, -1, -1, -1, -1, 239, -1, -1, 242, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, 214, 214, 214, 370, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 372, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 151, 214, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, -1, -1, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, 182, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 51, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 52, 605, -1, 605, 439, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 392, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 53, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 259, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 54, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 261, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 55, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 184, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 265, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 56, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 393, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 57, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, 42, 16, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 267, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 60, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 401, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 68, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 269, -1, -1, -1, -1, -1, -1, 394, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 69, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 70, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 71, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 72, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 73, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, -1, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 74, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 76, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, 277, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 77, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 395, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 78, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 279, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 79, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 396, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 80, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 283, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 81, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 287, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 82, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, -1, 289, 289, 289, 289, 289, 289, 289, 289, -1, -1, 289, -1, 289, -1, -1, -1, -1, 289, -1, 271, -1, 289, 289, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, 398, -1, 289 },
			{ -1, 605, 605, 605, 83, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 84, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, 92, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 85, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 402, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 86, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 399, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 87, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, 301, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 88, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 89, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 90, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 404, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 91, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 289, 93, 289, 289, 289, 289, 289, 289, 289, 289, -1, -1, 289, 289, 289, -1, -1, -1, -1, 289, -1, -1, -1, 289, 289, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 289, 289, 186, -1, -1, -1, -1, -1, -1, 289 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 94, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, -1, 309, 309, 309, 309, 309, 309, 309, 309, -1, -1, 309, -1, 309, -1, -1, -1, -1, 309, -1, -1, -1, 309, 309, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309 },
			{ -1, 605, 605, 605, 605, 605, 95, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 96, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, 92, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 97, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 98, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 99, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 100, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 303, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 103, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 104, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 105, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, 309, -1, 309, 309, 309, 309, 309, 309, 309, 309, -1, -1, 309, 309, 309, -1, -1, -1, -1, 309, -1, -1, -1, 309, 309, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309, 309, -1, -1, -1, 333, -1, -1, -1, 309 },
			{ -1, 106, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, -1, 311, 311, 311, 311, 311, 311, 311, 311, -1, -1, 311, 311, 311, -1, -1, -1, -1, 311, -1, -1, -1, 311, 311, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 311, 311, -1, -1, -1, -1, -1, 333, -1, 311 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 107, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, 406, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 108, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 408, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 109, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 110, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, 111, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 113, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 114, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 115, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 116, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 117, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 118, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 121, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 122, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 335, 119, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 123, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, 120, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 124, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 125, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 126, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 134, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 127, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 128, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 143, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 129, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, -1, 185, 185, 185, 185, 185, 185, 185, 185, -1, -1, 185, -1, 185, -1, -1, -1, -1, 185, -1, -1, -1, 185, 185, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 144, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185 },
			{ -1, 130, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 131, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 132, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, -1, 146, 146, 146, 146, 146, 146, 146, 146, -1, -1, 146, -1, 146, -1, -1, -1, -1, 146, -1, -1, -1, 146, 146, 146, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 146 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 133, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 2, 147, 147, 147, 359, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 148, 147, 147, 147, 2 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 135, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1 },
			{ -1, 136, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, -1 },
			{ -1, 137, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 143, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 138, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 139, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 140, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1 },
			{ 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, 214, 214, 214, 214, 214, 214, -1 },
			{ 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 143, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1 },
			{ 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1 },
			{ 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, -1 },
			{ 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1 },
			{ -1, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 375, 152, 375, 375, 375, 375, 375, 375, 375, 375, -1, -1, 375, 375, 375, -1, -1, -1, -1, 375, -1, -1, -1, 375, 375, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 375, 375, 188, -1, -1, -1, -1, -1, 376, 375 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 152, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 188, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2 },
			{ 153, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 8, 154, 154, 154, 154, 154, 154, 154, 154, 189, 153, 154, 153, 154, 153, 153, 153, 153, 154, 153, 8, 153, 154, 154, 154, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 8, 153, 153, 153, 153, 153, 153, 154 },
			{ 156, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 156, 190, 190, 190, 190, 190, 190, 190, 190, 156, 156, 197, 156, 190, 156, 156, 156, 156, 190, 156, 156, 156, 190, 190, 190, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 197 },
			{ 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 2, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 159, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 2 },
			{ 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 2, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 162, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 2 },
			{ 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 165, 164, 164, 164, 164, 164, 164, 164, 164, 164, 166, 2, 164, 164, 164, 164, 164, 192, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 192, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 191, 164, 164, 164, 164, 164, 164, 2 },
			{ 6, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 168, 605, 605, 605, 605, 605, 605, 605, 605, 169, 169, 605, 170, 605, 169, 168, 169, 169, 605, 169, 168, 169, 605, 605, 605, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 193, 169, 169, 194, 170, 168, 171, 169, 168, 168, 169, 169, 605 },
			{ 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 2, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 2 },
			{ 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 2, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 177 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 216, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 271, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 275, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 397, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, 311, -1, 311, 311, 311, 311, 311, 311, 311, 311, -1, -1, 311, -1, 311, -1, -1, -1, -1, 311, -1, -1, -1, 311, 311, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 311 },
			{ -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 219, 605, 605, -1, 605, 605, 418, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 285, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 420, 605, 605, 605, 542, 605, 605, 225, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 540, 605, 605, 228, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 231, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 421, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 237, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 240, 570, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 434, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 243, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 246, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 435, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 249, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 587, 605, 605, 605, 605, 436, 605, 437, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 438, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 440, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 543, 605, 605, 571, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 441, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 598, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 445, 605, 605, 605, 605, -1, 605, 446, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 447, 605, 605, 605, 605, 605, 605, 252, 605, 605, 615, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 548, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 572, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 448, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 549, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 449, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 254, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 256, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 546, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 588, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 453, 605, 605, 605, 605, 605, 605, 600, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 454, 455, 456, 605, 457, 599, 605, 605, 605, 605, 551, -1, 458, 605, 552, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 258, 605, 553, 460, 605, 605, 605, 605, 461, 605, 605, 605, -1, 605, 605, 605, 462, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 260, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 573, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 463, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 262, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 264, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 266, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 268, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 464, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 270, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 575, 605, 605, 605, 605, 605, 605, 272, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 274, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 276, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 278, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 468, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 280, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 282, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 284, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 286, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 288, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 604, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 606, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 471, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 472, 605, 605, 605, 554, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 473, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 555, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 475, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 290, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 477, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 559, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 557, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 480, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 580, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 484, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 294, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 296, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 298, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 300, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 302, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 488, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 489, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 561, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 490, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 304, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 493, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 563, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 495, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 306, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 497, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 308, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 310, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 314, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 564, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 500, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 316, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 318, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 320, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 502, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 582, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 504, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 505, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 584, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 322, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 507, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 508, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 509, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 324, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 326, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 328, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 330, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 332, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 334, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 517, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 518, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 336, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 338, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 340, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 522, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 523, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 342, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 344, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 346, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 616, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 524, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 348, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 525, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 565, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 350, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 352, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 526, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 354, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 356, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 527, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 358, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 529, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 532, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 533, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 360, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 362, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 364, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 534, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 535, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 366, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 536, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 537, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 368, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 569, 605, 605, 605, 422, -1, 605, 423, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 547, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 443, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 450, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 442, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 590, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 451, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 452, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 469, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 577, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 466, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 591, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 478, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 578, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 556, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 476, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 614, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 491, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 492, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 496, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 583, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 494, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 499, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 562, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 515, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 506, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 511, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 528, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 530, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 424, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 425, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 589, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 444, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 459, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 576, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 467, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 479, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 579, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 560, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 482, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 611, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 581, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 501, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 498, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 503, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 516, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 512, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 519, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 531, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 426, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 550, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 470, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 574, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 481, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 486, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 483, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 558, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 510, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 513, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 520, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 427, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 465, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 474, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 593, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 485, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 514, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 428, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 487, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 612, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 544, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 429, 605, 605, 605, 430, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 431, 605, 605, 605, 605, 432, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 433, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 594, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 595, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 566, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 602, 605, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 605, 601, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 },
			{ -1, 605, 605, 605, 605, 605, 605, 605, 605, 605, 585, 605, 605, 605, 605, -1, 605, 605, 605, 605, 605, 605, 605, 605, -1, -1, 605, 605, 605, -1, -1, -1, -1, 605, -1, -1, -1, 605, 605, 605, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 605, 605, -1, -1, -1, -1, -1, -1, -1, 605 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  207,
			  141,
			  357,
			  201,
			  210,
			  377,
			  378,
			  379,
			  381,
			  382,
			  383,
			  384,
			  387,
			  388
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 617);
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

