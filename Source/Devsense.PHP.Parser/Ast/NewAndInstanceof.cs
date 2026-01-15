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
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    #region NewEx

    /// <summary>
    /// <c>new</c> expression.
    /// </summary>
    public sealed class NewEx : VarLikeConstructUse
    {
        int _span_start = -1;

        public override Span Span
        {
            get => _span_start < 0 ? Span.Invalid : Span.FromBounds(_span_start, CallSignature.Span.IsValid ? CallSignature.Span.End : ClassNameRef.Span.End);
            protected set => _span_start = value.IsValid ? value.Start : -1;
        }

        public override Operations Operation { get { return Operations.New; } }

        internal override bool AllowsPassByReference { get { return true; } }

        /// <remarks>Always <c>null</c>.</remarks>
        public override sealed Expression IsMemberOf => null;

        /// <summary>Type of class being instantiated</summary>
        public TypeRef /*!*/ ClassNameRef { get; }

        /// <summary>Call signature of constructor</summary>
        public CallSignature CallSignature { get; }

        public NewEx(Text.Span span, TypeRef/*!*/ classNameRef, ActualParam[]/*!*/ parameters, Text.Span parametersSpan)
            : base(span)
        {
            this.ClassNameRef = classNameRef ?? throw new ArgumentNullException(nameof(classNameRef));
            this.CallSignature = new CallSignature(parameters, parametersSpan);
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitNewEx(this);
        }
    }

    #endregion

    #region InstanceOfEx

    /// <summary>
    /// <c>instanceof</c> expression.
    /// </summary>
    public sealed class InstanceOfEx : Expression
    {
        public override Span Span
        {
            get => Span.FromBounds(this.Expression.Span.Start, this.ClassNameRef.Span.End);
            protected set { /*ignored*/ }
        }

        public override Operations Operation { get { return Operations.InstanceOf; } }

        /// <summary>Expression being tested</summary>
        public Expression /*!*/ Expression { get; }

        /// <summary>Type to test if <see cref="Expression"/> is of</summary>
        public TypeRef/*!*/ ClassNameRef { get; }

        public InstanceOfEx(Text.Span span, Expression/*!*/ expression, TypeRef/*!*/ classNameRef)
            : base(span)
        {
            Debug.Assert(expression != null && classNameRef != null);

            this.Expression = expression;
            this.ClassNameRef = classNameRef;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitInstanceOfEx(this);
        }
    }

    #endregion

    //#region TypeOfEx

    ///// <summary>
    ///// <c>typeof</c> expression.
    ///// </summary>
    //public sealed class TypeOfEx : Expression
    //{
    //    public override Operations Operation { get { return Operations.TypeOf; } }

    //    public TypeRef/*!*/ ClassNameRef { get { return classNameRef; } }
    //    private TypeRef/*!*/ classNameRef;

    //    public TypeOfEx(Text.Span span, TypeRef/*!*/ classNameRef)
    //        : base(span)
    //    {
    //        Debug.Assert(classNameRef != null);

    //        this.classNameRef = classNameRef;
    //    }

    //    /// <summary>
    //    /// Call the right Visit* method on the given Visitor object.
    //    /// </summary>
    //    /// <param name="visitor">Visitor to be called.</param>
    //    public override void VisitMe(TreeVisitor visitor)
    //    {
    //        visitor.VisitTypeOfEx(this);
    //    }
    //}

    //#endregion
}
