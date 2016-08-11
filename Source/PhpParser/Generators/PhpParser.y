/*

Copyright (c) 2016 Michal Brabec

Parser was generated using The Gardens Point Parser Generator (GPPG) using PHP language grammar based on Flex and Bison files
distributed with PHP7 interpreter.

*/

using System.Linq;
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
    { SetNamingContext(null); } top_statement_list
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
	|	function_declaration_statement		{ $$ = $1; ((FunctionDecl)$$).IsConditional = false; }
	|	class_declaration_statement			{ $$ = $1; ((TypeDecl)$$).IsConditional = false; }
	|	trait_declaration_statement			{ $$ = $1; ((TypeDecl)$$).IsConditional = false; }
	|	interface_declaration_statement		{ $$ = $1; ((TypeDecl)$$).IsConditional = false; }
	|	T_HALT_COMPILER '(' ')' ';'
			{ $$ = _astFactory.HaltCompiler(@$); }
	|	T_NAMESPACE namespace_name ';'
			{
				AssignNamingContext();
                QualifiedName name = new QualifiedName((List<string>)$2, false, true);
                SetNamingContext(name.NamespacePhpName);
                $$ = _currentNamespace = (NamespaceDecl)_astFactory.Namespace(yypos, name, @2, (List<LangElement>)null, _namingContext);
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
		statement								{ $$ = $1; }
	|	function_declaration_statement 			{ $$ = $1; }
	|	class_declaration_statement 			{ $$ = $1; }
	|	trait_declaration_statement				{ $$ = $1; }
	|	interface_declaration_statement			{ $$ = $1; }
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
			{ $$ = _astFactory.For(@$, (List<LangElement>)$3, (List<LangElement>)$5, (List<LangElement>)$7, StatementBlock(@9, $9)); }
	|	T_SWITCH '(' expr ')' switch_case_list
			{ $$ = _astFactory.Switch(@$, (LangElement)$3, (List<LangElement>)$5); }
	|	T_BREAK optional_expr ';'		{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Break, (LangElement)$2);}
	|	T_CONTINUE optional_expr ';'	{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Continue, (LangElement)$2); }
	|	T_RETURN optional_expr ';'		{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Return, (LangElement)$2); }
	|	T_GLOBAL global_var_list ';'	{ $$ = _astFactory.Global(@$, (List<LangElement>)$2); }
	|	T_STATIC static_var_list ';'	{ $$ = _astFactory.Static(@$, (List<LangElement>)$2); }
	|	T_ECHO echo_expr_list ';'		{ $$ = _astFactory.Echo(@$, (List<LangElement>)$2); }
	|	T_INLINE_HTML { $$ = _astFactory.InlineHtml(@$, (string)$1); }
	|	expr ';' { $$ = _astFactory.ExpressionStmt(@$, (LangElement)$1); }
	|	T_UNSET '(' unset_variables ')' ';' { $$ = _astFactory.Unset(@$, (List<LangElement>)$3); }
	|	T_FOREACH '(' expr T_AS foreach_variable ')' foreach_statement
			{ $$ = _astFactory.Foreach(@$, (LangElement)$3, null, (VariableUse)$5, StatementBlock(@7, $7)); }
	|	T_FOREACH '(' expr T_AS foreach_variable T_DOUBLE_ARROW foreach_variable ')'
		foreach_statement
			{ $$ = _astFactory.Foreach(@$, (LangElement)$3, (VariableUse)$5, (VariableUse)$7, StatementBlock(@9, $9)); }
	|	T_DECLARE '(' const_list ')' declare_statement
			{ $$ = _astFactory.Declare(@$, (LangElement)$5); }
	|	';'	/* empty statement */ { $$ = null; }
	|	T_TRY '{' inner_statement_list '}' catch_list finally_statement
			{ $$ = _astFactory.TryCatch(@$, _astFactory.Block(@3, (List<LangElement>)$3), (List<CatchItem>)$5, (LangElement)$6); }
	|	T_THROW expr ';' { $$ = _astFactory.Throw(@$, (LangElement)$2); }
	|	T_GOTO T_STRING ';' { $$ = _astFactory.Goto(@$, (string)$2, @2); }
	|	T_STRING ':' { $$ = _astFactory.Label(@$, (string)$1, @1); }
;

