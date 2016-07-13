using System;
using System.Text;
using System.Collections.Generic;

#line 1 "C:\Users\Michal\Projects\Parsers\Source\PhpParser\Generators\PhpParser.y"
/*

Copyright (c) 2016 Michal Brabec

Parser was generated using The Gardens Point Parser Generator (GPPG) using PHP language grammar based on Flex and Bison files
distributed with PHP7 interpreter.

*/

using System.Diagnostics;
using System.Runtime.InteropServices;
using PHP.Core;
using PHP.Core.AST;
using PHP.Core.Text;

using FcnParam = System.Tuple<System.Collections.Generic.List<PHP.Core.AST.TypeRef>, System.Collections.Generic.List<PHP.Core.AST.ActualParam>, System.Collections.Generic.List<PHP.Core.AST.Expression>>;


namespace PhpParser.Parser
{
public enum Tokens {T_INCLUDE=262,T_INCLUDE_ONCE=261,T_EVAL=260,T_REQUIRE=259,T_REQUIRE_ONCE=258,T_LOGICAL_OR=263,T_LOGICAL_XOR=264,T_LOGICAL_AND=265,T_PRINT=266,T_YIELD=267,T_DOUBLE_ARROW=268,T_YIELD_FROM=269,T_PLUS_EQUAL=281,T_MINUS_EQUAL=280,T_MUL_EQUAL=279,T_DIV_EQUAL=278,T_CONCAT_EQUAL=277,T_MOD_EQUAL=276,T_AND_EQUAL=275,T_OR_EQUAL=274,T_XOR_EQUAL=273,T_SL_EQUAL=272,T_SR_EQUAL=271,T_POW_EQUAL=270,T_COALESCE=282,T_BOOLEAN_OR=283,T_BOOLEAN_AND=284,T_IS_EQUAL=289,T_IS_NOT_EQUAL=288,T_IS_IDENTICAL=287,T_IS_NOT_IDENTICAL=286,T_SPACESHIP=285,T_IS_SMALLER_OR_EQUAL=291,T_IS_GREATER_OR_EQUAL=290,T_SL=293,T_SR=292,T_INSTANCEOF=294,T_INC=303,T_DEC=302,T_INT_CAST=301,T_DOUBLE_CAST=300,T_STRING_CAST=299,T_ARRAY_CAST=298,T_OBJECT_CAST=297,T_BOOL_CAST=296,T_UNSET_CAST=295,T_POW=304,T_NEW=306,T_CLONE=305,T_NOELSE=176,T_ELSEIF=308,T_ELSE=309,T_ENDIF=310,T_STATIC=316,T_ABSTRACT=315,T_FINAL=314,T_PRIVATE=313,T_PROTECTED=312,T_PUBLIC=311,T_LNUMBER=317,T_DNUMBER=318,T_STRING=319,T_VARIABLE=320,T_INLINE_HTML=321,T_ENCAPSED_AND_WHITESPACE=322,T_CONSTANT_ENCAPSED_STRING=323,T_STRING_VARNAME=324,T_NUM_STRING=325,END=0,T_EXIT=326,T_IF=327,T_ECHO=328,T_DO=329,T_WHILE=330,T_ENDWHILE=331,T_FOR=332,T_ENDFOR=333,T_FOREACH=334,T_ENDFOREACH=335,T_DECLARE=336,T_ENDDECLARE=337,T_AS=338,T_SWITCH=339,T_ENDSWITCH=340,T_CASE=341,T_DEFAULT=342,T_BREAK=343,T_CONTINUE=344,T_GOTO=345,T_FUNCTION=346,T_CONST=347,T_RETURN=348,T_TRY=349,T_CATCH=350,T_FINALLY=351,T_THROW=352,T_USE=353,T_INSTEADOF=354,T_GLOBAL=355,T_VAR=356,T_UNSET=357,T_ISSET=358,T_EMPTY=359,T_HALT_COMPILER=360,T_CLASS=361,T_TRAIT=362,T_INTERFACE=363,T_EXTENDS=364,T_IMPLEMENTS=365,T_OBJECT_OPERATOR=366,T_LIST=367,T_ARRAY=368,T_CALLABLE=369,T_LINE=370,T_FILE=371,T_DIR=372,T_CLASS_C=373,T_TRAIT_C=374,T_METHOD_C=375,T_FUNC_C=376,T_COMMENT=377,T_DOC_COMMENT=378,T_OPEN_TAG=379,T_OPEN_TAG_WITH_ECHO=380,T_CLOSE_TAG=381,T_WHITESPACE=382,T_START_HEREDOC=383,T_END_HEREDOC=384,T_DOLLAR_OPEN_CURLY_BRACES=385,T_CURLY_OPEN=386,T_DOUBLE_COLON=387,T_NAMESPACE=388,T_NS_C=389,T_NS_SEPARATOR=390,T_ELLIPSIS=391,T_ERROR=262,EOF=263};

[StructLayout(LayoutKind.Explicit)]
public partial struct SemanticValueType
#line 29 "C:\Users\Michal\Projects\Parsers\Source\PhpParser\Generators\PhpParser.y"
			{
	// Integer and Offset are both used when generating code for string 
	// with 'inline' variables. Other fields are not combined.
	
	[FieldOffset(0)]		
	public int Integer;
	[FieldOffset(4)]
	public int Offset;

	[FieldOffset(0)]
	public double Double;
	[FieldOffset(0)]
	public long Long;

	[FieldOffset(8)]
	public object Object; 
}

public partial class Parser: ShiftReduceParser<SemanticValueType,Span>
{

  protected override string[] NonTerminals { get { return nonTerminals; } }
  private static string[] nonTerminals;

  protected override State[] States { get { return states; } }
  private static readonly State[] states;

  protected override Rule[] Rules { get { return rules; } }
  private static readonly Rule[] rules;


  #region Construction

  static Parser()
  {

    #region states
    states = new State[]
    {
      new State(0, -2, new int[] {-125,1}),
      new State(1, new int[] {263,2}),
      new State(2, -1),
    };
    #endregion

    #region rules
    rules = new Rule[]
    {
    default(Rule),
    new Rule(-126, new int[]{-125,263}),
    new Rule(-125, new int[]{}),
    };
    #endregion

    nonTerminals = new string[] {"", "top_statement", "namespace_name", 
      "name", "statement", "function_declaration_statement", "class_declaration_statement", 
      "trait_declaration_statement", "interface_declaration_statement", "interface_extends_list", 
      "group_use_declaration", "inline_use_declarations", "inline_use_declaration", 
      "mixed_group_use_declaration", "use_declaration", "unprefixed_use_declaration", 
      "unprefixed_use_declarations", "const_decl", "inner_statement", "expr", 
      "optional_expr", "while_statement", "for_statement", "foreach_variable", 
      "foreach_statement", "declare_statement", "finally_statement", "unset_variable", 
      "variable", "extends_from", "parameter", "optional_type", "argument", "expr_without_variable", 
      "global_var", "static_var", "class_statement", "trait_adaptation", "trait_precedence", 
      "trait_alias", "absolute_trait_method_reference", "trait_method_reference", 
      "property", "echo_expr", "new_expr", "anonymous_class", "class_name", "class_name_reference", 
      "simple_variable", "internal_functions_in_yacc", "exit_expr", "scalar", 
      "backticks_expr", "lexical_var", "function_call", "member_name", "property_name", 
      "variable_class_name", "dereferencable_scalar", "constant", "dereferencable", 
      "callable_expr", "callable_variable", "static_member", "new_variable", 
      "encaps_var", "encaps_var_offset", "isset_variables", "top_statement_list", 
      "use_declarations", "const_list", "inner_statement_list", "if_stmt", "alt_if_stmt", 
      "for_exprs", "switch_case_list", "global_var_list", "static_var_list", 
      "echo_expr_list", "unset_variables", "catch_name_list", "catch_list", "parameter_list", 
      "class_statement_list", "implements_list", "case_list", "if_stmt_without_else", 
      "non_empty_parameter_list", "argument_list", "non_empty_argument_list", 
      "property_list", "class_const_list", "class_const_decl", "name_list", "trait_adaptations", 
      "method_body", "non_empty_for_exprs", "ctor_arguments", "alt_if_stmt_without_else", 
      "trait_adaptation_list", "lexical_vars", "lexical_var_list", "encaps_list", 
      "array_pair", "non_empty_array_pair_list", "array_pair_list", "possible_array_pair", 
      "isset_variable", "type", "return_type", "type_expr", "identifier", "returns_ref", 
      "function", "is_reference", "is_variadic", "variable_modifiers", "method_modifiers", 
      "non_empty_member_modifiers", "member_modifier", "class_modifiers", "class_modifier", 
      "use_type", "backup_fn_flags", "backup_doc_comment", "start", "$accept", 
      };
  }

