using System;
using System.Collections.Generic;
using System.Text;
using Devsense.PHP.Syntax;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Basic node interface - language elements and global code.
    /// </summary>
    public interface IAstNode
    {
        /// <summary>
        /// Gets property collection associated with this node.
        /// </summary>
        IPropertyCollection Properties { get; }
    }

    /// <summary>
	/// Represents all AST elements - statements and expressions.
	/// </summary>
    public interface ILangElement : IAstNode
    {
        /// <summary>
        /// Position of the element in source file.
        /// </summary>
        Span Span { get; set; }

        /// <summary>
        /// Implements the visitor pattern.
        /// </summary>
        void VisitMe(TreeVisitor/*!*/visitor);
    }

    /// <summary>
    /// An expression.
    /// </summary>
    public interface IExpression : ILangElement
    {
        Operations Operation { get; }
    }

    /// <summary>
    /// A statement.
    /// </summary>
    public interface IStatement : ILangElement
    {

    }
}
