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
					// #line 92
					{ 
						return ProcessEof(Tokens.T_INLINE_HTML);
					}
					break;
					
				case 3:
					// #line 714
					{
						yymore(); break;
					}
					break;
					
				case 4:
					// #line 701
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						if (this._allowShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							yymore(); break;
						}
					}
					break;
					
				case 5:
					// #line 681
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
					// #line 691
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
					// #line 78
					{
						return Tokens.EOF;
					}
					break;
					
				case 8:
					// #line 589
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 735
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 278
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 11:
					// #line 629
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 12:
					// #line 906
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 13:
					// #line 301
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 14:
					// #line 739
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 15:
					// #line 594
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 602
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 882
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 872
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 821
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 309
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 755
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 157
					{
						return ProcessToken(Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 181
					{
						return ProcessToken(Tokens.T_DO);
					}
					break;
					
				case 24:
					// #line 569
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 25:
					// #line 213
					{
						return ProcessToken(Tokens.T_AS);
					}
					break;
					
				case 26:
					// #line 477
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 27:
					// #line 273
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 28:
					// #line 513
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 29:
					// #line 585
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 30:
					// #line 505
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 31:
					// #line 645
					{
						return ProcessRealNumber();
					}
					break;
					
				case 32:
					// #line 297
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 33:
					// #line 533
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 34:
					// #line 745
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 35:
					// #line 529
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 36:
					// #line 521
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 37:
					// #line 517
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 38:
					// #line 457
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 39:
					// #line 489
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 40:
					// #line 509
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 41:
					// #line 473
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 42:
					// #line 493
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 43:
					// #line 501
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 44:
					// #line 581
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 45:
					// #line 537
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 46:
					// #line 549
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 47:
					// #line 565
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 48:
					// #line 553
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 49:
					// #line 561
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 50:
					// #line 557
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 51:
					// #line 760
					{
						return ProcessVariable();
					}
					break;
					
				case 52:
					// #line 577
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 53:
					// #line 141
					{
						return ProcessToken(Tokens.T_TRY);
					}
					break;
					
				case 54:
					// #line 117
					{
						return ProcessToken(Tokens.T_EXIT);
					}
					break;
					
				case 55:
					// #line 185
					{
						return ProcessToken(Tokens.T_FOR);
					}
					break;
					
				case 56:
					// #line 377
					{
						return ProcessToken(Tokens.T_USE);
					}
					break;
					
				case 57:
					// #line 313
					{
						return ProcessToken(Tokens.T_NEW);
					}
					break;
					
				case 58:
					// #line 573
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 59:
					// #line 545
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 60:
					// #line 305
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 61:
					// #line 321
					{
						return ProcessToken(Tokens.T_VAR);
					}
					break;
					
				case 62:
					// #line 525
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 63:
					// #line 481
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 64:
					// #line 485
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 65:
					// #line 497
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 66:
					// #line 541
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 67:
					// #line 633
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 68:
					// #line 625
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 69:
					// #line 113
					{ 
						return ProcessToken(Tokens.T_EXIT);
					}
					break;
					
				case 70:
					// #line 245
					{
						return ProcessToken(Tokens.T_ECHO);
					}
					break;
					
				case 71:
					// #line 169
					{
						return ProcessToken(Tokens.T_ELSE);
					}
					break;
					
				case 72:
					// #line 353
					{
						return ProcessToken(Tokens.T_EVAL);
					}
					break;
					
				case 73:
					// #line 225
					{
						return ProcessToken(Tokens.T_CASE);
					}
					break;
					
				case 74:
					// #line 461
					{
						return ProcessToken(Tokens.T_LIST);
					}
					break;
					
				case 75:
					// #line 241
					{
						return ProcessToken(Tokens.T_GOTO);
					}
					break;
					
				case 76:
					// #line 750
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 77:
					// #line 165
					{
						return ProcessToken(Tokens.T_ENDIF);
					}
					break;
					
				case 78:
					// #line 393
					{
						return ProcessToken(Tokens.T_EMPTY);
					}
					break;
					
				case 79:
					// #line 389
					{
						return ProcessToken(Tokens.T_ISSET);
					}
					break;
					
				case 80:
					// #line 261
					{
						return ProcessToken(Tokens.T_TRAIT);
					}
					break;
					
				case 81:
					// #line 153
					{
						return ProcessToken(Tokens.T_THROW);
					}
					break;
					
				case 82:
					// #line 437
					{
						return ProcessToken(Tokens.T_FINAL);
					}
					break;
					
				case 83:
					// #line 453
					{
						return ProcessToken(Tokens.T_UNSET);
					}
					break;
					
				case 84:
					// #line 125
					{
						return ProcessToken(Tokens.T_CONST);
					}
					break;
					
				case 85:
					// #line 317
					{
						return ProcessToken(Tokens.T_CLONE);
					}
					break;
					
				case 86:
					// #line 253
					{
						return ProcessToken(Tokens.T_CLASS);
					}
					break;
					
				case 87:
					// #line 145
					{
						return ProcessToken(Tokens.T_CATCH);
					}
					break;
					
				case 88:
					// #line 137
					{
						return ProcessToken(Tokens.T_YIELD);
					}
					break;
					
				case 89:
					// #line 465
					{
						return ProcessToken(Tokens.T_ARRAY);
					}
					break;
					
				case 90:
					// #line 173
					{
						return ProcessToken(Tokens.T_WHILE);
					}
					break;
					
				case 91:
					// #line 233
					{
						return ProcessToken(Tokens.T_BREAK);
					}
					break;
					
				case 92:
					// #line 249
					{
						return ProcessToken(Tokens.T_PRINT);
					}
					break;
					
				case 93:
					// #line 325
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 94:
					// #line 764
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
					// #line 189
					{
						return ProcessToken(Tokens.T_ENDFOR);
					}
					break;
					
				case 96:
					// #line 161
					{
						return ProcessToken(Tokens.T_ELSEIF);
					}
					break;
					
				case 97:
					// #line 429
					{
						return ProcessToken(Tokens.T_STATIC);
					}
					break;
					
				case 98:
					// #line 217
					{
						return ProcessToken(Tokens.T_SWITCH);
					}
					break;
					
				case 99:
					// #line 129
					{
						return ProcessToken(Tokens.T_RETURN);
					}
					break;
					
				case 100:
					// #line 385
					{
						return ProcessToken(Tokens.T_GLOBAL);
					}
					break;
					
				case 101:
					// #line 449
					{
						return ProcessToken(Tokens.T_PUBLIC);
					}
					break;
					
				case 102:
					// #line 329
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 103:
					// #line 345
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 104:
					// #line 265
					{
						return ProcessToken(Tokens.T_EXTENDS);
					}
					break;
					
				case 105:
					// #line 357
					{
						return ProcessToken(Tokens.T_INCLUDE);
					}
					break;
					
				case 106:
					// #line 229
					{
						return ProcessToken(Tokens.T_DEFAULT);
					}
					break;
					
				case 107:
					// #line 201
					{
						return ProcessToken(Tokens.T_DECLARE);
					}
					break;
					
				case 108:
					// #line 149
					{
						return ProcessToken(Tokens.T_FINALLY);
					}
					break;
					
				case 109:
					// #line 193
					{
						return ProcessToken(Tokens.T_FOREACH);
					}
					break;
					
				case 110:
					// #line 365
					{
						return ProcessToken(Tokens.T_REQUIRE);
					}
					break;
					
				case 111:
					// #line 441
					{
						return ProcessToken(Tokens.T_PRIVATE);
					}
					break;
					
				case 112:
					// #line 349
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 113:
					// #line 337
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 114:
					// #line 673
					{
						return ProcessToken(Tokens.T_DIR);
					}
					break;
					
				case 115:
					// #line 177
					{
						return ProcessToken(Tokens.T_ENDWHILE);
					}
					break;
					
				case 116:
					// #line 121
					{
						return ProcessToken(Tokens.T_FUNCTION);
					}
					break;
					
				case 117:
					// #line 237
					{
						return ProcessToken(Tokens.T_CONTINUE);
					}
					break;
					
				case 118:
					// #line 469
					{
						return ProcessToken(Tokens.T_CALLABLE);
					}
					break;
					
				case 119:
					// #line 433
					{
						return ProcessToken(Tokens.T_ABSTRACT);
					}
					break;
					
				case 120:
					// #line 341
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 121:
					// #line 333
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 122:
					// #line 669
					{
						return ProcessToken(Tokens.T_FILE);
					}
					break;
					
				case 123:
					// #line 665
					{
						return ProcessToken(Tokens.T_LINE);
					}
					break;
					
				case 124:
					// #line 221
					{
						return ProcessToken(Tokens.T_ENDSWITCH);
					}
					break;
					
				case 125:
					// #line 257
					{
						return ProcessToken(Tokens.T_INTERFACE);
					}
					break;
					
				case 126:
					// #line 381
					{
						return ProcessToken(Tokens.T_INSTEADOF);
					}
					break;
					
				case 127:
					// #line 373
					{
						return ProcessToken(Tokens.T_NAMESPACE);
					}
					break;
					
				case 128:
					// #line 445
					{
						return ProcessToken(Tokens.T_PROTECTED);
					}
					break;
					
				case 129:
					// #line 653
					{
						return ProcessToken(Tokens.T_TRAIT_C);
					}
					break;
					
				case 130:
					// #line 649
					{
						return ProcessToken(Tokens.T_CLASS_C);
					}
					break;
					
				case 131:
					// #line 205
					{
						return ProcessToken(Tokens.T_ENDDECLARE);
					}
					break;
					
				case 132:
					// #line 197
					{
						return ProcessToken(Tokens.T_ENDFOREACH);
					}
					break;
					
				case 133:
					// #line 209
					{
						return ProcessToken(Tokens.T_INSTANCEOF);
					}
					break;
					
				case 134:
					// #line 269
					{
						return ProcessToken(Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 135:
					// #line 133
					{
						return Tokens.T_YIELD_FROM;
					}
					break;
					
				case 136:
					// #line 661
					{
						return ProcessToken(Tokens.T_METHOD_C);
					}
					break;
					
				case 137:
					// #line 361
					{
						return ProcessToken(Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 138:
					// #line 369
					{
						return ProcessToken(Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 139:
					// #line 657
					{
						return ProcessToken(Tokens.T_FUNC_C);
					}
					break;
					
				case 140:
					// #line 677
					{
						return ProcessToken(Tokens.T_NS_C);
					}
					break;
					
				case 141:
					// #line 412
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 142:
					// #line 84
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 143:
					// #line 880
					{ yymore(); break; }
					break;
					
				case 144:
					// #line 878
					{ yymore(); break; }
					break;
					
				case 145:
					// #line 876
					{ Tokens token; if (ProcessString(1, out token)) return token; else break; }
					break;
					
				case 146:
					// #line 879
					{ yymore(); break; }
					break;
					
				case 147:
					// #line 875
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 148:
					// #line 874
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 149:
					// #line 873
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 150:
					// #line 81
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 151:
					// #line 825
					{ yymore(); break; }
					break;
					
				case 152:
					// #line 824
					{ yymore(); break; }
					break;
					
				case 153:
					// #line 823
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 154:
					// #line 822
					{ yymore(); break; }
					break;
					
				case 155:
					// #line 890
					{ yymore(); break; }
					break;
					
				case 156:
					// #line 888
					{ yymore(); break; }
					break;
					
				case 157:
					// #line 886
					{ Tokens token; if (ProcessShell(1, out token)) return token; else break; }
					break;
					
				case 158:
					// #line 889
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 885
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 160:
					// #line 884
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 161:
					// #line 883
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 162:
					// #line 899
					{ yymore(); break; }
					break;
					
				case 163:
					// #line 898
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 896
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 897
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 894
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 167:
					// #line 893
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 168:
					// #line 892
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 169:
					// #line 810
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => (string)ProcessEscapedString(s, _encoding, false)) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 170:
					// #line 291
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 171:
					// #line 286
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 172:
					// #line 282
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 173:
					// #line 618
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 174:
					// #line 610
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return ProcessToken(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 175:
					// #line 104
					{
						if(TokenLength > 0)
						{
							SetDocBlock(); 
							return Tokens.T_DOC_COMMENT; 
						}
						return Tokens.EOF;
					}
					break;
					
				case 176:
					// #line 751
					{ yymore(); break; }
					break;
					
				case 177:
					// #line 753
					{ yymore(); break; }
					break;
					
				case 178:
					// #line 752
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 179:
					// #line 96
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 180:
					// #line 746
					{ yymore(); break; }
					break;
					
				case 181:
					// #line 748
					{ yymore(); break; }
					break;
					
				case 182:
					// #line 747
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 183:
					// #line 904
					{ yymore(); break; }
					break;
					
				case 184:
					// #line 100
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 185:
					// #line 903
					{ yymore(); break; }
					break;
					
				case 186:
					// #line 901
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 187:
					// #line 902
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 188:
					// #line 723
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 189:
					// #line 637
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 190:
					// #line 718
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 191:
					// #line 641
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 192:
					// #line 797
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 193:
					// #line 790
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						_yyless(LabelTrailLength());
						this._tokenSemantics.Object = this._hereDocLabel;
						return Tokens.T_END_HEREDOC;
					}
					break;
					
				case 194:
					// #line 819
					{ yymore(); break; }
					break;
					
				case 195:
					// #line 801
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => s) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 196:
					// #line 424
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 197:
					// #line 418
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 198:
					// #line 397
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 199:
					// #line 421
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 200:
					// #line 422
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 201:
					// #line 420
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 202:
					// #line 402
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 203:
					// #line 407
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 204:
					// #line 866
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 205:
					// #line 855
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 206:
					// #line 849
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 207:
					// #line 844
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 208:
					// #line 827
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 209:
					// #line 838
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 210:
					// #line 832
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 211:
					// #line 860
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 214: goto case 3;
				case 215: goto case 6;
				case 216: goto case 8;
				case 217: goto case 9;
				case 218: goto case 11;
				case 219: goto case 21;
				case 220: goto case 31;
				case 221: goto case 42;
				case 222: goto case 94;
				case 223: goto case 144;
				case 224: goto case 152;
				case 225: goto case 156;
				case 226: goto case 163;
				case 227: goto case 164;
				case 228: goto case 169;
				case 229: goto case 170;
				case 230: goto case 173;
				case 231: goto case 183;
				case 232: goto case 186;
				case 233: goto case 188;
				case 234: goto case 189;
				case 235: goto case 191;
				case 236: goto case 193;
				case 237: goto case 195;
				case 238: goto case 196;
				case 239: goto case 204;
				case 241: goto case 8;
				case 242: goto case 9;
				case 243: goto case 21;
				case 244: goto case 191;
				case 245: goto case 204;
				case 247: goto case 8;
				case 248: goto case 9;
				case 250: goto case 8;
				case 251: goto case 9;
				case 253: goto case 8;
				case 254: goto case 9;
				case 256: goto case 8;
				case 257: goto case 9;
				case 259: goto case 8;
				case 260: goto case 9;
				case 262: goto case 8;
				case 263: goto case 9;
				case 265: goto case 8;
				case 266: goto case 9;
				case 268: goto case 8;
				case 269: goto case 9;
				case 271: goto case 8;
				case 272: goto case 9;
				case 274: goto case 8;
				case 275: goto case 9;
				case 277: goto case 8;
				case 278: goto case 9;
				case 280: goto case 8;
				case 281: goto case 9;
				case 283: goto case 8;
				case 284: goto case 9;
				case 286: goto case 8;
				case 287: goto case 9;
				case 289: goto case 8;
				case 290: goto case 9;
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
				case 429: goto case 9;
				case 441: goto case 9;
				case 445: goto case 9;
				case 447: goto case 9;
				case 449: goto case 9;
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
				case 657: goto case 9;
				case 658: goto case 9;
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
			AcceptConditions.AcceptOnStart, // 169
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
			AcceptConditions.AcceptOnStart, // 193
			AcceptConditions.Accept, // 194
			AcceptConditions.AcceptOnStart, // 195
			AcceptConditions.Accept, // 196
			AcceptConditions.Accept, // 197
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
			AcceptConditions.NotAccept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.Accept, // 214
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
			AcceptConditions.AcceptOnStart, // 228
			AcceptConditions.Accept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.Accept, // 234
			AcceptConditions.Accept, // 235
			AcceptConditions.AcceptOnStart, // 236
			AcceptConditions.AcceptOnStart, // 237
			AcceptConditions.Accept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.NotAccept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.Accept, // 244
			AcceptConditions.Accept, // 245
			AcceptConditions.NotAccept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.Accept, // 248
			AcceptConditions.NotAccept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.Accept, // 251
			AcceptConditions.NotAccept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.Accept, // 254
			AcceptConditions.NotAccept, // 255
			AcceptConditions.Accept, // 256
			AcceptConditions.Accept, // 257
			AcceptConditions.NotAccept, // 258
			AcceptConditions.Accept, // 259
			AcceptConditions.Accept, // 260
			AcceptConditions.NotAccept, // 261
			AcceptConditions.Accept, // 262
			AcceptConditions.Accept, // 263
			AcceptConditions.NotAccept, // 264
			AcceptConditions.Accept, // 265
			AcceptConditions.Accept, // 266
			AcceptConditions.NotAccept, // 267
			AcceptConditions.Accept, // 268
			AcceptConditions.Accept, // 269
			AcceptConditions.NotAccept, // 270
			AcceptConditions.Accept, // 271
			AcceptConditions.Accept, // 272
			AcceptConditions.NotAccept, // 273
			AcceptConditions.Accept, // 274
			AcceptConditions.Accept, // 275
			AcceptConditions.NotAccept, // 276
			AcceptConditions.Accept, // 277
			AcceptConditions.Accept, // 278
			AcceptConditions.NotAccept, // 279
			AcceptConditions.Accept, // 280
			AcceptConditions.Accept, // 281
			AcceptConditions.NotAccept, // 282
			AcceptConditions.Accept, // 283
			AcceptConditions.Accept, // 284
			AcceptConditions.NotAccept, // 285
			AcceptConditions.Accept, // 286
			AcceptConditions.Accept, // 287
			AcceptConditions.NotAccept, // 288
			AcceptConditions.Accept, // 289
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
			AcceptConditions.NotAccept, // 410
			AcceptConditions.NotAccept, // 411
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
			AcceptConditions.Accept, // 429
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
			AcceptConditions.Accept, // 442
			AcceptConditions.NotAccept, // 443
			AcceptConditions.NotAccept, // 444
			AcceptConditions.Accept, // 445
			AcceptConditions.NotAccept, // 446
			AcceptConditions.Accept, // 447
			AcceptConditions.NotAccept, // 448
			AcceptConditions.Accept, // 449
			AcceptConditions.NotAccept, // 450
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
			AcceptConditions.Accept, // 657
			AcceptConditions.Accept, // 658
		};
		
		private static int[] colMap = new int[]
		{
			30, 30, 30, 30, 30, 30, 30, 30, 30, 36, 17, 30, 30, 59, 30, 30, 
			30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 
			36, 47, 62, 44, 63, 49, 50, 64, 35, 37, 43, 46, 53, 26, 33, 42, 
			57, 58, 29, 29, 29, 29, 29, 29, 29, 29, 31, 41, 48, 45, 27, 2, 
			53, 19, 22, 11, 7, 3, 8, 24, 20, 5, 38, 23, 16, 18, 10, 12, 
			25, 40, 14, 13, 6, 9, 34, 21, 4, 15, 28, 56, 32, 60, 52, 39, 
			61, 19, 22, 11, 7, 3, 8, 24, 20, 5, 38, 23, 16, 18, 10, 12, 
			25, 40, 14, 13, 6, 9, 34, 21, 4, 15, 28, 54, 51, 55, 53, 30, 
			0, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 1, 2, 1, 1, 1, 3, 4, 5, 6, 1, 1, 1, 1, 
			1, 1, 1, 1, 1, 7, 8, 8, 8, 8, 1, 1, 1, 9, 1, 10, 
			1, 1, 11, 1, 12, 1, 1, 13, 1, 1, 14, 15, 16, 1, 1, 1, 
			1, 1, 1, 17, 8, 8, 8, 18, 8, 8, 8, 1, 1, 8, 1, 1, 
			1, 1, 1, 19, 20, 8, 8, 21, 8, 8, 8, 8, 22, 8, 8, 8, 
			8, 8, 23, 8, 8, 8, 8, 8, 24, 8, 8, 8, 8, 1, 1, 25, 
			8, 8, 8, 8, 8, 8, 1, 1, 8, 26, 8, 8, 8, 8, 27, 8, 
			1, 1, 8, 8, 8, 8, 8, 8, 1, 1, 8, 8, 8, 8, 8, 8, 
			8, 8, 8, 8, 8, 8, 8, 1, 8, 8, 8, 8, 8, 8, 1, 28, 
			29, 1, 1, 1, 1, 1, 1, 30, 30, 1, 1, 31, 32, 1, 1, 1, 
			1, 1, 33, 1, 34, 1, 1, 1, 1, 1, 1, 35, 1, 1, 1, 1, 
			36, 37, 1, 1, 38, 39, 1, 40, 1, 41, 1, 1, 1, 42, 1, 43, 
			1, 1, 1, 1, 1, 44, 1, 1, 45, 46, 1, 1, 1, 1, 1, 47, 
			1, 1, 1, 1, 48, 49, 50, 51, 52, 53, 54, 1, 55, 1, 56, 57, 
			58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 
			74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 
			1, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 
			105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 87, 118, 119, 
			19, 67, 120, 20, 121, 55, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 
			132, 133, 134, 22, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 
			147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 
			163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 
			179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 
			195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 
			211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 
			227, 228, 229, 230, 231, 232, 233, 64, 234, 235, 236, 237, 69, 78, 238, 239, 
			240, 241, 242, 243, 244, 46, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 
			255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 
			271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 
			287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 
			303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 
			319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 
			335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 
			351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 
			367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 
			383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 
			399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 
			415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 
			431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 
			447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 
			463, 464, 465, 466, 467, 468, 469, 8, 470, 471, 472, 473, 474, 475, 476, 477, 
			478, 479, 480
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 214, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 212, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 452, 647, 647, 647, 647, 647, 453, 454, 647, 647, 647, 647, 455, -1, 456, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 457, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 243, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, 51, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, 51, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 634, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 67, -1, -1, -1, 67, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, 68, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 332, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 352, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 349, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, 349, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, 349, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 638, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 563, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 655, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, -1, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, -1, 143, 143, 143, 143, 143, 143, 143, -1, -1, 143 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 147, -1 },
			{ -1, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1 },
			{ -1, -1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, -1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, -1, 155, 155, 155, 155, 155, 155, -1, 155, -1, 155 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 159, -1 },
			{ -1, -1, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, -1, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, -1, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, -1, 162, 162, 162, 162, -1, 162, 162, 162, -1, 162 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 166, -1 },
			{ -1, -1, -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, -1, -1, 171, 171, -1, -1, -1, -1, 171, -1, -1, -1, 171, 171, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 171, 171, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, -1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, -1, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 184, 185, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 186, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 232, 231, 231, 231, 231, 231 },
			{ -1, 184, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, 189, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, 191, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 421, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, -1, 207, 207, 207, 207, 207, 207, 207, 207, 425, -1, 207, 207, -1, -1, -1, -1, 207, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 209, 207, 207, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, -1, 397, 397, 397, 397, 397, 397, 397, 397, -1, -1, 397, -1, -1, -1, -1, -1, 397, -1, -1, -1, 397, 397, 397, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 22, 647, 458, 647, 647, 459, 647, 647, 647, -1, 581, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, 288, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, 220, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1, 148, 148, 148, 148, 148, 148, 148, 148, -1, -1, 148, -1, -1, -1, -1, -1, 148, -1, -1, -1, 148, 148, 148, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 149, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 152, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1 },
			{ -1, -1, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1, 160, 160, 160, 160, 160, 160, 160, 160, -1, -1, 160, -1, -1, -1, -1, -1, 160, -1, -1, -1, 160, 160, 160, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 161, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 163, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, -1, -1, 167, -1, -1, -1, -1, -1, 167, -1, -1, -1, 167, 167, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 407, 407, 407, 407, 407, 407, 407, 407, 407, 407, 407, 407, 407, 407, -1, 407, 407, 407, 407, 407, 407, 407, 407, -1, -1, 407, 407, -1, -1, -1, -1, 407, -1, -1, -1, 407, 407, 407, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, 407, 407, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, -1, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, -1, 231, 231, 231, 231, 231 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, -1, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 412, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 413, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, 191, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 235, -1, -1, -1, 235, 235, -1, -1, 235, -1, -1, -1, -1, -1, -1, -1, 235, -1, -1, 235, -1, -1, -1, -1, -1, -1, 235, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 235, 235, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 206, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 461, 647, 263, 647, 647, 647, 647, 647, 647, 23, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, 244, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, -1, 207, 207, 207, 207, 207, 207, 207, 207, -1, -1, 207, -1, -1, -1, -1, -1, 207, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 215, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 24, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 8, 9, 429, 217, 441, 242, 445, 447, 449, 580, 248, 609, 628, 639, 645, 10, 647, 251, 647, 649, 254, 647, 650, 651, 216, 241, 647, 11, 12, 247, 13, 250, 451, 253, 10, 256, 647, 652, 647, 256, 259, 262, 14, 265, 268, 271, 274, 277, 280, 283, 286, 256, 15, 16, 256, 218, 11, 10, 256, 17, 18, 289, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, 258, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 275, 647, 647, 25, 583, 647, 647, -1, 647, 647, 647, 647, 610, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 431, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 261, -1, 264, 267, 432, -1, -1, 270, 273, 276, -1, -1, -1, -1, 279, -1, -1, 282, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 285, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 587, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, 220, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 52, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 34, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 53, 647, -1, 647, 481, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 433, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 54, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 55, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 56, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 301, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 57, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 434, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 58, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 61, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 69, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, 435, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 70, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 72, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 74, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 75, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 77, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 78, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 437, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 79, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 80, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 81, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 82, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, -1, 325, 325, 325, 325, 325, 325, 325, 325, -1, -1, 325, -1, -1, -1, -1, -1, 325, -1, 309, -1, 325, 325, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, 439 },
			{ -1, -1, -1, 647, 647, 647, 83, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 84, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 444, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 85, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 86, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 87, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 88, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 89, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 90, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 94, 325, 325, 325, 325, 325, 325, 325, 325, -1, -1, 325, 325, -1, -1, -1, -1, 325, -1, -1, -1, 325, 325, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, 325, 222, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 91, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, -1, 345, 345, 345, 345, 345, 345, 345, 345, -1, -1, 345, -1, -1, -1, -1, -1, 345, -1, -1, -1, 345, 345, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 92, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 95, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 96, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 97, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 98, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 99, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 100, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 101, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 104, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, -1, 345, 345, 345, 345, 345, 345, 345, 345, -1, -1, 345, 345, -1, -1, -1, -1, 345, -1, -1, -1, 345, 345, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, 345, -1, -1, -1, 369, -1, -1 },
			{ -1, -1, -1, 105, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, -1, 347, 347, 347, 347, 347, 347, 347, 347, -1, -1, 347, 347, -1, -1, -1, -1, 347, -1, -1, -1, 347, 347, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, 347, -1, -1, -1, -1, -1, 369 },
			{ -1, -1, -1, 647, 647, 647, 106, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 448, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 107, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 450, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 108, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 109, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 110, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 111, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 114, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, 113, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 115, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 116, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 117, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 118, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 119, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, 120, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 122, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 123, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 124, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 125, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 135, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 126, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 1, 142, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 383, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 144, 143, 143, 143, 143, 143, 143, 143, 145, 223, 143 },
			{ -1, -1, -1, 127, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146 },
			{ -1, -1, -1, 647, 647, 647, 647, 128, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 1, 150, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 152, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 387, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 224, 151, 151, 151, 151, 153 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 129, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 130, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 1, 150, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 391, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 156, 155, 155, 155, 155, 155, 155, 157, 155, 225, 155 },
			{ -1, -1, -1, 131, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 132, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 213, 150, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 163, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 395, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 164, 162, 162, 162, 162, 226, 162, 162, 162, 227, 162 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 133, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 134, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 397, 169, 397, 397, 397, 397, 397, 397, 397, 397, -1, -1, 397, 397, -1, -1, -1, -1, 397, -1, -1, -1, 397, 397, 397, 399, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 397, 397, 228, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 136, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 137, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 138, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 170, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 10, 171, 171, 171, 171, 171, 171, 171, 171, 229, 170, 171, 170, 170, 170, 170, 170, 171, 170, 10, 170, 171, 171, 171, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 10, 170, 170, 170, 170, 170 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 139, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 173, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 173, 230, 230, 230, 230, 230, 230, 230, 230, 173, 173, 230, 173, 173, 173, 173, 173, 230, 173, 173, 173, 230, 230, 230, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 140, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 141, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ 1, 175, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 177, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176 },
			{ 1, 179, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 181, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180 },
			{ 1, 7, 188, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 12, 647, 647, 647, 647, 647, 647, 647, 647, 188, 188, 647, 189, 12, 188, 12, 188, 647, 188, 12, 188, 647, 647, 647, 188, 188, 188, 12, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 234, 189, 12, 190, 188, 188, 233, 12 },
			{ 430, 7, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192 },
			{ -1, -1, -1, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 193, 415, 415, 415, 415, 415, 415, 415, 415, -1, -1, 415, 415, -1, -1, -1, -1, 415, -1, -1, -1, 415, 415, 415, 416, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 415, 415, 236, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1 },
			{ 442, 150, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194 },
			{ -1, -1, -1, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 195, 418, 418, 418, 418, 418, 418, 418, 418, -1, -1, 418, 418, -1, -1, -1, -1, 418, -1, -1, -1, 418, 418, 418, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 418, 418, 237, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, -1, -1, -1 },
			{ 1, 7, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 198, 197, 196, 196, 196, 196, 196, 238, 196, 199, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196 },
			{ 1, 7, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 202, 196, 196, 196, 196, 238, 196, 199, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196 },
			{ 1, 7, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 203, 238, 196, 199, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 197, 196, 196, 196, 196, 196 },
			{ 1, 7, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 239, 204, 204, 204, 204, 204, 204, 204, 205, 245, 204 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 426, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 210, 210, -1, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, -1, 210, 210, 210, 210, 210, 210, 210, 210, -1, -1, 210, -1, -1, -1, -1, -1, 210, -1, -1, -1, 210, 210, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 239, 204, 204, 204, 204, 204, 204, 211, 204, 245, 204 },
			{ 1, 1, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 239, 204, 204, 204, 204, 204, 204, 204, 204, 245, 204 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 257, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, 415, -1, 415, 415, 415, 415, 415, 415, 415, 415, -1, -1, 415, -1, -1, -1, -1, -1, 415, -1, -1, -1, 415, 415, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 438, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, -1, 347, 347, 347, 347, 347, 347, 347, 347, -1, -1, 347, -1, -1, -1, -1, -1, 347, -1, -1, -1, 347, 347, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 260, 647, 647, -1, 647, 647, 460, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, 418, -1, 418, 418, 418, 418, 418, 418, 418, 418, -1, -1, 418, -1, -1, -1, -1, -1, 418, -1, -1, -1, 418, 418, 418, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 462, 647, 647, 647, 584, 647, 647, 266, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 582, 647, 647, 269, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 272, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 463, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 278, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 281, 612, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 476, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 284, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 287, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 477, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 290, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 629, 647, 647, 647, 647, 478, 647, 479, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 480, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 482, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 585, 647, 647, 613, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 483, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 640, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 487, 647, 647, 647, 647, -1, 647, 488, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 489, 647, 647, 647, 647, 647, 647, 292, 647, 647, 657, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 590, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 614, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 490, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 591, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 491, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 294, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 296, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 588, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 630, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 495, 647, 647, 647, 647, 647, 647, 642, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 496, 497, 498, 647, 499, 641, 647, 647, 647, 647, 593, -1, 500, 647, 594, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 298, 647, 595, 502, 647, 647, 647, 647, 503, 647, 647, 647, -1, 647, 647, 647, 504, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 300, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 615, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 505, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 302, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 304, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 306, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 308, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 506, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 310, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 617, 647, 647, 647, 647, 647, 647, 312, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 314, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 316, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 318, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 510, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 320, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 322, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 324, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 326, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 328, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 646, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 648, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 513, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 514, 647, 647, 647, 596, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 515, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 597, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 517, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 330, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 519, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 601, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 599, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 522, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 622, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 526, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 334, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 336, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 338, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 340, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 342, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 530, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 531, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 603, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 532, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 344, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 535, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 605, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 537, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 346, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 539, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 348, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 350, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 354, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 606, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 542, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 356, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 358, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 360, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 544, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 624, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 546, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 547, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 626, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 362, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 549, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 550, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 551, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 364, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 366, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 368, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 370, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 372, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 374, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 559, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 560, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 376, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 378, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 380, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 564, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 565, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 382, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 384, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 386, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 658, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 566, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 388, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 567, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 607, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 390, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 392, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 568, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 394, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 396, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 569, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 398, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 571, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 574, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 575, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 400, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 402, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 404, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 576, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 577, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 406, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 578, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 579, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 408, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 611, 647, 647, 647, 464, -1, 647, 465, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 589, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 485, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 492, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 484, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 632, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 493, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 494, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 511, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 619, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 508, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 633, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 520, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 620, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 598, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 518, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 656, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 533, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 534, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 538, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 625, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 536, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 541, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 604, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 557, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 548, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 553, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 570, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 572, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 466, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 467, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 631, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 486, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 501, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 618, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 509, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 521, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 621, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 602, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 524, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 653, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 623, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 543, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 540, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 545, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 558, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 554, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 561, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 573, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 468, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 592, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 512, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 616, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 523, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 528, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 525, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 600, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 552, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 555, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 562, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 469, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 507, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 516, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 635, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 527, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 556, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 470, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 529, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 654, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 586, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 471, 647, 647, 647, 472, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 473, 647, 647, 647, 647, 474, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 475, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 636, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 637, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 608, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 644, 647, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 647, 643, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 647, 647, 647, 647, 647, 647, 647, 647, 627, 647, 647, 647, 647, -1, 647, 647, 647, 647, 647, 647, 647, 647, -1, -1, 647, 647, -1, -1, -1, -1, 647, -1, -1, -1, 647, 647, 647, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 647, 647, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  249,
			  381,
			  385,
			  389,
			  393,
			  401,
			  403,
			  405,
			  409,
			  410,
			  183,
			  411,
			  414,
			  417,
			  420,
			  422,
			  423,
			  424,
			  427,
			  428
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 659);
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

