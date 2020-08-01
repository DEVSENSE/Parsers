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
					// #line 723
					{
						yymore(); break;
					}
					break;
					
				case 4:
					// #line 710
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
					// #line 690
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
					// #line 700
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
					// #line 598
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 744
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 283
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 11:
					// #line 638
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 12:
					// #line 924
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 13:
					// #line 306
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 14:
					// #line 748
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 15:
					// #line 603
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 611
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 900
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 890
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 839
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 314
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 764
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 162
					{
						return (Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 186
					{
						return (Tokens.T_DO);
					}
					break;
					
				case 24:
					// #line 122
					{
						return (Tokens.T_FN);
					}
					break;
					
				case 25:
					// #line 578
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 26:
					// #line 218
					{
						return (Tokens.T_AS);
					}
					break;
					
				case 27:
					// #line 482
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 28:
					// #line 278
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 29:
					// #line 518
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 30:
					// #line 594
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 31:
					// #line 510
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 32:
					// #line 654
					{
						return ProcessRealNumber();
					}
					break;
					
				case 33:
					// #line 302
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 34:
					// #line 538
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 35:
					// #line 754
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 36:
					// #line 534
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 37:
					// #line 526
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 38:
					// #line 522
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 39:
					// #line 462
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 40:
					// #line 494
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 41:
					// #line 514
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 42:
					// #line 478
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 43:
					// #line 498
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 44:
					// #line 506
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 45:
					// #line 590
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 46:
					// #line 542
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 47:
					// #line 554
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 48:
					// #line 574
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 49:
					// #line 558
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 50:
					// #line 570
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 51:
					// #line 562
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 52:
					// #line 769
					{
						return ProcessVariable();
					}
					break;
					
				case 53:
					// #line 566
					{
						return (Tokens.T_COALESCE_EQUAL);
					}
					break;
					
				case 54:
					// #line 586
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 55:
					// #line 146
					{
						return (Tokens.T_TRY);
					}
					break;
					
				case 56:
					// #line 118
					{
						return (Tokens.T_EXIT);
					}
					break;
					
				case 57:
					// #line 190
					{
						return (Tokens.T_FOR);
					}
					break;
					
				case 58:
					// #line 318
					{
						return (Tokens.T_NEW);
					}
					break;
					
				case 59:
					// #line 382
					{
						return (Tokens.T_USE);
					}
					break;
					
				case 60:
					// #line 582
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 61:
					// #line 550
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 62:
					// #line 310
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 63:
					// #line 326
					{
						return (Tokens.T_VAR);
					}
					break;
					
				case 64:
					// #line 530
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 65:
					// #line 486
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 66:
					// #line 490
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 67:
					// #line 502
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 68:
					// #line 546
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 69:
					// #line 642
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 70:
					// #line 634
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 71:
					// #line 114
					{ 
						return (Tokens.T_EXIT);
					}
					break;
					
				case 72:
					// #line 250
					{
						return (Tokens.T_ECHO);
					}
					break;
					
				case 73:
					// #line 174
					{
						return (Tokens.T_ELSE);
					}
					break;
					
				case 74:
					// #line 358
					{
						return (Tokens.T_EVAL);
					}
					break;
					
				case 75:
					// #line 230
					{
						return (Tokens.T_CASE);
					}
					break;
					
				case 76:
					// #line 466
					{
						return (Tokens.T_LIST);
					}
					break;
					
				case 77:
					// #line 246
					{
						return (Tokens.T_GOTO);
					}
					break;
					
				case 78:
					// #line 759
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 79:
					// #line 170
					{
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 80:
					// #line 398
					{
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 81:
					// #line 394
					{
						return (Tokens.T_ISSET);
					}
					break;
					
				case 82:
					// #line 266
					{
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 83:
					// #line 158
					{
						return (Tokens.T_THROW);
					}
					break;
					
				case 84:
					// #line 442
					{
						return (Tokens.T_FINAL);
					}
					break;
					
				case 85:
					// #line 458
					{
						return (Tokens.T_UNSET);
					}
					break;
					
				case 86:
					// #line 130
					{
						return (Tokens.T_CONST);
					}
					break;
					
				case 87:
					// #line 322
					{
						return (Tokens.T_CLONE);
					}
					break;
					
				case 88:
					// #line 258
					{
						return (Tokens.T_CLASS);
					}
					break;
					
				case 89:
					// #line 150
					{
						return (Tokens.T_CATCH);
					}
					break;
					
				case 90:
					// #line 142
					{
						return (Tokens.T_YIELD);
					}
					break;
					
				case 91:
					// #line 470
					{
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 92:
					// #line 178
					{
						return (Tokens.T_WHILE);
					}
					break;
					
				case 93:
					// #line 238
					{
						return (Tokens.T_BREAK);
					}
					break;
					
				case 94:
					// #line 254
					{
						return (Tokens.T_PRINT);
					}
					break;
					
				case 95:
					// #line 330
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 96:
					// #line 773
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
						_tokenSemantics.Object = _hereDocLabel = GetTokenSubstring(s, length);
						//
						return Tokens.T_START_HEREDOC;
					}
					break;
					
				case 97:
					// #line 194
					{
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 98:
					// #line 166
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 99:
					// #line 434
					{
						return (Tokens.T_STATIC);
					}
					break;
					
				case 100:
					// #line 222
					{
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 101:
					// #line 134
					{
						return (Tokens.T_RETURN);
					}
					break;
					
				case 102:
					// #line 390
					{
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 103:
					// #line 454
					{
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 104:
					// #line 334
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 105:
					// #line 350
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 106:
					// #line 270
					{
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 107:
					// #line 362
					{
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 108:
					// #line 234
					{
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 109:
					// #line 206
					{
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 110:
					// #line 154
					{
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 111:
					// #line 198
					{
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 112:
					// #line 370
					{
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 113:
					// #line 446
					{
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 114:
					// #line 354
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 115:
					// #line 342
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 116:
					// #line 682
					{
						return (Tokens.T_DIR);
					}
					break;
					
				case 117:
					// #line 182
					{
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 118:
					// #line 126
					{
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 119:
					// #line 242
					{
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 120:
					// #line 474
					{
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 121:
					// #line 438
					{
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 122:
					// #line 346
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 123:
					// #line 338
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 124:
					// #line 678
					{
						return (Tokens.T_FILE);
					}
					break;
					
				case 125:
					// #line 674
					{
						return (Tokens.T_LINE);
					}
					break;
					
				case 126:
					// #line 226
					{
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 127:
					// #line 262
					{
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 128:
					// #line 386
					{
						return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 129:
					// #line 378
					{
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 130:
					// #line 450
					{
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 131:
					// #line 662
					{
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 132:
					// #line 658
					{
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 133:
					// #line 210
					{
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 134:
					// #line 202
					{
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 135:
					// #line 214
					{
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 136:
					// #line 274
					{
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 137:
					// #line 138
					{
						return Tokens.T_YIELD_FROM;
					}
					break;
					
				case 138:
					// #line 670
					{
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 139:
					// #line 366
					{
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 140:
					// #line 374
					{
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 141:
					// #line 666
					{
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 142:
					// #line 686
					{
						return (Tokens.T_NS_C);
					}
					break;
					
				case 143:
					// #line 417
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 144:
					// #line 85
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 145:
					// #line 898
					{ yymore(); break; }
					break;
					
				case 146:
					// #line 896
					{ yymore(); break; }
					break;
					
				case 147:
					// #line 894
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 148:
					// #line 897
					{ yymore(); break; }
					break;
					
				case 149:
					// #line 893
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 150:
					// #line 892
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 151:
					// #line 891
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 152:
					// #line 82
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 153:
					// #line 842
					{ yymore(); break; }
					break;
					
				case 154:
					// #line 841
					{ yymore(); break; }
					break;
					
				case 155:
					// #line 843
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 156:
					// #line 840
					{ yymore(); break; }
					break;
					
				case 157:
					// #line 908
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 906
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 904
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 160:
					// #line 907
					{ yymore(); break; }
					break;
					
				case 161:
					// #line 903
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 162:
					// #line 902
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 163:
					// #line 901
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 164:
					// #line 917
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 916
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 914
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 828
					{
					    if(!string.IsNullOrEmpty(_hereDocLabel) && VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(_processDoubleQuotedString) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 168:
					// #line 915
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 912
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 170:
					// #line 911
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 171:
					// #line 910
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 172:
					// #line 296
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 173:
					// #line 291
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 174:
					// #line 287
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 175:
					// #line 627
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 176:
					// #line 619
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 177:
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
					
				case 178:
					// #line 760
					{ yymore(); break; }
					break;
					
				case 179:
					// #line 762
					{ yymore(); break; }
					break;
					
				case 180:
					// #line 761
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 181:
					// #line 97
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 182:
					// #line 755
					{ yymore(); break; }
					break;
					
				case 183:
					// #line 757
					{ yymore(); break; }
					break;
					
				case 184:
					// #line 756
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 185:
					// #line 922
					{ yymore(); break; }
					break;
					
				case 186:
					// #line 101
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 187:
					// #line 921
					{ yymore(); break; }
					break;
					
				case 188:
					// #line 919
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 189:
					// #line 920
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 190:
					// #line 732
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 191:
					// #line 646
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 192:
					// #line 727
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 193:
					// #line 650
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 194:
					// #line 815
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 195:
					// #line 804
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						if (GetTokenSpan().TrimLeft() != _hereDocLabel)
						{
							_errors.Error(_tokenPosition, Devsense.PHP.Errors.FatalErrors.SyntaxError, "Incorrect heredoc end label: " + GetTokenSpan().Trim().ToString());
						}
						_yyless(LabelTrailLength());
						_tokenSemantics.Object = _hereDocLabel;
						return Tokens.T_END_HEREDOC;
					}
					break;
					
				case 196:
					// #line 837
					{ yymore(); break; }
					break;
					
				case 197:
					// #line 819
					{
					    if(!string.IsNullOrEmpty(_hereDocLabel) && VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(null) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 198:
					// #line 429
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 199:
					// #line 423
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 200:
					// #line 402
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 201:
					// #line 426
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 202:
					// #line 427
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 203:
					// #line 425
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 204:
					// #line 407
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 205:
					// #line 412
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 206:
					// #line 884
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 207:
					// #line 873
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 208:
					// #line 867
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 209:
					// #line 862
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 210:
					// #line 845
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 211:
					// #line 856
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 212:
					// #line 850
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 213:
					// #line 878
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 216: goto case 3;
				case 217: goto case 6;
				case 218: goto case 8;
				case 219: goto case 9;
				case 220: goto case 11;
				case 221: goto case 21;
				case 222: goto case 32;
				case 223: goto case 43;
				case 224: goto case 96;
				case 225: goto case 146;
				case 226: goto case 154;
				case 227: goto case 158;
				case 228: goto case 165;
				case 229: goto case 166;
				case 230: goto case 172;
				case 231: goto case 175;
				case 232: goto case 185;
				case 233: goto case 188;
				case 234: goto case 190;
				case 235: goto case 191;
				case 236: goto case 193;
				case 237: goto case 198;
				case 238: goto case 206;
				case 241: goto case 8;
				case 242: goto case 9;
				case 243: goto case 21;
				case 244: goto case 32;
				case 245: goto case 193;
				case 246: goto case 206;
				case 248: goto case 8;
				case 249: goto case 9;
				case 251: goto case 8;
				case 252: goto case 9;
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
				case 293: goto case 9;
				case 295: goto case 9;
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
				case 430: goto case 9;
				case 441: goto case 9;
				case 444: goto case 9;
				case 446: goto case 9;
				case 448: goto case 9;
				case 450: goto case 9;
				case 451: goto case 9;
				case 452: goto case 9;
				case 453: goto case 9;
				case 454: goto case 9;
				case 455: goto case 9;
				case 456: goto case 9;
				case 457: goto case 9;
				case 458: goto case 9;
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
			AcceptConditions.AcceptOnStart, // 167
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
			AcceptConditions.Accept, // 190
			AcceptConditions.Accept, // 191
			AcceptConditions.Accept, // 192
			AcceptConditions.Accept, // 193
			AcceptConditions.Accept, // 194
			AcceptConditions.Accept, // 195
			AcceptConditions.Accept, // 196
			AcceptConditions.AcceptOnStart, // 197
			AcceptConditions.Accept, // 198
			AcceptConditions.Accept, // 199
			AcceptConditions.Accept, // 200
			AcceptConditions.Accept, // 201
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
			AcceptConditions.NotAccept, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.Accept, // 216
			AcceptConditions.Accept, // 217
			AcceptConditions.Accept, // 218
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
			AcceptConditions.NotAccept, // 239
			AcceptConditions.Accept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.Accept, // 244
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
			AcceptConditions.NotAccept, // 294
			AcceptConditions.Accept, // 295
			AcceptConditions.NotAccept, // 296
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
			AcceptConditions.NotAccept, // 413
			AcceptConditions.NotAccept, // 414
			AcceptConditions.NotAccept, // 415
			AcceptConditions.NotAccept, // 416
			AcceptConditions.NotAccept, // 417
			AcceptConditions.NotAccept, // 418
			AcceptConditions.NotAccept, // 419
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
			AcceptConditions.Accept, // 430
			AcceptConditions.NotAccept, // 431
			AcceptConditions.NotAccept, // 432
			AcceptConditions.NotAccept, // 433
			AcceptConditions.NotAccept, // 434
			AcceptConditions.NotAccept, // 435
			AcceptConditions.NotAccept, // 436
			AcceptConditions.NotAccept, // 437
			AcceptConditions.NotAccept, // 438
			AcceptConditions.NotAccept, // 439
			AcceptConditions.NotAccept, // 440
			AcceptConditions.Accept, // 441
			AcceptConditions.NotAccept, // 442
			AcceptConditions.NotAccept, // 443
			AcceptConditions.Accept, // 444
			AcceptConditions.NotAccept, // 445
			AcceptConditions.Accept, // 446
			AcceptConditions.NotAccept, // 447
			AcceptConditions.Accept, // 448
			AcceptConditions.NotAccept, // 449
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
		};
		
		private static int[] colMap = new int[]
		{
			30, 30, 30, 30, 30, 30, 30, 30, 30, 36, 17, 30, 30, 59, 30, 30, 
			30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 
			36, 47, 62, 44, 63, 49, 50, 64, 35, 37, 43, 46, 53, 26, 33, 42, 
			57, 58, 29, 29, 29, 29, 29, 29, 29, 29, 31, 41, 48, 45, 27, 2, 
			53, 19, 22, 11, 7, 3, 8, 24, 20, 5, 38, 23, 16, 18, 9, 12, 
			25, 40, 14, 13, 6, 10, 34, 21, 4, 15, 28, 56, 32, 60, 52, 39, 
			61, 19, 22, 11, 7, 3, 8, 24, 20, 5, 38, 23, 16, 18, 9, 12, 
			25, 40, 14, 13, 6, 10, 34, 21, 4, 15, 28, 54, 51, 55, 53, 30, 
			0, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 1, 2, 1, 1, 1, 3, 4, 5, 6, 1, 1, 1, 1, 
			1, 1, 1, 1, 7, 8, 9, 9, 9, 9, 9, 1, 1, 1, 10, 1, 
			11, 1, 1, 12, 1, 13, 1, 1, 14, 1, 1, 15, 16, 17, 1, 1, 
			1, 1, 1, 1, 18, 1, 9, 9, 9, 19, 9, 9, 9, 1, 1, 9, 
			1, 1, 1, 1, 1, 20, 21, 9, 9, 22, 9, 9, 9, 9, 23, 9, 
			9, 9, 9, 9, 24, 9, 9, 9, 9, 9, 25, 9, 9, 9, 9, 1, 
			1, 26, 9, 9, 9, 9, 9, 9, 1, 1, 9, 27, 9, 9, 9, 9, 
			28, 9, 1, 1, 9, 9, 9, 9, 9, 9, 1, 1, 9, 9, 9, 9, 
			9, 9, 9, 9, 9, 9, 9, 9, 9, 1, 9, 9, 9, 9, 9, 9, 
			1, 29, 30, 1, 1, 1, 1, 1, 1, 31, 31, 1, 1, 32, 33, 1, 
			1, 1, 1, 1, 34, 1, 35, 36, 1, 1, 1, 1, 1, 37, 1, 1, 
			1, 1, 38, 39, 1, 1, 40, 41, 1, 42, 1, 43, 1, 1, 1, 44, 
			1, 45, 1, 46, 1, 47, 1, 48, 1, 1, 49, 50, 1, 1, 1, 1, 
			1, 51, 1, 1, 1, 1, 52, 53, 54, 55, 56, 57, 58, 1, 59, 1, 
			60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 
			76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 
			92, 1, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 
			107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 
			123, 90, 70, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 
			137, 138, 139, 140, 141, 142, 143, 144, 23, 145, 146, 147, 148, 149, 150, 151, 
			152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 
			168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 
			184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 
			200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 
			216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 
			232, 233, 53, 234, 235, 236, 237, 238, 239, 240, 67, 241, 242, 243, 244, 245, 
			246, 247, 248, 249, 76, 250, 50, 251, 252, 253, 254, 255, 256, 257, 258, 259, 
			260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 
			276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 
			292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 
			308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 
			324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 
			340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 
			356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 
			372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 
			388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 
			404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 
			420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 
			436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 
			452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 
			468, 469, 470, 471, 472, 9, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 
			483
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 216, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 450, 645, 645, 645, 645, 451, 645, 452, 645, 645, 645, 645, 453, -1, 454, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 455, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 32, -1, -1, -1, -1, -1, 259, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 243, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, 222, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 312, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, -1, 52, 52, 52, 52, 52, 52, 52, 52, -1, -1, 52, 52, -1, -1, -1, -1, 52, -1, -1, -1, 52, 52, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, 52, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 632, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 69, -1, -1, -1, 69, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, 69, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 294, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, 70, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 335, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 355, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 354, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, 354, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, 354, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 636, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 561, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 653, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, -1, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, -1, 145, 145, 145, 145, 145, 145, 145, -1, -1, 145 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 149, -1 },
			{ -1, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1 },
			{ -1, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, 157, 157, 157, 157, 157, 157, -1, 157, -1, 157 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 161, -1 },
			{ -1, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, -1, 164, 164, 164, -1, 164 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1 },
			{ -1, -1, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, -1, -1, 167, 167, -1, -1, -1, -1, 167, -1, -1, -1, 167, 167, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, 167, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, -1, -1, 173, 173, -1, -1, -1, -1, 173, -1, -1, -1, 173, 173, 173, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 173, 173, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, -1, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 180, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, -1, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 184, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 186, 187, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 188, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 233, 232, 232, 232, 232, 232 },
			{ -1, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, 191, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, 193, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, -1, 195, 195, 195, 195, 195, 195, 195, 195, -1, -1, 195, 195, -1, -1, -1, -1, 195, -1, -1, -1, 195, 195, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, 195, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, -1, 197, 197, 197, 197, 197, 197, 197, 197, -1, -1, 197, 197, -1, -1, -1, -1, 197, -1, -1, -1, 197, 197, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 422, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, 426, -1, 209, 209, -1, -1, -1, -1, 209, -1, -1, -1, 209, 209, 209, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, 209, 209, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 239, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, -1, -1, 167, -1, -1, -1, -1, -1, 167, -1, 402, -1, 167, 167, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 22, 456, 645, 645, 645, 457, 645, 645, 645, -1, 579, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 256, 292, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 294, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 32, -1, -1, -1, -1, -1, 259, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, 298, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, 222, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 96, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, 150, 150, -1, -1, 150, -1, -1, -1, -1, -1, 150, -1, -1, -1, 150, 150, 150, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 151, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 154, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1 },
			{ -1, -1, -1, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, -1, 162, 162, 162, 162, 162, 162, 162, 162, -1, -1, 162, -1, -1, -1, -1, -1, 162, -1, -1, -1, 162, 162, 162, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 163, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, 170, -1, -1, -1, -1, -1, 170, -1, -1, -1, 170, 170, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 410, 410, 410, 410, 410, 410, 410, 410, 410, 410, 410, 410, 410, 410, -1, 410, 410, 410, 410, 410, 410, 410, 410, -1, -1, 410, 410, -1, -1, -1, -1, 410, -1, -1, -1, 410, 410, 410, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, 176, 410, 410, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, -1, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, -1, 232, 232, 232, 232, 232 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 188, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, -1, 52, 52, 52, 52, 52, 52, 52, 52, -1, -1, 52, -1, -1, -1, -1, -1, 52, -1, -1, -1, 52, 52, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 416, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, 193, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 236, -1, -1, -1, 236, 236, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, 236, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, -1, -1, 416, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, 236, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, 202, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 247, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, -1, 197, 197, 197, 197, 197, 197, 197, 197, -1, -1, 197, -1, -1, -1, -1, -1, 197, -1, 420, -1, 197, 197, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 459, 645, 267, 645, 645, 645, 645, 645, 645, 23, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, -1, -1, -1, -1, -1, -1, -1, -1, -1, 296, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, 244, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, 245, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, -1, -1, 209, -1, -1, -1, -1, -1, 209, -1, -1, -1, 209, 209, 209, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 460, 645, 645, 645, 24, 582, 645, 270, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 8, 9, 430, 219, 441, 242, 249, 444, 446, 578, 252, 607, 626, 637, 643, 10, 645, 255, 645, 647, 258, 645, 648, 649, 218, 241, 645, 11, 12, 248, 13, 251, 448, 254, 10, 257, 645, 650, 645, 257, 260, 263, 14, 266, 269, 272, 275, 278, 281, 284, 287, 257, 15, 16, 257, 220, 11, 10, 257, 17, 18, 290, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, 262, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, 222, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 25, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 431, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 265, -1, 268, 271, -1, 432, -1, 274, 277, 280, -1, -1, -1, -1, 283, -1, -1, 286, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 279, 645, 645, 645, 26, 581, 645, 645, -1, 645, 645, 645, 645, 608, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 296, -1, -1, 244, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 296, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, 244, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 585, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, 253, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 35, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 54, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 55, 645, -1, 645, 479, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 433, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 56, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 300, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 57, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 302, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 58, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 306, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 59, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 434, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 60, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 63, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 442, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 71, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 310, -1, -1, -1, -1, -1, -1, 435, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 72, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 69, -1, -1, -1, 69, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, 69, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 74, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, 70, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 75, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, 244, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 76, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, 222, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 77, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 79, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 80, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 81, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 437, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 82, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 83, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 84, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 85, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, -1, 330, 330, 330, 330, 330, 330, 330, 330, -1, -1, 330, -1, -1, -1, -1, -1, 330, -1, 314, -1, 330, 330, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 332, -1, 439 },
			{ -1, -1, -1, 645, 645, 645, 86, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 334, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 336, 95, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 87, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 88, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 89, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 90, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 91, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 92, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 445, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 93, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 96, 330, 330, 330, 330, 330, 330, 330, 330, -1, -1, 330, 330, -1, -1, -1, -1, 330, -1, -1, -1, 330, 330, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, 330, 224, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 94, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, -1, 350, 350, 350, 350, 350, 350, 350, 350, -1, -1, 350, -1, -1, -1, -1, -1, 350, -1, -1, -1, 350, 350, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 97, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 98, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 336, 95, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 99, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 100, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 101, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 102, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 103, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 106, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, 105, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 107, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, -1, 350, 350, 350, 350, 350, 350, 350, 350, -1, -1, 350, 350, -1, -1, -1, -1, 350, -1, -1, -1, 350, 350, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 350, 350, -1, -1, -1, 374, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 108, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, -1, 352, 352, 352, 352, 352, 352, 352, 352, -1, -1, 352, 352, -1, -1, -1, -1, 352, -1, -1, -1, 352, 352, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 352, 352, -1, -1, -1, -1, -1, 374 },
			{ -1, -1, -1, 109, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, -1, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 354, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 110, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 449, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 111, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 112, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, 114, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 113, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 116, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 117, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, 115, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 118, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 119, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 120, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, 105, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 121, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 96, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 224, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 124, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 376, 122, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 125, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, 123, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 126, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 127, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 128, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 137, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 129, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 1, 144, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 388, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 146, 145, 145, 145, 145, 145, 145, 145, 147, 225, 145 },
			{ -1, -1, -1, 645, 645, 645, 645, 130, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 131, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 1, 152, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 154, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 392, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 226, 153, 153, 153, 153, 155 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 132, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156 },
			{ -1, -1, -1, 133, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 1, 152, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 396, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 158, 157, 157, 157, 157, 157, 157, 159, 157, 227, 157 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 134, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 135, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 215, 152, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 165, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 400, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 166, 164, 164, 164, 164, 228, 164, 164, 164, 229, 164 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 136, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 138, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 139, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 140, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 172, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 10, 173, 173, 173, 173, 173, 173, 173, 173, 230, 172, 173, 172, 172, 172, 172, 172, 173, 172, 10, 172, 173, 173, 173, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 10, 172, 172, 172, 172, 172 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 141, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 175, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 175, 231, 231, 231, 231, 231, 231, 231, 231, 175, 175, 231, 175, 175, 175, 175, 175, 231, 175, 175, 175, 231, 231, 231, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 142, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 143, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ 1, 177, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178 },
			{ 1, 181, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182 },
			{ 1, 7, 190, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 12, 645, 645, 645, 645, 645, 645, 645, 645, 190, 190, 645, 191, 12, 190, 12, 190, 645, 190, 12, 190, 645, 645, 645, 190, 190, 190, 12, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 235, 191, 12, 192, 190, 190, 234, 12 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, 193, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 236, -1, -1, -1, 236, 236, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, 236, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, 236, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, 245, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 194, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 194, 195, 195, 195, 195, 195, 195, 195, 195, 194, 194, 195, 194, 194, 194, 194, 194, 195, 194, 194, 194, 195, 195, 195, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194 },
			{ 240, 152, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196 },
			{ 1, 7, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 200, 199, 198, 198, 198, 198, 198, 237, 198, 201, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198 },
			{ 1, 7, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 204, 198, 198, 198, 198, 237, 198, 201, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198 },
			{ 1, 7, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 205, 237, 198, 201, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198 },
			{ 1, 7, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 238, 206, 206, 206, 206, 206, 206, 206, 207, 246, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 427, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 212, 212, -1, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, -1, 212, 212, 212, 212, 212, 212, 212, 212, -1, -1, 212, -1, -1, -1, -1, -1, 212, -1, -1, -1, 212, 212, 212, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 238, 206, 206, 206, 206, 206, 206, 213, 206, 246, 206 },
			{ 1, 1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 238, 206, 206, 206, 206, 206, 206, 206, 206, 246, 206 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 261, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 438, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, 352, -1, 352, 352, 352, 352, 352, 352, 352, 352, -1, -1, 352, -1, -1, -1, -1, -1, 352, -1, -1, -1, 352, 352, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 264, 645, 645, -1, 645, 645, 458, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 326, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 273, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 461, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 368, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 580, 645, 645, 645, 276, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 382, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 282, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 336, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 285, 610, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 474, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 288, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 291, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 475, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 293, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 627, 645, 645, 645, 645, 476, 645, 477, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 478, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 480, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 583, 645, 645, 611, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 481, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 638, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 485, 645, 645, 645, 645, -1, 645, 486, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 487, 645, 645, 645, 645, 645, 645, 295, 645, 645, 655, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 588, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 612, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 488, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 589, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 489, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 297, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 299, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 586, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 628, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 493, 645, 645, 645, 645, 645, 645, 640, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 494, 495, 496, 497, 645, 639, 645, 645, 645, 645, 591, -1, 498, 645, 592, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 301, 645, 593, 500, 645, 645, 645, 645, 501, 645, 645, 645, -1, 645, 645, 645, 502, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 303, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 613, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 503, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 305, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 307, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 309, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 311, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 504, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 313, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 615, 645, 645, 645, 645, 645, 645, 315, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 317, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 319, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 321, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 508, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 323, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 325, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 327, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 329, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 331, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 644, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 646, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 511, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 512, 645, 645, 645, 645, 594, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 513, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 595, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 515, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 333, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 517, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 599, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 597, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 520, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 620, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 524, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 337, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 339, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 341, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 343, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 345, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 528, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 529, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 601, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 530, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 347, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 533, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 603, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 535, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 349, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 537, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 351, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 353, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 357, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 604, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 540, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 359, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 361, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 363, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 542, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 622, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 544, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 545, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 624, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 365, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 547, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 548, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 549, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 367, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 369, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 371, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 373, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 375, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 377, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 557, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 558, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 379, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 381, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 383, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 562, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 563, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 385, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 387, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 389, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 656, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 564, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 391, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 565, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 605, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 393, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 395, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 566, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 397, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 399, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 567, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 401, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 569, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 572, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 573, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 403, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 405, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 407, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 574, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 575, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 409, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 576, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 577, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 411, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 609, 645, 645, 645, 462, -1, 645, 463, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 587, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 483, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 490, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 482, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 630, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 491, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 492, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 509, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 617, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 506, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 631, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 518, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 618, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 596, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 516, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 654, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 531, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 532, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 536, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 623, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 534, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 539, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 602, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 555, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 546, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 551, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 568, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 570, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 464, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 465, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 629, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 484, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 499, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 616, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 507, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 519, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 619, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 600, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 522, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 621, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 541, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 538, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 543, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 556, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 552, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 559, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 571, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 466, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 590, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 510, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 614, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 521, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 526, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 523, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 598, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 550, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 553, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 560, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 467, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 505, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 514, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 633, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 525, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 554, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 468, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 527, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 652, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 584, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 469, 645, 645, 645, 470, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 471, 645, 645, 645, 472, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 473, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 634, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 635, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 606, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 642, 645, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 645, 641, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 645, 645, 645, 645, 645, 645, 645, 645, 625, 645, 645, 645, 645, -1, 645, 645, 645, 645, 645, 645, 645, 645, -1, -1, 645, 645, -1, -1, -1, -1, 645, -1, -1, -1, 645, 645, 645, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 645, 645, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  250,
			  386,
			  390,
			  394,
			  398,
			  404,
			  406,
			  408,
			  412,
			  413,
			  185,
			  414,
			  418,
			  419,
			  421,
			  423,
			  424,
			  425,
			  428,
			  429
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 657);
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

