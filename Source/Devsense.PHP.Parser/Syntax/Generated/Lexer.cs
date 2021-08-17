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
				case 1:
					// #line 757
					{
						yymore(); break;
					}
					break;
					
				case 2:
					// #line 94
					{ 
						return ProcessEof(Tokens.T_INLINE_HTML);
					}
					break;
					
				case 4:
					// #line 744
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
					// #line 724
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
					// #line 734
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
					// #line 958
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 8:
					// #line 80
					{
						return Tokens.EOF;
					}
					break;
					
				case 9:
					// #line 628
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 10:
					// #line 778
					{
						return ProcessLabel();
					}
					break;
					
				case 11:
					// #line 782
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 12:
					// #line 306
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 13:
					// #line 672
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 14:
					// #line 333
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 15:
					// #line 633
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 641
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 934
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 924
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 873
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 341
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 798
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
					// #line 608
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
					// #line 512
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 29:
					// #line 296
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 30:
					// #line 548
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 31:
					// #line 624
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 32:
					// #line 540
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 33:
					// #line 688
					{
						return ProcessRealNumber();
					}
					break;
					
				case 34:
					// #line 329
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 35:
					// #line 568
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 36:
					// #line 788
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 37:
					// #line 564
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 38:
					// #line 556
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 39:
					// #line 552
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 40:
					// #line 492
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 41:
					// #line 524
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 42:
					// #line 544
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 43:
					// #line 508
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 44:
					// #line 528
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 45:
					// #line 536
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 46:
					// #line 620
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 47:
					// #line 572
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 48:
					// #line 584
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 49:
					// #line 604
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 50:
					// #line 588
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 51:
					// #line 600
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 52:
					// #line 592
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 53:
					// #line 803
					{
						return ProcessVariable();
					}
					break;
					
				case 54:
					// #line 596
					{
						return (Tokens.T_COALESCE_EQUAL);
					}
					break;
					
				case 55:
					// #line 301
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 56:
					// #line 616
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
					// #line 345
					{
						return Identifier(Tokens.T_NEW);
					}
					break;
					
				case 61:
					// #line 409
					{
						return Identifier(Tokens.T_USE);
					}
					break;
					
				case 62:
					// #line 612
					{
						return Identifier(Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 63:
					// #line 580
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 64:
					// #line 337
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 65:
					// #line 353
					{
						return Identifier(Tokens.T_VAR);
					}
					break;
					
				case 66:
					// #line 560
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 67:
					// #line 516
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 68:
					// #line 520
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 69:
					// #line 532
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 70:
					// #line 576
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 71:
					// #line 676
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 72:
					// #line 668
					{
						return ProcessOctalNumber();
					}
					break;
					
				case 73:
					// #line 664
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
					// #line 385
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
					// #line 496
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
					// #line 793
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 82:
					// #line 175
					{
						return Identifier(Tokens.T_ENDIF);
					}
					break;
					
				case 83:
					// #line 425
					{
						return Identifier(Tokens.T_EMPTY);
					}
					break;
					
				case 84:
					// #line 421
					{
						return Identifier(Tokens.T_ISSET);
					}
					break;
					
				case 85:
					// #line 284
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
					// #line 469
					{
						return Identifier(Tokens.T_FINAL);
					}
					break;
					
				case 88:
					// #line 488
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
					// #line 349
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
					// #line 500
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
					// #line 357
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 100:
					// #line 807
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
					// #line 279
					{
						yyless(4); // consume 4 characters
						return Identifier(Tokens.T_ENUM);
					}
					break;
					
				case 103:
					// #line 171
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 104:
					// #line 461
					{
						return Identifier(Tokens.T_STATIC);
					}
					break;
					
				case 105:
					// #line 227
					{
						return Identifier(Tokens.T_SWITCH);
					}
					break;
					
				case 106:
					// #line 135
					{
						return Identifier(Tokens.T_RETURN);
					}
					break;
					
				case 107:
					// #line 417
					{
						return Identifier(Tokens.T_GLOBAL);
					}
					break;
					
				case 108:
					// #line 481
					{
						return Identifier(Tokens.T_PUBLIC);
					}
					break;
					
				case 109:
					// #line 361
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 110:
					// #line 377
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 111:
					// #line 288
					{
						return Identifier(Tokens.T_EXTENDS);
					}
					break;
					
				case 112:
					// #line 389
					{
						return Identifier(Tokens.T_INCLUDE);
					}
					break;
					
				case 113:
					// #line 243
					{
						return Identifier(Tokens.T_DEFAULT);
					}
					break;
					
				case 114:
					// #line 211
					{
						return Identifier(Tokens.T_DECLARE);
					}
					break;
					
				case 115:
					// #line 159
					{
						return Identifier(Tokens.T_FINALLY);
					}
					break;
					
				case 116:
					// #line 203
					{
						return Identifier(Tokens.T_FOREACH);
					}
					break;
					
				case 117:
					// #line 397
					{
						return Identifier(Tokens.T_REQUIRE);
					}
					break;
					
				case 118:
					// #line 473
					{
						return Identifier(Tokens.T_PRIVATE);
					}
					break;
					
				case 119:
					// #line 381
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 120:
					// #line 369
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 121:
					// #line 716
					{
						return Identifier(Tokens.T_DIR);
					}
					break;
					
				case 122:
					// #line 187
					{
						return Identifier(Tokens.T_ENDWHILE);
					}
					break;
					
				case 123:
					// #line 127
					{
						return Identifier(Tokens.T_FUNCTION);
					}
					break;
					
				case 124:
					// #line 251
					{
						return Identifier(Tokens.T_CONTINUE);
					}
					break;
					
				case 125:
					// #line 504
					{
						return Identifier(Tokens.T_CALLABLE);
					}
					break;
					
				case 126:
					// #line 484
					{
						return Identifier(Tokens.T_READONLY);
					}
					break;
					
				case 127:
					// #line 465
					{
						return Identifier(Tokens.T_ABSTRACT);
					}
					break;
					
				case 128:
					// #line 373
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 129:
					// #line 365
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 130:
					// #line 712
					{
						return Identifier(Tokens.T_FILE);
					}
					break;
					
				case 131:
					// #line 708
					{
						return Identifier(Tokens.T_LINE);
					}
					break;
					
				case 132:
					// #line 235
					{
						return Identifier(Tokens.T_ENDSWITCH);
					}
					break;
					
				case 133:
					// #line 271
					{
						return Identifier(Tokens.T_INTERFACE);
					}
					break;
					
				case 134:
					// #line 413
					{
						return Identifier(Tokens.T_INSTEADOF);
					}
					break;
					
				case 135:
					// #line 405
					{
						return Identifier(Tokens.T_NAMESPACE);
					}
					break;
					
				case 136:
					// #line 477
					{
						return Identifier(Tokens.T_PROTECTED);
					}
					break;
					
				case 137:
					// #line 696
					{
						return Identifier(Tokens.T_TRAIT_C);
					}
					break;
					
				case 138:
					// #line 692
					{
						return Identifier(Tokens.T_CLASS_C);
					}
					break;
					
				case 139:
					// #line 215
					{
						return Identifier(Tokens.T_ENDDECLARE);
					}
					break;
					
				case 140:
					// #line 207
					{
						return Identifier(Tokens.T_ENDFOREACH);
					}
					break;
					
				case 141:
					// #line 219
					{
						return Identifier(Tokens.T_INSTANCEOF);
					}
					break;
					
				case 142:
					// #line 292
					{
						return Identifier(Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 143:
					// #line 143
					{
						return Identifier(Tokens.T_YIELD_FROM);
					}
					break;
					
				case 144:
					// #line 704
					{
						return Identifier(Tokens.T_METHOD_C);
					}
					break;
					
				case 145:
					// #line 275
					{
						yyless(4); // consume 4 characters
						return ProcessLabel();
					}
					break;
					
				case 146:
					// #line 393
					{
						return Identifier(Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 147:
					// #line 401
					{
						return Identifier(Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 148:
					// #line 700
					{
						return Identifier(Tokens.T_FUNC_C);
					}
					break;
					
				case 149:
					// #line 720
					{
						return Identifier(Tokens.T_NS_C);
					}
					break;
					
				case 150:
					// #line 444
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return Identifier(Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 151:
					// #line 932
					{ yymore(); break; }
					break;
					
				case 152:
					// #line 86
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 153:
					// #line 930
					{ yymore(); break; }
					break;
					
				case 154:
					// #line 928
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 155:
					// #line 931
					{ yymore(); break; }
					break;
					
				case 156:
					// #line 927
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 157:
					// #line 926
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 158:
					// #line 925
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 159:
					// #line 876
					{ yymore(); break; }
					break;
					
				case 160:
					// #line 83
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 161:
					// #line 875
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 877
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 163:
					// #line 874
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 942
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 940
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 938
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 167:
					// #line 941
					{ yymore(); break; }
					break;
					
				case 168:
					// #line 937
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 169:
					// #line 936
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 170:
					// #line 935
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 171:
					// #line 951
					{ yymore(); break; }
					break;
					
				case 172:
					// #line 950
					{ yymore(); break; }
					break;
					
				case 173:
					// #line 948
					{ yymore(); break; }
					break;
					
				case 174:
					// #line 862
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(_processDoubleQuotedString) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 175:
					// #line 949
					{ yymore(); break; }
					break;
					
				case 176:
					// #line 946
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 177:
					// #line 945
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 178:
					// #line 944
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 179:
					// #line 323
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 180:
					// #line 318
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 181:
					// #line 310
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 182:
					// #line 314
					{
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 183:
					// #line 657
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 184:
					// #line 649
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 185:
					// #line 794
					{ yymore(); break; }
					break;
					
				case 186:
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
					
				case 187:
					// #line 796
					{ yymore(); break; }
					break;
					
				case 188:
					// #line 795
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 189:
					// #line 789
					{ yymore(); break; }
					break;
					
				case 190:
					// #line 98
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 191:
					// #line 791
					{ yymore(); break; }
					break;
					
				case 192:
					// #line 790
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 193:
					// #line 956
					{ yymore(); break; }
					break;
					
				case 194:
					// #line 102
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 195:
					// #line 955
					{ yymore(); break; }
					break;
					
				case 196:
					// #line 953
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 197:
					// #line 954
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 198:
					// #line 766
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 199:
					// #line 680
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 200:
					// #line 761
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 201:
					// #line 684
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 202:
					// #line 849
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 203:
					// #line 838
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
					
				case 204:
					// #line 871
					{ yymore(); break; }
					break;
					
				case 205:
					// #line 853
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(null) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 206:
					// #line 456
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 207:
					// #line 453
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 208:
					// #line 450
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 209:
					// #line 429
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 210:
					// #line 454
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 211:
					// #line 452
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 212:
					// #line 434
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 213:
					// #line 439
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 214:
					// #line 918
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 215:
					// #line 907
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 216:
					// #line 901
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 217:
					// #line 896
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 218:
					// #line 879
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 219:
					// #line 890
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 220:
					// #line 884
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 221:
					// #line 912
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 223: goto case 1;
				case 225: goto case 6;
				case 226: goto case 9;
				case 227: goto case 10;
				case 228: goto case 11;
				case 229: goto case 13;
				case 230: goto case 21;
				case 231: goto case 33;
				case 232: goto case 44;
				case 233: goto case 100;
				case 234: goto case 102;
				case 235: goto case 153;
				case 236: goto case 161;
				case 237: goto case 165;
				case 238: goto case 172;
				case 239: goto case 173;
				case 240: goto case 179;
				case 241: goto case 183;
				case 242: goto case 193;
				case 243: goto case 196;
				case 244: goto case 198;
				case 245: goto case 199;
				case 246: goto case 201;
				case 247: goto case 206;
				case 248: goto case 214;
				case 251: goto case 9;
				case 252: goto case 10;
				case 253: goto case 21;
				case 254: goto case 33;
				case 255: goto case 102;
				case 256: goto case 179;
				case 257: goto case 201;
				case 258: goto case 214;
				case 260: goto case 9;
				case 261: goto case 10;
				case 262: goto case 201;
				case 264: goto case 9;
				case 265: goto case 10;
				case 267: goto case 9;
				case 268: goto case 10;
				case 270: goto case 9;
				case 271: goto case 10;
				case 273: goto case 9;
				case 274: goto case 10;
				case 276: goto case 9;
				case 277: goto case 10;
				case 279: goto case 9;
				case 280: goto case 10;
				case 282: goto case 9;
				case 283: goto case 10;
				case 285: goto case 9;
				case 286: goto case 10;
				case 288: goto case 9;
				case 289: goto case 10;
				case 291: goto case 9;
				case 292: goto case 10;
				case 294: goto case 9;
				case 295: goto case 10;
				case 297: goto case 9;
				case 298: goto case 10;
				case 300: goto case 9;
				case 301: goto case 10;
				case 303: goto case 9;
				case 304: goto case 10;
				case 306: goto case 10;
				case 308: goto case 10;
				case 310: goto case 10;
				case 312: goto case 10;
				case 314: goto case 10;
				case 316: goto case 10;
				case 318: goto case 10;
				case 320: goto case 10;
				case 322: goto case 10;
				case 324: goto case 10;
				case 326: goto case 10;
				case 328: goto case 10;
				case 330: goto case 10;
				case 332: goto case 10;
				case 334: goto case 10;
				case 336: goto case 10;
				case 338: goto case 10;
				case 340: goto case 10;
				case 342: goto case 10;
				case 344: goto case 10;
				case 346: goto case 10;
				case 348: goto case 10;
				case 350: goto case 10;
				case 352: goto case 10;
				case 354: goto case 10;
				case 356: goto case 10;
				case 358: goto case 10;
				case 360: goto case 10;
				case 362: goto case 10;
				case 364: goto case 10;
				case 366: goto case 10;
				case 368: goto case 10;
				case 370: goto case 10;
				case 372: goto case 10;
				case 374: goto case 10;
				case 376: goto case 10;
				case 378: goto case 10;
				case 380: goto case 10;
				case 382: goto case 10;
				case 384: goto case 10;
				case 386: goto case 10;
				case 388: goto case 10;
				case 390: goto case 10;
				case 392: goto case 10;
				case 394: goto case 10;
				case 396: goto case 10;
				case 398: goto case 10;
				case 400: goto case 10;
				case 402: goto case 10;
				case 404: goto case 10;
				case 406: goto case 10;
				case 408: goto case 10;
				case 410: goto case 10;
				case 412: goto case 10;
				case 414: goto case 10;
				case 416: goto case 10;
				case 418: goto case 10;
				case 420: goto case 10;
				case 422: goto case 10;
				case 424: goto case 10;
				case 426: goto case 10;
				case 428: goto case 10;
				case 430: goto case 10;
				case 462: goto case 10;
				case 474: goto case 10;
				case 477: goto case 10;
				case 479: goto case 10;
				case 481: goto case 10;
				case 483: goto case 10;
				case 484: goto case 10;
				case 485: goto case 10;
				case 486: goto case 10;
				case 487: goto case 10;
				case 488: goto case 10;
				case 489: goto case 10;
				case 490: goto case 10;
				case 491: goto case 10;
				case 492: goto case 10;
				case 493: goto case 10;
				case 494: goto case 10;
				case 495: goto case 10;
				case 496: goto case 10;
				case 497: goto case 10;
				case 498: goto case 10;
				case 499: goto case 10;
				case 500: goto case 10;
				case 501: goto case 10;
				case 502: goto case 10;
				case 503: goto case 10;
				case 504: goto case 10;
				case 505: goto case 10;
				case 506: goto case 10;
				case 507: goto case 10;
				case 508: goto case 10;
				case 509: goto case 10;
				case 510: goto case 10;
				case 511: goto case 10;
				case 512: goto case 10;
				case 513: goto case 10;
				case 514: goto case 10;
				case 515: goto case 10;
				case 516: goto case 10;
				case 517: goto case 10;
				case 518: goto case 10;
				case 519: goto case 10;
				case 520: goto case 10;
				case 521: goto case 10;
				case 522: goto case 10;
				case 523: goto case 10;
				case 524: goto case 10;
				case 525: goto case 10;
				case 526: goto case 10;
				case 527: goto case 10;
				case 528: goto case 10;
				case 529: goto case 10;
				case 530: goto case 10;
				case 531: goto case 10;
				case 532: goto case 10;
				case 533: goto case 10;
				case 534: goto case 10;
				case 535: goto case 10;
				case 536: goto case 10;
				case 537: goto case 10;
				case 538: goto case 10;
				case 539: goto case 10;
				case 540: goto case 10;
				case 541: goto case 10;
				case 542: goto case 10;
				case 543: goto case 10;
				case 544: goto case 10;
				case 545: goto case 10;
				case 546: goto case 10;
				case 547: goto case 10;
				case 548: goto case 10;
				case 549: goto case 10;
				case 550: goto case 10;
				case 551: goto case 10;
				case 552: goto case 10;
				case 553: goto case 10;
				case 554: goto case 10;
				case 555: goto case 10;
				case 556: goto case 10;
				case 557: goto case 10;
				case 558: goto case 10;
				case 559: goto case 10;
				case 560: goto case 10;
				case 561: goto case 10;
				case 562: goto case 10;
				case 563: goto case 10;
				case 564: goto case 10;
				case 565: goto case 10;
				case 566: goto case 10;
				case 567: goto case 10;
				case 568: goto case 10;
				case 569: goto case 10;
				case 570: goto case 10;
				case 571: goto case 10;
				case 572: goto case 10;
				case 573: goto case 10;
				case 574: goto case 10;
				case 575: goto case 10;
				case 576: goto case 10;
				case 577: goto case 10;
				case 578: goto case 10;
				case 579: goto case 10;
				case 580: goto case 10;
				case 581: goto case 10;
				case 582: goto case 10;
				case 583: goto case 10;
				case 584: goto case 10;
				case 585: goto case 10;
				case 586: goto case 10;
				case 587: goto case 10;
				case 588: goto case 10;
				case 589: goto case 10;
				case 590: goto case 10;
				case 591: goto case 10;
				case 592: goto case 10;
				case 593: goto case 10;
				case 594: goto case 10;
				case 595: goto case 10;
				case 596: goto case 10;
				case 597: goto case 10;
				case 598: goto case 10;
				case 599: goto case 10;
				case 600: goto case 10;
				case 601: goto case 10;
				case 602: goto case 10;
				case 603: goto case 10;
				case 604: goto case 10;
				case 605: goto case 10;
				case 606: goto case 10;
				case 607: goto case 10;
				case 608: goto case 10;
				case 609: goto case 10;
				case 610: goto case 10;
				case 611: goto case 10;
				case 612: goto case 10;
				case 613: goto case 10;
				case 614: goto case 10;
				case 615: goto case 10;
				case 616: goto case 10;
				case 617: goto case 10;
				case 618: goto case 10;
				case 619: goto case 10;
				case 620: goto case 10;
				case 621: goto case 10;
				case 622: goto case 10;
				case 623: goto case 10;
				case 624: goto case 10;
				case 625: goto case 10;
				case 626: goto case 10;
				case 627: goto case 10;
				case 628: goto case 10;
				case 629: goto case 10;
				case 630: goto case 10;
				case 631: goto case 10;
				case 632: goto case 10;
				case 633: goto case 10;
				case 634: goto case 10;
				case 635: goto case 10;
				case 636: goto case 10;
				case 637: goto case 10;
				case 638: goto case 10;
				case 639: goto case 10;
				case 640: goto case 10;
				case 641: goto case 10;
				case 642: goto case 10;
				case 643: goto case 10;
				case 644: goto case 10;
				case 645: goto case 10;
				case 646: goto case 10;
				case 647: goto case 10;
				case 648: goto case 10;
				case 649: goto case 10;
				case 650: goto case 10;
				case 651: goto case 10;
				case 652: goto case 10;
				case 653: goto case 10;
				case 654: goto case 10;
				case 655: goto case 10;
				case 656: goto case 10;
				case 657: goto case 10;
				case 658: goto case 10;
				case 659: goto case 10;
				case 660: goto case 10;
				case 661: goto case 10;
				case 662: goto case 10;
				case 663: goto case 10;
				case 664: goto case 10;
				case 665: goto case 10;
				case 666: goto case 10;
				case 667: goto case 10;
				case 668: goto case 10;
				case 669: goto case 10;
				case 670: goto case 10;
				case 671: goto case 10;
				case 672: goto case 10;
				case 673: goto case 10;
				case 674: goto case 10;
				case 675: goto case 10;
				case 676: goto case 10;
				case 677: goto case 10;
				case 678: goto case 10;
				case 679: goto case 10;
				case 680: goto case 10;
				case 681: goto case 10;
				case 682: goto case 10;
				case 683: goto case 10;
				case 684: goto case 10;
				case 685: goto case 10;
				case 686: goto case 10;
				case 687: goto case 10;
				case 688: goto case 10;
				case 689: goto case 10;
				case 690: goto case 10;
				case 691: goto case 10;
				case 692: goto case 10;
				case 693: goto case 10;
				case 694: goto case 10;
				case 695: goto case 10;
				case 696: goto case 10;
				case 697: goto case 10;
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
			AcceptConditions.Accept, // 171
			AcceptConditions.Accept, // 172
			AcceptConditions.Accept, // 173
			AcceptConditions.AcceptOnStart, // 174
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
			AcceptConditions.Accept, // 202
			AcceptConditions.Accept, // 203
			AcceptConditions.Accept, // 204
			AcceptConditions.AcceptOnStart, // 205
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
			AcceptConditions.Accept, // 219
			AcceptConditions.Accept, // 220
			AcceptConditions.Accept, // 221
			AcceptConditions.NotAccept, // 222
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
			AcceptConditions.Accept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.Accept, // 248
			AcceptConditions.NotAccept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.Accept, // 251
			AcceptConditions.Accept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.Accept, // 254
			AcceptConditions.Accept, // 255
			AcceptConditions.Accept, // 256
			AcceptConditions.Accept, // 257
			AcceptConditions.Accept, // 258
			AcceptConditions.NotAccept, // 259
			AcceptConditions.Accept, // 260
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
			AcceptConditions.Accept, // 295
			AcceptConditions.NotAccept, // 296
			AcceptConditions.Accept, // 297
			AcceptConditions.Accept, // 298
			AcceptConditions.NotAccept, // 299
			AcceptConditions.Accept, // 300
			AcceptConditions.Accept, // 301
			AcceptConditions.NotAccept, // 302
			AcceptConditions.Accept, // 303
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
			AcceptConditions.Accept, // 416
			AcceptConditions.NotAccept, // 417
			AcceptConditions.Accept, // 418
			AcceptConditions.NotAccept, // 419
			AcceptConditions.Accept, // 420
			AcceptConditions.NotAccept, // 421
			AcceptConditions.Accept, // 422
			AcceptConditions.NotAccept, // 423
			AcceptConditions.Accept, // 424
			AcceptConditions.NotAccept, // 425
			AcceptConditions.Accept, // 426
			AcceptConditions.NotAccept, // 427
			AcceptConditions.Accept, // 428
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
			AcceptConditions.NotAccept, // 441
			AcceptConditions.NotAccept, // 442
			AcceptConditions.NotAccept, // 443
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
			AcceptConditions.NotAccept, // 454
			AcceptConditions.NotAccept, // 455
			AcceptConditions.NotAccept, // 456
			AcceptConditions.NotAccept, // 457
			AcceptConditions.NotAccept, // 458
			AcceptConditions.NotAccept, // 459
			AcceptConditions.NotAccept, // 460
			AcceptConditions.NotAccept, // 461
			AcceptConditions.Accept, // 462
			AcceptConditions.NotAccept, // 463
			AcceptConditions.NotAccept, // 464
			AcceptConditions.NotAccept, // 465
			AcceptConditions.NotAccept, // 466
			AcceptConditions.NotAccept, // 467
			AcceptConditions.NotAccept, // 468
			AcceptConditions.NotAccept, // 469
			AcceptConditions.NotAccept, // 470
			AcceptConditions.NotAccept, // 471
			AcceptConditions.NotAccept, // 472
			AcceptConditions.NotAccept, // 473
			AcceptConditions.Accept, // 474
			AcceptConditions.NotAccept, // 475
			AcceptConditions.NotAccept, // 476
			AcceptConditions.Accept, // 477
			AcceptConditions.NotAccept, // 478
			AcceptConditions.Accept, // 479
			AcceptConditions.NotAccept, // 480
			AcceptConditions.Accept, // 481
			AcceptConditions.NotAccept, // 482
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
			AcceptConditions.Accept, // 673
			AcceptConditions.Accept, // 674
			AcceptConditions.Accept, // 675
			AcceptConditions.Accept, // 676
			AcceptConditions.Accept, // 677
			AcceptConditions.Accept, // 678
			AcceptConditions.Accept, // 679
			AcceptConditions.Accept, // 680
			AcceptConditions.Accept, // 681
			AcceptConditions.Accept, // 682
			AcceptConditions.Accept, // 683
			AcceptConditions.Accept, // 684
			AcceptConditions.Accept, // 685
			AcceptConditions.Accept, // 686
			AcceptConditions.Accept, // 687
			AcceptConditions.Accept, // 688
			AcceptConditions.Accept, // 689
			AcceptConditions.Accept, // 690
			AcceptConditions.Accept, // 691
			AcceptConditions.Accept, // 692
			AcceptConditions.Accept, // 693
			AcceptConditions.Accept, // 694
			AcceptConditions.Accept, // 695
			AcceptConditions.Accept, // 696
			AcceptConditions.Accept, // 697
		};
		
		private static int[] colMap = new int[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 38, 19, 0, 0, 60, 0, 0, 
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			38, 48, 63, 15, 64, 50, 51, 65, 37, 39, 45, 47, 54, 29, 35, 44, 
			57, 58, 59, 59, 59, 59, 59, 59, 32, 32, 33, 43, 49, 46, 30, 2, 
			54, 21, 24, 11, 7, 3, 8, 26, 22, 5, 40, 25, 18, 20, 9, 12, 
			27, 42, 14, 13, 6, 10, 36, 23, 4, 17, 31, 16, 34, 61, 53, 41, 
			62, 21, 24, 11, 7, 3, 8, 26, 22, 5, 40, 25, 18, 20, 9, 12, 
			27, 42, 14, 13, 6, 10, 36, 23, 4, 17, 31, 55, 52, 56, 54, 0, 
			28, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 1, 2, 1, 1, 1, 1, 3, 4, 5, 6, 7, 1, 1, 
			1, 1, 1, 1, 8, 9, 10, 10, 10, 10, 1, 10, 1, 1, 1, 11, 
			1, 12, 1, 1, 13, 1, 14, 1, 1, 15, 1, 1, 16, 17, 18, 1, 
			1, 1, 1, 1, 1, 19, 1, 1, 10, 10, 10, 20, 10, 10, 10, 1, 
			1, 10, 1, 1, 1, 1, 1, 21, 22, 23, 10, 10, 24, 10, 10, 10, 
			10, 25, 10, 10, 10, 10, 10, 26, 10, 10, 10, 10, 10, 27, 10, 10, 
			10, 10, 10, 1, 1, 28, 1, 10, 10, 10, 10, 10, 10, 1, 1, 10, 
			29, 10, 10, 10, 10, 30, 10, 1, 1, 10, 10, 10, 10, 10, 10, 10, 
			1, 1, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 1, 
			10, 1, 10, 10, 10, 10, 10, 31, 1, 32, 1, 1, 1, 1, 1, 33, 
			1, 33, 1, 1, 34, 35, 1, 1, 1, 1, 1, 36, 1, 37, 38, 1, 
			1, 1, 1, 1, 39, 1, 1, 1, 1, 40, 1, 41, 1, 42, 1, 43, 
			1, 44, 1, 45, 1, 1, 1, 46, 1, 47, 1, 48, 1, 49, 1, 1, 
			50, 1, 51, 52, 1, 1, 1, 1, 1, 53, 1, 1, 1, 1, 54, 55, 
			56, 57, 1, 58, 1, 59, 1, 60, 1, 61, 62, 63, 64, 65, 66, 67, 
			68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 
			84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 
			100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 
			116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 72, 
			131, 98, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 
			146, 147, 148, 149, 150, 151, 152, 153, 154, 25, 155, 156, 157, 158, 159, 160, 
			161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 
			177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 
			193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 
			209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 
			225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 
			241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 
			257, 258, 259, 260, 261, 56, 262, 263, 264, 265, 69, 266, 267, 268, 269, 270, 
			271, 272, 273, 274, 78, 275, 52, 276, 277, 278, 279, 280, 281, 282, 283, 284, 
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
			445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 
			461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 
			477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 
			493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 10, 
			508, 509, 510, 511, 512, 513, 514, 515, 516, 517
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 223, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 266, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 483, 687, 687, 687, 687, 484, 687, 485, 687, 687, 687, -1, -1, 687, 486, -1, 487, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 488, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 272, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, 33, -1, -1, -1, -1, -1, 275, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 54, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 230, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 253, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 272, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, 231, 231, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, 53, -1, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, -1, 53, 53, -1, -1, -1, 53, -1, -1, -1, 53, 53, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 53, 53, 53, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 673, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, -1, -1, -1, 71, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, 71, 71, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, 72, 72, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, 73, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 352, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 81, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 81, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 81, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 372, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, 373, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, 373, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, 373, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 677, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 600, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 694, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, -1, -1, 151 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 156, -1 },
			{ 159, -1, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, -1, 159, 159, 159, 159, 159, -1, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, -1 },
			{ 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, -1, 164, -1, 164 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, -1 },
			{ 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, -1, 171, 171, 171, 171, 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, -1, 171, 171, 171, 171, -1, 171, 171, 171, -1, 171 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, -1 },
			{ -1, -1, -1, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, 174, 174, -1, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, -1, 174, 174, -1, -1, -1, 174, -1, -1, -1, 174, 174, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, 174, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, -1, -1, 180, 180, -1, 180, 180, 180, 180, 180, 180, 180, 180, -1, -1, -1, 180, 180, -1, -1, -1, 180, -1, -1, -1, 180, 180, 180, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 180, 180, 180, -1, -1, -1, -1, -1, -1 },
			{ 185, -1, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, -1, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, -1, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 188, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 192, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 242, 194, 195, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 196, 242, 242, 242, 242, 242, 242, 242, 242, 3, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 243, 242, 242, 242, 242, 242 },
			{ -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1, -1, -1, -1, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, 199, 199, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, 201, 201, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, -1, -1, 203, 203, -1, 203, 203, 203, 203, 203, 203, 203, 203, -1, -1, -1, 203, 203, -1, -1, -1, 203, -1, -1, -1, 203, 203, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, 203, 203, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, -1, -1, 205, 205, -1, 205, 205, 205, 205, 205, 205, 205, 205, -1, -1, -1, 205, 205, -1, -1, -1, 205, -1, -1, -1, 205, 205, 205, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, 205, 205, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 454, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, -1, 219, 217, 217, -1, 217, 217, 217, 217, 217, 217, 217, 217, -1, 458, -1, 217, 217, -1, -1, -1, 217, -1, -1, -1, 217, 217, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, 217, 217, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 249, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, 174, 174, -1, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, -1, 174, -1, -1, -1, -1, 174, -1, 437, -1, 174, 174, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 22, 489, 687, 687, 687, 490, 687, -1, -1, 687, 687, -1, 618, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 272, 307, -1, -1, -1, -1, -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, 33, -1, -1, -1, -1, -1, 275, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 272, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, 231, 231, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 100, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 473, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, -1, 157, 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, -1, -1, -1, 157, -1, -1, -1, -1, 157, -1, -1, -1, 157, 157, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 158, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 159, -1, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 161, 159, 159, 159, 159, 159, 159, 159, 159, -1, 159, 159, 159, 159, 159, -1, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, -1 },
			{ -1, -1, -1, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, -1, -1, 169, 169, -1, 169, 169, 169, 169, 169, 169, 169, 169, -1, -1, -1, 169, -1, -1, -1, -1, 169, -1, -1, -1, 169, 169, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, -1, -1, 177, 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, -1, -1, -1, 177, -1, -1, -1, -1, 177, -1, -1, -1, 177, 177, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 442, 442, 442, 442, 442, 442, 442, 442, 442, 442, 442, 442, -1, 184, 442, 442, -1, 442, 442, 442, 442, 442, 442, 442, 442, -1, -1, -1, 442, 442, -1, -1, -1, 442, -1, -1, -1, 442, 442, 442, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 184, 442, 442, 442, -1, -1, -1, -1, -1, -1 },
			{ 242, -1, -1, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, -1, 242, 242, 242, 242, 242, 242, 242, 242, -1, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, 242, -1, 242, 242, 242, 242, 242 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, 53, 53, -1, 53, 53, 53, 53, 53, 53, 53, 53, -1, -1, -1, 53, -1, -1, -1, -1, 53, -1, -1, -1, 53, 53, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, 448, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 449, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, 201, 201, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 246, -1, -1, -1, 246, 246, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, 246, 246, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 216, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 259, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, -1, -1, 205, 205, -1, 205, 205, 205, 205, 205, 205, 205, 205, -1, -1, -1, 205, -1, -1, -1, -1, 205, -1, 452, -1, 205, 205, 205, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 492, 687, 280, 687, 687, 687, 687, 687, 687, 23, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 230, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 254, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 254, 254, 254, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 395, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 448, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, 257, 257, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, 217, -1, -1, 217, 217, -1, 217, 217, 217, 217, 217, 217, 217, 217, -1, -1, -1, 217, -1, -1, -1, -1, 217, -1, -1, -1, 217, 217, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 225, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 493, 687, 687, 687, 24, 621, 687, 283, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 449, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 262, 262, -1, -1, -1, -1, -1, -1, -1 },
			{ 7, 8, 9, 10, 462, 227, 474, 252, 261, 477, 479, 617, 265, 646, 667, 11, 226, 678, 683, 12, 685, 268, 687, 688, 271, 687, 689, 690, 3, 251, 260, 687, 13, 264, 14, 267, 481, 270, 12, 226, 687, 691, 687, 226, 273, 276, 279, 282, 285, 288, 291, 294, 297, 300, 226, 15, 16, 229, 13, 13, 12, 226, 17, 18, 303, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 25, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 55, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, -1, -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, 231, 231, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 292, 687, 687, 687, 27, 620, -1, -1, 687, 687, -1, 687, 687, 687, 687, 647, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 463, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 281, -1, 284, 287, -1, 464, -1, 290, 293, 296, -1, -1, -1, -1, -1, -1, 299, -1, -1, 302, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 624, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, 269, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, 254, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, 254, 254, 254, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, 36, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 56, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 57, 687, -1, 687, 513, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 58, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 465, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 59, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 60, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 232, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 61, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 62, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 466, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 65, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 74, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 475, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 75, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, 467, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 76, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 77, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, -1, -1, -1, 71, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, 71, 71, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 78, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, 72, 72, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 79, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, 73, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 80, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 254, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 254, 254, 254, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 82, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, 231, 231, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, 351, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, 351, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, 351, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 83, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 468, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 84, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 85, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 469, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 86, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 87, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 88, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 89, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, -1, -1, 347, 347, -1, 347, 347, 347, 347, 347, 347, 347, 347, -1, -1, -1, 347, -1, -1, -1, -1, 347, -1, 331, -1, 347, 347, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, 471 },
			{ -1, -1, -1, 90, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, 99, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 91, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 476, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 92, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 472, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 93, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 94, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 95, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 96, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 478, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 97, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, 347, -1, -1, 347, 347, 100, 347, 347, 347, 347, 347, 347, 347, 347, -1, -1, -1, 347, 347, -1, -1, -1, 347, -1, -1, -1, 347, 347, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, 347, 347, 233, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 98, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 369, 369, 369, 369, 369, 369, 369, 369, 369, 369, 369, 369, -1, -1, 369, 369, -1, 369, 369, 369, 369, 369, 369, 369, 369, -1, -1, -1, 369, -1, -1, -1, -1, 369, -1, -1, -1, 369, 369, 369, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 101, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, 102, -1, 234, 102, 255, 102, 102, 102, 102, 102, 102, 102, 102, 102, -1, -1, 102, 102, 351, 102, 102, 102, 102, 102, 102, 102, 102, 102, -1, -1, 102, -1, -1, -1, -1, 102, -1, 351, -1, 102, 102, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 103, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 104, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, 99, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 105, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 106, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 381, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 107, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 383, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 108, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, 109, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 111, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 112, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 389, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 391, 110, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 113, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 369, 369, 369, 369, 369, 369, 369, 369, 369, 369, 369, 369, -1, -1, 369, 369, -1, 369, 369, 369, 369, 369, 369, 369, 369, -1, -1, -1, 369, 369, -1, -1, -1, 369, -1, -1, -1, 369, 369, 369, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 369, 369, 369, -1, -1, -1, 393, -1, -1 },
			{ -1, -1, -1, 114, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 371, 371, 371, 371, 371, 371, 371, 371, 371, 371, 371, 371, -1, -1, 371, 371, -1, 371, 371, 371, 371, 371, 371, 371, 371, -1, -1, -1, 371, 371, -1, -1, -1, 371, -1, -1, -1, 371, 371, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, 371, 371, -1, -1, -1, -1, -1, 393 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 115, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 480, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 116, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 482, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 117, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 118, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, 119, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 121, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 397, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 122, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 399, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 123, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, 120, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 124, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 399, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 125, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 401, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 126, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 391, 110, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 127, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 100, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 130, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 405, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 131, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 397, 128, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 132, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 399, 129, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 133, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 391, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 134, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 409, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 135, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 411, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 136, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 413, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 137, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 138, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 139, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 143, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 140, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 141, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 421, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 142, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 145, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 144, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 423, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 146, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 425, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 147, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 148, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ 151, 152, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 3, 151, 151, 151, 151, 151, 429, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 153, 151, 151, 151, 151, 151, 151, 151, 154, 235, 151 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 149, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ 155, -1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, -1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 150, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ 159, 160, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 161, 159, 159, 159, 159, 159, 159, 159, 159, 3, 159, 159, 159, 159, 159, 432, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 236, 159, 159, 159, 159, 162 },
			{ 163, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163 },
			{ 164, 160, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 3, 164, 164, 164, 164, 164, 434, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 165, 164, 164, 164, 164, 164, 164, 166, 164, 237, 164 },
			{ 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167 },
			{ 171, 160, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 172, 171, 171, 171, 171, 171, 171, 171, 171, 224, 171, 171, 171, 171, 171, 436, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 173, 171, 171, 171, 171, 238, 171, 171, 171, 239, 171 },
			{ 175, -1, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, -1, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175 },
			{ -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 179, 8, 240, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 179, 179, 180, 180, 12, 180, 180, 180, 180, 180, 180, 180, 180, 3, 256, 179, 180, 179, 179, 179, 179, 180, 179, 12, 179, 180, 180, 180, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 12, 179, 179, 179, 179, 179 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 183, 8, 183, 241, 241, 241, 241, 241, 241, 241, 241, 241, 241, 241, 241, 183, 183, 241, 241, 183, 241, 241, 241, 241, 241, 241, 241, 241, 3, 183, 183, 241, 183, 183, 183, 183, 241, 183, 183, 183, 241, 241, 241, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183 },
			{ 185, 186, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 3, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 187, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185 },
			{ 189, 190, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 3, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 191, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189 },
			{ 7, 8, 198, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 7, 198, 687, 687, 7, 687, 687, 687, 687, 687, 687, 687, 687, 3, 198, 198, 687, 199, 198, 7, 198, 687, 198, 7, 198, 687, 687, 687, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 245, 199, 199, 7, 200, 198, 198, 244, 7 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, 201, 201, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 246, -1, -1, -1, 246, 246, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, 246, 246, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 257, 257, 257, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 262, 262, -1, -1, -1, -1, -1, -1, -1 },
			{ 202, 8, 202, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 202, 202, 203, 203, 202, 203, 203, 203, 203, 203, 203, 203, 203, 3, 202, 202, 203, 202, 202, 202, 202, 203, 202, 202, 202, 203, 203, 203, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202 },
			{ 204, 160, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 250, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204 },
			{ 206, 8, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 207, 206, 206, 206, 208, 206, 206, 206, 206, 206, 206, 206, 206, 3, 206, 206, 206, 206, 206, 206, 206, 206, 209, 208, 206, 206, 206, 206, 206, 247, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 208, 206, 206, 206, 206, 206 },
			{ 206, 8, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 207, 206, 206, 206, 208, 206, 206, 206, 206, 206, 206, 206, 206, 3, 206, 206, 206, 206, 206, 206, 206, 206, 206, 208, 212, 206, 206, 206, 206, 247, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 208, 206, 206, 206, 206, 206 },
			{ 206, 8, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 207, 206, 206, 206, 208, 206, 206, 206, 206, 206, 206, 206, 206, 3, 206, 206, 206, 206, 206, 206, 206, 206, 206, 208, 206, 206, 206, 206, 213, 247, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 208, 206, 206, 206, 206, 206 },
			{ 214, 8, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 3, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 248, 214, 214, 214, 214, 214, 214, 214, 215, 258, 214 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 459, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 220, -1, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, -1, -1, 220, 220, -1, 220, 220, 220, 220, 220, 220, 220, 220, 220, -1, -1, 220, -1, -1, -1, -1, 220, -1, -1, -1, 220, 220, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 214, 3, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 3, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 248, 214, 214, 214, 214, 214, 214, 221, 214, 258, 214 },
			{ 214, 3, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 3, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 248, 214, 214, 214, 214, 214, 214, 214, 214, 258, 214 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 274, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 470, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 371, 371, 371, 371, 371, 371, 371, 371, 371, 371, 371, 371, -1, -1, 371, 371, -1, 371, 371, 371, 371, 371, 371, 371, 371, -1, -1, -1, 371, -1, -1, -1, -1, 371, -1, -1, -1, 371, 371, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 403, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 277, -1, -1, 687, 687, -1, 687, 687, 491, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 286, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 494, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 387, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 619, 687, 687, 687, 289, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 407, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 295, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 298, 649, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 508, 687, 687, 509, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 301, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 304, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 510, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 306, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 668, 687, 687, 687, 687, 511, 687, 625, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 512, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 514, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 622, 687, 687, 651, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 515, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 679, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 519, 687, 687, -1, -1, 687, 687, -1, 687, 520, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 521, 687, 687, 687, 687, 687, 687, 308, 687, -1, -1, 687, 696, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 650, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 669, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 522, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 523, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 628, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 524, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 310, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 525, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 312, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 626, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 670, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 530, 687, 687, 687, 687, 687, 687, 629, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 531, 532, 533, 534, 687, 680, 687, 687, 687, -1, -1, 687, 631, -1, 535, 687, 632, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 314, 687, 633, 537, 687, 687, 687, 687, 538, 687, -1, -1, 687, 687, -1, 687, 687, 687, 539, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 316, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 318, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 652, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 320, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 322, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 324, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 326, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 653, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 328, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 655, 687, 687, 687, 687, 687, 687, 330, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 332, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 334, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 336, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 544, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 545, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 338, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 340, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 342, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 654, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 344, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 346, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 348, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 684, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 686, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 548, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 549, 687, 687, 687, 687, 634, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 550, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 552, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 553, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 350, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 555, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 638, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 636, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 558, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 562, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 354, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 356, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 358, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 661, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 360, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 362, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 566, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 567, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 640, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 568, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 569, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 364, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 571, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 642, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 573, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 366, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 575, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 368, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 370, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 374, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 643, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 578, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 376, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 378, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 380, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 581, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 663, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 583, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 584, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 665, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 382, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 586, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 587, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 588, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 384, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 386, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 388, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 390, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 392, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 394, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 396, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 596, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 597, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 398, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 400, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 402, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 601, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 602, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 404, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 406, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 408, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 697, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 603, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 410, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 604, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 644, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 412, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 414, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 605, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 416, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 418, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 606, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 420, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 608, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 611, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 612, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 422, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 424, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 426, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 613, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 614, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 428, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 615, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 616, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 430, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 648, 687, 687, -1, -1, 687, 495, -1, 687, 496, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 627, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 517, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 526, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 516, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 671, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 528, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 529, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 540, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 546, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 657, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 672, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 674, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 556, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 658, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 635, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 554, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 695, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 570, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 574, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 664, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 572, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 577, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 594, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 585, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 590, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 607, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 609, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 497, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 498, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 527, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 518, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 536, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 542, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 656, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 557, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 660, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 659, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 639, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 560, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 692, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 662, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 580, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 576, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 579, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 582, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 595, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 591, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 598, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 610, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 499, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 630, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 543, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 547, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 559, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 564, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 561, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 637, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 589, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 592, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 599, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 500, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 541, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 551, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 563, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 593, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 501, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 565, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 502, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 693, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 623, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 503, 687, 687, -1, -1, 687, 504, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 505, 687, 687, 687, 506, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 507, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 675, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 676, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 645, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 682, 687, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, 687, 681, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 687, 687, 687, 687, 687, 687, 687, 687, 687, 666, 687, 687, -1, -1, 687, 687, -1, 687, 687, 687, 687, 687, 687, 687, 687, -1, -1, -1, 687, 687, -1, -1, -1, 687, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 687, 687, 687, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  263,
			  427,
			  431,
			  433,
			  435,
			  438,
			  439,
			  441,
			  443,
			  444,
			  193,
			  445,
			  450,
			  451,
			  453,
			  455,
			  456,
			  457,
			  460,
			  461
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 698);
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

