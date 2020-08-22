using System;
using System.Text;
using System.Collections.Generic;

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

using System.Linq;
using System.Runtime.InteropServices;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using Devsense.PHP.Errors;

using CompleteAlias = System.Tuple<Devsense.PHP.Syntax.QualifiedNameRef, Devsense.PHP.Syntax.NameRef>;
using ContextAlias = System.Tuple<Devsense.PHP.Text.Span, Devsense.PHP.Syntax.QualifiedNameRef, Devsense.PHP.Syntax.NameRef, Devsense.PHP.Syntax.AliasKind>;
using AnonymousClass = System.Tuple<Devsense.PHP.Syntax.Ast.TypeRef, System.Collections.Generic.List<Devsense.PHP.Syntax.Ast.ActualParam>, Devsense.PHP.Text.Span>;
using StringPair = System.Collections.Generic.KeyValuePair<string, string>;


namespace Devsense.PHP.Syntax
{
public enum Tokens {
/// <summary>&quot;throw (T_THROW)&quot;</summary>
T_THROW=352,
PREC_ARROW_FUNCTION=128,
/// <summary>&quot;include (T_INCLUDE)&quot;</summary>
T_INCLUDE=262,
/// <summary>&quot;include_once (T_INCLUDE_ONCE)&quot;</summary>
T_INCLUDE_ONCE=261,
/// <summary>&quot;require (T_REQUIRE)&quot;</summary>
T_REQUIRE=259,
/// <summary>&quot;require_once (T_REQUIRE_ONCE)&quot;</summary>
T_REQUIRE_ONCE=258,
/// <summary>&quot;or (T_LOGICAL_OR)&quot;</summary>
T_LOGICAL_OR=263,
/// <summary>&quot;xor (T_LOGICAL_XOR)&quot;</summary>
T_LOGICAL_XOR=264,
/// <summary>&quot;and (T_LOGICAL_AND)&quot;</summary>
T_LOGICAL_AND=265,
/// <summary>&quot;print (T_PRINT)&quot;</summary>
T_PRINT=266,
/// <summary>&quot;yield (T_YIELD)&quot;</summary>
T_YIELD=267,
/// <summary>&quot;=&gt; (T_DOUBLE_ARROW)&quot;</summary>
T_DOUBLE_ARROW=268,
/// <summary>&quot;yield from (T_YIELD_FROM)&quot;</summary>
T_YIELD_FROM=269,
/// <summary>&quot;+= (T_PLUS_EQUAL)&quot;</summary>
T_PLUS_EQUAL=270,
/// <summary>&quot;-= (T_MINUS_EQUAL)&quot;</summary>
T_MINUS_EQUAL=271,
/// <summary>&quot;*= (T_MUL_EQUAL)&quot;</summary>
T_MUL_EQUAL=279,
/// <summary>&quot;/= (T_DIV_EQUAL)&quot;</summary>
T_DIV_EQUAL=278,
/// <summary>&quot;.= (T_CONCAT_EQUAL)&quot;</summary>
T_CONCAT_EQUAL=277,
/// <summary>&quot;%= (T_MOD_EQUAL)&quot;</summary>
T_MOD_EQUAL=276,
/// <summary>&quot;&amp;= (T_AND_EQUAL)&quot;</summary>
T_AND_EQUAL=275,
/// <summary>&quot;|= (T_OR_EQUAL)&quot;</summary>
T_OR_EQUAL=274,
/// <summary>&quot;^= (T_XOR_EQUAL)&quot;</summary>
T_XOR_EQUAL=273,
/// <summary>&quot;&lt;&lt;= (T_SL_EQUAL)&quot;</summary>
T_SL_EQUAL=272,
/// <summary>&quot;&gt;&gt;= (T_SR_EQUAL)&quot;</summary>
T_SR_EQUAL=280,
/// <summary>&quot;**= (T_POW_EQUAL)&quot;</summary>
T_POW_EQUAL=281,
/// <summary> &quot;??= (T_COALESCE_EQUAL)&quot;</summary>
T_COALESCE_EQUAL=282,
/// <summary>&quot;?? (T_COALESCE)&quot;</summary>
T_COALESCE=283,
/// <summary>&quot;|| (T_BOOLEAN_OR)&quot;</summary>
T_BOOLEAN_OR=284,
/// <summary>&quot;&amp;&amp; (T_BOOLEAN_AND)&quot;</summary>
T_BOOLEAN_AND=285,
/// <summary>&quot;== (T_IS_EQUAL)&quot;</summary>
T_IS_EQUAL=289,
/// <summary>&quot;!= (T_IS_NOT_EQUAL)&quot;</summary>
T_IS_NOT_EQUAL=288,
/// <summary>&quot;=== (T_IS_IDENTICAL)&quot;</summary>
T_IS_IDENTICAL=287,
/// <summary>&quot;!== (T_IS_NOT_IDENTICAL)&quot;</summary>
T_IS_NOT_IDENTICAL=286,
/// <summary>&quot;&lt;=&gt; (T_SPACESHIP)&quot;</summary>
T_SPACESHIP=290,
/// <summary>&quot;&lt;= (T_IS_SMALLER_OR_EQUAL)&quot;</summary>
T_IS_SMALLER_OR_EQUAL=291,
/// <summary>&quot;&gt;= (T_IS_GREATER_OR_EQUAL)&quot;</summary>
T_IS_GREATER_OR_EQUAL=292,
/// <summary>&quot;&lt;&lt; (T_SL)&quot;</summary>
T_SL=293,
/// <summary>&quot;&gt;&gt; (T_SR)&quot;</summary>
T_SR=294,
/// <summary>&quot;instanceof (T_INSTANCEOF)&quot;</summary>
T_INSTANCEOF=295,
/// <summary>&quot;++ (T_INC)&quot;</summary>
T_INC=303,
/// <summary>&quot;-- (T_DEC)&quot;</summary>
T_DEC=302,
/// <summary>&quot;(int) (T_INT_CAST)&quot;</summary>
T_INT_CAST=301,
/// <summary>&quot;(double) (T_DOUBLE_CAST)&quot;</summary>
T_DOUBLE_CAST=300,
/// <summary>&quot; (T_STRING_CAST)&quot;</summary>
T_STRING_CAST=299,
/// <summary>&quot;(array) (T_ARRAY_CAST)&quot;</summary>
T_ARRAY_CAST=298,
/// <summary>&quot;(object) (T_OBJECT_CAST)&quot;</summary>
T_OBJECT_CAST=297,
/// <summary>&quot;(bool) (T_BOOL_CAST)&quot;</summary>
T_BOOL_CAST=296,
/// <summary>&quot;(unset) (T_UNSET_CAST)&quot;</summary>
T_UNSET_CAST=304,
/// <summary>&quot;** (T_POW)&quot;</summary>
T_POW=305,
/// <summary>&quot;new (T_NEW)&quot;</summary>
T_NEW=306,
/// <summary>&quot;clone (T_CLONE)&quot;</summary>
T_CLONE=307,
T_NOELSE=178,
/// <summary>&quot;elseif (T_ELSEIF)&quot;</summary>
T_ELSEIF=308,
/// <summary>&quot;else (T_ELSE)&quot;</summary>
T_ELSE=309,
/// <summary>&quot;endif (T_ENDIF)&quot;</summary>
T_ENDIF=310,
/// <summary>&quot;static (T_STATIC)&quot;</summary>
T_STATIC=353,
/// <summary>&quot;abstract (T_ABSTRACT)&quot;</summary>
T_ABSTRACT=315,
/// <summary>&quot;final (T_FINAL)&quot;</summary>
T_FINAL=314,
/// <summary>&quot;private (T_PRIVATE)&quot;</summary>
T_PRIVATE=313,
/// <summary>&quot;protected (T_PROTECTED)&quot;</summary>
T_PROTECTED=357,
/// <summary>&quot;public (T_PUBLIC)&quot;</summary>
T_PUBLIC=311,
/// <summary>&quot;integer number (T_LNUMBER)&quot;</summary>
T_LNUMBER=317,
/// <summary>&quot;floating-point number (T_DNUMBER)&quot;</summary>
T_DNUMBER=312,
/// <summary>&quot;identifier (T_STRING)&quot;</summary>
T_STRING=319,
/// <summary>&quot;variable (T_VARIABLE)&quot;</summary>
T_VARIABLE=320,
T_INLINE_HTML=321,
/// <summary>&quot;quoted-string and whitespace (T_ENCAPSED_AND_WHITESPACE)&quot;</summary>
T_ENCAPSED_AND_WHITESPACE=316,
/// <summary>&quot;quoted-string (T_CONSTANT_ENCAPSED_STRING)&quot;</summary>
T_CONSTANT_ENCAPSED_STRING=323,
/// <summary>&quot;variable name (T_STRING_VARNAME)&quot;</summary>
T_STRING_VARNAME=318,
/// <summary>&quot;number (T_NUM_STRING)&quot;</summary>
T_NUM_STRING=325,
/// <summary>&#39;!&#39;</summary>
T_EXCLAM=33,
/// <summary>&#39;&quot;&#39;</summary>
T_DOUBLE_QUOTES=34,
/// <summary>&#39;$&#39;</summary>
T_DOLLAR=36,
/// <summary>&#39;%&#39;</summary>
T_PERCENT=37,
/// <summary>&#39;&amp;&#39;</summary>
T_AMP=38,
/// <summary>&#39;\&#39;&#39;</summary>
T_SINGLE_QUOTES=39,
/// <summary>&#39;(&#39;</summary>
T_LPAREN=40,
/// <summary>&#39;)&#39;</summary>
T_RPAREN=41,
/// <summary>&#39;*&#39;</summary>
T_MUL=42,
/// <summary>&#39;+&#39;</summary>
T_PLUS=43,
/// <summary>&#39;,&#39;</summary>
T_COMMA=44,
/// <summary>&#39;-&#39;</summary>
T_MINUS=45,
/// <summary>&#39;.&#39;</summary>
T_DOT=46,
/// <summary>&#39;/&#39;</summary>
T_SLASH=47,
/// <summary>&#39;:&#39;</summary>
T_COLON=58,
/// <summary>&#39;;&#39;</summary>
T_SEMI=59,
/// <summary>&#39;&lt;&#39;</summary>
T_LT=60,
/// <summary>&#39;=&#39;</summary>
T_EQ=61,
/// <summary>&#39;&gt;&#39;</summary>
T_GT=62,
/// <summary>&#39;?&#39;</summary>
T_QUESTION=63,
/// <summary>&#39;@&#39;</summary>
T_AT=64,
/// <summary>&#39;[&#39;</summary>
T_LBRACKET=91,
/// <summary>&#39;]&#39;</summary>
T_RBRACKET=93,
/// <summary>&#39;^&#39;</summary>
T_CARET=94,
/// <summary>&#39;`&#39;</summary>
T_BACKQUOTE=96,
/// <summary>&#39;{&#39;</summary>
T_LBRACE=123,
/// <summary>&#39;|&#39;</summary>
T_PIPE=124,
/// <summary>&#39;}&#39;</summary>
T_RBRACE=125,
/// <summary>&#39;~&#39;</summary>
T_TILDE=126,
/// <summary>&quot;end of file&quot;</summary>
END=0,
/// <summary>&quot;eval (T_EVAL)&quot;</summary>
T_EVAL=260,
/// <summary>&quot;exit (T_EXIT)&quot;</summary>
T_EXIT=326,
/// <summary>&quot;if (T_IF)&quot;</summary>
T_IF=322,
/// <summary>&quot;echo (T_ECHO)&quot;</summary>
T_ECHO=324,
/// <summary>&quot;do (T_DO)&quot;</summary>
T_DO=329,
/// <summary>&quot;while (T_WHILE)&quot;</summary>
T_WHILE=330,
/// <summary>&quot;endwhile (T_ENDWHILE)&quot;</summary>
T_ENDWHILE=327,
/// <summary>&quot;for (T_FOR)&quot;</summary>
T_FOR=328,
/// <summary>&quot;endfor (T_ENDFOR)&quot;</summary>
T_ENDFOR=333,
/// <summary>&quot;foreach (T_FOREACH)&quot;</summary>
T_FOREACH=334,
/// <summary>&quot;endforeach (T_ENDFOREACH)&quot;</summary>
T_ENDFOREACH=331,
/// <summary>&quot;declare (T_DECLARE)&quot;</summary>
T_DECLARE=332,
/// <summary>&quot;enddeclare (T_ENDDECLARE)&quot;</summary>
T_ENDDECLARE=337,
/// <summary>&quot;as (T_AS)&quot;</summary>
T_AS=338,
/// <summary>&quot;switch (T_SWITCH)&quot;</summary>
T_SWITCH=335,
/// <summary>&quot;endswitch (T_ENDSWITCH)&quot;</summary>
T_ENDSWITCH=336,
/// <summary>&quot;case (T_CASE)&quot;</summary>
T_CASE=341,
/// <summary>&quot;default (T_DEFAULT)&quot;</summary>
T_DEFAULT=342,
/// <summary>&quot;match&quot; (T_MATCH)</summary>
T_MATCH=395,
/// <summary>&quot;break (T_BREAK)&quot;</summary>
T_BREAK=339,
/// <summary>&quot;continue (T_CONTINUE)&quot;</summary>
T_CONTINUE=340,
/// <summary>&quot;goto (T_GOTO)&quot;</summary>
T_GOTO=345,
/// <summary>&quot;function (T_FUNCTION)&quot;</summary>
T_FUNCTION=346,
/// <summary> &quot;fn (T_FN)&quot;</summary>
T_FN=343,
/// <summary>&quot;const (T_CONST)&quot;</summary>
T_CONST=344,
/// <summary>&quot;return (T_RETURN)&quot;</summary>
T_RETURN=348,
/// <summary>&quot;try (T_TRY)&quot;</summary>
T_TRY=349,
/// <summary>&quot;catch (T_CATCH)&quot;</summary>
T_CATCH=347,
/// <summary>&quot;finally (T_FINALLY)&quot;</summary>
T_FINALLY=351,
/// <summary>&quot;use (T_USE)&quot;</summary>
T_USE=350,
/// <summary>&quot;insteadof (T_INSTEADOF)&quot;</summary>
T_INSTEADOF=354,
/// <summary>&quot;exit_scope (T_GLOBAL)&quot;</summary>
T_GLOBAL=355,
/// <summary>&quot;var (T_VAR)&quot;</summary>
T_VAR=356,
/// <summary>&quot;unset (T_UNSET)&quot;</summary>
T_UNSET=360,
/// <summary>&quot;isset (T_ISSET)&quot;</summary>
T_ISSET=358,
/// <summary>&quot;empty (T_EMPTY)&quot;</summary>
T_EMPTY=359,
/// <summary>&quot;__halt_compiler (T_HALT_COMPILER)&quot;</summary>
T_HALT_COMPILER=363,
/// <summary>&quot;class (T_CLASS)&quot;</summary>
T_CLASS=361,
/// <summary>&quot;trait (T_TRAIT)&quot;</summary>
T_TRAIT=362,
/// <summary>&quot;interface (T_INTERFACE)&quot;</summary>
T_INTERFACE=366,
/// <summary>&quot;extends (T_EXTENDS)&quot;</summary>
T_EXTENDS=364,
/// <summary>&quot;implements (T_IMPLEMENTS)&quot;</summary>
T_IMPLEMENTS=365,
/// <summary>&quot;-&gt; (T_OBJECT_OPERATOR)&quot;</summary>
T_OBJECT_OPERATOR=369,
/// <summary>&quot;?-&gt; (T_NULLSAFE_OBJECT_OPERATOR )&quot;</summary>
T_NULLSAFE_OBJECT_OPERATOR=396,
/// <summary>&quot;list (T_LIST)&quot;</summary>
T_LIST=367,
/// <summary>&quot;array (T_ARRAY)&quot;</summary>
T_ARRAY=368,
/// <summary>&quot;callable (T_CALLABLE)&quot;</summary>
T_CALLABLE=372,
/// <summary>&quot;__LINE__ (T_LINE)&quot;</summary>
T_LINE=370,
/// <summary>&quot;__FILE__ (T_FILE)&quot;</summary>
T_FILE=371,
/// <summary>&quot;__DIR__ (T_DIR)&quot;</summary>
T_DIR=375,
/// <summary>&quot;__CLASS__ (T_CLASS_C)&quot;</summary>
T_CLASS_C=373,
/// <summary>&quot;__TRAIT__ (T_TRAIT_C)&quot;</summary>
T_TRAIT_C=374,
/// <summary>&quot;__METHOD__ (T_METHOD_C)&quot;</summary>
T_METHOD_C=378,
/// <summary>&quot;__FUNCTION__ (T_FUNC_C)&quot;</summary>
T_FUNC_C=376,
/// <summary>&quot;comment (T_COMMENT)&quot;</summary>
T_COMMENT=377,
/// <summary>&quot;doc comment (T_DOC_COMMENT)&quot;</summary>
T_DOC_COMMENT=381,
/// <summary>&quot;open tag (T_OPEN_TAG)&quot;</summary>
T_OPEN_TAG=379,
/// <summary>&quot;open tag with echo (T_OPEN_TAG_WITH_ECHO)&quot;</summary>
T_OPEN_TAG_WITH_ECHO=380,
/// <summary>&quot;close tag (T_CLOSE_TAG)&quot;</summary>
T_CLOSE_TAG=384,
/// <summary>&quot;whitespace (T_WHITESPACE)&quot;</summary>
T_WHITESPACE=382,
/// <summary>&quot;heredoc start (T_START_HEREDOC)&quot;</summary>
T_START_HEREDOC=383,
/// <summary>&quot;heredoc end (T_END_HEREDOC)&quot;</summary>
T_END_HEREDOC=387,
/// <summary>&quot;${ (T_DOLLAR_OPEN_CURLY_BRACES)&quot;</summary>
T_DOLLAR_OPEN_CURLY_BRACES=385,
/// <summary>&quot;{$ (T_CURLY_OPEN)&quot;</summary>
T_CURLY_OPEN=386,
/// <summary>&quot;:: (T_DOUBLE_COLON)&quot;</summary>
T_DOUBLE_COLON=390,
/// <summary>&quot;namespace (T_NAMESPACE)&quot;</summary>
T_NAMESPACE=391,
/// <summary>&quot;__NAMESPACE__ (T_NS_C)&quot;</summary>
T_NS_C=392,
/// <summary>&quot;\\ (T_NS_SEPARATOR)&quot;</summary>
T_NS_SEPARATOR=393,
/// <summary>&quot;... (T_ELLIPSIS)&quot;</summary>
T_ELLIPSIS=394,
/// <summary> type safe declaration</summary>
T_ERROR=257,
EOF=0
};

[StructLayout(LayoutKind.Explicit)]
public partial struct SemanticValueType
{
	// Integer and Offset are both used when generating code for string 
	// with 'inline' variables. Other fields are not combined.
	
	[FieldOffset(0)]		
	public bool Bool;
	[FieldOffset(0)]		
	public int Integer;
	[FieldOffset(0)]
	public double Double;
	[FieldOffset(0)]
	public long Long;
	[FieldOffset(0)]
	public QualifiedNameRef QualifiedNameReference;
	[FieldOffset(0)]
	public AliasKind Kind;
	/// <summary>Token that encapsulates the string literal.</summary>
	[FieldOffset(0)]
	public Tokens QuoteToken;
	/// <summary>The original token.</summary>
	[FieldOffset(0)]
	public Tokens Token;

	[FieldOffset(8)]
	public object Object;

