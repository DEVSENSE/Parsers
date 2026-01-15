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

using System.Runtime.InteropServices;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using Devsense.PHP.Errors;
using TNode = Devsense.PHP.Syntax.Ast.LangElement;

%%

%namespace Devsense.PHP.Syntax
%valuetype SemanticValueType
%positiontype Span
%tokentype Tokens
%visibility public

%union
{
	// defined in partial
}

%left T_THROW
%left PREC_ARROW_FUNCTION
%left T_INCLUDE T_INCLUDE_ONCE T_REQUIRE T_REQUIRE_ONCE
%left T_LOGICAL_OR
%left T_LOGICAL_XOR
%left T_LOGICAL_AND
%right T_PRINT
%right T_YIELD
%right T_DOUBLE_ARROW
%right T_YIELD_FROM
%left '=' T_PLUS_EQUAL T_MINUS_EQUAL T_MUL_EQUAL T_DIV_EQUAL T_CONCAT_EQUAL T_MOD_EQUAL T_AND_EQUAL T_OR_EQUAL T_XOR_EQUAL T_SL_EQUAL T_SR_EQUAL T_POW_EQUAL T_COALESCE_EQUAL
%left '?' ':'
%right T_COALESCE
%left T_BOOLEAN_OR
%left T_BOOLEAN_AND
%left '|'
%left '^'
%left T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG
%nonassoc T_IS_EQUAL T_IS_NOT_EQUAL T_IS_IDENTICAL T_IS_NOT_IDENTICAL T_SPACESHIP
%nonassoc '<' T_IS_SMALLER_OR_EQUAL '>' T_IS_GREATER_OR_EQUAL
%left T_PIPE_OPERATOR   // |>
%left '.'
%left T_SL T_SR
%left '+' '-'
%left '*' '/' '%'
%right '!'
%right T_INSTANCEOF
%right '~' T_INC T_DEC T_INT_CAST T_DOUBLE_CAST T_STRING_CAST T_ARRAY_CAST T_OBJECT_CAST T_BOOL_CAST T_UNSET_CAST '@'
%right T_POW
%right '['
%nonassoc T_NEW T_CLONE
%left T_NOELSE
%left T_ELSEIF
%left T_ELSE
%left T_ENDIF
%right T_STATIC T_ABSTRACT T_FINAL T_PRIVATE T_PROTECTED T_PUBLIC T_PUBLIC_SET T_PROTECTED_SET T_PRIVATE_SET T_READONLY

%token <Long> T_LNUMBER 317   //"integer number (T_LNUMBER)"
%token <Double> T_DNUMBER 312   //"floating-point number (T_DNUMBER)"
%token <String> T_STRING 319   //"identifier (T_STRING)"
%token <String> T_VARIABLE 320 //"variable (T_VARIABLE)"
%token <String> T_INLINE_HTML 321
%token <Strings> T_ENCAPSED_AND_WHITESPACE 316  //"quoted-string and whitespace (T_ENCAPSED_AND_WHITESPACE)"
%token <Object> T_CONSTANT_ENCAPSED_STRING 323 //"quoted-string (T_CONSTANT_ENCAPSED_STRING)"
%token <String> T_STRING_VARNAME 318 //"variable name (T_STRING_VARNAME)"
%token <Long> T_NUM_STRING 325 //"number (T_NUM_STRING)"

/* Character tokens */
%token T_EXCLAM 33 //'!'
%token T_DOUBLE_QUOTES 34 //'"'
%token T_DOLLAR 36 //'$'
%token T_PERCENT 37 //'%'
%token T_SINGLE_QUOTES 39 //'\''
%token T_LPAREN 40 //'('
%token T_RPAREN 41 //')'
%token T_MUL 42 //'*'
%token T_PLUS 43 //'+'
%token T_COMMA 44 //','
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
%token <Object> T_PLUS_EQUAL 270  //"+= (T_PLUS_EQUAL)"
%token <Object> T_MINUS_EQUAL 271  //"-= (T_MINUS_EQUAL)"
%token <Object> T_MUL_EQUAL 279    //"*= (T_MUL_EQUAL)"
%token <Object> T_DIV_EQUAL 278   //"/= (T_DIV_EQUAL)"
%token <Object> T_CONCAT_EQUAL 277 //".= (T_CONCAT_EQUAL)"
%token <Object> T_MOD_EQUAL 276    //"%= (T_MOD_EQUAL)"
%token <Object> T_AND_EQUAL 275    //"&= (T_AND_EQUAL)"
%token <Object> T_OR_EQUAL 274     //"|= (T_OR_EQUAL)"
%token <Object> T_XOR_EQUAL 273    //"^= (T_XOR_EQUAL)"
%token <Object> T_SL_EQUAL 272    //"<<= (T_SL_EQUAL)"
%token <Object> T_SR_EQUAL 280     //">>= (T_SR_EQUAL)"
%token <Object> T_BOOLEAN_OR 284  //"|| (T_BOOLEAN_OR)"
%token <Object> T_BOOLEAN_AND 285  //"&& (T_BOOLEAN_AND)"
%token <Object> T_IS_EQUAL 289     //"== (T_IS_EQUAL)"
%token <Object> T_IS_NOT_EQUAL 288 //"!= (T_IS_NOT_EQUAL)"
%token <Object> T_IS_IDENTICAL 287 //"=== (T_IS_IDENTICAL)"
%token <Object> T_IS_NOT_IDENTICAL 286 //"!== (T_IS_NOT_IDENTICAL)"
%token <Object> T_IS_SMALLER_OR_EQUAL 291 //"<= (T_IS_SMALLER_OR_EQUAL)"
%token <Object> T_IS_GREATER_OR_EQUAL 292 //">= (T_IS_GREATER_OR_EQUAL)"
%token <Object> T_SPACESHIP 290 //"<=> (T_SPACESHIP)"
%token <Object> T_SL 293 //"<< (T_SL)"
%token <Object> T_SR 294 //">> (T_SR)"
%token <Object> T_INSTANCEOF 295  //"instanceof (T_INSTANCEOF)"
%token <Object> T_INC 303 //"++ (T_INC)"
%token <Object> T_DEC 302 //"-- (T_DEC)"
%token <Object> T_INT_CAST 301   //"(int) (T_INT_CAST)"
%token <Object> T_DOUBLE_CAST 300 //"(double) (T_DOUBLE_CAST)"
%token <Object> T_STRING_CAST 299 //" (T_STRING_CAST)"
%token <Object> T_ARRAY_CAST 298  //"(array) (T_ARRAY_CAST)"
%token <Object> T_OBJECT_CAST 297 //"(object) (T_OBJECT_CAST)"
%token <Object> T_BOOL_CAST 296   //"(bool) (T_BOOL_CAST)"
%token <Object> T_UNSET_CAST 304  //"(unset) (T_UNSET_CAST)"
%token <Object> T_NEW 306      //"new (T_NEW)"
%token <Object> T_CLONE 307     //"clone (T_CLONE)"
%token <Object> T_EXIT 326      //"exit (T_EXIT)"
%token <Object> T_IF 322       //"if (T_IF)"
%token <Object> T_ELSEIF 308   //"elseif (T_ELSEIF)"
%token <Object> T_ELSE 309     //"else (T_ELSE)"
%token <Object> T_ENDIF 310    //"endif (T_ENDIF)"
%token <Object> T_ECHO 324      //"echo (T_ECHO)"
%token <Object> T_DO 329        //"do (T_DO)"
%token <Object> T_WHILE 330     //"while (T_WHILE)"
%token <Object> T_ENDWHILE 327  //"endwhile (T_ENDWHILE)"
%token <Object> T_FOR 328       //"for (T_FOR)"
%token <Object> T_ENDFOR 333    //"endfor (T_ENDFOR)"
%token <Object> T_FOREACH 334   //"foreach (T_FOREACH)"
%token <Object> T_ENDFOREACH 331 //"endforeach (T_ENDFOREACH)"
%token <Object> T_DECLARE 332   //"declare (T_DECLARE)"
%token <Object> T_ENDDECLARE 337 //"enddeclare (T_ENDDECLARE)"
%token <Object> T_AS 338        //"as (T_AS)"
%token <Object> T_SWITCH 335    //"switch (T_SWITCH)"
%token <Object> T_ENDSWITCH 336 //"endswitch (T_ENDSWITCH)"
%token <Object> T_CASE 341      //"case (T_CASE)"
%token <Object> T_DEFAULT 342   //"default (T_DEFAULT)"
%token <Object> T_MATCH	395		//"match" (T_MATCH)
%token <Object> T_BREAK 339     //"break (T_BREAK)"
%token <Object> T_CONTINUE 340  //"continue (T_CONTINUE)"
%token <Object> T_GOTO 345      //"goto (T_GOTO)"
%token <Object> T_FUNCTION 346  //"function (T_FUNCTION)"
%token <Object> T_FN 343        // "fn (T_FN)"
%token <Object> T_CONST 344     //"const (T_CONST)"
%token <Object> T_RETURN 348     //"return (T_RETURN)"
%token <Object> T_TRY 349       //"try (T_TRY)"
%token <Object> T_CATCH 347     //"catch (T_CATCH)"
%token <Object> T_FINALLY 351   //"finally (T_FINALLY)"
%token <Object> T_THROW 352     //"throw (T_THROW)"
%token <Object> T_USE 350       //"use (T_USE)"
%token <Object> T_INSTEADOF 354  //"insteadof (T_INSTEADOF)"
%token <Object> T_GLOBAL 355    //"exit_scope (T_GLOBAL)"
%token <Object> T_STATIC 353    //"static (T_STATIC)"
%token <Object> T_ABSTRACT 315  //"abstract (T_ABSTRACT)"
%token <Object> T_FINAL 314     //"final (T_FINAL)"
%token <Object> T_PRIVATE 313   //"private (T_PRIVATE)"
%token <Object> T_PROTECTED 357 //"protected (T_PROTECTED)"
%token <Object> T_PUBLIC 311    //"public (T_PUBLIC)"
%token <String> T_READONLY 398  //"readonly (T_READONLY)"
%token <Object> T_VAR 356        //"var (T_VAR)"
%token <Object> T_UNSET 360     //"unset (T_UNSET)"
%token <Object> T_ISSET 358     //"isset (T_ISSET)"
%token <Object> T_EMPTY 359     //"empty (T_EMPTY)"
%token <Object> T_HALT_COMPILER 363 //"__halt_compiler (T_HALT_COMPILER)"
%token <Object> T_CLASS 361     //"class (T_CLASS)"
%token <Object> T_TRAIT 362     //"trait (T_TRAIT)"
%token <Object> T_INTERFACE 366 //"interface (T_INTERFACE)"
%token <Object> T_ENUM 388      // "enum" (T_ENUM)
%token <Object> T_EXTENDS 364   //"extends (T_EXTENDS)"
%token <Object> T_IMPLEMENTS 365 //"implements (T_IMPLEMENTS)"
%token <Object> T_OBJECT_OPERATOR 369 //"-> (T_OBJECT_OPERATOR)"
%token <Object> T_NULLSAFE_OBJECT_OPERATOR  396 //"?-> (T_NULLSAFE_OBJECT_OPERATOR )"
%token <Object> T_DOUBLE_ARROW 268    //"=> (T_DOUBLE_ARROW)"
%token <Object> T_LIST 367           //"list (T_LIST)"
%token <Object> T_ARRAY 368          //"array (T_ARRAY)"
%token <Object> T_CALLABLE 372       //"callable (T_CALLABLE)"
%token <Object> T_LINE 370           //"__LINE__ (T_LINE)"
%token <Object> T_FILE 371           //"__FILE__ (T_FILE)"
%token <Object> T_DIR 375            //"__DIR__ (T_DIR)"
%token <Object> T_CLASS_C 373        //"__CLASS__ (T_CLASS_C)"
%token <Object> T_TRAIT_C 374        //"__TRAIT__ (T_TRAIT_C)"
%token <Object> T_PROPERTY_C 389       //"__PROPERTY__ (T_PROPERTY)"
%token <Object> T_METHOD_C 378       //"__METHOD__ (T_METHOD_C)"
%token <Object> T_FUNC_C 376         //"__FUNCTION__ (T_FUNC_C)"
%token <Object> T_COMMENT 377        //"comment (T_COMMENT)"
%token <Object> T_DOC_COMMENT 381    //"doc comment (T_DOC_COMMENT)"
%token <Object> T_OPEN_TAG 379       //"open tag (T_OPEN_TAG)"
%token <Object> T_OPEN_TAG_WITH_ECHO 380 //"open tag with echo (T_OPEN_TAG_WITH_ECHO)"
%token <Object> T_CLOSE_TAG 384      //"close tag (T_CLOSE_TAG)"
%token <Object> T_WHITESPACE 382     //"whitespace (T_WHITESPACE)"
%token T_START_HEREDOC 383  //"heredoc start (T_START_HEREDOC)"
%token <HereDocValue> T_END_HEREDOC 387    //"heredoc end (T_END_HEREDOC)"
%token <Object> T_DOLLAR_OPEN_CURLY_BRACES 385 //"${ (T_DOLLAR_OPEN_CURLY_BRACES)"
%token <Object> T_CURLY_OPEN 386     //"{$ (T_CURLY_OPEN)"
%token <Object> T_DOUBLE_COLON 390  //":: (T_DOUBLE_COLON)"
%token <Object> T_NAMESPACE 391       //"namespace (T_NAMESPACE)"
%token <Object> T_NS_C 392            //"__NAMESPACE__ (T_NS_C)"
%token <Object> T_NS_SEPARATOR 393    //"\\ (T_NS_SEPARATOR)"
%token <Object> T_ELLIPSIS 394        //"... (T_ELLIPSIS)"
%token <Object> T_POW_EQUAL 281       //"**= (T_POW_EQUAL)"
%token <Object> T_PIPE_OPERATOR	399	  //"|> (T_PIPE_OPERATOR)"
%token <Object> T_COALESCE_EQUAL 282  //"??= (T_COALESCE_EQUAL)"
%token <Object> T_COALESCE 283        //"?? (T_COALESCE)"
%token <Object> T_POW 305             //"** (T_POW)"
%token <Object> T_ATTRIBUTE 397             //"#[ (T_ATTRIBUTE)"
%token <Object> T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG 400     // "&"
%token <Object> T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG 401 // "&"