  #endregion

  protected override void DoAction(int action)
  {
    switch (action)
    {
    }
  }

  protected override string TerminalToString(int terminal)
  {
    if (((Tokens)terminal).ToString() != terminal.ToString())
      return ((Tokens)terminal).ToString();
    else
      return CharToString((char)terminal);
  }

#line 292 "C:\Users\Michal\Projects\Parsers\Source\PhpParser\Generators\PhpParser.y"


#if false

start:
    top_statement_list	{ CG(ast) = $1; }
;
reserved_non_modifiers:
	  T_INCLUDE | T_INCLUDE_ONCE | T_EVAL | T_REQUIRE | T_REQUIRE_ONCE | T_LOGICAL_OR | T_LOGICAL_XOR | T_LOGICAL_AND
	| T_INSTANCEOF | T_NEW | T_CLONE | T_EXIT | T_IF | T_ELSEIF | T_ELSE | T_ENDIF | T_ECHO | T_DO | T_WHILE | T_ENDWHILE
	| T_FOR | T_ENDFOR | T_FOREACH | T_ENDFOREACH | T_DECLARE | T_ENDDECLARE | T_AS | T_TRY | T_CATCH | T_FINALLY
	| T_THROW | T_USE | T_INSTEADOF | T_GLOBAL | T_VAR | T_UNSET | T_ISSET | T_EMPTY | T_CONTINUE | T_GOTO
	| T_FUNCTION | T_CONST | T_RETURN | T_PRINT | T_YIELD | T_LIST | T_SWITCH | T_ENDSWITCH | T_CASE | T_DEFAULT | T_BREAK
	| T_ARRAY | T_CALLABLE | T_EXTENDS | T_IMPLEMENTS | T_NAMESPACE | T_TRAIT | T_INTERFACE | T_CLASS
	| T_CLASS_C | T_TRAIT_C | T_FUNC_C | T_METHOD_C | T_LINE | T_FILE | T_DIR | T_NS_C
;

semi_reserved:
	  reserved_non_modifiers
	| T_STATIC | T_ABSTRACT | T_FINAL | T_PRIVATE | T_PROTECTED | T_PUBLIC
;

identifier:
		T_STRING { $$ = $1; }
	| 	semi_reserved  {
			zval zv;
			zend_lex_tstring(&zv);
			$$ = zend_ast_create_zval(&zv);
		}
;

top_statement_list:
		top_statement_list top_statement { $$ = zend_ast_list_add($1, $2); }
	|	/* empty */ { $$ = zend_ast_create_list(0, ZEND_AST_STMT_LIST); }
;

namespace_name:
		T_STRING								{ $$ = $1; }
	|	namespace_name T_NS_SEPARATOR T_STRING	{ $$ = zend_ast_append_str($1, $3); }
;

name:
		namespace_name								{ $$ = $1; $$->attr = ZEND_NAME_NOT_FQ; }
	|	T_NAMESPACE T_NS_SEPARATOR namespace_name	{ $$ = $3; $$->attr = ZEND_NAME_RELATIVE; }
	|	T_NS_SEPARATOR namespace_name				{ $$ = $2; $$->attr = ZEND_NAME_FQ; }
;

top_statement:
		statement							{ $$ = $1; }
	|	function_declaration_statement		{ $$ = $1; }
	|	class_declaration_statement			{ $$ = $1; }
	|	trait_declaration_statement			{ $$ = $1; }
	|	interface_declaration_statement		{ $$ = $1; }
	|	T_HALT_COMPILER '(' ')' ';'
			{ $$ = zend_ast_create(ZEND_AST_HALT_COMPILER,
			      zend_ast_create_zval_from_long(zend_get_scanned_file_offset()));
			  zend_stop_lexing(); }
	|	T_NAMESPACE namespace_name ';'
			{ $$ = zend_ast_create(ZEND_AST_NAMESPACE, $2, NULL);
			  RESET_DOC_COMMENT(); }
	|	T_NAMESPACE namespace_name { RESET_DOC_COMMENT(); }
		'{' top_statement_list '}'
			{ $$ = zend_ast_create(ZEND_AST_NAMESPACE, $2, $5); }
	|	T_NAMESPACE { RESET_DOC_COMMENT(); }
		'{' top_statement_list '}'
			{ $$ = zend_ast_create(ZEND_AST_NAMESPACE, NULL, $4); }
	|	T_USE mixed_group_use_declaration ';'		{ $$ = $2; }
	|	T_USE use_type group_use_declaration ';'	{ $$ = $3; $$->attr = $2; }
	|	T_USE use_declarations ';'					{ $$ = $2; $$->attr = T_CLASS; }
	|	T_USE use_type use_declarations ';'			{ $$ = $3; $$->attr = $2; }
	|	T_CONST const_list ';'						{ $$ = $2; }
;

use_type:
	 	T_FUNCTION 		{ $$ = T_FUNCTION; }
	| 	T_CONST 		{ $$ = T_CONST; }
;

group_use_declaration:
		namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations '}'
			{ $$ = zend_ast_create(ZEND_AST_GROUP_USE, $1, $4); }
	|	T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations '}'
			{ $$ = zend_ast_create(ZEND_AST_GROUP_USE, $2, $5); }
;

mixed_group_use_declaration:
		namespace_name T_NS_SEPARATOR '{' inline_use_declarations '}'
			{ $$ = zend_ast_create(ZEND_AST_GROUP_USE, $1, $4);}
	|	T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' inline_use_declarations '}'
			{ $$ = zend_ast_create(ZEND_AST_GROUP_USE, $2, $5); }
;

inline_use_declarations:
		inline_use_declarations ',' inline_use_declaration
			{ $$ = zend_ast_list_add($1, $3); }
	|	inline_use_declaration
			{ $$ = zend_ast_create_list(1, ZEND_AST_USE, $1); }
;

unprefixed_use_declarations:
		unprefixed_use_declarations ',' unprefixed_use_declaration
			{ $$ = zend_ast_list_add($1, $3); }
	|	unprefixed_use_declaration
			{ $$ = zend_ast_create_list(1, ZEND_AST_USE, $1); }
;

use_declarations:
		use_declarations ',' use_declaration
			{ $$ = zend_ast_list_add($1, $3); }
	|	use_declaration
			{ $$ = zend_ast_create_list(1, ZEND_AST_USE, $1); }
;

inline_use_declaration:
		unprefixed_use_declaration { $$ = $1; $$->attr = T_CLASS; }
	|	use_type unprefixed_use_declaration { $$ = $2; $$->attr = $1; }
;

unprefixed_use_declaration:
		namespace_name
			{ $$ = zend_ast_create(ZEND_AST_USE_ELEM, $1, NULL); }
	|	namespace_name T_AS T_STRING
			{ $$ = zend_ast_create(ZEND_AST_USE_ELEM, $1, $3); }
;

use_declaration:
		unprefixed_use_declaration                { $$ = $1; }
	|	T_NS_SEPARATOR unprefixed_use_declaration { $$ = $2; }
;

const_list:
		const_list ',' const_decl { $$ = zend_ast_list_add($1, $3); }
	|	const_decl { $$ = zend_ast_create_list(1, ZEND_AST_CONST_DECL, $1); }
;

inner_statement_list:
		inner_statement_list inner_statement
			{ $$ = zend_ast_list_add($1, $2); }
	|	/* empty */
			{ $$ = zend_ast_create_list(0, ZEND_AST_STMT_LIST); }
;


inner_statement:
		statement { $$ = $1; }
	|	function_declaration_statement 		{ $$ = $1; }
	|	class_declaration_statement 		{ $$ = $1; }
	|	trait_declaration_statement			{ $$ = $1; }
	|	interface_declaration_statement		{ $$ = $1; }
	|	T_HALT_COMPILER '(' ')' ';'
			{ $$ = NULL; zend_error_noreturn(E_COMPILE_ERROR,
			      "__HALT_COMPILER() can only be used from the outermost scope"); }
;


statement:
		'{' inner_statement_list '}' { $$ = $2; }
	|	if_stmt { $$ = $1; }
	|	alt_if_stmt { $$ = $1; }
	|	T_WHILE '(' expr ')' while_statement
			{ $$ = zend_ast_create(ZEND_AST_WHILE, $3, $5); }
	|	T_DO statement T_WHILE '(' expr ')' ';'
			{ $$ = zend_ast_create(ZEND_AST_DO_WHILE, $2, $5); }
	|	T_FOR '(' for_exprs ';' for_exprs ';' for_exprs ')' for_statement
			{ $$ = zend_ast_create(ZEND_AST_FOR, $3, $5, $7, $9); }
	|	T_SWITCH '(' expr ')' switch_case_list
			{ $$ = zend_ast_create(ZEND_AST_SWITCH, $3, $5); }
	|	T_BREAK optional_expr ';'		{ $$ = zend_ast_create(ZEND_AST_BREAK, $2); }
	|	T_CONTINUE optional_expr ';'	{ $$ = zend_ast_create(ZEND_AST_CONTINUE, $2); }
	|	T_RETURN optional_expr ';'		{ $$ = zend_ast_create(ZEND_AST_RETURN, $2); }
	|	T_GLOBAL global_var_list ';'	{ $$ = $2; }
	|	T_STATIC static_var_list ';'	{ $$ = $2; }
	|	T_ECHO echo_expr_list ';'		{ $$ = $2; }
	|	T_INLINE_HTML { $$ = zend_ast_create(ZEND_AST_ECHO, $1); }
	|	expr ';' { $$ = $1; }
	|	T_UNSET '(' unset_variables ')' ';' { $$ = $3; }
	|	T_FOREACH '(' expr T_AS foreach_variable ')' foreach_statement
			{ $$ = zend_ast_create(ZEND_AST_FOREACH, $3, $5, NULL, $7); }
	|	T_FOREACH '(' expr T_AS foreach_variable T_DOUBLE_ARROW foreach_variable ')'
		foreach_statement
			{ $$ = zend_ast_create(ZEND_AST_FOREACH, $3, $7, $5, $9); }
	|	T_DECLARE '(' const_list ')'
			{ zend_handle_encoding_declaration($3); }
		declare_statement
			{ $$ = zend_ast_create(ZEND_AST_DECLARE, $3, $6); }
	|	';'	/* empty statement */ { $$ = NULL; }
	|	T_TRY '{' inner_statement_list '}' catch_list finally_statement
			{ $$ = zend_ast_create(ZEND_AST_TRY, $3, $5, $6); }
	|	T_THROW expr ';' { $$ = zend_ast_create(ZEND_AST_THROW, $2); }
	|	T_GOTO T_STRING ';' { $$ = zend_ast_create(ZEND_AST_GOTO, $2); }
	|	T_STRING ':' { $$ = zend_ast_create(ZEND_AST_LABEL, $1); }
;

catch_list:
		/* empty */
			{ $$ = zend_ast_create_list(0, ZEND_AST_CATCH_LIST); }
	|	catch_list T_CATCH '(' catch_name_list T_VARIABLE ')' '{' inner_statement_list '}'
			{ $$ = zend_ast_list_add($1, zend_ast_create(ZEND_AST_CATCH, $4, $5, $8)); }
;

catch_name_list:
		name { $$ = zend_ast_create_list(1, ZEND_AST_NAME_LIST, $1); }
	|	catch_name_list '|' name { $$ = zend_ast_list_add($1, $3); }
;

finally_statement:
		/* empty */ { $$ = NULL; }
	|	T_FINALLY '{' inner_statement_list '}' { $$ = $3; }
;

unset_variables:
		unset_variable { $$ = zend_ast_create_list(1, ZEND_AST_STMT_LIST, $1); }
	|	unset_variables ',' unset_variable { $$ = zend_ast_list_add($1, $3); }
;

unset_variable:
		variable { $$ = zend_ast_create(ZEND_AST_UNSET, $1); }
;

function_declaration_statement:
	function returns_ref T_STRING backup_doc_comment '(' parameter_list ')' return_type
	backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
		{ $$ = zend_ast_create_decl(ZEND_AST_FUNC_DECL, $2 | $13, $1, $4,
		      zend_ast_get_str($3), $6, NULL, $11, $8); CG(extra_fn_flags) = $9; }
;

is_reference:
		/* empty */	{ $$ = 0; }
	|	'&'			{ $$ = ZEND_PARAM_REF; }
;

is_variadic:
		/* empty */ { $$ = 0; }
	|	T_ELLIPSIS  { $$ = ZEND_PARAM_VARIADIC; }
;

class_declaration_statement:
		class_modifiers T_CLASS { $<num>$ = CG(zend_lineno); }
		T_STRING extends_from implements_list backup_doc_comment '{' class_statement_list '}'
			{ $$ = zend_ast_create_decl(ZEND_AST_CLASS, $1, $<num>3, $7, zend_ast_get_str($4), $5, $6, $9, NULL); }
	|	T_CLASS { $<num>$ = CG(zend_lineno); }
		T_STRING extends_from implements_list backup_doc_comment '{' class_statement_list '}'
			{ $$ = zend_ast_create_decl(ZEND_AST_CLASS, 0, $<num>2, $6, zend_ast_get_str($3), $4, $5, $8, NULL); }
;

class_modifiers:
		class_modifier 					{ $$ = $1; }
	|	class_modifiers class_modifier 	{ $$ = zend_add_class_modifier($1, $2); }
;

class_modifier:
		T_ABSTRACT 		{ $$ = ZEND_ACC_EXPLICIT_ABSTRACT_CLASS; }
	|	T_FINAL 		{ $$ = ZEND_ACC_FINAL; }
;

trait_declaration_statement:
		T_TRAIT { $<num>$ = CG(zend_lineno); }
		T_STRING backup_doc_comment '{' class_statement_list '}'
			{ $$ = zend_ast_create_decl(ZEND_AST_CLASS, ZEND_ACC_TRAIT, $<num>2, $4, zend_ast_get_str($3), NULL, NULL, $6, NULL); }
;

interface_declaration_statement:
		T_INTERFACE { $<num>$ = CG(zend_lineno); }
		T_STRING interface_extends_list backup_doc_comment '{' class_statement_list '}'
			{ $$ = zend_ast_create_decl(ZEND_AST_CLASS, ZEND_ACC_INTERFACE, $<num>2, $5, zend_ast_get_str($3), NULL, $4, $7, NULL); }
;

extends_from:
		/* empty */		{ $$ = NULL; }
	|	T_EXTENDS name	{ $$ = $2; }
;

interface_extends_list:
		/* empty */			{ $$ = NULL; }
	|	T_EXTENDS name_list	{ $$ = $2; }
;

implements_list:
		/* empty */				{ $$ = NULL; }
	|	T_IMPLEMENTS name_list	{ $$ = $2; }
;

foreach_variable:
		variable			{ $$ = $1; }
	|	'&' variable		{ $$ = zend_ast_create(ZEND_AST_REF, $2); }
	|	T_LIST '(' array_pair_list ')' { $3->attr = 1; $$ = $3; }
	|	'[' array_pair_list ']' { $$ = $2; }
;

for_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDFOR ';' { $$ = $2; }
;

foreach_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDFOREACH ';' { $$ = $2; }
;

declare_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDDECLARE ';' { $$ = $2; }
;

switch_case_list:
		'{' case_list '}'					{ $$ = $2; }
	|	'{' ';' case_list '}'				{ $$ = $3; }
	|	':' case_list T_ENDSWITCH ';'		{ $$ = $2; }
	|	':' ';' case_list T_ENDSWITCH ';'	{ $$ = $3; }
;

case_list:
		/* empty */ { $$ = zend_ast_create_list(0, ZEND_AST_SWITCH_LIST); }
	|	case_list T_CASE expr case_separator inner_statement_list
			{ $$ = zend_ast_list_add($1, zend_ast_create(ZEND_AST_SWITCH_CASE, $3, $5)); }
	|	case_list T_DEFAULT case_separator inner_statement_list
			{ $$ = zend_ast_list_add($1, zend_ast_create(ZEND_AST_SWITCH_CASE, NULL, $4)); }
;

case_separator:
		':'
	|	';'
;


while_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDWHILE ';' { $$ = $2; }
;


if_stmt_without_else:
		T_IF '(' expr ')' statement
			{ $$ = zend_ast_create_list(1, ZEND_AST_IF,
			      zend_ast_create(ZEND_AST_IF_ELEM, $3, $5)); }
	|	if_stmt_without_else T_ELSEIF '(' expr ')' statement
			{ $$ = zend_ast_list_add($1,
			      zend_ast_create(ZEND_AST_IF_ELEM, $4, $6)); }
;

if_stmt:
		if_stmt_without_else %prec T_NOELSE { $$ = $1; }
	|	if_stmt_without_else T_ELSE statement
			{ $$ = zend_ast_list_add($1, zend_ast_create(ZEND_AST_IF_ELEM, NULL, $3)); }
;

alt_if_stmt_without_else:
		T_IF '(' expr ')' ':' inner_statement_list
			{ $$ = zend_ast_create_list(1, ZEND_AST_IF,
			      zend_ast_create(ZEND_AST_IF_ELEM, $3, $6)); }
	|	alt_if_stmt_without_else T_ELSEIF '(' expr ')' ':' inner_statement_list
			{ $$ = zend_ast_list_add($1,
			      zend_ast_create(ZEND_AST_IF_ELEM, $4, $7)); }
;

alt_if_stmt:
		alt_if_stmt_without_else T_ENDIF ';' { $$ = $1; }
	|	alt_if_stmt_without_else T_ELSE ':' inner_statement_list T_ENDIF ';'
			{ $$ = zend_ast_list_add($1,
			      zend_ast_create(ZEND_AST_IF_ELEM, NULL, $4)); }
;

parameter_list:
		non_empty_parameter_list { $$ = $1; }
	|	/* empty */	{ $$ = zend_ast_create_list(0, ZEND_AST_PARAM_LIST); }
;


non_empty_parameter_list:
		parameter
			{ $$ = zend_ast_create_list(1, ZEND_AST_PARAM_LIST, $1); }
	|	non_empty_parameter_list ',' parameter
			{ $$ = zend_ast_list_add($1, $3); }
;

parameter:
		optional_type is_reference is_variadic T_VARIABLE
			{ $$ = zend_ast_create_ex(ZEND_AST_PARAM, $2 | $3, $1, $4, NULL); }
	|	optional_type is_reference is_variadic T_VARIABLE '=' expr
			{ $$ = zend_ast_create_ex(ZEND_AST_PARAM, $2 | $3, $1, $4, $6); }
;


optional_type:
		/* empty */	{ $$ = NULL; }
	|	type_expr	{ $$ = $1; }
;

type_expr:
		type		{ $$ = $1; }
	|	'?' type	{ $$ = $2; $$->attr |= ZEND_TYPE_NULLABLE; }
;

type:
		T_ARRAY		{ $$ = zend_ast_create_ex(ZEND_AST_TYPE, IS_ARRAY); }
	|	T_CALLABLE	{ $$ = zend_ast_create_ex(ZEND_AST_TYPE, IS_CALLABLE); }
	|	name		{ $$ = $1; }
;

return_type:
		/* empty */	{ $$ = NULL; }
	|	':' type_expr	{ $$ = $2; }
;

argument_list:
		'(' ')'	{ $$ = zend_ast_create_list(0, ZEND_AST_ARG_LIST); }
	|	'(' non_empty_argument_list ')' { $$ = $2; }
;

non_empty_argument_list:
		argument
			{ $$ = zend_ast_create_list(1, ZEND_AST_ARG_LIST, $1); }
	|	non_empty_argument_list ',' argument
			{ $$ = zend_ast_list_add($1, $3); }
;

argument:
		expr			{ $$ = $1; }
	|	T_ELLIPSIS expr	{ $$ = zend_ast_create(ZEND_AST_UNPACK, $2); }
;

global_var_list:
		global_var_list ',' global_var { $$ = zend_ast_list_add($1, $3); }
	|	global_var { $$ = zend_ast_create_list(1, ZEND_AST_STMT_LIST, $1); }
;

global_var:
	simple_variable
		{ $$ = zend_ast_create(ZEND_AST_GLOBAL, zend_ast_create(ZEND_AST_VAR, $1)); }
;


static_var_list:
		static_var_list ',' static_var { $$ = zend_ast_list_add($1, $3); }
	|	static_var { $$ = zend_ast_create_list(1, ZEND_AST_STMT_LIST, $1); }
;

static_var:
		T_VARIABLE			{ $$ = zend_ast_create(ZEND_AST_STATIC, $1, NULL); }
	|	T_VARIABLE '=' expr	{ $$ = zend_ast_create(ZEND_AST_STATIC, $1, $3); }
;


class_statement_list:
		class_statement_list class_statement
			{ $$ = zend_ast_list_add($1, $2); }
	|	/* empty */
			{ $$ = zend_ast_create_list(0, ZEND_AST_STMT_LIST); }
;


class_statement:
		variable_modifiers property_list ';'
			{ $$ = $2; $$->attr = $1; }
	|	method_modifiers T_CONST class_const_list ';'
			{ $$ = $3; $$->attr = $1; }
	|	T_USE name_list trait_adaptations
			{ $$ = zend_ast_create(ZEND_AST_USE_TRAIT, $2, $3); }
	|	method_modifiers function returns_ref identifier backup_doc_comment '(' parameter_list ')'
		return_type backup_fn_flags method_body backup_fn_flags
			{ $$ = zend_ast_create_decl(ZEND_AST_METHOD, $3 | $1 | $12, $2, $5,
				  zend_ast_get_str($4), $7, NULL, $11, $9); CG(extra_fn_flags) = $10; }
;

name_list:
		name { $$ = zend_ast_create_list(1, ZEND_AST_NAME_LIST, $1); }
	|	name_list ',' name { $$ = zend_ast_list_add($1, $3); }
;

trait_adaptations:
		';'								{ $$ = NULL; }
	|	'{' '}'							{ $$ = NULL; }
	|	'{' trait_adaptation_list '}'	{ $$ = $2; }
;

trait_adaptation_list:
		trait_adaptation
			{ $$ = zend_ast_create_list(1, ZEND_AST_TRAIT_ADAPTATIONS, $1); }
	|	trait_adaptation_list trait_adaptation
			{ $$ = zend_ast_list_add($1, $2); }
;

trait_adaptation:
		trait_precedence ';'	{ $$ = $1; }
	|	trait_alias ';'			{ $$ = $1; }
;

trait_precedence:
	absolute_trait_method_reference T_INSTEADOF name_list
		{ $$ = zend_ast_create(ZEND_AST_TRAIT_PRECEDENCE, $1, $3); }
;

trait_alias:
		trait_method_reference T_AS T_STRING
			{ $$ = zend_ast_create_ex(ZEND_AST_TRAIT_ALIAS, 0, $1, $3); }
	|	trait_method_reference T_AS reserved_non_modifiers
			{ zval zv; zend_lex_tstring(&zv); $$ = zend_ast_create_ex(ZEND_AST_TRAIT_ALIAS, 0, $1, zend_ast_create_zval(&zv)); }
	|	trait_method_reference T_AS member_modifier identifier
			{ $$ = zend_ast_create_ex(ZEND_AST_TRAIT_ALIAS, $3, $1, $4); }
	|	trait_method_reference T_AS member_modifier
			{ $$ = zend_ast_create_ex(ZEND_AST_TRAIT_ALIAS, $3, $1, NULL); }
;

trait_method_reference:
		identifier
			{ $$ = zend_ast_create(ZEND_AST_METHOD_REFERENCE, NULL, $1); }
	|	absolute_trait_method_reference { $$ = $1; }
;

absolute_trait_method_reference:
	name T_DOUBLE_COLON identifier
		{ $$ = zend_ast_create(ZEND_AST_METHOD_REFERENCE, $1, $3); }
;

method_body:
		';' /* abstract method */		{ $$ = NULL; }
	|	'{' inner_statement_list '}'	{ $$ = $2; }
;

variable_modifiers:
		non_empty_member_modifiers		{ $$ = $1; }
	|	T_VAR							{ $$ = ZEND_ACC_PUBLIC; }
;

method_modifiers:
		/* empty */						{ $$ = ZEND_ACC_PUBLIC; }
	|	non_empty_member_modifiers
			{ $$ = $1; if (!($$ & ZEND_ACC_PPP_MASK)) { $$ |= ZEND_ACC_PUBLIC; } }
;

non_empty_member_modifiers:
		member_modifier			{ $$ = $1; }
	|	non_empty_member_modifiers member_modifier
			{ $$ = zend_add_member_modifier($1, $2); }
;

member_modifier:
		T_PUBLIC				{ $$ = ZEND_ACC_PUBLIC; }
	|	T_PROTECTED				{ $$ = ZEND_ACC_PROTECTED; }
	|	T_PRIVATE				{ $$ = ZEND_ACC_PRIVATE; }
	|	T_STATIC				{ $$ = ZEND_ACC_STATIC; }
	|	T_ABSTRACT				{ $$ = ZEND_ACC_ABSTRACT; }
	|	T_FINAL					{ $$ = ZEND_ACC_FINAL; }
;

property_list:
		property_list ',' property { $$ = zend_ast_list_add($1, $3); }
	|	property { $$ = zend_ast_create_list(1, ZEND_AST_PROP_DECL, $1); }
;

property:
		T_VARIABLE backup_doc_comment
			{ $$ = zend_ast_create(ZEND_AST_PROP_ELEM, $1, NULL, ($2 ? zend_ast_create_zval_from_str($2) : NULL)); }
	|	T_VARIABLE '=' expr backup_doc_comment
			{ $$ = zend_ast_create(ZEND_AST_PROP_ELEM, $1, $3, ($4 ? zend_ast_create_zval_from_str($4) : NULL)); }
;

class_const_list:
		class_const_list ',' class_const_decl { $$ = zend_ast_list_add($1, $3); }
	|	class_const_decl { $$ = zend_ast_create_list(1, ZEND_AST_CLASS_CONST_DECL, $1); }
;

class_const_decl:
	identifier '=' expr backup_doc_comment { $$ = zend_ast_create(ZEND_AST_CONST_ELEM, $1, $3, ($4 ? zend_ast_create_zval_from_str($4) : NULL)); }
;

const_decl:
	T_STRING '=' expr backup_doc_comment { $$ = zend_ast_create(ZEND_AST_CONST_ELEM, $1, $3, ($4 ? zend_ast_create_zval_from_str($4) : NULL)); }
;

echo_expr_list:
		echo_expr_list ',' echo_expr { $$ = zend_ast_list_add($1, $3); }
	|	echo_expr { $$ = zend_ast_create_list(1, ZEND_AST_STMT_LIST, $1); }
;
echo_expr:
	expr { $$ = zend_ast_create(ZEND_AST_ECHO, $1); }
;

for_exprs:
		/* empty */			{ $$ = NULL; }
	|	non_empty_for_exprs	{ $$ = $1; }
;

non_empty_for_exprs:
		non_empty_for_exprs ',' expr { $$ = zend_ast_list_add($1, $3); }
	|	expr { $$ = zend_ast_create_list(1, ZEND_AST_EXPR_LIST, $1); }
;

anonymous_class:
        T_CLASS { $<num>$ = CG(zend_lineno); } ctor_arguments
		extends_from implements_list backup_doc_comment '{' class_statement_list '}' {
			zend_ast *decl = zend_ast_create_decl(
				ZEND_AST_CLASS, ZEND_ACC_ANON_CLASS, $<num>2, $6, NULL,
				$4, $5, $8, NULL);
			$$ = zend_ast_create(ZEND_AST_NEW, decl, $3);
		}
;

new_expr:
		T_NEW class_name_reference ctor_arguments
			{ $$ = zend_ast_create(ZEND_AST_NEW, $2, $3); }
	|	T_NEW anonymous_class
			{ $$ = $2; }
;

expr_without_variable:
		T_LIST '(' array_pair_list ')' '=' expr
			{ $3->attr = 1; $$ = zend_ast_create(ZEND_AST_ASSIGN, $3, $6); }
	|	'[' array_pair_list ']' '=' expr
			{ $$ = zend_ast_create(ZEND_AST_ASSIGN, $2, $5); }
	|	variable '=' expr
			{ $$ = zend_ast_create(ZEND_AST_ASSIGN, $1, $3); }
	|	variable '=' '&' variable
			{ $$ = zend_ast_create(ZEND_AST_ASSIGN_REF, $1, $4); }
	|	T_CLONE expr { $$ = zend_ast_create(ZEND_AST_CLONE, $2); }
	|	variable T_PLUS_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_ADD, $1, $3); }
	|	variable T_MINUS_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_SUB, $1, $3); }
	|	variable T_MUL_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_MUL, $1, $3); }
	|	variable T_POW_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_POW, $1, $3); }
	|	variable T_DIV_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_DIV, $1, $3); }
	|	variable T_CONCAT_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_CONCAT, $1, $3); }
	|	variable T_MOD_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_MOD, $1, $3); }
	|	variable T_AND_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_BW_AND, $1, $3); }
	|	variable T_OR_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_BW_OR, $1, $3); }
	|	variable T_XOR_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_BW_XOR, $1, $3); }
	|	variable T_SL_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_SL, $1, $3); }
	|	variable T_SR_EQUAL expr
			{ $$ = zend_ast_create_assign_op(ZEND_ASSIGN_SR, $1, $3); }
	|	variable T_INC { $$ = zend_ast_create(ZEND_AST_POST_INC, $1); }
	|	T_INC variable { $$ = zend_ast_create(ZEND_AST_PRE_INC, $2); }
	|	variable T_DEC { $$ = zend_ast_create(ZEND_AST_POST_DEC, $1); }
	|	T_DEC variable { $$ = zend_ast_create(ZEND_AST_PRE_DEC, $2); }
	|	expr T_BOOLEAN_OR expr
			{ $$ = zend_ast_create(ZEND_AST_OR, $1, $3); }
	|	expr T_BOOLEAN_AND expr
			{ $$ = zend_ast_create(ZEND_AST_AND, $1, $3); }
	|	expr T_LOGICAL_OR expr
			{ $$ = zend_ast_create(ZEND_AST_OR, $1, $3); }
	|	expr T_LOGICAL_AND expr
			{ $$ = zend_ast_create(ZEND_AST_AND, $1, $3); }
	|	expr T_LOGICAL_XOR expr
			{ $$ = zend_ast_create_binary_op(ZEND_BOOL_XOR, $1, $3); }
	|	expr '|' expr	{ $$ = zend_ast_create_binary_op(ZEND_BW_OR, $1, $3); }
	|	expr '&' expr	{ $$ = zend_ast_create_binary_op(ZEND_BW_AND, $1, $3); }
	|	expr '^' expr	{ $$ = zend_ast_create_binary_op(ZEND_BW_XOR, $1, $3); }
	|	expr '.' expr 	{ $$ = zend_ast_create_binary_op(ZEND_CONCAT, $1, $3); }
	|	expr '+' expr 	{ $$ = zend_ast_create_binary_op(ZEND_ADD, $1, $3); }
	|	expr '-' expr 	{ $$ = zend_ast_create_binary_op(ZEND_SUB, $1, $3); }
	|	expr '*' expr	{ $$ = zend_ast_create_binary_op(ZEND_MUL, $1, $3); }
	|	expr T_POW expr	{ $$ = zend_ast_create_binary_op(ZEND_POW, $1, $3); }
	|	expr '/' expr	{ $$ = zend_ast_create_binary_op(ZEND_DIV, $1, $3); }
	|	expr '%' expr 	{ $$ = zend_ast_create_binary_op(ZEND_MOD, $1, $3); }
	| 	expr T_SL expr	{ $$ = zend_ast_create_binary_op(ZEND_SL, $1, $3); }
	|	expr T_SR expr	{ $$ = zend_ast_create_binary_op(ZEND_SR, $1, $3); }
	|	'+' expr %prec T_INC { $$ = zend_ast_create(ZEND_AST_UNARY_PLUS, $2); }
	|	'-' expr %prec T_INC { $$ = zend_ast_create(ZEND_AST_UNARY_MINUS, $2); }
	|	'!' expr { $$ = zend_ast_create_ex(ZEND_AST_UNARY_OP, ZEND_BOOL_NOT, $2); }
	|	'~' expr { $$ = zend_ast_create_ex(ZEND_AST_UNARY_OP, ZEND_BW_NOT, $2); }
	|	expr T_IS_IDENTICAL expr
			{ $$ = zend_ast_create_binary_op(ZEND_IS_IDENTICAL, $1, $3); }
	|	expr T_IS_NOT_IDENTICAL expr
			{ $$ = zend_ast_create_binary_op(ZEND_IS_NOT_IDENTICAL, $1, $3); }
	|	expr T_IS_EQUAL expr
			{ $$ = zend_ast_create_binary_op(ZEND_IS_EQUAL, $1, $3); }
	|	expr T_IS_NOT_EQUAL expr
			{ $$ = zend_ast_create_binary_op(ZEND_IS_NOT_EQUAL, $1, $3); }
	|	expr '<' expr
			{ $$ = zend_ast_create_binary_op(ZEND_IS_SMALLER, $1, $3); }
	|	expr T_IS_SMALLER_OR_EQUAL expr
			{ $$ = zend_ast_create_binary_op(ZEND_IS_SMALLER_OR_EQUAL, $1, $3); }
	|	expr '>' expr
			{ $$ = zend_ast_create(ZEND_AST_GREATER, $1, $3); }
	|	expr T_IS_GREATER_OR_EQUAL expr
			{ $$ = zend_ast_create(ZEND_AST_GREATER_EQUAL, $1, $3); }
	|	expr T_SPACESHIP expr
			{ $$ = zend_ast_create_binary_op(ZEND_SPACESHIP, $1, $3); }
	|	expr T_INSTANCEOF class_name_reference
			{ $$ = zend_ast_create(ZEND_AST_INSTANCEOF, $1, $3); }
	|	'(' expr ')' { $$ = $2; }
	|	new_expr { $$ = $1; }
	|	expr '?' expr ':' expr
			{ $$ = zend_ast_create(ZEND_AST_CONDITIONAL, $1, $3, $5); }
	|	expr '?' ':' expr
			{ $$ = zend_ast_create(ZEND_AST_CONDITIONAL, $1, NULL, $4); }
	|	expr T_COALESCE expr
			{ $$ = zend_ast_create(ZEND_AST_COALESCE, $1, $3); }
	|	internal_functions_in_yacc { $$ = $1; }
	|	T_INT_CAST expr		{ $$ = zend_ast_create_cast(IS_LONG, $2); }
	|	T_DOUBLE_CAST expr	{ $$ = zend_ast_create_cast(IS_DOUBLE, $2); }
	|	T_STRING_CAST expr	{ $$ = zend_ast_create_cast(IS_STRING, $2); }
	|	T_ARRAY_CAST expr	{ $$ = zend_ast_create_cast(IS_ARRAY, $2); }
	|	T_OBJECT_CAST expr	{ $$ = zend_ast_create_cast(IS_OBJECT, $2); }
	|	T_BOOL_CAST expr	{ $$ = zend_ast_create_cast(_IS_BOOL, $2); }
	|	T_UNSET_CAST expr	{ $$ = zend_ast_create_cast(IS_NULL, $2); }
	|	T_EXIT exit_expr	{ $$ = zend_ast_create(ZEND_AST_EXIT, $2); }
	|	'@' expr			{ $$ = zend_ast_create(ZEND_AST_SILENCE, $2); }
	|	scalar { $$ = $1; }
	|	'`' backticks_expr '`' { $$ = zend_ast_create(ZEND_AST_SHELL_EXEC, $2); }
	|	T_PRINT expr { $$ = zend_ast_create(ZEND_AST_PRINT, $2); }
	|	T_YIELD { $$ = zend_ast_create(ZEND_AST_YIELD, NULL, NULL); CG(extra_fn_flags) |= ZEND_ACC_GENERATOR; }
	|	T_YIELD expr { $$ = zend_ast_create(ZEND_AST_YIELD, $2, NULL); CG(extra_fn_flags) |= ZEND_ACC_GENERATOR; }
	|	T_YIELD expr T_DOUBLE_ARROW expr { $$ = zend_ast_create(ZEND_AST_YIELD, $4, $2); CG(extra_fn_flags) |= ZEND_ACC_GENERATOR; }
	|	T_YIELD_FROM expr { $$ = zend_ast_create(ZEND_AST_YIELD_FROM, $2); CG(extra_fn_flags) |= ZEND_ACC_GENERATOR; }
	|	function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars return_type
		backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
			{ $$ = zend_ast_create_decl(ZEND_AST_CLOSURE, $2 | $13, $1, $3,
				  zend_string_init("{closure}", sizeof("{closure}") - 1, 0),
			      $5, $7, $11, $8); CG(extra_fn_flags) = $9; }
	|	T_STATIC function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars
		return_type backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
			{ $$ = zend_ast_create_decl(ZEND_AST_CLOSURE, $3 | $14 | ZEND_ACC_STATIC, $2, $4,
			      zend_string_init("{closure}", sizeof("{closure}") - 1, 0),
			      $6, $8, $12, $9); CG(extra_fn_flags) = $10; }
