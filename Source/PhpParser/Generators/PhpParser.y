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
using PHP.Syntax;

%%

%namespace PhpParser.Parser
%valuetype SemanticValueType
%positiontype Span
%tokentype Tokens
%visibility public

%valuetypeattributes StructLayout(LayoutKind.Explicit)

%union
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

	[FieldOffset(16)]
	public int Attr;
}


%left T_INCLUDE T_INCLUDE_ONCE T_EVAL T_REQUIRE T_REQUIRE_ONCE
%left ','
%left T_LOGICAL_OR
%left T_LOGICAL_XOR
%left T_LOGICAL_AND
%right T_PRINT
%right T_YIELD
%right T_DOUBLE_ARROW
%right T_YIELD_FROM
%left '=' T_PLUS_EQUAL T_MINUS_EQUAL T_MUL_EQUAL T_DIV_EQUAL T_CONCAT_EQUAL T_MOD_EQUAL T_AND_EQUAL T_OR_EQUAL T_XOR_EQUAL T_SL_EQUAL T_SR_EQUAL T_POW_EQUAL
%left '?' ':'
%right T_COALESCE
%left T_BOOLEAN_OR
%left T_BOOLEAN_AND
%left '|'
%left '^'
%left '&'
%nonassoc T_IS_EQUAL T_IS_NOT_EQUAL T_IS_IDENTICAL T_IS_NOT_IDENTICAL T_SPACESHIP
%nonassoc '<' T_IS_SMALLER_OR_EQUAL '>' T_IS_GREATER_OR_EQUAL
%left T_SL T_SR
%left '+' '-' '.'
%left '*' '/' '%'
%right '!'
%nonassoc T_INSTANCEOF
%right '~' T_INC T_DEC T_INT_CAST T_DOUBLE_CAST T_STRING_CAST T_ARRAY_CAST T_OBJECT_CAST T_BOOL_CAST T_UNSET_CAST '@'
%right T_POW
%right '['
%nonassoc T_NEW T_CLONE
%left T_NOELSE
%left T_ELSEIF
%left T_ELSE
%left T_ENDIF
%right T_STATIC T_ABSTRACT T_FINAL T_PRIVATE T_PROTECTED T_PUBLIC

%token <Long> T_LNUMBER 317   //"integer number (T_LNUMBER)"
%token <Double> T_DNUMBER 318   //"floating-point number (T_DNUMBER)"
%token <Object> T_STRING 319   //"identifier (T_STRING)"
%token <Object> T_VARIABLE 320 //"variable (T_VARIABLE)"
%token <Object> T_INLINE_HTML 321
%token <Object> T_ENCAPSED_AND_WHITESPACE 322  //"quoted-string and whitespace (T_ENCAPSED_AND_WHITESPACE)"
%token <Object> T_CONSTANT_ENCAPSED_STRING 323 //"quoted-string (T_CONSTANT_ENCAPSED_STRING)"
%token <Object> T_STRING_VARNAME 324 //"variable name (T_STRING_VARNAME)"
%token <Double> T_NUM_STRING 325 //"number (T_NUM_STRING)"

/* Character tokens */
%token T_EXCLAM 33 //'!'
%token T_DOUBLE_QUOTES 34 //'"'
%token T_DOLLAR 36 //'$'
%token T_PERCENT 37 //'%'
%token T_AMP 38 //'&'
%token T_LPAREN 40 //'('
%token T_RPAREN 41 //')'
%token T_MUL 42 //'*'
%token T_PLUS 43 //'+'
%token T_COMMA 44 //''
%token T_MINUS 45 //'-'
%token T_DOT 46 //'.'
%token T_SLASH 47 //'/'
%token T_COLON 58 //':'
%token T_SEMI 59 //';'
%token T_LT 60 //'<'
%token T_EQ 61 //'='
%token T_GT 62 //'>'
%token T_QUESTION 63 //'?'
%token T_AT 64 //'@'
%token T_LBRACKET 91 //'['
%token T_RBRACKET 93 //']'
%token T_CARET 94 //'^'
%token T_BACKQUOTE 96 //'`'
%token T_LBRACE 123 //'{'
%token T_PIPE 124 //'|'
%token T_RBRACE 125 //'}'
%token T_TILDE 126 //'~'

