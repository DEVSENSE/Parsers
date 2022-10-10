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
/// <summary> &quot;&amp;&quot;</summary>
T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG=401,
/// <summary> &quot;&amp;&quot;</summary>
T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG=400,
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
T_NOELSE=180,
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
/// <summary>&quot;readonly (T_READONLY)&quot;</summary>
T_READONLY=398,
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
/// <summary> &quot;enum&quot; (T_ENUM)</summary>
T_ENUM=388,
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
/// <summary>&quot;#[ (T_ATTRIBUTE)&quot;</summary>
T_ATTRIBUTE=397,
/// <summary> type safe declaration</summary>
T_ERROR=257,
EOF=0
};

[StructLayout(LayoutKind.Explicit)]
public partial struct SemanticValueType
{
	public bool Bool		{ get => Long != 0;			set => Long = value ? 1 : 0; }
	public int Integer		{ get => (int)Long;			set => Long = value; }
	public AliasKind Kind	{ get => (AliasKind)Long;	set => Long = (long)value; }
    /// <summary>Token that encapsulates the string literal.</summary>
    public Tokens QuoteToken { get => (Tokens)Long;		set => Long = (long)value; }
    /// <summary>The original token.</summary>
    public Tokens Token		{ get => (Tokens)Long;		set => Long = (long)value; }

	// Integer and Offset are both used when generating code for string 
	// with 'inline' variables. Other fields are not combined.
	
	[FieldOffset(0)]
	public double Double;
	[FieldOffset(0)]
	public long Long;

	[FieldOffset(8)]
	public object Object;

