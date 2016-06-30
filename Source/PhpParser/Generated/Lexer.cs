namespace PhpParser.Parser
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
using PHP.Core;
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
		
		public Lexer(System.IO.TextReader reader)
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
					// #line 649
					{
						return Tokens.ERROR;
					}
					break;
					
				case 3:
					// #line 640
					{
						if (CG(short_tags)) {
							BEGIN(LexicalStates.ST_IN_SCRIPTING);
							return (Tokens.T_OPEN_TAG);
						} else {
							goto inline_char_handler;
						}
					}
					break;
					
				case 4:
					// #line 627
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG_WITH_ECHO);
					}
					break;
					
				case 5:
					// #line 633
					{
						HANDLE_NEWLINE(yytext[yyleng-1]);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_OPEN_TAG);
					}
					break;
					
				case 6:
					// #line 1182
					{
						if (YYCURSOR > YYLIMIT) {
							return (Tokens.END);
						}
						zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
						goto restart;
					}
					break;
					
				case 7:
					// #line 690
					{
						zend_copy_value(zendlval, yytext, yyleng);
						return (Tokens.T_STRING);
					}
					break;
					
				case 8:
					// #line 231
					{
						HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_WHITESPACE);
					}
					break;
					
				case 9:
					// #line 516
					{
						return (Tokens.yytext[0]);
					}
					break;
					
				case 10:
					// #line 561
					{
						return ProcessDecimalNumber();
					}
					break;
					
				case 11:
					// #line 256
					{
						return (Tokens.T_NS_SEPARATOR);
					}
					break;
					
				case 12:
					// #line 521
					{
						yy_push_state(LexicalStates.ST_IN_SCRIPTING); 
						return Tokens.T_LBRACE;
					}
					break;
					
				case 13:
					// #line 533
					{
						RESET_DOC_COMMENT();
						if (!yy_pop_state()) 
							return Tokens.T_ERROR; 
						return Tokens.T_RBRACE;
					}
					break;
					
				case 14:
					// #line 920
					{
						BEGIN(LexicalStates.ST_BACKQUOTE); 
						return Tokens.T_BACKQUOTE; 
					}
					break;
					
				case 15:
					// #line 764
					{
						register char *s, *t;
						char *end;
						int bprefix = (yytext[0] != '\'') ? 1 : 0;
						while (1) {
							if (YYCURSOR < YYLIMIT) {
								if (*YYCURSOR == '\'') {
									YYCURSOR++;
									yyleng = YYCURSOR - SCNG(yy_text);
									break;
								} else if (*YYCURSOR++ == '\\' && YYCURSOR < YYLIMIT) {
									YYCURSOR++;
								}
							} else {
								yyleng = YYLIMIT - SCNG(yy_text);
								ZVAL_NULL(zendlval);
								return (Tokens.T_ENCAPSED_AND_WHITESPACE);
							}
						}
						ZVAL_STRINGL(zendlval, yytext+bprefix+1, yyleng-bprefix-2);
						/* convert escape sequences */
						s = t = Z_STRVAL_P(zendlval);
						end = s+Z_STRLEN_P(zendlval);
						while (s<end) {
							if (*s=='\\') {
								s++;
								switch(*s) {
									case '\\':
									case '\'':
										*t++ = *s;
										Z_STRLEN_P(zendlval)--;
										break;
									default:
										*t++ = '\\';
										*t++ = *s;
										break;
								}
							} else {
								*t++ = *s;
							}
							if (*s == '\n' || (*s == '\r' && (*(s+1) != '\n'))) {
								CG(zend_lineno)++;
							}
							s++;
						}
						*t = 0;
						if (SCNG(output_filter)) {
							size_t sz = 0;
							char *str = NULL;
							s = Z_STRVAL_P(zendlval);
							// TODO: avoid reallocation ???
							SCNG(output_filter)((unsigned char **)&str, &sz, (unsigned char *)s, (size_t)Z_STRLEN_P(zendlval));
							ZVAL_STRINGL(zendlval, str, sz);
						}
						return (Tokens.T_CONSTANT_ENCAPSED_STRING);
					}
					break;
					
				case 16:
					// #line 696
					{
						while (YYCURSOR < YYLIMIT) {
							switch (*YYCURSOR++) {
								case '\r':
									if (*YYCURSOR == '\n') {
										YYCURSOR++;
									}
									/* fall through */
								case '\n':
									CG(zend_lineno)++;
									break;
								case '?':
									if (*YYCURSOR == '>') {
										YYCURSOR--;
										break;
									}
									/* fall through */
								default:
									continue;
							}
							break;
						}
						yyleng = YYCURSOR - SCNG(yy_text);
						return (Tokens.T_COMMENT);
					}
					break;
					
				case 17:
					// #line 830
					{
						int bprefix = (yytext[0] != '"') ? 1 : 0;
						while (YYCURSOR < YYLIMIT) {
							switch (*YYCURSOR++) {
								case '"':
									yyleng = YYCURSOR - SCNG(yy_text);
									zend_scan_escape_string(zendlval, yytext+bprefix+1, yyleng-bprefix-2, '"');
									return (Tokens.T_CONSTANT_ENCAPSED_STRING);
								case '$':
									if (IS_LABEL_START(*YYCURSOR) || *YYCURSOR == '{') {
										break;
									}
									continue;
								case '{':
									if (*YYCURSOR == '$') {
										break;
									}
									continue;
								case '\\':
									if (YYCURSOR < YYLIMIT) {
										YYCURSOR++;
									}
									/* fall through */
								default:
									continue;
							}
							YYCURSOR--;
							break;
						}
						/* Remember how much was scanned to save rescanning */
						SET_DOUBLE_QUOTES_SCANNED_LENGTH(YYCURSOR - SCNG(yy_text) - yyleng);
						YYCURSOR = SCNG(yy_text) + yyleng;
						BEGIN(ST_DOUBLE_QUOTES);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 18:
					// #line 110
					{
						return (Tokens.T_IF);
					}
					break;
					
				case 19:
					// #line 134
					{
						return (Tokens.T_DO);
					}
					break;
					
				case 20:
					// #line 496
					{
						return (Tokens.T_LOGICAL_OR);
					}
					break;
					
				case 21:
					// #line 166
					{
						return (Tokens.T_AS);
					}
					break;
					
				case 22:
					// #line 404
					{
						return (Tokens.T_DEC);
					}
					break;
					
				case 23:
					// #line 226
					{
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 24:
					// #line 440
					{
						return (Tokens.T_MINUS_EQUAL);
					}
					break;
					
				case 25:
					// #line 512
					{
						return (Tokens.T_SR);
					}
					break;
					
				case 26:
					// #line 432
					{
						return (Tokens.T_IS_GREATER_OR_EQUAL);
					}
					break;
					
				case 27:
					// #line 590
					{
						return ProcessRealNumber();
					}
					break;
					
				case 28:
					// #line 252
					{
						return (Tokens.T_PAAMAYIM_NEKUDOTAYIM);
					}
					break;
					
				case 29:
					// #line 460
					{
						return (Tokens.T_CONCAT_EQUAL);
					}
					break;
					
				case 30:
					// #line 758
					{
						BEGIN(LexicalStates.INITIAL);
						return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
					}
					break;
					
				case 31:
					// #line 264
					{
						return (Tokens.T_COALESCE);
					}
					break;
					
				case 32:
					// #line 384
					{
						return (Tokens.T_DOUBLE_ARROW);
					}
					break;
					
				case 33:
					// #line 416
					{
						return (Tokens.T_IS_EQUAL);
					}
					break;
					
				case 34:
					// #line 436
					{
						return (Tokens.T_PLUS_EQUAL);
					}
					break;
					
				case 35:
					// #line 400
					{
						return (Tokens.T_INC);
					}
					break;
					
				case 36:
					// #line 420
					{
						return (Tokens.T_IS_NOT_EQUAL);
					}
					break;
					
				case 37:
					// #line 428
					{
						return (Tokens.T_IS_SMALLER_OR_EQUAL);
					}
					break;
					
				case 38:
					// #line 508
					{
						return (Tokens.T_SL);
					}
					break;
					
				case 39:
					// #line 444
					{
						return (Tokens.T_MUL_EQUAL);
					}
					break;
					
				case 40:
					// #line 448
					{
						return (Tokens.T_POW);
					}
					break;
					
				case 41:
					// #line 456
					{
						return (Tokens.T_DIV_EQUAL);
					}
					break;
					
				case 42:
					// #line 725
					{
						int doc_com;
						if (yyleng > 2) {
							doc_com = 1;
							RESET_DOC_COMMENT();
						} else {
							doc_com = 0;
						}
						while (YYCURSOR < YYLIMIT) {
							if (*YYCURSOR++ == '*' && *YYCURSOR == '/') {
								break;
							}
						}
						if (YYCURSOR < YYLIMIT) {
							YYCURSOR++;
						} else {
							zend_error(E_COMPILE_WARNING, "Unterminated comment starting line %d", CG(zend_lineno));
						}
						yyleng = YYCURSOR - SCNG(yy_text);
						HANDLE_NEWLINES(yytext, yyleng);
						if (doc_com) {
							CG(doc_comment) = zend_string_init(yytext, yyleng, 0);
							return (Tokens.T_DOC_COMMENT);
						}
						return (Tokens.T_COMMENT);
					}
					break;
					
				case 43:
					// #line 464
					{
						return (Tokens.T_MOD_EQUAL);
					}
					break;
					
				case 44:
					// #line 476
					{
						return (Tokens.T_AND_EQUAL);
					}
					break;
					
				case 45:
					// #line 492
					{
						return (Tokens.T_BOOLEAN_AND);
					}
					break;
					
				case 46:
					// #line 480
					{
						return (Tokens.T_OR_EQUAL);
					}
					break;
					
				case 47:
					// #line 488
					{
						return (Tokens.T_BOOLEAN_OR);
					}
					break;
					
				case 48:
					// #line 484
					{
						return (Tokens.T_XOR_EQUAL);
					}
					break;
					
				case 49:
					// #line 667
					{
						zend_copy_value(zendlval, (yytext+1), (yyleng-1));
						return (Tokens.T_VARIABLE);
					}
					break;
					
				case 50:
					// #line 504
					{
						return (Tokens.T_LOGICAL_XOR);
					}
					break;
					
				case 51:
					// #line 94
					{
						return (Tokens.T_TRY);
					}
					break;
					
				case 52:
					// #line 69
					{
						return (Tokens.T_EXIT);
					}
					break;
					
				case 53:
					// #line 138
					{
						return (Tokens.T_FOR);
					}
					break;
					
				case 54:
					// #line 332
					{
						return (Tokens.T_USE);
					}
					break;
					
				case 55:
					// #line 268
					{
						return (Tokens.T_NEW);
					}
					break;
					
				case 56:
					// #line 500
					{
						return (Tokens.T_LOGICAL_AND);
					}
					break;
					
				case 57:
					// #line 472
					{
						return (Tokens.T_SR_EQUAL);
					}
					break;
					
				case 58:
					// #line 260
					{
						return (Tokens.T_ELLIPSIS);
					}
					break;
					
				case 59:
					// #line 276
					{
						return (Tokens.T_VAR);
					}
					break;
					
				case 60:
					// #line 408
					{
						return (Tokens.T_IS_IDENTICAL);
					}
					break;
					
				case 61:
					// #line 412
					{
						return (Tokens.T_IS_NOT_IDENTICAL);
					}
					break;
					
				case 62:
					// #line 424
					{
						return (Tokens.T_SPACESHIP);
					}
					break;
					
				case 63:
					// #line 468
					{
						return (Tokens.T_SL_EQUAL);
					}
					break;
					
				case 64:
					// #line 452
					{
						return (Tokens.T_POW_EQUAL);
					}
					break;
					
				case 65:
					// #line 565
					{
						return ProcessHexadecimalNumber();
					}
					break;
					
				case 66:
					// #line 557
					{
						return ProcessBinaryNumber();
					}
					break;
					
				case 67:
					// #line 65
					{ 
						return (Tokens.T_EXIT); 
					}
					break;
					
				case 68:
					// #line 198
					{
						return (Tokens.T_ECHO);
					}
					break;
					
				case 69:
					// #line 122
					{
						return (Tokens.T_ELSE);
					}
					break;
					
				case 70:
					// #line 308
					{
						return (Tokens.T_EVAL);
					}
					break;
					
				case 71:
					// #line 178
					{
						return (Tokens.T_CASE);
					}
					break;
					
				case 72:
					// #line 388
					{
						return (Tokens.T_LIST);
					}
					break;
					
				case 73:
					// #line 194
					{
						return (Tokens.T_GOTO);
					}
					break;
					
				case 74:
					// #line 118
					{
						return (Tokens.T_ENDIF);
					}
					break;
					
				case 75:
					// #line 348
					{
						return (Tokens.T_EMPTY);
					}
					break;
					
				case 76:
					// #line 344
					{
						return (Tokens.T_ISSET);
					}
					break;
					
				case 77:
					// #line 214
					{
						return (Tokens.T_TRAIT);
					}
					break;
					
				case 78:
					// #line 106
					{
						return (Tokens.T_THROW);
					}
					break;
					
				case 79:
					// #line 364
					{
						return (Tokens.T_FINAL);
					}
					break;
					
				case 80:
					// #line 380
					{
						return (Tokens.T_UNSET);
					}
					break;
					
				case 81:
					// #line 77
					{
						return (Tokens.T_CONST);
					}
					break;
					
				case 82:
					// #line 272
					{
						return (Tokens.T_CLONE);
					}
					break;
					
				case 83:
					// #line 206
					{
						return (Tokens.T_CLASS);
					}
					break;
					
				case 84:
					// #line 98
					{
						return (Tokens.T_CATCH);
					}
					break;
					
				case 85:
					// #line 90
					{
						return (Tokens.T_YIELD);
					}
					break;
					
				case 86:
					// #line 392
					{
						return (Tokens.T_ARRAY);
					}
					break;
					
				case 87:
					// #line 126
					{
						return (Tokens.T_WHILE);
					}
					break;
					
				case 88:
					// #line 186
					{
						return (Tokens.T_BREAK);
					}
					break;
					
				case 89:
					// #line 202
					{
						return (Tokens.T_PRINT);
					}
					break;
					
				case 90:
					// #line 280
					{
						return (Tokens.T_INT_CAST);
					}
					break;
					
				case 91:
					// #line 872
					{
						char *s;
						int bprefix = (yytext[0] != '<') ? 1 : 0;
						zend_heredoc_label *heredoc_label = emalloc(sizeof(zend_heredoc_label));
						CG(zend_lineno)++;
						heredoc_label->length = yyleng-bprefix-3-1-(yytext[yyleng-2]=='\r'?1:0);
						s = yytext+bprefix+3;
						while ((*s == ' ') || (*s == '\t')) {
							s++;
							heredoc_label->length--;
						}
						if (*s == '\'') {
							s++;
							heredoc_label->length -= 2;
							BEGIN(ST_NOWDOC);
						} else {
							if (*s == '"') {
								s++;
								heredoc_label->length -= 2;
							}
							BEGIN(ST_HEREDOC);
						}
						heredoc_label->label = estrndup(s, heredoc_label->length);
						/* Check for ending label on the next line */
						if (heredoc_label->length < YYLIMIT - YYCURSOR && !memcmp(YYCURSOR, s, heredoc_label->length)) {
							YYCTYPE *end = YYCURSOR + heredoc_label->length;
							if (*end == ';') {
								end++;
							}
							if (*end == '\n' || *end == '\r') {
								BEGIN(ST_END_HEREDOC);
							}
						}
						zend_ptr_stack_push(&SCNG(heredoc_label_stack), (void *) heredoc_label);
						return (Tokens.T_START_HEREDOC);
					}
					break;
					
				case 92:
					// #line 142
					{
						return (Tokens.T_ENDFOR);
					}
					break;
					
				case 93:
					// #line 114
					{
						return (Tokens.T_ELSEIF);
					}
					break;
					
				case 94:
					// #line 356
					{
						return (Tokens.T_STATIC);
					}
					break;
					
				case 95:
					// #line 170
					{
						return (Tokens.T_SWITCH);
					}
					break;
					
				case 96:
					// #line 81
					{
						return (Tokens.T_RETURN);
					}
					break;
					
				case 97:
					// #line 340
					{
						return (Tokens.T_GLOBAL);
					}
					break;
					
				case 98:
					// #line 376
					{
						return (Tokens.T_PUBLIC);
					}
					break;
					
				case 99:
					// #line 284
					{
						return (Tokens.T_DOUBLE_CAST);
					}
					break;
					
				case 100:
					// #line 300
					{
						return (Tokens.T_BOOL_CAST);
					}
					break;
					
				case 101:
					// #line 218
					{
						return (Tokens.T_EXTENDS);
					}
					break;
					
				case 102:
					// #line 312
					{
						return (Tokens.T_INCLUDE);
					}
					break;
					
				case 103:
					// #line 182
					{
						return (Tokens.T_DEFAULT);
					}
					break;
					
				case 104:
					// #line 154
					{
						return (Tokens.T_DECLARE);
					}
					break;
					
				case 105:
					// #line 102
					{
						return (Tokens.T_FINALLY);
					}
					break;
					
				case 106:
					// #line 146
					{
						return (Tokens.T_FOREACH);
					}
					break;
					
				case 107:
					// #line 320
					{
						return (Tokens.T_REQUIRE);
					}
					break;
					
				case 108:
					// #line 368
					{
						return (Tokens.T_PRIVATE);
					}
					break;
					
				case 109:
					// #line 304
					{
						return (Tokens.T_UNSET_CAST);
					}
					break;
					
				case 110:
					// #line 292
					{
						return (Tokens.T_ARRAY_CAST);
					}
					break;
					
				case 111:
					// #line 618
					{
						return (Tokens.T_DIR);
					}
					break;
					
				case 112:
					// #line 130
					{
						return (Tokens.T_ENDWHILE);
					}
					break;
					
				case 113:
					// #line 73
					{
						return (Tokens.T_FUNCTION);
					}
					break;
					
				case 114:
					// #line 190
					{
						return (Tokens.T_CONTINUE);
					}
					break;
					
				case 115:
					// #line 396
					{
						return (Tokens.T_CALLABLE);
					}
					break;
					
				case 116:
					// #line 360
					{
						return (Tokens.T_ABSTRACT);
					}
					break;
					
				case 117:
					// #line 296
					{
						return (Tokens.T_OBJECT_CAST);
					}
					break;
					
				case 118:
					// #line 288
					{
						return (Tokens.T_STRING_CAST);
					}
					break;
					
				case 119:
					// #line 614
					{
						return (Tokens.T_FILE);
					}
					break;
					
				case 120:
					// #line 610
					{
						return (Tokens.T_LINE);
					}
					break;
					
				case 121:
					// #line 174
					{
						return (Tokens.T_ENDSWITCH);
					}
					break;
					
				case 122:
					// #line 210
					{
						return (Tokens.T_INTERFACE);
					}
					break;
					
				case 123:
					// #line 336
					{
					    return (Tokens.T_INSTEADOF);
					}
					break;
					
				case 124:
					// #line 328
					{
						return (Tokens.T_NAMESPACE);
					}
					break;
					
				case 125:
					// #line 372
					{
						return (Tokens.T_PROTECTED);
					}
					break;
					
				case 126:
					// #line 598
					{
						return (Tokens.T_TRAIT_C);
					}
					break;
					
				case 127:
					// #line 594
					{
						return (Tokens.T_CLASS_C);
					}
					break;
					
				case 128:
					// #line 158
					{
						return (Tokens.T_ENDDECLARE);
					}
					break;
					
				case 129:
					// #line 150
					{
						return (Tokens.T_ENDFOREACH);
					}
					break;
					
				case 130:
					// #line 162
					{
						return (Tokens.T_INSTANCEOF);
					}
					break;
					
				case 131:
					// #line 222
					{
						return (Tokens.T_IMPLEMENTS);
					}
					break;
					
				case 132:
					// #line 85
					{
						HANDLE_NEWLINES(yytext, yyleng);
						return (Tokens.T_YIELD_FROM);
					}
					break;
					
				case 133:
					// #line 606
					{
						return (Tokens.T_METHOD_C);
					}
					break;
					
				case 134:
					// #line 316
					{
						return (Tokens.T_INCLUDE_ONCE);
					}
					break;
					
				case 135:
					// #line 324
					{
						return (Tokens.T_REQUIRE_ONCE);
					}
					break;
					
				case 136:
					// #line 602
					{
						return (Tokens.T_FUNC_C);
					}
					break;
					
				case 137:
					// #line 622
					{
						return (Tokens.T_NS_C);
					}
					break;
					
				case 138:
					// #line 352
					{
						return (Tokens.T_HALT_COMPILER);
					}
					break;
					
				case 139:
					// #line 959
					{
						if (GET_DOUBLE_QUOTES_SCANNED_LENGTH()) {
							YYCURSOR += GET_DOUBLE_QUOTES_SCANNED_LENGTH() - 1;
							SET_DOUBLE_QUOTES_SCANNED_LENGTH(0);
							goto double_quotes_scan_done;
						}
						if (YYCURSOR > YYLIMIT) {
							return (Tokens.END);
						}
						if (yytext[0] == '\\' && YYCURSOR < YYLIMIT) {
							YYCURSOR++;
						}
						while (YYCURSOR < YYLIMIT) {
							switch (*YYCURSOR++) {
								case '"':
									break;
								case '$':
									if (IS_LABEL_START(*YYCURSOR) || *YYCURSOR == '{') {
										break;
									}
									continue;
								case '{':
									if (*YYCURSOR == '$') {
										break;
									}
									continue;
								case '\\':
									if (YYCURSOR < YYLIMIT) {
										YYCURSOR++;
									}
									/* fall through */
								default:
									continue;
							}
							YYCURSOR--;
							break;
						}
					double_quotes_scan_done:
						yyleng = YYCURSOR - SCNG(yy_text);
						zend_scan_escape_string(zendlval, yytext, yyleng, '"');
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 140:
					// #line 948
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_DOUBLE_QUOTES);
					}
					break;
					
				case 141:
					// #line 940
					{
						Z_LVAL_P(zendlval) = (zend_long) '{';
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						yyless(1);
						return (Tokens.T_CURLY_OPEN);
					}
					break;
					
				case 142:
					// #line 527
					{
						yy_push_state(ST_LOOKING_FOR_VARNAME);
						return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
					}
					break;
					
				case 143:
					// #line 660
					{
						yyless(yyleng - 1);
						yy_push_state(ST_VAR_OFFSET);
						zend_copy_value(zendlval, (yytext+1), (yyleng-1));
						return (Tokens.T_VARIABLE);
					}
					break;
					
				case 144:
					// #line 653
					{
						yyless(yyleng - 3);
						yy_push_state(LexicalStates.ST_LOOKING_FOR_PROPERTY);
						zend_copy_value(zendlval, (yytext+1), (yyleng-1));
						return (Tokens.T_VARIABLE);
					}
					break;
					
				case 145:
					// #line 1009
					{
						if (YYCURSOR > YYLIMIT) {
							return (Tokens.END);
						}
						if (yytext[0] == '\\' && YYCURSOR < YYLIMIT) {
							YYCURSOR++;
						}
						while (YYCURSOR < YYLIMIT) {
							switch (*YYCURSOR++) {
								case '`':
									break;
								case '$':
									if (IS_LABEL_START(*YYCURSOR) || *YYCURSOR == '{') {
										break;
									}
									continue;
								case '{':
									if (*YYCURSOR == '$') {
										break;
									}
									continue;
								case '\\':
									if (YYCURSOR < YYLIMIT) {
										YYCURSOR++;
									}
									/* fall through */
								default:
									continue;
							}
							YYCURSOR--;
							break;
						}
						yyleng = YYCURSOR - SCNG(yy_text);
						zend_scan_escape_string(zendlval, yytext, yyleng, '`');
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 146:
					// #line 953
					{
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_BACKQUOTE);
					}
					break;
					
				case 147:
					// #line 1051
					{
						int newline = 0;
						zend_heredoc_label *heredoc_label = zend_ptr_stack_top(&SCNG(heredoc_label_stack));
						if (YYCURSOR > YYLIMIT) {
							return (Tokens.END);
						}
						YYCURSOR--;
						while (YYCURSOR < YYLIMIT) {
							switch (*YYCURSOR++) {
								case '\r':
									if (*YYCURSOR == '\n') {
										YYCURSOR++;
									}
									/* fall through */
								case '\n':
									/* Check for ending label on the next line */
									if (IS_LABEL_START(*YYCURSOR) && heredoc_label->length < YYLIMIT - YYCURSOR && !memcmp(YYCURSOR, heredoc_label->label, heredoc_label->length)) {
										YYCTYPE *end = YYCURSOR + heredoc_label->length;
										if (*end == ';') {
											end++;
										}
										if (*end == '\n' || *end == '\r') {
											/* newline before label will be subtracted from returned text, but
											 * yyleng/yytext will include it, for zend_highlight/strip, tokenizer, etc. */
											if (YYCURSOR[-2] == '\r' && YYCURSOR[-1] == '\n') {
												newline = 2; /* Windows newline */
											} else {
												newline = 1;
											}
											CG(increment_lineno) = 1; /* For newline before label */
											BEGIN(ST_END_HEREDOC);
											goto heredoc_scan_done;
										}
									}
									continue;
								case '$':
									if (IS_LABEL_START(*YYCURSOR) || *YYCURSOR == '{') {
										break;
									}
									continue;
								case '{':
									if (*YYCURSOR == '$') {
										break;
									}
									continue;
								case '\\':
									if (YYCURSOR < YYLIMIT && *YYCURSOR != '\n' && *YYCURSOR != '\r') {
										YYCURSOR++;
									}
									/* fall through */
								default:
									continue;
							}
							YYCURSOR--;
							break;
						}
					heredoc_scan_done:
						yyleng = YYCURSOR - SCNG(yy_text);
						zend_scan_escape_string(zendlval, yytext, yyleng - newline, 0);
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 148:
					// #line 246
					{
						yyless(0);
						yy_pop_state();
						goto restart;
					}
					break;
					
				case 149:
					// #line 240
					{
						yy_pop_state();
						zend_copy_value(zendlval, yytext, yyleng);
						return (Tokens.T_STRING);
					}
					break;
					
				case 150:
					// #line 236
					{
						return (Tokens.T_OBJECT_OPERATOR);
					}
					break;
					
				case 151:
					// #line 550
					{
						yyless(0);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						goto restart;
					}
					break;
					
				case 152:
					// #line 541
					{
						yyless(yyleng - 1);
						zend_copy_value(zendlval, yytext, yyleng);
						yy_pop_state();
						yy_push_state(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_STRING_VARNAME);
					}
					break;
					
				case 153:
					// #line 682
					{
						/* Invalid rule to return a more explicit parse error with proper line number */
						yyless(0);
						yy_pop_state();
						ZVAL_NULL(zendlval);
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 154:
					// #line 677
					{
						/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
						return (Tokens.yytext[0]);
					}
					break;
					
				case 155:
					// #line 569
					{ /* Offset could be treated as a long */
						if (yyleng < MAX_LENGTH_OF_LONG - 1 || (yyleng == MAX_LENGTH_OF_LONG - 1 && strcmp(yytext, long_min_digits) < 0)) {
							char *end;
							errno = 0;
							ZVAL_LONG(zendlval, ZEND_STRTOL(yytext, &end, 10));
							if (errno == ERANGE) {
								goto string;
							}
							ZEND_ASSERT(end == yytext + yyleng);
						} else {
					string:
							ZVAL_STRINGL(zendlval, yytext, yyleng);
						}
						return (Tokens.T_NUM_STRING);
					}
					break;
					
				case 156:
					// #line 672
					{
						yy_pop_state();
						return (Tokens.T_RBRACKET);
					}
					break;
					
				case 157:
					// #line 585
					{ /* Offset must be treated as a string */
						ZVAL_STRINGL(zendlval, yytext, yyleng);
						return (Tokens.T_NUM_STRING);
					}
					break;
					
				case 158:
					// #line 926
					{
						zend_heredoc_label *heredoc_label = zend_ptr_stack_pop(&SCNG(heredoc_label_stack));
						YYCURSOR += heredoc_label->length - 1;
						yyleng = heredoc_label->length;
						heredoc_label_dtor(heredoc_label);
						efree(heredoc_label);
						BEGIN(LexicalStates.ST_IN_SCRIPTING);
						return (Tokens.T_END_HEREDOC);
					}
					break;
					
				case 159:
					// #line 1125
					{
						int newline = 0;
						zend_heredoc_label *heredoc_label = zend_ptr_stack_top(&SCNG(heredoc_label_stack));
						if (YYCURSOR > YYLIMIT) {
							return (Tokens.END);
						}
						YYCURSOR--;
						while (YYCURSOR < YYLIMIT) {
							switch (*YYCURSOR++) {
								case '\r':
									if (*YYCURSOR == '\n') {
										YYCURSOR++;
									}
									/* fall through */
								case '\n':
									/* Check for ending label on the next line */
									if (IS_LABEL_START(*YYCURSOR) && heredoc_label->length < YYLIMIT - YYCURSOR && !memcmp(YYCURSOR, heredoc_label->label, heredoc_label->length)) {
										YYCTYPE *end = YYCURSOR + heredoc_label->length;
										if (*end == ';') {
											end++;
										}
										if (*end == '\n' || *end == '\r') {
											/* newline before label will be subtracted from returned text, but
											 * yyleng/yytext will include it, for zend_highlight/strip, tokenizer, etc. */
											if (YYCURSOR[-2] == '\r' && YYCURSOR[-1] == '\n') {
												newline = 2; /* Windows newline */
											} else {
												newline = 1;
											}
											CG(increment_lineno) = 1; /* For newline before label */
											BEGIN(ST_END_HEREDOC);
											goto nowdoc_scan_done;
										}
									}
									/* fall through */
								default:
									continue;
							}
						}
					nowdoc_scan_done:
						yyleng = YYCURSOR - SCNG(yy_text);
						zend_copy_value(zendlval, yytext, yyleng - newline);
						HANDLE_NEWLINES(yytext, yyleng - newline);
						return (Tokens.T_ENCAPSED_AND_WHITESPACE);
					}
					break;
					
				case 161: goto case 1;
				case 163: goto case 5;
				case 164: goto case 7;
				case 165: goto case 9;
				case 166: goto case 10;
				case 167: goto case 27;
				case 168: goto case 30;
				case 169: goto case 36;
				case 170: goto case 42;
				case 171: goto case 49;
				case 172: goto case 91;
				case 173: goto case 139;
				case 174: goto case 145;
				case 175: goto case 147;
				case 176: goto case 148;
				case 177: goto case 151;
				case 178: goto case 154;
				case 179: goto case 155;
				case 180: goto case 157;
				case 182: goto case 7;
				case 183: goto case 9;
				case 184: goto case 30;
				case 185: goto case 139;
				case 186: goto case 145;
				case 187: goto case 147;
				case 188: goto case 157;
				case 190: goto case 7;
				case 191: goto case 9;
				case 193: goto case 7;
				case 194: goto case 9;
				case 196: goto case 7;
				case 197: goto case 9;
				case 199: goto case 7;
				case 200: goto case 9;
				case 202: goto case 7;
				case 203: goto case 9;
				case 205: goto case 7;
				case 206: goto case 9;
				case 208: goto case 7;
				case 209: goto case 9;
				case 211: goto case 7;
				case 212: goto case 9;
				case 214: goto case 7;
				case 215: goto case 9;
				case 217: goto case 7;
				case 218: goto case 9;
				case 220: goto case 7;
				case 221: goto case 9;
				case 223: goto case 7;
				case 224: goto case 9;
				case 226: goto case 7;
				case 227: goto case 9;
				case 229: goto case 7;
				case 230: goto case 9;
				case 232: goto case 7;
				case 233: goto case 9;
				case 235: goto case 7;
				case 237: goto case 7;
				case 239: goto case 7;
				case 241: goto case 7;
				case 243: goto case 7;
				case 245: goto case 7;
				case 247: goto case 7;
				case 249: goto case 7;
				case 251: goto case 7;
				case 253: goto case 7;
				case 255: goto case 7;
				case 257: goto case 7;
				case 259: goto case 7;
				case 261: goto case 7;
				case 263: goto case 7;
				case 265: goto case 7;
				case 267: goto case 7;
				case 269: goto case 7;
				case 271: goto case 7;
				case 273: goto case 7;
				case 275: goto case 7;
				case 277: goto case 7;
				case 279: goto case 7;
				case 281: goto case 7;
				case 283: goto case 7;
				case 285: goto case 7;
				case 287: goto case 7;
				case 289: goto case 7;
				case 291: goto case 7;
				case 293: goto case 7;
				case 295: goto case 7;
				case 297: goto case 7;
				case 299: goto case 7;
				case 301: goto case 7;
				case 303: goto case 7;
				case 305: goto case 7;
				case 307: goto case 7;
				case 309: goto case 7;
				case 311: goto case 7;
				case 313: goto case 7;
				case 315: goto case 7;
				case 317: goto case 7;
				case 319: goto case 7;
				case 321: goto case 7;
				case 323: goto case 7;
				case 325: goto case 7;
				case 327: goto case 7;
				case 329: goto case 7;
				case 331: goto case 7;
				case 333: goto case 7;
				case 335: goto case 7;
				case 337: goto case 7;
				case 339: goto case 7;
				case 341: goto case 7;
				case 343: goto case 7;
				case 345: goto case 7;
				case 347: goto case 7;
				case 349: goto case 7;
				case 351: goto case 7;
				case 352: goto case 7;
				case 363: goto case 7;
				case 366: goto case 7;
				case 368: goto case 7;
				case 370: goto case 7;
				case 372: goto case 7;
				case 373: goto case 7;
				case 374: goto case 7;
				case 375: goto case 7;
				case 376: goto case 7;
				case 377: goto case 7;
				case 378: goto case 7;
				case 379: goto case 7;
				case 380: goto case 7;
				case 381: goto case 7;
				case 382: goto case 7;
				case 383: goto case 7;
				case 384: goto case 7;
				case 385: goto case 7;
				case 386: goto case 7;
				case 387: goto case 7;
				case 388: goto case 7;
				case 389: goto case 7;
				case 390: goto case 7;
				case 391: goto case 7;
				case 392: goto case 7;
				case 393: goto case 7;
				case 394: goto case 7;
				case 395: goto case 7;
				case 396: goto case 7;
				case 397: goto case 7;
				case 398: goto case 7;
				case 399: goto case 7;
				case 400: goto case 7;
				case 401: goto case 7;
				case 402: goto case 7;
				case 403: goto case 7;
				case 404: goto case 7;
				case 405: goto case 7;
				case 406: goto case 7;
				case 407: goto case 7;
				case 408: goto case 7;
				case 409: goto case 7;
				case 410: goto case 7;
				case 411: goto case 7;
				case 412: goto case 7;
				case 413: goto case 7;
				case 414: goto case 7;
				case 415: goto case 7;
				case 416: goto case 7;
				case 417: goto case 7;
				case 418: goto case 7;
				case 419: goto case 7;
				case 420: goto case 7;
				case 421: goto case 7;
				case 422: goto case 7;
				case 423: goto case 7;
				case 424: goto case 7;
				case 425: goto case 7;
				case 426: goto case 7;
				case 427: goto case 7;
				case 428: goto case 7;
				case 429: goto case 7;
				case 430: goto case 7;
				case 431: goto case 7;
				case 432: goto case 7;
				case 433: goto case 7;
				case 434: goto case 7;
				case 435: goto case 7;
				case 436: goto case 7;
				case 437: goto case 7;
				case 438: goto case 7;
				case 439: goto case 7;
				case 440: goto case 7;
				case 441: goto case 7;
				case 442: goto case 7;
				case 443: goto case 7;
				case 444: goto case 7;
				case 445: goto case 7;
				case 446: goto case 7;
				case 447: goto case 7;
				case 448: goto case 7;
				case 449: goto case 7;
				case 450: goto case 7;
				case 451: goto case 7;
				case 452: goto case 7;
				case 453: goto case 7;
				case 454: goto case 7;
				case 455: goto case 7;
				case 456: goto case 7;
				case 457: goto case 7;
				case 458: goto case 7;
				case 459: goto case 7;
				case 460: goto case 7;
				case 461: goto case 7;
				case 462: goto case 7;
				case 463: goto case 7;
				case 464: goto case 7;
				case 465: goto case 7;
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
			AcceptConditions.NotAccept, // 160
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
			AcceptConditions.NotAccept, // 181
			AcceptConditions.Accept, // 182
			AcceptConditions.Accept, // 183
			AcceptConditions.Accept, // 184
			AcceptConditions.Accept, // 185
			AcceptConditions.Accept, // 186
			AcceptConditions.Accept, // 187
			AcceptConditions.Accept, // 188
			AcceptConditions.NotAccept, // 189
			AcceptConditions.Accept, // 190
			AcceptConditions.Accept, // 191
			AcceptConditions.NotAccept, // 192
			AcceptConditions.Accept, // 193
			AcceptConditions.Accept, // 194
			AcceptConditions.NotAccept, // 195
			AcceptConditions.Accept, // 196
			AcceptConditions.Accept, // 197
			AcceptConditions.NotAccept, // 198
			AcceptConditions.Accept, // 199
			AcceptConditions.Accept, // 200
			AcceptConditions.NotAccept, // 201
			AcceptConditions.Accept, // 202
			AcceptConditions.Accept, // 203
			AcceptConditions.NotAccept, // 204
			AcceptConditions.Accept, // 205
			AcceptConditions.Accept, // 206
			AcceptConditions.NotAccept, // 207
			AcceptConditions.Accept, // 208
			AcceptConditions.Accept, // 209
			AcceptConditions.NotAccept, // 210
			AcceptConditions.Accept, // 211
			AcceptConditions.Accept, // 212
			AcceptConditions.NotAccept, // 213
			AcceptConditions.Accept, // 214
			AcceptConditions.Accept, // 215
			AcceptConditions.NotAccept, // 216
			AcceptConditions.Accept, // 217
			AcceptConditions.Accept, // 218
			AcceptConditions.NotAccept, // 219
			AcceptConditions.Accept, // 220
			AcceptConditions.Accept, // 221
			AcceptConditions.NotAccept, // 222
			AcceptConditions.Accept, // 223
			AcceptConditions.Accept, // 224
			AcceptConditions.NotAccept, // 225
			AcceptConditions.Accept, // 226
			AcceptConditions.Accept, // 227
			AcceptConditions.NotAccept, // 228
			AcceptConditions.Accept, // 229
			AcceptConditions.Accept, // 230
			AcceptConditions.NotAccept, // 231
			AcceptConditions.Accept, // 232
			AcceptConditions.Accept, // 233
			AcceptConditions.NotAccept, // 234
			AcceptConditions.Accept, // 235
			AcceptConditions.NotAccept, // 236
			AcceptConditions.Accept, // 237
			AcceptConditions.NotAccept, // 238
			AcceptConditions.Accept, // 239
			AcceptConditions.NotAccept, // 240
			AcceptConditions.Accept, // 241
			AcceptConditions.NotAccept, // 242
			AcceptConditions.Accept, // 243
			AcceptConditions.NotAccept, // 244
			AcceptConditions.Accept, // 245
			AcceptConditions.NotAccept, // 246
			AcceptConditions.Accept, // 247
			AcceptConditions.NotAccept, // 248
			AcceptConditions.Accept, // 249
			AcceptConditions.NotAccept, // 250
			AcceptConditions.Accept, // 251
			AcceptConditions.NotAccept, // 252
			AcceptConditions.Accept, // 253
			AcceptConditions.NotAccept, // 254
			AcceptConditions.Accept, // 255
			AcceptConditions.NotAccept, // 256
			AcceptConditions.Accept, // 257
			AcceptConditions.NotAccept, // 258
			AcceptConditions.Accept, // 259
			AcceptConditions.NotAccept, // 260
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
			AcceptConditions.Accept, // 352
			AcceptConditions.NotAccept, // 353
			AcceptConditions.NotAccept, // 354
			AcceptConditions.NotAccept, // 355
			AcceptConditions.NotAccept, // 356
			AcceptConditions.NotAccept, // 357
			AcceptConditions.NotAccept, // 358
			AcceptConditions.NotAccept, // 359
			AcceptConditions.NotAccept, // 360
			AcceptConditions.NotAccept, // 361
			AcceptConditions.NotAccept, // 362
			AcceptConditions.Accept, // 363
			AcceptConditions.NotAccept, // 364
			AcceptConditions.NotAccept, // 365
			AcceptConditions.Accept, // 366
			AcceptConditions.NotAccept, // 367
			AcceptConditions.Accept, // 368
			AcceptConditions.NotAccept, // 369
			AcceptConditions.Accept, // 370
			AcceptConditions.NotAccept, // 371
			AcceptConditions.Accept, // 372
			AcceptConditions.Accept, // 373
			AcceptConditions.Accept, // 374
			AcceptConditions.Accept, // 375
			AcceptConditions.Accept, // 376
			AcceptConditions.Accept, // 377
			AcceptConditions.Accept, // 378
			AcceptConditions.Accept, // 379
			AcceptConditions.Accept, // 380
			AcceptConditions.Accept, // 381
			AcceptConditions.Accept, // 382
			AcceptConditions.Accept, // 383
			AcceptConditions.Accept, // 384
			AcceptConditions.Accept, // 385
			AcceptConditions.Accept, // 386
			AcceptConditions.Accept, // 387
			AcceptConditions.Accept, // 388
			AcceptConditions.Accept, // 389
			AcceptConditions.Accept, // 390
			AcceptConditions.Accept, // 391
			AcceptConditions.Accept, // 392
			AcceptConditions.Accept, // 393
			AcceptConditions.Accept, // 394
			AcceptConditions.Accept, // 395
			AcceptConditions.Accept, // 396
			AcceptConditions.Accept, // 397
			AcceptConditions.Accept, // 398
			AcceptConditions.Accept, // 399
			AcceptConditions.Accept, // 400
			AcceptConditions.Accept, // 401
			AcceptConditions.Accept, // 402
			AcceptConditions.Accept, // 403
			AcceptConditions.Accept, // 404
			AcceptConditions.Accept, // 405
			AcceptConditions.Accept, // 406
			AcceptConditions.Accept, // 407
			AcceptConditions.Accept, // 408
			AcceptConditions.Accept, // 409
			AcceptConditions.Accept, // 410
			AcceptConditions.Accept, // 411
			AcceptConditions.Accept, // 412
			AcceptConditions.Accept, // 413
			AcceptConditions.Accept, // 414
			AcceptConditions.Accept, // 415
			AcceptConditions.Accept, // 416
			AcceptConditions.Accept, // 417
			AcceptConditions.Accept, // 418
			AcceptConditions.Accept, // 419
			AcceptConditions.Accept, // 420
			AcceptConditions.Accept, // 421
			AcceptConditions.Accept, // 422
			AcceptConditions.Accept, // 423
			AcceptConditions.Accept, // 424
			AcceptConditions.Accept, // 425
			AcceptConditions.Accept, // 426
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
		};
		
		private static int[] colMap = new int[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 35, 15, 0, 0, 57, 0, 0, 
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
			35, 42, 62, 61, 52, 46, 47, 60, 34, 36, 44, 41, 50, 24, 31, 45, 
			55, 56, 27, 27, 27, 27, 27, 27, 27, 27, 29, 50, 43, 40, 25, 32, 
			50, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 28, 54, 30, 58, 49, 38, 
			59, 17, 20, 9, 5, 1, 6, 22, 18, 3, 37, 21, 14, 16, 8, 10, 
			23, 39, 12, 11, 4, 7, 33, 19, 2, 13, 28, 51, 48, 53, 50, 0, 
			26, 26
		};
		
		private static int[] rowMap = new int[]
		{
			0, 1, 1, 2, 1, 1, 1, 3, 4, 5, 6, 1, 1, 1, 1, 1, 
			1, 1, 7, 7, 7, 7, 1, 1, 1, 8, 1, 9, 1, 1, 10, 1, 
			1, 11, 1, 1, 12, 13, 14, 1, 15, 1, 16, 1, 1, 1, 1, 1, 
			1, 17, 7, 7, 7, 18, 7, 7, 7, 1, 1, 7, 1, 1, 1, 1, 
			1, 19, 20, 7, 7, 21, 7, 7, 7, 7, 7, 7, 7, 7, 7, 22, 
			7, 7, 7, 7, 7, 23, 7, 7, 7, 7, 1, 1, 24, 7, 7, 7, 
			7, 7, 7, 1, 1, 7, 25, 7, 7, 7, 7, 26, 7, 1, 1, 7, 
			7, 7, 7, 7, 7, 1, 1, 7, 7, 7, 7, 7, 7, 7, 7, 7, 
			7, 7, 7, 7, 1, 7, 7, 7, 7, 7, 7, 1, 1, 1, 1, 1, 
			1, 1, 1, 1, 1, 27, 1, 1, 1, 1, 1, 28, 1, 29, 1, 1, 
			30, 31, 32, 33, 34, 35, 36, 37, 1, 1, 38, 39, 40, 41, 41, 41, 
			42, 32, 43, 44, 45, 46, 47, 48, 49, 50, 50, 50, 51, 52, 53, 54, 
			55, 56, 57, 58, 59, 60, 61, 62, 1, 63, 64, 65, 66, 67, 68, 69, 
			70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 
			86, 87, 88, 89, 60, 90, 91, 19, 92, 43, 20, 93, 37, 94, 95, 96, 
			97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 38, 109, 110, 111, 
			112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 
			128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 
			144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 
			176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 
			192, 193, 194, 195, 32, 196, 197, 198, 45, 199, 51, 200, 201, 202, 203, 204, 
			205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 
			221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 
			237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 
			253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 
			269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 
			285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 
			301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 
			317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 
			333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 
			349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 
			365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 
			381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 
			397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 
			413, 414, 415, 416, 417, 418, 419, 420, 7, 421, 422, 423, 424, 425, 426, 427, 
			428, 429, 430, 431
		};
		
		private static int[,] nextState = new int[,]
		{
			{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 161, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 160, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 373, 568, 568, 568, 568, 568, 374, 375, 568, 568, 568, 568, 376, -1, 377, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 378, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 22, 23, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 57, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 198, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 184, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 61, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, 250, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 252, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, 49, 49, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 49, 49, -1, -1, -1, -1, -1, -1 },
			{ -1, 555, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 65, -1, -1, -1, 65, 65, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, 65, -1, -1, 65, -1, -1, -1, -1, -1, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 65, 65, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 66, 66, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 275, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 295, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 292, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, 292, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, 292, -1, -1, -1, -1, -1 },
			{ -1, 559, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 484, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 576, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, -1, 149, 149, 149, 149, 149, 149, 149, 149, -1, -1, 149, 149, 149, -1, -1, -1, -1, 149, -1, -1, -1, 149, 149, 149, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 149, 149, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 155, 155, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, 157, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 181, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 340, 340, 340, 340, 340, 340, 340, 340, 340, 340, 340, 340, 340, 340, -1, 340, 340, 340, 340, 340, 340, 340, 340, -1, -1, 340, 340, 340, -1, -1, -1, -1, 340, -1, -1, -1, 340, 340, 340, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 152, 152, 340, 340, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 18, 568, 379, 568, 568, 380, 568, 568, 568, -1, 502, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 198, 231, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 234, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, 167, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 170, -1, -1, -1, -1, -1 },
			{ -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, 326, -1, 171, 171, 171, -1, -1, -1, -1, 171, -1, -1, -1, 171, 171, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 143, 171, 171, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 91, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 141, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 150, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, -1, 49, 49, 49, 49, 49, 49, 49, 49, -1, -1, 49, -1, 49, -1, -1, -1, -1, 49, -1, -1, -1, 49, 49, 49, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, 344, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 346, -1, -1, -1, -1, -1, -1, 157, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 157, 157, -1, -1, -1, -1, -1, -1 },
			{ -1, 180, -1, -1, -1, 180, 180, -1, -1, 180, -1, -1, -1, -1, -1, -1, -1, 180, -1, -1, 180, -1, -1, -1, -1, -1, -1, 180, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 180, 180, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 189, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 382, 568, 205, 568, 568, 568, 568, 568, 568, 19, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 168, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, -1, 171, 171, 171, 171, 171, 171, 171, 171, -1, -1, 171, -1, 171, -1, -1, -1, -1, 171, -1, -1, -1, 171, 171, 171, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 142, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 188, 188, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 163, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 20, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, -1, -1, -1, 201, -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 27, 27, -1, -1, -1, -1, -1, -1 },
			{ 6, 7, 352, 164, 363, 182, 366, 368, 370, 501, 190, 530, 549, 560, 566, 8, 568, 193, 568, 570, 196, 568, 571, 572, 9, 165, 568, 10, 568, 183, 11, 191, 194, 372, 197, 8, 200, 568, 573, 568, 203, 206, 209, 212, 215, 218, 221, 224, 227, 230, 200, 12, 233, 13, 200, 166, 10, 8, 200, 14, 15, 16, 17 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 217, 568, 568, 21, 504, 568, 568, -1, 568, 568, 568, 568, 531, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, -1, -1, -1, -1, -1, -1, 31, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 353, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 508, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, 195, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, 15, -1, 17 },
			{ -1, -1, -1, 204, -1, 207, 210, 354, -1, -1, 213, 216, 219, -1, -1, -1, -1, 222, -1, -1, 225, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 228, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, 167, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 236, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 167, 167, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 50, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 58, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 51, 568, -1, 568, 402, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 355, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 52, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 34, 35, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 238, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 53, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 36, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 240, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 54, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 169, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 37, -1, -1, 38, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 244, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 55, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 39, -1, -1, -1, 40, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 356, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 56, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 41, -1, -1, -1, 42, 16, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 246, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 59, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 43, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 364, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 67, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 44, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 248, -1, -1, -1, -1, -1, -1, 357, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 68, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, 47, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 69, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 48, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 70, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 71, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 72, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, 256, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 73, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 358, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 74, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 258, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 75, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 359, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 76, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 262, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 77, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 266, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 78, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, -1, 268, 268, 268, 268, 268, 268, 268, 268, -1, -1, 268, -1, 268, -1, -1, -1, -1, 268, -1, 250, -1, 268, 268, 268, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 270, -1, 361 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 79, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 80, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 272, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 274, 90, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 81, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 365, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 82, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 362, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 83, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, 280, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 84, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 282, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 85, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 284, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 86, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 367, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 87, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 268, 91, 268, 268, 268, 268, 268, 268, 268, 268, -1, -1, 268, 268, 268, -1, -1, -1, -1, 268, -1, -1, -1, 268, 268, 268, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 268, 268, 172, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 88, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, -1, 288, 288, 288, 288, 288, 288, 288, 288, -1, -1, 288, -1, 288, -1, -1, -1, -1, 288, -1, -1, -1, 288, 288, 288, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 89, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 294, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 92, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 274, 90, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 93, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 282, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 94, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, 300, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 95, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 302, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 96, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 282, 99, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 97, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 304, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 98, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 308, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 310, 100, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 101, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, 288, -1, 288, 288, 288, 288, 288, 288, 288, 288, -1, -1, 288, 288, 288, -1, -1, -1, -1, 288, -1, -1, -1, 288, 288, 288, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 288, 288, -1, -1, -1, 312, -1, -1 },
			{ -1, 102, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, -1, 290, 290, 290, 290, 290, 290, 290, 290, -1, -1, 290, 290, 290, -1, -1, -1, -1, 290, -1, -1, -1, 290, 290, 290, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 290, 290, -1, -1, -1, -1, -1, 312 },
			{ -1, 568, 568, 568, 103, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, 369, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 292, -1, -1, -1, -1, -1 },
			{ -1, 104, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 371, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 105, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 282, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 106, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 298, 109, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 107, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 314, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 108, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 111, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 304, 110, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 112, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 316, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 113, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 318, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 114, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 310, 100, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 115, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 91, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 172, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 116, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 314, 117, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 119, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 316, 118, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 120, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 310, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 121, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 322, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 122, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 132, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 123, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 2, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 173, 185, 139, 139, 139, 139, 139, 139, 139, 139, 139, 140 },
			{ -1, 124, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 328, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 125, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, -1, 144, 144, 144, 144, 144, 144, 144, 144, -1, -1, 144, -1, 144, -1, -1, -1, -1, 144, -1, -1, -1, 144, 144, 144, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 126, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 127, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 2, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 145, 174, 186, 145, 145, 145, 145, 145, 145, 146, 145, 145, 145 },
			{ -1, 128, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 2, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147, 175, 187, 147, 147, 147, 147, 147, 147, 147, 147, 147, 147 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 129, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ 148, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 149, 8, 149, 149, 149, 149, 149, 149, 149, 149, 176, 148, 149, 148, 149, 148, 148, 148, 148, 149, 148, 8, 148, 149, 149, 149, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 148, 8, 148, 148, 148, 148, 148 },
			{ -1, 568, 568, 568, 568, 568, 130, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ 151, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 177, 151, 177, 177, 177, 177, 177, 177, 177, 177, 151, 151, 162, 151, 177, 151, 151, 151, 151, 177, 151, 151, 151, 177, 177, 177, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151, 151 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 131, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 133, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ 6, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 153, 568, 568, 568, 568, 568, 568, 568, 568, 154, 154, 568, 155, 568, 154, 153, 154, 154, 568, 154, 153, 154, 568, 568, 568, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 154, 178, 154, 154, 179, 155, 153, 156, 154, 153, 153, 154 },
			{ -1, 134, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 135, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 136, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 2, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158, 158 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 137, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 2, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159, 159 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 138, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 199, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 250, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, 242, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 254, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 260, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 360, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 276, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 278, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 286, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, 290, -1, 290, 290, 290, 290, 290, 290, 290, 290, -1, -1, 290, -1, 290, -1, -1, -1, -1, 290, -1, -1, -1, 290, 290, 290, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, 298, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 202, 568, 568, -1, 568, 568, 381, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 264, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 296, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 383, 568, 568, 568, 505, 568, 568, 208, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 306, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 503, 568, 568, 211, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 320, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 214, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 384, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 274, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 220, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 223, 533, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 397, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 226, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 229, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 398, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 232, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 550, 568, 568, 568, 568, 399, 568, 400, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 401, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 403, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 506, 568, 568, 534, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 404, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 561, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 408, 568, 568, 568, 568, -1, 568, 409, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 410, 568, 568, 568, 568, 568, 568, 235, 568, 568, 578, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 511, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 535, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 411, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 512, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 412, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 237, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 239, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 509, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 551, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 416, 568, 568, 568, 568, 568, 568, 563, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 417, 418, 419, 568, 420, 562, 568, 568, 568, 568, 514, -1, 421, 568, 515, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 241, 568, 516, 423, 568, 568, 568, 568, 424, 568, 568, 568, -1, 568, 568, 568, 425, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 243, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 536, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 426, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 245, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 247, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 249, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 251, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 427, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 253, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 538, 568, 568, 568, 568, 568, 568, 255, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 257, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 259, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 261, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 431, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 263, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 265, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 267, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 269, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 271, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 567, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 569, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 434, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 435, 568, 568, 568, 517, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 436, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 518, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 438, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 273, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 440, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 522, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 520, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 443, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 543, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 447, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 277, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 279, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 281, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 283, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 285, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 451, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 452, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 524, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 453, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 287, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 456, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 526, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 458, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 289, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 460, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 291, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 293, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 297, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 527, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 463, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 299, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 301, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 303, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 465, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 545, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 467, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 468, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 547, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 305, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 470, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 471, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 472, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 307, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 309, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 311, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 313, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 315, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 317, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 480, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 481, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 319, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 321, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 323, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 485, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 486, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 325, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 327, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 329, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 579, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 487, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 331, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 488, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 528, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 333, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 335, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 489, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 337, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 339, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 490, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 341, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 492, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 495, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 496, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 343, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 345, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 347, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 497, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 498, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 349, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 499, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 500, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 351, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 532, 568, 568, 568, 385, -1, 568, 386, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 510, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 406, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 413, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 405, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 553, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 414, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 415, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 432, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 540, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 429, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 554, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 441, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 541, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 519, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 439, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 577, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 454, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 455, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 459, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 546, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 457, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 462, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 525, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 478, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 469, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 474, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 491, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 493, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 387, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 388, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 552, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 407, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 422, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 539, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 430, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 442, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 542, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 523, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 445, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 574, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 544, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 464, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 461, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 466, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 479, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 475, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 482, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 494, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 389, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 513, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 433, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 537, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 444, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 449, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 446, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 521, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 473, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 476, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 483, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 390, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 428, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 437, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 556, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 448, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 477, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 391, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 450, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 575, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 507, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 392, 568, 568, 568, 393, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 394, 568, 568, 568, 568, 395, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 396, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 557, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 558, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 529, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 565, 568, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 568, 564, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 },
			{ -1, 568, 568, 568, 568, 568, 568, 568, 568, 568, 548, 568, 568, 568, 568, -1, 568, 568, 568, 568, 568, 568, 568, 568, -1, -1, 568, 568, 568, -1, -1, -1, -1, 568, -1, -1, -1, 568, 568, 568, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 568, 568, -1, -1, -1, -1, -1, -1 }
		};
		
		
		private static int[] yy_state_dtrans = new int[]
		{
			  0,
			  192,
			  324,
			  330,
			  332,
			  334,
			  330,
			  336,
			  338,
			  330,
			  330,
			  330,
			  342,
			  348,
			  350
		};
		
		#endregion
		
		public Tokens GetNextToken()
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
					return Tokens.END;
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
							System.Diagnostics.Debug.Assert(last_accept_state >= 580);
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
		} // end of GetNextToken
	}
}

