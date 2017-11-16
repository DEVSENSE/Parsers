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
    #region LambdaFunctionDecl

    /// <summary>
    /// Represents a function declaration.
    /// </summary>
    public sealed class LambdaFunctionExpr : Expression
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

        public Signature Signature { get { return signature; } }
        private readonly Signature signature;

        /// <summary>
        /// Parameters specified within <c>use</c> 
        /// </summary>
        public IList<FormalParam> UseParams { get { return useParams; } }
        private readonly IList<FormalParam> useParams;

        /// <summary>
        /// Lambda body.
        /// </summary>
        public BlockStmt/*!*/ Body { get { return body; } }
        private readonly BlockStmt/*!*/ body;

        /// <summary>
        /// Span of the lambda header.
        /// </summary>
        public Text.Span HeadingSpan { get { return headingSpan; } }
        private Text.Span headingSpan;

        /// <summary>
        /// Span of the parameters and the parentheses.
        /// </summary>
        public Text.Span ParametersSpan => signature.Span;

        /// <summary>
        /// Return type if present.
        /// </summary>
        public TypeRef ReturnType { get { return returnType; } }
        private TypeRef returnType;

        /// <summary>
        /// Modifiers, <see cref="PhpMemberAttributes.Static"/> or none.
        /// </summary>
        public PhpMemberAttributes Modifiers => _modifiers;
        private PhpMemberAttributes _modifiers;

        #region Construction

        public LambdaFunctionExpr(
            Text.Span span, Text.Span headingSpan,
            bool aliasReturn, PhpMemberAttributes modifiers, IList<FormalParam>/*!*/ formalParams,
            Text.Span paramSpan, IList<FormalParam> useParams,
            BlockStmt/*!*/ body, TypeRef returnType)
            : base(span)
        {
            Debug.Assert(formalParams != null && body != null);

            this.signature = new Signature(aliasReturn, formalParams, paramSpan);
            this.useParams = useParams;
            //this.typeSignature = new TypeSignature(genericParams);
            this.body = body;
            this.headingSpan = headingSpan;
            this.returnType = returnType;
            _modifiers = modifiers;
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

    #endregion
}