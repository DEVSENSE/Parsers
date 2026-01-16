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
using System.Diagnostics;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    #region FunctionCall

    public abstract class FunctionCall : VarLikeConstructUse
    {
        public override sealed Span Span
        {
            get => Span.FromBounds(IsMemberOf != null ? IsMemberOf.Span.Start : this.NameSpan.Start, this.CallSignature.Span.End);
            protected set { }
        }

        /// <summary>Function call arguments.</summary>
        public CallSignature CallSignature { get; set; }

        /// <summary>
        /// Position of called function name in source code.
        /// </summary>
        public abstract Text.Span NameSpan { get; }

        public FunctionCall(Text.Span span, CallSignature signature)
            : base(span)
        {
            Debug.Assert(signature.Parameters != null);

            this.CallSignature = signature;
        }
    }

    #endregion

    #region DirectFcnCall

    public abstract class DirectFcnCall : FunctionCall
    {
        sealed class LocalDirectFcnCall : DirectFcnCall
        {
            public override Expression IsMemberOf => null;

            public override TranslatedQualifiedName FullName { get; }

            public override Text.Span NameSpan => FullName.Span;

            public LocalDirectFcnCall(Text.Span span, TranslatedQualifiedName name, CallSignature signature)
                : base(span, signature)
            {
                this.FullName = name;
            }
        }
        
        sealed class SimpleLocalDirectFcnCall : DirectFcnCall
        {
            readonly int _name_start;

            public override Expression IsMemberOf => null;

            public override Name SimpleName { get; }

            public override TranslatedQualifiedName FullName => new TranslatedQualifiedName(
                new QualifiedName(SimpleName),
                NameSpan
            );

            public override Span NameSpan => new Span(_name_start, this.SimpleName.Value.Length);

            public SimpleLocalDirectFcnCall(Text.Span span, NameRef name, CallSignature signature)
                : base(span, signature)
            {
                Debug.Assert(name.HasValue);

                this.SimpleName = name;

                _name_start = name.Span.Start;
            }
        }

        sealed class CloneFcnCall : DirectFcnCall
        {
            readonly int _name_start;

            public override Expression IsMemberOf => null;

            public override Name SimpleName => Name.CloneName;

            public override TranslatedQualifiedName FullName => new TranslatedQualifiedName(
                new QualifiedName(SimpleName),
                NameSpan
            );

            public override Span NameSpan => new Span(_name_start, this.SimpleName.Value.Length);

            public CloneFcnCall(Text.Span span, CallSignature signature)
                : base(span, signature)
            {
                _name_start = span.Start;
            }
        }

        sealed class MemberDirectFcnCall : DirectFcnCall
        {
            readonly int _name_start;

            public override Name SimpleName { get; }

            public override Span NameSpan => new Span(_name_start, SimpleName.Value.Length);
            
            public override Expression IsMemberOf { get; }

            public override TranslatedQualifiedName FullName => new TranslatedQualifiedName(
                new QualifiedName(SimpleName),
                NameSpan
            );
            
            public MemberDirectFcnCall(Text.Span span, NameRef name, CallSignature signature, Expression isMemberOf)
                : base(span, signature)
            {
                this.IsMemberOf = isMemberOf;
                this.SimpleName = name.Name;
                
                _name_start = name.Span.Start;
            }
        }

        public override Operations Operation { get { return Operations.DirectCall; } }

        /// <summary>
        /// Complete translated name, contains translated, original and fallback names.
        /// </summary>
        public abstract TranslatedQualifiedName FullName { get; }

        /// <summary>
        /// Original simple name of the function.
        /// </summary>
        public virtual Name SimpleName => FullName.OriginalName.Name;

        /// <summary>Simple name for methods.</summary>
        [Obsolete]
        public QualifiedName QualifiedName => FullName.Name;

        [Obsolete]
        public QualifiedName? FallbackQualifiedName => FullName.FallbackName;

        internal static DirectFcnCall CreateClone(Span span, CallSignature signature) => new CloneFcnCall(span, signature);

        public static DirectFcnCall Create(Text.Span span, TranslatedQualifiedName name, CallSignature signature) =>
            name.OriginalName.IsSimpleName && name.FallbackName.HasValue == false && name.OriginalName.Equals(name.Name.QualifiedName) && name.OriginalName.IsFullyQualifiedName == false
            ? new SimpleLocalDirectFcnCall(span, new NameRef(name.Span, name.OriginalName.Name), signature)
            : new LocalDirectFcnCall(span, name, signature)
        ;

        public static DirectFcnCall Create(Text.Span span, TranslatedQualifiedName name, CallSignature signature, Expression isMemberOf) =>
            isMemberOf != null
            ? Create(span, new NameRef(name.Span, name.OriginalName.Name), signature, isMemberOf)
            : Create(span, name, signature)
            ;

        public static DirectFcnCall Create(Text.Span span, NameRef name, CallSignature signature, Expression isMemberOf) =>
            isMemberOf != null
            ? new MemberDirectFcnCall(span, name, signature, isMemberOf)
            : new SimpleLocalDirectFcnCall(span, name, signature)
            ;
        
        protected DirectFcnCall(Text.Span span, CallSignature signature)
            : base(span, signature)
        {
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDirectFcnCall(this);
        }
    }

    #endregion

    #region IndirectFcnCall

    public sealed class IndirectFcnCall : FunctionCall
    {
        public override Operations Operation { get { return Operations.IndirectCall; } }

        public Expression/*!*/ NameExpr { get; }

        public override Text.Span NameSpan => NameExpr.Span;

        public override Expression IsMemberOf { get; }

        public IndirectFcnCall(Text.Span p, Expression/*!*/ nameExpr, CallSignature signature, Expression isMemberOf = null)
            : base(p, signature)
        {
            this.NameExpr = nameExpr;
            this.IsMemberOf = isMemberOf;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIndirectFcnCall(this);
        }
    }

    #endregion

    #region StaticMtdCall

    public abstract class StaticMtdCall : FunctionCall
    {
        public override sealed Expression IsMemberOf => null;

        public TypeRef TargetType => this.typeRef;
        protected readonly TypeRef/*!*/typeRef;

        /// <summary>
        /// Static method call.
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="className">Name of the containing class.</param>
        /// <param name="classNamePosition">Class name position.</param>
        /// <param name="signature">The call signature.</param>
        internal StaticMtdCall(Text.Span span, GenericQualifiedName className, Text.Span classNamePosition, CallSignature signature)
            : this(span, TypeRef.FromGenericQualifiedName(classNamePosition, className), signature)
        {
        }

        public StaticMtdCall(Text.Span span, TypeRef typeRef, CallSignature signature)
            : base(span, signature)
        {
            Debug.Assert(typeRef != null);

            this.typeRef = typeRef;
        }
    }

    #endregion

    #region DirectStMtdCall

    public sealed class DirectStMtdCall : StaticMtdCall
    {
        public override Operations Operation { get { return Operations.DirectStaticCall; } }

        private NameRef methodName;
        public NameRef MethodName => methodName;
        public override Text.Span NameSpan => methodName.Span;

        public DirectStMtdCall(Text.Span span, TypeRef targetType, NameRef nameRef, CallSignature signature)
            : base(span, targetType, signature)
        {
            this.methodName = nameRef;
        }

        public DirectStMtdCall(
            Text.Span span, GenericQualifiedName className, Text.Span classNamePosition,
            Name methodName, Text.Span methodNamePosition,
            CallSignature signature)
            : base(span, className, classNamePosition, signature)
        {
            this.methodName = new NameRef(methodNamePosition, methodName);
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDirectStMtdCall(this);
        }
    }

    #endregion

    #region IndirectStMtdCall

    public sealed class IndirectStMtdCall : StaticMtdCall
    {
        public override Operations Operation { get { return Operations.IndirectStaticCall; } }

        /// <summary>Expression that represents name of method.</summary>
        public Expression/*!*/ MethodNameExpression => _methodNameExpr;
        Expression/*!*/_methodNameExpr;

        public override Text.Span NameSpan => _methodNameExpr.Span;


        public IndirectStMtdCall(Text.Span span,
                                 GenericQualifiedName className, Text.Span classNamePosition, Expression/*!*/ nameExpr,
                                 CallSignature signature)
            : base(span, className, classNamePosition, signature)
        {
            _methodNameExpr = nameExpr;
        }

        public IndirectStMtdCall(Text.Span span,
                                 TypeRef/*!*/typeRef, Expression/*!*/ mtdNameVar,
                                 CallSignature signature)
            : base(span, typeRef, signature)
        {
            _methodNameExpr = mtdNameVar;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIndirectStMtdCall(this);
        }
    }

    #endregion
}