%token <Object> END 0 //"end of file"
%token <Object> T_INCLUDE 262      //"include (T_INCLUDE)"
%token <Object> T_INCLUDE_ONCE 261 //"include_once (T_INCLUDE_ONCE)"
%token <Object> T_EVAL 260         //"eval (T_EVAL)"
%token <Object> T_REQUIRE 259      //"require (T_REQUIRE)"
%token <Object> T_REQUIRE_ONCE 258 //"require_once (T_REQUIRE_ONCE)"
%token <Object> T_LOGICAL_OR 263   //"or (T_LOGICAL_OR)"
%token <Object> T_LOGICAL_XOR 264  //"xor (T_LOGICAL_XOR)"
%token <Object> T_LOGICAL_AND 265  //"and (T_LOGICAL_AND)"
%token <Object> T_PRINT 266        //"print (T_PRINT)"
%token <Object> T_YIELD 267        //"yield (T_YIELD)"
%token <Object> T_YIELD_FROM 269    //"yield from (T_YIELD_FROM)"
%token <Object> T_PLUS_EQUAL 281  //"+= (T_PLUS_EQUAL)"
%token <Object> T_MINUS_EQUAL 280  //"-= (T_MINUS_EQUAL)"
%token <Object> T_MUL_EQUAL 279    //"*= (T_MUL_EQUAL)"
%token <Object> T_DIV_EQUAL 278   //"/= (T_DIV_EQUAL)"
%token <Object> T_CONCAT_EQUAL 277 //".= (T_CONCAT_EQUAL)"
%token <Object> T_MOD_EQUAL 276    //"%= (T_MOD_EQUAL)"
%token <Object> T_AND_EQUAL 275    //"&= (T_AND_EQUAL)"
%token <Object> T_OR_EQUAL 274     //"|= (T_OR_EQUAL)"
%token <Object> T_XOR_EQUAL 273    //"^= (T_XOR_EQUAL)"
%token <Object> T_SL_EQUAL 272    //"<<= (T_SL_EQUAL)"
%token <Object> T_SR_EQUAL 271     //">>= (T_SR_EQUAL)"
%token <Object> T_BOOLEAN_OR 283  //"|| (T_BOOLEAN_OR)"
%token <Object> T_BOOLEAN_AND 284  //"&& (T_BOOLEAN_AND)"
%token <Object> T_IS_EQUAL 289     //"== (T_IS_EQUAL)"
%token <Object> T_IS_NOT_EQUAL 288 //"!= (T_IS_NOT_EQUAL)"
%token <Object> T_IS_IDENTICAL 287 //"=== (T_IS_IDENTICAL)"
%token <Object> T_IS_NOT_IDENTICAL 286 //"!== (T_IS_NOT_IDENTICAL)"
%token <Object> T_IS_SMALLER_OR_EQUAL 291 //"<= (T_IS_SMALLER_OR_EQUAL)"
%token <Object> T_IS_GREATER_OR_EQUAL 290 //">= (T_IS_GREATER_OR_EQUAL)"
%token <Object> T_SPACESHIP 285 //"<=> (T_SPACESHIP)"
%token <Object> T_SL 293 //"<< (T_SL)"
%token <Object> T_SR 292 //">> (T_SR)"
%token <Object> T_INSTANCEOF 294  //"instanceof (T_INSTANCEOF)"
%token <Object> T_INC 303 //"++ (T_INC)"
%token <Object> T_DEC 302 //"-- (T_DEC)"
%token <Object> T_INT_CAST 301   //"(int) (T_INT_CAST)"
%token <Object> T_DOUBLE_CAST 300 //"(double) (T_DOUBLE_CAST)"
%token <Object> T_STRING_CAST 299 //"(string) (T_STRING_CAST)"
%token <Object> T_ARRAY_CAST 298  //"(array) (T_ARRAY_CAST)"
%token <Object> T_OBJECT_CAST 297 //"(object) (T_OBJECT_CAST)"
%token <Object> T_BOOL_CAST 296   //"(bool) (T_BOOL_CAST)"
%token <Object> T_UNSET_CAST 295  //"(unset) (T_UNSET_CAST)"
%token <Object> T_NEW 306      //"new (T_NEW)"
%token <Object> T_CLONE 305     //"clone (T_CLONE)"
%token <Object> T_EXIT 326      //"exit (T_EXIT)"
%token <Object> T_IF 327       //"if (T_IF)"
%token <Object> T_ELSEIF 308   //"elseif (T_ELSEIF)"
%token <Object> T_ELSE 309     //"else (T_ELSE)"
%token <Object> T_ENDIF 310    //"endif (T_ENDIF)"
%token <Object> T_ECHO 328      //"echo (T_ECHO)"
%token <Object> T_DO 329        //"do (T_DO)"
%token <Object> T_WHILE 330     //"while (T_WHILE)"
%token <Object> T_ENDWHILE 331  //"endwhile (T_ENDWHILE)"
%token <Object> T_FOR 332       //"for (T_FOR)"
%token <Object> T_ENDFOR 333    //"endfor (T_ENDFOR)"
%token <Object> T_FOREACH 334   //"foreach (T_FOREACH)"
%token <Object> T_ENDFOREACH 335  //"endforeach (T_ENDFOREACH)"
%token <Object> T_DECLARE 336   //"declare (T_DECLARE)"
%token <Object> T_ENDDECLARE 337 //"enddeclare (T_ENDDECLARE)"
%token <Object> T_AS 338        //"as (T_AS)"
%token <Object> T_SWITCH 339    //"switch (T_SWITCH)"
%token <Object> T_ENDSWITCH 340 //"endswitch (T_ENDSWITCH)"
%token <Object> T_CASE 341      //"case (T_CASE)"
%token <Object> T_DEFAULT 342   //"default (T_DEFAULT)"
%token <Object> T_BREAK 343     //"break (T_BREAK)"
%token <Object> T_CONTINUE 344  //"continue (T_CONTINUE)"
%token <Object> T_GOTO 345      //"goto (T_GOTO)"
%token <Object> T_FUNCTION 346   //"function (T_FUNCTION)"
%token <Object> T_CONST 347     //"const (T_CONST)"
%token <Object> T_RETURN 348     //"return (T_RETURN)"
%token <Object> T_TRY 349       //"try (T_TRY)"
%token <Object> T_CATCH 350     //"catch (T_CATCH)"
%token <Object> T_FINALLY 351   //"finally (T_FINALLY)"
%token <Object> T_THROW 352     //"throw (T_THROW)"
%token <Object> T_USE 353       //"use (T_USE)"
%token <Object> T_INSTEADOF 354  //"insteadof (T_INSTEADOF)"
%token <Object> T_GLOBAL 355    //"global (T_GLOBAL)"
%token <Object> T_STATIC 316    //"static (T_STATIC)"
%token <Object> T_ABSTRACT 315  //"abstract (T_ABSTRACT)"
%token <Object> T_FINAL 314     //"final (T_FINAL)"
%token <Object> T_PRIVATE 313   //"private (T_PRIVATE)"
%token <Object> T_PROTECTED 312 //"protected (T_PROTECTED)"
%token <Object> T_PUBLIC 311    //"public (T_PUBLIC)"
%token <Object> T_VAR 356        //"var (T_VAR)"
%token <Object> T_UNSET 357     //"unset (T_UNSET)"
%token <Object> T_ISSET 358     //"isset (T_ISSET)"
%token <Object> T_EMPTY 359     //"empty (T_EMPTY)"
%token <Object> T_HALT_COMPILER 360 //"__halt_compiler (T_HALT_COMPILER)"
%token <Object> T_CLASS 361     //"class (T_CLASS)"
%token <Object> T_TRAIT 362     //"trait (T_TRAIT)"
%token <Object> T_INTERFACE 363 //"interface (T_INTERFACE)"
%token <Object> T_EXTENDS 364   //"extends (T_EXTENDS)"
%token <Object> T_IMPLEMENTS 365 //"implements (T_IMPLEMENTS)"
%token <Object> T_OBJECT_OPERATOR 366 //"-> (T_OBJECT_OPERATOR)"
%token <Object> T_DOUBLE_ARROW 268    //"=> (T_DOUBLE_ARROW)"
%token <Object> T_LIST 367           //"list (T_LIST)"
%token <Object> T_ARRAY 368          //"array (T_ARRAY)"
%token <Object> T_CALLABLE 369       //"callable (T_CALLABLE)"
%token <Object> T_LINE 370           //"__LINE__ (T_LINE)"
%token <Object> T_FILE 371           //"__FILE__ (T_FILE)"
%token <Object> T_DIR 372            //"__DIR__ (T_DIR)"
%token <Object> T_CLASS_C 373        //"__CLASS__ (T_CLASS_C)"
%token <Object> T_TRAIT_C 374        //"__TRAIT__ (T_TRAIT_C)"
%token <Object> T_METHOD_C 375       //"__METHOD__ (T_METHOD_C)"
%token <Object> T_FUNC_C 376         //"__FUNCTION__ (T_FUNC_C)"
%token <Object> T_COMMENT 377        //"comment (T_COMMENT)"
%token <Object> T_DOC_COMMENT 378    //"doc comment (T_DOC_COMMENT)"
%token <Object> T_OPEN_TAG 379       //"open tag (T_OPEN_TAG)"
%token <Object> T_OPEN_TAG_WITH_ECHO 380 //"open tag with echo (T_OPEN_TAG_WITH_ECHO)"
%token <Object> T_CLOSE_TAG 381      //"close tag (T_CLOSE_TAG)"
%token <Object> T_WHITESPACE 382     //"whitespace (T_WHITESPACE)"
%token <Object> T_START_HEREDOC 383  //"heredoc start (T_START_HEREDOC)"
%token <Object> T_END_HEREDOC 384    //"heredoc end (T_END_HEREDOC)"
%token <Object> T_DOLLAR_OPEN_CURLY_BRACES 385 //"${ (T_DOLLAR_OPEN_CURLY_BRACES)"
%token <Object> T_CURLY_OPEN 386     //"{$ (T_CURLY_OPEN)"
%token <Object> T_DOUBLE_COLON 387  //":: (T_DOUBLE_COLON)"
%token <Object> T_NAMESPACE 388       //"namespace (T_NAMESPACE)"
%token <Object> T_NS_C 389            //"__NAMESPACE__ (T_NS_C)"
%token <Object> T_NS_SEPARATOR 390    //"\\ (T_NS_SEPARATOR)"
%token <Object> T_ELLIPSIS 391       //"... (T_ELLIPSIS)"
%token <Object> T_COALESCE 282        //"?? (T_COALESCE)"
%token <Object> T_POW 304            //"** (T_POW)"
%token <Object> T_POW_EQUAL 270       //"**= (T_POW_EQUAL)"

/* Token used to force a parse error from the lexer */
%token T_ERROR

