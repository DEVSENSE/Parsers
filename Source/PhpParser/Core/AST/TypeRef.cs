using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using PHP.Syntax;
using PhpParser;
using PHP.Core.Text;

namespace PHP.Core.AST
{
    #region TypeRef

    /// <summary>
    /// Represents a use of a class name.
    /// </summary>
    public abstract class TypeRef : LangElement
    {
        virtual public GenericQualifiedName GenericQualifiedName
        {
            get
            {
                return new GenericQualifiedName(this.QualifiedName);
            }
        }

        internal abstract QualifiedName QualifiedName { get; }

        public TypeRef(Text.Span span)
            : base(span)
        {
        }

        /// <summary>
		/// Gets the static type reference or <B>null</B> if the reference cannot be resolved at compile time.
		/// </summary>
		internal abstract object ToStaticTypeRef(IErrorSink<Span> errors, SourceUnit sourceUnit);

        internal static object[]/*!!*/ ToStaticTypeRefs(List<TypeRef>/*!*/ typeRefs, IErrorSink<Span> errors, SourceUnit sourceUnit)
        {
            if (typeRefs == null || typeRefs.Count == 0)
                return ArrayUtils.EmptyObjects;

            object[] result = new object[typeRefs.Count];

            for (int i = 0; i < typeRefs.Count; i++)
            {
                if ((result[i] = typeRefs[i].ToStaticTypeRef(errors, sourceUnit)) == null)
                {
                    if (errors != null)
                        errors.Error(typeRefs[i].Span, Errors.GenericParameterMustBeType);

                    result[i] = new PrimitiveTypeName(QualifiedName.Object);
                }
            }

            return result;
        }
    }

    #endregion

    #region PrimitiveTypeRef

    /// <summary>
    /// Primitive type reference.
    /// </summary>
    public sealed class PrimitiveTypeRef : TypeRef
    {
        private PrimitiveTypeName typeName;

        public PrimitiveTypeRef(Text.Span span, PrimitiveTypeName name)
            : base(span)
        {
            this.typeName = name;
        }

        internal override object ToStaticTypeRef(IErrorSink<Span> errors, SourceUnit sourceUnit)
        {
            return typeName;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitPrimitiveTypeRef(this);
        }

        internal override QualifiedName QualifiedName
        {
            get { return this.typeName.QualifiedName; }
        }
    }

    #endregion

    #region DirectTypeRef

    /// <summary>
    /// Direct use of class name.
    /// </summary>
    public sealed class DirectTypeRef : TypeRef
    {
        public QualifiedName ClassName { get { return className; } }
        private QualifiedName className;

        internal override QualifiedName QualifiedName
        {
            get { return this.ClassName; }
        }

        internal override object ToStaticTypeRef(IErrorSink<Span> errors, SourceUnit/*!*/ sourceUnit)
        {
            return new GenericQualifiedName(className);
        }

        public DirectTypeRef(Text.Span span, QualifiedName className)
            : base(span)
        {
            this.className = className;
        }

        internal static TypeRef/*!*/FromGenericQualifiedName(Text.Span span, GenericQualifiedName genericQualifiedName)
        {
            
            if (genericQualifiedName.IsGeneric)
            {
                List<TypeRef> genericParams = new List<TypeRef>(genericQualifiedName.GenericParams.Length);
                foreach (var obj in genericQualifiedName.GenericParams)
                {
                    TypeRef objtype;
                    if (obj is GenericQualifiedName) objtype = FromGenericQualifiedName(Text.Span.Invalid, (GenericQualifiedName)obj);
                    else if (obj is PrimitiveTypeName) objtype = new PrimitiveTypeRef(Text.Span.Invalid, (PrimitiveTypeName)obj);
                    else objtype = new PrimitiveTypeRef(Text.Span.Invalid, new PrimitiveTypeName(QualifiedName.Object));

                    genericParams.Add(objtype);
                }
                return new GenericTypeRef(span, new DirectTypeRef(span, genericQualifiedName.QualifiedName), genericParams);
            }
            else
            {
                //if (genericQualifiedName.QualifiedName.IsPrimitiveTypeName)
                //    return new PrimitiveTypeRef(position, new PrimitiveTypeName(genericQualifiedName.QualifiedName));
            }

            return new DirectTypeRef(span, genericQualifiedName.QualifiedName);
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDirectTypeRef(this);
        }

        public override bool Equals(object obj)
        {
            var other = obj as DirectTypeRef;
            if (other == null)
                return false;

            return this.QualifiedName.Equals(other.QualifiedName);
        }

        public override int GetHashCode()
        {
            return this.QualifiedName.GetHashCode();
        }
    }

    #endregion

    #region IndirectTypeRef