/* Token used to force a parse error from the lexer */
%token T_ERROR 257

// type safe declaration
%type <Object> property_name member_name

%type <Bool> possible_comma
%type <VariableName> variable_init_optional /* helper rule to match T_VARIABLE = expr */

%type <Long> returns_ref function fn is_reference is_variadic variable_modifiers property_hook_modifiers
%type <Long> method_modifiers non_empty_member_modifiers member_modifier class_modifier class_modifiers
%type <Long> optional_property_modifiers property_modifier
%type <Kind> use_type

%type <Object> backup_doc_comment enter_scope exit_scope

%type <QualifiedNameReference> name
%type <Token> object_operator

%type <TypeReference> type type_without_static return_type type_expr type_expr_without_static optional_type optional_type_without_static extends_from union_type_element union_type_without_static_element
%type <TypeReference> class_name_reference class_name

%type <TypeRefList> name_list catch_name_list implements_list interface_extends_list union_type union_type_without_static intersection_type intersection_type_without_static

%type <Node> top_statement statement function_declaration_statement class_declaration_statement 
%type <Node> trait_declaration_statement interface_declaratioimplements_listn_statement 
%type <Node> interface_declaration_statement inline_html isset_variable expr variable 
%type <Node> internal_functions_in_yacc callable_variable
%type <Node> new_dereferenceable new_non_dereferenceable
%type <Node> simple_variable scalar constant class_constant dereferencable_scalar function_call
%type <Node> static_member if_stmt alt_if_stmt const_decl unset_variable
%type <Node> global_var static_var echo_expr optional_expr 
%type <Node> property encaps_var encaps_var_offset declare_statement
%type <Node> trait_adaptation trait_precedence trait_alias
%type <Node> class_const_decl dereferencable array_object_dereferenceable for_statement foreach_statement
%type <Node> while_statement backticks_expr method_body exit_expr
%type <Node> finally_statement new_variable callable_expr
%type <Node> trait_adaptations variable_class_name inner_statement class_statement
%type <Node> inline_function
%type <Node> attributed_statement attributed_class_statement
%type <FormalParam> attributed_parameter
%type <Node> attribute attribute_decl
%type <NodeList> attributes attribute_group
%type <Node> match match_arm
%type <NodeList> match_arm_list match_arm_cond_list non_empty_match_arm_list
%type <Node> enum_declaration_statement enum_backing_type enum_case enum_case_expr
%type <Node> property_hook hooked_property property_hook_body
%type <NodeList> property_hook_list optional_property_hook_list

%type <NodeList> top_statement_list const_list class_const_list
%type <NodeList> inner_statement_list class_statement_list for_exprs
%type <NodeList> global_var_list static_var_list echo_expr_list unset_variables
%type <NodeList> trait_adaptation_list encaps_list isset_variables property_list
%type <NodeList> non_empty_for_exprs catch_list
%type <Node> optional_variable
%type <SwitchObject> case_list switch_case_list

%type <String> identifier semi_reserved reserved_non_modifiers function_name
%type <StringList> namespace_name

%type <Alias> unprefixed_use_declaration
%type <Object> trait_method_reference absolute_trait_method_reference
%type <Alias> use_declaration
%type <AliasList> unprefixed_use_declarations

%type <ContextAlias> inline_use_declaration
%type <ContextAliasList> inline_use_declarations

%type <Param> argument argument_expr argument_no_expr
%type <ParamList> ctor_arguments argument_list non_empty_argument_list clone_argument_list non_empty_clone_argument_list

%type <FormalParam> parameter lexical_var
%type <FormalParamList> optional_parameter_list parameter_list non_empty_parameter_list lexical_vars lexical_var_list

%type <Item> possible_array_pair array_pair
%type <ItemList> non_empty_array_pair_list non_empty_array_pair_list array_pair_list

%type <IfItemList> if_stmt_without_else alt_if_stmt_without_else

%type <ForeachVar> foreach_variable

%type <AnonymousClass> anonymous_class

%type <UseList> use_declarations group_use_declaration mixed_group_use_declaration

%% /* Rules */

start:
    { SetNamingContext(null); }
	top_statement_list
	{ 
		AssignNamingContext(); 
        _lexer.DocCommentList.Merge(new Span(0, int.MaxValue), $2, _astFactory);
		AssignStatements($2);
		_astRoot = _astFactory.GlobalCode(@$, GetArrayAndFree<TNode, Statement>($2), namingContext);
	}
;
reserved_non_modifiers:
	  T_INCLUDE
	| T_INCLUDE_ONCE
	| T_EVAL
	| T_REQUIRE
	| T_REQUIRE_ONCE
	| T_LOGICAL_OR
	| T_LOGICAL_XOR
	| T_LOGICAL_AND
	| T_INSTANCEOF
	| T_NEW
	| T_CLONE
	| T_EXIT	
	| T_IF	
	| T_ELSEIF	
	| T_ELSE	
	| T_ENDIF	
	| T_ECHO	
	| T_DO	
	| T_WHILE	
	| T_ENDWHILE 	
	| T_FOR	
	| T_ENDFOR	
	| T_FOREACH	
	| T_ENDFOREACH	
	| T_DECLARE	
	| T_ENDDECLARE	
	| T_AS	
	| T_TRY	
	| T_CATCH	
	| T_FINALLY 	
	| T_THROW	
	| T_USE	
	| T_INSTEADOF	
	| T_GLOBAL	
	| T_VAR	
	| T_UNSET	
	| T_ISSET	
	| T_EMPTY	
	| T_CONTINUE
	| T_GOTO 	
	| T_FUNCTION	
	| T_CONST	
	| T_RETURN	
	| T_PRINT	
	| T_YIELD	
	| T_LIST
	| T_SWITCH	
	| T_ENDSWITCH	
	| T_CASE	
	| T_DEFAULT
	| T_BREAK
	| T_ARRAY
	| T_CALLABLE
	| T_EXTENDS	
	| T_IMPLEMENTS	
	| T_NAMESPACE	
	| T_TRAIT	
	| T_INTERFACE	
	| T_CLASS	
	| T_CLASS_C	
	| T_TRAIT_C	
	| T_FUNC_C	
	| T_METHOD_C	
	| T_LINE	
	| T_FILE	
	| T_DIR	
	| T_NS_C
	| T_FN
	| T_MATCH
	| T_ENUM
	| T_PROPERTY_C
;

semi_reserved:
	  reserved_non_modifiers
	| T_STATIC
	| T_ABSTRACT
	| T_FINAL
	| T_PRIVATE
	| T_PROTECTED
	| T_PUBLIC
	| T_READONLY
;

ampersand:
		T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG
	|	T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG
;

identifier:
		T_STRING
	| 	semi_reserved
;

top_statement_list:
		top_statement_list top_statement	{ $$ = AddNotNull($1, $2); }
	|	/* empty */							{ $$ = NewList<LangElement>(); }
;

namespace_name:
		T_STRING								{ $$ = Add(NewList<string>(), $1); }
	|	namespace_name T_NS_SEPARATOR T_STRING	{ $$ = Add($1, $3); }
;

name:	// TODO - count as translate (use a helper object)
		namespace_name								{ $$ = new QualifiedNameRef(@$, new QualifiedName($1, true, false)); FreeList($1); }
	|	T_NAMESPACE T_NS_SEPARATOR namespace_name	{ $$ = new QualifiedNameRef(@$, MergeWithCurrentNamespace(namingContext.CurrentNamespace, $3)); FreeList($3); }
	|	T_NS_SEPARATOR namespace_name				{ $$ = new QualifiedNameRef(@$, new QualifiedName($2, true,  true)); FreeList($2); }
;

attribute_decl:
		class_name
			{ $$ = _astFactory.Attribute(@$, $1); }
	|	class_name argument_list
			{ $$ = _astFactory.Attribute(@$, $1, FinalizeCallSignature(@2, $2)); }
;

attribute_group:
		attribute_decl	{ $$ = NewList<LangElement>($1); }
	|	attribute_group ',' attribute_decl	{ $$ = AddToList($1, $3); }
;