%type <Object> top_statement namespace_name name statement function_declaration_statement
%type <Object> class_declaration_statement trait_declaration_statement
%type <Object> interface_declaration_statement interface_extends_list
%type <Object> group_use_declaration inline_use_declarations inline_use_declaration
%type <Object> mixed_group_use_declaration use_declaration unprefixed_use_declaration
%type <Object> unprefixed_use_declarations const_decl inner_statement
%type <Object> expr optional_expr while_statement for_statement foreach_variable
%type <Object> foreach_statement declare_statement finally_statement unset_variable variable
%type <Object> extends_from parameter optional_type argument expr_without_variable global_var
%type <Object> static_var class_statement trait_adaptation trait_precedence trait_alias
%type <Object> absolute_trait_method_reference trait_method_reference property echo_expr
%type <Object> new_expr anonymous_class class_name class_name_reference simple_variable
%type <Object> internal_functions_in_yacc
%type <Object> exit_expr scalar backticks_expr lexical_var function_call member_name property_name
%type <Object> variable_class_name dereferencable_scalar constant dereferencable
%type <Object> callable_expr callable_variable static_member new_variable
%type <Object> encaps_var encaps_var_offset isset_variables
%type <Object> top_statement_list use_declarations const_list inner_statement_list if_stmt
%type <Object> alt_if_stmt for_exprs switch_case_list global_var_list static_var_list
%type <Object> echo_expr_list unset_variables catch_name_list catch_list parameter_list class_statement_list
%type <Object> implements_list case_list if_stmt_without_else
%type <Object> non_empty_parameter_list argument_list non_empty_argument_list property_list
%type <Object> class_const_list class_const_decl name_list trait_adaptations method_body non_empty_for_exprs
%type <Object> ctor_arguments alt_if_stmt_without_else trait_adaptation_list lexical_vars
%type <Object> lexical_var_list encaps_list
%type <Object> array_pair non_empty_array_pair_list array_pair_list possible_array_pair
%type <Object> isset_variable type return_type type_expr
%type <Object> identifier

%type <Long> returns_ref function is_reference is_variadic variable_modifiers
%type <Long> method_modifiers non_empty_member_modifiers member_modifier
%type <Long> class_modifiers class_modifier use_type backup_fn_flags

%type <Object> backup_doc_comment

%type <Object> inline_html                     // Expression

%% /* Rules */

start:
    { SetNamingContext(null); } top_statement_list END
	{ 
		AssignNamingContext(); 
		AssignStatements((List<LangElement>)$2);
		_astRoot = _astFactory.GlobalCode(@$, (List<LangElement>)$2, _namingContext); 
		ResetNamingContext(); 
	}
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
	  reserved_non_modifiers { $$ = $1; }
	| T_STATIC | T_ABSTRACT | T_FINAL | T_PRIVATE | T_PROTECTED | T_PUBLIC
;

identifier:
		T_STRING { $$ = $1; }
	| 	semi_reserved   { $$ = $1; }
;

top_statement_list:
		top_statement_list top_statement { $$ = AddToList<LangElement>($1, $2); }
	|	/* empty */ { $$ = new List<LangElement>(); }
;

namespace_name:
		T_STRING								{ $$ = new List<string>() { (string)$1 }; }
	|	namespace_name T_NS_SEPARATOR T_STRING	{ $$ = AddToList<string>($1, $3); }
;

name:
		namespace_name								{ $$ = new QualifiedName((List<string>)$1, true, false); }
	|	T_NAMESPACE T_NS_SEPARATOR namespace_name	{ $$ = new QualifiedName((List<string>)$3, false, false); }
	|	T_NS_SEPARATOR namespace_name				{ $$ = new QualifiedName((List<string>)$2, true, true); }
;

top_statement:
		statement							{ $$ = $1; }
	|	function_declaration_statement		{ $$ = $1; }
	|	class_declaration_statement			{ $$ = $1; }
	|	trait_declaration_statement			{ $$ = $1; }
	|	interface_declaration_statement		{ $$ = $1; }
	|	T_HALT_COMPILER '(' ')' ';'
			{ $$ = _astFactory.HaltCompiler(@$); }
	|	T_NAMESPACE namespace_name ';'
			{
				AssignNamingContext();
                QualifiedName name = new QualifiedName((List<string>)value_stack.array[value_stack.top - 2].yyval.Object, false, true);
                SetNamingContext(name.NamespacePhpName);
                yyval.Object = _currentNamespace = (NamespaceDecl)_astFactory.Namespace(yypos, name, value_stack.array[value_stack.top-2].yypos, (List<LangElement>)null, _namingContext);
				RESET_DOC_COMMENT(); 
			}
	|	T_NAMESPACE namespace_name { RESET_DOC_COMMENT(); var list = (List<string>)$2; SetNamingContext((list != null && list.Count > 0)? string.Join(QualifiedName.Separator.ToString(), list): null); }
		'{' top_statement_list '}'
			{ 
				$$ = _astFactory.Namespace(@$, new QualifiedName((List<string>)$2, false, true), @2, _astFactory.Block(@5, (List<LangElement>)$5), _namingContext); 
				ResetNamingContext(); 
			}
	|	T_NAMESPACE { RESET_DOC_COMMENT(); SetNamingContext(null); }
		'{' top_statement_list '}'
			{ 
				$$ = _astFactory.Namespace(@$, null, @$, _astFactory.Block(@4, (List<LangElement>)$4), _namingContext); 
				ResetNamingContext(); 
			}
	|	T_USE mixed_group_use_declaration ';'		{ _contextType = ContextType.Class; }
	|	T_USE use_type group_use_declaration ';'	{ _contextType = ContextType.Class; }
	|	T_USE use_declarations ';'					{ _contextType = ContextType.Class; }
	|	T_USE use_type use_declarations ';'			{ _contextType = ContextType.Class; }
	|	T_CONST const_list ';'						{ $$ = _astFactory.DeclList(@$, PhpMemberAttributes.None, (List<LangElement>)$2); }
;

use_type:
	 	T_FUNCTION 		{ $$ = (long)ContextType.Function; _contextType = (ContextType)$$; }
	| 	T_CONST 		{ $$ = (long)ContextType.Constant; _contextType = (ContextType)$$; }
;

group_use_declaration:
		namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations '}'
			{ foreach (var item in (List<Tuple<List<string>, string>>)$4) AddAlias((List<string>)$1, item); }
	|	T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations '}'
			{ foreach (var item in (List<Tuple<List<string>, string>>)$5) AddAlias((List<string>)$2, item); }
;

mixed_group_use_declaration:
		namespace_name T_NS_SEPARATOR '{' inline_use_declarations '}'
			{ foreach (var item in (List<Tuple<List<string>, string, ContextType>>)$4) AddAlias((List<string>)$1, item); }
	|	T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' inline_use_declarations '}'
			{ foreach (var item in (List<Tuple<List<string>, string, ContextType>>)$5) AddAlias((List<string>)$2, item); }
;

inline_use_declarations:
		inline_use_declarations ',' inline_use_declaration
			{ $$ = AddToList<Tuple<List<string>, string, ContextType>>($1, $3); }
	|	inline_use_declaration
			{ $$ = new List<Tuple<List<string>, string, ContextType>>() { (Tuple<List<string>, string, ContextType>)$1 }; }
;

unprefixed_use_declarations:
		unprefixed_use_declarations ',' unprefixed_use_declaration
			{ $$ = AddToList<Tuple<List<string>, string>>($1, $3); }
	|	unprefixed_use_declaration
			{ $$ = new List<Tuple<List<string>, string>>() { (Tuple<List<string>, string>)$1 }; }
;

use_declarations:
		use_declarations ',' use_declaration
			{ AddAlias((Tuple<List<string>, string>)$1); }
	|	use_declaration
			{ AddAlias((Tuple<List<string>, string>)$1); }
;

inline_use_declaration:
		unprefixed_use_declaration { $$ = JoinTuples((Tuple<List<string>, string>)$1, ContextType.Class); }
	|	use_type unprefixed_use_declaration { $$ = JoinTuples((Tuple<List<string>, string>)$2, (ContextType)$1);  }
;

unprefixed_use_declaration:
		namespace_name
			{ $$ = new Tuple<List<string>, string>((List<string>)$1, null); }
	|	namespace_name T_AS T_STRING
			{ $$ = new Tuple<List<string>, string>((List<string>)$1, (string)$3); }