;

function:
	T_FUNCTION { $$ = CG(zend_lineno); }
;

backup_doc_comment:
	/* empty */ { $$ = CG(doc_comment); CG(doc_comment) = NULL; }
;

backup_fn_flags:
	/* empty */ { $$ = CG(extra_fn_flags); CG(extra_fn_flags) = 0; }
;

returns_ref:
		/* empty */	{ $$ = 0; }
	|	'&'			{ $$ = ZEND_ACC_RETURN_REFERENCE; }
;

lexical_vars:
		/* empty */ { $$ = NULL; }
	|	T_USE '(' lexical_var_list ')' { $$ = $3; }
;

lexical_var_list:
		lexical_var_list ',' lexical_var { $$ = zend_ast_list_add($1, $3); }
	|	lexical_var { $$ = zend_ast_create_list(1, ZEND_AST_CLOSURE_USES, $1); }
;

lexical_var:
		T_VARIABLE		{ $$ = $1; }
	|	'&' T_VARIABLE	{ $$ = $2; $$->attr = 1; }
;

function_call:
		name argument_list
			{ $$ = zend_ast_create(ZEND_AST_CALL, $1, $2); }
	|	class_name T_DOUBLE_COLON member_name argument_list
			{ $$ = zend_ast_create(ZEND_AST_STATIC_CALL, $1, $3, $4); }
	|	variable_class_name T_DOUBLE_COLON member_name argument_list
			{ $$ = zend_ast_create(ZEND_AST_STATIC_CALL, $1, $3, $4); }
	|	callable_expr argument_list
			{ $$ = zend_ast_create(ZEND_AST_CALL, $1, $2); }