attribute:
		T_ATTRIBUTE attribute_group possible_comma ']'	{ $$ = _astFactory.AttributeGroup(@$, GetArrayAndFree<TNode, IAttributeElement>($2)); }
;

attributes:
		attribute				{ $$ = NewList<LangElement>($1); }
	|	attributes attribute	{ $$ = AddToList($1, $2); }
;

attributed_statement:
		function_declaration_statement		{ $$ = $1; }
	|	class_declaration_statement			{ $$ = $1; }
	|	trait_declaration_statement			{ $$ = $1; }
	|	interface_declaration_statement		{ $$ = $1; }
	|	enum_declaration_statement			{ $$ = $1; }
;

top_statement:
		statement							{ $$ = $1; }
	|	attributed_statement				{ $$ = $1; }
	|	attributes attributed_statement		{ $$ = FinalizeAttributes($2, $1); }
	|	T_HALT_COMPILER '(' ')' ';'			{ $$ = _astFactory.HaltCompiler(@$); }
	|	T_NAMESPACE namespace_name ';'
		{
			AssignNamingContext();
            SetNamingContext($2);
            SetDoc($$ = _currentNamespace = (NamespaceDecl)_astFactory.Namespace(@$, namingContext.CurrentNamespace, @2, namingContext));
			FreeList($2);
		}
	|	T_NAMESPACE namespace_name backup_doc_comment { SetNamingContext($2); }
		'{' top_statement_list '}'
		{
			$$ = _astFactory.Namespace(@$, namingContext.CurrentNamespace, @2, FinalizeBlock(CombineSpans(@5, @7), $6), namingContext);
			SetDoc($$);
			ResetNamingContext(); 
			FreeList($2);
		}
	|	T_NAMESPACE backup_doc_comment { SetNamingContext(null); }
		'{' top_statement_list '}'
		{ 
			$$ = _astFactory.Namespace(@$, null, @$, FinalizeBlock(CombineSpans(@4, @6), $5), namingContext);
			SetDoc($$);
			ResetNamingContext();
		}
	|	T_USE mixed_group_use_declaration ';'		{ $$ = _astFactory.Use(@$, GetArrayAndFree($2), AliasKind.Type); _contextType = AliasKind.Type;	/* TODO: Error - must contain only a single group use */	}	
	|	T_USE use_type group_use_declaration ';'	{ $$ = _astFactory.Use(@$, GetArrayAndFree($3), $2); _contextType = AliasKind.Type;				/* TODO: Error - must contain only a single group use */	}				
	|	T_USE use_declarations ';'					{ $$ = _astFactory.Use(@$, GetArrayAndFree($2), AliasKind.Type); _contextType = AliasKind.Type;	/* TODO: Error - must contain only simple uses		  */	}	
	|	T_USE use_type use_declarations ';'			{ $$ = _astFactory.Use(@$, GetArrayAndFree($3), $2); _contextType = AliasKind.Type;				/* TODO: Error - must contain only simple uses		  */	}				
	|	T_CONST const_list ';'	
		{
			SetDoc($$ = _astFactory.DeclList(@$, PhpMemberAttributes.None, $2, null));
			FreeList($2);
		}
;

use_type:
	 	T_FUNCTION 		{ $$ = _contextType = AliasKind.Function; }
	| 	T_CONST 		{ $$ = _contextType = AliasKind.Constant; }
;

group_use_declaration:
		namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations possible_comma '}' {
			$$ = NewList<UseBase>( AddAliases(@$, $1, CombineSpans(@1, @2), AddTrailingComma($4, $5), false) );
			FreeList($1);
			FreeList($4);
		}
	|	T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations possible_comma '}' {
			$$ = NewList<UseBase>( AddAliases(@$, $2, CombineSpans(@1, @2, @3), AddTrailingComma($5, $6), true) );
			FreeList($2);
			FreeList($5);
		}
;

mixed_group_use_declaration:
		namespace_name T_NS_SEPARATOR '{' inline_use_declarations possible_comma '}' {
			$$ = NewList<UseBase>( AddAliases(@$, $1, CombineSpans(@1, @2), AddTrailingComma($4, $5), false) );
			FreeList($1);
			FreeList($4);
		}
	|	T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' inline_use_declarations possible_comma '}' {
			$$ = NewList<UseBase>( AddAliases(@$, $2, CombineSpans(@1, @2, @3), AddTrailingComma($5, $6), true) );
			FreeList($2);
			FreeList($5);
		}
;

possible_comma:
		/* empty */	{ $$ = false; }
	|	','			{ $$ = true;  }
;

inline_use_declarations:
		inline_use_declarations ',' inline_use_declaration
			{ $$ = AddToList<ContextAlias>($1, $3); }
	|	inline_use_declaration
			{ $$ = NewList<ContextAlias>( $1 ); }
;

unprefixed_use_declarations:
		unprefixed_use_declarations ',' unprefixed_use_declaration
			{ $$ = AddToList<CompleteAlias>($1, $3); }
	|	unprefixed_use_declaration
			{ $$ = NewList<CompleteAlias>( $1 ); }
;

use_declarations:
		use_declarations ',' use_declaration
			{ $$ = AddToList<UseBase>($1, AddAlias($3)); }
	|	use_declaration
			{ $$ = NewList<UseBase>( AddAlias($1) ); }
;

inline_use_declaration:
		unprefixed_use_declaration { $$ = JoinTuples(@$, $1, AliasKind.Type); }
	|	use_type unprefixed_use_declaration { $$ = JoinTuples(@$, $2, (AliasKind)$1);  }
;

unprefixed_use_declaration:
		namespace_name {
			$$ = new CompleteAlias(new QualifiedNameRef(@$, new QualifiedName($1, true, false)), NameRef.Invalid);
			FreeList($1);
		}
	|	namespace_name T_AS T_STRING {
			$$ = new CompleteAlias(new QualifiedNameRef(@1, new QualifiedName($1, true, false)), new NameRef(@3, $3));
			FreeList($1);
		}
;

use_declaration:
		unprefixed_use_declaration                
			{ $$ = $1; }
	|	T_NS_SEPARATOR unprefixed_use_declaration 
			{ 
				var src = $2;
				$$ = new CompleteAlias(
					new QualifiedNameRef(
						CombineSpans(@1, src.QualifiedName.Span),
						src.QualifiedName.QualifiedName.WithFullyQualified(true)
					),
					src.Name
				);
			}
;

const_list:
		const_list ',' const_decl { $$ = Add($1, $3); }
	|	const_decl { $$ = NewList<LangElement>( $1 ); }
;

inner_statement_list:
		inner_statement_list inner_statement	{ $$ = AddNotNull($1, $2); }
	|	/* empty */								{ $$ = NewList<LangElement>(); }
;

inner_statement:
		statement								{ $$ = $1; }
	|	attributed_statement					{ $$ = $1; }
	|	attributes attributed_statement			{ $$ = FinalizeAttributes($2, $1); }
	|	T_HALT_COMPILER '(' ')' ';'
			{ 
				$$ = _astFactory.HaltCompiler(@$); 
				_errors.Error(@$, FatalErrors.InvalidHaltCompiler); 
			}
;

statement:
		'{' inner_statement_list '}' { $$ = FinalizeBlock(@$, $2); }
	|	enter_scope if_stmt exit_scope { $$ = $2; }
	|	enter_scope alt_if_stmt exit_scope { $$ = $2; }
	|	T_WHILE '(' expr ')' enter_scope while_statement exit_scope
			{ $$ = _astFactory.While(@$, $3, CombineSpans(@2, @4), $6); }
	|	T_DO enter_scope statement T_WHILE '(' expr ')' ';' exit_scope
			{ $$ = _astFactory.Do(@$, $3, $6, CombineSpans(@5, @7), @4); }
	|	T_FOR '(' for_exprs ';' for_exprs ';' for_exprs ')' enter_scope for_statement exit_scope {
			$$ = _astFactory.For(@$, $3, $5, $7, CombineSpans(@2, @8), $10);
			FreeList($3);
			FreeList($5);
			FreeList($7);
		}
	|	T_SWITCH '(' expr ')' enter_scope switch_case_list exit_scope
			{ $$ = _astFactory.Switch(@$, $3, CombineSpans(@2, @4), $6.CaseList, $6.ClosingToken, $6.ClosingTokenSpan); }
	|	T_BREAK optional_expr ';'		{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Break, $2);}
	|	T_CONTINUE optional_expr ';'	{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Continue, $2); }
	|	T_RETURN optional_expr ';'		{ $$ = _astFactory.Jump(@$, JumpStmt.Types.Return, $2); }
	|	T_GLOBAL global_var_list ';'	{ $$ = _astFactory.Global(@$, $2); }
	|	T_STATIC static_var_list ';'	{ $$ = _astFactory.Static(@$, $2); }
	|	T_ECHO echo_expr_list ';'		{ $$ = _astFactory.Echo(@$, $2); }
	|	T_INLINE_HTML { $$ = _astFactory.InlineHtml(@$, $1); }
	|	expr ';' { $$ = _astFactory.ExpressionStmt(@$, $1); }
	|	T_UNSET '(' unset_variables possible_comma ')' ';' { $$ = _astFactory.Unset(@$, AddTrailingComma($3, $4)); }
	|	T_FOREACH '(' expr T_AS foreach_variable ')' enter_scope foreach_statement exit_scope
			{ $$ = _astFactory.Foreach(@$, $3, null, $5, $8); }
	|	T_FOREACH '(' expr T_AS foreach_variable T_DOUBLE_ARROW foreach_variable ')' enter_scope foreach_statement exit_scope
			{ $$ = _astFactory.Foreach(@$, $3, $5, $7, $10); }
	|	T_DECLARE '(' const_list ')' declare_statement
			{ $$ = _astFactory.Declare(@$, $3, $5); }
	|	';'	/* empty statement */ { $$ = _astFactory.EmptyStmt(@$); }
	|	T_TRY '{' inner_statement_list '}' enter_scope catch_list finally_statement exit_scope {
			$$ = _astFactory.TryCatch(@$, FinalizeBlock(CombineSpans(@2, @4), $3), $6, $7);
			FreeList($6);
		}
	|	T_GOTO T_STRING ';' { $$ = _astFactory.Goto(@$, $2, @2); }
	|	T_STRING ':' { $$ = _astFactory.Label(@$, $1, @1); }
;

catch_list:
		/* empty */
			{ $$ = NewList<LangElement>(); }
	|	catch_list T_CATCH '(' catch_name_list optional_variable ')' '{' inner_statement_list '}'
			{ 
				$$ = AddToList($1, _astFactory.Catch(CombineSpans(@2, @9), _astFactory.TypeReference(@4, $4), 
					(DirectVarUse)$5, FinalizeBlock(CombineSpans(@7, @9), $8))); 
			}
;

optional_variable:
		/*empty*/ { $$ = null; }
	|	T_VARIABLE { $$ = _astFactory.Variable(@1, $1, NullLangElement, true); }
;

catch_name_list:
		name { $$ = NewList<TypeRef>( CreateTypeRef($1) ); }
	|	catch_name_list '|' name { $$ = AddToList<TypeRef>($1, CreateTypeRef($3)); }
;

finally_statement:
		/* empty */ { $$ = null; }
	|	T_FINALLY '{' inner_statement_list '}' { $$ =_astFactory.Finally(@$, FinalizeBlock(CombineSpans(@2, @4), $3)); }