;

use_declaration:
		unprefixed_use_declaration                { $$ = $1; }
	|	T_NS_SEPARATOR unprefixed_use_declaration { $$ = $2; }
;

const_list:
		const_list ',' const_decl { $$ = AddToList<LangElement>($1, $3); }
	|	const_decl { $$ = new List<LangElement>() { (LangElement)$1 }; }
;

inner_statement_list:
		inner_statement_list inner_statement
			{ $$ = AddToList<LangElement>($1, $2); }
	|	/* empty */
			{ $$ = new List<LangElement>(); }
;


inner_statement:
		statement { $$ = $1; }
	|	function_declaration_statement 		{ $$ = $1; }
	|	class_declaration_statement 		{ $$ = $1; }
	|	trait_declaration_statement			{ $$ = $1; }
	|	interface_declaration_statement		{ $$ = $1; }
	|	T_HALT_COMPILER '(' ')' ';'
			{ 
				$$ = null; 
				_astFactory.Error(@$, FatalErrors.InternalError, "__HALT_COMPILER() can only be used from the outermost scope"); 
			}
;


statement:
		'{' inner_statement_list '}' { $$ = _astFactory.Block(@$, (List<LangElement>)$2); }
	|	if_stmt { $$ = $1; }
	|	alt_if_stmt { $$ = $1; }
	|	T_WHILE '(' expr ')' while_statement
			{ $$ = _astFactory.While(@$, (LangElement)$3, (LangElement)$5); }
	|	T_DO statement T_WHILE '(' expr ')' ';'
			{ $$ = _astFactory.Do(@$, (LangElement)$2, (LangElement)$5); }
	|	T_FOR '(' for_exprs ';' for_exprs ';' for_exprs ')' for_statement
			{ $$ = _astFactory.For(@$, (List<LangElement>)$3, (List<LangElement>)$5, (List<LangElement>)$7, (LangElement)$9); }
	|	T_SWITCH '(' expr ')' switch_case_list
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_SWITCH, $3, $5); }
	|	T_BREAK optional_expr ';'		{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Break, (LangElement)$2);}
	|	T_CONTINUE optional_expr ';'	{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Continue, (LangElement)$2); }
	|	T_RETURN optional_expr ';'		{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Return, (LangElement)$2); }
	|	T_GLOBAL global_var_list ';'	{ $$ = $2; }
	|	T_STATIC static_var_list ';'	{ $$ = $2; }
	|	T_ECHO echo_expr_list ';'		{ $$ = _astFactory.Echo(@$, (List<LangElement>)$2); }
	|	T_INLINE_HTML { $$ = _astFactory.InlineHtml(@$, (string)$1); }
	|	expr ';' { $$ = _astFactory.ExpressionStmt(@$, (LangElement)$1); }
	|	T_UNSET '(' unset_variables ')' ';' { $$ = $3; }
	|	T_FOREACH '(' expr T_AS foreach_variable ')' foreach_statement
			{ $$ = _astFactory.Foreach(@$, (LangElement)$3, null, (ForeachVar)$5, (LangElement)$7); }
	|	T_FOREACH '(' expr T_AS foreach_variable T_DOUBLE_ARROW foreach_variable ')'
		foreach_statement
			{ $$ = _astFactory.Foreach(@$, (LangElement)$3, (ForeachVar)$5, (ForeachVar)$7, (LangElement)$9); }
	|	T_DECLARE '(' const_list ')'
			{ zend_handle_encoding_declaration($3); }
		declare_statement
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_DECLARE, $3, $6); }
	|	';'	/* empty statement */ { $$ = null; }
	|	T_TRY '{' inner_statement_list '}' catch_list finally_statement
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_TRY, $3, $5, $6); }
	|	T_THROW expr ';' { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_THROW, $2); }
	|	T_GOTO T_STRING ';' { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_GOTO, $2); }
	|	T_STRING ':' { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_LABEL, $1); }
;

catch_list:
		/* empty */
			{ $$ = zend_ast_create_list(0, _zend_ast_kind.ZEND_AST_CATCH_LIST); }
	|	catch_list T_CATCH '(' catch_name_list T_VARIABLE ')' '{' inner_statement_list '}'
			{ $$ = zend_ast_list_add($1, zend_ast_create(_zend_ast_kind.ZEND_AST_CATCH, $4, $5, $8)); }
;

catch_name_list:
		name { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_NAME_LIST, $1); }
	|	catch_name_list '|' name { $$ = zend_ast_list_add($1, $3); }
;

finally_statement:
		/* empty */ { $$ = null; }
	|	T_FINALLY '{' inner_statement_list '}' { $$ = $3; }
;

unset_variables:
		unset_variable { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_STMT_LIST, $1); }
	|	unset_variables ',' unset_variable { $$ = zend_ast_list_add($1, $3); }
;

unset_variable:
		variable { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_UNSET, $1); }
;

function_declaration_statement:
	function returns_ref T_STRING backup_doc_comment '(' parameter_list ')' return_type
	backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
		{ $$ = zend_ast_create_decl(_zend_ast_kind.ZEND_AST_FUNC_DECL, $2 | $13, $1, $4,
		      zend_ast_get_str($3), $6, null, $11, $8); CG(extra_fn_flags) ;/*= $9;*/ }
;

is_reference:
		/* empty */	{ $$ = 0; }
	|	'&'			{ $$ = (long)_zend_sup.ZEND_PARAM_REF; }
;

is_variadic:
		/* empty */ { $$ = 0; }
	|	T_ELLIPSIS  { $$ = (long)_zend_sup.ZEND_PARAM_VARIADIC; }
;

class_declaration_statement:
		class_modifiers T_CLASS { $<Long>$ = CG(zend_lineno); }
		T_STRING extends_from implements_list backup_doc_comment '{' class_statement_list '}'
			{ $$ = zend_ast_create_decl(_zend_ast_kind.ZEND_AST_CLASS, $1, $<Long>3, $7, zend_ast_get_str($4), $5, $6, $9, null); }
	|	T_CLASS { $<Long>$ = CG(zend_lineno); }
		T_STRING extends_from implements_list backup_doc_comment '{' class_statement_list '}'
			{ $$ = zend_ast_create_decl(_zend_ast_kind.ZEND_AST_CLASS, 0, $<Long>2, $6, zend_ast_get_str($3), $4, $5, $8, null); }
;

class_modifiers:
		class_modifier 					{ $$ = $1; }
	|	class_modifiers class_modifier 	{ $$ = zend_add_class_modifier($1, $2); }
;

class_modifier:
		T_ABSTRACT 		{ $$ = (long)_zend_sup.ZEND_ACC_EXPLICIT_ABSTRACT_CLASS; }
	|	T_FINAL 		{ $$ = (long)_zend_sup.ZEND_ACC_FINAL; }
;

trait_declaration_statement:
		T_TRAIT { $<Long>$ = CG(zend_lineno); }
		T_STRING backup_doc_comment '{' class_statement_list '}'
			{ $$ = zend_ast_create_decl(_zend_ast_kind.ZEND_AST_CLASS, (long)_zend_sup.ZEND_ACC_TRAIT, $<Long>2, $4, zend_ast_get_str($3), null, null, $6, null); }
;

interface_declaration_statement:
		T_INTERFACE { $<Long>$ = CG(zend_lineno); }
		T_STRING interface_extends_list backup_doc_comment '{' class_statement_list '}'
			{ $$ = zend_ast_create_decl(_zend_ast_kind.ZEND_AST_CLASS, (long)_zend_sup.ZEND_ACC_INTERFACE, $<Long>2, $5, zend_ast_get_str($3), null, $4, $7, null); }
;

extends_from:
		/* empty */		{ $$ = null; }
	|	T_EXTENDS name	{ $$ = $2; }
;

