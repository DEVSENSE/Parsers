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

namespace Devsense.PHP.Syntax.Ast
{
	#region FunctionCall

	public abstract class FunctionCall : VarLikeConstructUse
	{
		protected CallSignature callSignature;
        /// <summary>GetUserEntryPoint calling signature</summary>
        public CallSignature CallSignature { get { return callSignature; } internal set { callSignature = value; } }

		/// <summary>
        /// Position of called function name in source code.
        /// </summary>
        public abstract Text.Span NameSpan { get; }

        public FunctionCall(Text.Span span, IList<ActualParam> parameters, IList<TypeRef> genericParams)
			: base(span)
		{
			Debug.Assert(parameters != null);

			this.callSignature = new CallSignature(parameters, genericParams);
		}
	}

	#endregion

	#region DirectFcnCall

    public sealed class DirectFcnCall : FunctionCall
	{
        public override Operations Operation { get { return Operations.DirectCall; } }

        /// <summary>
		/// Simple name for methods.
		/// </summary>
		private QualifiedNameRef qualifiedName;
        private QualifiedName? fallbackQualifiedName;
        /// <summary>Simple name for methods.</summary>
        public QualifiedName QualifiedName { get { return qualifiedName.QualifiedName; } }
        public QualifiedName? FallbackQualifiedName { get { return fallbackQualifiedName; } }
        public override Text.Span NameSpan => qualifiedName.Span;

        public DirectFcnCall(Text.Span span,
            QualifiedName qualifiedName, QualifiedName? fallbackQualifiedName, Text.Span qualifiedNameSpan,
            IList<ActualParam> parameters, IList<TypeRef> genericParams)
            : base(span, parameters, genericParams)
		{
            this.qualifiedName = new QualifiedNameRef(qualifiedNameSpan, qualifiedName);
            this.fallbackQualifiedName = fallbackQualifiedName;
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

		public Expression/*!*/ NameExpr { get { return nameExpr; } }
		internal Expression/*!*/ nameExpr;
        public override Text.Span NameSpan => NameExpr.Span;

        public IndirectFcnCall(Text.Span p, Expression/*!*/ nameExpr, IList<ActualParam> parameters, IList<TypeRef> genericParams)
            : base(p, parameters, genericParams)
		{
			this.nameExpr = nameExpr;
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
        public TypeRef TargetType { get { return this.typeRef; } }
        protected readonly TypeRef/*!*/typeRef;

        /// <summary>
        /// Position of <see cref="TargetType"/> in source code.
        /// </summary>
        public Text.Span ClassNamePosition { get { return this.typeRef.Span; } }

        internal StaticMtdCall(Text.Span span, GenericQualifiedName className, Text.Span classNamePosition, IList<ActualParam> parameters, IList<TypeRef> genericParams)
            : this(span, TypeRef.FromGenericQualifiedName(classNamePosition, className), parameters, genericParams)
		{	
		}

        public StaticMtdCall(Text.Span span, TypeRef typeRef, IList<ActualParam> parameters, IList<TypeRef> genericParams)
            : base(span, parameters, genericParams)
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
        public NameRef MethodName { get { return methodName; } }
        public override Text.Span NameSpan => MethodName.Span;

        public DirectStMtdCall(Text.Span span, ClassConstUse/*!*/ classConstant,
            IList<ActualParam>/*!*/ parameters, IList<TypeRef>/*!*/ genericParams)
			: base(span, classConstant.TargetType, parameters, genericParams)
		{
			this.methodName = new NameRef(classConstant.NamePosition, classConstant.Name.Value);
		}

        public DirectStMtdCall(Text.Span span, GenericQualifiedName className, Text.Span classNamePosition,
            Name methodName, Text.Span methodNamePosition, IList<ActualParam> parameters, IList<TypeRef> genericParams)
			: base(span, className, classNamePosition, parameters, genericParams)
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

		private CompoundVarUse/*!*/ methodNameVar;
        /// <summary>Expression that represents name of method</summary>
        public CompoundVarUse/*!*/ MethodNameVar { get { return methodNameVar; } }
        public override Text.Span NameSpan => MethodNameVar.Span;

        public IndirectStMtdCall(Text.Span span,
                                 GenericQualifiedName className, Text.Span classNamePosition, CompoundVarUse/*!*/ mtdNameVar,
	                             IList<ActualParam> parameters, IList<TypeRef> genericParams)
            : base(span, className, classNamePosition, parameters, genericParams)
		{
			this.methodNameVar = mtdNameVar;
		}

        public IndirectStMtdCall(Text.Span span,
                                 TypeRef/*!*/typeRef, CompoundVarUse/*!*/ mtdNameVar,
                                 IList<ActualParam> parameters, IList<TypeRef> genericParams)
            : base(span, typeRef, parameters, genericParams)
        {
            this.methodNameVar = mtdNameVar;
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
