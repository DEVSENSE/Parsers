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
					// #line 727
					{
						yymore(); break;
					}
					break;
					
				case 4:
					// #line 714
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
					// #line 694
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
					// #line 704
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
					// #line 602
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 748
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 287
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 11:
					// #line 642
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 12:
					// #line 928
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 13:
					// #line 310
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 14:
					// #line 752
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 15:
					// #line 607
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 615
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 904
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 894
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 843
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 318
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 768
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
					// #line 582
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
					// #line 486
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 28:
					// #line 282
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 29:
					// #line 522
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 30:
					// #line 598
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 31:
					// #line 514
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 32:
					// #line 658
					{
						return ProcessRealNumber();
					}
					break;
					
				case 33:
					// #line 306
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 34:
					// #line 542
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 35:
					// #line 758
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 36:
					// #line 538
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 37:
					// #line 530
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 38:
					// #line 526
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 39:
					// #line 466
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 40:
					// #line 498
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 41:
					// #line 518
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 42:
					// #line 482
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 43:
					// #line 502
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 44:
					// #line 510
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 45:
					// #line 594
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 46:
					// #line 546
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 47:
					// #line 558
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 48:
					// #line 578
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 49:
					// #line 562
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 50:
					// #line 574
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 51:
					// #line 566
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 52:
					// #line 773
					{
						return ProcessVariable();
					}
					break;
					
				case 53:
					// #line 570
					{
						return (Tokens.T_COALESCE_EQUAL);
					}
					break;
					
				case 54:
					// #line 590
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
					// #line 322
					{
						return (Tokens.T_NEW);
					}
					break;
					
				case 59:
					// #line 386
					{
						return (Tokens.T_USE);
					}
					break;
					
				case 60:
					// #line 586
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 61:
					// #line 554
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 62:
					// #line 314
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 63:
					// #line 330
					{
						return (Tokens.T_VAR);
					}
					break;
					
				case 64:
					// #line 534
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 65:
					// #line 490
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 66:
					// #line 494
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 67:
					// #line 506
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 68:
					// #line 550
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 69:
					// #line 646
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 70:
					// #line 638
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
					// #line 254
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
					// #line 362
					{
						return (Tokens.T_EVAL);
					}
					break;
					
				case 75:
					// #line 234
					{
						return (Tokens.T_CASE);
					}
					break;
					
				case 76:
					// #line 470
					{
						return (Tokens.T_LIST);
					}
					break;
					
				case 77:
					// #line 250
					{
						return (Tokens.T_GOTO);
					}
					break;
					
				case 78:
					// #line 763
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 79:
					// #line 170
					{
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 80:
					// #line 402
					{
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 81:
					// #line 398
					{
						return (Tokens.T_ISSET);
					}
					break;
					
				case 82:
					// #line 270
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
					// #line 446
					{
						return (Tokens.T_FINAL);
					}
					break;
					
				case 85:
					// #line 462
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
					// #line 326
					{
						return (Tokens.T_CLONE);
					}
					break;
					
				case 88:
					// #line 262
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
					// #line 226
					{
						return (Tokens.T_MATCH);
					}
					break;
					
				case 92:
					// #line 474
					{
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 93:
					// #line 178
					{
						return (Tokens.T_WHILE);
					}
					break;
					
				case 94:
					// #line 242
					{
						return (Tokens.T_BREAK);
					}
					break;
					
				case 95:
					// #line 258
					{
						return (Tokens.T_PRINT);
					}
					break;
					
				case 96:
					// #line 334
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 97:
					// #line 777
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
					
				case 98:
					// #line 194
					{
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 99:
					// #line 166
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 100:
					// #line 438
					{
						return (Tokens.T_STATIC);
					}
					break;
					
				case 101:
					// #line 222
					{
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 102:
					// #line 134
					{
						return (Tokens.T_RETURN);
					}
					break;
					
				case 103:
					// #line 394
					{
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 104:
					// #line 458
					{
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 105:
					// #line 338
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 106:
					// #line 354
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 107:
					// #line 274
					{
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 108:
					// #line 366
					{
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 109:
					// #line 238
					{
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 110:
					// #line 206
					{
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 111:
					// #line 154
					{
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 112:
					// #line 198
					{
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 113:
					// #line 374
					{
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 114:
					// #line 450
					{
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 115:
					// #line 358
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 116:
					// #line 346
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 117:
					// #line 686
					{
						return (Tokens.T_DIR);
					}
					break;
					
				case 118:
					// #line 182
					{
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 119:
					// #line 126
					{
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 120:
					// #line 246
					{
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 121:
					// #line 478
					{
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 122:
					// #line 442
					{
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 123:
					// #line 350
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 124:
					// #line 342
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 125:
					// #line 682
					{
						return (Tokens.T_FILE);
					}
					break;
					
				case 126:
					// #line 678
					{
						return (Tokens.T_LINE);
					}
					break;
					
				case 127:
					// #line 230
					{
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 128:
					// #line 266
					{
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 129:
					// #line 390
					{
						return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 130:
					// #line 382
					{
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 131:
					// #line 454
					{
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 132:
					// #line 666
					{
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 133:
					// #line 662
					{
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 134:
					// #line 210
					{
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 135:
					// #line 202
					{
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 136:
					// #line 214
					{
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 137:
					// #line 278
					{
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 138:
					// #line 138
					{
						return Tokens.T_YIELD_FROM;
					}
					break;
					
				case 139:
					// #line 674
					{
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 140:
					// #line 370
					{
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 141:
					// #line 378
					{
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 142:
					// #line 670
					{
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 143:
					// #line 690
					{
						return (Tokens.T_NS_C);
					}
					break;
					
				case 144:
					// #line 421
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 145:
					// #line 85
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 146:
					// #line 902
					{ yymore(); break; }
					break;
					
				case 147:
					// #line 900
					{ yymore(); break; }
					break;
					
				case 148:
					// #line 898
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 149:
					// #line 901
					{ yymore(); break; }
					break;
					
				case 150:
					// #line 897
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 151:
					// #line 896
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 152:
					// #line 895
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 153:
					// #line 82
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 154:
					// #line 846
					{ yymore(); break; }
					break;
					
				case 155:
					// #line 845
					{ yymore(); break; }
					break;
					
				case 156:
					// #line 847
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 157:
					// #line 844
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 912
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 910
					{ yymore(); break; }
					break;
					
				case 160:
					// #line 908
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 161:
					// #line 911
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 907
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 163:
					// #line 906
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 164:
					// #line 905
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 165:
					// #line 921
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 920
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 918
					{ yymore(); break; }
					break;
					
				case 168:
					// #line 832
					{
					    if(!string.IsNullOrEmpty(_hereDocLabel) && VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(_processDoubleQuotedString) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 169:
					// #line 919
					{ yymore(); break; }
					break;
					
				case 170:
					// #line 916
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 171:
					// #line 915
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 172:
					// #line 914
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 173:
					// #line 300
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 174:
					// #line 295
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 175:
					// #line 291
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 176:
					// #line 631
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 177:
					// #line 623
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 178:
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
					
				case 179:
					// #line 764
					{ yymore(); break; }
					break;
					
				case 180:
					// #line 766
					{ yymore(); break; }
					break;
					
				case 181:
					// #line 765
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 182:
					// #line 97
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 183:
					// #line 759
					{ yymore(); break; }
					break;
					
				case 184:
					// #line 761
					{ yymore(); break; }
					break;
					
				case 185:
					// #line 760
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 186:
					// #line 926
					{ yymore(); break; }
					break;
					
				case 187:
					// #line 101
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 188:
					// #line 925
					{ yymore(); break; }
					break;
					
				case 189:
					// #line 923
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 190:
					// #line 924
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 191:
					// #line 736
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 192:
					// #line 650
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 193:
					// #line 731
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 194:
					// #line 654
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 195:
					// #line 819
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 196:
					// #line 808
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
					
				case 197:
					// #line 841
					{ yymore(); break; }
					break;
					
				case 198:
					// #line 823
					{
					    if(!string.IsNullOrEmpty(_hereDocLabel) && VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(null) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 199:
					// #line 433
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 200:
					// #line 427
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 201:
					// #line 406
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 202:
					// #line 430
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 203:
					// #line 431
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 204:
					// #line 429
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 205:
					// #line 411
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 206:
					// #line 416
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 207:
					// #line 888
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 208:
					// #line 877
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 209:
					// #line 871
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 210:
					// #line 866
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 211:
					// #line 849
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 212:
					// #line 860
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 213:
					// #line 854
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 214:
					// #line 882
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 217: goto case 3;
				case 218: goto case 6;
				case 219: goto case 8;
				case 220: goto case 9;
				case 221: goto case 11;
				case 222: goto case 21;
				case 223: goto case 32;
				case 224: goto case 43;
				case 225: goto case 97;
				case 226: goto case 147;
				case 227: goto case 155;
				case 228: goto case 159;
				case 229: goto case 166;
				case 230: goto case 167;
				case 231: goto case 173;
				case 232: goto case 176;
				case 233: goto case 186;
				case 234: goto case 189;
				case 235: goto case 191;
				case 236: goto case 192;
				case 237: goto case 194;
				case 238: goto case 199;
				case 239: goto case 207;
				case 242: goto case 8;
				case 243: goto case 9;
				case 244: goto case 21;
				case 245: goto case 32;
				case 246: goto case 194;
				case 247: goto case 207;
				case 249: goto case 8;
				case 250: goto case 9;
				case 252: goto case 8;
				case 253: goto case 9;
				case 255: goto case 8;
				case 256: goto case 9;
				case 258: goto case 8;
				case 259: goto case 9;
				case 261: goto case 8;
				case 262: goto case 9;
				case 264: goto case 8;
				case 265: goto case 9;
				case 267: goto case 8;
				case 268: goto case 9;
				case 270: goto case 8;
				case 271: goto case 9;
				case 273: goto case 8;
				case 274: goto case 9;
				case 276: goto case 8;
				case 277: goto case 9;
				case 279: goto case 8;
				case 280: goto case 9;
				case 282: goto case 8;
				case 283: goto case 9;
				case 285: goto case 8;
				case 286: goto case 9;
				case 288: goto case 8;
				case 289: goto case 9;
				case 291: goto case 8;
				case 292: goto case 9;
				case 294: goto case 9;
				case 296: goto case 9;
				case 298: goto case 9;
				case 300: goto case 9;
				case 302: goto case 9;
				case 304: goto case 9;
				case 306: goto case 9;
				case 308: goto case 9;
				case 310: goto case 9;
				case 312: goto case 9;
				case 314: goto case 9;
				case 316: goto case 9;
				case 318: goto case 9;
				case 320: goto case 9;
				case 322: goto case 9;
				case 324: goto case 9;
				case 326: goto case 9;
				case 328: goto case 9;
				case 330: goto case 9;
				case 332: goto case 9;
				case 334: goto case 9;
				case 336: goto case 9;
				case 338: goto case 9;
				case 340: goto case 9;
				case 342: goto case 9;
				case 344: goto case 9;
				case 346: goto case 9;
				case 348: goto case 9;
				case 350: goto case 9;
				case 352: goto case 9;
				case 354: goto case 9;
				case 356: goto case 9;
				case 358: goto case 9;
				case 360: goto case 9;
				case 362: goto case 9;
				case 364: goto case 9;
				case 366: goto case 9;
				case 368: goto case 9;
				case 370: goto case 9;
				case 372: goto case 9;
				case 374: goto case 9;
				case 376: goto case 9;
				case 378: goto case 9;
				case 380: goto case 9;
				case 382: goto case 9;
				case 384: goto case 9;
				case 386: goto case 9;
				case 388: goto case 9;
				case 390: goto case 9;
				case 392: goto case 9;
				case 394: goto case 9;
				case 396: goto case 9;
				case 398: goto case 9;
				case 400: goto case 9;
				case 402: goto case 9;
				case 404: goto case 9;
				case 406: goto case 9;
				case 408: goto case 9;
				case 410: goto case 9;
				case 412: goto case 9;
				case 414: goto case 9;
				case 432: goto case 9;
				case 443: goto case 9;
				case 446: goto case 9;
				case 448: goto case 9;
				case 450: goto case 9;
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
				case 657: goto case 9;
				case 658: goto case 9;
				case 659: goto case 9;
				case 660: goto case 9;
				case 661: goto case 9;
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
			AcceptConditions.AcceptOnStart, // 168
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
			AcceptConditions.Accept, // 197
			AcceptConditions.AcceptOnStart, // 198
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
			AcceptConditions.Accept, // 214
			AcceptConditions.NotAccept, // 215
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
			AcceptConditions.Accept, // 239
			AcceptConditions.NotAccept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.Accept, // 244
			AcceptConditions.Accept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.NotAccept, // 248
			AcceptConditions.Accept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.NotAccept, // 251
			AcceptConditions.Accept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.NotAccept, // 254
			AcceptConditions.Accept, // 255
			AcceptConditions.Accept, // 256
			AcceptConditions.NotAccept, // 257
			AcceptConditions.Accept, // 258
			AcceptConditions.Accept, // 259
			AcceptConditions.NotAccept, // 260
			AcceptConditions.Accept, // 261
			AcceptConditions.Accept, // 262
			AcceptConditions.NotAccept, // 263
			AcceptConditions.Accept, // 264
			AcceptConditions.Accept, // 265
			AcceptConditions.NotAccept, // 266
			AcceptConditions.Accept, // 267
			AcceptConditions.Accept, // 268
			AcceptConditions.NotAccept, // 269
			AcceptConditions.Accept, // 270
			AcceptConditions.Accept, // 271
			AcceptConditions.NotAccept, // 272
			AcceptConditions.Accept, // 273
			AcceptConditions.Accept, // 274
			AcceptConditions.NotAccept, // 275
			AcceptConditions.Accept, // 276
			AcceptConditions.Accept, // 277
			AcceptConditions.NotAccept, // 278
			AcceptConditions.Accept, // 279
			AcceptConditions.Accept, // 280
			AcceptConditions.NotAccept, // 281
			AcceptConditions.Accept, // 282
			AcceptConditions.Accept, // 283
			AcceptConditions.NotAccept, // 284
			AcceptConditions.Accept, // 285
			AcceptConditions.Accept, // 286
			AcceptConditions.NotAccept, // 287
			AcceptConditions.Accept, // 288
			AcceptConditions.Accept, // 289
			AcceptConditions.NotAccept, // 290
			AcceptConditions.Accept, // 291
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
			AcceptConditions.Accept, // 372
			AcceptConditions.NotAccept, // 373
			AcceptConditions.Accept, // 374
			AcceptConditions.NotAccept, // 375
			AcceptConditions.Accept, // 376
			AcceptConditions.NotAccept, // 377
			AcceptConditions.Accept, // 378
			AcceptConditions.NotAccept, // 379
			AcceptConditions.Accept, // 380
			AcceptConditions.NotAccept, // 381
			AcceptConditions.Accept, // 382
			AcceptConditions.NotAccept, // 383
			AcceptConditions.Accept, // 384
			AcceptConditions.NotAccept, // 385
			AcceptConditions.Accept, // 386
			AcceptConditions.NotAccept, // 387
			AcceptConditions.Accept, // 388
			AcceptConditions.NotAccept, // 389
			AcceptConditions.Accept, // 390
			AcceptConditions.NotAccept, // 391
			AcceptConditions.Accept, // 392
			AcceptConditions.NotAccept, // 393
			AcceptConditions.Accept, // 394
			AcceptConditions.NotAccept, // 395
			AcceptConditions.Accept, // 396
			AcceptConditions.NotAccept, // 397
			AcceptConditions.Accept, // 398
			AcceptConditions.NotAccept, // 399
			AcceptConditions.Accept, // 400
			AcceptConditions.NotAccept, // 401
			AcceptConditions.Accept, // 402
			AcceptConditions.NotAccept, // 403
			AcceptConditions.Accept, // 404
			AcceptConditions.NotAccept, // 405
			AcceptConditions.Accept, // 406
			AcceptConditions.NotAccept, // 407
			AcceptConditions.Accept, // 408
			AcceptConditions.NotAccept, // 409
			AcceptConditions.Accept, // 410
			AcceptConditions.NotAccept, // 411
			AcceptConditions.Accept, // 412
			AcceptConditions.NotAccept, // 413
			AcceptConditions.Accept, // 414
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
			AcceptConditions.NotAccept, // 430
			AcceptConditions.NotAccept, // 431
			AcceptConditions.Accept, // 432
			AcceptConditions.NotAccept, // 433
			AcceptConditions.NotAccept, // 434
			AcceptConditions.NotAccept, // 435
			AcceptConditions.NotAccept, // 436
			AcceptConditions.NotAccept, // 437
			AcceptConditions.NotAccept, // 438
			AcceptConditions.NotAccept, // 439
			AcceptConditions.NotAccept, // 440
			AcceptConditions.NotAccept, // 441
			AcceptConditions.NotAccept, // 442
			AcceptConditions.Accept, // 443
			AcceptConditions.NotAccept, // 444
			AcceptConditions.NotAccept, // 445
			AcceptConditions.Accept, // 446
			AcceptConditions.NotAccept, // 447
			AcceptConditions.Accept, // 448
			AcceptConditions.NotAccept, // 449
			AcceptConditions.Accept, // 450
			AcceptConditions.NotAccept, // 451
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
			AcceptConditions.Accept, // 657
			AcceptConditions.Accept, // 658
			AcceptConditions.Accept, // 659
			AcceptConditions.Accept, // 660
			AcceptConditions.Accept, // 661
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
			9, 9, 9, 9, 24, 9, 9, 9, 9, 9, 25, 9, 9, 9, 9, 9, 
			1, 1, 26, 9, 9, 9, 9, 9, 9, 1, 1, 9, 27, 9, 9, 9, 
			9, 28, 9, 1, 1, 9, 9, 9, 9, 9, 9, 1, 1, 9, 9, 9, 
			9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 1, 9, 9, 9, 9, 9, 
			9, 1, 29, 30, 1, 1, 1, 1, 1, 1, 31, 31, 1, 1, 32, 33, 
			1, 1, 1, 1, 1, 34, 1, 35, 36, 1, 1, 1, 1, 1, 37, 1, 
			1, 1, 1, 38, 39, 1, 1, 40, 41, 1, 42, 1, 43, 1, 1, 1, 
			44, 1, 45, 1, 46, 1, 47, 1, 48, 1, 1, 49, 50, 1, 1, 1, 
			1, 1, 51, 1, 1, 1, 1, 52, 53, 54, 55, 56, 57, 58, 1, 59, 
			1, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 
			75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 
			91, 92, 1, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 
			106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 
			122, 123, 90, 70, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 
			136, 137, 138, 139, 140, 141, 142, 143, 144, 23, 145, 146, 147, 148, 149, 150, 
			151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 
			167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 
			183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 
			199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 
			215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 
			231, 232, 233, 53, 234, 235, 236, 237, 238, 239, 240, 67, 241, 242, 243, 244, 
			245, 246, 247, 248, 249, 250, 76, 251, 50, 252, 253, 254, 255, 256, 257, 258, 
			259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 
			275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 
			291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 
			307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 
			323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 
			339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 
			355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 
			371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 
			387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 
			403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 
			419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 
			435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 
			451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 
			467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 9, 478, 479, 480, 481, 
			482, 483, 484, 485, 486, 487
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 217, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 215, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 452, 651, 651, 651, 651, 453, 651, 454, 651, 651, 651, 651, 455, -1, 456, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 457, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 257, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 32, -1, -1, -1, -1, -1, 260, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 257, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, 223, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, -1, 52, 52, 52, 52, 52, 52, 52, 52, -1, -1, 52, 52, -1, -1, -1, -1, 52, -1, -1, -1, 52, 52, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, 52, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 637, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 69, -1, -1, -1, 69, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, 69, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, 70, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 338, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 358, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 355, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, 355, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, 355, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 565, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 658, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, -1, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, -1, 146, 146, 146, 146, 146, 146, 146, -1, -1, 146 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 150, -1 },
			{ -1, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1 },
			{ -1, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1, 158, 158, 158, 158, 158, 158, -1, 158, -1, 158 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 162, -1 },
			{ -1, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, 165, 165, 165, 165, -1, 165, 165, 165, -1, 165 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1 },
			{ -1, -1, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, -1, -1, 168, 168, -1, -1, -1, -1, 168, -1, -1, -1, 168, 168, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, 168, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, -1, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, 174, 174, -1, -1, -1, -1, 174, -1, -1, -1, 174, 174, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, -1, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, -1, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 187, 188, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 189, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 234, 233, 233, 233, 233, 233 },
			{ -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 190, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 192, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 192, 192, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, 194, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, -1, 196, 196, 196, 196, 196, 196, 196, 196, -1, -1, 196, 196, -1, -1, -1, -1, 196, -1, -1, -1, 196, 196, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, 196, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1, 198, 198, 198, 198, 198, 198, 198, 198, -1, -1, 198, 198, -1, -1, -1, -1, 198, -1, -1, -1, 198, 198, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, 198, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 424, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, -1, 210, 210, 210, 210, 210, 210, 210, 210, 428, -1, 210, 210, -1, -1, -1, -1, 210, -1, -1, -1, 210, 210, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 212, 210, 210, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, -1, -1, 168, -1, -1, -1, -1, -1, 168, -1, 403, -1, 168, 168, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 22, 458, 651, 651, 651, 459, 651, 651, 651, -1, 583, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 257, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 32, -1, -1, -1, -1, -1, 260, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 257, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, -1, -1, -1, -1, -1, -1, -1, -1, -1, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, 223, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 97, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, -1, -1, 151, -1, -1, -1, -1, -1, 151, -1, -1, -1, 151, 151, 151, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 152, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 155, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1 },
			{ -1, -1, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1, 163, 163, 163, 163, 163, 163, 163, 163, -1, -1, 163, -1, -1, -1, -1, -1, 163, -1, -1, -1, 163, 163, 163, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 164, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 166, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, -1, -1, 171, -1, -1, -1, -1, -1, 171, -1, -1, -1, 171, 171, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 175, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 411, 411, 411, 411, 411, 411, 411, 411, 411, 411, 411, 411, 411, 411, -1, 411, 411, 411, 411, 411, 411, 411, 411, -1, -1, 411, 411, -1, -1, -1, -1, 411, -1, -1, -1, 411, 411, 411, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, 177, 411, 411, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, -1, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, 233, -1, 233, 233, 233, 233, 233 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, -1, 52, 52, 52, 52, 52, 52, 52, 52, -1, -1, 52, -1, -1, -1, -1, -1, 52, -1, -1, -1, 52, 52, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 418, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 419, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, 194, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 237, -1, -1, -1, 237, 237, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, 237, -1, -1, -1, -1, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, -1, -1, 418, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, 237, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 202, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 209, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1, 198, 198, 198, 198, 198, 198, 198, 198, -1, -1, 198, -1, -1, -1, -1, -1, 198, -1, 422, -1, 198, 198, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 461, 651, 268, 651, 651, 651, 651, 651, 651, 23, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, 245, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, 246, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, -1, 210, 210, 210, 210, 210, 210, 210, 210, -1, -1, 210, -1, -1, -1, -1, -1, 210, -1, -1, -1, 210, 210, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 462, 651, 651, 651, 24, 586, 651, 271, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 8, 9, 432, 220, 443, 243, 250, 446, 448, 582, 253, 611, 631, 642, 647, 10, 649, 256, 651, 652, 259, 651, 653, 654, 219, 242, 651, 11, 12, 249, 13, 252, 450, 255, 10, 258, 651, 655, 651, 258, 261, 264, 14, 267, 270, 273, 276, 279, 282, 285, 288, 258, 15, 16, 258, 221, 11, 10, 258, 17, 18, 291, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, -1, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, 223, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 25, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 433, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 266, -1, 269, 272, -1, 434, -1, 275, 278, 281, -1, -1, -1, -1, 284, -1, -1, 287, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 290, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 280, 651, 651, 651, 26, 585, 651, 651, -1, 651, 651, 651, 651, 612, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, 245, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, 245, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 589, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, 254, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 35, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 54, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 55, 651, -1, 651, 481, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 435, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 56, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 301, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 57, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 58, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 224, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 59, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 60, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 63, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 444, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 71, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, 437, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 72, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 69, -1, -1, -1, 69, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, 69, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 74, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, 70, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 75, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, 245, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 76, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, 223, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 77, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 79, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 438, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 80, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 81, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 439, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 82, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 83, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 84, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 85, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, -1, 331, 331, 331, 331, 331, 331, 331, 331, -1, -1, 331, -1, -1, -1, -1, -1, 331, -1, 315, -1, 331, 331, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 333, -1, 441 },
			{ -1, -1, -1, 651, 651, 651, 86, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, 96, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 87, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 445, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 88, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 442, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 89, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 90, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 91, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 92, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 93, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 331, 97, 331, 331, 331, 331, 331, 331, 331, 331, -1, -1, 331, 331, -1, -1, -1, -1, 331, -1, -1, -1, 331, 331, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 331, 225, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 94, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, -1, 351, 351, 351, 351, 351, 351, 351, 351, -1, -1, 351, -1, -1, -1, -1, -1, 351, -1, -1, -1, 351, 351, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 95, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 98, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, 96, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 99, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 100, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 101, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 102, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, 105, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 103, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 104, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, 106, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 107, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, 351, -1, 351, 351, 351, 351, 351, 351, 351, 351, -1, -1, 351, 351, -1, -1, -1, -1, 351, -1, -1, -1, 351, 351, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, 351, -1, -1, -1, 375, -1, -1 },
			{ -1, -1, -1, 108, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, -1, 353, 353, 353, 353, 353, 353, 353, 353, -1, -1, 353, 353, -1, -1, -1, -1, 353, -1, -1, -1, 353, 353, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, 353, -1, -1, -1, -1, -1, 375 },
			{ -1, -1, -1, 651, 651, 651, 109, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 449, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 110, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 451, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 111, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 112, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, 115, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 113, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 114, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 117, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, 116, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 118, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 119, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 381, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 120, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, 106, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 121, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 97, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 225, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 122, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 377, 123, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 125, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, 124, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 126, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 127, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 128, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 138, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 129, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 145, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 389, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 147, 146, 146, 146, 146, 146, 146, 146, 148, 226, 146 },
			{ -1, -1, -1, 130, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149 },
			{ -1, -1, -1, 651, 651, 651, 651, 131, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 153, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 155, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 393, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 227, 154, 154, 154, 154, 156 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 132, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 133, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 153, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 397, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 159, 158, 158, 158, 158, 158, 158, 160, 158, 228, 158 },
			{ -1, -1, -1, 134, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 135, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 216, 153, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 166, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 401, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 167, 165, 165, 165, 165, 229, 165, 165, 165, 230, 165 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 136, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 137, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 139, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 140, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 173, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 10, 174, 174, 174, 174, 174, 174, 174, 174, 231, 173, 174, 173, 173, 173, 173, 173, 174, 173, 10, 173, 174, 174, 174, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 10, 173, 173, 173, 173, 173 },
			{ -1, -1, -1, 141, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 176, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 176, 232, 232, 232, 232, 232, 232, 232, 232, 176, 176, 232, 176, 176, 176, 176, 176, 232, 176, 176, 176, 232, 232, 232, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 142, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 143, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 178, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 180, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 144, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ 1, 182, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 184, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183 },
			{ 1, 7, 191, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 12, 651, 651, 651, 651, 651, 651, 651, 651, 191, 191, 651, 192, 12, 191, 12, 191, 651, 191, 12, 191, 651, 651, 651, 191, 191, 191, 12, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 236, 192, 12, 193, 191, 191, 235, 12 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, 194, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 237, -1, -1, -1, 237, 237, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, 237, -1, -1, -1, -1, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, 237, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, 246, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 195, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 195, 196, 196, 196, 196, 196, 196, 196, 196, 195, 195, 196, 195, 195, 195, 195, 195, 196, 195, 195, 195, 196, 196, 196, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195 },
			{ 241, 153, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197 },
			{ 1, 7, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 200, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 201, 200, 199, 199, 199, 199, 199, 238, 199, 202, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 200, 199, 199, 199, 199, 199 },
			{ 1, 7, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 200, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 200, 205, 199, 199, 199, 199, 238, 199, 202, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 200, 199, 199, 199, 199, 199 },
			{ 1, 7, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 200, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 200, 199, 199, 199, 199, 206, 238, 199, 202, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 200, 199, 199, 199, 199, 199 },
			{ 1, 7, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 239, 207, 207, 207, 207, 207, 207, 207, 208, 247, 207 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 429, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 213, 213, -1, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, -1, 213, 213, 213, 213, 213, 213, 213, 213, -1, -1, 213, -1, -1, -1, -1, -1, 213, -1, -1, -1, 213, 213, 213, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 239, 207, 207, 207, 207, 207, 207, 214, 207, 247, 207 },
			{ 1, 1, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 239, 207, 207, 207, 207, 207, 207, 207, 207, 247, 207 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 262, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, 353, -1, 353, 353, 353, 353, 353, 353, 353, 353, -1, -1, 353, -1, -1, -1, -1, -1, 353, -1, -1, -1, 353, 353, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 265, 651, 651, -1, 651, 651, 460, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 274, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 463, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 369, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 584, 651, 651, 651, 277, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 383, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 283, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 286, 614, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 477, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 289, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 292, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 478, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 294, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 632, 651, 651, 651, 651, 479, 651, 590, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 480, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 482, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 587, 651, 651, 616, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 483, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 643, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 487, 651, 651, 651, 651, -1, 651, 488, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 489, 651, 651, 651, 651, 651, 651, 296, 651, 651, 660, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 615, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 633, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 490, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 593, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 491, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 298, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 492, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 300, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 591, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 634, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 497, 651, 651, 651, 651, 651, 651, 594, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 498, 499, 500, 501, 651, 644, 651, 651, 651, 651, 596, -1, 502, 651, 597, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 302, 651, 598, 504, 651, 651, 651, 651, 505, 651, 651, 651, -1, 651, 651, 651, 506, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 304, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 617, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 306, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 308, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 310, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 312, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 618, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 314, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 620, 651, 651, 651, 651, 651, 651, 316, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 318, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 320, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 322, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 511, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 324, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 326, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 328, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 619, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 330, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 332, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 334, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 648, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 650, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 514, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 515, 651, 651, 651, 651, 599, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 516, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 518, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 519, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 336, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 521, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 603, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 601, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 524, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 528, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 340, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 342, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 344, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 346, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 348, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 532, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 533, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 605, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 534, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 535, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 350, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 537, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 607, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 539, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 352, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 541, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 354, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 356, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 360, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 608, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 544, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 362, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 364, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 366, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 546, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 627, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 548, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 549, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 629, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 368, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 551, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 552, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 553, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 370, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 372, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 374, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 376, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 378, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 380, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 561, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 562, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 382, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 384, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 386, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 566, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 567, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 388, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 390, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 392, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 661, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 568, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 394, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 569, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 609, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 396, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 398, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 570, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 400, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 402, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 571, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 404, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 573, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 576, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 577, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 406, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 408, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 410, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 578, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 579, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 412, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 580, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 581, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 414, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 613, 651, 651, 651, 464, -1, 651, 465, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 592, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 485, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 493, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 484, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 635, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 495, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 496, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 507, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 512, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 622, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 636, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 638, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 522, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 623, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 600, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 520, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 659, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 536, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 540, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 628, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 538, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 543, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 606, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 559, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 550, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 555, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 572, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 574, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 466, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 467, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 494, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 486, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 503, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 509, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 621, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 523, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 625, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 624, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 604, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 526, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 656, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 626, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 545, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 542, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 547, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 560, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 556, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 563, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 575, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 468, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 595, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 510, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 513, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 525, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 530, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 527, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 602, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 554, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 557, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 564, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 469, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 508, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 517, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 529, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 558, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 470, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 531, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 471, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 657, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 588, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 472, 651, 651, 651, 473, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 474, 651, 651, 651, 475, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 476, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 639, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 640, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 610, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 646, 651, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 651, 645, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 651, 651, 651, 651, 651, 651, 651, 651, 651, 630, 651, 651, 651, 651, -1, 651, 651, 651, 651, 651, 651, 651, 651, -1, -1, 651, 651, -1, -1, -1, -1, 651, -1, -1, -1, 651, 651, 651, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 651, 651, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  251,
			  387,
			  391,
			  395,
			  399,
			  405,
			  407,
			  409,
			  413,
			  415,
			  186,
			  416,
			  420,
			  421,
			  423,
			  425,
			  426,
			  427,
			  430,
			  431
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 662);
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

