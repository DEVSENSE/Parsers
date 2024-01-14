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
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    #region StaticFieldUse

    /// <summary>
    /// Base class for static field uses.
    /// </summary>
    public abstract class StaticFieldUse : VariableUse
    {
        /// <summary>Position of the field name.</summary>
        public abstract Text.Span NameSpan { get; }

        public TypeRef TargetType { get { return targetType; } }
        protected TypeRef targetType;

        internal StaticFieldUse(Span span, GenericQualifiedName typeName, Span typeNamePosition)
            : this(span, TypeRef.FromGenericQualifiedName(typeNamePosition, typeName))
        {
        }

        public StaticFieldUse(Span span, TypeRef typeRef)
            : base(span)
        {
            Debug.Assert(typeRef != null);

            this.targetType = typeRef;
        }
    }

    #endregion

    #region DirectStFldUse

    /// <summary>
    /// Direct static field uses (a static field accessed by field identifier).
    /// </summary>
    public sealed class DirectStFldUse : StaticFieldUse
    {
        public override Operations Operation { get { return Operations.DirectStaticFieldUse; } }

        private readonly VariableNameRef propertyName;

        /// <summary>Name of static field being accessed</summary>
        public VariableName PropertyName => propertyName.Name;

        /// <summary>
        /// <see cref="PropertyName"/> position within AST.
        /// </summary>
        public override Span NameSpan => propertyName.Span;

        public DirectStFldUse(Span span, TypeRef typeRef, VariableNameRef propertyName)
            : base(span, typeRef)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDirectStFldUse(this);
        }
    }

    #endregion

    #region IndirectStFldUse

    /// <summary>
    /// Indirect static field used - a static field accessed by run-time evaluated name.
    /// </summary>
    public sealed class IndirectStFldUse : StaticFieldUse
    {
        public override Operations Operation { get { return Operations.IndirectStaticFieldUse; } }

        /// <summary>Expression that produces name of the field</summary>
        public Expression/*!*/ FieldNameExpr { get { return fieldNameExpr; } internal set { fieldNameExpr = value; } }
        private Expression/*!*/ fieldNameExpr;

        public override Span NameSpan => fieldNameExpr.Span;

        public IndirectStFldUse(Text.Span span, TypeRef typeRef, Expression/*!*/ fieldNameExpr)
            : base(span, typeRef)
        {
            this.fieldNameExpr = fieldNameExpr;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIndirectStFldUse(this);
        }
    }

    #endregion
}
