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
					// #line 78
					{
						return Tokens.EOF;
					}
					break;
					
				case 3:
					// #line 751
					{
					    this._tokenSemantics.Object = GetTokenString();
						return Tokens.T_INLINE_HTML; 
					}
					break;
					
				case 4:
					// #line 778
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 5:
					// #line 769
					{
						if (this._allowShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							yymore(); break;//return Tokens.T_INLINE_HTML;
						}
					}
					break;
					
				case 6:
					// #line 756
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 7:
					// #line 762
					{
						//HANDLE_NEWLINE(yytext[yyleng-1]);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 8:
					// #line 650
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 800
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 314
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 11:
					// #line 691
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 12:
					// #line 967
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 13:
					// #line 338
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 14:
					// #line 804
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 15:
					// #line 655
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 663
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 943
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
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
					// #line 346
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 820
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 164
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 194
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DO);
					}
					break;
					
				case 24:
					// #line 630
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 25:
					// #line 234
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_AS);
					}
					break;
					
				case 26:
					// #line 538
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 27:
					// #line 309
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 28:
					// #line 574
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 29:
					// #line 646
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 30:
					// #line 566
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 31:
					// #line 707
					{
						return ProcessRealNumber();
					}
					break;
					
				case 32:
					// #line 334
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 33:
					// #line 594
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 34:
					// #line 810
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 35:
					// #line 590
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 36:
					// #line 582
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 37:
					// #line 578
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 38:
					// #line 515
					{
						return (Tokens.T_DOUBLE_ARROW);
					}
					break;
					
				case 39:
					// #line 550
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 40:
					// #line 570
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 41:
					// #line 534
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 42:
					// #line 554
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 43:
					// #line 562
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 44:
					// #line 642
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 45:
					// #line 598
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 46:
					// #line 610
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 47:
					// #line 626
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 48:
					// #line 614
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 49:
					// #line 622
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 50:
					// #line 618
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 51:
					// #line 825
					{
						return ProcessVariable();
					}
					break;
					
				case 52:
					// #line 638
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 53:
					// #line 144
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRY);
					}
					break;
					
				case 54:
					// #line 114
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT);
					}
					break;
					
				case 55:
					// #line 199
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOR);
					}
					break;
					
				case 56:
					// #line 423
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_USE);
					}
					break;
					
				case 57:
					// #line 350
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NEW);
					}
					break;
					
				case 58:
					// #line 634
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 59:
					// #line 606
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 60:
					// #line 342
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 61:
					// #line 360
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_VAR);
					}
					break;
					
				case 62:
					// #line 586
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 63:
					// #line 542
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 64:
					// #line 546
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 65:
					// #line 558
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 66:
					// #line 602
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 67:
					// #line 695
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 68:
					// #line 687
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 69:
					// #line 109
					{ 
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 70:
					// #line 274
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ECHO);
					}
					break;
					
				case 71:
					// #line 179
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSE);
					}
					break;
					
				case 72:
					// #line 393
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EVAL);
					}
					break;
					
				case 73:
					// #line 249
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CASE);
					}
					break;
					
				case 74:
					// #line 519
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LIST);
					}
					break;
					
				case 75:
					// #line 269
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GOTO);
					}
					break;
					
				case 76:
					// #line 815
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 77:
					// #line 174
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 78:
					// #line 443
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 79:
					// #line 438
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ISSET);
					}
					break;
					
				case 80:
					// #line 294
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 81:
					// #line 159
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_THROW);
					}
					break;
					
				case 82:
					// #line 490
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINAL);
					}
					break;
					
				case 83:
					// #line 510
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_UNSET);
					}
					break;
					
				case 84:
					// #line 124
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONST);
					}
					break;
					
				case 85:
					// #line 355
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLONE);
					}
					break;
					
				case 86:
					// #line 284
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS);
					}
					break;
					
				case 87:
					// #line 149
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CATCH);
					}
					break;
					
				case 88:
					// #line 139
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_YIELD);
					}
					break;
					
				case 89:
					// #line 524
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 90:
					// #line 184
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_WHILE);
					}
					break;
					
				case 91:
					// #line 259
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_BREAK);
					}
					break;
					
				case 92:
					// #line 279
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRINT);
					}
					break;
					
				case 93:
					// #line 365
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 94:
					// #line 829
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
					// #line 204
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 96:
					// #line 169
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 97:
					// #line 480
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_STATIC);
					}
					break;
					
				case 98:
					// #line 239
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 99:
					// #line 129
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_RETURN);
					}
					break;
					
				case 100:
					// #line 433
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 101:
					// #line 505
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 102:
					// #line 369
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 103:
					// #line 385
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 104:
					// #line 299
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 105:
					// #line 398
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 106:
					// #line 254
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 107:
					// #line 219
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 108:
					// #line 154
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 109:
					// #line 209
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 110:
					// #line 408
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 111:
					// #line 495
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 112:
					// #line 389
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 113:
					// #line 377
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 114:
					// #line 741
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DIR);
					}
					break;
					
				case 115:
					// #line 189
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 116:
					// #line 119
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 117:
					// #line 264
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 118:
					// #line 529
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 119:
					// #line 485
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 120:
					// #line 381
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 121:
					// #line 373
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 122:
					// #line 736
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FILE);
					}
					break;
					
				case 123:
					// #line 731
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LINE);
					}
					break;
					
				case 124:
					// #line 244
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 125:
					// #line 289
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 126:
					// #line 428
					{
						this._tokenSemantics.Object = GetTokenString();
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 127:
					// #line 418
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 128:
					// #line 500
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 129:
					// #line 716
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 130:
					// #line 711
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 131:
					// #line 224
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 132:
					// #line 214
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 133:
					// #line 229
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 134:
					// #line 304
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 135:
					// #line 134
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 136:
					// #line 726
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 137:
					// #line 403
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 138:
					// #line 413
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 139:
					// #line 721
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 140:
					// #line 746
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NS_C);
					}
					break;
					
				case 141:
					// #line 463
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 142:
					// #line 81
					{
						if(!string.IsNullOrEmpty(GetTokenString()))
						{
							_tokenSemantics.Object = GetTokenString(); 
							return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
						return Tokens.EOF;
					}
					break;
					
				case 143:
					// #line 941
					{ yymore(); break; }
					break;
					
				case 144:
					// #line 939
					{ yymore(); break; }
					break;
					
				case 145:
					// #line 937
					{ Tokens token; if (ProcessString(1, out token)) return token; else break; }
					break;
					
				case 146:
					// #line 940
					{ yymore(); break; }
					break;
					
				case 147:
					// #line 936
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 148:
					// #line 935
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 149:
					// #line 934
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 150:
					// #line 886
					{ yymore(); break; }
					break;
					
				case 151:
					// #line 885
					{ yymore(); break; }
					break;
					
				case 152:
					// #line 884
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 153:
					// #line 883
					{ yymore(); break; }
					break;
					
				case 154:
					// #line 951
					{ yymore(); break; }
					break;
					
				case 155:
					// #line 949
					{ yymore(); break; }
					break;
					
				case 156:
					// #line 947
					{ Tokens token; if (ProcessShell(1, out token)) return token; else break; }
					break;
					
				case 157:
					// #line 950
					{ yymore(); break; }
					break;
					
				case 158:
					// #line 946
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 159:
					// #line 945
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 160:
					// #line 944
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 161:
					// #line 960
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 959
					{ yymore(); break; }
					break;
					
				case 163:
					// #line 957
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 958
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 955
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 166:
					// #line 954
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 167:
					// #line 953
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 168:
					// #line 871
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => (string)ProcessEscapedString(s, _encoding, false)) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 169:
					// #line 328
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 170:
					// #line 323
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 171:
					// #line 319
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 172:
					// #line 680
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 173:
					// #line 671
					{
						yyless(TokenLength - 1);
						this._tokenSemantics.Object = GetTokenString();
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 174:
					// #line 100
					{
						if(!string.IsNullOrEmpty(GetTokenString()))
						{
							SetDocBlock(); 
							return Tokens.T_DOC_COMMENT; 
						}
						return Tokens.EOF;
					}
					break;
					
				case 175:
					// #line 816
					{ yymore(); break; }
					break;
					
				case 176:
					// #line 818
					{ yymore(); break; }
					break;
					
				case 177:
					// #line 817
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 178:
					// #line 90
					{ 
						if(!string.IsNullOrEmpty(GetTokenString()))
							return Tokens.T_COMMENT; 
						return Tokens.EOF;
					}
					break;
					
				case 179:
					// #line 811
					{ yymore(); break; }
					break;
					
				case 180:
					// #line 813
					{ yymore(); break; }
					break;
					
				case 181:
					// #line 812
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 182:
					// #line 965
					{ yymore(); break; }
					break;
					
				case 183:
					// #line 95
					{ 
						if(!string.IsNullOrEmpty(GetTokenString()))
							return Tokens.T_COMMENT; 
						return Tokens.EOF;
					}
					break;
					
				case 184:
					// #line 964
					{ yymore(); break; }
					break;
					
				case 185:
					// #line 962
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 186:
					// #line 963
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 187:
					// #line 788
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 188:
					// #line 793
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 189:
					// #line 699
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 190:
					// #line 783
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 191:
					// #line 703
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 192:
					// #line 856
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						this._tokenSemantics.Object = this._hereDocLabel;
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 193:
					// #line 880
					{ yymore(); break; }
					break;
					
				case 194:
					// #line 862
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => s) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 195:
					// #line 475
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 196:
					// #line 469
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 197:
					// #line 448
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 198:
					// #line 472
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 199:
					// #line 473
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 200:
					// #line 471
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 201:
					// #line 453
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 202:
					// #line 458
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 203:
					// #line 927
					{ 
						yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 204:
					// #line 916
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 205:
					// #line 910
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 206:
					// #line 905
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 207:
					// #line 888
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 208:
					// #line 899
					{
						yyless(TokenLength - 1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 209:
					// #line 893
					{
						yyless(TokenLength - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 210:
					// #line 921
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 213: goto case 7;
				case 214: goto case 8;
				case 215: goto case 9;
				case 216: goto case 11;
				case 217: goto case 21;
				case 218: goto case 31;
				case 219: goto case 42;
				case 220: goto case 94;
				case 221: goto case 144;
				case 222: goto case 151;
				case 223: goto case 155;
				case 224: goto case 162;
				case 225: goto case 163;
				case 226: goto case 168;
				case 227: goto case 169;
				case 228: goto case 172;
				case 229: goto case 182;
				case 230: goto case 185;
				case 231: goto case 187;
				case 232: goto case 189;
				case 233: goto case 191;
				case 234: goto case 194;
				case 235: goto case 195;
				case 236: goto case 203;
				case 238: goto case 8;
				case 239: goto case 9;
				case 240: goto case 21;
				case 241: goto case 191;
				case 242: goto case 203;
				case 244: goto case 8;
				case 245: goto case 9;
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
				case 289: goto case 9;
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
				case 425: goto case 9;
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
				case 653: goto case 9;
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
			AcceptConditions.AcceptOnStart, // 194
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
			AcceptConditions.NotAccept, // 211
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
			AcceptConditions.AcceptOnStart, // 226
			AcceptConditions.Accept, // 227
			AcceptConditions.Accept, // 228
			AcceptConditions.Accept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.AcceptOnStart, // 234
			AcceptConditions.Accept, // 235
			AcceptConditions.Accept, // 236
			AcceptConditions.NotAccept, // 237
			AcceptConditions.Accept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.Accept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.NotAccept, // 243
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
			AcceptConditions.NotAccept, // 290
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
			AcceptConditions.NotAccept, // 407
			AcceptConditions.NotAccept, // 408
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
			AcceptConditions.Accept, // 425
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
			AcceptConditions.Accept, // 653
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
			0, 1, 1, 2, 3, 4, 1, 1, 5, 6, 7, 8, 1, 1, 1, 1, 
			1, 1, 1, 1, 1, 9, 10, 10, 10, 10, 1, 1, 1, 11, 1, 12, 
			1, 1, 13, 1, 14, 1, 1, 15, 1, 1, 16, 17, 18, 1, 1, 1, 
			1, 1, 1, 19, 10, 10, 10, 20, 10, 10, 10, 1, 1, 10, 1, 1, 
			1, 1, 1, 21, 22, 10, 10, 23, 10, 10, 10, 10, 24, 10, 10, 10, 
			10, 10, 25, 10, 10, 10, 10, 10, 26, 10, 10, 10, 10, 1, 1, 27, 
			10, 10, 10, 10, 10, 10, 1, 1, 10, 28, 10, 10, 10, 10, 29, 10, 
			1, 1, 10, 10, 10, 10, 10, 10, 1, 1, 10, 10, 10, 10, 10, 10, 
			10, 10, 10, 10, 10, 10, 10, 1, 10, 10, 10, 10, 10, 10, 1, 30, 
			31, 1, 1, 1, 1, 1, 32, 32, 1, 1, 33, 34, 1, 1, 1, 1, 
			1, 35, 1, 36, 1, 1, 1, 1, 1, 1, 37, 1, 1, 1, 1, 38, 
			39, 1, 1, 40, 41, 1, 42, 1, 43, 1, 1, 1, 1, 44, 1, 45, 
			1, 1, 1, 1, 46, 1, 1, 47, 48, 1, 1, 1, 1, 1, 49, 1, 
			1, 1, 1, 50, 51, 52, 53, 54, 55, 1, 56, 1, 57, 58, 59, 60, 
			61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 
			77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 1, 90, 91, 
			92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 
			108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 87, 68, 121, 
			21, 122, 22, 123, 56, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 
			135, 136, 24, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 
			150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 
			166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 
			182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 
			198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 
			214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 
			230, 231, 232, 233, 234, 235, 65, 236, 237, 238, 70, 78, 239, 240, 241, 242, 
			243, 48, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 
			258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 
			274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 
			290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 
			306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 
			322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 
			338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 
			354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 
			370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 
			386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 
			402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 
			418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 
			434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 
			450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 
			466, 467, 10, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 211, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 237, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 447, 642, 642, 642, 642, 642, 448, 449, 642, 642, 642, 642, 450, -1, 451, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 452, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 306, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, 51, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, 51, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 629, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 67, -1, -1, -1, 67, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, 68, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 329, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 349, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 348, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, 348, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, 348, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 633, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 558, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 650, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, -1, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, -1, 143, 143, 143, 143, 143, 143, 143, -1, -1, 143 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 147, -1 },
			{ -1, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1 },
			{ -1, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, -1, 154, 154, 154, 154, 154, 154, -1, 154, -1, 154 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 158, -1 },
			{ -1, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, -1, 161, 161, 161, 161, -1, 161, 161, 161, -1, 161 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1 },
			{ -1, -1, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, -1, -1, 170, 170, -1, -1, -1, -1, 170, -1, -1, -1, 170, 170, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, 170, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, -1, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, -1, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 183, 184, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 185, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 230, 229, 229, 229, 229, 229 },
			{ -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, 189, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, 191, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, 421, -1, 206, 206, -1, -1, -1, -1, 206, -1, -1, -1, 206, 206, 206, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, 206, 206, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, -1, 396, 396, 396, 396, 396, 396, 396, 396, -1, -1, 396, -1, -1, -1, -1, -1, 396, -1, -1, -1, 396, 396, 396, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 22, 642, 453, 642, 642, 454, 642, 642, 642, -1, 576, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 255, 288, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 290, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, 218, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1, 148, 148, 148, 148, 148, 148, 148, 148, -1, -1, 148, -1, -1, -1, -1, -1, 148, -1, -1, -1, 148, 148, 148, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 149, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 151, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1 },
			{ -1, -1, -1, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, -1, 159, 159, 159, 159, 159, 159, 159, 159, -1, -1, 159, -1, -1, -1, -1, -1, 159, -1, -1, -1, 159, 159, 159, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 160, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 162, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, -1, -1, 166, -1, -1, -1, -1, -1, 166, -1, -1, -1, 166, 166, 166, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, 406, -1, 406, 406, 406, 406, 406, 406, 406, 406, -1, -1, 406, 406, -1, -1, -1, -1, 406, -1, -1, -1, 406, 406, 406, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 173, 173, 406, 406, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, -1, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, -1, 229, 229, 229, 229, 229 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, -1, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 410, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 411, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, 191, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 233, -1, -1, -1, 233, 233, -1, -1, 233, -1, -1, -1, -1, -1, -1, -1, 233, -1, -1, 233, -1, -1, -1, -1, -1, -1, 233, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 233, 233, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 243, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 456, 642, 260, 642, 642, 642, 642, 642, 642, 23, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 241, 241, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, -1, -1, 206, -1, -1, -1, -1, -1, 206, -1, -1, -1, 206, 206, 206, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 24, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 213, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, 258, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 272, 642, 642, 25, 578, 642, 642, -1, 642, 642, 642, 642, 605, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 8, 9, 425, 215, 437, 239, 440, 442, 444, 575, 245, 604, 623, 634, 640, 10, 642, 248, 642, 644, 251, 642, 645, 646, 214, 238, 642, 11, 12, 244, 13, 247, 446, 250, 10, 253, 642, 647, 642, 253, 256, 259, 14, 262, 265, 268, 271, 274, 277, 280, 283, 253, 15, 16, 253, 216, 11, 10, 253, 17, 18, 286, 19 },
			{ -1, -1, -1, -1, -1, 261, -1, 264, 267, 428, -1, -1, 270, 273, 276, -1, -1, -1, -1, 279, -1, -1, 282, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 285, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 582, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 427, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 52, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, 218, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 34, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 53, 642, -1, 642, 476, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 54, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 429, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 55, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 294, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 56, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 296, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 57, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 300, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 58, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 430, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 61, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 302, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 69, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 438, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 70, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, 431, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 72, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 74, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 75, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 312, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 77, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 432, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 78, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 79, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 433, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 80, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 81, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 82, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 83, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, -1, 324, 324, 324, 324, 324, 324, 324, 324, -1, -1, 324, -1, -1, -1, -1, -1, 324, -1, 308, -1, 324, 324, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 326, -1, 435 },
			{ -1, -1, -1, 642, 642, 642, 84, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 85, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 439, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 86, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 87, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 336, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 88, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 89, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 90, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 441, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 91, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 324, 94, 324, 324, 324, 324, 324, 324, 324, 324, -1, -1, 324, 324, -1, -1, -1, -1, 324, -1, -1, -1, 324, 324, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 324, 324, 220, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 92, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, -1, 344, 344, 344, 344, 344, 344, 344, 344, -1, -1, 344, -1, -1, -1, -1, -1, 344, -1, -1, -1, 344, 344, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 95, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 96, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 97, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 98, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 99, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 100, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 338, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 101, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 104, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 105, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, 344, -1, 344, 344, 344, 344, 344, 344, 344, 344, -1, -1, 344, 344, -1, -1, -1, -1, 344, -1, -1, -1, 344, 344, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, 344, -1, -1, -1, 368, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 106, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, -1, 346, 346, 346, 346, 346, 346, 346, 346, -1, -1, 346, 346, -1, -1, -1, -1, 346, -1, -1, -1, 346, 346, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 346, 346, -1, -1, -1, -1, -1, 368 },
			{ -1, -1, -1, 107, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 348, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 108, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 445, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 109, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 110, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 354, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 111, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 370, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 114, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 115, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, 113, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 116, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 117, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 118, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 119, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 122, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 370, 120, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 123, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 372, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 124, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 366, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 125, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 378, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 126, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 135, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 127, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 142, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 382, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 144, 143, 143, 143, 143, 143, 143, 143, 145, 221, 143 },
			{ -1, -1, -1, 642, 642, 642, 642, 128, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 129, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 142, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 151, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 386, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 222, 150, 150, 150, 150, 152 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 130, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153, 153 },
			{ -1, -1, -1, 131, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 142, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 390, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 155, 154, 154, 154, 154, 154, 154, 156, 154, 223, 154 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 132, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 133, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 212, 142, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 162, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 394, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 163, 161, 161, 161, 161, 224, 161, 161, 161, 225, 161 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 134, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 136, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 396, 168, 396, 396, 396, 396, 396, 396, 396, 396, -1, -1, 396, 396, -1, -1, -1, -1, 396, -1, -1, -1, 396, 396, 396, 398, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 396, 396, 226, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 137, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 226, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 138, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 139, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 169, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 10, 170, 170, 170, 170, 170, 170, 170, 170, 227, 169, 170, 169, 169, 169, 169, 169, 170, 169, 10, 169, 170, 170, 170, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 169, 10, 169, 169, 169, 169, 169 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 140, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 172, 228, 228, 228, 228, 228, 228, 228, 228, 228, 228, 228, 228, 228, 228, 172, 228, 228, 228, 228, 228, 228, 228, 228, 172, 172, 228, 172, 172, 172, 172, 172, 228, 172, 172, 172, 228, 228, 228, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172, 172 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 141, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 174, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 176, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175 },
			{ 1, 178, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 180, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179 },
			{ 1, 2, 187, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 188, 642, 642, 642, 642, 642, 642, 642, 642, 187, 187, 642, 189, 12, 187, 188, 187, 642, 187, 188, 187, 642, 642, 642, 187, 187, 187, 188, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 187, 232, 189, 188, 190, 187, 187, 231, 188 },
			{ 1, 2, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192 },
			{ 426, 142, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193 },
			{ -1, -1, -1, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 194, 414, 414, 414, 414, 414, 414, 414, 414, -1, -1, 414, 414, -1, -1, -1, -1, 414, -1, -1, -1, 414, 414, 414, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 414, 414, 234, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 234, -1, -1, -1, -1, -1 },
			{ 1, 2, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 197, 196, 195, 195, 195, 195, 195, 235, 195, 198, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195 },
			{ 1, 2, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 201, 195, 195, 195, 195, 235, 195, 198, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195 },
			{ 1, 2, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 202, 235, 195, 198, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195 },
			{ 1, 2, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 236, 203, 203, 203, 203, 203, 203, 203, 204, 242, 203 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 422, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, -1, -1, 209, -1, -1, -1, -1, -1, 209, -1, -1, -1, 209, 209, 209, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 236, 203, 203, 203, 203, 203, 203, 210, 203, 242, 203 },
			{ 1, 1, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 236, 203, 203, 203, 203, 203, 203, 203, 203, 242, 203 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 254, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, -1, 414, 414, 414, 414, 414, 414, 414, 414, -1, -1, 414, -1, -1, -1, -1, -1, 414, -1, -1, -1, 414, 414, 414, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 298, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 434, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 334, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, 346, -1, 346, 346, 346, 346, 346, 346, 346, 346, -1, -1, 346, -1, -1, -1, -1, -1, 346, -1, -1, -1, 346, 346, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 257, 642, 642, -1, 642, 642, 455, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 352, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 457, 642, 642, 642, 579, 642, 642, 263, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 577, 642, 642, 266, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 376, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 269, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 458, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 275, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 278, 607, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 471, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 281, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 284, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 472, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 287, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 624, 642, 642, 642, 642, 473, 642, 474, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 475, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 477, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 580, 642, 642, 608, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 478, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 635, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 482, 642, 642, 642, 642, -1, 642, 483, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 484, 642, 642, 642, 642, 642, 642, 289, 642, 642, 652, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 585, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 609, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 485, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 586, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 486, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 291, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 293, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 583, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 625, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 490, 642, 642, 642, 642, 642, 642, 637, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 491, 492, 493, 642, 494, 636, 642, 642, 642, 642, 588, -1, 495, 642, 589, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 295, 642, 590, 497, 642, 642, 642, 642, 498, 642, 642, 642, -1, 642, 642, 642, 499, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 297, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 610, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 500, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 299, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 301, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 303, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 305, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 501, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 307, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 612, 642, 642, 642, 642, 642, 642, 309, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 311, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 313, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 315, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 505, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 317, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 319, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 321, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 323, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 325, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 641, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 643, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 508, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 509, 642, 642, 642, 591, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 510, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 592, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 512, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 327, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 514, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 596, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 594, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 517, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 617, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 521, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 331, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 333, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 335, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 337, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 339, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 525, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 526, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 598, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 527, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 341, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 530, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 600, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 532, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 343, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 534, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 345, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 347, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 351, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 601, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 537, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 353, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 355, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 357, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 539, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 619, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 541, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 542, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 621, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 359, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 544, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 545, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 546, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 361, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 363, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 365, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 367, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 369, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 371, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 554, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 555, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 373, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 375, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 377, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 559, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 560, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 379, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 381, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 383, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 653, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 561, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 385, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 562, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 602, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 387, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 389, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 563, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 391, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 393, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 564, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 395, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 566, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 569, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 570, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 397, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 399, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 401, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 571, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 572, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 403, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 573, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 574, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 405, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 606, 642, 642, 642, 459, -1, 642, 460, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 584, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 480, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 487, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 479, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 627, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 488, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 489, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 506, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 614, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 503, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 628, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 515, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 615, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 593, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 513, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 651, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 528, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 529, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 533, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 620, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 531, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 536, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 599, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 552, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 543, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 548, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 565, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 567, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 461, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 462, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 626, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 481, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 496, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 613, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 504, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 516, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 616, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 597, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 519, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 648, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 618, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 538, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 535, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 540, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 553, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 549, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 556, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 568, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 463, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 587, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 507, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 611, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 518, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 523, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 520, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 595, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 547, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 550, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 557, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 464, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 502, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 511, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 630, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 522, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 551, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 465, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 524, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 649, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 581, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 466, 642, 642, 642, 467, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 468, 642, 642, 642, 642, 469, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 470, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 631, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 632, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 603, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 639, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 638, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 622, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  249,
			  380,
			  384,
			  388,
			  392,
			  400,
			  402,
			  404,
			  407,
			  408,
			  182,
			  409,
			  412,
			  413,
			  416,
			  418,
			  419,
			  420,
			  423,
			  424
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 654);
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