interface_extends_list:
		/* empty */			{ $$ = null; }
	|	T_EXTENDS name_list	{ $$ = $2; }
;

implements_list:
		/* empty */				{ $$ = null; }
	|	T_IMPLEMENTS name_list	{ $$ = $2; }
;

foreach_variable:
		variable			{ $$ = $1; }
	|	'&' variable		{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_REF, $2); }
	|	T_LIST '(' array_pair_list ')' { $3/*->attr*/ = 1; $$ = $3; }
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
		/* empty */ { $$ = zend_ast_create_list(0, _zend_ast_kind.ZEND_AST_SWITCH_LIST); }
	|	case_list T_CASE expr case_separator inner_statement_list
			{ $$ = zend_ast_list_add($1, zend_ast_create(_zend_ast_kind.ZEND_AST_SWITCH_CASE, $3, $5)); }
	|	case_list T_DEFAULT case_separator inner_statement_list
			{ $$ = zend_ast_list_add($1, zend_ast_create(_zend_ast_kind.ZEND_AST_SWITCH_CASE, null, $4)); }
;

case_separator:
		':'
	|	';'
;


while_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDWHILE ';' { $$ = _astFactory.Block(@$, (List<LangElement>)$2); }
;


if_stmt_without_else:
		T_IF '(' expr ')' statement
			{ $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_IF,
			      zend_ast_create(_zend_ast_kind.ZEND_AST_IF_ELEM, $3, $5)); }
	|	if_stmt_without_else T_ELSEIF '(' expr ')' statement
			{ $$ = zend_ast_list_add($1,
			      zend_ast_create(_zend_ast_kind.ZEND_AST_IF_ELEM, $4, $6)); }
;

if_stmt:
		if_stmt_without_else %prec T_NOELSE { $$ = $1; }
	|	if_stmt_without_else T_ELSE statement
			{ $$ = zend_ast_list_add($1, zend_ast_create(_zend_ast_kind.ZEND_AST_IF_ELEM, null, $3)); }
;

alt_if_stmt_without_else:
		T_IF '(' expr ')' ':' inner_statement_list
			{ $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_IF,
			      zend_ast_create(_zend_ast_kind.ZEND_AST_IF_ELEM, $3, $6)); }
	|	alt_if_stmt_without_else T_ELSEIF '(' expr ')' ':' inner_statement_list
			{ $$ = zend_ast_list_add($1,
			      zend_ast_create(_zend_ast_kind.ZEND_AST_IF_ELEM, $4, $7)); }
;

alt_if_stmt:
		alt_if_stmt_without_else T_ENDIF ';' { $$ = $1; }
	|	alt_if_stmt_without_else T_ELSE ':' inner_statement_list T_ENDIF ';'
			{ $$ = zend_ast_list_add($1,
			      zend_ast_create(_zend_ast_kind.ZEND_AST_IF_ELEM, null, $4)); }
;

parameter_list:
		non_empty_parameter_list { $$ = $1; }
	|	/* empty */	{ $$ = zend_ast_create_list(0, _zend_ast_kind.ZEND_AST_PARAM_LIST); }
;


non_empty_parameter_list:
		parameter
			{ $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_PARAM_LIST, $1); }
	|	non_empty_parameter_list ',' parameter
			{ $$ = zend_ast_list_add($1, $3); }
;

parameter:
		optional_type is_reference is_variadic T_VARIABLE
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_PARAM, $2 | $3, $1, $4, null); }
	|	optional_type is_reference is_variadic T_VARIABLE '=' expr
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_PARAM, $2 | $3, $1, $4, $6); }
;


optional_type:
		/* empty */	{ $$ = null; }
	|	type_expr	{ $$ = $1; }
;

type_expr:
		type		{ $$ = $1; }
	|	'?' type	{ $$ = $2; /*$$->attr |= _zend_sup.ZEND_TYPE_nullABLE;*/ }
;

type:
		T_ARRAY		{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_TYPE, _zend_sup.IS_ARRAY); }
	|	T_CALLABLE	{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_TYPE, _zend_sup.IS_CALLABLE); }
	|	name		{ $$ = $1; }
;

return_type:
		/* empty */	{ $$ = null; }
	|	':' type_expr	{ $$ = $2; }
;

argument_list:
		'(' ')'	{ $$ = new List<ActualParam>(); }
	|	'(' non_empty_argument_list ')' { $$ = $2; }
;

non_empty_argument_list:
		argument
			{ $$ = new List<ActualParam>() { new ActualParam(@1, (Expression)$1) }; }
	|	non_empty_argument_list ',' argument
			{ $$ = AddToList<ActualParam>($1, new ActualParam(@3, (Expression)$3)); }
;

argument:
		expr			{ $$ = $1; }
	|	T_ELLIPSIS expr	{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_UNPACK, $2); }
;

global_var_list:
		global_var_list ',' global_var { $$ = zend_ast_list_add($1, $3); }
	|	global_var { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_STMT_LIST, $1); }
;

global_var:
	simple_variable
		{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_GLOBAL, zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $1)); }
;


static_var_list:
		static_var_list ',' static_var { $$ = zend_ast_list_add($1, $3); }
	|	static_var { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_STMT_LIST, $1); }
;

static_var:
		T_VARIABLE			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_STATIC, $1, null); }
	|	T_VARIABLE '=' expr	{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_STATIC, $1, $3); }
;


class_statement_list:
		class_statement_list class_statement
			{ $$ = zend_ast_list_add($1, $2); }
	|	/* empty */
			{ $$ = zend_ast_create_list(0, _zend_ast_kind.ZEND_AST_STMT_LIST); }
;


class_statement:
		variable_modifiers property_list ';'
			{ $$ = $2; $$/*->attr*/ = $1; }
	|	method_modifiers T_CONST class_const_list ';'
			{ $$ = $3; $$/*->attr*/ = $1; }
	|	T_USE name_list trait_adaptations
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_USE_TRAIT, $2, $3); }
	|	method_modifiers function returns_ref identifier backup_doc_comment '(' parameter_list ')'
		return_type backup_fn_flags method_body backup_fn_flags
			{ $$ = zend_ast_create_decl(_zend_ast_kind.ZEND_AST_METHOD, $3 | $1 | $12, $2, $5,
				  zend_ast_get_str($4), $7, null, $11, $9); CG(extra_fn_flags) ;/*= $10;*/ }
;

name_list:
		name { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_NAME_LIST, $1); }
	|	name_list ',' name { $$ = zend_ast_list_add($1, $3); }
;

trait_adaptations:
		';'								{ $$ = null; }
	|	'{' '}'							{ $$ = null; }
	|	'{' trait_adaptation_list '}'	{ $$ = $2; }
;

trait_adaptation_list:
		trait_adaptation
			{ $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_TRAIT_ADAPTATIONS, $1); }
	|	trait_adaptation_list trait_adaptation
			{ $$ = zend_ast_list_add($1, $2); }
;

trait_adaptation:
		trait_precedence ';'	{ $$ = $1; }
	|	trait_alias ';'			{ $$ = $1; }
;

trait_precedence:
	absolute_trait_method_reference T_INSTEADOF name_list
		{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_TRAIT_PRECEDENCE, $1, $3); }
;

trait_alias:
		trait_method_reference T_AS T_STRING
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_TRAIT_ALIAS, 0, $1, $3); }
	|	trait_method_reference T_AS reserved_non_modifiers
			{ long zv = 0; zend_lex_tstring(zv); $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_TRAIT_ALIAS, 0, $1, zend_ast_create_zval(zv)); }
	|	trait_method_reference T_AS member_modifier identifier
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_TRAIT_ALIAS, $3, $1, $4); }
	|	trait_method_reference T_AS member_modifier
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_TRAIT_ALIAS, $3, $1, null); }
;

trait_method_reference:
		identifier
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_METHOD_REFERENCE, null, $1); }
	|	absolute_trait_method_reference { $$ = $1; }
