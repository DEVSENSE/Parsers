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

        // TODO: TraitsUse

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

        // TODO: assert

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

        #endregion
    }

    /// <summary>
    /// Nodes factory used by <see cref="Parser.Parser"/>.
    /// </summary>
    public interface IParserNodesFactory : INodesFactory<LangElement, Span>
    {

    }
}
