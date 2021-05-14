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
					// #line 94
					{ 
						return ProcessEof(Tokens.T_INLINE_HTML);
					}
					break;
					
				case 3:
					// #line 745
					{
						yymore(); break;
					}
					break;
					
				case 4:
					// #line 732
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
					// #line 712
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
					// #line 722
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
					// #line 80
					{
						return Tokens.EOF;
					}
					break;
					
				case 8:
					// #line 616
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 766
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 770
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 11:
					// #line 297
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 12:
					// #line 660
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 13:
					// #line 946
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 14:
					// #line 324
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 15:
					// #line 621
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 629
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 922
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 912
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 861
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 332
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 786
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 167
					{
						return Identifier(Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 191
					{
						return Identifier(Tokens.T_DO);
					}
					break;
					
				case 24:
					// #line 123
					{
						return Identifier(Tokens.T_FN);
					}
					break;
					
				case 25:
					// #line 596
					{
						return Identifier(Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 26:
					// #line 139
					{
						return Tokens.T_ATTRIBUTE;
					}
					break;
					
				case 27:
					// #line 223
					{
						return Identifier(Tokens.T_AS);
					}
					break;
					
				case 28:
					// #line 500
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 29:
					// #line 287
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 30:
					// #line 536
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 31:
					// #line 612
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 32:
					// #line 528
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 33:
					// #line 676
					{
						return ProcessRealNumber();
					}
					break;
					
				case 34:
					// #line 320
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 35:
					// #line 556
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 36:
					// #line 776
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 37:
					// #line 552
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 38:
					// #line 544
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 39:
					// #line 540
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 40:
					// #line 480
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 41:
					// #line 512
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 42:
					// #line 532
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 43:
					// #line 496
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 44:
					// #line 516
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 45:
					// #line 524
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 46:
					// #line 608
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 47:
					// #line 560
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 48:
					// #line 572
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 49:
					// #line 592
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 50:
					// #line 576
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 51:
					// #line 588
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 52:
					// #line 580
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 53:
					// #line 791
					{
						return ProcessVariable();
					}
					break;
					
				case 54:
					// #line 584
					{
						return (Tokens.T_COALESCE_EQUAL);
					}
					break;
					
				case 55:
					// #line 292
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 56:
					// #line 604
					{
						return Identifier(Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 57:
					// #line 151
					{
						return Identifier(Tokens.T_TRY);
					}
					break;
					
				case 58:
					// #line 119
					{
						return Identifier(Tokens.T_EXIT);
					}
					break;
					
				case 59:
					// #line 195
					{
						return Identifier(Tokens.T_FOR);
					}
					break;
					
				case 60:
					// #line 336
					{
						return Identifier(Tokens.T_NEW);
					}
					break;
					
				case 61:
					// #line 400
					{
						return Identifier(Tokens.T_USE);
					}
					break;
					
				case 62:
					// #line 600
					{
						return Identifier(Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 63:
					// #line 568
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 64:
					// #line 328
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 65:
					// #line 344
					{
						return Identifier(Tokens.T_VAR);
					}
					break;
					
				case 66:
					// #line 548
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 67:
					// #line 504
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 68:
					// #line 508
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 69:
					// #line 520
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 70:
					// #line 564
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 71:
					// #line 664
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 72:
					// #line 656
					{
						return ProcessOctalNumber();
					}
					break;
					
				case 73:
					// #line 652
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 74:
					// #line 115
					{ 
						return Identifier(Tokens.T_EXIT);
					}
					break;
					
				case 75:
					// #line 259
					{
						return Identifier(Tokens.T_ECHO);
					}
					break;
					
				case 76:
					// #line 179
					{
						return Identifier(Tokens.T_ELSE);
					}
					break;
					
				case 77:
					// #line 376
					{
						return Identifier(Tokens.T_EVAL);
					}
					break;
					
				case 78:
					// #line 239
					{
						return Identifier(Tokens.T_CASE);
					}
					break;
					
				case 79:
					// #line 484
					{
						return Identifier(Tokens.T_LIST);
					}
					break;
					
				case 80:
					// #line 255
					{
						return Identifier(Tokens.T_GOTO);
					}
					break;
					
				case 81:
					// #line 781
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 82:
					// #line 175
					{
						return Identifier(Tokens.T_ENDIF);
					}
					break;
					
				case 83:
					// #line 416
					{
						return Identifier(Tokens.T_EMPTY);
					}
					break;
					
				case 84:
					// #line 412
					{
						return Identifier(Tokens.T_ISSET);
					}
					break;
					
				case 85:
					// #line 275
					{
						return Identifier(Tokens.T_TRAIT);
					}
					break;
					
				case 86:
					// #line 163
					{
						return Identifier(Tokens.T_THROW);
					}
					break;
					
				case 87:
					// #line 460
					{
						return Identifier(Tokens.T_FINAL);
					}
					break;
					
				case 88:
					// #line 476
					{
						return Identifier(Tokens.T_UNSET);
					}
					break;
					
				case 89:
					// #line 131
					{
						return Identifier(Tokens.T_CONST);
					}
					break;
					
				case 90:
					// #line 340
					{
						return Identifier(Tokens.T_CLONE);
					}
					break;
					
				case 91:
					// #line 267
					{
						return Identifier(Tokens.T_CLASS);
					}
					break;
					
				case 92:
					// #line 155
					{
						return Identifier(Tokens.T_CATCH);
					}
					break;
					
				case 93:
					// #line 147
					{
						return Identifier(Tokens.T_YIELD);
					}
					break;
					
				case 94:
					// #line 231
					{
						return Identifier(Tokens.T_MATCH);
					}
					break;
					
				case 95:
					// #line 488
					{
						return Identifier(Tokens.T_ARRAY);
					}
					break;
					
				case 96:
					// #line 183
					{
						return Identifier(Tokens.T_WHILE);
					}
					break;
					
				case 97:
					// #line 247
					{
						return Identifier(Tokens.T_BREAK);
					}
					break;
					
				case 98:
					// #line 263
					{
						return Identifier(Tokens.T_PRINT);
					}
					break;
					
				case 99:
					// #line 348
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 100:
					// #line 795
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
					
				case 101:
					// #line 199
					{
						return Identifier(Tokens.T_ENDFOR);
					}
					break;
					
				case 102:
					// #line 171
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 103:
					// #line 452
					{
						return Identifier(Tokens.T_STATIC);
					}
					break;
					
				case 104:
					// #line 227
					{
						return Identifier(Tokens.T_SWITCH);
					}
					break;
					
				case 105:
					// #line 135
					{
						return Identifier(Tokens.T_RETURN);
					}
					break;
					
				case 106:
					// #line 408
					{
						return Identifier(Tokens.T_GLOBAL);
					}
					break;
					
				case 107:
					// #line 472
					{
						return Identifier(Tokens.T_PUBLIC);
					}
					break;
					
				case 108:
					// #line 352
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 109:
					// #line 368
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 110:
					// #line 279
					{
						return Identifier(Tokens.T_EXTENDS);
					}
					break;
					
				case 111:
					// #line 380
					{
						return Identifier(Tokens.T_INCLUDE);
					}
					break;
					
				case 112:
					// #line 243
					{
						return Identifier(Tokens.T_DEFAULT);
					}
					break;
					
				case 113:
					// #line 211
					{
						return Identifier(Tokens.T_DECLARE);
					}
					break;
					
				case 114:
					// #line 159
					{
						return Identifier(Tokens.T_FINALLY);
					}
					break;
					
				case 115:
					// #line 203
					{
						return Identifier(Tokens.T_FOREACH);
					}
					break;
					
				case 116:
					// #line 388
					{
						return Identifier(Tokens.T_REQUIRE);
					}
					break;
					
				case 117:
					// #line 464
					{
						return Identifier(Tokens.T_PRIVATE);
					}
					break;
					
				case 118:
					// #line 372
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 119:
					// #line 360
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 120:
					// #line 704
					{
						return Identifier(Tokens.T_DIR);
					}
					break;
					
				case 121:
					// #line 187
					{
						return Identifier(Tokens.T_ENDWHILE);
					}
					break;
					
				case 122:
					// #line 127
					{
						return Identifier(Tokens.T_FUNCTION);
					}
					break;
					
				case 123:
					// #line 251
					{
						return Identifier(Tokens.T_CONTINUE);
					}
					break;
					
				case 124:
					// #line 492
					{
						return Identifier(Tokens.T_CALLABLE);
					}
					break;
					
				case 125:
					// #line 456
					{
						return Identifier(Tokens.T_ABSTRACT);
					}
					break;
					
				case 126:
					// #line 364
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 127:
					// #line 356
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 128:
					// #line 700
					{
						return Identifier(Tokens.T_FILE);
					}
					break;
					
				case 129:
					// #line 696
					{
						return Identifier(Tokens.T_LINE);
					}
					break;
					
				case 130:
					// #line 235
					{
						return Identifier(Tokens.T_ENDSWITCH);
					}
					break;
					
				case 131:
					// #line 271
					{
						return Identifier(Tokens.T_INTERFACE);
					}
					break;
					
				case 132:
					// #line 404
					{
						return Identifier(Tokens.T_INSTEADOF);
					}
					break;
					
				case 133:
					// #line 396
					{
						return Identifier(Tokens.T_NAMESPACE);
					}
					break;
					
				case 134:
					// #line 468
					{
						return Identifier(Tokens.T_PROTECTED);
					}
					break;
					
				case 135:
					// #line 684
					{
						return Identifier(Tokens.T_TRAIT_C);
					}
					break;
					
				case 136:
					// #line 680
					{
						return Identifier(Tokens.T_CLASS_C);
					}
					break;
					
				case 137:
					// #line 215
					{
						return Identifier(Tokens.T_ENDDECLARE);
					}
					break;
					
				case 138:
					// #line 207
					{
						return Identifier(Tokens.T_ENDFOREACH);
					}
					break;
					
				case 139:
					// #line 219
					{
						return Identifier(Tokens.T_INSTANCEOF);
					}
					break;
					
				case 140:
					// #line 283
					{
						return Identifier(Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 141:
					// #line 143
					{
						return Identifier(Tokens.T_YIELD_FROM);
					}
					break;
					
				case 142:
					// #line 692
					{
						return Identifier(Tokens.T_METHOD_C);
					}
					break;
					
				case 143:
					// #line 384
					{
						return Identifier(Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 144:
					// #line 392
					{
						return Identifier(Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 145:
					// #line 688
					{
						return Identifier(Tokens.T_FUNC_C);
					}
					break;
					
				case 146:
					// #line 708
					{
						return Identifier(Tokens.T_NS_C);
					}
					break;
					
				case 147:
					// #line 435
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return Identifier(Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 148:
					// #line 86
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 149:
					// #line 920
					{ yymore(); break; }
					break;
					
				case 150:
					// #line 918
					{ yymore(); break; }
					break;
					
				case 151:
					// #line 916
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 152:
					// #line 919
					{ yymore(); break; }
					break;
					
				case 153:
					// #line 915
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 154:
					// #line 914
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 155:
					// #line 913
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 156:
					// #line 83
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 157:
					// #line 864
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 863
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 865
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 160:
					// #line 862
					{ yymore(); break; }
					break;
					
				case 161:
					// #line 930
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 928
					{ yymore(); break; }
					break;
					
				case 163:
					// #line 926
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 164:
					// #line 929
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 925
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 166:
					// #line 924
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 167:
					// #line 923
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 168:
					// #line 939
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 938
					{ yymore(); break; }
					break;
					
				case 170:
					// #line 936
					{ yymore(); break; }
					break;
					
				case 171:
					// #line 850
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(_processDoubleQuotedString) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 172:
					// #line 937
					{ yymore(); break; }
					break;
					
				case 173:
					// #line 934
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 174:
					// #line 933
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 175:
					// #line 932
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 176:
					// #line 314
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 177:
					// #line 309
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 178:
					// #line 301
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 179:
					// #line 305
					{
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 180:
					// #line 645
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 181:
					// #line 637
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 182:
					// #line 106
					{
						if(TokenLength > 0)
						{
							SetDocBlock(); 
							return Tokens.T_DOC_COMMENT; 
						}
						return Tokens.EOF;
					}
					break;
					
				case 183:
					// #line 782
					{ yymore(); break; }
					break;
					
				case 184:
					// #line 784
					{ yymore(); break; }
					break;
					
				case 185:
					// #line 783
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 186:
					// #line 98
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 187:
					// #line 777
					{ yymore(); break; }
					break;
					
				case 188:
					// #line 779
					{ yymore(); break; }
					break;
					
				case 189:
					// #line 778
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 190:
					// #line 944
					{ yymore(); break; }
					break;
					
				case 191:
					// #line 102
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 192:
					// #line 943
					{ yymore(); break; }
					break;
					
				case 193:
					// #line 941
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 194:
					// #line 942
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 195:
					// #line 754
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 196:
					// #line 668
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 197:
					// #line 749
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 198:
					// #line 672
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 199:
					// #line 837
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 200:
					// #line 826
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
					
				case 201:
					// #line 859
					{ yymore(); break; }
					break;
					
				case 202:
					// #line 841
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(null) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 203:
					// #line 447
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 204:
					// #line 444
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 205:
					// #line 441
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 206:
					// #line 420
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 207:
					// #line 445
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 208:
					// #line 443
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 209:
					// #line 425
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 210:
					// #line 430
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 211:
					// #line 906
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 212:
					// #line 895
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 213:
					// #line 889
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 214:
					// #line 884
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 215:
					// #line 867
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 216:
					// #line 878
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 217:
					// #line 872
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 218:
					// #line 900
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 221: goto case 3;
				case 222: goto case 6;
				case 223: goto case 8;
				case 224: goto case 9;
				case 225: goto case 10;
				case 226: goto case 12;
				case 227: goto case 21;
				case 228: goto case 33;
				case 229: goto case 44;
				case 230: goto case 100;
				case 231: goto case 150;
				case 232: goto case 158;
				case 233: goto case 162;
				case 234: goto case 169;
				case 235: goto case 170;
				case 236: goto case 176;
				case 237: goto case 180;
				case 238: goto case 190;
				case 239: goto case 193;
				case 240: goto case 195;
				case 241: goto case 196;
				case 242: goto case 198;
				case 243: goto case 203;
				case 244: goto case 211;
				case 247: goto case 8;
				case 248: goto case 9;
				case 249: goto case 21;
				case 250: goto case 33;
				case 251: goto case 176;
				case 252: goto case 198;
				case 253: goto case 211;
				case 255: goto case 8;
				case 256: goto case 9;
				case 257: goto case 198;
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
				case 292: goto case 8;
				case 293: goto case 9;
				case 295: goto case 8;
				case 296: goto case 9;
				case 298: goto case 8;
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
				case 421: goto case 9;
				case 443: goto case 9;
				case 454: goto case 9;
				case 457: goto case 9;
				case 459: goto case 9;
				case 461: goto case 9;
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
				case 669: goto case 9;
				case 670: goto case 9;
				case 671: goto case 9;
				case 672: goto case 9;
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
			AcceptConditions.Accept, // 170
			AcceptConditions.AcceptOnStart, // 171
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
			AcceptConditions.Accept, // 201
			AcceptConditions.AcceptOnStart, // 202
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
			AcceptConditions.Accept, // 218
			AcceptConditions.NotAccept, // 219
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
			AcceptConditions.Accept, // 244
			AcceptConditions.NotAccept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.Accept, // 248
			AcceptConditions.Accept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.Accept, // 251
			AcceptConditions.Accept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.NotAccept, // 254
			AcceptConditions.Accept, // 255
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
			AcceptConditions.Accept, // 293
			AcceptConditions.NotAccept, // 294
			AcceptConditions.Accept, // 295
			AcceptConditions.Accept, // 296
			AcceptConditions.NotAccept, // 297
			AcceptConditions.Accept, // 298
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
			AcceptConditions.Accept, // 421
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
			AcceptConditions.NotAccept, // 439
			AcceptConditions.NotAccept, // 440
			AcceptConditions.NotAccept, // 441
			AcceptConditions.NotAccept, // 442
			AcceptConditions.Accept, // 443
			AcceptConditions.NotAccept, // 444
			AcceptConditions.NotAccept, // 445
			AcceptConditions.NotAccept, // 446
			AcceptConditions.NotAccept, // 447
			AcceptConditions.NotAccept, // 448
			AcceptConditions.NotAccept, // 449
			AcceptConditions.NotAccept, // 450
			AcceptConditions.NotAccept, // 451
			AcceptConditions.NotAccept, // 452
			AcceptConditions.NotAccept, // 453
			AcceptConditions.Accept, // 454
			AcceptConditions.NotAccept, // 455
			AcceptConditions.NotAccept, // 456
			AcceptConditions.Accept, // 457
			AcceptConditions.NotAccept, // 458
			AcceptConditions.Accept, // 459
			AcceptConditions.NotAccept, // 460
			AcceptConditions.Accept, // 461
			AcceptConditions.NotAccept, // 462
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
			AcceptConditions.Accept, // 669
			AcceptConditions.Accept, // 670
			AcceptConditions.Accept, // 671
			AcceptConditions.Accept, // 672
		};
		
		private static int[] colMap = new int[]
		{
			32, 32, 32, 32, 32, 32, 32, 32, 32, 38, 19, 32, 32, 60, 32, 32, 
			32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 
			38, 48, 63, 15, 64, 50, 51, 65, 37, 39, 45, 47, 54, 28, 35, 44, 
			57, 58, 59, 59, 59, 59, 59, 59, 31, 31, 33, 43, 49, 46, 29, 2, 
			54, 21, 24, 11, 7, 3, 8, 26, 22, 5, 40, 25, 18, 20, 9, 12, 
			27, 42, 14, 13, 6, 10, 36, 23, 4, 17, 30, 16, 34, 61, 53, 41, 
			62, 21, 24, 11, 7, 3, 8, 26, 22, 5, 40, 25, 18, 20, 9, 12, 
			27, 42, 14, 13, 6, 10, 36, 23, 4, 17, 30, 55, 52, 56, 54, 32, 
			0, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 1, 2, 1, 1, 1, 3, 4, 5, 6, 7, 1, 1, 1, 
			1, 1, 1, 1, 8, 9, 10, 10, 10, 10, 1, 10, 1, 1, 1, 11, 
			1, 12, 1, 1, 13, 1, 14, 1, 1, 15, 1, 1, 16, 17, 18, 1, 
			1, 1, 1, 1, 1, 19, 1, 1, 10, 10, 10, 20, 10, 10, 10, 1, 
			1, 10, 1, 1, 1, 1, 1, 21, 22, 23, 10, 10, 24, 10, 10, 10, 
			10, 25, 10, 10, 10, 10, 10, 26, 10, 10, 10, 10, 10, 27, 10, 10, 
			10, 10, 10, 1, 1, 28, 10, 10, 10, 10, 10, 10, 1, 1, 10, 29, 
			10, 10, 10, 10, 30, 10, 1, 1, 10, 10, 10, 10, 10, 10, 1, 1, 
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 1, 10, 10, 
			10, 10, 10, 10, 1, 31, 32, 1, 1, 1, 1, 1, 1, 33, 33, 1, 
			1, 34, 35, 1, 1, 1, 1, 1, 36, 1, 37, 38, 1, 1, 1, 1, 
			39, 40, 1, 1, 1, 1, 1, 41, 42, 1, 1, 43, 44, 1, 45, 1, 
			46, 1, 1, 1, 47, 1, 48, 1, 49, 1, 50, 1, 1, 51, 1, 52, 
			53, 1, 1, 1, 1, 1, 54, 1, 1, 1, 1, 55, 56, 57, 58, 1, 
			59, 1, 60, 1, 61, 1, 62, 63, 64, 65, 66, 67, 1, 68, 69, 70, 
			71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 
			87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 
			103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 
			119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 71, 129, 96, 130, 131, 132, 
			133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 
			149, 150, 151, 152, 25, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 
			164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 
			180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 
			196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 
			212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 
			228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 56, 242, 
			243, 244, 245, 246, 247, 248, 249, 68, 250, 251, 252, 253, 254, 255, 256, 257, 
			258, 77, 259, 53, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 
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
			480, 481, 482, 483, 484, 485, 10, 486, 487, 488, 489, 490, 491, 492, 493, 494, 
			495
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 221, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 261, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 463, 662, 662, 662, 662, 464, 662, 465, 662, 662, 662, -1, -1, 662, 466, -1, 467, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 468, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 267, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, 33, -1, -1, -1, -1, -1, 270, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 12, 12, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 54, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 267, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, 228, 228, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, -1, -1, 326, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, 53, -1, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, 53, -1, -1, -1, -1, 53, -1, -1, -1, 53, 53, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 53, 53, 53, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 648, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, -1, -1, -1, 71, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, 302, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, 71, 71, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, 72, 72, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 306, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, 73, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 345, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 81, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 81, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 81, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 365, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, 366, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, 366, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, 366, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 652, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 576, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 669, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, -1, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, -1, 149, 149, 149, 149, 149, 149, 149, -1, -1, 149 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 153, -1 },
			{ -1, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1 },
			{ -1, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, -1, 161, -1, 161 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1 },
			{ -1, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, -1, 168, 168, 168, -1, 168 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 173, -1 },
			{ -1, -1, -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, -1, -1, 171, 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, -1, -1, 171, 171, -1, -1, -1, -1, 171, -1, -1, -1, 171, 171, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 171, 171, 171, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 420, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, -1, -1, 177, 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, -1, -1, 177, 177, -1, -1, -1, -1, 177, -1, -1, -1, 177, 177, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, 177, 177, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, -1, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 191, 192, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 193, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 239, 238, 238, 238, 238, 238 },
			{ -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, 427, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, 196, 196, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, 427, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, 198, 198, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, -1, -1, 200, 200, -1, 200, 200, 200, 200, 200, 200, 200, 200, -1, -1, 200, 200, -1, -1, -1, -1, 200, -1, -1, -1, 200, 200, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, 200, 200, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, -1, -1, 202, 202, -1, 202, 202, 202, 202, 202, 202, 202, 202, -1, -1, 202, 202, -1, -1, -1, -1, 202, -1, -1, -1, 202, 202, 202, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 202, 202, 202, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 435, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, 216, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, 439, -1, 214, 214, -1, -1, -1, -1, 214, -1, -1, -1, 214, 214, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, 214, 214, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 245, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, -1, -1, 171, 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, -1, -1, 171, -1, -1, -1, -1, -1, 171, -1, 414, -1, 171, 171, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 22, 469, 662, 662, 662, 470, 662, -1, -1, 662, 662, -1, 594, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 267, 302, -1, -1, -1, -1, -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 306, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, 33, -1, -1, -1, -1, -1, 270, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 12, 12, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 267, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1, -1, -1, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, 228, 228, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 100, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1, -1, 154, 154, -1, 154, 154, 154, 154, 154, 154, 154, 154, -1, -1, 154, -1, -1, -1, -1, -1, 154, -1, -1, -1, 154, 154, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 158, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1 },
			{ -1, -1, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, -1, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, -1, -1, 166, -1, -1, -1, -1, -1, 166, -1, -1, -1, 166, 166, 166, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, 174, 174, -1, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, 174, -1, -1, -1, -1, -1, 174, -1, -1, -1, 174, 174, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 175, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 423, 423, 423, 423, 423, 423, 423, 423, 423, 423, 423, 423, -1, 181, 423, 423, -1, 423, 423, 423, 423, 423, 423, 423, 423, -1, -1, 423, 423, -1, -1, -1, -1, 423, -1, -1, -1, 423, 423, 423, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, 423, 423, 423, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, -1, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, 238, -1, 238, 238, 238, 238, 238 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, 53, -1, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, -1, -1, -1, -1, -1, 53, -1, -1, -1, 53, 53, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 428, -1, -1, -1, -1, -1, -1, -1, 429, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 430, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, 427, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, 198, 198, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 242, -1, -1, -1, 242, 242, -1, -1, 242, -1, -1, -1, -1, -1, -1, -1, -1, -1, 242, -1, -1, 242, -1, -1, -1, -1, -1, -1, 242, -1, -1, -1, -1, -1, -1, -1, -1, -1, 428, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 242, 242, 242, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 213, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 254, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, -1, -1, 202, 202, -1, 202, 202, 202, 202, 202, 202, 202, 202, -1, -1, 202, -1, -1, -1, -1, -1, 202, -1, 433, -1, 202, 202, 202, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 472, 662, 275, 662, 662, 662, 662, 662, 662, 23, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 250, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 250, 250, 250, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 429, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 252, 252, 252, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, -1, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, -1, -1, 214, -1, -1, -1, -1, -1, 214, -1, -1, -1, 214, 214, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 215, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 473, 662, 662, 662, 24, 597, 662, 278, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 430, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, 257, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 8, 9, 443, 224, 454, 248, 256, 457, 459, 593, 260, 622, 642, 10, 223, 653, 658, 11, 660, 263, 662, 663, 266, 662, 664, 665, 247, 255, 662, 12, 13, 259, 14, 262, 461, 265, 11, 223, 662, 666, 662, 223, 268, 271, 274, 277, 280, 283, 286, 289, 292, 295, 223, 15, 16, 226, 12, 12, 11, 223, 17, 18, 298, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 25, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 55, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, 273, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, 228, 228, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 287, 662, 662, 662, 27, 596, -1, -1, 662, 662, -1, 662, 662, 662, 662, 623, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 444, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 276, -1, 279, 282, -1, 445, -1, 285, 288, 291, -1, -1, -1, -1, -1, -1, 294, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 300, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 600, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, 264, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, -1, -1, 250, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, 250, 250, 250, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 225, 36, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 56, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 12, 12, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 57, 662, -1, 662, 492, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 58, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 59, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 312, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 60, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 229, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 61, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 62, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 65, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 74, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 455, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 75, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, 448, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 76, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 77, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, -1, -1, -1, 71, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, 71, 71, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 78, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, 72, 72, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 79, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, 73, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 80, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 250, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 250, 250, 250, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 82, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, 228, 228, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 83, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 84, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 449, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 85, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 86, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 450, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 87, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 336, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 88, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 89, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 90, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 342, 342, 342, 342, 342, 342, 342, 342, 342, 342, 342, 342, -1, -1, 342, 342, -1, 342, 342, 342, 342, 342, 342, 342, 342, -1, -1, 342, -1, -1, -1, -1, -1, 342, -1, 326, -1, 342, 342, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, -1, 452 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 91, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, 99, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 92, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 456, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 93, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 453, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 94, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 95, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 96, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 97, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 458, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 98, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 342, 342, 342, 342, 342, 342, 342, 342, 342, 342, 342, 342, -1, -1, 342, 342, 100, 342, 342, 342, 342, 342, 342, 342, 342, -1, -1, 342, 342, -1, -1, -1, -1, 342, -1, -1, -1, 342, 342, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 342, 342, 342, 230, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 101, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, -1, -1, 362, 362, -1, 362, 362, 362, 362, 362, 362, 362, 362, -1, -1, 362, -1, -1, -1, -1, -1, 362, -1, -1, -1, 362, 362, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 102, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 368, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 103, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, 99, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 104, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 105, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 106, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 107, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, 108, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 110, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 111, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 382, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, 109, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 112, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, 362, -1, -1, 362, 362, -1, 362, 362, 362, 362, 362, 362, 362, 362, -1, -1, 362, 362, -1, -1, -1, -1, 362, -1, -1, -1, 362, 362, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, 362, 362, -1, -1, -1, 386, -1, -1 },
			{ -1, -1, -1, 113, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 364, 364, 364, 364, 364, 364, 364, 364, 364, 364, 364, 364, -1, -1, 364, 364, -1, 364, 364, 364, 364, 364, 364, 364, 364, -1, -1, 364, 364, -1, -1, -1, -1, 364, -1, -1, -1, 364, 364, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 364, 364, 364, -1, -1, -1, -1, -1, 386 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 114, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 460, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 115, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 462, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 116, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 117, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, 118, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 120, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 388, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 121, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 390, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 122, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, 119, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 123, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 390, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 124, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 392, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 125, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, 109, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 128, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 100, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 230, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 129, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 388, 126, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 130, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 390, 127, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 131, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 132, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 396, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 133, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 141, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 134, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ 1, 148, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 400, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 150, 149, 149, 149, 149, 149, 149, 149, 151, 231, 149 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 135, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152, 152 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 136, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ 1, 156, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 158, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 404, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 232, 157, 157, 157, 157, 159 },
			{ -1, -1, -1, 137, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 138, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ 1, 156, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 408, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 162, 161, 161, 161, 161, 161, 161, 163, 161, 233, 161 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 139, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 140, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ 220, 156, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 169, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 412, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 170, 168, 168, 168, 168, 234, 168, 168, 168, 235, 168 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 142, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172 },
			{ -1, -1, -1, 143, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 144, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 145, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 176, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 236, 236, 177, 177, 11, 177, 177, 177, 177, 177, 177, 177, 177, 251, 236, 177, 236, 236, 236, 236, 236, 177, 236, 11, 236, 177, 177, 177, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 236, 11, 236, 236, 236, 236, 236 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 146, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 179, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 147, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 180, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 237, 180, 180, 237, 237, 180, 237, 237, 237, 237, 237, 237, 237, 237, 180, 180, 237, 180, 180, 180, 180, 180, 237, 180, 180, 180, 237, 237, 237, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180 },
			{ 1, 182, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 184, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183 },
			{ 1, 186, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 188, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187 },
			{ 1, 7, 195, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 13, 195, 662, 662, 13, 662, 662, 662, 662, 662, 662, 662, 662, 195, 195, 662, 196, 13, 195, 13, 195, 662, 195, 13, 195, 662, 662, 662, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 241, 196, 196, 13, 197, 195, 195, 240, 13 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, 198, 198, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 242, -1, -1, -1, 242, 242, -1, -1, 242, -1, -1, -1, -1, -1, -1, -1, -1, -1, 242, -1, -1, 242, -1, -1, -1, -1, -1, -1, 242, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 242, 242, 242, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 252, 252, 252, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, 257, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 199, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 199, 199, 200, 200, 199, 200, 200, 200, 200, 200, 200, 200, 200, 199, 199, 200, 199, 199, 199, 199, 199, 200, 199, 199, 199, 200, 200, 200, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199 },
			{ 246, 156, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201 },
			{ 1, 7, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 204, 203, 203, 203, 205, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 206, 205, 203, 203, 203, 203, 203, 243, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 205, 203, 203, 203, 203, 203 },
			{ 1, 7, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 204, 203, 203, 203, 205, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 205, 209, 203, 203, 203, 203, 243, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 205, 203, 203, 203, 203, 203 },
			{ 1, 7, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 204, 203, 203, 203, 205, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 205, 203, 203, 203, 203, 210, 243, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 205, 203, 203, 203, 203, 203 },
			{ 1, 7, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 244, 211, 211, 211, 211, 211, 211, 211, 212, 253, 211 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 217, 217, -1, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, -1, -1, 217, 217, -1, 217, 217, 217, 217, 217, 217, 217, 217, -1, -1, 217, -1, -1, -1, -1, -1, 217, -1, -1, -1, 217, 217, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 244, 211, 211, 211, 211, 211, 211, 218, 211, 253, 211 },
			{ 1, 1, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 244, 211, 211, 211, 211, 211, 211, 211, 211, 253, 211 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 269, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 326, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 334, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 451, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 364, 364, 364, 364, 364, 364, 364, 364, 364, 364, 364, 364, -1, -1, 364, 364, -1, 364, 364, 364, 364, 364, 364, 364, 364, -1, -1, 364, -1, -1, -1, -1, -1, 364, -1, -1, -1, 364, 364, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 272, -1, -1, 662, 662, -1, 662, 662, 471, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 281, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 474, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 595, 662, 662, 662, 284, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 394, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 290, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 293, 625, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 488, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 296, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 299, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 489, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 301, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 643, 662, 662, 662, 662, 490, 662, 601, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 491, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 493, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 598, 662, 662, 627, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 494, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 654, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 498, 662, 662, -1, -1, 662, 662, -1, 662, 499, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 500, 662, 662, 662, 662, 662, 662, 303, 662, -1, -1, 662, 671, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 626, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 644, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 501, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 604, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 502, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 305, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 503, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 307, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 602, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 645, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 508, 662, 662, 662, 662, 662, 662, 605, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 509, 510, 511, 512, 662, 655, 662, 662, 662, -1, -1, 662, 607, -1, 513, 662, 608, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 309, 662, 609, 515, 662, 662, 662, 662, 516, 662, -1, -1, 662, 662, -1, 662, 662, 662, 517, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 311, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 628, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 313, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 315, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 317, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 319, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 629, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 321, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 631, 662, 662, 662, 662, 662, 662, 323, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 325, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 327, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 329, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 522, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 331, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 333, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 335, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 630, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 337, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 339, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 341, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 659, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 661, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 525, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 526, 662, 662, 662, 662, 610, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 527, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 529, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 530, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 343, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 532, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 614, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 612, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 535, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 539, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 347, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 349, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 351, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 353, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 355, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 543, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 544, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 616, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 545, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 546, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 357, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 548, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 618, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 550, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 359, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 552, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 361, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 363, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 367, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 619, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 555, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 369, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 371, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 373, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 557, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 638, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 559, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 560, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 640, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 375, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 562, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 563, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 564, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 377, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 379, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 381, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 383, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 385, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 387, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 572, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 573, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 389, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 391, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 393, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 577, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 578, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 395, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 397, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 399, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 672, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 579, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 401, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 580, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 620, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 403, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 405, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 581, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 407, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 409, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 582, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 411, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 584, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 587, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 588, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 413, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 415, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 417, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 589, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 590, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 419, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 591, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 592, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 421, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 624, 662, 662, -1, -1, 662, 475, -1, 662, 476, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 603, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 496, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 504, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 495, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 646, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 506, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 507, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 518, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 523, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 633, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 647, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 649, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 533, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 634, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 611, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 531, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 670, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 547, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 551, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 639, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 549, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 554, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 617, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 570, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 561, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 566, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 583, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 585, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 477, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 478, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 505, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 497, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 514, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 520, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 632, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 534, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 636, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 635, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 615, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 537, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 667, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 637, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 556, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 553, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 558, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 571, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 567, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 574, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 586, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 479, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 606, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 521, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 524, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 536, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 541, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 538, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 613, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 565, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 568, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 575, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 480, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 519, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 528, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 540, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 569, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 481, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 542, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 482, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 668, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 599, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 483, 662, 662, -1, -1, 662, 484, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 485, 662, 662, 662, 486, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 487, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 650, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 651, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 621, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 657, 662, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 656, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 641, 662, 662, -1, -1, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  258,
			  398,
			  402,
			  406,
			  410,
			  416,
			  418,
			  422,
			  424,
			  425,
			  190,
			  426,
			  431,
			  432,
			  434,
			  436,
			  437,
			  438,
			  441,
			  442
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 673);
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