;

absolute_trait_method_reference:
	name T_DOUBLE_COLON identifier
		{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_METHOD_REFERENCE, $1, $3); }
;

method_body:
		';' /* abstract method */		{ $$ = null; }
	|	'{' inner_statement_list '}'	{ $$ = $2; }
;

variable_modifiers:
		non_empty_member_modifiers		{ $$ = $1; }
	|	T_VAR							{ $$ = (long)_zend_sup.ZEND_ACC_PUBLIC; }
;

method_modifiers:
		/* empty */						{ $$ = (long)_zend_sup.ZEND_ACC_PUBLIC; }
	|	non_empty_member_modifiers
			{ $$ = $1; /*if (!($$ & (long)_zend_sup.ZEND_ACC_PPP_MASK)) { $$ |= (long)_zend_sup.ZEND_ACC_PUBLIC; } */}
;

non_empty_member_modifiers:
		member_modifier			{ $$ = $1; }
	|	non_empty_member_modifiers member_modifier
			{ $$ = zend_add_member_modifier($1, $2); }
;

member_modifier:
		T_PUBLIC				{ $$ = (long)_zend_sup.ZEND_ACC_PUBLIC; }
	|	T_PROTECTED				{ $$ = (long)_zend_sup.ZEND_ACC_PROTECTED; }
	|	T_PRIVATE				{ $$ = (long)_zend_sup.ZEND_ACC_PRIVATE; }
	|	T_STATIC				{ $$ = (long)_zend_sup.ZEND_ACC_STATIC; }
	|	T_ABSTRACT				{ $$ = (long)_zend_sup.ZEND_ACC_ABSTRACT; }
	|	T_FINAL					{ $$ = (long)_zend_sup.ZEND_ACC_FINAL; }
;

property_list:
		property_list ',' property { $$ = zend_ast_list_add($1, $3); }
	|	property { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_PROP_DECL, $1); }
;

property:
		T_VARIABLE backup_doc_comment
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_PROP_ELEM, $1, null, ((bool)$2 ? zend_ast_create_zval_from_str($2) : null)); }
	|	T_VARIABLE '=' expr backup_doc_comment
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_PROP_ELEM, $1, $3, ((bool)$4 ? zend_ast_create_zval_from_str($4) : null)); }
;

class_const_list:
		class_const_list ',' class_const_decl { $$ = zend_ast_list_add($1, $3); }
	|	class_const_decl { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_CLASS_CONST_DECL, $1); }
;

class_const_decl:
	identifier '=' expr backup_doc_comment { $$ = _astFactory.ClassConstDecl(@$, new VariableName((string)$1), (LangElement)$3); }
;

const_decl:
	T_STRING '=' expr backup_doc_comment { $$ = _astFactory.GlobalConstDecl(@$, false, new VariableName((string)$1), (LangElement)$3); }
;

echo_expr_list:
		echo_expr_list ',' echo_expr { $$ = AddToList<LangElement>($1, $3); }
	|	echo_expr { $$ = new List<LangElement>() { (LangElement)$1 }; }
;
echo_expr:
	expr { $$ = $1; }
;

for_exprs:
		/* empty */			{ $$ = null; }
	|	non_empty_for_exprs	{ $$ = $1; }
;

non_empty_for_exprs:
		non_empty_for_exprs ',' expr { $$ = zend_ast_list_add($1, $3); }
	|	expr { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_EXPR_LIST, $1); }
;

anonymous_class:
        T_CLASS { $<Long>$ = CG(zend_lineno); } ctor_arguments
		extends_from implements_list backup_doc_comment '{' class_statement_list '}' {
			object decl = zend_ast_create_decl(
				_zend_ast_kind.ZEND_AST_CLASS, (long)_zend_sup.ZEND_ACC_ANON_CLASS, $<Long>2, $6, null,
				$4, $5, $8, null);
			$$ = zend_ast_create(_zend_ast_kind.ZEND_AST_NEW, decl, $3);
		}
;

new_expr:
		T_NEW class_name_reference ctor_arguments
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_NEW, $2, $3); }
	|	T_NEW anonymous_class
			{ $$ = $2; }
;

