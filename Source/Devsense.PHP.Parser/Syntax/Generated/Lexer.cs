namespace Devsense.PHP.Syntax
{
	#region User Code
	
	/*
 Copyright (c) 2016 Michal Brabec
 The use and distribution terms for this software are contained in the file named License.txt, 
 which can be found in the root of the Phalanger distribution. By using this software 
 in any fashion, you are agreeing to be bound by the terms of this license.
 You must not remove this notice from this software.
*/
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
					// #line 714
					{ 
					    this._tokenSemantics.Object = GetTokenString();
						return Tokens.T_INLINE_HTML; 
					}
					break;
					
				case 3:
					// #line 732
					{
						if (this._allowShortTags) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							yymore(); break;//return Tokens.T_INLINE_HTML;
						}
					}
					break;
					
				case 4:
					// #line 719
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 5:
					// #line 725
					{
						//HANDLE_NEWLINE(yytext[yyleng-1]);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 6:
					// #line 779
					{
						return ProcessLabel();
					}
					break;
					
				case 7:
					// #line 273
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 8:
					// #line 609
					{
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 9:
					// #line 654
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 10:
					// #line 918
					{
						//zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						return Tokens.T_ERROR;
					}
					break;
					
				case 11:
					// #line 297
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 12:
					// #line 783
					{
						yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); 
						yymore(); 
						break;
					}
					break;
					
				case 13:
					// #line 614
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return (Tokens.T_LBRACE);
					}
					break;
					
				case 14:
					// #line 626
					{
						ResetDocBlock();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return (Tokens.T_RBRACE);
					}
					break;
					
				case 15:
					// #line 854
					{
						BEGIN(LexicalStates.ST_BACKQUOTE); 
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 16:
					// #line 821
					{
						BEGIN(LexicalStates.ST_DOUBLE_QUOTES);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 17:
					// #line 806
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
					// #line 123
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IF);
					}
					break;
					
				case 19:
					// #line 153
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DO);
					}
					break;
					
				case 20:
					// #line 589
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 21:
					// #line 193
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_AS);
					}
					break;
					
				case 22:
					// #line 497
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 23:
					// #line 268
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 24:
					// #line 533
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 25:
					// #line 605
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 26:
					// #line 525
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 27:
					// #line 670
					{
						return ProcessRealNumber();
					}
					break;
					
				case 28:
					// #line 293
					{
						return (Tokens.T_DOUBLE_COLON);
					}
					break;
					
				case 29:
					// #line 553
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 30:
					// #line 799
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 31:
					// #line 305
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 32:
					// #line 789
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 33:
					// #line 549
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 34:
					// #line 541
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 35:
					// #line 537
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 36:
					// #line 474
					{
						return (Tokens.T_DOUBLE_ARROW);
					}
					break;
					
				case 37:
					// #line 509
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 38:
					// #line 529
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 39:
					// #line 493
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 40:
					// #line 513
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 41:
					// #line 521
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 42:
					// #line 601
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 43:
					// #line 557
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 44:
					// #line 569
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 45:
					// #line 585
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 46:
					// #line 573
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 47:
					// #line 581
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 48:
					// #line 577
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 49:
					// #line 758
					{
						return ProcessVariable();
					}
					break;
					
				case 50:
					// #line 817
					{
						return ProcessDoubleQuotedString();
					}
					break;
					
				case 51:
					// #line 804
					{ return ProcessSingleQuotedString(); }
					break;
					
				case 52:
					// #line 597
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 53:
					// #line 103
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRY);
					}
					break;
					
				case 54:
					// #line 73
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT);
					}
					break;
					
				case 55:
					// #line 158
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOR);
					}
					break;
					
				case 56:
					// #line 382
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_USE);
					}
					break;
					
				case 57:
					// #line 309
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NEW);
					}
					break;
					
				case 58:
					// #line 593
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 59:
					// #line 565
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 60:
					// #line 301
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 61:
					// #line 319
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_VAR);
					}
					break;
					
				case 62:
					// #line 545
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 63:
					// #line 501
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 64:
					// #line 505
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 65:
					// #line 517
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 66:
					// #line 561
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 67:
					// #line 658
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 68:
					// #line 650
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 69:
					// #line 68
					{ 
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 70:
					// #line 233
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ECHO);
					}
					break;
					
				case 71:
					// #line 138
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSE);
					}
					break;
					
				case 72:
					// #line 352
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EVAL);
					}
					break;
					
				case 73:
					// #line 208
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CASE);
					}
					break;
					
				case 74:
					// #line 478
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LIST);
					}
					break;
					
				case 75:
					// #line 228
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GOTO);
					}
					break;
					
				case 76:
					// #line 794
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 77:
					// #line 133
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 78:
					// #line 402
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 79:
					// #line 397
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ISSET);
					}
					break;
					
				case 80:
					// #line 253
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 81:
					// #line 118
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_THROW);
					}
					break;
					
				case 82:
					// #line 449
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINAL);
					}
					break;
					
				case 83:
					// #line 469
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_UNSET);
					}
					break;
					
				case 84:
					// #line 83
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONST);
					}
					break;
					
				case 85:
					// #line 314
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLONE);
					}
					break;
					
				case 86:
					// #line 243
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS);
					}
					break;
					
				case 87:
					// #line 108
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CATCH);
					}
					break;
					
				case 88:
					// #line 98
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_YIELD);
					}
					break;
					
				case 89:
					// #line 483
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 90:
					// #line 143
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_WHILE);
					}
					break;
					
				case 91:
					// #line 218
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_BREAK);
					}
					break;
					
				case 92:
					// #line 238
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRINT);
					}
					break;
					
				case 93:
					// #line 324
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 94:
					// #line 827
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
					// #line 163
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 96:
					// #line 128
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 97:
					// #line 439
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_STATIC);
					}
					break;
					
				case 98:
					// #line 198
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 99:
					// #line 88
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_RETURN);
					}
					break;
					
				case 100:
					// #line 392
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 101:
					// #line 464
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 102:
					// #line 328
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 103:
					// #line 344
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 104:
					// #line 258
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 105:
					// #line 357
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 106:
					// #line 213
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 107:
					// #line 178
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 108:
					// #line 113
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 109:
					// #line 168
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 110:
					// #line 367
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 111:
					// #line 454
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 112:
					// #line 348
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 113:
					// #line 336
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 114:
					// #line 704
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_DIR);
					}
					break;
					
				case 115:
					// #line 148
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 116:
					// #line 78
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 117:
					// #line 223
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 118:
					// #line 488
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 119:
					// #line 444
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 120:
					// #line 340
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 121:
					// #line 332
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 122:
					// #line 699
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FILE);
					}
					break;
					
				case 123:
					// #line 694
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_LINE);
					}
					break;
					
				case 124:
					// #line 203
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 125:
					// #line 248
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 126:
					// #line 387
					{
						this._tokenSemantics.Object = GetTokenString();
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 127:
					// #line 377
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 128:
					// #line 459
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 129:
					// #line 679
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 130:
					// #line 674
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 131:
					// #line 183
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 132:
					// #line 173
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 133:
					// #line 188
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 134:
					// #line 263
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 135:
					// #line 93
					{
						//HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 136:
					// #line 689
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 137:
					// #line 362
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 138:
					// #line 372
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 139:
					// #line 684
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 140:
					// #line 709
					{
						this._tokenSemantics.Object = GetTokenString();
						return (Tokens.T_NS_C);
					}
					break;
					
				case 141:
					// #line 422
					{
						// IMPORTANT - Added because PHP lexer explicitly checks halt compiler syntax and reverts to initial state after semicolon
						yy_push_state(LexicalStates.ST_HALT_COMPILER1); 
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 142:
					// #line 900
					{
					    this._tokenSemantics.Object = ProcessEscapedString(GetTokenString(), _encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 143:
					// #line 876
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 144:
					// #line 867
					{
						//Z_LVAL_P(zendlval) = (zend_long) '{';
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 145:
					// #line 620
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 146:
					// #line 752
					{
						yyless(TokenLength - 1);
						yy_push_state(LexicalStates.ST_VAR_OFFSET);
						return ProcessVariable();
					}
					break;
					
				case 147:
					// #line 746
					{
						yyless(TokenLength - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return ProcessVariable();
					}
					break;
					
				case 148:
					// #line 814
					{ yymore(); break; }
					break;
					
				case 149:
					// #line 815
					{ BEGIN(LexicalStates.ST_IN_SCRIPTING); return ProcessSingleQuotedString(); }
					break;
					
				case 150:
					// #line 905
					{
					    this._tokenSemantics.Object = ProcessEscapedString(GetTokenString(), _encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 151:
					// #line 881
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 152:
					// #line 912
					{
					    this._tokenSemantics.Object = ProcessEscapedString(GetTokenString(), _encoding, false);
					    return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 153:
					// #line 910
					{ yymore(); break; }
					break;
					
				case 154:
					// #line 892
					{
					    if(GetTokenString().Contains(this._hereDocLabel))
							return ProcessEndNowDoc(s => (string)ProcessEscapedString(s, _encoding, false));
					    yymore(); break;
					}
					break;
					
				case 155:
					// #line 282
					{
						yy_pop_state();
						return ProcessLabel();
					}
					break;
					
				case 156:
					// #line 287
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						break;
					}
					break;
					
				case 157:
					// #line 278
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 158:
					// #line 643
					{
						yyless(0);
						if (!yy_pop_state()) return Tokens.T_ERROR;
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						break;
					}
					break;
					
				case 159:
					// #line 634
					{
						yyless(TokenLength - 1);
						this._tokenSemantics.Object = GetTokenString();
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 160:
					// #line 795
					{ yymore(); break; }
					break;
					
				case 161:
					// #line 797
					{ yymore(); break; }
					break;
					
				case 162:
					// #line 796
					{ yy_pop_state(); SetDocBlock(); return Tokens.T_DOC_COMMENT; }
					break;
					
				case 163:
					// #line 790
					{ yymore(); break; }
					break;
					
				case 164:
					// #line 792
					{ yymore(); break; }
					break;
					
				case 165:
					// #line 791
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 166:
					// #line 925
					{ yymore(); break; }
					break;
					
				case 167:
					// #line 926
					{ yy_pop_state(); return Tokens.T_COMMENT; }
					break;
					
				case 168:
					// #line 924
					{ yymore(); break; }
					break;
					
				case 169:
					// #line 928
					{ yymore(); break; }
					break;
					
				case 170:
					// #line 772
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 171:
					// #line 767
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 172:
					// #line 662
					{ /* Offset could be treated as a long */
						return ProcessVariableOffsetNumber();
					}
					break;
					
				case 173:
					// #line 762
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 174:
					// #line 666
					{ /* Offset must be treated as a string */
						return ProcessVariableOffsetString();
					}
					break;
					
				case 175:
					// #line 860
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						this._tokenSemantics.Object = this._hereDocLabel;
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 176:
					// #line 898
					{ yymore(); break; }
					break;
					
				case 177:
					// #line 886
					{
					    if(GetTokenString().Contains(this._hereDocLabel))
							return ProcessEndNowDoc(s => s);
					    yymore(); break;
					}
					break;
					
				case 178:
					// #line 434
					{
						yy_pop_state();
						yymore(); break;
					}
					break;
					
				case 179:
					// #line 428
					{ return (Tokens.T_WHITESPACE); }
					break;
					
				case 180:
					// #line 407
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER2);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 181:
					// #line 431
					{ yy_push_state(LexicalStates.ST_ONE_LINE_COMMENT); yymore(); break; }
					break;
					
				case 182:
					// #line 432
					{ yy_push_state(LexicalStates.ST_COMMENT); yymore(); break; }
					break;
					
				case 183:
					// #line 430
					{ yy_push_state(LexicalStates.ST_DOC_COMMENT); yymore(); ResetDocBlock(); break; }
					break;
					
				case 184:
					// #line 412
					{
						BEGIN(LexicalStates.ST_HALT_COMPILER3);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 185:
					// #line 417
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens)GetTokenChar(0);
					}
					break;
					
				case 188: goto case 2;
				case 189: goto case 5;
				case 190: goto case 6;
				case 191: goto case 8;
				case 192: goto case 9;
				case 193: goto case 27;
				case 194: goto case 30;
				case 195: goto case 40;
				case 196: goto case 49;
				case 197: goto case 94;
				case 198: goto case 142;
				case 199: goto case 150;
				case 200: goto case 152;
				case 201: goto case 153;
				case 202: goto case 154;
				case 203: goto case 156;
				case 204: goto case 158;
				case 205: goto case 167;
				case 206: goto case 168;
				case 207: goto case 171;
				case 208: goto case 172;
				case 209: goto case 174;
				case 210: goto case 177;
				case 211: goto case 178;
				case 213: goto case 6;
				case 214: goto case 8;
				case 215: goto case 30;
				case 216: goto case 174;
				case 218: goto case 6;
				case 219: goto case 8;
				case 221: goto case 6;
				case 222: goto case 8;
				case 224: goto case 6;
				case 225: goto case 8;
				case 227: goto case 6;
				case 228: goto case 8;
				case 230: goto case 6;
				case 231: goto case 8;
				case 233: goto case 6;
				case 234: goto case 8;
				case 236: goto case 6;
				case 237: goto case 8;
				case 239: goto case 6;
				case 240: goto case 8;
				case 242: goto case 6;
				case 243: goto case 8;
				case 245: goto case 6;
				case 246: goto case 8;
				case 248: goto case 6;
				case 249: goto case 8;
				case 251: goto case 6;
				case 252: goto case 8;
				case 254: goto case 6;
				case 255: goto case 8;
				case 257: goto case 6;
				case 258: goto case 8;
				case 260: goto case 6;
				case 261: goto case 8;
				case 263: goto case 6;
				case 265: goto case 6;
				case 267: goto case 6;
				case 269: goto case 6;
				case 271: goto case 6;
				case 273: goto case 6;
				case 275: goto case 6;
				case 277: goto case 6;
				case 279: goto case 6;
				case 281: goto case 6;
				case 283: goto case 6;
				case 285: goto case 6;
				case 287: goto case 6;
				case 289: goto case 6;
				case 291: goto case 6;
				case 293: goto case 6;
				case 295: goto case 6;
				case 297: goto case 6;
				case 299: goto case 6;
				case 301: goto case 6;
				case 303: goto case 6;
				case 305: goto case 6;
				case 307: goto case 6;
				case 309: goto case 6;
				case 311: goto case 6;
				case 313: goto case 6;
				case 315: goto case 6;
				case 317: goto case 6;
				case 319: goto case 6;
				case 321: goto case 6;
				case 323: goto case 6;
				case 325: goto case 6;
				case 327: goto case 6;
				case 329: goto case 6;
				case 331: goto case 6;
				case 333: goto case 6;
				case 335: goto case 6;
				case 337: goto case 6;
				case 339: goto case 6;
				case 341: goto case 6;
				case 343: goto case 6;
				case 345: goto case 6;
				case 347: goto case 6;
				case 349: goto case 6;
				case 351: goto case 6;
				case 353: goto case 6;
				case 355: goto case 6;
				case 357: goto case 6;
				case 359: goto case 6;
				case 361: goto case 6;
				case 363: goto case 6;
				case 365: goto case 6;
				case 367: goto case 6;
				case 369: goto case 6;
				case 371: goto case 6;
				case 373: goto case 6;
				case 375: goto case 6;
				case 377: goto case 6;
				case 379: goto case 6;
				case 406: goto case 6;
				case 418: goto case 6;
				case 421: goto case 6;
				case 423: goto case 6;
				case 425: goto case 6;
				case 427: goto case 6;
				case 428: goto case 6;
				case 429: goto case 6;
				case 430: goto case 6;
				case 431: goto case 6;
				case 432: goto case 6;
				case 433: goto case 6;
				case 434: goto case 6;
				case 435: goto case 6;
				case 436: goto case 6;
				case 437: goto case 6;
				case 438: goto case 6;
				case 439: goto case 6;
				case 440: goto case 6;
				case 441: goto case 6;
				case 442: goto case 6;
				case 443: goto case 6;
				case 444: goto case 6;
				case 445: goto case 6;
				case 446: goto case 6;
				case 447: goto case 6;
				case 448: goto case 6;
				case 449: goto case 6;
				case 450: goto case 6;
				case 451: goto case 6;
				case 452: goto case 6;
				case 453: goto case 6;
				case 454: goto case 6;
				case 455: goto case 6;
				case 456: goto case 6;
				case 457: goto case 6;
				case 458: goto case 6;
				case 459: goto case 6;
				case 460: goto case 6;
				case 461: goto case 6;
				case 462: goto case 6;
				case 463: goto case 6;
				case 464: goto case 6;
				case 465: goto case 6;
				case 466: goto case 6;
				case 467: goto case 6;
				case 468: goto case 6;
				case 469: goto case 6;
				case 470: goto case 6;
				case 471: goto case 6;
				case 472: goto case 6;
				case 473: goto case 6;
				case 474: goto case 6;
				case 475: goto case 6;
				case 476: goto case 6;
				case 477: goto case 6;
				case 478: goto case 6;
				case 479: goto case 6;
				case 480: goto case 6;
				case 481: goto case 6;
				case 482: goto case 6;
				case 483: goto case 6;
				case 484: goto case 6;
				case 485: goto case 6;
				case 486: goto case 6;
				case 487: goto case 6;
				case 488: goto case 6;
				case 489: goto case 6;
				case 490: goto case 6;
				case 491: goto case 6;
				case 492: goto case 6;
				case 493: goto case 6;
				case 494: goto case 6;
				case 495: goto case 6;
				case 496: goto case 6;
				case 497: goto case 6;
				case 498: goto case 6;
				case 499: goto case 6;
				case 500: goto case 6;
				case 501: goto case 6;
				case 502: goto case 6;
				case 503: goto case 6;
				case 504: goto case 6;
				case 505: goto case 6;
				case 506: goto case 6;
				case 507: goto case 6;
				case 508: goto case 6;
				case 509: goto case 6;
				case 510: goto case 6;
				case 511: goto case 6;
				case 512: goto case 6;
				case 513: goto case 6;
				case 514: goto case 6;
				case 515: goto case 6;
				case 516: goto case 6;
				case 517: goto case 6;
				case 518: goto case 6;
				case 519: goto case 6;
				case 520: goto case 6;
				case 521: goto case 6;
				case 522: goto case 6;
				case 523: goto case 6;
				case 524: goto case 6;
				case 525: goto case 6;
				case 526: goto case 6;
				case 527: goto case 6;
				case 528: goto case 6;
				case 529: goto case 6;
				case 530: goto case 6;
				case 531: goto case 6;
				case 532: goto case 6;
				case 533: goto case 6;
				case 534: goto case 6;
				case 535: goto case 6;
				case 536: goto case 6;
				case 537: goto case 6;
				case 538: goto case 6;
				case 539: goto case 6;
				case 540: goto case 6;
				case 541: goto case 6;
				case 542: goto case 6;
				case 543: goto case 6;
				case 544: goto case 6;
				case 545: goto case 6;
				case 546: goto case 6;
				case 547: goto case 6;
				case 548: goto case 6;
				case 549: goto case 6;
				case 550: goto case 6;
				case 551: goto case 6;
				case 552: goto case 6;
				case 553: goto case 6;
				case 554: goto case 6;
				case 555: goto case 6;
				case 556: goto case 6;
				case 557: goto case 6;
				case 558: goto case 6;
				case 559: goto case 6;
				case 560: goto case 6;
				case 561: goto case 6;
				case 562: goto case 6;
				case 563: goto case 6;
				case 564: goto case 6;
				case 565: goto case 6;
				case 566: goto case 6;
				case 567: goto case 6;
				case 568: goto case 6;
				case 569: goto case 6;
				case 570: goto case 6;
				case 571: goto case 6;
				case 572: goto case 6;
				case 573: goto case 6;
				case 574: goto case 6;
				case 575: goto case 6;
				case 576: goto case 6;
				case 577: goto case 6;
				case 578: goto case 6;
				case 579: goto case 6;
				case 580: goto case 6;
				case 581: goto case 6;
				case 582: goto case 6;
				case 583: goto case 6;
				case 584: goto case 6;
				case 585: goto case 6;
				case 586: goto case 6;
				case 587: goto case 6;
				case 588: goto case 6;
				case 589: goto case 6;
				case 590: goto case 6;
				case 591: goto case 6;
				case 592: goto case 6;
				case 593: goto case 6;
				case 594: goto case 6;
				case 595: goto case 6;
				case 596: goto case 6;
				case 597: goto case 6;
				case 598: goto case 6;
				case 599: goto case 6;
				case 600: goto case 6;
				case 601: goto case 6;
				case 602: goto case 6;
				case 603: goto case 6;
				case 604: goto case 6;
				case 605: goto case 6;
				case 606: goto case 6;
				case 607: goto case 6;
				case 608: goto case 6;
				case 609: goto case 6;
				case 610: goto case 6;
				case 611: goto case 6;
				case 612: goto case 6;
				case 613: goto case 6;
				case 614: goto case 6;
				case 615: goto case 6;
				case 616: goto case 6;
				case 617: goto case 6;
				case 618: goto case 6;
				case 619: goto case 6;
				case 620: goto case 6;
				case 621: goto case 6;
				case 622: goto case 6;
				case 623: goto case 6;
				case 624: goto case 6;
				case 625: goto case 6;
				case 626: goto case 6;
				case 627: goto case 6;
				case 628: goto case 6;
				case 629: goto case 6;
				case 630: goto case 6;
				case 631: goto case 6;
				case 632: goto case 6;
				case 633: goto case 6;
				case 634: goto case 6;
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
			AcceptConditions.AcceptOnStart, // 154
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
			AcceptConditions.AcceptOnStart, // 177
			AcceptConditions.Accept, // 178
			AcceptConditions.Accept, // 179
			AcceptConditions.Accept, // 180
			AcceptConditions.Accept, // 181
			AcceptConditions.Accept, // 182
			AcceptConditions.Accept, // 183
			AcceptConditions.Accept, // 184
			AcceptConditions.Accept, // 185
			AcceptConditions.NotAccept, // 186
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
			AcceptConditions.AcceptOnStart, // 210
			AcceptConditions.Accept, // 211
			AcceptConditions.NotAccept, // 212
			AcceptConditions.Accept, // 213
			AcceptConditions.Accept, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.Accept, // 216
			AcceptConditions.NotAccept, // 217
			AcceptConditions.Accept, // 218
			AcceptConditions.Accept, // 219
			AcceptConditions.NotAccept, // 220
			AcceptConditions.Accept, // 221
			AcceptConditions.Accept, // 222
			AcceptConditions.NotAccept, // 223
			AcceptConditions.Accept, // 224
			AcceptConditions.Accept, // 225
			AcceptConditions.NotAccept, // 226
			AcceptConditions.Accept, // 227
			AcceptConditions.Accept, // 228
			AcceptConditions.NotAccept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.Accept, // 231
			AcceptConditions.NotAccept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.Accept, // 234
			AcceptConditions.NotAccept, // 235
			AcceptConditions.Accept, // 236
			AcceptConditions.Accept, // 237
			AcceptConditions.NotAccept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.Accept, // 240
			AcceptConditions.NotAccept, // 241
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
			AcceptConditions.NotAccept, // 264
			AcceptConditions.Accept, // 265
			AcceptConditions.NotAccept, // 266
			AcceptConditions.Accept, // 267
			AcceptConditions.NotAccept, // 268
			AcceptConditions.Accept, // 269
			AcceptConditions.NotAccept, // 270
			AcceptConditions.Accept, // 271
			AcceptConditions.NotAccept, // 272
			AcceptConditions.Accept, // 273
			AcceptConditions.NotAccept, // 274
			AcceptConditions.Accept, // 275
			AcceptConditions.NotAccept, // 276
			AcceptConditions.Accept, // 277
			AcceptConditions.NotAccept, // 278
			AcceptConditions.Accept, // 279
			AcceptConditions.NotAccept, // 280
			AcceptConditions.Accept, // 281
			AcceptConditions.NotAccept, // 282
			AcceptConditions.Accept, // 283
			AcceptConditions.NotAccept, // 284
			AcceptConditions.Accept, // 285
			AcceptConditions.NotAccept, // 286
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
			AcceptConditions.NotAccept, // 381
			AcceptConditions.NotAccept, // 382
			AcceptConditions.NotAccept, // 383
			AcceptConditions.NotAccept, // 384
			AcceptConditions.NotAccept, // 385
			AcceptConditions.NotAccept, // 386
			AcceptConditions.NotAccept, // 387
			AcceptConditions.NotAccept, // 388
			AcceptConditions.NotAccept, // 389
			AcceptConditions.NotAccept, // 390
			AcceptConditions.NotAccept, // 391
			AcceptConditions.NotAccept, // 392
			AcceptConditions.NotAccept, // 393
			AcceptConditions.NotAccept, // 394
			AcceptConditions.NotAccept, // 395
			AcceptConditions.NotAccept, // 396
			AcceptConditions.NotAccept, // 397
			AcceptConditions.NotAccept, // 398
			AcceptConditions.NotAccept, // 399
			AcceptConditions.NotAccept, // 400
			AcceptConditions.NotAccept, // 401
			AcceptConditions.NotAccept, // 402
			AcceptConditions.NotAccept, // 403
			AcceptConditions.NotAccept, // 404
			AcceptConditions.NotAccept, // 405
			AcceptConditions.Accept, // 406
			AcceptConditions.Accept, // 407
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
			AcceptConditions.Accept, // 418
			AcceptConditions.NotAccept, // 419
			AcceptConditions.NotAccept, // 420
			AcceptConditions.Accept, // 421
			AcceptConditions.NotAccept, // 422
			AcceptConditions.Accept, // 423
			AcceptConditions.NotAccept, // 424
			AcceptConditions.Accept, // 425
			AcceptConditions.NotAccept, // 426
			AcceptConditions.Accept, // 427
			AcceptConditions.Accept, // 428
			AcceptConditions.Accept, // 429
			AcceptConditions.Accept, // 430
			AcceptConditions.Accept, // 431
			AcceptConditions.Accept, // 432
			AcceptConditions.Accept, // 433
			AcceptConditions.Accept, // 434
			AcceptConditions.Accept, // 435
			AcceptConditions.Accept, // 436
			AcceptConditions.Accept, // 437
			AcceptConditions.Accept, // 438
			AcceptConditions.Accept, // 439
			AcceptConditions.Accept, // 440
			AcceptConditions.Accept, // 441
			AcceptConditions.Accept, // 442
			AcceptConditions.Accept, // 443
			AcceptConditions.Accept, // 444
			AcceptConditions.Accept, // 445
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
		};
		
		private static int[] colMap = new int[]
		{
			28, 28, 28, 28, 28, 28, 28, 28, 28, 35, 15, 28, 28, 59, 28, 28, 
			28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 
			35, 46, 63, 43, 54, 48, 49, 64, 34, 36, 42, 45, 52, 24, 31, 41, 
			57, 58, 27, 27, 27, 27, 27, 27, 60, 60, 29, 40, 47, 44, 25, 32, 
			52, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 26, 56, 30, 61, 51, 38, 
			62, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 26, 53, 50, 55, 52, 28, 
			65, 0
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 2, 3, 1, 1, 4, 5, 6, 7, 1, 1, 1, 1, 1, 1, 
			8, 9, 10, 10, 10, 10, 1, 1, 1, 11, 1, 12, 1, 1, 13, 1, 
			14, 1, 15, 1, 1, 16, 1, 1, 17, 18, 19, 1, 1, 1, 1, 1, 
			1, 20, 1, 1, 10, 10, 10, 21, 10, 10, 10, 1, 1, 10, 1, 1, 
			1, 1, 1, 22, 23, 10, 10, 24, 10, 10, 10, 10, 25, 10, 10, 10, 
			10, 10, 26, 10, 10, 10, 10, 10, 27, 10, 10, 10, 10, 1, 1, 28, 
			10, 10, 10, 10, 10, 10, 1, 1, 10, 29, 10, 10, 10, 10, 30, 10, 
			1, 1, 10, 10, 10, 10, 10, 10, 1, 1, 10, 10, 10, 10, 10, 10, 
			10, 10, 10, 10, 10, 10, 10, 1, 10, 10, 10, 10, 10, 10, 31, 1, 
			1, 1, 1, 1, 32, 1, 33, 1, 34, 1, 1, 35, 36, 1, 37, 1, 
			38, 39, 1, 40, 41, 1, 42, 1, 1, 1, 1, 1, 43, 1, 44, 1, 
			1, 1, 1, 45, 1, 1, 46, 47, 1, 1, 48, 49, 50, 51, 52, 53, 
			54, 55, 1, 1, 56, 57, 58, 59, 60, 61, 62, 1, 1, 63, 64, 65, 
			66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 
			82, 83, 84, 85, 1, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 
			97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 
			113, 114, 115, 83, 116, 65, 22, 117, 23, 118, 8, 119, 120, 121, 122, 123, 
			9, 124, 125, 126, 55, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 
			138, 139, 25, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 
			153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 
			169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 
			185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 
			201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 
			217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 
			233, 234, 235, 236, 237, 238, 239, 37, 240, 241, 242, 243, 67, 74, 244, 245, 
			246, 247, 248, 47, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 
			261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 
			277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 
			293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 
			309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 
			325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 
			341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 
			357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 
			373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 
			389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 
			405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 
			421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 
			437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 
			453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 10, 
			468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 188, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 186, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 212, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 428, 623, 623, 623, 623, 623, 429, 430, 623, 623, 623, 623, 431, -1, 432, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 433, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 22, 23, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 229, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 9, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 9, 9, -1, 9, -1, -1, -1, -1, -1 },
			{ -1, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 268, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 270, -1, 266, 266, 266, 266, 266, 266, 266, 266, 50, 266, -1 },
			{ -1, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 274, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 51, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 59, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 229, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, 27, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 215, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 290, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, -1, -1, 292, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, 49, -1, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, 49, -1, 49, -1, -1, -1, -1, -1 },
			{ -1, 610, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 67, -1, -1, -1, 67, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, 67, -1, -1, 67, -1, -1, -1, -1, -1, -1, 67, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 67, 67, -1, 67, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 68, 68, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 303, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 76, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 323, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 332, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, 332, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, 332, 623, -1, -1, -1, -1, -1 },
			{ -1, 614, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 539, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 631, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ 1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 364, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 366, 368, 198, 198, 198, 198, 198, 198, 198, 198, 143, 198, 1 },
			{ -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 378, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1, -1 },
			{ 1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 380, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 381, 368, 199, 199, 199, 199, 199, 199, 199, 151, 199, 199, 1 },
			{ 1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 153, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 383, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 384, 368, 200, 200, 200, 200, 201, -1, 200, 200, 200, 200, 187 },
			{ -1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, -1, 155, 155, 155, 155, 155, 155, 155, 155, -1, -1, 155, 155, -1, -1, -1, -1, -1, 155, -1, -1, -1, 155, 155, 155, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, 155, -1, 155, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, 391, -1, 391, 391, 391, 391, 391, 391, 391, 391, -1, -1, 391, 391, -1, -1, -1, -1, -1, 391, -1, -1, -1, 391, 391, 391, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 159, 159, 391, 391, -1, 391, -1, -1, -1, -1, -1 },
			{ -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 162, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 165, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, -1, 166, 166, 166, 166, 166, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, 172, -1, 172, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, -1, 174, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 179, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 179, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 179, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 403, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 183, -1, -1, -1, -1, -1, -1 },
			{ -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1 },
			{ -1, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, -1, 386, 386, 386, 386, 386, 386, 386, 386, -1, -1, 386, -1, -1, -1, -1, -1, -1, 386, -1, -1, -1, 386, 386, 386, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 18, 623, 434, 623, 623, 435, 623, 623, 623, -1, 557, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 229, 262, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 264, -1, -1, -1, -1, -1, -1, 9, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 9, 9, -1, 9, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, 193, -1, 193, -1, -1, -1, -1, -1 },
			{ -1, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, -1, 196, 196, 196, 196, 196, 196, 196, 196, 372, -1, 196, 196, -1, -1, -1, -1, -1, 196, -1, -1, -1, 196, 196, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 146, 196, 196, -1, 196, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 364, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 370, -1, 198, 198, 198, 198, 198, 198, 198, 198, -1, 198, -1 },
			{ -1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 380, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 382, -1, 199, 199, 199, 199, 199, 199, 199, -1, 199, 199, -1 },
			{ -1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 153, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 383, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 385, -1, 200, 200, 200, 200, 201, -1, 200, 200, 200, 200, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 153, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, -1, -1, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 396, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 397, -1, -1, -1, -1, -1, -1, 174, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 174, 174, -1, 174, -1, -1, -1, -1, -1 },
			{ -1, 209, -1, -1, -1, 209, 209, -1, -1, 209, -1, -1, -1, -1, -1, -1, -1, 209, -1, -1, 209, -1, -1, -1, -1, -1, -1, 209, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 209, 209, -1, 209, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, 182, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 217, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 437, 623, 233, 623, 623, 623, 623, 623, 623, 19, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 194, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 216, 216, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 220, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 20, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, 232, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, 27, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 245, 623, 623, 21, 559, 623, 623, -1, 623, 623, 623, 623, 586, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 6, 406, 190, 418, 213, 421, 423, 425, 556, 218, 585, 604, 615, 621, 7, 623, 221, 623, 625, 224, 623, 626, 627, 8, 191, 623, 9, 10, 214, 11, 219, 222, 427, 225, 7, 228, 623, 628, 623, 228, 231, 234, 12, 237, 240, 243, 246, 249, 252, 255, 258, 228, 13, 261, 14, 228, 192, 9, 7, 9, 228, 15, 16, 17, 1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 563, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, 226, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, 16, 17, -1 },
			{ -1, -1, -1, 235, -1, 238, 241, 409, -1, -1, 244, 247, 250, -1, -1, -1, -1, 253, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 259, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 408, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 52, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 276, -1, -1, 193, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 276, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 193, 193, -1, 193, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 53, 623, -1, 623, 457, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 12, 32, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 54, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, -1, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 410, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 55, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 56, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 38, 39, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 280, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 57, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 284, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 58, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, 42, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 411, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 61, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 286, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 69, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 419, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 70, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 288, -1, -1, -1, -1, -1, -1, 412, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 71, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 72, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 73, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 74, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 75, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, -1, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, -1, 266, 266, 266, 266, 266, -1 },
			{ -1, 623, 623, 623, 623, 623, 77, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, -1, 266, 266, 266, 266, 266, 266, 266, 266, 266, 266, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 78, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 79, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, 272, -1 },
			{ -1, 623, 623, 623, 80, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 81, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 296, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 82, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 413, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 83, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 298, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 84, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 414, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 85, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 302, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 86, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 306, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 87, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 88, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, -1, 308, 308, 308, 308, 308, 308, 308, 308, -1, -1, 308, -1, -1, -1, -1, -1, -1, 308, -1, 292, -1, 308, 308, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 310, 416, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 89, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 312, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 90, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 420, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 91, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 417, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 92, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 95, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 96, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 324, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 97, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 422, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 98, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 308, 94, 308, 308, 308, 308, 308, 308, 308, 308, -1, -1, 308, 308, -1, -1, -1, -1, -1, 308, -1, -1, -1, 308, 308, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 308, 308, 197, 308, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 99, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, -1, 328, 328, 328, 328, 328, 328, 328, 328, -1, -1, 328, -1, -1, -1, -1, -1, -1, 328, -1, -1, -1, 328, 328, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 100, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 334, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 101, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, 93, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 104, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 105, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 106, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 342, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 107, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, 102, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 108, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 109, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 348, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 350, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 110, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, 328, -1, 328, 328, 328, 328, 328, 328, 328, 328, -1, -1, 328, 328, -1, -1, -1, -1, -1, 328, -1, -1, -1, 328, 328, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 328, 328, -1, 328, -1, -1, 352, -1, -1 },
			{ -1, 111, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, -1, 330, 330, 330, 330, 330, 330, 330, 330, -1, -1, 330, 330, -1, -1, -1, -1, -1, 330, -1, -1, -1, 330, 330, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 330, 330, -1, 330, -1, -1, -1, 352, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 114, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 424, -1, -1, -1, -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 332, -1, -1, -1, -1, -1, -1 },
			{ -1, 115, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 426, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 116, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 117, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 338, 112, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 118, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 354, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 119, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 122, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 344, 113, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 123, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 124, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 125, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 350, 103, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 126, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 94, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 197, -1, -1, -1, -1, -1, -1 },
			{ -1, 127, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 354, 120, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 128, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 356, 121, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 129, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 350, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 130, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 131, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 135, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 132, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1, 198, 198, 198, 198, 198, -1 },
			{ -1, 623, 623, 623, 623, 623, 133, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 144, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 134, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, 196, -1, 196, 196, 196, 196, 196, 196, 196, 196, -1, -1, 196, -1, -1, -1, -1, -1, -1, 196, -1, -1, -1, 196, 196, 196, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 145, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 136, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1, 198, 198, 198, 198, 198, 198, 198, 198, 198, 198, -1 },
			{ -1, 137, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 374, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 138, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, -1, 147, 147, 147, 147, 147, 147, 147, 147, -1, -1, 147, -1, -1, -1, -1, -1, -1, 147, -1, -1, -1, 147, 147, 147, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 147 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 139, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ 1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 378, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 149, 1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 140, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 141, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, -1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, -1, 199, 199, 199, 199, 199, -1 },
			{ -1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 144, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, -1 },
			{ -1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, -1, 199, 199, 199, 199, 199, 199, 199, 199, 199, 199, -1 },
			{ -1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, -1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, -1, 200, 200, 200, 200, 200, -1 },
			{ -1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 144, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, -1 },
			{ -1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, -1, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, -1 },
			{ -1, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 386, 154, 386, 386, 386, 386, 386, 386, 386, 386, -1, -1, 386, 386, -1, -1, -1, -1, -1, 386, -1, -1, -1, 386, 386, 386, 387, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 386, 386, 202, 386, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 154, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 202, -1, -1, -1, -1, -1, -1 },
			{ 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1 },
			{ 1, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 155, 7, 155, 155, 155, 155, 155, 155, 155, 155, 156, 203, 155, 203, 203, 203, 203, 203, 203, 155, 203, 7, 203, 155, 155, 155, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 7, 203, 203, 203, 203, 203, 1 },
			{ 1, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 204, 158, 158, 158, 158, 158, 158, 158, 158, 204, 204, 158, 204, 204, 204, 204, 204, 204, 158, 204, 204, 204, 158, 158, 158, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 204, 1 },
			{ 1, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 161, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 1 },
			{ 1, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 164, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 163, 1 },
			{ 1, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 167, 166, 166, 166, 166, 166, 166, 166, 166, 166, 168, 166, 166, 166, 166, 166, 166, 206, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 206, 166, 166, 166, 166, 166, 166, 166, 166, 166, 166, 205, 166, 166, 166, 166, 166, 1 },
			{ 1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 170, 623, 623, 623, 623, 623, 623, 623, 623, 171, 171, 623, 172, 10, 171, 170, 171, 171, 623, 171, 170, 171, 623, 623, 623, 171, 171, 171, 170, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 207, 171, 171, 208, 172, 170, 172, 173, 171, 171, 170, 1 },
			{ 1, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 175, 1 },
			{ 1, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 176, 407 },
			{ -1, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 177, 400, 400, 400, 400, 400, 400, 400, 400, -1, -1, 400, 400, -1, -1, -1, -1, -1, 400, -1, -1, -1, 400, 400, 400, 401, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 400, 400, 210, 400, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 177, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 210, -1, -1, -1, -1, -1, -1 },
			{ 1, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 180, 179, 178, 178, 178, 178, 178, 211, 178, 181, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 178, 1 },
			{ 1, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 184, 178, 178, 178, 178, 211, 178, 181, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 178, 1 },
			{ 1, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 185, 211, 178, 181, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 179, 178, 178, 178, 178, 178, 1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 227, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, 400, -1, 400, 400, 400, 400, 400, 400, 400, 400, -1, -1, 400, -1, -1, -1, -1, -1, -1, 400, -1, -1, -1, 400, 400, 400, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 282, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 294, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 300, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 415, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 326, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, 330, -1, 330, 330, 330, 330, 330, 330, 330, 330, -1, -1, 330, -1, -1, -1, -1, -1, -1, 330, -1, -1, -1, 330, 330, 330, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 338, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 230, 623, 623, -1, 623, 623, 436, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 336, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 438, 623, 623, 623, 560, 623, 623, 236, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 346, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 558, 623, 623, 239, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 242, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 439, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 248, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 251, 588, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 452, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 254, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 257, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 453, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 260, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 605, 623, 623, 623, 623, 454, 623, 455, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 456, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 458, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 561, 623, 623, 589, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 459, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 616, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 463, 623, 623, 623, 623, -1, 623, 464, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 465, 623, 623, 623, 623, 623, 623, 263, 623, 623, 633, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 566, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 590, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 466, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 567, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 467, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 265, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 267, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 564, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 606, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 471, 623, 623, 623, 623, 623, 623, 618, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 472, 473, 474, 623, 475, 617, 623, 623, 623, 623, 569, -1, 476, 623, 570, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 269, 623, 571, 478, 623, 623, 623, 623, 479, 623, 623, 623, -1, 623, 623, 623, 480, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 271, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 591, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 481, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 273, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 275, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 277, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 279, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 482, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 281, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 593, 623, 623, 623, 623, 623, 623, 283, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 285, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 287, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 289, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 486, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 291, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 293, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 295, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 297, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 299, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 622, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 624, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 489, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 490, 623, 623, 623, 572, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 491, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 573, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 493, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 301, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 495, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 577, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 575, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 498, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 598, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 502, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 305, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 307, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 309, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 311, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 313, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 506, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 507, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 579, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 508, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 315, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 511, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 581, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 513, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 317, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 515, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 319, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 321, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 325, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 582, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 518, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 327, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 329, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 331, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 520, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 600, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 522, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 523, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 602, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 333, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 525, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 526, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 527, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 335, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 337, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 339, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 341, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 343, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 345, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 535, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 536, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 347, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 349, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 351, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 540, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 541, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 353, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 355, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 357, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 634, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 542, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 359, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 543, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 583, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 361, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 363, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 544, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 365, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 367, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 545, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 369, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 547, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 550, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 551, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 371, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 373, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 375, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 552, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 553, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 377, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 554, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 555, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 379, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 587, 623, 623, 623, 440, -1, 623, 441, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 565, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 461, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 468, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 460, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 608, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 469, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 470, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 487, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 595, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 484, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 609, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 496, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 596, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 574, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 494, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 632, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 509, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 510, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 514, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 601, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 512, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 517, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 580, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 533, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 524, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 529, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 546, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 548, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 442, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 443, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 607, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 462, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 477, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 594, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 485, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 497, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 597, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 578, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 500, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 629, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 599, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 519, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 516, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 521, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 534, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 530, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 537, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 549, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 444, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 568, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 488, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 592, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 499, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 504, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 501, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 576, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 528, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 531, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 538, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 445, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 483, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 492, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 611, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 503, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 532, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 446, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 505, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 630, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 562, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 447, 623, 623, 623, 448, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 449, 623, 623, 623, 623, 450, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 451, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 612, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 613, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 584, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 620, 623, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 623, 619, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 },
			{ -1, 623, 623, 623, 623, 623, 623, 623, 623, 623, 603, 623, 623, 623, 623, -1, 623, 623, 623, 623, 623, 623, 623, 623, -1, -1, 623, 623, -1, -1, -1, -1, -1, 623, -1, -1, -1, 623, 623, 623, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 623, 623, -1, 623, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  223,
			  142,
			  376,
			  150,
			  152,
			  388,
			  389,
			  390,
			  392,
			  393,
			  394,
			  395,
			  398,
			  399,
			  402,
			  404,
			  405
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
				
				if (lookahead == EOF && is_initial_state)
				{
					return Tokens.EOF;
				}
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 635);
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