	public QualifiedNameRef QualifiedNameReference		{ get { return (QualifiedNameRef)Object; }			set { Object = value; } }
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
	public Lexer.HereDocTokenValue HereDocValue			{ get { return (Lexer.HereDocTokenValue)Object; }	set { Object = value; } }
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
      new State(0, -2, new int[] {-162,1,-164,3}),
      new State(1, new int[] {0,2}),
      new State(2, -1),
      new State(3, -87, new int[] {-111,4}),
      new State(4, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,1053,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,1064,350,1068,344,1124,0,-3,322,-453,361,-188}, new int[] {-41,5,-42,6,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,1061,-98,522,-102,523,-95,1063,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(5, -86),
      new State(6, -105),
      new State(7, -142, new int[] {-114,8}),
      new State(8, new int[] {125,9,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(9, -147),
      new State(10, -141),
      new State(11, -143),
      new State(12, new int[] {322,1032}, new int[] {-63,13,-64,15,-155,17,-156,1039}),
      new State(13, -454, new int[] {-20,14}),
      new State(14, -148),
      new State(15, -454, new int[] {-20,16}),
      new State(16, -149),
      new State(17, new int[] {308,18,309,1030,123,-242,330,-242,329,-242,328,-242,335,-242,339,-242,340,-242,348,-242,355,-242,353,-242,324,-242,321,-242,320,-242,36,-242,319,-242,391,-242,393,-242,40,-242,368,-242,91,-242,323,-242,367,-242,307,-242,303,-242,302,-242,43,-242,45,-242,33,-242,126,-242,306,-242,358,-242,359,-242,262,-242,261,-242,260,-242,259,-242,258,-242,301,-242,300,-242,299,-242,298,-242,297,-242,296,-242,304,-242,326,-242,64,-242,317,-242,312,-242,370,-242,371,-242,375,-242,374,-242,378,-242,376,-242,392,-242,373,-242,34,-242,383,-242,96,-242,266,-242,267,-242,269,-242,352,-242,346,-242,343,-242,397,-242,395,-242,360,-242,334,-242,332,-242,59,-242,349,-242,345,-242,315,-242,314,-242,398,-242,362,-242,366,-242,388,-242,363,-242,350,-242,344,-242,322,-242,361,-242,0,-242,125,-242,341,-242,342,-242,336,-242,337,-242,331,-242,333,-242,327,-242,310,-242}),
      new State(18, new int[] {40,19}),
      new State(19, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,20,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(20, new int[] {41,21,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(21, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,322,-453}, new int[] {-42,22,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(22, -241),
      new State(23, new int[] {40,24}),
      new State(24, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,25,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(25, new int[] {41,26,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(26, -453, new int[] {-19,27}),
      new State(27, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,58,1026,322,-453}, new int[] {-83,28,-42,30,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(28, -454, new int[] {-20,29}),
      new State(29, -150),
      new State(30, -238),
      new State(31, -453, new int[] {-19,32}),
      new State(32, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,322,-453}, new int[] {-42,33,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(33, new int[] {330,34}),
      new State(34, new int[] {40,35}),
      new State(35, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,36,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(36, new int[] {41,37,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(37, new int[] {59,38}),
      new State(38, -454, new int[] {-20,39}),
      new State(39, -151),
      new State(40, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,41,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(41, new int[] {284,-388,285,42,263,-388,265,-388,264,-388,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-388,283,-388,59,-388,41,-388,125,-388,58,-388,93,-388,44,-388,268,-388,338,-388}),
      new State(42, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,43,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(43, new int[] {284,-389,285,-389,263,-389,265,-389,264,-389,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-389,283,-389,59,-389,41,-389,125,-389,58,-389,93,-389,44,-389,268,-389,338,-389}),
      new State(44, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,45,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(45, new int[] {284,40,285,42,263,-390,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-390,41,-390,125,-390,58,-390,93,-390,44,-390,268,-390,338,-390}),
      new State(46, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,47,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(47, new int[] {284,40,285,42,263,-391,265,-391,264,-391,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-391,41,-391,125,-391,58,-391,93,-391,44,-391,268,-391,338,-391}),
      new State(48, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,49,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(49, new int[] {284,40,285,42,263,-392,265,46,264,-392,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-392,41,-392,125,-392,58,-392,93,-392,44,-392,268,-392,338,-392}),
      new State(50, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,51,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(51, new int[] {284,-393,285,-393,263,-393,265,-393,264,-393,124,-393,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-393,283,-393,59,-393,41,-393,125,-393,58,-393,93,-393,44,-393,268,-393,338,-393}),
      new State(52, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,53,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(53, new int[] {284,-394,285,-394,263,-394,265,-394,264,-394,124,-394,401,-394,400,-394,94,-394,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-394,283,-394,59,-394,41,-394,125,-394,58,-394,93,-394,44,-394,268,-394,338,-394}),
      new State(54, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,55,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(55, new int[] {284,-395,285,-395,263,-395,265,-395,264,-395,124,-395,401,-395,400,-395,94,-395,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-395,283,-395,59,-395,41,-395,125,-395,58,-395,93,-395,44,-395,268,-395,338,-395}),
      new State(56, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,57,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(57, new int[] {284,-396,285,-396,263,-396,265,-396,264,-396,124,-396,401,52,400,54,94,-396,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-396,283,-396,59,-396,41,-396,125,-396,58,-396,93,-396,44,-396,268,-396,338,-396}),
      new State(58, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,59,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(59, new int[] {284,-397,285,-397,263,-397,265,-397,264,-397,124,-397,401,-397,400,-397,94,-397,46,-397,43,-397,45,-397,42,64,305,66,47,68,37,70,293,-397,294,-397,287,-397,286,-397,289,-397,288,-397,60,-397,291,-397,62,-397,292,-397,290,-397,295,94,63,-397,283,-397,59,-397,41,-397,125,-397,58,-397,93,-397,44,-397,268,-397,338,-397}),
      new State(60, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,61,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(61, new int[] {284,-398,285,-398,263,-398,265,-398,264,-398,124,-398,401,-398,400,-398,94,-398,46,-398,43,-398,45,-398,42,64,305,66,47,68,37,70,293,-398,294,-398,287,-398,286,-398,289,-398,288,-398,60,-398,291,-398,62,-398,292,-398,290,-398,295,94,63,-398,283,-398,59,-398,41,-398,125,-398,58,-398,93,-398,44,-398,268,-398,338,-398}),
      new State(62, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,63,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(63, new int[] {284,-399,285,-399,263,-399,265,-399,264,-399,124,-399,401,-399,400,-399,94,-399,46,-399,43,-399,45,-399,42,64,305,66,47,68,37,70,293,-399,294,-399,287,-399,286,-399,289,-399,288,-399,60,-399,291,-399,62,-399,292,-399,290,-399,295,94,63,-399,283,-399,59,-399,41,-399,125,-399,58,-399,93,-399,44,-399,268,-399,338,-399}),
      new State(64, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,65,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(65, new int[] {284,-400,285,-400,263,-400,265,-400,264,-400,124,-400,401,-400,400,-400,94,-400,46,-400,43,-400,45,-400,42,-400,305,66,47,-400,37,-400,293,-400,294,-400,287,-400,286,-400,289,-400,288,-400,60,-400,291,-400,62,-400,292,-400,290,-400,295,94,63,-400,283,-400,59,-400,41,-400,125,-400,58,-400,93,-400,44,-400,268,-400,338,-400}),
      new State(66, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,67,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(67, new int[] {284,-401,285,-401,263,-401,265,-401,264,-401,124,-401,401,-401,400,-401,94,-401,46,-401,43,-401,45,-401,42,-401,305,66,47,-401,37,-401,293,-401,294,-401,287,-401,286,-401,289,-401,288,-401,60,-401,291,-401,62,-401,292,-401,290,-401,295,-401,63,-401,283,-401,59,-401,41,-401,125,-401,58,-401,93,-401,44,-401,268,-401,338,-401}),
      new State(68, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,69,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(69, new int[] {284,-402,285,-402,263,-402,265,-402,264,-402,124,-402,401,-402,400,-402,94,-402,46,-402,43,-402,45,-402,42,-402,305,66,47,-402,37,-402,293,-402,294,-402,287,-402,286,-402,289,-402,288,-402,60,-402,291,-402,62,-402,292,-402,290,-402,295,94,63,-402,283,-402,59,-402,41,-402,125,-402,58,-402,93,-402,44,-402,268,-402,338,-402}),
      new State(70, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,71,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(71, new int[] {284,-403,285,-403,263,-403,265,-403,264,-403,124,-403,401,-403,400,-403,94,-403,46,-403,43,-403,45,-403,42,-403,305,66,47,-403,37,-403,293,-403,294,-403,287,-403,286,-403,289,-403,288,-403,60,-403,291,-403,62,-403,292,-403,290,-403,295,94,63,-403,283,-403,59,-403,41,-403,125,-403,58,-403,93,-403,44,-403,268,-403,338,-403}),
      new State(72, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,73,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(73, new int[] {284,-404,285,-404,263,-404,265,-404,264,-404,124,-404,401,-404,400,-404,94,-404,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,-404,294,-404,287,-404,286,-404,289,-404,288,-404,60,-404,291,-404,62,-404,292,-404,290,-404,295,94,63,-404,283,-404,59,-404,41,-404,125,-404,58,-404,93,-404,44,-404,268,-404,338,-404}),
      new State(74, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,75,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(75, new int[] {284,-405,285,-405,263,-405,265,-405,264,-405,124,-405,401,-405,400,-405,94,-405,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,-405,294,-405,287,-405,286,-405,289,-405,288,-405,60,-405,291,-405,62,-405,292,-405,290,-405,295,94,63,-405,283,-405,59,-405,41,-405,125,-405,58,-405,93,-405,44,-405,268,-405,338,-405}),
      new State(76, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,77,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(77, new int[] {284,-410,285,-410,263,-410,265,-410,264,-410,124,-410,401,-410,400,-410,94,-410,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-410,283,-410,59,-410,41,-410,125,-410,58,-410,93,-410,44,-410,268,-410,338,-410}),
      new State(78, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,79,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(79, new int[] {284,-411,285,-411,263,-411,265,-411,264,-411,124,-411,401,-411,400,-411,94,-411,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-411,283,-411,59,-411,41,-411,125,-411,58,-411,93,-411,44,-411,268,-411,338,-411}),
      new State(80, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,81,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(81, new int[] {284,-412,285,-412,263,-412,265,-412,264,-412,124,-412,401,-412,400,-412,94,-412,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-412,283,-412,59,-412,41,-412,125,-412,58,-412,93,-412,44,-412,268,-412,338,-412}),
      new State(82, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,83,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(83, new int[] {284,-413,285,-413,263,-413,265,-413,264,-413,124,-413,401,-413,400,-413,94,-413,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-413,283,-413,59,-413,41,-413,125,-413,58,-413,93,-413,44,-413,268,-413,338,-413}),
      new State(84, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,85,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(85, new int[] {284,-414,285,-414,263,-414,265,-414,264,-414,124,-414,401,-414,400,-414,94,-414,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,-414,286,-414,289,-414,288,-414,60,84,291,86,62,88,292,90,290,-414,295,94,63,-414,283,-414,59,-414,41,-414,125,-414,58,-414,93,-414,44,-414,268,-414,338,-414}),
      new State(86, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,87,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(87, new int[] {284,-415,285,-415,263,-415,265,-415,264,-415,124,-415,401,-415,400,-415,94,-415,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,-415,286,-415,289,-415,288,-415,60,84,291,86,62,88,292,90,290,-415,295,94,63,-415,283,-415,59,-415,41,-415,125,-415,58,-415,93,-415,44,-415,268,-415,338,-415}),
      new State(88, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,89,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(89, new int[] {284,-416,285,-416,263,-416,265,-416,264,-416,124,-416,401,-416,400,-416,94,-416,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,-416,286,-416,289,-416,288,-416,60,84,291,86,62,88,292,90,290,-416,295,94,63,-416,283,-416,59,-416,41,-416,125,-416,58,-416,93,-416,44,-416,268,-416,338,-416}),
      new State(90, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,91,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(91, new int[] {284,-417,285,-417,263,-417,265,-417,264,-417,124,-417,401,-417,400,-417,94,-417,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,-417,286,-417,289,-417,288,-417,60,84,291,86,62,88,292,90,290,-417,295,94,63,-417,283,-417,59,-417,41,-417,125,-417,58,-417,93,-417,44,-417,268,-417,338,-417}),
      new State(92, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,93,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(93, new int[] {284,-418,285,-418,263,-418,265,-418,264,-418,124,-418,401,-418,400,-418,94,-418,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-418,283,-418,59,-418,41,-418,125,-418,58,-418,93,-418,44,-418,268,-418,338,-418}),
      new State(94, new int[] {353,316,319,204,391,205,393,208,320,99,36,100,40,914}, new int[] {-31,95,-32,96,-21,520,-133,201,-88,897,-56,913}),
      new State(95, -419),
      new State(96, new int[] {390,97,59,-471,284,-471,285,-471,263,-471,265,-471,264,-471,124,-471,401,-471,400,-471,94,-471,46,-471,43,-471,45,-471,42,-471,305,-471,47,-471,37,-471,293,-471,294,-471,287,-471,286,-471,289,-471,288,-471,60,-471,291,-471,62,-471,292,-471,290,-471,295,-471,63,-471,283,-471,41,-471,125,-471,58,-471,93,-471,44,-471,268,-471,338,-471,40,-471}),
      new State(97, new int[] {320,99,36,100}, new int[] {-56,98}),
      new State(98, -537),
      new State(99, -528),
      new State(100, new int[] {123,101,320,99,36,100}, new int[] {-56,1025}),
      new State(101, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,102,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(102, new int[] {125,103,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(103, -529),
      new State(104, new int[] {58,1023,320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,105,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(105, new int[] {58,106,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(106, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,107,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(107, new int[] {284,40,285,42,263,-422,265,-422,264,-422,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-422,283,108,59,-422,41,-422,125,-422,58,-422,93,-422,44,-422,268,-422,338,-422}),
      new State(108, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,109,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(109, new int[] {284,40,285,42,263,-424,265,-424,264,-424,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-424,283,108,59,-424,41,-424,125,-424,58,-424,93,-424,44,-424,268,-424,338,-424}),
      new State(110, new int[] {61,111,270,995,271,997,279,999,281,1001,278,1003,277,1005,276,1007,275,1009,274,1011,273,1013,272,1015,280,1017,282,1019,303,1021,302,1022,59,-504,284,-504,285,-504,263,-504,265,-504,264,-504,124,-504,401,-504,400,-504,94,-504,46,-504,43,-504,45,-504,42,-504,305,-504,47,-504,37,-504,293,-504,294,-504,287,-504,286,-504,289,-504,288,-504,60,-504,291,-504,62,-504,292,-504,290,-504,295,-504,63,-504,283,-504,41,-504,125,-504,58,-504,93,-504,44,-504,268,-504,338,-504,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(111, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863}, new int[] {-50,112,-165,113,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(112, new int[] {284,40,285,42,263,-367,265,-367,264,-367,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-367,41,-367,125,-367,58,-367,93,-367,44,-367,268,-367,338,-367}),
      new State(113, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325,306,364}, new int[] {-51,114,-53,115,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(114, new int[] {59,-368,284,-368,285,-368,263,-368,265,-368,264,-368,124,-368,401,-368,400,-368,94,-368,46,-368,43,-368,45,-368,42,-368,305,-368,47,-368,37,-368,293,-368,294,-368,287,-368,286,-368,289,-368,288,-368,60,-368,291,-368,62,-368,292,-368,290,-368,295,-368,63,-368,283,-368,41,-368,125,-368,58,-368,93,-368,44,-368,268,-368,338,-368,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(115, -369),
      new State(116, new int[] {61,-525,270,-525,271,-525,279,-525,281,-525,278,-525,277,-525,276,-525,275,-525,274,-525,273,-525,272,-525,280,-525,282,-525,303,-525,302,-525,59,-525,284,-525,285,-525,263,-525,265,-525,264,-525,124,-525,401,-525,400,-525,94,-525,46,-525,43,-525,45,-525,42,-525,305,-525,47,-525,37,-525,293,-525,294,-525,287,-525,286,-525,289,-525,288,-525,60,-525,291,-525,62,-525,292,-525,290,-525,295,-525,63,-525,283,-525,91,-525,123,-525,369,-525,396,-525,390,-525,41,-525,125,-525,58,-525,93,-525,44,-525,268,-525,338,-525,40,-517}),
      new State(117, -520),
      new State(118, new int[] {91,119,123,992,369,467,396,468}, new int[] {-22,989}),
      new State(119, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,93,-506}, new int[] {-70,120,-50,122,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(120, new int[] {93,121}),
      new State(121, -521),
      new State(122, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,93,-507,59,-507,41,-507}),
      new State(123, new int[] {91,-515,123,-515,369,-515,396,-515,390,-510}),
      new State(124, -526),
      new State(125, new int[] {390,126}),
      new State(126, new int[] {320,99,36,100,319,216,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295,123,296}, new int[] {-56,127,-130,128,-2,129,-131,217,-132,218}),
      new State(127, new int[] {61,-531,270,-531,271,-531,279,-531,281,-531,278,-531,277,-531,276,-531,275,-531,274,-531,273,-531,272,-531,280,-531,282,-531,303,-531,302,-531,59,-531,284,-531,285,-531,263,-531,265,-531,264,-531,124,-531,401,-531,400,-531,94,-531,46,-531,43,-531,45,-531,42,-531,305,-531,47,-531,37,-531,293,-531,294,-531,287,-531,286,-531,289,-531,288,-531,60,-531,291,-531,62,-531,292,-531,290,-531,295,-531,63,-531,283,-531,91,-531,123,-531,369,-531,396,-531,390,-531,41,-531,125,-531,58,-531,93,-531,44,-531,268,-531,338,-531,40,-541}),
      new State(128, new int[] {91,-502,123,-502,369,-502,396,-502,390,-502,59,-502,284,-502,285,-502,263,-502,265,-502,264,-502,124,-502,401,-502,400,-502,94,-502,46,-502,43,-502,45,-502,42,-502,305,-502,47,-502,37,-502,293,-502,294,-502,287,-502,286,-502,289,-502,288,-502,60,-502,291,-502,62,-502,292,-502,290,-502,295,-502,63,-502,283,-502,41,-502,125,-502,58,-502,93,-502,44,-502,268,-502,338,-502,40,-539}),
      new State(129, new int[] {40,131}, new int[] {-143,130}),
      new State(130, -466),
      new State(131, new int[] {41,132,394,986,320,99,36,100,353,139,319,699,391,700,393,208,40,299,368,955,91,320,323,325,367,956,307,957,303,340,302,351,43,355,45,357,33,359,126,361,306,958,358,959,359,960,262,961,261,962,260,963,259,964,258,965,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,966,64,440,317,443,312,444,370,967,371,968,375,969,374,970,378,971,376,972,392,973,373,974,34,453,383,478,96,490,266,975,267,976,269,502,352,977,346,978,343,979,397,512,395,980,263,224,264,225,265,226,295,227,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,350,250,354,251,355,252,356,253,360,254,340,257,345,258,344,260,348,261,335,265,336,266,341,267,342,268,339,269,372,271,364,272,365,273,362,275,366,276,361,277,388,288,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-144,133,-141,988,-50,138,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-130,981,-131,217,-132,218}),
      new State(132, -289),
      new State(133, new int[] {44,136,41,-125}, new int[] {-3,134}),
      new State(134, new int[] {41,135}),
      new State(135, -290),
      new State(136, new int[] {320,99,36,100,353,139,319,699,391,700,393,208,40,299,368,955,91,320,323,325,367,956,307,957,303,340,302,351,43,355,45,357,33,359,126,361,306,958,358,959,359,960,262,961,261,962,260,963,259,964,258,965,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,966,64,440,317,443,312,444,370,967,371,968,375,969,374,970,378,971,376,972,392,973,373,974,34,453,383,478,96,490,266,975,267,976,269,502,352,977,346,978,343,979,397,512,395,980,263,224,264,225,265,226,295,227,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,350,250,354,251,355,252,356,253,360,254,340,257,345,258,344,260,348,261,335,265,336,266,341,267,342,268,339,269,372,271,364,272,365,273,362,275,366,276,361,277,388,288,315,290,314,291,313,292,357,293,311,294,398,295,394,984,41,-126}, new int[] {-141,137,-50,138,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-130,981,-131,217,-132,218}),
      new State(137, -293),
      new State(138, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-294,41,-294}),
      new State(139, new int[] {346,186,343,507,390,-469,58,-75}, new int[] {-94,140,-5,141,-6,187}),
      new State(140, -445),
      new State(141, new int[] {400,862,401,863,40,-457}, new int[] {-4,142,-165,896}),
      new State(142, -452, new int[] {-18,143}),
      new State(143, new int[] {40,144}),
      new State(144, new int[] {397,512,368,-254,372,-254,319,-254,391,-254,393,-254,63,-254,311,-254,357,-254,313,-254,398,-254,400,-254,394,-254,320,-254,41,-249}, new int[] {-147,145,-148,874,-97,895,-100,878,-98,522,-145,894,-15,880}),
      new State(145, new int[] {41,146}),
      new State(146, new int[] {350,945,58,-459,123,-459}, new int[] {-149,147}),
      new State(147, new int[] {58,872,123,-287}, new int[] {-25,148}),
      new State(148, -455, new int[] {-168,149}),
      new State(149, -453, new int[] {-19,150}),
      new State(150, new int[] {123,151}),
      new State(151, -142, new int[] {-114,152}),
      new State(152, new int[] {125,153,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(153, -454, new int[] {-20,154}),
      new State(154, -455, new int[] {-168,155}),
      new State(155, -448),
      new State(156, new int[] {40,157}),
      new State(157, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,59,-356}, new int[] {-116,158,-127,941,-50,944,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(158, new int[] {59,159}),
      new State(159, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,59,-356}, new int[] {-116,160,-127,941,-50,944,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(160, new int[] {59,161}),
      new State(161, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,41,-356}, new int[] {-116,162,-127,941,-50,944,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(162, new int[] {41,163}),
      new State(163, -453, new int[] {-19,164}),
      new State(164, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,58,937,322,-453}, new int[] {-81,165,-42,167,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(165, -454, new int[] {-20,166}),
      new State(166, -152),
      new State(167, -214),
      new State(168, new int[] {40,169}),
      new State(169, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,170,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(170, new int[] {41,171,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(171, -453, new int[] {-19,172}),
      new State(172, new int[] {123,175,58,929}, new int[] {-126,173}),
      new State(173, -454, new int[] {-20,174}),
      new State(174, -153),
      new State(175, new int[] {59,926,125,-224,341,-224,342,-224}, new int[] {-125,176}),
      new State(176, new int[] {125,177,341,178,342,923}),
      new State(177, -220),
      new State(178, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,179,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(179, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,58,921,59,922}, new int[] {-173,180}),
      new State(180, -142, new int[] {-114,181}),
      new State(181, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,125,-225,341,-225,342,-225,336,-225,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(182, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,59,-506}, new int[] {-70,183,-50,122,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(183, new int[] {59,184}),
      new State(184, -154),
      new State(185, new int[] {346,186,343,507,390,-469}, new int[] {-94,140,-5,141,-6,187}),
      new State(186, -451),
      new State(187, new int[] {400,862,401,863,40,-457}, new int[] {-4,188,-165,896}),
      new State(188, new int[] {40,189}),
      new State(189, new int[] {397,512,368,-254,372,-254,319,-254,391,-254,393,-254,63,-254,311,-254,357,-254,313,-254,398,-254,400,-254,394,-254,320,-254,41,-249}, new int[] {-147,190,-148,874,-97,895,-100,878,-98,522,-145,894,-15,880}),
      new State(190, new int[] {41,191}),
      new State(191, new int[] {58,872,268,-287}, new int[] {-25,192}),
      new State(192, -452, new int[] {-18,193}),
      new State(193, new int[] {268,194}),
      new State(194, -455, new int[] {-168,195}),
      new State(195, -456, new int[] {-175,196}),
      new State(196, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,197,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(197, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-455,41,-455,125,-455,58,-455,93,-455,44,-455,268,-455,338,-455}, new int[] {-168,198}),
      new State(198, -449),
      new State(199, new int[] {40,131,390,-470,91,-501,123,-501,369,-501,396,-501,59,-501,284,-501,285,-501,263,-501,265,-501,264,-501,124,-501,401,-501,400,-501,94,-501,46,-501,43,-501,45,-501,42,-501,305,-501,47,-501,37,-501,293,-501,294,-501,287,-501,286,-501,289,-501,288,-501,60,-501,291,-501,62,-501,292,-501,290,-501,295,-501,63,-501,283,-501,41,-501,125,-501,58,-501,93,-501,44,-501,268,-501,338,-501}, new int[] {-143,200}),
      new State(200, -465),
      new State(201, new int[] {393,202,40,-90,390,-90,91,-90,123,-90,369,-90,396,-90,59,-90,284,-90,285,-90,263,-90,265,-90,264,-90,124,-90,401,-90,400,-90,94,-90,46,-90,43,-90,45,-90,42,-90,305,-90,47,-90,37,-90,293,-90,294,-90,287,-90,286,-90,289,-90,288,-90,60,-90,291,-90,62,-90,292,-90,290,-90,295,-90,63,-90,283,-90,41,-90,125,-90,58,-90,93,-90,44,-90,268,-90,338,-90,320,-90,394,-90,365,-90,364,-90}),
      new State(202, new int[] {319,203}),
      new State(203, -89),
      new State(204, -88),
      new State(205, new int[] {393,206}),
      new State(206, new int[] {319,204}, new int[] {-133,207}),
      new State(207, new int[] {393,202,40,-91,390,-91,91,-91,123,-91,369,-91,396,-91,59,-91,284,-91,285,-91,263,-91,265,-91,264,-91,124,-91,401,-91,400,-91,94,-91,46,-91,43,-91,45,-91,42,-91,305,-91,47,-91,37,-91,293,-91,294,-91,287,-91,286,-91,289,-91,288,-91,60,-91,291,-91,62,-91,292,-91,290,-91,295,-91,63,-91,283,-91,41,-91,125,-91,58,-91,93,-91,44,-91,268,-91,338,-91,320,-91,394,-91,365,-91,364,-91}),
      new State(208, new int[] {319,204}, new int[] {-133,209}),
      new State(209, new int[] {393,202,40,-92,390,-92,91,-92,123,-92,369,-92,396,-92,59,-92,284,-92,285,-92,263,-92,265,-92,264,-92,124,-92,401,-92,400,-92,94,-92,46,-92,43,-92,45,-92,42,-92,305,-92,47,-92,37,-92,293,-92,294,-92,287,-92,286,-92,289,-92,288,-92,60,-92,291,-92,62,-92,292,-92,290,-92,295,-92,63,-92,283,-92,41,-92,125,-92,58,-92,93,-92,44,-92,268,-92,338,-92,320,-92,394,-92,365,-92,364,-92}),
      new State(210, new int[] {390,211}),
      new State(211, new int[] {320,99,36,100,319,216,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295,123,296}, new int[] {-56,212,-130,213,-2,214,-131,217,-132,218}),
      new State(212, new int[] {61,-532,270,-532,271,-532,279,-532,281,-532,278,-532,277,-532,276,-532,275,-532,274,-532,273,-532,272,-532,280,-532,282,-532,303,-532,302,-532,59,-532,284,-532,285,-532,263,-532,265,-532,264,-532,124,-532,401,-532,400,-532,94,-532,46,-532,43,-532,45,-532,42,-532,305,-532,47,-532,37,-532,293,-532,294,-532,287,-532,286,-532,289,-532,288,-532,60,-532,291,-532,62,-532,292,-532,290,-532,295,-532,63,-532,283,-532,91,-532,123,-532,369,-532,396,-532,390,-532,41,-532,125,-532,58,-532,93,-532,44,-532,268,-532,338,-532,40,-541}),
      new State(213, new int[] {91,-503,123,-503,369,-503,396,-503,390,-503,59,-503,284,-503,285,-503,263,-503,265,-503,264,-503,124,-503,401,-503,400,-503,94,-503,46,-503,43,-503,45,-503,42,-503,305,-503,47,-503,37,-503,293,-503,294,-503,287,-503,286,-503,289,-503,288,-503,60,-503,291,-503,62,-503,292,-503,290,-503,295,-503,63,-503,283,-503,41,-503,125,-503,58,-503,93,-503,44,-503,268,-503,338,-503,40,-539}),
      new State(214, new int[] {40,131}, new int[] {-143,215}),
      new State(215, -467),
      new State(216, -84),
      new State(217, -85),
      new State(218, -74),
      new State(219, -4),
      new State(220, -5),
      new State(221, -6),
      new State(222, -7),
      new State(223, -8),
      new State(224, -9),
      new State(225, -10),
      new State(226, -11),
      new State(227, -12),
      new State(228, -13),
      new State(229, -14),
      new State(230, -15),
      new State(231, -16),
      new State(232, -17),
      new State(233, -18),
      new State(234, -19),
      new State(235, -20),
      new State(236, -21),
      new State(237, -22),
      new State(238, -23),
      new State(239, -24),
      new State(240, -25),
      new State(241, -26),
      new State(242, -27),
      new State(243, -28),
      new State(244, -29),
      new State(245, -30),
      new State(246, -31),
      new State(247, -32),
      new State(248, -33),
      new State(249, -34),
      new State(250, -35),
      new State(251, -36),
      new State(252, -37),
      new State(253, -38),
      new State(254, -39),
      new State(255, -40),
      new State(256, -41),
      new State(257, -42),
      new State(258, -43),
      new State(259, -44),
      new State(260, -45),
      new State(261, -46),
      new State(262, -47),
      new State(263, -48),
      new State(264, -49),
      new State(265, -50),
      new State(266, -51),
      new State(267, -52),
      new State(268, -53),
      new State(269, -54),
      new State(270, -55),
      new State(271, -56),
      new State(272, -57),
      new State(273, -58),
      new State(274, -59),
      new State(275, -60),
      new State(276, -61),
      new State(277, -62),
      new State(278, -63),
      new State(279, -64),
      new State(280, -65),
      new State(281, -66),
      new State(282, -67),
      new State(283, -68),
      new State(284, -69),
      new State(285, -70),
      new State(286, -71),
      new State(287, -72),
      new State(288, -73),
      new State(289, -75),
      new State(290, -76),
      new State(291, -77),
      new State(292, -78),
      new State(293, -79),
      new State(294, -80),
      new State(295, -81),
      new State(296, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,297,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(297, new int[] {125,298,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(298, -540),
      new State(299, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,300,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(300, new int[] {41,301,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(301, new int[] {91,-512,123,-512,369,-512,396,-512,390,-512,40,-518,59,-420,284,-420,285,-420,263,-420,265,-420,264,-420,124,-420,401,-420,400,-420,94,-420,46,-420,43,-420,45,-420,42,-420,305,-420,47,-420,37,-420,293,-420,294,-420,287,-420,286,-420,289,-420,288,-420,60,-420,291,-420,62,-420,292,-420,290,-420,295,-420,63,-420,283,-420,41,-420,125,-420,58,-420,93,-420,44,-420,268,-420,338,-420}),
      new State(302, new int[] {91,-513,123,-513,369,-513,396,-513,390,-513,40,-519,59,-498,284,-498,285,-498,263,-498,265,-498,264,-498,124,-498,401,-498,400,-498,94,-498,46,-498,43,-498,45,-498,42,-498,305,-498,47,-498,37,-498,293,-498,294,-498,287,-498,286,-498,289,-498,288,-498,60,-498,291,-498,62,-498,292,-498,290,-498,295,-498,63,-498,283,-498,41,-498,125,-498,58,-498,93,-498,44,-498,268,-498,338,-498}),
      new State(303, new int[] {40,304}),
      new State(304, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,41,-546}, new int[] {-154,305,-153,307,-151,346,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(305, new int[] {41,306}),
      new State(306, -481),
      new State(307, new int[] {44,308,41,-545,93,-545}),
      new State(308, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,41,-546,93,-546}, new int[] {-151,309,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(309, -548),
      new State(310, -547),
      new State(311, new int[] {268,312,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-551,41,-551,93,-551}),
      new State(312, new int[] {367,917,320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863}, new int[] {-50,313,-165,314,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(313, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-550,41,-550,93,-550}),
      new State(314, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325}, new int[] {-51,315,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(315, new int[] {44,-552,41,-552,93,-552,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(316, -469),
      new State(317, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,318,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(318, new int[] {41,319,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(319, new int[] {91,-512,123,-512,369,-512,396,-512,390,-512,40,-518}),
      new State(320, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,93,-546}, new int[] {-154,321,-153,307,-151,346,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(321, new int[] {93,322}),
      new State(322, new int[] {61,323,91,-482,123,-482,369,-482,396,-482,390,-482,40,-482,59,-482,284,-482,285,-482,263,-482,265,-482,264,-482,124,-482,401,-482,400,-482,94,-482,46,-482,43,-482,45,-482,42,-482,305,-482,47,-482,37,-482,293,-482,294,-482,287,-482,286,-482,289,-482,288,-482,60,-482,291,-482,62,-482,292,-482,290,-482,295,-482,63,-482,283,-482,41,-482,125,-482,58,-482,93,-482,44,-482,268,-482,338,-482}),
      new State(323, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,324,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(324, new int[] {284,40,285,42,263,-366,265,-366,264,-366,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-366,41,-366,125,-366,58,-366,93,-366,44,-366,268,-366,338,-366}),
      new State(325, -483),
      new State(326, new int[] {91,-514,123,-514,369,-514,396,-514,390,-514,59,-500,284,-500,285,-500,263,-500,265,-500,264,-500,124,-500,401,-500,400,-500,94,-500,46,-500,43,-500,45,-500,42,-500,305,-500,47,-500,37,-500,293,-500,294,-500,287,-500,286,-500,289,-500,288,-500,60,-500,291,-500,62,-500,292,-500,290,-500,295,-500,63,-500,283,-500,41,-500,125,-500,58,-500,93,-500,44,-500,268,-500,338,-500}),
      new State(327, new int[] {91,-516,123,-516,369,-516,396,-516,59,-499,284,-499,285,-499,263,-499,265,-499,264,-499,124,-499,401,-499,400,-499,94,-499,46,-499,43,-499,45,-499,42,-499,305,-499,47,-499,37,-499,293,-499,294,-499,287,-499,286,-499,289,-499,288,-499,60,-499,291,-499,62,-499,292,-499,290,-499,295,-499,63,-499,283,-499,41,-499,125,-499,58,-499,93,-499,44,-499,268,-499,338,-499}),
      new State(328, -524),
      new State(329, new int[] {40,131}, new int[] {-143,330}),
      new State(330, -468),
      new State(331, -505),
      new State(332, new int[] {40,333}),
      new State(333, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,41,-546}, new int[] {-154,334,-153,307,-151,346,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(334, new int[] {41,335}),
      new State(335, new int[] {61,336}),
      new State(336, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,337,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(337, new int[] {284,40,285,42,263,-365,265,-365,264,-365,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-365,41,-365,125,-365,58,-365,93,-365,44,-365,268,-365,338,-365}),
      new State(338, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,339,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(339, -370),
      new State(340, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325}, new int[] {-51,341,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(341, new int[] {59,-385,284,-385,285,-385,263,-385,265,-385,264,-385,124,-385,401,-385,400,-385,94,-385,46,-385,43,-385,45,-385,42,-385,305,-385,47,-385,37,-385,293,-385,294,-385,287,-385,286,-385,289,-385,288,-385,60,-385,291,-385,62,-385,292,-385,290,-385,295,-385,63,-385,283,-385,41,-385,125,-385,58,-385,93,-385,44,-385,268,-385,338,-385,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(342, new int[] {91,-513,123,-513,369,-513,396,-513,390,-513,40,-519}),
      new State(343, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,93,-546}, new int[] {-154,344,-153,307,-151,346,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(344, new int[] {93,345}),
      new State(345, -482),
      new State(346, -549),
      new State(347, new int[] {40,348}),
      new State(348, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,41,-546}, new int[] {-154,349,-153,307,-151,346,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(349, new int[] {41,350}),
      new State(350, new int[] {61,336,44,-556,41,-556,93,-556}),
      new State(351, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325}, new int[] {-51,352,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(352, new int[] {59,-387,284,-387,285,-387,263,-387,265,-387,264,-387,124,-387,401,-387,400,-387,94,-387,46,-387,43,-387,45,-387,42,-387,305,-387,47,-387,37,-387,293,-387,294,-387,287,-387,286,-387,289,-387,288,-387,60,-387,291,-387,62,-387,292,-387,290,-387,295,-387,63,-387,283,-387,41,-387,125,-387,58,-387,93,-387,44,-387,268,-387,338,-387,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(353, -514),
      new State(354, -516),
      new State(355, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,356,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(356, new int[] {284,-406,285,-406,263,-406,265,-406,264,-406,124,-406,401,-406,400,-406,94,-406,46,-406,43,-406,45,-406,42,-406,305,66,47,-406,37,-406,293,-406,294,-406,287,-406,286,-406,289,-406,288,-406,60,-406,291,-406,62,-406,292,-406,290,-406,295,-406,63,-406,283,-406,59,-406,41,-406,125,-406,58,-406,93,-406,44,-406,268,-406,338,-406}),
      new State(357, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,358,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(358, new int[] {284,-407,285,-407,263,-407,265,-407,264,-407,124,-407,401,-407,400,-407,94,-407,46,-407,43,-407,45,-407,42,-407,305,66,47,-407,37,-407,293,-407,294,-407,287,-407,286,-407,289,-407,288,-407,60,-407,291,-407,62,-407,292,-407,290,-407,295,-407,63,-407,283,-407,59,-407,41,-407,125,-407,58,-407,93,-407,44,-407,268,-407,338,-407}),
      new State(359, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,360,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(360, new int[] {284,-408,285,-408,263,-408,265,-408,264,-408,124,-408,401,-408,400,-408,94,-408,46,-408,43,-408,45,-408,42,-408,305,66,47,-408,37,-408,293,-408,294,-408,287,-408,286,-408,289,-408,288,-408,60,-408,291,-408,62,-408,292,-408,290,-408,295,94,63,-408,283,-408,59,-408,41,-408,125,-408,58,-408,93,-408,44,-408,268,-408,338,-408}),
      new State(361, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,362,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(362, new int[] {284,-409,285,-409,263,-409,265,-409,264,-409,124,-409,401,-409,400,-409,94,-409,46,-409,43,-409,45,-409,42,-409,305,66,47,-409,37,-409,293,-409,294,-409,287,-409,286,-409,289,-409,288,-409,60,-409,291,-409,62,-409,292,-409,290,-409,295,-409,63,-409,283,-409,59,-409,41,-409,125,-409,58,-409,93,-409,44,-409,268,-409,338,-409}),
      new State(363, -421),
      new State(364, new int[] {353,316,319,204,391,205,393,208,320,99,36,100,40,914,361,372,397,512}, new int[] {-31,365,-158,368,-100,369,-32,96,-21,520,-133,201,-88,897,-56,913,-98,522}),
      new State(365, new int[] {40,131,59,-479,284,-479,285,-479,263,-479,265,-479,264,-479,124,-479,401,-479,400,-479,94,-479,46,-479,43,-479,45,-479,42,-479,305,-479,47,-479,37,-479,293,-479,294,-479,287,-479,286,-479,289,-479,288,-479,60,-479,291,-479,62,-479,292,-479,290,-479,295,-479,63,-479,283,-479,41,-479,125,-479,58,-479,93,-479,44,-479,268,-479,338,-479}, new int[] {-142,366,-143,367}),
      new State(366, -362),
      new State(367, -480),
      new State(368, -363),
      new State(369, new int[] {361,372,397,512}, new int[] {-158,370,-98,371}),
      new State(370, -364),
      new State(371, -99),
      new State(372, new int[] {40,131,364,-479,365,-479,123,-479}, new int[] {-142,373,-143,367}),
      new State(373, new int[] {364,716,365,-204,123,-204}, new int[] {-30,374}),
      new State(374, -360, new int[] {-174,375}),
      new State(375, new int[] {365,714,123,-208}, new int[] {-35,376}),
      new State(376, -452, new int[] {-18,377}),
      new State(377, -453, new int[] {-19,378}),
      new State(378, new int[] {123,379}),
      new State(379, -305, new int[] {-115,380}),
      new State(380, new int[] {125,381,311,570,357,571,313,572,353,573,315,574,314,575,398,576,356,578,341,671,397,512,350,679,344,-334,346,-334}, new int[] {-93,383,-96,384,-9,385,-11,568,-12,577,-10,579,-109,670,-100,677,-98,522}),
      new State(381, -454, new int[] {-20,382}),
      new State(382, -361),
      new State(383, -304),
      new State(384, -310),
      new State(385, new int[] {368,555,372,556,319,204,391,205,393,208,63,560,320,-264}, new int[] {-29,386,-27,551,-24,552,-21,557,-133,201,-38,562,-40,565}),
      new State(386, new int[] {320,391}, new int[] {-124,387,-71,550}),
      new State(387, new int[] {59,388,44,389}),
      new State(388, -306),
      new State(389, new int[] {320,391}, new int[] {-71,390}),
      new State(390, -345),
      new State(391, new int[] {61,393,59,-452,44,-452}, new int[] {-18,392}),
      new State(392, -347),
      new State(393, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,394,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(394, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-452,44,-452}, new int[] {-18,395}),
      new State(395, -348),
      new State(396, -425),
      new State(397, new int[] {40,398}),
      new State(398, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-123,399,-49,549,-50,404,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(399, new int[] {44,402,41,-125}, new int[] {-3,400}),
      new State(400, new int[] {41,401}),
      new State(401, -571),
      new State(402, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,41,-126}, new int[] {-49,403,-50,404,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(403, -579),
      new State(404, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-580,41,-580}),
      new State(405, new int[] {40,406}),
      new State(406, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,407,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(407, new int[] {41,408,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(408, -572),
      new State(409, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,410,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(410, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-573,41,-573,125,-573,58,-573,93,-573,44,-573,268,-573,338,-573}),
      new State(411, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,412,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(412, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-574,41,-574,125,-574,58,-574,93,-574,44,-574,268,-574,338,-574}),
      new State(413, new int[] {40,414}),
      new State(414, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,415,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(415, new int[] {41,416,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(416, -575),
      new State(417, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,418,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(418, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-576,41,-576,125,-576,58,-576,93,-576,44,-576,268,-576,338,-576}),
      new State(419, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,420,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(420, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-577,41,-577,125,-577,58,-577,93,-577,44,-577,268,-577,338,-577}),
      new State(421, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,422,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(422, new int[] {284,-426,285,-426,263,-426,265,-426,264,-426,124,-426,401,-426,400,-426,94,-426,46,-426,43,-426,45,-426,42,-426,305,66,47,-426,37,-426,293,-426,294,-426,287,-426,286,-426,289,-426,288,-426,60,-426,291,-426,62,-426,292,-426,290,-426,295,-426,63,-426,283,-426,59,-426,41,-426,125,-426,58,-426,93,-426,44,-426,268,-426,338,-426}),
      new State(423, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,424,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(424, new int[] {284,-427,285,-427,263,-427,265,-427,264,-427,124,-427,401,-427,400,-427,94,-427,46,-427,43,-427,45,-427,42,-427,305,66,47,-427,37,-427,293,-427,294,-427,287,-427,286,-427,289,-427,288,-427,60,-427,291,-427,62,-427,292,-427,290,-427,295,-427,63,-427,283,-427,59,-427,41,-427,125,-427,58,-427,93,-427,44,-427,268,-427,338,-427}),
      new State(425, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,426,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(426, new int[] {284,-428,285,-428,263,-428,265,-428,264,-428,124,-428,401,-428,400,-428,94,-428,46,-428,43,-428,45,-428,42,-428,305,66,47,-428,37,-428,293,-428,294,-428,287,-428,286,-428,289,-428,288,-428,60,-428,291,-428,62,-428,292,-428,290,-428,295,-428,63,-428,283,-428,59,-428,41,-428,125,-428,58,-428,93,-428,44,-428,268,-428,338,-428}),
      new State(427, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,428,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(428, new int[] {284,-429,285,-429,263,-429,265,-429,264,-429,124,-429,401,-429,400,-429,94,-429,46,-429,43,-429,45,-429,42,-429,305,66,47,-429,37,-429,293,-429,294,-429,287,-429,286,-429,289,-429,288,-429,60,-429,291,-429,62,-429,292,-429,290,-429,295,-429,63,-429,283,-429,59,-429,41,-429,125,-429,58,-429,93,-429,44,-429,268,-429,338,-429}),
      new State(429, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,430,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(430, new int[] {284,-430,285,-430,263,-430,265,-430,264,-430,124,-430,401,-430,400,-430,94,-430,46,-430,43,-430,45,-430,42,-430,305,66,47,-430,37,-430,293,-430,294,-430,287,-430,286,-430,289,-430,288,-430,60,-430,291,-430,62,-430,292,-430,290,-430,295,-430,63,-430,283,-430,59,-430,41,-430,125,-430,58,-430,93,-430,44,-430,268,-430,338,-430}),
      new State(431, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,432,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(432, new int[] {284,-431,285,-431,263,-431,265,-431,264,-431,124,-431,401,-431,400,-431,94,-431,46,-431,43,-431,45,-431,42,-431,305,66,47,-431,37,-431,293,-431,294,-431,287,-431,286,-431,289,-431,288,-431,60,-431,291,-431,62,-431,292,-431,290,-431,295,-431,63,-431,283,-431,59,-431,41,-431,125,-431,58,-431,93,-431,44,-431,268,-431,338,-431}),
      new State(433, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,434,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(434, new int[] {284,-432,285,-432,263,-432,265,-432,264,-432,124,-432,401,-432,400,-432,94,-432,46,-432,43,-432,45,-432,42,-432,305,66,47,-432,37,-432,293,-432,294,-432,287,-432,286,-432,289,-432,288,-432,60,-432,291,-432,62,-432,292,-432,290,-432,295,-432,63,-432,283,-432,59,-432,41,-432,125,-432,58,-432,93,-432,44,-432,268,-432,338,-432}),
      new State(435, new int[] {40,437,59,-474,284,-474,285,-474,263,-474,265,-474,264,-474,124,-474,401,-474,400,-474,94,-474,46,-474,43,-474,45,-474,42,-474,305,-474,47,-474,37,-474,293,-474,294,-474,287,-474,286,-474,289,-474,288,-474,60,-474,291,-474,62,-474,292,-474,290,-474,295,-474,63,-474,283,-474,41,-474,125,-474,58,-474,93,-474,44,-474,268,-474,338,-474}, new int[] {-86,436}),
      new State(436, -433),
      new State(437, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,41,-506}, new int[] {-70,438,-50,122,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(438, new int[] {41,439}),
      new State(439, -475),
      new State(440, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,441,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(441, new int[] {284,-434,285,-434,263,-434,265,-434,264,-434,124,-434,401,-434,400,-434,94,-434,46,-434,43,-434,45,-434,42,-434,305,66,47,-434,37,-434,293,-434,294,-434,287,-434,286,-434,289,-434,288,-434,60,-434,291,-434,62,-434,292,-434,290,-434,295,-434,63,-434,283,-434,59,-434,41,-434,125,-434,58,-434,93,-434,44,-434,268,-434,338,-434}),
      new State(442, -435),
      new State(443, -484),
      new State(444, -485),
      new State(445, -486),
      new State(446, -487),
      new State(447, -488),
      new State(448, -489),
      new State(449, -490),
      new State(450, -491),
      new State(451, -492),
      new State(452, -493),
      new State(453, new int[] {320,458,385,469,386,483,316,548}, new int[] {-122,454,-72,488}),
      new State(454, new int[] {34,455,316,457,320,458,385,469,386,483}, new int[] {-72,456}),
      new State(455, -494),
      new State(456, -557),
      new State(457, -558),
      new State(458, new int[] {91,459,369,467,396,468,34,-561,316,-561,320,-561,385,-561,386,-561,387,-561,96,-561}, new int[] {-22,465}),
      new State(459, new int[] {319,462,325,463,320,464}, new int[] {-73,460}),
      new State(460, new int[] {93,461}),
      new State(461, -562),
      new State(462, -568),
      new State(463, -569),
      new State(464, -570),
      new State(465, new int[] {319,466}),
      new State(466, -563),
      new State(467, -508),
      new State(468, -509),
      new State(469, new int[] {318,472,320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,470,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(470, new int[] {125,471,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(471, -564),
      new State(472, new int[] {125,473,91,474}),
      new State(473, -565),
      new State(474, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,475,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(475, new int[] {93,476,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(476, new int[] {125,477}),
      new State(477, -566),
      new State(478, new int[] {387,479,316,480,320,458,385,469,386,483}, new int[] {-122,486,-72,488}),
      new State(479, -495),
      new State(480, new int[] {387,481,320,458,385,469,386,483}, new int[] {-72,482}),
      new State(481, -496),
      new State(482, -560),
      new State(483, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325}, new int[] {-51,484,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(484, new int[] {125,485,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(485, -567),
      new State(486, new int[] {387,487,316,457,320,458,385,469,386,483}, new int[] {-72,456}),
      new State(487, -497),
      new State(488, -559),
      new State(489, -436),
      new State(490, new int[] {96,491,316,492,320,458,385,469,386,483}, new int[] {-122,494,-72,488}),
      new State(491, -476),
      new State(492, new int[] {96,493,320,458,385,469,386,483}, new int[] {-72,482}),
      new State(493, -477),
      new State(494, new int[] {96,495,316,457,320,458,385,469,386,483}, new int[] {-72,456}),
      new State(495, -478),
      new State(496, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,497,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(497, new int[] {284,40,285,42,263,-437,265,-437,264,-437,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-437,41,-437,125,-437,58,-437,93,-437,44,-437,268,-437,338,-437}),
      new State(498, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,59,-438,284,-438,285,-438,263,-438,265,-438,264,-438,124,-438,401,-438,400,-438,94,-438,46,-438,42,-438,305,-438,47,-438,37,-438,293,-438,294,-438,287,-438,286,-438,289,-438,288,-438,60,-438,291,-438,62,-438,292,-438,290,-438,295,-438,63,-438,283,-438,41,-438,125,-438,58,-438,93,-438,44,-438,268,-438,338,-438}, new int[] {-50,499,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(499, new int[] {268,500,284,40,285,42,263,-439,265,-439,264,-439,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-439,41,-439,125,-439,58,-439,93,-439,44,-439,338,-439}),
      new State(500, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,501,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(501, new int[] {284,40,285,42,263,-440,265,-440,264,-440,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-440,41,-440,125,-440,58,-440,93,-440,44,-440,268,-440,338,-440}),
      new State(502, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,503,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(503, new int[] {284,40,285,42,263,-441,265,-441,264,-441,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-441,41,-441,125,-441,58,-441,93,-441,44,-441,268,-441,338,-441}),
      new State(504, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,505,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(505, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-442,41,-442,125,-442,58,-442,93,-442,44,-442,268,-442,338,-442}),
      new State(506, -443),
      new State(507, -450),
      new State(508, new int[] {353,510,346,186,343,507,397,512}, new int[] {-94,509,-98,371,-5,141,-6,187}),
      new State(509, -444),
      new State(510, new int[] {346,186,343,507}, new int[] {-94,511,-5,141,-6,187}),
      new State(511, -446),
      new State(512, new int[] {353,316,319,204,391,205,393,208}, new int[] {-101,513,-99,521,-32,518,-21,520,-133,201}),
      new State(513, new int[] {44,516,93,-125}, new int[] {-3,514}),
      new State(514, new int[] {93,515}),
      new State(515, -97),
      new State(516, new int[] {353,316,319,204,391,205,393,208,93,-126}, new int[] {-99,517,-32,518,-21,520,-133,201}),
      new State(517, -96),
      new State(518, new int[] {40,131,44,-93,93,-93}, new int[] {-143,519}),
      new State(519, -94),
      new State(520, -470),
      new State(521, -95),
      new State(522, -98),
      new State(523, -447),
      new State(524, new int[] {40,525}),
      new State(525, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,526,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(526, new int[] {41,527,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(527, new int[] {123,528}),
      new State(528, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,342,542,125,-230}, new int[] {-104,529,-106,531,-103,547,-105,535,-50,541,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(529, new int[] {125,530}),
      new State(530, -229),
      new State(531, new int[] {44,533,125,-125}, new int[] {-3,532}),
      new State(532, -231),
      new State(533, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,342,542,125,-126}, new int[] {-103,534,-105,535,-50,541,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(534, -233),
      new State(535, new int[] {44,539,268,-125}, new int[] {-3,536}),
      new State(536, new int[] {268,537}),
      new State(537, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,538,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(538, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-234,125,-234}),
      new State(539, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,268,-126}, new int[] {-50,540,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(540, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-237,268,-237}),
      new State(541, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-236,268,-236}),
      new State(542, new int[] {44,546,268,-125}, new int[] {-3,543}),
      new State(543, new int[] {268,544}),
      new State(544, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,545,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(545, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-235,125,-235}),
      new State(546, -126),
      new State(547, -232),
      new State(548, new int[] {320,458,385,469,386,483}, new int[] {-72,482}),
      new State(549, -578),
      new State(550, -346),
      new State(551, -265),
      new State(552, new int[] {124,553,401,558,320,-270,400,-270,394,-270}),
      new State(553, new int[] {368,555,372,556,319,204,391,205,393,208}, new int[] {-24,554,-21,557,-133,201}),
      new State(554, -281),
      new State(555, -276),
      new State(556, -277),
      new State(557, -278),
      new State(558, new int[] {368,555,372,556,319,204,391,205,393,208}, new int[] {-24,559,-21,557,-133,201}),
      new State(559, -285),
      new State(560, new int[] {368,555,372,556,319,204,391,205,393,208}, new int[] {-24,561,-21,557,-133,201}),
      new State(561, -271),
      new State(562, new int[] {124,563,320,-272,400,-272,394,-272}),
      new State(563, new int[] {368,555,372,556,319,204,391,205,393,208}, new int[] {-24,564,-21,557,-133,201}),
      new State(564, -282),
      new State(565, new int[] {401,566,320,-273,400,-273,394,-273}),
      new State(566, new int[] {368,555,372,556,319,204,391,205,393,208}, new int[] {-24,567,-21,557,-133,201}),
      new State(567, -286),
      new State(568, new int[] {311,570,357,571,313,572,353,573,315,574,314,575,398,576,368,-332,372,-332,319,-332,391,-332,393,-332,63,-332,320,-332,344,-335,346,-335}, new int[] {-12,569}),
      new State(569, -337),
      new State(570, -338),
      new State(571, -339),
      new State(572, -340),
      new State(573, -341),
      new State(574, -342),
      new State(575, -343),
      new State(576, -344),
      new State(577, -336),
      new State(578, -333),
      new State(579, new int[] {344,580,346,186}, new int[] {-5,590}),
      new State(580, new int[] {319,216,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-113,581,-78,589,-130,585,-131,217,-132,218}),
      new State(581, new int[] {59,582,44,583}),
      new State(582, -307),
      new State(583, new int[] {319,216,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-78,584,-130,585,-131,217,-132,218}),
      new State(584, -349),
      new State(585, new int[] {61,586}),
      new State(586, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,587,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(587, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-452,44,-452}, new int[] {-18,588}),
      new State(588, -351),
      new State(589, -350),
      new State(590, new int[] {400,862,401,863,319,-457,262,-457,261,-457,260,-457,259,-457,258,-457,263,-457,264,-457,265,-457,295,-457,306,-457,307,-457,326,-457,322,-457,308,-457,309,-457,310,-457,324,-457,329,-457,330,-457,327,-457,328,-457,333,-457,334,-457,331,-457,332,-457,337,-457,338,-457,349,-457,347,-457,351,-457,352,-457,350,-457,354,-457,355,-457,356,-457,360,-457,358,-457,359,-457,340,-457,345,-457,346,-457,344,-457,348,-457,266,-457,267,-457,367,-457,335,-457,336,-457,341,-457,342,-457,339,-457,368,-457,372,-457,364,-457,365,-457,391,-457,362,-457,366,-457,361,-457,373,-457,374,-457,376,-457,378,-457,370,-457,371,-457,375,-457,392,-457,343,-457,395,-457,388,-457,353,-457,315,-457,314,-457,313,-457,357,-457,311,-457,398,-457}, new int[] {-4,591,-165,896}),
      new State(591, new int[] {319,216,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-130,592,-131,217,-132,218}),
      new State(592, -452, new int[] {-18,593}),
      new State(593, new int[] {40,594}),
      new State(594, new int[] {397,512,368,-254,372,-254,319,-254,391,-254,393,-254,63,-254,311,-254,357,-254,313,-254,398,-254,400,-254,394,-254,320,-254,41,-249}, new int[] {-147,595,-148,874,-97,895,-100,878,-98,522,-145,894,-15,880}),
      new State(595, new int[] {41,596}),
      new State(596, new int[] {58,872,59,-287,123,-287}, new int[] {-25,597}),
      new State(597, -455, new int[] {-168,598}),
      new State(598, new int[] {59,601,123,602}, new int[] {-85,599}),
      new State(599, -455, new int[] {-168,600}),
      new State(600, -308),
      new State(601, -330),
      new State(602, -142, new int[] {-114,603}),
      new State(603, new int[] {125,604,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(604, -331),
      new State(605, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,59,-506}, new int[] {-70,606,-50,122,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(606, new int[] {59,607}),
      new State(607, -155),
      new State(608, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,59,-506}, new int[] {-70,609,-50,122,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(609, new int[] {59,610}),
      new State(610, -156),
      new State(611, new int[] {320,99,36,100}, new int[] {-117,612,-67,617,-56,616}),
      new State(612, new int[] {59,613,44,614}),
      new State(613, -157),
      new State(614, new int[] {320,99,36,100}, new int[] {-67,615,-56,616}),
      new State(615, -297),
      new State(616, -299),
      new State(617, -298),
      new State(618, new int[] {320,623,346,186,343,507,390,-469}, new int[] {-118,619,-94,140,-68,626,-5,141,-6,187}),
      new State(619, new int[] {59,620,44,621}),
      new State(620, -158),
      new State(621, new int[] {320,623}, new int[] {-68,622}),
      new State(622, -300),
      new State(623, new int[] {61,624,59,-302,44,-302}),
      new State(624, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,625,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(625, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-303,44,-303}),
      new State(626, -301),
      new State(627, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-119,628,-69,633,-50,632,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(628, new int[] {59,629,44,630}),
      new State(629, -159),
      new State(630, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-69,631,-50,632,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(631, -353),
      new State(632, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-355,44,-355}),
      new State(633, -354),
      new State(634, -160),
      new State(635, new int[] {59,636,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(636, -161),
      new State(637, new int[] {58,638,393,-88,40,-88,390,-88,91,-88,123,-88,369,-88,396,-88,59,-88,284,-88,285,-88,263,-88,265,-88,264,-88,124,-88,401,-88,400,-88,94,-88,46,-88,43,-88,45,-88,42,-88,305,-88,47,-88,37,-88,293,-88,294,-88,287,-88,286,-88,289,-88,288,-88,60,-88,291,-88,62,-88,292,-88,290,-88,295,-88,63,-88,283,-88}),
      new State(638, -169),
      new State(639, new int[] {400,862,401,863,319,-457,40,-457}, new int[] {-4,640,-165,896}),
      new State(640, new int[] {319,641,40,-452}, new int[] {-18,143}),
      new State(641, -452, new int[] {-18,642}),
      new State(642, new int[] {40,643}),
      new State(643, new int[] {397,512,368,-254,372,-254,319,-254,391,-254,393,-254,63,-254,311,-254,357,-254,313,-254,398,-254,400,-254,394,-254,320,-254,41,-249}, new int[] {-147,644,-148,874,-97,895,-100,878,-98,522,-145,894,-15,880}),
      new State(644, new int[] {41,645}),
      new State(645, new int[] {58,872,123,-287}, new int[] {-25,646}),
      new State(646, -455, new int[] {-168,647}),
      new State(647, -453, new int[] {-19,648}),
      new State(648, new int[] {123,649}),
      new State(649, -142, new int[] {-114,650}),
      new State(650, new int[] {125,651,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(651, -454, new int[] {-20,652}),
      new State(652, -455, new int[] {-168,653}),
      new State(653, -181),
      new State(654, new int[] {353,510,346,186,343,507,397,512,315,720,314,721,398,722,362,724,366,734,388,747,361,-188}, new int[] {-94,509,-98,371,-95,655,-5,639,-6,187,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(655, -145),
      new State(656, -100),
      new State(657, -101),
      new State(658, new int[] {361,659}),
      new State(659, new int[] {319,660}),
      new State(660, new int[] {364,716,365,-204,123,-204}, new int[] {-30,661}),
      new State(661, -186, new int[] {-169,662}),
      new State(662, new int[] {365,714,123,-208}, new int[] {-35,663}),
      new State(663, -452, new int[] {-18,664}),
      new State(664, -453, new int[] {-19,665}),
      new State(665, new int[] {123,666}),
      new State(666, -305, new int[] {-115,667}),
      new State(667, new int[] {125,668,311,570,357,571,313,572,353,573,315,574,314,575,398,576,356,578,341,671,397,512,350,679,344,-334,346,-334}, new int[] {-93,383,-96,384,-9,385,-11,568,-12,577,-10,579,-109,670,-100,677,-98,522}),
      new State(668, -454, new int[] {-20,669}),
      new State(669, -187),
      new State(670, -309),
      new State(671, new int[] {319,216,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-130,672,-131,217,-132,218}),
      new State(672, new int[] {61,675,59,-202}, new int[] {-110,673}),
      new State(673, new int[] {59,674}),
      new State(674, -201),
      new State(675, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,676,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(676, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-203}),
      new State(677, new int[] {311,570,357,571,313,572,353,573,315,574,314,575,398,576,356,578,341,671,397,512,344,-334,346,-334}, new int[] {-96,678,-98,371,-9,385,-11,568,-12,577,-10,579,-109,670}),
      new State(678, -311),
      new State(679, new int[] {319,204,391,205,393,208}, new int[] {-33,680,-21,695,-133,201}),
      new State(680, new int[] {44,682,59,684,123,685}, new int[] {-90,681}),
      new State(681, -312),
      new State(682, new int[] {319,204,391,205,393,208}, new int[] {-21,683,-133,201}),
      new State(683, -314),
      new State(684, -315),
      new State(685, new int[] {125,686,319,699,391,700,393,208,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-121,687,-75,713,-76,690,-136,691,-21,696,-133,201,-77,701,-135,702,-130,712,-131,217,-132,218}),
      new State(686, -316),
      new State(687, new int[] {125,688,319,699,391,700,393,208,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-75,689,-76,690,-136,691,-21,696,-133,201,-77,701,-135,702,-130,712,-131,217,-132,218}),
      new State(688, -317),
      new State(689, -319),
      new State(690, -320),
      new State(691, new int[] {354,692,338,-328}),
      new State(692, new int[] {319,204,391,205,393,208}, new int[] {-33,693,-21,695,-133,201}),
      new State(693, new int[] {59,694,44,682}),
      new State(694, -322),
      new State(695, -313),
      new State(696, new int[] {390,697}),
      new State(697, new int[] {319,216,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-130,698,-131,217,-132,218}),
      new State(698, -329),
      new State(699, new int[] {393,-88,40,-88,390,-88,91,-88,123,-88,369,-88,396,-88,284,-88,285,-88,263,-88,265,-88,264,-88,124,-88,401,-88,400,-88,94,-88,46,-88,43,-88,45,-88,42,-88,305,-88,47,-88,37,-88,293,-88,294,-88,287,-88,286,-88,289,-88,288,-88,60,-88,291,-88,62,-88,292,-88,290,-88,295,-88,63,-88,283,-88,44,-88,41,-88,58,-84,338,-84}),
      new State(700, new int[] {393,206,58,-59,338,-59}),
      new State(701, -321),
      new State(702, new int[] {338,703}),
      new State(703, new int[] {319,704,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,311,570,357,571,313,572,353,573,315,574,314,575,398,576}, new int[] {-132,706,-12,708}),
      new State(704, new int[] {59,705}),
      new State(705, -323),
      new State(706, new int[] {59,707}),
      new State(707, -324),
      new State(708, new int[] {59,711,319,216,262,219,261,220,260,221,259,222,258,223,263,224,264,225,265,226,295,227,306,228,307,229,326,230,322,231,308,232,309,233,310,234,324,235,329,236,330,237,327,238,328,239,333,240,334,241,331,242,332,243,337,244,338,245,349,246,347,247,351,248,352,249,350,250,354,251,355,252,356,253,360,254,358,255,359,256,340,257,345,258,346,259,344,260,348,261,266,262,267,263,367,264,335,265,336,266,341,267,342,268,339,269,368,270,372,271,364,272,365,273,391,274,362,275,366,276,361,277,373,278,374,279,376,280,378,281,370,282,371,283,375,284,392,285,343,286,395,287,388,288,353,289,315,290,314,291,313,292,357,293,311,294,398,295}, new int[] {-130,709,-131,217,-132,218}),
      new State(709, new int[] {59,710}),
      new State(710, -325),
      new State(711, -326),
      new State(712, -327),
      new State(713, -318),
      new State(714, new int[] {319,204,391,205,393,208}, new int[] {-33,715,-21,695,-133,201}),
      new State(715, new int[] {44,682,123,-209}),
      new State(716, new int[] {319,204,391,205,393,208}, new int[] {-21,717,-133,201}),
      new State(717, -205),
      new State(718, new int[] {315,720,314,721,398,722,361,-188}, new int[] {-14,719,-13,718}),
      new State(719, -189),
      new State(720, -190),
      new State(721, -191),
      new State(722, -192),
      new State(723, -102),
      new State(724, new int[] {319,725}),
      new State(725, -193, new int[] {-170,726}),
      new State(726, -452, new int[] {-18,727}),
      new State(727, -453, new int[] {-19,728}),
      new State(728, new int[] {123,729}),
      new State(729, -305, new int[] {-115,730}),
      new State(730, new int[] {125,731,311,570,357,571,313,572,353,573,315,574,314,575,398,576,356,578,341,671,397,512,350,679,344,-334,346,-334}, new int[] {-93,383,-96,384,-9,385,-11,568,-12,577,-10,579,-109,670,-100,677,-98,522}),
      new State(731, -454, new int[] {-20,732}),
      new State(732, -194),
      new State(733, -103),
      new State(734, new int[] {319,735}),
      new State(735, -195, new int[] {-171,736}),
      new State(736, new int[] {364,744,123,-206}, new int[] {-36,737}),
      new State(737, -452, new int[] {-18,738}),
      new State(738, -453, new int[] {-19,739}),
      new State(739, new int[] {123,740}),
      new State(740, -305, new int[] {-115,741}),
      new State(741, new int[] {125,742,311,570,357,571,313,572,353,573,315,574,314,575,398,576,356,578,341,671,397,512,350,679,344,-334,346,-334}, new int[] {-93,383,-96,384,-9,385,-11,568,-12,577,-10,579,-109,670,-100,677,-98,522}),
      new State(742, -454, new int[] {-20,743}),
      new State(743, -196),
      new State(744, new int[] {319,204,391,205,393,208}, new int[] {-33,745,-21,695,-133,201}),
      new State(745, new int[] {44,682,123,-207}),
      new State(746, -104),
      new State(747, new int[] {319,748}),
      new State(748, new int[] {58,759,364,-199,365,-199,123,-199}, new int[] {-108,749}),
      new State(749, new int[] {364,716,365,-204,123,-204}, new int[] {-30,750}),
      new State(750, -197, new int[] {-172,751}),
      new State(751, new int[] {365,714,123,-208}, new int[] {-35,752}),
      new State(752, -452, new int[] {-18,753}),
      new State(753, -453, new int[] {-19,754}),
      new State(754, new int[] {123,755}),
      new State(755, -305, new int[] {-115,756}),
      new State(756, new int[] {125,757,311,570,357,571,313,572,353,573,315,574,314,575,398,576,356,578,341,671,397,512,350,679,344,-334,346,-334}, new int[] {-93,383,-96,384,-9,385,-11,568,-12,577,-10,579,-109,670,-100,677,-98,522}),
      new State(757, -454, new int[] {-20,758}),
      new State(758, -198),
      new State(759, new int[] {368,555,372,556,319,204,391,205,393,208,353,765,63,768}, new int[] {-26,760,-23,761,-24,764,-21,557,-133,201,-37,770,-39,773}),
      new State(760, -200),
      new State(761, new int[] {124,762,401,766,364,-266,365,-266,123,-266,268,-266,59,-266}),
      new State(762, new int[] {368,555,372,556,319,204,391,205,393,208,353,765}, new int[] {-23,763,-24,764,-21,557,-133,201}),
      new State(763, -279),
      new State(764, -274),
      new State(765, -275),
      new State(766, new int[] {368,555,372,556,319,204,391,205,393,208,353,765}, new int[] {-23,767,-24,764,-21,557,-133,201}),
      new State(767, -283),
      new State(768, new int[] {368,555,372,556,319,204,391,205,393,208,353,765}, new int[] {-23,769,-24,764,-21,557,-133,201}),
      new State(769, -267),
      new State(770, new int[] {124,771,364,-268,365,-268,123,-268,268,-268,59,-268}),
      new State(771, new int[] {368,555,372,556,319,204,391,205,393,208,353,765}, new int[] {-23,772,-24,764,-21,557,-133,201}),
      new State(772, -280),
      new State(773, new int[] {401,774,364,-269,365,-269,123,-269,268,-269,59,-269}),
      new State(774, new int[] {368,555,372,556,319,204,391,205,393,208,353,765}, new int[] {-23,775,-24,764,-21,557,-133,201}),
      new State(775, -284),
      new State(776, new int[] {40,777}),
      new State(777, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325}, new int[] {-120,778,-66,785,-51,784,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(778, new int[] {44,782,41,-125}, new int[] {-3,779}),
      new State(779, new int[] {41,780}),
      new State(780, new int[] {59,781}),
      new State(781, -162),
      new State(782, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325,41,-126}, new int[] {-66,783,-51,784,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(783, -179),
      new State(784, new int[] {44,-180,41,-180,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(785, -178),
      new State(786, new int[] {40,787}),
      new State(787, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,788,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(788, new int[] {338,789,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(789, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,857,323,325,400,862,401,863,367,868}, new int[] {-157,790,-51,856,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329,-165,866}),
      new State(790, new int[] {41,791,268,850}),
      new State(791, -453, new int[] {-19,792}),
      new State(792, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,58,846,322,-453}, new int[] {-82,793,-42,795,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(793, -454, new int[] {-20,794}),
      new State(794, -163),
      new State(795, -216),
      new State(796, new int[] {40,797}),
      new State(797, new int[] {319,841}, new int[] {-112,798,-65,845}),
      new State(798, new int[] {41,799,44,839}),
      new State(799, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,58,835,322,-453}, new int[] {-74,800,-42,801,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(800, -165),
      new State(801, -218),
      new State(802, -166),
      new State(803, new int[] {123,804}),
      new State(804, -142, new int[] {-114,805}),
      new State(805, new int[] {125,806,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(806, -453, new int[] {-19,807}),
      new State(807, -170, new int[] {-128,808}),
      new State(808, new int[] {347,811,351,831,123,-176,330,-176,329,-176,328,-176,335,-176,339,-176,340,-176,348,-176,355,-176,353,-176,324,-176,321,-176,320,-176,36,-176,319,-176,391,-176,393,-176,40,-176,368,-176,91,-176,323,-176,367,-176,307,-176,303,-176,302,-176,43,-176,45,-176,33,-176,126,-176,306,-176,358,-176,359,-176,262,-176,261,-176,260,-176,259,-176,258,-176,301,-176,300,-176,299,-176,298,-176,297,-176,296,-176,304,-176,326,-176,64,-176,317,-176,312,-176,370,-176,371,-176,375,-176,374,-176,378,-176,376,-176,392,-176,373,-176,34,-176,383,-176,96,-176,266,-176,267,-176,269,-176,352,-176,346,-176,343,-176,397,-176,395,-176,360,-176,334,-176,332,-176,59,-176,349,-176,345,-176,315,-176,314,-176,398,-176,362,-176,366,-176,388,-176,363,-176,350,-176,344,-176,322,-176,361,-176,0,-176,125,-176,308,-176,309,-176,341,-176,342,-176,336,-176,337,-176,331,-176,333,-176,327,-176,310,-176}, new int[] {-87,809}),
      new State(809, -454, new int[] {-20,810}),
      new State(810, -167),
      new State(811, new int[] {40,812}),
      new State(812, new int[] {319,204,391,205,393,208}, new int[] {-34,813,-21,830,-133,201}),
      new State(813, new int[] {124,827,320,829,41,-172}, new int[] {-129,814}),
      new State(814, new int[] {41,815}),
      new State(815, new int[] {123,816}),
      new State(816, -142, new int[] {-114,817}),
      new State(817, new int[] {125,818,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(818, -171),
      new State(819, new int[] {319,820}),
      new State(820, new int[] {59,821}),
      new State(821, -168),
      new State(822, -144),
      new State(823, new int[] {40,824}),
      new State(824, new int[] {41,825}),
      new State(825, new int[] {59,826}),
      new State(826, -146),
      new State(827, new int[] {319,204,391,205,393,208}, new int[] {-21,828,-133,201}),
      new State(828, -175),
      new State(829, -173),
      new State(830, -174),
      new State(831, new int[] {123,832}),
      new State(832, -142, new int[] {-114,833}),
      new State(833, new int[] {125,834,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(834, -177),
      new State(835, -142, new int[] {-114,836}),
      new State(836, new int[] {337,837,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(837, new int[] {59,838}),
      new State(838, -219),
      new State(839, new int[] {319,841}, new int[] {-65,840}),
      new State(840, -139),
      new State(841, new int[] {61,842}),
      new State(842, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,843,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(843, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,41,-452,44,-452,59,-452}, new int[] {-18,844}),
      new State(844, -352),
      new State(845, -140),
      new State(846, -142, new int[] {-114,847}),
      new State(847, new int[] {331,848,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(848, new int[] {59,849}),
      new State(849, -217),
      new State(850, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,857,323,325,400,862,401,863,367,868}, new int[] {-157,851,-51,856,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329,-165,866}),
      new State(851, new int[] {41,852}),
      new State(852, -453, new int[] {-19,853}),
      new State(853, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,58,846,322,-453}, new int[] {-82,854,-42,795,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(854, -454, new int[] {-20,855}),
      new State(855, -164),
      new State(856, new int[] {41,-210,268,-210,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(857, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,93,-546}, new int[] {-154,858,-153,307,-151,346,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(858, new int[] {93,859}),
      new State(859, new int[] {91,-482,123,-482,369,-482,396,-482,390,-482,40,-482,41,-213,268,-213}),
      new State(860, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325}, new int[] {-51,861,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(861, new int[] {44,-553,41,-553,93,-553,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(862, -82),
      new State(863, -83),
      new State(864, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,865,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(865, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-554,41,-554,93,-554}),
      new State(866, new int[] {320,99,36,100,353,316,319,204,391,205,393,208,40,317,368,303,91,343,323,325}, new int[] {-51,867,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,342,-59,353,-58,354,-61,328,-89,329}),
      new State(867, new int[] {41,-211,268,-211,91,-511,123,-511,369,-511,396,-511,390,-511}),
      new State(868, new int[] {40,869}),
      new State(869, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,41,-546}, new int[] {-154,870,-153,307,-151,346,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(870, new int[] {41,871}),
      new State(871, -212),
      new State(872, new int[] {368,555,372,556,319,204,391,205,393,208,353,765,63,768}, new int[] {-26,873,-23,761,-24,764,-21,557,-133,201,-37,770,-39,773}),
      new State(873, -288),
      new State(874, new int[] {44,876,41,-125}, new int[] {-3,875}),
      new State(875, -248),
      new State(876, new int[] {397,512,41,-126,368,-254,372,-254,319,-254,391,-254,393,-254,63,-254,311,-254,357,-254,313,-254,398,-254,400,-254,394,-254,320,-254}, new int[] {-97,877,-100,878,-98,522,-145,894,-15,880}),
      new State(877, -251),
      new State(878, new int[] {397,512,368,-254,372,-254,319,-254,391,-254,393,-254,63,-254,311,-254,357,-254,313,-254,398,-254,400,-254,394,-254,320,-254}, new int[] {-145,879,-98,371,-15,880}),
      new State(879, -252),
      new State(880, new int[] {368,555,372,556,319,204,391,205,393,208,63,560,311,890,357,891,313,892,398,893,400,-264,394,-264,320,-264}, new int[] {-29,881,-16,889,-27,551,-24,552,-21,557,-133,201,-38,562,-40,565}),
      new State(881, new int[] {400,888,394,-182,320,-182}, new int[] {-7,882}),
      new State(882, new int[] {394,887,320,-184}, new int[] {-8,883}),
      new State(883, new int[] {320,884}),
      new State(884, new int[] {61,885,44,-260,41,-260}),
      new State(885, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,886,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(886, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-261,41,-261}),
      new State(887, -185),
      new State(888, -183),
      new State(889, -255),
      new State(890, -256),
      new State(891, -257),
      new State(892, -258),
      new State(893, -259),
      new State(894, -253),
      new State(895, -250),
      new State(896, -458),
      new State(897, new int[] {91,898,123,901,390,911,369,467,396,468,59,-472,284,-472,285,-472,263,-472,265,-472,264,-472,124,-472,401,-472,400,-472,94,-472,46,-472,43,-472,45,-472,42,-472,305,-472,47,-472,37,-472,293,-472,294,-472,287,-472,286,-472,289,-472,288,-472,60,-472,291,-472,62,-472,292,-472,290,-472,295,-472,63,-472,283,-472,41,-472,125,-472,58,-472,93,-472,44,-472,268,-472,338,-472,40,-472}, new int[] {-22,904}),
      new State(898, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,93,-506}, new int[] {-70,899,-50,122,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(899, new int[] {93,900}),
      new State(900, -534),
      new State(901, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,902,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(902, new int[] {125,903,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(903, -535),
      new State(904, new int[] {319,906,123,907,320,99,36,100}, new int[] {-1,905,-56,910}),
      new State(905, -536),
      new State(906, -542),
      new State(907, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,908,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(908, new int[] {125,909,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(909, -543),
      new State(910, -544),
      new State(911, new int[] {320,99,36,100}, new int[] {-56,912}),
      new State(912, -538),
      new State(913, -533),
      new State(914, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,915,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(915, new int[] {41,916,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(916, -473),
      new State(917, new int[] {40,918}),
      new State(918, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,347,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,400,862,401,863,394,864,44,-546,41,-546}, new int[] {-154,919,-153,307,-151,346,-152,310,-50,311,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523,-165,860}),
      new State(919, new int[] {41,920}),
      new State(920, new int[] {61,336,44,-555,41,-555,93,-555}),
      new State(921, -227),
      new State(922, -228),
      new State(923, new int[] {58,921,59,922}, new int[] {-173,924}),
      new State(924, -142, new int[] {-114,925}),
      new State(925, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,125,-226,341,-226,342,-226,336,-226,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(926, -224, new int[] {-125,927}),
      new State(927, new int[] {125,928,341,178,342,923}),
      new State(928, -221),
      new State(929, new int[] {59,933,336,-224,341,-224,342,-224}, new int[] {-125,930}),
      new State(930, new int[] {336,931,341,178,342,923}),
      new State(931, new int[] {59,932}),
      new State(932, -222),
      new State(933, -224, new int[] {-125,934}),
      new State(934, new int[] {336,935,341,178,342,923}),
      new State(935, new int[] {59,936}),
      new State(936, -223),
      new State(937, -142, new int[] {-114,938}),
      new State(938, new int[] {333,939,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(939, new int[] {59,940}),
      new State(940, -215),
      new State(941, new int[] {44,942,59,-357,41,-357}),
      new State(942, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,943,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(943, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-358,59,-358,41,-358}),
      new State(944, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-359,59,-359,41,-359}),
      new State(945, new int[] {40,946}),
      new State(946, new int[] {320,951,400,862,401,863}, new int[] {-150,947,-146,954,-165,952}),
      new State(947, new int[] {41,948,44,949}),
      new State(948, -460),
      new State(949, new int[] {320,951,400,862,401,863}, new int[] {-146,950,-165,952}),
      new State(950, -461),
      new State(951, -463),
      new State(952, new int[] {320,953}),
      new State(953, -464),
      new State(954, -462),
      new State(955, new int[] {40,304,58,-55}),
      new State(956, new int[] {40,333,58,-49}),
      new State(957, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,58,-14}, new int[] {-50,339,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(958, new int[] {353,316,319,204,391,205,393,208,320,99,36,100,40,914,361,372,397,512,58,-13}, new int[] {-31,365,-158,368,-100,369,-32,96,-21,520,-133,201,-88,897,-56,913,-98,522}),
      new State(959, new int[] {40,398,58,-40}),
      new State(960, new int[] {40,406,58,-41}),
      new State(961, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,58,-4}, new int[] {-50,410,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(962, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,58,-5}, new int[] {-50,412,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(963, new int[] {40,414,58,-6}),
      new State(964, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,58,-7}, new int[] {-50,418,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(965, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,58,-8}, new int[] {-50,420,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(966, new int[] {40,437,58,-15,284,-474,285,-474,263,-474,265,-474,264,-474,124,-474,401,-474,400,-474,94,-474,46,-474,43,-474,45,-474,42,-474,305,-474,47,-474,37,-474,293,-474,294,-474,287,-474,286,-474,289,-474,288,-474,60,-474,291,-474,62,-474,292,-474,290,-474,295,-474,63,-474,283,-474,44,-474,41,-474}, new int[] {-86,436}),
      new State(967, new int[] {284,-486,285,-486,263,-486,265,-486,264,-486,124,-486,401,-486,400,-486,94,-486,46,-486,43,-486,45,-486,42,-486,305,-486,47,-486,37,-486,293,-486,294,-486,287,-486,286,-486,289,-486,288,-486,60,-486,291,-486,62,-486,292,-486,290,-486,295,-486,63,-486,283,-486,44,-486,41,-486,58,-67}),
      new State(968, new int[] {284,-487,285,-487,263,-487,265,-487,264,-487,124,-487,401,-487,400,-487,94,-487,46,-487,43,-487,45,-487,42,-487,305,-487,47,-487,37,-487,293,-487,294,-487,287,-487,286,-487,289,-487,288,-487,60,-487,291,-487,62,-487,292,-487,290,-487,295,-487,63,-487,283,-487,44,-487,41,-487,58,-68}),
      new State(969, new int[] {284,-488,285,-488,263,-488,265,-488,264,-488,124,-488,401,-488,400,-488,94,-488,46,-488,43,-488,45,-488,42,-488,305,-488,47,-488,37,-488,293,-488,294,-488,287,-488,286,-488,289,-488,288,-488,60,-488,291,-488,62,-488,292,-488,290,-488,295,-488,63,-488,283,-488,44,-488,41,-488,58,-69}),
      new State(970, new int[] {284,-489,285,-489,263,-489,265,-489,264,-489,124,-489,401,-489,400,-489,94,-489,46,-489,43,-489,45,-489,42,-489,305,-489,47,-489,37,-489,293,-489,294,-489,287,-489,286,-489,289,-489,288,-489,60,-489,291,-489,62,-489,292,-489,290,-489,295,-489,63,-489,283,-489,44,-489,41,-489,58,-64}),
      new State(971, new int[] {284,-490,285,-490,263,-490,265,-490,264,-490,124,-490,401,-490,400,-490,94,-490,46,-490,43,-490,45,-490,42,-490,305,-490,47,-490,37,-490,293,-490,294,-490,287,-490,286,-490,289,-490,288,-490,60,-490,291,-490,62,-490,292,-490,290,-490,295,-490,63,-490,283,-490,44,-490,41,-490,58,-66}),
      new State(972, new int[] {284,-491,285,-491,263,-491,265,-491,264,-491,124,-491,401,-491,400,-491,94,-491,46,-491,43,-491,45,-491,42,-491,305,-491,47,-491,37,-491,293,-491,294,-491,287,-491,286,-491,289,-491,288,-491,60,-491,291,-491,62,-491,292,-491,290,-491,295,-491,63,-491,283,-491,44,-491,41,-491,58,-65}),
      new State(973, new int[] {284,-492,285,-492,263,-492,265,-492,264,-492,124,-492,401,-492,400,-492,94,-492,46,-492,43,-492,45,-492,42,-492,305,-492,47,-492,37,-492,293,-492,294,-492,287,-492,286,-492,289,-492,288,-492,60,-492,291,-492,62,-492,292,-492,290,-492,295,-492,63,-492,283,-492,44,-492,41,-492,58,-70}),
      new State(974, new int[] {284,-493,285,-493,263,-493,265,-493,264,-493,124,-493,401,-493,400,-493,94,-493,46,-493,43,-493,45,-493,42,-493,305,-493,47,-493,37,-493,293,-493,294,-493,287,-493,286,-493,289,-493,288,-493,60,-493,291,-493,62,-493,292,-493,290,-493,295,-493,63,-493,283,-493,44,-493,41,-493,58,-63}),
      new State(975, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,58,-47}, new int[] {-50,497,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(976, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,284,-438,285,-438,263,-438,265,-438,264,-438,124,-438,401,-438,400,-438,94,-438,46,-438,42,-438,305,-438,47,-438,37,-438,293,-438,294,-438,287,-438,286,-438,289,-438,288,-438,60,-438,291,-438,62,-438,292,-438,290,-438,295,-438,63,-438,283,-438,44,-438,41,-438,58,-48}, new int[] {-50,499,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(977, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,58,-34}, new int[] {-50,505,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(978, new int[] {400,-451,401,-451,40,-451,58,-44}),
      new State(979, new int[] {400,-450,401,-450,40,-450,58,-71}),
      new State(980, new int[] {40,525,58,-72}),
      new State(981, new int[] {58,982}),
      new State(982, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,983,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(983, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-295,41,-295}),
      new State(984, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,985,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(985, new int[] {284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,44,-296,41,-296}),
      new State(986, new int[] {41,987,320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,985,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(987, -291),
      new State(988, -292),
      new State(989, new int[] {319,906,123,907,320,99,36,100}, new int[] {-1,990,-56,910}),
      new State(990, new int[] {40,131,61,-527,270,-527,271,-527,279,-527,281,-527,278,-527,277,-527,276,-527,275,-527,274,-527,273,-527,272,-527,280,-527,282,-527,303,-527,302,-527,59,-527,284,-527,285,-527,263,-527,265,-527,264,-527,124,-527,401,-527,400,-527,94,-527,46,-527,43,-527,45,-527,42,-527,305,-527,47,-527,37,-527,293,-527,294,-527,287,-527,286,-527,289,-527,288,-527,60,-527,291,-527,62,-527,292,-527,290,-527,295,-527,63,-527,283,-527,91,-527,123,-527,369,-527,396,-527,390,-527,41,-527,125,-527,58,-527,93,-527,44,-527,268,-527,338,-527}, new int[] {-143,991}),
      new State(991, -523),
      new State(992, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,993,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(993, new int[] {125,994,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(994, -522),
      new State(995, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,996,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(996, new int[] {284,40,285,42,263,-371,265,-371,264,-371,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-371,41,-371,125,-371,58,-371,93,-371,44,-371,268,-371,338,-371}),
      new State(997, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,998,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(998, new int[] {284,40,285,42,263,-372,265,-372,264,-372,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-372,41,-372,125,-372,58,-372,93,-372,44,-372,268,-372,338,-372}),
      new State(999, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1000,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1000, new int[] {284,40,285,42,263,-373,265,-373,264,-373,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-373,41,-373,125,-373,58,-373,93,-373,44,-373,268,-373,338,-373}),
      new State(1001, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1002,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1002, new int[] {284,40,285,42,263,-374,265,-374,264,-374,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-374,41,-374,125,-374,58,-374,93,-374,44,-374,268,-374,338,-374}),
      new State(1003, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1004,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1004, new int[] {284,40,285,42,263,-375,265,-375,264,-375,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-375,41,-375,125,-375,58,-375,93,-375,44,-375,268,-375,338,-375}),
      new State(1005, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1006,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1006, new int[] {284,40,285,42,263,-376,265,-376,264,-376,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-376,41,-376,125,-376,58,-376,93,-376,44,-376,268,-376,338,-376}),
      new State(1007, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1008,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1008, new int[] {284,40,285,42,263,-377,265,-377,264,-377,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-377,41,-377,125,-377,58,-377,93,-377,44,-377,268,-377,338,-377}),
      new State(1009, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1010,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1010, new int[] {284,40,285,42,263,-378,265,-378,264,-378,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-378,41,-378,125,-378,58,-378,93,-378,44,-378,268,-378,338,-378}),
      new State(1011, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1012,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1012, new int[] {284,40,285,42,263,-379,265,-379,264,-379,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-379,41,-379,125,-379,58,-379,93,-379,44,-379,268,-379,338,-379}),
      new State(1013, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1014,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1014, new int[] {284,40,285,42,263,-380,265,-380,264,-380,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-380,41,-380,125,-380,58,-380,93,-380,44,-380,268,-380,338,-380}),
      new State(1015, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1016,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1016, new int[] {284,40,285,42,263,-381,265,-381,264,-381,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-381,41,-381,125,-381,58,-381,93,-381,44,-381,268,-381,338,-381}),
      new State(1017, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1018,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1018, new int[] {284,40,285,42,263,-382,265,-382,264,-382,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-382,41,-382,125,-382,58,-382,93,-382,44,-382,268,-382,338,-382}),
      new State(1019, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1020,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1020, new int[] {284,40,285,42,263,-383,265,-383,264,-383,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108,59,-383,41,-383,125,-383,58,-383,93,-383,44,-383,268,-383,338,-383}),
      new State(1021, -384),
      new State(1022, -386),
      new State(1023, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1024,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1024, new int[] {284,40,285,42,263,-423,265,-423,264,-423,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,-423,283,108,59,-423,41,-423,125,-423,58,-423,93,-423,44,-423,268,-423,338,-423}),
      new State(1025, -530),
      new State(1026, -142, new int[] {-114,1027}),
      new State(1027, new int[] {327,1028,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(1028, new int[] {59,1029}),
      new State(1029, -239),
      new State(1030, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,322,-453}, new int[] {-42,1031,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1031, -243),
      new State(1032, new int[] {40,1033}),
      new State(1033, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1034,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1034, new int[] {41,1035,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(1035, new int[] {58,1037,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,322,-453}, new int[] {-42,1036,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1036, -240),
      new State(1037, -142, new int[] {-114,1038}),
      new State(1038, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,310,-244,308,-244,309,-244,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(1039, new int[] {310,1040,308,1042,309,1048}),
      new State(1040, new int[] {59,1041}),
      new State(1041, -246),
      new State(1042, new int[] {40,1043}),
      new State(1043, new int[] {320,99,36,100,353,185,319,204,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524}, new int[] {-50,1044,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,141,-6,187,-100,508,-98,522,-102,523}),
      new State(1044, new int[] {41,1045,284,40,285,42,263,44,265,46,264,48,124,50,401,52,400,54,94,56,46,58,43,60,45,62,42,64,305,66,47,68,37,70,293,72,294,74,287,76,286,78,289,80,288,82,60,84,291,86,62,88,292,90,290,92,295,94,63,104,283,108}),
      new State(1045, new int[] {58,1046}),
      new State(1046, -142, new int[] {-114,1047}),
      new State(1047, new int[] {123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,310,-245,308,-245,309,-245,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(1048, new int[] {58,1049}),
      new State(1049, -142, new int[] {-114,1050}),
      new State(1050, new int[] {310,1051,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,205,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,823,322,-453,361,-188}, new int[] {-92,10,-42,11,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,654,-98,522,-102,523,-95,822,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(1051, new int[] {59,1052}),
      new State(1052, -247),
      new State(1053, new int[] {393,206,319,204,123,-452}, new int[] {-133,1054,-18,1127}),
      new State(1054, new int[] {59,1055,393,202,123,-452}, new int[] {-18,1056}),
      new State(1055, -109),
      new State(1056, -110, new int[] {-166,1057}),
      new State(1057, new int[] {123,1058}),
      new State(1058, -87, new int[] {-111,1059}),
      new State(1059, new int[] {125,1060,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,1053,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,1064,350,1068,344,1124,322,-453,361,-188}, new int[] {-41,5,-42,6,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,1061,-98,522,-102,523,-95,1063,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(1060, -111),
      new State(1061, new int[] {353,510,346,186,343,507,397,512,315,720,314,721,398,722,362,724,366,734,388,747,361,-188}, new int[] {-94,509,-98,371,-95,1062,-5,639,-6,187,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(1062, -107),
      new State(1063, -106),
      new State(1064, new int[] {40,1065}),
      new State(1065, new int[] {41,1066}),
      new State(1066, new int[] {59,1067}),
      new State(1067, -108),
      new State(1068, new int[] {319,204,393,1117,346,1114,344,1115}, new int[] {-161,1069,-17,1071,-159,1101,-133,1103,-137,1100,-134,1078}),
      new State(1069, new int[] {59,1070}),
      new State(1070, -114),
      new State(1071, new int[] {319,204,393,1093}, new int[] {-160,1072,-159,1074,-133,1084,-137,1100,-134,1078}),
      new State(1072, new int[] {59,1073}),
      new State(1073, -115),
      new State(1074, new int[] {59,1075,44,1076}),
      new State(1075, -117),
      new State(1076, new int[] {319,204,393,1082}, new int[] {-137,1077,-134,1078,-133,1079}),
      new State(1077, -131),
      new State(1078, -137),
      new State(1079, new int[] {393,202,338,1080,59,-135,44,-135,125,-135}),
      new State(1080, new int[] {319,1081}),
      new State(1081, -136),
      new State(1082, new int[] {319,204}, new int[] {-134,1083,-133,1079}),
      new State(1083, -138),
      new State(1084, new int[] {393,1085,338,1080,59,-135,44,-135}),
      new State(1085, new int[] {123,1086,319,203}),
      new State(1086, new int[] {319,204}, new int[] {-138,1087,-134,1092,-133,1079}),
      new State(1087, new int[] {44,1090,125,-125}, new int[] {-3,1088}),
      new State(1088, new int[] {125,1089}),
      new State(1089, -121),
      new State(1090, new int[] {319,204,125,-126}, new int[] {-134,1091,-133,1079}),
      new State(1091, -129),
      new State(1092, -130),
      new State(1093, new int[] {319,204}, new int[] {-133,1094,-134,1083}),
      new State(1094, new int[] {393,1095,338,1080,59,-135,44,-135}),
      new State(1095, new int[] {123,1096,319,203}),
      new State(1096, new int[] {319,204}, new int[] {-138,1097,-134,1092,-133,1079}),
      new State(1097, new int[] {44,1090,125,-125}, new int[] {-3,1098}),
      new State(1098, new int[] {125,1099}),
      new State(1099, -122),
      new State(1100, -132),
      new State(1101, new int[] {59,1102,44,1076}),
      new State(1102, -116),
      new State(1103, new int[] {393,1104,338,1080,59,-135,44,-135}),
      new State(1104, new int[] {123,1105,319,203}),
      new State(1105, new int[] {319,204,346,1114,344,1115}, new int[] {-140,1106,-139,1116,-134,1111,-133,1079,-17,1112}),
      new State(1106, new int[] {44,1109,125,-125}, new int[] {-3,1107}),
      new State(1107, new int[] {125,1108}),
      new State(1108, -123),
      new State(1109, new int[] {319,204,346,1114,344,1115,125,-126}, new int[] {-139,1110,-134,1111,-133,1079,-17,1112}),
      new State(1110, -127),
      new State(1111, -133),
      new State(1112, new int[] {319,204}, new int[] {-134,1113,-133,1079}),
      new State(1113, -134),
      new State(1114, -119),
      new State(1115, -120),
      new State(1116, -128),
      new State(1117, new int[] {319,204}, new int[] {-133,1118,-134,1083}),
      new State(1118, new int[] {393,1119,338,1080,59,-135,44,-135}),
      new State(1119, new int[] {123,1120,319,203}),
      new State(1120, new int[] {319,204,346,1114,344,1115}, new int[] {-140,1121,-139,1116,-134,1111,-133,1079,-17,1112}),
      new State(1121, new int[] {44,1109,125,-125}, new int[] {-3,1122}),
      new State(1122, new int[] {125,1123}),
      new State(1123, -124),
      new State(1124, new int[] {319,841}, new int[] {-112,1125,-65,845}),
      new State(1125, new int[] {59,1126,44,839}),
      new State(1126, -118),
      new State(1127, -112, new int[] {-167,1128}),
      new State(1128, new int[] {123,1129}),
      new State(1129, -87, new int[] {-111,1130}),
      new State(1130, new int[] {125,1131,123,7,330,23,329,31,328,156,335,168,339,182,340,605,348,608,355,611,353,618,324,627,321,634,320,99,36,100,319,637,391,1053,393,208,40,299,368,303,91,320,323,325,367,332,307,338,303,340,302,351,43,355,45,357,33,359,126,361,306,364,358,397,359,405,262,409,261,411,260,413,259,417,258,419,301,421,300,423,299,425,298,427,297,429,296,431,304,433,326,435,64,440,317,443,312,444,370,445,371,446,375,447,374,448,378,449,376,450,392,451,373,452,34,453,383,478,96,490,266,496,267,498,269,502,352,504,346,186,343,507,397,512,395,524,360,776,334,786,332,796,59,802,349,803,345,819,315,720,314,721,398,722,362,724,366,734,388,747,363,1064,350,1068,344,1124,322,-453,361,-188}, new int[] {-41,5,-42,6,-19,12,-50,635,-51,110,-55,116,-56,117,-80,118,-79,123,-62,124,-32,125,-21,199,-133,201,-91,210,-60,302,-59,326,-58,327,-61,328,-89,329,-52,331,-53,363,-54,396,-57,442,-84,489,-94,506,-5,639,-6,187,-100,1061,-98,522,-102,523,-95,1063,-43,656,-44,657,-14,658,-13,718,-45,723,-47,733,-107,746}),
      new State(1131, -113),
    };
    #endregion

    #region rules
    rules = new Rule[]
    {
    default(Rule),
    new Rule(-163, new int[]{-162,0}),
    new Rule(-164, new int[]{}),
    new Rule(-162, new int[]{-164,-111}),
    new Rule(-132, new int[]{262}),
    new Rule(-132, new int[]{261}),
    new Rule(-132, new int[]{260}),
    new Rule(-132, new int[]{259}),
    new Rule(-132, new int[]{258}),
    new Rule(-132, new int[]{263}),
    new Rule(-132, new int[]{264}),
    new Rule(-132, new int[]{265}),
    new Rule(-132, new int[]{295}),
    new Rule(-132, new int[]{306}),
    new Rule(-132, new int[]{307}),
    new Rule(-132, new int[]{326}),
    new Rule(-132, new int[]{322}),
    new Rule(-132, new int[]{308}),
    new Rule(-132, new int[]{309}),
    new Rule(-132, new int[]{310}),
    new Rule(-132, new int[]{324}),
    new Rule(-132, new int[]{329}),
    new Rule(-132, new int[]{330}),
    new Rule(-132, new int[]{327}),
    new Rule(-132, new int[]{328}),
    new Rule(-132, new int[]{333}),
    new Rule(-132, new int[]{334}),
    new Rule(-132, new int[]{331}),
    new Rule(-132, new int[]{332}),
    new Rule(-132, new int[]{337}),
    new Rule(-132, new int[]{338}),
    new Rule(-132, new int[]{349}),
    new Rule(-132, new int[]{347}),
    new Rule(-132, new int[]{351}),
    new Rule(-132, new int[]{352}),
    new Rule(-132, new int[]{350}),
    new Rule(-132, new int[]{354}),
    new Rule(-132, new int[]{355}),
    new Rule(-132, new int[]{356}),
    new Rule(-132, new int[]{360}),
    new Rule(-132, new int[]{358}),
    new Rule(-132, new int[]{359}),
    new Rule(-132, new int[]{340}),
    new Rule(-132, new int[]{345}),
    new Rule(-132, new int[]{346}),
    new Rule(-132, new int[]{344}),
    new Rule(-132, new int[]{348}),
    new Rule(-132, new int[]{266}),
    new Rule(-132, new int[]{267}),
    new Rule(-132, new int[]{367}),
    new Rule(-132, new int[]{335}),
    new Rule(-132, new int[]{336}),
    new Rule(-132, new int[]{341}),
    new Rule(-132, new int[]{342}),
    new Rule(-132, new int[]{339}),
    new Rule(-132, new int[]{368}),
    new Rule(-132, new int[]{372}),
    new Rule(-132, new int[]{364}),
    new Rule(-132, new int[]{365}),
    new Rule(-132, new int[]{391}),
    new Rule(-132, new int[]{362}),
    new Rule(-132, new int[]{366}),
    new Rule(-132, new int[]{361}),
    new Rule(-132, new int[]{373}),
    new Rule(-132, new int[]{374}),
    new Rule(-132, new int[]{376}),
    new Rule(-132, new int[]{378}),
    new Rule(-132, new int[]{370}),
    new Rule(-132, new int[]{371}),
    new Rule(-132, new int[]{375}),
    new Rule(-132, new int[]{392}),
    new Rule(-132, new int[]{343}),
    new Rule(-132, new int[]{395}),
    new Rule(-132, new int[]{388}),
    new Rule(-131, new int[]{-132}),
    new Rule(-131, new int[]{353}),
    new Rule(-131, new int[]{315}),
    new Rule(-131, new int[]{314}),
    new Rule(-131, new int[]{313}),
    new Rule(-131, new int[]{357}),
    new Rule(-131, new int[]{311}),
    new Rule(-131, new int[]{398}),
    new Rule(-165, new int[]{400}),
    new Rule(-165, new int[]{401}),
    new Rule(-130, new int[]{319}),
    new Rule(-130, new int[]{-131}),
    new Rule(-111, new int[]{-111,-41}),
    new Rule(-111, new int[]{}),
    new Rule(-133, new int[]{319}),
    new Rule(-133, new int[]{-133,393,319}),
    new Rule(-21, new int[]{-133}),
    new Rule(-21, new int[]{391,393,-133}),
    new Rule(-21, new int[]{393,-133}),
    new Rule(-99, new int[]{-32}),
    new Rule(-99, new int[]{-32,-143}),
    new Rule(-101, new int[]{-99}),
    new Rule(-101, new int[]{-101,44,-99}),
    new Rule(-98, new int[]{397,-101,-3,93}),
    new Rule(-100, new int[]{-98}),
    new Rule(-100, new int[]{-100,-98}),
    new Rule(-95, new int[]{-43}),
    new Rule(-95, new int[]{-44}),
    new Rule(-95, new int[]{-45}),
    new Rule(-95, new int[]{-47}),
    new Rule(-95, new int[]{-107}),
    new Rule(-41, new int[]{-42}),
    new Rule(-41, new int[]{-95}),
    new Rule(-41, new int[]{-100,-95}),
    new Rule(-41, new int[]{363,40,41,59}),
    new Rule(-41, new int[]{391,-133,59}),
    new Rule(-166, new int[]{}),
    new Rule(-41, new int[]{391,-133,-18,-166,123,-111,125}),
    new Rule(-167, new int[]{}),
    new Rule(-41, new int[]{391,-18,-167,123,-111,125}),
    new Rule(-41, new int[]{350,-161,59}),
    new Rule(-41, new int[]{350,-17,-160,59}),
    new Rule(-41, new int[]{350,-159,59}),
    new Rule(-41, new int[]{350,-17,-159,59}),
    new Rule(-41, new int[]{344,-112,59}),
    new Rule(-17, new int[]{346}),
    new Rule(-17, new int[]{344}),
    new Rule(-160, new int[]{-133,393,123,-138,-3,125}),
    new Rule(-160, new int[]{393,-133,393,123,-138,-3,125}),
    new Rule(-161, new int[]{-133,393,123,-140,-3,125}),
    new Rule(-161, new int[]{393,-133,393,123,-140,-3,125}),
    new Rule(-3, new int[]{}),
    new Rule(-3, new int[]{44}),
    new Rule(-140, new int[]{-140,44,-139}),
    new Rule(-140, new int[]{-139}),
    new Rule(-138, new int[]{-138,44,-134}),
    new Rule(-138, new int[]{-134}),
    new Rule(-159, new int[]{-159,44,-137}),
    new Rule(-159, new int[]{-137}),
    new Rule(-139, new int[]{-134}),
    new Rule(-139, new int[]{-17,-134}),
    new Rule(-134, new int[]{-133}),
    new Rule(-134, new int[]{-133,338,319}),
    new Rule(-137, new int[]{-134}),
    new Rule(-137, new int[]{393,-134}),
    new Rule(-112, new int[]{-112,44,-65}),
    new Rule(-112, new int[]{-65}),
    new Rule(-114, new int[]{-114,-92}),
    new Rule(-114, new int[]{}),
    new Rule(-92, new int[]{-42}),
    new Rule(-92, new int[]{-95}),
    new Rule(-92, new int[]{-100,-95}),
    new Rule(-92, new int[]{363,40,41,59}),
    new Rule(-42, new int[]{123,-114,125}),
    new Rule(-42, new int[]{-19,-63,-20}),
    new Rule(-42, new int[]{-19,-64,-20}),
    new Rule(-42, new int[]{330,40,-50,41,-19,-83,-20}),
    new Rule(-42, new int[]{329,-19,-42,330,40,-50,41,59,-20}),
    new Rule(-42, new int[]{328,40,-116,59,-116,59,-116,41,-19,-81,-20}),
    new Rule(-42, new int[]{335,40,-50,41,-19,-126,-20}),
    new Rule(-42, new int[]{339,-70,59}),
    new Rule(-42, new int[]{340,-70,59}),
    new Rule(-42, new int[]{348,-70,59}),
    new Rule(-42, new int[]{355,-117,59}),
    new Rule(-42, new int[]{353,-118,59}),
    new Rule(-42, new int[]{324,-119,59}),
    new Rule(-42, new int[]{321}),
    new Rule(-42, new int[]{-50,59}),
    new Rule(-42, new int[]{360,40,-120,-3,41,59}),
    new Rule(-42, new int[]{334,40,-50,338,-157,41,-19,-82,-20}),
    new Rule(-42, new int[]{334,40,-50,338,-157,268,-157,41,-19,-82,-20}),
    new Rule(-42, new int[]{332,40,-112,41,-74}),
    new Rule(-42, new int[]{59}),
    new Rule(-42, new int[]{349,123,-114,125,-19,-128,-87,-20}),
    new Rule(-42, new int[]{345,319,59}),
    new Rule(-42, new int[]{319,58}),
    new Rule(-128, new int[]{}),
    new Rule(-128, new int[]{-128,347,40,-34,-129,41,123,-114,125}),
    new Rule(-129, new int[]{}),
    new Rule(-129, new int[]{320}),
    new Rule(-34, new int[]{-21}),
    new Rule(-34, new int[]{-34,124,-21}),
    new Rule(-87, new int[]{}),
    new Rule(-87, new int[]{351,123,-114,125}),
    new Rule(-120, new int[]{-66}),
    new Rule(-120, new int[]{-120,44,-66}),
    new Rule(-66, new int[]{-51}),
    new Rule(-43, new int[]{-5,-4,319,-18,40,-147,41,-25,-168,-19,123,-114,125,-20,-168}),
    new Rule(-7, new int[]{}),
    new Rule(-7, new int[]{400}),
    new Rule(-8, new int[]{}),
    new Rule(-8, new int[]{394}),
    new Rule(-169, new int[]{}),
    new Rule(-44, new int[]{-14,361,319,-30,-169,-35,-18,-19,123,-115,125,-20}),
    new Rule(-14, new int[]{}),
    new Rule(-14, new int[]{-13,-14}),
    new Rule(-13, new int[]{315}),
    new Rule(-13, new int[]{314}),
    new Rule(-13, new int[]{398}),
    new Rule(-170, new int[]{}),
    new Rule(-45, new int[]{362,319,-170,-18,-19,123,-115,125,-20}),
    new Rule(-171, new int[]{}),
    new Rule(-47, new int[]{366,319,-171,-36,-18,-19,123,-115,125,-20}),
    new Rule(-172, new int[]{}),
    new Rule(-107, new int[]{388,319,-108,-30,-172,-35,-18,-19,123,-115,125,-20}),
    new Rule(-108, new int[]{}),
    new Rule(-108, new int[]{58,-26}),
    new Rule(-109, new int[]{341,-130,-110,59}),
    new Rule(-110, new int[]{}),
    new Rule(-110, new int[]{61,-50}),
    new Rule(-30, new int[]{}),
    new Rule(-30, new int[]{364,-21}),
    new Rule(-36, new int[]{}),
    new Rule(-36, new int[]{364,-33}),
    new Rule(-35, new int[]{}),
    new Rule(-35, new int[]{365,-33}),
    new Rule(-157, new int[]{-51}),
    new Rule(-157, new int[]{-165,-51}),
    new Rule(-157, new int[]{367,40,-154,41}),
    new Rule(-157, new int[]{91,-154,93}),
    new Rule(-81, new int[]{-42}),
    new Rule(-81, new int[]{58,-114,333,59}),
    new Rule(-82, new int[]{-42}),
    new Rule(-82, new int[]{58,-114,331,59}),
    new Rule(-74, new int[]{-42}),
    new Rule(-74, new int[]{58,-114,337,59}),
    new Rule(-126, new int[]{123,-125,125}),
    new Rule(-126, new int[]{123,59,-125,125}),
    new Rule(-126, new int[]{58,-125,336,59}),
    new Rule(-126, new int[]{58,59,-125,336,59}),
    new Rule(-125, new int[]{}),
    new Rule(-125, new int[]{-125,341,-50,-173,-114}),
    new Rule(-125, new int[]{-125,342,-173,-114}),
    new Rule(-173, new int[]{58}),
    new Rule(-173, new int[]{59}),
    new Rule(-102, new int[]{395,40,-50,41,123,-104,125}),
    new Rule(-104, new int[]{}),
    new Rule(-104, new int[]{-106,-3}),
    new Rule(-106, new int[]{-103}),
    new Rule(-106, new int[]{-106,44,-103}),
    new Rule(-103, new int[]{-105,-3,268,-50}),
    new Rule(-103, new int[]{342,-3,268,-50}),
    new Rule(-105, new int[]{-50}),
    new Rule(-105, new int[]{-105,44,-50}),
    new Rule(-83, new int[]{-42}),
    new Rule(-83, new int[]{58,-114,327,59}),
    new Rule(-155, new int[]{322,40,-50,41,-42}),
    new Rule(-155, new int[]{-155,308,40,-50,41,-42}),
    new Rule(-63, new int[]{-155}),
    new Rule(-63, new int[]{-155,309,-42}),
    new Rule(-156, new int[]{322,40,-50,41,58,-114}),
    new Rule(-156, new int[]{-156,308,40,-50,41,58,-114}),
    new Rule(-64, new int[]{-156,310,59}),
    new Rule(-64, new int[]{-156,309,58,-114,310,59}),
    new Rule(-147, new int[]{-148,-3}),
    new Rule(-147, new int[]{}),
    new Rule(-148, new int[]{-97}),
    new Rule(-148, new int[]{-148,44,-97}),
    new Rule(-97, new int[]{-100,-145}),
    new Rule(-97, new int[]{-145}),
    new Rule(-15, new int[]{}),
    new Rule(-15, new int[]{-15,-16}),
    new Rule(-16, new int[]{311}),
    new Rule(-16, new int[]{357}),
    new Rule(-16, new int[]{313}),
    new Rule(-16, new int[]{398}),
    new Rule(-145, new int[]{-15,-29,-7,-8,320}),
    new Rule(-145, new int[]{-15,-29,-7,-8,320,61,-50}),
    new Rule(-28, new int[]{}),
    new Rule(-28, new int[]{-26}),
    new Rule(-29, new int[]{}),
    new Rule(-29, new int[]{-27}),
    new Rule(-26, new int[]{-23}),
    new Rule(-26, new int[]{63,-23}),
    new Rule(-26, new int[]{-37}),
    new Rule(-26, new int[]{-39}),
    new Rule(-27, new int[]{-24}),
    new Rule(-27, new int[]{63,-24}),
    new Rule(-27, new int[]{-38}),
    new Rule(-27, new int[]{-40}),
    new Rule(-23, new int[]{-24}),
    new Rule(-23, new int[]{353}),
    new Rule(-24, new int[]{368}),
    new Rule(-24, new int[]{372}),
    new Rule(-24, new int[]{-21}),
    new Rule(-37, new int[]{-23,124,-23}),
    new Rule(-37, new int[]{-37,124,-23}),
    new Rule(-38, new int[]{-24,124,-24}),
    new Rule(-38, new int[]{-38,124,-24}),
    new Rule(-39, new int[]{-23,401,-23}),
    new Rule(-39, new int[]{-39,401,-23}),
    new Rule(-40, new int[]{-24,401,-24}),
    new Rule(-40, new int[]{-40,401,-24}),
    new Rule(-25, new int[]{}),
    new Rule(-25, new int[]{58,-26}),
    new Rule(-143, new int[]{40,41}),
    new Rule(-143, new int[]{40,-144,-3,41}),
    new Rule(-143, new int[]{40,394,41}),
    new Rule(-144, new int[]{-141}),
    new Rule(-144, new int[]{-144,44,-141}),
    new Rule(-141, new int[]{-50}),
    new Rule(-141, new int[]{-130,58,-50}),
    new Rule(-141, new int[]{394,-50}),
    new Rule(-117, new int[]{-117,44,-67}),
    new Rule(-117, new int[]{-67}),
    new Rule(-67, new int[]{-56}),
    new Rule(-118, new int[]{-118,44,-68}),
    new Rule(-118, new int[]{-68}),
    new Rule(-68, new int[]{320}),
    new Rule(-68, new int[]{320,61,-50}),
    new Rule(-115, new int[]{-115,-93}),
    new Rule(-115, new int[]{}),
    new Rule(-96, new int[]{-9,-29,-124,59}),
    new Rule(-96, new int[]{-10,344,-113,59}),
    new Rule(-96, new int[]{-10,-5,-4,-130,-18,40,-147,41,-25,-168,-85,-168}),
    new Rule(-96, new int[]{-109}),
    new Rule(-93, new int[]{-96}),
    new Rule(-93, new int[]{-100,-96}),
    new Rule(-93, new int[]{350,-33,-90}),
    new Rule(-33, new int[]{-21}),
    new Rule(-33, new int[]{-33,44,-21}),
    new Rule(-90, new int[]{59}),
    new Rule(-90, new int[]{123,125}),
    new Rule(-90, new int[]{123,-121,125}),
    new Rule(-121, new int[]{-75}),
    new Rule(-121, new int[]{-121,-75}),
    new Rule(-75, new int[]{-76}),
    new Rule(-75, new int[]{-77}),
    new Rule(-76, new int[]{-136,354,-33,59}),
    new Rule(-77, new int[]{-135,338,319,59}),
    new Rule(-77, new int[]{-135,338,-132,59}),
    new Rule(-77, new int[]{-135,338,-12,-130,59}),
    new Rule(-77, new int[]{-135,338,-12,59}),
    new Rule(-135, new int[]{-130}),
    new Rule(-135, new int[]{-136}),
    new Rule(-136, new int[]{-21,390,-130}),
    new Rule(-85, new int[]{59}),
    new Rule(-85, new int[]{123,-114,125}),
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
    new Rule(-12, new int[]{398}),
    new Rule(-124, new int[]{-124,44,-71}),
    new Rule(-124, new int[]{-71}),
    new Rule(-71, new int[]{320,-18}),
    new Rule(-71, new int[]{320,61,-50,-18}),
    new Rule(-113, new int[]{-113,44,-78}),
    new Rule(-113, new int[]{-78}),
    new Rule(-78, new int[]{-130,61,-50,-18}),
    new Rule(-65, new int[]{319,61,-50,-18}),
    new Rule(-119, new int[]{-119,44,-69}),
    new Rule(-119, new int[]{-69}),
    new Rule(-69, new int[]{-50}),
    new Rule(-116, new int[]{}),
    new Rule(-116, new int[]{-127}),
    new Rule(-127, new int[]{-127,44,-50}),
    new Rule(-127, new int[]{-50}),
    new Rule(-174, new int[]{}),
    new Rule(-158, new int[]{361,-142,-30,-174,-35,-18,-19,123,-115,125,-20}),
    new Rule(-53, new int[]{306,-31,-142}),
    new Rule(-53, new int[]{306,-158}),
    new Rule(-53, new int[]{306,-100,-158}),
    new Rule(-52, new int[]{367,40,-154,41,61,-50}),
    new Rule(-52, new int[]{91,-154,93,61,-50}),
    new Rule(-52, new int[]{-51,61,-50}),
    new Rule(-52, new int[]{-51,61,-165,-51}),
    new Rule(-52, new int[]{-51,61,-165,-53}),
    new Rule(-52, new int[]{307,-50}),
    new Rule(-52, new int[]{-51,270,-50}),
    new Rule(-52, new int[]{-51,271,-50}),
    new Rule(-52, new int[]{-51,279,-50}),
    new Rule(-52, new int[]{-51,281,-50}),
    new Rule(-52, new int[]{-51,278,-50}),
    new Rule(-52, new int[]{-51,277,-50}),
    new Rule(-52, new int[]{-51,276,-50}),
    new Rule(-52, new int[]{-51,275,-50}),
    new Rule(-52, new int[]{-51,274,-50}),
    new Rule(-52, new int[]{-51,273,-50}),
    new Rule(-52, new int[]{-51,272,-50}),
    new Rule(-52, new int[]{-51,280,-50}),
    new Rule(-52, new int[]{-51,282,-50}),
    new Rule(-52, new int[]{-51,303}),
    new Rule(-52, new int[]{303,-51}),
    new Rule(-52, new int[]{-51,302}),
    new Rule(-52, new int[]{302,-51}),
    new Rule(-52, new int[]{-50,284,-50}),
    new Rule(-52, new int[]{-50,285,-50}),
    new Rule(-52, new int[]{-50,263,-50}),
    new Rule(-52, new int[]{-50,265,-50}),
    new Rule(-52, new int[]{-50,264,-50}),
    new Rule(-52, new int[]{-50,124,-50}),
    new Rule(-52, new int[]{-50,401,-50}),
    new Rule(-52, new int[]{-50,400,-50}),
    new Rule(-52, new int[]{-50,94,-50}),
    new Rule(-52, new int[]{-50,46,-50}),
    new Rule(-52, new int[]{-50,43,-50}),
    new Rule(-52, new int[]{-50,45,-50}),
    new Rule(-52, new int[]{-50,42,-50}),
    new Rule(-52, new int[]{-50,305,-50}),
    new Rule(-52, new int[]{-50,47,-50}),
    new Rule(-52, new int[]{-50,37,-50}),
    new Rule(-52, new int[]{-50,293,-50}),
    new Rule(-52, new int[]{-50,294,-50}),
    new Rule(-52, new int[]{43,-50}),
    new Rule(-52, new int[]{45,-50}),
    new Rule(-52, new int[]{33,-50}),
    new Rule(-52, new int[]{126,-50}),
    new Rule(-52, new int[]{-50,287,-50}),
    new Rule(-52, new int[]{-50,286,-50}),
    new Rule(-52, new int[]{-50,289,-50}),
    new Rule(-52, new int[]{-50,288,-50}),
    new Rule(-52, new int[]{-50,60,-50}),
    new Rule(-52, new int[]{-50,291,-50}),
    new Rule(-52, new int[]{-50,62,-50}),
    new Rule(-52, new int[]{-50,292,-50}),
    new Rule(-52, new int[]{-50,290,-50}),
    new Rule(-52, new int[]{-50,295,-31}),
    new Rule(-52, new int[]{40,-50,41}),
    new Rule(-52, new int[]{-53}),
    new Rule(-52, new int[]{-50,63,-50,58,-50}),
    new Rule(-52, new int[]{-50,63,58,-50}),
    new Rule(-52, new int[]{-50,283,-50}),
    new Rule(-52, new int[]{-54}),
    new Rule(-52, new int[]{301,-50}),
    new Rule(-52, new int[]{300,-50}),
    new Rule(-52, new int[]{299,-50}),
    new Rule(-52, new int[]{298,-50}),
    new Rule(-52, new int[]{297,-50}),
    new Rule(-52, new int[]{296,-50}),
    new Rule(-52, new int[]{304,-50}),
    new Rule(-52, new int[]{326,-86}),
    new Rule(-52, new int[]{64,-50}),
    new Rule(-52, new int[]{-57}),
    new Rule(-52, new int[]{-84}),
    new Rule(-52, new int[]{266,-50}),
    new Rule(-52, new int[]{267}),
    new Rule(-52, new int[]{267,-50}),
    new Rule(-52, new int[]{267,-50,268,-50}),
    new Rule(-52, new int[]{269,-50}),
    new Rule(-52, new int[]{352,-50}),
    new Rule(-52, new int[]{-94}),
    new Rule(-52, new int[]{-100,-94}),
    new Rule(-52, new int[]{353,-94}),
    new Rule(-52, new int[]{-100,353,-94}),
    new Rule(-52, new int[]{-102}),
    new Rule(-94, new int[]{-5,-4,-18,40,-147,41,-149,-25,-168,-19,123,-114,125,-20,-168}),
    new Rule(-94, new int[]{-6,-4,40,-147,41,-25,-18,268,-168,-175,-50,-168}),
    new Rule(-6, new int[]{343}),
    new Rule(-5, new int[]{346}),
    new Rule(-18, new int[]{}),
    new Rule(-19, new int[]{}),
    new Rule(-20, new int[]{}),
    new Rule(-168, new int[]{}),
    new Rule(-175, new int[]{}),
    new Rule(-4, new int[]{}),
    new Rule(-4, new int[]{-165}),
    new Rule(-149, new int[]{}),
    new Rule(-149, new int[]{350,40,-150,41}),
    new Rule(-150, new int[]{-150,44,-146}),
    new Rule(-150, new int[]{-146}),
    new Rule(-146, new int[]{320}),
    new Rule(-146, new int[]{-165,320}),
    new Rule(-61, new int[]{-21,-143}),
    new Rule(-61, new int[]{-32,390,-2,-143}),
    new Rule(-61, new int[]{-91,390,-2,-143}),
    new Rule(-61, new int[]{-89,-143}),
    new Rule(-32, new int[]{353}),
    new Rule(-32, new int[]{-21}),
    new Rule(-31, new int[]{-32}),
    new Rule(-31, new int[]{-88}),
    new Rule(-31, new int[]{40,-50,41}),
    new Rule(-86, new int[]{}),
    new Rule(-86, new int[]{40,-70,41}),
    new Rule(-84, new int[]{96,96}),
    new Rule(-84, new int[]{96,316,96}),
    new Rule(-84, new int[]{96,-122,96}),
    new Rule(-142, new int[]{}),
    new Rule(-142, new int[]{-143}),
    new Rule(-60, new int[]{368,40,-154,41}),
    new Rule(-60, new int[]{91,-154,93}),
    new Rule(-60, new int[]{323}),
    new Rule(-57, new int[]{317}),
    new Rule(-57, new int[]{312}),
    new Rule(-57, new int[]{370}),
    new Rule(-57, new int[]{371}),
    new Rule(-57, new int[]{375}),
    new Rule(-57, new int[]{374}),
    new Rule(-57, new int[]{378}),
    new Rule(-57, new int[]{376}),
    new Rule(-57, new int[]{392}),
    new Rule(-57, new int[]{373}),
    new Rule(-57, new int[]{34,-122,34}),
    new Rule(-57, new int[]{383,387}),
    new Rule(-57, new int[]{383,316,387}),
    new Rule(-57, new int[]{383,-122,387}),
    new Rule(-57, new int[]{-60}),
    new Rule(-57, new int[]{-58}),
    new Rule(-57, new int[]{-59}),
    new Rule(-58, new int[]{-21}),
    new Rule(-59, new int[]{-32,390,-130}),
    new Rule(-59, new int[]{-91,390,-130}),
    new Rule(-50, new int[]{-51}),
    new Rule(-50, new int[]{-52}),
    new Rule(-70, new int[]{}),
    new Rule(-70, new int[]{-50}),
    new Rule(-22, new int[]{369}),
    new Rule(-22, new int[]{396}),
    new Rule(-91, new int[]{-79}),
    new Rule(-79, new int[]{-51}),
    new Rule(-79, new int[]{40,-50,41}),
    new Rule(-79, new int[]{-60}),
    new Rule(-79, new int[]{-59}),
    new Rule(-80, new int[]{-79}),
    new Rule(-80, new int[]{-58}),
    new Rule(-89, new int[]{-55}),
    new Rule(-89, new int[]{40,-50,41}),
    new Rule(-89, new int[]{-60}),
    new Rule(-55, new int[]{-56}),
    new Rule(-55, new int[]{-80,91,-70,93}),
    new Rule(-55, new int[]{-80,123,-50,125}),
    new Rule(-55, new int[]{-80,-22,-1,-143}),
    new Rule(-55, new int[]{-61}),
    new Rule(-51, new int[]{-55}),
    new Rule(-51, new int[]{-62}),
    new Rule(-51, new int[]{-80,-22,-1}),
    new Rule(-56, new int[]{320}),
    new Rule(-56, new int[]{36,123,-50,125}),
    new Rule(-56, new int[]{36,-56}),
    new Rule(-62, new int[]{-32,390,-56}),
    new Rule(-62, new int[]{-91,390,-56}),
    new Rule(-88, new int[]{-56}),
    new Rule(-88, new int[]{-88,91,-70,93}),
    new Rule(-88, new int[]{-88,123,-50,125}),
    new Rule(-88, new int[]{-88,-22,-1}),
    new Rule(-88, new int[]{-32,390,-56}),
    new Rule(-88, new int[]{-88,390,-56}),
    new Rule(-2, new int[]{-130}),
    new Rule(-2, new int[]{123,-50,125}),
    new Rule(-2, new int[]{-56}),
    new Rule(-1, new int[]{319}),
    new Rule(-1, new int[]{123,-50,125}),
    new Rule(-1, new int[]{-56}),
    new Rule(-154, new int[]{-153}),
    new Rule(-151, new int[]{}),
    new Rule(-151, new int[]{-152}),
    new Rule(-153, new int[]{-153,44,-151}),
    new Rule(-153, new int[]{-151}),
    new Rule(-152, new int[]{-50,268,-50}),
    new Rule(-152, new int[]{-50}),
    new Rule(-152, new int[]{-50,268,-165,-51}),
    new Rule(-152, new int[]{-165,-51}),
    new Rule(-152, new int[]{394,-50}),
    new Rule(-152, new int[]{-50,268,367,40,-154,41}),
    new Rule(-152, new int[]{367,40,-154,41}),
    new Rule(-122, new int[]{-122,-72}),
    new Rule(-122, new int[]{-122,316}),
    new Rule(-122, new int[]{-72}),
    new Rule(-122, new int[]{316,-72}),
    new Rule(-72, new int[]{320}),
    new Rule(-72, new int[]{320,91,-73,93}),
    new Rule(-72, new int[]{320,-22,319}),
    new Rule(-72, new int[]{385,-50,125}),
    new Rule(-72, new int[]{385,318,125}),
    new Rule(-72, new int[]{385,318,91,-50,93,125}),
    new Rule(-72, new int[]{386,-51,125}),
    new Rule(-73, new int[]{319}),
    new Rule(-73, new int[]{325}),
    new Rule(-73, new int[]{320}),
    new Rule(-54, new int[]{358,40,-123,-3,41}),
    new Rule(-54, new int[]{359,40,-50,41}),
    new Rule(-54, new int[]{262,-50}),
    new Rule(-54, new int[]{261,-50}),
    new Rule(-54, new int[]{260,40,-50,41}),
    new Rule(-54, new int[]{259,-50}),
    new Rule(-54, new int[]{258,-50}),
    new Rule(-123, new int[]{-49}),
    new Rule(-123, new int[]{-123,44,-49}),
    new Rule(-49, new int[]{-50}),
    };
    #endregion

    nonTerminals = new string[] {"", "property_name", "member_name", "possible_comma", 
      "returns_ref", "function", "fn", "is_reference", "is_variadic", "variable_modifiers", 
      "method_modifiers", "non_empty_member_modifiers", "member_modifier", "class_modifier", 
      "class_modifiers", "optional_property_modifiers", "property_modifier", 
      "use_type", "backup_doc_comment", "enter_scope", "exit_scope", "name", 
      "object_operator", "type", "type_without_static", "return_type", "type_expr", 
      "type_expr_without_static", "optional_type", "optional_type_without_static", 
      "extends_from", "class_name_reference", "class_name", "name_list", "catch_name_list", 
      "implements_list", "interface_extends_list", "union_type", "union_type_without_static", 
      "intersection_type", "intersection_type_without_static", "top_statement", 
      "statement", "function_declaration_statement", "class_declaration_statement", 
      "trait_declaration_statement", "interface_declaratioimplements_listn_statement", 
      "interface_declaration_statement", "inline_html", "isset_variable", "expr", 
      "variable", "expr_without_variable", "new_expr", "internal_functions_in_yacc", 
      "callable_variable", "simple_variable", "scalar", "constant", "class_constant", 
      "dereferencable_scalar", "function_call", "static_member", "if_stmt", "alt_if_stmt", 
      "const_decl", "unset_variable", "global_var", "static_var", "echo_expr", 
      "optional_expr", "property", "encaps_var", "encaps_var_offset", "declare_statement", 
      "trait_adaptation", "trait_precedence", "trait_alias", "class_const_decl", 
      "dereferencable", "array_object_dereferenceable", "for_statement", "foreach_statement", 
      "while_statement", "backticks_expr", "method_body", "exit_expr", "finally_statement", 
      "new_variable", "callable_expr", "trait_adaptations", "variable_class_name", 
      "inner_statement", "class_statement", "inline_function", "attributed_statement", 
      "attributed_class_statement", "attributed_parameter", "attribute", "attribute_decl", 
      "attributes", "attribute_group", "match", "match_arm", "match_arm_list", 
      "match_arm_cond_list", "non_empty_match_arm_list", "enum_declaration_statement", 
      "enum_backing_type", "enum_case", "enum_case_expr", "top_statement_list", 
      "const_list", "class_const_list", "inner_statement_list", "class_statement_list", 
      "for_exprs", "global_var_list", "static_var_list", "echo_expr_list", "unset_variables", 
      "trait_adaptation_list", "encaps_list", "isset_variables", "property_list", 
      "case_list", "switch_case_list", "non_empty_for_exprs", "catch_list", "optional_variable", 
      "identifier", "semi_reserved", "reserved_non_modifiers", "namespace_name", 
      "unprefixed_use_declaration", "trait_method_reference", "absolute_trait_method_reference", 
      "use_declaration", "unprefixed_use_declarations", "inline_use_declaration", 
      "inline_use_declarations", "argument", "ctor_arguments", "argument_list", 
      "non_empty_argument_list", "parameter", "lexical_var", "parameter_list", 
      "non_empty_parameter_list", "lexical_vars", "lexical_var_list", "possible_array_pair", 
      "array_pair", "non_empty_array_pair_list", "array_pair_list", "if_stmt_without_else", 
      "alt_if_stmt_without_else", "foreach_variable", "anonymous_class", "use_declarations", 
      "group_use_declaration", "mixed_group_use_declaration", "start", "$accept", 
      "@1", "ampersand", "@2", "@3", "backup_fn_flags", "@4", "@5", "@6", "@7", 
      "case_separator", "@8", "backup_lex_pos", };
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
        _lexer.DocCommentList.Merge(new Span(0, int.MaxValue), value_stack.array[value_stack.top-1].yyval.NodeList, _astFactory);
		AssignStatements(value_stack.array[value_stack.top-1].yyval.NodeList);
		_astRoot = _astFactory.GlobalCode(yypos, value_stack.array[value_stack.top-1].yyval.NodeList, namingContext);
	}
        return;
      case 86: // top_statement_list -> top_statement_list top_statement 
{ yyval.NodeList = AddToTopStatementList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 87: // top_statement_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 88: // namespace_name -> T_STRING 
{ yyval.StringList = new List<string>() { value_stack.array[value_stack.top-1].yyval.String }; }
        return;
      case 89: // namespace_name -> namespace_name T_NS_SEPARATOR T_STRING 
{ yyval.StringList = AddToList<string>(value_stack.array[value_stack.top-3].yyval.StringList, value_stack.array[value_stack.top-1].yyval.String); }
        return;
      case 90: // name -> namespace_name 
{ yyval.QualifiedNameReference = new QualifiedNameRef(yypos, new QualifiedName(value_stack.array[value_stack.top-1].yyval.StringList, true, false)); }
        return;
      case 91: // name -> T_NAMESPACE T_NS_SEPARATOR namespace_name 
{ yyval.QualifiedNameReference = new QualifiedNameRef(yypos, MergeWithCurrentNamespace(namingContext.CurrentNamespace, value_stack.array[value_stack.top-1].yyval.StringList)); }
        return;
      case 92: // name -> T_NS_SEPARATOR namespace_name 
{ yyval.QualifiedNameReference = new QualifiedNameRef(yypos, new QualifiedName(value_stack.array[value_stack.top-1].yyval.StringList, true,  true)); }
        return;
      case 93: // attribute_decl -> class_name 
{ yyval.Node = _astFactory.Attribute(yypos, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 94: // attribute_decl -> class_name argument_list 
{ yyval.Node = _astFactory.Attribute(yypos, value_stack.array[value_stack.top-2].yyval.TypeReference, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos)); }
        return;
      case 95: // attribute_group -> attribute_decl 
{ yyval.NodeList = new List<LangElement>(1) { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 96: // attribute_group -> attribute_group ',' attribute_decl 
{ yyval.NodeList = AddToList(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 97: // attribute -> T_ATTRIBUTE attribute_group possible_comma ']' 
{ yyval.Node = _astFactory.AttributeGroup(yypos, value_stack.array[value_stack.top-3].yyval.NodeList); }
        return;
      case 98: // attributes -> attribute 
{ yyval.NodeList = new List<LangElement>(1) { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 99: // attributes -> attributes attribute 
{ yyval.NodeList = AddToList(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 100: // attributed_statement -> function_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 101: // attributed_statement -> class_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 102: // attributed_statement -> trait_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 103: // attributed_statement -> interface_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 104: // attributed_statement -> enum_declaration_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 105: // top_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 106: // top_statement -> attributed_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 107: // top_statement -> attributes attributed_statement 
{ yyval.Node = WithAttributes(value_stack.array[value_stack.top-1].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 108: // top_statement -> T_HALT_COMPILER '(' ')' ';' 
{ yyval.Node = _astFactory.HaltCompiler(yypos); }
        return;
      case 109: // top_statement -> T_NAMESPACE namespace_name ';' 
{
			AssignNamingContext();
            SetNamingContext(value_stack.array[value_stack.top-2].yyval.StringList);
            SetDoc(yyval.Node = _currentNamespace = (NamespaceDecl)_astFactory.Namespace(yypos, namingContext.CurrentNamespace, value_stack.array[value_stack.top-2].yypos, namingContext));
		}
        return;
      case 110: // @2 -> 
{ SetNamingContext(value_stack.array[value_stack.top-2].yyval.StringList); }
        return;
      case 111: // top_statement -> T_NAMESPACE namespace_name backup_doc_comment @2 '{' top_statement_list '}' 
{ 
			yyval.Node = _astFactory.Namespace(yypos, namingContext.CurrentNamespace, value_stack.array[value_stack.top-6].yypos, CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList), namingContext);
			SetDoc(yyval.Node);
			ResetNamingContext(); 
		}
        return;
      case 112: // @3 -> 
{ SetNamingContext(null); }
        return;
      case 113: // top_statement -> T_NAMESPACE backup_doc_comment @3 '{' top_statement_list '}' 
{ 
			yyval.Node = _astFactory.Namespace(yypos, null, yypos, CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList), namingContext);
			SetDoc(yyval.Node);
			ResetNamingContext(); 
		}
        return;
      case 114: // top_statement -> T_USE mixed_group_use_declaration ';' 
{ yyval.Node = _astFactory.Use(yypos, value_stack.array[value_stack.top-2].yyval.UseList, AliasKind.Type); _contextType = AliasKind.Type;	/* TODO: Error - must contain only a single group use */	}
        return;
      case 115: // top_statement -> T_USE use_type group_use_declaration ';' 
{ yyval.Node = _astFactory.Use(yypos, value_stack.array[value_stack.top-2].yyval.UseList, value_stack.array[value_stack.top-3].yyval.Kind); _contextType = AliasKind.Type;				/* TODO: Error - must contain only a single group use */	}
        return;
      case 116: // top_statement -> T_USE use_declarations ';' 
{ yyval.Node = _astFactory.Use(yypos, value_stack.array[value_stack.top-2].yyval.UseList, AliasKind.Type); _contextType = AliasKind.Type;	/* TODO: Error - must contain only simple uses		  */	}
        return;
      case 117: // top_statement -> T_USE use_type use_declarations ';' 
{ yyval.Node = _astFactory.Use(yypos, value_stack.array[value_stack.top-2].yyval.UseList, value_stack.array[value_stack.top-3].yyval.Kind); _contextType = AliasKind.Type;				/* TODO: Error - must contain only simple uses		  */	}
        return;
      case 118: // top_statement -> T_CONST const_list ';' 
{
			SetDoc(yyval.Node = _astFactory.DeclList(yypos, PhpMemberAttributes.None, value_stack.array[value_stack.top-2].yyval.NodeList, null));
		}
        return;
      case 119: // use_type -> T_FUNCTION 
{ yyval.Kind = _contextType = AliasKind.Function; }
        return;
      case 120: // use_type -> T_CONST 
{ yyval.Kind = _contextType = AliasKind.Constant; }
        return;
      case 121: // group_use_declaration -> namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations possible_comma '}' 
{ yyval.UseList = new List<UseBase>() { AddAliases(yypos, value_stack.array[value_stack.top-6].yyval.StringList, CombineSpans(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos), AddTrailingComma(value_stack.array[value_stack.top-3].yyval.AliasList, value_stack.array[value_stack.top-2].yyval.Bool), false) }; }
        return;
      case 122: // group_use_declaration -> T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' unprefixed_use_declarations possible_comma '}' 
{ yyval.UseList = new List<UseBase>() { AddAliases(yypos, value_stack.array[value_stack.top-6].yyval.StringList, CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos), AddTrailingComma(value_stack.array[value_stack.top-3].yyval.AliasList, value_stack.array[value_stack.top-2].yyval.Bool), true) }; }
        return;
      case 123: // mixed_group_use_declaration -> namespace_name T_NS_SEPARATOR '{' inline_use_declarations possible_comma '}' 
{  yyval.UseList = new List<UseBase>() { AddAliases(yypos, value_stack.array[value_stack.top-6].yyval.StringList, CombineSpans(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos), AddTrailingComma(value_stack.array[value_stack.top-3].yyval.ContextAliasList, value_stack.array[value_stack.top-2].yyval.Bool), false) }; }
        return;
      case 124: // mixed_group_use_declaration -> T_NS_SEPARATOR namespace_name T_NS_SEPARATOR '{' inline_use_declarations possible_comma '}' 
{ yyval.UseList = new List<UseBase>() { AddAliases(yypos, value_stack.array[value_stack.top-6].yyval.StringList, CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos), AddTrailingComma(value_stack.array[value_stack.top-3].yyval.ContextAliasList, value_stack.array[value_stack.top-2].yyval.Bool), true) }; }
        return;
      case 125: // possible_comma -> 
{ yyval.Bool = false; }
        return;
      case 126: // possible_comma -> ',' 
{ yyval.Bool = true;  }
        return;
      case 127: // inline_use_declarations -> inline_use_declarations ',' inline_use_declaration 
{ yyval.ContextAliasList = AddToList<ContextAlias>(value_stack.array[value_stack.top-3].yyval.ContextAliasList, value_stack.array[value_stack.top-1].yyval.ContextAlias); }
        return;
      case 128: // inline_use_declarations -> inline_use_declaration 
{ yyval.ContextAliasList = new List<ContextAlias>() { value_stack.array[value_stack.top-1].yyval.ContextAlias }; }
        return;
      case 129: // unprefixed_use_declarations -> unprefixed_use_declarations ',' unprefixed_use_declaration 
{ yyval.AliasList = AddToList<CompleteAlias>(value_stack.array[value_stack.top-3].yyval.AliasList, value_stack.array[value_stack.top-1].yyval.Alias); }
        return;
      case 130: // unprefixed_use_declarations -> unprefixed_use_declaration 
{ yyval.AliasList = new List<CompleteAlias>() { value_stack.array[value_stack.top-1].yyval.Alias }; }
        return;
      case 131: // use_declarations -> use_declarations ',' use_declaration 
{ yyval.UseList = AddToList<UseBase>(value_stack.array[value_stack.top-3].yyval.UseList, AddAlias(value_stack.array[value_stack.top-1].yyval.Alias)); }
        return;
      case 132: // use_declarations -> use_declaration 
{ yyval.UseList = new List<UseBase>() { AddAlias(value_stack.array[value_stack.top-1].yyval.Alias) }; }
        return;
      case 133: // inline_use_declaration -> unprefixed_use_declaration 
{ yyval.ContextAlias = JoinTuples(yypos, value_stack.array[value_stack.top-1].yyval.Alias, AliasKind.Type); }
        return;
      case 134: // inline_use_declaration -> use_type unprefixed_use_declaration 
{ yyval.ContextAlias = JoinTuples(yypos, value_stack.array[value_stack.top-1].yyval.Alias, (AliasKind)value_stack.array[value_stack.top-2].yyval.Kind);  }
        return;
      case 135: // unprefixed_use_declaration -> namespace_name 
{ yyval.Alias = new CompleteAlias(new QualifiedNameRef(yypos, new QualifiedName(value_stack.array[value_stack.top-1].yyval.StringList, true, false)), NameRef.Invalid); }
        return;
      case 136: // unprefixed_use_declaration -> namespace_name T_AS T_STRING 
{ yyval.Alias = new CompleteAlias(new QualifiedNameRef(value_stack.array[value_stack.top-3].yypos, new QualifiedName(value_stack.array[value_stack.top-3].yyval.StringList, true, false)), new NameRef(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.String)); }
        return;
      case 137: // use_declaration -> unprefixed_use_declaration 
{ yyval.Alias = value_stack.array[value_stack.top-1].yyval.Alias; }
        return;
      case 138: // use_declaration -> T_NS_SEPARATOR unprefixed_use_declaration 
{ 
				var src = value_stack.array[value_stack.top-1].yyval.Alias;
				yyval.Alias = new CompleteAlias(new QualifiedNameRef(CombineSpans(value_stack.array[value_stack.top-2].yypos, src.Item1.Span), 
					new QualifiedName(src.Item1.QualifiedName.Name, src.Item1.QualifiedName.Namespaces, true)), src.Item2); 
			}
        return;
      case 139: // const_list -> const_list ',' const_decl 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 140: // const_list -> const_decl 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 141: // inner_statement_list -> inner_statement_list inner_statement 
{ yyval.NodeList = AddToTopStatementList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 142: // inner_statement_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 143: // inner_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 144: // inner_statement -> attributed_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 145: // inner_statement -> attributes attributed_statement 
{ yyval.Node = WithAttributes(value_stack.array[value_stack.top-1].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 146: // inner_statement -> T_HALT_COMPILER '(' ')' ';' 
{ 
				yyval.Node = _astFactory.HaltCompiler(yypos); 
				_errors.Error(yypos, FatalErrors.InvalidHaltCompiler); 
			}
        return;
      case 147: // statement -> '{' inner_statement_list '}' 
{ yyval.Node = CreateBlock(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 148: // statement -> enter_scope if_stmt exit_scope 
{ yyval.Node = value_stack.array[value_stack.top-2].yyval.Node; }
        return;
      case 149: // statement -> enter_scope alt_if_stmt exit_scope 
{ yyval.Node = value_stack.array[value_stack.top-2].yyval.Node; }
        return;
      case 150: // statement -> T_WHILE '(' expr ')' enter_scope while_statement exit_scope 
{ yyval.Node = _astFactory.While(yypos, value_stack.array[value_stack.top-5].yyval.Node, CombineSpans(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-4].yypos), value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 151: // statement -> T_DO enter_scope statement T_WHILE '(' expr ')' ';' exit_scope 
{ yyval.Node = _astFactory.Do(yypos, value_stack.array[value_stack.top-7].yyval.Node, value_stack.array[value_stack.top-4].yyval.Node, CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-3].yypos)); }
        return;
      case 152: // statement -> T_FOR '(' for_exprs ';' for_exprs ';' for_exprs ')' enter_scope for_statement exit_scope 
{ yyval.Node = _astFactory.For(yypos, value_stack.array[value_stack.top-9].yyval.NodeList, value_stack.array[value_stack.top-7].yyval.NodeList, value_stack.array[value_stack.top-5].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-4].yypos), value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 153: // statement -> T_SWITCH '(' expr ')' enter_scope switch_case_list exit_scope 
{ yyval.Node = _astFactory.Switch(yypos, value_stack.array[value_stack.top-5].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 154: // statement -> T_BREAK optional_expr ';' 
{ yyval.Node = _astFactory.Jump(yypos, JumpStmt.Types.Break, value_stack.array[value_stack.top-2].yyval.Node);}
        return;
      case 155: // statement -> T_CONTINUE optional_expr ';' 
{ yyval.Node = _astFactory.Jump(yypos, JumpStmt.Types.Continue, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 156: // statement -> T_RETURN optional_expr ';' 
{ yyval.Node = _astFactory.Jump(yypos, JumpStmt.Types.Return, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 157: // statement -> T_GLOBAL global_var_list ';' 
{ yyval.Node = _astFactory.Global(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 158: // statement -> T_STATIC static_var_list ';' 
{ yyval.Node = _astFactory.Static(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 159: // statement -> T_ECHO echo_expr_list ';' 
{ yyval.Node = _astFactory.Echo(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 160: // statement -> T_INLINE_HTML 
{ yyval.Node = _astFactory.InlineHtml(yypos, value_stack.array[value_stack.top-1].yyval.String); }
        return;
      case 161: // statement -> expr ';' 
{ yyval.Node = _astFactory.ExpressionStmt(yypos, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 162: // statement -> T_UNSET '(' unset_variables possible_comma ')' ';' 
{ yyval.Node = _astFactory.Unset(yypos, AddTrailingComma(value_stack.array[value_stack.top-4].yyval.NodeList, value_stack.array[value_stack.top-3].yyval.Bool)); }
        return;
      case 163: // statement -> T_FOREACH '(' expr T_AS foreach_variable ')' enter_scope foreach_statement exit_scope 
{ yyval.Node = _astFactory.Foreach(yypos, value_stack.array[value_stack.top-7].yyval.Node, null, value_stack.array[value_stack.top-5].yyval.ForeachVar, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 164: // statement -> T_FOREACH '(' expr T_AS foreach_variable T_DOUBLE_ARROW foreach_variable ')' enter_scope foreach_statement exit_scope 
{ yyval.Node = _astFactory.Foreach(yypos, value_stack.array[value_stack.top-9].yyval.Node, value_stack.array[value_stack.top-7].yyval.ForeachVar, value_stack.array[value_stack.top-5].yyval.ForeachVar, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 165: // statement -> T_DECLARE '(' const_list ')' declare_statement 
{ yyval.Node = _astFactory.Declare(yypos, value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 166: // statement -> ';' 
{ yyval.Node = _astFactory.EmptyStmt(yypos); }
        return;
      case 167: // statement -> T_TRY '{' inner_statement_list '}' enter_scope catch_list finally_statement exit_scope 
{ yyval.Node = _astFactory.TryCatch(yypos, CreateBlock(CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-5].yypos), value_stack.array[value_stack.top-6].yyval.NodeList), value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 168: // statement -> T_GOTO T_STRING ';' 
{ yyval.Node = _astFactory.Goto(yypos, value_stack.array[value_stack.top-2].yyval.String, value_stack.array[value_stack.top-2].yypos); }
        return;
      case 169: // statement -> T_STRING ':' 
{ yyval.Node = _astFactory.Label(yypos, value_stack.array[value_stack.top-2].yyval.String, value_stack.array[value_stack.top-2].yypos); }
        return;
      case 170: // catch_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 171: // catch_list -> catch_list T_CATCH '(' catch_name_list optional_variable ')' '{' inner_statement_list '}' 
{ 
				yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-9].yyval.NodeList, _astFactory.Catch(CombineSpans(value_stack.array[value_stack.top-8].yypos, value_stack.array[value_stack.top-1].yypos), _astFactory.TypeReference(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-6].yyval.TypeRefList), 
					(DirectVarUse)value_stack.array[value_stack.top-5].yyval.Node, CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList))); 
			}
        return;
      case 172: // optional_variable -> 
{ yyval.Node = null; }
        return;
      case 173: // optional_variable -> T_VARIABLE 
{ yyval.Node = _astFactory.Variable(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.String, NullLangElement, true); }
        return;
      case 174: // catch_name_list -> name 
{ yyval.TypeRefList = new List<TypeRef>() { CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference) }; }
        return;
      case 175: // catch_name_list -> catch_name_list '|' name 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference)); }
        return;
      case 176: // finally_statement -> 
{ yyval.Node = null; }
        return;
      case 177: // finally_statement -> T_FINALLY '{' inner_statement_list '}' 
{ yyval.Node =_astFactory.Finally(yypos, CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList)); }
        return;
      case 178: // unset_variables -> unset_variable 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 179: // unset_variables -> unset_variables ',' unset_variable 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 180: // unset_variable -> variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 181: // function_declaration_statement -> function returns_ref T_STRING backup_doc_comment '(' parameter_list ')' return_type backup_fn_flags enter_scope '{' inner_statement_list '}' exit_scope backup_fn_flags 
{ 
			yyval.Node = _astFactory.Function(yypos, isConditional, value_stack.array[value_stack.top-14].yyval.Long == (long)FormalParam.Flags.IsByRef, PhpMemberAttributes.None, value_stack.array[value_stack.top-8].yyval.TypeReference, 
			new Name(value_stack.array[value_stack.top-13].yyval.String), value_stack.array[value_stack.top-13].yypos, null, value_stack.array[value_stack.top-10].yyval.FormalParamList, CombineSpans(value_stack.array[value_stack.top-11].yypos, value_stack.array[value_stack.top-9].yypos), 
			CreateBlock(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-3].yypos), value_stack.array[value_stack.top-4].yyval.NodeList)); 
			SetDoc(yyval.Node);
		}
        return;
      case 182: // is_reference -> 
{ yyval.Long = 0; }
        return;
      case 183: // is_reference -> T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG 
{ yyval.Long = (long)FormalParam.Flags.IsByRef; }
        return;
      case 184: // is_variadic -> 
{ yyval.Long = 0; }
        return;
      case 185: // is_variadic -> T_ELLIPSIS 
{ yyval.Long = (long)FormalParam.Flags.IsVariadic; }
        return;
      case 186: // @4 -> 
{PushClassContext(value_stack.array[value_stack.top-2].yyval.String, value_stack.array[value_stack.top-1].yyval.TypeReference, (PhpMemberAttributes)value_stack.array[value_stack.top-4].yyval.Long);}
        return;
      case 187: // class_declaration_statement -> class_modifiers T_CLASS T_STRING extends_from @4 implements_list backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{ 
			yyval.Node = _astFactory.Type(yypos, CombineSpans(value_stack.array[value_stack.top-12].yypos, value_stack.array[value_stack.top-11].yypos, value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-7].yypos), isConditional, (PhpMemberAttributes)value_stack.array[value_stack.top-12].yyval.Long, new Name(value_stack.array[value_stack.top-10].yyval.String), value_stack.array[value_stack.top-10].yypos, null, 
				ConvertToNamedTypeRef(value_stack.array[value_stack.top-9].yyval.TypeReference), value_stack.array[value_stack.top-7].yyval.TypeRefList.Select(ConvertToNamedTypeRef), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos)); 
			SetDoc(yyval.Node);
			PopClassContext();
		}
        return;
      case 188: // class_modifiers -> 
{ yyval.Long = (long)PhpMemberAttributes.None; }
        return;
      case 189: // class_modifiers -> class_modifier class_modifiers 
{ yyval.Long = AddModifier(value_stack.array[value_stack.top-2].yyval.Long, value_stack.array[value_stack.top-1].yyval.Long, value_stack.array[value_stack.top-1].yypos); }
        return;
      case 190: // class_modifier -> T_ABSTRACT 
{ yyval.Long = (long)PhpMemberAttributes.Abstract; }
        return;
      case 191: // class_modifier -> T_FINAL 
{ yyval.Long = (long)PhpMemberAttributes.Final; }
        return;
      case 192: // class_modifier -> T_READONLY 
{ yyval.Long = (long)PhpMemberAttributes.ReadOnly; }
        return;
      case 193: // @5 -> 
{PushClassContext(value_stack.array[value_stack.top-1].yyval.String, null, PhpMemberAttributes.Trait);}
        return;
      case 194: // trait_declaration_statement -> T_TRAIT T_STRING @5 backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{ 
			yyval.Node = _astFactory.Type(yypos, CombineSpans(value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-8].yypos), isConditional, PhpMemberAttributes.Trait, 
				new Name(value_stack.array[value_stack.top-8].yyval.String), value_stack.array[value_stack.top-8].yypos, null, null, new List<INamedTypeRef>(), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos)); 
			SetDoc(yyval.Node);
			PopClassContext();
		}
        return;
      case 195: // @6 -> 
{PushClassContext(value_stack.array[value_stack.top-1].yyval.String, null, PhpMemberAttributes.Interface);}
        return;
      case 196: // interface_declaration_statement -> T_INTERFACE T_STRING @6 interface_extends_list backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{ 
			yyval.Node = _astFactory.Type(yypos, CombineSpans(value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-7].yypos), isConditional, PhpMemberAttributes.Interface, 
				new Name(value_stack.array[value_stack.top-9].yyval.String), value_stack.array[value_stack.top-9].yypos, null, null, value_stack.array[value_stack.top-7].yyval.TypeRefList.Select(ConvertToNamedTypeRef), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos)); 
			SetDoc(yyval.Node);
			PopClassContext();
		}
        return;
      case 197: // @7 -> 
{PushClassContext(value_stack.array[value_stack.top-3].yyval.String, value_stack.array[value_stack.top-1].yyval.TypeReference, PhpMemberAttributes.Enum);}
        return;
      case 198: // enum_declaration_statement -> T_ENUM T_STRING enum_backing_type extends_from @7 implements_list backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{ 
			yyval.Node = _astFactory.Type(yypos, CombineSpans(value_stack.array[value_stack.top-12].yypos, value_stack.array[value_stack.top-11].yypos, value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-7].yypos), isConditional, PhpMemberAttributes.Enum, new Name(value_stack.array[value_stack.top-11].yyval.String), value_stack.array[value_stack.top-11].yypos, null, 
				ConvertToNamedTypeRef(value_stack.array[value_stack.top-9].yyval.TypeReference), value_stack.array[value_stack.top-7].yyval.TypeRefList.Select(ConvertToNamedTypeRef), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos)); 
			SetDoc(yyval.Node);
			SetEnumBackingType(yyval.Node, value_stack.array[value_stack.top-10].yyval.Node);
			PopClassContext();
		}
        return;
      case 199: // enum_backing_type -> 
{ yyval.Node = null; }
        return;
      case 200: // enum_backing_type -> ':' type_expr 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 201: // enum_case -> T_CASE identifier enum_case_expr ';' 
{ yyval.Node = _astFactory.EnumCase(yypos, value_stack.array[value_stack.top-3].yyval.String, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 202: // enum_case_expr -> 
{ yyval.Node = null; }
        return;
      case 203: // enum_case_expr -> '=' expr 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 204: // extends_from -> 
{ yyval.TypeReference = null; }
        return;
      case 205: // extends_from -> T_EXTENDS name 
{ yyval.TypeReference = CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference); }
        return;
      case 206: // interface_extends_list -> 
{ yyval.TypeRefList = TypeRef.EmptyList; }
        return;
      case 207: // interface_extends_list -> T_EXTENDS name_list 
{ yyval.TypeRefList = value_stack.array[value_stack.top-1].yyval.TypeRefList; }
        return;
      case 208: // implements_list -> 
{ yyval.TypeRefList = TypeRef.EmptyList; }
        return;
      case 209: // implements_list -> T_IMPLEMENTS name_list 
{ yyval.TypeRefList = value_stack.array[value_stack.top-1].yyval.TypeRefList; }
        return;
      case 210: // foreach_variable -> variable 
{ yyval.ForeachVar = _astFactory.ForeachVariable(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 211: // foreach_variable -> ampersand variable 
{ yyval.ForeachVar = _astFactory.ForeachVariable(yypos, value_stack.array[value_stack.top-1].yyval.Node, true); }
        return;
      case 212: // foreach_variable -> T_LIST '(' array_pair_list ')' 
{ yyval.ForeachVar = _astFactory.ForeachVariable(yypos, _astFactory.List(yypos, value_stack.array[value_stack.top-2].yyval.ItemList, true)); }
        return;
      case 213: // foreach_variable -> '[' array_pair_list ']' 
{ yyval.ForeachVar = _astFactory.ForeachVariable(yypos, _astFactory.List(yypos, value_stack.array[value_stack.top-2].yyval.ItemList, false)); }
        return;
      case 214: // for_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 215: // for_statement -> ':' inner_statement_list T_ENDFOR ';' 
{ yyval.Node = StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDFOR); }
        return;
      case 216: // foreach_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 217: // foreach_statement -> ':' inner_statement_list T_ENDFOREACH ';' 
{ yyval.Node = StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDFOREACH); }
        return;
      case 218: // declare_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 219: // declare_statement -> ':' inner_statement_list T_ENDDECLARE ';' 
{ yyval.Node = StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDDECLARE); }
        return;
      case 220: // switch_case_list -> '{' case_list '}' 
{ yyval.NodeList = value_stack.array[value_stack.top-2].yyval.NodeList; }
        return;
      case 221: // switch_case_list -> '{' ';' case_list '}' 
{ yyval.NodeList = value_stack.array[value_stack.top-2].yyval.NodeList; }
        return;
      case 222: // switch_case_list -> ':' case_list T_ENDSWITCH ';' 
{ yyval.NodeList = value_stack.array[value_stack.top-3].yyval.NodeList; }
        return;
      case 223: // switch_case_list -> ':' ';' case_list T_ENDSWITCH ';' 
{ yyval.NodeList = value_stack.array[value_stack.top-3].yyval.NodeList; }
        return;
      case 224: // case_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 225: // case_list -> case_list T_CASE expr case_separator inner_statement_list 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-5].yyval.NodeList, _astFactory.Case(CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), 
				value_stack.array[value_stack.top-3].yyval.Node, CreateCaseBlock(CombineSpans(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-1].yyval.NodeList))); }
        return;
      case 226: // case_list -> case_list T_DEFAULT case_separator inner_statement_list 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-4].yyval.NodeList, _astFactory.Case(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), 
				null, CreateCaseBlock(CombineSpans(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-1].yyval.NodeList))); }
        return;
      case 229: // match -> T_MATCH '(' expr ')' '{' match_arm_list '}' 
{ yyval.Node = (LangElement)_astFactory.Match(yypos, value_stack.array[value_stack.top-5].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 230: // match_arm_list -> 
{ yyval.NodeList = LangElement.EmptyList; }
        return;
      case 231: // match_arm_list -> non_empty_match_arm_list possible_comma 
{ yyval.NodeList = value_stack.array[value_stack.top-2].yyval.NodeList; }
        return;
      case 232: // non_empty_match_arm_list -> match_arm 
{ yyval.NodeList = new List<LangElement>(1) { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 233: // non_empty_match_arm_list -> non_empty_match_arm_list ',' match_arm 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 234: // match_arm -> match_arm_cond_list possible_comma T_DOUBLE_ARROW expr 
{ yyval.Node = (LangElement)_astFactory.MatchArm(yypos, value_stack.array[value_stack.top-4].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 235: // match_arm -> T_DEFAULT possible_comma T_DOUBLE_ARROW expr 
{ yyval.Node = (LangElement)_astFactory.MatchArm(yypos, LangElement.EmptyList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 236: // match_arm_cond_list -> expr 
{ yyval.NodeList = new List<LangElement>(1) { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 237: // match_arm_cond_list -> match_arm_cond_list ',' expr 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 238: // while_statement -> statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 239: // while_statement -> ':' inner_statement_list T_ENDWHILE ';' 
{ yyval.Node = StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDWHILE); }
        return;
      case 240: // if_stmt_without_else -> T_IF '(' expr ')' statement 
{ yyval.IfItemList = new List<IfStatement>() { 
				new IfStatement(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node) }; 
			}
        return;
      case 241: // if_stmt_without_else -> if_stmt_without_else T_ELSEIF '(' expr ')' statement 
{ yyval.IfItemList = AddToList<IfStatement>(value_stack.array[value_stack.top-6].yyval.IfItemList, 
				new IfStatement(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node)); 
			}
        return;
      case 242: // if_stmt -> if_stmt_without_else 
{ ((List<IfStatement>)value_stack.array[value_stack.top-1].yyval.IfItemList).Reverse(); yyval.Node = null; 
			foreach (var item in (List<IfStatement>)value_stack.array[value_stack.top-1].yyval.IfItemList) 
				yyval.Node = _astFactory.If(yyval.Node != null? CombineSpans(item.Span, (yyval.Node).Span): item.Span, item.Condition, item.Body, yyval.Node); }
        return;
      case 243: // if_stmt -> if_stmt_without_else T_ELSE statement 
{ ((List<IfStatement>)value_stack.array[value_stack.top-3].yyval.IfItemList).Reverse(); yyval.Node = _astFactory.If(CombineSpans(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), null, value_stack.array[value_stack.top-1].yyval.Node, null); 
			foreach (var item in (List<IfStatement>)value_stack.array[value_stack.top-3].yyval.IfItemList) yyval.Node = _astFactory.If(CombineSpans(item.Span, (yyval.Node).Span), item.Condition, item.Body, yyval.Node); }
        return;
      case 244: // alt_if_stmt_without_else -> T_IF '(' expr ')' ':' inner_statement_list 
{ 
				yyval.IfItemList = new List<IfStatement>() { new IfStatement(yypos, value_stack.array[value_stack.top-4].yyval.Node, StatementsToBlock(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yyval.NodeList, Tokens.END)) }; 
			}
        return;
      case 245: // alt_if_stmt_without_else -> alt_if_stmt_without_else T_ELSEIF '(' expr ')' ':' inner_statement_list 
{ 
				RebuildLast(value_stack.array[value_stack.top-7].yyval.IfItemList, value_stack.array[value_stack.top-6].yypos, Tokens.T_ELSEIF);
				yyval.IfItemList = AddToList<IfStatement>(value_stack.array[value_stack.top-7].yyval.IfItemList, 
					new IfStatement(CombineSpans(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-4].yyval.Node, StatementsToBlock(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yyval.NodeList, Tokens.END))); 
			}
        return;
      case 246: // alt_if_stmt -> alt_if_stmt_without_else T_ENDIF ';' 
{ RebuildLast(value_stack.array[value_stack.top-3].yyval.IfItemList, value_stack.array[value_stack.top-2].yypos, Tokens.T_ENDIF);
			 ((List<IfStatement>)value_stack.array[value_stack.top-3].yyval.IfItemList).Reverse(); yyval.Node = null; 
			foreach (var item in (List<IfStatement>)value_stack.array[value_stack.top-3].yyval.IfItemList) 
				yyval.Node = _astFactory.If(yyval.Node != null? CombineSpans(item.Span, (yyval.Node).Span): item.Span, item.Condition, item.Body, yyval.Node); }
        return;
      case 247: // alt_if_stmt -> alt_if_stmt_without_else T_ELSE ':' inner_statement_list T_ENDIF ';' 
{ RebuildLast(value_stack.array[value_stack.top-6].yyval.IfItemList, value_stack.array[value_stack.top-5].yypos, Tokens.T_ELSE);
			((List<IfStatement>)value_stack.array[value_stack.top-6].yyval.IfItemList).Reverse(); yyval.Node = _astFactory.If(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-1].yypos), null, StatementsToBlock(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-3].yyval.NodeList, Tokens.T_ENDIF), null); 
			foreach (var item in (List<IfStatement>)value_stack.array[value_stack.top-6].yyval.IfItemList) yyval.Node = _astFactory.If(CombineSpans(item.Span, (yyval.Node).Span), item.Condition, item.Body, yyval.Node); }
        return;
      case 248: // parameter_list -> non_empty_parameter_list possible_comma 
{ yyval.FormalParamList = AddTrailingComma(value_stack.array[value_stack.top-2].yyval.FormalParamList, value_stack.array[value_stack.top-1].yyval.Bool); }
        return;
      case 249: // parameter_list -> 
{ yyval.FormalParamList = new List<FormalParam>(); }
        return;
      case 250: // non_empty_parameter_list -> attributed_parameter 
{ yyval.FormalParamList = new List<FormalParam>() { (FormalParam)value_stack.array[value_stack.top-1].yyval.FormalParam }; }
        return;
      case 251: // non_empty_parameter_list -> non_empty_parameter_list ',' attributed_parameter 
{ yyval.FormalParamList = AddToList<FormalParam>(value_stack.array[value_stack.top-3].yyval.FormalParamList, value_stack.array[value_stack.top-1].yyval.FormalParam); }
        return;
      case 252: // attributed_parameter -> attributes parameter 
{ yyval.FormalParam = WithAttributes(value_stack.array[value_stack.top-1].yyval.FormalParam, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 253: // attributed_parameter -> parameter 
{ yyval.FormalParam = value_stack.array[value_stack.top-1].yyval.FormalParam; }
        return;
      case 254: // optional_property_modifiers -> 
{ yyval.Long = 0; /* None */ }
        return;
      case 255: // optional_property_modifiers -> optional_property_modifiers property_modifier 
{ yyval.Long = AddModifier(value_stack.array[value_stack.top-2].yyval.Long, value_stack.array[value_stack.top-1].yyval.Long, value_stack.array[value_stack.top-1].yypos) | (long)PhpMemberAttributes.Constructor; }
        return;
      case 256: // property_modifier -> T_PUBLIC 
{ yyval.Long = (long)PhpMemberAttributes.Public; }
        return;
      case 257: // property_modifier -> T_PROTECTED 
{ yyval.Long = (long)PhpMemberAttributes.Protected; }
        return;
      case 258: // property_modifier -> T_PRIVATE 
{ yyval.Long = (long)PhpMemberAttributes.Private; }
        return;
      case 259: // property_modifier -> T_READONLY 
{ yyval.Long = (long)PhpMemberAttributes.ReadOnly; }
        return;
      case 260: // parameter -> optional_property_modifiers optional_type_without_static is_reference is_variadic T_VARIABLE 
{ yyval.FormalParam = _astFactory.Parameter(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-1].yyval.String, value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-4].yyval.TypeReference, (FormalParam.Flags)value_stack.array[value_stack.top-3].yyval.Long|(FormalParam.Flags)value_stack.array[value_stack.top-2].yyval.Long, null, (PhpMemberAttributes)value_stack.array[value_stack.top-5].yyval.Long); /* Important - @$ is invalid when optional_type is empty */ }
        return;
      case 261: // parameter -> optional_property_modifiers optional_type_without_static is_reference is_variadic T_VARIABLE '=' expr 
{ yyval.FormalParam = _astFactory.Parameter(CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-3].yyval.String, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-6].yyval.TypeReference, (FormalParam.Flags)value_stack.array[value_stack.top-5].yyval.Long|(FormalParam.Flags)value_stack.array[value_stack.top-4].yyval.Long, (Expression)value_stack.array[value_stack.top-1].yyval.Node, (PhpMemberAttributes)value_stack.array[value_stack.top-7].yyval.Long); /* Important - @$ is invalid when optional_type is empty */ }
        return;
      case 262: // optional_type -> 
{ yyval.TypeReference = null; }
        return;
      case 263: // optional_type -> type_expr 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 264: // optional_type_without_static -> 
{ yyval.TypeReference = null; }
        return;
      case 265: // optional_type_without_static -> type_expr_without_static 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 266: // type_expr -> type 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 267: // type_expr -> '?' type 
{ yyval.TypeReference = _astFactory.NullableTypeReference(yypos, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 268: // type_expr -> union_type 
{ yyval.TypeReference = _astFactory.TypeReference(yypos, value_stack.array[value_stack.top-1].yyval.TypeRefList); }
        return;
      case 269: // type_expr -> intersection_type 
{ yyval.TypeReference = _astFactory.IntersectionTypeReference(yypos, value_stack.array[value_stack.top-1].yyval.TypeRefList); }
        return;
      case 270: // type_expr_without_static -> type_without_static 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 271: // type_expr_without_static -> '?' type_without_static 
{ yyval.TypeReference = _astFactory.NullableTypeReference(yypos, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 272: // type_expr_without_static -> union_type_without_static 
{ yyval.TypeReference = _astFactory.TypeReference(yypos, value_stack.array[value_stack.top-1].yyval.TypeRefList); }
        return;
      case 273: // type_expr_without_static -> intersection_type_without_static 
{ yyval.TypeReference = _astFactory.IntersectionTypeReference(yypos, value_stack.array[value_stack.top-1].yyval.TypeRefList); }
        return;
      case 274: // type -> type_without_static 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 275: // type -> T_STATIC 
{ yyval.TypeReference = _astFactory.ReservedTypeReference(yypos, _reservedTypeStatic); }
        return;
      case 276: // type_without_static -> T_ARRAY 
{ yyval.TypeReference = _astFactory.PrimitiveTypeReference(yypos, PrimitiveTypeRef.PrimitiveType.array); }
        return;
      case 277: // type_without_static -> T_CALLABLE 
{ yyval.TypeReference = _astFactory.PrimitiveTypeReference(yypos, PrimitiveTypeRef.PrimitiveType.callable); }
        return;
      case 278: // type_without_static -> name 
{ yyval.TypeReference = CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference, true); }
        return;
      case 279: // union_type -> type '|' type 
{ yyval.TypeRefList = new List<TypeRef>(2){ value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-1].yyval.TypeReference }; }
        return;
      case 280: // union_type -> union_type '|' type 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 281: // union_type_without_static -> type_without_static '|' type_without_static 
{ yyval.TypeRefList = new List<TypeRef>(2){ value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-1].yyval.TypeReference }; }
        return;
      case 282: // union_type_without_static -> union_type_without_static '|' type_without_static 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 283: // intersection_type -> type T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG type 
{ yyval.TypeRefList = new List<TypeRef>(2){ value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-1].yyval.TypeReference }; }
        return;
      case 284: // intersection_type -> intersection_type T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG type 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 285: // intersection_type_without_static -> type_without_static T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG type_without_static 
{ yyval.TypeRefList = new List<TypeRef>(2){ value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-1].yyval.TypeReference }; }
        return;
      case 286: // intersection_type_without_static -> intersection_type_without_static T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG type_without_static 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 287: // return_type -> 
{ yyval.TypeReference = null; }
        return;
      case 288: // return_type -> ':' type_expr 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 289: // argument_list -> '(' ')' 
{ yyval.ParamList = new List<ActualParam>(); }
        return;
      case 290: // argument_list -> '(' non_empty_argument_list possible_comma ')' 
{ yyval.ParamList = AddTrailingComma(value_stack.array[value_stack.top-3].yyval.ParamList, value_stack.array[value_stack.top-2].yyval.Bool); }
        return;
      case 291: // argument_list -> '(' T_ELLIPSIS ')' 
{ yyval.ParamList = CallSignature.CreateCallableConvert(value_stack.array[value_stack.top-2].yypos); }
        return;
      case 292: // non_empty_argument_list -> argument 
{ yyval.ParamList = new List<ActualParam>() { value_stack.array[value_stack.top-1].yyval.Param }; }
        return;
      case 293: // non_empty_argument_list -> non_empty_argument_list ',' argument 
{ yyval.ParamList = AddToList<ActualParam>(value_stack.array[value_stack.top-3].yyval.ParamList, value_stack.array[value_stack.top-1].yyval.Param); }
        return;
      case 294: // argument -> expr 
{ yyval.Param = _astFactory.ActualParameter(yypos, value_stack.array[value_stack.top-1].yyval.Node, ActualParam.Flags.Default); }
        return;
      case 295: // argument -> identifier ':' expr 
{ yyval.Param = _astFactory.ActualParameter(yypos, value_stack.array[value_stack.top-1].yyval.Node, ActualParam.Flags.Default, new VariableNameRef(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-3].yyval.String)); }
        return;
      case 296: // argument -> T_ELLIPSIS expr 
{ yyval.Param = _astFactory.ActualParameter(yypos, value_stack.array[value_stack.top-1].yyval.Node, ActualParam.Flags.IsUnpack); }
        return;
      case 297: // global_var_list -> global_var_list ',' global_var 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 298: // global_var_list -> global_var 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 299: // global_var -> simple_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 300: // static_var_list -> static_var_list ',' static_var 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 301: // static_var_list -> static_var 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 302: // static_var -> T_VARIABLE 
{ yyval.Node = _astFactory.StaticVarDecl(yypos, new VariableName(value_stack.array[value_stack.top-1].yyval.String), null); }
        return;
      case 303: // static_var -> T_VARIABLE '=' expr 
{ yyval.Node = _astFactory.StaticVarDecl(yypos, new VariableName(value_stack.array[value_stack.top-3].yyval.String), value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 304: // class_statement_list -> class_statement_list class_statement 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 305: // class_statement_list -> 
{ yyval.NodeList = new List<LangElement>(); }
        return;
      case 306: // attributed_class_statement -> variable_modifiers optional_type_without_static property_list ';' 
{ 
				yyval.Node = _astFactory.DeclList(yypos, (PhpMemberAttributes)value_stack.array[value_stack.top-4].yyval.Long, value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-3].yyval.TypeReference); 
				SetDoc(yyval.Node);
			}
        return;
      case 307: // attributed_class_statement -> method_modifiers T_CONST class_const_list ';' 
{ 
				yyval.Node = _astFactory.DeclList(yypos, (PhpMemberAttributes)value_stack.array[value_stack.top-4].yyval.Long, value_stack.array[value_stack.top-2].yyval.NodeList, null); 
				SetDoc(yyval.Node);
			}
        return;
      case 308: // attributed_class_statement -> method_modifiers function returns_ref identifier backup_doc_comment '(' parameter_list ')' return_type backup_fn_flags method_body backup_fn_flags 
{	
				yyval.Node = _astFactory.Method(yypos, value_stack.array[value_stack.top-10].yyval.Long == (long)FormalParam.Flags.IsByRef, (PhpMemberAttributes)value_stack.array[value_stack.top-12].yyval.Long, 
					value_stack.array[value_stack.top-4].yyval.TypeReference, value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-9].yyval.String, value_stack.array[value_stack.top-9].yypos, null, value_stack.array[value_stack.top-6].yyval.FormalParamList, CombineSpans(value_stack.array[value_stack.top-7].yypos, value_stack.array[value_stack.top-5].yypos), null, value_stack.array[value_stack.top-2].yyval.Node);
				SetDoc(yyval.Node);
			}
        return;
      case 309: // attributed_class_statement -> enum_case 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 310: // class_statement -> attributed_class_statement 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 311: // class_statement -> attributes attributed_class_statement 
{ yyval.Node = WithAttributes(value_stack.array[value_stack.top-1].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 312: // class_statement -> T_USE name_list trait_adaptations 
{ yyval.Node = _astFactory.TraitUse(yypos, value_stack.array[value_stack.top-2].yyval.TypeRefList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 313: // name_list -> name 
{ yyval.TypeRefList = new List<TypeRef>() { CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference) }; }
        return;
      case 314: // name_list -> name_list ',' name 
{ yyval.TypeRefList = AddToList<TypeRef>(value_stack.array[value_stack.top-3].yyval.TypeRefList, CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference)); }
        return;
      case 315: // trait_adaptations -> ';' 
{ yyval.Node = null; }
        return;
      case 316: // trait_adaptations -> '{' '}' 
{ yyval.Node = _astFactory.TraitAdaptationBlock(yypos, new List<LangElement>()); }
        return;
      case 317: // trait_adaptations -> '{' trait_adaptation_list '}' 
{ yyval.Node = _astFactory.TraitAdaptationBlock(yypos, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 318: // trait_adaptation_list -> trait_adaptation 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node };
 }
        return;
      case 319: // trait_adaptation_list -> trait_adaptation_list trait_adaptation 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 320: // trait_adaptation -> trait_precedence 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 321: // trait_adaptation -> trait_alias 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 322: // trait_precedence -> absolute_trait_method_reference T_INSTEADOF name_list ';' 
{ yyval.Node = _astFactory.TraitAdaptationPrecedence(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-4].yyval.Object, value_stack.array[value_stack.top-2].yyval.TypeRefList); }
        return;
      case 323: // trait_alias -> trait_method_reference T_AS T_STRING ';' 
{ yyval.Node = _astFactory.TraitAdaptationAlias(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-4].yyval.Object, new NameRef(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.String), null); }
        return;
      case 324: // trait_alias -> trait_method_reference T_AS reserved_non_modifiers ';' 
{ yyval.Node = _astFactory.TraitAdaptationAlias(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-4].yyval.Object, new NameRef(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.String), null); }
        return;
      case 325: // trait_alias -> trait_method_reference T_AS member_modifier identifier ';' 
{ 
				yyval.Node = _astFactory.TraitAdaptationAlias(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-5].yyval.Object, new NameRef(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.String), (PhpMemberAttributes)value_stack.array[value_stack.top-3].yyval.Long); 
			}
        return;
      case 326: // trait_alias -> trait_method_reference T_AS member_modifier ';' 
{ 
				yyval.Node = _astFactory.TraitAdaptationAlias(yypos, (Tuple<TypeRef,NameRef>)value_stack.array[value_stack.top-4].yyval.Object, NameRef.Invalid, (PhpMemberAttributes)value_stack.array[value_stack.top-2].yyval.Long); 
			}
        return;
      case 327: // trait_method_reference -> identifier 
{ yyval.Object = new Tuple<TypeRef,NameRef>(null, new NameRef(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.String)); }
        return;
      case 328: // trait_method_reference -> absolute_trait_method_reference 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.Object; }
        return;
      case 329: // absolute_trait_method_reference -> name T_DOUBLE_COLON identifier 
{ yyval.Object = new Tuple<TypeRef,NameRef>(CreateTypeRef(value_stack.array[value_stack.top-3].yyval.QualifiedNameReference), new NameRef(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.String)); }
        return;
      case 330: // method_body -> ';' 
{ yyval.Node = null; }
        return;
      case 331: // method_body -> '{' inner_statement_list '}' 
{ yyval.Node = CreateBlock(CombineSpans(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 332: // variable_modifiers -> non_empty_member_modifiers 
{ yyval.Long = value_stack.array[value_stack.top-1].yyval.Long; }
        return;
      case 333: // variable_modifiers -> T_VAR 
{ yyval.Long = (long)PhpMemberAttributes.None; }
        return;
      case 334: // method_modifiers -> 
{ yyval.Long = (long)PhpMemberAttributes.None; }
        return;
      case 335: // method_modifiers -> non_empty_member_modifiers 
{ yyval.Long = value_stack.array[value_stack.top-1].yyval.Long; }
        return;
      case 336: // non_empty_member_modifiers -> member_modifier 
{ yyval.Long = value_stack.array[value_stack.top-1].yyval.Long; }
        return;
      case 337: // non_empty_member_modifiers -> non_empty_member_modifiers member_modifier 
{ yyval.Long = AddModifier(value_stack.array[value_stack.top-2].yyval.Long, value_stack.array[value_stack.top-1].yyval.Long, value_stack.array[value_stack.top-1].yypos); }
        return;
      case 338: // member_modifier -> T_PUBLIC 
{ yyval.Long = (long)PhpMemberAttributes.Public; }
        return;
      case 339: // member_modifier -> T_PROTECTED 
{ yyval.Long = (long)PhpMemberAttributes.Protected; }
        return;
      case 340: // member_modifier -> T_PRIVATE 
{ yyval.Long = (long)PhpMemberAttributes.Private; }
        return;
      case 341: // member_modifier -> T_STATIC 
{ yyval.Long = (long)PhpMemberAttributes.Static; }
        return;
      case 342: // member_modifier -> T_ABSTRACT 
{ yyval.Long = (long)PhpMemberAttributes.Abstract; }
        return;
      case 343: // member_modifier -> T_FINAL 
{ yyval.Long = (long)PhpMemberAttributes.Final; }
        return;
      case 344: // member_modifier -> T_READONLY 
{ yyval.Long = (long)PhpMemberAttributes.ReadOnly; }
        return;
      case 345: // property_list -> property_list ',' property 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 346: // property_list -> property 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 347: // property -> T_VARIABLE backup_doc_comment 
{ SetMemberDoc(yyval.Node = _astFactory.FieldDecl(yypos, new VariableName(value_stack.array[value_stack.top-2].yyval.String), null)); }
        return;
      case 348: // property -> T_VARIABLE '=' expr backup_doc_comment 
{ SetMemberDoc(yyval.Node = _astFactory.FieldDecl(yypos, new VariableName(value_stack.array[value_stack.top-4].yyval.String), (Expression)value_stack.array[value_stack.top-2].yyval.Node)); }
        return;
      case 349: // class_const_list -> class_const_list ',' class_const_decl 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 350: // class_const_list -> class_const_decl 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 351: // class_const_decl -> identifier '=' expr backup_doc_comment 
{
		yyval.Node = _astFactory.ClassConstDecl(yypos, new VariableName(value_stack.array[value_stack.top-4].yyval.String), value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yyval.Node); 
		SetMemberDoc(yyval.Node);
	}
        return;
      case 352: // const_decl -> T_STRING '=' expr backup_doc_comment 
{ yyval.Node = _astFactory.GlobalConstDecl(yypos, false, new VariableName(value_stack.array[value_stack.top-4].yyval.String), value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yyval.Node); 
		SetMemberDoc(yyval.Node);
	}
        return;
      case 353: // echo_expr_list -> echo_expr_list ',' echo_expr 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 354: // echo_expr_list -> echo_expr 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 355: // echo_expr -> expr 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 356: // for_exprs -> 
{ yyval.NodeList = LangElement.EmptyList; }
        return;
      case 357: // for_exprs -> non_empty_for_exprs 
{ yyval.NodeList = value_stack.array[value_stack.top-1].yyval.NodeList; }
        return;
      case 358: // non_empty_for_exprs -> non_empty_for_exprs ',' expr 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 359: // non_empty_for_exprs -> expr 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 360: // @8 -> 
{ PushAnonymousClassContext(value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 361: // anonymous_class -> T_CLASS ctor_arguments extends_from @8 implements_list backup_doc_comment enter_scope '{' class_statement_list '}' exit_scope 
{
			var typeRef = _astFactory.AnonymousTypeReference(yypos, CombineSpans(value_stack.array[value_stack.top-11].yypos, value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-7].yypos), isConditional, PhpMemberAttributes.None, null, ConvertToNamedTypeRef(value_stack.array[value_stack.top-9].yyval.TypeReference), value_stack.array[value_stack.top-7].yyval.TypeRefList.Select(ConvertToNamedTypeRef), value_stack.array[value_stack.top-3].yyval.NodeList, CombineSpans(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-2].yypos));
			SetDoc(((AnonymousTypeRef)typeRef).TypeDeclaration);
			yyval.AnonymousClass = new AnonymousClass(typeRef, value_stack.array[value_stack.top-10].yyval.ParamList, value_stack.array[value_stack.top-10].yypos); 
			PopClassContext();
		}
        return;
      case 362: // new_expr -> T_NEW class_name_reference ctor_arguments 
{ yyval.Node = _astFactory.New(yypos, value_stack.array[value_stack.top-2].yyval.TypeReference, value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos); }
        return;
      case 363: // new_expr -> T_NEW anonymous_class 
{ yyval.Node = _astFactory.New(yypos, ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item1, ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item2, ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item3); }
        return;
      case 364: // new_expr -> T_NEW attributes anonymous_class 
{ yyval.Node = _astFactory.New(yypos, WithAttributes(((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item1, value_stack.array[value_stack.top-2].yyval.NodeList), ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item2, ((AnonymousClass)value_stack.array[value_stack.top-1].yyval.AnonymousClass).Item3); }
        return;
      case 365: // expr_without_variable -> T_LIST '(' array_pair_list ')' '=' expr 
{ yyval.Node = _astFactory.Assignment(yypos, _astFactory.List(Span.Combine(value_stack.array[value_stack.top-6].yypos, value_stack.array[value_stack.top-3].yypos), value_stack.array[value_stack.top-4].yyval.ItemList, true), value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignValue); }
        return;
      case 366: // expr_without_variable -> '[' array_pair_list ']' '=' expr 
{ yyval.Node = _astFactory.Assignment(yypos, _astFactory.List(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-3].yypos), value_stack.array[value_stack.top-4].yyval.ItemList, false), value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignValue); }
        return;
      case 367: // expr_without_variable -> variable '=' expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignValue); }
        return;
      case 368: // expr_without_variable -> variable '=' ampersand variable 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignRef); }
        return;
      case 369: // expr_without_variable -> variable '=' ampersand new_expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignRef); _errors.Error(yypos, Warnings.AssignNewByRefDeprecated); }
        return;
      case 370: // expr_without_variable -> T_CLONE expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Clone,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 371: // expr_without_variable -> variable T_PLUS_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignAdd); }
        return;
      case 372: // expr_without_variable -> variable T_MINUS_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignSub); }
        return;
      case 373: // expr_without_variable -> variable T_MUL_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignMul); }
        return;
      case 374: // expr_without_variable -> variable T_POW_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignPow); }
        return;
      case 375: // expr_without_variable -> variable T_DIV_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignDiv); }
        return;
      case 376: // expr_without_variable -> variable T_CONCAT_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignAppend); }
        return;
      case 377: // expr_without_variable -> variable T_MOD_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignMod); }
        return;
      case 378: // expr_without_variable -> variable T_AND_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignAnd); }
        return;
      case 379: // expr_without_variable -> variable T_OR_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignOr); }
        return;
      case 380: // expr_without_variable -> variable T_XOR_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignXor); }
        return;
      case 381: // expr_without_variable -> variable T_SL_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignShiftLeft); }
        return;
      case 382: // expr_without_variable -> variable T_SR_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignShiftRight); }
        return;
      case 383: // expr_without_variable -> variable T_COALESCE_EQUAL expr 
{ yyval.Node = _astFactory.Assignment(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node, Operations.AssignCoalesce); }
        return;
      case 384: // expr_without_variable -> variable T_INC 
{ yyval.Node = CreateIncDec(yypos, value_stack.array[value_stack.top-2].yyval.Node, true, true); }
        return;
      case 385: // expr_without_variable -> T_INC variable 
{ yyval.Node = CreateIncDec(yypos, value_stack.array[value_stack.top-1].yyval.Node, true, false); }
        return;
      case 386: // expr_without_variable -> variable T_DEC 
{ yyval.Node = CreateIncDec(yypos, value_stack.array[value_stack.top-2].yyval.Node, false, true); }
        return;
      case 387: // expr_without_variable -> T_DEC variable 
{ yyval.Node = CreateIncDec(yypos, value_stack.array[value_stack.top-1].yyval.Node, false, false); }
        return;
      case 388: // expr_without_variable -> expr T_BOOLEAN_OR expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Or,   value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 389: // expr_without_variable -> expr T_BOOLEAN_AND expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.And,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 390: // expr_without_variable -> expr T_LOGICAL_OR expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Or,   value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 391: // expr_without_variable -> expr T_LOGICAL_AND expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.And,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 392: // expr_without_variable -> expr T_LOGICAL_XOR expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Xor,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 393: // expr_without_variable -> expr '|' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.BitOr,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 394: // expr_without_variable -> expr T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.BitAnd, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 395: // expr_without_variable -> expr T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.BitAnd, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 396: // expr_without_variable -> expr '^' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.BitXor, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 397: // expr_without_variable -> expr '.' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Concat, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 398: // expr_without_variable -> expr '+' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Add,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 399: // expr_without_variable -> expr '-' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Sub,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 400: // expr_without_variable -> expr '*' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Mul,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 401: // expr_without_variable -> expr T_POW expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Pow,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 402: // expr_without_variable -> expr '/' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Div,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 403: // expr_without_variable -> expr '%' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Mod,    value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 404: // expr_without_variable -> expr T_SL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.ShiftLeft,  value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 405: // expr_without_variable -> expr T_SR expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.ShiftRight, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 406: // expr_without_variable -> '+' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Plus,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 407: // expr_without_variable -> '-' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Minus,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 408: // expr_without_variable -> '!' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.LogicNegation, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 409: // expr_without_variable -> '~' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.BitNegation,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 410: // expr_without_variable -> expr T_IS_IDENTICAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Identical, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 411: // expr_without_variable -> expr T_IS_NOT_IDENTICAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.NotIdentical, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 412: // expr_without_variable -> expr T_IS_EQUAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Equal, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 413: // expr_without_variable -> expr T_IS_NOT_EQUAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.NotEqual, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 414: // expr_without_variable -> expr '<' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.LessThan, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 415: // expr_without_variable -> expr T_IS_SMALLER_OR_EQUAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.LessThanOrEqual, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 416: // expr_without_variable -> expr '>' expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.GreaterThan, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 417: // expr_without_variable -> expr T_IS_GREATER_OR_EQUAL expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.GreaterThanOrEqual, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 418: // expr_without_variable -> expr T_SPACESHIP expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Spaceship, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 419: // expr_without_variable -> expr T_INSTANCEOF class_name_reference 
{ yyval.Node = _astFactory.InstanceOf(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.TypeReference); }
        return;
      case 420: // expr_without_variable -> '(' expr ')' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN); }
        return;
      case 421: // expr_without_variable -> new_expr 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 422: // expr_without_variable -> expr '?' expr ':' expr 
{ yyval.Node = _astFactory.ConditionalEx(yypos, value_stack.array[value_stack.top-5].yyval.Node, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 423: // expr_without_variable -> expr '?' ':' expr 
{ yyval.Node = _astFactory.ConditionalEx(yypos, value_stack.array[value_stack.top-4].yyval.Node, null, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 424: // expr_without_variable -> expr T_COALESCE expr 
{ yyval.Node = _astFactory.BinaryOperation(yypos, Operations.Coalesce, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 425: // expr_without_variable -> internal_functions_in_yacc 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 426: // expr_without_variable -> T_INT_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Int64Cast,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 427: // expr_without_variable -> T_DOUBLE_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.DoubleCast, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 428: // expr_without_variable -> T_STRING_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.StringCast, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 429: // expr_without_variable -> T_ARRAY_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.ArrayCast,  (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 430: // expr_without_variable -> T_OBJECT_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.ObjectCast, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 431: // expr_without_variable -> T_BOOL_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.BoolCast,   (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 432: // expr_without_variable -> T_UNSET_CAST expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.UnsetCast,  (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 433: // expr_without_variable -> T_EXIT exit_expr 
{ yyval.Node = _astFactory.Exit(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 434: // expr_without_variable -> '@' expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.AtSign,     (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 435: // expr_without_variable -> scalar 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 436: // expr_without_variable -> backticks_expr 
{ yyval.Node = _astFactory.Shell(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 437: // expr_without_variable -> T_PRINT expr 
{ yyval.Node = _astFactory.UnaryOperation(yypos, Operations.Print, (Expression)value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 438: // expr_without_variable -> T_YIELD 
{ yyval.Node = _astFactory.Yield(yypos, null, null); }
        return;
      case 439: // expr_without_variable -> T_YIELD expr 
{ yyval.Node = _astFactory.Yield(yypos, null, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 440: // expr_without_variable -> T_YIELD expr T_DOUBLE_ARROW expr 
{ yyval.Node = _astFactory.Yield(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 441: // expr_without_variable -> T_YIELD_FROM expr 
{ yyval.Node = _astFactory.YieldFrom(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 442: // expr_without_variable -> T_THROW expr 
{ yyval.Node = _astFactory.Throw(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 443: // expr_without_variable -> inline_function 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 444: // expr_without_variable -> attributes inline_function 
{ yyval.Node = WithAttributes(value_stack.array[value_stack.top-1].yyval.Node, value_stack.array[value_stack.top-2].yyval.NodeList); }
        return;
      case 445: // expr_without_variable -> T_STATIC inline_function 
{
			var lambda = (LambdaFunctionExpr)value_stack.array[value_stack.top-1].yyval.Node;
			lambda.IsStatic = true;
			lambda.Span = yypos;
			yyval.Node = lambda;
		}
        return;
      case 446: // expr_without_variable -> attributes T_STATIC inline_function 
{
			var lambda = (LambdaFunctionExpr)value_stack.array[value_stack.top-1].yyval.Node;
			lambda.IsStatic = true;
			lambda.Span = yypos;
			yyval.Node = WithAttributes(lambda, value_stack.array[value_stack.top-3].yyval.NodeList);
		}
        return;
      case 447: // expr_without_variable -> match 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 448: // inline_function -> function returns_ref backup_doc_comment '(' parameter_list ')' lexical_vars return_type backup_fn_flags enter_scope '{' inner_statement_list '}' exit_scope backup_fn_flags 
{ 
				yyval.Node = _astFactory.Lambda(yypos, CombineSpans(value_stack.array[value_stack.top-15].yypos, value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-9].yypos, value_stack.array[value_stack.top-8].yypos), value_stack.array[value_stack.top-14].yyval.Long == (long)FormalParam.Flags.IsByRef, value_stack.array[value_stack.top-8].yyval.TypeReference, 
					value_stack.array[value_stack.top-11].yyval.FormalParamList, CombineSpans(value_stack.array[value_stack.top-12].yypos, value_stack.array[value_stack.top-10].yypos), value_stack.array[value_stack.top-9].yyval.FormalParamList, CreateBlock(CombineSpans(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-3].yypos), value_stack.array[value_stack.top-4].yyval.NodeList)); 
				SetDoc(yyval.Node);
			}
        return;
      case 449: // inline_function -> fn returns_ref '(' parameter_list ')' return_type backup_doc_comment T_DOUBLE_ARROW backup_fn_flags backup_lex_pos expr backup_fn_flags 
{
				yyval.Node = _astFactory.ArrowFunc(yypos, CombineSpans(value_stack.array[value_stack.top-12].yypos, value_stack.array[value_stack.top-8].yypos, value_stack.array[value_stack.top-7].yypos), value_stack.array[value_stack.top-11].yyval.Long == (long)FormalParam.Flags.IsByRef, value_stack.array[value_stack.top-7].yyval.TypeReference, 
					value_stack.array[value_stack.top-9].yyval.FormalParamList, CombineSpans(value_stack.array[value_stack.top-10].yypos, value_stack.array[value_stack.top-8].yypos), value_stack.array[value_stack.top-2].yyval.Node); 
				SetDoc(yyval.Node);
			}
        return;
      case 452: // backup_doc_comment -> 
{ }
        return;
      case 453: // enter_scope -> 
{ _currentScope.Increment(); }
        return;
      case 454: // exit_scope -> 
{ _currentScope.Decrement(); }
        return;
      case 455: // backup_fn_flags -> 
{  }
        return;
      case 456: // backup_lex_pos -> 
{  }
        return;
      case 457: // returns_ref -> 
{ yyval.Long = 0; }
        return;
      case 458: // returns_ref -> ampersand 
{ yyval.Long = (long)FormalParam.Flags.IsByRef; }
        return;
      case 459: // lexical_vars -> 
{ yyval.FormalParamList = new List<FormalParam>(); }
        return;
      case 460: // lexical_vars -> T_USE '(' lexical_var_list ')' 
{ yyval.FormalParamList = value_stack.array[value_stack.top-2].yyval.FormalParamList; }
        return;
      case 461: // lexical_var_list -> lexical_var_list ',' lexical_var 
{ yyval.FormalParamList = AddToList<FormalParam>(value_stack.array[value_stack.top-3].yyval.FormalParamList, value_stack.array[value_stack.top-1].yyval.FormalParam); }
        return;
      case 462: // lexical_var_list -> lexical_var 
{ yyval.FormalParamList = new List<FormalParam>() { (FormalParam)value_stack.array[value_stack.top-1].yyval.FormalParam }; }
        return;
      case 463: // lexical_var -> T_VARIABLE 
{ yyval.FormalParam = _astFactory.Parameter(yypos, value_stack.array[value_stack.top-1].yyval.String, value_stack.array[value_stack.top-1].yypos, null, FormalParam.Flags.Default); }
        return;
      case 464: // lexical_var -> ampersand T_VARIABLE 
{ yyval.FormalParam = _astFactory.Parameter(yypos, value_stack.array[value_stack.top-1].yyval.String, value_stack.array[value_stack.top-1].yypos, null, FormalParam.Flags.IsByRef); }
        return;
      case 465: // function_call -> name argument_list 
{ yyval.Node = _astFactory.Call(yypos, TranslateQNRFunction(value_stack.array[value_stack.top-2].yyval.QualifiedNameReference), new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), null); }
        return;
      case 466: // function_call -> class_name T_DOUBLE_COLON member_name argument_list 
{
				if (value_stack.array[value_stack.top-2].yyval.Object is string namestr)
					yyval.Node = _astFactory.Call(yypos, new Name(namestr), value_stack.array[value_stack.top-2].yypos, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-4].yyval.TypeReference); 
				else
					yyval.Node = _astFactory.Call(yypos, (LangElement)value_stack.array[value_stack.top-2].yyval.Object, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-4].yyval.TypeReference); 
			}
        return;
      case 467: // function_call -> variable_class_name T_DOUBLE_COLON member_name argument_list 
{
				if (value_stack.array[value_stack.top-2].yyval.Object is string namestr)
					yyval.Node = _astFactory.Call(yypos, new Name(namestr), value_stack.array[value_stack.top-2].yypos, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), _astFactory.TypeReference(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-4].yyval.Node)); 
				else
					yyval.Node = _astFactory.Call(yypos, (LangElement)value_stack.array[value_stack.top-2].yyval.Object, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), _astFactory.TypeReference(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-4].yyval.Node)); 
			}
        return;
      case 468: // function_call -> callable_expr argument_list 
{ yyval.Node = _astFactory.Call(yypos, value_stack.array[value_stack.top-2].yyval.Node, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), NullLangElement);}
        return;
      case 469: // class_name -> T_STATIC 
{ yyval.TypeReference = _astFactory.ReservedTypeReference(yypos, _reservedTypeStatic); }
        return;
      case 470: // class_name -> name 
{ yyval.TypeReference = CreateTypeRef(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference); }
        return;
      case 471: // class_name_reference -> class_name 
{ yyval.TypeReference = value_stack.array[value_stack.top-1].yyval.TypeReference; }
        return;
      case 472: // class_name_reference -> new_variable 
{ yyval.TypeReference = _astFactory.TypeReference(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 473: // class_name_reference -> '(' expr ')' 
{ yyval.TypeReference = _astFactory.TypeReference(yypos, _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN)); }
        return;
      case 474: // exit_expr -> 
{ yyval.Node = null; }
        return;
      case 475: // exit_expr -> '(' optional_expr ')' 
{ yyval.Node = value_stack.array[value_stack.top-2].yyval.Node == null? null: _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN); }
        return;
      case 476: // backticks_expr -> '`' '`' 
{ yyval.Node = _astFactory.Literal(yypos, string.Empty, "``"); }
        return;
      case 477: // backticks_expr -> '`' T_ENCAPSED_AND_WHITESPACE '`' 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-2].yyval.Strings.Key, string.Format("`{0}`", value_stack.array[value_stack.top-2].yyval.Strings.Value)); }
        return;
      case 478: // backticks_expr -> '`' encaps_list '`' 
{ yyval.Node = _astFactory.StringEncapsedExpression(yypos, _astFactory.Concat(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.NodeList), Tokens.T_BACKQUOTE); }
        return;
      case 479: // ctor_arguments -> 
{ yyval.ParamList = new List<ActualParam>(); }
        return;
      case 480: // ctor_arguments -> argument_list 
{ yyval.ParamList = value_stack.array[value_stack.top-1].yyval.ParamList; }
        return;
      case 481: // dereferencable_scalar -> T_ARRAY '(' array_pair_list ')' 
{ yyval.Node = _astFactory.NewArray(yypos, value_stack.array[value_stack.top-2].yyval.ItemList, true); }
        return;
      case 482: // dereferencable_scalar -> '[' array_pair_list ']' 
{ yyval.Node = _astFactory.NewArray(yypos, value_stack.array[value_stack.top-2].yyval.ItemList, false); }
        return;
      case 483: // dereferencable_scalar -> T_CONSTANT_ENCAPSED_STRING 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.Object, _lexer.TokenText); }
        return;
      case 484: // scalar -> T_LNUMBER 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.Long, _lexer.TokenText); }
        return;
      case 485: // scalar -> T_DNUMBER 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.Double, _lexer.TokenText); }
        return;
      case 486: // scalar -> T_LINE 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Line); }
        return;
      case 487: // scalar -> T_FILE 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.File); }
        return;
      case 488: // scalar -> T_DIR 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Dir); }
        return;
      case 489: // scalar -> T_TRAIT_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Trait); }
        return;
      case 490: // scalar -> T_METHOD_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Method); }
        return;
      case 491: // scalar -> T_FUNC_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Function); }
        return;
      case 492: // scalar -> T_NS_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Namespace); }
        return;
      case 493: // scalar -> T_CLASS_C 
{ yyval.Node = _astFactory.PseudoConstUse(yypos, PseudoConstUse.Types.Class); }
        return;
      case 494: // scalar -> '"' encaps_list '"' 
{ yyval.Node = _astFactory.StringEncapsedExpression(yypos, _astFactory.Concat(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.NodeList), Tokens.T_DOUBLE_QUOTES); }
        return;
      case 495: // scalar -> T_START_HEREDOC T_END_HEREDOC 
{ yyval.Node = _astFactory.HeredocExpression(yypos, _astFactory.Literal(new Span(value_stack.array[value_stack.top-2].yypos.End, 0), "", ""), value_stack.array[value_stack.top-2].yyval.QuoteToken, value_stack.array[value_stack.top-1].yyval.HereDocValue); }
        return;
      case 496: // scalar -> T_START_HEREDOC T_ENCAPSED_AND_WHITESPACE T_END_HEREDOC 
{ yyval.Node = _astFactory.HeredocExpression(yypos, RemoveHereDocIndentation(_astFactory.Literal(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.Strings.Key, value_stack.array[value_stack.top-2].yyval.Strings.Value), value_stack.array[value_stack.top-1].yyval.HereDocValue, true), value_stack.array[value_stack.top-3].yyval.QuoteToken, value_stack.array[value_stack.top-1].yyval.HereDocValue); }
        return;
      case 497: // scalar -> T_START_HEREDOC encaps_list T_END_HEREDOC 
{ yyval.Node = _astFactory.HeredocExpression(yypos, RemoveHereDocIndentation(_astFactory.Concat(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.NodeList), value_stack.array[value_stack.top-1].yyval.HereDocValue, true), value_stack.array[value_stack.top-3].yyval.QuoteToken, value_stack.array[value_stack.top-1].yyval.HereDocValue); }
        return;
      case 498: // scalar -> dereferencable_scalar 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 499: // scalar -> constant 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 500: // scalar -> class_constant 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 501: // constant -> name 
{ yyval.Node = _astFactory.ConstUse(yypos, TranslateQNRConstant(value_stack.array[value_stack.top-1].yyval.QualifiedNameReference)); }
        return;
      case 502: // class_constant -> class_name T_DOUBLE_COLON identifier 
{ yyval.Node = _astFactory.ClassConstUse(yypos, value_stack.array[value_stack.top-3].yyval.TypeReference, new Name(value_stack.array[value_stack.top-1].yyval.String), value_stack.array[value_stack.top-1].yypos); }
        return;
      case 503: // class_constant -> variable_class_name T_DOUBLE_COLON identifier 
{ yyval.Node = _astFactory.ClassConstUse(yypos, _astFactory.TypeReference(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-3].yyval.Node), new Name(value_stack.array[value_stack.top-1].yyval.String), value_stack.array[value_stack.top-1].yypos); }
        return;
      case 504: // expr -> variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 505: // expr -> expr_without_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 506: // optional_expr -> 
{ yyval.Node = null; }
        return;
      case 507: // optional_expr -> expr 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 508: // object_operator -> T_OBJECT_OPERATOR 
{ yyval.Token = Tokens.T_OBJECT_OPERATOR; }
        return;
      case 509: // object_operator -> T_NULLSAFE_OBJECT_OPERATOR 
{ yyval.Token = Tokens.T_NULLSAFE_OBJECT_OPERATOR; }
        return;
      case 510: // variable_class_name -> dereferencable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; /* TODO if (!($1 is VarLikeConstructUse)) _errors.Error(@$, FatalErrors.CheckVarUseFault); */ }
        return;
      case 511: // dereferencable -> variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 512: // dereferencable -> '(' expr ')' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN); }
        return;
      case 513: // dereferencable -> dereferencable_scalar 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 514: // dereferencable -> class_constant 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 515: // array_object_dereferenceable -> dereferencable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 516: // array_object_dereferenceable -> constant 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 517: // callable_expr -> callable_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 518: // callable_expr -> '(' expr ')' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LPAREN); }
        return;
      case 519: // callable_expr -> dereferencable_scalar 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 520: // callable_variable -> simple_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 521: // callable_variable -> array_object_dereferenceable '[' optional_expr ']' 
{ yyval.Node = _astFactory.ArrayItem(yypos, false, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 522: // callable_variable -> array_object_dereferenceable '{' expr '}' 
{ yyval.Node = _astFactory.ArrayItem(yypos, true, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 523: // callable_variable -> array_object_dereferenceable object_operator property_name argument_list 
{
			if (value_stack.array[value_stack.top-2].yyval.Object is string name)
				yyval.Node = _astFactory.Call(yypos, new TranslatedQualifiedName(new QualifiedName(new Name(name)), value_stack.array[value_stack.top-2].yypos), new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), VerifyMemberOf(value_stack.array[value_stack.top-4].yyval.Node));
			else
				yyval.Node = _astFactory.Call(yypos, (LangElement)value_stack.array[value_stack.top-2].yyval.Object, new CallSignature(value_stack.array[value_stack.top-1].yyval.ParamList, value_stack.array[value_stack.top-1].yypos), VerifyMemberOf(value_stack.array[value_stack.top-4].yyval.Node));

			AdjustNullSafeOperator(yyval.Node, value_stack.array[value_stack.top-3].yyval.Token);
		}
        return;
      case 524: // callable_variable -> function_call 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 525: // variable -> callable_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 526: // variable -> static_member 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 527: // variable -> array_object_dereferenceable object_operator property_name 
{
			yyval.Node = AdjustNullSafeOperator(CreateProperty(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Object), value_stack.array[value_stack.top-2].yyval.Token);
		}
        return;
      case 528: // simple_variable -> T_VARIABLE 
{ yyval.Node = _astFactory.Variable(yypos, value_stack.array[value_stack.top-1].yyval.String,	NullLangElement, true); }
        return;
      case 529: // simple_variable -> '$' '{' expr '}' 
{ yyval.Node = _astFactory.Variable(yypos, _astFactory.EncapsedExpression(Span.Combine(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LBRACE), NullLangElement); }
        return;
      case 530: // simple_variable -> '$' simple_variable 
{ yyval.Node = _astFactory.Variable(yypos, value_stack.array[value_stack.top-1].yyval.Node, NullLangElement); }
        return;
      case 531: // static_member -> class_name T_DOUBLE_COLON simple_variable 
{ yyval.Node = CreateStaticProperty(yypos, value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 532: // static_member -> variable_class_name T_DOUBLE_COLON simple_variable 
{ yyval.Node = CreateStaticProperty(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 533: // new_variable -> simple_variable 
{ yyval.Node = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 534: // new_variable -> new_variable '[' optional_expr ']' 
{ yyval.Node = _astFactory.ArrayItem(yypos, false, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 535: // new_variable -> new_variable '{' expr '}' 
{ yyval.Node = _astFactory.ArrayItem(yypos, true, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 536: // new_variable -> new_variable object_operator property_name 
{ yyval.Node = AdjustNullSafeOperator(CreateProperty(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Object), value_stack.array[value_stack.top-2].yyval.Token); }
        return;
      case 537: // new_variable -> class_name T_DOUBLE_COLON simple_variable 
{ yyval.Node = CreateStaticProperty(yypos, value_stack.array[value_stack.top-3].yyval.TypeReference, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 538: // new_variable -> new_variable T_DOUBLE_COLON simple_variable 
{ yyval.Node = CreateStaticProperty(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 539: // member_name -> identifier 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.String; }
        return;
      case 540: // member_name -> '{' expr '}' 
{ yyval.Object = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LBRACE); }
        return;
      case 541: // member_name -> simple_variable 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 542: // property_name -> T_STRING 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.String; }
        return;
      case 543: // property_name -> '{' expr '}' 
{ yyval.Object = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LBRACE); }
        return;
      case 544: // property_name -> simple_variable 
{ yyval.Object = value_stack.array[value_stack.top-1].yyval.Node; }
        return;
      case 545: // array_pair_list -> non_empty_array_pair_list 
{ yyval.ItemList = value_stack.array[value_stack.top-1].yyval.ItemList;  }
        return;
      case 546: // possible_array_pair -> 
{ yyval.Item = null; }
        return;
      case 547: // possible_array_pair -> array_pair 
{ yyval.Item = value_stack.array[value_stack.top-1].yyval.Item; }
        return;
      case 548: // non_empty_array_pair_list -> non_empty_array_pair_list ',' possible_array_pair 
{ yyval.ItemList = AddToList<Item>(value_stack.array[value_stack.top-3].yyval.ItemList, value_stack.array[value_stack.top-1].yyval.Item); }
        return;
      case 549: // non_empty_array_pair_list -> possible_array_pair 
{ yyval.ItemList = new List<Item>() { value_stack.array[value_stack.top-1].yyval.Item }; }
        return;
      case 550: // array_pair -> expr T_DOUBLE_ARROW expr 
{ yyval.Item = _astFactory.ArrayItemValue(yypos, value_stack.array[value_stack.top-3].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 551: // array_pair -> expr 
{ yyval.Item = _astFactory.ArrayItemValue(yypos, null, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 552: // array_pair -> expr T_DOUBLE_ARROW ampersand variable 
{ yyval.Item = _astFactory.ArrayItemRef(yypos, value_stack.array[value_stack.top-4].yyval.Node, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 553: // array_pair -> ampersand variable 
{ yyval.Item = _astFactory.ArrayItemRef(yypos, null, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 554: // array_pair -> T_ELLIPSIS expr 
{ yyval.Item = _astFactory.ArrayItemSpread(yypos, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 555: // array_pair -> expr T_DOUBLE_ARROW T_LIST '(' array_pair_list ')' 
{ yyval.Item = _astFactory.ArrayItemValue(yypos, value_stack.array[value_stack.top-6].yyval.Node, _astFactory.List(Span.Combine(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.ItemList, true)); }
        return;
      case 556: // array_pair -> T_LIST '(' array_pair_list ')' 
{ yyval.Item = _astFactory.ArrayItemValue(yypos, null, _astFactory.List(Span.Combine(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-1].yypos), value_stack.array[value_stack.top-2].yyval.ItemList, true)); }
        return;
      case 557: // encaps_list -> encaps_list encaps_var 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 558: // encaps_list -> encaps_list T_ENCAPSED_AND_WHITESPACE 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-2].yyval.NodeList, _astFactory.Literal(value_stack.array[value_stack.top-1].yypos, value_stack.array[value_stack.top-1].yyval.Strings.Key, _lexer.TokenText)); }
        return;
      case 559: // encaps_list -> encaps_var 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 560: // encaps_list -> T_ENCAPSED_AND_WHITESPACE encaps_var 
{ yyval.NodeList = new List<LangElement>() { _astFactory.Literal(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.Strings.Key, value_stack.array[value_stack.top-2].yyval.Strings.Value), value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 561: // encaps_var -> T_VARIABLE 
{ yyval.Node = _astFactory.Variable(yypos, value_stack.array[value_stack.top-1].yyval.String, NullLangElement, true); }
        return;
      case 562: // encaps_var -> T_VARIABLE '[' encaps_var_offset ']' 
{ yyval.Node = _astFactory.ArrayItem(yypos, false,
					_astFactory.Variable(value_stack.array[value_stack.top-4].yypos, value_stack.array[value_stack.top-4].yyval.String, NullLangElement, true), value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 563: // encaps_var -> T_VARIABLE object_operator T_STRING 
{ yyval.Node = AdjustNullSafeOperator(CreateProperty(yypos, _astFactory.Variable(value_stack.array[value_stack.top-3].yypos, value_stack.array[value_stack.top-3].yyval.String, NullLangElement, true), value_stack.array[value_stack.top-1].yyval.String), value_stack.array[value_stack.top-2].yyval.Token); }
        return;
      case 564: // encaps_var -> T_DOLLAR_OPEN_CURLY_BRACES expr '}' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, _astFactory.Variable(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.Node, NullLangElement), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
        return;
      case 565: // encaps_var -> T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '}' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, _astFactory.Variable(value_stack.array[value_stack.top-2].yypos, value_stack.array[value_stack.top-2].yyval.String, NullLangElement, false), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
        return;
      case 566: // encaps_var -> T_DOLLAR_OPEN_CURLY_BRACES T_STRING_VARNAME '[' expr ']' '}' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, _astFactory.ArrayItem(Span.Combine(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-2].yypos), false,
					_astFactory.Variable(value_stack.array[value_stack.top-5].yypos, value_stack.array[value_stack.top-5].yyval.String, NullLangElement, false), value_stack.array[value_stack.top-3].yyval.Node), Tokens.T_DOLLAR_OPEN_CURLY_BRACES); }
        return;
      case 567: // encaps_var -> T_CURLY_OPEN variable '}' 
{ yyval.Node = _astFactory.EncapsedExpression(yypos, value_stack.array[value_stack.top-2].yyval.Node, Tokens.T_LBRACE); }
        return;
      case 568: // encaps_var_offset -> T_STRING 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.String, _lexer.TokenText); }
        return;
      case 569: // encaps_var_offset -> T_NUM_STRING 
{ yyval.Node = _astFactory.Literal(yypos, value_stack.array[value_stack.top-1].yyval.Long, _lexer.TokenText); }
        return;
      case 570: // encaps_var_offset -> T_VARIABLE 
{ yyval.Node = _astFactory.Variable(yypos, value_stack.array[value_stack.top-1].yyval.String, NullLangElement, true); }
        return;
      case 571: // internal_functions_in_yacc -> T_ISSET '(' isset_variables possible_comma ')' 
{ yyval.Node = _astFactory.Isset(yypos, AddTrailingComma(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-2].yyval.Bool)); }
        return;
      case 572: // internal_functions_in_yacc -> T_EMPTY '(' expr ')' 
{ yyval.Node = _astFactory.Empty(yypos, value_stack.array[value_stack.top-2].yyval.Node);}
        return;
      case 573: // internal_functions_in_yacc -> T_INCLUDE expr 
{ yyval.Node = _astFactory.Inclusion(yypos, isConditional, InclusionTypes.Include, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 574: // internal_functions_in_yacc -> T_INCLUDE_ONCE expr 
{ yyval.Node = _astFactory.Inclusion(yypos, isConditional, InclusionTypes.IncludeOnce, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 575: // internal_functions_in_yacc -> T_EVAL '(' expr ')' 
{ yyval.Node = _astFactory.Eval(yypos, value_stack.array[value_stack.top-2].yyval.Node); }
        return;
      case 576: // internal_functions_in_yacc -> T_REQUIRE expr 
{ yyval.Node = _astFactory.Inclusion(yypos, isConditional, InclusionTypes.Require, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 577: // internal_functions_in_yacc -> T_REQUIRE_ONCE expr 
{ yyval.Node = _astFactory.Inclusion(yypos, isConditional, InclusionTypes.RequireOnce, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 578: // isset_variables -> isset_variable 
{ yyval.NodeList = new List<LangElement>() { value_stack.array[value_stack.top-1].yyval.Node }; }
        return;
      case 579: // isset_variables -> isset_variables ',' isset_variable 
{ yyval.NodeList = AddToList<LangElement>(value_stack.array[value_stack.top-3].yyval.NodeList, value_stack.array[value_stack.top-1].yyval.Node); }
        return;
      case 580: // isset_variable -> expr 
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