;

class_name:
		T_STATIC
			{ zval zv; ZVAL_INTERNED_STR(&zv, CG(known_strings)[ZEND_STR_STATIC]);
			  $$ = zend_ast_create_zval_ex(&zv, ZEND_NAME_NOT_FQ); }
	|	name { $$ = $1; }
;

class_name_reference:
		class_name		{ $$ = $1; }
	|	new_variable	{ $$ = $1; }
;

exit_expr:
		/* empty */				{ $$ = NULL; }
	|	'(' optional_expr ')'	{ $$ = $2; }
;

backticks_expr:
		/* empty */
			{ $$ = zend_ast_create_zval_from_str(ZSTR_EMPTY_ALLOC()); }
	|	T_ENCAPSED_AND_WHITESPACE { $$ = $1; }
	|	encaps_list { $$ = $1; }
;


ctor_arguments:
		/* empty */	{ $$ = zend_ast_create_list(0, ZEND_AST_ARG_LIST); }
	|	argument_list { $$ = $1; }
;


dereferencable_scalar:
		T_ARRAY '(' array_pair_list ')'	{ $$ = $3; }
	|	'[' array_pair_list ']'			{ $$ = $2; }
	|	T_CONSTANT_ENCAPSED_STRING		{ $$ = $1; }
;