catch_list:
		/* empty */
			{ $$ = new List<CatchItem>(); }
	|	catch_list T_CATCH '(' catch_name_list T_VARIABLE ')' '{' inner_statement_list '}'
			{ 
				$$ = AddToList<CatchItem>($1, _astFactory.Catch(@$, 
					(TypeRef)_astFactory.TypeReference(@4, (List<QualifiedName>)$4, null), 
					(DirectVarUse)_astFactory.Variable(@5, new VariableName((string)$5), (LangElement)null), 
					_astFactory.Block(@8, (List<LangElement>)$8))); 
			}
;

catch_name_list:
		name { $$ = new List<QualifiedName>() { (QualifiedName)$1 }; }
	|	catch_name_list '|' name { $$ = AddToList<QualifiedName>($1, $3); }
;

finally_statement:
		/* empty */ { $$ = null; }
	|	T_FINALLY '{' inner_statement_list '}' { $$ =_astFactory.Finally(@$, _astFactory.Block(@3, (List<LangElement>)$3)); }
;

unset_variables:
		unset_variable { $$ = new List<LangElement>() { (LangElement)$1 }; }
	|	unset_variables ',' unset_variable { $$ = AddToList<LangElement>($1, $3); }
;

unset_variable:
		variable { $$ = $1; }
;

function_declaration_statement:
	function returns_ref T_STRING backup_doc_comment '(' parameter_list ')' return_type
	backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
		{ $$ = _astFactory.Function(@$, true, false, PhpMemberAttributes.None, (TypeRef)$8, @8, 
			new Name((string)$3), @3, null, (List<FormalParam>)$6, @7, 
			_astFactory.Block(@11, (List<LangElement>)$11)); 
		if($4 != null)
			((FunctionDecl)$$).PHPDoc = new PHPDocBlock((string)$4, @4);
		}
;

is_reference:
		/* empty */	{ $$ = 0; }
	|	'&'			{ $$ = (long)FormalParam.Flags.IsByRef; }
;

is_variadic:
		/* empty */ { $$ = 0; }
	|	T_ELLIPSIS  { $$ = (long)FormalParam.Flags.IsVariadic; }
;

class_declaration_statement:
		class_modifiers T_CLASS T_STRING extends_from implements_list backup_doc_comment '{' class_statement_list '}'
			{ 
				$$ = _astFactory.Type(@$, true, (PhpMemberAttributes)$1, new Name((string)$3), @3, null, 
				(Tuple<GenericQualifiedName, Span>)$4, (List<Tuple<GenericQualifiedName, Span>>)$5, (List<LangElement>)$8, @8); 
				if($6 != null) ((TypeDecl)$$).PHPDoc = new PHPDocBlock((string)$6, @6);
			}
	|	T_CLASS T_STRING extends_from implements_list backup_doc_comment '{' class_statement_list '}'
			{ 
				$$ = _astFactory.Type(@$, true, PhpMemberAttributes.None, new Name((string)$2), @2, null, 
				(Tuple<GenericQualifiedName, Span>)$3, (List<Tuple<GenericQualifiedName, Span>>)$4, (List<LangElement>)$7, @7); 
				if($5 != null) ((TypeDecl)$$).PHPDoc = new PHPDocBlock((string)$5, @5);
			}
;

class_modifiers:
		class_modifier 					{ $$ = $1; }
	|	class_modifiers class_modifier 	{ $$ = $1 | $2; }
;

class_modifier:
		T_ABSTRACT 		{ $$ = (long)PhpMemberAttributes.Abstract; }
	|	T_FINAL 		{ $$ = (long)PhpMemberAttributes.Final; }
;

trait_declaration_statement:
		T_TRAIT T_STRING backup_doc_comment '{' class_statement_list '}'
			{ 
				$$ = _astFactory.Type(@$, true, PhpMemberAttributes.Trait, new Name((string)$2), @2, null, 
				null, null, (List<LangElement>)$5, @5); 
				if($3 != null) ((TypeDecl)$$).PHPDoc = new PHPDocBlock((string)$3, @3);
			}
;

interface_declaration_statement:
		T_INTERFACE T_STRING interface_extends_list backup_doc_comment '{' class_statement_list '}'
			{ 
				$$ = _astFactory.Type(@$, true, PhpMemberAttributes.Interface, new Name((string)$2), @2, null, 
				null, (List<Tuple<GenericQualifiedName, Span>>)$3, (List<LangElement>)$6, @6); 
				if($4 != null) ((TypeDecl)$$).PHPDoc = new PHPDocBlock((string)$4, @4);
			}
