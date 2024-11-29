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
					// #line 782
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
					// #line 769
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
					// #line 749
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
					// #line 759
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
					// #line 983
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
					// #line 649
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 10:
					// #line 803
					{
						return ProcessLabel();
					}
					break;
					
				case 11:
					// #line 807
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
					// #line 693
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
					// #line 645
					{
						return (Tokens.T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG);
					}
					break;
					
				case 16:
					// #line 654
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 17:
					// #line 662
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 18:
					// #line 949
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 898
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 959
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 21:
					// #line 341
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 22:
					// #line 823
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
					// #line 620
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
					// #line 524
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
					// #line 560
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 32:
					// #line 636
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 33:
					// #line 552
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 34:
					// #line 709
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
					// #line 580
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 37:
					// #line 813
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 38:
					// #line 576
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 39:
					// #line 568
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 40:
					// #line 564
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 41:
					// #line 504
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 42:
					// #line 536
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 43:
					// #line 556
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 44:
					// #line 520
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 45:
					// #line 540
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 46:
					// #line 548
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 47:
					// #line 632
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 48:
					// #line 584
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 49:
					// #line 596
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 50:
					// #line 616
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 51:
					// #line 640
					{
						yyless(1);
						return (Tokens.T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG);
					}
					break;
					
				case 52:
					// #line 600
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 53:
					// #line 612
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 54:
					// #line 604
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 55:
					// #line 828
					{
						return ProcessVariable();
					}
					break;
					
				case 56:
					// #line 608
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
					// #line 628
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
					// #line 624
					{
						return Identifier(Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 65:
					// #line 592
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
					// #line 572
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 69:
					// #line 528
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 70:
					// #line 532
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 71:
					// #line 544
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 72:
					// #line 588
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 73:
					// #line 697
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 74:
					// #line 689
					{
						return ProcessOctalNumber();
					}
					break;
					
				case 75:
					// #line 685
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
					// #line 508
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
					// #line 818
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
					// #line 500
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
					// #line 512
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
					// #line 832
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
					// #line 493
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
					// #line 741
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
					// #line 516
					{
						return Identifier(Tokens.T_CALLABLE);
					}
					break;
					
				case 128:
					// #line 496
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
					// #line 737
					{
						return Identifier(Tokens.T_FILE);
					}
					break;
					
				case 133:
					// #line 733
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
					// #line 717
					{
						return Identifier(Tokens.T_TRAIT_C);
					}
					break;
					
				case 140:
					// #line 713
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
					// #line 729
					{
						return Identifier(Tokens.T_METHOD_C);
					}
					break;
					
				case 147:
					// #line 481
					{
						return Identifier(Tokens.T_PUBLIC_SET);
					}
					break;
					
				case 148:
					// #line 275
					{
						yyless(4); // consume 4 characters
						return ProcessLabel();
					}
					break;
					
				case 149:
					// #line 393
					{
						return Identifier(Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 150:
					// #line 401
					{
						return Identifier(Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 151:
					// #line 489
					{
						return Identifier(Tokens.T_PRIVATE_SET);
					}
					break;
					
				case 152:
					// #line 721
					{
						return Identifier(Tokens.T_FUNC_C);
					}
					break;
					
				case 153:
					// #line 725
					{
						return Identifier(Tokens.T_PROPERTY_C);
					}
					break;
					
				case 154:
					// #line 745
					{
						return Identifier(Tokens.T_NS_C);
					}
					break;
					
				case 155:
					// #line 485
					{
						return Identifier(Tokens.T_PROTECTED_SET);
					}
					break;
					
				case 156:
					// #line 444
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return Identifier(Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 157:
					// #line 957
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 86
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 159:
					// #line 955
					{ yymore(); break; }
					break;
					
				case 160:
					// #line 953
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 161:
					// #line 956
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 951
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 163:
					// #line 950
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 164:
					// #line 952
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 165:
					// #line 901
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 83
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 167:
					// #line 900
					{ yymore(); break; }
					break;
					
				case 168:
					// #line 902
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 169:
					// #line 899
					{ yymore(); break; }
					break;
					
				case 170:
					// #line 967
					{ yymore(); break; }
					break;
					
				case 171:
					// #line 965
					{ yymore(); break; }
					break;
					
				case 172:
					// #line 963
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 173:
					// #line 966
					{ yymore(); break; }
					break;
					
				case 174:
					// #line 961
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 175:
					// #line 960
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 176:
					// #line 962
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 177:
					// #line 976
					{ yymore(); break; }
					break;
					
				case 178:
					// #line 975
					{ yymore(); break; }
					break;
					
				case 179:
					// #line 973
					{ yymore(); break; }
					break;
					
				case 180:
					// #line 887
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(_processDoubleQuotedString) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 181:
					// #line 974
					{ yymore(); break; }
					break;
					
				case 182:
					// #line 970
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 183:
					// #line 969
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 184:
					// #line 971
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 185:
					// #line 323
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 186:
					// #line 318
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 187:
					// #line 310
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 188:
					// #line 314
					{
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 189:
					// #line 678
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 190:
					// #line 670
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 191:
					// #line 819
					{ yymore(); break; }
					break;
					
				case 192:
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
					
				case 193:
					// #line 821
					{ yymore(); break; }
					break;
					
				case 194:
					// #line 820
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 195:
					// #line 814
					{ yymore(); break; }
					break;
					
				case 196:
					// #line 98
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 197:
					// #line 816
					{ yymore(); break; }
					break;
					
				case 198:
					// #line 815
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 199:
					// #line 981
					{ yymore(); break; }
					break;
					
				case 200:
					// #line 102
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 201:
					// #line 980
					{ yymore(); break; }
					break;
					
				case 202:
					// #line 978
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 203:
					// #line 979
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 204:
					// #line 791
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 205:
					// #line 701
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 206:
					// #line 786
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 207:
					// #line 705
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 208:
					// #line 874
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 209:
					// #line 863
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
					
				case 210:
					// #line 896
					{ yymore(); break; }
					break;
					
				case 211:
					// #line 878
					{
					    if(VerifyEndLabel(GetTokenSpan()))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(null) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 212:
					// #line 456
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 213:
					// #line 453
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 214:
					// #line 450
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 215:
					// #line 429
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 216:
					// #line 454
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 217:
					// #line 452
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 218:
					// #line 434
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 219:
					// #line 439
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 220:
					// #line 943
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 221:
					// #line 932
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 222:
					// #line 921
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 223:
					// #line 904
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 224:
					// #line 926
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 225:
					// #line 915
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 226:
					// #line 909
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 227:
					// #line 937
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 229: goto case 1;
				case 231: goto case 6;
				case 232: goto case 9;
				case 233: goto case 10;
				case 234: goto case 11;
				case 235: goto case 13;
				case 236: goto case 22;
				case 237: goto case 34;
				case 238: goto case 45;
				case 239: goto case 102;
				case 240: goto case 104;
				case 241: goto case 159;
				case 242: goto case 167;
				case 243: goto case 171;
				case 244: goto case 178;
				case 245: goto case 179;
				case 246: goto case 185;
				case 247: goto case 189;
				case 248: goto case 199;
				case 249: goto case 202;
				case 250: goto case 204;
				case 251: goto case 205;
				case 252: goto case 207;
				case 253: goto case 212;
				case 254: goto case 220;
				case 257: goto case 9;
				case 258: goto case 10;
				case 259: goto case 22;
				case 260: goto case 34;
				case 261: goto case 104;
				case 262: goto case 185;
				case 263: goto case 207;
				case 264: goto case 220;
				case 266: goto case 9;
				case 267: goto case 10;
				case 268: goto case 207;
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
				case 306: goto case 9;
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
				case 433: goto case 10;
				case 435: goto case 10;
				case 437: goto case 10;
				case 480: goto case 10;
				case 495: goto case 10;
				case 498: goto case 10;
				case 500: goto case 10;
				case 502: goto case 10;
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
				case 702: goto case 10;
				case 703: goto case 10;
				case 704: goto case 10;
				case 705: goto case 10;
				case 706: goto case 10;
				case 707: goto case 10;
				case 708: goto case 10;
				case 709: goto case 10;
				case 710: goto case 10;
				case 711: goto case 10;
				case 712: goto case 10;
				case 713: goto case 10;
				case 714: goto case 10;
				case 715: goto case 10;
				case 716: goto case 10;
				case 717: goto case 10;
				case 718: goto case 10;
				case 719: goto case 10;
				case 720: goto case 10;
				case 721: goto case 10;
				case 722: goto case 10;
				case 723: goto case 10;
				case 724: goto case 10;
				case 725: goto case 10;
				case 726: goto case 10;
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
			AcceptConditions.Accept, // 176
			AcceptConditions.Accept, // 177
			AcceptConditions.Accept, // 178
			AcceptConditions.Accept, // 179
			AcceptConditions.AcceptOnStart, // 180
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
			AcceptConditions.Accept, // 207
			AcceptConditions.Accept, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.Accept, // 210
			AcceptConditions.AcceptOnStart, // 211
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
			AcceptConditions.Accept, // 224
			AcceptConditions.Accept, // 225
			AcceptConditions.Accept, // 226
			AcceptConditions.Accept, // 227
			AcceptConditions.NotAccept, // 228
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
			AcceptConditions.Accept, // 251
			AcceptConditions.Accept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.Accept, // 254
			AcceptConditions.NotAccept, // 255
			AcceptConditions.Accept, // 256
			AcceptConditions.Accept, // 257
			AcceptConditions.Accept, // 258
			AcceptConditions.Accept, // 259
			AcceptConditions.Accept, // 260
			AcceptConditions.Accept, // 261
			AcceptConditions.Accept, // 262
			AcceptConditions.Accept, // 263
			AcceptConditions.Accept, // 264
			AcceptConditions.NotAccept, // 265
			AcceptConditions.Accept, // 266
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
			AcceptConditions.Accept, // 433
			AcceptConditions.NotAccept, // 434
			AcceptConditions.Accept, // 435
			AcceptConditions.NotAccept, // 436
			AcceptConditions.Accept, // 437
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
			AcceptConditions.NotAccept, // 466
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
			AcceptConditions.NotAccept, // 478
			AcceptConditions.NotAccept, // 479
			AcceptConditions.Accept, // 480
			AcceptConditions.NotAccept, // 481
			AcceptConditions.NotAccept, // 482
			AcceptConditions.NotAccept, // 483
			AcceptConditions.NotAccept, // 484
			AcceptConditions.NotAccept, // 485
			AcceptConditions.NotAccept, // 486
			AcceptConditions.NotAccept, // 487
			AcceptConditions.NotAccept, // 488
			AcceptConditions.NotAccept, // 489
			AcceptConditions.NotAccept, // 490
			AcceptConditions.NotAccept, // 491
			AcceptConditions.NotAccept, // 492
			AcceptConditions.NotAccept, // 493
			AcceptConditions.NotAccept, // 494
			AcceptConditions.Accept, // 495
			AcceptConditions.NotAccept, // 496
			AcceptConditions.NotAccept, // 497
			AcceptConditions.Accept, // 498
			AcceptConditions.NotAccept, // 499
			AcceptConditions.Accept, // 500
			AcceptConditions.NotAccept, // 501
			AcceptConditions.Accept, // 502
			AcceptConditions.NotAccept, // 503
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
			AcceptConditions.Accept, // 702
			AcceptConditions.Accept, // 703
			AcceptConditions.Accept, // 704
			AcceptConditions.Accept, // 705
			AcceptConditions.Accept, // 706
			AcceptConditions.Accept, // 707
			AcceptConditions.Accept, // 708
			AcceptConditions.Accept, // 709
			AcceptConditions.Accept, // 710
			AcceptConditions.Accept, // 711
			AcceptConditions.Accept, // 712
			AcceptConditions.Accept, // 713
			AcceptConditions.Accept, // 714
			AcceptConditions.Accept, // 715
			AcceptConditions.Accept, // 716
			AcceptConditions.Accept, // 717
			AcceptConditions.Accept, // 718
			AcceptConditions.Accept, // 719
			AcceptConditions.Accept, // 720
			AcceptConditions.Accept, // 721
			AcceptConditions.Accept, // 722
			AcceptConditions.Accept, // 723
			AcceptConditions.Accept, // 724
			AcceptConditions.Accept, // 725
			AcceptConditions.Accept, // 726
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
			11, 11, 11, 11, 11, 1, 1, 29, 1, 11, 11, 11, 11, 11, 30, 1, 
			1, 11, 31, 11, 11, 11, 11, 32, 33, 1, 1, 11, 11, 11, 11, 11, 
			11, 11, 1, 1, 11, 11, 11, 11, 11, 11, 34, 11, 11, 11, 11, 11, 
			11, 1, 11, 1, 1, 11, 11, 1, 11, 11, 11, 1, 11, 35, 1, 36, 
			1, 1, 1, 1, 1, 37, 1, 37, 1, 1, 38, 39, 1, 1, 1, 1, 
			1, 40, 1, 41, 42, 1, 1, 1, 1, 1, 43, 1, 1, 1, 1, 44, 
			1, 45, 1, 46, 1, 47, 1, 48, 1, 49, 1, 1, 1, 50, 1, 51, 
			1, 52, 1, 53, 1, 1, 54, 1, 55, 56, 1, 1, 1, 1, 57, 1, 
			1, 1, 1, 1, 58, 59, 60, 61, 1, 62, 1, 63, 1, 64, 1, 65, 
			66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 
			82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 
			98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 
			114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 
			130, 131, 76, 132, 133, 134, 102, 135, 136, 137, 138, 139, 140, 141, 142, 143, 
			144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 
			160, 161, 26, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 
			175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 
			191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 
			207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 
			223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 
			239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 
			255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 
			271, 272, 273, 274, 275, 276, 277, 60, 278, 279, 280, 281, 73, 282, 283, 284, 
			285, 286, 287, 288, 289, 290, 82, 291, 56, 292, 293, 294, 295, 296, 297, 298, 
			299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 
			315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 
			331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 
			347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 
			363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 
			379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 
			395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 
			411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 
			427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 
			443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 
			459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 
			475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 
			491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 
			507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 
			523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 11, 535, 536, 537, 
			538, 539, 540, 541, 542, 543, 544
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 229, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 272, 22, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 504, 716, 716, 716, 716, 505, 716, 506, 716, 716, 716, -1, -1, 716, 507, -1, 508, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 509, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, -1, -1, -1, -1 },
			{ -1, -1, -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, 34, -1, -1, -1, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 312, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, 50, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 56, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 259, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, 237, 237, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, -1, -1, 55, 55, -1, 55, 55, 55, 55, 55, 55, 55, 55, -1, -1, -1, 55, 55, -1, -1, -1, 55, -1, -1, -1, 55, 55, 55, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 55, 55, 55, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 702, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, -1, -1, -1, 73, 73, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, 73, 73, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 74, 74, 74, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, 75, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 357, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 83, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 83, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 83, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 377, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, 384, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, 384, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, 384, -1, -1, -1, -1 },
			{ -1, -1, -1, 706, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, 408, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 622, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 723, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, 422, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, 439, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, 157, 157, 157, 157, 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, 157, -1, 157, 157, 157, 157, 157, 157, -1, 157, 157 },
			{ -1, -1, -1, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, -1, -1, 162, 162, -1, 162, 162, 162, 162, 162, 162, 162, 162, -1, -1, -1, 162, -1, -1, -1, -1, 162, -1, -1, -1, 162, 162, 162, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 163, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, 165, 165, 165, 165, 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, 165 },
			{ 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, -1 },
			{ -1, -1, -1, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, 174, 174, -1, 174, 174, 174, 174, 174, 174, 174, 174, -1, -1, -1, 174, -1, -1, -1, -1, 174, -1, -1, -1, 174, 174, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 175, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, -1, 177, 177, 177, 177, 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, -1, 177, -1, 177, 177, 177, 177, -1, 177, 177, 177, 177 },
			{ -1, -1, -1, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, -1, -1, 182, 182, -1, 182, 182, 182, 182, 182, 182, 182, 182, -1, -1, -1, 182, -1, -1, -1, -1, 182, -1, -1, -1, 182, 182, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, -1, -1, 180, 180, -1, 180, 180, 180, 180, 180, 180, 180, 180, -1, -1, -1, 180, 180, -1, -1, -1, 180, -1, -1, -1, 180, 180, 180, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 180, 180, 180, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, -1, -1, 186, 186, -1, 186, 186, 186, 186, 186, 186, 186, 186, -1, -1, -1, 186, 186, -1, -1, -1, 186, -1, -1, -1, 186, 186, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, 186, 186, -1, -1, -1, -1, -1 },
			{ 191, -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, -1, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 195, -1, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, -1, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, -1, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 248, 200, 201, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 202, 248, 248, 248, 248, 248, 248, 248, 248, 3, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 249, 248, 248, 248, 248 },
			{ -1, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1, -1, -1, -1, -1, -1, -1, -1, 464, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, 205, 205, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, 464, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, -1, -1, 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, -1, -1, -1, 209, 209, -1, -1, -1, 209, -1, -1, -1, 209, 209, 209, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 209, 209, 209, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, -1, -1, 211, 211, -1, 211, 211, 211, 211, 211, 211, 211, 211, -1, -1, -1, 211, 211, -1, -1, -1, 211, -1, -1, -1, 211, 211, 211, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, 211, 211, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 472, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1 },
			{ -1, -1, -1, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, -1, 225, 222, 222, -1, 222, 222, 222, 222, 222, 222, 222, 222, -1, 476, -1, 222, 222, -1, -1, -1, 222, -1, -1, -1, 222, 222, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 222, 222, 222, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, -1, -1, 180, 180, -1, 180, 180, 180, 180, 180, 180, 180, 180, -1, -1, -1, 180, -1, -1, -1, -1, 180, -1, 455, -1, 180, 180, 180, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 23, 510, 716, 716, 716, 511, 716, -1, -1, 716, 716, -1, 643, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 278, 316, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, 34, -1, -1, -1, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, -1, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, 237, 237, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 491, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 164, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 167, 165, 165, 165, 165, 165, 165, 165, 165, -1, 165, 165, 165, 165, 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, 165 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 184, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 458, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 460, 460, 460, 460, 460, 460, 460, 460, 460, 460, 460, 460, -1, 190, 460, 460, -1, 460, 460, 460, 460, 460, 460, 460, 460, -1, -1, -1, 460, 460, -1, -1, -1, 460, -1, -1, -1, 460, 460, 460, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 190, 460, 460, 460, -1, -1, -1, -1, -1 },
			{ 248, -1, -1, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, -1, 248, 248, 248, 248, 248, 248, 248, 248, -1, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, 248, -1, 248, 248, 248, 248 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 202, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, -1, -1, 55, 55, -1, 55, 55, 55, 55, 55, 55, 55, 55, -1, -1, -1, 55, -1, -1, -1, -1, 55, -1, -1, -1, 55, 55, 55, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 465, -1, -1, -1, -1, -1, -1, -1, 466, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 467, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, 464, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 252, -1, -1, -1, 252, 252, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, -1, 252, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, 465, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 252, 252, 252, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 213, 216, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, 222, -1, -1, 222, 222, -1, 222, 222, 222, 222, 222, 222, 222, 222, -1, -1, -1, 222, -1, -1, -1, -1, 222, -1, -1, -1, 222, 222, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 223, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 265, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, -1, -1, 211, 211, -1, 211, 211, 211, 211, 211, 211, 211, 211, -1, -1, -1, 211, -1, -1, -1, -1, 211, -1, 470, -1, 211, 211, 211, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 513, 716, 286, 716, 716, 716, 716, 716, 716, 24, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 260, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 260, 260, 260, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 406, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 466, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, 263, 263, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 224, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 514, 716, 716, 716, 25, 646, 716, 289, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 467, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 268, 268, -1, -1, -1, -1, -1, -1 },
			{ 7, 8, 9, 10, 480, 233, 495, 258, 267, 498, 500, 642, 271, 674, 695, 11, 232, 707, 712, 12, 714, 274, 716, 717, 277, 716, 718, 719, 3, 257, 266, 716, 13, 270, 14, 273, 502, 276, 12, 232, 716, 720, 716, 232, 279, 282, 285, 288, 291, 294, 297, 15, 300, 303, 306, 232, 16, 17, 235, 13, 13, 12, 232, 18, 19, 20 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 26, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 57, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, 284, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, 237, 237, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 298, 716, 716, 716, 28, 645, -1, -1, 716, 716, -1, 716, 716, 716, 716, 675, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 481, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 287, -1, 290, 293, -1, 482, -1, 296, 299, 302, -1, -1, -1, -1, -1, -1, 305, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 649, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, 275, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, 18, 19, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, 260, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 260, 260, 260, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 234, 37, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 58, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 13, 13, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 59, 716, -1, 716, 534, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 60, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 483, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 61, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 326, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 62, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 238, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 63, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 64, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 484, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, -1, -1, -1, -1, -1, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 67, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 334, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 54, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 76, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 496, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 77, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 336, -1, -1, -1, -1, -1, -1, 485, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 78, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 79, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 80, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 312, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 81, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, -1, -1, -1, 73, 73, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, 73, 73, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 82, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 74, 74, 74, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 84, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, 75, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, 362, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, 362, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, 362, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 260, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 260, 260, 260, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 85, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, 237, 237, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 86, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 87, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 486, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 88, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 89, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 487, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 90, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 91, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 92, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 93, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, -1, -1, 358, 358, -1, 358, 358, 358, 358, 358, 358, 358, 358, -1, -1, -1, 358, -1, -1, -1, -1, 358, -1, 340, -1, 358, 358, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, 489, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 94, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 95, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 96, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 497, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 97, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 490, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 98, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 99, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 100, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 103, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 499, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 105, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, 358, -1, -1, 358, 358, 102, 358, 358, 358, 358, 358, 358, 358, 358, -1, -1, -1, 358, 358, -1, -1, -1, 358, -1, -1, -1, 358, 358, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, 358, 358, 239, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 106, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, -1, -1, 380, 380, -1, 380, 380, 380, 380, 380, 380, 380, 380, -1, -1, -1, 380, -1, -1, -1, -1, 380, -1, -1, -1, 380, 380, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 107, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, 104, -1, 240, 104, 261, 104, 104, 104, 104, 104, 104, 104, 104, 104, -1, -1, 104, 104, 362, 104, 104, 104, 104, 104, 104, 104, 104, 104, -1, -1, 104, -1, -1, -1, -1, 104, -1, 362, -1, 104, 104, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 108, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 386, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 109, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, 101, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 110, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 113, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 392, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 114, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 394, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 115, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, 111, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 116, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 396, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 117, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 400, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 402, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 118, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, 380, -1, -1, 380, 380, -1, 380, 380, 380, 380, 380, 380, 380, 380, -1, -1, -1, 380, 380, -1, -1, -1, 380, -1, -1, -1, 380, 380, 380, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 380, 380, 380, -1, -1, 404, -1, -1 },
			{ -1, -1, -1, 119, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, -1, -1, 382, 382, -1, 382, 382, 382, 382, 382, 382, 382, 382, -1, -1, -1, 382, 382, -1, -1, -1, 382, -1, -1, -1, 382, 382, 382, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 382, 382, 382, -1, -1, -1, 404, -1 },
			{ -1, -1, -1, 120, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 501, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 384, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 123, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 503, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 124, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 125, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 390, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 126, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 410, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 127, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 412, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 128, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 396, 122, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 129, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 412, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 132, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 414, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 133, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 402, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 134, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 239, -1, -1, -1, -1 },
			{ -1, -1, -1, 135, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 418, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 136, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 492, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 137, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 410, 130, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 138, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 412, 131, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 139, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 402, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 140, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 424, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 141, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 426, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 142, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 428, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 143, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 493, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 144, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 432, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 146, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 434, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 149, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 145, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 150, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 152, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 153, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 441, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 154, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 147, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 156, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 442, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 494, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 148, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 151, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 445, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 157, 158, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 3, 157, 157, 157, 157, 157, 448, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 159, 157, 241, 157, 157, 157, 157, 157, 157, 160, 157, 157 },
			{ 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161 },
			{ 165, 166, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 167, 165, 165, 165, 165, 165, 165, 165, 165, 3, 165, 165, 165, 165, 165, 450, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 242, 165, 165, 168, 165 },
			{ 169, -1, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, -1, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169 },
			{ 170, 166, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 3, 170, 170, 170, 170, 170, 452, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 171, 170, 243, 170, 170, 170, 170, 170, 170, 170, 170, 172 },
			{ 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173 },
			{ 177, 166, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 178, 177, 177, 177, 177, 177, 177, 177, 177, 230, 177, 177, 177, 177, 177, 454, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 179, 177, 245, 177, 177, 177, 177, 244, 177, 177, 177, 177 },
			{ 181, -1, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, -1, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181, 181 },
			{ -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 185, 8, 246, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 186, 185, 185, 186, 186, 12, 186, 186, 186, 186, 186, 186, 186, 186, 3, 262, 185, 186, 185, 185, 185, 185, 186, 185, 12, 185, 186, 186, 186, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 12, 185, 185, 185, 185 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 188, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 189, 8, 189, 247, 247, 247, 247, 247, 247, 247, 247, 247, 247, 247, 247, 189, 189, 247, 247, 189, 247, 247, 247, 247, 247, 247, 247, 247, 3, 189, 189, 247, 189, 189, 189, 189, 247, 189, 189, 189, 247, 247, 247, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189 },
			{ 191, 192, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 3, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 193, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191, 191 },
			{ 195, 196, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 3, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 197, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195 },
			{ 7, 8, 204, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 7, 204, 716, 716, 7, 716, 716, 716, 716, 716, 716, 716, 716, 3, 204, 204, 716, 205, 204, 7, 204, 716, 204, 7, 204, 716, 716, 716, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 250, 204, 204, 204, 251, 205, 205, 7, 206, 204, 7, 204 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, 207, 207, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 252, -1, -1, -1, 252, 252, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, -1, 252, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 252, 252, 252, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, 263, 263, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 268, 268, -1, -1, -1, -1, -1, -1 },
			{ 208, 8, 208, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 208, 208, 209, 209, 208, 209, 209, 209, 209, 209, 209, 209, 209, 3, 208, 208, 209, 208, 208, 208, 208, 209, 208, 208, 208, 209, 209, 209, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208, 208 },
			{ 210, 166, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 256, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210 },
			{ 212, 8, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 213, 212, 212, 212, 214, 212, 212, 212, 212, 212, 212, 212, 212, 3, 212, 212, 212, 212, 212, 212, 212, 212, 215, 214, 212, 212, 212, 212, 212, 253, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 214, 212, 212, 212, 212 },
			{ 212, 8, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 213, 212, 212, 212, 214, 212, 212, 212, 212, 212, 212, 212, 212, 3, 212, 212, 212, 212, 212, 212, 212, 212, 212, 214, 218, 212, 212, 212, 212, 253, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 214, 212, 212, 212, 212 },
			{ 212, 8, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 213, 212, 212, 212, 214, 212, 212, 212, 212, 212, 212, 212, 212, 3, 212, 212, 212, 212, 212, 212, 212, 212, 212, 214, 212, 212, 212, 212, 219, 253, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 214, 212, 212, 212, 212 },
			{ 220, 8, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 3, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 254, 220, 264, 220, 220, 220, 220, 220, 220, 221, 220, 220 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 477, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, -1, 226, 226, 226, 226, 226, 226, 226, 226, 226, 226, 226, 226, -1, -1, 226, 226, -1, 226, 226, 226, 226, 226, 226, 226, 226, 226, -1, -1, 226, -1, -1, -1, -1, 226, -1, -1, -1, 226, 226, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 220, 3, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 3, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 254, 220, 264, 220, 220, 220, 220, 220, 220, 220, 220, 227 },
			{ 220, 3, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 3, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 220, 254, 220, 264, 220, 220, 220, 220, 220, 220, 220, 220, 220 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 280, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 488, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 368, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, 382, -1, -1, 382, 382, -1, 382, 382, 382, 382, 382, 382, 382, 382, -1, -1, -1, 382, -1, -1, -1, -1, 382, -1, -1, -1, 382, 382, 382, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 390, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 416, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 430, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 438, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 444, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 283, -1, -1, 716, 716, -1, 716, 716, 512, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 388, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 292, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 515, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 398, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 644, 716, 716, 716, 295, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 420, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 301, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 304, 677, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 529, 716, 716, 530, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 307, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 309, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 531, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 311, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 696, 716, 716, 716, 716, 532, 716, 650, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 533, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 535, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 647, 716, 716, 679, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 536, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 708, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 540, 716, 716, -1, -1, 716, 716, -1, 716, 541, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 542, 716, 716, 716, 716, 716, 716, 313, 716, -1, -1, 716, 725, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 678, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 697, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 543, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 544, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 653, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 545, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 315, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 546, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 317, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 651, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 698, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 551, 716, 716, 716, 716, 716, 716, 654, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 552, 553, 554, 555, 716, 709, 716, 716, 716, -1, -1, 716, 656, -1, 556, 716, 657, 716, 716, 716, 716, 655, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 319, 716, 658, 558, 716, 716, 716, 716, 559, 716, -1, -1, 716, 716, -1, 716, 716, 716, 560, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 321, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 323, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 680, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 325, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 327, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 329, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 331, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 681, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 333, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 683, 716, 716, 716, 716, 716, 716, 335, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 337, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 339, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 341, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 565, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 566, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 343, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 345, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 347, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 700, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 349, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 351, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 353, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 713, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 715, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 569, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 570, 716, 716, 716, 716, 659, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 571, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 573, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 574, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 355, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 576, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 664, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 662, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 579, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 583, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 359, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 361, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 363, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 689, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 365, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 367, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 587, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 588, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 667, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 589, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 590, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 369, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 592, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 669, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 594, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 371, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 596, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 373, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 375, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 379, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 670, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 599, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 381, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 383, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 385, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 602, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 691, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 604, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 605, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 693, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 387, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 608, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 609, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 671, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 389, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 391, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 393, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 395, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 397, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 399, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 401, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 617, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 618, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 619, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 403, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 405, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 407, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 624, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 409, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 411, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 413, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 726, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 625, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 415, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 626, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 672, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 627, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 417, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 419, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 628, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 421, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 423, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 629, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 425, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 631, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 632, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 635, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 636, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 637, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 427, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 429, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 431, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 638, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 639, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 433, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 435, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 640, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 641, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 437, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 676, 716, 716, -1, -1, 716, 516, -1, 716, 517, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 652, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 538, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 547, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 537, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 699, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 549, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 550, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 561, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 567, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 685, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 701, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 703, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 660, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 686, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 661, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 575, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 724, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 666, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 591, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 595, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 692, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 593, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 598, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 606, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 668, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 615, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 607, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 611, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 623, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 630, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 633, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 518, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 519, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 548, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 539, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 557, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 563, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 684, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 578, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 688, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 577, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 665, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 581, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 721, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 690, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 601, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 597, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 600, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 603, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 616, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 612, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 620, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 634, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 520, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 682, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 564, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 568, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 580, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 687, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 585, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 582, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 610, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 613, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 621, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 521, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 562, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 572, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 584, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 614, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 522, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 586, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 523, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 722, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 648, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 524, 716, 716, -1, -1, 716, 525, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 526, 716, 716, 716, 527, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 528, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 704, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 705, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 673, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 711, 716, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, 716, 710, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 716, 716, 716, 716, 716, 716, 716, 716, 716, 694, 716, 716, -1, -1, 716, 716, -1, 716, 716, 716, 716, 716, 716, 716, 716, -1, -1, -1, 716, 716, -1, -1, -1, 716, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 716, 716, 716, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  269,
			  447,
			  449,
			  451,
			  453,
			  456,
			  457,
			  459,
			  461,
			  462,
			  199,
			  463,
			  468,
			  469,
			  471,
			  473,
			  474,
			  475,
			  478,
			  479
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 727);
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