	public TypeRef TypeReference						{ get { return (TypeRef)Object; }					set { Object = value; } }
	public IList<TypeRef> TypeRefList					{ get { return (IList<TypeRef>)Object; }			set { Object = value; } }
	public LangElement Node								{ get { return (LangElement)Object; }				set { Object = value; } }
	public List<LangElement> NodeList					{ get { return (List<LangElement>)Object; }			set { Object = value; } }
	public string String								{ get { return (string)Object; }					set { Object = value; } }
	public StringPair Strings							{ get { return (StringPair)Object; }				set { Object = value; } }
	public List<string> StringList						{ get { return (List<string>)Object; }				set { Object = value; } }
	public CompleteAlias Alias							{ get { return (CompleteAlias)Object; }				set { Object = value; } }
	public List<CompleteAlias> AliasList				{ get { return (List<CompleteAlias>)Object; }		set { Object = value; } }
	public ContextAlias ContextAlias					{ get { return (ContextAlias)Object; }				set { Object = value; } }
	public List<ContextAlias> ContextAliasList			{ get { return (List<ContextAlias>)Object; }		set { Object = value; } }
	public ActualParam Param							{ get { return (ActualParam)Object; }				set { Object = value; } }
	public List<ActualParam> ParamList					{ get { return (List<ActualParam>)Object; }			set { Object = value; } }
	public FormalParam FormalParam						{ get { return (FormalParam)Object; }				set { Object = value; } }
	public List<FormalParam> FormalParamList			{ get { return (List<FormalParam>)Object; }			set { Object = value; } }
	public Item Item									{ get { return (Item)Object; }						set { Object = value; } }
	public List<Item> ItemList							{ get { return (List<Item>)Object; }				set { Object = value; } }
	internal List<IfStatement> IfItemList				{ get { return (List<IfStatement>)Object; }			set { Object = value; } }
	public ForeachVar ForeachVar						{ get { return (ForeachVar)Object; }				set { Object = value; } }
	public AnonymousClass AnonymousClass				{ get { return (AnonymousClass)Object; }			set { Object = value; } }
	public UseBase Use									{ get { return (UseBase)Object; }					set { Object = value; } }
	public List<UseBase> UseList						{ get { return (List<UseBase>)Object; }				set { Object = value; } }
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
      new State(0, -2, new int[] {-149,1,-151,3}),
      new State(1, new int[] {0,2}),
      new State(2, -1),
      new State(3, -83, new int[] {-98,4}),
      new State(4, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,964,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,975,350,979,344,1035,0,-3,322,-418,361,-182}, new int[] {-34,5,-35,6,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,972,-89,518,-93,519,-86,974,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(5, -82),
      new State(6, -99),
      new State(7, -136, new int[] {-101,8}),
      new State(8, new int[] {125,9,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(9, -141),
      new State(10, -135),
      new State(11, -137),
      new State(12, new int[] {322,943}, new int[] {-55,13,-56,15,-142,17,-143,950}),
      new State(13, -419, new int[] {-19,14}),
      new State(14, -142),
      new State(15, -419, new int[] {-19,16}),
      new State(16, -143),
      new State(17, new int[] {308,18,309,941,123,-228,330,-228,329,-228,328,-228,335,-228,339,-228,340,-228,348,-228,355,-228,353,-228,324,-228,321,-228,320,-228,36,-228,319,-228,391,-228,393,-228,40,-228,368,-228,91,-228,323,-228,367,-228,307,-228,303,-228,302,-228,43,-228,45,-228,33,-228,126,-228,306,-228,358,-228,359,-228,262,-228,261,-228,260,-228,259,-228,258,-228,301,-228,300,-228,299,-228,298,-228,297,-228,296,-228,304,-228,326,-228,64,-228,317,-228,312,-228,370,-228,371,-228,375,-228,374,-228,378,-228,376,-228,392,-228,373,-228,34,-228,383,-228,96,-228,266,-228,267,-228,269,-228,352,-228,346,-228,343,-228,293,-228,395,-228,360,-228,334,-228,332,-228,59,-228,349,-228,345,-228,315,-228,314,-228,362,-228,366,-228,363,-228,350,-228,344,-228,322,-228,361,-228,0,-228,125,-228,341,-228,342,-228,336,-228,337,-228,331,-228,333,-228,327,-228,310,-228}),
      new State(18, new int[] {40,19}),
      new State(19, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,20,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(20, new int[] {41,21,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(21, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,322,-418}, new int[] {-35,22,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(22, -227),
      new State(23, new int[] {40,24}),
      new State(24, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,25,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(25, new int[] {41,26,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(26, -418, new int[] {-18,27}),
      new State(27, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,58,937,322,-418}, new int[] {-74,28,-35,30,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(28, -419, new int[] {-19,29}),
      new State(29, -144),
      new State(30, -224),
      new State(31, -418, new int[] {-18,32}),
      new State(32, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,322,-418}, new int[] {-35,33,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(33, new int[] {330,34}),
      new State(34, new int[] {40,35}),
      new State(35, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,36,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(36, new int[] {41,37,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(37, new int[] {59,38}),
      new State(38, -419, new int[] {-19,39}),
      new State(39, -145),
      new State(40, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,41,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(41, new int[] {284,-354,285,42,263,-354,265,-354,264,-354,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-354,283,-354,59,-354,41,-354,125,-354,58,-354,93,-354,44,-354,268,-354,338,-354}),
      new State(42, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,43,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(43, new int[] {284,-355,285,-355,263,-355,265,-355,264,-355,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-355,283,-355,59,-355,41,-355,125,-355,58,-355,93,-355,44,-355,268,-355,338,-355}),
      new State(44, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,45,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(45, new int[] {284,40,285,42,263,-356,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-356,41,-356,125,-356,58,-356,93,-356,44,-356,268,-356,338,-356}),
      new State(46, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,47,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(47, new int[] {284,40,285,42,263,-357,265,-357,264,-357,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-357,41,-357,125,-357,58,-357,93,-357,44,-357,268,-357,338,-357}),
      new State(48, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,49,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(49, new int[] {284,40,285,42,263,-358,265,46,264,-358,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-358,41,-358,125,-358,58,-358,93,-358,44,-358,268,-358,338,-358}),
      new State(50, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,51,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(51, new int[] {284,-359,285,-359,263,-359,265,-359,264,-359,124,-359,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-359,283,-359,59,-359,41,-359,125,-359,58,-359,93,-359,44,-359,268,-359,338,-359}),
      new State(52, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,53,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(53, new int[] {284,-360,285,-360,263,-360,265,-360,264,-360,124,-360,38,-360,94,-360,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-360,283,-360,59,-360,41,-360,125,-360,58,-360,93,-360,44,-360,268,-360,338,-360}),
      new State(54, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,55,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(55, new int[] {284,-361,285,-361,263,-361,265,-361,264,-361,124,-361,38,52,94,-361,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-361,283,-361,59,-361,41,-361,125,-361,58,-361,93,-361,44,-361,268,-361,338,-361}),
      new State(56, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,57,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(57, new int[] {284,-362,285,-362,263,-362,265,-362,264,-362,124,-362,38,-362,94,-362,46,-362,43,-362,45,-362,42,62,305,64,47,66,37,68,293,-362,294,-362,287,-362,286,-362,289,-362,288,-362,60,-362,291,-362,62,-362,292,-362,290,-362,295,92,63,-362,283,-362,59,-362,41,-362,125,-362,58,-362,93,-362,44,-362,268,-362,338,-362}),
      new State(58, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,59,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(59, new int[] {284,-363,285,-363,263,-363,265,-363,264,-363,124,-363,38,-363,94,-363,46,-363,43,-363,45,-363,42,62,305,64,47,66,37,68,293,-363,294,-363,287,-363,286,-363,289,-363,288,-363,60,-363,291,-363,62,-363,292,-363,290,-363,295,92,63,-363,283,-363,59,-363,41,-363,125,-363,58,-363,93,-363,44,-363,268,-363,338,-363}),
      new State(60, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,61,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(61, new int[] {284,-364,285,-364,263,-364,265,-364,264,-364,124,-364,38,-364,94,-364,46,-364,43,-364,45,-364,42,62,305,64,47,66,37,68,293,-364,294,-364,287,-364,286,-364,289,-364,288,-364,60,-364,291,-364,62,-364,292,-364,290,-364,295,92,63,-364,283,-364,59,-364,41,-364,125,-364,58,-364,93,-364,44,-364,268,-364,338,-364}),
      new State(62, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,63,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(63, new int[] {284,-365,285,-365,263,-365,265,-365,264,-365,124,-365,38,-365,94,-365,46,-365,43,-365,45,-365,42,-365,305,64,47,-365,37,-365,293,-365,294,-365,287,-365,286,-365,289,-365,288,-365,60,-365,291,-365,62,-365,292,-365,290,-365,295,92,63,-365,283,-365,59,-365,41,-365,125,-365,58,-365,93,-365,44,-365,268,-365,338,-365}),
      new State(64, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,65,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(65, new int[] {284,-366,285,-366,263,-366,265,-366,264,-366,124,-366,38,-366,94,-366,46,-366,43,-366,45,-366,42,-366,305,64,47,-366,37,-366,293,-366,294,-366,287,-366,286,-366,289,-366,288,-366,60,-366,291,-366,62,-366,292,-366,290,-366,295,-366,63,-366,283,-366,59,-366,41,-366,125,-366,58,-366,93,-366,44,-366,268,-366,338,-366}),
      new State(66, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,67,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(67, new int[] {284,-367,285,-367,263,-367,265,-367,264,-367,124,-367,38,-367,94,-367,46,-367,43,-367,45,-367,42,-367,305,64,47,-367,37,-367,293,-367,294,-367,287,-367,286,-367,289,-367,288,-367,60,-367,291,-367,62,-367,292,-367,290,-367,295,92,63,-367,283,-367,59,-367,41,-367,125,-367,58,-367,93,-367,44,-367,268,-367,338,-367}),
      new State(68, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,69,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(69, new int[] {284,-368,285,-368,263,-368,265,-368,264,-368,124,-368,38,-368,94,-368,46,-368,43,-368,45,-368,42,-368,305,64,47,-368,37,-368,293,-368,294,-368,287,-368,286,-368,289,-368,288,-368,60,-368,291,-368,62,-368,292,-368,290,-368,295,92,63,-368,283,-368,59,-368,41,-368,125,-368,58,-368,93,-368,44,-368,268,-368,338,-368}),
      new State(70, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,71,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(71, new int[] {284,-369,285,-369,263,-369,265,-369,264,-369,124,-369,38,-369,94,-369,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,-369,294,-369,287,-369,286,-369,289,-369,288,-369,60,-369,291,-369,62,-369,292,-369,290,-369,295,92,63,-369,283,-369,59,-369,41,-369,125,-369,58,-369,93,-369,44,-369,268,-369,338,-369}),
      new State(72, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,73,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(73, new int[] {284,-370,285,-370,263,-370,265,-370,264,-370,124,-370,38,-370,94,-370,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,-370,294,-370,287,-370,286,-370,289,-370,288,-370,60,-370,291,-370,62,-370,292,-370,290,-370,295,92,63,-370,283,-370,59,-370,41,-370,125,-370,58,-370,93,-370,44,-370,268,-370,338,-370}),
      new State(74, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,75,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(75, new int[] {284,-375,285,-375,263,-375,265,-375,264,-375,124,-375,38,-375,94,-375,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-375,283,-375,59,-375,41,-375,125,-375,58,-375,93,-375,44,-375,268,-375,338,-375}),
      new State(76, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,77,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(77, new int[] {284,-376,285,-376,263,-376,265,-376,264,-376,124,-376,38,-376,94,-376,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-376,283,-376,59,-376,41,-376,125,-376,58,-376,93,-376,44,-376,268,-376,338,-376}),
      new State(78, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,79,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(79, new int[] {284,-377,285,-377,263,-377,265,-377,264,-377,124,-377,38,-377,94,-377,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-377,283,-377,59,-377,41,-377,125,-377,58,-377,93,-377,44,-377,268,-377,338,-377}),
      new State(80, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,81,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(81, new int[] {284,-378,285,-378,263,-378,265,-378,264,-378,124,-378,38,-378,94,-378,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-378,283,-378,59,-378,41,-378,125,-378,58,-378,93,-378,44,-378,268,-378,338,-378}),
      new State(82, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,83,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(83, new int[] {284,-379,285,-379,263,-379,265,-379,264,-379,124,-379,38,-379,94,-379,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,-379,286,-379,289,-379,288,-379,60,82,291,84,62,86,292,88,290,-379,295,92,63,-379,283,-379,59,-379,41,-379,125,-379,58,-379,93,-379,44,-379,268,-379,338,-379}),
      new State(84, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,85,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(85, new int[] {284,-380,285,-380,263,-380,265,-380,264,-380,124,-380,38,-380,94,-380,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,-380,286,-380,289,-380,288,-380,60,82,291,84,62,86,292,88,290,-380,295,92,63,-380,283,-380,59,-380,41,-380,125,-380,58,-380,93,-380,44,-380,268,-380,338,-380}),
      new State(86, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,87,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(87, new int[] {284,-381,285,-381,263,-381,265,-381,264,-381,124,-381,38,-381,94,-381,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,-381,286,-381,289,-381,288,-381,60,82,291,84,62,86,292,88,290,-381,295,92,63,-381,283,-381,59,-381,41,-381,125,-381,58,-381,93,-381,44,-381,268,-381,338,-381}),
      new State(88, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,89,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(89, new int[] {284,-382,285,-382,263,-382,265,-382,264,-382,124,-382,38,-382,94,-382,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,-382,286,-382,289,-382,288,-382,60,82,291,84,62,86,292,88,290,-382,295,92,63,-382,283,-382,59,-382,41,-382,125,-382,58,-382,93,-382,44,-382,268,-382,338,-382}),
      new State(90, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,91,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(91, new int[] {284,-383,285,-383,263,-383,265,-383,264,-383,124,-383,38,-383,94,-383,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-383,283,-383,59,-383,41,-383,125,-383,58,-383,93,-383,44,-383,268,-383,338,-383}),
      new State(92, new int[] {353,297,319,187,391,188,393,191,320,97,36,98}, new int[] {-27,93,-28,94,-20,513,-120,184,-79,514,-49,556}),
      new State(93, -384),
      new State(94, new int[] {390,95,59,-436,284,-436,285,-436,263,-436,265,-436,264,-436,124,-436,38,-436,94,-436,46,-436,43,-436,45,-436,42,-436,305,-436,47,-436,37,-436,293,-436,294,-436,287,-436,286,-436,289,-436,288,-436,60,-436,291,-436,62,-436,292,-436,290,-436,295,-436,63,-436,283,-436,41,-436,125,-436,58,-436,93,-436,44,-436,268,-436,338,-436,40,-436}),
      new State(95, new int[] {320,97,36,98}, new int[] {-49,96}),
      new State(96, -498),
      new State(97, -489),
      new State(98, new int[] {123,99,320,97,36,98}, new int[] {-49,936}),
      new State(99, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,100,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(100, new int[] {125,101,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(101, -490),
      new State(102, new int[] {58,934,320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,103,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(103, new int[] {58,104,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(104, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,105,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(105, new int[] {284,40,285,42,263,-387,265,-387,264,-387,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-387,283,106,59,-387,41,-387,125,-387,58,-387,93,-387,44,-387,268,-387,338,-387}),
      new State(106, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,107,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(107, new int[] {284,40,285,42,263,-389,265,-389,264,-389,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-389,283,106,59,-389,41,-389,125,-389,58,-389,93,-389,44,-389,268,-389,338,-389}),
      new State(108, new int[] {61,109,270,906,271,908,279,910,281,912,278,914,277,916,276,918,275,920,274,922,273,924,272,926,280,928,282,930,303,932,302,933,59,-467,284,-467,285,-467,263,-467,265,-467,264,-467,124,-467,38,-467,94,-467,46,-467,43,-467,45,-467,42,-467,305,-467,47,-467,37,-467,293,-467,294,-467,287,-467,286,-467,289,-467,288,-467,60,-467,291,-467,62,-467,292,-467,290,-467,295,-467,63,-467,283,-467,41,-467,125,-467,58,-467,93,-467,44,-467,268,-467,338,-467,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(109, new int[] {38,111,320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,110,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(110, new int[] {284,40,285,42,263,-333,265,-333,264,-333,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-333,41,-333,125,-333,58,-333,93,-333,44,-333,268,-333,338,-333}),
      new State(111, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306,306,346}, new int[] {-44,112,-46,113,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(112, new int[] {59,-334,284,-334,285,-334,263,-334,265,-334,264,-334,124,-334,38,-334,94,-334,46,-334,43,-334,45,-334,42,-334,305,-334,47,-334,37,-334,293,-334,294,-334,287,-334,286,-334,289,-334,288,-334,60,-334,291,-334,62,-334,292,-334,290,-334,295,-334,63,-334,283,-334,41,-334,125,-334,58,-334,93,-334,44,-334,268,-334,338,-334,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(113, -335),
      new State(114, new int[] {61,-486,270,-486,271,-486,279,-486,281,-486,278,-486,277,-486,276,-486,275,-486,274,-486,273,-486,272,-486,280,-486,282,-486,303,-486,302,-486,59,-486,284,-486,285,-486,263,-486,265,-486,264,-486,124,-486,38,-486,94,-486,46,-486,43,-486,45,-486,42,-486,305,-486,47,-486,37,-486,293,-486,294,-486,287,-486,286,-486,289,-486,288,-486,60,-486,291,-486,62,-486,292,-486,290,-486,295,-486,63,-486,283,-486,91,-486,123,-486,369,-486,396,-486,390,-486,41,-486,125,-486,58,-486,93,-486,44,-486,268,-486,338,-486,40,-477}),
      new State(115, -480),
      new State(116, new int[] {91,117,123,903,369,449,396,450,390,-473}, new int[] {-21,900}),
      new State(117, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,93,-469}, new int[] {-62,118,-43,120,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(118, new int[] {93,119}),
      new State(119, -481),
      new State(120, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,93,-470,59,-470,41,-470}),
      new State(121, -487),
      new State(122, new int[] {390,123}),
      new State(123, new int[] {320,97,36,98,319,199,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,391,257,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276,123,277}, new int[] {-49,124,-117,125,-2,126,-118,200,-119,201}),
      new State(124, new int[] {61,-492,270,-492,271,-492,279,-492,281,-492,278,-492,277,-492,276,-492,275,-492,274,-492,273,-492,272,-492,280,-492,282,-492,303,-492,302,-492,59,-492,284,-492,285,-492,263,-492,265,-492,264,-492,124,-492,38,-492,94,-492,46,-492,43,-492,45,-492,42,-492,305,-492,47,-492,37,-492,293,-492,294,-492,287,-492,286,-492,289,-492,288,-492,60,-492,291,-492,62,-492,292,-492,290,-492,295,-492,63,-492,283,-492,91,-492,123,-492,369,-492,396,-492,390,-492,41,-492,125,-492,58,-492,93,-492,44,-492,268,-492,338,-492,40,-502}),
      new State(125, new int[] {91,-465,59,-465,284,-465,285,-465,263,-465,265,-465,264,-465,124,-465,38,-465,94,-465,46,-465,43,-465,45,-465,42,-465,305,-465,47,-465,37,-465,293,-465,294,-465,287,-465,286,-465,289,-465,288,-465,60,-465,291,-465,62,-465,292,-465,290,-465,295,-465,63,-465,283,-465,41,-465,125,-465,58,-465,93,-465,44,-465,268,-465,338,-465,40,-500}),
      new State(126, new int[] {40,128}, new int[] {-130,127}),
      new State(127, -431),
      new State(128, new int[] {41,129,320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,394,897}, new int[] {-131,130,-128,899,-43,135,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(129, -259),
      new State(130, new int[] {44,133,41,-119}, new int[] {-3,131}),
      new State(131, new int[] {41,132}),
      new State(132, -260),
      new State(133, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,394,897,41,-120}, new int[] {-128,134,-43,135,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(134, -262),
      new State(135, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-263,41,-263}),
      new State(136, new int[] {346,489,343,502,390,-434}, new int[] {-85,137,-5,138,-6,490}),
      new State(137, -410),
      new State(138, new int[] {38,592,40,-422}, new int[] {-4,139}),
      new State(139, -417, new int[] {-17,140}),
      new State(140, new int[] {40,141}),
      new State(141, new int[] {293,507,311,587,357,588,313,589,368,-240,372,-240,353,-240,319,-240,391,-240,393,-240,63,-240,38,-240,394,-240,320,-240,41,-235}, new int[] {-134,142,-135,571,-88,591,-91,575,-89,518,-132,590,-15,577}),
      new State(142, new int[] {41,143}),
      new State(143, new int[] {350,887,58,-424,123,-424}, new int[] {-136,144}),
      new State(144, new int[] {58,557,123,-257}, new int[] {-23,145}),
      new State(145, -420, new int[] {-154,146}),
      new State(146, -418, new int[] {-18,147}),
      new State(147, new int[] {123,148}),
      new State(148, -136, new int[] {-101,149}),
      new State(149, new int[] {125,150,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(150, -419, new int[] {-19,151}),
      new State(151, -420, new int[] {-154,152}),
      new State(152, -413),
      new State(153, new int[] {40,154}),
      new State(154, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,59,-322}, new int[] {-103,155,-114,883,-43,886,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(155, new int[] {59,156}),
      new State(156, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,59,-322}, new int[] {-103,157,-114,883,-43,886,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(157, new int[] {59,158}),
      new State(158, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,41,-322}, new int[] {-103,159,-114,883,-43,886,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(159, new int[] {41,160}),
      new State(160, -418, new int[] {-18,161}),
      new State(161, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,58,879,322,-418}, new int[] {-72,162,-35,164,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(162, -419, new int[] {-19,163}),
      new State(163, -146),
      new State(164, -200),
      new State(165, new int[] {40,166}),
      new State(166, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,167,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(167, new int[] {41,168,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(168, -418, new int[] {-18,169}),
      new State(169, new int[] {123,172,58,871}, new int[] {-113,170}),
      new State(170, -419, new int[] {-19,171}),
      new State(171, -147),
      new State(172, new int[] {59,868,125,-210,341,-210,342,-210}, new int[] {-112,173}),
      new State(173, new int[] {125,174,341,175,342,865}),
      new State(174, -206),
      new State(175, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,176,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(176, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,58,863,59,864}, new int[] {-158,177}),
      new State(177, -136, new int[] {-101,178}),
      new State(178, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,125,-211,341,-211,342,-211,336,-211,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(179, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,59,-469}, new int[] {-62,180,-43,120,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(180, new int[] {59,181}),
      new State(181, -148),
      new State(182, new int[] {40,128,390,-435,91,-464,59,-464,284,-464,285,-464,263,-464,265,-464,264,-464,124,-464,38,-464,94,-464,46,-464,43,-464,45,-464,42,-464,305,-464,47,-464,37,-464,293,-464,294,-464,287,-464,286,-464,289,-464,288,-464,60,-464,291,-464,62,-464,292,-464,290,-464,295,-464,63,-464,283,-464,41,-464,125,-464,58,-464,93,-464,44,-464,268,-464,338,-464}, new int[] {-130,183}),
      new State(183, -430),
      new State(184, new int[] {393,185,40,-86,390,-86,91,-86,59,-86,284,-86,285,-86,263,-86,265,-86,264,-86,124,-86,38,-86,94,-86,46,-86,43,-86,45,-86,42,-86,305,-86,47,-86,37,-86,293,-86,294,-86,287,-86,286,-86,289,-86,288,-86,60,-86,291,-86,62,-86,292,-86,290,-86,295,-86,63,-86,283,-86,41,-86,125,-86,58,-86,93,-86,44,-86,268,-86,338,-86,320,-86,123,-86,394,-86,365,-86}),
      new State(185, new int[] {319,186}),
      new State(186, -85),
      new State(187, -84),
      new State(188, new int[] {393,189}),
      new State(189, new int[] {319,187}, new int[] {-120,190}),
      new State(190, new int[] {393,185,40,-87,390,-87,91,-87,59,-87,284,-87,285,-87,263,-87,265,-87,264,-87,124,-87,38,-87,94,-87,46,-87,43,-87,45,-87,42,-87,305,-87,47,-87,37,-87,293,-87,294,-87,287,-87,286,-87,289,-87,288,-87,60,-87,291,-87,62,-87,292,-87,290,-87,295,-87,63,-87,283,-87,41,-87,125,-87,58,-87,93,-87,44,-87,268,-87,338,-87,320,-87,123,-87,394,-87,365,-87}),
      new State(191, new int[] {319,187}, new int[] {-120,192}),
      new State(192, new int[] {393,185,40,-88,390,-88,91,-88,59,-88,284,-88,285,-88,263,-88,265,-88,264,-88,124,-88,38,-88,94,-88,46,-88,43,-88,45,-88,42,-88,305,-88,47,-88,37,-88,293,-88,294,-88,287,-88,286,-88,289,-88,288,-88,60,-88,291,-88,62,-88,292,-88,290,-88,295,-88,63,-88,283,-88,41,-88,125,-88,58,-88,93,-88,44,-88,268,-88,338,-88,320,-88,123,-88,394,-88,365,-88}),
      new State(193, new int[] {390,194}),
      new State(194, new int[] {320,97,36,98,319,199,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,391,257,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276,123,277}, new int[] {-49,195,-117,196,-2,197,-118,200,-119,201}),
      new State(195, new int[] {61,-493,270,-493,271,-493,279,-493,281,-493,278,-493,277,-493,276,-493,275,-493,274,-493,273,-493,272,-493,280,-493,282,-493,303,-493,302,-493,59,-493,284,-493,285,-493,263,-493,265,-493,264,-493,124,-493,38,-493,94,-493,46,-493,43,-493,45,-493,42,-493,305,-493,47,-493,37,-493,293,-493,294,-493,287,-493,286,-493,289,-493,288,-493,60,-493,291,-493,62,-493,292,-493,290,-493,295,-493,63,-493,283,-493,91,-493,123,-493,369,-493,396,-493,390,-493,41,-493,125,-493,58,-493,93,-493,44,-493,268,-493,338,-493,40,-502}),
      new State(196, new int[] {91,-466,59,-466,284,-466,285,-466,263,-466,265,-466,264,-466,124,-466,38,-466,94,-466,46,-466,43,-466,45,-466,42,-466,305,-466,47,-466,37,-466,293,-466,294,-466,287,-466,286,-466,289,-466,288,-466,60,-466,291,-466,62,-466,292,-466,290,-466,295,-466,63,-466,283,-466,41,-466,125,-466,58,-466,93,-466,44,-466,268,-466,338,-466,40,-500}),
      new State(197, new int[] {40,128}, new int[] {-130,198}),
      new State(198, -432),
      new State(199, -80),
      new State(200, -81),
      new State(201, -73),
      new State(202, -4),
      new State(203, -5),
      new State(204, -6),
      new State(205, -7),
      new State(206, -8),
      new State(207, -9),
      new State(208, -10),
      new State(209, -11),
      new State(210, -12),
      new State(211, -13),
      new State(212, -14),
      new State(213, -15),
      new State(214, -16),
      new State(215, -17),
      new State(216, -18),
      new State(217, -19),
      new State(218, -20),
      new State(219, -21),
      new State(220, -22),
      new State(221, -23),
      new State(222, -24),
      new State(223, -25),
      new State(224, -26),
      new State(225, -27),
      new State(226, -28),
      new State(227, -29),
      new State(228, -30),
      new State(229, -31),
      new State(230, -32),
      new State(231, -33),
      new State(232, -34),
      new State(233, -35),
      new State(234, -36),
      new State(235, -37),
      new State(236, -38),
      new State(237, -39),
      new State(238, -40),
      new State(239, -41),
      new State(240, -42),
      new State(241, -43),
      new State(242, -44),
      new State(243, -45),
      new State(244, -46),
      new State(245, -47),
      new State(246, -48),
      new State(247, -49),
      new State(248, -50),
      new State(249, -51),
      new State(250, -52),
      new State(251, -53),
      new State(252, -54),
      new State(253, -55),
      new State(254, -56),
      new State(255, -57),
      new State(256, -58),
      new State(257, -59),
      new State(258, -60),
      new State(259, -61),
      new State(260, -62),
      new State(261, -63),
      new State(262, -64),
      new State(263, -65),
      new State(264, -66),
      new State(265, -67),
      new State(266, -68),
      new State(267, -69),
      new State(268, -70),
      new State(269, -71),
      new State(270, -72),
      new State(271, -74),
      new State(272, -75),
      new State(273, -76),
      new State(274, -77),
      new State(275, -78),
      new State(276, -79),
      new State(277, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,278,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(278, new int[] {125,279,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(279, -501),
      new State(280, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,281,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(281, new int[] {41,282,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(282, new int[] {91,-475,123,-475,369,-475,396,-475,390,-475,40,-478,59,-385,284,-385,285,-385,263,-385,265,-385,264,-385,124,-385,38,-385,94,-385,46,-385,43,-385,45,-385,42,-385,305,-385,47,-385,37,-385,293,-385,294,-385,287,-385,286,-385,289,-385,288,-385,60,-385,291,-385,62,-385,292,-385,290,-385,295,-385,63,-385,283,-385,41,-385,125,-385,58,-385,93,-385,44,-385,268,-385,338,-385}),
      new State(283, new int[] {91,-476,123,-476,369,-476,396,-476,390,-476,40,-479,59,-462,284,-462,285,-462,263,-462,265,-462,264,-462,124,-462,38,-462,94,-462,46,-462,43,-462,45,-462,42,-462,305,-462,47,-462,37,-462,293,-462,294,-462,287,-462,286,-462,289,-462,288,-462,60,-462,291,-462,62,-462,292,-462,290,-462,295,-462,63,-462,283,-462,41,-462,125,-462,58,-462,93,-462,44,-462,268,-462,338,-462}),
      new State(284, new int[] {40,285}),
      new State(285, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,41,-507}, new int[] {-141,286,-140,288,-138,329,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(286, new int[] {41,287}),
      new State(287, -445),
      new State(288, new int[] {44,289,41,-506,93,-506}),
      new State(289, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,41,-507,93,-507}, new int[] {-138,290,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(290, -509),
      new State(291, -508),
      new State(292, new int[] {268,293,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-512,41,-512,93,-512}),
      new State(293, new int[] {38,295,367,859,320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,294,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(294, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-511,41,-511,93,-511}),
      new State(295, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306}, new int[] {-44,296,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(296, new int[] {44,-513,41,-513,93,-513,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(297, -434),
      new State(298, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,299,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(299, new int[] {41,300,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(300, new int[] {91,-475,123,-475,369,-475,396,-475,390,-475,40,-478}),
      new State(301, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,93,-507}, new int[] {-141,302,-140,288,-138,329,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(302, new int[] {93,303}),
      new State(303, new int[] {61,304,91,-446,123,-446,369,-446,396,-446,390,-446,40,-446,59,-446,284,-446,285,-446,263,-446,265,-446,264,-446,124,-446,38,-446,94,-446,46,-446,43,-446,45,-446,42,-446,305,-446,47,-446,37,-446,293,-446,294,-446,287,-446,286,-446,289,-446,288,-446,60,-446,291,-446,62,-446,292,-446,290,-446,295,-446,63,-446,283,-446,41,-446,125,-446,58,-446,93,-446,44,-446,268,-446,338,-446}),
      new State(304, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,305,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(305, new int[] {284,40,285,42,263,-332,265,-332,264,-332,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-332,41,-332,125,-332,58,-332,93,-332,44,-332,268,-332,338,-332}),
      new State(306, -447),
      new State(307, new int[] {91,308,59,-463,284,-463,285,-463,263,-463,265,-463,264,-463,124,-463,38,-463,94,-463,46,-463,43,-463,45,-463,42,-463,305,-463,47,-463,37,-463,293,-463,294,-463,287,-463,286,-463,289,-463,288,-463,60,-463,291,-463,62,-463,292,-463,290,-463,295,-463,63,-463,283,-463,41,-463,125,-463,58,-463,93,-463,44,-463,268,-463,338,-463}),
      new State(308, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,93,-469}, new int[] {-62,309,-43,120,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(309, new int[] {93,310}),
      new State(310, -482),
      new State(311, -485),
      new State(312, new int[] {40,128}, new int[] {-130,313}),
      new State(313, -433),
      new State(314, -468),
      new State(315, new int[] {40,316}),
      new State(316, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,41,-507}, new int[] {-141,317,-140,288,-138,329,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(317, new int[] {41,318}),
      new State(318, new int[] {61,319}),
      new State(319, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,320,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(320, new int[] {284,40,285,42,263,-331,265,-331,264,-331,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-331,41,-331,125,-331,58,-331,93,-331,44,-331,268,-331,338,-331}),
      new State(321, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,322,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(322, -336),
      new State(323, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306}, new int[] {-44,324,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(324, new int[] {59,-351,284,-351,285,-351,263,-351,265,-351,264,-351,124,-351,38,-351,94,-351,46,-351,43,-351,45,-351,42,-351,305,-351,47,-351,37,-351,293,-351,294,-351,287,-351,286,-351,289,-351,288,-351,60,-351,291,-351,62,-351,292,-351,290,-351,295,-351,63,-351,283,-351,41,-351,125,-351,58,-351,93,-351,44,-351,268,-351,338,-351,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(325, new int[] {91,-476,123,-476,369,-476,396,-476,390,-476,40,-479}),
      new State(326, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,93,-507}, new int[] {-141,327,-140,288,-138,329,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(327, new int[] {93,328}),
      new State(328, -446),
      new State(329, -510),
      new State(330, new int[] {40,331}),
      new State(331, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,41,-507}, new int[] {-141,332,-140,288,-138,329,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(332, new int[] {41,333}),
      new State(333, new int[] {61,319,44,-517,41,-517,93,-517}),
      new State(334, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306}, new int[] {-44,335,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(335, new int[] {59,-353,284,-353,285,-353,263,-353,265,-353,264,-353,124,-353,38,-353,94,-353,46,-353,43,-353,45,-353,42,-353,305,-353,47,-353,37,-353,293,-353,294,-353,287,-353,286,-353,289,-353,288,-353,60,-353,291,-353,62,-353,292,-353,290,-353,295,-353,63,-353,283,-353,41,-353,125,-353,58,-353,93,-353,44,-353,268,-353,338,-353,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(336, new int[] {91,308}),
      new State(337, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,338,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(338, new int[] {284,-371,285,-371,263,-371,265,-371,264,-371,124,-371,38,-371,94,-371,46,-371,43,-371,45,-371,42,-371,305,64,47,-371,37,-371,293,-371,294,-371,287,-371,286,-371,289,-371,288,-371,60,-371,291,-371,62,-371,292,-371,290,-371,295,-371,63,-371,283,-371,59,-371,41,-371,125,-371,58,-371,93,-371,44,-371,268,-371,338,-371}),
      new State(339, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,340,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(340, new int[] {284,-372,285,-372,263,-372,265,-372,264,-372,124,-372,38,-372,94,-372,46,-372,43,-372,45,-372,42,-372,305,64,47,-372,37,-372,293,-372,294,-372,287,-372,286,-372,289,-372,288,-372,60,-372,291,-372,62,-372,292,-372,290,-372,295,-372,63,-372,283,-372,59,-372,41,-372,125,-372,58,-372,93,-372,44,-372,268,-372,338,-372}),
      new State(341, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,342,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(342, new int[] {284,-373,285,-373,263,-373,265,-373,264,-373,124,-373,38,-373,94,-373,46,-373,43,-373,45,-373,42,-373,305,64,47,-373,37,-373,293,-373,294,-373,287,-373,286,-373,289,-373,288,-373,60,-373,291,-373,62,-373,292,-373,290,-373,295,92,63,-373,283,-373,59,-373,41,-373,125,-373,58,-373,93,-373,44,-373,268,-373,338,-373}),
      new State(343, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,344,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(344, new int[] {284,-374,285,-374,263,-374,265,-374,264,-374,124,-374,38,-374,94,-374,46,-374,43,-374,45,-374,42,-374,305,64,47,-374,37,-374,293,-374,294,-374,287,-374,286,-374,289,-374,288,-374,60,-374,291,-374,62,-374,292,-374,290,-374,295,-374,63,-374,283,-374,59,-374,41,-374,125,-374,58,-374,93,-374,44,-374,268,-374,338,-374}),
      new State(345, -386),
      new State(346, new int[] {353,297,319,187,391,188,393,191,320,97,36,98,361,354,293,507}, new int[] {-27,347,-145,350,-91,351,-28,94,-20,513,-120,184,-79,514,-49,556,-89,518}),
      new State(347, new int[] {40,128,59,-443,284,-443,285,-443,263,-443,265,-443,264,-443,124,-443,38,-443,94,-443,46,-443,43,-443,45,-443,42,-443,305,-443,47,-443,37,-443,293,-443,294,-443,287,-443,286,-443,289,-443,288,-443,60,-443,291,-443,62,-443,292,-443,290,-443,295,-443,63,-443,283,-443,41,-443,125,-443,58,-443,93,-443,44,-443,268,-443,338,-443}, new int[] {-129,348,-130,349}),
      new State(348, -328),
      new State(349, -444),
      new State(350, -329),
      new State(351, new int[] {361,354,293,507}, new int[] {-145,352,-89,353}),
      new State(352, -330),
      new State(353, -94),
      new State(354, new int[] {40,128,364,-443,365,-443,123,-443}, new int[] {-129,355,-130,349}),
      new State(355, new int[] {364,736,365,-190,123,-190}, new int[] {-26,356}),
      new State(356, -326, new int[] {-159,357}),
      new State(357, new int[] {365,734,123,-194}, new int[] {-31,358}),
      new State(358, -417, new int[] {-17,359}),
      new State(359, -418, new int[] {-18,360}),
      new State(360, new int[] {123,361}),
      new State(361, -273, new int[] {-102,362}),
      new State(362, new int[] {125,363,311,598,357,599,313,600,353,601,315,602,314,603,356,605,293,507,350,699,344,-301,346,-301}, new int[] {-84,365,-87,366,-9,367,-11,596,-12,604,-10,606,-91,697,-89,518}),
      new State(363, -419, new int[] {-19,364}),
      new State(364, -327),
      new State(365, -272),
      new State(366, -277),
      new State(367, new int[] {368,562,372,563,353,564,319,187,391,188,393,191,63,566,320,-246}, new int[] {-25,368,-24,586,-22,559,-20,565,-120,184,-33,568}),
      new State(368, new int[] {320,373}, new int[] {-111,369,-63,595}),
      new State(369, new int[] {59,370,44,371}),
      new State(370, -274),
      new State(371, new int[] {320,373}, new int[] {-63,372}),
      new State(372, -311),
      new State(373, new int[] {61,375,59,-417,44,-417}, new int[] {-17,374}),
      new State(374, -313),
      new State(375, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,376,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(376, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-417,44,-417}, new int[] {-17,377}),
      new State(377, -314),
      new State(378, -390),
      new State(379, new int[] {40,380}),
      new State(380, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-110,381,-42,594,-43,386,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(381, new int[] {44,384,41,-119}, new int[] {-3,382}),
      new State(382, new int[] {41,383}),
      new State(383, -532),
      new State(384, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,41,-120}, new int[] {-42,385,-43,386,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(385, -540),
      new State(386, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-541,41,-541}),
      new State(387, new int[] {40,388}),
      new State(388, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,389,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(389, new int[] {41,390,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(390, -533),
      new State(391, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,392,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(392, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-534,41,-534,125,-534,58,-534,93,-534,44,-534,268,-534,338,-534}),
      new State(393, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,394,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(394, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-535,41,-535,125,-535,58,-535,93,-535,44,-535,268,-535,338,-535}),
      new State(395, new int[] {40,396}),
      new State(396, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,397,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(397, new int[] {41,398,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(398, -536),
      new State(399, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,400,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(400, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-537,41,-537,125,-537,58,-537,93,-537,44,-537,268,-537,338,-537}),
      new State(401, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,402,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(402, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-538,41,-538,125,-538,58,-538,93,-538,44,-538,268,-538,338,-538}),
      new State(403, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,404,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(404, new int[] {284,-391,285,-391,263,-391,265,-391,264,-391,124,-391,38,-391,94,-391,46,-391,43,-391,45,-391,42,-391,305,64,47,-391,37,-391,293,-391,294,-391,287,-391,286,-391,289,-391,288,-391,60,-391,291,-391,62,-391,292,-391,290,-391,295,-391,63,-391,283,-391,59,-391,41,-391,125,-391,58,-391,93,-391,44,-391,268,-391,338,-391}),
      new State(405, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,406,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(406, new int[] {284,-392,285,-392,263,-392,265,-392,264,-392,124,-392,38,-392,94,-392,46,-392,43,-392,45,-392,42,-392,305,64,47,-392,37,-392,293,-392,294,-392,287,-392,286,-392,289,-392,288,-392,60,-392,291,-392,62,-392,292,-392,290,-392,295,-392,63,-392,283,-392,59,-392,41,-392,125,-392,58,-392,93,-392,44,-392,268,-392,338,-392}),
      new State(407, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,408,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(408, new int[] {284,-393,285,-393,263,-393,265,-393,264,-393,124,-393,38,-393,94,-393,46,-393,43,-393,45,-393,42,-393,305,64,47,-393,37,-393,293,-393,294,-393,287,-393,286,-393,289,-393,288,-393,60,-393,291,-393,62,-393,292,-393,290,-393,295,-393,63,-393,283,-393,59,-393,41,-393,125,-393,58,-393,93,-393,44,-393,268,-393,338,-393}),
      new State(409, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,410,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(410, new int[] {284,-394,285,-394,263,-394,265,-394,264,-394,124,-394,38,-394,94,-394,46,-394,43,-394,45,-394,42,-394,305,64,47,-394,37,-394,293,-394,294,-394,287,-394,286,-394,289,-394,288,-394,60,-394,291,-394,62,-394,292,-394,290,-394,295,-394,63,-394,283,-394,59,-394,41,-394,125,-394,58,-394,93,-394,44,-394,268,-394,338,-394}),
      new State(411, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,412,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(412, new int[] {284,-395,285,-395,263,-395,265,-395,264,-395,124,-395,38,-395,94,-395,46,-395,43,-395,45,-395,42,-395,305,64,47,-395,37,-395,293,-395,294,-395,287,-395,286,-395,289,-395,288,-395,60,-395,291,-395,62,-395,292,-395,290,-395,295,-395,63,-395,283,-395,59,-395,41,-395,125,-395,58,-395,93,-395,44,-395,268,-395,338,-395}),
      new State(413, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,414,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(414, new int[] {284,-396,285,-396,263,-396,265,-396,264,-396,124,-396,38,-396,94,-396,46,-396,43,-396,45,-396,42,-396,305,64,47,-396,37,-396,293,-396,294,-396,287,-396,286,-396,289,-396,288,-396,60,-396,291,-396,62,-396,292,-396,290,-396,295,-396,63,-396,283,-396,59,-396,41,-396,125,-396,58,-396,93,-396,44,-396,268,-396,338,-396}),
      new State(415, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,416,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(416, new int[] {284,-397,285,-397,263,-397,265,-397,264,-397,124,-397,38,-397,94,-397,46,-397,43,-397,45,-397,42,-397,305,64,47,-397,37,-397,293,-397,294,-397,287,-397,286,-397,289,-397,288,-397,60,-397,291,-397,62,-397,292,-397,290,-397,295,-397,63,-397,283,-397,59,-397,41,-397,125,-397,58,-397,93,-397,44,-397,268,-397,338,-397}),
      new State(417, new int[] {40,419,59,-438,284,-438,285,-438,263,-438,265,-438,264,-438,124,-438,38,-438,94,-438,46,-438,43,-438,45,-438,42,-438,305,-438,47,-438,37,-438,293,-438,294,-438,287,-438,286,-438,289,-438,288,-438,60,-438,291,-438,62,-438,292,-438,290,-438,295,-438,63,-438,283,-438,41,-438,125,-438,58,-438,93,-438,44,-438,268,-438,338,-438}, new int[] {-77,418}),
      new State(418, -398),
      new State(419, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,41,-469}, new int[] {-62,420,-43,120,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(420, new int[] {41,421}),
      new State(421, -439),
      new State(422, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,423,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(423, new int[] {284,-399,285,-399,263,-399,265,-399,264,-399,124,-399,38,-399,94,-399,46,-399,43,-399,45,-399,42,-399,305,64,47,-399,37,-399,293,-399,294,-399,287,-399,286,-399,289,-399,288,-399,60,-399,291,-399,62,-399,292,-399,290,-399,295,-399,63,-399,283,-399,59,-399,41,-399,125,-399,58,-399,93,-399,44,-399,268,-399,338,-399}),
      new State(424, -400),
      new State(425, -448),
      new State(426, -449),
      new State(427, -450),
      new State(428, -451),
      new State(429, -452),
      new State(430, -453),
      new State(431, -454),
      new State(432, -455),
      new State(433, -456),
      new State(434, -457),
      new State(435, new int[] {320,440,385,451,386,465,316,593}, new int[] {-109,436,-64,470}),
      new State(436, new int[] {34,437,316,439,320,440,385,451,386,465}, new int[] {-64,438}),
      new State(437, -458),
      new State(438, -518),
      new State(439, -519),
      new State(440, new int[] {91,441,369,449,396,450,34,-522,316,-522,320,-522,385,-522,386,-522,387,-522,96,-522}, new int[] {-21,447}),
      new State(441, new int[] {319,444,325,445,320,446}, new int[] {-65,442}),
      new State(442, new int[] {93,443}),
      new State(443, -523),
      new State(444, -529),
      new State(445, -530),
      new State(446, -531),
      new State(447, new int[] {319,448}),
      new State(448, -524),
      new State(449, -471),
      new State(450, -472),
      new State(451, new int[] {318,454,320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,452,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(452, new int[] {125,453,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(453, -525),
      new State(454, new int[] {125,455,91,456}),
      new State(455, -526),
      new State(456, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,457,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(457, new int[] {93,458,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(458, new int[] {125,459}),
      new State(459, -527),
      new State(460, new int[] {387,461,316,462,320,440,385,451,386,465}, new int[] {-109,468,-64,470}),
      new State(461, -459),
      new State(462, new int[] {387,463,320,440,385,451,386,465}, new int[] {-64,464}),
      new State(463, -460),
      new State(464, -521),
      new State(465, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306}, new int[] {-44,466,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(466, new int[] {125,467,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(467, -528),
      new State(468, new int[] {387,469,316,439,320,440,385,451,386,465}, new int[] {-64,438}),
      new State(469, -461),
      new State(470, -520),
      new State(471, -401),
      new State(472, new int[] {96,473,316,474,320,440,385,451,386,465}, new int[] {-109,476,-64,470}),
      new State(473, -440),
      new State(474, new int[] {96,475,320,440,385,451,386,465}, new int[] {-64,464}),
      new State(475, -441),
      new State(476, new int[] {96,477,316,439,320,440,385,451,386,465}, new int[] {-64,438}),
      new State(477, -442),
      new State(478, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,479,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(479, new int[] {284,40,285,42,263,-402,265,-402,264,-402,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-402,41,-402,125,-402,58,-402,93,-402,44,-402,268,-402,338,-402}),
      new State(480, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,59,-403,284,-403,285,-403,263,-403,265,-403,264,-403,124,-403,38,-403,94,-403,46,-403,42,-403,305,-403,47,-403,37,-403,294,-403,287,-403,286,-403,289,-403,288,-403,60,-403,291,-403,62,-403,292,-403,290,-403,295,-403,63,-403,283,-403,41,-403,125,-403,58,-403,93,-403,44,-403,268,-403,338,-403}, new int[] {-43,481,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(481, new int[] {268,482,284,40,285,42,263,-404,265,-404,264,-404,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-404,41,-404,125,-404,58,-404,93,-404,44,-404,338,-404}),
      new State(482, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,483,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(483, new int[] {284,40,285,42,263,-405,265,-405,264,-405,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-405,41,-405,125,-405,58,-405,93,-405,44,-405,268,-405,338,-405}),
      new State(484, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,485,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(485, new int[] {284,40,285,42,263,-406,265,-406,264,-406,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-406,41,-406,125,-406,58,-406,93,-406,44,-406,268,-406,338,-406}),
      new State(486, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,487,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(487, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-407,41,-407,125,-407,58,-407,93,-407,44,-407,268,-407,338,-407}),
      new State(488, -408),
      new State(489, -416),
      new State(490, new int[] {38,592,40,-422}, new int[] {-4,491}),
      new State(491, new int[] {40,492}),
      new State(492, new int[] {293,507,311,587,357,588,313,589,368,-240,372,-240,353,-240,319,-240,391,-240,393,-240,63,-240,38,-240,394,-240,320,-240,41,-235}, new int[] {-134,493,-135,571,-88,591,-91,575,-89,518,-132,590,-15,577}),
      new State(493, new int[] {41,494}),
      new State(494, new int[] {58,557,268,-257}, new int[] {-23,495}),
      new State(495, -417, new int[] {-17,496}),
      new State(496, new int[] {268,497}),
      new State(497, -420, new int[] {-154,498}),
      new State(498, -421, new int[] {-160,499}),
      new State(499, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,500,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(500, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-420,41,-420,125,-420,58,-420,93,-420,44,-420,268,-420,338,-420}, new int[] {-154,501}),
      new State(501, -414),
      new State(502, -415),
      new State(503, new int[] {353,505,346,489,343,502,293,507}, new int[] {-85,504,-89,353,-5,138,-6,490}),
      new State(504, -409),
      new State(505, new int[] {346,489,343,502}, new int[] {-85,506,-5,138,-6,490}),
      new State(506, -411),
      new State(507, new int[] {353,297,319,187,391,188,393,191,320,97,36,98}, new int[] {-90,508,-27,510,-28,94,-20,513,-120,184,-79,514,-49,556}),
      new State(508, new int[] {294,509}),
      new State(509, -92),
      new State(510, new int[] {40,128,294,-90}, new int[] {-92,511,-130,512}),
      new State(511, -91),
      new State(512, -89),
      new State(513, -435),
      new State(514, new int[] {91,515,123,544,390,554,369,449,396,450,59,-437,284,-437,285,-437,263,-437,265,-437,264,-437,124,-437,38,-437,94,-437,46,-437,43,-437,45,-437,42,-437,305,-437,47,-437,37,-437,293,-437,294,-437,287,-437,286,-437,289,-437,288,-437,60,-437,291,-437,62,-437,292,-437,290,-437,295,-437,63,-437,283,-437,41,-437,125,-437,58,-437,93,-437,44,-437,268,-437,338,-437,40,-437}, new int[] {-21,547}),
      new State(515, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,93,-469}, new int[] {-62,516,-43,120,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(516, new int[] {93,517}),
      new State(517, -495),
      new State(518, -93),
      new State(519, -412),
      new State(520, new int[] {40,521}),
      new State(521, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,522,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(522, new int[] {41,523,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(523, new int[] {123,524}),
      new State(524, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,342,538,125,-216}, new int[] {-95,525,-97,527,-94,543,-96,531,-43,537,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(525, new int[] {125,526}),
      new State(526, -215),
      new State(527, new int[] {44,529,125,-119}, new int[] {-3,528}),
      new State(528, -217),
      new State(529, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,342,538,125,-120}, new int[] {-94,530,-96,531,-43,537,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(530, -219),
      new State(531, new int[] {44,535,268,-119}, new int[] {-3,532}),
      new State(532, new int[] {268,533}),
      new State(533, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,534,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(534, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-220,125,-220}),
      new State(535, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,268,-120}, new int[] {-43,536,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(536, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-223,268,-223}),
      new State(537, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-222,268,-222}),
      new State(538, new int[] {44,542,268,-119}, new int[] {-3,539}),
      new State(539, new int[] {268,540}),
      new State(540, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,541,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(541, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-221,125,-221}),
      new State(542, -120),
      new State(543, -218),
      new State(544, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,545,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(545, new int[] {125,546,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(546, -496),
      new State(547, new int[] {319,549,123,550,320,97,36,98}, new int[] {-1,548,-49,553}),
      new State(548, -497),
      new State(549, -503),
      new State(550, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,551,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(551, new int[] {125,552,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(552, -504),
      new State(553, -505),
      new State(554, new int[] {320,97,36,98}, new int[] {-49,555}),
      new State(555, -499),
      new State(556, -494),
      new State(557, new int[] {368,562,372,563,353,564,319,187,391,188,393,191,63,566}, new int[] {-24,558,-22,559,-20,565,-120,184,-33,568}),
      new State(558, -258),
      new State(559, new int[] {124,560,320,-248,123,-248,268,-248,59,-248,38,-248,394,-248}),
      new State(560, new int[] {368,562,372,563,353,564,319,187,391,188,393,191}, new int[] {-22,561,-20,565,-120,184}),
      new State(561, -255),
      new State(562, -251),
      new State(563, -252),
      new State(564, -253),
      new State(565, -254),
      new State(566, new int[] {368,562,372,563,353,564,319,187,391,188,393,191}, new int[] {-22,567,-20,565,-120,184}),
      new State(567, -249),
      new State(568, new int[] {124,569,320,-250,123,-250,268,-250,59,-250,38,-250,394,-250}),
      new State(569, new int[] {368,562,372,563,353,564,319,187,391,188,393,191}, new int[] {-22,570,-20,565,-120,184}),
      new State(570, -256),
      new State(571, new int[] {44,573,41,-119}, new int[] {-3,572}),
      new State(572, -234),
      new State(573, new int[] {293,507,311,587,357,588,313,589,41,-120,368,-240,372,-240,353,-240,319,-240,391,-240,393,-240,63,-240,38,-240,394,-240,320,-240}, new int[] {-88,574,-91,575,-89,518,-132,590,-15,577}),
      new State(574, -237),
      new State(575, new int[] {311,587,357,588,313,589,293,507,368,-240,372,-240,353,-240,319,-240,391,-240,393,-240,63,-240,38,-240,394,-240,320,-240}, new int[] {-132,576,-89,353,-15,577}),
      new State(576, -238),
      new State(577, new int[] {368,562,372,563,353,564,319,187,391,188,393,191,63,566,38,-246,394,-246,320,-246}, new int[] {-25,578,-24,586,-22,559,-20,565,-120,184,-33,568}),
      new State(578, new int[] {38,585,394,-176,320,-176}, new int[] {-7,579}),
      new State(579, new int[] {394,584,320,-178}, new int[] {-8,580}),
      new State(580, new int[] {320,581}),
      new State(581, new int[] {61,582,44,-244,41,-244}),
      new State(582, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,583,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(583, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-245,41,-245}),
      new State(584, -179),
      new State(585, -177),
      new State(586, -247),
      new State(587, -241),
      new State(588, -242),
      new State(589, -243),
      new State(590, -239),
      new State(591, -236),
      new State(592, -423),
      new State(593, new int[] {320,440,385,451,386,465}, new int[] {-64,464}),
      new State(594, -539),
      new State(595, -312),
      new State(596, new int[] {311,598,357,599,313,600,353,601,315,602,314,603,368,-299,372,-299,319,-299,391,-299,393,-299,63,-299,320,-299,344,-302,346,-302}, new int[] {-12,597}),
      new State(597, -304),
      new State(598, -305),
      new State(599, -306),
      new State(600, -307),
      new State(601, -308),
      new State(602, -309),
      new State(603, -310),
      new State(604, -303),
      new State(605, -300),
      new State(606, new int[] {344,607,346,489}, new int[] {-5,617}),
      new State(607, new int[] {319,199,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,391,257,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276}, new int[] {-100,608,-70,616,-117,612,-118,200,-119,201}),
      new State(608, new int[] {59,609,44,610}),
      new State(609, -275),
      new State(610, new int[] {319,199,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,391,257,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276}, new int[] {-70,611,-117,612,-118,200,-119,201}),
      new State(611, -315),
      new State(612, new int[] {61,613}),
      new State(613, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,614,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(614, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-417,44,-417}, new int[] {-17,615}),
      new State(615, -317),
      new State(616, -316),
      new State(617, new int[] {38,592,319,-422,262,-422,261,-422,260,-422,259,-422,258,-422,263,-422,264,-422,265,-422,295,-422,306,-422,307,-422,326,-422,322,-422,308,-422,309,-422,310,-422,324,-422,329,-422,330,-422,327,-422,328,-422,333,-422,334,-422,331,-422,332,-422,337,-422,338,-422,349,-422,347,-422,351,-422,352,-422,350,-422,354,-422,355,-422,356,-422,360,-422,358,-422,359,-422,340,-422,345,-422,346,-422,344,-422,348,-422,266,-422,267,-422,367,-422,335,-422,336,-422,341,-422,342,-422,339,-422,368,-422,372,-422,364,-422,365,-422,391,-422,362,-422,366,-422,361,-422,373,-422,374,-422,376,-422,378,-422,370,-422,371,-422,375,-422,392,-422,343,-422,395,-422,353,-422,315,-422,314,-422,313,-422,357,-422,311,-422}, new int[] {-4,618}),
      new State(618, new int[] {319,199,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,391,257,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276}, new int[] {-117,619,-118,200,-119,201}),
      new State(619, -417, new int[] {-17,620}),
      new State(620, new int[] {40,621}),
      new State(621, new int[] {293,507,311,587,357,588,313,589,368,-240,372,-240,353,-240,319,-240,391,-240,393,-240,63,-240,38,-240,394,-240,320,-240,41,-235}, new int[] {-134,622,-135,571,-88,591,-91,575,-89,518,-132,590,-15,577}),
      new State(622, new int[] {41,623}),
      new State(623, new int[] {58,557,59,-257,123,-257}, new int[] {-23,624}),
      new State(624, -420, new int[] {-154,625}),
      new State(625, new int[] {59,628,123,629}, new int[] {-76,626}),
      new State(626, -420, new int[] {-154,627}),
      new State(627, -276),
      new State(628, -297),
      new State(629, -136, new int[] {-101,630}),
      new State(630, new int[] {125,631,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(631, -298),
      new State(632, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,59,-469}, new int[] {-62,633,-43,120,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(633, new int[] {59,634}),
      new State(634, -149),
      new State(635, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,59,-469}, new int[] {-62,636,-43,120,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(636, new int[] {59,637}),
      new State(637, -150),
      new State(638, new int[] {320,97,36,98}, new int[] {-104,639,-59,644,-49,643}),
      new State(639, new int[] {59,640,44,641}),
      new State(640, -151),
      new State(641, new int[] {320,97,36,98}, new int[] {-59,642,-49,643}),
      new State(642, -265),
      new State(643, -267),
      new State(644, -266),
      new State(645, new int[] {320,650,346,489,343,502,390,-434}, new int[] {-105,646,-85,137,-60,653,-5,138,-6,490}),
      new State(646, new int[] {59,647,44,648}),
      new State(647, -152),
      new State(648, new int[] {320,650}, new int[] {-60,649}),
      new State(649, -268),
      new State(650, new int[] {61,651,59,-270,44,-270}),
      new State(651, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,652,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(652, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-271,44,-271}),
      new State(653, -269),
      new State(654, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-106,655,-61,660,-43,659,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(655, new int[] {59,656,44,657}),
      new State(656, -153),
      new State(657, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-61,658,-43,659,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(658, -319),
      new State(659, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-321,44,-321}),
      new State(660, -320),
      new State(661, -154),
      new State(662, new int[] {59,663,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(663, -155),
      new State(664, new int[] {58,665,393,-84,40,-84,390,-84,91,-84,59,-84,284,-84,285,-84,263,-84,265,-84,264,-84,124,-84,38,-84,94,-84,46,-84,43,-84,45,-84,42,-84,305,-84,47,-84,37,-84,293,-84,294,-84,287,-84,286,-84,289,-84,288,-84,60,-84,291,-84,62,-84,292,-84,290,-84,295,-84,63,-84,283,-84}),
      new State(665, -163),
      new State(666, new int[] {38,592,319,-422,40,-422}, new int[] {-4,667}),
      new State(667, new int[] {319,668,40,-417}, new int[] {-17,140}),
      new State(668, -417, new int[] {-17,669}),
      new State(669, new int[] {40,670}),
      new State(670, new int[] {293,507,311,587,357,588,313,589,368,-240,372,-240,353,-240,319,-240,391,-240,393,-240,63,-240,38,-240,394,-240,320,-240,41,-235}, new int[] {-134,671,-135,571,-88,591,-91,575,-89,518,-132,590,-15,577}),
      new State(671, new int[] {41,672}),
      new State(672, new int[] {58,557,123,-257}, new int[] {-23,673}),
      new State(673, -420, new int[] {-154,674}),
      new State(674, -418, new int[] {-18,675}),
      new State(675, new int[] {123,676}),
      new State(676, -136, new int[] {-101,677}),
      new State(677, new int[] {125,678,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(678, -419, new int[] {-19,679}),
      new State(679, -420, new int[] {-154,680}),
      new State(680, -175),
      new State(681, new int[] {353,505,346,489,343,502,293,507,315,740,314,741,362,743,366,753,361,-182}, new int[] {-85,504,-89,353,-86,682,-5,666,-6,490,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(682, -139),
      new State(683, -95),
      new State(684, -96),
      new State(685, new int[] {361,686}),
      new State(686, new int[] {319,687}),
      new State(687, new int[] {364,736,365,-190,123,-190}, new int[] {-26,688}),
      new State(688, -180, new int[] {-155,689}),
      new State(689, new int[] {365,734,123,-194}, new int[] {-31,690}),
      new State(690, -417, new int[] {-17,691}),
      new State(691, -418, new int[] {-18,692}),
      new State(692, new int[] {123,693}),
      new State(693, -273, new int[] {-102,694}),
      new State(694, new int[] {125,695,311,598,357,599,313,600,353,601,315,602,314,603,356,605,293,507,350,699,344,-301,346,-301}, new int[] {-84,365,-87,366,-9,367,-11,596,-12,604,-10,606,-91,697,-89,518}),
      new State(695, -419, new int[] {-19,696}),
      new State(696, -181),
      new State(697, new int[] {311,598,357,599,313,600,353,601,315,602,314,603,356,605,293,507,344,-301,346,-301}, new int[] {-87,698,-89,353,-9,367,-11,596,-12,604,-10,606}),
      new State(698, -278),
      new State(699, new int[] {319,187,391,188,393,191}, new int[] {-29,700,-20,715,-120,184}),
      new State(700, new int[] {44,702,59,704,123,705}, new int[] {-81,701}),
      new State(701, -279),
      new State(702, new int[] {319,187,391,188,393,191}, new int[] {-20,703,-120,184}),
      new State(703, -281),
      new State(704, -282),
      new State(705, new int[] {125,706,319,719,391,720,393,191,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276}, new int[] {-108,707,-67,733,-68,710,-123,711,-20,716,-120,184,-69,721,-122,722,-117,732,-118,200,-119,201}),
      new State(706, -283),
      new State(707, new int[] {125,708,319,719,391,720,393,191,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276}, new int[] {-67,709,-68,710,-123,711,-20,716,-120,184,-69,721,-122,722,-117,732,-118,200,-119,201}),
      new State(708, -284),
      new State(709, -286),
      new State(710, -287),
      new State(711, new int[] {354,712,338,-295}),
      new State(712, new int[] {319,187,391,188,393,191}, new int[] {-29,713,-20,715,-120,184}),
      new State(713, new int[] {59,714,44,702}),
      new State(714, -289),
      new State(715, -280),
      new State(716, new int[] {390,717}),
      new State(717, new int[] {319,199,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,391,257,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276}, new int[] {-117,718,-118,200,-119,201}),
      new State(718, -296),
      new State(719, new int[] {393,-84,390,-84,338,-80}),
      new State(720, new int[] {393,189,338,-59}),
      new State(721, -288),
      new State(722, new int[] {338,723}),
      new State(723, new int[] {319,724,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,391,257,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,311,598,357,599,313,600,353,601,315,602,314,603}, new int[] {-119,726,-12,728}),
      new State(724, new int[] {59,725}),
      new State(725, -290),
      new State(726, new int[] {59,727}),
      new State(727, -291),
      new State(728, new int[] {59,731,319,199,262,202,261,203,260,204,259,205,258,206,263,207,264,208,265,209,295,210,306,211,307,212,326,213,322,214,308,215,309,216,310,217,324,218,329,219,330,220,327,221,328,222,333,223,334,224,331,225,332,226,337,227,338,228,349,229,347,230,351,231,352,232,350,233,354,234,355,235,356,236,360,237,358,238,359,239,340,240,345,241,346,242,344,243,348,244,266,245,267,246,367,247,335,248,336,249,341,250,342,251,339,252,368,253,372,254,364,255,365,256,391,257,362,258,366,259,361,260,373,261,374,262,376,263,378,264,370,265,371,266,375,267,392,268,343,269,395,270,353,271,315,272,314,273,313,274,357,275,311,276}, new int[] {-117,729,-118,200,-119,201}),
      new State(729, new int[] {59,730}),
      new State(730, -292),
      new State(731, -293),
      new State(732, -294),
      new State(733, -285),
      new State(734, new int[] {319,187,391,188,393,191}, new int[] {-29,735,-20,715,-120,184}),
      new State(735, new int[] {44,702,123,-195}),
      new State(736, new int[] {319,187,391,188,393,191}, new int[] {-20,737,-120,184}),
      new State(737, -191),
      new State(738, new int[] {315,740,314,741,361,-182}, new int[] {-14,739,-13,738}),
      new State(739, -183),
      new State(740, -184),
      new State(741, -185),
      new State(742, -97),
      new State(743, new int[] {319,744}),
      new State(744, -186, new int[] {-156,745}),
      new State(745, -417, new int[] {-17,746}),
      new State(746, -418, new int[] {-18,747}),
      new State(747, new int[] {123,748}),
      new State(748, -273, new int[] {-102,749}),
      new State(749, new int[] {125,750,311,598,357,599,313,600,353,601,315,602,314,603,356,605,293,507,350,699,344,-301,346,-301}, new int[] {-84,365,-87,366,-9,367,-11,596,-12,604,-10,606,-91,697,-89,518}),
      new State(750, -419, new int[] {-19,751}),
      new State(751, -187),
      new State(752, -98),
      new State(753, new int[] {319,754}),
      new State(754, -188, new int[] {-157,755}),
      new State(755, new int[] {364,763,123,-192}, new int[] {-32,756}),
      new State(756, -417, new int[] {-17,757}),
      new State(757, -418, new int[] {-18,758}),
      new State(758, new int[] {123,759}),
      new State(759, -273, new int[] {-102,760}),
      new State(760, new int[] {125,761,311,598,357,599,313,600,353,601,315,602,314,603,356,605,293,507,350,699,344,-301,346,-301}, new int[] {-84,365,-87,366,-9,367,-11,596,-12,604,-10,606,-91,697,-89,518}),
      new State(761, -419, new int[] {-19,762}),
      new State(762, -189),
      new State(763, new int[] {319,187,391,188,393,191}, new int[] {-29,764,-20,715,-120,184}),
      new State(764, new int[] {44,702,123,-193}),
      new State(765, new int[] {40,766}),
      new State(766, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306}, new int[] {-107,767,-58,774,-44,773,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(767, new int[] {44,771,41,-119}, new int[] {-3,768}),
      new State(768, new int[] {41,769}),
      new State(769, new int[] {59,770}),
      new State(770, -156),
      new State(771, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306,41,-120}, new int[] {-58,772,-44,773,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(772, -173),
      new State(773, new int[] {44,-174,41,-174,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(774, -172),
      new State(775, new int[] {40,776}),
      new State(776, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,777,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(777, new int[] {338,778,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(778, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,846,323,306,38,853,367,855}, new int[] {-144,779,-44,845,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(779, new int[] {41,780,268,839}),
      new State(780, -418, new int[] {-18,781}),
      new State(781, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,58,835,322,-418}, new int[] {-73,782,-35,784,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(782, -419, new int[] {-19,783}),
      new State(783, -157),
      new State(784, -202),
      new State(785, new int[] {40,786}),
      new State(786, new int[] {319,830}, new int[] {-99,787,-57,834}),
      new State(787, new int[] {41,788,44,828}),
      new State(788, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,58,824,322,-418}, new int[] {-66,789,-35,790,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(789, -159),
      new State(790, -204),
      new State(791, -160),
      new State(792, new int[] {123,793}),
      new State(793, -136, new int[] {-101,794}),
      new State(794, new int[] {125,795,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(795, -418, new int[] {-18,796}),
      new State(796, -164, new int[] {-115,797}),
      new State(797, new int[] {347,800,351,820,123,-170,330,-170,329,-170,328,-170,335,-170,339,-170,340,-170,348,-170,355,-170,353,-170,324,-170,321,-170,320,-170,36,-170,319,-170,391,-170,393,-170,40,-170,368,-170,91,-170,323,-170,367,-170,307,-170,303,-170,302,-170,43,-170,45,-170,33,-170,126,-170,306,-170,358,-170,359,-170,262,-170,261,-170,260,-170,259,-170,258,-170,301,-170,300,-170,299,-170,298,-170,297,-170,296,-170,304,-170,326,-170,64,-170,317,-170,312,-170,370,-170,371,-170,375,-170,374,-170,378,-170,376,-170,392,-170,373,-170,34,-170,383,-170,96,-170,266,-170,267,-170,269,-170,352,-170,346,-170,343,-170,293,-170,395,-170,360,-170,334,-170,332,-170,59,-170,349,-170,345,-170,315,-170,314,-170,362,-170,366,-170,363,-170,350,-170,344,-170,322,-170,361,-170,0,-170,125,-170,308,-170,309,-170,341,-170,342,-170,336,-170,337,-170,331,-170,333,-170,327,-170,310,-170}, new int[] {-78,798}),
      new State(798, -419, new int[] {-19,799}),
      new State(799, -161),
      new State(800, new int[] {40,801}),
      new State(801, new int[] {319,187,391,188,393,191}, new int[] {-30,802,-20,819,-120,184}),
      new State(802, new int[] {124,816,320,818,41,-166}, new int[] {-116,803}),
      new State(803, new int[] {41,804}),
      new State(804, new int[] {123,805}),
      new State(805, -136, new int[] {-101,806}),
      new State(806, new int[] {125,807,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(807, -165),
      new State(808, new int[] {319,809}),
      new State(809, new int[] {59,810}),
      new State(810, -162),
      new State(811, -138),
      new State(812, new int[] {40,813}),
      new State(813, new int[] {41,814}),
      new State(814, new int[] {59,815}),
      new State(815, -140),
      new State(816, new int[] {319,187,391,188,393,191}, new int[] {-20,817,-120,184}),
      new State(817, -169),
      new State(818, -167),
      new State(819, -168),
      new State(820, new int[] {123,821}),
      new State(821, -136, new int[] {-101,822}),
      new State(822, new int[] {125,823,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(823, -171),
      new State(824, -136, new int[] {-101,825}),
      new State(825, new int[] {337,826,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(826, new int[] {59,827}),
      new State(827, -205),
      new State(828, new int[] {319,830}, new int[] {-57,829}),
      new State(829, -133),
      new State(830, new int[] {61,831}),
      new State(831, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,832,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(832, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,41,-417,44,-417,59,-417}, new int[] {-17,833}),
      new State(833, -318),
      new State(834, -134),
      new State(835, -136, new int[] {-101,836}),
      new State(836, new int[] {331,837,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(837, new int[] {59,838}),
      new State(838, -203),
      new State(839, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,846,323,306,38,853,367,855}, new int[] {-144,840,-44,845,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(840, new int[] {41,841}),
      new State(841, -418, new int[] {-18,842}),
      new State(842, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,58,835,322,-418}, new int[] {-73,843,-35,784,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(843, -419, new int[] {-19,844}),
      new State(844, -158),
      new State(845, new int[] {41,-196,268,-196,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(846, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,93,-507}, new int[] {-141,847,-140,288,-138,329,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(847, new int[] {93,848}),
      new State(848, new int[] {91,-446,123,-446,369,-446,396,-446,390,-446,40,-446,41,-199,268,-199}),
      new State(849, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306}, new int[] {-44,850,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(850, new int[] {44,-514,41,-514,93,-514,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(851, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,852,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(852, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-515,41,-515,93,-515}),
      new State(853, new int[] {320,97,36,98,353,297,319,187,391,188,393,191,40,298,368,284,91,326,323,306}, new int[] {-44,854,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,325,-51,336,-53,311,-80,312}),
      new State(854, new int[] {41,-197,268,-197,91,-474,123,-474,369,-474,396,-474,390,-474}),
      new State(855, new int[] {40,856}),
      new State(856, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,41,-507}, new int[] {-141,857,-140,288,-138,329,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(857, new int[] {41,858}),
      new State(858, -198),
      new State(859, new int[] {40,860}),
      new State(860, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,330,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,38,849,394,851,44,-507,41,-507}, new int[] {-141,861,-140,288,-138,329,-139,291,-43,292,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(861, new int[] {41,862}),
      new State(862, new int[] {61,319,44,-516,41,-516,93,-516}),
      new State(863, -213),
      new State(864, -214),
      new State(865, new int[] {58,863,59,864}, new int[] {-158,866}),
      new State(866, -136, new int[] {-101,867}),
      new State(867, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,125,-212,341,-212,342,-212,336,-212,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(868, -210, new int[] {-112,869}),
      new State(869, new int[] {125,870,341,175,342,865}),
      new State(870, -207),
      new State(871, new int[] {59,875,336,-210,341,-210,342,-210}, new int[] {-112,872}),
      new State(872, new int[] {336,873,341,175,342,865}),
      new State(873, new int[] {59,874}),
      new State(874, -208),
      new State(875, -210, new int[] {-112,876}),
      new State(876, new int[] {336,877,341,175,342,865}),
      new State(877, new int[] {59,878}),
      new State(878, -209),
      new State(879, -136, new int[] {-101,880}),
      new State(880, new int[] {333,881,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(881, new int[] {59,882}),
      new State(882, -201),
      new State(883, new int[] {44,884,59,-323,41,-323}),
      new State(884, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,885,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(885, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-324,59,-324,41,-324}),
      new State(886, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-325,59,-325,41,-325}),
      new State(887, new int[] {40,888}),
      new State(888, new int[] {320,893,38,894}, new int[] {-137,889,-133,896}),
      new State(889, new int[] {41,890,44,891}),
      new State(890, -425),
      new State(891, new int[] {320,893,38,894}, new int[] {-133,892}),
      new State(892, -426),
      new State(893, -428),
      new State(894, new int[] {320,895}),
      new State(895, -429),
      new State(896, -427),
      new State(897, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,898,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(898, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,44,-264,41,-264}),
      new State(899, -261),
      new State(900, new int[] {319,549,123,550,320,97,36,98}, new int[] {-1,901,-49,553}),
      new State(901, new int[] {40,128,61,-488,270,-488,271,-488,279,-488,281,-488,278,-488,277,-488,276,-488,275,-488,274,-488,273,-488,272,-488,280,-488,282,-488,303,-488,302,-488,59,-488,284,-488,285,-488,263,-488,265,-488,264,-488,124,-488,38,-488,94,-488,46,-488,43,-488,45,-488,42,-488,305,-488,47,-488,37,-488,293,-488,294,-488,287,-488,286,-488,289,-488,288,-488,60,-488,291,-488,62,-488,292,-488,290,-488,295,-488,63,-488,283,-488,91,-488,123,-488,369,-488,396,-488,390,-488,41,-488,125,-488,58,-488,93,-488,44,-488,268,-488,338,-488}, new int[] {-130,902}),
      new State(902, -484),
      new State(903, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,904,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(904, new int[] {125,905,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(905, -483),
      new State(906, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,907,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(907, new int[] {284,40,285,42,263,-337,265,-337,264,-337,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-337,41,-337,125,-337,58,-337,93,-337,44,-337,268,-337,338,-337}),
      new State(908, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,909,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(909, new int[] {284,40,285,42,263,-338,265,-338,264,-338,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-338,41,-338,125,-338,58,-338,93,-338,44,-338,268,-338,338,-338}),
      new State(910, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,911,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(911, new int[] {284,40,285,42,263,-339,265,-339,264,-339,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-339,41,-339,125,-339,58,-339,93,-339,44,-339,268,-339,338,-339}),
      new State(912, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,913,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(913, new int[] {284,40,285,42,263,-340,265,-340,264,-340,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-340,41,-340,125,-340,58,-340,93,-340,44,-340,268,-340,338,-340}),
      new State(914, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,915,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(915, new int[] {284,40,285,42,263,-341,265,-341,264,-341,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-341,41,-341,125,-341,58,-341,93,-341,44,-341,268,-341,338,-341}),
      new State(916, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,917,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(917, new int[] {284,40,285,42,263,-342,265,-342,264,-342,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-342,41,-342,125,-342,58,-342,93,-342,44,-342,268,-342,338,-342}),
      new State(918, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,919,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(919, new int[] {284,40,285,42,263,-343,265,-343,264,-343,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-343,41,-343,125,-343,58,-343,93,-343,44,-343,268,-343,338,-343}),
      new State(920, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,921,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(921, new int[] {284,40,285,42,263,-344,265,-344,264,-344,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-344,41,-344,125,-344,58,-344,93,-344,44,-344,268,-344,338,-344}),
      new State(922, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,923,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(923, new int[] {284,40,285,42,263,-345,265,-345,264,-345,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-345,41,-345,125,-345,58,-345,93,-345,44,-345,268,-345,338,-345}),
      new State(924, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,925,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(925, new int[] {284,40,285,42,263,-346,265,-346,264,-346,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-346,41,-346,125,-346,58,-346,93,-346,44,-346,268,-346,338,-346}),
      new State(926, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,927,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(927, new int[] {284,40,285,42,263,-347,265,-347,264,-347,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-347,41,-347,125,-347,58,-347,93,-347,44,-347,268,-347,338,-347}),
      new State(928, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,929,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(929, new int[] {284,40,285,42,263,-348,265,-348,264,-348,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-348,41,-348,125,-348,58,-348,93,-348,44,-348,268,-348,338,-348}),
      new State(930, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,931,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(931, new int[] {284,40,285,42,263,-349,265,-349,264,-349,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106,59,-349,41,-349,125,-349,58,-349,93,-349,44,-349,268,-349,338,-349}),
      new State(932, -350),
      new State(933, -352),
      new State(934, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,935,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(935, new int[] {284,40,285,42,263,-388,265,-388,264,-388,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,-388,283,106,59,-388,41,-388,125,-388,58,-388,93,-388,44,-388,268,-388,338,-388}),
      new State(936, -491),
      new State(937, -136, new int[] {-101,938}),
      new State(938, new int[] {327,939,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(939, new int[] {59,940}),
      new State(940, -225),
      new State(941, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,322,-418}, new int[] {-35,942,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(942, -229),
      new State(943, new int[] {40,944}),
      new State(944, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,945,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(945, new int[] {41,946,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(946, new int[] {58,948,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,322,-418}, new int[] {-35,947,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(947, -226),
      new State(948, -136, new int[] {-101,949}),
      new State(949, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,310,-230,308,-230,309,-230,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(950, new int[] {310,951,308,953,309,959}),
      new State(951, new int[] {59,952}),
      new State(952, -232),
      new State(953, new int[] {40,954}),
      new State(954, new int[] {320,97,36,98,353,136,319,187,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520}, new int[] {-43,955,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,138,-6,490,-91,503,-89,518,-93,519}),
      new State(955, new int[] {41,956,284,40,285,42,263,44,265,46,264,48,124,50,38,52,94,54,46,56,43,58,45,60,42,62,305,64,47,66,37,68,293,70,294,72,287,74,286,76,289,78,288,80,60,82,291,84,62,86,292,88,290,90,295,92,63,102,283,106}),
      new State(956, new int[] {58,957}),
      new State(957, -136, new int[] {-101,958}),
      new State(958, new int[] {123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,310,-231,308,-231,309,-231,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(959, new int[] {58,960}),
      new State(960, -136, new int[] {-101,961}),
      new State(961, new int[] {310,962,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,188,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,812,322,-418,361,-182}, new int[] {-83,10,-35,11,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,681,-89,518,-93,519,-86,811,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(962, new int[] {59,963}),
      new State(963, -233),
      new State(964, new int[] {393,189,319,187,123,-417}, new int[] {-120,965,-17,1038}),
      new State(965, new int[] {59,966,393,185,123,-417}, new int[] {-17,967}),
      new State(966, -103),
      new State(967, -104, new int[] {-152,968}),
      new State(968, new int[] {123,969}),
      new State(969, -83, new int[] {-98,970}),
      new State(970, new int[] {125,971,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,964,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,975,350,979,344,1035,322,-418,361,-182}, new int[] {-34,5,-35,6,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,972,-89,518,-93,519,-86,974,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(971, -105),
      new State(972, new int[] {353,505,346,489,343,502,293,507,315,740,314,741,362,743,366,753,361,-182}, new int[] {-85,504,-89,353,-86,973,-5,666,-6,490,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(973, -101),
      new State(974, -100),
      new State(975, new int[] {40,976}),
      new State(976, new int[] {41,977}),
      new State(977, new int[] {59,978}),
      new State(978, -102),
      new State(979, new int[] {319,187,393,1028,346,1025,344,1026}, new int[] {-148,980,-16,982,-146,1012,-120,1014,-124,1011,-121,989}),
      new State(980, new int[] {59,981}),
      new State(981, -108),
      new State(982, new int[] {319,187,393,1004}, new int[] {-147,983,-146,985,-120,995,-124,1011,-121,989}),
      new State(983, new int[] {59,984}),
      new State(984, -109),
      new State(985, new int[] {59,986,44,987}),
      new State(986, -111),
      new State(987, new int[] {319,187,393,993}, new int[] {-124,988,-121,989,-120,990}),
      new State(988, -125),
      new State(989, -131),
      new State(990, new int[] {393,185,338,991,59,-129,44,-129,125,-129}),
      new State(991, new int[] {319,992}),
      new State(992, -130),
      new State(993, new int[] {319,187}, new int[] {-121,994,-120,990}),
      new State(994, -132),
      new State(995, new int[] {393,996,338,991,59,-129,44,-129}),
      new State(996, new int[] {123,997,319,186}),
      new State(997, new int[] {319,187}, new int[] {-125,998,-121,1003,-120,990}),
      new State(998, new int[] {44,1001,125,-119}, new int[] {-3,999}),
      new State(999, new int[] {125,1000}),
      new State(1000, -115),
      new State(1001, new int[] {319,187,125,-120}, new int[] {-121,1002,-120,990}),
      new State(1002, -123),
      new State(1003, -124),
      new State(1004, new int[] {319,187}, new int[] {-120,1005,-121,994}),
      new State(1005, new int[] {393,1006,338,991,59,-129,44,-129}),
      new State(1006, new int[] {123,1007,319,186}),
      new State(1007, new int[] {319,187}, new int[] {-125,1008,-121,1003,-120,990}),
      new State(1008, new int[] {44,1001,125,-119}, new int[] {-3,1009}),
      new State(1009, new int[] {125,1010}),
      new State(1010, -116),
      new State(1011, -126),
      new State(1012, new int[] {59,1013,44,987}),
      new State(1013, -110),
      new State(1014, new int[] {393,1015,338,991,59,-129,44,-129}),
      new State(1015, new int[] {123,1016,319,186}),
      new State(1016, new int[] {319,187,346,1025,344,1026}, new int[] {-127,1017,-126,1027,-121,1022,-120,990,-16,1023}),
      new State(1017, new int[] {44,1020,125,-119}, new int[] {-3,1018}),
      new State(1018, new int[] {125,1019}),
      new State(1019, -117),
      new State(1020, new int[] {319,187,346,1025,344,1026,125,-120}, new int[] {-126,1021,-121,1022,-120,990,-16,1023}),
      new State(1021, -121),
      new State(1022, -127),
      new State(1023, new int[] {319,187}, new int[] {-121,1024,-120,990}),
      new State(1024, -128),
      new State(1025, -113),
      new State(1026, -114),
      new State(1027, -122),
      new State(1028, new int[] {319,187}, new int[] {-120,1029,-121,994}),
      new State(1029, new int[] {393,1030,338,991,59,-129,44,-129}),
      new State(1030, new int[] {123,1031,319,186}),
      new State(1031, new int[] {319,187,346,1025,344,1026}, new int[] {-127,1032,-126,1027,-121,1022,-120,990,-16,1023}),
      new State(1032, new int[] {44,1020,125,-119}, new int[] {-3,1033}),
      new State(1033, new int[] {125,1034}),
      new State(1034, -118),
      new State(1035, new int[] {319,830}, new int[] {-99,1036,-57,834}),
      new State(1036, new int[] {59,1037,44,828}),
      new State(1037, -112),
      new State(1038, -106, new int[] {-153,1039}),
      new State(1039, new int[] {123,1040}),
      new State(1040, -83, new int[] {-98,1041}),
      new State(1041, new int[] {125,1042,123,7,330,23,329,31,328,153,335,165,339,179,340,632,348,635,355,638,353,645,324,654,321,661,320,97,36,98,319,664,391,964,393,191,40,280,368,284,91,301,323,306,367,315,307,321,303,323,302,334,43,337,45,339,33,341,126,343,306,346,358,379,359,387,262,391,261,393,260,395,259,399,258,401,301,403,300,405,299,407,298,409,297,411,296,413,304,415,326,417,64,422,317,425,312,426,370,427,371,428,375,429,374,430,378,431,376,432,392,433,373,434,34,435,383,460,96,472,266,478,267,480,269,484,352,486,346,489,343,502,293,507,395,520,360,765,334,775,332,785,59,791,349,792,345,808,315,740,314,741,362,743,366,753,363,975,350,979,344,1035,322,-418,361,-182}, new int[] {-34,5,-35,6,-18,12,-43,662,-44,108,-48,114,-49,115,-71,116,-54,121,-28,122,-20,182,-120,184,-82,193,-52,283,-51,307,-53,311,-80,312,-45,314,-46,345,-47,378,-50,424,-75,471,-85,488,-5,666,-6,490,-91,972,-89,518,-93,519,-86,974,-36,683,-37,684,-14,685,-13,738,-38,742,-40,752}),
      new State(1042, -107),
    };
    #endregion

    #region rules
    rules = new Rule[]
    {
    default(Rule),
    new Rule(-150, new int[]{-149,0}),
    new Rule(-151, new int[]{}),
    new Rule(-149, new int[]{-151,-98}),
    new Rule(-119, new int[]{262}),
    new Rule(-119, new int[]{261}),
    new Rule(-119, new int[]{260}),
    new Rule(-119, new int[]{259}),
    new Rule(-119, new int[]{258}),
    new Rule(-119, new int[]{263}),
    new Rule(-119, new int[]{264}),
    new Rule(-119, new int[]{265}),
    new Rule(-119, new int[]{295}),
    new Rule(-119, new int[]{306}),
    new Rule(-119, new int[]{307}),
    new Rule(-119, new int[]{326}),
    new Rule(-119, new int[]{322}),
    new Rule(-119, new int[]{308}),
    new Rule(-119, new int[]{309}),
    new Rule(-119, new int[]{310}),
    new Rule(-119, new int[]{324}),
    new Rule(-119, new int[]{329}),
    new Rule(-119, new int[]{330}),
    new Rule(-119, new int[]{327}),
    new Rule(-119, new int[]{328}),
    new Rule(-119, new int[]{333}),
    new Rule(-119, new int[]{334}),
    new Rule(-119, new int[]{331}),
    new Rule(-119, new int[]{332}),
    new Rule(-119, new int[]{337}),
    new Rule(-119, new int[]{338}),
    new Rule(-119, new int[]{349}),
    new Rule(-119, new int[]{347}),
    new Rule(-119, new int[]{351}),
    new Rule(-119, new int[]{352}),
    new Rule(-119, new int[]{350}),
    new Rule(-119, new int[]{354}),
    new Rule(-119, new int[]{355}),
    new Rule(-119, new int[]{356}),
    new Rule(-119, new int[]{360}),
    new Rule(-119, new int[]{358}),
    new Rule(-119, new int[]{359}),
    new Rule(-119, new int[]{340}),
    new Rule(-119, new int[]{345}),
    new Rule(-119, new int[]{346}),
    new Rule(-119, new int[]{344}),
    new Rule(-119, new int[]{348}),
    new Rule(-119, new int[]{266}),
    new Rule(-119, new int[]{267}),
    new Rule(-119, new int[]{367}),
    new Rule(-119, new int[]{335}),
    new Rule(-119, new int[]{336}),
    new Rule(-119, new int[]{341}),
    new Rule(-119, new int[]{342}),
    new Rule(-119, new int[]{339}),
    new Rule(-119, new int[]{368}),
    new Rule(-119, new int[]{372}),
    new Rule(-119, new int[]{364}),
    new Rule(-119, new int[]{365}),
    new Rule(-119, new int[]{391}),
    new Rule(-119, new int[]{362}),
    new Rule(-119, new int[]{366}),
    new Rule(-119, new int[]{361}),
    new Rule(-119, new int[]{373}),
    new Rule(-119, new int[]{374}),
    new Rule(-119, new int[]{376}),
    new Rule(-119, new int[]{378}),
    new Rule(-119, new int[]{370}),
    new Rule(-119, new int[]{371}),
    new Rule(-119, new int[]{375}),
    new Rule(-119, new int[]{392}),
    new Rule(-119, new int[]{343}),
    new Rule(-119, new int[]{395}),
    new Rule(-118, new int[]{-119}),
    new Rule(-118, new int[]{353}),
    new Rule(-118, new int[]{315}),
    new Rule(-118, new int[]{314}),
    new Rule(-118, new int[]{313}),
    new Rule(-118, new int[]{357}),
    new Rule(-118, new int[]{311}),
    new Rule(-117, new int[]{319}),
    new Rule(-117, new int[]{-118}),
    new Rule(-98, new int[]{-98,-34}),
    new Rule(-98, new int[]{}),
    new Rule(-120, new int[]{319}),
    new Rule(-120, new int[]{-120,393,319}),
    new Rule(-20, new int[]{-120}),
    new Rule(-20, new int[]{391,393,-120}),
    new Rule(-20, new int[]{393,-120}),
    new Rule(-92, new int[]{-130}),
    new Rule(-90, new int[]{-27}),
    new Rule(-90, new int[]{-27,-92}),
    new Rule(-89, new int[]{293,-90,294}),
    new Rule(-91, new int[]{-89}),
    new Rule(-91, new int[]{-91,-89}),
    new Rule(-86, new int[]{-36}),
    new Rule(-86, new int[]{-37}),
    new Rule(-86, new int[]{-38}),
    new Rule(-86, new int[]{-40}),
    new Rule(-34, new int[]{-35}),
    new Rule(-34, new int[]{-86}),
    new Rule(-34, new int[]{-91,-86}),
    new Rule(-34, new int[]{363,40,41,59}),
    new Rule(-34, new int[]{391,-120,59}),
    new Rule(-152, new int[]{}),
    new Rule(-34, new int[]{391,-120,-17,-152,123,-98,125}),
    new Rule(-153, new int[]{}),
    new Rule(-34, new int[]{391,-17,-153,123,-98,125}),
    new Rule(-34, new int[]{350,-148,59}),
    new Rule(-34, new int[]{350,-16,-147,59}),
    new Rule(-34, new int[]{350,-146,59}),
    new Rule(-34, new int[]{350,-16,-146,59}),
    new Rule(-34, new int[]{344,-99,59}),
    new Rule(-16, new int[]{346}),
    new Rule(-16, new int[]{344}),
    new Rule(-147, new int[]{-120,393,123,-125,-3,125}),
    new Rule(-147, new int[]{393,-120,393,123,-125,-3,125}),
    new Rule(-148, new int[]{-120,393,123,-127,-3,125}),
    new Rule(-148, new int[]{393,-120,393,123,-127,-3,125}),
    new Rule(-3, new int[]{}),
    new Rule(-3, new int[]{44}),
    new Rule(-127, new int[]{-127,44,-126}),
    new Rule(-127, new int[]{-126}),
    new Rule(-125, new int[]{-125,44,-121}),
    new Rule(-125, new int[]{-121}),
    new Rule(-146, new int[]{-146,44,-124}),
    new Rule(-146, new int[]{-124}),
    new Rule(-126, new int[]{-121}),
    new Rule(-126, new int[]{-16,-121}),
    new Rule(-121, new int[]{-120}),
    new Rule(-121, new int[]{-120,338,319}),
    new Rule(-124, new int[]{-121}),
    new Rule(-124, new int[]{393,-121}),
    new Rule(-99, new int[]{-99,44,-57}),
    new Rule(-99, new int[]{-57}),
    new Rule(-101, new int[]{-101,-83}),
    new Rule(-101, new int[]{}),
    new Rule(-83, new int[]{-35}),
    new Rule(-83, new int[]{-86}),
    new Rule(-83, new int[]{-91,-86}),
    new Rule(-83, new int[]{363,40,41,59}),
    new Rule(-35, new int[]{123,-101,125}),
    new Rule(-35, new int[]{-18,-55,-19}),
    new Rule(-35, new int[]{-18,-56,-19}),
    new Rule(-35, new int[]{330,40,-43,41,-18,-74,-19}),
    new Rule(-35, new int[]{329,-18,-35,330,40,-43,41,59,-19}),
    new Rule(-35, new int[]{328,40,-103,59,-103,59,-103,41,-18,-72,-19}),
    new Rule(-35, new int[]{335,40,-43,41,-18,-113,-19}),
    new Rule(-35, new int[]{339,-62,59}),
    new Rule(-35, new int[]{340,-62,59}),
    new Rule(-35, new int[]{348,-62,59}),
    new Rule(-35, new int[]{355,-104,59}),
    new Rule(-35, new int[]{353,-105,59}),
    new Rule(-35, new int[]{324,-106,59}),
    new Rule(-35, new int[]{321}),
    new Rule(-35, new int[]{-43,59}),
    new Rule(-35, new int[]{360,40,-107,-3,41,59}),
    new Rule(-35, new int[]{334,40,-43,338,-144,41,-18,-73,-19}),
    new Rule(-35, new int[]{334,40,-43,338,-144,268,-144,41,-18,-73,-19}),
    new Rule(-35, new int[]{332,40,-99,41,-66}),
    new Rule(-35, new int[]{59}),
    new Rule(-35, new int[]{349,123,-101,125,-18,-115,-78,-19}),
    new Rule(-35, new int[]{345,319,59}),
    new Rule(-35, new int[]{319,58}),
    new Rule(-115, new int[]{}),
    new Rule(-115, new int[]{-115,347,40,-30,-116,41,123,-101,125}),
    new Rule(-116, new int[]{}),
    new Rule(-116, new int[]{320}),
    new Rule(-30, new int[]{-20}),
    new Rule(-30, new int[]{-30,124,-20}),
    new Rule(-78, new int[]{}),
    new Rule(-78, new int[]{351,123,-101,125}),
    new Rule(-107, new int[]{-58}),
    new Rule(-107, new int[]{-107,44,-58}),
    new Rule(-58, new int[]{-44}),
    new Rule(-36, new int[]{-5,-4,319,-17,40,-134,41,-23,-154,-18,123,-101,125,-19,-154}),
    new Rule(-7, new int[]{}),
    new Rule(-7, new int[]{38}),
    new Rule(-8, new int[]{}),
    new Rule(-8, new int[]{394}),
    new Rule(-155, new int[]{}),
    new Rule(-37, new int[]{-14,361,319,-26,-155,-31,-17,-18,123,-102,125,-19}),
    new Rule(-14, new int[]{}),
    new Rule(-14, new int[]{-13,-14}),
    new Rule(-13, new int[]{315}),
    new Rule(-13, new int[]{314}),
    new Rule(-156, new int[]{}),
    new Rule(-38, new int[]{362,319,-156,-17,-18,123,-102,125,-19}),
    new Rule(-157, new int[]{}),
    new Rule(-40, new int[]{366,319,-157,-32,-17,-18,123,-102,125,-19}),
    new Rule(-26, new int[]{}),
    new Rule(-26, new int[]{364,-20}),
    new Rule(-32, new int[]{}),
    new Rule(-32, new int[]{364,-29}),
    new Rule(-31, new int[]{}),
    new Rule(-31, new int[]{365,-29}),
    new Rule(-144, new int[]{-44}),
    new Rule(-144, new int[]{38,-44}),
    new Rule(-144, new int[]{367,40,-141,41}),
    new Rule(-144, new int[]{91,-141,93}),
    new Rule(-72, new int[]{-35}),
    new Rule(-72, new int[]{58,-101,333,59}),
    new Rule(-73, new int[]{-35}),
    new Rule(-73, new int[]{58,-101,331,59}),
    new Rule(-66, new int[]{-35}),
    new Rule(-66, new int[]{58,-101,337,59}),
    new Rule(-113, new int[]{123,-112,125}),
    new Rule(-113, new int[]{123,59,-112,125}),
    new Rule(-113, new int[]{58,-112,336,59}),
    new Rule(-113, new int[]{58,59,-112,336,59}),
    new Rule(-112, new int[]{}),
    new Rule(-112, new int[]{-112,341,-43,-158,-101}),
    new Rule(-112, new int[]{-112,342,-158,-101}),
    new Rule(-158, new int[]{58}),
    new Rule(-158, new int[]{59}),
    new Rule(-93, new int[]{395,40,-43,41,123,-95,125}),
    new Rule(-95, new int[]{}),
    new Rule(-95, new int[]{-97,-3}),
    new Rule(-97, new int[]{-94}),
    new Rule(-97, new int[]{-97,44,-94}),
    new Rule(-94, new int[]{-96,-3,268,-43}),
    new Rule(-94, new int[]{342,-3,268,-43}),
    new Rule(-96, new int[]{-43}),
    new Rule(-96, new int[]{-96,44,-43}),
    new Rule(-74, new int[]{-35}),
    new Rule(-74, new int[]{58,-101,327,59}),
    new Rule(-142, new int[]{322,40,-43,41,-35}),
    new Rule(-142, new int[]{-142,308,40,-43,41,-35}),
    new Rule(-55, new int[]{-142}),
    new Rule(-55, new int[]{-142,309,-35}),
    new Rule(-143, new int[]{322,40,-43,41,58,-101}),
    new Rule(-143, new int[]{-143,308,40,-43,41,58,-101}),
    new Rule(-56, new int[]{-143,310,59}),
    new Rule(-56, new int[]{-143,309,58,-101,310,59}),
    new Rule(-134, new int[]{-135,-3}),
    new Rule(-134, new int[]{}),
    new Rule(-135, new int[]{-88}),
    new Rule(-135, new int[]{-135,44,-88}),
    new Rule(-88, new int[]{-91,-132}),
    new Rule(-88, new int[]{-132}),
    new Rule(-15, new int[]{}),
    new Rule(-15, new int[]{311}),
    new Rule(-15, new int[]{357}),
    new Rule(-15, new int[]{313}),
    new Rule(-132, new int[]{-15,-25,-7,-8,320}),
    new Rule(-132, new int[]{-15,-25,-7,-8,320,61,-43}),
    new Rule(-25, new int[]{}),
    new Rule(-25, new int[]{-24}),
    new Rule(-24, new int[]{-22}),
    new Rule(-24, new int[]{63,-22}),
    new Rule(-24, new int[]{-33}),
    new Rule(-22, new int[]{368}),
    new Rule(-22, new int[]{372}),
    new Rule(-22, new int[]{353}),
    new Rule(-22, new int[]{-20}),
    new Rule(-33, new int[]{-22,124,-22}),
    new Rule(-33, new int[]{-33,124,-22}),
    new Rule(-23, new int[]{}),
    new Rule(-23, new int[]{58,-24}),
    new Rule(-130, new int[]{40,41}),
    new Rule(-130, new int[]{40,-131,-3,41}),
    new Rule(-131, new int[]{-128}),
    new Rule(-131, new int[]{-131,44,-128}),
    new Rule(-128, new int[]{-43}),
    new Rule(-128, new int[]{394,-43}),
    new Rule(-104, new int[]{-104,44,-59}),
    new Rule(-104, new int[]{-59}),
    new Rule(-59, new int[]{-49}),
    new Rule(-105, new int[]{-105,44,-60}),
    new Rule(-105, new int[]{-60}),
    new Rule(-60, new int[]{320}),
    new Rule(-60, new int[]{320,61,-43}),
    new Rule(-102, new int[]{-102,-84}),
    new Rule(-102, new int[]{}),
    new Rule(-87, new int[]{-9,-25,-111,59}),
    new Rule(-87, new int[]{-10,344,-100,59}),
    new Rule(-87, new int[]{-10,-5,-4,-117,-17,40,-134,41,-23,-154,-76,-154}),
    new Rule(-84, new int[]{-87}),
    new Rule(-84, new int[]{-91,-87}),
    new Rule(-84, new int[]{350,-29,-81}),
    new Rule(-29, new int[]{-20}),
    new Rule(-29, new int[]{-29,44,-20}),
    new Rule(-81, new int[]{59}),
    new Rule(-81, new int[]{123,125}),
    new Rule(-81, new int[]{123,-108,125}),
    new Rule(-108, new int[]{-67}),
    new Rule(-108, new int[]{-108,-67}),
    new Rule(-67, new int[]{-68}),
    new Rule(-67, new int[]{-69}),
    new Rule(-68, new int[]{-123,354,-29,59}),
    new Rule(-69, new int[]{-122,338,319,59}),
    new Rule(-69, new int[]{-122,338,-119,59}),
    new Rule(-69, new int[]{-122,338,-12,-117,59}),
    new Rule(-69, new int[]{-122,338,-12,59}),
    new Rule(-122, new int[]{-117}),
    new Rule(-122, new int[]{-123}),
    new Rule(-123, new int[]{-20,390,-117}),
    new Rule(-76, new int[]{59}),
    new Rule(-76, new int[]{123,-101,125}),
    new Rule(-9, new int[]{-11}),
    new Rule(-9, new int[]{356}),
    new Rule(-10, new int[]{}),
    new Rule(-10, new int[]{-11}),
    new Rule(-11, new int[]{-12}),
    new Rule(-11, new int[]{-11,-12}),
    new Rule(-12, new int[]{311}),
    new Rule(-12, new int[]{357}),
    new Rule(-12, new int[]{313}),
    new Rule(-12, new int[]{353}),
    new Rule(-12, new int[]{315}),
    new Rule(-12, new int[]{314}),
    new Rule(-111, new int[]{-111,44,-63}),
    new Rule(-111, new int[]{-63}),
    new Rule(-63, new int[]{320,-17}),
    new Rule(-63, new int[]{320,61,-43,-17}),
    new Rule(-100, new int[]{-100,44,-70}),
    new Rule(-100, new int[]{-70}),
    new Rule(-70, new int[]{-117,61,-43,-17}),
    new Rule(-57, new int[]{319,61,-43,-17}),
    new Rule(-106, new int[]{-106,44,-61}),
    new Rule(-106, new int[]{-61}),
    new Rule(-61, new int[]{-43}),
    new Rule(-103, new int[]{}),
    new Rule(-103, new int[]{-114}),
    new Rule(-114, new int[]{-114,44,-43}),
    new Rule(-114, new int[]{-43}),
    new Rule(-159, new int[]{}),
    new Rule(-145, new int[]{361,-129,-26,-159,-31,-17,-18,123,-102,125,-19}),
    new Rule(-46, new int[]{306,-27,-129}),
    new Rule(-46, new int[]{306,-145}),
    new Rule(-46, new int[]{306,-91,-145}),
    new Rule(-45, new int[]{367,40,-141,41,61,-43}),
    new Rule(-45, new int[]{91,-141,93,61,-43}),
    new Rule(-45, new int[]{-44,61,-43}),
    new Rule(-45, new int[]{-44,61,38,-44}),
    new Rule(-45, new int[]{-44,61,38,-46}),
    new Rule(-45, new int[]{307,-43}),
    new Rule(-45, new int[]{-44,270,-43}),
    new Rule(-45, new int[]{-44,271,-43}),
    new Rule(-45, new int[]{-44,279,-43}),
    new Rule(-45, new int[]{-44,281,-43}),
    new Rule(-45, new int[]{-44,278,-43}),
    new Rule(-45, new int[]{-44,277,-43}),
    new Rule(-45, new int[]{-44,276,-43}),
    new Rule(-45, new int[]{-44,275,-43}),
    new Rule(-45, new int[]{-44,274,-43}),
    new Rule(-45, new int[]{-44,273,-43}),
    new Rule(-45, new int[]{-44,272,-43}),
    new Rule(-45, new int[]{-44,280,-43}),
    new Rule(-45, new int[]{-44,282,-43}),
    new Rule(-45, new int[]{-44,303}),
    new Rule(-45, new int[]{303,-44}),
    new Rule(-45, new int[]{-44,302}),
    new Rule(-45, new int[]{302,-44}),
    new Rule(-45, new int[]{-43,284,-43}),
    new Rule(-45, new int[]{-43,285,-43}),
    new Rule(-45, new int[]{-43,263,-43}),
    new Rule(-45, new int[]{-43,265,-43}),
    new Rule(-45, new int[]{-43,264,-43}),
    new Rule(-45, new int[]{-43,124,-43}),
    new Rule(-45, new int[]{-43,38,-43}),
    new Rule(-45, new int[]{-43,94,-43}),
    new Rule(-45, new int[]{-43,46,-43}),
    new Rule(-45, new int[]{-43,43,-43}),
    new Rule(-45, new int[]{-43,45,-43}),
    new Rule(-45, new int[]{-43,42,-43}),
    new Rule(-45, new int[]{-43,305,-43}),
    new Rule(-45, new int[]{-43,47,-43}),
    new Rule(-45, new int[]{-43,37,-43}),
    new Rule(-45, new int[]{-43,293,-43}),
    new Rule(-45, new int[]{-43,294,-43}),
    new Rule(-45, new int[]{43,-43}),
    new Rule(-45, new int[]{45,-43}),
    new Rule(-45, new int[]{33,-43}),
    new Rule(-45, new int[]{126,-43}),
    new Rule(-45, new int[]{-43,287,-43}),
    new Rule(-45, new int[]{-43,286,-43}),
    new Rule(-45, new int[]{-43,289,-43}),
    new Rule(-45, new int[]{-43,288,-43}),
    new Rule(-45, new int[]{-43,60,-43}),
    new Rule(-45, new int[]{-43,291,-43}),
    new Rule(-45, new int[]{-43,62,-43}),
    new Rule(-45, new int[]{-43,292,-43}),
    new Rule(-45, new int[]{-43,290,-43}),
    new Rule(-45, new int[]{-43,295,-27}),
    new Rule(-45, new int[]{40,-43,41}),
    new Rule(-45, new int[]{-46}),
    new Rule(-45, new int[]{-43,63,-43,58,-43}),
    new Rule(-45, new int[]{-43,63,58,-43}),
    new Rule(-45, new int[]{-43,283,-43}),
    new Rule(-45, new int[]{-47}),
    new Rule(-45, new int[]{301,-43}),
    new Rule(-45, new int[]{300,-43}),
    new Rule(-45, new int[]{299,-43}),
    new Rule(-45, new int[]{298,-43}),
    new Rule(-45, new int[]{297,-43}),
    new Rule(-45, new int[]{296,-43}),
    new Rule(-45, new int[]{304,-43}),
    new Rule(-45, new int[]{326,-77}),
    new Rule(-45, new int[]{64,-43}),
    new Rule(-45, new int[]{-50}),
    new Rule(-45, new int[]{-75}),
    new Rule(-45, new int[]{266,-43}),
    new Rule(-45, new int[]{267}),
    new Rule(-45, new int[]{267,-43}),
    new Rule(-45, new int[]{267,-43,268,-43}),
    new Rule(-45, new int[]{269,-43}),
    new Rule(-45, new int[]{352,-43}),
    new Rule(-45, new int[]{-85}),
    new Rule(-45, new int[]{-91,-85}),
    new Rule(-45, new int[]{353,-85}),
    new Rule(-45, new int[]{-91,353,-85}),
    new Rule(-45, new int[]{-93}),
    new Rule(-85, new int[]{-5,-4,-17,40,-134,41,-136,-23,-154,-18,123,-101,125,-19,-154}),
    new Rule(-85, new int[]{-6,-4,40,-134,41,-23,-17,268,-154,-160,-43,-154}),
    new Rule(-6, new int[]{343}),
    new Rule(-5, new int[]{346}),
    new Rule(-17, new int[]{}),
    new Rule(-18, new int[]{}),
    new Rule(-19, new int[]{}),
    new Rule(-154, new int[]{}),
    new Rule(-160, new int[]{}),
    new Rule(-4, new int[]{}),
    new Rule(-4, new int[]{38}),
    new Rule(-136, new int[]{}),
    new Rule(-136, new int[]{350,40,-137,41}),
    new Rule(-137, new int[]{-137,44,-133}),
    new Rule(-137, new int[]{-133}),
    new Rule(-133, new int[]{320}),
    new Rule(-133, new int[]{38,320}),
    new Rule(-53, new int[]{-20,-130}),
    new Rule(-53, new int[]{-28,390,-2,-130}),
    new Rule(-53, new int[]{-82,390,-2,-130}),
    new Rule(-53, new int[]{-80,-130}),
    new Rule(-28, new int[]{353}),
    new Rule(-28, new int[]{-20}),
    new Rule(-27, new int[]{-28}),
    new Rule(-27, new int[]{-79}),
    new Rule(-77, new int[]{}),
    new Rule(-77, new int[]{40,-62,41}),
    new Rule(-75, new int[]{96,96}),
    new Rule(-75, new int[]{96,316,96}),
    new Rule(-75, new int[]{96,-109,96}),
    new Rule(-129, new int[]{}),
    new Rule(-129, new int[]{-130}),
    new Rule(-52, new int[]{368,40,-141,41}),
    new Rule(-52, new int[]{91,-141,93}),
    new Rule(-52, new int[]{323}),
    new Rule(-50, new int[]{317}),
    new Rule(-50, new int[]{312}),
    new Rule(-50, new int[]{370}),
    new Rule(-50, new int[]{371}),
    new Rule(-50, new int[]{375}),
    new Rule(-50, new int[]{374}),
    new Rule(-50, new int[]{378}),
    new Rule(-50, new int[]{376}),
    new Rule(-50, new int[]{392}),
    new Rule(-50, new int[]{373}),
    new Rule(-50, new int[]{34,-109,34}),
    new Rule(-50, new int[]{383,387}),
    new Rule(-50, new int[]{383,316,387}),
    new Rule(-50, new int[]{383,-109,387}),
    new Rule(-50, new int[]{-52}),
    new Rule(-50, new int[]{-51}),
    new Rule(-51, new int[]{-20}),
    new Rule(-51, new int[]{-28,390,-117}),
    new Rule(-51, new int[]{-82,390,-117}),
    new Rule(-43, new int[]{-44}),
    new Rule(-43, new int[]{-45}),
    new Rule(-62, new int[]{}),
    new Rule(-62, new int[]{-43}),
    new Rule(-21, new int[]{369}),
    new Rule(-21, new int[]{396}),
    new Rule(-82, new int[]{-71}),
    new Rule(-71, new int[]{-44}),
    new Rule(-71, new int[]{40,-43,41}),
    new Rule(-71, new int[]{-52}),
    new Rule(-80, new int[]{-48}),
    new Rule(-80, new int[]{40,-43,41}),
    new Rule(-80, new int[]{-52}),
    new Rule(-48, new int[]{-49}),
    new Rule(-48, new int[]{-71,91,-62,93}),
    new Rule(-48, new int[]{-51,91,-62,93}),
    new Rule(-48, new int[]{-71,123,-43,125}),
    new Rule(-48, new int[]{-71,-21,-1,-130}),
    new Rule(-48, new int[]{-53}),
    new Rule(-44, new int[]{-48}),
    new Rule(-44, new int[]{-54}),
    new Rule(-44, new int[]{-71,-21,-1}),
    new Rule(-49, new int[]{320}),
    new Rule(-49, new int[]{36,123,-43,125}),
    new Rule(-49, new int[]{36,-49}),
    new Rule(-54, new int[]{-28,390,-49}),
    new Rule(-54, new int[]{-82,390,-49}),
    new Rule(-79, new int[]{-49}),
    new Rule(-79, new int[]{-79,91,-62,93}),
    new Rule(-79, new int[]{-79,123,-43,125}),
    new Rule(-79, new int[]{-79,-21,-1}),
    new Rule(-79, new int[]{-28,390,-49}),
    new Rule(-79, new int[]{-79,390,-49}),
    new Rule(-2, new int[]{-117}),
    new Rule(-2, new int[]{123,-43,125}),
    new Rule(-2, new int[]{-49}),
    new Rule(-1, new int[]{319}),
    new Rule(-1, new int[]{123,-43,125}),
    new Rule(-1, new int[]{-49}),
    new Rule(-141, new int[]{-140}),
    new Rule(-138, new int[]{}),
    new Rule(-138, new int[]{-139}),
    new Rule(-140, new int[]{-140,44,-138}),
    new Rule(-140, new int[]{-138}),
    new Rule(-139, new int[]{-43,268,-43}),
    new Rule(-139, new int[]{-43}),
    new Rule(-139, new int[]{-43,268,38,-44}),
    new Rule(-139, new int[]{38,-44}),
    new Rule(-139, new int[]{394,-43}),
    new Rule(-139, new int[]{-43,268,367,40,-141,41}),
    new Rule(-139, new int[]{367,40,-141,41}),
    new Rule(-109, new int[]{-109,-64}),
    new Rule(-109, new int[]{-109,316}),
    new Rule(-109, new int[]{-64}),
    new Rule(-109, new int[]{316,-64}),
    new Rule(-64, new int[]{320}),
    new Rule(-64, new int[]{320,91,-65,93}),
    new Rule(-64, new int[]{320,-21,319}),
    new Rule(-64, new int[]{385,-43,125}),
    new Rule(-64, new int[]{385,318,125}),
    new Rule(-64, new int[]{385,318,91,-43,93,125}),
    new Rule(-64, new int[]{386,-44,125}),
    new Rule(-65, new int[]{319}),
    new Rule(-65, new int[]{325}),
    new Rule(-65, new int[]{320}),
    new Rule(-47, new int[]{358,40,-110,-3,41}),
    new Rule(-47, new int[]{359,40,-43,41}),
    new Rule(-47, new int[]{262,-43}),
    new Rule(-47, new int[]{261,-43}),
    new Rule(-47, new int[]{260,40,-43,41}),
    new Rule(-47, new int[]{259,-43}),
    new Rule(-47, new int[]{258,-43}),
    new Rule(-110, new int[]{-42}),
    new Rule(-110, new int[]{-110,44,-42}),
    new Rule(-42, new int[]{-43}),
    };
    #endregion

    nonTerminals = new string[] {"", "property_name", "member_name", "possible_comma", 
      "returns_ref", "function", "fn", "is_reference", "is_variadic", "variable_modifiers", 
      "method_modifiers", "non_empty_member_modifiers", "member_modifier", "class_modifier", 
      "class_modifiers", "optional_visibility_modifier", "use_type", "backup_doc_comment", 
      "enter_scope", "exit_scope", "name", "object_operator", "type", "return_type", 
      "type_expr", "optional_type", "extends_from", "class_name_reference", "class_name", 
      "name_list", "catch_name_list", "implements_list", "interface_extends_list", 
      "union_type", "top_statement", "statement", "function_declaration_statement", 
      "class_declaration_statement", "trait_declaration_statement", "interface_declaratioimplements_listn_statement", 
      "interface_declaration_statement", "inline_html", "isset_variable", "expr", 
      "variable", "expr_without_variable", "new_expr", "internal_functions_in_yacc", 
      "callable_variable", "simple_variable", "scalar", "constant", "dereferencable_scalar", 
      "function_call", "static_member", "if_stmt", "alt_if_stmt", "const_decl", 
      "unset_variable", "global_var", "static_var", "echo_expr", "optional_expr", 
      "property", "encaps_var", "encaps_var_offset", "declare_statement", "trait_adaptation", 
      "trait_precedence", "trait_alias", "class_const_decl", "dereferencable", 
      "for_statement", "foreach_statement", "while_statement", "backticks_expr", 
      "method_body", "exit_expr", "finally_statement", "new_variable", "callable_expr", 
      "trait_adaptations", "variable_class_name", "inner_statement", "class_statement", 
      "inline_function", "attributed_statement", "attributed_class_statement", 
      "attributed_parameter", "attribute", "attribute_decl", "attributes", "attribute_arguments", 
      "match", "match_arm", "match_arm_list", "match_arm_cond_list", "non_empty_match_arm_list", 
      "top_statement_list", "const_list", "class_const_list", "inner_statement_list", 
      "class_statement_list", "for_exprs", "global_var_list", "static_var_list", 
      "echo_expr_list", "unset_variables", "trait_adaptation_list", "encaps_list", 
      "isset_variables", "property_list", "case_list", "switch_case_list", "non_empty_for_exprs", 
      "catch_list", "optional_variable", "identifier", "semi_reserved", "reserved_non_modifiers", 
      "namespace_name", "unprefixed_use_declaration", "trait_method_reference", 
      "absolute_trait_method_reference", "use_declaration", "unprefixed_use_declarations", 
      "inline_use_declaration", "inline_use_declarations", "argument", "ctor_arguments", 
      "argument_list", "non_empty_argument_list", "parameter", "lexical_var", 
      "parameter_list", "non_empty_parameter_list", "lexical_vars", "lexical_var_list", 
      "possible_array_pair", "array_pair", "non_empty_array_pair_list", "array_pair_list", 
      "if_stmt_without_else", "alt_if_stmt_without_else", "foreach_variable", 
      "anonymous_class", "use_declarations", "group_use_declaration", "mixed_group_use_declaration", 
      "start", "$accept", "@1", "@2", "@3", "backup_fn_flags", "@4", "@5", "@6", 
      "case_separator", "@7", "backup_lex_pos", };
  }

  #endregion

  protected override void DoAction(int action)
  {
    switch (action)
    {
      case 2: // @1 -> 
{ SetNamingContext(null); }
        return;
      case 3: // start -> @1 top_statement_list 
{ 
		AssignNamingContext(); 
        _lexer.DocBlockList.Merge(new Span(0, int.MaxValue), value_stack.array[value_stack.top-1].yyval.NodeList, _astFactory);
		AssignStatements(value_stack.array[value_stack.top-1].yyval.NodeList);
		_astRoot = _astFactory.GlobalCode(yypos, value_stack.array[value_stack.top-1].yyval.NodeList, namingContext);
	}
        return;
      case 4: // reserved_non_modifiers -> T_INCLUDE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 5: // reserved_non_modifiers -> T_INCLUDE_ONCE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 6: // reserved_non_modifiers -> T_EVAL 
{ yyval.String = _lexer.TokenText; }
        return;
      case 7: // reserved_non_modifiers -> T_REQUIRE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 8: // reserved_non_modifiers -> T_REQUIRE_ONCE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 9: // reserved_non_modifiers -> T_LOGICAL_OR 
{ yyval.String = _lexer.TokenText; }
        return;
      case 10: // reserved_non_modifiers -> T_LOGICAL_XOR 
{ yyval.String = _lexer.TokenText; }
        return;
      case 11: // reserved_non_modifiers -> T_LOGICAL_AND 
{ yyval.String = _lexer.TokenText; }
        return;
      case 12: // reserved_non_modifiers -> T_INSTANCEOF 
{ yyval.String = _lexer.TokenText; }
        return;
      case 13: // reserved_non_modifiers -> T_NEW 
{ yyval.String = _lexer.TokenText; }
        return;
      case 14: // reserved_non_modifiers -> T_CLONE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 15: // reserved_non_modifiers -> T_EXIT 
{ yyval.String = _lexer.TokenText; }
        return;
      case 16: // reserved_non_modifiers -> T_IF 
{ yyval.String = _lexer.TokenText; }
        return;
      case 17: // reserved_non_modifiers -> T_ELSEIF 
{ yyval.String = _lexer.TokenText; }
        return;
      case 18: // reserved_non_modifiers -> T_ELSE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 19: // reserved_non_modifiers -> T_ENDIF 
{ yyval.String = _lexer.TokenText; }
        return;
      case 20: // reserved_non_modifiers -> T_ECHO 
{ yyval.String = _lexer.TokenText; }
        return;
      case 21: // reserved_non_modifiers -> T_DO 
{ yyval.String = _lexer.TokenText; }
        return;
      case 22: // reserved_non_modifiers -> T_WHILE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 23: // reserved_non_modifiers -> T_ENDWHILE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 24: // reserved_non_modifiers -> T_FOR 
{ yyval.String = _lexer.TokenText; }
        return;
      case 25: // reserved_non_modifiers -> T_ENDFOR 
{ yyval.String = _lexer.TokenText; }
        return;
      case 26: // reserved_non_modifiers -> T_FOREACH 
{ yyval.String = _lexer.TokenText; }
        return;
      case 27: // reserved_non_modifiers -> T_ENDFOREACH 
{ yyval.String = _lexer.TokenText; }
        return;
      case 28: // reserved_non_modifiers -> T_DECLARE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 29: // reserved_non_modifiers -> T_ENDDECLARE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 30: // reserved_non_modifiers -> T_AS 
{ yyval.String = _lexer.TokenText; }
        return;
      case 31: // reserved_non_modifiers -> T_TRY 
{ yyval.String = _lexer.TokenText; }
        return;
      case 32: // reserved_non_modifiers -> T_CATCH 
{ yyval.String = _lexer.TokenText; }
        return;
      case 33: // reserved_non_modifiers -> T_FINALLY 
{ yyval.String = _lexer.TokenText; }
        return;
      case 34: // reserved_non_modifiers -> T_THROW 
{ yyval.String = _lexer.TokenText; }
        return;
      case 35: // reserved_non_modifiers -> T_USE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 36: // reserved_non_modifiers -> T_INSTEADOF 
{ yyval.String = _lexer.TokenText; }
        return;
      case 37: // reserved_non_modifiers -> T_GLOBAL 
{ yyval.String = _lexer.TokenText; }
        return;
      case 38: // reserved_non_modifiers -> T_VAR 
{ yyval.String = _lexer.TokenText; }
        return;
      case 39: // reserved_non_modifiers -> T_UNSET 
{ yyval.String = _lexer.TokenText; }
        return;
      case 40: // reserved_non_modifiers -> T_ISSET 
{ yyval.String = _lexer.TokenText; }
        return;
      case 41: // reserved_non_modifiers -> T_EMPTY 
{ yyval.String = _lexer.TokenText; }
        return;
      case 42: // reserved_non_modifiers -> T_CONTINUE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 43: // reserved_non_modifiers -> T_GOTO 
{ yyval.String = _lexer.TokenText; }
        return;
      case 44: // reserved_non_modifiers -> T_FUNCTION 
{ yyval.String = _lexer.TokenText; }
        return;
      case 45: // reserved_non_modifiers -> T_CONST 
{ yyval.String = _lexer.TokenText; }
        return;
      case 46: // reserved_non_modifiers -> T_RETURN 
{ yyval.String = _lexer.TokenText; }
        return;
      case 47: // reserved_non_modifiers -> T_PRINT 
{ yyval.String = _lexer.TokenText; }
        return;
      case 48: // reserved_non_modifiers -> T_YIELD 
{ yyval.String = _lexer.TokenText; }
        return;
      case 49: // reserved_non_modifiers -> T_LIST 
{ yyval.String = _lexer.TokenText; }
        return;
      case 50: // reserved_non_modifiers -> T_SWITCH 
{ yyval.String = _lexer.TokenText; }
        return;
      case 51: // reserved_non_modifiers -> T_ENDSWITCH 
{ yyval.String = _lexer.TokenText; }
        return;
      case 52: // reserved_non_modifiers -> T_CASE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 53: // reserved_non_modifiers -> T_DEFAULT 
{ yyval.String = _lexer.TokenText; }
        return;
      case 54: // reserved_non_modifiers -> T_BREAK 
{ yyval.String = _lexer.TokenText; }
        return;
      case 55: // reserved_non_modifiers -> T_ARRAY 
{ yyval.String = _lexer.TokenText; }
        return;
      case 56: // reserved_non_modifiers -> T_CALLABLE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 57: // reserved_non_modifiers -> T_EXTENDS 
{ yyval.String = _lexer.TokenText; }
        return;
      case 58: // reserved_non_modifiers -> T_IMPLEMENTS 
{ yyval.String = _lexer.TokenText; }
        return;
      case 59: // reserved_non_modifiers -> T_NAMESPACE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 60: // reserved_non_modifiers -> T_TRAIT 
{ yyval.String = _lexer.TokenText; }
        return;
      case 61: // reserved_non_modifiers -> T_INTERFACE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 62: // reserved_non_modifiers -> T_CLASS 
{ yyval.String = _lexer.TokenText; }
        return;
      case 63: // reserved_non_modifiers -> T_CLASS_C 
{ yyval.String = _lexer.TokenText; }
        return;
      case 64: // reserved_non_modifiers -> T_TRAIT_C 
{ yyval.String = _lexer.TokenText; }
        return;
      case 65: // reserved_non_modifiers -> T_FUNC_C 
{ yyval.String = _lexer.TokenText; }
        return;
      case 66: // reserved_non_modifiers -> T_METHOD_C 
{ yyval.String = _lexer.TokenText; }
        return;
      case 67: // reserved_non_modifiers -> T_LINE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 68: // reserved_non_modifiers -> T_FILE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 69: // reserved_non_modifiers -> T_DIR 
{ yyval.String = _lexer.TokenText; }
        return;
      case 70: // reserved_non_modifiers -> T_NS_C 
{ yyval.String = _lexer.TokenText; }
        return;
      case 71: // reserved_non_modifiers -> T_FN 
{ yyval.String = _lexer.TokenText; }
        return;
      case 72: // reserved_non_modifiers -> T_MATCH 
{ yyval.String = _lexer.TokenText; }
        return;
      case 74: // semi_reserved -> T_STATIC 
{ yyval.String = _lexer.TokenText; }
        return;
      case 75: // semi_reserved -> T_ABSTRACT 
{ yyval.String = _lexer.TokenText; }
        return;
      case 76: // semi_reserved -> T_FINAL 
{ yyval.String = _lexer.TokenText; }
        return;
      case 77: // semi_reserved -> T_PRIVATE 
{ yyval.String = _lexer.TokenText; }
        return;
      case 78: // semi_reserved -> T_PROTECTED 
{ yyval.String = _lexer.TokenText; }
        return;
      case 79: // semi_reserved -> T_PUBLIC 
{ yyval.String = _lexer.TokenText; }
        return;
      case 82: // top_statement_list -> top_statement_list top_statement 
{ yyval.NodeList = AddToTopStatementList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 83: // top_statement_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 84: // namespace_name -> T_STRING 
{ yyval.StringList = new List<string>() { value_stack.array[value_stack.top-1].yyval.String }; }
        return;
      case 85: // namespace_name -> namespace_name T_NS_SEPARATOR T_STRING 
{ yyval.StringList = AddToList<string>(value_stack.array[value_stack.top-3].yyval.StringList, value_stack.array[value_stack.top-1].yyval.String); }
        return;
      case 86: // name -> namespace_name 
{ yyval.QualifiedNameReference = new QualifiedNameRef(yypos, new QualifiedName(value_stack.array[value_stack.top-1].yyval.StringList, true, false)); }
        return;
      case 87: // name -> T_NAMESPACE T_NS_SEPARATOR namespace_name 
{ yyval.QualifiedNameReference = new QualifiedNameRef(yypos, MergeWithCurrentNamespace(namingContext.CurrentNamespace, value_stack.array[value_stack.top-1].yyval.StringList)); }
        return;
      case 88: // name -> T_NS_SEPARATOR namespace_name 
{ yyval.QualifiedNameReference = new QualifiedNameRef(yypos, new QualifiedName(value_stack.array[value_stack.top-1].yyval.StringList, true,  true)); }
        return;
      case 89: // attribute_arguments -> argument_list 
{ yyval.ParamList = value_stack.array[value_stack.top-1].yyval.ParamList; /* intentionally more benevolent rule */ }
        return;
      case 90: // attribute_decl -> class_name_reference 
{ yyval.Node = _astFactory.Attribute(yypos, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 91: // attribute_decl -> class_name_reference attribute_arguments 
{ yyval.Node = _astFactory.Attribute(yypos, value_stack.array[value_stack.top-2].yyval.TypeReference, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos)); }
        return;
      case 92: // attribute -> T_SL attribute_decl T_SR 
{ value_stack.array[value_stack.top-2].yyval.Node.Span = yypos; yyval.Node = value_stack.array[value_stack.top-2].yyval.Node; }
        return;
      case 93: // attributes -> attribute 
{ yyval.NodeList = new List<LangElement>(1) { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 94: // attributes -> attributes attribute 
{ yyval.NodeList = AddToList(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 95: // attributed_statement -> function_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 96: // attributed_statement -> class_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 97: // attributed_statement -> trait_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 98: // attributed_statement -> interface_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 99: // top_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 100: // top_statement -> attributed_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 101: // top_statement -> attributes attributed_statement 
{ yyval.Node = WithAttributes(value_stack.array[value_stack.top-1].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 102: // top_statement -> T_HALT_COMPILER '(' ')' ';' 
{ yyval.Node = _astFactory.HaltCompiler(yypos); }
        return;
      case 103: // top_statement -> T_NAMESPACE namespace_name ';' 
{
			AssignNamingContext();
            SetNamingContext(value_stack.array[value_stack.top-2].yyval.StringList);
            SetDoc(yyval.Node = _currentNamespace = (NamespaceDecl)_astFactory.Namespace(yypos, namingContext.CurrentNamespace, value_stack.array[value_stack.top-2].yypos, namingContext));
		}
        return;
      case 104: // @2 -> 
{ SetNamingContext(value_stack.array[value_stack.top-2].yyval.StringList); }
        return;
      case 105: // top_statement -> T_NAMESPACE namespace_name backup_doc_comment @2 '{' top_statement_list '}' 
{ 
			yyval.Node = _astFactory.Namespace(yypos, namingContext.CurrentNamespace, value_stack.array[value_stack.top-6].yypos, CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList), namingContext);
			SetDoc(yyval.Node);
			ResetNamingContext(); 
		}
        return;
      case 106: // @3 -> 
{ SetNamingContext(null); }
        return;
      case 107: // top_statement -> T_NAMESPACE backup_doc_comment @3 '{' top_statement_list '}' 
{ 
			yyval.Node = _astFactory.Namespace(yypos, null, yypos, CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList), namingContext);
			SetDoc(yyval.Node);
			ResetNamingContext(); 
		}
        return;
      case 108: // top_statement -> T_USE mixed_group_use_declaration ';' 
{ yyval.Node = _astFactory.Use(yypos, value_stack.array[value_stack.top-2].yyval.UseList, AliasKind.Type); _contextType = AliasKind.Type;	/* TODO: Error - must contain only a single group use */	}
        return;
      case 109: // top_statement -> T_USE use_type group_use_declaration ';' 
{ yyval.Node = _astFactory.Use(yypos, value_stack.array[value_stack.top-2].yyval.UseList, value_stack.array[value_stack.top-3].yyval.Kind); _contextType = AliasKind.Type;				/* TODO: Error - must contain only a single group use */	}
        return;
      case 110: // top_statement -> T_USE use_declarations ';' 
{ yyval.Node = _astFactory.Use(yypos, value_stack.array[value_stack.top-2].yyval.UseList, AliasKind.Type); _contextType = AliasKind.Type;	/* TODO: Error - must contain only simple uses		  */	}
        return;
      case 111: // top_statement -> T_USE use_type use_declarations ';' 
{ yyval.Node = _astFactory.Use(yypos, value_stack.array[value_stack.top-2].yyval.UseList, value_stack.array[value_stack.top-3].yyval.Kind); _contextType = AliasKind.Type;				/* TODO: Error - must contain only simple uses		  */	}
        return;
      case 112: // top_statement -> T_CONST const_list ';' 
{
			SetDoc(yyval.Node = _astFactory.DeclList(yypos, PhpMemberAttributes.None, value_stack.array[value_stack.top-2].yyval.NodeList, null));
		}
        return;
      case 113: // use_type -> T_FUNCTION 
{ yyval.Kind = _contextType = AliasKind.Function; }
        return;
      case 114: // use_type -> T_CONST 
{ yyval.Kind = _contextType = AliasKind.Constant; }
        return;
      case 115: // group_use_declaration -> namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations possible_comma '}' 
{ yyval.UseList = new List<UseBase>() { AddAliases(yypos, value_stack.array[value_stack.top-6].yyval.StringList, CombineSpans(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos), AddTrailingComma(value_stack.array[value_stack.top-3].yyval.AliasList, value_stack.array[value_stack.top-2].yyval.Bool), false) }; }
        return;
      case 116: // group_use_declaration -> T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations possible_comma '}' 
{ yyval.UseList = new List<UseBase>() { AddAliases(yypos, value_stack.array[value_stack.top-6].yyval.StringList, CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos), AddTrailingComma(value_stack.array[value_stack.top-3].yyval.AliasList, value_stack.array[value_stack.top-2].yyval.Bool), true) }; }
        return;
      case 117: // mixed_group_use_declaration -> namespace_name T_NS_SEPARATOR '{' inline_use_declarations possible_comma '}' 
{  yyval.UseList = new List<UseBase>() { AddAliases(yypos, value_stack.array[value_stack.top-6].yyval.StringList, CombineSpans(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos), AddTrailingComma(value_stack.array[value_stack.top-3].yyval.ContextAliasList, value_stack.array[value_stack.top-2].yyval.Bool), false) }; }
        return;
      case 118: // mixed_group_use_declaration -> T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' inline_use_declarations possible_comma '}' 
{ yyval.UseList = new List<UseBase>() { AddAliases(yypos, value_stack.array[value_stack.top-6].yyval.StringList, CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos), AddTrailingComma(value_stack.array[value_stack.top-3].yyval.ContextAliasList, value_stack.array[value_stack.top-2].yyval.Bool), true) }; }
        return;
      case 119: // possible_comma -> 
{ yyval.Bool = false; }
        return;
      case 120: // possible_comma -> ',' 
{ yyval.Bool = true;  }
        return;
      case 121: // inline_use_declarations -> inline_use_declarations ',' inline_use_declaration 
{ yyval.ContextAliasList = AddToList<ContextAlias>(value_stack.array[value_stack.top-3].yyval.ContextAliasList, value_stack.array[value_stack.top-1].yyval.ContextAlias); }
        return;
      case 122: // inline_use_declarations -> inline_use_declaration 
{ yyval.ContextAliasList = new List<ContextAlias>() { value_stack.array[value_stack.top-1].yyval.ContextAlias }; }
        return;
      case 123: // unprefixed_use_declarations -> unprefixed_use_declarations ',' unprefixed_use_declaration 
{ yyval.AliasList = AddToList<CompleteAlias>(value_stack.array[value_stack.top-3].yyval.AliasList, value_stack.array[value_stack.top-1].yyval.Alias); }
        return;
      case 124: // unprefixed_use_declarations -> unprefixed_use_declaration 
{ yyval.AliasList = new List<CompleteAlias>() { value_stack.array[value_stack.top-1].yyval.Alias }; }
        return;
      case 125: // use_declarations -> use_declarations ',' use_declaration 
{ yyval.UseList = AddToList<UseBase>(value_stack.array[value_stack.top-3].yyval.UseList, AddAlias(value_stack.array[value_stack.top-1].yyval.Alias)); }
        return;
      case 126: // use_declarations -> use_declaration 
{ yyval.UseList = new List<UseBase>() { AddAlias(value_stack.array[value_stack.top-1].yyval.Alias) }; }
        return;
      case 127: // inline_use_declaration -> unprefixed_use_declaration 
{ yyval.ContextAlias = JoinTuples(yypos, value_stack.array[value_stack.top-1].yyval.Alias, AliasKind.Type); }
        return;
      case 128: // inline_use_declaration -> use_type unprefixed_use_declaration 
{ yyval.ContextAlias = JoinTuples(yypos, value_stack.array[value_stack.top-1].yyval.Alias, (AliasKind)value_stack.array[value_stack.top-2].yyval.Kind);  }
        return;
      case 129: // unprefixed_use_declaration -> namespace_name 
{ yyval.Alias = new CompleteAlias(new QualifiedNameRef(yypos, new QualifiedName(value_stack.array[value_stack.top-1].yyval.StringList, true, false)), NameRef.Invalid); }
        return;
      case 130: // unprefixed_use_declaration -> namespace_name T_AS T_STRING 
{ yyval.Alias = new CompleteAlias(new QualifiedNameRef(value_stack.array[value_stack.top-3].yypos, new QualifiedName(value_stack.array[value_stack.top-3].yyval.StringList, true, false)), new NameRef(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.String)); }
        return;
      case 131: // use_declaration -> unprefixed_use_declaration 
{ yyval.Alias = value_stack.array[value_stack.top-1].yyval.Alias; }
        return;
      case 132: // use_declaration -> T_NS_SEPARATOR unprefixed_use_declaration 
{ 
				var src = value_stack.array[value_stack.top-1].yyval.Alias;
				yyval.Alias = new CompleteAlias(new QualifiedNameRef(CombineSpans(value_stack.array[value_stack.top-2].yypos, src.Item1.Span), 
					new QualifiedName(src.Item1.QualifiedName.Name, src.Item1.QualifiedName.Namespaces, true)), src.Item2); 
			}
        return;
      case 133: // const_list -> const_list ',' const_decl 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 134: // const_list -> const_decl 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 135: // inner_statement_list -> inner_statement_list inner_statement 
{ yyval.NodeList = AddToTopStatementList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 136: // inner_statement_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 137: // inner_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 138: // inner_statement -> attributed_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 139: // inner_statement -> attributes attributed_statement 
{ yyval.Node = WithAttributes(value_stack.array[value_stack.top-1].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 140: // inner_statement -> T_HALT_COMPILER '(' ')' ';' 
{ 
				yyval.Node = _astFactory.HaltCompiler(yypos); 
				_errors.Error(yypos, FatalErrors.InvalidHaltCompiler); 
			}
        return;
      case 141: // statement -> '{' inner_statement_list '}' 
{ yyval.Node = CreateBlock(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 142: // statement -> enter_scope if_stmt exit_scope 
{ yyval.Node = value_stack.array[value_stack.top-2].yyval.Node; }
        return;
      case 143: // statement -> enter_scope alt_if_stmt exit_scope 
{ yyval.Node = value_stack.array[value_stack.top-2].yyval.Node; }
        return;
      case 144: // statement -> T_WHILE '(' expr ')' enter_scope while_statement exit_scope 
{ yyval.Node = _astFactory.While(yypos, value_stack.array[value_stack.top-5].yyval.Node, CombineSpans(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-4].yypos), value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 145: // statement -> T_DO enter_scope statement T_WHILE '(' expr ')' ';' exit_scope 
{ yyval.Node = _astFactory.Do(yypos, value_stack.array[value_stack.top-7].yyval.Node, value_stack.array[value_stack.top-4].yyval.Node, CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-3].yypos)); }
        return;
      case 146: // statement -> T_FOR '(' for_exprs ';' for_exprs ';' for_exprs ')' enter_scope for_statement exit_scope 
{ yyval.Node = _astFactory.For(yypos, value_stack.array[value_stack.top-9].yyval.NodeList, value_stack.array[value_stack.top-7].yyval.NodeList, value_stack.array[value_stack.top-5].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-4].yypos), value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 147: // statement -> T_SWITCH '(' expr ')' enter_scope switch_case_list exit_scope 
{ yyval.Node = _astFactory.Switch(yypos, value_stack.array[value_stack.top-5].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 148: // statement -> T_BREAK optional_expr ';' 
{ yyval.Node = _astFactory.Jump(yypos, JumpStmt.Types.Break, value_stack.array[value_stack.top-2].yyval.Node);}
        return;
      case 149: // statement -> T_CONTINUE optional_expr ';' 
{ yyval.Node = _astFactory.Jump(yypos, JumpStmt.Types.Continue, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 150: // statement -> T_RETURN optional_expr ';' 
{ yyval.Node = _astFactory.Jump(yypos, JumpStmt.Types.Return, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 151: // statement -> T_GLOBAL global_var_list ';' 
{ yyval.Node = _astFactory.Global(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 152: // statement -> T_STATIC static_var_list ';' 
{ yyval.Node = _astFactory.Static(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 153: // statement -> T_ECHO echo_expr_list ';' 
{ yyval.Node = _astFactory.Echo(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 154: // statement -> T_INLINE_HTML 
{ yyval.Node = _astFactory.InlineHtml(yypos, value_stack.array[value_stack.top-1].yyval.String); }
        return;
      case 155: // statement -> expr ';' 
{ yyval.Node = _astFactory.ExpressionStmt(yypos, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 156: // statement -> T_UNSET '(' unset_variables possible_comma ')' ';' 
{ yyval.Node = _astFactory.Unset(yypos, AddTrailingComma(value_stack.array[value_stack.top-4].yyval.NodeList, value_stack.array[value_stack.top-3].yyval.Bool)); }
        return;
      case 157: // statement -> T_FOREACH '(' expr T_AS foreach_variable ')' enter_scope foreach_statement exit_scope 
{ yyval.Node = _astFactory.Foreach(yypos, value_stack.array[value_stack.top-7].yyval.Node, null, value_stack.array[value_stack.top-5].yyval.ForeachVar, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 158: // statement -> T_FOREACH '(' expr T_AS foreach_variable T_DOUBLE_ARROW foreach_variable ')' enter_scope foreach_statement exit_scope 
{ yyval.Node = _astFactory.Foreach(yypos, value_stack.array[value_stack.top-9].yyval.Node, value_stack.array[value_stack.top-7].yyval.ForeachVar, value_stack.array[value_stack.top-5].yyval.ForeachVar, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 159: // statement -> T_DECLARE '(' const_list ')' declare_statement 
{ yyval.Node = _astFactory.Declare(yypos, value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 160: // statement -> ';' 
{ yyval.Node = _astFactory.EmptyStmt(yypos); }
        return;
      case 161: // statement -> T_TRY '{' inner_statement_list '}' enter_scope catch_list finally_statement exit_scope 
{ yyval.Node = _astFactory.TryCatch(yypos, CreateBlock(CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-5].yypos), value_stack.array[value_stack.top-6].yyval.NodeList), value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 162: // statement -> T_GOTO T_STRING ';' 
{ yyval.Node = _astFactory.Goto(yypos, value_stack.array[value_stack.top-2].yyval.String, value_stack.array[value_stack.top-2].yypos); }
        return;
      case 163: // statement -> T_STRING ':' 
{ yyval.Node = _astFactory.Label(yypos, value_stack.array[value_stack.top-2].yyval.String, value_stack.array[value_stack.top-2].yypos); }
        return;
      case 164: // catch_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 165: // catch_list -> catch_list T_CATCH '(' catch_name_list optional_variable ')' '{' inner_statement_list '}' 
{ 
				yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-9].yyval.NodeList, _astFactory.Catch(CombineSpans(value_stack.array[value_stack.top-8].yypos, value_stack.array[value_stack.top-1].yypos), _astFactory.TypeReference(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-6].yyval.TypeRefList), 
					(DirectVarUse)value_stack.array[value_stack.top-5].yyval.Node, CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList))); 
			}
        return;
      case 166: // optional_variable -> 
{ yyval.Node = null; }
        return;
      case 167: // optional_variable -> T_VARIABLE 
{ yyval.Node = _astFactory.Variable(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.String, NullLangElement, true); }
        return;
      case 168: // catch_name_list -> name 
{ yyval.TypeRefList = new List<TypeRef>() { CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference) }; }
        return;
      case 169: // catch_name_list -> catch_name_list '|' name 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference)); }
        return;
      case 170: // finally_statement -> 
{ yyval.Node = null; }
        return;
      case 171: // finally_statement -> T_FINALLY '{' inner_statement_list '}' 
{ yyval.Node =_astFactory.Finally(yypos, CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList)); }
        return;
      case 172: // unset_variables -> unset_variable 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 173: // unset_variables -> unset_variables ',' unset_variable 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 174: // unset_variable -> variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 175: // function_declaration_statement -> function returns_ref T_STRING backup_doc_comment '(' parameter_list ')' return_type backup_fn_flags enter_scope '{' inner_statement_list '}' exit_scope backup_fn_flags 
{ 
			yyval.Node = _astFactory.Function(yypos, isConditional, value_stack.array[value_stack.top-14].yyval.Long == (long)FormalParam.Flags.IsByRef, PhpMemberAttributes.None, value_stack.array[value_stack.top-8].yyval.TypeReference, 
			new Name(value_stack.array[value_stack.top-13].yyval.String), value_stack.array[value_stack.top-13].yypos, null, value_stack.array[value_stack.top-10].yyval.FormalParamList, CombineSpans(value_stack.array[value_stack.top-11].yypos, value_stack.array[value_stack.top-9].yypos), 
			CreateBlock(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-3].yypos), value_stack.array[value_stack.top-4].yyval.NodeList)); 
			SetDoc(yyval.Node);
		}
        return;
      case 176: // is_reference -> 
{ yyval.Long = 0; }
        return;
      case 177: // is_reference -> '&' 
{ yyval.Long = (long)FormalParam.Flags.IsByRef; }
        return;
      case 178: // is_variadic -> 
{ yyval.Long = 0; }
        return;
      case 179: // is_variadic -> T_ELLIPSIS 
{ yyval.Long = (long)FormalParam.Flags.IsVariadic; }
        return;
      case 180: // @4 -> 
{PushClassContext(value_stack.array[value_stack.top-2].yyval.String, value_stack.array[value_stack.top-1].yyval.TypeReference, (PhpMemberAttributes)value_stack.array[value_stack.top-4].yyval.Long);}
        return;
      case 181: // class_declaration_statement -> class_modifiers T_CLASS T_STRING extends_from @4 implements_list backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{ 
			yyval.Node = _astFactory.Type(yypos, CombineSpans(value_stack.array[value_stack.top-12].yypos, value_stack.array[value_stack.top-11].yypos, value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-7].yypos), isConditional, (PhpMemberAttributes)value_stack.array[value_stack.top-12].yyval.Long, new Name(value_stack.array[value_stack.top-10].yyval.String), value_stack.array[value_stack.top-10].yypos, null, 
				ConvertToNamedTypeRef(value_stack.array[value_stack.top-9].yyval.TypeReference), value_stack.array[value_stack.top-7].yyval.TypeRefList.Select(ConvertToNamedTypeRef), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos)); 
			SetDoc(yyval.Node);
			PopClassContext();
		}
        return;
      case 182: // class_modifiers -> 
{ yyval.Long = (long)PhpMemberAttributes.None; }
        return;
      case 183: // class_modifiers -> class_modifier class_modifiers 
{ yyval.Long = value_stack.array[value_stack.top-2].yyval.Long | value_stack.array[value_stack.top-1].yyval.Long; }
        return;
      case 184: // class_modifier -> T_ABSTRACT 
{ yyval.Long = (long)PhpMemberAttributes.Abstract; }
        return;
      case 185: // class_modifier -> T_FINAL 
{ yyval.Long = (long)PhpMemberAttributes.Final; }
        return;
      case 186: // @5 -> 
{PushClassContext(value_stack.array[value_stack.top-1].yyval.String, null, PhpMemberAttributes.Trait);}
        return;
      case 187: // trait_declaration_statement -> T_TRAIT T_STRING @5 backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{ 
			yyval.Node = _astFactory.Type(yypos, CombineSpans(value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-8].yypos), isConditional, PhpMemberAttributes.Trait, 
				new Name(value_stack.array[value_stack.top-8].yyval.String), value_stack.array[value_stack.top-8].yypos, null, null, new List<INamedTypeRef>(), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos)); 
			SetDoc(yyval.Node);
			PopClassContext();
		}
        return;
      case 188: // @6 -> 
{PushClassContext(value_stack.array[value_stack.top-1].yyval.String, null, PhpMemberAttributes.Interface);}
        return;
      case 189: // interface_declaration_statement -> T_INTERFACE T_STRING @6 interface_extends_list backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{ 
			yyval.Node = _astFactory.Type(yypos, CombineSpans(value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-7].yypos), isConditional, PhpMemberAttributes.Interface, 
				new Name(value_stack.array[value_stack.top-9].yyval.String), value_stack.array[value_stack.top-9].yypos, null, null, value_stack.array[value_stack.top-7].yyval.TypeRefList.Select(ConvertToNamedTypeRef), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos)); 
			SetDoc(yyval.Node);
			PopClassContext();
		}
        return;
      case 190: // extends_from -> 
{ yyval.TypeReference = null; }
        return;
      case 191: // extends_from -> T_EXTENDS name 
{ yyval.TypeReference = CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference); }
        return;
      case 192: // interface_extends_list -> 
{ yyval.TypeRefList = TypeRef.EmptyList; }
        return;
      case 193: // interface_extends_list -> T_EXTENDS name_list 
{ yyval.TypeRefList = value_stack.array[value_stack.top-1].yyval.TypeRefList; }
        return;
      case 194: // implements_list -> 
{ yyval.TypeRefList = TypeRef.EmptyList; }
        return;
      case 195: // implements_list -> T_IMPLEMENTS name_list 
{ yyval.TypeRefList = value_stack.array[value_stack.top-1].yyval.TypeRefList; }
        return;
      case 196: // foreach_variable -> variable 
{ yyval.ForeachVar = _astFactory.ForeachVariable(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 197: // foreach_variable -> '&' variable 
{ yyval.ForeachVar = _astFactory.ForeachVariable(yypos, value_stack.array[value_stack.top-1].yyval.Node, true); }
        return;
      case 198: // foreach_variable -> T_LIST '(' array_pair_list ')' 
{ yyval.ForeachVar = _astFactory.ForeachVariable(yypos, _astFactory.List(yypos, value_stack.array[value_stack.top-2].yyval.ItemList, true)); }
        return;
      case 199: // foreach_variable -> '[' array_pair_list ']' 
{ yyval.ForeachVar = _astFactory.ForeachVariable(yypos, _astFactory.List(yypos, value_stack.array[value_stack.top-2].yyval.ItemList, false)); }
        return;
      case 200: // for_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 201: // for_statement -> ':' inner_statement_list T_ENDFOR ';' 
{ yyval.Node = StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDFOR); }
        return;
      case 202: // foreach_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 203: // foreach_statement -> ':' inner_statement_list T_ENDFOREACH ';' 
{ yyval.Node = StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDFOREACH); }
        return;
      case 204: // declare_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 205: // declare_statement -> ':' inner_statement_list T_ENDDECLARE ';' 
{ yyval.Node = StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDDECLARE); }
        return;
      case 206: // switch_case_list -> '{' case_list '}' 
{ yyval.NodeList = value_stack.array[value_stack.top-2].yyval.NodeList; }
        return;
      case 207: // switch_case_list -> '{' ';' case_list '}' 
{ yyval.NodeList = value_stack.array[value_stack.top-2].yyval.NodeList; }
        return;
      case 208: // switch_case_list -> ':' case_list T_ENDSWITCH ';' 
{ yyval.NodeList = value_stack.array[value_stack.top-3].yyval.NodeList; }
        return;
      case 209: // switch_case_list -> ':' ';' case_list T_ENDSWITCH ';' 
{ yyval.NodeList = value_stack.array[value_stack.top-3].yyval.NodeList; }
        return;
      case 210: // case_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 211: // case_list -> case_list T_CASE expr case_separator inner_statement_list 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-5].yyval.NodeList, _astFactory.Case(CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), 
				value_stack.array[value_stack.top-3].yyval.Node, CreateCaseBlock(CombineSpans(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-1].yyval.NodeList))); }
        return;
      case 212: // case_list -> case_list T_DEFAULT case_separator inner_statement_list 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-4].yyval.NodeList, _astFactory.Case(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), 
				null, CreateCaseBlock(CombineSpans(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-1].yyval.NodeList))); }
        return;
      case 215: // match -> T_MATCH '(' expr ')' '{' match_arm_list '}' 
{ yyval.Node = (LangElement)_astFactory.Match(yypos, value_stack.array[value_stack.top-5].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 216: // match_arm_list -> 
{ yyval.NodeList = LangElement.EmptyList; }
        return;
      case 217: // match_arm_list -> non_empty_match_arm_list possible_comma 
{ yyval.NodeList = value_stack.array[value_stack.top-2].yyval.NodeList; }
        return;
      case 218: // non_empty_match_arm_list -> match_arm 
{ yyval.NodeList = new List<LangElement>(1) { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 219: // non_empty_match_arm_list -> non_empty_match_arm_list ',' match_arm 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 220: // match_arm -> match_arm_cond_list possible_comma T_DOUBLE_ARROW expr 
{ yyval.Node = (LangElement)_astFactory.MatchArm(yypos, value_stack.array[value_stack.top-4].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 221: // match_arm -> T_DEFAULT possible_comma T_DOUBLE_ARROW expr 
{ yyval.Node = (LangElement)_astFactory.MatchArm(yypos, LangElement.EmptyList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 222: // match_arm_cond_list -> expr 
{ yyval.NodeList = new List<LangElement>(1) { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 223: // match_arm_cond_list -> match_arm_cond_list ',' expr 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 224: // while_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 225: // while_statement -> ':' inner_statement_list T_ENDWHILE ';' 
{ yyval.Node = StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDWHILE); }
        return;
      case 226: // if_stmt_without_else -> T_IF '(' expr ')' statement 
{ yyval.IfItemList = new List<IfStatement>() { 
				new IfStatement(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node) }; 
			}
        return;
      case 227: // if_stmt_without_else -> if_stmt_without_else T_ELSEIF '(' expr ')' statement 
{ yyval.IfItemList = AddToList<IfStatement>(value_stack.array[value_stack.top-6].yyval.IfItemList, 
				new IfStatement(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node)); 
			}
        return;
      case 228: // if_stmt -> if_stmt_without_else 
{ ((List<IfStatement>)value_stack.array[value_stack.top-1].yyval.IfItemList).Reverse(); yyval.Node = null; 
			foreach (var item in (List<IfStatement>)value_stack.array[value_stack.top-1].yyval.IfItemList) 
				yyval.Node = _astFactory.If(yyval.Node != null? CombineSpans(item.Span, (yyval.Node).Span): item.Span, item.Condition, item.Body, yyval.Node); }
        return;
      case 229: // if_stmt -> if_stmt_without_else T_ELSE statement 
{ ((List<IfStatement>)value_stack.array[value_stack.top-3].yyval.IfItemList).Reverse(); yyval.Node = _astFactory.If(CombineSpans(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), null, value_stack.array[value_stack.top-1].yyval.Node, null); 
			foreach (var item in (List<IfStatement>)value_stack.array[value_stack.top-3].yyval.IfItemList) yyval.Node = _astFactory.If(CombineSpans(item.Span, (yyval.Node).Span), item.Condition, item.Body, yyval.Node); }
        return;
      case 230: // alt_if_stmt_without_else -> T_IF '(' expr ')' ':' inner_statement_list 
{ 
				yyval.IfItemList = new List<IfStatement>() { new IfStatement(yypos, value_stack.array[value_stack.top-4].yyval.Node, StatementsToBlock(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yyval.NodeList, Tokens.END)) }; 
			}
        return;
      case 231: // alt_if_stmt_without_else -> alt_if_stmt_without_else T_ELSEIF '(' expr ')' ':' inner_statement_list 
{ 
				RebuildLast(value_stack.array[value_stack.top-7].yyval.IfItemList, value_stack.array[value_stack.top-6].yypos, Tokens.T_ELSEIF);
				yyval.IfItemList = AddToList<IfStatement>(value_stack.array[value_stack.top-7].yyval.IfItemList, 
					new IfStatement(CombineSpans(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-4].yyval.Node, StatementsToBlock(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yyval.NodeList, Tokens.END))); 
			}
        return;
      case 232: // alt_if_stmt -> alt_if_stmt_without_else T_ENDIF ';' 
{ RebuildLast(value_stack.array[value_stack.top-3].yyval.IfItemList, value_stack.array[value_stack.top-2].yypos, Tokens.T_ENDIF);
			 ((List<IfStatement>)value_stack.array[value_stack.top-3].yyval.IfItemList).Reverse(); yyval.Node = null; 
			foreach (var item in (List<IfStatement>)value_stack.array[value_stack.top-3].yyval.IfItemList) 
				yyval.Node = _astFactory.If(yyval.Node != null? CombineSpans(item.Span, (yyval.Node).Span): item.Span, item.Condition, item.Body, yyval.Node); }
        return;
      case 233: // alt_if_stmt -> alt_if_stmt_without_else T_ELSE ':' inner_statement_list T_ENDIF ';' 
{ RebuildLast(value_stack.array[value_stack.top-6].yyval.IfItemList, value_stack.array[value_stack.top-5].yypos, Tokens.T_ELSE);
			((List<IfStatement>)value_stack.array[value_stack.top-6].yyval.IfItemList).Reverse(); yyval.Node = _astFactory.If(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-1].yypos), null, StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDIF), null); 
			foreach (var item in (List<IfStatement>)value_stack.array[value_stack.top-6].yyval.IfItemList) yyval.Node = _astFactory.If(CombineSpans(item.Span, (yyval.Node).Span), item.Condition, item.Body, yyval.Node); }
        return;
      case 234: // parameter_list -> non_empty_parameter_list possible_comma 
{ yyval.FormalParamList = AddTrailingComma(value_stack.array[value_stack.top-2].yyval.FormalParamList, value_stack.array[value_stack.top-1].yyval.Bool); }
        return;
      case 235: // parameter_list -> 
{ yyval.FormalParamList = new List<FormalParam>(); }
        return;
      case 236: // non_empty_parameter_list -> attributed_parameter 
{ yyval.FormalParamList = new List<FormalParam>() { (FormalParam)value_stack.array[value_stack.top-1].yyval.FormalParam }; }
        return;
      case 237: // non_empty_parameter_list -> non_empty_parameter_list ',' attributed_parameter 
{ yyval.FormalParamList = AddToList<FormalParam>(value_stack.array[value_stack.top-3].yyval.FormalParamList, value_stack.array[value_stack.top-1].yyval.FormalParam); }
        return;
      case 238: // attributed_parameter -> attributes parameter 
{ yyval.FormalParam = WithAttributes(value_stack.array[value_stack.top-1].yyval.FormalParam, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 239: // attributed_parameter -> parameter 
{ yyval.FormalParam = value_stack.array[value_stack.top-1].yyval.FormalParam; }
        return;
      case 240: // optional_visibility_modifier -> 
{ yyval.Long = 0; /* None */ }
        return;
      case 241: // optional_visibility_modifier -> T_PUBLIC 
{ yyval.Long = (long)(PhpMemberAttributes.Public | PhpMemberAttributes.Constructor); }
        return;
      case 242: // optional_visibility_modifier -> T_PROTECTED 
{ yyval.Long = (long)(PhpMemberAttributes.Protected | PhpMemberAttributes.Constructor); }
        return;
      case 243: // optional_visibility_modifier -> T_PRIVATE 
{ yyval.Long = (long)(PhpMemberAttributes.Private | PhpMemberAttributes.Constructor); }
        return;
      case 244: // parameter -> optional_visibility_modifier optional_type is_reference is_variadic T_VARIABLE 
{ yyval.FormalParam = _astFactory.Parameter(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-1].yyval.String, value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-4].yyval.TypeReference, (FormalParam.Flags)value_stack.array[value_stack.top-3].yyval.Long|(FormalParam.Flags)value_stack.array[value_stack.top-2].yyval.Long, null, (PhpMemberAttributes)value_stack.array[value_stack.top-5].yyval.Long); /* Important - @$ is invalid when optional_type is empty */ }
        return;
      case 245: // parameter -> optional_visibility_modifier optional_type is_reference is_variadic T_VARIABLE '=' expr 
{ yyval.FormalParam = _astFactory.Parameter(CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-3].yyval.String, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-6].yyval.TypeReference, (FormalParam.Flags)value_stack.array[value_stack.top-5].yyval.Long|(FormalParam.Flags)value_stack.array[value_stack.top-4].yyval.Long, (Expression)value_stack.array[value_stack.top-1].yyval.Node, (PhpMemberAttributes)value_stack.array[value_stack.top-7].yyval.Long); /* Important - @$ is invalid when optional_type is empty */ }
        return;
      case 246: // optional_type -> 
{ yyval.TypeReference = null; }
        return;
      case 247: // optional_type -> type_expr 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 248: // type_expr -> type 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 249: // type_expr -> '?' type 
{ yyval.TypeReference = _astFactory.NullableTypeReference(yypos, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 250: // type_expr -> union_type 
{ yyval.TypeReference = _astFactory.TypeReference(yypos, value_stack.array[value_stack.top-1].yyval.TypeRefList); }
        return;
      case 251: // type -> T_ARRAY 
{ yyval.TypeReference = _astFactory.PrimitiveTypeReference(yypos, PrimitiveTypeRef.PrimitiveType.array); }
        return;
      case 252: // type -> T_CALLABLE 
{ yyval.TypeReference = _astFactory.PrimitiveTypeReference(yypos, PrimitiveTypeRef.PrimitiveType.callable); }
        return;
      case 253: // type -> T_STATIC 
{ yyval.TypeReference = _astFactory.ReservedTypeReference(yypos, _reservedTypeStatic); }
        return;
      case 254: // type -> name 
{ yyval.TypeReference = CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference, true); }
        return;
      case 255: // union_type -> type '|' type 
{ yyval.TypeRefList = new List<TypeRef>(2){ value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-1].yyval.TypeReference }; }
        return;
      case 256: // union_type -> union_type '|' type 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 257: // return_type -> 
{ yyval.TypeReference = null; }
        return;
      case 258: // return_type -> ':' type_expr 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 259: // argument_list -> '(' ')' 
{ yyval.ParamList = new List<ActualParam>(); }
        return;
      case 260: // argument_list -> '(' non_empty_argument_list possible_comma ')' 
{ yyval.ParamList = AddTrailingComma(value_stack.array[value_stack.top-3].yyval.ParamList, value_stack.array[value_stack.top-2].yyval.Bool); }
        return;
      case 261: // non_empty_argument_list -> argument 
{ yyval.ParamList = new List<ActualParam>() { value_stack.array[value_stack.top-1].yyval.Param }; }
        return;
      case 262: // non_empty_argument_list -> non_empty_argument_list ',' argument 
{ yyval.ParamList = AddToList<ActualParam>(value_stack.array[value_stack.top-3].yyval.ParamList, value_stack.array[value_stack.top-1].yyval.Param); }
        return;
      case 263: // argument -> expr 
{ yyval.Param = _astFactory.ActualParameter(yypos, value_stack.array[value_stack.top-1].yyval.Node, ActualParam.Flags.Default); }
        return;
      case 264: // argument -> T_ELLIPSIS expr 
{ yyval.Param = _astFactory.ActualParameter(yypos, value_stack.array[value_stack.top-1].yyval.Node, ActualParam.Flags.IsUnpack); }
        return;
      case 265: // global_var_list -> global_var_list ',' global_var 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 266: // global_var_list -> global_var 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 267: // global_var -> simple_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 268: // static_var_list -> static_var_list ',' static_var 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 269: // static_var_list -> static_var 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 270: // static_var -> T_VARIABLE 
{ yyval.Node = _astFactory.StaticVarDecl(yypos, new VariableName(value_stack.array[value_stack.top-1].yyval.String), null); }
        return;
      case 271: // static_var -> T_VARIABLE '=' expr 
{ yyval.Node = _astFactory.StaticVarDecl(yypos, new VariableName(value_stack.array[value_stack.top-3].yyval.String), value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 272: // class_statement_list -> class_statement_list class_statement 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 273: // class_statement_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 274: // attributed_class_statement -> variable_modifiers optional_type property_list ';' 
{ 
				yyval.Node = _astFactory.DeclList(yypos, (PhpMemberAttributes)value_stack.array[value_stack.top-4].yyval.Long, value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-3].yyval.TypeReference); 
				SetDoc(yyval.Node);
			}
        return;
      case 275: // attributed_class_statement -> method_modifiers T_CONST class_const_list ';' 
{ 
				yyval.Node = _astFactory.DeclList(yypos, (PhpMemberAttributes)value_stack.array[value_stack.top-4].yyval.Long, value_stack.array[value_stack.top-2].yyval.NodeList, null); 
				SetDoc(yyval.Node);
			}
        return;
      case 276: // attributed_class_statement -> method_modifiers function returns_ref identifier backup_doc_comment '(' parameter_list ')' return_type backup_fn_flags method_body backup_fn_flags 
{	
				yyval.Node = _astFactory.Method(yypos, value_stack.array[value_stack.top-10].yyval.Long == (long)FormalParam.Flags.IsByRef, (PhpMemberAttributes)value_stack.array[value_stack.top-12].yyval.Long, 
					value_stack.array[value_stack.top-4].yyval.TypeReference, value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-9].yyval.String, value_stack.array[value_stack.top-9].yypos, null, value_stack.array[value_stack.top-6].yyval.FormalParamList, CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-5].yypos), null, value_stack.array[value_stack.top-2].yyval.Node);
				SetDoc(yyval.Node);
			}
        return;
      case 277: // class_statement -> attributed_class_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 278: // class_statement -> attributes attributed_class_statement 
{ yyval.Node = WithAttributes(value_stack.array[value_stack.top-1].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 279: // class_statement -> T_USE name_list trait_adaptations 
{ yyval.Node = _astFactory.TraitUse(yypos, value_stack.array[value_stack.top-2].yyval.TypeRefList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 280: // name_list -> name 
{ yyval.TypeRefList = new List<TypeRef>() { CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference) }; }
        return;
      case 281: // name_list -> name_list ',' name 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference)); }
        return;
      case 282: // trait_adaptations -> ';' 
{ yyval.Node = null; }
        return;
      case 283: // trait_adaptations -> '{' '}' 
{ yyval.Node = _astFactory.TraitAdaptationBlock(yypos, new List<LangElement>()); }
        return;
      case 284: // trait_adaptations -> '{' trait_adaptation_list '}' 
{ yyval.Node = _astFactory.TraitAdaptationBlock(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 285: // trait_adaptation_list -> trait_adaptation 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node };
 }
        return;
      case 286: // trait_adaptation_list -> trait_adaptation_list trait_adaptation 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 287: // trait_adaptation -> trait_precedence 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 288: // trait_adaptation -> trait_alias 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 289: // trait_precedence -> absolute_trait_method_reference T_INSTEADOF name_list ';' 
{ yyval.Node = _astFactory.TraitAdaptationPrecedence(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-4].yyval.Object, value_stack.array[value_stack.top-2].yyval.TypeRefList); }
        return;
      case 290: // trait_alias -> trait_method_reference T_AS T_STRING ';' 
{ yyval.Node = _astFactory.TraitAdaptationAlias(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-4].yyval.Object, new NameRef(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.String), null); }
        return;
      case 291: // trait_alias -> trait_method_reference T_AS reserved_non_modifiers ';' 
{ yyval.Node = _astFactory.TraitAdaptationAlias(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-4].yyval.Object, new NameRef(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.String), null); }
        return;
      case 292: // trait_alias -> trait_method_reference T_AS member_modifier identifier ';' 
{ 
				yyval.Node = _astFactory.TraitAdaptationAlias(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-5].yyval.Object, new NameRef(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.String), (PhpMemberAttributes)value_stack.array[value_stack.top-3].yyval.Long); 
			}
        return;
      case 293: // trait_alias -> trait_method_reference T_AS member_modifier ';' 
{ 
				yyval.Node = _astFactory.TraitAdaptationAlias(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-4].yyval.Object, NameRef.Invalid, (PhpMemberAttributes)value_stack.array[value_stack.top-2].yyval.Long); 
			}
        return;
      case 294: // trait_method_reference -> identifier 
{ yyval.Object = new Tuple<TypeRef,NameRef>(null, new NameRef(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.String)); }
        return;
      case 295: // trait_method_reference -> absolute_trait_method_reference 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.Object; }
        return;
      case 296: // absolute_trait_method_reference -> name T_DOUBLE_COLON identifier 
{ yyval.Object = new Tuple<TypeRef,NameRef>(CreateTypeRef(value_stack.array[value_stack.top-3].yyval.QualifiedNameReference), new NameRef(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.String)); }
        return;
      case 297: // method_body -> ';' 
{ yyval.Node = null; }
        return;
      case 298: // method_body -> '{' inner_statement_list '}' 
{ yyval.Node = CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 299: // variable_modifiers -> non_empty_member_modifiers 
{ yyval.Long = value_stack.array[value_stack.top-1].yyval.Long; }
        return;
      case 300: // variable_modifiers -> T_VAR 
{ yyval.Long = (long)PhpMemberAttributes.None; }
        return;
      case 301: // method_modifiers -> 
{ yyval.Long = (long)PhpMemberAttributes.None; }
        return;
      case 302: // method_modifiers -> non_empty_member_modifiers 
{ yyval.Long = value_stack.array[value_stack.top-1].yyval.Long; }
        return;
      case 303: // non_empty_member_modifiers -> member_modifier 
{ yyval.Long = value_stack.array[value_stack.top-1].yyval.Long; }
        return;
      case 304: // non_empty_member_modifiers -> non_empty_member_modifiers member_modifier 
{ yyval.Long = value_stack.array[value_stack.top-2].yyval.Long | value_stack.array[value_stack.top-1].yyval.Long; }
        return;
      case 305: // member_modifier -> T_PUBLIC 
{ yyval.Long = (long)PhpMemberAttributes.Public; }
        return;
      case 306: // member_modifier -> T_PROTECTED 
{ yyval.Long = (long)PhpMemberAttributes.Protected; }
        return;
      case 307: // member_modifier -> T_PRIVATE 
{ yyval.Long = (long)PhpMemberAttributes.Private; }
        return;
      case 308: // member_modifier -> T_STATIC 
{ yyval.Long = (long)PhpMemberAttributes.Static; }
        return;
      case 309: // member_modifier -> T_ABSTRACT 
{ yyval.Long = (long)PhpMemberAttributes.Abstract; }
        return;
      case 310: // member_modifier -> T_FINAL 
{ yyval.Long = (long)PhpMemberAttributes.Final; }
        return;
      case 311: // property_list -> property_list ',' property 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 312: // property_list -> property 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 313: // property -> T_VARIABLE backup_doc_comment 
{ SetMemberDoc(yyval.Node = _astFactory.FieldDecl(yypos, new VariableName(value_stack.array[value_stack.top-2].yyval.String), null)); }
        return;
      case 314: // property -> T_VARIABLE '=' expr backup_doc_comment 
{ SetMemberDoc(yyval.Node = _astFactory.FieldDecl(yypos, new VariableName(value_stack.array[value_stack.top-4].yyval.String), (Expression)value_stack.array[value_stack.top-2].yyval.Node)); }
        return;
      case 315: // class_const_list -> class_const_list ',' class_const_decl 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 316: // class_const_list -> class_const_decl 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 317: // class_const_decl -> identifier '=' expr backup_doc_comment 
{
		yyval.Node = _astFactory.ClassConstDecl(yypos, new VariableName(value_stack.array[value_stack.top-4].yyval.String), value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yyval.Node); 
		SetMemberDoc(yyval.Node);
	}
        return;
      case 318: // const_decl -> T_STRING '=' expr backup_doc_comment 
{ yyval.Node = _astFactory.GlobalConstDecl(yypos, false, new VariableName(value_stack.array[value_stack.top-4].yyval.String), value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yyval.Node); 
		SetMemberDoc(yyval.Node);
	}
        return;
      case 319: // echo_expr_list -> echo_expr_list ',' echo_expr 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 320: // echo_expr_list -> echo_expr 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 321: // echo_expr -> expr 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 322: // for_exprs -> 
{ yyval.NodeList = LangElement.EmptyList; }
        return;
      case 323: // for_exprs -> non_empty_for_exprs 
{ yyval.NodeList = value_stack.array[value_stack.top-1].yyval.NodeList; }
        return;
      case 324: // non_empty_for_exprs -> non_empty_for_exprs ',' expr 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 325: // non_empty_for_exprs -> expr 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 326: // @7 -> 
{ PushAnonymousClassContext(value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 327: // anonymous_class -> T_CLASS ctor_arguments extends_from @7 implements_list backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{
			var typeRef = _astFactory.AnonymousTypeReference(yypos, CombineSpans(value_stack.array[value_stack.top-11].yypos, value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-7].yypos), isConditional, PhpMemberAttributes.None, null, ConvertToNamedTypeRef(value_stack.array[value_stack.top-9].yyval.TypeReference), value_stack.array[value_stack.top-7].yyval.TypeRefList.Select(ConvertToNamedTypeRef), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos));
			SetDoc(((AnonymousTypeRef)typeRef).TypeDeclaration);
			yyval.AnonymousClass = new AnonymousClass(typeRef, value_stack.array[value_stack.top-10].yyval.ParamList, value_stack.array[value_stack.top-10].yypos); 
			PopClassContext();
		}
        return;
      case 328: // new_expr -> T_NEW class_name_reference ctor_arguments 
{ yyval.Node = _astFactory.New(yypos, value_stack.array[value_stack.top-2].yyval.TypeReference, value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos); }
        return;
      case 329: // new_expr -> T_NEW anonymous_class 
{ yyval.Node = _astFactory.New(yypos, ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item1, ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item2, ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item3); }
        return;
      case 330: // new_expr -> T_NEW attributes anonymous_class 
{ yyval.Node = _astFactory.New(yypos, WithAttributes(((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item1, value_stack.array[value_stack.top-2].yyval.NodeList), ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item2, ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item3); }
        return;
      case 331: // expr_without_variable -> T_LIST '(' array_pair_list ')' '=' expr 
{ yyval.Node = _astFactory.Assignment(yypos, _astFactory.List(Span.Combine(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-3].yypos), value_stack.array[value_stack.top-4].yyval.ItemList, true), value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignValue); }
        return;
      case 332: // expr_without_variable -> '[' array_pair_list ']' '=' expr 
{ yyval.Node = _astFactory.Assignment(yypos, _astFactory.List(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-3].yypos), value_stack.array[value_stack.top-4].yyval.ItemList, false), value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignValue); }
        return;
      case 333: // expr_without_variable -> variable '=' expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignValue); }
        return;
      case 334: // expr_without_variable -> variable '=' '&' variable 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignRef); }
        return;
      case 335: // expr_without_variable -> variable '=' '&' new_expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignRef); _errors.Error(yypos, Warnings.AssignNewByRefDeprecated); }
        return;
      case 336: // expr_without_variable -> T_CLONE expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Clone,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 337: // expr_without_variable -> variable T_PLUS_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignAdd); }
        return;
      case 338: // expr_without_variable -> variable T_MINUS_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignSub); }
        return;
      case 339: // expr_without_variable -> variable T_MUL_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignMul); }
        return;
      case 340: // expr_without_variable -> variable T_POW_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignPow); }
        return;
      case 341: // expr_without_variable -> variable T_DIV_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignDiv); }
        return;
      case 342: // expr_without_variable -> variable T_CONCAT_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignAppend); }
        return;
      case 343: // expr_without_variable -> variable T_MOD_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignMod); }
        return;
      case 344: // expr_without_variable -> variable T_AND_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignAnd); }
        return;
      case 345: // expr_without_variable -> variable T_OR_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignOr); }
        return;
      case 346: // expr_without_variable -> variable T_XOR_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignXor); }
        return;
      case 347: // expr_without_variable -> variable T_SL_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignShiftLeft); }
        return;
      case 348: // expr_without_variable -> variable T_SR_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignShiftRight); }
        return;
      case 349: // expr_without_variable -> variable T_COALESCE_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignCoalesce); }
        return;
      case 350: // expr_without_variable -> variable T_INC 
{ yyval.Node = CreateIncDec(yypos, value_stack.array[value_stack.top-2].yyval.Node, true, true); }
        return;
      case 351: // expr_without_variable -> T_INC variable 
{ yyval.Node = CreateIncDec(yypos, value_stack.array[value_stack.top-1].yyval.Node, true,  false); }
        return;
      case 352: // expr_without_variable -> variable T_DEC 
{ yyval.Node = CreateIncDec(yypos, value_stack.array[value_stack.top-2].yyval.Node, false, true); }
        return;
      case 353: // expr_without_variable -> T_DEC variable 
{ yyval.Node = CreateIncDec(yypos, value_stack.array[value_stack.top-1].yyval.Node, false, false); }
        return;
      case 354: // expr_without_variable -> expr T_BOOLEAN_OR expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Or,   value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 355: // expr_without_variable -> expr T_BOOLEAN_AND expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.And,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 356: // expr_without_variable -> expr T_LOGICAL_OR expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Or,   value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 357: // expr_without_variable -> expr T_LOGICAL_AND expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.And,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 358: // expr_without_variable -> expr T_LOGICAL_XOR expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Xor,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 359: // expr_without_variable -> expr '|' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.BitOr,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 360: // expr_without_variable -> expr '&' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.BitAnd, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 361: // expr_without_variable -> expr '^' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.BitXor, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 362: // expr_without_variable -> expr '.' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Concat, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 363: // expr_without_variable -> expr '+' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Add,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 364: // expr_without_variable -> expr '-' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Sub,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 365: // expr_without_variable -> expr '*' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Mul,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 366: // expr_without_variable -> expr T_POW expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Pow,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 367: // expr_without_variable -> expr '/' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Div,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 368: // expr_without_variable -> expr '%' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Mod,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 369: // expr_without_variable -> expr T_SL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.ShiftLeft,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 370: // expr_without_variable -> expr T_SR expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.ShiftRight, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 371: // expr_without_variable -> '+' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Plus,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 372: // expr_without_variable -> '-' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Minus,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 373: // expr_without_variable -> '!' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.LogicNegation, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 374: // expr_without_variable -> '~' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.BitNegation,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 375: // expr_without_variable -> expr T_IS_IDENTICAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Identical, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 376: // expr_without_variable -> expr T_IS_NOT_IDENTICAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.NotIdentical, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 377: // expr_without_variable -> expr T_IS_EQUAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Equal, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 378: // expr_without_variable -> expr T_IS_NOT_EQUAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.NotEqual, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 379: // expr_without_variable -> expr '<' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.LessThan, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 380: // expr_without_variable -> expr T_IS_SMALLER_OR_EQUAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.LessThanOrEqual, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 381: // expr_without_variable -> expr '>' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.GreaterThan, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 382: // expr_without_variable -> expr T_IS_GREATER_OR_EQUAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.GreaterThanOrEqual, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 383: // expr_without_variable -> expr T_SPACESHIP expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Spaceship, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 384: // expr_without_variable -> expr T_INSTANCEOF class_name_reference 
{ yyval.Node = _astFactory.InstanceOf(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 385: // expr_without_variable -> '(' expr ')' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN); }
        return;
      case 386: // expr_without_variable -> new_expr 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 387: // expr_without_variable -> expr '?' expr ':' expr 
{ yyval.Node = _astFactory.ConditionalEx(yypos, value_stack.array[value_stack.top-5].yyval.Node, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 388: // expr_without_variable -> expr '?' ':' expr 
{ yyval.Node = _astFactory.ConditionalEx(yypos, value_stack.array[value_stack.top-4].yyval.Node, null, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 389: // expr_without_variable -> expr T_COALESCE expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Coalesce, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 390: // expr_without_variable -> internal_functions_in_yacc 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 391: // expr_without_variable -> T_INT_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Int64Cast,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 392: // expr_without_variable -> T_DOUBLE_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.DoubleCast, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 393: // expr_without_variable -> T_STRING_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.StringCast, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 394: // expr_without_variable -> T_ARRAY_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.ArrayCast,  (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 395: // expr_without_variable -> T_OBJECT_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.ObjectCast, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 396: // expr_without_variable -> T_BOOL_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.BoolCast,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 397: // expr_without_variable -> T_UNSET_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.UnsetCast,  (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 398: // expr_without_variable -> T_EXIT exit_expr 
{ yyval.Node = _astFactory.Exit(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 399: // expr_without_variable -> '@' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.AtSign,     (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 400: // expr_without_variable -> scalar 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 401: // expr_without_variable -> backticks_expr 
{ yyval.Node = _astFactory.Shell(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 402: // expr_without_variable -> T_PRINT expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Print, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 403: // expr_without_variable -> T_YIELD 
{ yyval.Node = _astFactory.Yield(yypos, null, null); }
        return;
      case 404: // expr_without_variable -> T_YIELD expr 
{ yyval.Node = _astFactory.Yield(yypos, null, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 405: // expr_without_variable -> T_YIELD expr T_DOUBLE_ARROW expr 
{ yyval.Node = _astFactory.Yield(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 406: // expr_without_variable -> T_YIELD_FROM expr 
{ yyval.Node = _astFactory.YieldFrom(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 407: // expr_without_variable -> T_THROW expr 
{ yyval.Node = _astFactory.Throw(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 408: // expr_without_variable -> inline_function 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 409: // expr_without_variable -> attributes inline_function 
{ yyval.Node = WithAttributes(value_stack.array[value_stack.top-1].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 410: // expr_without_variable -> T_STATIC inline_function 
{
			var lambda = (LambdaFunctionExpr)value_stack.array[value_stack.top-1].yyval.Node;
			lambda.IsStatic = true;
			lambda.Span = yypos;
			yyval.Node = lambda;
		}
        return;
      case 411: // expr_without_variable -> attributes T_STATIC inline_function 
{
			var lambda = (LambdaFunctionExpr)value_stack.array[value_stack.top-1].yyval.Node;
			lambda.IsStatic = true;
			lambda.Span = yypos;
			yyval.Node = WithAttributes(lambda, value_stack.array[value_stack.top-3].yyval.NodeList);
		}
        return;
      case 412: // expr_without_variable -> match 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 413: // inline_function -> function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars return_type backup_fn_flags enter_scope '{' inner_statement_list '}' exit_scope backup_fn_flags 
{ 
				yyval.Node = _astFactory.Lambda(yypos, CombineSpans(value_stack.array[value_stack.top-15].yypos, value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-8].yypos), value_stack.array[value_stack.top-14].yyval.Long == (long)FormalParam.Flags.IsByRef, value_stack.array[value_stack.top-8].yyval.TypeReference, 
					value_stack.array[value_stack.top-11].yyval.FormalParamList, CombineSpans(value_stack.array[value_stack.top-12].yypos, value_stack.array[value_stack.top-10].yypos), value_stack.array[value_stack.top-9].yyval.FormalParamList, CreateBlock(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-3].yypos), value_stack.array[value_stack.top-4].yyval.NodeList)); 
				SetDoc(yyval.Node);
			}
        return;
      case 414: // inline_function -> fn returns_ref '(' parameter_list ')' return_type backup_doc_comment T_DOUBLE_ARROW backup_fn_flags backup_lex_pos expr backup_fn_flags 
{
				yyval.Node = _astFactory.ArrowFunc(yypos, CombineSpans(value_stack.array[value_stack.top-12].yypos, value_stack.array[value_stack.top-8].yypos, value_stack.array[value_stack.top-7].yypos), value_stack.array[value_stack.top-11].yyval.Long == (long)FormalParam.Flags.IsByRef, value_stack.array[value_stack.top-7].yyval.TypeReference, 
					value_stack.array[value_stack.top-9].yyval.FormalParamList, CombineSpans(value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-8].yypos), value_stack.array[value_stack.top-2].yyval.Node); 
				SetDoc(yyval.Node);
			}
        return;
      case 417: // backup_doc_comment -> 
{ }
        return;
      case 418: // enter_scope -> 
{ _currentScope.Increment(); }
        return;
      case 419: // exit_scope -> 
{ _currentScope.Decrement(); }
        return;
      case 420: // backup_fn_flags -> 
{  }
        return;
      case 421: // backup_lex_pos -> 
{  }
        return;
      case 422: // returns_ref -> 
{ yyval.Long = 0; }
        return;
      case 423: // returns_ref -> '&' 
{ yyval.Long = (long)FormalParam.Flags.IsByRef; }
        return;
      case 424: // lexical_vars -> 
{ yyval.FormalParamList = new List<FormalParam>(); }
        return;
      case 425: // lexical_vars -> T_USE '(' lexical_var_list ')' 
{ yyval.FormalParamList = value_stack.array[value_stack.top-2].yyval.FormalParamList; }
        return;
      case 426: // lexical_var_list -> lexical_var_list ',' lexical_var 
{ yyval.FormalParamList = AddToList<FormalParam>(value_stack.array[value_stack.top-3].yyval.FormalParamList, value_stack.array[value_stack.top-1].yyval.FormalParam); }
        return;
      case 427: // lexical_var_list -> lexical_var 
{ yyval.FormalParamList = new List<FormalParam>() { (FormalParam)value_stack.array[value_stack.top-1].yyval.FormalParam }; }
        return;
      case 428: // lexical_var -> T_VARIABLE 
{ yyval.FormalParam = _astFactory.Parameter(yypos, value_stack.array[value_stack.top-1].yyval.String, value_stack.array[value_stack.top-1].yypos, null, FormalParam.Flags.Default); }
        return;
      case 429: // lexical_var -> '&' T_VARIABLE 
{ yyval.FormalParam = _astFactory.Parameter(yypos, value_stack.array[value_stack.top-1].yyval.String, value_stack.array[value_stack.top-1].yypos, null, FormalParam.Flags.IsByRef); }
        return;
      case 430: // function_call -> name argument_list 
{ yyval.Node = _astFactory.Call(yypos, TranslateQNRFunction(value_stack.array[value_stack.top-2].yyval.QualifiedNameReference), new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), null); }
        return;
      case 431: // function_call -> class_name T_DOUBLE_COLON member_name argument_list 
{
				if (value_stack.array[value_stack.top-2].yyval.Object is string namestr)
					yyval.Node = _astFactory.Call(yypos, new Name(namestr), value_stack.array[value_stack.top-2].yypos, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-4].yyval.TypeReference); 
				else
					yyval.Node = _astFactory.Call(yypos, (LangElement)value_stack.array[value_stack.top-2].yyval.Object, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-4].yyval.TypeReference); 
			}
        return;
      case 432: // function_call -> variable_class_name T_DOUBLE_COLON member_name argument_list 
{
				if (value_stack.array[value_stack.top-2].yyval.Object is string namestr)
					yyval.Node = _astFactory.Call(yypos, new Name(namestr), value_stack.array[value_stack.top-2].yypos, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), _astFactory.TypeReference(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-4].yyval.Node)); 
				else
					yyval.Node = _astFactory.Call(yypos, (LangElement)value_stack.array[value_stack.top-2].yyval.Object, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), _astFactory.TypeReference(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-4].yyval.Node)); 
			}
        return;
      case 433: // function_call -> callable_expr argument_list 
{ yyval.Node = _astFactory.Call(yypos, value_stack.array[value_stack.top-2].yyval.Node, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), NullLangElement);}
        return;
      case 434: // class_name -> T_STATIC 
{ yyval.TypeReference = _astFactory.ReservedTypeReference(yypos, _reservedTypeStatic); }
        return;
      case 435: // class_name -> name 
{ yyval.TypeReference = CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference); }
        return;
      case 436: // class_name_reference -> class_name 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 437: // class_name_reference -> new_variable 
{ yyval.TypeReference = _astFactory.TypeReference(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 438: // exit_expr -> 
{ yyval.Node = null; }
        return;
      case 439: // exit_expr -> '(' optional_expr ')' 
{ yyval.Node = value_stack.array[value_stack.top-2].yyval.Node == null? null: _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN); }
        return;
      case 440: // backticks_expr -> '`' '`' 
{ yyval.Node = _astFactory.Literal(yypos, string.Empty, "``"); }
        return;
      case 441: // backticks_expr -> '`' T_ENCAPSED_AND_WHITESPACE '`' 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-2].yyval.Strings.Key, string.Format("`{0}`", value_stack.array[value_stack.top-2].yyval.Strings.Value)); }
        return;
      case 442: // backticks_expr -> '`' encaps_list '`' 
{ yyval.Node = _astFactory.StringEncapsedExpression(yypos, _astFactory.Concat(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.NodeList), Tokens.T_BACKQUOTE); }
        return;
      case 443: // ctor_arguments -> 
{ yyval.ParamList = new List<ActualParam>(); }
        return;
      case 444: // ctor_arguments -> argument_list 
{ yyval.ParamList = value_stack.array[value_stack.top-1].yyval.ParamList; }
        return;
      case 445: // dereferencable_scalar -> T_ARRAY '(' array_pair_list ')' 
{ yyval.Node = _astFactory.NewArray(yypos, value_stack.array[value_stack.top-2].yyval.ItemList, true); }
        return;
      case 446: // dereferencable_scalar -> '[' array_pair_list ']' 
{ yyval.Node = _astFactory.NewArray(yypos, value_stack.array[value_stack.top-2].yyval.ItemList, false); }
        return;
      case 447: // dereferencable_scalar -> T_CONSTANT_ENCAPSED_STRING 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.String, _lexer.TokenText); }
        return;
      case 448: // scalar -> T_LNUMBER 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.Long, _lexer.TokenText); }
        return;
      case 449: // scalar -> T_DNUMBER 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.Double, _lexer.TokenText); }
        return;
      case 450: // scalar -> T_LINE 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Line); }
        return;
      case 451: // scalar -> T_FILE 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.File); }
        return;
      case 452: // scalar -> T_DIR 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Dir); }
        return;
      case 453: // scalar -> T_TRAIT_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Trait); }
        return;
      case 454: // scalar -> T_METHOD_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Method); }
        return;
      case 455: // scalar -> T_FUNC_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Function); }
        return;
      case 456: // scalar -> T_NS_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Namespace); }
        return;
      case 457: // scalar -> T_CLASS_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Class); }
        return;
      case 458: // scalar -> '"' encaps_list '"' 
{ yyval.Node = _astFactory.StringEncapsedExpression(yypos, _astFactory.Concat(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.NodeList), Tokens.T_DOUBLE_QUOTES); }
        return;
      case 459: // scalar -> T_START_HEREDOC T_END_HEREDOC 
{ yyval.Node = _astFactory.HeredocExpression(yypos, _astFactory.Literal(new Span(value_stack.array[value_stack.top-2].yypos.End, 0), "", ""), value_stack.array[value_stack.top-2].yyval.QuoteToken, value_stack.array[value_stack.top-2].yyval.String); }
        return;
      case 460: // scalar -> T_START_HEREDOC T_ENCAPSED_AND_WHITESPACE T_END_HEREDOC 
{ yyval.Node = _astFactory.HeredocExpression(yypos, _astFactory.Literal(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.Strings.Key, value_stack.array[value_stack.top-2].yyval.Strings.Value), value_stack.array[value_stack.top-3].yyval.QuoteToken, value_stack.array[value_stack.top-3].yyval.String); }
        return;
      case 461: // scalar -> T_START_HEREDOC encaps_list T_END_HEREDOC 
{ yyval.Node = _astFactory.HeredocExpression(yypos, _astFactory.Concat(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.NodeList), value_stack.array[value_stack.top-3].yyval.QuoteToken, value_stack.array[value_stack.top-3].yyval.String); }
        return;
      case 462: // scalar -> dereferencable_scalar 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 463: // scalar -> constant 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 464: // constant -> name 
{ yyval.Node = _astFactory.ConstUse(yypos, TranslateQNRConstant(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference)); }
        return;
      case 465: // constant -> class_name T_DOUBLE_COLON identifier 
{ yyval.Node = _astFactory.ClassConstUse(yypos, value_stack.array[value_stack.top-3].yyval.TypeReference, new Name(value_stack.array[value_stack.top-1].yyval.String), value_stack.array[value_stack.top-1].yypos); }
        return;
      case 466: // constant -> variable_class_name T_DOUBLE_COLON identifier 
{ yyval.Node = _astFactory.ClassConstUse(yypos, _astFactory.TypeReference(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-3].yyval.Node), new Name(value_stack.array[value_stack.top-1].yyval.String), value_stack.array[value_stack.top-1].yypos); }
        return;
      case 467: // expr -> variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 468: // expr -> expr_without_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 469: // optional_expr -> 
{ yyval.Node = null; }
        return;
      case 470: // optional_expr -> expr 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 471: // object_operator -> T_OBJECT_OPERATOR 
{ yyval.Token = Tokens.T_OBJECT_OPERATOR; }
        return;
      case 472: // object_operator -> T_NULLSAFE_OBJECT_OPERATOR 
{ yyval.Token = Tokens.T_NULLSAFE_OBJECT_OPERATOR; }
        return;
      case 473: // variable_class_name -> dereferencable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; /* TODO if (!($1 is VarLikeConstructUse)) _errors.Error(@$, FatalErrors.CheckVarUseFault); */ }
        return;
      case 474: // dereferencable -> variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 475: // dereferencable -> '(' expr ')' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN); }
        return;
      case 476: // dereferencable -> dereferencable_scalar 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 477: // callable_expr -> callable_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 478: // callable_expr -> '(' expr ')' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN); }
        return;
      case 479: // callable_expr -> dereferencable_scalar 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 480: // callable_variable -> simple_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 481: // callable_variable -> dereferencable '[' optional_expr ']' 
{ yyval.Node = _astFactory.ArrayItem(yypos, false, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 482: // callable_variable -> constant '[' optional_expr ']' 
{ yyval.Node = _astFactory.ArrayItem(yypos, false, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 483: // callable_variable -> dereferencable '{' expr '}' 
{ yyval.Node = _astFactory.ArrayItem(yypos, true, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 484: // callable_variable -> dereferencable object_operator property_name argument_list 
{
			if (value_stack.array[value_stack.top-2].yyval.Object is string name)
				yyval.Node = _astFactory.Call(yypos, new TranslatedQualifiedName(new QualifiedName(new Name(name)), value_stack.array[value_stack.top-2].yypos), new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), VerifyMemberOf(value_stack.array[value_stack.top-4].yyval.Node));
			else
				yyval.Node = _astFactory.Call(yypos, (LangElement)value_stack.array[value_stack.top-2].yyval.Object, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), VerifyMemberOf(value_stack.array[value_stack.top-4].yyval.Node));

			AdjustNullSafeOperator(yyval.Node, value_stack.array[value_stack.top-3].yyval.Token);
		}
        return;
      case 485: // callable_variable -> function_call 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 486: // variable -> callable_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 487: // variable -> static_member 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 488: // variable -> dereferencable object_operator property_name 
{
			yyval.Node = AdjustNullSafeOperator(CreateProperty(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Object), value_stack.array[value_stack.top-2].yyval.Token);
		}
        return;
      case 489: // simple_variable -> T_VARIABLE 
{ yyval.Node = _astFactory.Variable(yypos, value_stack.array[value_stack.top-1].yyval.String,	NullLangElement, true); }
        return;
      case 490: // simple_variable -> '$' '{' expr '}' 
{ yyval.Node = _astFactory.Variable(yypos, _astFactory.EncapsedExpression(Span.Combine(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LBRACE), NullLangElement); }
        return;
      case 491: // simple_variable -> '$' simple_variable 
{ yyval.Node = _astFactory.Variable(yypos, value_stack.array[value_stack.top-1].yyval.Node, NullLangElement); }
        return;
      case 492: // static_member -> class_name T_DOUBLE_COLON simple_variable 
{ yyval.Node = CreateStaticProperty(yypos, value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 493: // static_member -> variable_class_name T_DOUBLE_COLON simple_variable 
{ yyval.Node = CreateStaticProperty(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 494: // new_variable -> simple_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 495: // new_variable -> new_variable '[' optional_expr ']' 
{ yyval.Node = _astFactory.ArrayItem(yypos, false, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 496: // new_variable -> new_variable '{' expr '}' 
{ yyval.Node = _astFactory.ArrayItem(yypos, true, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 497: // new_variable -> new_variable object_operator property_name 
{ yyval.Node = AdjustNullSafeOperator(CreateProperty(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Object), value_stack.array[value_stack.top-2].yyval.Token); }
        return;
      case 498: // new_variable -> class_name T_DOUBLE_COLON simple_variable 
{ yyval.Node = CreateStaticProperty(yypos, value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 499: // new_variable -> new_variable T_DOUBLE_COLON simple_variable 
{ yyval.Node = CreateStaticProperty(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 500: // member_name -> identifier 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.String; }
        return;
      case 501: // member_name -> '{' expr '}' 
{ yyval.Object = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LBRACE); }
        return;
      case 502: // member_name -> simple_variable 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 503: // property_name -> T_STRING 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.String; }
        return;
      case 504: // property_name -> '{' expr '}' 
{ yyval.Object = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LBRACE); }
        return;
      case 505: // property_name -> simple_variable 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 506: // array_pair_list -> non_empty_array_pair_list 
{ yyval.ItemList = value_stack.array[value_stack.top-1].yyval.ItemList;  }
        return;
      case 507: // possible_array_pair -> 
{ yyval.Item = null; }
        return;
      case 508: // possible_array_pair -> array_pair 
{ yyval.Item = value_stack.array[value_stack.top-1].yyval.Item; }
        return;
      case 509: // non_empty_array_pair_list -> non_empty_array_pair_list ',' possible_array_pair 
{ yyval.ItemList = AddToList<Item>(value_stack.array[value_stack.top-3].yyval.ItemList, value_stack.array[value_stack.top-1].yyval.Item); }
        return;
      case 510: // non_empty_array_pair_list -> possible_array_pair 
{ yyval.ItemList = new List<Item>() { value_stack.array[value_stack.top-1].yyval.Item }; }
        return;
      case 511: // array_pair -> expr T_DOUBLE_ARROW expr 
{ yyval.Item = _astFactory.ArrayItemValue(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 512: // array_pair -> expr 
{ yyval.Item = _astFactory.ArrayItemValue(yypos, null, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 513: // array_pair -> expr T_DOUBLE_ARROW '&' variable 
{ yyval.Item = _astFactory.ArrayItemRef(yypos, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 514: // array_pair -> '&' variable 
{ yyval.Item = _astFactory.ArrayItemRef(yypos, null, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 515: // array_pair -> T_ELLIPSIS expr 
{ yyval.Item = _astFactory.ArrayItemSpread(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 516: // array_pair -> expr T_DOUBLE_ARROW T_LIST '(' array_pair_list ')' 
{ yyval.Item = _astFactory.ArrayItemValue(yypos, value_stack.array[value_stack.top-6].yyval.Node, _astFactory.List(Span.Combine(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.ItemList, true)); }
        return;
      case 517: // array_pair -> T_LIST '(' array_pair_list ')' 
{ yyval.Item = _astFactory.ArrayItemValue(yypos, null, _astFactory.List(Span.Combine(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.ItemList, true)); }
        return;
      case 518: // encaps_list -> encaps_list encaps_var 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 519: // encaps_list -> encaps_list T_ENCAPSED_AND_WHITESPACE 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, _astFactory.Literal(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.Strings.Key, _lexer.TokenText)); }
        return;
      case 520: // encaps_list -> encaps_var 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 521: // encaps_list -> T_ENCAPSED_AND_WHITESPACE encaps_var 
{ yyval.NodeList = new List<LangElement>() { _astFactory.Literal(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.Strings.Key, value_stack.array[value_stack.top-2].yyval.Strings.Value), value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 522: // encaps_var -> T_VARIABLE 
{ yyval.Node = _astFactory.Variable(yypos, value_stack.array[value_stack.top-1].yyval.String, NullLangElement, true); }
        return;
      case 523: // encaps_var -> T_VARIABLE '[' encaps_var_offset ']' 
{ yyval.Node = _astFactory.ArrayItem(yypos, false,
					_astFactory.Variable(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-4].yyval.String, NullLangElement, true), value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 524: // encaps_var -> T_VARIABLE object_operator T_STRING 
{ yyval.Node = AdjustNullSafeOperator(CreateProperty(yypos, _astFactory.Variable(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-3].yyval.String, NullLangElement, true), value_stack.array[value_stack.top-1].yyval.String), value_stack.array[value_stack.top-2].yyval.Token); }
        return;
      case 525: // encaps_var -> T_DOLLAR_OPEN_CURLY_BRACES expr '}' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, _astFactory.Variable(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.Node, NullLangElement), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
        return;
      case 526: // encaps_var -> T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '}' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, _astFactory.Variable(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.String, NullLangElement, false), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
        return;
      case 527: // encaps_var -> T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '[' expr ']' '}' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, _astFactory.ArrayItem(Span.Combine(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-2].yypos), false,
					_astFactory.Variable(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-5].yyval.String, NullLangElement, false), value_stack.array[value_stack.top-3].yyval.Node), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
        return;
      case 528: // encaps_var -> T_CURLY_OPEN variable '}' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LBRACE); }
        return;
      case 529: // encaps_var_offset -> T_STRING 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.String, _lexer.TokenText); }
        return;
      case 530: // encaps_var_offset -> T_NUM_STRING 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.Long, _lexer.TokenText); }
        return;
      case 531: // encaps_var_offset -> T_VARIABLE 
{ yyval.Node = _astFactory.Variable(yypos, value_stack.array[value_stack.top-1].yyval.String, NullLangElement, true); }
        return;
      case 532: // internal_functions_in_yacc -> T_ISSET '(' isset_variables possible_comma ')' 
{ yyval.Node = _astFactory.Isset(yypos, AddTrailingComma(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-2].yyval.Bool)); }
        return;
      case 533: // internal_functions_in_yacc -> T_EMPTY '(' expr ')' 
{ yyval.Node = _astFactory.Empty(yypos, value_stack.array[value_stack.top-2].yyval.Node);}
        return;
      case 534: // internal_functions_in_yacc -> T_INCLUDE expr 
{ yyval.Node = _astFactory.Inclusion(yypos, isConditional, InclusionTypes.Include, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 535: // internal_functions_in_yacc -> T_INCLUDE_ONCE expr 
{ yyval.Node = _astFactory.Inclusion(yypos, isConditional, InclusionTypes.IncludeOnce, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 536: // internal_functions_in_yacc -> T_EVAL '(' expr ')' 
{ yyval.Node = _astFactory.Eval(yypos, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 537: // internal_functions_in_yacc -> T_REQUIRE expr 
{ yyval.Node = _astFactory.Inclusion(yypos, isConditional, InclusionTypes.Require, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 538: // internal_functions_in_yacc -> T_REQUIRE_ONCE expr 
{ yyval.Node = _astFactory.Inclusion(yypos, isConditional, InclusionTypes.RequireOnce, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 539: // isset_variables -> isset_variable 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 540: // isset_variables -> isset_variables ',' isset_variable 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 541: // isset_variable -> expr 
{ yyval.Node = CreateIssetVar(value_stack.array[value_stack.top-1].yyval.Node); }
        return;
    }
  }

  protected override string TerminalToString(int terminal)
  {
    if (((Tokens)terminal).ToString() != terminal.ToString())
      return ((Tokens)terminal).ToString();
    else
      return CharToString((char)terminal);
  }


}
}