expr_without_variable:
		T_LIST '(' array_pair_list ')' '=' expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignValue); }
	|	'[' array_pair_list ']' '=' expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$2, (LangElement)$5, Operations.AssignValue); }
	|	variable '=' expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignValue); }
	|	variable '=' '&' variable
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$4, Operations.AssignRef); }
	|	T_CLONE expr { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_CLONE, $2); }
	|	variable T_PLUS_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignAdd); }
	|	variable T_MINUS_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignSub); }
	|	variable T_MUL_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignMul); }
	|	variable T_POW_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignPow); }
	|	variable T_DIV_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignDiv); }
	|	variable T_CONCAT_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignAppend); }
	|	variable T_MOD_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignMod); }
	|	variable T_AND_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignAnd); }
	|	variable T_OR_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignOr); }
	|	variable T_XOR_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignXor); }
	|	variable T_SL_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignShiftLeft); }
	|	variable T_SR_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignShiftRight); }
	|	variable T_INC { $$ = _astFactory.IncrementDecrement(@$, (Expression)$1, true,  true); }
	|	T_INC variable { $$ = _astFactory.IncrementDecrement(@$, (Expression)$2, true,  false); }
	|	variable T_DEC { $$ = _astFactory.IncrementDecrement(@$, (Expression)$1, false, true); }
	|	T_DEC variable { $$ = _astFactory.IncrementDecrement(@$, (Expression)$2, false, false); }
	|	expr T_BOOLEAN_OR expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.Or,   (LangElement)$1, (LangElement)$3); }
	|	expr T_BOOLEAN_AND expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.And,  (LangElement)$1, (LangElement)$3); }
	|	expr T_LOGICAL_OR expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.Or,   (LangElement)$1, (LangElement)$3); }
	|	expr T_LOGICAL_AND expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.And,  (LangElement)$1, (LangElement)$3); }
	|	expr T_LOGICAL_XOR expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.Xor,  (LangElement)$1, (LangElement)$3); }
	|	expr '|' expr	{ $$ = _astFactory.BinaryOperation(@$, Operations.BitOr,  (LangElement)$1, (LangElement)$3); }
	|	expr '&' expr	{ $$ = _astFactory.BinaryOperation(@$, Operations.BitAnd, (LangElement)$1, (LangElement)$3); }
	|	expr '^' expr	{ $$ = _astFactory.BinaryOperation(@$, Operations.BitXor, (LangElement)$1, (LangElement)$3); }
	|	expr '.' expr 	{ $$ = _astFactory.BinaryOperation(@$, Operations.Concat, (LangElement)$1, (LangElement)$3); }
	|	expr '+' expr 	{ $$ = _astFactory.BinaryOperation(@$, Operations.Add,    (LangElement)$1, (LangElement)$3); }
	|	expr '-' expr 	{ $$ = _astFactory.BinaryOperation(@$, Operations.Sub,    (LangElement)$1, (LangElement)$3); }
	|	expr '*' expr	{ $$ = _astFactory.BinaryOperation(@$, Operations.Mul,    (LangElement)$1, (LangElement)$3); }
	|	expr T_POW expr	{ $$ = _astFactory.BinaryOperation(@$, Operations.Pow,    (LangElement)$1, (LangElement)$3); }
	|	expr '/' expr	{ $$ = _astFactory.BinaryOperation(@$, Operations.Div,    (LangElement)$1, (LangElement)$3); }
	|	expr '%' expr 	{ $$ = _astFactory.BinaryOperation(@$, Operations.Mod,    (LangElement)$1, (LangElement)$3); }
	| 	expr T_SL expr	{ $$ = _astFactory.BinaryOperation(@$, Operations.ShiftLeft,  (LangElement)$1, (LangElement)$3); } 
	|	expr T_SR expr	{ $$ = _astFactory.BinaryOperation(@$, Operations.ShiftRight, (LangElement)$1, (LangElement)$3); } 
	|	'+' expr %prec T_INC { $$ = _astFactory.UnaryOperation(@$, Operations.Plus,   (Expression)$2); }
	|	'-' expr %prec T_INC { $$ = _astFactory.UnaryOperation(@$, Operations.Plus,   (Expression)$2); }
	|	'!' expr { $$ = _astFactory.UnaryOperation(@$, Operations.LogicNegation, (Expression)$2); }
	|	'~' expr { $$ = _astFactory.UnaryOperation(@$, Operations.BitNegation,   (Expression)$2); }
	|	expr T_IS_IDENTICAL expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.ShiftRight, (LangElement)$1, (LangElement)$3); }
	|	expr T_IS_NOT_IDENTICAL expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.NotIdentical, (LangElement)$1, (LangElement)$3); }
	|	expr T_IS_EQUAL expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.Equal, (LangElement)$1, (LangElement)$3); }
	|	expr T_IS_NOT_EQUAL expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.NotEqual, (LangElement)$1, (LangElement)$3); }
	|	expr '<' expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.LessThan, (LangElement)$1, (LangElement)$3); }
	|	expr T_IS_SMALLER_OR_EQUAL expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.LessThanOrEqual, (LangElement)$1, (LangElement)$3); }
	|	expr '>' expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.GreaterThan, (LangElement)$1, (LangElement)$3); }
	|	expr T_IS_GREATER_OR_EQUAL expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.GreaterThanOrEqual, (LangElement)$1, (LangElement)$3); }
	|	expr T_SPACESHIP expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.Spaceship, (LangElement)$1, (LangElement)$3); }
	|	expr T_INSTANCEOF class_name_reference
			{ $$ = _astFactory.BinaryOperation(@$, Operations.InstanceOf, (LangElement)$1, (LangElement)$3); }
	|	'(' expr ')' { $$ = $2; }
	|	new_expr { $$ = $1; }
	|	expr '?' expr ':' expr
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_CONDITIONAL, $1, $3, $5); }
	|	expr '?' ':' expr
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_CONDITIONAL, $1, null, $4); }
	|	expr T_COALESCE expr
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_COALESCE, $1, $3); }
	|	internal_functions_in_yacc { $$ = $1; }
	|	T_INT_CAST expr		{ $$ = _astFactory.UnaryOperation(@$, Operations.LongCast,   (Expression)$2); }
	|	T_DOUBLE_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.DoubleCast, (Expression)$2); }
	|	T_STRING_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.StringCast, (Expression)$2); }
	|	T_ARRAY_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.ArrayCast,  (Expression)$2); } 
	|	T_OBJECT_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.ObjectCast, (Expression)$2); }
	|	T_BOOL_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.BoolCast,   (Expression)$2); }
	|	T_UNSET_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.UnsetCast,  (Expression)$2); }
	|	T_EXIT exit_expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.Exit,       (Expression)$2); }
	|	'@' expr			{ $$ = _astFactory.UnaryOperation(@$, Operations.AtSign,     (Expression)$2); }
	|	scalar { $$ = $1; }
	|	'`' backticks_expr '`' { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_SHELL_EXEC, $2); }
	|	T_PRINT expr { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_PRINT, $2); }
	|	T_YIELD { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_YIELD, null, null); /*CG(extra_fn_flags) |= (long)_zend_sup.ZEND_ACC_GENERATOR;*/ }
	|	T_YIELD expr { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_YIELD, $2, null); /*CG(extra_fn_flags) |= (long)_zend_sup.ZEND_ACC_GENERATOR;*/ }
	|	T_YIELD expr T_DOUBLE_ARROW expr { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_YIELD, $4, $2); /*CG(extra_fn_flags) |= (long)_zend_sup.ZEND_ACC_GENERATOR;*/ }
	|	T_YIELD_FROM expr { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_YIELD_FROM, $2); /*CG(extra_fn_flags) |= (long)_zend_sup.ZEND_ACC_GENERATOR;*/ }
	|	function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars return_type
		backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
			{ $$ = zend_ast_create_decl(_zend_ast_kind.ZEND_AST_CLOSURE, $2 | $13, $1, $3,
				  zend_string_init("{closure}", /*sizeof("{closure}") - */1, 0),
			      $5, $7, $11, $8); CG(extra_fn_flags) ;/*= $9;*/ }
	|	T_STATIC function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars
		return_type backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
			{ $$ = zend_ast_create_decl(_zend_ast_kind.ZEND_AST_CLOSURE, $3 | $14 | (long)_zend_sup.ZEND_ACC_STATIC, $2, $4,
			      zend_string_init("{closure}", /*sizeof("{closure}") - */1, 0),
			      $6, $8, $12, $9); CG(extra_fn_flags) ;/*= $10;*/ }
;

function:
	T_FUNCTION { $$ = CG(zend_lineno); }
;

backup_doc_comment:
	/* empty */ { $$ = CG(doc_comment); /*CG(doc_comment) = null;*/ }
;

backup_fn_flags:
	/* empty */ { $$ = CG(extra_fn_flags); CG(extra_fn_flags) ;/*= 0;*/ }
;

returns_ref:
		/* empty */	{ $$ = 0; }
	|	'&'			{ $$ = (long)_zend_sup.ZEND_ACC_RETURN_REFERENCE; }
;

lexical_vars:
		/* empty */ { $$ = null; }
	|	T_USE '(' lexical_var_list ')' { $$ = $3; }
;

lexical_var_list:
		lexical_var_list ',' lexical_var { $$ = zend_ast_list_add($1, $3); }
	|	lexical_var { $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_CLOSURE_USES, $1); }
;

lexical_var:
		T_VARIABLE		{ $$ = $1; }
	|	'&' T_VARIABLE	{ $$ = $2; $$/*->attr*/ = 1; }
;

function_call:
		name argument_list
			{ $$ = _astFactory.Call(@$, (QualifiedName)$1, null, @1, new CallSignature((List<ActualParam>)$2), null); }
	|	class_name T_DOUBLE_COLON member_name argument_list
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_STATIC_CALL, $1, $3, $4); }
	|	variable_class_name T_DOUBLE_COLON member_name argument_list
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_STATIC_CALL, $1, $3, $4); }
	|	callable_expr argument_list
			{ $$ = _astFactory.Call(@$, new QualifiedName(), null, @1, new CallSignature((List<ActualParam>)$2), (LangElement)$1); }
;

class_name:
		T_STATIC
			{/* long zv = 0; ZVAL_INTERNED_STR(zv, CG(known_strings)[ZEND_STR_STATIC]);
			  $$ = zend_ast_create_zval_ex(zv, ZEND_NAME_NOT_FQ); */}
	|	name { $$ = $1; }
;

class_name_reference:
		class_name		{ $$ = $1; }
	|	new_variable	{ $$ = $1; }
;

exit_expr:
		/* empty */				{ $$ = null; }
	|	'(' optional_expr ')'	{ $$ = $2; }
;

backticks_expr:
		/* empty */
			{ $$ = zend_ast_create_zval_from_str(ZSTR_EMPTY_ALLOC()); }
	|	T_ENCAPSED_AND_WHITESPACE { $$ = $1; }
	|	encaps_list { $$ = $1; }
;


ctor_arguments:
		/* empty */	{ $$ = zend_ast_create_list(0, _zend_ast_kind.ZEND_AST_ARG_LIST); }
	|	argument_list { $$ = $1; }
;


dereferencable_scalar:
		T_ARRAY '(' array_pair_list ')'	{ $$ = $3; }
	|	'[' array_pair_list ']'			{ $$ = $2; }
	|	T_CONSTANT_ENCAPSED_STRING		{ $$ = _astFactory.Literal(@$, $1); }