scalar:
		T_LNUMBER 	{ $$ = $1; }
	|	T_DNUMBER 	{ $$ = $1; }
	|	T_LINE 		{ $$ = zend_ast_create_ex(ZEND_AST_MAGIC_CONST, T_LINE); }
	|	T_FILE 		{ $$ = zend_ast_create_ex(ZEND_AST_MAGIC_CONST, T_FILE); }
	|	T_DIR   	{ $$ = zend_ast_create_ex(ZEND_AST_MAGIC_CONST, T_DIR); }
	|	T_TRAIT_C	{ $$ = zend_ast_create_ex(ZEND_AST_MAGIC_CONST, T_TRAIT_C); }
	|	T_METHOD_C	{ $$ = zend_ast_create_ex(ZEND_AST_MAGIC_CONST, T_METHOD_C); }
	|	T_FUNC_C	{ $$ = zend_ast_create_ex(ZEND_AST_MAGIC_CONST, T_FUNC_C); }
	|	T_NS_C		{ $$ = zend_ast_create_ex(ZEND_AST_MAGIC_CONST, T_NS_C); }
	|	T_CLASS_C	{ $$ = zend_ast_create_ex(ZEND_AST_MAGIC_CONST, T_CLASS_C); }
	|	T_START_HEREDOC T_ENCAPSED_AND_WHITESPACE T_END_HEREDOC { $$ = $2; }
	|	T_START_HEREDOC T_END_HEREDOC
			{ $$ = zend_ast_create_zval_from_str(ZSTR_EMPTY_ALLOC()); }
	|	'"' encaps_list '"' 	{ $$ = $2; }
	|	T_START_HEREDOC encaps_list T_END_HEREDOC { $$ = $2; }
	|	dereferencable_scalar	{ $$ = $1; }
	|	constant			{ $$ = $1; }
