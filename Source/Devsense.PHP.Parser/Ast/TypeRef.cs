using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    #region TypeRef

    /// <summary>
    /// Represents a use of a class name.
    /// </summary>
    public abstract class TypeRef : LangElement
    {
        /// <summary>
        /// Immutable empty list of <see cref="TypeRef"/>.
        /// </summary>
		internal static readonly List<TypeRef>/*!*/EmptyList = new List<TypeRef>();

        /// <summary>
        /// Gets qualified name of the type if possible. Indirect type reference and multiple type references gets no value.
        /// </summary>
        public abstract QualifiedName? QualifiedName { get; }

        /// <summary>
        /// Gets textual representation of the type name.
        /// </summary>
        public override abstract string ToString();

        protected TypeRef(Span span)
            : base(span)
        {
        }

        #region Factory

        internal static TypeRef FromGenericQualifiedName(Span span, GenericQualifiedName qname)
        {
            var tref = new DirectTypeRef(span, qname.QualifiedName);
            if (qname.IsGeneric)
            {
                return new GenericTypeRef(span, tref, qname.GenericParams.Select(p => FromObject(Span.Invalid, p)).ToList());
            }
            else
            {
                return tref;
            }
        }

        internal static TypeRef FromObject(Span span, object obj)
        {
            if (obj is PrimitiveTypeName) return new PrimitiveTypeRef(span, (PrimitiveTypeName)obj);
            if (obj is QualifiedName) return new DirectTypeRef(span, (QualifiedName)obj);
            if (obj is GenericQualifiedName) return FromGenericQualifiedName(span, (GenericQualifiedName)obj);
            if (obj is TypeRef) return (TypeRef)obj;

            throw new ArgumentException("obj");
        }

        /// <summary>
        /// Creates direct type reference from a string.
        /// </summary>
        /// <param name="span">Position of the name.</param>
        /// <param name="name">Input string, primitive type name or a class name.</param>
        /// <param name="naming">Optional the naming context. The name will be translated.</param>
        /// <returns>Type reference. Cannot be <c>null</c>.</returns>
        public static TypeRef FromString(Span span, string name, NamingContext naming = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            // multiple types
            var sepindex = name.IndexOf(PHPDocBlock.TypeVarDescTag.TypeNamesSeparator);
            if (sepindex >= 0)
            {
                var names = name.Split(new char[] { PHPDocBlock.TypeVarDescTag.TypeNamesSeparator });
                Debug.Assert(names.Length > 1);
                var trefs = new TypeRef[names.Length];
                int offset = 0;
                for (int i = 0; i < names.Length; i++)
                {
                    var str = names[i];
                    trefs[i] = FromString(new Span(span.Start + offset, str.Length), str, naming);
                    offset += str.Length + 1; // + separator
                }
                return new MultipleTypeRef(span, trefs);
            }

            // nullable
            if (name.EndsWith("?", StringComparison.Ordinal))
            {
                return new NullableTypeRef(span, FromString(new Span(span.Start, span.Length - 1), name.Remove(name.Length - 1), naming));
            }

            // type names
            switch (name.ToLowerInvariant())
            {
                // primitive types

                case "bool":
                case "boolean":
                    return new PrimitiveTypeRef(span, new PrimitiveTypeName(Syntax.QualifiedName.Boolean));
                case "int":
                case "integer":
                    return new PrimitiveTypeRef(span, new PrimitiveTypeName(Syntax.QualifiedName.Integer));
                case "long":
                    return new PrimitiveTypeRef(span, new PrimitiveTypeName(Syntax.QualifiedName.LongInteger));
                case "float":
                case "double":
                    return new PrimitiveTypeRef(span, new PrimitiveTypeName(Syntax.QualifiedName.Double));
                case "string":
                    return new PrimitiveTypeRef(span, new PrimitiveTypeName(Syntax.QualifiedName.String));
                case "resource":
                    return new PrimitiveTypeRef(span, new PrimitiveTypeName(Syntax.QualifiedName.Resource));
                case "callable":
                    return new PrimitiveTypeRef(span, new PrimitiveTypeName(Syntax.QualifiedName.Callable));
                case "array":
                    return new PrimitiveTypeRef(span, new PrimitiveTypeName(Syntax.QualifiedName.Array));

                // direct types

                default:
                    var qname = Syntax.QualifiedName.Parse(name, false);
                    if (naming != null)
                    {
                        qname = Syntax.QualifiedName.TranslateAlias(qname, naming.Aliases, naming.CurrentNamespace);
                    }
                    return new DirectTypeRef(span, qname);

            }
        }

        #endregion
    }

    #endregion

    #region PrimitiveTypeRef

    /// <summary>
    /// Primitive type reference.
    /// </summary>
    [DebuggerDisplay("{_typeName.Name,nq}")]
    public sealed class PrimitiveTypeRef : TypeRef
    {
        /// <summary>
        /// Gets underlaying primitive type name.
        /// </summary>
        public PrimitiveTypeName PrimitiveTypeName => _typeName;
        private PrimitiveTypeName _typeName;

        public PrimitiveTypeRef(Span span, PrimitiveTypeName name)
            : base(span)
        {
            _typeName = name;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitPrimitiveTypeRef(this);

        public override QualifiedName? QualifiedName => _typeName.QualifiedName;

        public override string ToString() => _typeName.QualifiedName.ToString();
    }

    #endregion

    #region DirectTypeRef

    /// <summary>
    /// Direct use of class name.
    /// </summary>
    [DebuggerDisplay("{_className,nq}")]
    public sealed class DirectTypeRef : TypeRef, IEquatable<DirectTypeRef>
    {
        /// <summary>
        /// Non nullable <see cref="QualifiedName"/>.
        /// </summary>
        public QualifiedName ClassName => _className;
        private readonly QualifiedName _className;

        public override QualifiedName? QualifiedName => ClassName;

        public DirectTypeRef(Span span, QualifiedName className)
            : base(span)
        {
            _className = className;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitDirectTypeRef(this);

        public override string ToString() => _className.ToString();

        #region IEquatable

        public override bool Equals(object obj) => ((IEquatable<DirectTypeRef>)this).Equals(obj as DirectTypeRef);

        public override int GetHashCode() => _className.GetHashCode();

        bool IEquatable<DirectTypeRef>.Equals(DirectTypeRef other) => other != null && other._className == _className;

        #endregion
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
        public VariableUse/*!*/ ClassNameVar => _classNameVar;
        private readonly VariableUse/*!*/ _classNameVar;

        public override QualifiedName? QualifiedName => null;

        public IndirectTypeRef(Span span, VariableUse/*!*/ classNameVar)
            : base(span)
        {
            Debug.Assert(classNameVar != null);
            _classNameVar = classNameVar;
        }

        public override string ToString() => string.Empty;

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitIndirectTypeRef(this);
    }

    #endregion

    #region NullableTypeRef

    /// <summary>
    /// A nullable type reference (target type can be <c>null</c>).
    /// </summary>
    [DebuggerDisplay("{_targetType,nq}?")]
    public sealed class NullableTypeRef : TypeRef
    {
        /// <summary>
        /// <see cref="VariableUse"/> which value in runtime contains the name of the type.
        /// </summary>
        public TypeRef/*!*/ TargetType => _targetType;
        private readonly TypeRef/*!*/ _targetType;

        public override QualifiedName? QualifiedName => _targetType.QualifiedName;

        public NullableTypeRef(Span span, TypeRef/*!*/ targetType)
            : base(span)
        {
            Debug.Assert(targetType != null);
            Debug.Assert(!(targetType is NullableTypeRef), "Nullable of a nullable is not allowed.");
            _targetType = targetType;
        }

        public override string ToString() => _targetType.ToString() + "?";

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitNullableTypeRef(this);
    }

    #endregion

    #region MultipleTypeRef

    /// <summary>
    /// <see cref="TypeRef"/> referring to multiple types.
    /// </summary>
    public sealed class MultipleTypeRef : TypeRef
    {
        /// <summary>
        /// List of types represented by this reference.
        /// </summary>
        public IList<TypeRef>/*!*/ MultipleTypes { get { return this._types; } }
        private readonly IList<TypeRef>/*!!*/ _types;

        public override QualifiedName? QualifiedName => null;

        public MultipleTypeRef(Span span, IList<TypeRef>/*!*/ multipleTypes)
            : base(span)
        {
            Debug.Assert(multipleTypes != null);
            Debug.Assert(multipleTypes.All(x => x != null));

            this._types = multipleTypes;
        }

        public override string ToString() => string.Join(PHPDocBlock.TypeVarDescTag.TypeNamesSeparator.ToString(), _types);

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitMultipleTypeRef(this);
    }

    #endregion

    #region GenericTypeRef

    /// <summary>
    /// Type reference constructed with generic arguments.
    /// </summary>
    [DebuggerDisplay("{_targetType,nq}`{_genericArgs.Count}")]
    public sealed class GenericTypeRef : TypeRef
    {
        #region Nested class: GenericQualifiedNameResolver

        /// <summary>
        /// Helper class that resolves generic qualified name of a <see cref="TypeRef"/>.
        /// </summary>
        private sealed class GenericQualifiedNameResolver : TreeVisitor
        {
            /// <summary>
            /// Result of the type name resolving.
            /// </summary>
            public GenericQualifiedName? GenericQualifiedName => _results.Peek();

            readonly Stack<GenericQualifiedName?> _results = new Stack<GenericQualifiedName?>();

            public override void VisitElement(LangElement element)
            {
                Debug.Assert(element is TypeRef);
                var stack = _results.Count;
                base.VisitElement(element);
                Debug.Assert(_results.Count == stack + 1);
            }

            public override void VisitDirectTypeRef(DirectTypeRef x)
            {
                _results.Push(new GenericQualifiedName(x.ClassName));
            }

            public override void VisitIndirectTypeRef(IndirectTypeRef x)
            {
                _results.Push(null);
            }

            public override void VisitNullableTypeRef(NullableTypeRef x)
            {
                VisitElement(x.TargetType);
            }

            public override void VisitMultipleTypeRef(MultipleTypeRef x)
            {
                if (x.MultipleTypes.Count == 1)
                {
                    VisitElement(x.MultipleTypes[0]);
                }
                else
                {
                    _results.Push(null);
                }
            }

            public override void VisitPrimitiveTypeRef(PrimitiveTypeRef x)
            {
                _results.Push(new GenericQualifiedName(x.PrimitiveTypeName.QualifiedName));
            }

            public override void VisitGenericTypeRef(GenericTypeRef x)
            {
                VisitElement(x.TargetType);
                var t = _results.Pop();

                if (t.HasValue && !t.Value.IsGeneric)
                {
                    bool resolved = true;
                    var generics = new object[x.GenericParams.Count];
                    for (int i = 0; i < generics.Length; i++)
                    {
                        VisitElement(x.GenericParams[i]);
                        var g = _results.Pop();
                        if (g.HasValue)
                        {
                            generics[i] = g.Value;
                        }
                        else
                        {
                            resolved = false;
                        }
                    }

                    if (resolved)
                    {
                        _results.Push(new GenericQualifiedName(t.Value.QualifiedName, generics));
                        return;
                    }
                }

                //
                _results.Push(null);
            }
        }

        #endregion

        /// <summary>
        /// List of generic parameters.
        /// </summary>
        public List<TypeRef>/*!*/ GenericParams => _genericArgs;
        private readonly List<TypeRef>/*!*/_genericArgs;

        /// <summary>
        /// List of types represented by this reference.
        /// </summary>
        public TypeRef TargetType => this._targetType;
        private readonly TypeRef _targetType;

        public override QualifiedName? QualifiedName => _targetType.QualifiedName;

        public override string ToString() => $"{_targetType}<:{string.Join(",", _genericArgs)}:>";

        /// <summary>
        /// Gets generic qualified name if all the components are resolved.
        /// </summary>
        public GenericQualifiedName? ResolveGenericQualifiedName()
        {
            var visitor = new GenericQualifiedNameResolver();
            visitor.VisitElement(this);

            return visitor.GenericQualifiedName;
        }

        public GenericTypeRef(Span span, TypeRef targetType, List<TypeRef> genericParams)
            : base(span)
        {
            Debug.Assert(targetType != null && genericParams != null);

            _targetType = targetType;
            _genericArgs = genericParams;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitGenericTypeRef(this);
    }

    #endregion

    #region AnonymousTypeRef

    /// <summary>
    /// Direct use of class name.
    /// </summary>
    public sealed class AnonymousTypeRef : TypeRef
    {
        /// <summary>
        /// Non nullable <see cref="QualifiedName"/>.
        /// </summary>
        public AnonymousTypeDecl TypeDeclaration => _typeDeclaration;
        private readonly AnonymousTypeDecl _typeDeclaration;

        public override QualifiedName? QualifiedName => null;

        public AnonymousTypeRef(Span span, AnonymousTypeDecl typeDeclaration)
            : base(span)
        {
            _typeDeclaration = typeDeclaration;
        }

        public override string ToString() => "anonymous class";

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitAnonymousTypeRef(this);
    }

    #endregion
}