;

extends_from:
		/* empty */		{ $$ = null; }
	|	T_EXTENDS name	{ $$ = new Tuple<GenericQualifiedName, Span>(new GenericQualifiedName((QualifiedName)$2, new object[0]), @2); }
;

interface_extends_list:
		/* empty */			{ $$ = null; }
	|	T_EXTENDS name_list { $$ = NameListToImplementsList(@2, (List<QualifiedName>)$2); }
;

implements_list:
		/* empty */				{ $$ = null; }
	|	T_IMPLEMENTS name_list	{ $$ = NameListToImplementsList(@2, (List<QualifiedName>)$2); }
;

foreach_variable:
		variable			{ $$ = $1; }
	|	'&' variable		{ $$ = _astFactory.Variable(@$, (LangElement)$2, (LangElement)null); }
	|	T_LIST '(' array_pair_list ')' { $$ = _astFactory.List(@3, (List<Item>)$3); }
	|	'[' array_pair_list ']' { $$ = _astFactory.NewArray(@2, (List<Item>)$2); }
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
	|	':' inner_statement_list T_ENDDECLARE ';' { $$ = StatementBlock(@2, $2); }
;

switch_case_list:
		'{' case_list '}'					{ $$ = $2; }
	|	'{' ';' case_list '}'				{ $$ = $3; }
	|	':' case_list T_ENDSWITCH ';'		{ $$ = $2; }
	|	':' ';' case_list T_ENDSWITCH ';'	{ $$ = $3; }
;

case_list:
		/* empty */ { $$ = new List<LangElement>(); }
	|	case_list T_CASE expr case_separator inner_statement_list
			{ $$ = AddToList<LangElement>($1, _astFactory.Case(@$, (LangElement)$3, StatementBlock(@5, $5))); }
	|	case_list T_DEFAULT case_separator inner_statement_list
			{ $$ = AddToList<LangElement>($1, _astFactory.Case(@$, null, StatementBlock(@4, $4))); }
;

case_separator:
		':'
	|	';'
;


while_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDWHILE ';' { $$ = StatementsToBlock(@2, $2); }
;


if_stmt_without_else:
		T_IF '(' expr ')' statement
			{ $$ = new List<Tuple<LangElement, LangElement>>() { 
				new Tuple<LangElement, LangElement>((LangElement)$3, (LangElement)$5) }; 
			}
	|	if_stmt_without_else T_ELSEIF '(' expr ')' statement
			{ $$ = AddToList<LangElement>($1, 
				new Tuple<LangElement, LangElement>((LangElement)$4, (LangElement)$6)); 
			}
;

if_stmt:
		if_stmt_without_else %prec T_NOELSE 
			{ ((List<Tuple<LangElement, LangElement>>)$1).Reverse(); $$ = null; 
			foreach (var item in (List<Tuple<LangElement, LangElement>>)$1) $$ = _astFactory.If(@$, item.Item1, item.Item2, (LangElement)$$); }
	|	if_stmt_without_else T_ELSE statement
			{ ((List<Tuple<LangElement, LangElement>>)$1).Reverse(); $$ = _astFactory.If(@$, null, (LangElement)$3, null); 
			foreach (var item in (List<Tuple<LangElement, LangElement>>)$1) $$ = _astFactory.If(@$, item.Item1, item.Item2, (LangElement)$$); }
;

alt_if_stmt_without_else:
		T_IF '(' expr ')' ':' inner_statement_list
			{ $$ = new List<Tuple<LangElement, LangElement>>() { 
				new Tuple<LangElement, LangElement>((LangElement)$3, StatementsToBlock(@6, $6)) }; 
			}
	|	alt_if_stmt_without_else T_ELSEIF '(' expr ')' ':' inner_statement_list
			{ $$ = AddToList<LangElement>($1, 
				new Tuple<LangElement, LangElement>((LangElement)$4, StatementsToBlock(@7, $7))); 
			}
;