;

constant:
		name { $$ = zend_ast_create(ZEND_AST_CONST, $1); }
	|	class_name T_DOUBLE_COLON identifier
			{ $$ = zend_ast_create(ZEND_AST_CLASS_CONST, $1, $3); }
	|	variable_class_name T_DOUBLE_COLON identifier
			{ $$ = zend_ast_create(ZEND_AST_CLASS_CONST, $1, $3); }
;

expr:
		variable					{ $$ = $1; }
	|	expr_without_variable		{ $$ = $1; }
;

optional_expr:
		/* empty */	{ $$ = NULL; }
	|	expr		{ $$ = $1; }
;

variable_class_name:
	dereferencable { $$ = $1; }
;

dereferencable:
		variable				{ $$ = $1; }
	|	'(' expr ')'			{ $$ = $2; }
	|	dereferencable_scalar	{ $$ = $1; }
;

callable_expr:
		callable_variable		{ $$ = $1; }
	|	'(' expr ')'			{ $$ = $2; }
	|	dereferencable_scalar	{ $$ = $1; }
;

callable_variable:
		simple_variable
			{ $$ = zend_ast_create(ZEND_AST_VAR, $1); }
	|	dereferencable '[' optional_expr ']'
			{ $$ = zend_ast_create(ZEND_AST_DIM, $1, $3); }
	|	constant '[' optional_expr ']'
			{ $$ = zend_ast_create(ZEND_AST_DIM, $1, $3); }
	|	dereferencable '{' expr '}'
			{ $$ = zend_ast_create(ZEND_AST_DIM, $1, $3); }
	|	dereferencable T_OBJECT_OPERATOR property_name argument_list
			{ $$ = zend_ast_create(ZEND_AST_METHOD_CALL, $1, $3, $4); }
	|	function_call { $$ = $1; }
