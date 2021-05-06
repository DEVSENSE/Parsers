namespace Devsense.PHP.Syntax
{
	#region User Code
	
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
using Devsense.PHP.Text;
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
			ST_HALT_COMPILER1 = 15,
			ST_HALT_COMPILER2 = 16,
			ST_HALT_COMPILER3 = 17,
			ST_IN_STRING = 18,
			ST_IN_SHELL = 19,
			ST_IN_HEREDOC = 20,
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
					// #line 93
					{ 
						return ProcessEof(Tokens.T_INLINE_HTML);
					}
					break;
					
				case 3:
					// #line 740
					{
						yymore(); break;
					}
					break;
					
				case 4:
					// #line 727
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						if (EnableShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							yymore(); break;
						}
					}
					break;
					
				case 5:
					// #line 707
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 6:
					// #line 717
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 7:
					// #line 79
					{
						return Tokens.EOF;
					}
					break;
					
				case 8:
					// #line 615
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 761
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 765
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 11:
					// #line 296
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 12:
					// #line 655
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 13:
					// #line 941
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 14:
					// #line 323
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 15:
					// #line 620
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 628
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 917
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 907
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 856
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 331
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 781
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 166
					{
						return Identifier(Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 190
					{
						return Identifier(Tokens.T_DO);
					}
					break;
					
				case 24:
					// #line 122
					{
						return Identifier(Tokens.T_FN);
					}
					break;
					
				case 25:
					// #line 595
					{
						return Identifier(Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 26:
					// #line 138
					{
						return Tokens.T_ATTRIBUTE;
					}
					break;
					
				case 27:
					// #line 222
					{
						return Identifier(Tokens.T_AS);
					}
					break;
					
				case 28:
					// #line 499
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 29:
					// #line 286
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 30:
					// #line 535
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 31:
					// #line 611
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 32:
					// #line 527
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 33:
					// #line 671
					{
						return ProcessRealNumber();
					}
					break;
					
				case 34:
					// #line 319
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 35:
					// #line 555
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 36:
					// #line 771
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 37:
					// #line 551
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 38:
					// #line 543
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 39:
					// #line 539
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 40:
					// #line 479
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 41:
					// #line 511
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 42:
					// #line 531
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 43:
					// #line 495
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 44:
					// #line 515
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 45:
					// #line 523
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 46:
					// #line 607
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 47:
					// #line 559
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 48:
					// #line 571
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 49:
					// #line 591
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 50:
					// #line 575
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 51:
					// #line 587
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 52:
					// #line 579
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 53:
					// #line 786
					{
						return ProcessVariable();
					}
					break;
					
				case 54:
					// #line 583
					{
						return (Tokens.T_COALESCE_EQUAL);
					}
					break;
					
				case 55:
					// #line 291
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 56:
					// #line 603
					{
						return Identifier(Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 57:
					// #line 150
					{
						return Identifier(Tokens.T_TRY);
					}
					break;
					
				case 58:
					// #line 118
					{
						return Identifier(Tokens.T_EXIT);
					}
					break;
					
				case 59:
					// #line 194
					{
						return Identifier(Tokens.T_FOR);
					}
					break;
					
				case 60:
					// #line 335
					{
						return Identifier(Tokens.T_NEW);
					}
					break;
					
				case 61:
					// #line 399
					{
						return Identifier(Tokens.T_USE);
					}
					break;
					
				case 62:
					// #line 599
					{
						return Identifier(Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 63:
					// #line 567
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 64:
					// #line 327
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 65:
					// #line 343
					{
						return Identifier(Tokens.T_VAR);
					}
					break;
					
				case 66:
					// #line 547
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 67:
					// #line 503
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 68:
					// #line 507
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 69:
					// #line 519
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 70:
					// #line 563
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 71:
					// #line 659
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 72:
					// #line 651
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 73:
					// #line 114
					{ 
						return Identifier(Tokens.T_EXIT);
					}
					break;
					
				case 74:
					// #line 258
					{
						return Identifier(Tokens.T_ECHO);
					}
					break;
					
				case 75:
					// #line 178
					{
						return Identifier(Tokens.T_ELSE);
					}
					break;
					
				case 76:
					// #line 375
					{
						return Identifier(Tokens.T_EVAL);
					}
					break;
					
				case 77:
					// #line 238
					{
						return Identifier(Tokens.T_CASE);
					}
					break;
					
				case 78:
					// #line 483
					{
						return Identifier(Tokens.T_LIST);
					}
					break;
					
				case 79:
					// #line 254
					{
						return Identifier(Tokens.T_GOTO);
					}
					break;
					
				case 80:
					// #line 776
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 81:
					// #line 174
					{
						return Identifier(Tokens.T_ENDIF);
					}
					break;
					
				case 82:
					// #line 415
					{
						return Identifier(Tokens.T_EMPTY);
					}
					break;
					
				case 83:
					// #line 411
					{
						return Identifier(Tokens.T_ISSET);
					}
					break;
					
				case 84:
					// #line 274
					{
						return Identifier(Tokens.T_TRAIT);
					}
					break;
					
				case 85:
					// #line 162
					{
						return Identifier(Tokens.T_THROW);
					}
					break;
					
				case 86:
					// #line 459
					{
						return Identifier(Tokens.T_FINAL);
					}
					break;
					
				case 87:
					// #line 475
					{
						return Identifier(Tokens.T_UNSET);
					}
					break;
					
				case 88:
					// #line 130
					{
						return Identifier(Tokens.T_CONST);
					}
					break;
					
				case 89:
					// #line 339
					{
						return Identifier(Tokens.T_CLONE);
					}
					break;
					
				case 90:
					// #line 266
					{
						return Identifier(Tokens.T_CLASS);
					}
					break;
					
				case 91:
					// #line 154
					{
						return Identifier(Tokens.T_CATCH);
					}
					break;
					
				case 92:
					// #line 146
					{
						return Identifier(Tokens.T_YIELD);
					}
					break;
					
				case 93:
					// #line 230
					{
						return Identifier(Tokens.T_MATCH);
					}
					break;
					
				case 94:
					// #line 487
					{
						return Identifier(Tokens.T_ARRAY);
					}
					break;
					
				case 95:
					// #line 182
					{
						return Identifier(Tokens.T_WHILE);
					}
					break;
					
				case 96:
					// #line 246
					{
						return Identifier(Tokens.T_BREAK);
					}
					break;
					
				case 97:
					// #line 262
					{
						return Identifier(Tokens.T_PRINT);
					}
					break;
					
				case 98:
					// #line 347
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 99:
					// #line 790
					{
						int bprefix = (GetTokenChar(0) != '<') ? 1 : 0;
						int s = bprefix + 3;
					    int length = TokenLength - bprefix - 3 - 1 - (GetTokenChar(TokenLength - 2) == '\r' ? 1 : 0);
					    while (char.IsWhiteSpace(GetTokenChar(s))) {	// {TABS_AND_SPACES}
							s++;
					        length--;
					    }
						_tokenSemantics.QuoteToken = Tokens.END;
						if (GetTokenChar(s) == '\'') {
							_tokenSemantics.QuoteToken = Tokens.T_SINGLE_QUOTES;
					        BEGIN(LexicalStates.ST_NOWDOC);
						} else {
							if (GetTokenChar(s) == '"') {
								_tokenSemantics.QuoteToken = Tokens.T_DOUBLE_QUOTES;
					        }
							BEGIN(LexicalStates.ST_HEREDOC);
						}
						if (_tokenSemantics.QuoteToken != Tokens.END) {	// enclosed in quotes
							s++;
					        length -= 2;
						}
						_tokenSemantics.Object = _hereDocValue = new HereDocTokenValue(GetTokenSubstring(s, length));
						//
						return Tokens.T_START_HEREDOC;
					}
					break;
					
				case 100:
					// #line 198
					{
						return Identifier(Tokens.T_ENDFOR);
					}
					break;
					
				case 101:
					// #line 170
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 102:
					// #line 451
					{
						return Identifier(Tokens.T_STATIC);
					}
					break;
					
				case 103:
					// #line 226
					{
						return Identifier(Tokens.T_SWITCH);
					}
					break;
					
				case 104:
					// #line 134
					{
						return Identifier(Tokens.T_RETURN);
					}
					break;
					
				case 105:
					// #line 407
					{
						return Identifier(Tokens.T_GLOBAL);
					}
					break;
					
				case 106:
					// #line 471
					{
						return Identifier(Tokens.T_PUBLIC);
					}
					break;
					
				case 107:
					// #line 351
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 108:
					// #line 367
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 109:
					// #line 278
					{
						return Identifier(Tokens.T_EXTENDS);
					}
					break;
					
				case 110:
					// #line 379
					{
						return Identifier(Tokens.T_INCLUDE);
					}
					break;
					
				case 111:
					// #line 242
					{
						return Identifier(Tokens.T_DEFAULT);
					}
					break;
					
				case 112:
					// #line 210
					{
						return Identifier(Tokens.T_DECLARE);
					}
					break;
					
				case 113:
					// #line 158
					{
						return Identifier(Tokens.T_FINALLY);
					}
					break;
					
				case 114:
					// #line 202
					{
						return Identifier(Tokens.T_FOREACH);
					}
					break;
					
				case 115:
					// #line 387
					{
						return Identifier(Tokens.T_REQUIRE);
					}
					break;
					
				case 116:
					// #line 463
					{
						return Identifier(Tokens.T_PRIVATE);
					}
					break;
					
				case 117:
					// #line 371
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 118:
					// #line 359
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 119:
					// #line 699
					{
						return Identifier(Tokens.T_DIR);
					}
					break;
					
				case 120:
					// #line 186
					{
						return Identifier(Tokens.T_ENDWHILE);
					}
					break;
					
				case 121:
					// #line 126
					{
						return Identifier(Tokens.T_FUNCTION);
					}
					break;
					
				case 122:
					// #line 250
					{
						return Identifier(Tokens.T_CONTINUE);
					}
					break;
					
				case 123:
					// #line 491
					{
						return Identifier(Tokens.T_CALLABLE);
					}
					break;
					
				case 124:
					// #line 455
					{
						return Identifier(Tokens.T_ABSTRACT);
					}
					break;
					
				case 125:
					// #line 363
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 126:
					// #line 355
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 127:
					// #line 695
					{
						return Identifier(Tokens.T_FILE);
					}
					break;
					
				case 128:
					// #line 691
					{
						return Identifier(Tokens.T_LINE);
					}
					break;
					
				case 129:
					// #line 234
					{
						return Identifier(Tokens.T_ENDSWITCH);
					}
					break;
					
				case 130:
					// #line 270
					{
						return Identifier(Tokens.T_INTERFACE);
					}
					break;
					
				case 131:
					// #line 403
					{
						return Identifier(Tokens.T_INSTEADOF);
					}
					break;
					
				case 132:
					// #line 395
					{
						return Identifier(Tokens.T_NAMESPACE);
					}
					break;
					
				case 133:
					// #line 467
					{
						return Identifier(Tokens.T_PROTECTED);
					}
					break;
					
				case 134:
					// #line 679
					{
						return Identifier(Tokens.T_TRAIT_C);
					}
					break;
					
				case 135:
					// #line 675
					{
						return Identifier(Tokens.T_CLASS_C);
					}
					break;
					
				case 136:
					// #line 214
					{
						return Identifier(Tokens.T_ENDDECLARE);
					}
					break;
					
				case 137:
					// #line 206
					{
						return Identifier(Tokens.T_ENDFOREACH);
					}
					break;
					
				case 138:
					// #line 218
					{
						return Identifier(Tokens.T_INSTANCEOF);
					}
					break;
					
				case 139:
					// #line 282
					{
						return Identifier(Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 140:
					// #line 142
					{
						return Identifier(Tokens.T_YIELD_FROM);
					}
					break;
					
				case 141:
					// #line 687
					{
						return Identifier(Tokens.T_METHOD_C);
					}
					break;
					
				case 142:
					// #line 383
					{
						return Identifier(Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 143:
					// #line 391
					{
						return Identifier(Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 144:
					// #line 683
					{
						return Identifier(Tokens.T_FUNC_C);
					}
					break;
					
				case 145:
					// #line 703
					{
						return Identifier(Tokens.T_NS_C);
					}
					break;
					
				case 146:
					// #line 434
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return Identifier(Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 147:
					// #line 85
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 148:
					// #line 915
					{ yymore(); break; }
					break;
					
				case 149:
					// #line 913
					{ yymore(); break; }
					break;
					
				case 150:
					// #line 911
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 151:
					// #line 914
					{ yymore(); break; }
					break;
					
				case 152:
					// #line 910
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 153:
					// #line 909
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 154:
					// #line 908
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 155:
					// #line 82
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 156:
					// #line 859
					{ yymore(); break; }
					break;
					
				case 157:
					// #line 858
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 860
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 159:
					// #line 857
					{ yymore(); break; }
					break;
					
				case 160:
					// #line 925
					{ yymore(); break; }
					break;
					
				case 161:
					// #line 923
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 921
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 163:
					// #line 924
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 920
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 165:
					// #line 919
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 166:
					// #line 918
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 167:
					// #line 934
					{ yymore(); break; }
					break;
					
				case 168:
					// #line 933
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 931
					{ yymore(); break; }
					break;
					
				case 170:
					// #line 845
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(_processDoubleQuotedString) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 171:
					// #line 932
					{ yymore(); break; }
					break;
					
				case 172:
					// #line 929
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 173:
					// #line 928
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 174:
					// #line 927
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 175:
					// #line 313
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 176:
					// #line 308
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 177:
					// #line 300
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 178:
					// #line 304
					{
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 179:
					// #line 644
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 180:
					// #line 636
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 181:
					// #line 105
					{
						if(TokenLength > 0)
						{
							SetDocBlock(); 
							return Tokens.T_DOC_COMMENT; 
						}
						return Tokens.EOF;
					}
					break;
					
				case 182:
					// #line 777
					{ yymore(); break; }
					break;
					
				case 183:
					// #line 779
					{ yymore(); break; }
					break;
					
				case 184:
					// #line 778
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 185:
					// #line 97
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 186:
					// #line 772
					{ yymore(); break; }
					break;
					
				case 187:
					// #line 774
					{ yymore(); break; }
					break;
					
				case 188:
					// #line 773
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 189:
					// #line 939
					{ yymore(); break; }
					break;
					
				case 190:
					// #line 101
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 191:
					// #line 938
					{ yymore(); break; }
					break;
					
				case 192:
					// #line 936
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 193:
					// #line 937
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 194:
					// #line 749
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 195:
					// #line 663
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 196:
					// #line 744
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 197:
					// #line 667
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 198:
					// #line 832
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 199:
					// #line 821
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						if (GetTokenSpan().TrimLeft() != _hereDocValue.Label)
						{
							_errors.Error(_tokenPosition, Devsense.PHP.Errors.FatalErrors.SyntaxError, "Incorrect heredoc end label: " + GetTokenSpan().Trim().ToString());
						}
						_yyless(LabelTrailLength());
						_tokenSemantics.Object = _hereDocValue ?? throw new InvalidOperationException("Expected '_hereDocValue' to be set.");
						return Tokens.T_END_HEREDOC;
					}
					break;
					
				case 200:
					// #line 854
					{ yymore(); break; }
					break;
					
				case 201:
					// #line 836
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(null) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 202:
					// #line 446
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 203:
					// #line 443
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 204:
					// #line 440
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 205:
					// #line 419
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 206:
					// #line 444
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 207:
					// #line 442
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 208:
					// #line 424
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 209:
					// #line 429
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 210:
					// #line 901
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 211:
					// #line 890
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 212:
					// #line 884
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 213:
					// #line 879
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 214:
					// #line 862
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 215:
					// #line 873
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 216:
					// #line 867
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 217:
					// #line 895
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 220: goto case 3;
				case 221: goto case 6;
				case 222: goto case 8;
				case 223: goto case 9;
				case 224: goto case 10;
				case 225: goto case 12;
				case 226: goto case 21;
				case 227: goto case 33;
				case 228: goto case 44;
				case 229: goto case 99;
				case 230: goto case 149;
				case 231: goto case 157;
				case 232: goto case 161;
				case 233: goto case 168;
				case 234: goto case 169;
				case 235: goto case 175;
				case 236: goto case 179;
				case 237: goto case 189;
				case 238: goto case 192;
				case 239: goto case 194;
				case 240: goto case 195;
				case 241: goto case 197;
				case 242: goto case 202;
				case 243: goto case 210;
				case 246: goto case 8;
				case 247: goto case 9;
				case 248: goto case 21;
				case 249: goto case 33;
				case 250: goto case 175;
				case 251: goto case 197;
				case 252: goto case 210;
				case 254: goto case 8;
				case 255: goto case 9;
				case 257: goto case 8;
				case 258: goto case 9;
				case 260: goto case 8;
				case 261: goto case 9;
				case 263: goto case 8;
				case 264: goto case 9;
				case 266: goto case 8;
				case 267: goto case 9;
				case 269: goto case 8;
				case 270: goto case 9;
				case 272: goto case 8;
				case 273: goto case 9;
				case 275: goto case 8;
				case 276: goto case 9;
				case 278: goto case 8;
				case 279: goto case 9;
				case 281: goto case 8;
				case 282: goto case 9;
				case 284: goto case 8;
				case 285: goto case 9;
				case 287: goto case 8;
				case 288: goto case 9;
				case 290: goto case 8;
				case 291: goto case 9;
				case 293: goto case 8;
				case 294: goto case 9;
				case 296: goto case 8;
				case 297: goto case 9;
				case 299: goto case 9;
				case 301: goto case 9;
				case 303: goto case 9;
				case 305: goto case 9;
				case 307: goto case 9;
				case 309: goto case 9;
				case 311: goto case 9;
				case 313: goto case 9;
				case 315: goto case 9;
				case 317: goto case 9;
				case 319: goto case 9;
				case 321: goto case 9;
				case 323: goto case 9;
				case 325: goto case 9;
				case 327: goto case 9;
				case 329: goto case 9;
				case 331: goto case 9;
				case 333: goto case 9;
				case 335: goto case 9;
				case 337: goto case 9;
				case 339: goto case 9;
				case 341: goto case 9;
				case 343: goto case 9;
				case 345: goto case 9;
				case 347: goto case 9;
				case 349: goto case 9;
				case 351: goto case 9;
				case 353: goto case 9;
				case 355: goto case 9;
				case 357: goto case 9;
				case 359: goto case 9;
				case 361: goto case 9;
				case 363: goto case 9;
				case 365: goto case 9;
				case 367: goto case 9;
				case 369: goto case 9;
				case 371: goto case 9;
				case 373: goto case 9;
				case 375: goto case 9;
				case 377: goto case 9;
				case 379: goto case 9;
				case 381: goto case 9;
				case 383: goto case 9;
				case 385: goto case 9;
				case 387: goto case 9;
				case 389: goto case 9;
				case 391: goto case 9;
				case 393: goto case 9;
				case 395: goto case 9;
				case 397: goto case 9;
				case 399: goto case 9;
				case 401: goto case 9;
				case 403: goto case 9;
				case 405: goto case 9;
				case 407: goto case 9;
				case 409: goto case 9;
				case 411: goto case 9;
				case 413: goto case 9;
				case 415: goto case 9;
				case 417: goto case 9;
				case 419: goto case 9;
				case 439: goto case 9;
				case 450: goto case 9;
				case 453: goto case 9;
				case 455: goto case 9;
				case 457: goto case 9;
				case 459: goto case 9;
				case 460: goto case 9;
				case 461: goto case 9;
				case 462: goto case 9;
				case 463: goto case 9;
				case 464: goto case 9;
				case 465: goto case 9;
				case 466: goto case 9;
				case 467: goto case 9;
				case 468: goto case 9;
				case 469: goto case 9;
				case 470: goto case 9;
				case 471: goto case 9;
				case 472: goto case 9;
				case 473: goto case 9;
				case 474: goto case 9;
				case 475: goto case 9;
				case 476: goto case 9;
				case 477: goto case 9;
				case 478: goto case 9;
				case 479: goto case 9;
				case 480: goto case 9;
				case 481: goto case 9;
				case 482: goto case 9;
				case 483: goto case 9;
				case 484: goto case 9;
				case 485: goto case 9;
				case 486: goto case 9;
				case 487: goto case 9;
				case 488: goto case 9;
				case 489: goto case 9;
				case 490: goto case 9;
				case 491: goto case 9;
				case 492: goto case 9;
				case 493: goto case 9;
				case 494: goto case 9;
				case 495: goto case 9;
				case 496: goto case 9;
				case 497: goto case 9;
				case 498: goto case 9;
				case 499: goto case 9;
				case 500: goto case 9;
				case 501: goto case 9;
				case 502: goto case 9;
				case 503: goto case 9;
				case 504: goto case 9;
				case 505: goto case 9;
				case 506: goto case 9;
				case 507: goto case 9;
				case 508: goto case 9;
				case 509: goto case 9;
				case 510: goto case 9;
				case 511: goto case 9;
				case 512: goto case 9;
				case 513: goto case 9;
				case 514: goto case 9;
				case 515: goto case 9;
				case 516: goto case 9;
				case 517: goto case 9;
				case 518: goto case 9;
				case 519: goto case 9;
				case 520: goto case 9;
				case 521: goto case 9;
				case 522: goto case 9;
				case 523: goto case 9;
				case 524: goto case 9;
				case 525: goto case 9;
				case 526: goto case 9;
				case 527: goto case 9;
				case 528: goto case 9;
				case 529: goto case 9;
				case 530: goto case 9;
				case 531: goto case 9;
				case 532: goto case 9;
				case 533: goto case 9;
				case 534: goto case 9;
				case 535: goto case 9;
				case 536: goto case 9;
				case 537: goto case 9;
				case 538: goto case 9;
				case 539: goto case 9;
				case 540: goto case 9;
				case 541: goto case 9;
				case 542: goto case 9;
				case 543: goto case 9;
				case 544: goto case 9;
				case 545: goto case 9;
				case 546: goto case 9;
				case 547: goto case 9;
				case 548: goto case 9;
				case 549: goto case 9;
				case 550: goto case 9;
				case 551: goto case 9;
				case 552: goto case 9;
				case 553: goto case 9;
				case 554: goto case 9;
				case 555: goto case 9;
				case 556: goto case 9;
				case 557: goto case 9;
				case 558: goto case 9;
				case 559: goto case 9;
				case 560: goto case 9;
				case 561: goto case 9;
				case 562: goto case 9;
				case 563: goto case 9;
				case 564: goto case 9;
				case 565: goto case 9;
				case 566: goto case 9;
				case 567: goto case 9;
				case 568: goto case 9;
				case 569: goto case 9;
				case 570: goto case 9;
				case 571: goto case 9;
				case 572: goto case 9;
				case 573: goto case 9;
				case 574: goto case 9;
				case 575: goto case 9;
				case 576: goto case 9;
				case 577: goto case 9;
				case 578: goto case 9;
				case 579: goto case 9;
				case 580: goto case 9;
				case 581: goto case 9;
				case 582: goto case 9;
				case 583: goto case 9;
				case 584: goto case 9;
				case 585: goto case 9;
				case 586: goto case 9;
				case 587: goto case 9;
				case 588: goto case 9;
				case 589: goto case 9;
				case 590: goto case 9;
				case 591: goto case 9;
				case 592: goto case 9;
				case 593: goto case 9;
				case 594: goto case 9;
				case 595: goto case 9;
				case 596: goto case 9;
				case 597: goto case 9;
				case 598: goto case 9;
				case 599: goto case 9;
				case 600: goto case 9;
				case 601: goto case 9;
				case 602: goto case 9;
				case 603: goto case 9;
				case 604: goto case 9;
				case 605: goto case 9;
				case 606: goto case 9;
				case 607: goto case 9;
				case 608: goto case 9;
				case 609: goto case 9;
				case 610: goto case 9;
				case 611: goto case 9;
				case 612: goto case 9;
				case 613: goto case 9;
				case 614: goto case 9;
				case 615: goto case 9;
				case 616: goto case 9;
				case 617: goto case 9;
				case 618: goto case 9;
				case 619: goto case 9;
				case 620: goto case 9;
				case 621: goto case 9;
				case 622: goto case 9;
				case 623: goto case 9;
				case 624: goto case 9;
				case 625: goto case 9;
				case 626: goto case 9;
				case 627: goto case 9;
				case 628: goto case 9;
				case 629: goto case 9;
				case 630: goto case 9;
				case 631: goto case 9;
				case 632: goto case 9;
				case 633: goto case 9;
				case 634: goto case 9;
				case 635: goto case 9;
				case 636: goto case 9;
				case 637: goto case 9;
				case 638: goto case 9;
				case 639: goto case 9;
				case 640: goto case 9;
				case 641: goto case 9;
				case 642: goto case 9;
				case 643: goto case 9;
				case 644: goto case 9;
				case 645: goto case 9;
				case 646: goto case 9;
				case 647: goto case 9;
				case 648: goto case 9;
				case 649: goto case 9;
				case 650: goto case 9;
				case 651: goto case 9;
				case 652: goto case 9;
				case 653: goto case 9;
				case 654: goto case 9;
				case 655: goto case 9;
				case 656: goto case 9;
				case 657: goto case 9;
				case 658: goto case 9;
				case 659: goto case 9;
				case 660: goto case 9;
				case 661: goto case 9;
				case 662: goto case 9;
				case 663: goto case 9;
				case 664: goto case 9;
				case 665: goto case 9;
				case 666: goto case 9;
				case 667: goto case 9;
				case 668: goto case 9;
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
			AcceptConditions.Accept, // 167
			AcceptConditions.Accept, // 168
			AcceptConditions.Accept, // 169
			AcceptConditions.AcceptOnStart, // 170
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
			AcceptConditions.Accept, // 190
			AcceptConditions.Accept, // 191
			AcceptConditions.Accept, // 192
			AcceptConditions.Accept, // 193
			AcceptConditions.Accept, // 194
			AcceptConditions.Accept, // 195
			AcceptConditions.Accept, // 196
			AcceptConditions.Accept, // 197
			AcceptConditions.Accept, // 198
			AcceptConditions.Accept, // 199
			AcceptConditions.Accept, // 200
			AcceptConditions.AcceptOnStart, // 201
			AcceptConditions.Accept, // 202
			AcceptConditions.Accept, // 203
			AcceptConditions.Accept, // 204
			AcceptConditions.Accept, // 205
			AcceptConditions.Accept, // 206
			AcceptConditions.Accept, // 207
			AcceptConditions.Accept, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.Accept, // 210
			AcceptConditions.Accept, // 211
			AcceptConditions.Accept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.Accept, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.Accept, // 216
			AcceptConditions.Accept, // 217
			AcceptConditions.NotAccept, // 218
			AcceptConditions.Accept, // 219
			AcceptConditions.Accept, // 220
			AcceptConditions.Accept, // 221
			AcceptConditions.Accept, // 222
			AcceptConditions.Accept, // 223
			AcceptConditions.Accept, // 224
			AcceptConditions.Accept, // 225
			AcceptConditions.Accept, // 226
			AcceptConditions.Accept, // 227
			AcceptConditions.Accept, // 228
			AcceptConditions.Accept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.Accept, // 234
			AcceptConditions.Accept, // 235
			AcceptConditions.Accept, // 236
			AcceptConditions.Accept, // 237
			AcceptConditions.Accept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.Accept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.NotAccept, // 244
			AcceptConditions.Accept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.Accept, // 248
			AcceptConditions.Accept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.Accept, // 251
			AcceptConditions.Accept, // 252
			AcceptConditions.NotAccept, // 253
			AcceptConditions.Accept, // 254
			AcceptConditions.Accept, // 255
			AcceptConditions.NotAccept, // 256
			AcceptConditions.Accept, // 257
			AcceptConditions.Accept, // 258
			AcceptConditions.NotAccept, // 259
			AcceptConditions.Accept, // 260
			AcceptConditions.Accept, // 261
			AcceptConditions.NotAccept, // 262
			AcceptConditions.Accept, // 263
			AcceptConditions.Accept, // 264
			AcceptConditions.NotAccept, // 265
			AcceptConditions.Accept, // 266
			AcceptConditions.Accept, // 267
			AcceptConditions.NotAccept, // 268
			AcceptConditions.Accept, // 269
			AcceptConditions.Accept, // 270
			AcceptConditions.NotAccept, // 271
			AcceptConditions.Accept, // 272
			AcceptConditions.Accept, // 273
			AcceptConditions.NotAccept, // 274
			AcceptConditions.Accept, // 275
			AcceptConditions.Accept, // 276
			AcceptConditions.NotAccept, // 277
			AcceptConditions.Accept, // 278
			AcceptConditions.Accept, // 279
			AcceptConditions.NotAccept, // 280
			AcceptConditions.Accept, // 281
			AcceptConditions.Accept, // 282
			AcceptConditions.NotAccept, // 283
			AcceptConditions.Accept, // 284
			AcceptConditions.Accept, // 285
			AcceptConditions.NotAccept, // 286
			AcceptConditions.Accept, // 287
			AcceptConditions.Accept, // 288
			AcceptConditions.NotAccept, // 289
			AcceptConditions.Accept, // 290
			AcceptConditions.Accept, // 291
			AcceptConditions.NotAccept, // 292
			AcceptConditions.Accept, // 293
			AcceptConditions.Accept, // 294
			AcceptConditions.NotAccept, // 295
			AcceptConditions.Accept, // 296
			AcceptConditions.Accept, // 297
			AcceptConditions.NotAccept, // 298
			AcceptConditions.Accept, // 299
			AcceptConditions.NotAccept, // 300
			AcceptConditions.Accept, // 301
			AcceptConditions.NotAccept, // 302
			AcceptConditions.Accept, // 303
			AcceptConditions.NotAccept, // 304
			AcceptConditions.Accept, // 305
			AcceptConditions.NotAccept, // 306
			AcceptConditions.Accept, // 307
			AcceptConditions.NotAccept, // 308
			AcceptConditions.Accept, // 309
			AcceptConditions.NotAccept, // 310
			AcceptConditions.Accept, // 311
			AcceptConditions.NotAccept, // 312
			AcceptConditions.Accept, // 313
			AcceptConditions.NotAccept, // 314
			AcceptConditions.Accept, // 315
			AcceptConditions.NotAccept, // 316
			AcceptConditions.Accept, // 317
			AcceptConditions.NotAccept, // 318
			AcceptConditions.Accept, // 319
			AcceptConditions.NotAccept, // 320
			AcceptConditions.Accept, // 321
			AcceptConditions.NotAccept, // 322
			AcceptConditions.Accept, // 323
			AcceptConditions.NotAccept, // 324
			AcceptConditions.Accept, // 325
			AcceptConditions.NotAccept, // 326
			AcceptConditions.Accept, // 327
			AcceptConditions.NotAccept, // 328
			AcceptConditions.Accept, // 329
			AcceptConditions.NotAccept, // 330
			AcceptConditions.Accept, // 331
			AcceptConditions.NotAccept, // 332
			AcceptConditions.Accept, // 333
			AcceptConditions.NotAccept, // 334
			AcceptConditions.Accept, // 335
			AcceptConditions.NotAccept, // 336
			AcceptConditions.Accept, // 337
			AcceptConditions.NotAccept, // 338
			AcceptConditions.Accept, // 339
			AcceptConditions.NotAccept, // 340
			AcceptConditions.Accept, // 341
			AcceptConditions.NotAccept, // 342
			AcceptConditions.Accept, // 343
			AcceptConditions.NotAccept, // 344
			AcceptConditions.Accept, // 345
			AcceptConditions.NotAccept, // 346
			AcceptConditions.Accept, // 347
			AcceptConditions.NotAccept, // 348
			AcceptConditions.Accept, // 349
			AcceptConditions.NotAccept, // 350
			AcceptConditions.Accept, // 351
			AcceptConditions.NotAccept, // 352
			AcceptConditions.Accept, // 353
			AcceptConditions.NotAccept, // 354
			AcceptConditions.Accept, // 355
			AcceptConditions.NotAccept, // 356
			AcceptConditions.Accept, // 357
			AcceptConditions.NotAccept, // 358
			AcceptConditions.Accept, // 359
			AcceptConditions.NotAccept, // 360
			AcceptConditions.Accept, // 361
			AcceptConditions.NotAccept, // 362
			AcceptConditions.Accept, // 363
			AcceptConditions.NotAccept, // 364
			AcceptConditions.Accept, // 365
			AcceptConditions.NotAccept, // 366
			AcceptConditions.Accept, // 367
			AcceptConditions.NotAccept, // 368
			AcceptConditions.Accept, // 369
			AcceptConditions.NotAccept, // 370
			AcceptConditions.Accept, // 371
			AcceptConditions.NotAccept, // 372
			AcceptConditions.Accept, // 373
			AcceptConditions.NotAccept, // 374
			AcceptConditions.Accept, // 375
			AcceptConditions.NotAccept, // 376
			AcceptConditions.Accept, // 377
			AcceptConditions.NotAccept, // 378
			AcceptConditions.Accept, // 379
			AcceptConditions.NotAccept, // 380
			AcceptConditions.Accept, // 381
			AcceptConditions.NotAccept, // 382
			AcceptConditions.Accept, // 383
			AcceptConditions.NotAccept, // 384
			AcceptConditions.Accept, // 385
			AcceptConditions.NotAccept, // 386
			AcceptConditions.Accept, // 387
			AcceptConditions.NotAccept, // 388
			AcceptConditions.Accept, // 389
			AcceptConditions.NotAccept, // 390
			AcceptConditions.Accept, // 391
			AcceptConditions.NotAccept, // 392
			AcceptConditions.Accept, // 393
			AcceptConditions.NotAccept, // 394
			AcceptConditions.Accept, // 395
			AcceptConditions.NotAccept, // 396
			AcceptConditions.Accept, // 397
			AcceptConditions.NotAccept, // 398
			AcceptConditions.Accept, // 399
			AcceptConditions.NotAccept, // 400
			AcceptConditions.Accept, // 401
			AcceptConditions.NotAccept, // 402
			AcceptConditions.Accept, // 403
			AcceptConditions.NotAccept, // 404
			AcceptConditions.Accept, // 405
			AcceptConditions.NotAccept, // 406
			AcceptConditions.Accept, // 407
			AcceptConditions.NotAccept, // 408
			AcceptConditions.Accept, // 409
			AcceptConditions.NotAccept, // 410
			AcceptConditions.Accept, // 411
			AcceptConditions.NotAccept, // 412
			AcceptConditions.Accept, // 413
			AcceptConditions.NotAccept, // 414
			AcceptConditions.Accept, // 415
			AcceptConditions.NotAccept, // 416
			AcceptConditions.Accept, // 417
			AcceptConditions.NotAccept, // 418
			AcceptConditions.Accept, // 419
			AcceptConditions.NotAccept, // 420
			AcceptConditions.NotAccept, // 421
			AcceptConditions.NotAccept, // 422
			AcceptConditions.NotAccept, // 423
			AcceptConditions.NotAccept, // 424
			AcceptConditions.NotAccept, // 425
			AcceptConditions.NotAccept, // 426
			AcceptConditions.NotAccept, // 427
			AcceptConditions.NotAccept, // 428
			AcceptConditions.NotAccept, // 429
			AcceptConditions.NotAccept, // 430
			AcceptConditions.NotAccept, // 431
			AcceptConditions.NotAccept, // 432
			AcceptConditions.NotAccept, // 433
			AcceptConditions.NotAccept, // 434
			AcceptConditions.NotAccept, // 435
			AcceptConditions.NotAccept, // 436
			AcceptConditions.NotAccept, // 437
			AcceptConditions.NotAccept, // 438
			AcceptConditions.Accept, // 439
			AcceptConditions.NotAccept, // 440
			AcceptConditions.NotAccept, // 441
			AcceptConditions.NotAccept, // 442
			AcceptConditions.NotAccept, // 443
			AcceptConditions.NotAccept, // 444
			AcceptConditions.NotAccept, // 445
			AcceptConditions.NotAccept, // 446
			AcceptConditions.NotAccept, // 447
			AcceptConditions.NotAccept, // 448
			AcceptConditions.NotAccept, // 449
			AcceptConditions.Accept, // 450
			AcceptConditions.NotAccept, // 451
			AcceptConditions.NotAccept, // 452
			AcceptConditions.Accept, // 453
			AcceptConditions.NotAccept, // 454
			AcceptConditions.Accept, // 455
			AcceptConditions.NotAccept, // 456
			AcceptConditions.Accept, // 457
			AcceptConditions.NotAccept, // 458
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
			AcceptConditions.Accept, // 622
			AcceptConditions.Accept, // 623
			AcceptConditions.Accept, // 624
			AcceptConditions.Accept, // 625
			AcceptConditions.Accept, // 626
			AcceptConditions.Accept, // 627
			AcceptConditions.Accept, // 628
			AcceptConditions.Accept, // 629
			AcceptConditions.Accept, // 630
			AcceptConditions.Accept, // 631
			AcceptConditions.Accept, // 632
			AcceptConditions.Accept, // 633
			AcceptConditions.Accept, // 634
			AcceptConditions.Accept, // 635
			AcceptConditions.Accept, // 636
			AcceptConditions.Accept, // 637
			AcceptConditions.Accept, // 638
			AcceptConditions.Accept, // 639
			AcceptConditions.Accept, // 640
			AcceptConditions.Accept, // 641
			AcceptConditions.Accept, // 642
			AcceptConditions.Accept, // 643
			AcceptConditions.Accept, // 644
			AcceptConditions.Accept, // 645
			AcceptConditions.Accept, // 646
			AcceptConditions.Accept, // 647
			AcceptConditions.Accept, // 648
			AcceptConditions.Accept, // 649
			AcceptConditions.Accept, // 650
			AcceptConditions.Accept, // 651
			AcceptConditions.Accept, // 652
			AcceptConditions.Accept, // 653
			AcceptConditions.Accept, // 654
			AcceptConditions.Accept, // 655
			AcceptConditions.Accept, // 656
			AcceptConditions.Accept, // 657
			AcceptConditions.Accept, // 658
			AcceptConditions.Accept, // 659
			AcceptConditions.Accept, // 660
			AcceptConditions.Accept, // 661
			AcceptConditions.Accept, // 662
			AcceptConditions.Accept, // 663
			AcceptConditions.Accept, // 664
			AcceptConditions.Accept, // 665
			AcceptConditions.Accept, // 666
			AcceptConditions.Accept, // 667
			AcceptConditions.Accept, // 668
		};
		
		private static int[] colMap = new int[]
		{
			32, 32, 32, 32, 32, 32, 32, 32, 32, 38, 19, 32, 32, 59, 32, 32, 
			32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 
			38, 48, 62, 15, 63, 50, 51, 64, 37, 39, 45, 47, 54, 28, 35, 44, 
			57, 58, 31, 31, 31, 31, 31, 31, 31, 31, 33, 43, 49, 46, 29, 2, 
			54, 21, 24, 11, 7, 3, 8, 26, 22, 5, 40, 25, 18, 20, 9, 12, 
			27, 42, 14, 13, 6, 10, 36, 23, 4, 17, 30, 16, 34, 60, 53, 41, 
			61, 21, 24, 11, 7, 3, 8, 26, 22, 5, 40, 25, 18, 20, 9, 12, 
			27, 42, 14, 13, 6, 10, 36, 23, 4, 17, 30, 55, 52, 56, 54, 32, 
			0, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 1, 2, 1, 1, 1, 3, 4, 5, 6, 7, 1, 1, 1, 
			1, 1, 1, 1, 8, 9, 10, 10, 10, 10, 1, 10, 1, 1, 1, 11, 
			1, 12, 1, 1, 13, 1, 14, 1, 1, 15, 1, 1, 16, 17, 18, 1, 
			1, 1, 1, 1, 1, 19, 1, 1, 10, 10, 10, 20, 10, 10, 10, 1, 
			1, 10, 1, 1, 1, 1, 1, 21, 22, 10, 10, 23, 10, 10, 10, 10, 
			24, 10, 10, 10, 10, 10, 25, 10, 10, 10, 10, 10, 26, 10, 10, 10, 
			10, 10, 1, 1, 27, 10, 10, 10, 10, 10, 10, 1, 1, 10, 28, 10, 
			10, 10, 10, 29, 10, 1, 1, 10, 10, 10, 10, 10, 10, 1, 1, 10, 
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 1, 10, 10, 10, 
			10, 10, 10, 1, 30, 31, 1, 1, 1, 1, 1, 1, 32, 32, 1, 1, 
			33, 34, 1, 1, 1, 1, 1, 35, 1, 36, 37, 1, 1, 1, 1, 38, 
			39, 1, 1, 1, 1, 1, 40, 41, 1, 1, 42, 43, 1, 44, 1, 45, 
			1, 1, 1, 46, 1, 47, 1, 48, 1, 49, 1, 1, 50, 1, 51, 52, 
			1, 1, 1, 1, 1, 53, 1, 1, 1, 1, 54, 55, 56, 57, 1, 58, 
			1, 59, 1, 60, 1, 61, 62, 63, 64, 65, 66, 1, 67, 68, 69, 70, 
			71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 
			87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 
			103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 
			119, 120, 121, 122, 123, 124, 125, 126, 70, 127, 94, 128, 129, 130, 131, 132, 
			133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 
			24, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 
			164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 
			180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 
			196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 
			212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 
			228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 55, 238, 239, 240, 241, 242, 
			243, 244, 245, 246, 67, 247, 248, 249, 250, 251, 252, 253, 254, 76, 255, 52, 
			256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 
			272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 
			288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 
			304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 
			320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 
			336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 
			352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 
			368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 
			384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 
			400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 
			416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 
			432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 
			448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 
			464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 
			480, 481, 10, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 220, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 259, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 459, 658, 658, 658, 658, 460, 658, 461, 658, 658, 658, -1, -1, 658, 462, -1, 463, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 464, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 265, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, 33, -1, -1, -1, -1, -1, 268, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 12, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 54, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 248, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 265, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, 227, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, 53, -1, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, 53, -1, -1, -1, -1, 53, -1, -1, -1, 53, 53, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 53, 53, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 644, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, -1, -1, -1, 71, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, 300, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, 71, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 302, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, 72, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 343, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 80, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 80, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 80, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 363, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, 362, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, 362, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, 362, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 648, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 572, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 665, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1, 148, 148, 148, 148, 148, 148, -1, -1, 148 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 152, -1 },
			{ -1, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1 },
			{ -1, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1, 160, 160, 160, 160, 160, -1, 160, -1, 160 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 164, -1 },
			{ -1, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, -1, 167, 167, 167, -1, 167 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1 },
			{ -1, -1, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, 170, 170, -1, -1, -1, -1, 170, -1, -1, -1, 170, 170, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, 170, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 416, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, -1, -1, 176, 176, -1, 176, 176, 176, 176, 176, 176, 176, 176, -1, -1, 176, 176, -1, -1, -1, -1, 176, -1, -1, -1, 176, 176, 176, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, 176, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, -1, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 184, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, -1, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 188, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 190, 191, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 192, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 238, 237, 237, 237, 237, 237 },
			{ -1, 190, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, 424, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, 195, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, 424, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, -1, -1, 199, 199, -1, 199, 199, 199, 199, 199, 199, 199, 199, -1, -1, 199, 199, -1, -1, -1, -1, 199, -1, -1, -1, 199, 199, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, 199, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, -1, -1, 201, 201, -1, 201, 201, 201, 201, 201, 201, 201, 201, -1, -1, 201, 201, -1, -1, -1, -1, 201, -1, -1, -1, 201, 201, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, 201, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 431, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, -1, 215, 213, 213, -1, 213, 213, 213, 213, 213, 213, 213, 213, 435, -1, 213, 213, -1, -1, -1, -1, 213, -1, -1, -1, 213, 213, 213, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 213, 213, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, 170, -1, -1, -1, -1, -1, 170, -1, 410, -1, 170, 170, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 22, 465, 658, 658, 658, 466, 658, -1, -1, 658, 658, -1, 590, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 265, 300, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 302, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, 33, -1, -1, -1, -1, -1, 268, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 12, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 265, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, -1, -1, -1, -1, -1, -1, 306, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, 227, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 99, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1, -1, 153, 153, -1, 153, 153, 153, 153, 153, 153, 153, 153, -1, -1, 153, -1, -1, -1, -1, -1, 153, -1, -1, -1, 153, 153, 153, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 157, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, -1 },
			{ -1, -1, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, -1, 165, 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, -1, -1, 165, -1, -1, -1, -1, -1, 165, -1, -1, -1, 165, 165, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 166, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, -1, -1, 173, 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, -1, -1, 173, -1, -1, -1, -1, -1, 173, -1, -1, -1, 173, 173, 173, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 420, 420, 420, 420, 420, 420, 420, 420, 420, 420, 420, 420, -1, 180, 420, 420, -1, 420, 420, 420, 420, 420, 420, 420, 420, -1, -1, 420, 420, -1, -1, -1, -1, 420, -1, -1, -1, 420, 420, 420, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 180, 420, 420, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, -1, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, -1, 237, 237, 237, 237, 237 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 192, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, 53, -1, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, -1, -1, -1, -1, -1, 53, -1, -1, -1, 53, 53, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 425, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 426, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, 424, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 241, -1, -1, -1, 241, 241, -1, -1, 241, -1, -1, -1, -1, -1, -1, -1, -1, -1, 241, -1, -1, 241, -1, -1, -1, -1, -1, -1, 241, -1, -1, -1, -1, -1, -1, -1, -1, -1, 425, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 241, 241, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, 206, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 212, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 253, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, -1, -1, 201, 201, -1, 201, 201, 201, 201, 201, 201, 201, 201, -1, -1, 201, -1, -1, -1, -1, -1, 201, -1, 429, -1, 201, 201, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 468, 658, 273, 658, 658, 658, 658, 658, 658, 23, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, -1, -1, -1, -1, -1, -1, -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, 249, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 426, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 251, 251, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, -1, -1, 213, 213, -1, 213, 213, 213, 213, 213, 213, 213, 213, -1, -1, 213, -1, -1, -1, -1, -1, 213, -1, -1, -1, 213, 213, 213, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 469, 658, 658, 658, 24, 593, 658, 276, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 8, 9, 439, 223, 450, 247, 255, 453, 455, 589, 258, 618, 638, 10, 222, 649, 654, 11, 656, 261, 658, 659, 264, 658, 660, 661, 246, 254, 658, 12, 13, 257, 14, 260, 457, 263, 11, 222, 658, 662, 658, 222, 266, 269, 272, 275, 278, 281, 284, 287, 290, 293, 222, 15, 16, 225, 12, 11, 222, 17, 18, 296, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 25, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 55, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, 271, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, 227, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 285, 658, 658, 658, 27, 592, -1, -1, 658, 658, -1, 658, 658, 658, 658, 619, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 274, -1, 277, 280, -1, 441, -1, 283, 286, 289, -1, -1, -1, -1, -1, -1, 292, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 298, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 596, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, 262, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 304, -1, -1, 249, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, 249, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 224, 36, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 56, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 12, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 57, 658, -1, 658, 488, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 58, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 442, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 59, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 60, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 61, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 62, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 65, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 73, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 451, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 74, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, 444, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 75, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 76, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, -1, -1, -1, 71, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, 71, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 77, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, 72, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 78, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, 249, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 79, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, 227, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 81, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 326, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 82, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 445, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 83, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 84, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 85, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 86, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 336, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 87, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 88, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 338, 338, 338, 338, 338, 338, 338, 338, 338, 338, 338, 338, -1, -1, 338, 338, -1, 338, 338, 338, 338, 338, 338, 338, 338, -1, -1, 338, -1, -1, -1, -1, -1, 338, -1, 322, -1, 338, 338, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 340, -1, 448 },
			{ -1, -1, -1, 89, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, 98, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 90, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 452, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 91, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 449, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 92, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 93, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 94, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 95, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 454, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 96, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 338, 338, 338, 338, 338, 338, 338, 338, 338, 338, 338, 338, -1, -1, 338, 338, 99, 338, 338, 338, 338, 338, 338, 338, 338, -1, -1, 338, 338, -1, -1, -1, -1, 338, -1, -1, -1, 338, 338, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 338, 338, 229, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 97, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, -1, -1, 358, 358, -1, 358, 358, 358, 358, 358, 358, 358, 358, -1, -1, 358, -1, -1, -1, -1, -1, 358, -1, -1, -1, 358, 358, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 100, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 101, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, 98, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 102, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 103, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 104, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 105, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 352, 107, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 106, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 109, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, 108, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 110, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, -1, -1, 358, 358, -1, 358, 358, 358, 358, 358, 358, 358, 358, -1, -1, 358, 358, -1, -1, -1, -1, 358, -1, -1, -1, 358, 358, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, 358, -1, -1, -1, 382, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 111, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 360, 360, 360, 360, 360, 360, 360, 360, 360, 360, 360, 360, -1, -1, 360, 360, -1, 360, 360, 360, 360, 360, 360, 360, 360, -1, -1, 360, 360, -1, -1, -1, -1, 360, -1, -1, -1, 360, 360, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, 360, -1, -1, -1, -1, -1, 382 },
			{ -1, -1, -1, 112, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 456, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 113, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 458, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 114, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 115, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 368, 117, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 116, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 384, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 119, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 386, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 120, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, 118, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 121, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 386, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 122, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 388, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 123, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, 108, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 124, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 99, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 229, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 127, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, 125, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 128, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 386, 126, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 129, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 130, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 392, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 131, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 140, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 132, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 1, 147, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 396, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 149, 148, 148, 148, 148, 148, 148, 150, 230, 148 },
			{ -1, -1, -1, 658, 658, 658, 658, 133, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 134, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 1, 155, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 157, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 400, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 231, 156, 156, 156, 156, 158 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 135, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159 },
			{ -1, -1, -1, 136, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 1, 155, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 404, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 161, 160, 160, 160, 160, 160, 162, 160, 232, 160 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 137, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 138, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 219, 155, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 168, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 408, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 169, 167, 167, 167, 233, 167, 167, 167, 234, 167 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 139, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 141, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 142, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 143, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 175, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 235, 235, 176, 176, 11, 176, 176, 176, 176, 176, 176, 176, 176, 250, 235, 176, 235, 235, 235, 235, 235, 176, 235, 11, 235, 176, 176, 176, 235, 235, 235, 235, 235, 235, 235, 235, 235, 235, 235, 235, 235, 235, 235, 235, 11, 235, 235, 235, 235, 235 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 144, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 145, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 179, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 179, 179, 236, 236, 179, 236, 236, 236, 236, 236, 236, 236, 236, 179, 179, 236, 179, 179, 179, 179, 179, 236, 179, 179, 179, 236, 236, 236, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 146, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ 1, 181, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182 },
			{ 1, 185, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 187, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186 },
			{ 1, 7, 194, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 13, 194, 658, 658, 13, 658, 658, 658, 658, 658, 658, 658, 658, 194, 194, 658, 195, 13, 194, 13, 194, 658, 194, 13, 194, 658, 658, 658, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 240, 195, 13, 196, 194, 194, 239, 13 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 241, -1, -1, -1, 241, 241, -1, -1, 241, -1, -1, -1, -1, -1, -1, -1, -1, -1, 241, -1, -1, 241, -1, -1, -1, -1, -1, -1, 241, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 241, 241, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 251, 251, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 198, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 198, 198, 199, 199, 198, 199, 199, 199, 199, 199, 199, 199, 199, 198, 198, 199, 198, 198, 198, 198, 198, 199, 198, 198, 198, 199, 199, 199, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198 },
			{ 245, 155, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200 },
			{ 1, 7, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 203, 202, 202, 202, 204, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 205, 204, 202, 202, 202, 202, 202, 242, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 204, 202, 202, 202, 202, 202 },
			{ 1, 7, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 203, 202, 202, 202, 204, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 204, 208, 202, 202, 202, 202, 242, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 204, 202, 202, 202, 202, 202 },
			{ 1, 7, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 203, 202, 202, 202, 204, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 204, 202, 202, 202, 202, 209, 242, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 204, 202, 202, 202, 202, 202 },
			{ 1, 7, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 243, 210, 210, 210, 210, 210, 210, 211, 252, 210 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 216, 216, -1, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, -1, -1, 216, 216, -1, 216, 216, 216, 216, 216, 216, 216, 216, -1, -1, 216, -1, -1, -1, -1, -1, 216, -1, -1, -1, 216, 216, 216, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 243, 210, 210, 210, 210, 210, 217, 210, 252, 210 },
			{ 1, 1, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 243, 210, 210, 210, 210, 210, 210, 210, 252, 210 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 267, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 312, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 360, 360, 360, 360, 360, 360, 360, 360, 360, 360, 360, 360, -1, -1, 360, 360, -1, 360, 360, 360, 360, 360, 360, 360, 360, -1, -1, 360, -1, -1, -1, -1, -1, 360, -1, -1, -1, 360, 360, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 368, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 270, -1, -1, 658, 658, -1, 658, 658, 467, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 334, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 279, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 470, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 591, 658, 658, 658, 282, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 390, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 288, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 291, 621, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 484, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 294, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 297, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 485, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 299, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 639, 658, 658, 658, 658, 486, 658, 597, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 487, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 489, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 594, 658, 658, 623, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 490, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 650, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 494, 658, 658, -1, -1, 658, 658, -1, 658, 495, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 496, 658, 658, 658, 658, 658, 658, 301, 658, -1, -1, 658, 667, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 622, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 640, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 497, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 600, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 498, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 303, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 499, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 305, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 598, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 641, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 504, 658, 658, 658, 658, 658, 658, 601, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 505, 506, 507, 508, 658, 651, 658, 658, 658, -1, -1, 658, 603, -1, 509, 658, 604, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 307, 658, 605, 511, 658, 658, 658, 658, 512, 658, -1, -1, 658, 658, -1, 658, 658, 658, 513, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 309, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 624, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 311, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 313, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 315, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 317, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 625, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 319, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 627, 658, 658, 658, 658, 658, 658, 321, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 323, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 325, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 327, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 518, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 329, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 331, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 333, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 626, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 335, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 337, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 339, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 655, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 657, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 521, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 522, 658, 658, 658, 658, 606, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 523, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 525, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 526, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 341, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 528, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 610, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 608, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 531, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 535, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 345, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 347, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 349, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 351, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 353, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 539, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 540, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 612, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 541, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 542, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 355, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 544, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 614, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 546, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 357, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 548, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 359, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 361, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 365, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 615, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 551, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 367, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 369, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 371, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 553, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 634, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 555, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 556, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 636, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 373, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 558, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 559, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 560, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 375, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 377, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 379, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 381, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 383, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 385, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 568, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 569, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 387, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 389, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 391, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 573, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 574, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 393, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 395, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 397, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 668, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 575, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 399, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 576, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 616, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 401, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 403, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 577, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 405, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 407, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 578, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 409, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 580, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 583, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 584, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 411, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 413, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 415, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 585, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 586, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 417, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 587, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 588, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 419, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 620, 658, 658, -1, -1, 658, 471, -1, 658, 472, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 599, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 492, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 500, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 491, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 642, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 502, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 503, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 514, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 519, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 629, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 643, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 645, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 529, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 630, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 607, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 527, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 666, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 543, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 547, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 635, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 545, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 550, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 613, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 566, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 557, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 562, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 579, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 581, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 473, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 474, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 501, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 493, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 510, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 516, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 628, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 530, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 632, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 631, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 611, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 533, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 633, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 552, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 549, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 554, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 567, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 563, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 570, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 582, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 475, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 602, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 517, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 520, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 532, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 537, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 534, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 609, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 561, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 564, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 571, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 476, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 515, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 524, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 536, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 565, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 477, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 538, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 478, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 664, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 595, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 479, 658, 658, -1, -1, 658, 480, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 481, 658, 658, 658, 482, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 483, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 646, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 647, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 617, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 653, 658, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 652, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 658, 658, 658, 658, 658, 658, 658, 658, 658, 637, 658, 658, -1, -1, 658, 658, -1, 658, 658, 658, 658, 658, 658, 658, 658, -1, -1, 658, 658, -1, -1, -1, -1, 658, -1, -1, -1, 658, 658, 658, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 658, 658, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  256,
			  394,
			  398,
			  402,
			  406,
			  412,
			  414,
			  418,
			  421,
			  422,
			  189,
			  423,
			  427,
			  428,
			  430,
			  432,
			  433,
			  434,
			  437,
			  438
		};
		
		#endregion
		
		private Tokens NextToken()
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 669);
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

