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
					// #line 75
					{
						return Tokens.EOF;
					}
					break;
					
				case 3:
					// #line 752
					{
					    this._tokenSemantics.Object = GetTokenString();
						return Tokens.T_INLINE_HTML; 
					}
					break;
					
				case 4:
					// #line 779
					{
						return Tokens.T_ERROR;
					}
					break;
					
				case 5:
					// #line 770
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
					// #line 757
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 7:
					// #line 763
					{
						//HANDLE_NEWLINE(yytext[yyleng-1]);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 8:
					// #line 647
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 817
					{
						return ProcessLabel();
					}
					break;
					
				case 10:
					// #line 311
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 11:
					// #line 692
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 12:
					// #line 962
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 13:
					// #line 335
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 14:
					// #line 821
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 15:
					// #line 652
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 16:
					// #line 664
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 17:
					// #line 859
					{
						BEGIN(LexicalStates.ST_DOUBLE_QUOTES);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 18:
					// #line 844
					{ 
						// Gets here only in the case of unterminated singly-quoted string. That leads usually to an error token,
						// however when the source code is parsed per-line (as in Visual Studio colorizer) it is important to remember
						// that we are in the singly-quoted string at the end of the line.
						BEGIN(LexicalStates.ST_SINGLE_QUOTES); 
						yymore(); 
						break; 
					}
					break;
					
				case 19:
					// #line 892
					{
						BEGIN(LexicalStates.ST_BACKQUOTE); 
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 20:
					// #line 343
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 21:
					// #line 837
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 22:
					// #line 161
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IF);
					}
					break;
					
				case 23:
					// #line 191
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DO);
					}
					break;
					
				case 24:
					// #line 627
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 25:
					// #line 231
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_AS);
					}
					break;
					
				case 26:
					// #line 535
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 27:
					// #line 306
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 28:
					// #line 571
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 29:
					// #line 643
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 30:
					// #line 563
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 31:
					// #line 708
					{
						return ProcessRealNumber();
					}
					break;
					
				case 32:
					// #line 331
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 33:
					// #line 591
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 34:
					// #line 827
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 35:
					// #line 587
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 36:
					// #line 579
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 37:
					// #line 575
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 38:
					// #line 512
					{
						return (Tokens.T_DOUBLE_ARROW);
					}
					break;
					
				case 39:
					// #line 547
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 40:
					// #line 567
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 41:
					// #line 531
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 42:
					// #line 551
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 43:
					// #line 559
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 44:
					// #line 639
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 45:
					// #line 595
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 46:
					// #line 607
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 47:
					// #line 623
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 48:
					// #line 611
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 49:
					// #line 619
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 50:
					// #line 615
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 51:
					// #line 796
					{
						return ProcessVariable();
					}
					break;
					
				case 52:
					// #line 855
					{
						return ProcessDoubleQuotedString();
					}
					break;
					
				case 53:
					// #line 842
					{ return ProcessSingleQuotedString(); }
					break;
					
				case 54:
					// #line 635
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 55:
					// #line 141
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRY);
					}
					break;
					
				case 56:
					// #line 111
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT);
					}
					break;
					
				case 57:
					// #line 196
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOR);
					}
					break;
					
				case 58:
					// #line 420
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_USE);
					}
					break;
					
				case 59:
					// #line 347
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NEW);
					}
					break;
					
				case 60:
					// #line 631
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 61:
					// #line 603
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 62:
					// #line 339
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 63:
					// #line 357
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_VAR);
					}
					break;
					
				case 64:
					// #line 583
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 65:
					// #line 539
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 66:
					// #line 543
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 67:
					// #line 555
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 68:
					// #line 599
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 69:
					// #line 696
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 70:
					// #line 688
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 71:
					// #line 106
					{ 
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 72:
					// #line 271
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ECHO);
					}
					break;
					
				case 73:
					// #line 176
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSE);
					}
					break;
					
				case 74:
					// #line 390
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EVAL);
					}
					break;
					
				case 75:
					// #line 246
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CASE);
					}
					break;
					
				case 76:
					// #line 516
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LIST);
					}
					break;
					
				case 77:
					// #line 266
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GOTO);
					}
					break;
					
				case 78:
					// #line 832
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 79:
					// #line 171
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 80:
					// #line 440
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 81:
					// #line 435
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ISSET);
					}
					break;
					
				case 82:
					// #line 291
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 83:
					// #line 156
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_THROW);
					}
					break;
					
				case 84:
					// #line 487
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINAL);
					}
					break;
					
				case 85:
					// #line 507
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_UNSET);
					}
					break;
					
				case 86:
					// #line 121
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONST);
					}
					break;
					
				case 87:
					// #line 352
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLONE);
					}
					break;
					
				case 88:
					// #line 281
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS);
					}
					break;
					
				case 89:
					// #line 146
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CATCH);
					}
					break;
					
				case 90:
					// #line 136
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_YIELD);
					}
					break;
					
				case 91:
					// #line 521
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 92:
					// #line 181
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_WHILE);
					}
					break;
					
				case 93:
					// #line 256
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_BREAK);
					}
					break;
					
				case 94:
					// #line 276
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRINT);
					}
					break;
					
				case 95:
					// #line 362
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 96:
					// #line 865
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
					
				case 97:
					// #line 201
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 98:
					// #line 166
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 99:
					// #line 477
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_STATIC);
					}
					break;
					
				case 100:
					// #line 236
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 101:
					// #line 126
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_RETURN);
					}
					break;
					
				case 102:
					// #line 430
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 103:
					// #line 502
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 104:
					// #line 366
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 105:
					// #line 382
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 106:
					// #line 296
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 107:
					// #line 395
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 108:
					// #line 251
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 109:
					// #line 216
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 110:
					// #line 151
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 111:
					// #line 206
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 112:
					// #line 405
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 113:
					// #line 492
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 114:
					// #line 386
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 115:
					// #line 374
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 116:
					// #line 742
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DIR);
					}
					break;
					
				case 117:
					// #line 186
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 118:
					// #line 116
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 119:
					// #line 261
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 120:
					// #line 526
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 121:
					// #line 482
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 122:
					// #line 378
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 123:
					// #line 370
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 124:
					// #line 737
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FILE);
					}
					break;
					
				case 125:
					// #line 732
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LINE);
					}
					break;
					
				case 126:
					// #line 241
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 127:
					// #line 286
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 128:
					// #line 425
					{
						this._tokenSemantics.Object = GetTokenString();
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 129:
					// #line 415
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 130:
					// #line 497
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 131:
					// #line 717
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 132:
					// #line 712
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 133:
					// #line 221
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 134:
					// #line 211
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 135:
					// #line 226
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 136:
					// #line 301
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 137:
					// #line 131
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 138:
					// #line 727
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 139:
					// #line 400
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 140:
					// #line 410
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 141:
					// #line 722
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 142:
					// #line 747
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NS_C);
					}
					break;
					
				case 143:
					// #line 460
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 144:
					// #line 944
					{
					    this._tokenSemantics.Object = ProcessEscapedStringWithEnding(GetTokenString(), _encoding, false, '"');
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 145:
					// #line 914
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 146:
					// #line 905
					{
						//Z_LVAL_P(zendlval) = (zend_long) '{';
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 147:
					// #line 658
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 148:
					// #line 790
					{
						yyless(TokenLength - 1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 149:
					// #line 784
					{
						yyless(TokenLength - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 150:
					// #line 78
					{
						if(!string.IsNullOrEmpty(GetTokenString()))
						{
							_tokenSemantics.Object = GetTokenString(); 
							return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
						return Tokens.EOF;
					}
					break;
					
				case 151:
					// #line 852
					{ yymore(); break; }
					break;
					
				case 152:
					// #line 853
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 153:
					// #line 949
					{
					    this._tokenSemantics.Object = ProcessEscapedStringWithEnding(GetTokenString(), _encoding, false, '`');
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 154:
					// #line 919
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 155:
					// #line 956
					{
					    this._tokenSemantics.Object = ProcessEscapedString(GetTokenString(), _encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 156:
					// #line 954
					{ yymore(); break; }
					break;
					
				case 157:
					// #line 933
					{
					    if(!string.IsNullOrEmpty(this._hereDocLabel) && GetTokenString().Contains(this._hereDocLabel))
						{
							BEGIN(LexicalStates.ST_END_HEREDOC); 
							if( ProcessEndNowDoc(s => (string)ProcessEscapedString(s, _encoding, false)) ) return (Tokens.T_ENCAPSED_AND_WHITESPACE);
						}
					    yymore(); break;
					}
					break;
					
				case 158:
					// #line 325
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 159:
					// #line 320
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 160:
					// #line 316
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 161:
					// #line 681
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 162:
					// #line 672
					{
						yyless(TokenLength - 1);
						this._tokenSemantics.Object = GetTokenString();
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 163:
					// #line 97
					{
						if(!string.IsNullOrEmpty(GetTokenString()))
						{
							SetDocBlock(); 
							return Tokens.T_DOC_COMMENT; 
						}
						return Tokens.EOF;
					}
					break;
					
				case 164:
					// #line 833
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 835
					{ yymore(); break; }
					break;
					
				case 166:
					// #line 834
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 167:
					// #line 87
					{ 
						if(!string.IsNullOrEmpty(GetTokenString()))
							return Tokens.T_COMMENT; 
						return Tokens.EOF;
					}
					break;
					
				case 168:
					// #line 828
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 830
					{ yymore(); break; }
					break;
					
				case 170:
					// #line 829
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 171:
					// #line 973
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 172:
					// #line 92
					{ 
						if(!string.IsNullOrEmpty(GetTokenString()))
							return Tokens.T_COMMENT; 
						return Tokens.EOF;
					}
					break;
					
				case 173:
					// #line 968
					{ 
						yy_pop_state(); 
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);
						}
					break;
					
				case 174:
					// #line 805
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 175:
					// #line 810
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 176:
					// #line 700
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 177:
					// #line 800
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 178:
					// #line 704
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 179:
					// #line 898
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						this._tokenSemantics.Object = this._hereDocLabel;
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 180:
					// #line 942
					{ yymore(); break; }
					break;
					
				case 181:
					// #line 924
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
					// #line 472
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 183:
					// #line 466
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 184:
					// #line 445
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 185:
					// #line 469
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 186:
					// #line 470
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 187:
					// #line 468
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 188:
					// #line 450
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 189:
					// #line 455
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 192: goto case 7;
				case 193: goto case 8;
				case 194: goto case 9;
				case 195: goto case 11;
				case 196: goto case 21;
				case 197: goto case 31;
				case 198: goto case 42;
				case 199: goto case 51;
				case 200: goto case 96;
				case 201: goto case 144;
				case 202: goto case 146;
				case 203: goto case 147;
				case 204: goto case 153;
				case 205: goto case 155;
				case 206: goto case 156;
				case 207: goto case 157;
				case 208: goto case 158;
				case 209: goto case 161;
				case 210: goto case 171;
				case 211: goto case 174;
				case 212: goto case 176;
				case 213: goto case 178;
				case 214: goto case 181;
				case 215: goto case 182;
				case 217: goto case 8;
				case 218: goto case 9;
				case 219: goto case 21;
				case 220: goto case 144;
				case 221: goto case 146;
				case 222: goto case 147;
				case 223: goto case 153;
				case 224: goto case 155;
				case 225: goto case 156;
				case 226: goto case 171;
				case 227: goto case 178;
				case 229: goto case 8;
				case 230: goto case 9;
				case 231: goto case 144;
				case 232: goto case 153;
				case 233: goto case 155;
				case 234: goto case 156;
				case 235: goto case 171;
				case 237: goto case 8;
				case 238: goto case 9;
				case 239: goto case 155;
				case 241: goto case 8;
				case 242: goto case 9;
				case 243: goto case 155;
				case 245: goto case 8;
				case 246: goto case 9;
				case 247: goto case 155;
				case 249: goto case 8;
				case 250: goto case 9;
				case 251: goto case 155;
				case 253: goto case 8;
				case 254: goto case 9;
				case 255: goto case 155;
				case 257: goto case 8;
				case 258: goto case 9;
				case 259: goto case 155;
				case 261: goto case 8;
				case 262: goto case 9;
				case 263: goto case 155;
				case 265: goto case 8;
				case 266: goto case 9;
				case 267: goto case 155;
				case 269: goto case 8;
				case 270: goto case 9;
				case 271: goto case 155;
				case 273: goto case 8;
				case 274: goto case 9;
				case 275: goto case 155;
				case 277: goto case 8;
				case 278: goto case 9;
				case 279: goto case 155;
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
				case 446: goto case 9;
				case 458: goto case 9;
				case 461: goto case 9;
				case 463: goto case 9;
				case 465: goto case 9;
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
				case 673: goto case 9;
				case 674: goto case 9;
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
			AcceptConditions.AcceptOnStart, // 157
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
			AcceptConditions.AcceptOnStart, // 207
			AcceptConditions.Accept, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.Accept, // 210
			AcceptConditions.Accept, // 211
			AcceptConditions.Accept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.AcceptOnStart, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.NotAccept, // 216
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
			AcceptConditions.NotAccept, // 445
			AcceptConditions.Accept, // 446
			AcceptConditions.Accept, // 447
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
			AcceptConditions.Accept, // 458
			AcceptConditions.NotAccept, // 459
			AcceptConditions.NotAccept, // 460
			AcceptConditions.Accept, // 461
			AcceptConditions.NotAccept, // 462
			AcceptConditions.Accept, // 463
			AcceptConditions.NotAccept, // 464
			AcceptConditions.Accept, // 465
			AcceptConditions.NotAccept, // 466
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
			AcceptConditions.Accept, // 674
		};
		
		private static int[] colMap = new int[]
		{
			30, 30, 30, 30, 30, 30, 30, 30, 30, 36, 17, 30, 30, 60, 30, 30, 
			30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 
			36, 47, 62, 44, 55, 49, 50, 63, 35, 37, 43, 46, 53, 26, 33, 42, 
			58, 59, 29, 29, 29, 29, 29, 29, 29, 29, 31, 41, 48, 45, 27, 2, 
			53, 19, 22, 11, 7, 3, 8, 24, 20, 5, 38, 23, 16, 18, 10, 12, 
			25, 40, 14, 13, 6, 9, 34, 21, 4, 15, 28, 57, 32, 61, 52, 39, 
			64, 19, 22, 11, 7, 3, 8, 24, 20, 5, 38, 23, 16, 18, 10, 12, 
			25, 40, 14, 13, 6, 9, 34, 21, 4, 15, 28, 54, 51, 56, 53, 30, 
			0, 1
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 2, 3, 4, 1, 1, 5, 6, 7, 8, 1, 1, 1, 1, 
			1, 9, 10, 1, 1, 11, 12, 12, 12, 12, 1, 1, 1, 13, 1, 14, 
			1, 1, 15, 1, 16, 1, 1, 17, 1, 1, 18, 19, 20, 1, 1, 1, 
			1, 1, 1, 21, 1, 1, 12, 12, 12, 22, 12, 12, 12, 1, 1, 12, 
			1, 1, 1, 1, 1, 23, 24, 12, 12, 25, 12, 12, 12, 12, 26, 12, 
			12, 12, 12, 12, 27, 12, 12, 12, 12, 12, 28, 12, 12, 12, 12, 1, 
			1, 29, 12, 12, 12, 12, 12, 12, 1, 1, 12, 30, 12, 12, 12, 12, 
			31, 12, 1, 1, 12, 12, 12, 12, 12, 12, 1, 1, 12, 12, 12, 12, 
			12, 12, 12, 12, 12, 12, 12, 12, 12, 1, 12, 12, 12, 12, 12, 12, 
			32, 1, 33, 33, 1, 1, 1, 34, 1, 35, 1, 36, 1, 1, 1, 37, 
			1, 1, 1, 1, 38, 39, 1, 1, 40, 41, 1, 42, 1, 1, 1, 1, 
			43, 1, 44, 1, 1, 1, 1, 45, 1, 1, 46, 47, 1, 1, 48, 49, 
			50, 51, 52, 53, 1, 54, 1, 55, 56, 57, 58, 58, 59, 60, 61, 62, 
			63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 1, 75, 75, 1, 
			1, 76, 1, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 
			90, 91, 92, 93, 94, 1, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 
			105, 106, 107, 108, 109, 110, 111, 76, 112, 113, 114, 115, 116, 117, 118, 119, 
			120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 91, 66, 
			134, 23, 135, 24, 136, 9, 137, 138, 139, 140, 141, 142, 143, 10, 144, 145, 
			146, 54, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 26, 
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 
			176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 
			192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 
			208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 
			224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 
			240, 241, 242, 33, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 58, 
			254, 255, 256, 257, 258, 259, 260, 75, 261, 262, 263, 264, 265, 266, 64, 267, 
			268, 269, 270, 271, 68, 77, 272, 273, 274, 275, 276, 47, 277, 278, 279, 280, 
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
			489, 490, 491, 492, 493, 494, 495, 12, 496, 497, 498, 499, 500, 501, 502, 503, 
			504, 505, 506
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 190, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 216, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 468, 663, 663, 663, 663, 663, 469, 470, 663, 663, 663, 663, 471, -1, 472, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 473, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1 },
			{ -1, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1 },
			{ -1, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 295, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 297, 299, 293, 293, 293, 293, 293, 293, 52, 293, 293 },
			{ -1, -1, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 303, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 53, 301 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 219, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 248, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 319, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, 51, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 51, 51, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 650, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 69, -1, -1, -1, 69, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, 69, -1, -1, 69, -1, -1, -1, -1, -1, -1, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 69, 69, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 70, 70, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 330, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 78, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 350, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 361, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, 361, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, 361, -1, -1, -1, -1 },
			{ -1, -1, -1, 654, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 579, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 671, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ 1, 2, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 393, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 395, 397, 201, 201, 201, 201, 201, 201, 145, 201, 201 },
			{ -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 409, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, -1, 151 },
			{ 1, 2, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 410, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 411, 412, 204, 204, 204, 204, 204, 204, 204, 204, 154 },
			{ 191, 150, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 156, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 416, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 417, 418, 205, 205, 205, 205, 206, 205, 205, 205, 205 },
			{ -1, -1, -1, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, -1, 159, 159, 159, 159, 159, 159, 159, 159, -1, -1, 159, 159, -1, -1, -1, -1, 159, -1, -1, -1, 159, 159, 159, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 159, 159, -1, -1, -1, -1, -1 },
			{ -1, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, -1, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 166, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, -1, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 172, 433, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 226, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 235, 210, 210, 210, 210 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 176, 176, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, 178, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 443, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 187, -1, -1, -1, -1 },
			{ -1, -1, -1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
			{ -1, -1, -1, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, -1, 419, 419, 419, 419, 419, 419, 419, 419, -1, -1, 419, -1, -1, -1, -1, -1, 419, -1, -1, -1, 419, 419, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 22, 663, 474, 663, 663, 475, 663, 663, 663, -1, 597, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 248, 289, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 291, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, 11, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, -1, 199, 199, 199, 199, 199, 199, 199, 199, 405, -1, 199, 199, -1, -1, -1, -1, 199, -1, -1, -1, 199, 199, 199, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 148, 199, 199, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 96, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 393, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 399, 401, 201, 201, 201, 201, 201, 201, -1, 201, 201 },
			{ -1, 223, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 410, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 413, 414, 204, 204, 204, 204, 204, 204, 204, 204, -1 },
			{ -1, -1, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 156, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 416, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 420, 421, 205, 205, 205, 205, 206, 205, 205, 205, 205 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 156, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 160, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, 430, -1, 430, 430, 430, 430, 430, 430, 430, 430, -1, -1, 430, 430, -1, -1, -1, -1, 430, -1, -1, -1, 430, 430, 430, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 162, 162, 430, 430, -1, -1, -1, -1, -1 },
			{ -1, -1, 434, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 226, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 235, 210, 210, 210, 210 },
			{ -1, -1, -1, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, -1, 51, 51, 51, 51, 51, 51, 51, 51, -1, -1, 51, -1, -1, -1, -1, -1, 51, -1, -1, -1, 51, 51, 51, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 436, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 437, -1, -1, -1, -1, -1, -1, 178, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 178, 178, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 213, -1, -1, -1, 213, 213, -1, -1, 213, -1, -1, -1, -1, -1, -1, -1, 213, -1, -1, 213, -1, -1, -1, -1, -1, -1, 213, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 213, 213, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 185, 186, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 477, 663, 254, 663, 663, 663, 663, 663, 663, 23, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 224, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 424, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 425, 426, 263, 263, 263, 263, -1, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 227, 227, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 24, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 220, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 393, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 399, 401, 201, 201, 201, 201, 201, 201, -1, 201, 201 },
			{ -1, 223, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 410, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 413, 414, 204, 204, 204, 204, 204, 204, 204, 204, -1 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 243, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 247, 251, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 156, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 424, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 425, 426, 263, 263, 263, 263, -1, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 192, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 31, 31, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 270, 663, 663, 25, 599, 663, 663, -1, 663, 663, 663, 663, 626, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 156, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 416, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 420, 421, 205, 205, 205, 205, 206, 205, 205, 205, 205 },
			{ 1, 2, 8, 9, 446, 194, 458, 218, 461, 463, 465, 596, 230, 625, 644, 655, 661, 10, 663, 238, 663, 665, 242, 663, 666, 667, 193, 217, 663, 11, 12, 229, 13, 237, 467, 241, 10, 245, 663, 668, 663, 245, 249, 253, 14, 257, 261, 265, 269, 273, 277, 281, 284, 245, 15, 287, 16, 245, 195, 11, 10, 245, 17, 18, 19 },
			{ -1, -1, -1, -1, -1, 256, -1, 260, 264, 449, -1, -1, 268, 272, 276, -1, -1, -1, -1, 280, -1, -1, 283, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 286, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 603, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, 244, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, 17, 18, -1 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 225, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 233, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 255, 259, 205, 205, 205, 205, 234, 205, 205, 205, 205 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 448, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 54, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 225, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 233, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 255, 421, 205, 205, 205, 205, 234, 205, 205, 205, 205 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, 197, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 305, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, 197, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 14, 34, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 55, 663, -1, 663, 497, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 225, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 233, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 420, 259, 205, 205, 205, 205, 234, 205, 205, 205, 205 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 56, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 233, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 247, 426, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 450, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 57, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 205, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 205, 263, 263, 263, 263, 263, 263, 263, 263, 205, 205, 263, 205, 205, 205, 233, 205, 263, 205, 205, 205, 263, 263, 263, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 425, 251, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 307, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, 41, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 58, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 309, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 59, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 267, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 275, 279, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 313, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, 44, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 60, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 424, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 425, 426, 263, 263, 263, 263, -1, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, 451, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 63, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 267, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 275, 426, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, -1, -1, 315, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 71, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 224, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 267, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 425, 279, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 459, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 72, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 317, -1, -1, -1, -1, -1, -1, 452, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 50, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 73, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 74, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 75, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 76, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 77, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, -1, 293, 293, 293, 293 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 79, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 295, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, -1, 293, 293, 293, 293, 293, 293, 52, 293, 293 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 80, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 293, -1, -1, -1, -1, -1, -1, -1, -1, 293, 293, -1, 293, 293, 293, 295, 293, -1, 293, 293, 293, -1, -1, -1, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, 293, -1, 293, 293, 293, 293, 293, 293, 293, 52, 293, 293 },
			{ -1, -1, -1, 663, 663, 663, 81, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 82, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301, 301 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 83, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 84, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 325, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 85, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 453, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 86, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 327, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 87, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 454, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 88, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 331, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 89, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 335, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 90, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 91, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, -1, 337, 337, 337, 337, 337, 337, 337, 337, -1, -1, 337, -1, -1, -1, -1, -1, 337, -1, 321, -1, 337, 337, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 339, 456, -1 },
			{ -1, -1, -1, 92, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 341, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, 95, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 93, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 460, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 94, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 457, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 97, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, 349, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 98, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 99, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 100, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 462, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 101, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 337, 96, 337, 337, 337, 337, 337, 337, 337, 337, -1, -1, 337, 337, -1, -1, -1, -1, 337, -1, -1, -1, 337, 337, 337, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 337, 337, 200, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 102, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, -1, 357, 357, 357, 357, 357, 357, 357, 357, -1, -1, 357, -1, -1, -1, -1, -1, 357, -1, -1, -1, 357, 357, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 103, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 363, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 106, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, 95, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 107, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 108, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 369, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 109, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 110, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 351, 104, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 111, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 112, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 377, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, 105, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 113, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, 357, -1, 357, 357, 357, 357, 357, 357, 357, 357, -1, -1, 357, 357, -1, -1, -1, -1, 357, -1, -1, -1, 357, 357, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 357, 357, -1, -1, 381, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 116, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, -1, 359, 359, 359, 359, 359, 359, 359, 359, -1, -1, 359, 359, -1, -1, -1, -1, 359, -1, -1, -1, 359, 359, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 359, 359, -1, -1, -1, 381, -1 },
			{ -1, -1, -1, 117, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 464, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 361, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 118, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 466, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 119, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 351, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 120, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, 114, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 121, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 383, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 124, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 125, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 373, 115, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 126, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 127, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 387, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 128, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, 105, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 129, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 96, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 200, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 130, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 383, 122, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 131, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 385, 123, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 132, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 379, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 133, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 391, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 134, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 137, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 135, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 220, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 136, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 220, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 393, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 231, 146, 201, 201, 201, 201, 201, 201, 220, 201, 201 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 138, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 220, 201, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 201, 199, 199, 199, 199, 199, 199, 199, 199, 201, 201, 199, 201, 201, 201, 393, 201, 199, 201, 201, 201, 199, 199, 199, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 147, 231, 201, 201, 201, 201, 201, 201, 220, 201, 201 },
			{ -1, -1, -1, 139, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 220, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 393, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 231, 403, 201, 201, 201, 201, 201, 201, 220, 201, 201 },
			{ -1, -1, -1, 140, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, 220, 201, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, 201, 201, -1, 201, 201, 201, 393, 201, -1, 201, 201, 201, -1, -1, -1, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 403, 231, 201, 201, 201, 201, 201, 201, 220, 201, 201 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 141, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 142, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 407, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 143, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ 149, 149, -1, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, -1, 149, 149, 149, 149, 149, 149, 149, 149, -1, -1, 149, -1, -1, -1, -1, -1, 149, -1, -1, -1, 149, 149, 149, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 150, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 409, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 152, 151 },
			{ -1, -1, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151 },
			{ -1, 223, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204 },
			{ -1, 223, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 410, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 232, 202, 204, 204, 204, 204, 204, 204, 204, 204, 223 },
			{ -1, 223, 204, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 204, 199, 199, 199, 199, 199, 199, 199, 199, 204, 204, 199, 204, 204, 204, 410, 204, 199, 204, 204, 204, 199, 199, 199, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 203, 232, 204, 204, 204, 204, 204, 204, 204, 204, 223 },
			{ -1, 223, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 410, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 232, 415, 204, 204, 204, 204, 204, 204, 204, 204, 223 },
			{ -1, 223, 204, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 204, -1, -1, -1, -1, -1, -1, -1, -1, 204, 204, -1, 204, 204, 204, 410, 204, -1, 204, 204, 204, -1, -1, -1, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 415, 232, 204, 204, 204, 204, 204, 204, 204, 204, 223 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 233, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 239, 221, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, 224, 205, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 205, 199, 199, 199, 199, 199, 199, 199, 199, 205, 205, 199, 205, 205, 205, 233, 205, 199, 205, 205, 205, 199, 199, 199, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 222, 239, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, -1, -1, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 419, 157, 419, 419, 419, 419, 419, 419, 419, 419, -1, -1, 419, 419, -1, -1, -1, -1, 419, -1, -1, -1, 419, 419, 419, 422, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 419, 419, 207, -1, -1, -1, -1 },
			{ -1, 224, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 233, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 239, 423, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, 224, 205, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 205, -1, -1, -1, -1, -1, -1, -1, -1, 205, 205, -1, 205, 205, 205, 233, 205, -1, 205, 205, 205, -1, -1, -1, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 205, 423, 239, 205, 205, 205, 205, 205, 205, 205, 205, 205 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 207, -1, -1, -1, -1 },
			{ -1, 224, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, 224, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 267, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 271, 423, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ -1, 224, 263, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 263, -1, -1, -1, -1, -1, -1, -1, -1, 263, 263, -1, 263, 263, 263, 267, 263, -1, 263, 263, 263, -1, -1, -1, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 263, 423, 271, 263, 263, 263, 263, 263, 263, 263, 263, 263 },
			{ 1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 158, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 10, 159, 159, 159, 159, 159, 159, 159, 159, 208, 158, 159, 158, 158, 158, 158, 158, 159, 158, 10, 158, 159, 159, 159, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 10, 158, 158, 158, 158 },
			{ 1, 2, 161, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 209, 161, 209, 209, 209, 209, 209, 209, 209, 209, 161, 161, 209, 161, 161, 161, 161, 161, 209, 161, 161, 161, 209, 209, 209, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161, 161 },
			{ 1, 163, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 165, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164, 164 },
			{ 1, 167, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 169, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168, 168 },
			{ -1, 172, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 226, 210, 210, 210, 210, 210, 210, 210, 210, 210, 173, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 235, 210, 210, 210, 210 },
			{ -1, -1, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 226, 210, 210, 210, 210, 210, 210, 210, 210, 210, -1, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 235, 210, 210, 210, 210 },
			{ 1, 2, 174, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 175, 663, 663, 663, 663, 663, 663, 663, 663, 174, 174, 663, 176, 12, 174, 175, 174, 663, 174, 175, 174, 663, 663, 663, 174, 174, 174, 175, 174, 174, 174, 174, 174, 174, 174, 174, 174, 174, 211, 174, 174, 212, 176, 175, 177, 174, 175, 174 },
			{ 1, 2, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179, 179 },
			{ 447, 150, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180, 180 },
			{ -1, -1, -1, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 181, 440, 440, 440, 440, 440, 440, 440, 440, -1, -1, 440, 440, -1, -1, -1, -1, 440, -1, -1, -1, 440, 440, 440, 441, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 440, 440, 214, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 214, -1, -1, -1, -1 },
			{ 1, 2, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 184, 183, 182, 182, 182, 182, 182, 215, 182, 185, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182 },
			{ 1, 2, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 188, 182, 182, 182, 182, 215, 182, 185, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182 },
			{ 1, 2, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182, 189, 215, 182, 185, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 182, 183, 182, 182, 182, 182 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 246, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, 440, -1, 440, 440, 440, 440, 440, 440, 440, 440, -1, -1, 440, -1, -1, -1, -1, -1, 440, -1, -1, -1, 440, 440, 440, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 321, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 311, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 323, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 329, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 455, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 345, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 347, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, 359, -1, 359, 359, 359, 359, 359, 359, 359, 359, -1, -1, 359, -1, -1, -1, -1, -1, 359, -1, -1, -1, 359, 359, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 250, 663, 663, -1, 663, 663, 476, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 333, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 478, 663, 663, 663, 600, 663, 663, 258, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 375, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 598, 663, 663, 262, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 389, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 266, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 479, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 343, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 274, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 278, 628, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 492, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 282, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 285, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 493, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 288, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 645, 663, 663, 663, 663, 494, 663, 495, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 496, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 498, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 601, 663, 663, 629, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 499, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 656, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 503, 663, 663, 663, 663, -1, 663, 504, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 505, 663, 663, 663, 663, 663, 663, 290, 663, 663, 673, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 606, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 630, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 506, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 607, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 507, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 292, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 294, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 604, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 646, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 511, 663, 663, 663, 663, 663, 663, 658, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 512, 513, 514, 663, 515, 657, 663, 663, 663, 663, 609, -1, 516, 663, 610, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 296, 663, 611, 518, 663, 663, 663, 663, 519, 663, 663, 663, -1, 663, 663, 663, 520, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 298, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 631, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 521, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 300, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 302, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 304, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 306, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 522, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 308, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 633, 663, 663, 663, 663, 663, 663, 310, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 312, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 314, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 316, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 526, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 318, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 320, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 322, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 324, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 326, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 662, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 664, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 529, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 530, 663, 663, 663, 612, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 531, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 613, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 533, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 328, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 535, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 617, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 615, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 538, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 638, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 542, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 332, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 334, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 336, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 338, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 340, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 546, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 547, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 619, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 548, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 342, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 551, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 621, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 553, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 344, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 555, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 346, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 348, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 352, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 622, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 558, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 354, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 356, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 358, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 560, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 640, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 562, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 563, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 642, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 360, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 565, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 566, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 567, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 362, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 364, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 366, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 368, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 370, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 372, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 575, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 576, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 374, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 376, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 378, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 580, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 581, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 380, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 382, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 384, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 674, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 582, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 386, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 583, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 623, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 388, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 390, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 584, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 392, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 394, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 585, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 396, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 587, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 590, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 591, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 398, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 400, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 402, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 592, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 593, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 404, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 594, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 595, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 406, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 627, 663, 663, 663, 480, -1, 663, 481, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 605, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 501, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 508, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 500, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 648, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 509, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 510, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 527, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 635, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 524, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 649, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 536, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 636, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 614, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 534, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 672, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 549, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 550, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 554, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 641, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 552, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 557, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 620, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 573, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 564, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 569, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 586, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 588, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 482, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 483, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 647, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 502, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 517, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 634, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 525, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 537, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 637, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 618, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 540, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 669, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 639, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 559, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 556, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 561, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 574, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 570, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 577, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 589, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 484, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 608, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 528, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 632, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 539, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 544, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 541, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 616, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 568, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 571, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 578, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 485, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 523, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 532, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 651, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 543, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 572, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 486, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 545, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 670, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 602, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 487, 663, 663, 663, 488, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 489, 663, 663, 663, 663, 490, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 491, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 652, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 653, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 624, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 660, 663, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 663, 659, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 663, 663, 663, 663, 663, 663, 663, 663, 663, 643, 663, 663, 663, 663, -1, 663, 663, 663, 663, 663, 663, 663, 663, -1, -1, 663, 663, -1, -1, -1, -1, 663, -1, -1, -1, 663, 663, 663, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 663, 663, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  240,
			  144,
			  408,
			  153,
			  155,
			  427,
			  428,
			  429,
			  431,
			  432,
			  171,
			  435,
			  438,
			  439,
			  442,
			  444,
			  445
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 675);
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