;

unset_variables:
		unset_variable { $$ = NewList<LangElement>( $1 ); }
	|	unset_variables ',' unset_variable { $$ = AddToList($1, $3); }
;

unset_variable:
		variable { $$ = $1; }
;

function_name:
		T_STRING
	|	T_READONLY
;

function_declaration_statement:
	function returns_ref function_name backup_doc_comment '(' parameter_list ')' return_type
	backup_fn_flags enter_scope '{' inner_statement_list '}' exit_scope backup_fn_flags
		{ 
			$$ = _astFactory.Function(@$, isConditional, $2 == (long)FormalParam.Flags.IsByRef, PhpMemberAttributes.None, $8, 
			new Name($3), @3, null, GetArrayAndFreeList($6), CombineSpans(@5, @7), 
			FinalizeBlock(CombineSpans(@11, @13), $12)); 
			SetDoc($$);
		}
;

is_reference:
		/* empty */								{ $$ = 0; }
	|	T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG	{ $$ = (long)FormalParam.Flags.IsByRef; }
;

is_variadic:
		/* empty */ { $$ = 0; }
	|	T_ELLIPSIS  { $$ = (long)FormalParam.Flags.IsVariadic; }
;

class_declaration_statement:
		class_modifiers T_CLASS T_STRING extends_from {PushClassContext($3, $4, (PhpMemberAttributes)$1);} implements_list backup_doc_comment
		enter_scope '{' class_statement_list '}' exit_scope
		{ 
			$$ = _astFactory.Type(CombineSpans(@1, @2, @11), CombineSpans(@1, @2, @3, @4, @6), isConditional, (PhpMemberAttributes)$1, new Name($3), @3, null, 
				ConvertToNamedTypeRef($4), FinalizeNamedTypeRefArray($6), $10, CombineSpans(@9, @11)); 
			SetDoc($$);
			PopClassContext();
			FreeList($10);
		}
;

class_modifiers:
		/* empty */						{ $$ = (long)PhpMemberAttributes.None; }
	|	class_modifiers class_modifier 	{
			$$ = AddModifier($1, $2, @2);
			yypos = CombineSpans(@1, @2);
		}
;

class_modifier:
		T_ABSTRACT 		{ $$ = (long)PhpMemberAttributes.Abstract; }
	|	T_FINAL 		{ $$ = (long)PhpMemberAttributes.Final; }
	|	T_READONLY		{ $$ = (long)PhpMemberAttributes.ReadOnly; }
;

trait_declaration_statement:
		T_TRAIT T_STRING {PushClassContext($2, null, PhpMemberAttributes.Trait);} backup_doc_comment
		enter_scope '{' class_statement_list '}' exit_scope
		{ 
			$$ = _astFactory.Type(@$, CombineSpans(@1, @2), isConditional, PhpMemberAttributes.Trait, 
				new Name($2), @2, null, null, EmptyArray<INamedTypeRef>.Instance, $7, CombineSpans(@6, @8)); 
			SetDoc($$);
			PopClassContext();
			FreeList($7);
		}
;

interface_declaration_statement:
		T_INTERFACE T_STRING {PushClassContext($2, null, PhpMemberAttributes.Interface);} interface_extends_list backup_doc_comment
		enter_scope '{' class_statement_list '}' exit_scope
		{ 
			$$ = _astFactory.Type(@$, CombineSpans(@1, @2, @4), isConditional, PhpMemberAttributes.Interface, 
				new Name($2), @2, null, null, FinalizeNamedTypeRefArray($4), $8, CombineSpans(@7, @9)); 
			SetDoc($$);
			PopClassContext();
			FreeList($8);
		}
;

enum_declaration_statement:
		T_ENUM T_STRING enum_backing_type extends_from {PushClassContext($2, $4, PhpMemberAttributes.Enum);} implements_list backup_doc_comment
		enter_scope '{' class_statement_list '}' exit_scope
		{ 
			$$ = _astFactory.Type(@$, CombineSpans(@1, @2, @3, @4, @6), isConditional, PhpMemberAttributes.Enum, new Name($2), @2, null, 
				ConvertToNamedTypeRef($4), FinalizeNamedTypeRefArray($6), $10, CombineSpans(@9, @11)); 
			SetDoc($$);
			SetEnumBackingType($$, $3);
			PopClassContext();
			FreeList($10);
		}
;

enum_backing_type:
		/* empty */		{ $$ = null; }
	|	':' type_expr	{ $$ = $2; }
;

enum_case:
		T_CASE identifier enum_case_expr ';'
		{
		    $$ = _astFactory.EnumCase(@$, $2, @2, $3);
		    SetMemberDoc($$);
        }
;

enum_case_expr:
		/* empty */		{ $$ = null; }
	|	'=' expr		{ $$ = $2; }
;

extends_from:
		/* empty */		{ $$ = null; }
	|	T_EXTENDS name	{ $$ = CreateTypeRef($2); }
;

interface_extends_list:
		/* empty */			{ $$ = EmptyArray<TypeRef>.Instance; }
	|	T_EXTENDS name_list { $$ = $2; }
;

implements_list:
		/* empty */				{ $$ = EmptyArray<TypeRef>.Instance; }
	|	T_IMPLEMENTS name_list	{ $$ = $2; }
;

foreach_variable:
		variable			{ $$ = _astFactory.ForeachVariable(@$, $1); }
	|	ampersand variable		{ $$ = _astFactory.ForeachVariable(@$, $2, true); }
	|	T_LIST '(' array_pair_list ')' { $$ = _astFactory.ForeachVariable(@$, _astFactory.List(@$, GetArrayAndFreeList($3), true)); }
	|	'[' array_pair_list ']' { $$ = _astFactory.ForeachVariable(@$, _astFactory.List(@$, GetArrayAndFreeList($2), false)); }
;

for_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDFOR ';' { $$ = FinalizeBlock(@1, @3, $2, Tokens.T_ENDFOR); }
;

foreach_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDFOREACH ';' { $$ = FinalizeBlock(@1, @3, $2, Tokens.T_ENDFOREACH); }
;

declare_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDDECLARE ';' { $$ = FinalizeBlock(@1, @3, $2, Tokens.T_ENDDECLARE); }
;

switch_case_list:
		'{' case_list '}'					{ $$ = $2.WithClosingToken(Tokens.T_RBRACE, @3); }
	|	'{' ';' case_list '}'				{ $$ = $3.WithClosingToken(Tokens.T_RBRACE, @4); }
	|	':' case_list T_ENDSWITCH ';'		{ $$ = $2.WithClosingToken(Tokens.T_ENDSWITCH, @3); }
	|	':' ';' case_list T_ENDSWITCH ';'	{ $$ = $3.WithClosingToken(Tokens.T_ENDSWITCH, @4); }
;

case_list:
		/* empty */ {
			$$ = new SwitchObject();
		}
	|	case_list T_CASE expr case_separator inner_statement_list {
			AddToList(
				$1.CaseList,
				_astFactory.Case(CombineSpans(@2, @3, @4, @5), $3, FinalizeCaseBlock(CombineSpans(@4, @5), $5))
			);
			$$ = $1;
		}
	|	case_list T_DEFAULT case_separator inner_statement_list {
			AddToList(
				$1.CaseList,
				_astFactory.Case(CombineSpans(@2, @3, @4), null, FinalizeCaseBlock(CombineSpans(@3, @4), $4))
			);
			$$ = $1;
		}
;

case_separator:
		':'
	|	';'
;

match:
		T_MATCH '(' expr ')' '{' match_arm_list '}'	{ $$ = (LangElement)_astFactory.Match(@$, $3, $6); }
;

match_arm_list:
		/* empty */	{ $$ = NewList<LangElement>(); }
	|	non_empty_match_arm_list possible_comma { $$ = $1; }
;

non_empty_match_arm_list:
		match_arm { $$ = NewList<LangElement>( $1 ); }
	|	non_empty_match_arm_list ',' match_arm { $$ = AddToList<LangElement>($1, $3); }
;

match_arm:
		match_arm_cond_list possible_comma T_DOUBLE_ARROW expr { $$ = (LangElement)_astFactory.MatchArm(@$, GetArrayAndFree<TNode, IExpression>($1), $4); }
	|	T_DEFAULT possible_comma T_DOUBLE_ARROW expr { $$ = (LangElement)_astFactory.MatchArm(@$, EmptyArray<IExpression>.Instance, $4); }
;

match_arm_cond_list:
		expr { $$ = NewList<LangElement>( $1 ); }
	|	match_arm_cond_list ',' expr { $$ = AddToList<LangElement>($1, $3); }
;

while_statement:
		statement { $$ = $1; }
	|	':' inner_statement_list T_ENDWHILE ';' { $$ = FinalizeBlock(@1, @3, $2, Tokens.T_ENDWHILE); }
;


if_stmt_without_else:
		T_IF '(' expr ')' statement {
			$$ = NewList<IfStatement>(
				new IfStatement(@$, $3, CombineSpans(@2, @4), $5)
			); 
		}
	|	if_stmt_without_else T_ELSEIF '(' expr ')' statement {
			$$ = AddToList<IfStatement>(
				$1, 
				new IfStatement(CombineSpans(@2, @5, @6), $4, CombineSpans(@3, @5), $6)
			);
		}
;

if_stmt:
		if_stmt_without_else %prec T_NOELSE {
			$$ = CreateIfStatement($1);
		}
	|	if_stmt_without_else T_ELSE statement {
			$$ = CreateIfStatement(
				$1,
				new IfStatement(CombineSpans(@2, @3), null, Span.Invalid, $3)
			);
		}
;

alt_if_stmt_without_else:
		T_IF '(' expr ')' ':' inner_statement_list
			{ 
				$$ = NewList<IfStatement>(
					new IfStatement(@$, $3, CombineSpans(@2, @4), $6/*List<Statement>*/)
				);
			}
			
	|	alt_if_stmt_without_else T_ELSEIF '(' expr ')' ':' inner_statement_list
			{ 
				$$ = AddToList<IfStatement>(
					FinishColonIfStatement($1, @2, Tokens.T_ELSEIF), 
					new IfStatement(CombineSpans(@2, @6, @7), $4, CombineSpans(@3, @5), $7/*List<Statement>*/)
				);
			}
;

alt_if_stmt:
		alt_if_stmt_without_else T_ENDIF ';' 
			{
				$$ = CreateIfStatement(
					FinishColonIfStatement($1, @2, Tokens.T_ENDIF)
				);
			}
	|	alt_if_stmt_without_else T_ELSE ':' inner_statement_list T_ENDIF ';'
			{
				$$ = CreateIfStatement(
					FinishColonIfStatement($1, @2, Tokens.T_ELSE),
					new IfStatement(Span.FromBounds(@2.Start, @5.Start), null, Span.Invalid, $4/*List<Statement>*/, Tokens.T_ENDIF)
				);
			}
;

parameter_list:
		non_empty_parameter_list possible_comma { $$ = AddTrailingComma($1, $2); }
	|	/* empty */	{ $$ = EmptyArray<FormalParam>.Instance; }
;

optional_parameter_list:
		/* empty */ { $$ = null; }
	|	'(' parameter_list ')' { $$ = $2; }
