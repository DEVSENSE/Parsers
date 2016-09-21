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
using System.Linq;
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
					// #line 76
					{
						return Tokens.EOF;
					}
					break;
					
				case 3:
					// #line 748
					{ 
						if(GetTokenString().ToCharArray().All(c => c == '<')) { yymore(); break; }
					    this._tokenSemantics.Object = GetTokenString();
						return Tokens.T_INLINE_HTML; 
					}
					break;
					
				case 4:
					// #line 767
					{
						if (this._allowShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							yymore(); break;//return Tokens.T_INLINE_HTML;
						}
					}
					break;
					
				case 5:
					// #line 754
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 6:
					// #line 760
					{
						//HANDLE_NEWLINE(yytext[yyleng-1]);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 7:
					// #line 814
					{
						return ProcessLabel();
					}
					break;
					
				case 8:
					// #line 307
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 9:
					// #line 643
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 10:
					// #line 688
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 11:
					// #line 959
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 12:
					// #line 331
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 13:
					// #line 818
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 14:
					// #line 648
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 15:
					// #line 660
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 16:
					// #line 856
					{
						BEGIN(LexicalStates.ST_DOUBLE_QUOTES);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 17:
					// #line 841
					{ 
						// Gets here only in the case of unterminated singly-quoted string. That leads usually to an error token,
						// however when the source code is parsed per-line (as in Visual Studio colorizer) it is important to remember
						// that we are in the singly-quoted string at the end of the line.
						BEGIN(LexicalStates.ST_SINGLE_QUOTES); 
						yymore(); 
						break; 
					}
					break;
					
				case 18:
					// #line 889
					{
						BEGIN(LexicalStates.ST_BACKQUOTE); 
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 19:
					// #line 157
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IF);
					}
					break;
					
				case 20:
					// #line 187
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DO);
					}
					break;
					
				case 21:
					// #line 623
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 22:
					// #line 227
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_AS);
					}
					break;
					
				case 23:
					// #line 531
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 24:
					// #line 302
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 25:
					// #line 567
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 26:
					// #line 639
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 27:
					// #line 559
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 28:
					// #line 704
					{
						return ProcessRealNumber();
					}
					break;
					
				case 29:
					// #line 327
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 30:
					// #line 587
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 31:
					// #line 834
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 32:
					// #line 339
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 33:
					// #line 824
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 34:
					// #line 583
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 35:
					// #line 575
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 36:
					// #line 571
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 37:
					// #line 508
					{
						return (Tokens.T_DOUBLE_ARROW);
					}
					break;
					
				case 38:
					// #line 543
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 39:
					// #line 563
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 40:
					// #line 527
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 41:
					// #line 547
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 42:
					// #line 555
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 43:
					// #line 635
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 44:
					// #line 591
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 45:
					// #line 603
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 46:
					// #line 619
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 47:
					// #line 607
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 48:
					// #line 615
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 49:
					// #line 611
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 50:
					// #line 793
					{
						return ProcessVariable();
					}
					break;
					
				case 51:
					// #line 852
					{
						return ProcessDoubleQuotedString();
					}
					break;
					
				case 52:
					// #line 839
					{ return ProcessSingleQuotedString(); }
					break;
					
				case 53:
					// #line 631
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 54:
					// #line 137
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRY);
					}
					break;
					
				case 55:
					// #line 107
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT);
					}
					break;
					
				case 56:
					// #line 192
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOR);
					}
					break;
					
				case 57:
					// #line 416
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_USE);
					}
					break;
					
				case 58:
					// #line 343
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NEW);
					}
					break;
					
				case 59:
					// #line 627
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 60:
					// #line 599
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 61:
					// #line 335
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 62:
					// #line 353
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_VAR);
					}
					break;
					
				case 63:
					// #line 579
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 64:
					// #line 535
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 65:
					// #line 539
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 66:
					// #line 551
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 67:
					// #line 595
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 68:
					// #line 692
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 69:
					// #line 684
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 70:
					// #line 102
					{ 
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 71:
					// #line 267
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ECHO);
					}
					break;
					
				case 72:
					// #line 172
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSE);
					}
					break;
					
				case 73:
					// #line 386
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EVAL);
					}
					break;
					
				case 74:
					// #line 242
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CASE);
					}
					break;
					
				case 75:
					// #line 512
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LIST);
					}
					break;
					
				case 76:
					// #line 262
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GOTO);
					}
					break;
					
				case 77:
					// #line 829
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 78:
					// #line 167
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 79:
					// #line 436
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 80:
					// #line 431
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ISSET);
					}
					break;
					
				case 81:
					// #line 287
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 82:
					// #line 152
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_THROW);
					}
					break;
					
				case 83:
					// #line 483
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINAL);
					}
					break;
					
				case 84:
					// #line 503
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_UNSET);
					}
					break;
					
				case 85:
					// #line 117
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONST);
					}
					break;
					
				case 86:
					// #line 348
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLONE);
					}
					break;
					
				case 87:
					// #line 277
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS);
					}
					break;
					
				case 88:
					// #line 142
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CATCH);
					}
					break;
					
				case 89:
					// #line 132
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_YIELD);
					}
					break;
					
				case 90:
					// #line 517
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 91:
					// #line 177
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_WHILE);
					}
					break;
					
				case 92:
					// #line 252
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_BREAK);
					}
					break;
					
				case 93:
					// #line 272
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRINT);
					}
					break;
					
				case 94:
					// #line 358
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 95:
					// #line 862
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
					
				case 96:
					// #line 197
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 97:
					// #line 162
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 98:
					// #line 473
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_STATIC);
					}
					break;
					
				case 99:
					// #line 232
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 100:
					// #line 122
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_RETURN);
					}
					break;
					
				case 101:
					// #line 426
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 102:
					// #line 498
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 103:
					// #line 362
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 104:
					// #line 378
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 105:
					// #line 292
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 106:
					// #line 391
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 107:
					// #line 247
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 108:
					// #line 212
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 109:
					// #line 147
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 110:
					// #line 202
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 111:
					// #line 401
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 112:
					// #line 488
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 113:
					// #line 382
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 114:
					// #line 370
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 115:
					// #line 738
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DIR);
					}
					break;
					
				case 116:
					// #line 182
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 117:
					// #line 112
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 118:
					// #line 257
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 119:
					// #line 522
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 120:
					// #line 478
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 121:
					// #line 374
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 122:
					// #line 366
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 123:
					// #line 733
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FILE);
					}
					break;
					
				case 124:
					// #line 728
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LINE);
					}
					break;
					
				case 125:
					// #line 237
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 126:
					// #line 282
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 127:
					// #line 421
					{
						this._tokenSemantics.Object = GetTokenString();
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 128:
					// #line 411
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 129:
					// #line 493
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 130:
					// #line 713
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 131:
					// #line 708
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 132:
					// #line 217
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 133:
					// #line 207
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 134:
					// #line 222
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 135:
					// #line 297
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 136:
					// #line 127
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 137:
					// #line 723
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 138:
					// #line 396
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 139:
					// #line 406
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 140:
					// #line 718
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 141:
					// #line 743
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NS_C);
					}
					break;
					
				case 142:
					// #line 456
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 143:
					// #line 941
					{
					    this._tokenSemantics.Object = ProcessEscapedStringWithEnding(GetTokenString(), _encoding, false, '"');
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 144:
					// #line 911
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 145:
					// #line 902
					{
						//Z_LVAL_P(zendlval) = (zend_long) '{';
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 146:
					// #line 654
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 147:
					// #line 787
					{
						yyless(TokenLength - 1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 148:
					// #line 781
					{
						yyless(TokenLength - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 149:
					// #line 79
					{
						if(!string.IsNullOrEmpty(GetTokenString()))
						{
							_tokenSemantics.Object = GetTokenString(); 
							return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
						return Tokens.EOF;
					}
					break;
					
				case 150:
					// #line 849
					{ yymore(); break; }
					break;
					
				case 151:
					// #line 850
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 152:
					// #line 946
					{
					    this._tokenSemantics.Object = ProcessEscapedStringWithEnding(GetTokenString(), _encoding, false, '`');
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 153:
					// #line 916
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 154:
					// #line 953
					{
					    this._tokenSemantics.Object = ProcessEscapedString(GetTokenString(), _encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 155:
					// #line 951
					{ yymore(); break; }
					break;
					
				case 156:
					// #line 930
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => (string)ProcessEscapedString(s, _encoding, false)) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 157:
					// #line 316
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 158:
					// #line 321
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 159:
					// #line 312
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 160:
					// #line 677
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 161:
					// #line 668
					{
						yyless(TokenLength - 1);
						this._tokenSemantics.Object = GetTokenString();
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 162:
					// #line 93
					{
						if(!string.IsNullOrEmpty(GetTokenString()))
						{
							SetDocBlock(); 
							return Tokens.T_DOC_COMMENT; 
						}
						return Tokens.EOF;
					}
					break;
					
				case 163:
					// #line 830
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 832
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 831
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 166:
					// #line 88
					{ 
						if(!string.IsNullOrEmpty(GetTokenString()))
							return Tokens.T_COMMENT; 
						return Tokens.EOF;
					}
					break;
					
				case 167:
					// #line 825
					{ yymore(); break; }
					break;
					
				case 168:
					// #line 827
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 826
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 170:
					// #line 966
					{ yymore(); break; }
					break;
					
				case 171:
					// #line 967
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 172:
					// #line 965
					{ yymore(); break; }
					break;
					
				case 173:
					// #line 969
					{ yymore(); break; }
					break;
					
				case 174:
					// #line 807
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 175:
					// #line 802
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 176:
					// #line 696
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 177:
					// #line 797
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 178:
					// #line 700
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 179:
					// #line 895
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						this._tokenSemantics.Object = this._hereDocLabel;
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 180:
					// #line 939
					{ yymore(); break; }
					break;
					
				case 181:
					// #line 921
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => s) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 182:
					// #line 468
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 183:
					// #line 462
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 184:
					// #line 441
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 185:
					// #line 465
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 186:
					// #line 466
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 187:
					// #line 464
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 188:
					// #line 446
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 189:
					// #line 451
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 192: goto case 3;
				case 193: goto case 6;
				case 194: goto case 7;
				case 195: goto case 9;
				case 196: goto case 10;
				case 197: goto case 28;
				case 198: goto case 31;
				case 199: goto case 41;
				case 200: goto case 50;
				case 201: goto case 95;
				case 202: goto case 143;
				case 203: goto case 145;
				case 204: goto case 146;
				case 205: goto case 152;
				case 206: goto case 154;
				case 207: goto case 155;
				case 208: goto case 156;
				case 209: goto case 158;
				case 210: goto case 160;
				case 211: goto case 171;
				case 212: goto case 172;
				case 213: goto case 175;
				case 214: goto case 176;
				case 215: goto case 178;
				case 216: goto case 181;
				case 217: goto case 182;
				case 219: goto case 7;
				case 220: goto case 9;
				case 221: goto case 31;
				case 222: goto case 143;
				case 223: goto case 145;
				case 224: goto case 146;
				case 225: goto case 152;
				case 226: goto case 154;
				case 227: goto case 155;
				case 228: goto case 178;
				case 230: goto case 7;
				case 231: goto case 9;
				case 232: goto case 143;
				case 233: goto case 152;
				case 234: goto case 154;
				case 235: goto case 155;
				case 237: goto case 7;
				case 238: goto case 9;
				case 239: goto case 154;
				case 241: goto case 7;
				case 242: goto case 9;
				case 243: goto case 154;
				case 245: goto case 7;
				case 246: goto case 9;
				case 247: goto case 154;
				case 249: goto case 7;
				case 250: goto case 9;
				case 251: goto case 154;
				case 253: goto case 7;
				case 254: goto case 9;
				case 255: goto case 154;
				case 257: goto case 7;
				case 258: goto case 9;
				case 259: goto case 154;
				case 261: goto case 7;
				case 262: goto case 9;
				case 263: goto case 154;
				case 265: goto case 7;
				case 266: goto case 9;
				case 267: goto case 154;
				case 269: goto case 7;
				case 270: goto case 9;
				case 271: goto case 154;
				case 273: goto case 7;
				case 274: goto case 9;
				case 275: goto case 154;
				case 277: goto case 7;
				case 278: goto case 9;
				case 279: goto case 154;
				case 281: goto case 7;
				case 282: goto case 9;
				case 284: goto case 7;
				case 285: goto case 9;
				case 287: goto case 7;
				case 288: goto case 9;
				case 290: goto case 7;
				case 292: goto case 7;
				case 294: goto case 7;
				case 296: goto case 7;
				case 298: goto case 7;
				case 300: goto case 7;
				case 302: goto case 7;
				case 304: goto case 7;
				case 306: goto case 7;
				case 308: goto case 7;
				case 310: goto case 7;
				case 312: goto case 7;
				case 314: goto case 7;
				case 316: goto case 7;
				case 318: goto case 7;
				case 320: goto case 7;
				case 322: goto case 7;
				case 324: goto case 7;
				case 326: goto case 7;
				case 328: goto case 7;
				case 330: goto case 7;
				case 332: goto case 7;
				case 334: goto case 7;
				case 336: goto case 7;
				case 338: goto case 7;
				case 340: goto case 7;
				case 342: goto case 7;
				case 344: goto case 7;
				case 346: goto case 7;
				case 348: goto case 7;
				case 350: goto case 7;
				case 352: goto case 7;
				case 354: goto case 7;
				case 356: goto case 7;
				case 358: goto case 7;
				case 360: goto case 7;
				case 362: goto case 7;
				case 364: goto case 7;
				case 366: goto case 7;
				case 368: goto case 7;
				case 370: goto case 7;
				case 372: goto case 7;
				case 374: goto case 7;
				case 376: goto case 7;
				case 378: goto case 7;
				case 380: goto case 7;
				case 382: goto case 7;
				case 384: goto case 7;
				case 386: goto case 7;
				case 388: goto case 7;
				case 390: goto case 7;
				case 392: goto case 7;
				case 394: goto case 7;
				case 396: goto case 7;
				case 398: goto case 7;
				case 400: goto case 7;
				case 402: goto case 7;
				case 404: goto case 7;
				case 406: goto case 7;
				case 445: goto case 7;
				case 457: goto case 7;
				case 460: goto case 7;
				case 462: goto case 7;
				case 464: goto case 7;
				case 466: goto case 7;
				case 467: goto case 7;
				case 468: goto case 7;
				case 469: goto case 7;
				case 470: goto case 7;
				case 471: goto case 7;
				case 472: goto case 7;
				case 473: goto case 7;
				case 474: goto case 7;
				case 475: goto case 7;
				case 476: goto case 7;
				case 477: goto case 7;
				case 478: goto case 7;
				case 479: goto case 7;
				case 480: goto case 7;
				case 481: goto case 7;
				case 482: goto case 7;
				case 483: goto case 7;
				case 484: goto case 7;
				case 485: goto case 7;
				case 486: goto case 7;
				case 487: goto case 7;
				case 488: goto case 7;
				case 489: goto case 7;
				case 490: goto case 7;
				case 491: goto case 7;
				case 492: goto case 7;
				case 493: goto case 7;
				case 494: goto case 7;
				case 495: goto case 7;
				case 496: goto case 7;
				case 497: goto case 7;
				case 498: goto case 7;
				case 499: goto case 7;
				case 500: goto case 7;
				case 501: goto case 7;
				case 502: goto case 7;
				case 503: goto case 7;
				case 504: goto case 7;
				case 505: goto case 7;
				case 506: goto case 7;
				case 507: goto case 7;
				case 508: goto case 7;
				case 509: goto case 7;
				case 510: goto case 7;
				case 511: goto case 7;
				case 512: goto case 7;
				case 513: goto case 7;
				case 514: goto case 7;
				case 515: goto case 7;
				case 516: goto case 7;
				case 517: goto case 7;
				case 518: goto case 7;
				case 519: goto case 7;
				case 520: goto case 7;
				case 521: goto case 7;
				case 522: goto case 7;
				case 523: goto case 7;
				case 524: goto case 7;
				case 525: goto case 7;
				case 526: goto case 7;
				case 527: goto case 7;
				case 528: goto case 7;
				case 529: goto case 7;
				case 530: goto case 7;
				case 531: goto case 7;
				case 532: goto case 7;
				case 533: goto case 7;
				case 534: goto case 7;
				case 535: goto case 7;
				case 536: goto case 7;
				case 537: goto case 7;
				case 538: goto case 7;
				case 539: goto case 7;
				case 540: goto case 7;
				case 541: goto case 7;
				case 542: goto case 7;
				case 543: goto case 7;
				case 544: goto case 7;
				case 545: goto case 7;
				case 546: goto case 7;
				case 547: goto case 7;
				case 548: goto case 7;
				case 549: goto case 7;
				case 550: goto case 7;
				case 551: goto case 7;
				case 552: goto case 7;
				case 553: goto case 7;
				case 554: goto case 7;
				case 555: goto case 7;
				case 556: goto case 7;
				case 557: goto case 7;
				case 558: goto case 7;
				case 559: goto case 7;
				case 560: goto case 7;
				case 561: goto case 7;
				case 562: goto case 7;
				case 563: goto case 7;
				case 564: goto case 7;
				case 565: goto case 7;
				case 566: goto case 7;
				case 567: goto case 7;
				case 568: goto case 7;
				case 569: goto case 7;
				case 570: goto case 7;
				case 571: goto case 7;
				case 572: goto case 7;
				case 573: goto case 7;
				case 574: goto case 7;
				case 575: goto case 7;
				case 576: goto case 7;
				case 577: goto case 7;
				case 578: goto case 7;
				case 579: goto case 7;
				case 580: goto case 7;
				case 581: goto case 7;
				case 582: goto case 7;
				case 583: goto case 7;
				case 584: goto case 7;
				case 585: goto case 7;
				case 586: goto case 7;
				case 587: goto case 7;
				case 588: goto case 7;
				case 589: goto case 7;
				case 590: goto case 7;
				case 591: goto case 7;
				case 592: goto case 7;
				case 593: goto case 7;
				case 594: goto case 7;
				case 595: goto case 7;
				case 596: goto case 7;
				case 597: goto case 7;
				case 598: goto case 7;
				case 599: goto case 7;
				case 600: goto case 7;
				case 601: goto case 7;
				case 602: goto case 7;
				case 603: goto case 7;
				case 604: goto case 7;
				case 605: goto case 7;
				case 606: goto case 7;
				case 607: goto case 7;
				case 608: goto case 7;
				case 609: goto case 7;
				case 610: goto case 7;
				case 611: goto case 7;
				case 612: goto case 7;
				case 613: goto case 7;
				case 614: goto case 7;
				case 615: goto case 7;
				case 616: goto case 7;
				case 617: goto case 7;
				case 618: goto case 7;
				case 619: goto case 7;
				case 620: goto case 7;
				case 621: goto case 7;
				case 622: goto case 7;
				case 623: goto case 7;
				case 624: goto case 7;
				case 625: goto case 7;
				case 626: goto case 7;
				case 627: goto case 7;
				case 628: goto case 7;
				case 629: goto case 7;
				case 630: goto case 7;
				case 631: goto case 7;
				case 632: goto case 7;
				case 633: goto case 7;
				case 634: goto case 7;
				case 635: goto case 7;
				case 636: goto case 7;
				case 637: goto case 7;
				case 638: goto case 7;
				case 639: goto case 7;
				case 640: goto case 7;
				case 641: goto case 7;
				case 642: goto case 7;
				case 643: goto case 7;
				case 644: goto case 7;
				case 645: goto case 7;
				case 646: goto case 7;
				case 647: goto case 7;
				case 648: goto case 7;
				case 649: goto case 7;
				case 650: goto case 7;
				case 651: goto case 7;
				case 652: goto case 7;
				case 653: goto case 7;
				case 654: goto case 7;
				case 655: goto case 7;
				case 656: goto case 7;
				case 657: goto case 7;
				case 658: goto case 7;
				case 659: goto case 7;
				case 660: goto case 7;
				case 661: goto case 7;
				case 662: goto case 7;
				case 663: goto case 7;
				case 664: goto case 7;
				case 665: goto case 7;
				case 666: goto case 7;
				case 667: goto case 7;
				case 668: goto case 7;
				case 669: goto case 7;
				case 670: goto case 7;
				case 671: goto case 7;
				case 672: goto case 7;
				case 673: goto case 7;
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
			AcceptConditions.AcceptOnStart, // 156
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
			AcceptConditions.AcceptOnStart, // 181
			AcceptConditions.Accept, // 182
			AcceptConditions.Accept, // 183
			AcceptConditions.Accept, // 184
			AcceptConditions.Accept, // 185
			AcceptConditions.Accept, // 186
			AcceptConditions.Accept, // 187
			AcceptConditions.Accept, // 188
			AcceptConditions.Accept, // 189
			AcceptConditions.NotAccept, // 190
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
			AcceptConditions.AcceptOnStart, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.Accept, // 210
			AcceptConditions.Accept, // 211
			AcceptConditions.Accept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.Accept, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.AcceptOnStart, // 216
			AcceptConditions.Accept, // 217
			AcceptConditions.NotAccept, // 218
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
			AcceptConditions.NotAccept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.Accept, // 234
			AcceptConditions.Accept, // 235
			AcceptConditions.NotAccept, // 236
			AcceptConditions.Accept, // 237
			AcceptConditions.Accept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.NotAccept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.Accept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.NotAccept, // 244
			AcceptConditions.Accept, // 245
			AcceptConditions.Accept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.NotAccept, // 248
			AcceptConditions.Accept, // 249
			AcceptConditions.Accept, // 250
			AcceptConditions.Accept, // 251
			AcceptConditions.NotAccept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.Accept, // 254
			AcceptConditions.Accept, // 255
			AcceptConditions.NotAccept, // 256
			AcceptConditions.Accept, // 257
			AcceptConditions.Accept, // 258
			AcceptConditions.Accept, // 259
			AcceptConditions.NotAccept, // 260
			AcceptConditions.Accept, // 261
			AcceptConditions.Accept, // 262
			AcceptConditions.Accept, // 263
			AcceptConditions.NotAccept, // 264
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
			AcceptConditions.Accept, // 275
			AcceptConditions.NotAccept, // 276
			AcceptConditions.Accept, // 277
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
			AcceptConditions.NotAccept, // 443
			AcceptConditions.NotAccept, // 444
			AcceptConditions.Accept, // 445
			AcceptConditions.Accept, // 446
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
			AcceptConditions.Accept, // 457
			AcceptConditions.NotAccept, // 458
			AcceptConditions.NotAccept, // 459
			AcceptConditions.Accept, // 460
			AcceptConditions.NotAccept, // 461
			AcceptConditions.Accept, // 462
			AcceptConditions.NotAccept, // 463
			AcceptConditions.Accept, // 464
			AcceptConditions.NotAccept, // 465
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
			AcceptConditions.Accept, // 673
		};
		
		private static int[] colMap = new int[]
		{
			29, 29, 29, 29, 29, 29, 29, 29, 29, 36, 16, 29, 29, 60, 29, 29, 
			29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 
			36, 47, 62, 44, 55, 49, 50, 63, 35, 37, 43, 46, 53, 25, 32, 42, 
			58, 59, 28, 28, 28, 28, 28, 28, 28, 28, 30, 41, 48, 45, 26, 33, 
			53, 18, 21, 10, 6, 2, 7, 23, 19, 4, 38, 22, 15, 17, 9, 11, 
			24, 40, 13, 12, 5, 8, 34, 20, 3, 14, 27, 57, 31, 61, 52, 39, 
			64, 18, 21, 10, 6, 2, 7, 23, 19, 4, 38, 22, 15, 17, 9, 11, 
			24, 40, 13, 12, 5, 8, 34, 20, 3, 14, 27, 54, 51, 56, 53, 29, 
			0, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 2, 3, 1, 1, 4, 5, 6, 7, 1, 1, 1, 1, 1, 
			8, 9, 1, 10, 10, 10, 10, 1, 1, 1, 11, 1, 12, 1, 1, 13, 
			1, 14, 1, 15, 1, 1, 16, 1, 1, 17, 18, 19, 1, 1, 1, 1, 
			1, 1, 20, 1, 1, 10, 10, 10, 21, 10, 10, 10, 1, 1, 10, 1, 
			1, 1, 1, 1, 22, 23, 10, 10, 24, 10, 10, 10, 10, 25, 10, 10, 
			10, 10, 10, 26, 10, 10, 10, 10, 10, 27, 10, 10, 10, 10, 1, 1, 
			28, 10, 10, 10, 10, 10, 10, 1, 1, 10, 29, 10, 10, 10, 10, 30, 
			10, 1, 1, 10, 10, 10, 10, 10, 10, 1, 1, 10, 10, 10, 10, 10, 
			10, 10, 10, 10, 10, 10, 10, 10, 1, 10, 10, 10, 10, 10, 10, 31, 
			1, 32, 32, 1, 1, 1, 33, 1, 34, 1, 35, 1, 1, 36, 37, 1, 
			38, 1, 1, 39, 40, 1, 1, 41, 42, 1, 43, 1, 1, 1, 1, 1, 
			44, 1, 45, 1, 1, 1, 1, 46, 1, 1, 47, 48, 1, 1, 49, 50, 
			51, 52, 53, 54, 55, 56, 1, 1, 57, 58, 59, 60, 60, 61, 62, 63, 
			64, 1, 1, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 1, 76, 
			76, 1, 1, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 
			90, 91, 92, 93, 94, 95, 1, 96, 97, 98, 99, 100, 101, 102, 103, 104, 
			105, 106, 107, 108, 109, 110, 111, 77, 112, 113, 114, 115, 116, 117, 118, 119, 
			120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 92, 134, 
			67, 22, 135, 23, 136, 8, 137, 138, 139, 140, 141, 142, 143, 9, 144, 145, 
			146, 56, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 25, 
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 
			176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 
			192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 
			208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 
			224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 
			240, 241, 242, 32, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 60, 
			254, 255, 256, 257, 258, 259, 260, 76, 261, 262, 263, 264, 265, 266, 38, 267, 
			268, 269, 270, 69, 78, 271, 272, 273, 274, 275, 48, 276, 277, 278, 279, 280, 
			281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 
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
			489, 490, 491, 492, 493, 494, 10, 495, 496, 497, 498, 499, 500, 501, 502, 503, 
			504, 505
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 192, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 190, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 218, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 467, 662, 662, 662, 662, 662, 468, 469, 662, 662, 662, 662, 470, -1, 471, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 472, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 23, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 295, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 297, 299, 293, 293, 293, 293, 293, 293, 51, 293, 293 },
			{ -1, -1, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 303, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 52, 301 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, 28, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 221, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, -1, 50, 50, 50, 50, 50, 50, 50, 50, -1, -1, 50, 50, -1, -1, -1, -1, -1, 50, -1, -1, -1, 50, 50, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, 50, -1, -1, -1, -1, -1 },
			{ -1, -1, 649, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 68, -1, -1, -1, 68, 68, -1, -1, 68, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, 68, -1, -1, -1, -1, -1, -1, 68, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, 68, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, 69, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 330, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 77, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 77, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 77, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 350, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 361, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, 361, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, 361, -1, -1, -1, -1 },
			{ -1, -1, 653, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 578, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 670, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ 1, 2, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 393, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 395, 397, 202, 202, 202, 202, 202, 202, 144, 202, 202 },
			{ -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 409, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, -1, 150 },
			{ 1, 2, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 410, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 411, 412, 205, 205, 205, 205, 205, 205, 205, 205, 153 },
			{ 191, 149, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 155, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 416, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 417, 418, 206, 206, 206, 206, 207, 206, 206, 206, 206 },
			{ -1, -1, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, -1, 157, 157, 157, 157, 157, 157, 157, 157, -1, -1, 157, 157, -1, -1, -1, -1, -1, 157, -1, -1, -1, 157, 157, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, 157, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 159, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, -1, 430, 430, 430, 430, 430, 430, 430, 430, -1, -1, 430, 430, -1, -1, -1, -1, -1, 430, -1, -1, -1, 430, 430, 430, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 161, 161, 430, 430, -1, -1, -1, -1, -1 },
			{ -1, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, -1, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, -1, 170, 170, 170, 170 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, 176, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, 178, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 442, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1 },
			{ -1, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, -1, 419, 419, 419, 419, 419, 419, 419, 419, -1, -1, 419, -1, -1, -1, -1, -1, -1, 419, -1, -1, -1, 419, 419, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 19, 662, 473, 662, 662, 474, 662, 662, 662, -1, 596, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 248, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1 },
			{ -1, -1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, -1, 200, 200, 200, 200, 200, 200, 200, 200, 405, -1, 200, 200, -1, -1, -1, -1, -1, 200, -1, -1, -1, 200, 200, 200, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 147, 200, 200, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 95, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 393, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 399, 401, 202, 202, 202, 202, 202, 202, -1, 202, 202 },
			{ -1, 225, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 410, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 413, 414, 205, 205, 205, 205, 205, 205, 205, 205, -1 },
			{ -1, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 155, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 416, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 420, 421, 206, 206, 206, 206, 207, 206, 206, 206, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 156, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 173, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, -1, 50, 50, 50, 50, 50, 50, 50, 50, -1, -1, 50, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, 50, 50, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 435, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, 178, -1, -1, -1, -1, -1 },
			{ -1, -1, 215, -1, -1, -1, 215, 215, -1, -1, 215, -1, -1, -1, -1, -1, -1, -1, 215, -1, -1, 215, -1, -1, -1, -1, -1, -1, 215, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 215, 215, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 229, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 476, 662, 253, 662, 662, 662, 662, 662, 662, 20, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 424, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 425, 426, 263, 263, 263, 263, -1, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, 228, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 21, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, 28, -1, -1, -1, -1, -1 },
			{ -1, 222, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 393, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 399, 401, 202, 202, 202, 202, 202, 202, -1, 202, 202 },
			{ -1, 225, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 410, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 413, 414, 205, 205, 205, 205, 205, 205, 205, 205, -1 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 243, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 247, 251, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ -1, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 155, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 424, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 425, 426, 263, 263, 263, 263, -1, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 269, 662, 662, 22, 598, 662, 662, -1, 662, 662, 662, 662, 625, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 155, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 416, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 420, 421, 206, 206, 206, 206, 207, 206, 206, 206, 206 },
			{ 1, 2, 7, 445, 194, 457, 219, 460, 462, 464, 595, 230, 624, 643, 654, 660, 8, 662, 237, 662, 664, 241, 662, 665, 666, 9, 195, 662, 10, 11, 220, 12, 231, 238, 466, 242, 8, 246, 662, 667, 662, 246, 250, 254, 13, 258, 262, 266, 270, 274, 278, 282, 285, 246, 14, 288, 15, 246, 196, 10, 8, 246, 16, 17, 18 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 602, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, 244, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, 16, 17, -1 },
			{ -1, -1, -1, -1, 256, -1, 260, 264, 448, -1, -1, 268, 272, 276, -1, -1, -1, -1, 280, -1, -1, 283, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 286, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 227, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 234, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 255, 259, 206, 206, 206, 206, 235, 206, 206, 206, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 447, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 53, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 227, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 234, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 255, 421, 206, 206, 206, 206, 235, 206, 206, 206, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 54, 662, -1, 662, 496, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, 33, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 227, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 234, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 420, 259, 206, 206, 206, 206, 235, 206, 206, 206, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 55, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 35, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 234, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 247, 426, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 449, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 56, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 206, 263, 263, 263, 263, 263, 263, 263, 263, 206, 206, 263, 206, 206, 206, 234, 206, 206, 263, 206, 206, 206, 263, 263, 263, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 425, 251, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 57, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 58, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 267, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 275, 279, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 59, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 424, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 425, 426, 263, 263, 263, 263, -1, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, 450, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 62, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 267, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 275, 426, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 70, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 226, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 267, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 425, 279, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 458, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 71, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, 451, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 72, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 73, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 74, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 75, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 76, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, -1, 293, 293, 293, 293 },
			{ -1, -1, 662, 662, 662, 662, 662, 78, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 295, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, -1, 293, 293, 293, 293, 293, 293, 51, 293, 293 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 79, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, 293, 293, -1, 293, 293, 293, 295, 293, 293, -1, 293, 293, 293, -1, -1, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, -1, 293, 293, 293, 293, 293, 293, 293, 51, 293, 293 },
			{ -1, -1, 662, 662, 662, 80, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 81, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 82, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 83, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 84, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 452, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 85, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 86, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 453, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 87, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 88, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 89, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 90, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, -1, 337, 337, 337, 337, 337, 337, 337, 337, -1, -1, 337, -1, -1, -1, -1, -1, -1, 337, -1, 321, -1, 337, 337, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, 455, -1 },
			{ -1, -1, 91, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 92, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 459, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 93, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 456, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 96, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 97, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 98, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 99, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 461, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 100, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 95, 337, 337, 337, 337, 337, 337, 337, 337, -1, -1, 337, 337, -1, -1, -1, -1, -1, 337, -1, -1, -1, 337, 337, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, 337, 201, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 101, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, -1, 357, 357, 357, 357, 357, 357, 357, 357, -1, -1, 357, -1, -1, -1, -1, -1, -1, 357, -1, -1, -1, 357, 357, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 102, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 105, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 106, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 107, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 369, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 108, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 109, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 110, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 111, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 112, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, -1, 357, 357, 357, 357, 357, 357, 357, 357, -1, -1, 357, 357, -1, -1, -1, -1, -1, 357, -1, -1, -1, 357, 357, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 357, 357, -1, -1, 381, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 115, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, -1, 359, 359, 359, 359, 359, 359, 359, 359, -1, -1, 359, 359, -1, -1, -1, -1, -1, 359, -1, -1, -1, 359, 359, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 359, 359, -1, -1, -1, 381, -1 },
			{ -1, -1, 116, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 463, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 117, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 465, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 118, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 119, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, 113, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 120, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 383, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 123, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 124, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, 114, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 125, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 126, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 387, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 127, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 128, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 95, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 129, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 383, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 130, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, 122, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 131, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 132, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 391, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 133, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 136, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 134, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, 222, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 135, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, 222, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 393, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 232, 145, 202, 202, 202, 202, 202, 202, 222, 202, 202 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 137, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, 222, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 202, 200, 200, 200, 200, 200, 200, 200, 200, 202, 202, 200, 202, 202, 202, 393, 202, 202, 200, 202, 202, 202, 200, 200, 200, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 146, 232, 202, 202, 202, 202, 202, 202, 222, 202, 202 },
			{ -1, -1, 138, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, 222, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 393, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 232, 403, 202, 202, 202, 202, 202, 202, 222, 202, 202 },
			{ -1, -1, 139, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, 222, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 202, -1, -1, -1, -1, -1, -1, -1, -1, 202, 202, -1, 202, 202, 202, 393, 202, 202, -1, 202, 202, 202, -1, -1, -1, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 202, 403, 232, 202, 202, 202, 202, 202, 202, 222, 202, 202 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 140, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 141, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 407, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 142, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1, 148, 148, 148, 148, 148, 148, 148, 148, -1, -1, 148, -1, -1, -1, -1, -1, -1, 148, -1, -1, -1, 148, 148, 148, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 149, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 409, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 151, 150 },
			{ -1, -1, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150 },
			{ -1, 225, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, 225, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 410, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 233, 203, 205, 205, 205, 205, 205, 205, 205, 205, 225 },
			{ -1, 225, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 205, 200, 200, 200, 200, 200, 200, 200, 200, 205, 205, 200, 205, 205, 205, 410, 205, 205, 200, 205, 205, 205, 200, 200, 200, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 204, 233, 205, 205, 205, 205, 205, 205, 205, 205, 225 },
			{ -1, 225, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 410, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 233, 415, 205, 205, 205, 205, 205, 205, 205, 205, 225 },
			{ -1, 225, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1, -1, -1, -1, -1, -1, -1, -1, 205, 205, -1, 205, 205, 205, 410, 205, 205, -1, 205, 205, 205, -1, -1, -1, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 415, 233, 205, 205, 205, 205, 205, 205, 205, 205, 225 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 234, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 239, 223, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ -1, 226, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 206, 200, 200, 200, 200, 200, 200, 200, 200, 206, 206, 200, 206, 206, 206, 234, 206, 206, 200, 206, 206, 206, 200, 200, 200, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 224, 239, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ -1, -1, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 156, 419, 419, 419, 419, 419, 419, 419, 419, -1, -1, 419, 419, -1, -1, -1, -1, -1, 419, -1, -1, -1, 419, 419, 419, 422, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 419, 419, 208, -1, -1, -1, -1 },
			{ -1, 226, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 234, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 239, 423, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ -1, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 206, -1, -1, -1, -1, -1, -1, -1, -1, 206, 206, -1, 206, 206, 206, 234, 206, 206, -1, 206, 206, 206, -1, -1, -1, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 206, 423, 239, 206, 206, 206, 206, 206, 206, 206, 206, 206 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 156, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 208, -1, -1, -1, -1 },
			{ -1, 226, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, 226, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 267, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 271, 423, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, 263, 263, -1, 263, 263, 263, 267, 263, 263, -1, 263, 263, 263, -1, -1, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 423, 271, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ 1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 157, 8, 157, 157, 157, 157, 157, 157, 157, 157, 158, 209, 157, 209, 209, 209, 209, 209, 209, 157, 209, 8, 209, 157, 157, 157, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 8, 209, 209, 209, 209 },
			{ 1, 2, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 210, 160, 160, 160, 160, 160, 160, 160, 160, 210, 210, 160, 210, 210, 210, 210, 210, 210, 160, 210, 210, 210, 160, 160, 160, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210 },
			{ 1, 162, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 164, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163 },
			{ 1, 166, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 168, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167, 167 },
			{ 1, 2, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 171, 170, 170, 170, 170, 170, 170, 170, 170, 170, 172, 170, 170, 170, 170, 170, 170, 212, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 212, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 211, 170, 170, 170, 170 },
			{ 1, 2, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 174, 662, 662, 662, 662, 662, 662, 662, 662, 175, 175, 662, 176, 11, 175, 174, 175, 175, 662, 175, 174, 175, 662, 662, 662, 175, 175, 175, 174, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 213, 175, 175, 214, 176, 174, 177, 175, 174, 175 },
			{ 1, 2, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179 },
			{ 446, 149, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180 },
			{ -1, -1, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 181, 439, 439, 439, 439, 439, 439, 439, 439, -1, -1, 439, 439, -1, -1, -1, -1, -1, 439, -1, -1, -1, 439, 439, 439, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 439, 439, 216, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 216, -1, -1, -1, -1 },
			{ 1, 2, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 184, 183, 182, 182, 182, 182, 182, 217, 182, 185, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182 },
			{ 1, 2, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 188, 182, 182, 182, 182, 217, 182, 185, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182 },
			{ 1, 2, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 189, 217, 182, 185, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 245, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, 439, -1, 439, 439, 439, 439, 439, 439, 439, 439, -1, -1, 439, -1, -1, -1, -1, -1, -1, 439, -1, -1, -1, 439, 439, 439, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 454, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, -1, 359, 359, 359, 359, 359, 359, 359, 359, -1, -1, 359, -1, -1, -1, -1, -1, -1, 359, -1, -1, -1, 359, 359, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 249, 662, 662, -1, 662, 662, 475, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 477, 662, 662, 662, 599, 662, 662, 257, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 597, 662, 662, 261, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 389, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 265, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 478, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 273, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 277, 627, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 491, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 281, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 284, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 492, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 287, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 644, 662, 662, 662, 662, 493, 662, 494, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 495, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 497, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 600, 662, 662, 628, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 498, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 655, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 502, 662, 662, 662, 662, -1, 662, 503, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 504, 662, 662, 662, 662, 662, 662, 290, 662, 662, 672, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 605, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 629, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 505, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 606, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 506, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 292, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 294, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 603, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 645, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 510, 662, 662, 662, 662, 662, 662, 657, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 511, 512, 513, 662, 514, 656, 662, 662, 662, 662, 608, -1, 515, 662, 609, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 296, 662, 610, 517, 662, 662, 662, 662, 518, 662, 662, 662, -1, 662, 662, 662, 519, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 298, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 630, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 520, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 300, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 302, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 304, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 306, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 521, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 308, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 632, 662, 662, 662, 662, 662, 662, 310, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 312, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 314, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 316, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 525, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 318, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 320, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 322, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 324, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 326, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 661, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 663, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 528, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 529, 662, 662, 662, 611, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 530, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 612, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 532, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 328, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 534, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 616, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 614, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 537, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 637, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 541, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 332, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 334, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 336, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 338, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 340, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 545, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 546, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 618, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 547, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 342, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 550, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 620, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 552, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 344, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 554, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 346, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 348, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 352, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 621, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 557, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 354, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 356, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 358, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 559, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 639, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 561, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 562, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 641, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 360, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 564, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 565, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 566, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 362, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 364, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 366, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 368, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 370, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 372, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 574, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 575, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 374, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 376, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 378, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 579, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 580, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 380, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 382, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 384, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 673, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 581, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 386, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 582, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 622, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 388, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 390, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 583, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 392, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 394, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 584, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 396, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 586, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 589, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 590, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 398, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 400, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 402, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 591, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 592, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 404, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 593, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 594, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 406, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 626, 662, 662, 662, 479, -1, 662, 480, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 604, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 500, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 507, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 499, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 647, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 508, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 509, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 526, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 634, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 523, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 648, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 535, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 635, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 613, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 533, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 671, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 548, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 549, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 553, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 640, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 551, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 556, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 619, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 572, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 563, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 568, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 585, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 587, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 481, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 482, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 646, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 501, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 516, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 633, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 524, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 536, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 636, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 617, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 539, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 668, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 638, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 558, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 555, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 560, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 573, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 569, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 576, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 588, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 483, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 607, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 527, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 631, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 538, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 543, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 540, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 615, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 567, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 570, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 577, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 484, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 522, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 531, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 650, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 542, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 571, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 485, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 544, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 669, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 601, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 486, 662, 662, 662, 487, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 488, 662, 662, 662, 662, 489, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 490, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 651, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 652, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 623, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 659, 662, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 662, 658, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 },
			{ -1, -1, 662, 662, 662, 662, 662, 662, 662, 662, 662, 642, 662, 662, 662, 662, -1, 662, 662, 662, 662, 662, 662, 662, 662, -1, -1, 662, 662, -1, -1, -1, -1, -1, 662, -1, -1, -1, 662, 662, 662, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 662, 662, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  240,
			  143,
			  408,
			  152,
			  154,
			  427,
			  428,
			  429,
			  431,
			  432,
			  433,
			  434,
			  437,
			  438,
			  441,
			  443,
			  444
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 674);
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