;

variable:
		callable_variable
			{ $$ = $1; }
	|	static_member
			{ $$ = $1; }
	|	dereferencable T_OBJECT_OPERATOR property_name
			{ $$ = zend_ast_create(ZEND_AST_PROP, $1, $3); }
;

simple_variable:
		T_VARIABLE			{ $$ = $1; }
	|	'$' '{' expr '}'	{ $$ = $3; }
	|	'$' simple_variable	{ $$ = zend_ast_create(ZEND_AST_VAR, $2); }
;

static_member:
		class_name T_DOUBLE_COLON simple_variable
			{ $$ = zend_ast_create(ZEND_AST_STATIC_PROP, $1, $3); }
	|	variable_class_name T_DOUBLE_COLON simple_variable
			{ $$ = zend_ast_create(ZEND_AST_STATIC_PROP, $1, $3); }
;

new_variable:
		simple_variable
			{ $$ = zend_ast_create(ZEND_AST_VAR, $1); }
	|	new_variable '[' optional_expr ']'
			{ $$ = zend_ast_create(ZEND_AST_DIM, $1, $3); }
	|	new_variable '{' expr '}'
			{ $$ = zend_ast_create(ZEND_AST_DIM, $1, $3); }
	|	new_variable T_OBJECT_OPERATOR property_name
			{ $$ = zend_ast_create(ZEND_AST_PROP, $1, $3); }
	|	class_name T_DOUBLE_COLON simple_variable
			{ $$ = zend_ast_create(ZEND_AST_STATIC_PROP, $1, $3); }
	|	new_variable T_DOUBLE_COLON simple_variable
			{ $$ = zend_ast_create(ZEND_AST_STATIC_PROP, $1, $3); }
