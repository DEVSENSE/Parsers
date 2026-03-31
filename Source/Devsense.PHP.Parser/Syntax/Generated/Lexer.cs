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
		
		private const int NoState = -1;
		private const char BOL = (char)128;
		private const char EOF = (char)129;
		
		private Tokens yyreturn;
		
		private ReadOnlyMemory<char> source_string;
		
		private ReadOnlySpan<char> buffer => source_string.Span;
		
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
		
		private bool yy_at_bol = false;
		
		public LexicalStates CurrentLexicalState { get { return current_lexical_state; } set { current_lexical_state = value; } } 
		private LexicalStates current_lexical_state;
		
		protected Lexer(ReadOnlyMemory<char> source_string)
		{
			Initialize(source_string, LexicalStates.INITIAL);
		}
		
		public void Initialize(ReadOnlyMemory<char> source_string, LexicalStates lexicalState, bool atBol)
		{
			this.expanding_token = false;
			this.token_start = 0;
			this.lookahead_index = 0;
			this.token_chunk_start = 0;
			this.token_end = 0;
			this.source_string = source_string;
			this.yy_at_bol = atBol;
			this.current_lexical_state = lexicalState;
		}
		
		public void Initialize(ReadOnlyMemory<char> source_string, LexicalStates lexicalState)
		{
			Initialize(source_string, lexicalState, false);
		}
		
		#region Accept
		
		#pragma warning disable 162
		
		
		Tokens Accept0(int state,out bool accepted)
		{
			accepted = true;
			
			switch(state)
			{
				case 1:
					// #line 809
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
					// #line 796
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
					// #line 757
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
					// #line 776
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						/* Allow <?php followed by end of file. */
						if (this.lookahead_index == this.Source.Length) // EOF
						{
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);		
						}
						/* Degenerate case: <?phpX is interpreted as <? phpX with short tags. */
						if (EnableShortTags) {
							_yyless(3);
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						}
						yymore(); break;
					}
					break;
					
				case 7:
					// #line 767
					{
						if(ProcessPreOpenTag())
						{
							return Tokens.T_INLINE_HTML; 
						}
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 8:
					// #line 1010
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 9:
					// #line 80
					{
						return Tokens.EOF;
					}
					break;
					
				case 10:
					// #line 657
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 11:
					// #line 830
					{
						return ProcessLabel();
					}
					break;
					
				case 12:
					// #line 834
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 13:
					// #line 306
					{
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 14:
					// #line 701
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 15:
					// #line 333
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 16:
					// #line 653
					{
						return (Tokens.T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG);
					}
					break;
					
				case 17:
					// #line 662
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 18:
					// #line 670
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 19:
					// #line 976
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); _binary_string_prefix = GetTokenChar(0) == 'b'; yymore(); break; }
					break;
					
				case 20:
					// #line 925
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); _binary_string_prefix = GetTokenChar(0) == 'b'; yymore(); break; }
					break;
					
				case 21:
					// #line 986
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 22:
					// #line 341
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 23:
					// #line 850
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 24:
					// #line 167
					{
						return Identifier(Tokens.T_IF);
					}
					break;
					
				case 25:
					// #line 191
					{
						return Identifier(Tokens.T_DO);
					}
					break;
					
				case 26:
					// #line 123
					{
						return Identifier(Tokens.T_FN);
					}
					break;
					
				case 27:
					// #line 628
					{
						return Identifier(Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 28:
					// #line 139
					{
						return Tokens.T_ATTRIBUTE;
					}
					break;
					
				case 29:
					// #line 223
					{
						return Identifier(Tokens.T_AS);
					}
					break;
					
				case 30:
					// #line 528
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 31:
					// #line 296
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 32:
					// #line 564
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 33:
					// #line 644
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 34:
					// #line 556
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 35:
					// #line 717
					{
						return ProcessRealNumber();
					}
					break;
					
				case 36:
					// #line 329
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 37:
					// #line 584
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 38:
					// #line 840
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 39:
					// #line 580
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 40:
					// #line 572
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 41:
					// #line 568
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 42:
					// #line 508
					{
						return Tokens.T_DOUBLE_ARROW;
					}
					break;
					
				case 43:
					// #line 540
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 44:
					// #line 560
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 45:
					// #line 524
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 46:
					// #line 544
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 47:
					// #line 552
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 48:
					// #line 640
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 49:
					// #line 588
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 50:
					// #line 600
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 51:
					// #line 624
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 52:
					// #line 648
					{
						yyless(1);
						return (Tokens.T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG);
					}
					break;
					
				case 53:
					// #line 616
					{
						return (Tokens.T_PIPE_OPERATOR);
					}
					break;
					
				case 54:
					// #line 604
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 55:
					// #line 620
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 56:
					// #line 608
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 57:
					// #line 855
					{
						return ProcessVariable();
					}
					break;
					
				case 58:
					// #line 612
					{
						return (Tokens.T_COALESCE_EQUAL);
					}
					break;
					
				case 59:
					// #line 301
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 60:
					// #line 636
					{
						return Identifier(Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 61:
					// #line 151
					{
						return Identifier(Tokens.T_TRY);
					}
					break;
					
				case 62:
					// #line 119
					{
						return Identifier(Tokens.T_EXIT);
					}
					break;
					
				case 63:
					// #line 195
					{
						return Identifier(Tokens.T_FOR);
					}
					break;
					
				case 64:
					// #line 345
					{
						return Identifier(Tokens.T_NEW);
					}
					break;
					
				case 65:
					// #line 413
					{
						return Identifier(Tokens.T_USE);
					}
					break;
					
				case 66:
					// #line 632
					{
						return Identifier(Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 67:
					// #line 596
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 68:
					// #line 337
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 69:
					// #line 353
					{
						return Identifier(Tokens.T_VAR);
					}
					break;
					
				case 70:
					// #line 576
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 71:
					// #line 532
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 72:
					// #line 536
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 73:
					// #line 548
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 74:
					// #line 592
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 75:
					// #line 705
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 76:
					// #line 697
					{
						return ProcessOctalNumber();
					}
					break;
					
				case 77:
					// #line 693
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 78:
					// #line 115
					{ 
						return Identifier(Tokens.T_EXIT);
					}
					break;
					
				case 79:
					// #line 259
					{
						return Identifier(Tokens.T_ECHO);
					}
					break;
					
				case 80:
					// #line 179
					{
						return Identifier(Tokens.T_ELSE);
					}
					break;
					
				case 81:
					// #line 389
					{
						return Identifier(Tokens.T_EVAL);
					}
					break;
					
				case 82:
					// #line 239
					{
						return Identifier(Tokens.T_CASE);
					}
					break;
					
				case 83:
					// #line 512
					{
						return Identifier(Tokens.T_LIST);
					}
					break;
					
				case 84:
					// #line 255
					{
						return Identifier(Tokens.T_GOTO);
					}
					break;
					
				case 85:
					// #line 845
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 86:
					// #line 175
					{
						return Identifier(Tokens.T_ENDIF);
					}
					break;
					
				case 87:
					// #line 429
					{
						return Identifier(Tokens.T_EMPTY);
					}
					break;
					
				case 88:
					// #line 425
					{
						return Identifier(Tokens.T_ISSET);
					}
					break;
					
				case 89:
					// #line 284
					{
						return Identifier(Tokens.T_TRAIT);
					}
					break;
					
				case 90:
					// #line 163
					{
						return Identifier(Tokens.T_THROW);
					}
					break;
					
				case 91:
					// #line 473
					{
						return Identifier(Tokens.T_FINAL);
					}
					break;
					
				case 92:
					// #line 504
					{
						return Identifier(Tokens.T_UNSET);
					}
					break;
					
				case 93:
					// #line 131
					{
						return Identifier(Tokens.T_CONST);
					}
					break;
					
				case 94:
					// #line 349
					{
						return Identifier(Tokens.T_CLONE);
					}
					break;
					
				case 95:
					// #line 267
					{
						return Identifier(Tokens.T_CLASS);
					}
					break;
					
				case 96:
					// #line 155
					{
						return Identifier(Tokens.T_CATCH);
					}
					break;
					
				case 97:
					// #line 147
					{
						return Identifier(Tokens.T_YIELD);
					}
					break;
					
				case 98:
					// #line 231
					{
						return Identifier(Tokens.T_MATCH);
					}
					break;
					
				case 99:
					// #line 516
					{
						return Identifier(Tokens.T_ARRAY);
					}
					break;
					
				case 100:
					// #line 183
					{
						return Identifier(Tokens.T_WHILE);
					}
					break;
					
				case 101:
					// #line 247
					{
						return Identifier(Tokens.T_BREAK);
					}
					break;
					
				case 102:
					// #line 263
					{
						return Identifier(Tokens.T_PRINT);
					}
					break;
					
				case 103:
					// #line 357
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 104:
					// #line 859
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
					
				case 105:
					// #line 199
					{
						return Identifier(Tokens.T_ENDFOR);
					}
					break;
					
				case 106:
					// #line 279
					{
						yyless(4); // consume 4 characters
						return Identifier(Tokens.T_ENUM);
					}
					break;
					
				case 107:
					// #line 171
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 108:
					// #line 465
					{
						return Identifier(Tokens.T_STATIC);
					}
					break;
					
				case 109:
					// #line 227
					{
						return Identifier(Tokens.T_SWITCH);
					}
					break;
					
				case 110:
					// #line 135
					{
						return Identifier(Tokens.T_RETURN);
					}
					break;
					
				case 111:
					// #line 421
					{
						return Identifier(Tokens.T_GLOBAL);
					}
					break;
					
				case 112:
					// #line 497
					{
						return Identifier(Tokens.T_PUBLIC);
					}
					break;
					
				case 113:
					// #line 361
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 114:
					// #line 377
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 115:
					// #line 385
					{
						return (Tokens.T_VOID_CAST);
					}
					break;
					
				case 116:
					// #line 288
					{
						return Identifier(Tokens.T_EXTENDS);
					}
					break;
					
				case 117:
					// #line 393
					{
						return Identifier(Tokens.T_INCLUDE);
					}
					break;
					
				case 118:
					// #line 243
					{
						return Identifier(Tokens.T_DEFAULT);
					}
					break;
					
				case 119:
					// #line 211
					{
						return Identifier(Tokens.T_DECLARE);
					}
					break;
					
				case 120:
					// #line 159
					{
						return Identifier(Tokens.T_FINALLY);
					}
					break;
					
				case 121:
					// #line 203
					{
						return Identifier(Tokens.T_FOREACH);
					}
					break;
					
				case 122:
					// #line 401
					{
						return Identifier(Tokens.T_REQUIRE);
					}
					break;
					
				case 123:
					// #line 477
					{
						return Identifier(Tokens.T_PRIVATE);
					}
					break;
					
				case 124:
					// #line 381
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 125:
					// #line 369
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 126:
					// #line 749
					{
						return Identifier(Tokens.T_DIR);
					}
					break;
					
				case 127:
					// #line 187
					{
						return Identifier(Tokens.T_ENDWHILE);
					}
					break;
					
				case 128:
					// #line 127
					{
						return Identifier(Tokens.T_FUNCTION);
					}
					break;
					
				case 129:
					// #line 251
					{
						return Identifier(Tokens.T_CONTINUE);
					}
					break;
					
				case 130:
					// #line 520
					{
						return Identifier(Tokens.T_CALLABLE);
					}
					break;
					
				case 131:
					// #line 500
					{
						return Identifier(Tokens.T_READONLY);
					}
					break;
					
				case 132:
					// #line 469
					{
						return Identifier(Tokens.T_ABSTRACT);
					}
					break;
					
				case 133:
					// #line 373
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 134:
					// #line 365
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 135:
					// #line 745
					{
						return Identifier(Tokens.T_FILE);
					}
					break;
					
				case 136:
					// #line 741
					{
						return Identifier(Tokens.T_LINE);
					}
					break;
					
				case 137:
					// #line 235
					{
						return Identifier(Tokens.T_ENDSWITCH);
					}
					break;
					
				case 138:
					// #line 271
					{
						return Identifier(Tokens.T_INTERFACE);
					}
					break;
					
				case 139:
					// #line 417
					{
						return Identifier(Tokens.T_INSTEADOF);
					}
					break;
					
				case 140:
					// #line 409
					{
						return Identifier(Tokens.T_NAMESPACE);
					}
					break;
					
				case 141:
					// #line 481
					{
						return Identifier(Tokens.T_PROTECTED);
					}
					break;
					
				case 142:
					// #line 725
					{
						return Identifier(Tokens.T_TRAIT_C);
					}
					break;
					
				case 143:
					// #line 721
					{
						return Identifier(Tokens.T_CLASS_C);
					}
					break;
					
				case 144:
					// #line 215
					{
						return Identifier(Tokens.T_ENDDECLARE);
					}
					break;
					
				case 145:
					// #line 207
					{
						return Identifier(Tokens.T_ENDFOREACH);
					}
					break;
					
				case 146:
					// #line 219
					{
						return Identifier(Tokens.T_INSTANCEOF);
					}
					break;
					
				case 147:
					// #line 292
					{
						return Identifier(Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 148:
					// #line 143
					{
						return Identifier(Tokens.T_YIELD_FROM);
					}
					break;
					
				case 149:
					// #line 737
					{
						return Identifier(Tokens.T_METHOD_C);
					}
					break;
					
				case 150:
					// #line 485
					{
						return Identifier(Tokens.T_PUBLIC_SET);
					}
					break;
					
				case 151:
					// #line 275
					{
						yyless(4); // consume 4 characters
						return ProcessLabel();
					}
					break;
					
				case 152:
					// #line 397
					{
						return Identifier(Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 153:
					// #line 405
					{
						return Identifier(Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 154:
					// #line 493
					{
						return Identifier(Tokens.T_PRIVATE_SET);
					}
					break;
					
				case 155:
					// #line 729
					{
						return Identifier(Tokens.T_FUNC_C);
					}
					break;
					
				case 156:
					// #line 733
					{
						return Identifier(Tokens.T_PROPERTY_C);
					}
					break;
					
				case 157:
					// #line 753
					{
						return Identifier(Tokens.T_NS_C);
					}
					break;
					
				case 158:
					// #line 489
					{
						return Identifier(Tokens.T_PROTECTED_SET);
					}
					break;
					
				case 159:
					// #line 448
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return Identifier(Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 160:
					// #line 984
					{ yymore(); break; }
					break;
					
				case 161:
					// #line 86
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 162:
					// #line 982
					{ yymore(); break; }
					break;
					
				case 163:
					// #line 980
					{ if (ProcessString(1, out Tokens token)) return token; else break; }
					break;
					
				case 164:
					// #line 983
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 978
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 166:
					// #line 977
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 167:
					// #line 979
					{ if (ProcessString(2, out Tokens token)) return token; else break; }
					break;
					
				case 168:
					// #line 928
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 83
					{
						return ProcessEof(Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 170:
					// #line 927
					{ yymore(); break; }
					break;
					
				case 171:
					// #line 929
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 172:
					// #line 926
					{ yymore(); break; }
					break;
					
				case 173:
					// #line 994
					{ yymore(); break; }
					break;
					
				case 174:
					// #line 992
					{ yymore(); break; }
					break;
					
				case 175:
					// #line 990
					{ if (ProcessShell(1, out Tokens token)) return token; else break; }
					break;
					
				case 176:
					// #line 993
					{ yymore(); break; }
					break;
					
				case 177:
					// #line 988
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 178:
					// #line 987
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 179:
					// #line 989
					{ if (ProcessShell(2, out Tokens token)) return token; else break; }
					break;
					
				case 180:
					// #line 1003
					{ yymore(); break; }
					break;
					
				case 181:
					// #line 1002
					{ yymore(); break; }
					break;
					
				case 182:
					// #line 1000
					{ yymore(); break; }
					break;
					
				case 183:
					// #line 914
					{
					    if(VerifyEndLabel(TokenTextSpan))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(_processDoubleQuotedString) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 184:
					// #line 1001
					{ yymore(); break; }
					break;
					
				case 185:
					// #line 997
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 186:
					// #line 996
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 187:
					// #line 998
					{ if (ProcessHeredoc(2, out Tokens token)) return token; else break; }
					break;
					
				case 188:
					// #line 323
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 189:
					// #line 318
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 190:
					// #line 310
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 191:
					// #line 314
					{
						return (Tokens.T_NULLSAFE_OBJECT_OPERATOR);
					}
					break;
					
				case 192:
					// #line 686
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 193:
					// #line 678
					{
						_yyless(1);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return WithTokenString(Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 194:
					// #line 846
					{ yymore(); break; }
					break;
					
				case 195:
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
					
				case 196:
					// #line 848
					{ yymore(); break; }
					break;
					
				case 197:
					// #line 847
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 198:
					// #line 841
					{ yymore(); break; }
					break;
					
				case 199:
					// #line 98
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 200:
					// #line 843
					{ yymore(); break; }
					break;
					
				case 201:
					// #line 842
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 202:
					// #line 1008
					{ yymore(); break; }
					break;
					
				case 203:
					// #line 102
					{ 
						return ProcessEof(Tokens.T_COMMENT);
					}
					break;
					
				case 204:
					// #line 1007
					{ yymore(); break; }
					break;
					
				case 205:
					// #line 1005
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 206:
					// #line 1006
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 207:
					// #line 818
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 208:
					// #line 709
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 209:
					// #line 813
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 210:
					// #line 713
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 211:
					// #line 901
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 212:
					// #line 890
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						if (TokenTextSpan.TrimStart().Equals(_hereDocValue.Label.AsSpan(), StringComparison.Ordinal) == false)
						{
							_errors.Error(_tokenPosition, Devsense.PHP.Errors.FatalErrors.SyntaxError, $"Incorrect heredoc end label: {TokenTextSpan.TrimStart().ToString()}");
						}
						_yyless(LabelTrailLength());
						_tokenSemantics.Object = _hereDocValue ?? throw new InvalidOperationException("Expected '_hereDocValue' to be set.");
						return Tokens.T_END_HEREDOC;
					}
					break;
					
				case 213:
					// #line 923
					{ yymore(); break; }
					break;
					
				case 214:
					// #line 905
					{
					    if(VerifyEndLabel(TokenTextSpan))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(null) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 215:
					// #line 460
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 216:
					// #line 457
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 217:
					// #line 454
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 218:
					// #line 433
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 219:
					// #line 458
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 220:
					// #line 456
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 221:
					// #line 438
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 222:
					// #line 443
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 223:
					// #line 970
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 224:
					// #line 959
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 225:
					// #line 948
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 226:
					// #line 931
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 227:
					// #line 953
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 228:
					// #line 942
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 229:
					// #line 936
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 230:
					// #line 964
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 232: goto case 1;
				case 234: goto case 7;
				case 235: goto case 10;
				case 236: goto case 11;
				case 237: goto case 12;
				case 238: goto case 14;
				case 239: goto case 23;
				case 240: goto case 35;
				case 241: goto case 46;
				case 242: goto case 104;
				case 243: goto case 106;
				case 244: goto case 162;
				case 245: goto case 170;
				case 246: goto case 174;
				case 247: goto case 181;
				case 248: goto case 182;
				case 249: goto case 188;
				case 250: goto case 192;
				case 251: goto case 202;
				case 252: goto case 205;
				case 253: goto case 207;
				case 254: goto case 208;
				case 255: goto case 210;
				case 256: goto case 215;
				case 257: goto case 223;
				case 260: goto case 10;
				case 261: goto case 11;
				case 262: goto case 23;
				case 263: goto case 35;
				case 264: goto case 106;
				case 265: goto case 188;
				case 266: goto case 210;
				case 267: goto case 223;
				case 269: goto case 10;
				case 270: goto case 11;
				case 271: goto case 210;
				case 273: goto case 10;
				case 274: goto case 11;
				case 276: goto case 10;
				case 277: goto case 11;
				case 279: goto case 10;
				case 280: goto case 11;
				case 282: goto case 10;
				case 283: goto case 11;
				case 285: goto case 10;
				case 286: goto case 11;
				case 288: goto case 10;
				case 289: goto case 11;
				case 291: goto case 10;
				case 292: goto case 11;
				case 294: goto case 10;
				case 295: goto case 11;
				case 297: goto case 10;
				case 298: goto case 11;
				case 300: goto case 10;
				case 301: goto case 11;
				case 303: goto case 10;
				case 304: goto case 11;
				case 306: goto case 10;
				case 307: goto case 11;
				case 309: goto case 10;
				case 310: goto case 11;
				case 312: goto case 11;
				case 314: goto case 11;
				case 316: goto case 11;
				case 318: goto case 11;
				case 320: goto case 11;
				case 322: goto case 11;
				case 324: goto case 11;
				case 326: goto case 11;
				case 328: goto case 11;
				case 330: goto case 11;
				case 332: goto case 11;
				case 334: goto case 11;
				case 336: goto case 11;
				case 338: goto case 11;
				case 340: goto case 11;
				case 342: goto case 11;
				case 344: goto case 11;
				case 346: goto case 11;
				case 348: goto case 11;
				case 350: goto case 11;
				case 352: goto case 11;
				case 354: goto case 11;
				case 356: goto case 11;
				case 358: goto case 11;
				case 360: goto case 11;
				case 362: goto case 11;
				case 364: goto case 11;
				case 366: goto case 11;
				case 368: goto case 11;
				case 370: goto case 11;
				case 372: goto case 11;
				case 374: goto case 11;
				case 376: goto case 11;
				case 378: goto case 11;
				case 380: goto case 11;
				case 382: goto case 11;
				case 384: goto case 11;
				case 386: goto case 11;
				case 388: goto case 11;
				case 390: goto case 11;
				case 392: goto case 11;
				case 394: goto case 11;
				case 396: goto case 11;
				case 398: goto case 11;
				case 400: goto case 11;
				case 402: goto case 11;
				case 404: goto case 11;
				case 406: goto case 11;
				case 408: goto case 11;
				case 410: goto case 11;
				case 412: goto case 11;
				case 414: goto case 11;
				case 416: goto case 11;
				case 418: goto case 11;
				case 420: goto case 11;
				case 422: goto case 11;
				case 424: goto case 11;
				case 426: goto case 11;
				case 428: goto case 11;
				case 430: goto case 11;
				case 432: goto case 11;
				case 434: goto case 11;
				case 436: goto case 11;
				case 438: goto case 11;
				case 440: goto case 11;
				case 484: goto case 11;
				case 501: goto case 11;
				case 504: goto case 11;
				case 506: goto case 11;
				case 508: goto case 11;
				case 510: goto case 11;
				case 511: goto case 11;
				case 512: goto case 11;
				case 513: goto case 11;
				case 514: goto case 11;
				case 515: goto case 11;
				case 516: goto case 11;
				case 517: goto case 11;
				case 518: goto case 11;
				case 519: goto case 11;
				case 520: goto case 11;
				case 521: goto case 11;
				case 522: goto case 11;
				case 523: goto case 11;
				case 524: goto case 11;
				case 525: goto case 11;
				case 526: goto case 11;
				case 527: goto case 11;
				case 528: goto case 11;
				case 529: goto case 11;
				case 530: goto case 11;
				case 531: goto case 11;
				case 532: goto case 11;
				case 533: goto case 11;
				case 534: goto case 11;
				case 535: goto case 11;
				case 536: goto case 11;
				case 537: goto case 11;
				case 538: goto case 11;
				case 539: goto case 11;
				case 540: goto case 11;
				case 541: goto case 11;
				case 542: goto case 11;
				case 543: goto case 11;
				case 544: goto case 11;
				case 545: goto case 11;
				case 546: goto case 11;
				case 547: goto case 11;
				case 548: goto case 11;
				case 549: goto case 11;
				case 550: goto case 11;
				case 551: goto case 11;
				case 552: goto case 11;
				case 553: goto case 11;
				case 554: goto case 11;
				case 555: goto case 11;
				case 556: goto case 11;
				case 557: goto case 11;
				case 558: goto case 11;
				case 559: goto case 11;
				case 560: goto case 11;
				case 561: goto case 11;
				case 562: goto case 11;
				case 563: goto case 11;
				case 564: goto case 11;
				case 565: goto case 11;
				case 566: goto case 11;
				case 567: goto case 11;
				case 568: goto case 11;
				case 569: goto case 11;
				case 570: goto case 11;
				case 571: goto case 11;
				case 572: goto case 11;
				case 573: goto case 11;
				case 574: goto case 11;
				case 575: goto case 11;
				case 576: goto case 11;
				case 577: goto case 11;
				case 578: goto case 11;
				case 579: goto case 11;
				case 580: goto case 11;
				case 581: goto case 11;
				case 582: goto case 11;
				case 583: goto case 11;
				case 584: goto case 11;
				case 585: goto case 11;
				case 586: goto case 11;
				case 587: goto case 11;
				case 588: goto case 11;
				case 589: goto case 11;
				case 590: goto case 11;
				case 591: goto case 11;
				case 592: goto case 11;
				case 593: goto case 11;
				case 594: goto case 11;
				case 595: goto case 11;
				case 596: goto case 11;
				case 597: goto case 11;
				case 598: goto case 11;
				case 599: goto case 11;
				case 600: goto case 11;
				case 601: goto case 11;
				case 602: goto case 11;
				case 603: goto case 11;
				case 604: goto case 11;
				case 605: goto case 11;
				case 606: goto case 11;
				case 607: goto case 11;
				case 608: goto case 11;
				case 609: goto case 11;
				case 610: goto case 11;
				case 611: goto case 11;
				case 612: goto case 11;
				case 613: goto case 11;
				case 614: goto case 11;
				case 615: goto case 11;
				case 616: goto case 11;
				case 617: goto case 11;
				case 618: goto case 11;
				case 619: goto case 11;
				case 620: goto case 11;
				case 621: goto case 11;
				case 622: goto case 11;
				case 623: goto case 11;
				case 624: goto case 11;
				case 625: goto case 11;
				case 626: goto case 11;
				case 627: goto case 11;
				case 628: goto case 11;
				case 629: goto case 11;
				case 630: goto case 11;
				case 631: goto case 11;
				case 632: goto case 11;
				case 633: goto case 11;
				case 634: goto case 11;
				case 635: goto case 11;
				case 636: goto case 11;
				case 637: goto case 11;
				case 638: goto case 11;
				case 639: goto case 11;
				case 640: goto case 11;
				case 641: goto case 11;
				case 642: goto case 11;
				case 643: goto case 11;
				case 644: goto case 11;
				case 645: goto case 11;
				case 646: goto case 11;
				case 647: goto case 11;
				case 648: goto case 11;
				case 649: goto case 11;
				case 650: goto case 11;
				case 651: goto case 11;
				case 652: goto case 11;
				case 653: goto case 11;
				case 654: goto case 11;
				case 655: goto case 11;
				case 656: goto case 11;
				case 657: goto case 11;
				case 658: goto case 11;
				case 659: goto case 11;
				case 660: goto case 11;
				case 661: goto case 11;
				case 662: goto case 11;
				case 663: goto case 11;
				case 664: goto case 11;
				case 665: goto case 11;
				case 666: goto case 11;
				case 667: goto case 11;
				case 668: goto case 11;
				case 669: goto case 11;
				case 670: goto case 11;
				case 671: goto case 11;
				case 672: goto case 11;
				case 673: goto case 11;
				case 674: goto case 11;
				case 675: goto case 11;
				case 676: goto case 11;
				case 677: goto case 11;
				case 678: goto case 11;
				case 679: goto case 11;
				case 680: goto case 11;
				case 681: goto case 11;
				case 682: goto case 11;
				case 683: goto case 11;
				case 684: goto case 11;
				case 685: goto case 11;
				case 686: goto case 11;
				case 687: goto case 11;
				case 688: goto case 11;
				case 689: goto case 11;
				case 690: goto case 11;
				case 691: goto case 11;
				case 692: goto case 11;
				case 693: goto case 11;
				case 694: goto case 11;
				case 695: goto case 11;
				case 696: goto case 11;
				case 697: goto case 11;
				case 698: goto case 11;
				case 699: goto case 11;
				case 700: goto case 11;
				case 701: goto case 11;
				case 702: goto case 11;
				case 703: goto case 11;
				case 704: goto case 11;
				case 705: goto case 11;
				case 706: goto case 11;
				case 707: goto case 11;
				case 708: goto case 11;
				case 709: goto case 11;
				case 710: goto case 11;
				case 711: goto case 11;
				case 712: goto case 11;
				case 713: goto case 11;
				case 714: goto case 11;
				case 715: goto case 11;
				case 716: goto case 11;
				case 717: goto case 11;
				case 718: goto case 11;
				case 719: goto case 11;
				case 720: goto case 11;
				case 721: goto case 11;
				case 722: goto case 11;
				case 723: goto case 11;
				case 724: goto case 11;
				case 725: goto case 11;
				case 726: goto case 11;
				case 727: goto case 11;
				case 728: goto case 11;
				case 729: goto case 11;
				case 730: goto case 11;
				case 731: goto case 11;
				case 732: goto case 11;
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
			if (lookahead_index >= source_string.Length) return EOF;
			
			return Map(buffer[lookahead_index++]);
		}
		
		private char[] ResizeBuffer(char[] buf)
		{
			char[] result = new char[buf.Length << 1];
			System.Buffer.BlockCopy(buf, 0, result, 0, buf.Length << 1);
			return result;
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
		
		private static readonly AcceptConditions[] acceptCondition = new AcceptConditions[]
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
			AcceptConditions.Accept, // 180
			AcceptConditions.Accept, // 181
			AcceptConditions.Accept, // 182
			AcceptConditions.AcceptOnStart, // 183
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
			AcceptConditions.Accept, // 211
			AcceptConditions.Accept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.AcceptOnStart, // 214
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
			AcceptConditions.NotAccept, // 231
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
			AcceptConditions.Accept, // 255
			AcceptConditions.Accept, // 256
			AcceptConditions.Accept, // 257
			AcceptConditions.NotAccept, // 258
			AcceptConditions.Accept, // 259
			AcceptConditions.Accept, // 260
			AcceptConditions.Accept, // 261
			AcceptConditions.Accept, // 262
			AcceptConditions.Accept, // 263
			AcceptConditions.Accept, // 264
			AcceptConditions.Accept, // 265
			AcceptConditions.Accept, // 266
			AcceptConditions.Accept, // 267
			AcceptConditions.NotAccept, // 268
			AcceptConditions.Accept, // 269
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
			AcceptConditions.Accept, // 432
			AcceptConditions.NotAccept, // 433
			AcceptConditions.Accept, // 434
			AcceptConditions.NotAccept, // 435
			AcceptConditions.Accept, // 436
			AcceptConditions.NotAccept, // 437
			AcceptConditions.Accept, // 438
			AcceptConditions.NotAccept, // 439
			AcceptConditions.Accept, // 440
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
			AcceptConditions.NotAccept, // 480
			AcceptConditions.NotAccept, // 481
			AcceptConditions.NotAccept, // 482
			AcceptConditions.NotAccept, // 483
			AcceptConditions.Accept, // 484
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
			AcceptConditions.NotAccept, // 495
			AcceptConditions.NotAccept, // 496
			AcceptConditions.NotAccept, // 497
			AcceptConditions.NotAccept, // 498
			AcceptConditions.NotAccept, // 499
			AcceptConditions.NotAccept, // 500
			AcceptConditions.Accept, // 501
			AcceptConditions.NotAccept, // 502
			AcceptConditions.NotAccept, // 503
			AcceptConditions.Accept, // 504
			AcceptConditions.NotAccept, // 505
			AcceptConditions.Accept, // 506
			AcceptConditions.NotAccept, // 507
			AcceptConditions.Accept, // 508
			AcceptConditions.NotAccept, // 509
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
			AcceptConditions.Accept, // 727
			AcceptConditions.Accept, // 728
			AcceptConditions.Accept, // 729
			AcceptConditions.Accept, // 730
			AcceptConditions.Accept, // 731
			AcceptConditions.Accept, // 732
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
			0, 1, 1, 1, 2, 1, 3, 1, 1, 1, 4, 5, 6, 7, 8, 1, 
			9, 1, 1, 1, 1, 1, 10, 11, 12, 12, 12, 12, 1, 12, 1, 1, 
			1, 13, 1, 14, 1, 1, 15, 1, 16, 1, 1, 17, 1, 1, 18, 19, 
			20, 1, 1, 1, 1, 1, 1, 1, 1, 21, 1, 1, 12, 12, 12, 22, 
			12, 12, 12, 1, 1, 12, 1, 1, 1, 1, 1, 23, 24, 25, 12, 12, 
			26, 12, 12, 12, 12, 27, 12, 12, 12, 12, 12, 28, 12, 12, 12, 12, 
			12, 29, 12, 12, 12, 12, 12, 1, 1, 30, 1, 12, 12, 12, 12, 12, 
			31, 1, 1, 1, 12, 32, 12, 12, 12, 12, 33, 34, 1, 1, 12, 12, 
			12, 12, 12, 12, 12, 1, 1, 12, 12, 12, 12, 12, 12, 35, 12, 12, 
			12, 12, 12, 12, 1, 12, 1, 1, 12, 12, 1, 12, 12, 12, 1, 12, 
			36, 1, 37, 1, 1, 1, 1, 1, 38, 1, 38, 1, 1, 39, 40, 1, 
			1, 1, 1, 1, 41, 1, 42, 43, 1, 1, 1, 1, 1, 44, 1, 1, 
			1, 1, 45, 1, 46, 1, 47, 1, 48, 1, 49, 1, 50, 1, 1, 1, 
			51, 1, 52, 1, 53, 1, 54, 1, 1, 55, 1, 56, 57, 1, 1, 1, 
			1, 58, 1, 1, 1, 1, 1, 59, 60, 61, 62, 1, 63, 1, 64, 1, 
			65, 1, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 
			80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 
			96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 
			112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 
			128, 129, 130, 131, 132, 77, 133, 103, 134, 135, 136, 137, 138, 139, 140, 141, 
			142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 
			158, 159, 160, 161, 162, 27, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 
			173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 
			189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 
			205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 
			221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 
			237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 
			253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 
			269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 61, 280, 281, 282, 283, 
			74, 284, 285, 286, 287, 288, 289, 290, 291, 292, 83, 293, 57, 294, 295, 296, 
			297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 
			313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 
			329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 
			345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 
			361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 
			377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 
			393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 
			409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 
			425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 
			441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 
			457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 
			473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 
			489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 
			505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 
			521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 
			537, 538, 12, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548
		};
		
		private static readonly int[,] nextState = new int[,]
		{
			{ 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 232, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 231, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 234, -1, -1, -1, -1 },
			{ -1, -1, 22, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 272, 23, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 510, 722, 722, 722, 722, 511, 722, 512, 722, 722, 722, -1, -1, 722, 513, -1, 514, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 515, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, -1, -1 },
			{ -1, -1, -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, -1, -1, 35, -1, -1, -1, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 14, 14, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, 51, -1, -1, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 58, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 239, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 262, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, 240, 240, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 72, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 73, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 74, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, -1, -1, 57, 57, -1, 57, 57, 57, 57, 57, 57, 57, 57, -1, -1, -1, 57, 57, -1, -1, -1, 57, -1, -1, -1, 57, 57, 57, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 57, 57, 57, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 708, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 75, -1, -1, -1, 75, 75, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, 75, 75, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, 76, 76, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 77, 77, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 360, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 85, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 85, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 85, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 380, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, 389, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, 389, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, 389, -1, -1, -1, -1 },
			{ -1, -1, -1, 712, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, 413, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 628, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 729, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, 427, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, 443, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ 160, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1, 160, 160, 160, 160, 160, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1, 160, -1, 160, 160, 160, 160, 160, 160, -1, 160, 160 },
			{ -1, -1, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, -1, -1, 165, 165, -1, 165, 165, 165, 165, 165, 165, 165, 165, -1, -1, -1, 165, -1, -1, -1, -1, 165, -1, -1, -1, 165, 165, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 166, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168 },
			{ 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, -1, 173, 173, 173, 173, 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, -1, 173, -1, 173, 173, 173, 173, 173, 173, 173, 173, -1 },
			{ -1, -1, -1, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, -1, -1, 177, 177, -1, 177, 177, 177, 177, 177, 177, 177, 177, -1, -1, -1, 177, -1, -1, -1, -1, 177, -1, -1, -1, 177, 177, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 180, -1, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, -1, 180, 180, 180, 180, 180, 180, 180, 180, -1, 180, 180, 180, 180, 180, -1, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, -1, 180, -1, 180, 180, 180, 180, -1, 180, 180, 180, 180 },
			{ -1, -1, -1, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, 185, -1, -1, 185, 185, -1, 185, 185, 185, 185, 185, 185, 185, 185, -1, -1, -1, 185, -1, -1, -1, -1, 185, -1, -1, -1, 185, 185, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, -1, -1, 183, 183, -1, 183, 183, 183, 183, 183, 183, 183, 183, -1, -1, -1, 183, 183, -1, -1, -1, 183, -1, -1, -1, 183, 183, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, 183, 183, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, -1, -1, 189, 189, -1, 189, 189, 189, 189, 189, 189, 189, 189, -1, -1, -1, 189, 189, -1, -1, -1, 189, -1, -1, -1, 189, 189, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, 189, 189, -1, -1, -1, -1, -1 },
			{ 194, -1, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, -1, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, -1, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 198, -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 251, 203, 204, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 205, 251, 251, 251, 251, 251, 251, 251, 251, 3, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 252, 251, 251, 251, 251 },
			{ -1, 203, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 206, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1, -1, -1, -1, -1, 468, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, 208, 208, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1, -1, -1, 468, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, 210, 210, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, -1, -1, 212, 212, -1, 212, 212, 212, 212, 212, 212, 212, 212, -1, -1, -1, 212, 212, -1, -1, -1, 212, -1, -1, -1, 212, 212, 212, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 212, 212, 212, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, -1, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, -1, -1, -1, 214, 214, -1, -1, -1, 214, -1, -1, -1, 214, 214, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, 214, 214, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 476, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1 },
			{ -1, -1, -1, 225, 225, 225, 225, 225, 225, 225, 225, 225, 225, 225, 225, -1, 228, 225, 225, -1, 225, 225, 225, 225, 225, 225, 225, 225, -1, 480, -1, 225, 225, -1, -1, -1, 225, -1, -1, -1, 225, 225, 225, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 225, 225, 225, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 258, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, 183, -1, -1, 183, 183, -1, 183, 183, 183, 183, 183, 183, 183, 183, -1, -1, -1, 183, -1, -1, -1, -1, 183, -1, 459, -1, 183, 183, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 24, 516, 722, 722, 722, 517, 722, -1, -1, 722, 722, -1, 649, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 278, 317, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, 14, -1, -1, 35, -1, -1, -1, -1, -1, 281, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 14, 14, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, -1, -1, -1, -1, -1, -1, -1, -1, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, 240, 240, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 497, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 170, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 179, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 462, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 464, 464, 464, 464, 464, 464, 464, 464, 464, 464, 464, 464, -1, 193, 464, 464, -1, 464, 464, 464, 464, 464, 464, 464, 464, -1, -1, -1, 464, 464, -1, -1, -1, 464, -1, -1, -1, 464, 464, 464, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, 464, 464, 464, -1, -1, -1, -1, -1 },
			{ 251, -1, -1, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, -1, 251, 251, 251, 251, 251, 251, 251, 251, -1, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, 251, -1, 251, 251, 251, 251 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, -1, -1, 57, 57, -1, 57, 57, 57, 57, 57, 57, 57, 57, -1, -1, -1, 57, -1, -1, -1, -1, 57, -1, -1, -1, 57, 57, 57, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 469, -1, -1, -1, -1, -1, -1, -1, 470, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 471, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1, -1, -1, 468, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, 210, 210, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, -1, -1, -1, 255, 255, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, 255, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, 469, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 255, 255, 255, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 216, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 225, 225, 225, 225, 225, 225, 225, 225, 225, 225, 225, 225, -1, -1, 225, 225, -1, 225, 225, 225, 225, 225, 225, 225, 225, -1, -1, -1, 225, -1, -1, -1, -1, 225, -1, -1, -1, 225, 225, 225, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, 214, -1, -1, 214, 214, -1, 214, 214, 214, 214, 214, 214, 214, 214, -1, -1, -1, 214, -1, -1, -1, -1, 214, -1, 474, -1, 214, 214, 214, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 519, 722, 289, 722, 722, 722, 722, 722, 722, 25, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 239, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, 263, 263, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 411, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 190, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 470, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 266, 266, 266, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 9, 10, 11, 484, 236, 501, 261, 270, 504, 506, 648, 274, 680, 701, 12, 235, 713, 718, 13, 720, 277, 722, 723, 280, 722, 724, 725, 3, 260, 269, 722, 14, 273, 15, 276, 508, 279, 13, 235, 722, 726, 722, 235, 282, 285, 288, 291, 294, 297, 300, 16, 303, 306, 309, 235, 17, 18, 238, 14, 14, 13, 235, 19, 20, 21 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 520, 722, 722, 722, 26, 652, 722, 292, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 471, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 271, 271, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 27, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 485, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, -1, -1, 284, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, 240, 240, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 301, 722, 722, 722, 29, 651, -1, -1, 722, 722, -1, 722, 722, 722, 722, 681, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, 263, 263, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 287, -1, 290, 293, -1, 486, -1, 296, 299, 302, -1, -1, -1, -1, -1, -1, 305, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 487, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 655, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, 275, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, 19, 20, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 14, 14, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, 38, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 60, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 61, 722, -1, 722, 540, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 488, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 62, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 63, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 64, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 241, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 65, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 489, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 66, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 53, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 54, -1, -1, -1, -1, -1, 55, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 69, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 502, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 56, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 78, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, 490, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 79, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 80, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 81, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 82, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 75, -1, -1, -1, 75, 75, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, 75, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 75, 75, 75, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 83, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, 76, 76, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 84, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 77, 77, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 86, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, 263, 263, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, 365, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, 365, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, 365, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, 240, 240, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 87, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 88, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 491, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 89, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 90, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 493, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 91, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 92, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 93, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 94, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 95, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 361, 361, 361, 361, 361, 361, 361, 361, 361, 361, 361, 361, -1, -1, 361, 361, -1, 361, 361, 361, 361, 361, 361, 361, 361, -1, -1, -1, 361, -1, -1, -1, -1, 361, -1, 343, -1, 361, 361, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, 495, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 96, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 52, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 97, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 369, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 98, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 503, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 99, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 496, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 100, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 101, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 102, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 505, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 105, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 383, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 107, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 361, 361, 361, 361, 361, 361, 361, 361, 361, 361, 361, 361, -1, -1, 361, 361, 104, 361, 361, 361, 361, 361, 361, 361, 361, -1, -1, -1, 361, 361, -1, -1, -1, 361, -1, -1, -1, 361, 361, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, 361, 361, 242, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 108, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 385, 385, 385, 385, 385, 385, 385, 385, 385, 385, 385, 385, -1, -1, 385, 385, -1, 385, 385, 385, 385, 385, 385, 385, 385, -1, -1, -1, 385, -1, -1, -1, -1, 385, -1, -1, -1, 385, 385, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 109, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, 106, -1, 243, 106, 264, 106, 106, 106, 106, 106, 106, 106, 106, 106, -1, -1, 106, 106, 365, 106, 106, 106, 106, 106, 106, 106, 106, 106, -1, -1, 106, -1, -1, -1, -1, 106, -1, 365, -1, 106, 106, 106, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 110, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 391, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 111, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 369, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 112, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 116, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 397, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 117, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 399, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 118, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 377, 113, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 119, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 401, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 120, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 405, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 407, 114, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 121, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 383, 115, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 122, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 385, 385, 385, 385, 385, 385, 385, 385, 385, 385, 385, 385, -1, -1, 385, 385, -1, 385, 385, 385, 385, 385, 385, 385, 385, -1, -1, -1, 385, 385, -1, -1, -1, 385, -1, -1, -1, 385, 385, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, 385, 385, -1, -1, 409, -1, -1 },
			{ -1, -1, -1, 123, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 387, 387, 387, 387, 387, 387, 387, 387, 387, 387, 387, 387, -1, -1, 387, 387, -1, 387, 387, 387, 387, 387, 387, 387, 387, -1, -1, -1, 387, 387, -1, -1, -1, 387, -1, -1, -1, 387, 387, 387, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 387, 387, 387, -1, -1, -1, 409, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 126, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 507, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 389, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 389, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 389, -1, -1, -1, -1 },
			{ -1, -1, -1, 127, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 509, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 128, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 129, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 395, 124, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 130, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 131, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 132, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 401, 125, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 135, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 136, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 137, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 407, 114, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 138, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 242, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 139, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 423, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 140, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 498, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 141, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 415, 133, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 142, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, 134, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 143, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 407, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 144, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 429, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 145, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 431, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 146, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 433, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 147, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 499, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 149, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 437, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 152, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 439, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 153, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 148, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 155, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 441, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 156, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 444, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 157, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 445, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 159, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 150, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 446, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 500, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 151, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 449, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 450, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 444, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 158, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 160, 161, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 3, 160, 160, 160, 160, 160, 452, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 162, 160, 244, 160, 160, 160, 160, 160, 160, 163, 160, 160 },
			{ 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164 },
			{ 168, 169, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 170, 168, 168, 168, 168, 168, 168, 168, 168, 3, 168, 168, 168, 168, 168, 454, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 245, 168, 168, 171, 168 },
			{ 172, -1, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, -1, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172 },
			{ 173, 169, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 3, 173, 173, 173, 173, 173, 456, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 174, 173, 246, 173, 173, 173, 173, 173, 173, 173, 173, 175 },
			{ 176, -1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, -1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176 },
			{ 180, 169, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 181, 180, 180, 180, 180, 180, 180, 180, 180, 233, 180, 180, 180, 180, 180, 458, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 182, 180, 248, 180, 180, 180, 180, 247, 180, 180, 180, 180 },
			{ 184, -1, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, -1, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184, 184 },
			{ -1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 188, 9, 249, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 189, 188, 188, 189, 189, 13, 189, 189, 189, 189, 189, 189, 189, 189, 3, 265, 188, 189, 188, 188, 188, 188, 189, 188, 13, 188, 189, 189, 189, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 13, 188, 188, 188, 188 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 192, 9, 192, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 192, 192, 250, 250, 192, 250, 250, 250, 250, 250, 250, 250, 250, 3, 192, 192, 250, 192, 192, 192, 192, 250, 192, 192, 192, 250, 250, 250, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192 },
			{ 194, 195, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 3, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 196, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194, 194 },
			{ 198, 199, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 3, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 200, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198 },
			{ 8, 9, 207, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 8, 207, 722, 722, 8, 722, 722, 722, 722, 722, 722, 722, 722, 3, 207, 207, 722, 208, 207, 8, 207, 722, 207, 8, 207, 722, 722, 722, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 207, 253, 207, 207, 207, 254, 208, 208, 8, 209, 207, 8, 207 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, 210, 210, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, -1, -1, -1, 255, 255, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, 255, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 255, 255, 255, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 266, 266, 266, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 271, 271, -1, -1, -1, -1, -1, -1 },
			{ 211, 9, 211, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 212, 211, 211, 212, 212, 211, 212, 212, 212, 212, 212, 212, 212, 212, 3, 211, 211, 212, 211, 211, 211, 211, 212, 211, 211, 211, 212, 212, 212, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211, 211 },
			{ 213, 169, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 259, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213, 213 },
			{ 215, 9, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 216, 215, 215, 215, 217, 215, 215, 215, 215, 215, 215, 215, 215, 3, 215, 215, 215, 215, 215, 215, 215, 215, 218, 217, 215, 215, 215, 215, 215, 256, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 217, 215, 215, 215, 215 },
			{ 215, 9, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 216, 215, 215, 215, 217, 215, 215, 215, 215, 215, 215, 215, 215, 3, 215, 215, 215, 215, 215, 215, 215, 215, 215, 217, 221, 215, 215, 215, 215, 256, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 217, 215, 215, 215, 215 },
			{ 215, 9, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 216, 215, 215, 215, 217, 215, 215, 215, 215, 215, 215, 215, 215, 3, 215, 215, 215, 215, 215, 215, 215, 215, 215, 217, 215, 215, 215, 215, 222, 256, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 215, 217, 215, 215, 215, 215 },
			{ 223, 9, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 3, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 257, 223, 267, 223, 223, 223, 223, 223, 223, 224, 223, 223 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 481, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 229, -1, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, -1, -1, 229, 229, -1, 229, 229, 229, 229, 229, 229, 229, 229, 229, -1, -1, 229, -1, -1, -1, -1, 229, -1, -1, -1, 229, 229, 229, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 223, 3, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 3, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 257, 223, 267, 223, 223, 223, 223, 223, 223, 223, 223, 230 },
			{ 223, 3, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 3, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 223, 257, 223, 267, 223, 223, 223, 223, 223, 223, 223, 223, 223 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 283, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 492, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 494, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 381, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 387, 387, 387, 387, 387, 387, 387, 387, 387, 387, 387, 387, -1, -1, 387, 387, -1, 387, 387, 387, 387, 387, 387, 387, 387, -1, -1, -1, 387, -1, -1, -1, -1, 387, -1, -1, -1, 387, 387, 387, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 395, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 421, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 435, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 442, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 448, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 286, -1, -1, 722, 722, -1, 722, 722, 518, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 393, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 295, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 521, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 403, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 650, 722, 722, 722, 298, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 425, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 304, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 369, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 307, 683, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 535, 722, 722, 536, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 310, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 312, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 537, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 314, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 702, 722, 722, 722, 722, 538, 722, 656, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 539, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 541, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 653, 722, 722, 685, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 542, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 714, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 546, 722, 722, -1, -1, 722, 722, -1, 722, 547, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 548, 722, 722, 722, 722, 722, 722, 316, 722, -1, -1, 722, 731, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 684, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 703, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 549, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 550, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 659, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 551, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 318, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 552, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 320, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 657, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 704, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 557, 722, 722, 722, 722, 722, 722, 660, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 558, 559, 560, 561, 722, 715, 722, 722, 722, -1, -1, 722, 662, -1, 562, 722, 663, 722, 722, 722, 722, 661, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 322, 722, 664, 564, 722, 722, 722, 722, 565, 722, -1, -1, 722, 722, -1, 722, 722, 722, 566, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 324, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 326, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 686, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 328, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 330, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 332, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 334, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 687, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 336, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 689, 722, 722, 722, 722, 722, 722, 338, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 340, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 342, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 344, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 571, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 572, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 346, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 348, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 350, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 706, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 352, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 354, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 356, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 719, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 721, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 575, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 576, 722, 722, 722, 722, 665, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 577, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 579, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 580, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 358, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 582, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 670, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 668, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 585, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 589, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 362, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 364, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 366, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 695, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 368, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 370, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 593, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 594, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 673, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 595, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 596, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 372, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 598, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 675, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 600, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 374, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 602, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 376, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 378, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 382, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 676, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 605, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 384, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 386, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 388, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 608, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 697, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 610, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 611, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 699, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 390, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 614, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 615, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 677, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 392, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 394, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 396, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 398, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 400, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 402, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 404, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 623, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 624, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 625, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 406, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 408, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 410, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 630, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 412, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 414, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 416, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 732, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 631, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 418, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 632, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 678, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 633, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 420, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 422, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 634, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 424, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 426, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 635, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 428, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 637, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 638, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 641, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 642, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 643, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 430, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 432, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 434, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 644, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 645, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 436, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 438, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 646, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 647, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 440, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 682, 722, 722, -1, -1, 722, 522, -1, 722, 523, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 658, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 544, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 553, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 543, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 705, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 555, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 556, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 567, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 573, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 691, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 707, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 709, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 666, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 692, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 667, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 581, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 730, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 672, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 597, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 601, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 698, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 599, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 604, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 612, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 674, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 621, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 613, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 617, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 629, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 636, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 639, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 524, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 525, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 554, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 545, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 563, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 569, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 690, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 584, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 694, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 583, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 671, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 587, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 727, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 696, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 607, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 603, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 606, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 609, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 622, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 618, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 626, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 640, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 526, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 688, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 570, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 574, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 586, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 693, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 591, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 588, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 669, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 616, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 619, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 627, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 527, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 568, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 578, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 590, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 620, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 528, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 592, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 529, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 728, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 654, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 530, 722, 722, -1, -1, 722, 531, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 532, 722, 722, 722, 533, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 534, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 710, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 711, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 679, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 717, 722, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, 722, 716, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 722, 722, 722, 722, 722, 722, 722, 722, 722, 700, 722, 722, -1, -1, 722, 722, -1, 722, 722, 722, 722, 722, 722, 722, 722, -1, -1, -1, 722, 722, -1, -1, -1, 722, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 722, 722, 722, -1, -1, -1, -1, -1 }
		};
		
		
		private static readonly int[] yy_state_dtrans = new int[]
		{
			  0,
			  268,
			  451,
			  453,
			  455,
			  457,
			  460,
			  461,
			  463,
			  465,
			  466,
			  202,
			  467,
			  472,
			  473,
			  475,
			  477,
			  478,
			  479,
			  482,
			  483
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 733);
						}
						else
						{
							bool accepted = false;
							yyreturn = Accept0(last_accept_state, out accepted);
							if (accepted)
							{
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

