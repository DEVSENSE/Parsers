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

using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
    #region ConstantUse

    /// <summary>
    /// Base class for constant uses.
    /// </summary>
    public abstract class ConstantUse : VarLikeConstructUse
    {
        public override Span Span { get; protected set; }

        public override Expression IsMemberOf => null;

        public ConstantUse(Text.Span span)
            : base(span)
        {
        }
    }

    #endregion

    #region GlobalConstUse

    /// <summary>
    /// Global constant use (constants defined by <c>define</c> function).
    /// </summary>
    public sealed class GlobalConstUse : ConstantUse
    {
        public override Operations Operation { get { return Operations.GlobalConstUse; } }

        TranslatedQualifiedName _fullName;

        /// <summary>
        /// Complete translated name, contians translated, original and fallback names.
        /// </summary>
        public TranslatedQualifiedName FullName => _fullName;

        public QualifiedName Name => _fullName.Name;

        /// <summary>
        /// Name used when the <see cref="Name"/> is not found. Used when reading global constant in a namespace context.
        /// </summary>
        internal QualifiedName? FallbackName => _fullName.FallbackName;


        public GlobalConstUse(Text.Span span, TranslatedQualifiedName name)
            : base(span)
        {
            this._fullName = name;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitGlobalConstUse(this);
        }
    }

    #endregion

    #region ClassConstUse

    /// <summary>
    /// Class constant use.
    /// </summary>
    public abstract class ClassConstUse : ConstantUse, IStaticMemberUse
    {
        public override Operations Operation { get { return Operations.ClassConstUse; } }

        /// <summary>
        /// Class type reference.
        /// </summary>
        public TypeRef/*!*/TargetType { get; }

        /// <summary>
        /// Position of the constant name itself within the constant use.
        /// </summary>
        public abstract Text.Span NamePosition { get; }

        protected ClassConstUse(Text.Span span, TypeRef/*!*/typeRef)
            : base(span)
        {
            Debug.Assert(typeRef != null);
            
            this.TargetType = typeRef;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitClassConstUse(this);
        }
    }

    public class DirectClassConstUse : ClassConstUse
    {
        public VariableName Name { get; }

        public override Text.Span NamePosition { get; }

        public DirectClassConstUse(Text.Span span, TypeRef/*!*/typeRef, Text.Span nameSpan, string name)
            : base(span, typeRef)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));

            this.Name = new VariableName(name);
            this.NamePosition = nameSpan;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDirectClassConstUse(this);
        }
    }

    /// <summary>
    /// Class constant use.
    /// </summary>
    public class IndirectClassConstUse : ClassConstUse
    {
        /// <summary>
        /// The indirect class name.
        /// </summary>
        public IExpression NameExpression { get; }

        public override Text.Span NamePosition => NameExpression.Span;

        public IndirectClassConstUse(Text.Span span, TypeRef typeRef, IExpression nameExpr)
            : base(span, typeRef)
        {
            this.NameExpression = nameExpr ?? throw new ArgumentNullException(nameof(nameExpr));
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIndirectClassConstUse(this);
        }
    }

    /// <summary>
    /// Pseudo class constant use.
    /// </summary>
    public sealed class PseudoClassConstUse : ClassConstUse
    {
        /// <summary>
        /// Possible types of pseudo class constant.
        /// </summary>
        public enum Types
        {
            Class,
        }

        /// <summary>Type of pseudoconstant</summary>
        public Types Type { get; }

        public VariableName Name => TypeToName(this.Type);

        public override Text.Span NamePosition { get; }

        /// <summary>
        /// Gets string representation of <see cref="Types"/>.
        /// </summary>
        public static VariableName TypeToName(Types type)
        {
            return type switch
            {
                Types.Class => new VariableName("class"),
                _ => throw new ArgumentException(),
            };
        }

        internal PseudoClassConstUse(Text.Span span, GenericQualifiedName className, Text.Span classNamePosition, Types type, Text.Span namePosition)
            : this(span, ClassTypeRef.FromGenericQualifiedName(classNamePosition, className), type, namePosition)
        {
        }

        public PseudoClassConstUse(Text.Span span, TypeRef/*!*/typeRef, Types type, Text.Span namePosition)
            : base(span, typeRef)
        {
            this.Type = type;
            this.NamePosition = namePosition;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitPseudoClassConstUse(this);
        }
    }

    #endregion

    #region PseudoConstUse

    /// <summary>
    /// Pseudo-constant use (PHP keywords: __LINE__, __FILE__, __DIR__, __FUNCTION__, __METHOD__, __PROPERTY__, __CLASS__, __TRAIT__, __NAMESPACE__)
    /// </summary>
    public sealed class PseudoConstUse : Expression
    {
        int _span_start = -1;

        public override Span Span
        {
            get => _span_start < 0 ? Span.Invalid : new Span(_span_start, TypeToDefaultString(this.Type).Length);
            protected set => _span_start = value.IsValid ? value.Start : -1;
        }

        public override Operations Operation { get { return Operations.PseudoConstUse; } }

        public enum Types { Line, File, Class, Trait, Function, Method, Property, Namespace, Dir }

        /// <summary>Type of pseudoconstant</summary>
        public Types Type { get; }

        /// <summary>
        /// Pseudo-constant name corresponding to the <see cref="Type"/>.
        /// </summary>
        public Name TypeName => new Name(TypeToDefaultString(this.Type));

        /// <summary>
        /// Gets pseudo-constant corresponding to given type <paramref name="t"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string TypeToDefaultString(Types t)
        {
            switch (t)
            {
                case Types.Line: return "__LINE__";
                case Types.File: return "__FILE__";
                case Types.Class: return "__CLASS__";
                case Types.Trait: return "__TRAIT__";
                case Types.Function: return "__FUNCTION__";
                case Types.Method: return "__METHOD__";
                case Types.Property: return "__PROPERTY__";
                case Types.Namespace: return "__NAMESPACE__";
                case Types.Dir: return "__DIR__";
                default:
                    throw new ArgumentOutOfRangeException(nameof(t));
            }
        }

        public PseudoConstUse(Text.Span span, Types type)
            : base(span)
        {
            Debug.Assert(TypeToDefaultString(type) != null);
            this.Type = type;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitPseudoConstUse(this);
        }
    }

    #endregion
}