;

scalar:
		T_LNUMBER 	{ $$ = _astFactory.Literal(@$, $1); }
	|	T_DNUMBER 	{ $$ = _astFactory.Literal(@$, $1); }
	|	T_LINE 		{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Line); }
	|	T_FILE 		{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.File); }     
	|	T_DIR   	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Dir); }      
	|	T_TRAIT_C	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Trait); }    
	|	T_METHOD_C	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Method); }   
	|	T_FUNC_C	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Function); } 
	|	T_NS_C		{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Namespace); }
	|	T_CLASS_C	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Class); }    
	|	T_START_HEREDOC T_ENCAPSED_AND_WHITESPACE T_END_HEREDOC { $$ = _astFactory.Literal(@$, $2); }
	|	T_START_HEREDOC T_END_HEREDOC
			{ $$ = string.Empty; }
	|	'"' encaps_list '"' 	{ $$ = _astFactory.Concat(@$, (List<LangElement>)$2); }
	|	T_START_HEREDOC encaps_list T_END_HEREDOC { $$ = _astFactory.Concat(@$, (List<LangElement>)$2); }
	|	dereferencable_scalar	{ $$ = $1; }
	|	constant			{ $$ = $1; }
;

constant:
		name { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_CONST, $1); }
	|	class_name T_DOUBLE_COLON identifier
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_CLASS_CONST, $1, $3); }
	|	variable_class_name T_DOUBLE_COLON identifier
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_CLASS_CONST, $1, $3); }
;

expr:
		variable					{ $$ = $1; }
	|	expr_without_variable		{ $$ = $1; }
;

optional_expr:
		/* empty */	{ $$ = null; }
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
			{ $$ = $1; }
	|	dereferencable '[' optional_expr ']'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_DIM, $1, $3); }
	|	constant '[' optional_expr ']'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_DIM, $1, $3); }
	|	dereferencable '{' expr '}'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_DIM, $1, $3); }
	|	dereferencable T_OBJECT_OPERATOR property_name argument_list
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_METHOD_CALL, $1, $3, $4); }
	|	function_call { $$ = $1; }
;

variable:
		callable_variable
			{ $$ = $1; }
	|	static_member
			{ $$ = $1; }
	|	dereferencable T_OBJECT_OPERATOR property_name
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_PROP, $1, $3); }
;

simple_variable:
		T_VARIABLE			{ $$ = _astFactory.Variable(@$, new VariableName((string)$1), (LangElement)null); }
	|	'$' '{' expr '}'	{ $$ = _astFactory.Variable(@$, (LangElement)$3, (LangElement)null); }
	|	'$' simple_variable	{ $$ = $2; }
;

static_member:
		class_name T_DOUBLE_COLON simple_variable
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_STATIC_PROP, $1, $3); }
	|	variable_class_name T_DOUBLE_COLON simple_variable
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_STATIC_PROP, $1, $3); }
;

new_variable:
		simple_variable
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $1); }
	|	new_variable '[' optional_expr ']'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_DIM, $1, $3); }
	|	new_variable '{' expr '}'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_DIM, $1, $3); }
	|	new_variable T_OBJECT_OPERATOR property_name
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_PROP, $1, $3); }
	|	class_name T_DOUBLE_COLON simple_variable
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_STATIC_PROP, $1, $3); }
	|	new_variable T_DOUBLE_COLON simple_variable
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_STATIC_PROP, $1, $3); }
;

member_name:
		identifier { $$ = $1; }
	|	'{' expr '}'	{ $$ = $2; }
	|	simple_variable	{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $1); }
;

property_name:
		T_STRING { $$ = $1; }
	|	'{' expr '}'	{ $$ = $2; }
	|	simple_variable	{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $1); }
;

array_pair_list:
		non_empty_array_pair_list
			{ /* allow single trailing comma */ $$ = zend_ast_list_rtrim($1); }
;

possible_array_pair:
		/* empty */ { $$ = null; }
	|	array_pair  { $$ = $1; }
;

non_empty_array_pair_list:
		non_empty_array_pair_list ',' possible_array_pair
			{ $$ = zend_ast_list_add($1, $3); }
	|	possible_array_pair
			{ $$ = zend_ast_create_list(1, _zend_ast_kind.ZEND_AST_ARRAY, $1); }
;

array_pair:
		expr T_DOUBLE_ARROW expr
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_ARRAY_ELEM, $3, $1); }
	|	expr
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_ARRAY_ELEM, $1, null); }
	|	expr T_DOUBLE_ARROW '&' variable
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_ARRAY_ELEM, 1, $4, $1); }
	|	'&' variable
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_ARRAY_ELEM, 1, $2, null); }
	|	expr T_DOUBLE_ARROW T_LIST '(' array_pair_list ')'
			{ $5/*->attr*/ = 1; $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_ARRAY_ELEM, $5, $1); }
	|	T_LIST '(' array_pair_list ')'
			{ $3/*->attr*/ = 1; $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_ARRAY_ELEM, $3, null); }
;

encaps_list:
		encaps_list encaps_var
			{ $$ = AddToList<LangElement>($1, $2); }
	|	encaps_list T_ENCAPSED_AND_WHITESPACE
			{ $$ = AddToList<LangElement>($1, $2); }
	|	encaps_var
			{ $$ = new List<LangElement>() { _astFactory.Variable(@$, new VariableName((string)$1), (LangElement)null) }; }
	|	T_ENCAPSED_AND_WHITESPACE encaps_var
			{ $$ = new List<LangElement>() { _astFactory.Literal(@1, $1), _astFactory.Variable(@2, new VariableName((string)$2), (LangElement)null) }; }
;

encaps_var:
		T_VARIABLE
			{ _astFactory.Variable(@$, new VariableName((string)$1), (LangElement)null); }
	|	T_VARIABLE '[' encaps_var_offset ']'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_DIM,
			      zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $1), $3); }
	|	T_VARIABLE T_OBJECT_OPERATOR T_STRING
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_PROP,
			      zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $1), $3); }
	|	T_DOLLAR_OPEN_CURLY_BRACES expr '}'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $2); }
	|	T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '}'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $2); }
	|	T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '[' expr ']' '}'
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_DIM,
			      zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $2), $4); }
	|	T_CURLY_OPEN variable '}' { $$ = $2; }
;

encaps_var_offset:
		T_STRING		{ $$ = $1; }
	|	T_NUM_STRING	{ $$ = $1; }
	|	T_VARIABLE		{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_VAR, $1); }
;


internal_functions_in_yacc:
		T_ISSET '(' isset_variables ')' { $$ = $3; }
	|	T_EMPTY '(' expr ')' { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_EMPTY, $3); }
	|	T_INCLUDE expr
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_INCLUDE_OR_EVAL, _zend_sup.ZEND_INCLUDE, $2); }
	|	T_INCLUDE_ONCE expr
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_INCLUDE_OR_EVAL, _zend_sup.ZEND_INCLUDE_ONCE, $2); }
	|	T_EVAL '(' expr ')'
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_INCLUDE_OR_EVAL, _zend_sup.ZEND_EVAL, $3); }
	|	T_REQUIRE expr
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_INCLUDE_OR_EVAL, _zend_sup.ZEND_REQUIRE, $2); }
	|	T_REQUIRE_ONCE expr
			{ $$ = zend_ast_create_ex(_zend_ast_kind.ZEND_AST_INCLUDE_OR_EVAL, _zend_sup.ZEND_REQUIRE_ONCE, $2); }
;

isset_variables:
		isset_variable { $$ = $1; }
	|	isset_variables ',' isset_variable
			{ $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_AND, $1, $3); }
;

isset_variable:
		expr { $$ = zend_ast_create(_zend_ast_kind.ZEND_AST_ISSET, $1); }
;

%%