alt_if_stmt:
		alt_if_stmt_without_else T_ENDIF ';' 
			{ ((List<Tuple<LangElement, LangElement>>)$1).Reverse(); $$ = null; 
			foreach (var item in (List<Tuple<LangElement, LangElement>>)$1) $$ = _astFactory.If(@$, item.Item1, item.Item2, (LangElement)$$); }
	|	alt_if_stmt_without_else T_ELSE ':' inner_statement_list T_ENDIF ';'
			{ ((List<Tuple<LangElement, LangElement>>)$1).Reverse(); $$ = _astFactory.If(@$, null, StatementsToBlock(@4, $4), null); 
			foreach (var item in (List<Tuple<LangElement, LangElement>>)$1) $$ = _astFactory.If(@$, item.Item1, item.Item2, (LangElement)$$); }
;

parameter_list:
		non_empty_parameter_list { $$ = $1; }
	|	/* empty */	{ $$ = new List<FormalParam>(); }
;


non_empty_parameter_list:
		parameter
			{ $$ = new List<FormalParam>() { (FormalParam)$1 }; }
	|	non_empty_parameter_list ',' parameter
			{ $$ = AddToList<FormalParam>($1, $3); }
;

parameter:
		optional_type is_reference is_variadic T_VARIABLE
			{ $$ = _astFactory.Parameter(@$, (string)$4, (TypeRef)$1, (FormalParam.Flags)$2|(FormalParam.Flags)$3, null); }
	|	optional_type is_reference is_variadic T_VARIABLE '=' expr
			{ $$ = _astFactory.Parameter(@$, (string)$4, (TypeRef)$1, (FormalParam.Flags)$2|(FormalParam.Flags)$3|FormalParam.Flags.Default, (Expression)$6); }
;


optional_type:
		/* empty */	{ $$ = null; }
	|	type_expr	{ $$ = $1; }
;

type_expr:
		type		{ $$ = _astFactory.TypeReference(@$, (QualifiedName)$1, false, null); }
	|	'?' type	{ $$ = _astFactory.TypeReference(@$, (QualifiedName)$2, true, null); }
;

type:
		T_ARRAY		{ $$ = QualifiedName.Array; }
	|	T_CALLABLE	{ $$ = QualifiedName.Callable; }
	|	name		{ $$ = (QualifiedName)$1; }
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
			{ $$ = new List<ActualParam>() { (ActualParam)$1 }; }
	|	non_empty_argument_list ',' argument
			{ $$ = AddToList<ActualParam>($1, (ActualParam)$3); }
;

argument:
		expr			{ $$ = _astFactory.ActualParameter(@1, (Expression)$1, ActualParam.Flags.Default); }
	|	T_ELLIPSIS expr	{ $$ = _astFactory.ActualParameter(@2, (Expression)$2, ActualParam.Flags.IsUnpack); }
;

global_var_list:
		global_var_list ',' global_var { $$ = AddToList<LangElement>($1, $3); }
	|	global_var { $$ = new List<LangElement>() { (LangElement)$1 }; }
;

global_var:
	simple_variable
		{ $$ = $1; }
;


static_var_list:
		static_var_list ',' static_var { $$ = AddToList<LangElement>($1, $3); }
	|	static_var { $$ = new List<LangElement>() { (LangElement)$1 }; }
;

static_var:
		T_VARIABLE			{ $$ = _astFactory.StaticVarDecl(@$, new VariableName((string)$1), null); }
	|	T_VARIABLE '=' expr	{ $$ = _astFactory.StaticVarDecl(@$, new VariableName((string)$1), (LangElement)$3); }
;


class_statement_list:
		class_statement_list class_statement
			{ $$ = AddToList<LangElement>($1, $2); }
	|	/* empty */
			{ $$ = new List<LangElement>(); }
;


class_statement:
		variable_modifiers property_list ';'
			{ $$ = _astFactory.DeclList(@$, (PhpMemberAttributes)$1, (List<LangElement>)$2); }
	|	method_modifiers T_CONST class_const_list ';'
			{ $$ = _astFactory.DeclList(@$, (PhpMemberAttributes)$1, (List<LangElement>)$3); }
	|	T_USE name_list trait_adaptations
			{ $$ = _astFactory.TraitUse(@$, (List<QualifiedName>)$2, (List<TraitsUse.TraitAdaptation>)$3); }
	|	method_modifiers function returns_ref identifier backup_doc_comment '(' parameter_list ')'
		return_type backup_fn_flags method_body backup_fn_flags
			{ $$ = _astFactory.Method(@$, false, (PhpMemberAttributes)$1, 
				(TypeRef)$9, @9, 
				new Name((string)$4), @4, null, (List<FormalParam>)$7, @8, 
				null, _astFactory.Block(@11, (List<LangElement>)$11)); 
			if($5 != null)
				((MethodDecl)$$).PHPDoc = new PHPDocBlock((string)$5, @5);
			}
