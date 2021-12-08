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
					// #line 766
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
					// #line 753
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
					// #line 733
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
					// #line 743
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
					// #line 967
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
					// #line 637
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 10:
					// #line 787
					{
						return ProcessLabel();
					}
					break;
					
				case 11:
					// #line 791
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
					// #line 681
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
						return (Tokens.T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG);
					}
					break;
					
				case 16:
					// #line 642
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 17:
					// #line 650
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 18:
					// #line 933
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 882
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 943
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 21:
					// #line 341
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 22:
					// #line 807
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 23:
					// #line 167
					{
						return Identifier(Tokens.T_IF);
					}
					break;
					
				case 24:
					// #line 191
					{
						return Identifier(Tokens.T_DO);
					}
					break;
					
				case 25:
					// #line 123
					{
						return Identifier(Tokens.T_FN);
					}
					break;
					
				case 26:
					// #line 608
					{
						return Identifier(Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 27:
					// #line 139
					{
						return Tokens.T_ATTRIBUTE;
					}
					break;
					
				case 28:
					// #line 223
					{
						return Identifier(Tokens.T_AS);
					}
					break;
					
				case 29:
					// #line 512
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 30:
					// #line 296
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 31:
					// #line 548
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 32:
					// #line 624
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 33:
					// #line 540
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 34:
					// #line 697
					{
						return ProcessRealNumber();
					}
					break;
					
				case 35:
					// #line 329
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 36:
					// #line 568
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 37:
					// #line 797
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 38:
					// #line 564
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 39:
					// #line 556
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 40:
					// #line 552
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 41:
					// #line 492
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 42:
					// #line 524
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 43:
					// #line 544
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 44:
					// #line 508
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 45:
					// #line 528
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 46:
					// #line 536
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 47:
					// #line 620
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 48:
					// #line 572
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 49:
					// #line 584
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 50:
					// #line 604
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 51:
					// #line 628
					{
						yyless(1);
						return (Tokens.T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG);
					}
					break;
					
				case 52:
					// #line 588
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 53:
					// #line 600
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 54:
					// #line 592
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 55:
					// #line 812
					{
						return ProcessVariable();
					}
					break;
					
				case 56:
					// #line 596
					{
						return (Tokens.T_COALESCE_EQUAL);
					}
					break;
					
				case 57:
					// #line 301
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 58:
					// #line 616
					{
						return Identifier(Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 59:
					// #line 151
					{
						return Identifier(Tokens.T_TRY);
					}
					break;
					
				case 60:
					// #line 119
					{
						return Identifier(Tokens.T_EXIT);
					}
					break;
					
				case 61:
					// #line 195
					{
						return Identifier(Tokens.T_FOR);
					}
					break;
					
				case 62:
					// #line 345
					{
						return Identifier(Tokens.T_NEW);
					}
					break;
					
				case 63:
					// #line 409
					{
						return Identifier(Tokens.T_USE);
					}
					break;
					
				case 64:
					// #line 612
					{
						return Identifier(Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 65:
					// #line 580
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 66:
					// #line 337
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 67:
					// #line 353
					{
						return Identifier(Tokens.T_VAR);
					}
					break;
					
				case 68:
					// #line 560
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 69:
					// #line 516
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 70:
					// #line 520
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 71:
					// #line 532
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 72:
					// #line 576
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 73:
					// #line 685
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 74:
					// #line 677
					{
						return ProcessOctalNumber();
					}
					break;
					
				case 75:
					// #line 673
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 76:
					// #line 115
					{ 
						return Identifier(Tokens.T_EXIT);
					}
					break;
					
				case 77:
					// #line 259
					{
						return Identifier(Tokens.T_ECHO);
					}
					break;
					
				case 78:
					// #line 179
					{
						return Identifier(Tokens.T_ELSE);
					}
					break;
					
				case 79:
					// #line 385
					{
						return Identifier(Tokens.T_EVAL);
					}
					break;
					
				case 80:
					// #line 239
					{
						return Identifier(Tokens.T_CASE);
					}
					break;
					
				case 81:
					// #line 496
					{
						return Identifier(Tokens.T_LIST);
					}
					break;
					
				case 82:
					// #line 255
					{
						return Identifier(Tokens.T_GOTO);
					}
					break;
					
				case 83:
					// #line 802
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 84:
					// #line 175
					{
						return Identifier(Tokens.T_ENDIF);
					}
					break;
					
				case 85:
					// #line 425
					{
						return Identifier(Tokens.T_EMPTY);
					}
					break;
					
				case 86:
					// #line 421
					{
						return Identifier(Tokens.T_ISSET);
					}
					break;
					
				case 87:
					// #line 284
					{
						return Identifier(Tokens.T_TRAIT);
					}
					break;
					
				case 88:
					// #line 163
					{
						return Identifier(Tokens.T_THROW);
					}
					break;
					
				case 89:
					// #line 469
					{
						return Identifier(Tokens.T_FINAL);
					}
					break;
					
				case 90:
					// #line 488
					{
						return Identifier(Tokens.T_UNSET);
					}
					break;
					
				case 91:
					// #line 131
					{
						return Identifier(Tokens.T_CONST);
					}
					break;
					
				case 92:
					// #line 349
					{
						return Identifier(Tokens.T_CLONE);
					}
					break;
					
				case 93:
					// #line 267
					{
						return Identifier(Tokens.T_CLASS);
					}
					break;
					
				case 94:
					// #line 155
					{
						return Identifier(Tokens.T_CATCH);
					}
					break;
					
				case 95:
					// #line 147
					{
						return Identifier(Tokens.T_YIELD);
					}
					break;
					
				case 96:
					// #line 231
					{
						return Identifier(Tokens.T_MATCH);
					}
					break;
					
				case 97:
					// #line 500
					{
						return Identifier(Tokens.T_ARRAY);
					}
					break;
					
				case 98:
					// #line 183
					{
						return Identifier(Tokens.T_WHILE);
					}
					break;
					
				case 99:
					// #line 247
					{
						return Identifier(Tokens.T_BREAK);
					}
					break;
					
				case 100:
					// #line 263
					{
						return Identifier(Tokens.T_PRINT);
					}
					break;
					
				case 101:
					// #line 357
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 102:
					// #line 816
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
					
				case 103:
					// #line 199
					{
						return Identifier(Tokens.T_ENDFOR);
					}
					break;
					
				case 104:
					// #line 279
					{
						yyless(4); // consume 4 characters
						return Identifier(Tokens.T_ENUM);
					}
					break;
					
				case 105:
					// #line 171
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 106:
					// #line 461
					{
						return Identifier(Tokens.T_STATIC);
					}
					break;
					
				case 107:
					// #line 227
					{
						return Identifier(Tokens.T_SWITCH);
					}
					break;
					
				case 108:
					// #line 135
					{
						return Identifier(Tokens.T_RETURN);
					}
					break;
					
				case 109:
					// #line 417
					{
						return Identifier(Tokens.T_GLOBAL);
					}
					break;
					
				case 110:
					// #line 481
					{
						return Identifier(Tokens.T_PUBLIC);
					}
					break;
					
				case 111:
					// #line 361
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 112:
					// #line 377
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 113:
					// #line 288
					{
						return Identifier(Tokens.T_EXTENDS);
					}
					break;
					
				case 114:
					// #line 389
					{
						return Identifier(Tokens.T_INCLUDE);
					}
					break;
					
				case 115:
					// #line 243
					{
						return Identifier(Tokens.T_DEFAULT);
					}
					break;
					
				case 116:
					// #line 211
					{
						return Identifier(Tokens.T_DECLARE);
					}
					break;
					
				case 117:
					// #line 159
					{
						return Identifier(Tokens.T_FINALLY);
					}
					break;
					
				case 118:
					// #line 203
					{
						return Identifier(Tokens.T_FOREACH);
					}
					break;
					
				case 119:
					// #line 397
					{
						return Identifier(Tokens.T_REQUIRE);
					}
					break;
					
				case 120:
					// #line 473
					{
						return Identifier(Tokens.T_PRIVATE);
					}
					break;
					
				case 121:
					// #line 381
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 122:
					// #line 369
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 123:
					// #line 725
					{
						return Identifier(Tokens.T_DIR);
					}
					break;
					
				case 124:
					// #line 187
					{
						return Identifier(Tokens.T_ENDWHILE);
					}
					break;
					
				case 125:
					// #line 127
					{
						return Identifier(Tokens.T_FUNCTION);
					}
					break;
					
				case 126:
					// #line 251
					{
						return Identifier(Tokens.T_CONTINUE);
					}
					break;
					
				case 127:
					// #line 504
					{
						return Identifier(Tokens.T_CALLABLE);
					}
					break;
					
				case 128:
					// #line 484
					{
						return Identifier(Tokens.T_READONLY);
					}
					break;
					
				case 129:
					// #line 465
					{
						return Identifier(Tokens.T_ABSTRACT);
					}
					break;
					
				case 130:
					// #line 373
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 131:
					// #line 365
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 132:
					// #line 721
					{
						return Identifier(Tokens.T_FILE);
					}
					break;
					
				case 133:
					// #line 717
					{
						return Identifier(Tokens.T_LINE);
					}
					break;
					
				case 134:
					// #line 235
					{
						return Identifier(Tokens.T_ENDSWITCH);
					}
					break;
					
				case 135:
					// #line 271
					{
						return Identifier(Tokens.T_INTERFACE);
					}
					break;
					
				case 136:
					// #line 413
					{
						return Identifier(Tokens.T_INSTEADOF);
					}
					break;
					
				case 137:
					// #line 405
					{
						return Identifier(Tokens.T_NAMESPACE);
					}
					break;
					
				case 138:
					// #line 477
					{
						return Identifier(Tokens.T_PROTECTED);
					}
					break;
					
				case 139:
					// #line 705
					{
						return Identifier(Tokens.T_TRAIT_C);
					}
					break;
					
				case 140:
					// #line 701
					{
						return Identifier(Tokens.T_CLASS_C);
					}
					break;
					
				case 141:
					// #line 215
					{
						return Identifier(Tokens.T_ENDDECLARE);
					}
					break;
					
				case 142:
					// #line 207
					{
						return Identifier(Tokens.T_ENDFOREACH);
					}
					break;
					
				case 143:
					// #line 219
					{
						return Identifier(Tokens.T_INSTANCEOF);
					}
					break;
					
				case 144:
					// #line 292
					{
						return Identifier(Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 145:
					// #line 143
					{
						return Identifier(Tokens.T_YIELD_FROM);
					}
					break;
					
				case 146:
					// #line 713
					{
						return Identifier(Tokens.T_METHOD_C);
					}
					break;
					
				case 147:
					// #line 275
					{
						yyless(4); // consume 4 characters
						return ProcessLabel();
					}
					break;
					
				case 148:
					// #line 393
					{
						return Identifier(Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 149:
					// #line 401
					{
						return Identifier(Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 150:
					// #line 709
					{
						return Identifier(Tokens.T_FUNC_C);
					}
					break;
					
				case 151:
					// #line 729
					{
						return Identifier(Tokens.T_NS_C);
					}
					break;
					
				case 152:
					// #line 444
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return Identifier(Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 153:
					// #line 941
					{ yymore(); break; }
					break;
					
				case 154:
					// #line 86
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 155:
					// #line 939
					{ yymore(); break; }
					break;
					
				case 156:
					// #line 937
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 157:
					// #line 940
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 935
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 159:
					// #line 934
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 160:
					// #line 936
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 161:
					// #line 885
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 83
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 163:
					// #line 884
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 886
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 165:
					// #line 883
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 951
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 949
					{ yymore(); break; }
					break;
					
				case 168:
					// #line 947
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 169:
					// #line 950
					{ yymore(); break; }
					break;
					
				case 170:
					// #line 945
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 171:
					// #line 944
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 172:
					// #line 946
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 173:
					// #line 960
					{ yymore(); break; }
					break;
					
				case 174:
					// #line 959
					{ yymore(); break; }
					break;
					
				case 175:
					// #line 957
					{ yymore(); break; }
					break;
					
				case 176:
					// #line 871
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(_processDoubleQuotedString) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 177:
					// #line 958
					{ yymore(); break; }
					break;
					
				case 178:
					// #line 954
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 179:
					// #line 953
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 180:
					// #line 955
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 181:
					// #line 323
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 182:
					// #line 318
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 183:
					// #line 310
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 184:
					// #line 314
					{
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 185:
					// #line 666
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 186:
					// #line 658
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 187:
					// #line 803
					{ yymore(); break; }
					break;
					
				case 188:
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
					
				case 189:
					// #line 805
					{ yymore(); break; }
					break;
					
				case 190:
					// #line 804
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 191:
					// #line 798
					{ yymore(); break; }
					break;
					
				case 192:
					// #line 98
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 193:
					// #line 800
					{ yymore(); break; }
					break;
					
				case 194:
					// #line 799
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 195:
					// #line 965
					{ yymore(); break; }
					break;
					
				case 196:
					// #line 102
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 197:
					// #line 964
					{ yymore(); break; }
					break;
					
				case 198:
					// #line 962
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 199:
					// #line 963
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 200:
					// #line 775
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 201:
					// #line 689
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 202:
					// #line 770
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 203:
					// #line 693
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 204:
					// #line 858
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 205:
					// #line 847
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
					
				case 206:
					// #line 880
					{ yymore(); break; }
					break;
					
				case 207:
					// #line 862
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(null) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 208:
					// #line 456
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 209:
					// #line 453
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 210:
					// #line 450
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 211:
					// #line 429
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 212:
					// #line 454
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 213:
					// #line 452
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 214:
					// #line 434
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 215:
					// #line 439
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 216:
					// #line 927
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 217:
					// #line 916
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 218:
					// #line 905
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 219:
					// #line 888
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 220:
					// #line 910
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 221:
					// #line 899
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 222:
					// #line 893
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 223:
					// #line 921
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 225: goto case 1;
				case 227: goto case 6;
				case 228: goto case 9;
				case 229: goto case 10;
				case 230: goto case 11;
				case 231: goto case 13;
				case 232: goto case 22;
				case 233: goto case 34;
				case 234: goto case 45;
				case 235: goto case 102;
				case 236: goto case 104;
				case 237: goto case 155;
				case 238: goto case 163;
				case 239: goto case 167;
				case 240: goto case 174;
				case 241: goto case 175;
				case 242: goto case 181;
				case 243: goto case 185;
				case 244: goto case 195;
				case 245: goto case 198;
				case 246: goto case 200;
				case 247: goto case 201;
				case 248: goto case 203;
				case 249: goto case 208;
				case 250: goto case 216;
				case 253: goto case 9;
				case 254: goto case 10;
				case 255: goto case 22;
				case 256: goto case 34;
				case 257: goto case 104;
				case 258: goto case 181;
				case 259: goto case 203;
				case 260: goto case 216;
				case 262: goto case 9;
				case 263: goto case 10;
				case 264: goto case 203;
				case 266: goto case 9;
				case 267: goto case 10;
				case 269: goto case 9;
				case 270: goto case 10;
				case 272: goto case 9;
				case 273: goto case 10;
				case 275: goto case 9;
				case 276: goto case 10;
				case 278: goto case 9;
				case 279: goto case 10;
				case 281: goto case 9;
				case 282: goto case 10;
				case 284: goto case 9;
				case 285: goto case 10;
				case 287: goto case 9;
				case 288: goto case 10;
				case 290: goto case 9;
				case 291: goto case 10;
				case 293: goto case 9;
				case 294: goto case 10;
				case 296: goto case 9;
				case 297: goto case 10;
				case 299: goto case 9;
				case 300: goto case 10;
				case 302: goto case 9;
				case 303: goto case 10;
				case 305: goto case 10;
				case 307: goto case 10;
				case 309: goto case 10;
				case 311: goto case 10;
				case 313: goto case 10;
				case 315: goto case 10;
				case 317: goto case 10;
				case 319: goto case 10;
				case 321: goto case 10;
				case 323: goto case 10;
				case 325: goto case 10;
				case 327: goto case 10;
				case 329: goto case 10;
				case 331: goto case 10;
				case 333: goto case 10;
				case 335: goto case 10;
				case 337: goto case 10;
				case 339: goto case 10;
				case 341: goto case 10;
				case 343: goto case 10;
				case 345: goto case 10;
				case 347: goto case 10;
				case 349: goto case 10;
				case 351: goto case 10;
				case 353: goto case 10;
				case 355: goto case 10;
				case 357: goto case 10;
				case 359: goto case 10;
				case 361: goto case 10;
				case 363: goto case 10;
				case 365: goto case 10;
				case 367: goto case 10;
				case 369: goto case 10;
				case 371: goto case 10;
				case 373: goto case 10;
				case 375: goto case 10;
				case 377: goto case 10;
				case 379: goto case 10;
				case 381: goto case 10;
				case 383: goto case 10;
				case 385: goto case 10;
				case 387: goto case 10;
				case 389: goto case 10;
				case 391: goto case 10;
				case 393: goto case 10;
				case 395: goto case 10;
				case 397: goto case 10;
				case 399: goto case 10;
				case 401: goto case 10;
				case 403: goto case 10;
				case 405: goto case 10;
				case 407: goto case 10;
				case 409: goto case 10;
				case 411: goto case 10;
				case 413: goto case 10;
				case 415: goto case 10;
				case 417: goto case 10;
				case 419: goto case 10;
				case 421: goto case 10;
				case 423: goto case 10;
				case 425: goto case 10;
				case 427: goto case 10;
				case 429: goto case 10;
				case 431: goto case 10;
				case 466: goto case 10;
				case 478: goto case 10;
				case 481: goto case 10;
				case 483: goto case 10;
				case 485: goto case 10;
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
				case 698: goto case 10;
				case 699: goto case 10;
				case 700: goto case 10;
				case 701: goto case 10;
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
			AcceptConditions.Accept, // 174
			AcceptConditions.Accept, // 175
			AcceptConditions.AcceptOnStart, // 176
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
			AcceptConditions.Accept, // 205
			AcceptConditions.Accept, // 206
			AcceptConditions.AcceptOnStart, // 207
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
			AcceptConditions.Accept, // 222
			AcceptConditions.Accept, // 223
			AcceptConditions.NotAccept, // 224
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
			AcceptConditions.Accept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.NotAccept, // 251
			AcceptConditions.Accept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.Accept, // 254
			AcceptConditions.Accept, // 255
			AcceptConditions.Accept, // 256
			AcceptConditions.Accept, // 257
			AcceptConditions.Accept, // 258
			AcceptConditions.Accept, // 259
			AcceptConditions.Accept, // 260
			AcceptConditions.NotAccept, // 261
			AcceptConditions.Accept, // 262
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
			AcceptConditions.Accept, // 300
			AcceptConditions.NotAccept, // 301
			AcceptConditions.Accept, // 302
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
			AcceptConditions.Accept, // 423
			AcceptConditions.NotAccept, // 424
			AcceptConditions.Accept, // 425
			AcceptConditions.NotAccept, // 426
			AcceptConditions.Accept, // 427
			AcceptConditions.NotAccept, // 428
			AcceptConditions.Accept, // 429
			AcceptConditions.NotAccept, // 430
			AcceptConditions.Accept, // 431
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
			AcceptConditions.NotAccept, // 462
			AcceptConditions.NotAccept, // 463
			AcceptConditions.NotAccept, // 464
			AcceptConditions.NotAccept, // 465
			AcceptConditions.Accept, // 466
			AcceptConditions.NotAccept, // 467
			AcceptConditions.NotAccept, // 468
			AcceptConditions.NotAccept, // 469
			AcceptConditions.NotAccept, // 470
			AcceptConditions.NotAccept, // 471
			AcceptConditions.NotAccept, // 472
			AcceptConditions.NotAccept, // 473
			AcceptConditions.NotAccept, // 474
			AcceptConditions.NotAccept, // 475
			AcceptConditions.NotAccept, // 476
			AcceptConditions.NotAccept, // 477
			AcceptConditions.Accept, // 478
			AcceptConditions.NotAccept, // 479
			AcceptConditions.NotAccept, // 480
			AcceptConditions.Accept, // 481
			AcceptConditions.NotAccept, // 482
			AcceptConditions.Accept, // 483
			AcceptConditions.NotAccept, // 484
			AcceptConditions.Accept, // 485
			AcceptConditions.NotAccept, // 486
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
			AcceptConditions.Accept, // 698
			AcceptConditions.Accept, // 699
			AcceptConditions.Accept, // 700
			AcceptConditions.Accept, // 701
		};
		
		private static int[] colMap = new int[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 38, 19, 0, 0, 61, 0, 0, 
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			38, 48, 63, 15, 54, 50, 51, 64, 37, 39, 45, 47, 55, 29, 35, 44, 
			58, 59, 60, 60, 60, 60, 60, 60, 32, 32, 33, 43, 49, 46, 30, 2, 
			55, 21, 24, 11, 7, 3, 8, 26, 22, 5, 40, 25, 18, 20, 9, 12, 
			27, 42, 14, 13, 6, 10, 36, 23, 4, 17, 31, 16, 34, 62, 53, 41, 
			65, 21, 24, 11, 7, 3, 8, 26, 22, 5, 40, 25, 18, 20, 9, 12, 
			27, 42, 14, 13, 6, 10, 36, 23, 4, 17, 31, 56, 52, 57, 55, 0, 
			28, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 1, 2, 1, 1, 1, 1, 3, 4, 5, 6, 7, 1, 8, 
			1, 1, 1, 1, 1, 9, 10, 11, 11, 11, 11, 1, 11, 1, 1, 1, 
			12, 1, 13, 1, 1, 14, 1, 15, 1, 1, 16, 1, 1, 17, 18, 19, 
			1, 1, 1, 1, 1, 1, 1, 20, 1, 1, 11, 11, 11, 21, 11, 11, 
			11, 1, 1, 11, 1, 1, 1, 1, 1, 22, 23, 24, 11, 11, 25, 11, 
			11, 11, 11, 26, 11, 11, 11, 11, 11, 27, 11, 11, 11, 11, 11, 28, 
			11, 11, 11, 11, 11, 1, 1, 29, 1, 11, 11, 11, 11, 11, 11, 1, 
			1, 11, 30, 11, 11, 11, 11, 31, 11, 1, 1, 11, 11, 11, 11, 11, 
			11, 11, 1, 1, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 
			11, 1, 11, 1, 11, 11, 11, 11, 11, 32, 1, 33, 1, 1, 1, 1, 
			1, 34, 1, 34, 1, 1, 35, 36, 1, 1, 1, 1, 1, 37, 1, 38, 
			39, 1, 1, 1, 1, 1, 40, 1, 1, 1, 1, 41, 1, 42, 1, 43, 
			1, 44, 1, 45, 1, 46, 1, 1, 1, 47, 1, 48, 1, 49, 1, 50, 
			1, 1, 51, 1, 52, 53, 1, 1, 1, 1, 54, 1, 1, 1, 1, 1, 
			55, 56, 57, 58, 1, 59, 1, 60, 1, 61, 1, 62, 63, 64, 65, 66, 
			67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 
			83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 
			99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 
			115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 73, 129, 
			130, 131, 99, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 
			145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 26, 159, 
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 
			176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 
			192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 
			208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 
			224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 
			240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 
			256, 257, 258, 259, 260, 261, 262, 263, 264, 57, 265, 266, 267, 268, 70, 269, 
			270, 271, 272, 273, 274, 275, 276, 277, 79, 278, 53, 279, 280, 281, 282, 283, 
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
			444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 
			460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 
			476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 
			492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 
			508, 509, 510, 11, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 225, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 224, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 268, 22, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 487, 691, 691, 691, 691, 488, 691, 489, 691, 691, 691, -1, -1, 691, 490, -1, 491, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 492, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1 },
			{ -1, -1, -1, 274, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, 34, -1, -1, -1, -1, -1, 277, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, 50, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 56, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 232, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 255, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 274, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, 233, 233, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 334, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, -1, -1, 336, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, -1, -1, 55, 55, -1, 55, 55, 55, 55, 55, 55, 55, 55, -1, -1, -1, 55, 55, -1, -1, -1, 55, -1, -1, -1, 55, 55, 55, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 55, 55, 55, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 677, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, -1, -1, -1, 73, 73, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, 312, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, 73, 73, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 74, 74, 74, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, 75, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 353, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 83, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 83, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 83, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 373, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, 380, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, 380, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, 380, -1, -1, -1, -1 },
			{ -1, -1, -1, 681, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 604, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 698, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ 153, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1, 153, 153, 153, 153, 153, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, -1, 153, -1, 153, 153, 153, 153, 153, 153, -1, 153, 153 },
			{ -1, -1, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, -1, -1, 158, 158, -1, 158, 158, 158, 158, 158, 158, 158, 158, -1, -1, -1, 158, -1, -1, -1, -1, 158, -1, -1, -1, 158, 158, 158, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 159, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161 },
			{ 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, -1 },
			{ -1, -1, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, -1, 170, -1, -1, -1, -1, 170, -1, -1, -1, 170, 170, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, -1, 173, 173, 173, 173, 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, -1, 173, -1, 173, 173, 173, 173, -1, 173, 173, 173, 173 },
			{ -1, -1, -1, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, -1, -1, 178, 178, -1, 178, 178, 178, 178, 178, 178, 178, 178, -1, -1, -1, 178, -1, -1, -1, -1, 178, -1, -1, -1, 178, 178, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 179, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, -1, -1, 176, 176, -1, 176, 176, 176, 176, 176, 176, 176, 176, -1, -1, -1, 176, 176, -1, -1, -1, 176, -1, -1, -1, 176, 176, 176, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, 176, 176, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, -1, -1, 182, 182, -1, 182, 182, 182, 182, 182, 182, 182, 182, -1, -1, -1, 182, 182, -1, -1, -1, 182, -1, -1, -1, 182, 182, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 182, 182, 182, -1, -1, -1, -1, -1 },
			{ 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, -1, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 190, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 191, -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 244, 196, 197, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 198, 244, 244, 244, 244, 244, 244, 244, 244, 3, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 245, 244, 244, 244, 244 },
			{ -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, 450, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, 201, 201, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, 450, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, 203, 203, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, -1, -1, 205, 205, -1, 205, 205, 205, 205, 205, 205, 205, 205, -1, -1, -1, 205, 205, -1, -1, -1, 205, -1, -1, -1, 205, 205, 205, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, 205, 205, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, -1, -1, 207, 207, -1, 207, 207, 207, 207, 207, 207, 207, 207, -1, -1, -1, 207, 207, -1, -1, -1, 207, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 458, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 213, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 213, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 213, -1, -1, -1, -1 },
			{ -1, -1, -1, 218, 218, 218, 218, 218, 218, 218, 218, 218, 218, 218, 218, -1, 221, 218, 218, -1, 218, 218, 218, 218, 218, 218, 218, 218, -1, 462, -1, 218, 218, -1, -1, -1, 218, -1, -1, -1, 218, 218, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, 218, 218, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 251, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, -1, -1, 176, 176, -1, 176, 176, 176, 176, 176, 176, 176, 176, -1, -1, -1, 176, -1, -1, -1, -1, 176, -1, 441, -1, 176, 176, 176, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 23, 493, 691, 691, 691, 494, 691, -1, -1, 691, 691, -1, 622, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 274, 312, -1, -1, -1, -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, 34, -1, -1, -1, -1, -1, 277, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 274, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, -1, -1, -1, -1, -1, -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, 233, 233, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 477, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 160, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 163, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 180, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 444, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 446, 446, 446, 446, 446, 446, 446, 446, 446, 446, 446, 446, -1, 186, 446, 446, -1, 446, 446, 446, 446, 446, 446, 446, 446, -1, -1, -1, 446, 446, -1, -1, -1, 446, -1, -1, -1, 446, 446, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, 446, 446, 446, -1, -1, -1, -1, -1 },
			{ 244, -1, -1, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, -1, 244, 244, 244, 244, 244, 244, 244, 244, -1, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, 244, -1, 244, 244, 244, 244 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, -1, -1, 55, 55, -1, 55, 55, 55, 55, 55, 55, 55, 55, -1, -1, -1, 55, -1, -1, -1, -1, 55, -1, -1, -1, 55, 55, 55, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 451, -1, -1, -1, -1, -1, -1, -1, 452, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 453, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, 450, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, 203, 203, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 248, -1, -1, -1, 248, 248, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, 248, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, 451, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 248, 248, 248, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 209, 212, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 218, 218, 218, 218, 218, 218, 218, 218, 218, 218, 218, 218, -1, -1, 218, 218, -1, 218, 218, 218, 218, 218, 218, 218, 218, -1, -1, -1, 218, -1, -1, -1, -1, 218, -1, -1, -1, 218, 218, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 261, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, -1, -1, 207, 207, -1, 207, 207, 207, 207, 207, 207, 207, 207, -1, -1, -1, 207, -1, -1, -1, -1, 207, -1, 456, -1, 207, 207, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 496, 691, 282, 691, 691, 691, 691, 691, 691, 24, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 232, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 256, 256, 256, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 402, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 452, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 259, 259, 259, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 497, 691, 691, 691, 25, 625, 691, 285, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 453, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 264, 264, -1, -1, -1, -1, -1, -1 },
			{ 7, 8, 9, 10, 466, 229, 478, 254, 263, 481, 483, 621, 267, 650, 671, 11, 228, 682, 687, 12, 689, 270, 691, 692, 273, 691, 693, 694, 3, 253, 262, 691, 13, 266, 14, 269, 485, 272, 12, 228, 691, 695, 691, 228, 275, 278, 281, 284, 287, 290, 293, 15, 296, 299, 302, 228, 16, 17, 231, 13, 13, 12, 228, 18, 19, 20 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 26, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 57, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, -1, -1, 280, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, 233, 233, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 294, 691, 691, 691, 28, 624, -1, -1, 691, 691, -1, 691, 691, 691, 691, 651, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 467, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 283, -1, 286, 289, -1, 468, -1, 292, 295, 298, -1, -1, -1, -1, -1, -1, 301, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 306, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 628, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, 271, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, 18, 19, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 256, 256, 256, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 230, 37, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 58, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 59, 691, -1, 691, 517, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 60, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 469, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 61, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 62, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 234, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 63, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 64, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 470, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, -1, -1, -1, -1, -1, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 67, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 54, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 76, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 479, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 77, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1, 471, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 78, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 79, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 80, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 81, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, -1, -1, -1, 73, 73, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, 73, 73, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 82, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 74, 74, 74, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 84, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, 75, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, 358, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, 358, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, 358, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 256, 256, 256, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 85, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, 233, 233, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 86, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 87, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 472, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 88, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 89, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 473, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 90, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 91, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 92, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 93, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 354, 354, 354, 354, 354, 354, 354, 354, 354, 354, 354, 354, -1, -1, 354, 354, -1, 354, 354, 354, 354, 354, 354, 354, 354, -1, -1, -1, 354, -1, -1, -1, -1, 354, -1, 336, -1, 354, 354, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, 475, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 94, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 95, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 96, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 480, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 97, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 476, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 98, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 368, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 99, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 100, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 103, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 482, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 105, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 354, 354, 354, 354, 354, 354, 354, 354, 354, 354, 354, 354, -1, -1, 354, 354, 102, 354, 354, 354, 354, 354, 354, 354, 354, -1, -1, -1, 354, 354, -1, -1, -1, 354, -1, -1, -1, 354, 354, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 354, 354, 354, 235, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 106, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 376, 376, 376, 376, 376, 376, 376, 376, 376, 376, 376, 376, -1, -1, 376, 376, -1, 376, 376, 376, 376, 376, 376, 376, 376, -1, -1, -1, 376, -1, -1, -1, -1, 376, -1, -1, -1, 376, 376, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 107, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, 104, -1, 236, 104, 257, 104, 104, 104, 104, 104, 104, 104, 104, 104, -1, -1, 104, 104, 358, 104, 104, 104, 104, 104, 104, 104, 104, 104, -1, -1, 104, -1, -1, -1, -1, 104, -1, 358, -1, 104, 104, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 108, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 382, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 109, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 110, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 113, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 388, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 114, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 390, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 115, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 370, 111, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 116, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 392, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 117, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 396, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 398, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 118, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 376, 376, 376, 376, 376, 376, 376, 376, 376, 376, 376, 376, -1, -1, 376, 376, -1, 376, 376, 376, 376, 376, 376, 376, 376, -1, -1, -1, 376, 376, -1, -1, -1, 376, -1, -1, -1, 376, 376, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 376, 376, 376, -1, -1, 400, -1, -1 },
			{ -1, -1, -1, 119, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 378, 378, 378, 378, 378, 378, 378, 378, 378, 378, 378, 378, -1, -1, 378, 378, -1, 378, 378, 378, 378, 378, 378, 378, 378, -1, -1, -1, 378, 378, -1, -1, -1, 378, -1, -1, -1, 378, 378, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, 378, 378, -1, -1, -1, 400, -1 },
			{ -1, -1, -1, 120, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 484, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 123, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 486, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 124, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 125, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 386, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 126, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 404, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 127, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 406, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 128, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 392, 122, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 129, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 406, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 132, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 408, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 133, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 398, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 134, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 235, -1, -1, -1, -1 },
			{ -1, -1, -1, 135, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 412, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 136, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 404, 130, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 137, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 406, 131, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 138, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 398, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 139, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 416, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 140, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 418, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 141, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 420, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 142, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 422, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 143, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 424, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 144, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 145, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 146, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 426, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 148, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 428, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 149, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 147, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 150, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 430, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 151, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 432, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 152, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 426, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 153, 154, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 3, 153, 153, 153, 153, 153, 434, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 155, 153, 237, 153, 153, 153, 153, 153, 153, 156, 153, 153 },
			{ 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157 },
			{ 161, 162, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 163, 161, 161, 161, 161, 161, 161, 161, 161, 3, 161, 161, 161, 161, 161, 436, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 238, 161, 161, 164, 161 },
			{ 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165 },
			{ 166, 162, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 3, 166, 166, 166, 166, 166, 438, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 167, 166, 239, 166, 166, 166, 166, 166, 166, 166, 166, 168 },
			{ 169, -1, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, -1, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169 },
			{ 173, 162, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 174, 173, 173, 173, 173, 173, 173, 173, 173, 226, 173, 173, 173, 173, 173, 440, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 175, 173, 241, 173, 173, 173, 173, 240, 173, 173, 173, 173 },
			{ 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177 },
			{ -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 181, 8, 242, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 181, 181, 182, 182, 12, 182, 182, 182, 182, 182, 182, 182, 182, 3, 258, 181, 182, 181, 181, 181, 181, 182, 181, 12, 181, 182, 182, 182, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 12, 181, 181, 181, 181 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 184, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 185, 8, 185, 243, 243, 243, 243, 243, 243, 243, 243, 243, 243, 243, 243, 185, 185, 243, 243, 185, 243, 243, 243, 243, 243, 243, 243, 243, 3, 185, 185, 243, 185, 185, 185, 185, 243, 185, 185, 185, 243, 243, 243, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185 },
			{ 187, 188, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 3, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 189, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187 },
			{ 191, 192, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 3, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 193, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191 },
			{ 7, 8, 200, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 7, 200, 691, 691, 7, 691, 691, 691, 691, 691, 691, 691, 691, 3, 200, 200, 691, 201, 200, 7, 200, 691, 200, 7, 200, 691, 691, 691, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 246, 200, 200, 200, 247, 201, 201, 7, 202, 200, 7, 200 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, 203, 203, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 248, -1, -1, -1, 248, 248, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, 248, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 248, 248, 248, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 259, 259, 259, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 264, 264, -1, -1, -1, -1, -1, -1 },
			{ 204, 8, 204, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 204, 204, 205, 205, 204, 205, 205, 205, 205, 205, 205, 205, 205, 3, 204, 204, 205, 204, 204, 204, 204, 205, 204, 204, 204, 205, 205, 205, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204 },
			{ 206, 162, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 252, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ 208, 8, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 209, 208, 208, 208, 210, 208, 208, 208, 208, 208, 208, 208, 208, 3, 208, 208, 208, 208, 208, 208, 208, 208, 211, 210, 208, 208, 208, 208, 208, 249, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 210, 208, 208, 208, 208 },
			{ 208, 8, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 209, 208, 208, 208, 210, 208, 208, 208, 208, 208, 208, 208, 208, 3, 208, 208, 208, 208, 208, 208, 208, 208, 208, 210, 214, 208, 208, 208, 208, 249, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 210, 208, 208, 208, 208 },
			{ 208, 8, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 209, 208, 208, 208, 210, 208, 208, 208, 208, 208, 208, 208, 208, 3, 208, 208, 208, 208, 208, 208, 208, 208, 208, 210, 208, 208, 208, 208, 215, 249, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 210, 208, 208, 208, 208 },
			{ 216, 8, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 3, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 250, 216, 260, 216, 216, 216, 216, 216, 216, 217, 216, 216 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 463, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 222, -1, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, -1, -1, 222, 222, -1, 222, 222, 222, 222, 222, 222, 222, 222, 222, -1, -1, 222, -1, -1, -1, -1, 222, -1, -1, -1, 222, 222, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 216, 3, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 3, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 250, 216, 260, 216, 216, 216, 216, 216, 216, 216, 216, 223 },
			{ 216, 3, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 3, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 216, 250, 216, 260, 216, 216, 216, 216, 216, 216, 216, 216, 216 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 276, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 336, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 326, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 474, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 366, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 378, 378, 378, 378, 378, 378, 378, 378, 378, 378, 378, 378, -1, -1, 378, 378, -1, 378, 378, 378, 378, 378, 378, 378, 378, -1, -1, -1, 378, -1, -1, -1, -1, 378, -1, -1, -1, 378, 378, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 386, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 410, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 279, -1, -1, 691, 691, -1, 691, 691, 495, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 288, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 498, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 394, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 623, 691, 691, 691, 291, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 414, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 297, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 300, 653, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 512, 691, 691, 513, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 303, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 305, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 514, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 307, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 672, 691, 691, 691, 691, 515, 691, 629, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 516, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 518, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 626, 691, 691, 655, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 519, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 683, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 523, 691, 691, -1, -1, 691, 691, -1, 691, 524, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 525, 691, 691, 691, 691, 691, 691, 309, 691, -1, -1, 691, 700, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 654, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 673, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 526, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 527, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 632, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 528, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 311, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 529, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 313, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 630, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 674, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 534, 691, 691, 691, 691, 691, 691, 633, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 535, 536, 537, 538, 691, 684, 691, 691, 691, -1, -1, 691, 635, -1, 539, 691, 636, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 315, 691, 637, 541, 691, 691, 691, 691, 542, 691, -1, -1, 691, 691, -1, 691, 691, 691, 543, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 317, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 319, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 656, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 321, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 323, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 325, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 327, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 657, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 329, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 659, 691, 691, 691, 691, 691, 691, 331, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 333, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 335, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 337, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 548, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 549, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 339, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 341, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 343, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 658, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 345, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 347, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 349, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 688, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 690, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 552, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 553, 691, 691, 691, 691, 638, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 554, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 556, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 557, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 351, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 559, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 642, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 640, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 562, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 566, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 355, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 357, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 359, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 665, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 361, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 363, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 570, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 571, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 644, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 572, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 573, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 365, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 575, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 646, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 577, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 367, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 579, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 369, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 371, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 375, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 647, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 582, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 377, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 379, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 381, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 585, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 667, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 587, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 588, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 669, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 383, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 590, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 591, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 592, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 385, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 387, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 389, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 391, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 393, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 395, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 397, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 600, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 601, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 399, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 401, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 403, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 605, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 606, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 405, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 407, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 409, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 701, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 607, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 411, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 608, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 648, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 413, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 415, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 609, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 417, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 419, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 610, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 421, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 612, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 615, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 616, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 423, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 425, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 427, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 617, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 618, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 429, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 619, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 620, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 431, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 652, 691, 691, -1, -1, 691, 499, -1, 691, 500, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 631, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 521, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 530, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 520, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 675, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 532, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 533, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 544, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 550, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 661, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 676, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 678, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 560, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 662, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 639, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 558, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 699, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 574, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 578, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 668, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 576, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 581, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 645, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 598, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 589, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 594, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 611, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 613, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 501, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 502, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 531, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 522, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 540, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 546, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 660, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 561, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 664, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 663, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 643, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 564, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 696, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 666, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 584, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 580, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 583, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 586, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 599, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 595, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 602, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 614, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 503, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 634, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 547, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 551, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 563, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 568, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 565, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 641, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 593, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 596, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 603, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 504, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 545, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 555, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 567, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 597, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 505, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 569, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 506, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 697, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 627, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 507, 691, 691, -1, -1, 691, 508, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 509, 691, 691, 691, 510, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 511, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 679, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 680, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 649, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 686, 691, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, 691, 685, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 691, 691, 691, 691, 691, 691, 691, 691, 691, 670, 691, 691, -1, -1, 691, 691, -1, 691, 691, 691, 691, 691, 691, 691, 691, -1, -1, -1, 691, 691, -1, -1, -1, 691, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 691, 691, 691, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  265,
			  433,
			  435,
			  437,
			  439,
			  442,
			  443,
			  445,
			  447,
			  448,
			  195,
			  449,
			  454,
			  455,
			  457,
			  459,
			  460,
			  461,
			  464,
			  465
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 702);
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