;


non_empty_parameter_list:
		attributed_parameter
			{ $$ = NewList<FormalParam>( (FormalParam)$1 ); }
	|	non_empty_parameter_list ',' attributed_parameter
			{ $$ = AddToList<FormalParam>($1, $3); }
;

attributed_parameter:
		attributes parameter	{
			$$ = FinalizeAttributes($2, $1);
			SetDocSpan($$);
		}
	|	parameter				{ $$ = $1; }
;

optional_property_modifiers:
		/* empty */				{ $$ = 0; /* None */ }
	|	optional_property_modifiers property_modifier {
			$$ = AddModifier($1, $2, @2) | (long)PhpMemberAttributes.Constructor;
			yypos = CombineSpans(@1, @2);
		}
;

property_modifier:
		T_PUBLIC	    { $$ = (long)PhpMemberAttributes.Public; }
	|	T_PROTECTED	    { $$ = (long)PhpMemberAttributes.Protected; }
	|	T_PRIVATE	    { $$ = (long)PhpMemberAttributes.Private; }
	|	T_READONLY	    { $$ = (long)PhpMemberAttributes.ReadOnly; }
	|	T_PUBLIC_SET	{ $$ = (long)PhpMemberAttributes.PublicSet; }
    |	T_PROTECTED_SET	{ $$ = (long)PhpMemberAttributes.ProtectedSet; }
    |	T_PRIVATE_SET	{ $$ = (long)PhpMemberAttributes.PrivateSet; }	
;

parameter:
		optional_property_modifiers optional_type_without_static is_reference is_variadic variable_init_optional optional_property_hook_list {
			/* Important - @$ is invalid when optional_type is empty */
			$$ = _astFactory.Parameter(
				CombineSpans(@1, @2, @3, @4, @5), $5.Name, $2,
				(FormalParam.Flags)$3|(FormalParam.Flags)$4,
				(Expression)$5.Value,
				(PhpMemberAttributes)$1,
				$6 /* List<LangElement> optional_property_hook_list */
			);
			SetDocSpan($$);
		}
;


optional_type:
		/* empty */	{ $$ = null; }
	|	type_expr	{ $$ = $1; }
;
optional_type_without_static:
		/* empty */	{ $$ = null; }
	|	type_expr_without_static	{ $$ = $1; }
;

type_expr:
		type		{ $$ = $1; }
	|	'?' type	{ $$ = _astFactory.NullableTypeReference(@$, $2); }
	|   union_type  { $$ = _astFactory.TypeReference(@$, $1); }
	|	intersection_type	{ $$ = _astFactory.IntersectionTypeReference(@$, $1); }
;

type_expr_without_static:
		type_without_static					{ $$ = $1; }
	|	'?' type_without_static				{ $$ = _astFactory.NullableTypeReference(@$, $2); }
	|   union_type_without_static			{ $$ = _astFactory.TypeReference(@$, $1); }
	|	intersection_type_without_static	{ $$ = _astFactory.IntersectionTypeReference(@$, $1); }
;

type:   
		type_without_static		{ $$ = $1; }
	|	T_STATIC				{ $$ = _astFactory.ReservedTypeReference(@$, _reservedTypeStatic); }
;

type_without_static:
		T_ARRAY		{ $$ = _astFactory.PrimitiveTypeReference(@$, PrimitiveTypeRef.PrimitiveType.array); }
	|	T_CALLABLE	{ $$ = _astFactory.PrimitiveTypeReference(@$, PrimitiveTypeRef.PrimitiveType.callable); }
	|	name		{ $$ = CreateTypeRef($1, true); }
;

union_type_element:
		type						{ $$ = $1; }
	|	'(' intersection_type ')'	{ $$ = _astFactory.IntersectionTypeReference(@$, $2); }
;

union_type:
		union_type_element '|' union_type_element	{ $$ = NewList<TypeRef>( $1, $3 ); }
	|	union_type '|' union_type_element			{ $$ = AddToList<TypeRef>($1, $3); }
;

union_type_without_static_element:
		type_without_static							{ $$ = $1; }
	|	'(' intersection_type_without_static ')'	{ $$ = _astFactory.IntersectionTypeReference(@$, $2); }
;

union_type_without_static:
		union_type_without_static_element '|' union_type_without_static_element	{ $$ = NewList<TypeRef>( $1, $3 ); }
	|	union_type_without_static '|' union_type_without_static_element			{ $$ = AddToList<TypeRef>($1, $3); }
;

intersection_type:
		type T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG type       { $$ = NewList<TypeRef>( $1, $3 ); }
	|	intersection_type T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG type { $$ = AddToList<TypeRef>($1, $3); }
;

intersection_type_without_static:
		type_without_static T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG type_without_static				{ $$ = NewList<TypeRef>( $1, $3 ); }
	|	intersection_type_without_static T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG type_without_static	{ $$ = AddToList<TypeRef>($1, $3); }
;

return_type:
		/* empty */	{ $$ = null; }
	|	':' type_expr	{ $$ = $2; }
;

argument_list:
		'(' ')'	{ $$ = ActualParam.EmptyArray; }
	|	'(' non_empty_argument_list possible_comma ')' { $$ = AddTrailingComma($2, $3); }
	|	'(' T_ELLIPSIS ')' { $$ = CallSignature.CreateCallableConvert(@2); }
;

non_empty_argument_list:
		argument
			{ $$ = NewList<ActualParam>( $1 ); }
	|	non_empty_argument_list ',' argument
			{ $$ = AddToList<ActualParam>($1, $3); }
;

/* `clone_argument_list` is necessary to resolve a parser ambiguity (shift-reduce conflict)
 * of `clone($expr)`, which could either be parsed as a function call with `$expr` as the first
 * argument or as a use of the `clone` language construct with an expression with useless
 * parenthesis. Both would be valid and result in the same AST / the same semantics.
 * `clone_argument_list` is defined in a way that an `expr` in the first position needs to
 * be followed by a `,` which is not valid syntax for a parenthesized `expr`, ensuring
 * that calling `clone()` with a single unnamed parameter is handled by the language construct
 * syntax.
 */
clone_argument_list:
		'(' ')'													{ $$ = ActualParam.EmptyArray; }
	|	'(' non_empty_clone_argument_list possible_comma ')'	{ $$ = AddTrailingComma($2, $3); }
	|	'(' argument_expr ',' ')'								{ $$ = AddTrailingComma(NewList<ActualParam>($2), true); }
;

non_empty_clone_argument_list:
		argument_expr ',' argument					{ $$ = NewList<ActualParam>($1, $3); }
	|	argument_no_expr							{ $$ = NewList<ActualParam>( $1 ); }
	|	non_empty_clone_argument_list ',' argument	{ $$ = AddToList<ActualParam>($1, $3); }
;

argument_no_expr:
		identifier ':' expr { $$ = _astFactory.ActualParameter(@$, $3, ActualParam.Flags.Default, new VariableNameRef(@1, $1)); }
	|	T_ELLIPSIS expr	{ $$ = _astFactory.ActualParameter(@$, $2, ActualParam.Flags.IsUnpack); }
;

argument_expr:
		expr			{ $$ = _astFactory.ActualParameter(@$, $1); }
;

argument:
		argument_expr		{ $$ = $1; }
	|	argument_no_expr	{ $$ = $1; }
;

global_var_list:
		global_var_list ',' global_var { $$ = AddToList<LangElement>($1, $3); }
	|	global_var { $$ = NewList<LangElement>( $1 ); }
;

global_var:
	simple_variable
		{ $$ = $1; }
;


static_var_list:
		static_var_list ',' static_var { $$ = AddToList<LangElement>($1, $3); }
	|	static_var { $$ = NewList<LangElement>( $1 ); }
;

static_var:
		variable_init_optional { $$ = _astFactory.StaticVarDecl(@$, $1.Name, (Expression)$1.Value); }
;


class_statement_list:
		class_statement_list class_statement
			{ $$ = AddToList<LangElement>($1, $2); }
	|	/* empty */
			{ $$ = NewList<LangElement>(); }
;

attributed_class_statement:
		variable_modifiers optional_type_without_static property_list ';'
			{ 
				$$ = _astFactory.DeclList(@$, (PhpMemberAttributes)$1, $3, $2); 
				SetDoc($$);
				FreeList($3);
			}
	|	method_modifiers T_CONST class_const_list ';'
			{ 
				$$ = _astFactory.DeclList(@$, (PhpMemberAttributes)$1, $3, null); 
				SetDoc($$);
				FreeList($3);
			}
	|	method_modifiers T_CONST type_expr class_const_list ';'
			{ 
				$$ = _astFactory.DeclList(@$, (PhpMemberAttributes)$1, $4, $3); 
				SetDoc($$);
				FreeList($4);
			}
	|	method_modifiers function returns_ref identifier backup_doc_comment '(' parameter_list ')'
		return_type backup_fn_flags method_body backup_fn_flags
			{	
				$$ = _astFactory.Method(@$, $3 == (long)FormalParam.Flags.IsByRef, (PhpMemberAttributes)$1, 
					$9, @9, $4, @4, null, GetArrayAndFreeList($7), CombineSpans(@6, @8), null, $11);
				SetDoc($$);
			}
	|	enum_case { $$ = $1; }
	|	hooked_property { $$ = $1; }
;

class_statement:
		attributed_class_statement				{ $$ = $1; }
	|   attributes attributed_class_statement	{ $$ = FinalizeAttributes($2, $1); }
	|	T_USE name_list trait_adaptations {
			$$ = _astFactory.TraitUse(@$, $2, $3);
			SetDoc($$);
		}
;

name_list:
		name { $$ = NewList<TypeRef>( CreateTypeRef($1) ); }
	|	name_list ',' name { $$ = AddToList<TypeRef>($1, CreateTypeRef($3)); }
;

trait_adaptations:
		';'								{ $$ = null; }
	|	'{' '}'							{ $$ = _astFactory.TraitAdaptationBlock(@$, EmptyArray<LangElement>.Instance); }
	|	'{' trait_adaptation_list '}'	{ $$ = _astFactory.TraitAdaptationBlock(@$, $2); }
;

trait_adaptation_list:
		trait_adaptation
			{ $$ = NewList<LangElement>( $1 );
 }
	|	trait_adaptation_list trait_adaptation
			{ $$ = AddToList<LangElement>($1, $2); }
;

trait_adaptation:
		trait_precedence	{ $$ = $1; }
	|	trait_alias			{ $$ = $1; }
;

trait_precedence:
	absolute_trait_method_reference T_INSTEADOF name_list ';'
		{ $$ = _astFactory.TraitAdaptationPrecedence(@$, (Tuple<TypeRef,NameRef>)$1, $3); }
;

trait_alias:
		trait_method_reference T_AS T_STRING ';'
			{ $$ = _astFactory.TraitAdaptationAlias(@$, (Tuple<TypeRef,NameRef>)$1, new NameRef(@3, $3), null); }
	|	trait_method_reference T_AS reserved_non_modifiers ';'
			{ $$ = _astFactory.TraitAdaptationAlias(@$, (Tuple<TypeRef,NameRef>)$1, new NameRef(@3, $3), null); }
	|	trait_method_reference T_AS member_modifier identifier ';'
			{ 
				$$ = _astFactory.TraitAdaptationAlias(@$, (Tuple<TypeRef,NameRef>)$1, new NameRef(@4, $4), (PhpMemberAttributes)$3); 
			}
	|	trait_method_reference T_AS member_modifier ';'
			{ 
				$$ = _astFactory.TraitAdaptationAlias(@$, (Tuple<TypeRef,NameRef>)$1, NameRef.Invalid, (PhpMemberAttributes)$3); 
			}
