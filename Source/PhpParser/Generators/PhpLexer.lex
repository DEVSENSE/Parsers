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

%%

%namespace PhpParser.Parser
%type Tokens
%class Lexer
%eofval Tokens.EOF
%errorval Tokens.ERROR
%attributes public partial
%function GetNextToken
%ignorecase
%charmap Map
%char

%x INITIAL
%x ST_IN_SCRIPTING
%x ST_DOUBLE_QUOTES
%x ST_SINGLE_QUOTES
%x ST_BACKQUOTE
%x ST_HEREDOC
%x ST_NEWDOC
%x ST_LOOKING_FOR_PROPERTY
%x ST_LOOKING_FOR_VARNAME
%x ST_DOC_COMMENT
%x ST_COMMENT
%x ST_ONE_LINE_COMMENT

re2c:yyfill:check = 0;
LNUM	[0-9]+
DNUM	([0-9]*"."[0-9]+)|([0-9]+"."[0-9]*)
EXPONENT_DNUM	(({LNUM}|{DNUM})[eE][+-]?{LNUM})
HNUM	"0x"[0-9a-fA-F]+
BNUM	"0b"[01]+
LABEL	[a-zA-Z_\x80-\xff][a-zA-Z0-9_\x80-\xff]*
WHITESPACE [ \n\r\t]+
TABS_AND_SPACES [ \t]*
TOKENS [;:,.\[\]()|^&+-/*=%!~$<>?@]
ANY_CHAR [^]
NEWLINE ("\r"|"\n"|"\r\n")

/* compute yyleng before each rule */
<!*> := yyleng = YYCURSOR - SCNG(yy_text);

%%

<ST_IN_SCRIPTING>"exit" { 
	return (Tokens.T_EXIT); 
}

<ST_IN_SCRIPTING>"die" {
	return (Tokens.T_EXIT);
}

<ST_IN_SCRIPTING>"function" {
	return (Tokens.T_FUNCTION);
}

<ST_IN_SCRIPTING>"const" {
	return (Tokens.T_CONST);
}

<ST_IN_SCRIPTING>"return" {
	return (Tokens.T_RETURN);
}

<ST_IN_SCRIPTING>"yield"{WHITESPACE}"from" {
	HANDLE_NEWLINES(yytext, yyleng);
	return (Tokens.T_YIELD_FROM);
}

<ST_IN_SCRIPTING>"yield" {
	return (Tokens.T_YIELD);
}

<ST_IN_SCRIPTING>"try" {
	return (Tokens.T_TRY);
}

<ST_IN_SCRIPTING>"catch" {
	return (Tokens.T_CATCH);
}

<ST_IN_SCRIPTING>"finally" {
	return (Tokens.T_FINALLY);
}

<ST_IN_SCRIPTING>"throw" {
	return (Tokens.T_THROW);
}

<ST_IN_SCRIPTING>"if" {
	return (Tokens.T_IF);
}

<ST_IN_SCRIPTING>"elseif" {
	return (Tokens.T_ELSEIF);
}

<ST_IN_SCRIPTING>"endif" {
	return (Tokens.T_ENDIF);
}

<ST_IN_SCRIPTING>"else" {
	return (Tokens.T_ELSE);
}

<ST_IN_SCRIPTING>"while" {
	return (Tokens.T_WHILE);
}

<ST_IN_SCRIPTING>"endwhile" {
	return (Tokens.T_ENDWHILE);
}

<ST_IN_SCRIPTING>"do" {
	return (Tokens.T_DO);
}

<ST_IN_SCRIPTING>"for" {
	return (Tokens.T_FOR);
}

<ST_IN_SCRIPTING>"endfor" {
	return (Tokens.T_ENDFOR);
}

<ST_IN_SCRIPTING>"foreach" {
	return (Tokens.T_FOREACH);
}

<ST_IN_SCRIPTING>"endforeach" {
	return (Tokens.T_ENDFOREACH);
}

<ST_IN_SCRIPTING>"declare" {
	return (Tokens.T_DECLARE);
}

<ST_IN_SCRIPTING>"enddeclare" {
	return (Tokens.T_ENDDECLARE);
}

<ST_IN_SCRIPTING>"instanceof" {
	return (Tokens.T_INSTANCEOF);
}

<ST_IN_SCRIPTING>"as" {
	return (Tokens.T_AS);
}

<ST_IN_SCRIPTING>"switch" {
	return (Tokens.T_SWITCH);
}

<ST_IN_SCRIPTING>"endswitch" {
	return (Tokens.T_ENDSWITCH);
}

<ST_IN_SCRIPTING>"case" {
	return (Tokens.T_CASE);
}

<ST_IN_SCRIPTING>"default" {
	return (Tokens.T_DEFAULT);
}

<ST_IN_SCRIPTING>"break" {
	return (Tokens.T_BREAK);
}

<ST_IN_SCRIPTING>"continue" {
	return (Tokens.T_CONTINUE);
}

<ST_IN_SCRIPTING>"goto" {
	return (Tokens.T_GOTO);
}

<ST_IN_SCRIPTING>"echo" {
	return (Tokens.T_ECHO);
}

<ST_IN_SCRIPTING>"print" {
	return (Tokens.T_PRINT);
}

<ST_IN_SCRIPTING>"class" {
	return (Tokens.T_CLASS);
}

<ST_IN_SCRIPTING>"interface" {
	return (Tokens.T_INTERFACE);
}

<ST_IN_SCRIPTING>"trait" {
	return (Tokens.T_TRAIT);
}

<ST_IN_SCRIPTING>"extends" {
	return (Tokens.T_EXTENDS);
}

<ST_IN_SCRIPTING>"implements" {
	return (Tokens.T_IMPLEMENTS);
}

<ST_IN_SCRIPTING>"->" {
	yy_push_state(ST_LOOKING_FOR_PROPERTY);
	return (Tokens.T_OBJECT_OPERATOR);
}

<ST_IN_SCRIPTING,ST_LOOKING_FOR_PROPERTY>{WHITESPACE}+ {
	HANDLE_NEWLINES(yytext, yyleng);
	return (Tokens.T_WHITESPACE);
}

<ST_LOOKING_FOR_PROPERTY>"->" {
	return (Tokens.T_OBJECT_OPERATOR);
}

<ST_LOOKING_FOR_PROPERTY>{LABEL} {
	yy_pop_state();
	zend_copy_value(zendlval, yytext, yyleng);
	return (Tokens.T_STRING);
}

<ST_LOOKING_FOR_PROPERTY>{ANY_CHAR} {
	yyless(0);
	yy_pop_state();
	goto restart;
}

<ST_IN_SCRIPTING>"::" {
	return (Tokens.T_PAAMAYIM_NEKUDOTAYIM);
}

<ST_IN_SCRIPTING>"\\" {
	return (Tokens.T_NS_SEPARATOR);
}

<ST_IN_SCRIPTING>"..." {
	return (Tokens.T_ELLIPSIS);
}

<ST_IN_SCRIPTING>"??" {
	return (Tokens.T_COALESCE);
}

<ST_IN_SCRIPTING>"new" {
	return (Tokens.T_NEW);
}

<ST_IN_SCRIPTING>"clone" {
	return (Tokens.T_CLONE);
}

<ST_IN_SCRIPTING>"var" {
	return (Tokens.T_VAR);
}

<ST_IN_SCRIPTING>"("{TABS_AND_SPACES}("int"|"integer"){TABS_AND_SPACES}")" {
	return (Tokens.T_INT_CAST);
}

<ST_IN_SCRIPTING>"("{TABS_AND_SPACES}("real"|"double"|"float"){TABS_AND_SPACES}")" {
	return (Tokens.T_DOUBLE_CAST);
}

<ST_IN_SCRIPTING>"("{TABS_AND_SPACES}("string"|"binary"){TABS_AND_SPACES}")" {
	return (Tokens.T_STRING_CAST);
}

<ST_IN_SCRIPTING>"("{TABS_AND_SPACES}"array"{TABS_AND_SPACES}")" {
	return (Tokens.T_ARRAY_CAST);
}

<ST_IN_SCRIPTING>"("{TABS_AND_SPACES}"object"{TABS_AND_SPACES}")" {
	return (Tokens.T_OBJECT_CAST);
}

<ST_IN_SCRIPTING>"("{TABS_AND_SPACES}("bool"|"boolean"){TABS_AND_SPACES}")" {
	return (Tokens.T_BOOL_CAST);
}

<ST_IN_SCRIPTING>"("{TABS_AND_SPACES}("unset"){TABS_AND_SPACES}")" {
	return (Tokens.T_UNSET_CAST);
}

<ST_IN_SCRIPTING>"eval" {
	return (Tokens.T_EVAL);
}

<ST_IN_SCRIPTING>"include" {
	return (Tokens.T_INCLUDE);
}

<ST_IN_SCRIPTING>"include_once" {
	return (Tokens.T_INCLUDE_ONCE);
}

<ST_IN_SCRIPTING>"require" {
	return (Tokens.T_REQUIRE);
}

<ST_IN_SCRIPTING>"require_once" {
	return (Tokens.T_REQUIRE_ONCE);
}

<ST_IN_SCRIPTING>"namespace" {
	return (Tokens.T_NAMESPACE);
}

<ST_IN_SCRIPTING>"use" {
	return (Tokens.T_USE);
}

<ST_IN_SCRIPTING>"insteadof" {
    return (Tokens.T_INSTEADOF);
}

<ST_IN_SCRIPTING>"global" {
	return (Tokens.T_GLOBAL);
}

<ST_IN_SCRIPTING>"isset" {
	return (Tokens.T_ISSET);
}

<ST_IN_SCRIPTING>"empty" {
	return (Tokens.T_EMPTY);
}

<ST_IN_SCRIPTING>"__halt_compiler" {
	return (Tokens.T_HALT_COMPILER);
}

<ST_IN_SCRIPTING>"static" {
	return (Tokens.T_STATIC);
}

<ST_IN_SCRIPTING>"abstract" {
	return (Tokens.T_ABSTRACT);
}

<ST_IN_SCRIPTING>"final" {
	return (Tokens.T_FINAL);
}

<ST_IN_SCRIPTING>"private" {
	return (Tokens.T_PRIVATE);
}

<ST_IN_SCRIPTING>"protected" {
	return (Tokens.T_PROTECTED);
}

<ST_IN_SCRIPTING>"public" {
	return (Tokens.T_PUBLIC);
}

<ST_IN_SCRIPTING>"unset" {
	return (Tokens.T_UNSET);
}

<ST_IN_SCRIPTING>"=>" {
	return (Tokens.T_DOUBLE_ARROW);
}

<ST_IN_SCRIPTING>"list" {
	return (Tokens.T_LIST);
}

<ST_IN_SCRIPTING>"array" {
	return (Tokens.T_ARRAY);
}

<ST_IN_SCRIPTING>"callable" {
	return (Tokens.T_CALLABLE);
}

<ST_IN_SCRIPTING>"++" {
	return (Tokens.T_INC);
}

<ST_IN_SCRIPTING>"--" {
	return (Tokens.T_DEC);
}

<ST_IN_SCRIPTING>"===" {
	return (Tokens.T_IS_IDENTICAL);
}

<ST_IN_SCRIPTING>"!==" {
	return (Tokens.T_IS_NOT_IDENTICAL);
}

<ST_IN_SCRIPTING>"==" {
	return (Tokens.T_IS_EQUAL);
}

<ST_IN_SCRIPTING>"!="|"<>" {
	return (Tokens.T_IS_NOT_EQUAL);
}

<ST_IN_SCRIPTING>"<=>" {
	return (Tokens.T_SPACESHIP);
}

<ST_IN_SCRIPTING>"<=" {
	return (Tokens.T_IS_SMALLER_OR_EQUAL);
}

<ST_IN_SCRIPTING>">=" {
	return (Tokens.T_IS_GREATER_OR_EQUAL);
}

<ST_IN_SCRIPTING>"+=" {
	return (Tokens.T_PLUS_EQUAL);
}

<ST_IN_SCRIPTING>"-=" {
	return (Tokens.T_MINUS_EQUAL);
}

<ST_IN_SCRIPTING>"*=" {
	return (Tokens.T_MUL_EQUAL);
}

<ST_IN_SCRIPTING>"*\*" {
	return (Tokens.T_POW);
}

<ST_IN_SCRIPTING>"*\*=" {
	return (Tokens.T_POW_EQUAL);
}

<ST_IN_SCRIPTING>"/=" {
	return (Tokens.T_DIV_EQUAL);
}

<ST_IN_SCRIPTING>".=" {
	return (Tokens.T_CONCAT_EQUAL);
}

<ST_IN_SCRIPTING>"%=" {
	return (Tokens.T_MOD_EQUAL);
}

<ST_IN_SCRIPTING>"<<=" {
	return (Tokens.T_SL_EQUAL);
}

<ST_IN_SCRIPTING>">>=" {
	return (Tokens.T_SR_EQUAL);
}

<ST_IN_SCRIPTING>"&=" {
	return (Tokens.T_AND_EQUAL);
}

<ST_IN_SCRIPTING>"|=" {
	return (Tokens.T_OR_EQUAL);
}

<ST_IN_SCRIPTING>"^=" {
	return (Tokens.T_XOR_EQUAL);
}

<ST_IN_SCRIPTING>"||" {
	return (Tokens.T_BOOLEAN_OR);
}

<ST_IN_SCRIPTING>"&&" {
	return (Tokens.T_BOOLEAN_AND);
}

<ST_IN_SCRIPTING>"OR" {
	return (Tokens.T_LOGICAL_OR);
}

<ST_IN_SCRIPTING>"AND" {
	return (Tokens.T_LOGICAL_AND);
}

<ST_IN_SCRIPTING>"XOR" {
	return (Tokens.T_LOGICAL_XOR);
}

<ST_IN_SCRIPTING>"<<" {
	return (Tokens.T_SL);
}

<ST_IN_SCRIPTING>">>" {
	return (Tokens.T_SR);
}

<ST_IN_SCRIPTING>{TOKENS} {
	return (Tokens.yytext[0]);
}


<ST_IN_SCRIPTING>"{" {
	yy_push_state(ST_IN_SCRIPTING);
	return (Tokens.'{');
}


<ST_DOUBLE_QUOTES,ST_BACKQUOTE,ST_HEREDOC>"${" {
	yy_push_state(ST_LOOKING_FOR_VARNAME);
	return (Tokens.T_DOLLAR_OPEN_CURLY_BRACES);
}


<ST_IN_SCRIPTING>"}" {
	RESET_DOC_COMMENT();
	if (!zend_stack_is_empty(&SCNG(state_stack))) {
		yy_pop_state();
	}
	return (Tokens.'}');
}


<ST_LOOKING_FOR_VARNAME>{LABEL}[[}] {
	yyless(yyleng - 1);
	zend_copy_value(zendlval, yytext, yyleng);
	yy_pop_state();
	yy_push_state(ST_IN_SCRIPTING);
	return (Tokens.T_STRING_VARNAME);
}


<ST_LOOKING_FOR_VARNAME>{ANY_CHAR} {
	yyless(0);
	yy_pop_state();
	yy_push_state(ST_IN_SCRIPTING);
	goto restart;
}

<ST_IN_SCRIPTING>{BNUM} {
	char *bin = yytext + 2; /* Skip "0b" */
	int len = yyleng - 2;
	char *end;

	/* Skip any leading 0s */
	while (*bin == '0') {
		++bin;
		--len;
	}

	if (len < SIZEOF_ZEND_LONG * 8) {
		if (len == 0) {
			ZVAL_LONG(zendlval, 0);
		} else {
			errno = 0;
			ZVAL_LONG(zendlval, ZEND_STRTOL(bin, &end, 2));
			ZEND_ASSERT(!errno && end == yytext + yyleng);
		}
		return (Tokens.T_LNUMBER);
	} else {
		ZVAL_DOUBLE(zendlval, zend_bin_strtod(bin, (const char **)&end));
		/* errno isn't checked since we allow HUGE_VAL/INF overflow */
		ZEND_ASSERT(end == yytext + yyleng);
		return (Tokens.T_DNUMBER);
	}
}

<ST_IN_SCRIPTING>{LNUM} {
	char *end;
	if (yyleng < MAX_LENGTH_OF_LONG - 1) { /* Won't overflow */
		errno = 0;
		ZVAL_LONG(zendlval, ZEND_STRTOL(yytext, &end, 0));
		/* This isn't an assert, we need to ensure 019 isn't valid octal
		 * Because the lexing itself doesn't do that for us
		 */
		if (end != yytext + yyleng) {
			zend_throw_exception(zend_ce_parse_error, "Invalid numeric literal", 0);
			ZVAL_UNDEF(zendlval);
			return (Tokens.T_LNUMBER);
		}
	} else {
		errno = 0;
		ZVAL_LONG(zendlval, ZEND_STRTOL(yytext, &end, 0));
		if (errno == ERANGE) { /* Overflow */
			errno = 0;
			if (yytext[0] == '0') { /* octal overflow */
				errno = 0;
				ZVAL_DOUBLE(zendlval, zend_oct_strtod(yytext, (const char **)&end));
			} else {
				ZVAL_DOUBLE(zendlval, zend_strtod(yytext, (const char **)&end));
			}
			/* Also not an assert for the same reason */
			if (end != yytext + yyleng) {
				zend_throw_exception(zend_ce_parse_error,
					"Invalid numeric literal", 0);
				ZVAL_UNDEF(zendlval);
				return (Tokens.T_DNUMBER);
			}
			ZEND_ASSERT(!errno);
			return (Tokens.T_DNUMBER);
		}
		/* Also not an assert for the same reason */
		if (end != yytext + yyleng) {
			zend_throw_exception(zend_ce_parse_error, "Invalid numeric literal", 0);
			ZVAL_UNDEF(zendlval);
			return (Tokens.T_DNUMBER);
		}
	}
	ZEND_ASSERT(!errno);
	return (Tokens.T_LNUMBER);
}

<ST_IN_SCRIPTING>{HNUM} {
	char *hex = yytext + 2; /* Skip "0x" */
	int len = yyleng - 2;
	char *end;

	/* Skip any leading 0s */
	while (*hex == '0') {
		hex++;
		len--;
	}

	if (len < SIZEOF_ZEND_LONG * 2 || (len == SIZEOF_ZEND_LONG * 2 && *hex <= '7')) {
		if (len == 0) {
			ZVAL_LONG(zendlval, 0);
		} else {
			errno = 0;
			ZVAL_LONG(zendlval, ZEND_STRTOL(hex, &end, 16));
			ZEND_ASSERT(!errno && end == hex + len);
		}
		return (Tokens.T_LNUMBER);
	} else {
		ZVAL_DOUBLE(zendlval, zend_hex_strtod(hex, (const char **)&end));
		/* errno isn't checked since we allow HUGE_VAL/INF overflow */
		ZEND_ASSERT(end == hex + len);
		return (Tokens.T_DNUMBER);
	}
}

<ST_VAR_OFFSET>[0]|([1-9][0-9]*) { /* Offset could be treated as a long */
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

<ST_VAR_OFFSET>{LNUM}|{HNUM}|{BNUM} { /* Offset must be treated as a string */
	ZVAL_STRINGL(zendlval, yytext, yyleng);
	return (Tokens.T_NUM_STRING);
}

<ST_IN_SCRIPTING>{DNUM}|{EXPONENT_DNUM} {
	const char *end;

	ZVAL_DOUBLE(zendlval, zend_strtod(yytext, &end));
	/* errno isn't checked since we allow HUGE_VAL/INF overflow */
	ZEND_ASSERT(end == yytext + yyleng);
	return (Tokens.T_DNUMBER);
}

<ST_IN_SCRIPTING>"__CLASS__" {
	return (Tokens.T_CLASS_C);
}

<ST_IN_SCRIPTING>"__TRAIT__" {
	return (Tokens.T_TRAIT_C);
}

<ST_IN_SCRIPTING>"__FUNCTION__" {
	return (Tokens.T_FUNC_C);
}

<ST_IN_SCRIPTING>"__METHOD__" {
	return (Tokens.T_METHOD_C);
}

<ST_IN_SCRIPTING>"__LINE__" {
	return (Tokens.T_LINE);
}

<ST_IN_SCRIPTING>"__FILE__" {
	return (Tokens.T_FILE);
}

<ST_IN_SCRIPTING>"__DIR__" {
	return (Tokens.T_DIR);
}

<ST_IN_SCRIPTING>"__NAMESPACE__" {
	return (Tokens.T_NS_C);
}


<INITIAL>"<?=" {
	BEGIN(ST_IN_SCRIPTING);
	return (Tokens.T_OPEN_TAG_WITH_ECHO);
}


<INITIAL>"<?php"([ \t]|{NEWLINE}) {
	HANDLE_NEWLINE(yytext[yyleng-1]);
	BEGIN(ST_IN_SCRIPTING);
	return (Tokens.T_OPEN_TAG);
}


<INITIAL>"<?" {
	if (CG(short_tags)) {
		BEGIN(ST_IN_SCRIPTING);
		return (Tokens.T_OPEN_TAG);
	} else {
		goto inline_char_handler;
	}
}

<INITIAL>{ANY_CHAR} {
	if (YYCURSOR > YYLIMIT) {
		return (Tokens.END);
	}

inline_char_handler:

	while (1) {
		YYCTYPE *ptr = memchr(YYCURSOR, '<', YYLIMIT - YYCURSOR);

		YYCURSOR = ptr ? ptr + 1 : YYLIMIT;

		if (YYCURSOR >= YYLIMIT) {
			break;
		}

		if (*YYCURSOR == '?') {
			if (CG(short_tags) || !strncasecmp((char*)YYCURSOR + 1, "php", 3) || (*(YYCURSOR + 1) == '=')) { /* Assume [ \t\n\r] follows "php" */

				YYCURSOR--;
				break;
			}
		}
	}

	yyleng = YYCURSOR - SCNG(yy_text);

	if (SCNG(output_filter)) {
		size_t readsize;
		char *s = NULL;
		size_t sz = 0;
		// TODO: avoid reallocation ???
		readsize = SCNG(output_filter)((unsigned char **)&s, &sz, (unsigned char *)yytext, (size_t)yyleng);
		ZVAL_STRINGL(zendlval, s, sz);
		efree(s);
		if (readsize < yyleng) {
			yyless(readsize);
		}
	} else {
	  ZVAL_STRINGL(zendlval, yytext, yyleng);
	}
	HANDLE_NEWLINES(yytext, yyleng);
	return (Tokens.T_INLINE_HTML);
}


/* Make sure a label character follows "->", otherwise there is no property
 * and "->" will be taken literally
 */
<ST_DOUBLE_QUOTES,ST_HEREDOC,ST_BACKQUOTE>"$"{LABEL}"->"[a-zA-Z_\x80-\xff] {
	yyless(yyleng - 3);
	yy_push_state(ST_LOOKING_FOR_PROPERTY);
	zend_copy_value(zendlval, (yytext+1), (yyleng-1));
	return (Tokens.T_VARIABLE);
}

/* A [ always designates a variable offset, regardless of what follows
 */
<ST_DOUBLE_QUOTES,ST_HEREDOC,ST_BACKQUOTE>"$"{LABEL}"[" {
	yyless(yyleng - 1);
	yy_push_state(ST_VAR_OFFSET);
	zend_copy_value(zendlval, (yytext+1), (yyleng-1));
	return (Tokens.T_VARIABLE);
}

<ST_IN_SCRIPTING,ST_DOUBLE_QUOTES,ST_HEREDOC,ST_BACKQUOTE,ST_VAR_OFFSET>"$"{LABEL} {
	zend_copy_value(zendlval, (yytext+1), (yyleng-1));
	return (Tokens.T_VARIABLE);
}

<ST_VAR_OFFSET>"]" {
	yy_pop_state();
	return (Tokens.']');
}

<ST_VAR_OFFSET>{TOKENS}|[{}"`] {
	/* Only '[' can be valid, but returning other tokens will allow a more explicit parse error */
	return (Tokens.yytext[0]);
}

<ST_VAR_OFFSET>[ \n\r\t\\'#] {
	/* Invalid rule to return a more explicit parse error with proper line number */
	yyless(0);
	yy_pop_state();
	ZVAL_NULL(zendlval);
	return (Tokens.T_ENCAPSED_AND_WHITESPACE);
}

<ST_IN_SCRIPTING,ST_VAR_OFFSET>{LABEL} {
	zend_copy_value(zendlval, yytext, yyleng);
	return (Tokens.T_STRING);
}


<ST_IN_SCRIPTING>"#"|"//" {
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

<ST_IN_SCRIPTING>"/*"|"/**"{WHITESPACE} {
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

<ST_IN_SCRIPTING>"?>"{NEWLINE}? {
	BEGIN(INITIAL);
	return (Tokens.T_CLOSE_TAG);  /* implicit ';' at php-end tag */
}


<ST_IN_SCRIPTING>b?['] {
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

			/* Unclosed single quotes; treat similar to double quotes, but without a separate token
			 * for ' (unrecognized by parser), instead of old flex fallback to "Unexpected character..."
			 * rule, which continued in ST_IN_SCRIPTING state after the quote */
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


<ST_IN_SCRIPTING>b?["] {
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
	return (Tokens.'"');
}


<ST_IN_SCRIPTING>b?"<<<"{TABS_AND_SPACES}({LABEL}|([']{LABEL}['])|(["]{LABEL}["])){NEWLINE} {
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


<ST_IN_SCRIPTING>[`] {
	BEGIN(ST_BACKQUOTE);
	return (Tokens.'`');
}


<ST_END_HEREDOC>{ANY_CHAR} {
	zend_heredoc_label *heredoc_label = zend_ptr_stack_pop(&SCNG(heredoc_label_stack));

	YYCURSOR += heredoc_label->length - 1;
	yyleng = heredoc_label->length;

	heredoc_label_dtor(heredoc_label);
	efree(heredoc_label);

	BEGIN(ST_IN_SCRIPTING);
	return (Tokens.T_END_HEREDOC);
}


<ST_DOUBLE_QUOTES,ST_BACKQUOTE,ST_HEREDOC>"{$" {
	Z_LVAL_P(zendlval) = (zend_long) '{';
	yy_push_state(ST_IN_SCRIPTING);
	yyless(1);
	return (Tokens.T_CURLY_OPEN);
}


<ST_DOUBLE_QUOTES>["] {
	BEGIN(ST_IN_SCRIPTING);
	return (Tokens.'"');
}

<ST_BACKQUOTE>[`] {
	BEGIN(ST_IN_SCRIPTING);
	return (Tokens.'`');
}


<ST_DOUBLE_QUOTES>{ANY_CHAR} {
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


<ST_BACKQUOTE>{ANY_CHAR} {
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


<ST_HEREDOC>{ANY_CHAR} {
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


<ST_NOWDOC>{ANY_CHAR} {
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


<ST_IN_SCRIPTING,ST_VAR_OFFSET>{ANY_CHAR} {
	if (YYCURSOR > YYLIMIT) {
		return (Tokens.END);
	}

	zend_error(E_COMPILE_WARNING,"Unexpected character in input:  '%c' (ASCII=%d) state=%d", yytext[0], yytext[0], YYSTATE);
	goto restart;
}

*/
}
