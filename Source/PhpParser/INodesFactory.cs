using PHP.Core.AST;
using PHP.Core.Text;
using PHP.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhpParser
{
    // TODO: interfaces instead of TNode

    /// <summary>
    /// Factory providing instantiation of AST nodes.
    /// </summary>
    /// <typeparam name="TNode">Type of root node.</typeparam>
    /// <typeparam name="TSpan">Type of position object.</typeparam>
    public interface INodesFactory<TNode, TSpan>
    {
        #region GlobalCode

        /// <summary>
        /// Creates root of the syntax tree containing statements.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="statements">Top statements.</param>
        /// <returns>Global code node.</returns>
        TNode GlobalCode(TSpan span, IEnumerable<TNode> statements);

        #endregion

        #region Statements

        /// <summary>
        /// Create namespace declaration node.
        /// <code>
        /// namespace name {
        ///   statements
        /// }</code>
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Name of the namespace. Can be an empty name.</param>
        /// <param name="nameSpan">Name span. Can be <c>Invalid</c> in case the name is empty.</param>
        /// <param name="block">Block of code enclosed in brackets.</param>
        /// <returns>Namespace node.</returns>
        TNode Namespace(TSpan span, QualifiedName? name, TSpan nameSpan, TNode block);

        /// <summary>
        /// Create simple namespace declaration node.
        /// <code>
        /// namespace name;
        /// statements
        /// </code>
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Name of the namespace. Can be an empty name.</param>
        /// <param name="nameSpan">Name span. Can be <c>Invalid</c> in case the name is empty.</param>
        /// <param name="statements">List of statements within the namespace.</param>
        /// <returns>Namespace node.</returns>
        TNode Namespace(TSpan span, QualifiedName? name, TSpan nameSpan, IEnumerable<TNode> statements);

        /// <summary>
        /// Creates function declaration node.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="conditional"><c>True</c> whether the declaration is conditional.</param>
        /// <param name="aliasReturn">Whether the function returns an aliased value.</param>
        /// <param name="attributes">Declaration attributes in case of a type member.</param>
        /// <param name="returnType">Optional. Function return type.</param>
        /// <param name="returnTypeSpan"><paramref name="returnType"/> span, in case function has a return type</param>
        /// <param name="name">Function name.</param>
        /// <param name="nameSpan">Function name span.</param>
        /// <param name="typeParamsOpt">Optional. Generic type parameters.</param>
        /// <param name="formalParams">Function parameters.</param>
        /// <param name="formalParamsSpan">Parameters enclosing parenthesis span.</param>
        /// <param name="body">Function body.</param>
        /// <returns>Function node.</returns>
        TNode Function(TSpan span,
            bool conditional, bool aliasReturn, PhpMemberAttributes attributes,
            QualifiedName? returnType, TSpan returnTypeSpan,
            Name name, TSpan nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt,
            IEnumerable<FormalParam> formalParams, TSpan formalParamsSpan,
            TNode body);

        /// <summary>
        /// Creates type declaration node.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="conditional">Whether the declaration is conditional.</param>
        /// <param name="attributes">Type attributes.</param>
        /// <param name="name">Type name.</param>
        /// <param name="nameSpan">Name span.</param>
        /// <param name="typeParamsOpt">Optional. Generic type parameters.</param>
        /// <param name="baseClassOpt">Base class name if any.</param>
        /// <param name="implements">Enumeration of interfaces implemented by this type.</param>
        /// <param name="members">Enumeration of type members.</param>
        /// <param name="blockSpan">Span of block enclosing members (including <c>{</c> and <c>}</c>.</param>
        /// <returns>Type node.</returns>
        TNode Type(TSpan span,
            bool conditional, PhpMemberAttributes attributes,
            Name name, TSpan nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt,
            Tuple<GenericQualifiedName, TSpan> baseClassOpt,
            IEnumerable<Tuple<GenericQualifiedName, TSpan>> implements,
            IEnumerable<TNode> members, TSpan blockSpan);

        /// <summary>
        /// Creates class traits.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="traits">Enumeration of traits.</param>
        /// <param name="adaptations">Enumeration of trait adaptations.</param>
        /// <returns>Trait use type member.</returns>
        TNode TraitUse(TSpan span, IEnumerable<QualifiedName> traits, IEnumerable<TraitsUse.TraitAdaptation> adaptations);

        /// <summary>
        /// Creates declaration of class constants, class fields or global constants.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="attributes">Type member attributes.</param>
        /// <param name="decls">Enumeration of declarations.</param>
        /// <returns>Declaration list.</returns>
        TNode DeclList(TSpan span, PhpMemberAttributes attributes, IEnumerable<TNode> decls);

        /// <summary>
        /// Creates field declaration.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Field name.</param>
        /// <param name="initializerOpt">Optional. Field initializer.</param>
        /// <returns>Field declaration.</returns>
        TNode FieldDecl(TSpan span, VariableName name, TNode initializerOpt);

        /// <summary>
        /// Creates class constant declaration.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Constant name.</param>
        /// <param name="initializer">Constant value expression.</param>
        /// <returns>Class constant declaration.</returns>
        TNode ClassConstDecl(TSpan span, VariableName name, TNode initializer);

        /// <summary>
        /// Creates global constant declaration.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="conditional">Whether the declaration is conditional.</param>
        /// <param name="name">Constant name.</param>
        /// <param name="initializer">Constant value expression.</param>
        /// <returns>Global constant declaration.</returns>
        TNode GlobalConstDecl(TSpan span,
            bool conditional,
            VariableName name, TNode initializer);

        /// <summary>
        /// Creates block of code enclosed in braces (<c>{</c> ... <c>}</c>).
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="statements">Containing statements.</param>
        /// <returns>Block statement.</returns>
        TNode Block(TSpan span, IEnumerable<TNode> statements);

        /// <summary>
        /// Creates block of code enclosed between colon and <paramref name="endToken"/> followed by semicolon (<c>;</c>).
        /// <code>:
        ///   statements
        /// endToken;</code>
        /// </summary>
        /// <param name="span">Entire element span including initial <c>:</c> and ending token and semicolon.</param>
        /// <param name="statements">Containing statements.</param>
        /// <param name="endToken">Ending token.</param>
        /// <returns>Block statement.</returns>
        TNode ColonBlock(TSpan span, IEnumerable<TNode> statements, Parser.Tokens endToken);

        /// <summary>
        /// Creates <c>echo</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="parameters">Expressions to be echoed.</param>
        /// <returns>Echo expression.</returns>
        TNode Echo(TSpan span, IEnumerable<TNode> parameters);

        /// <summary>
        /// Creates pseudo <c>echo</c> statement representing HTML code.
        /// </summary>
        /// <param name="span">Entire text span.</param>
        /// <param name="html">Text.</param>
        /// <returns>Echo statement.</returns>
        TNode InlineHtml(TSpan span, string html);

        /// <summary>
        /// Creates <c>try</c>/<c>catch</c>/<c>finally</c> block.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="body">Try body.</param>
        /// <param name="catches">Catch items.</param>
        /// <param name="finallyBlockOpt">Optional. Finally block.</param>
        /// <returns></returns>
        TNode TryCatch(TSpan span, TNode body, IEnumerable<CatchItem> catches, TNode finallyBlockOpt);

        #endregion

        #region Loops, Branching

        /// <summary>
        /// Creates <c>do</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="body">Loop body.</param>
        /// <param name="cond">Condition that breaks the loop.</param>
        /// <returns>Do statement.</returns>
        TNode Do(TSpan span, TNode body, TNode cond);

        /// <summary>
        /// Creates <c>while</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="cond">Condition that breaks the loop.</param>
        /// <param name="body">Loop body.</param>
        /// <returns>While statement.</returns>
        TNode While(TSpan span, TNode cond, TNode body);

        /// <summary>
        /// Creates <c>for</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="init">Initializer expressions.</param>
        /// <param name="cond">Conditions, none, one or more. Only the last one breaks the loop.</param>
        /// <param name="action">Actions to be performed after each loop.</param>
        /// <param name="body">Loop body.</param>
        /// <returns>For statement.</returns>
        TNode For(TSpan span, IEnumerable<TNode> init, IEnumerable<TNode> cond, IEnumerable<TNode> action, TNode body);

        /// <summary>
        /// Creates <c>foreach</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="enumeree">Expression being enumerated.</param>
        /// <param name="keyOpt">Optional. The key variable.</param>
        /// <param name="value">The value variable.</param>
        /// <param name="body">Loop body.</param>
        /// <returns>Foreach statement.</returns>
        TNode Foreach(TSpan span, TNode enumeree, ForeachVar keyOpt, ForeachVar value, TNode body);

        /// <summary>
        /// Creates <c>if</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="cond">Condition expression.</param>
        /// <param name="body">True branch statement.</param>
        /// <param name="elseOpt">Optional. False branch statement.
        /// Can be another <c>if</c> in case of <c>elseif</c>, another statement in case of <c>else</c> or <c>null</c>.</param>
        /// <returns>If statement.</returns>
        TNode If(TSpan span, TNode cond, TNode body, TNode elseOpt);

        /// <summary>
        /// Creates <c>switch</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="value">Switch value expression.</param>
        /// <param name="block">Block statement containing only <see cref="SwitchItem"/> statements.</param>
        /// <returns>Switch statement.</returns>
        TNode Switch(TSpan span, TNode value, TNode block);

        /// <summary>
        /// Creates a jump statement (<c>return</c>, <c>break</c> or <c>continue</c>);
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="type">Jump type.</param>
        /// <param name="exprOpt">Optional. Jump argument.</param>
        /// <returns>Jump statement.</returns>
        TNode Jump(TSpan span, JumpStmt.Types type, TNode exprOpt);

        /// <summary>
        /// Creates <c>yield</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="keyOpt">Optional. The yielded value key. If provided, <paramref name="valueOpt"/> is required.</param>
        /// <param name="valueOpt">Optional. The yielded value. If not provided, <c>null</c> is yielded instead.</param>
        /// <returns>Yield expression.</returns>
        TNode Yield(TSpan span, TNode keyOpt, TNode valueOpt);

        /// <summary>
        /// Creates <c>yield from</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="fromExpr">Expression representing enumeration to be yielded from.</param>
        /// <returns>Yield from expression.</returns>
        TNode YieldFrom(TSpan span, TNode fromExpr);

        /// <summary>
        /// Creates label statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="label">Label name.</param>
        /// <param name="labelSpan">Name span.</param>
        /// <returns>Label statement.</returns>
        TNode Label(TSpan span, string label, TSpan labelSpan);

        /// <summary>
        /// Creates goto statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="label">Label name.</param>
        /// <param name="labelSpan">Name span.</param>
        /// <returns>Goto statement.</returns>
        TNode Goto(TSpan span, string label, TSpan labelSpan);

        #endregion

        #region Expressions

        /// <summary>
        /// Encloses given expression in parenthesis expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="expression">Enclosed expression.</param>
        /// <returns>Expression.</returns>
        /// <remarks>In case parenthesis are not needed, original <paramref name="expression"/> may be returned.</remarks>
        TNode ParenthesisExpression(TSpan span, TNode expression);

        /// <summary>
        /// Creates <c>exit</c> expression with optional result status expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="statusOpt">Optional. Exit status expression.</param>
        /// <returns>Exit expression.</returns>
        TNode Exit(TSpan span, TNode statusOpt);

        /// <summary>
        /// Creates <c>eval</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="code">Code expression.</param>
        /// <returns>Eval expression.</returns>
        TNode Eval(TSpan span, TNode statusOpt);

        /// <summary>
        /// Creates assertion expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="assertion">Text to be interpreted as an assertion expression.</param>
        /// <param name="failureOpt">Optional. Second argument.</param>
        /// <returns>Assertion expression.</returns>
        TNode Assert(TSpan span, TNode assertion, TNode failureOpt);

        /// <summary>
        /// Creates inclusion (<c>include</c>, <c>require</c>, <c>include_once</c>, <c>require_once</c>) expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="conditional">Whether the inclusion is conditional.</param>
        /// <param name="type">Inclusion type.</param>
        /// <param name="fileNameExpression">Included file name expression.</param>
        /// <returns>Inclusion expression.</returns>
        TNode Inclusion(TSpan span, bool conditional, InclusionTypes type, TNode fileNameExpression);

        /// <summary>
        /// Creates literal expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="value">Literal value.</param>
        /// <returns>Literal expression.</returns>
        TNode Literal(TSpan span, object value);

        /// <summary>
        /// Creates binary operation expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="operation">Binary expression operation.</param>
        /// <param name="leftExpression">Left expression.</param>
        /// <param name="rightExpression">Right expression.</param>
        /// <returns>Binary operation expression.</returns>
        TNode BinaryOperation(TSpan span, Operations operation, TNode leftExpression, TNode rightExpression);

        /// <summary>
        /// Creates unary operation expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="operation">Unary expression operation.</param>
        /// <param name="expression">Operation parameter.</param>
        /// <returns>Unary operation expression.</returns>
        TNode UnaryOperation(TSpan span, Operations operation, TNode expression);

        /// <summary>
        /// Creates increment or decrement unary operation.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="refexpression">Variable expression.</param>
        /// <param name="inc"><c>true</c> for increment, <c>false</c> for decrement.</param>
        /// <param name="post"><c>true</c> for post incrementation(<c>i++</c>), <c>false</c> for pre incrementation (<c>++i</c>).</param>
        /// <returns>Increment/Decrement expression.</returns>
        TNode IncrementDecrement(TSpan span, TNode refexpression, bool inc, bool post);

        /// <summary>
        /// Creates expression representing a string concatenation.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="expressions">Enumeration of expression.</param>
        /// <returns>Expression resulting in a concatenated string (<c>ConcatEx</c> or a reduced expression.).</returns>
        /// <remarks>A factory implementation may reduce the expression into a literal or a binary operation.</remarks>
        TNode Concat(TSpan span, IEnumerable<TNode> expressions);

        /// <summary>
        /// Creates assignment operation.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="target">Target expression (variable reference).</param>
        /// <param name="value">Value expression.</param>
        /// <param name="assignOp">Assign operation.</param>
        /// <returns>Assignment expression.</returns>
        TNode Assignment(TSpan span, TNode target, TNode value, Operations assignOp);

        /// <summary>
        /// Direct variable or field.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Variable or field name.</param>
        /// <param name="memberOfOpt">Optional. In case of a field, expression representing instance.</param>
        /// <returns>Direct variable access expression.</returns>
        TNode Variable(TSpan span, VariableName name, TNode memberOfOpt);

        /// <summary>
        /// Direct static field.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Field name.</param>
        /// <param name="typeRef">Field containing type.</param>
        /// <returns>Direct static field access expression.</returns>
        TNode Variable(TSpan span, VariableName name, TypeRef typeRef);

        /// <summary>
        /// Indirect variable or field.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="nameExpr">Expression representing variable or field name.</param>
        /// <param name="memberOfOpt">Optional. In case of a field, expression representing instance.</param>
        /// <returns>Direct variable access expression.</returns>
        TNode Variable(TSpan span, TNode nameExpr, TNode memberOfOpt);

        /// <summary>
        /// Indirect static field.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="nameExpr">Expression representing field name.</param>
        /// <param name="typeRef">Field containing type.</param>
        /// <returns>Direct static field access expression.</returns>
        TNode Variable(TSpan span, TNode nameExpr, TypeRef typeRef);

        /// <summary>
        /// Creates function or instance method call expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Function name.</param>
        /// <param name="nameFallback">Optional. Alternative function name in case the <paramref name="name"/> is not found in current context.</param>
        /// <param name="nameSpan">Span of the <paramref name="name"/>.</param>
        /// <param name="signature">Function call signature.</param>
        /// <param name="memberOfOpt">Optional. Target expression in case of an instance method call.</param>
        /// <returns>Function call expression.</returns>
        TNode Call(TSpan span, QualifiedName name, QualifiedName? nameFallback, TSpan nameSpan, CallSignature signature, TNode memberOfOpt);

        /// <summary>
        /// Creates indirect function or instance method call expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="nameExpr">Function name expression.</param>
        /// <param name="signature">Function call signature.</param>
        /// <param name="memberOfOpt">Optional. Target expression in case of an instance method call.</param>
        /// <returns>Indirect function call expression.</returns>
        TNode Call(TSpan span, TNode nameExpr, CallSignature signature, TNode memberOfOpt);

        /// <summary>
        /// Creates static method call expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Function name.</param>
        /// <param name="nameSpan">Span of the <paramref name="name"/>.</param>
        /// <param name="signature">Function call signature.</param>
        /// <param name="typeRef">Method containing type.</param>
        /// <returns>Function call expression.</returns>
        TNode Call(TSpan span, QualifiedName name, TSpan nameSpan, CallSignature signature, TypeRef typeRef);

        /// <summary>
        /// Creates indirect static method call expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="nameExpr">Function name expression.</param>
        /// <param name="signature">Function call signature.</param>
        /// <param name="typeRef">Method containing type.</param>
        /// <returns>Indirect function call expression.</returns>
        TNode Call(TSpan span, TNode nameExpr, CallSignature signature, TypeRef typeRef);

        /// <summary>
        /// Creates new <c>array</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="itemsOpt">Optional. Enumeration of array items.</param>
        /// <returns>Array expression.</returns>
        TNode NewArray(TSpan span, IEnumerable<Item> itemsOpt);

        /// <summary>
        /// Creates array item access expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="expression">Expression representing an array.</param>
        /// <param name="indexOpt">Optional. Expression representing an index.</param>
        /// <returns>Array item expression.</returns>
        TNode ArrayItem(TSpan span, TNode expression, TNode indexOpt);

        /// <summary>
        /// Creates <c>new</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="classNameRef">Name of the instantiated class.</param>
        /// <param name="argsOpt">Optional. Class constructor arguments.</param>
        /// <returns>The new expression.</returns>
        TNode New(TSpan span, TypeRef classNameRef, IEnumerable<ActualParam> argsOpt);

        /// <summary>
        /// Creates <c>instanceof</c> operation expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="expression">Expression.</param>
        /// <param name="typeRef">Type reference.</param>
        /// <returns>The instanceof expression.</returns>
        TNode InstanceOf(TSpan span, TNode expression, TypeRef typeRef);

        /// <summary>
        /// Creates <c>list</c> expression used as a target of a value assignment.
        /// <code>
        /// list($a, $b, list($c, $d)) = [1, 2, [3, 4]];
        /// </code>
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="targets">Enumeration of reference expressions (variables, fields, array items) or other <c>list</c> expressions.</param>
        /// <returns>List expression.</returns>
        TNode List(TSpan span, IEnumerable<TNode> targets);

        /// <summary>
        /// Creates shell expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="command">Command expression. Can be a string literal or a string concatenation. The command is enclosed in backtick operator.</param>
        /// <returns>Shell expression.</returns>
        TNode Shell(TSpan span, TNode command);

        #endregion

        #region Comments

        /// <summary>
        /// Creates documentary comment element.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="content">Content of the comment including leading <c>/**</c> and trailing <c>*/</c>.</param>
        /// <returns>PHPDoc comment element.</returns>
        TNode PHPDoc(TSpan span, string content);

        /// <summary>
        /// Creates line comment.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="content">Comment text including leading <c>//</c> or <c>#</c>.</param>
        /// <returns>Line comment element.</returns>
        TNode LineComment(TSpan span, string content);

        /// <summary>
        /// Creates block comment.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="content">Comment text including leading <c>/*</c> and trailing <c>*/</c>.</param>
        /// <returns>Block comment element.</returns>
        TNode BlockComment(TSpan span, string content);

        #endregion
    }

    /// <summary>
    /// Nodes factory used by <see cref="Parser.Parser"/>.
    /// </summary>
    public interface IParserNodesFactory : INodesFactory<LangElement, Span>
    {

    }
}
