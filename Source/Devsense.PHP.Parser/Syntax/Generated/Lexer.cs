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
					// #line 97
					{ 
						if(TokenLength > 0)
						{
							this._tokenSemantics.Object = GetTokenString();
							return Tokens.T_INLINE_HTML;
						}
						return Tokens.EOF;
					}
					break;
					
				case 3:
					// #line 810
					{
						yymore(); break;
					}
					break;
					
				case 4:
					// #line 794
					{
						if (TokenLength > 2)
						{
							string text = GetTokenString();
							_yyless(Math.Abs(text.LastIndexOf('<') - text.Length));
							this._tokenSemantics.Object = text.Substring(0, text.LastIndexOf('<'));
							return Tokens.T_INLINE_HTML; 
						}
						if (this._allowShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							yymore(); break;//return Tokens.T_INLINE_HTML;
						}
					}
					break;
					
				case 5:
					// #line 767
					{
						if (TokenLength > 3)
						{
							string text = GetTokenString();
							_yyless(Math.Abs(text.LastIndexOf('<') - text.Length));
							this._tokenSemantics.Object = text.Substring(0, text.LastIndexOf('<'));
							return Tokens.T_INLINE_HTML; 
						}
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 6:
					// #line 780
					{
						if (GetTokenString().LastIndexOf('<') != 0)
						{
							string text = GetTokenString();
							_yyless(Math.Abs(text.LastIndexOf('<') - text.Length));
							this._tokenSemantics.Object = text.Substring(0, text.LastIndexOf('<'));
							return Tokens.T_INLINE_HTML; 
						}
						//HANDLE_NEWLINE(yytext[yyleng-1]);
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
					// #line 666
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 831
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 330
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 11:
					// #line 707
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 12:
					// #line 998
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 13:
					// #line 354
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 14:
					// #line 835
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 15:
					// #line 671
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 679
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 974
					{ BEGIN(LexicalStates.ST_BACKQUOTE); return Tokens.T_BACKQUOTE; }
					break;
					
				case 18:
					// #line 964
					{ BEGIN(LexicalStates.ST_DOUBLE_QUOTES); yymore(); break; }
					break;
					
				case 19:
					// #line 913
					{ BEGIN(LexicalStates.ST_SINGLE_QUOTES); yymore(); break; }
					break;
					
				case 20:
					// #line 362
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 851
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 180
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 210
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DO);
					}
					break;
					
				case 24:
					// #line 646
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 25:
					// #line 250
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_AS);
					}
					break;
					
				case 26:
					// #line 554
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 27:
					// #line 325
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 28:
					// #line 590
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 29:
					// #line 662
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 30:
					// #line 582
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 31:
					// #line 723
					{
						return ProcessRealNumber();
					}
					break;
					
				case 32:
					// #line 350
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 33:
					// #line 610
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 34:
					// #line 841
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 35:
					// #line 606
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 36:
					// #line 598
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 37:
					// #line 594
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 38:
					// #line 531
					{
						return (Tokens.T_DOUBLE_ARROW);
					}
					break;
					
				case 39:
					// #line 566
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 40:
					// #line 586
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 41:
					// #line 550
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 42:
					// #line 570
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 43:
					// #line 578
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 44:
					// #line 658
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 45:
					// #line 614
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 46:
					// #line 626
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 47:
					// #line 642
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 48:
					// #line 630
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 49:
					// #line 638
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 50:
					// #line 634
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 51:
					// #line 856
					{
						return ProcessVariable();
					}
					break;
					
				case 52:
					// #line 654
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 53:
					// #line 160
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRY);
					}
					break;
					
				case 54:
					// #line 130
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT);
					}
					break;
					
				case 55:
					// #line 215
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOR);
					}
					break;
					
				case 56:
					// #line 439
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_USE);
					}
					break;
					
				case 57:
					// #line 366
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NEW);
					}
					break;
					
				case 58:
					// #line 650
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 59:
					// #line 622
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 60:
					// #line 358
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 61:
					// #line 376
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_VAR);
					}
					break;
					
				case 62:
					// #line 602
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 63:
					// #line 558
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 64:
					// #line 562
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 65:
					// #line 574
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 66:
					// #line 618
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 67:
					// #line 711
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 68:
					// #line 703
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 69:
					// #line 125
					{ 
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 70:
					// #line 290
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ECHO);
					}
					break;
					
				case 71:
					// #line 195
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSE);
					}
					break;
					
				case 72:
					// #line 409
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EVAL);
					}
					break;
					
				case 73:
					// #line 265
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CASE);
					}
					break;
					
				case 74:
					// #line 535
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LIST);
					}
					break;
					
				case 75:
					// #line 285
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GOTO);
					}
					break;
					
				case 76:
					// #line 846
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 77:
					// #line 190
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 78:
					// #line 459
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 79:
					// #line 454
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ISSET);
					}
					break;
					
				case 80:
					// #line 310
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 81:
					// #line 175
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_THROW);
					}
					break;
					
				case 82:
					// #line 506
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINAL);
					}
					break;
					
				case 83:
					// #line 526
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_UNSET);
					}
					break;
					
				case 84:
					// #line 140
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONST);
					}
					break;
					
				case 85:
					// #line 371
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLONE);
					}
					break;
					
				case 86:
					// #line 300
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS);
					}
					break;
					
				case 87:
					// #line 165
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CATCH);
					}
					break;
					
				case 88:
					// #line 155
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_YIELD);
					}
					break;
					
				case 89:
					// #line 540
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 90:
					// #line 200
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_WHILE);
					}
					break;
					
				case 91:
					// #line 275
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_BREAK);
					}
					break;
					
				case 92:
					// #line 295
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRINT);
					}
					break;
					
				case 93:
					// #line 381
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 94:
					// #line 860
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
					// #line 220
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 96:
					// #line 185
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 97:
					// #line 496
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_STATIC);
					}
					break;
					
				case 98:
					// #line 255
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 99:
					// #line 145
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_RETURN);
					}
					break;
					
				case 100:
					// #line 449
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 101:
					// #line 521
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 102:
					// #line 385
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 103:
					// #line 401
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 104:
					// #line 315
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 105:
					// #line 414
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 106:
					// #line 270
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 107:
					// #line 235
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 108:
					// #line 170
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 109:
					// #line 225
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 110:
					// #line 424
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 111:
					// #line 511
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 112:
					// #line 405
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 113:
					// #line 393
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 114:
					// #line 757
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DIR);
					}
					break;
					
				case 115:
					// #line 205
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 116:
					// #line 135
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 117:
					// #line 280
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 118:
					// #line 545
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 119:
					// #line 501
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 120:
					// #line 397
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 121:
					// #line 389
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 122:
					// #line 752
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FILE);
					}
					break;
					
				case 123:
					// #line 747
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LINE);
					}
					break;
					
				case 124:
					// #line 260
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 125:
					// #line 305
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 126:
					// #line 444
					{
						this._tokenSemantics.Object = GetTokenString();
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 127:
					// #line 434
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 128:
					// #line 516
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 129:
					// #line 732
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 130:
					// #line 727
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 131:
					// #line 240
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 132:
					// #line 230
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 133:
					// #line 245
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 134:
					// #line 320
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 135:
					// #line 150
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 136:
					// #line 742
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 137:
					// #line 419
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 138:
					// #line 429
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 139:
					// #line 737
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 140:
					// #line 762
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NS_C);
					}
					break;
					
				case 141:
					// #line 479
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 142:
					// #line 89
					{
						if(TokenLength > 0)
						{
							return ProcessStringEOF(); 
						}
						return Tokens.EOF;
					}
					break;
					
				case 143:
					// #line 972
					{ yymore(); break; }
					break;
					
				case 144:
					// #line 970
					{ yymore(); break; }
					break;
					
				case 145:
					// #line 968
					{ Tokens token; if (ProcessString(1, out token)) return token; else break; }
					break;
					
				case 146:
					// #line 971
					{ yymore(); break; }
					break;
					
				case 147:
					// #line 967
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 148:
					// #line 966
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 149:
					// #line 965
					{ Tokens token; if (ProcessString(2, out token)) return token; else break; }
					break;
					
				case 150:
					// #line 81
					{
						if(TokenLength > 0)
						{
							_tokenSemantics.Object = GetTokenString(); 
							return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
						return Tokens.EOF;
					}
					break;
					
				case 151:
					// #line 917
					{ yymore(); break; }
					break;
					
				case 152:
					// #line 916
					{ yymore(); break; }
					break;
					
				case 153:
					// #line 915
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 154:
					// #line 914
					{ yymore(); break; }
					break;
					
				case 155:
					// #line 982
					{ yymore(); break; }
					break;
					
				case 156:
					// #line 980
					{ yymore(); break; }
					break;
					
				case 157:
					// #line 978
					{ Tokens token; if (ProcessShell(1, out token)) return token; else break; }
					break;
					
				case 158:
					// #line 981
					{ yymore(); break; }
					break;
					
				case 159:
					// #line 977
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 160:
					// #line 976
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 161:
					// #line 975
					{ Tokens token; if (ProcessShell(2, out token)) return token; else break; }
					break;
					
				case 162:
					// #line 991
					{ yymore(); break; }
					break;
					
				case 163:
					// #line 990
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 988
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 989
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 986
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 167:
					// #line 985
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 168:
					// #line 984
					{ Tokens token; if (ProcessHeredoc(2, out token)) return token; else break; }
					break;
					
				case 169:
					// #line 902
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
					// #line 344
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 171:
					// #line 339
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 172:
					// #line 335
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 173:
					// #line 696
					{
						_yyless(1);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 174:
					// #line 687
					{
						_yyless(1);
						this._tokenSemantics.Object = GetTokenString();
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 175:
					// #line 116
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
					// #line 847
					{ yymore(); break; }
					break;
					
				case 177:
					// #line 849
					{ yymore(); break; }
					break;
					
				case 178:
					// #line 848
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 179:
					// #line 106
					{ 
						if(TokenLength > 0)
							return Tokens.T_COMMENT; 
						return Tokens.EOF;
					}
					break;
					
				case 180:
					// #line 842
					{ yymore(); break; }
					break;
					
				case 181:
					// #line 844
					{ yymore(); break; }
					break;
					
				case 182:
					// #line 843
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 183:
					// #line 996
					{ yymore(); break; }
					break;
					
				case 184:
					// #line 111
					{ 
						if(TokenLength > 0)
							return Tokens.T_COMMENT; 
						return Tokens.EOF;
					}
					break;
					
				case 185:
					// #line 995
					{ yymore(); break; }
					break;
					
				case 186:
					// #line 993
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 187:
					// #line 994
					{ _yyless(2); yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 188:
					// #line 819
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 189:
					// #line 715
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 190:
					// #line 814
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 191:
					// #line 719
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 192:
					// #line 887
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						this._tokenSemantics.Object = this._hereDocLabel;
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 193:
					// #line 911
					{ yymore(); break; }
					break;
					
				case 194:
					// #line 893
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
					// #line 491
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 196:
					// #line 485
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 197:
					// #line 464
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 198:
					// #line 488
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 199:
					// #line 489
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 200:
					// #line 487
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 201:
					// #line 469
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 202:
					// #line 474
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 203:
					// #line 958
					{ 
						_yyless(1); 
						yy_pop_state(); 
						yymore(); break; 
					}
					break;
					
				case 204:
					// #line 947
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 205:
					// #line 941
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						_yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 206:
					// #line 936
					{
						yy_pop_state();
						return ProcessVariable();
					}
					break;
					
				case 207:
					// #line 919
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 208:
					// #line 930
					{
						_yyless(1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 209:
					// #line 924
					{
						_yyless(3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 210:
					// #line 952
					{
						yy_pop_state();
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 213: goto case 3;
				case 214: goto case 6;
				case 215: goto case 8;
				case 216: goto case 9;
				case 217: goto case 11;
				case 218: goto case 21;
				case 219: goto case 31;
				case 220: goto case 42;
				case 221: goto case 94;
				case 222: goto case 144;
				case 223: goto case 152;
				case 224: goto case 156;
				case 225: goto case 163;
				case 226: goto case 164;
				case 227: goto case 169;
				case 228: goto case 170;
				case 229: goto case 173;
				case 230: goto case 183;
				case 231: goto case 186;
				case 232: goto case 188;
				case 233: goto case 189;
				case 234: goto case 191;
				case 235: goto case 194;
				case 236: goto case 195;
				case 237: goto case 203;
				case 239: goto case 8;
				case 240: goto case 9;
				case 241: goto case 21;
				case 242: goto case 191;
				case 243: goto case 203;
				case 245: goto case 8;
				case 246: goto case 9;
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
			AcceptConditions.Accept, // 226
			AcceptConditions.AcceptOnStart, // 227
			AcceptConditions.Accept, // 228
			AcceptConditions.Accept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.Accept, // 234
			AcceptConditions.AcceptOnStart, // 235
			AcceptConditions.Accept, // 236
			AcceptConditions.Accept, // 237
			AcceptConditions.NotAccept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.Accept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.NotAccept, // 244
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
			1, 1, 1, 1, 44, 1, 1, 45, 46, 1, 1, 1, 1, 1, 47, 1, 
			1, 1, 1, 48, 49, 50, 51, 52, 53, 54, 1, 55, 1, 56, 57, 58, 
			59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 
			75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 1, 89, 
			90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 
			106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 86, 117, 118, 19, 67, 
			119, 20, 120, 55, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 
			133, 22, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 
			148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 
			164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 
			180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 
			196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 
			212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 
			228, 229, 230, 231, 232, 64, 233, 234, 235, 236, 69, 77, 237, 238, 239, 240, 
			241, 46, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 
			256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 
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
			464, 465, 8, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 213, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 211, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 447, 642, 642, 642, 642, 642, 448, 449, 642, 642, 642, 642, 450, -1, 451, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 452, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 253, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 241, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 253, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, 51, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, 51, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 629, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 67, -1, -1, -1, 67, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, 68, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 330, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 350, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 347, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, 347, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, 347, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 633, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 558, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 650, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
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
			{ 1, 184, 185, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 186, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 231, 230, 230, 230, 230, 230 },
			{ -1, 184, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, 189, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, 191, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, 421, -1, 206, 206, -1, -1, -1, -1, 206, -1, -1, -1, 206, 206, 206, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, 206, 206, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 238, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, -1, 395, 395, 395, 395, 395, 395, 395, 395, -1, -1, 395, -1, -1, -1, -1, -1, 395, -1, -1, -1, 395, 395, 395, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 22, 642, 453, 642, 642, 454, 642, 642, 642, -1, 576, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 253, 286, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 289, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, 219, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1, 148, 148, 148, 148, 148, 148, 148, 148, -1, -1, 148, -1, -1, -1, -1, -1, 148, -1, -1, -1, 148, 148, 148, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 149, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 152, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1 },
			{ -1, -1, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1, 160, 160, 160, 160, 160, 160, 160, 160, -1, -1, 160, -1, -1, -1, -1, -1, 160, -1, -1, -1, 160, 160, 160, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 161, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 163, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, -1, -1, 167, -1, -1, -1, -1, -1, 167, -1, -1, -1, 167, 167, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, 405, -1, 405, 405, 405, 405, 405, 405, 405, 405, -1, -1, 405, 405, -1, -1, -1, -1, 405, -1, -1, -1, 405, 405, 405, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, 405, 405, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, -1, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, 230, -1, 230, 230, 230, 230, 230 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, -1, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 410, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 411, -1, -1, -1, -1, -1, -1, 191, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 191, 191, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 234, -1, -1, -1, 234, 234, -1, -1, 234, -1, -1, -1, -1, -1, -1, -1, 234, -1, -1, 234, -1, -1, -1, -1, -1, -1, 234, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 234, 234, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 456, 642, 261, 642, 642, 642, 642, 642, 642, 23, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 242, 242, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, -1, 206, 206, 206, 206, 206, 206, 206, 206, -1, -1, 206, -1, -1, -1, -1, -1, 206, -1, -1, -1, 206, 206, 206, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 24, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 8, 9, 425, 216, 437, 240, 440, 442, 444, 575, 246, 604, 623, 634, 640, 10, 642, 249, 642, 644, 252, 642, 645, 646, 215, 239, 642, 11, 12, 245, 13, 248, 446, 251, 10, 254, 642, 647, 642, 254, 257, 260, 14, 263, 266, 269, 272, 275, 278, 281, 284, 254, 15, 16, 254, 217, 11, 10, 254, 17, 18, 287, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 273, 642, 642, 25, 578, 642, 642, -1, 642, 642, 642, 642, 605, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 427, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 259, -1, 262, 265, 428, -1, -1, 268, 271, 274, -1, -1, -1, -1, 277, -1, -1, 280, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 283, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 582, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, 250, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, 18, -1, 19 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, -1, 219, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, 219, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 52, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 34, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 53, 642, -1, 642, 476, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 429, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 54, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 55, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 295, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 56, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 299, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 57, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 430, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 58, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 301, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 61, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 438, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 69, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 303, -1, -1, -1, -1, -1, -1, 431, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 70, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 71, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 72, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 74, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 75, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 432, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 77, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 78, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 433, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 79, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 80, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 81, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 82, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, -1, 323, 323, 323, 323, 323, 323, 323, 323, -1, -1, 323, -1, -1, -1, -1, -1, 323, -1, 307, -1, 323, 323, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, -1, 435 },
			{ -1, -1, -1, 642, 642, 642, 83, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 84, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 439, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 85, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 86, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 87, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 88, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 89, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 441, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 90, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 323, 94, 323, 323, 323, 323, 323, 323, 323, 323, -1, -1, 323, 323, -1, -1, -1, -1, 323, -1, -1, -1, 323, 323, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 323, 323, 221, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 91, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, -1, 343, 343, 343, 343, 343, 343, 343, 343, -1, -1, 343, -1, -1, -1, -1, -1, 343, -1, -1, -1, 343, 343, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 92, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 95, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 96, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 97, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 98, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 99, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 100, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 101, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 104, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, 343, -1, 343, 343, 343, 343, 343, 343, 343, 343, -1, -1, 343, 343, -1, -1, -1, -1, 343, -1, -1, -1, 343, 343, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, 343, -1, -1, -1, 367, -1, -1 },
			{ -1, -1, -1, 105, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, -1, 345, 345, 345, 345, 345, 345, 345, 345, -1, -1, 345, 345, -1, -1, -1, -1, 345, -1, -1, -1, 345, 345, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, 345, -1, -1, -1, -1, -1, 367 },
			{ -1, -1, -1, 642, 642, 642, 106, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 347, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 107, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 445, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 108, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 109, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 110, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 369, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 111, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 114, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 359, 113, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 115, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 116, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 117, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 118, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 119, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 369, 120, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 122, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 123, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 124, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 125, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 135, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 126, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 142, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 381, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 143, 144, 143, 143, 143, 143, 143, 143, 143, 145, 222, 143 },
			{ -1, -1, -1, 127, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146, 146 },
			{ -1, -1, -1, 642, 642, 642, 642, 128, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 150, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 152, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 385, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 223, 151, 151, 151, 151, 153 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 129, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 130, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 150, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 389, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 156, 155, 155, 155, 155, 155, 155, 157, 155, 224, 155 },
			{ -1, -1, -1, 131, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 132, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 212, 150, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 163, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 393, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 162, 164, 162, 162, 162, 162, 225, 162, 162, 162, 226, 162 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 133, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165, 165 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 134, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 395, 169, 395, 395, 395, 395, 395, 395, 395, 395, -1, -1, 395, 395, -1, -1, -1, -1, 395, -1, -1, -1, 395, 395, 395, 397, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 395, 395, 227, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 136, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 137, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 138, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 170, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 10, 171, 171, 171, 171, 171, 171, 171, 171, 228, 170, 171, 170, 170, 170, 170, 170, 171, 170, 10, 170, 171, 171, 171, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 10, 170, 170, 170, 170, 170 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 139, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 7, 173, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 229, 173, 229, 229, 229, 229, 229, 229, 229, 229, 173, 173, 229, 173, 173, 173, 173, 173, 229, 173, 173, 173, 229, 229, 229, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173, 173 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 140, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 141, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ 1, 175, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 177, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176 },
			{ 1, 179, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 181, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180 },
			{ 1, 7, 188, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 12, 642, 642, 642, 642, 642, 642, 642, 642, 188, 188, 642, 189, 12, 188, 12, 188, 642, 188, 12, 188, 642, 642, 642, 188, 188, 188, 12, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 188, 233, 189, 12, 190, 188, 188, 232, 12 },
			{ 1, 7, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192, 192 },
			{ 426, 150, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193, 193 },
			{ -1, -1, -1, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 194, 414, 414, 414, 414, 414, 414, 414, 414, -1, -1, 414, 414, -1, -1, -1, -1, 414, -1, -1, -1, 414, 414, 414, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 414, 414, 235, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 235, -1, -1, -1, -1, -1 },
			{ 1, 7, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 197, 196, 195, 195, 195, 195, 195, 236, 195, 198, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195 },
			{ 1, 7, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 201, 195, 195, 195, 195, 236, 195, 198, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195 },
			{ 1, 7, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 202, 236, 195, 198, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 195, 196, 195, 195, 195, 195, 195 },
			{ 1, 7, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 237, 203, 203, 203, 203, 203, 203, 203, 204, 243, 203 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 422, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, -1, 209, 209, 209, 209, 209, 209, 209, 209, -1, -1, 209, -1, -1, -1, -1, -1, 209, -1, -1, -1, 209, 209, 209, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 1, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 237, 203, 203, 203, 203, 203, 203, 210, 203, 243, 203 },
			{ 1, 1, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 237, 203, 203, 203, 203, 203, 203, 203, 203, 243, 203 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 255, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, 414, -1, 414, 414, 414, 414, 414, 414, 414, 414, -1, -1, 414, -1, -1, -1, -1, -1, 414, -1, -1, -1, 414, 414, 414, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 297, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 434, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, 345, -1, 345, 345, 345, 345, 345, 345, 345, 345, -1, -1, 345, -1, -1, -1, -1, -1, 345, -1, -1, -1, 345, 345, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 258, 642, 642, -1, 642, 642, 455, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 457, 642, 642, 642, 579, 642, 642, 264, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 577, 642, 642, 267, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 270, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 458, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 276, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 279, 607, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 471, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 282, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 285, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 472, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 288, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 624, 642, 642, 642, 642, 473, 642, 474, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 475, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 477, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 580, 642, 642, 608, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 478, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 635, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 482, 642, 642, 642, 642, -1, 642, 483, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 484, 642, 642, 642, 642, 642, 642, 290, 642, 642, 652, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 585, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 609, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 485, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 586, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 486, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 292, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 294, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 583, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 625, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 490, 642, 642, 642, 642, 642, 642, 637, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 491, 492, 493, 642, 494, 636, 642, 642, 642, 642, 588, -1, 495, 642, 589, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 296, 642, 590, 497, 642, 642, 642, 642, 498, 642, 642, 642, -1, 642, 642, 642, 499, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 298, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 610, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 500, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 300, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 302, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 304, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 306, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 501, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 308, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 612, 642, 642, 642, 642, 642, 642, 310, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 312, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 314, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 316, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 505, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 318, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 320, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 322, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 324, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 326, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 641, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 643, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 508, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 509, 642, 642, 642, 591, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 510, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 592, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 512, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 328, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 514, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 596, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 594, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 517, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 617, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 521, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 332, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 334, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 336, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 338, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 340, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 525, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 526, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 598, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 527, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 342, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 530, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 600, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 532, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 344, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 534, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 346, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 348, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 352, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 601, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 537, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 354, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 356, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 358, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 539, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 619, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 541, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 542, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 621, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 360, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 544, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 545, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 546, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 362, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 364, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 366, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 368, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 370, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 372, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 554, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 555, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 374, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 376, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 378, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 559, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 560, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 380, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 382, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 384, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 653, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 561, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 386, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 562, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 602, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 388, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 390, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 563, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 392, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 394, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 564, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 396, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 566, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 569, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 570, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 398, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 400, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 402, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 571, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 572, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 404, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 573, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 574, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 406, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, 642, -1, 642, 642, 642, 642, 642, 642, 642, 642, -1, -1, 642, 642, -1, -1, -1, -1, 642, -1, -1, -1, 642, 642, 642, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 642, 642, -1, -1, -1, -1, -1, -1 },
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
			  247,
			  379,
			  383,
			  387,
			  391,
			  399,
			  401,
			  403,
			  407,
			  408,
			  183,
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