    /// <summary>
    /// Indirect use of class name (through variable).
    /// </summary>
    public sealed class IndirectTypeRef : TypeRef
    {
        /// <summary>
        /// <see cref="VariableUse"/> which value in runtime contains the name of the type.
        /// </summary>
        public VariableUse/*!*/ ClassNameVar { get { return this.classNameVar; } }
        private readonly VariableUse/*!*/ classNameVar;

        internal override QualifiedName QualifiedName
        {
            get { return new QualifiedName(Name.EmptyBaseName); }
        }

        public IndirectTypeRef(Text.Span span, VariableUse/*!*/ classNameVar)
            : base(span)
        {
            Debug.Assert(classNameVar != null);

            this.classNameVar = classNameVar;
        }

        internal override object ToStaticTypeRef(IErrorSink<Span> errors, SourceUnit sourceUnit)
        {
            return null;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIndirectTypeRef(this);
        }
    }

    #endregion

    #region NullableTypeRef

    /// <summary>
    /// Indirect use of class name (through variable).
    /// </summary>
    public sealed class NullableTypeRef : TypeRef
    {
        /// <summary>
        /// <see cref="VariableUse"/> which value in runtime contains the name of the type.
        /// </summary>
        public TypeRef/*!*/ TargetType { get { return this.targetType; } }
        private readonly TypeRef/*!*/ targetType;

        internal override QualifiedName QualifiedName
        {
            get { return new QualifiedName(Name.EmptyBaseName); }
        }

        public NullableTypeRef(Text.Span span, TypeRef/*!*/ targetType)
            : base(span)
        {
            Debug.Assert(targetType != null);

            this.targetType = targetType;
        }

        internal override object ToStaticTypeRef(IErrorSink<Span> errors, SourceUnit sourceUnit)
        {
            return null;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitNullableTypeRef(this);
        }
    }

    #endregion

    #region MultipleTypeRef

    /// <summary>
    /// Indirect use of class name (through variable).
    /// </summary>
    public sealed class MultipleTypeRef : TypeRef
    {
        /// <summary>
        /// List of types represented by this reference.
        /// </summary>
        public IList<TypeRef>/*!*/ MultipleTypes { get { return this.multipleTypes; } }
        private readonly IList<TypeRef>/*!*/ multipleTypes;

        internal override QualifiedName QualifiedName
        {
            get { return new QualifiedName(Name.EmptyBaseName); }
        }

        public MultipleTypeRef(Text.Span span, IList<TypeRef>/*!*/ multipleTypes)
            : base(span)
        {
            Debug.Assert(multipleTypes != null);

            this.multipleTypes = multipleTypes;
        }

        internal override object ToStaticTypeRef(IErrorSink<Span> errors, SourceUnit sourceUnit)
        {
            return null;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitMultipleTypeRef(this);
        }
    }

    #endregion

    #region GenericTypeRef

    /// <summary>
    /// Indirect use of class name (through variable).
    /// </summary>
    public sealed class GenericTypeRef : TypeRef
    {
        /// <summary>
        /// Immutable empty list of <see cref="TypeRef"/>.
        /// </summary>
		internal static readonly List<TypeRef>/*!*/ EmptyList = new List<TypeRef>();

        /// <summary>
        /// Key to property collection to get/store generic parameters list.
        /// </summary>
        private const string GenericParamsPropertyKey = "GenericParams";

        /// <summary>
        /// List of generic parameters.
        /// </summary>
        public List<TypeRef>/*!*/ GenericParams
        {
            get
            {
                return this.Properties[GenericParamsPropertyKey] as List<TypeRef> ?? GenericTypeRef.EmptyList;
            }
            private set
            {
                if (value != null && value.Count > 0)
                    this.Properties[GenericParamsPropertyKey] = value;
                else
                    this.Properties.RemoveProperty(GenericParamsPropertyKey);
            }
        }

        /// <summary>
        /// List of types represented by this reference.
        /// </summary>
        public TypeRef TargetType { get { return this.targetType; } }
        private readonly TypeRef targetType;

        internal override QualifiedName QualifiedName
        {
            get { return targetType.QualifiedName; }
        }
        override public GenericQualifiedName GenericQualifiedName
        {
            get
            {
                return new GenericQualifiedName(this.QualifiedName, ToStaticTypeRefs(this.GenericParams, null, null));
            }
        }

        public GenericTypeRef(Text.Span span, TypeRef targetType, List<TypeRef> genericParams)
            : base(span)
        {
            Debug.Assert(targetType != null && genericParams != null);

            this.GenericParams = genericParams;
            this.targetType = targetType;
        }

        internal override object ToStaticTypeRef(IErrorSink<Span> errors, SourceUnit sourceUnit)
        {
            return null;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitGenericTypeRef(this);
        }
    }

    #endregion
}