;

trait_method_reference:
		identifier						{ $$ = new Tuple<TypeRef,NameRef>(null, new NameRef(@1, $1)); }
	|	absolute_trait_method_reference	{ $$ = $1; }
;

absolute_trait_method_reference:
	name T_DOUBLE_COLON identifier
		{ $$ = new Tuple<TypeRef,NameRef>(CreateTypeRef($1), new NameRef(@3, $3)); }
;

method_body:
		';' /* abstract method */		{ $$ = null; }
	|	'{' inner_statement_list '}'	{ $$ = FinalizeBlock(CombineSpans(@1, @3), $2); }
;

variable_modifiers:
		non_empty_member_modifiers		{ $$ = $1; }
	|	T_VAR							{ $$ = (long)PhpMemberAttributes.None; }
;

method_modifiers:
		/* empty */						{ $$ = (long)PhpMemberAttributes.None; }
	|	non_empty_member_modifiers		{ $$ = $1; }
;

non_empty_member_modifiers:
		member_modifier			{ $$ = $1; }
	|	non_empty_member_modifiers member_modifier { $$ = AddModifier($1, $2, @2); }
;

member_modifier:
		T_PUBLIC				{ $$ = (long)PhpMemberAttributes.Public; }
	|	T_PROTECTED				{ $$ = (long)PhpMemberAttributes.Protected; }
	|	T_PRIVATE				{ $$ = (long)PhpMemberAttributes.Private; }
	|	T_PUBLIC_SET			{ $$ = (long)PhpMemberAttributes.PublicSet; }
	|	T_PROTECTED_SET			{ $$ = (long)PhpMemberAttributes.ProtectedSet; }
	|	T_PRIVATE_SET			{ $$ = (long)PhpMemberAttributes.PrivateSet; }
	|	T_STATIC				{ $$ = (long)PhpMemberAttributes.Static; }
	|	T_ABSTRACT				{ $$ = (long)PhpMemberAttributes.Abstract; }
	|	T_FINAL					{ $$ = (long)PhpMemberAttributes.Final; }
	|	T_READONLY				{ $$ = (long)PhpMemberAttributes.ReadOnly; }
;

variable_init_optional:
		T_VARIABLE '=' expr		{ $$ = new VariableNameValue(@1, $1, $3); }
	|	T_VARIABLE				{ $$ = new VariableNameValue(@1, $1, null); }
;

property_list:
		property_list ',' property { $$ = AddToList<LangElement>($1, $3); }
	|	property { $$ = NewList<LangElement>( $1 ); }
;

property:
		variable_init_optional backup_doc_comment	{ SetMemberDoc($$ = _astFactory.FieldDecl(@$, $1.Name.Name, (Expression)$1.Value)); }
;

hooked_property:
		variable_modifiers optional_type_without_static variable_init_optional backup_doc_comment '{' property_hook_list '}'
		{
			$$ = _astFactory.PropertyDecl(@$, (PhpMemberAttributes)$1, $2, $3.Name, $6, $3.Value); 
			SetDoc($$);
			FreeList($6);
		}
;

property_hook_list:
		/* empty */
		{
			$$ = NewList<LangElement>();
		}
	|	property_hook_list property_hook
		{
			$$ = AddToList<LangElement>( $1, $2 );
		}
	|	property_hook_list attributes property_hook
		{
			$$ = AddToList<LangElement>( $1, FinalizeAttributes( $3, $2 ) );
		}
;

optional_property_hook_list:
		/* empty */					{ $$ = null; }
	|	'{' property_hook_list '}'	{ $$ = $2; }
;

property_hook_modifiers:
		/* empty */						{ $$ = (long)PhpMemberAttributes.None; }
	|	non_empty_member_modifiers		{ $$ = $1; }
;

property_hook:
		property_hook_modifiers returns_ref T_STRING
		backup_doc_comment
		optional_parameter_list backup_fn_flags property_hook_body backup_fn_flags
		{
			$$ = _astFactory.PropertyHook(
				@$,
				$2 == (long)FormalParam.Flags.IsByRef,
				(PhpMemberAttributes)$1,
				new Name($3), @3,
				GetArrayAndFreeList($5), @5,
				$7
			);
			SetDoc($$);
		}
;

property_hook_body:
		';'								{ $$ = null; }
	|	'{' inner_statement_list '}'	{ $$ = FinalizeBlock(CombineSpans(@1, @3), $2); }
	|	T_DOUBLE_ARROW expr ';'			{ $$ = $2; }
;

class_const_list:
		class_const_list ',' class_const_decl { $$ = AddToList<LangElement>($1, $3); }
	|	class_const_decl { $$ = NewList<LangElement>( $1 ); }
;

class_const_decl:
	identifier '=' expr backup_doc_comment {
		$$ = _astFactory.ClassConstDecl(@$, new VariableName($1), @1, $3); 
		SetMemberDoc($$);
	}
;

const_decl:
	T_STRING '=' expr backup_doc_comment { $$ = _astFactory.GlobalConstDecl(@$, false, new VariableName($1), @1, $3); 
		SetMemberDoc($$);
	}
;

echo_expr_list:
		echo_expr_list ',' echo_expr { $$ = AddToList<LangElement>($1, $3); }
	|	echo_expr { $$ = NewList<LangElement>( $1 ); }
;
echo_expr:
	expr { $$ = $1; }
;

for_exprs:
		/* empty */			{ $$ = NewList<LangElement>(); }
	|	non_empty_for_exprs	{ $$ = $1; }
;

non_empty_for_exprs:
		non_empty_for_exprs ',' expr { $$ = AddToList<LangElement>($1, $3); }
	|	expr { $$ = NewList<LangElement>( $1 ); }
;

anonymous_class:
        class_modifiers T_CLASS ctor_arguments extends_from { PushAnonymousClassContext($4); } implements_list backup_doc_comment
		enter_scope '{' class_statement_list '}' exit_scope
		{
			var typeRef = _astFactory.AnonymousTypeReference(
				CombineSpans(@1, @2, @11),
				CombineSpans(@1, @2, @3, @4, @6),
				isConditional,
				(PhpMemberAttributes)$1,
				null,
				ConvertToNamedTypeRef($4),
				ConvertToNamedTypeRef($6),
				$10,
				CombineSpans(@9, @11)
			);
			SetDoc(((AnonymousTypeRef)typeRef).TypeDeclaration);
			$$ = new AnonymousClass(typeRef, GetArrayAndFreeList($3), @3);
			PopClassContext();
			FreeList($10);
		}
;

new_dereferenceable:
		T_NEW class_name_reference argument_list
			{ $$ = _astFactory.New(@$, $2, GetArrayAndFreeList($3), @3); }
	|	T_NEW anonymous_class
			{ $$ = _astFactory.New(@$, $2.TypeRef, $2.ActualParams, $2.Span); }
	|	T_NEW attributes anonymous_class {
			var x = $3;
			FinalizeAttributes(((AnonymousTypeRef)x.TypeRef).TypeDeclaration, $2);
			$$ = _astFactory.New(@$, x.TypeRef, x.ActualParams, x.Span);
		}
;

new_non_dereferenceable:
		T_NEW class_name_reference
			{ $$ = _astFactory.New(@$, $2, null, Span.Invalid); }
;