;

name_list:
		name { $$ = new List<QualifiedName>() { (QualifiedName)$1 };
 }
	|	name_list ',' name { $$ = AddToList<QualifiedName>($1, $3); }
;

trait_adaptations:
		';'								{ $$ = null; }
	|	'{' '}'							{ $$ = null; }
	|	'{' trait_adaptation_list '}'	{ $$ = $2; }
;

trait_adaptation_list:
		trait_adaptation
			{ $$ = new List<TraitsUse.TraitAdaptation>() { (TraitsUse.TraitAdaptation)$1 };
 }
	|	trait_adaptation_list trait_adaptation
			{ $$ = AddToList<TraitsUse.TraitAdaptation>($1, $2); }
;

trait_adaptation:
		trait_precedence ';'	{ $$ = $1; }
	|	trait_alias ';'			{ $$ = $1; }
;

trait_precedence:
	absolute_trait_method_reference T_INSTEADOF name_list
		{ $$ = _astFactory.TraitAdaptationPrecedence(@$, (Tuple<QualifiedName?,Name>)$1, (List<QualifiedName>)$3); }
;

trait_alias:
		trait_method_reference T_AS T_STRING
			{ $$ = _astFactory.TraitAdaptationAlias(@$, (Tuple<QualifiedName?, Name>)$1, (string)$3, null); }
	|	trait_method_reference T_AS reserved_non_modifiers
			{ $$ = _astFactory.TraitAdaptationAlias(@$, (Tuple<QualifiedName?, Name>)$1, (string)$3.Object, null); }
	|	trait_method_reference T_AS member_modifier identifier
			{ $$ = _astFactory.TraitAdaptationAlias(@$, (Tuple<QualifiedName?, Name>)$1, (string)$4, (PhpMemberAttributes)$3); }
	|	trait_method_reference T_AS member_modifier
			{ $$ = _astFactory.TraitAdaptationAlias(@$, (Tuple<QualifiedName?, Name>)$1, null, (PhpMemberAttributes)$3); }
;

trait_method_reference:
		identifier
			{ $$ = new Tuple<QualifiedName?,Name>(null, new Name((string)$1)); }
	|	absolute_trait_method_reference { $$ = $1; }
;

absolute_trait_method_reference:
	name T_DOUBLE_COLON identifier
		{ $$ = new Tuple<QualifiedName?,Name>((QualifiedName)$1, new Name((string)$3)); }
;

method_body:
		';' /* abstract method */		{ $$ = null; }
	|	'{' inner_statement_list '}'	{ $$ = $2; }
;

variable_modifiers:
		non_empty_member_modifiers		{ $$ = $1; }
	|	T_VAR							{ $$ = (long)PhpMemberAttributes.Public; }
;

method_modifiers:
		/* empty */						{ $$ = (long)PhpMemberAttributes.Public; }
	|	non_empty_member_modifiers
			{ $$ = $1; if (($$ & (long)PhpMemberAttributes.VisibilityMask) == 0) { $$ |= (long)PhpMemberAttributes.Public; } }
;

non_empty_member_modifiers:
		member_modifier			{ $$ = $1; }
	|	non_empty_member_modifiers member_modifier
			{ $$ = $1 | $2; }
;

member_modifier:
		T_PUBLIC				{ $$ = (long)PhpMemberAttributes.Public; }
	|	T_PROTECTED				{ $$ = (long)PhpMemberAttributes.Protected; }
	|	T_PRIVATE				{ $$ = (long)PhpMemberAttributes.Private; }
	|	T_STATIC				{ $$ = (long)PhpMemberAttributes.Static; }
	|	T_ABSTRACT				{ $$ = (long)PhpMemberAttributes.Abstract; }
	|	T_FINAL					{ $$ = (long)PhpMemberAttributes.Final; }
;

property_list:
		property_list ',' property { $$ = AddToList<LangElement>($1, $3); }
	|	property { $$ = new List<LangElement>() { (LangElement)$1 }; }
;

