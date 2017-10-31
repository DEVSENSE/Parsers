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
    public sealed class LambdaFunctionExpr : Expression, IAliasReturn
    {
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

        //private readonly TypeSignature typeSignature;
        public BlockStmt/*!*/ Body { get { return body; } }
        private readonly BlockStmt/*!*/ body;

        public Text.Span HeadingSpan { get { return headingSpan; } }
        private Text.Span headingSpan;

        public Text.Span ParametersSpan { get { return parametersSpan; } }
        private Text.Span parametersSpan;

        public TypeRef ReturnType { get { return returnType; } }
        private TypeRef returnType;

        /// <summary>
        /// Position of the reference symbol, <c>-1</c> if none present.
        /// </summary>
        public int ReferencePosition
        {
            get { return _referenceOffset < 0 ? -1 : Span.Start + _referenceOffset; }
            set { _referenceOffset = value < 0 ? (short)-1 : (short)(value - Span.Start); }
        }
        private short _referenceOffset = -1;

        /// <summary>
        /// Position of the reference symbol, <c>-1</c> if none present.
        /// </summary>
        public int FunctionPosition
        {
            get { return _functionOffset < 0 ? -1 : Span.Start + _functionOffset; }
            set { _functionOffset = value < 0 ? (short)-1 : (short)(value - Span.Start); }
        }
        private short _functionOffset = -1;

        public PhpMemberAttributes Modifiers => FunctionPosition == Span.Start ? PhpMemberAttributes.None : PhpMemberAttributes.Static;

        /// <summary>
        /// Position of the reference symbol, <c>-1</c> if none present.
        /// </summary>
        public int UsePosition
        {
            get { return _useOffset < 0 ? -1 : Span.Start + _useOffset; }
            set { _useOffset = value < 0 ? (short)-1 : (short)(value - Span.Start); }
        }
        private short _useOffset = -1;

        /// <summary>
        /// Position of the reference symbol, <c>-1</c> if none present.
        /// </summary>
        public Text.Span UseSignaturePosition
        {
            get { return _useSignatureOffset; }
            set { _useSignatureOffset = value; }
        }
        private Text.Span _useSignatureOffset;

        #region Construction

        public LambdaFunctionExpr(
            Text.Span span, Text.Span headingSpan,
            Scope scope, bool aliasReturn, IList<FormalParam>/*!*/ formalParams,
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
            this.parametersSpan = paramSpan;
            this.returnType = returnType;
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