expr:
		variable
			{ $$ = $1; }
	|	T_LIST '(' array_pair_list ')' '=' expr
			{ $$ = _astFactory.Assignment(@$, _astFactory.List(Span.Combine(@1, @4), GetArrayAndFreeList($3), true), $6, Operations.AssignValue); }
	|	'[' array_pair_list ']' '=' expr
			{ $$ = _astFactory.Assignment(@$, _astFactory.List(CombineSpans(@1, @3), GetArrayAndFreeList($2), false), $5, Operations.AssignValue); }
	|	variable '=' expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignValue); }
	|	variable '=' ampersand variable
			{ $$ = _astFactory.Assignment(@$, $1, $4, Operations.AssignRef); }
	|	variable '=' ampersand new_dereferenceable
			{ $$ = _astFactory.Assignment(@$, $1, $4, Operations.AssignRef); _errors.Error(@$, Warnings.AssignNewByRefDeprecated); }
	|	variable '=' ampersand new_non_dereferenceable
			{ $$ = _astFactory.Assignment(@$, $1, $4, Operations.AssignRef); _errors.Error(@$, Warnings.AssignNewByRefDeprecated); }
	|	T_CLONE clone_argument_list
			{ $$ = _astFactory.Clone(@$, FinalizeCallSignature(@2, $2)); }
	|	T_CLONE '(' T_ELLIPSIS ')'
			{ $$ = _astFactory.Clone(@$, FinalizeCallSignature(CombineSpans(@2, @4), CallSignature.CreateCallableConvert(@3))); }
	|	T_CLONE expr
			{ $$ = _astFactory.Clone(@$, FinalizeCallSignature(Span.Invalid, new ActualParam[]{ _astFactory.ActualParameter(@2, $2) })); }
	|	variable T_PLUS_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignAdd); }
	|	variable T_MINUS_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignSub); }
	|	variable T_MUL_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignMul); }
	|	variable T_POW_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignPow); }
	|	variable T_DIV_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignDiv); }
	|	variable T_CONCAT_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignAppend); }
	|	variable T_MOD_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignMod); }
	|	variable T_AND_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignAnd); }
	|	variable T_OR_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignOr); }
	|	variable T_XOR_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignXor); }
	|	variable T_SL_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignShiftLeft); }
	|	variable T_SR_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignShiftRight); }
	|	variable T_COALESCE_EQUAL expr
			{ $$ = _astFactory.Assignment(@$, $1, $3, Operations.AssignCoalesce); }
	|	variable T_INC { $$ = CreateIncDec(@$, $1, true, true); }
	|	T_INC variable { $$ = CreateIncDec(@$, $2, true, false); }
	|	variable T_DEC { $$ = CreateIncDec(@$, $1, false, true); }
	|	T_DEC variable { $$ = CreateIncDec(@$, $2, false, false); }
	|	expr T_BOOLEAN_OR expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_BOOLEAN_OR,   $1, $3); }
	|	expr T_BOOLEAN_AND expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_BOOLEAN_AND,  $1, $3); }
	|	expr T_LOGICAL_OR expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_LOGICAL_OR,   $1, $3); }
	|	expr T_LOGICAL_AND expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_LOGICAL_AND,  $1, $3); }
	|	expr T_LOGICAL_XOR expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_LOGICAL_XOR,  $1, $3); }
	|	expr '|' expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_PIPE,  $1, $3); }
	|	expr T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG expr	{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG, $1, $3); }
	|	expr T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG, $1, $3); }
	|	expr '^' expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_CARET, $1, $3); }
	|	expr '.' expr 				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_DOT, $1, $3); }
	|	expr '+' expr 				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_PLUS,    $1, $3); }
	|	expr '-' expr 				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_MINUS,    $1, $3); }
	|	expr '*' expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_MUL,    $1, $3); }
	|	expr T_POW expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_POW,    $1, $3); }
	|	expr '/' expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_SLASH,    $1, $3); }
	|	expr '%' expr 				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_PERCENT,    $1, $3); }
	| 	expr T_SL expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_SL,  $1, $3); } 
	|	expr T_SR expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_SR, $1, $3); }
	|	expr T_IS_IDENTICAL expr	{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_IS_IDENTICAL, $1, $3); }
	|	expr T_IS_NOT_IDENTICAL expr{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_IS_NOT_IDENTICAL, $1, $3); }
	|	expr T_IS_EQUAL expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_IS_EQUAL, $1, $3); }
	|	expr T_IS_NOT_EQUAL expr	{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_IS_NOT_EQUAL, $1, $3); }
	|	expr T_PIPE_OPERATOR expr	{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_PIPE_OPERATOR, $1, $3); }
	|	expr '<' expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_LT, $1, $3); }
	|	expr T_IS_SMALLER_OR_EQUAL expr	{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_IS_SMALLER_OR_EQUAL, $1, $3); }
	|	expr '>' expr				{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_GT, $1, $3); }
	|	expr T_IS_GREATER_OR_EQUAL expr	{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_IS_GREATER_OR_EQUAL, $1, $3); }
	|	expr T_SPACESHIP expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_SPACESHIP, $1, $3); }
	|	expr T_COALESCE expr		{ $$ = _astFactory.BinaryOperation(@$, Tokens.T_COALESCE, $1, $3); }

	|	'+' expr %prec T_INC { $$ = _astFactory.UnaryOperation(@$, Operations.Plus,   (Expression)$2); }
	|	'-' expr %prec T_INC { $$ = _astFactory.UnaryOperation(@$, Operations.Minus,   (Expression)$2); }
	|	'!' expr { $$ = _astFactory.UnaryOperation(@$, Operations.LogicNegation, (Expression)$2); }
	|	'~' expr { $$ = _astFactory.UnaryOperation(@$, Operations.BitNegation,   (Expression)$2); }
	|	expr T_INSTANCEOF class_name_reference
			{ $$ = _astFactory.InstanceOf(@$, $1, $3); }
	|	'(' expr ')' { $$ = _astFactory.EncapsedExpression(@$, $2, Tokens.T_LPAREN); }
	|	new_dereferenceable { $$ = $1; }
	|	new_non_dereferenceable { $$ = $1; }
	|	expr '?' expr ':' expr
			{ $$ = _astFactory.ConditionalEx(@$, $1, $3, $5); }
	|	expr '?' ':' expr
			{ $$ = _astFactory.ConditionalEx(@$, $1, null, $4); }
	|	internal_functions_in_yacc { $$ = $1; }
	|	T_INT_CAST expr		{ $$ = _astFactory.UnaryOperation(@$, Operations.Int64Cast,   (Expression)$2); }
	|	T_DOUBLE_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.DoubleCast, (Expression)$2); }
	|	T_STRING_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.StringCast, (Expression)$2); }
	|	T_ARRAY_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.ArrayCast,  (Expression)$2); } 
	|	T_OBJECT_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.ObjectCast, (Expression)$2); }
	|	T_BOOL_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.BoolCast,   (Expression)$2); }
	|	T_UNSET_CAST expr	{ $$ = _astFactory.UnaryOperation(@$, Operations.UnsetCast,  (Expression)$2); }
	|	T_EXIT exit_expr	{ $$ = _astFactory.Exit(@$, $2); }
	|	'@' expr			{ $$ = _astFactory.UnaryOperation(@$, Operations.AtSign,     (Expression)$2); }
	|	scalar { $$ = $1; }
	|	backticks_expr { $$ = _astFactory.Shell(@$, $1); }
	|	T_PRINT expr { $$ = _astFactory.UnaryOperation(@$, Operations.Print, (Expression)$2); }
	|	T_YIELD { $$ = _astFactory.Yield(@$, null, null); }
	|	T_YIELD expr { $$ = _astFactory.Yield(@$, null, $2); }
	|	T_YIELD expr T_DOUBLE_ARROW expr { $$ = _astFactory.Yield(@$, $2, $4); }
	|	T_YIELD_FROM expr { $$ = _astFactory.YieldFrom(@$, $2); }
	|	T_THROW expr { $$ = _astFactory.Throw(@$, $2); }
	|	inline_function { $$ = $1; }
	|	attributes inline_function { $$ = FinalizeAttributes($2, $1); }
	|	T_STATIC inline_function {
			var lambda = (LambdaFunctionExpr)$2;
			lambda.IsStatic = true;
			lambda.ChangeSpan(@$); // lambda.Span = @$;
			$$ = lambda;
		}
	|	attributes T_STATIC inline_function {
			var lambda = (LambdaFunctionExpr)$3;
			lambda.IsStatic = true;
			lambda.ChangeSpan(@$); // lambda.Span = @$;
			$$ = FinalizeAttributes(lambda, $1);
		}
	|	match { $$ = $1; }
;

inline_function:
		function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars return_type
		backup_fn_flags enter_scope '{' inner_statement_list '}' exit_scope backup_fn_flags
			{ 
				$$ = _astFactory.Lambda(@$, CombineSpans(@1, @6, @7, @8), $2 == (long)FormalParam.Flags.IsByRef, $8, 
					GetArrayAndFreeList($5), CombineSpans(@4, @6), GetArrayAndFreeList($7), FinalizeBlock(CombineSpans(@11, @13), $12)); 
				SetDoc($$);
			}
	|	fn returns_ref  '(' parameter_list ')' return_type backup_doc_comment T_DOUBLE_ARROW backup_fn_flags backup_lex_pos expr backup_fn_flags
			{
				$$ = _astFactory.ArrowFunc(@$, CombineSpans(@1, @5, @6), $2 == (long)FormalParam.Flags.IsByRef, $6, 
					GetArrayAndFreeList($4), CombineSpans(@3, @5), $11); 
				SetDoc($$);
			}
;

fn:
	T_FN
;

function:
	T_FUNCTION
;

backup_doc_comment:
	/* empty */ { }
;

enter_scope:
	/* empty */ { _currentScope.Increment(); }
;

exit_scope:
	/* empty */ { _currentScope.Decrement(); }
;

backup_fn_flags:
	%prec PREC_ARROW_FUNCTION /* empty */ {  }
;

backup_lex_pos:
	/* empty */ {  }
;

returns_ref:
		/* empty */	{ $$ = 0; }
	|	ampersand	{ $$ = (long)FormalParam.Flags.IsByRef; }
;

lexical_vars:
		/* empty */ { $$ = EmptyArray<FormalParam>.Instance; }
	|	T_USE '(' lexical_var_list possible_comma ')' { $$ = AddTrailingComma($3, $4); }
;

lexical_var_list:
		lexical_var_list ',' lexical_var { $$ = AddToList<FormalParam>($1, $3); }
	|	lexical_var { $$ = NewList<FormalParam>( (FormalParam)$1 ); }
;

lexical_var:
		T_VARIABLE {
			$$ = _astFactory.Parameter(@$, new VariableNameRef(@1, $1), null, FormalParam.Flags.Default);
			SetDoc($$);
		}
	|	ampersand T_VARIABLE {
			$$ = _astFactory.Parameter(@$, new VariableNameRef(@2, $2), null, FormalParam.Flags.IsByRef);
			SetDoc($$);
		}
;

function_call:
		name argument_list
			{ $$ = _astFactory.Call(@$, TranslateQNRFunction($1), FinalizeCallSignature(@2, $2), null); }
	|	class_name T_DOUBLE_COLON member_name argument_list
			{
				if ($3 is string namestr)
					$$ = _astFactory.Call(@$, new Name(namestr), @3, FinalizeCallSignature(@4, $4), $1); 
				else
					$$ = _astFactory.Call(@$, (LangElement)$3, FinalizeCallSignature(@4, $4), $1); 
			}
	|	variable_class_name T_DOUBLE_COLON member_name argument_list
			{
				if ($3 is string namestr)
					$$ = _astFactory.Call(@$, new Name(namestr), @3, FinalizeCallSignature(@4, $4), _astFactory.TypeReference(@1, $1)); 
				else
					$$ = _astFactory.Call(@$, (LangElement)$3, FinalizeCallSignature(@4, $4), _astFactory.TypeReference(@1, $1)); 
			}
	|	callable_expr argument_list
			{ $$ = _astFactory.Call(@$, $1, FinalizeCallSignature(@2, $2), NullLangElement);}
;

class_name:
		T_STATIC	{ $$ = _astFactory.ReservedTypeReference(@$, _reservedTypeStatic); }
	|	name		{ $$ = CreateTypeRef($1); }
;

class_name_reference:
		class_name		{ $$ = $1; }
	|	new_variable	{ $$ = _astFactory.TypeReference(@$, $1); }
	|	'(' expr ')'	{ $$ = _astFactory.TypeReference(@$, _astFactory.EncapsedExpression(@$, $2, Tokens.T_LPAREN)); }
;

exit_expr:
		/* empty */				{ $$ = null; }
	|	'(' optional_expr ')'	{ $$ = $2 == null? null: _astFactory.EncapsedExpression(@$, $2, Tokens.T_LPAREN); }
;

backticks_expr:
		'`' '`' { $$ = _astFactory.Literal(@$, string.Empty, "``".AsSpan()); }
	|	'`' T_ENCAPSED_AND_WHITESPACE '`' { $$ = _astFactory.Literal(@$, $2.Text, string.Format("`{0}`", $2.SourceCode).AsSpan()); }
	|	'`' encaps_list '`' { $$ = _astFactory.StringEncapsedExpression(@$, _astFactory.Concat(@2, $2), Tokens.T_BACKQUOTE); }
;

ctor_arguments:
		/* empty */	{ $$ = EmptyArray<ActualParam>.Instance; }
	|	argument_list { $$ = $1; }
;


dereferencable_scalar:
		T_ARRAY '(' array_pair_list ')'	{ $$ = _astFactory.NewArray(@$, GetArrayAndFreeList($3), true); }
	|	'[' array_pair_list ']'			{ $$ = _astFactory.NewArray(@$, GetArrayAndFreeList($2), false); }
	|	T_CONSTANT_ENCAPSED_STRING		{ $$ = _astFactory.Literal(@$, $1, _lexer.TokenTextSpan); }
	|	'"' encaps_list '"' 	{ $$ = _astFactory.StringEncapsedExpression(@$, _astFactory.Concat(@2, $2), Tokens.T_DOUBLE_QUOTES); }
;

