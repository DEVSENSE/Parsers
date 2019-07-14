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
using System.Diagnostics;
using System.Collections.Generic;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Common interface for lambda function expression, encapsulates closures and arrow functions.
    /// </summary>
    public interface ILambdaExpression : IExpression
    {
        /// <summary>
        /// Function signature.
        /// </summary>
        Signature Signature { get; }

        /// <summary>
        /// Return type if present.
        /// </summary>
        TypeRef ReturnType { get; }

        /// <summary>
        /// Whether the function is 'static'.
        /// </summary>
        bool IsStatic { get; set; }

        /// <summary>
        /// Anonymous function body. Is either a block statement (closure) or an expression (arrow func).
        /// </summary>
        ILangElement Body { get; }
    }

    /// <summary>
    /// Represents a lambda function declaration.
    /// </summary>
    public class LambdaFunctionExpr : Expression, ILambdaExpression
    {
        /// <summary>
        /// Expression operation.
        /// </summary>
        public override Operations Operation
        {
            get { return Operations.Closure; }
        }

        /// <summary>
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }

        public Signature Signature { get; }

        /// <summary>
        /// Parameters specified within <c>use</c> 
        /// </summary>
        public IList<FormalParam> UseParams { get; }

        /// <summary>
        /// Lambda body.
        /// </summary>
        public BlockStmt/*!*/Body { get { return body as BlockStmt; } }
        public Expression/*!*/Expression { get { return body as Expression; } }

        ILangElement ILambdaExpression.Body { get { return body; } }
        private readonly ILangElement/*!*/ body;

        /// <summary>
        /// Span of the lambda header.
        /// </summary>
        public Text.Span HeadingSpan { get { return Text.Span.FromBounds(Span.Start, _headingEnd); } }
        readonly int _headingEnd;

        /// <summary>
        /// Span of the parameters and the parentheses.
        /// </summary>
        public Text.Span ParametersSpan => this.Signature.Span;

        /// <summary>
        /// Return type if present.
        /// </summary>
        public TypeRef ReturnType { get; }

        /// <summary>
        /// Modifiers, <see cref="PhpMemberAttributes.Static"/> or none.
        /// </summary>
        public PhpMemberAttributes Modifiers => _modifiers;
        private PhpMemberAttributes _modifiers;

        /// <summary>
        /// Whether the function is 'static'.
        /// </summary>
        public bool IsStatic
        {
            get { return (_modifiers & PhpMemberAttributes.Static) != 0; }
            set
            {
                if (value) _modifiers |= PhpMemberAttributes.Static;
                else _modifiers &= ~PhpMemberAttributes.Static;
            }
        }

        #region Construction

        public LambdaFunctionExpr(
            Text.Span span, Text.Span headingSpan,
            Signature signature, IList<FormalParam> useParams,
            ILangElement/*!*/ body, TypeRef returnType)
            : base(span)
        {
            this.Signature = signature;
            this.UseParams = useParams;
            this.body = body;
            this.ReturnType = returnType;

            _headingEnd = headingSpan.End;
        }

        [Obsolete]
        public LambdaFunctionExpr(
            Text.Span span, Text.Span headingSpan,
            bool aliasReturn, PhpMemberAttributes modifiers, IList<FormalParam>/*!*/ formalParams,
            Text.Span paramSpan, IList<FormalParam> useParams,
            BlockStmt/*!*/ body, TypeRef returnType)
            : this(span, headingSpan, new Signature(aliasReturn, formalParams, paramSpan), useParams, body, returnType)
        {
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitLambdaFunctionExpr(this);
        }
    }

    /// <summary>
    /// Represents an arrow function declaration.
    /// </summary>
    public sealed class ArrowFunctionExpr : LambdaFunctionExpr
    {
        /// <summary>
        /// Expression operation.
        /// </summary>
        public override Operations Operation
        {
            get { return Operations.ArrowFunc; }
        }

        #region Construction

        public ArrowFunctionExpr(
            Text.Span span, Text.Span headingSpan,
            Signature signature, Expression/*!*/expression, TypeRef returnType)
            : base(span, headingSpan, signature, null, expression, returnType)
        {
        }

        #endregion
    }
}