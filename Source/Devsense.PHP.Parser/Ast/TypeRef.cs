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
            var tref = new ClassTypeRef(span, qname.QualifiedName);
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
            if (obj is QualifiedName) return new ClassTypeRef(span, (QualifiedName)obj);
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
            if (name[0] == '?')
            {
                return new NullableTypeRef(span, FromString(new Span(span.Start + 1, span.Length - 1), name.Substring(1), naming));
            }

            //
            var qname = Syntax.QualifiedName.Parse(name, false);

            // primitive types
            if (qname.IsPrimitiveTypeName)
            {
                return new PrimitiveTypeRef(span, new PrimitiveTypeName(qname));
            }
            else
            {
                if (naming != null && !qname.IsReservedClassName)
                {
                    qname = Syntax.QualifiedName.TranslateAlias(qname, naming.Aliases, naming.CurrentNamespace);
                }

                // direct types
                return new ClassTypeRef(span, qname);
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

    #region ReservedTypeRef

    /// <summary>
    /// Primitive type reference.
    /// </summary>
    [DebuggerDisplay("{_typeName.Name,nq}")]
    public sealed class ReservedTypeRef : TypeRef
    {
        public enum ReservedType
        {
            /// <summary>
            /// Parent class reference.
            /// </summary>
           parent,

            /// <summary>
            /// This class reference.
            /// </summary>
            self,

            /// <summary>
            /// Static class reference.
            /// </summary>
            @static
        }

        public static readonly Dictionary<Name, ReservedTypeRef.ReservedType> ReservedTypes = new Dictionary<Name, ReservedTypeRef.ReservedType>() {
            { Name.StaticClassName, ReservedTypeRef.ReservedType.@static },
            { Name.SelfClassName, ReservedTypeRef.ReservedType.self },
            { Name.ParentClassName, ReservedTypeRef.ReservedType.parent }
        };

        ReservedType Reserved => _reservedType;
        ReservedType _reservedType;

        public ReservedTypeRef(Span span, ReservedType type)
            : base(span)
        {
            _reservedType = type;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitReservedTypeRef(this);

        public override QualifiedName? QualifiedName => new QualifiedName(new Name(_reservedType.ToString()));

        public override string ToString() => QualifiedName.ToString();
    }

    #endregion

    #region DirectTypeRef

    /// <summary>
    /// Direct use of class name.
    /// </summary>
    [DebuggerDisplay("{_className,nq}")]
    public sealed class ClassTypeRef : TypeRef, IEquatable<ClassTypeRef>
    {
        /// <summary>
        /// Non nullable <see cref="QualifiedName"/>.
        /// </summary>
        public QualifiedName ClassName => _className;
        private readonly QualifiedName _className;

        public override QualifiedName? QualifiedName => ClassName;

        public ClassTypeRef(Span span, QualifiedName className)
            : base(span)
        {
            Debug.Assert(!className.IsPrimitiveTypeName);
            _className = className;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitClassTypeRef(this);

        public override string ToString() => _className.ToString();

        #region IEquatable

        public override bool Equals(object obj) => ((IEquatable<ClassTypeRef>)this).Equals(obj as ClassTypeRef);

        public override int GetHashCode() => _className.GetHashCode();

        bool IEquatable<ClassTypeRef>.Equals(ClassTypeRef other) => other != null && other._className == _className;

        #endregion
    }

    #endregion

    #region AliasedTypeRef

    /// <summary>
    /// Direct use of class name.
    /// </summary>
    [DebuggerDisplay("{_className,nq}")]
    public sealed class AliasedTypeRef : TypeRef, IEquatable<AliasedTypeRef>
    {
        /// <summary>
        /// Non nullable <see cref="QualifiedName"/>.
        /// </summary>
        public QualifiedName ClassName => _className;
        private readonly QualifiedName _className;

        public override QualifiedName? QualifiedName => ClassName;

        /// <summary>
        /// Original type reference before alias translation.
        /// Non nullable <see cref="ClassTypeRef"/>.
        /// </summary>
        public ClassTypeRef OriginalType => _originalType;
        private readonly ClassTypeRef _originalType;

        public AliasedTypeRef(Span span, QualifiedName className, ClassTypeRef originalType)
            : base(span)
        {
            Debug.Assert(!className.IsPrimitiveTypeName);
            _className = className;
            _originalType = originalType;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor) => visitor.VisitAliasedTypeRef(this);

        public override string ToString() => _className.ToString();

        #region IEquatable

        public override bool Equals(object obj) => ((IEquatable<AliasedTypeRef>)this).Equals(obj as AliasedTypeRef);

        public override int GetHashCode() => _className.GetHashCode();

        bool IEquatable<AliasedTypeRef>.Equals(AliasedTypeRef other) => other != null && other._originalType.Equals(_originalType);

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

        public override string ToString() => "?" + _targetType.ToString();

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

            public override void VisitClassTypeRef(ClassTypeRef x)
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