scalar:
		T_LNUMBER 	{ $$ = _astFactory.Literal(@$, $1, _lexer.TokenTextSpan); }
	|	T_DNUMBER 	{ $$ = _astFactory.Literal(@$, $1, _lexer.TokenTextSpan); }
	|	T_LINE 		{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Line); }
	|	T_FILE 		{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.File); }
	|	T_DIR   	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Dir); }
	|	T_TRAIT_C	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Trait); }
	|	T_METHOD_C	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Method); }
	|	T_PROPERTY_C{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Property); }
	|	T_FUNC_C	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Function); }
	|	T_NS_C		{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Namespace); }
	|	T_CLASS_C	{ $$ = _astFactory.PseudoConstUse(@$, PseudoConstUse.Types.Class); }
	|	T_START_HEREDOC T_END_HEREDOC							{ $$ = _astFactory.HeredocExpression(@$, _astFactory.Literal(new Span(@1.End, 0), "", string.Empty.AsSpan()), $1.QuoteToken, $2); }
	|	T_START_HEREDOC T_ENCAPSED_AND_WHITESPACE T_END_HEREDOC { $$ = _astFactory.HeredocExpression(@$, RemoveHereDocIndentation(_astFactory.Literal(@2, $2.Text, $2.SourceCode.AsSpan()), $3, true), $1.QuoteToken, $3); }
	|	T_START_HEREDOC encaps_list T_END_HEREDOC				{ $$ = _astFactory.HeredocExpression(@$, RemoveHereDocIndentation(_astFactory.Concat(@2, $2), $3, true), $1.QuoteToken, $3); }
	|	dereferencable_scalar	{ $$ = $1; }
	|	constant				{ $$ = $1; }
	|	class_constant			{ $$ = $1; }
;

constant:
		name	{ $$ = _astFactory.ConstUse(@$, TranslateQNRConstant($1)); }
;

class_constant:
		class_name T_DOUBLE_COLON identifier			{ $$ = _astFactory.ClassConstUse(@$, $1, new Name($3), @3); }
	|	variable_class_name T_DOUBLE_COLON identifier	{ $$ = _astFactory.ClassConstUse(@$, _astFactory.TypeReference(@1, $1), new Name($3), @3); }
	|	class_name T_DOUBLE_COLON  '{' expr '}'				{ $$ = _astFactory.ClassConstUse(@$, $1, $4); }
	|	variable_class_name T_DOUBLE_COLON  '{' expr '}'	{ $$ = _astFactory.ClassConstUse(@$, _astFactory.TypeReference(@1, $1), $4); }
;

optional_expr:
		/* empty */	{ $$ = null; }
	|	expr		{ $$ = $1; }
;

object_operator:
		T_OBJECT_OPERATOR			{ $$ = Tokens.T_OBJECT_OPERATOR; }
	|	T_NULLSAFE_OBJECT_OPERATOR	{ $$ = Tokens.T_NULLSAFE_OBJECT_OPERATOR; }
;

variable_class_name:
	dereferencable { $$ = $1; /* TODO if (!($1 is VarLikeConstructUse)) _errors.Error(@$, FatalErrors.CheckVarUseFault); */ }
;

dereferencable:
		variable				{ $$ = $1; }
	|	'(' expr ')'			{ $$ = _astFactory.EncapsedExpression(@$, $2, Tokens.T_LPAREN); } 
	|	dereferencable_scalar	{ $$ = $1; }
	|	class_constant			{ $$ = $1; }
	|	new_dereferenceable		{ $$ = $1; }
;

array_object_dereferenceable:
		dereferencable			{ $$ = $1; }
	|	constant				{ $$ = $1; }
;

callable_expr:
		callable_variable		{ $$ = $1; }
	|	'(' expr ')'			{ $$ = _astFactory.EncapsedExpression(@$, $2, Tokens.T_LPAREN); }
	|	dereferencable_scalar	{ $$ = $1; }
	|	new_dereferenceable		{ $$ = $1; }
;

callable_variable:
		simple_variable
			{ $$ = $1; }
	|	array_object_dereferenceable '[' optional_expr ']'
			{ $$ = _astFactory.ArrayItem(@$, false, $1, $3); }
	|	array_object_dereferenceable object_operator property_name argument_list
		{
			if ($3 is string name)
				$$ = _astFactory.Call(@$, new TranslatedQualifiedName(new QualifiedName(new Name(name)), @3), FinalizeCallSignature(@4, $4), VerifyMemberOf($1));
			else
				$$ = _astFactory.Call(@$, (LangElement)$3, FinalizeCallSignature(@4, $4), VerifyMemberOf($1));

			AdjustNullSafeOperator($$, $2);
		}
	|	function_call { $$ = $1; }
;

variable:
		callable_variable	{ $$ = $1; }
	|	static_member		{ $$ = $1; }
	|	array_object_dereferenceable object_operator property_name
		{
			$$ = AdjustNullSafeOperator(CreateProperty(@$, $1, $3), $2);
		}
;

simple_variable:
		T_VARIABLE			{ $$ = _astFactory.Variable(@$, $1,	NullLangElement, true); }
	|	'$' '{' expr '}'	{ $$ = _astFactory.Variable(@$, _astFactory.EncapsedExpression(Span.Combine(@2, @4), $3, Tokens.T_LBRACE), NullLangElement); }
	|	'$' simple_variable	{ $$ = _astFactory.Variable(@$, $2, NullLangElement); }
;

static_member:
		class_name T_DOUBLE_COLON simple_variable
			{ $$ = CreateStaticProperty(@$, $1, $3); }	
	|	variable_class_name T_DOUBLE_COLON simple_variable
			{ $$ = CreateStaticProperty(@$, $1, @1, $3); }
;

new_variable:
		simple_variable
			{ $$ = $1; }
	|	new_variable '[' optional_expr ']'
			{ $$ = _astFactory.ArrayItem(@$, false, $1, $3); }
	|	new_variable object_operator property_name
			{ $$ = AdjustNullSafeOperator(CreateProperty(@$, $1, $3), $2); }
	|	class_name T_DOUBLE_COLON simple_variable
			{ $$ = CreateStaticProperty(@$, $1, $3); }
	|	new_variable T_DOUBLE_COLON simple_variable
			{ $$ = CreateStaticProperty(@$, $1, @1, $3); }
;

member_name:
		identifier { $$ = $1; }
	|	'{' expr '}'	{ $$ = _astFactory.EncapsedExpression(@$, $2, Tokens.T_LBRACE); }
	|	simple_variable	{ $$ = $1; }
;

property_name:
		T_STRING { $$ = $1; }
	|	'{' expr '}'	{ $$ = _astFactory.EncapsedExpression(@$, $2, Tokens.T_LBRACE); }
	|	simple_variable	{ $$ = $1; }
;

array_pair_list:
		non_empty_array_pair_list { $$ = $1;  }
;

possible_array_pair:
		/* empty */ { $$ = default(ArrayItem); }
	|	array_pair  { $$ = $1; }
;

non_empty_array_pair_list:
		non_empty_array_pair_list ',' possible_array_pair
			{ $$ = AddToList<ArrayItem>($1, $3); }
	|	possible_array_pair
			{ $$ = NewList<ArrayItem>( $1 ); }
;

array_pair:
		expr T_DOUBLE_ARROW expr
			{ $$ = _astFactory.ArrayItemValue(@$, $1, $3); }
	|	expr
			{ $$ = _astFactory.ArrayItemValue(@$, null, $1); }
	|	expr T_DOUBLE_ARROW ampersand variable
			{ $$ = _astFactory.ArrayItemRef(@$, $1, $4); }
	|	ampersand variable
			{ $$ = _astFactory.ArrayItemRef(@$, null, $2); }
	|	T_ELLIPSIS expr
			{ $$ = _astFactory.ArrayItemSpread(@$, $2); }
	|	expr T_DOUBLE_ARROW T_LIST '(' array_pair_list ')'
			{ $$ = _astFactory.ArrayItemValue(@$, $1, _astFactory.List(Span.Combine(@3, @6), GetArrayAndFreeList($5), true)); }
	|	T_LIST '(' array_pair_list ')'
			{ $$ = _astFactory.ArrayItemValue(@$, null, _astFactory.List(Span.Combine(@1, @4), GetArrayAndFreeList($3), true)); }
;

encaps_list:
		encaps_list encaps_var
			{ $$ = AddToList<LangElement>($1, $2); }
	|	encaps_list T_ENCAPSED_AND_WHITESPACE
			{ $$ = AddToList<LangElement>($1, _astFactory.Literal(@2, $2.Text, _lexer.TokenTextSpan)); }
	|	encaps_var
			{ $$ = NewList<LangElement>( $1 ); }
	|	T_ENCAPSED_AND_WHITESPACE encaps_var
			{ $$ = NewList<LangElement>( _astFactory.Literal(@1, $1.Text, $1.SourceCode.AsSpan()), $2 ); }
;

encaps_var:
		T_VARIABLE
			{ $$ = _astFactory.Variable(@$, $1, NullLangElement, true); }
	|	T_VARIABLE '[' encaps_var_offset ']'
			{ $$ = _astFactory.ArrayItem(@$, false,
					_astFactory.Variable(@1, $1, NullLangElement, true), $3); }
	|	T_VARIABLE object_operator T_STRING
			{ $$ = AdjustNullSafeOperator(CreateProperty(@$, _astFactory.Variable(@1, $1, NullLangElement, true), $3), $2); }
	|	T_DOLLAR_OPEN_CURLY_BRACES expr '}'
			{ $$ = _astFactory.EncapsedExpression(@$, _astFactory.Variable(@2, $2, NullLangElement), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
	|	T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '}'
			{ $$ = _astFactory.EncapsedExpression(@$, _astFactory.Variable(@2, $2, NullLangElement, false), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
	|	T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '[' expr ']' '}'
			{ $$ = _astFactory.EncapsedExpression(@$, _astFactory.ArrayItem(Span.Combine(@2, @5), false,
					_astFactory.Variable(@2, $2, NullLangElement, false), $4), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
	|	T_CURLY_OPEN variable '}' { $$ = _astFactory.EncapsedExpression(@$, $2, Tokens.T_LBRACE); }
;

encaps_var_offset:
		T_STRING		{ $$ = _astFactory.Literal(@$, $1, _lexer.TokenTextSpan); }
	|	T_NUM_STRING	{ $$ = _astFactory.Literal(@$, $1, _lexer.TokenTextSpan); }
	|	T_VARIABLE		{ $$ = _astFactory.Variable(@$, $1, NullLangElement, true); }
;


internal_functions_in_yacc:
		T_ISSET '(' isset_variables possible_comma ')' { $$ = _astFactory.Isset(@$, AddTrailingComma($3, $4)); }
	|	T_EMPTY '(' expr ')' { $$ = _astFactory.Empty(@$, $3);}
	|	T_INCLUDE expr
			{ $$ = _astFactory.Inclusion(@$, isConditional, InclusionTypes.Include, $2); }
	|	T_INCLUDE_ONCE expr
			{ $$ = _astFactory.Inclusion(@$, isConditional, InclusionTypes.IncludeOnce, $2); }
	|	T_EVAL '(' expr ')'
			{ $$ = _astFactory.Eval(@$, $3); }
	|	T_REQUIRE expr
			{ $$ = _astFactory.Inclusion(@$, isConditional, InclusionTypes.Require, $2); }
	|	T_REQUIRE_ONCE expr
			{ $$ = _astFactory.Inclusion(@$, isConditional, InclusionTypes.RequireOnce, $2); }
;

isset_variables:
		isset_variable { $$ = NewList<LangElement>( $1 ); }
	|	isset_variables ',' isset_variable { $$ = AddToList<LangElement>($1, $3); }
;

isset_variable:
		expr { $$ = $1; }
;

%%
