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
					// #line 722
					{
						yymore(); break;
					}
					break;
					
				case 4:
					// #line 709
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
					// #line 689
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
					// #line 699
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
					// #line 597
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 743
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 282
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 11:
					// #line 637
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 12:
					// #line 921
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 13:
					// #line 305
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 14:
					// #line 747
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 15:
					// #line 602
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 610
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 897
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 887
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 836
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 313
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 763
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 161
					{
						return (Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 185
					{
						return (Tokens.T_DO);
					}
					break;
					
				case 24:
					// #line 121
					{
						return (Tokens.T_FN);
					}
					break;
					
				case 25:
					// #line 577
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 26:
					// #line 217
					{
						return (Tokens.T_AS);
					}
					break;
					
				case 27:
					// #line 481
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 28:
					// #line 277
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 29:
					// #line 517
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 30:
					// #line 593
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 31:
					// #line 509
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 32:
					// #line 653
					{
						return ProcessRealNumber();
					}
					break;
					
				case 33:
					// #line 301
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 34:
					// #line 537
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 35:
					// #line 753
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 36:
					// #line 533
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 37:
					// #line 525
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 38:
					// #line 521
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 39:
					// #line 461
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 40:
					// #line 493
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 41:
					// #line 513
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 42:
					// #line 477
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 43:
					// #line 497
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 44:
					// #line 505
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 45:
					// #line 589
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 46:
					// #line 541
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 47:
					// #line 553
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 48:
					// #line 573
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 49:
					// #line 557
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 50:
					// #line 569
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 51:
					// #line 561
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 52:
					// #line 768
					{
						return ProcessVariable();
					}
					break;
					
				case 53:
					// #line 565
					{
						return (Tokens.T_COALESCE_EQUAL);
					}
					break;
					
				case 54:
					// #line 585
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 55:
					// #line 145
					{
						return (Tokens.T_TRY);
					}
					break;
					
				case 56:
					// #line 117
					{
						return (Tokens.T_EXIT);
					}
					break;
					
				case 57:
					// #line 189
					{
						return (Tokens.T_FOR);
					}
					break;
					
				case 58:
					// #line 317
					{
						return (Tokens.T_NEW);
					}
					break;
					
				case 59:
					// #line 381
					{
						return (Tokens.T_USE);
					}
					break;
					
				case 60:
					// #line 581
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 61:
					// #line 549
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 62:
					// #line 309
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 63:
					// #line 325
					{
						return (Tokens.T_VAR);
					}
					break;
					
				case 64:
					// #line 529
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 65:
					// #line 485
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 66:
					// #line 489
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 67:
					// #line 501
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 68:
					// #line 545
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 69:
					// #line 641
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 70:
					// #line 633
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 71:
					// #line 113
					{ 
						return (Tokens.T_EXIT);
					}
					break;
					
				case 72:
					// #line 249
					{
						return (Tokens.T_ECHO);
					}
					break;
					
				case 73:
					// #line 173
					{
						return (Tokens.T_ELSE);
					}
					break;
					
				case 74:
					// #line 357
					{
						return (Tokens.T_EVAL);
					}
					break;
					
				case 75:
					// #line 229
					{
						return (Tokens.T_CASE);
					}
					break;
					
				case 76:
					// #line 465
					{
						return (Tokens.T_LIST);
					}
					break;
					
				case 77:
					// #line 245
					{
						return (Tokens.T_GOTO);
					}
					break;
					
				case 78:
					// #line 758
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 79:
					// #line 169
					{
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 80:
					// #line 397
					{
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 81:
					// #line 393
					{
						return (Tokens.T_ISSET);
					}
					break;
					
				case 82:
					// #line 265
					{
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 83:
					// #line 157
					{
						return (Tokens.T_THROW);
					}
					break;
					
				case 84:
					// #line 441
					{
						return (Tokens.T_FINAL);
					}
					break;
					
				case 85:
					// #line 457
					{
						return (Tokens.T_UNSET);
					}
					break;
					
				case 86:
					// #line 129
					{
						return (Tokens.T_CONST);
					}
					break;
					
				case 87:
					// #line 321
					{
						return (Tokens.T_CLONE);
					}
					break;
					
				case 88:
					// #line 257
					{
						return (Tokens.T_CLASS);
					}
					break;
					
				case 89:
					// #line 149
					{
						return (Tokens.T_CATCH);
					}
					break;
					
				case 90:
					// #line 141
					{
						return (Tokens.T_YIELD);
					}
					break;
					
				case 91:
					// #line 469
					{
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 92:
					// #line 177
					{
						return (Tokens.T_WHILE);
					}
					break;
					
				case 93:
					// #line 237
					{
						return (Tokens.T_BREAK);
					}
					break;
					
				case 94:
					// #line 253
					{
						return (Tokens.T_PRINT);
					}
					break;
					
				case 95:
					// #line 329
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 96:
					// #line 772
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
					// #line 193
					{
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 98:
					// #line 165
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 99:
					// #line 433
					{
						return (Tokens.T_STATIC);
					}
					break;
					
				case 100:
					// #line 221
					{
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 101:
					// #line 133
					{
						return (Tokens.T_RETURN);
					}
					break;
					
				case 102:
					// #line 389
					{
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 103:
					// #line 453
					{
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 104:
					// #line 333
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 105:
					// #line 349
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 106:
					// #line 269
					{
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 107:
					// #line 361
					{
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 108:
					// #line 233
					{
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 109:
					// #line 205
					{
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 110:
					// #line 153
					{
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 111:
					// #line 197
					{
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 112:
					// #line 369
					{
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 113:
					// #line 445
					{
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 114:
					// #line 353
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 115:
					// #line 341
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 116:
					// #line 681
					{
						return (Tokens.T_DIR);
					}
					break;
					
				case 117:
					// #line 181
					{
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 118:
					// #line 125
					{
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 119:
					// #line 241
					{
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 120:
					// #line 473
					{
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 121:
					// #line 437
					{
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 122:
					// #line 345
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 123:
					// #line 337
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 124:
					// #line 677
					{
						return (Tokens.T_FILE);
					}
					break;
					
				case 125:
					// #line 673
					{
						return (Tokens.T_LINE);
					}
					break;
					
				case 126:
					// #line 225
					{
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 127:
					// #line 261
					{
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 128:
					// #line 385
					{
						return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 129:
					// #line 377
					{
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 130:
					// #line 449
					{
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 131:
					// #line 661
					{
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 132:
					// #line 657
					{
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 133:
					// #line 209
					{
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 134:
					// #line 201
					{
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 135:
					// #line 213
					{
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 136:
					// #line 273
					{
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 137:
					// #line 137
					{
						return Tokens.T_YIELD_FROM;
					}
					break;
					
				case 138:
					// #line 669
					{
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 139:
					// #line 365
					{
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 140:
					// #line 373
					{
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 141:
					// #line 665
					{
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 142:
					// #line 685
					{
						return (Tokens.T_NS_C);
					}
					break;
					
				case 143:
					// #line 416
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 144:
					// #line 84
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 145:
					// #line 895
					{ yymore(); break; }
					break;
					
				case 146:
					// #line 893
					{ yymore(); break; }
					break;
					
				case 147:
					// #line 891
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 148:
					// #line 894
					{ yymore(); break; }
					break;
					
				case 149:
					// #line 890
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 150:
					// #line 889
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 151:
					// #line 888
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 152:
					// #line 81
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 153:
					// #line 839
					{ yymore(); break; }
					break;
					
				case 154:
					// #line 838
					{ yymore(); break; }
					break;
					
				case 155:
					// #line 840
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 156:
					// #line 837
					{ yymore(); break; }
					break;
					
				case 157:
					// #line 905
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 903
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 901
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 160:
					// #line 904
					{ yymore(); break; }
					break;
					
				case 161:
					// #line 900
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 162:
					// #line 899
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 163:
					// #line 898
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 164:
					// #line 914
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 913
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 911
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 825
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
					// #line 912
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 909
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 170:
					// #line 908
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 171:
					// #line 907
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 172:
					// #line 295
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 173:
					// #line 290
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 174:
					// #line 286
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 175:
					// #line 626
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 176:
					// #line 618
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 177:
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
					
				case 178:
					// #line 759
					{ yymore(); break; }
					break;
					
				case 179:
					// #line 761
					{ yymore(); break; }
					break;
					
				case 180:
					// #line 760
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 181:
					// #line 96
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 182:
					// #line 754
					{ yymore(); break; }
					break;
					
				case 183:
					// #line 756
					{ yymore(); break; }
					break;
					
				case 184:
					// #line 755
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 185:
					// #line 919
					{ yymore(); break; }
					break;
					
				case 186:
					// #line 100
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 187:
					// #line 918
					{ yymore(); break; }
					break;
					
				case 188:
					// #line 916
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 189:
					// #line 917
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 190:
					// #line 731
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 191:
					// #line 645
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 192:
					// #line 726
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 193:
					// #line 649
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 194:
					// #line 812
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 195:
					// #line 803
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						if (GetTokenString(intern: false) != _hereDocLabel) 
							_errors.Error(_tokenPosition, Devsense.PHP.Errors.FatalErrors.SyntaxError, "Incorrect heredoc end label: " + _hereDocLabel);
						_yyless(LabelTrailLength());
						_tokenSemantics.Object = _hereDocLabel;
						return Tokens.T_END_HEREDOC;
					}
					break;
					
				case 196:
					// #line 834
					{ yymore(); break; }
					break;
					
				case 197:
					// #line 816
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
					// #line 428
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 199:
					// #line 422
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 200:
					// #line 401
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 201:
					// #line 425
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 202:
					// #line 426
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 203:
					// #line 424
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 204:
					// #line 406
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 205:
					// #line 411
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 206:
					// #line 881
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 207:
					// #line 870
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 208:
					// #line 864
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 209:
					// #line 859
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 210:
					// #line 842
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 211:
					// #line 853
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 212:
					// #line 847
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 213:
					// #line 875
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
				case 244: goto case 193;
				case 245: goto case 206;
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
				case 410: goto case 9;
				case 426: goto case 9;
				case 437: goto case 9;
				case 440: goto case 9;
				case 442: goto case 9;
				case 444: goto case 9;
				case 446: goto case 9;
				case 447: goto case 9;
				case 448: goto case 9;
				case 449: goto case 9;
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
			AcceptConditions.Accept, // 410
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
			AcceptConditions.Accept, // 426
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
			AcceptConditions.Accept, // 437
			AcceptConditions.NotAccept, // 438
			AcceptConditions.NotAccept, // 439
			AcceptConditions.Accept, // 440
			AcceptConditions.NotAccept, // 441
			AcceptConditions.Accept, // 442
			AcceptConditions.NotAccept, // 443
			AcceptConditions.Accept, // 444
			AcceptConditions.NotAccept, // 445
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
			1, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 
			107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 89, 120, 121, 
			20, 70, 122, 21, 123, 59, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 
			134, 135, 136, 23, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 
			149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 
			165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 
			181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 
			197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 
			213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 53, 226, 227, 
			228, 229, 230, 231, 232, 67, 233, 234, 235, 236, 237, 238, 72, 80, 239, 240, 
			76, 241, 50, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 
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
			463, 9, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 216, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 446, 641, 641, 641, 641, 447, 641, 448, 641, 641, 641, 641, 449, -1, 450, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 451, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 243, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, 32, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, -1, 52, 52, 52, 52, 52, 52, 52, 52, -1, -1, 52, 52, -1, -1, -1, -1, 52, -1, -1, -1, 52, 52, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, 52, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 628, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 69, -1, -1, -1, 69, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, 69, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, 70, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 334, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 354, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 349, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, 349, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, 349, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 632, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 557, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 649, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
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
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, 191, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, 193, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, -1, 195, 195, 195, 195, 195, 195, 195, 195, -1, -1, 195, 195, -1, -1, -1, -1, 195, -1, -1, -1, 195, 195, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, 195, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, -1, 197, 197, 197, 197, 197, 197, 197, 197, -1, -1, 197, 197, -1, -1, -1, -1, 197, -1, -1, -1, 197, 197, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 418, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, 422, -1, 209, 209, -1, -1, -1, -1, 209, -1, -1, -1, 209, 209, 209, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, 209, 209, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 239, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, -1, -1, 167, -1, -1, -1, -1, -1, 167, -1, 397, -1, 167, 167, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 22, 452, 641, 641, 641, 453, 641, 641, 641, -1, 575, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, 288, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, 222, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 96, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, 150, 150, -1, -1, 150, -1, -1, -1, -1, -1, 150, -1, -1, -1, 150, 150, 150, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 151, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 154, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1 },
			{ -1, -1, -1, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, -1, 162, 162, 162, 162, 162, 162, 162, 162, -1, -1, 162, -1, -1, -1, -1, -1, 162, -1, -1, -1, 162, 162, 162, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 163, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, 170, -1, -1, -1, -1, -1, 170, -1, -1, -1, 170, 170, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, -1, 405, 405, 405, 405, 405, 405, 405, 405, -1, -1, 405, 405, -1, -1, -1, -1, 405, -1, -1, -1, 405, 405, 405, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, 176, 405, 405, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, -1, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, 232, -1, 232, 232, 232, 232, 232 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 188, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, -1, 52, 52, 52, 52, 52, 52, 52, 52, -1, -1, 52, -1, -1, -1, -1, -1, 52, -1, -1, -1, 52, 52, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 412, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 413, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, 193, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 236, -1, -1, -1, 236, 236, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, 236, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, 236, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, 202, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, 197, -1, 197, 197, 197, 197, 197, 197, 197, 197, -1, -1, 197, -1, -1, -1, -1, -1, 197, -1, 416, -1, 197, 197, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 455, 641, 266, 641, 641, 641, 641, 641, 641, 23, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, 244, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, -1, -1, 209, -1, -1, -1, -1, -1, 209, -1, -1, -1, 209, 209, 209, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 456, 641, 641, 641, 24, 578, 641, 269, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 8, 9, 426, 219, 437, 242, 248, 440, 442, 574, 251, 603, 622, 633, 639, 10, 641, 254, 641, 643, 257, 641, 644, 645, 218, 241, 641, 11, 12, 247, 13, 250, 444, 253, 10, 256, 641, 646, 641, 256, 259, 262, 14, 265, 268, 271, 274, 277, 280, 283, 286, 256, 15, 16, 256, 220, 11, 10, 256, 17, 18, 289, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, 258, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, 32, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 25, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 427, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 261, -1, 264, 267, -1, 428, -1, 270, 273, 276, -1, -1, -1, -1, 279, -1, -1, 282, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 285, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 278, 641, 641, 641, 26, 577, 641, 641, -1, 641, 641, 641, 641, 604, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, 222, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 581, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 35, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 54, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 429, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 55, 641, -1, 641, 475, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 56, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 57, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 301, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 58, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 430, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 59, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 303, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 60, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 438, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 63, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, 431, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 71, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 72, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 74, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 75, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 76, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 432, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 77, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 79, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 433, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 80, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 81, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 82, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 83, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, -1, 325, 325, 325, 325, 325, 325, 325, 325, -1, -1, 325, -1, -1, -1, -1, -1, 325, -1, 309, -1, 325, 325, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, 435 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 84, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 95, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 85, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 439, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 86, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 87, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 88, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 89, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 90, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 441, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 91, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 325, 96, 325, 325, 325, 325, 325, 325, 325, 325, -1, -1, 325, 325, -1, -1, -1, -1, 325, -1, -1, -1, 325, 325, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, 325, 224, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 92, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, -1, 345, 345, 345, 345, 345, 345, 345, 345, -1, -1, 345, -1, -1, -1, -1, -1, 345, -1, -1, -1, 345, 345, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 93, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 94, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, 95, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 97, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 98, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 99, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 100, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 101, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 102, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, 105, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 103, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, -1, 345, 345, 345, 345, 345, 345, 345, 345, -1, -1, 345, 345, -1, -1, -1, -1, 345, -1, -1, -1, 345, 345, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, 345, -1, -1, -1, 369, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 106, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, -1, 347, 347, 347, 347, 347, 347, 347, 347, -1, -1, 347, 347, -1, -1, -1, -1, 347, -1, -1, -1, 347, 347, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, 347, -1, -1, -1, -1, -1, 369 },
			{ -1, -1, -1, 107, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 108, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 445, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 109, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 110, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, 114, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 111, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 112, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 113, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, 115, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 116, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 117, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 118, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, 105, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 119, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 96, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 224, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 120, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, 122, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 121, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, 123, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 124, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 125, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 126, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 137, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 127, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 144, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 383, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 146, 145, 145, 145, 145, 145, 145, 145, 147, 225, 145 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 128, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148 },
			{ -1, -1, -1, 129, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 152, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 154, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 387, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 226, 153, 153, 153, 153, 155 },
			{ -1, -1, -1, 641, 641, 641, 641, 130, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156, 156 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 131, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 152, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 391, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 158, 157, 157, 157, 157, 157, 157, 159, 157, 227, 157 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 132, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160 },
			{ -1, -1, -1, 133, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 215, 152, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 165, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 395, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 166, 164, 164, 164, 164, 228, 164, 164, 164, 229, 164 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 134, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 135, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 136, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 138, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 172, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 10, 173, 173, 173, 173, 173, 173, 173, 173, 230, 172, 173, 172, 172, 172, 172, 172, 173, 172, 10, 172, 173, 173, 173, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 10, 172, 172, 172, 172, 172 },
			{ -1, -1, -1, 139, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 175, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 231, 175, 231, 231, 231, 231, 231, 231, 231, 231, 175, 175, 231, 175, 175, 175, 175, 175, 231, 175, 175, 175, 231, 231, 231, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175 },
			{ -1, -1, -1, 140, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 141, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 177, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 142, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 181, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 143, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 190, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 12, 641, 641, 641, 641, 641, 641, 641, 641, 190, 190, 641, 191, 12, 190, 12, 190, 641, 190, 12, 190, 641, 641, 641, 190, 190, 190, 12, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 235, 191, 12, 192, 190, 190, 234, 12 },
			{ 1, 7, 194, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 194, 195, 195, 195, 195, 195, 195, 195, 195, 194, 194, 195, 194, 194, 194, 194, 194, 195, 194, 194, 194, 195, 195, 195, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194 },
			{ 240, 152, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196 },
			{ 1, 7, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 200, 199, 198, 198, 198, 198, 198, 237, 198, 201, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198 },
			{ 1, 7, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 204, 198, 198, 198, 198, 237, 198, 201, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198 },
			{ 1, 7, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 205, 237, 198, 201, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 199, 198, 198, 198, 198, 198 },
			{ 1, 7, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 238, 206, 206, 206, 206, 206, 206, 206, 207, 245, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 423, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 212, 212, -1, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, -1, 212, 212, 212, 212, 212, 212, 212, 212, -1, -1, 212, -1, -1, -1, -1, -1, 212, -1, -1, -1, 212, 212, 212, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 238, 206, 206, 206, 206, 206, 206, 213, 206, 245, 206 },
			{ 1, 1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 238, 206, 206, 206, 206, 206, 206, 206, 206, 245, 206 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 260, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 434, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, -1, 347, 347, 347, 347, 347, 347, 347, 347, -1, -1, 347, -1, -1, -1, -1, -1, 347, -1, -1, -1, 347, 347, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 263, 641, 641, -1, 641, 641, 454, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 272, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 457, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 576, 641, 641, 641, 275, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 281, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 284, 606, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 470, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 287, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 290, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 471, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 292, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 623, 641, 641, 641, 641, 472, 641, 473, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 474, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 476, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 579, 641, 641, 607, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 477, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 634, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 481, 641, 641, 641, 641, -1, 641, 482, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 483, 641, 641, 641, 641, 641, 641, 294, 641, 641, 651, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 584, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 608, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 484, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 585, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 485, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 296, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 298, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 582, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 624, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 489, 641, 641, 641, 641, 641, 641, 636, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 490, 491, 492, 493, 641, 635, 641, 641, 641, 641, 587, -1, 494, 641, 588, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 300, 641, 589, 496, 641, 641, 641, 641, 497, 641, 641, 641, -1, 641, 641, 641, 498, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 302, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 609, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 499, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 304, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 306, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 308, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 310, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 500, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 312, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 611, 641, 641, 641, 641, 641, 641, 314, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 316, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 318, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 320, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 504, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 322, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 324, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 326, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 328, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 330, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 640, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 642, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 507, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 508, 641, 641, 641, 641, 590, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 509, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 591, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 511, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 332, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 513, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 595, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 593, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 516, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 616, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 520, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 336, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 338, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 340, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 342, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 344, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 524, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 525, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 597, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 526, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 346, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 529, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 599, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 531, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 348, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 533, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 350, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 352, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 356, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 600, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 536, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 358, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 360, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 362, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 538, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 618, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 540, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 541, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 620, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 364, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 543, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 544, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 545, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 366, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 368, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 370, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 372, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 374, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 376, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 553, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 554, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 378, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 380, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 382, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 558, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 559, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 384, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 386, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 388, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 652, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 560, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 390, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 561, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 601, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 392, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 394, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 562, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 396, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 398, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 563, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 400, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 565, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 568, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 569, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 402, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 404, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 406, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 570, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 571, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 408, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 572, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 573, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 410, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 605, 641, 641, 641, 458, -1, 641, 459, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 583, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 479, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 486, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 478, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 626, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 487, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 488, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 505, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 613, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 502, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 627, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 514, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 614, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 592, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 512, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 650, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 527, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 528, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 532, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 619, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 530, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 535, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 598, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 551, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 542, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 547, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 564, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 566, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 460, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 461, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 625, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 480, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 495, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 612, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 503, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 515, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 615, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 596, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 518, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 647, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 617, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 537, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 534, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 539, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 552, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 548, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 555, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 567, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 462, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 586, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 506, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 610, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 517, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 522, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 519, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 594, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 546, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 549, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 556, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 463, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 501, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 510, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 629, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 521, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 550, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 464, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 523, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 648, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 580, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 465, 641, 641, 641, 466, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 467, 641, 641, 641, 468, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 469, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 630, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 631, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 602, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 638, 641, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 641, 637, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 641, 641, 641, 641, 641, 641, 641, 641, 621, 641, 641, 641, 641, -1, 641, 641, 641, 641, 641, 641, 641, 641, -1, -1, 641, 641, -1, -1, -1, -1, 641, -1, -1, -1, 641, 641, 641, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 641, 641, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  249,
			  381,
			  385,
			  389,
			  393,
			  399,
			  401,
			  403,
			  407,
			  409,
			  185,
			  411,
			  414,
			  415,
			  417,
			  419,
			  420,
			  421,
			  424,
			  425
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 653);
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