;

member_name:
		identifier { $$ = $1; }
	|	'{' expr '}'	{ $$ = $2; }
	|	simple_variable	{ $$ = zend_ast_create(ZEND_AST_VAR, $1); }
;

property_name:
		T_STRING { $$ = $1; }
	|	'{' expr '}'	{ $$ = $2; }
	|	simple_variable	{ $$ = zend_ast_create(ZEND_AST_VAR, $1); }
;

array_pair_list:
		non_empty_array_pair_list
			{ /* allow single trailing comma */ $$ = zend_ast_list_rtrim($1); }
;

possible_array_pair:
		/* empty */ { $$ = NULL; }
	|	array_pair  { $$ = $1; }
;

non_empty_array_pair_list:
		non_empty_array_pair_list ',' possible_array_pair
			{ $$ = zend_ast_list_add($1, $3); }
	|	possible_array_pair
			{ $$ = zend_ast_create_list(1, ZEND_AST_ARRAY, $1); }
;

array_pair:
		expr T_DOUBLE_ARROW expr
			{ $$ = zend_ast_create(ZEND_AST_ARRAY_ELEM, $3, $1); }
	|	expr
			{ $$ = zend_ast_create(ZEND_AST_ARRAY_ELEM, $1, NULL); }
	|	expr T_DOUBLE_ARROW '&' variable
			{ $$ = zend_ast_create_ex(ZEND_AST_ARRAY_ELEM, 1, $4, $1); }
	|	'&' variable
			{ $$ = zend_ast_create_ex(ZEND_AST_ARRAY_ELEM, 1, $2, NULL); }
	|	expr T_DOUBLE_ARROW T_LIST '(' array_pair_list ')'
			{ $5->attr = 1; $$ = zend_ast_create(ZEND_AST_ARRAY_ELEM, $5, $1); }
	|	T_LIST '(' array_pair_list ')'
			{ $3->attr = 1; $$ = zend_ast_create(ZEND_AST_ARRAY_ELEM, $3, NULL); }
;

encaps_list:
		encaps_list encaps_var
			{ $$ = zend_ast_list_add($1, $2); }
	|	encaps_list T_ENCAPSED_AND_WHITESPACE
			{ $$ = zend_ast_list_add($1, $2); }
	|	encaps_var
			{ $$ = zend_ast_create_list(1, ZEND_AST_ENCAPS_LIST, $1); }
	|	T_ENCAPSED_AND_WHITESPACE encaps_var
			{ $$ = zend_ast_create_list(2, ZEND_AST_ENCAPS_LIST, $1, $2); }
;

encaps_var:
		T_VARIABLE
			{ $$ = zend_ast_create(ZEND_AST_VAR, $1); }
	|	T_VARIABLE '[' encaps_var_offset ']'
			{ $$ = zend_ast_create(ZEND_AST_DIM,
			      zend_ast_create(ZEND_AST_VAR, $1), $3); }
	|	T_VARIABLE T_OBJECT_OPERATOR T_STRING
			{ $$ = zend_ast_create(ZEND_AST_PROP,
			      zend_ast_create(ZEND_AST_VAR, $1), $3); }
	|	T_DOLLAR_OPEN_CURLY_BRACES expr '}'
			{ $$ = zend_ast_create(ZEND_AST_VAR, $2); }
	|	T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '}'
			{ $$ = zend_ast_create(ZEND_AST_VAR, $2); }
	|	T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '[' expr ']' '}'
			{ $$ = zend_ast_create(ZEND_AST_DIM,
			      zend_ast_create(ZEND_AST_VAR, $2), $4); }
	|	T_CURLY_OPEN variable '}' { $$ = $2; }
;

encaps_var_offset:
		T_STRING		{ $$ = $1; }
	|	T_NUM_STRING	{ $$ = $1; }
	|	T_VARIABLE		{ $$ = zend_ast_create(ZEND_AST_VAR, $1); }
;


internal_functions_in_yacc:
		T_ISSET '(' isset_variables ')' { $$ = $3; }
	|	T_EMPTY '(' expr ')' { $$ = zend_ast_create(ZEND_AST_EMPTY, $3); }
	|	T_INCLUDE expr
			{ $$ = zend_ast_create_ex(ZEND_AST_INCLUDE_OR_EVAL, ZEND_INCLUDE, $2); }
	|	T_INCLUDE_ONCE expr
			{ $$ = zend_ast_create_ex(ZEND_AST_INCLUDE_OR_EVAL, ZEND_INCLUDE_ONCE, $2); }
	|	T_EVAL '(' expr ')'
			{ $$ = zend_ast_create_ex(ZEND_AST_INCLUDE_OR_EVAL, ZEND_EVAL, $3); }
	|	T_REQUIRE expr
			{ $$ = zend_ast_create_ex(ZEND_AST_INCLUDE_OR_EVAL, ZEND_REQUIRE, $2); }
	|	T_REQUIRE_ONCE expr
			{ $$ = zend_ast_create_ex(ZEND_AST_INCLUDE_OR_EVAL, ZEND_REQUIRE_ONCE, $2); }
;

isset_variables:
		isset_variable { $$ = $1; }
	|	isset_variables ',' isset_variable
			{ $$ = zend_ast_create(ZEND_AST_AND, $1, $3); }
;

isset_variable:
		expr { $$ = zend_ast_create(ZEND_AST_ISSET, $1); }
;

%%

#endif
}
}