property:
		T_VARIABLE backup_doc_comment
			{ $$ = _astFactory.FieldDecl(@$, new VariableName((string)$1), null); if($2 != null) ((FieldDecl)$$).PHPDoc = new PHPDocBlock((string)$2, @2); }
	|	T_VARIABLE '=' expr backup_doc_comment
			{ $$ = _astFactory.FieldDecl(@$, new VariableName((string)$1), (Expression)$3); if($4 != null) ((FieldDecl)$$).PHPDoc = new PHPDocBlock((string)$4, @4); }
;

class_const_list:
		class_const_list ',' class_const_decl { $$ = AddToList<LangElement>($1, $3); }
	|	class_const_decl { $$ = new List<LangElement>() { (LangElement)$1 }; }
;

class_const_decl:
	identifier '=' expr backup_doc_comment { $$ = _astFactory.ClassConstDecl(@$, new VariableName((string)$1), (LangElement)$3); 
		if($4 != null) ((ClassConstantDecl)$$).PHPDoc = new PHPDocBlock((string)$4, @4); }
;

const_decl:
	T_STRING '=' expr backup_doc_comment { $$ = _astFactory.GlobalConstDecl(@$, false, new VariableName((string)$1), (LangElement)$3); 
		if($4 != null) ((GlobalConstantDecl)$$).PHPDoc = new PHPDocBlock((string)$4, @4); }
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
		non_empty_for_exprs ',' expr { $$ = AddToList<LangElement>($1, $3); }
	|	expr { $$ = new List<LangElement>() { (LangElement)$1 }; }
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
			{ $$ = _astFactory.New(@$, (TypeRef)$2, (List<ActualParam>)$3); }
	|	T_NEW anonymous_class
			{ $$ = $2; }
;

expr_without_variable:
		T_LIST '(' array_pair_list ')' '=' expr
			{ $$ = _astFactory.Assignment(@$, _astFactory.List(@3, (List<Item>)$3), (LangElement)$6, Operations.AssignValue); }
	|	'[' array_pair_list ']' '=' expr
			{ $$ = _astFactory.Assignment(@$, _astFactory.NewArray(@2, (List<Item>)$2), (LangElement)$5, Operations.AssignValue); }
	|	variable '=' expr
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, (LangElement)$3, Operations.AssignValue); }
	|	variable '=' '&' variable
			{ $$ = _astFactory.Assignment(@$, (LangElement)$1, _astFactory.Variable(@$, (LangElement)$4, (LangElement)null), Operations.AssignRef); }
	|	T_CLONE expr { $$ = _astFactory.UnaryOperation(@$, Operations.Clone,   (Expression)$2); }
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
			{ $$ = _astFactory.ConditionalEx(@$, (LangElement)$1, (LangElement)$3, (LangElement)$5); }
	|	expr '?' ':' expr
			{ $$ = _astFactory.ConditionalEx(@$, (LangElement)$1, null, (LangElement)$4); }
	|	expr T_COALESCE expr
			{ $$ = _astFactory.BinaryOperation(@$, Operations.Coalesce, (LangElement)$1, (LangElement)$3); }
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
	|	'`' backticks_expr '`' { $$ = _astFactory.Shell(@$, (LangElement)$2); }
	|	T_PRINT expr { $$ = _astFactory.UnaryOperation(@$, Operations.Print, (Expression)$2); }
	|	T_YIELD { $$ = _astFactory.Yield(@$, null, null); }
	|	T_YIELD expr { $$ = _astFactory.Yield(@$, null, (LangElement)$2); }
	|	T_YIELD expr T_DOUBLE_ARROW expr { $$ = _astFactory.Yield(@$, (LangElement)$2, (LangElement)$4); }
	|	T_YIELD_FROM expr { $$ = _astFactory.YieldFrom(@$, (LangElement)$2); }
	|	function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars return_type
		backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
			{ $$ = _astFactory.Lambda(@$, false, (TypeRef)$8, @8, 
				Span.FromBounds(@1.Start, @4.End), (List<FormalParam>)$5, @6, 
				(List<FormalParam>)$7, _astFactory.Block(@11, (List<LangElement>)$11)); 
			if($3 != null)
				((LambdaFunctionExpr)$$).PHPDoc = new PHPDocBlock((string)$3, @3);
			}
	|	T_STATIC function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars
		return_type backup_fn_flags '{' inner_statement_list '}' backup_fn_flags
			{ $$ = _astFactory.Lambda(@$, false, (TypeRef)$9, @9, 
				Span.FromBounds(@1.Start, @5.End), (List<FormalParam>)$6, @7, 
				(List<FormalParam>)$8, _astFactory.Block(@12, (List<LangElement>)$12)); 
			if($4 != null)
				((LambdaFunctionExpr)$$).PHPDoc = new PHPDocBlock((string)$4, @4);
			}
