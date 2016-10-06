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

using System;
using System.Collections.Generic;

using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
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
        /// <param name="context">Global code naming context.</param>
        /// <returns>Global code node.</returns>
        TNode GlobalCode(TSpan span, IEnumerable<TNode> statements, NamingContext context);

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
        /// <param name="context">Namespace naming context.</param>
        /// <returns>Namespace node.</returns>
        TNode Namespace(TSpan span, QualifiedName? name, TSpan nameSpan, TNode block, NamingContext context);

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
        /// <param name="context">Namespace naming context.</param>
        /// <returns>Namespace node.</returns>
        TNode Namespace(TSpan span, QualifiedName? name, TSpan nameSpan, NamingContext context);

        /// <summary>
        /// Create declare statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="decls">List of constant declarations.</param>
        /// <param name="statementOpt">List of statements within the namespace.</param>
        /// <returns>Declare statement.</returns>
        TNode Declare(TSpan span, IEnumerable<LangElement> decls, TNode statementOpt);

        /// <summary>
        /// Creates function declaration node.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="conditional"><c>True</c> whether the declaration is conditional.</param>
        /// <param name="aliasReturn">Whether the function returns an aliased value.</param>
        /// <param name="attributes">Declaration attributes in case of a type member.</param>
        /// <param name="returnType">Optional. Function return type.</param>
        /// <param name="name">Function name.</param>
        /// <param name="nameSpan">Function name span.</param>
        /// <param name="typeParamsOpt">Optional. Generic type parameters.</param>
        /// <param name="formalParams">Function parameters.</param>
        /// <param name="formalParamsSpan">Parameters enclosing parenthesis span.</param>
        /// <param name="body">Function body.</param>
        /// <returns>Function node.</returns>
        TNode Function(TSpan span,
            bool conditional, bool aliasReturn, PhpMemberAttributes attributes,
            TypeRef returnType,
            Name name, TSpan nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt,
            IEnumerable<FormalParam> formalParams, TSpan formalParamsSpan,
            TNode body);

        /// <summary>
        /// Creates a lambda declaration node.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="headingSpan">Heading span - ends after return type.</param>
        /// <param name="aliasReturn">Whether the function returns an aliased value.</param>
        /// <param name="returnType">Optional. Function return type.</param>
        /// <param name="formalParams">Lambda parameters.</param>
        /// <param name="formalParamsSpan">Parameters enclosing parenthesis span.</param>
        /// <param name="lexicalVars">Variables from parent scope.</param>
        /// <param name="body">Lambda body.</param>
        /// <returns>Lambda node.</returns>
        TNode Lambda(TSpan span, TSpan headingSpan, bool aliasReturn,
            TypeRef returnType,
            IEnumerable<FormalParam> formalParams, TSpan formalParamsSpan,
            IEnumerable<FormalParam> lexicalVars, TNode body);

        /// <summary>
        /// Creates <c>FormalParam</c> for a function of method declaration.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Parameter name.</param>
        /// <param name="nameSpan">Parameter name span.</param>
        /// <param name="typeOpt">Parameter type.</param>
        /// <param name="flags">Parameter flags.</param>
        /// <param name="initValue">Default value expression.</param>
        /// <returns></returns>
        TNode Parameter(Span span, string name, Span nameSpan, TypeRef typeOpt, FormalParam.Flags flags, Expression initValue);

        /// <summary>
        /// Creates type declaration node.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="headingSpan">Heading span - ends after implements list.</param>
        /// <param name="conditional">Whether the declaration is conditional.</param>
        /// <param name="attributes">Type attributes.</param>
        /// <param name="name">Type name.</param>
        /// <param name="nameSpan">Name span.</param>
        /// <param name="typeParamsOpt">Optional. Generic type parameters.</param>
        /// <param name="baseClassOpt">Base class name if any.</param>
        /// <param name="implements">Enumeration of interfaces implemented by this type.</param>
        /// <param name="members">Enumeration of type members.</param>
        /// <param name="bodySpan">Span of block enclosing members (including <c>{</c> and <c>}</c>.</param>
        /// <returns>Type node.</returns>
        TNode Type(TSpan span, Span headingSpan,
            bool conditional, PhpMemberAttributes attributes,
            Name name, TSpan nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt,
            TypeRef baseClassOpt,
            IEnumerable<TypeRef> implements,
            IEnumerable<TNode> members, TSpan bodySpan);

        /// <summary>
        /// Creates method declaration node.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="aliasReturn">Whether the function returns an aliased value.</param>
        /// <param name="attributes">Declaration attributes in case of a type member.</param>
        /// <param name="returnType">Optional. Function return type.</param>
        /// <param name="returnTypeSpan"><paramref name="returnType"/> span, in case function has a return type</param>
        /// <param name="name">Method name.</param>
        /// <param name="nameSpan">Method name span.</param>
        /// <param name="typeParamsOpt">Optional. Generic type parameters.</param>
        /// <param name="formalParams">Method parameters.</param>
        /// <param name="formalParamsSpan">Parameters enclosing parenthesis span.</param>
        /// <param name="baseCtorParams">Actual paramters of base constructor call.</param>
        /// <param name="body">Method body.</param>
        /// <returns>Method node.</returns>
        TNode Method(TSpan span,
            bool aliasReturn, PhpMemberAttributes attributes,
            TypeRef returnType, TSpan returnTypeSpan,
            string name, TSpan nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt,
            IEnumerable<FormalParam> formalParams, TSpan formalParamsSpan,
            IEnumerable<ActualParam> baseCtorParams, TNode body);

        /// <summary>
        /// Creates class trait use statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="traits">Enumeration of traits.</param>
        /// <param name="adaptationsBlock"><see cref="TraitAdaptationBlock"/> containing all adaptations.</param>
        /// <returns>Trait use type member.</returns>
        TNode TraitUse(TSpan span, IEnumerable<QualifiedNameRef> traits, TNode adaptationsBlock);

        /// <summary>
        /// Create <c>TraitAdaptationPrecedence</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Member affected by the precedence.</param>
        /// <param name="precedences">The renamed types and members.</param>
        /// <returns>TraitAdaptationPrecedence expression</returns>
        TNode TraitAdaptationPrecedence(TSpan span, Tuple<QualifiedNameRef, NameRef> name, IEnumerable<QualifiedNameRef> precedences);

        /// <summary>
        /// Create <c>TraitAdaptationAlias</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Member affected by the alias.</param>
        /// <param name="identifierOpt">New name assigned by the alias.</param>
        /// <param name="attributeOpt">New accessibility modifier.</param>
        /// <returns>TraitAdaptationAlias expression</returns>
        TNode TraitAdaptationAlias(TSpan span, Tuple<QualifiedNameRef, NameRef> name, NameRef identifierOpt, PhpMemberAttributes? attributeOpt);

        /// <summary>
        /// Create global declaration statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="variables">Global variables.</param>
        /// <returns>GlobalStmt statement.</returns>
        TNode Global(Span span, List<TNode> variables);

        /// <summary>
        /// Creates declaration of class constants, class fields or global constants.
        /// <code>
        /// /** $var X */
        /// var $field1, $field2;
        /// </code>
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
        /// <param name="nameSpan">Complete name span.</param>
        /// <param name="initializer">Constant value expression.</param>
        /// <returns>Class constant declaration.</returns>
        TNode ClassConstDecl(TSpan span, VariableName name, TSpan nameSpan, TNode initializer);

        /// <summary>
        /// Creates global constant declaration.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="conditional">Whether the declaration is conditional.</param>
        /// <param name="name">Constant name.</param>
        /// <param name="nameSpan">Complete name span.</param>
        /// <param name="initializer">Constant value expression.</param>
        /// <returns>Global constant declaration.</returns>
        TNode GlobalConstDecl(TSpan span,
            bool conditional,
            VariableName name, TSpan nameSpan, TNode initializer);

        /// <summary>
        /// Creates block of code enclosed in braces (<c>{</c> ... <c>}</c>).
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="statements">Containing statements.</param>
        /// <returns>Block statement.</returns>
        TNode Block(TSpan span, IEnumerable<TNode> statements);

        /// <summary>
        /// Creates block containing trait adaptations.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="adaptations">Containing adaptations.</param>
        /// <returns>Block trait adaptations.</returns>
        TNode TraitAdaptationBlock(TSpan span, IEnumerable<TNode> adaptations);

        /// <summary>
        /// Creates block of code enclosed between colon and <paramref name="endToken"/> followed by semicolon (<c>;</c>).
        /// <code>:
        ///   statements
        /// endToken;</code>
        /// </summary>
        /// <param name="span">Entire element span including initial <c>:</c> and ending token and semicolon.</param>
        /// <param name="statements">Containing statements.</param>
        /// <param name="endToken">Ending token.</param>
        /// <returns>Colon block statement.</returns>
        TNode ColonBlock(TSpan span, IEnumerable<TNode> statements, Tokens endToken);

        /// <summary>
        /// Creates block of code assigned to a simple declaration of a namespace (without explicit {} block)
        /// </summary>
        /// <param name="span">Entire element span starting with the first statement and ending with the last</param>
        /// <param name="statements">Containing statements.</param>
        /// <returns>Simple block statement.</returns>
        TNode SimpleBlock(TSpan span, IEnumerable<TNode> statements);

        /// <summary>
        /// Creates <c>echo</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="parameters">Expressions to be echoed.</param>
        /// <returns>Echo expression.</returns>
        TNode Echo(TSpan span, IEnumerable<TNode> parameters);

        /// <summary>
        /// Creates <c>unset</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="variables">Variables to be unset.</param>
        /// <returns>Unset statement.</returns>
        TNode Unset(Span span, IEnumerable<TNode> variables);

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
        /// <returns>Try block.</returns>
        TNode TryCatch(TSpan span, TNode body, IEnumerable<CatchItem> catches, TNode finallyBlockOpt);

        /// <summary>
        /// Creates <c>catch</c> block according to the optional parameters.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="typeOpt">Exception type.</param>
        /// <param name="variable">Exception variable.</param>
        /// <param name="block">Statements of the block.</param>
        /// <returns>Catch clause.</returns>
        TNode Catch(Span span, TypeRef typeOpt, DirectVarUse variable, TNode block);

        /// <summary>
        /// Creates <c>finally</c> block.
        /// When both optional parameters are null, <c>finally</c> block is created.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="block">Statements of the block.</param>
        /// <returns>Finally clause.</returns>
        TNode Finally(Span span, TNode block);

        /// <summary>
        /// Creates a <c>throw</c> statment;
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="expression">The exception statement.</param>
        /// <returns>Throw statment.</returns>
        TNode Throw(Span span, TNode expression);

        /// <summary>
        /// Create element representing <c>__haltcompiler();</c> call.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <returns>Hal compiler element.</returns>
        TNode HaltCompiler(TSpan span);

        /// <summary>
        /// Creates a statement from an expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="expression">The expression used as statement.</param>
        /// <returns>Expression statement.</returns>
        TNode ExpressionStmt(Span span, TNode expression);

        /// <summary>
        /// Create static variable declaration statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="staticVariables">List of variable declarations.</param>
        /// <returns>Static statement.</returns>
        TNode Static(Span span, IEnumerable<TNode> staticVariables);

        /// <summary>
        /// Create static variable declaration statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Variable naem.</param>
        /// <param name="initializerOpt">Variable initializer.</param>
        /// <returns>StaticVarDecl statement.</returns>
        TNode StaticVarDecl(Span span, VariableName name, TNode initializerOpt);

        /// <summary>
        /// An empty statement (<c>;</c>).
        /// </summary>
        /// <param name="span">Semicolon position.</param>
        /// <returns>Empty statement.</returns>
        TNode EmptyStmt(Span span);

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
        /// <param name="block">List of statements containing only <see cref="SwitchItem"/> statements.</param>
        /// <returns>Switch statement.</returns>
        TNode Switch(TSpan span, TNode value, List<TNode> block);

        /// <summary>
        /// Creates <c>case</c> or <c>default</c> statement used in <c>switch</c> statement.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="valueOpt">Case value expression, <c>null</c> for default.</param>
        /// <param name="block">Block of statements.</param>
        /// <returns>SwitchItem statement.</returns>
        TNode Case(Span span, TNode valueOpt, TNode block);

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
        TNode Eval(TSpan span, TNode code);

        /// <summary>
        /// Creates <c>empty</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="code">Code expression.</param>
        /// <returns>Empty expression.</returns>
        TNode Empty(TSpan span, TNode code);

        /// <summary>
        /// Creates <c>isset</c> expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="variables">List of variables.</param>
        /// <returns>Isset expression.</returns>
        TNode Isset(TSpan span, IEnumerable<TNode> variables);

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
        /// Creates conditional expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="condExpr">Condition expression.</param>
        /// <param name="trueExpr">True expression.</param>
        /// <param name="falseExpr">False expression.</param>
        /// <returns>Conditional expression.</returns>
        TNode ConditionalEx(TSpan span, TNode condExpr, TNode trueExpr, TNode falseExpr);

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
        TNode Variable(TSpan span, string name, TNode memberOfOpt);

        /// <summary>
        /// Direct static field.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Field name.</param>
        /// <param name="typeRef">Field containing type.</param>
        /// <returns>Direct static field access expression.</returns>
        TNode Variable(TSpan span, string name, TypeRef typeRef);

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
        /// Foreach variable used in <c>foreach</c> loop as key and value.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="variable">Expression representing the variable name.</param>
        /// <param name="alias">Indicates if the variable is alias.</param>
        /// <returns>Foreach variable expression.</returns>
        ForeachVar ForeachVariable(TSpan span, TNode variable, bool alias = false);

        /// <summary>
        /// Create <c>TypeRef</c> reference to a type.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="className">Qualified class name.</param>
        /// <returns>Type reference.</returns>
        TNode TypeReference(TSpan span, QualifiedName className);

        /// <summary>
        /// Create <c>NullableTypeRef</c> reference to a nullable type.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="className">Qualified class name.</param>
        /// <returns>Nullable type reference.</returns>
        TNode NullableTypeReference(TSpan span, TNode className);

        /// <summary>
        /// Create <c>GenericTypeRef</c> reference to a generic type.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="className">Qualified class name.</param>
        /// <param name="genericParams">Actual generic parameters.</param>
        /// <returns>Nullable type reference.</returns>
        TNode GenericTypeReference(TSpan span, TNode className, List<TypeRef> genericParams);

        /// <summary>
        /// Create <c>TypeRef</c> reference to a type.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="varName">Indirect name.</param>
        /// <returns>Type reference.</returns>
        TNode TypeReference(TSpan span, TNode varName);

        /// <summary>
        /// Create <c>TypeRef</c> reference to a type.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="classes">List of all qualified classes in the expression.</param>
        /// <returns>Type reference.</returns>
        TNode TypeReference(TSpan span, IEnumerable<TNode> classes);

        /// <summary>
        /// Creates anonymous type reference node.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="headingSpan">Span of the type header.</param>
        /// <param name="conditional">Whether the declaration is conditional.</param>
        /// <param name="attributes">Type attributes.</param>
        /// <param name="typeParamsOpt">Optional. Generic type parameters.</param>
        /// <param name="baseClassOpt">Base class name if any.</param>
        /// <param name="implements">Enumeration of interfaces implemented by this type.</param>
        /// <param name="members">Enumeration of type members.</param>
        /// <param name="blockSpan">Span of block enclosing members (including <c>{</c> and <c>}</c>.</param>
        /// <returns>Type node.</returns>
        TNode AnonymousTypeReference(TSpan span, Span headingSpan,
            bool conditional, PhpMemberAttributes attributes,
            IEnumerable<FormalTypeParam> typeParamsOpt,
            TypeRef baseClassOpt,
            IEnumerable<TypeRef> implements,
            IEnumerable<TNode> members, TSpan blockSpan);

        /// <summary>
        /// Creates a pseudo constant use.
        /// <code>
        /// __LINE__;
        /// </code>
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="type">Type of the pseudo constant.</param>
        /// <returns>Pseudoconstant access expression.</returns>
        TNode PseudoConstUse(TSpan span, PseudoConstUse.Types type);

        /// <summary>
        /// Creates global constant use.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="name">Constant name.</param>
        /// <param name="nameFallback">Fallback name in case <paramref name="name"/> does not exist in current context.</param>
        /// <returns>Global constant access expression.</returns>
        TNode ConstUse(TSpan span, QualifiedName name, QualifiedName? nameFallback);

        /// <summary>
        /// Creates global constant use.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="tref">Containing class name.</param>
        /// <param name="name">Constant name.</param>
        /// <param name="nameSpan">Span of <paramref name="name"/>.</param>
        /// <returns>Class constant access expression.</returns>
        TNode ClassConstUse(TSpan span, TypeRef tref, Name name, TSpan nameSpan);

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
        TNode Call(TSpan span, Name name, TSpan nameSpan, CallSignature signature, TypeRef typeRef);

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
        /// Creates <c>FormalParam</c> for a function or method declaration.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="expr">Argument expression.</param>
        /// <param name="flags">Parameter flags.</param>
        /// <returns>Function call argument.</returns>
        TNode ActualParameter(Span span, TNode expr, ActualParam.Flags flags);

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
        /// Creates array value item initialization expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="indexOpt">Optional. Expression representing an index.</param>
        /// <param name="valueExpr">Expression representing an item.</param>
        /// <returns>Array value item expression.</returns>
        Item ArrayItemValue(TSpan span, TNode indexOpt, TNode valueExpr);

        /// <summary>
        /// Creates array reference item initialization expression.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="indexOpt">Optional. Expression representing an index.</param>
        /// <param name="variable">Expression representing a variable.</param>
        /// <returns>Array reference item expression.</returns>
        Item ArrayItemRef(TSpan span, TNode indexOpt, TNode variable);

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
        TNode List(TSpan span, IEnumerable<Item> targets);

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
        TNode PHPDoc(TSpan span, TNode content);

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
}