;

function:
	T_FUNCTION 
;

backup_doc_comment:
	/* empty */ { $$ = Scanner.DocBlock; }
;

backup_fn_flags:
	/* empty */ {  }
;

returns_ref:
		/* empty */	{ $$ = 0; }
	|	'&'			{ $$ = (long)FormalParam.Flags.IsByRef; }
;

lexical_vars:
		/* empty */ { $$ = new List<FormalParam>(); }
	|	T_USE '(' lexical_var_list ')' { $$ = $3; }
;

lexical_var_list:
		lexical_var_list ',' lexical_var { $$ = AddToList<FormalParam>($1, $3); }
	|	lexical_var { $$ = new List<FormalParam>() { (FormalParam)$1 }; }
;

lexical_var:
		T_VARIABLE		{ $$ = _astFactory.Parameter(@$, (string)$1, null, FormalParam.Flags.Default, null); }
	|	'&' T_VARIABLE	{ $$ = _astFactory.Parameter(@$, (string)$2, null, FormalParam.Flags.IsByRef, null); }
;

function_call:
		name argument_list
			{ $$ = _astFactory.Call(@$, (QualifiedName)$1, null, @1, new CallSignature((List<ActualParam>)$2), null); }
	|	class_name T_DOUBLE_COLON member_name argument_list
			{
				if($3 is Name)
					$$ = _astFactory.Call(@$, (Name)$3, @2, new CallSignature((List<ActualParam>)$4), (TypeRef)_astFactory.TypeReference(@1, (QualifiedName)$1, false, null)); 
				else
					$$ = _astFactory.Call(@$, (LangElement)$3, new CallSignature((List<ActualParam>)$4), (TypeRef)_astFactory.TypeReference(@1, (QualifiedName)$1, false, null)); 
			}
	|	variable_class_name T_DOUBLE_COLON member_name argument_list
			{
				if($3 is Name)
					$$ = _astFactory.Call(@$, (Name)$3, @2, new CallSignature((List<ActualParam>)$4), (TypeRef)_astFactory.TypeReference(@1, (LangElement)$1, null)); 
				else
					$$ = _astFactory.Call(@$, (LangElement)$3, new CallSignature((List<ActualParam>)$4), (TypeRef)_astFactory.TypeReference(@1, (LangElement)$1, null)); 
			}
	|	callable_expr argument_list
			{ $$ = _astFactory.Call(@$, (LangElement)$1, new CallSignature((List<ActualParam>)$2), (LangElement)null);}
;

class_name:
		T_STATIC
			{ $$ = new QualifiedName(Name.StaticClassName); }
	|	name { $$ = $1; }
;

class_name_reference:
		class_name		{ $$ = _astFactory.TypeReference(@1, (QualifiedName)$1, false, null); }
	|	new_variable	{ $$ = _astFactory.TypeReference(@1, (LangElement)$1, null); }
;

exit_expr:
		/* empty */				{ $$ = null; }
	|	'(' optional_expr ')'	{ $$ = $2; }
;

backticks_expr:
		/* empty */
			{ $$ = _astFactory.Literal(@$, string.Empty); }
	|	T_ENCAPSED_AND_WHITESPACE { $$ = _astFactory.Literal(@$, $1); }
	|	encaps_list { $$ = $1; }
;


ctor_arguments:
		/* empty */	{ $$ = new List<ActualParam>(); }
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
		name { $$ = _astFactory.ConstUse(@$, (QualifiedName)$1, null); }
	|	class_name T_DOUBLE_COLON identifier
			{ $$ = _astFactory.ClassConstUse(@$, (TypeRef)_astFactory.TypeReference(@$, (QualifiedName)$1, false, null), new Name((string)$3), @3); }
	|	variable_class_name T_DOUBLE_COLON identifier
			{ $$ = _astFactory.ClassConstUse(@$, (TypeRef)_astFactory.TypeReference(@$, (LangElement)$1, null), new Name((string)$3), @3); }
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
			{ $$ = _astFactory.ArrayItem(@$, (LangElement)$1, (LangElement)$3); }
	|	constant '[' optional_expr ']'
			{ $$ = _astFactory.ArrayItem(@$, (LangElement)$1, (LangElement)$3); }
	|	dereferencable '{' expr '}'
			{ $$ = _astFactory.ArrayItem(@$, (LangElement)$1, (LangElement)$3); }
	|	dereferencable T_OBJECT_OPERATOR property_name argument_list
		{
			if($3 is Name)
				$$ = _astFactory.Call(@$, new QualifiedName((Name)$3), null, @3, new CallSignature((List<ActualParam>)$4), (LangElement)$1);
			else
				$$ = _astFactory.Call(@$, (LangElement)$3, new CallSignature((List<ActualParam>)$4), (LangElement)$1);
		}
	|	function_call { $$ = $1; }
;

variable:
		callable_variable
			{ $$ = $1; }
	|	static_member
			{ $$ = $1; }
	|	dereferencable T_OBJECT_OPERATOR property_name
			{ $$ = CreateProperty(@$, (LangElement)$1, $3); }
;

simple_variable:
		T_VARIABLE			{ $$ = _astFactory.Variable(@$, new VariableName((string)$1), (LangElement)null); }
	|	'$' '{' expr '}'	{ $$ = _astFactory.Variable(@$, (LangElement)$3, (LangElement)null); }
	|	'$' simple_variable	{ $$ = $2; }
;

static_member:
		class_name T_DOUBLE_COLON simple_variable
			{ $$ = CreateStaticProperty(@$, (QualifiedName)$1, @1, (LangElement)$3); }	
	|	variable_class_name T_DOUBLE_COLON simple_variable
			{ $$ = CreateStaticProperty(@$, (LangElement)$1, @1, (LangElement)$3); }
;

new_variable:
		simple_variable
			{ $$ = $1; }
	|	new_variable '[' optional_expr ']'
			{ $$ = _astFactory.ArrayItem(@$, (LangElement)$1, (LangElement)$3); }
	|	new_variable '{' expr '}'
			{ $$ = _astFactory.ArrayItem(@$, (LangElement)$1, (LangElement)$3); }
	|	new_variable T_OBJECT_OPERATOR property_name
			{ $$ = CreateProperty(@$, (LangElement)$1, $3); }
	|	class_name T_DOUBLE_COLON simple_variable
			{ $$ = CreateStaticProperty(@$, (QualifiedName)$1, @1, (LangElement)$3); }
	|	new_variable T_DOUBLE_COLON simple_variable
			{ $$ = CreateStaticProperty(@$, (LangElement)$1, @1, (LangElement)$3); }
;

member_name:
		identifier { $$ = new Name((string)$1); }
	|	'{' expr '}'	{ $$ = $2; }
	|	simple_variable	{ $$ = $1; }
;

property_name:
		T_STRING { $$ = new Name((string)$1); }
	|	'{' expr '}'	{ $$ = $2; }
	|	simple_variable	{ $$ = $1; }
;

array_pair_list:
		non_empty_array_pair_list
			{ $$ = RightTrimList((List<Item>)$1);  }
;

possible_array_pair:
		/* empty */ { $$ = null; }
	|	array_pair  { $$ = $1; }
;

non_empty_array_pair_list:
		non_empty_array_pair_list ',' possible_array_pair
			{ $$ = AddToList<Item>($1, $3); }
	|	possible_array_pair
			{ $$ = new List<Item>() { (Item)$1 }; }
;

array_pair:
		expr T_DOUBLE_ARROW expr
			{ $$ = _astFactory.ArrayItemValue(@$, (LangElement)$1, (LangElement)$3); }
	|	expr
			{ $$ = _astFactory.ArrayItemValue(@$, null, (LangElement)$1); }
	|	expr T_DOUBLE_ARROW '&' variable
			{ $$ = _astFactory.ArrayItemRef(@$, (LangElement)$1, (LangElement)$4); }
	|	'&' variable
			{ $$ = _astFactory.ArrayItemRef(@$, null, (LangElement)$2); }
	|	expr T_DOUBLE_ARROW T_LIST '(' array_pair_list ')'
			{ $$ = _astFactory.ArrayItemValue(@$, (LangElement)$1, _astFactory.List(@5, (List<Item>)$5)); }
	|	T_LIST '(' array_pair_list ')'
			{ $$ = _astFactory.ArrayItemValue(@$, null, _astFactory.List(@3, (List<Item>)$3)); }
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
