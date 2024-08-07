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

using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Devsense.PHP.Syntax.Ast
{
    #region FormalTypeParam

    public sealed class FormalTypeParam : LangElement
    {
        public Name Name { get { return name; } }
        private readonly Name name;

        /// <summary>
        /// Either <see cref="QualifiedName"/>, <see cref="GenericQualifiedName"/>, or <B>null</B>.
        /// </summary>
        public object DefaultType { get { return defaultType; } }
        private readonly object defaultType;

        #region Construction

        public FormalTypeParam(Text.Span span, Name name, object defaultType)
            : base(span)
        {
            Debug.Assert(defaultType == null || defaultType is QualifiedName || defaultType is GenericQualifiedName);

            this.name = name;
            this.defaultType = defaultType;
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitFormalTypeParam(this);
        }
    }

    #endregion

    #region TypeSignature

    public class TypeSignature
    {
        internal FormalTypeParam[]/*!!*/ TypeParams { get { return typeParams; } }
        private readonly FormalTypeParam[]/*!!*/ typeParams;

        internal static readonly TypeSignature s_empty = new TypeSignature(EmptyArray<FormalTypeParam>.Instance);

        #region Construction

        public TypeSignature(FormalTypeParam[]/*!!*/typeParams)
        {
            this.typeParams = typeParams ?? EmptyArray<FormalTypeParam>.Instance;
        }

        #endregion
    }

    #endregion

    #region TypeDecl

    /// <summary>
    /// Represents a class or an interface declaration.
    /// </summary>
    public abstract class TypeDecl : Statement
    {
        #region Nested class: TypeDeclData

        /// <summary>
        /// Additional data for <see cref="TypeDecl"/>, mostly not needed to save mem.
        /// </summary>
        class TypeDeclData
        {
            enum Fields
            {
                IsConditional = 1,
                IsPartial = 2,
            }

            Fields _fields;

            public bool IsConditional
            {
                get => (_fields & Fields.IsConditional) != 0;
                set
                {
                    if (value) _fields |= Fields.IsConditional;
                    else _fields &= ~Fields.IsConditional;
                }
            }

            public bool IsPartial
            {
                get => (_fields & Fields.IsPartial) != 0;
                set
                {
                    if (value) _fields |= Fields.IsPartial;
                    else _fields &= ~Fields.IsPartial;
                }
            }

            public static bool TryGet(TypeDecl tdecl, out TypeDeclData data) => tdecl.TryGetProperty<TypeDeclData>(out data);

            public static TypeDeclData GetOrAdd(TypeDecl tdecl) => TryGet(tdecl, out var data) ? data : Set(tdecl, new TypeDeclData());

            public static TypeDeclData Set(TypeDecl tdecl, TypeDeclData data)
            {
                if (data != null) tdecl.SetProperty(data);
                else tdecl.Properties.RemoveProperty<TypeDeclData>();

                return data;
            }
        }

        #endregion

        #region Properties

        internal override bool IsDeclaration { get { return true; } }

        /// <summary>
        /// Type name or empty name in case of anonymous type.
        /// </summary>
        public abstract NameRef Name { get; }

        /// <summary>
        /// Gets full type name including containing namespace.
        /// </summary>
        public abstract QualifiedName QualifiedName { get; }

        /// <summary>
		/// Name of the base class.
		/// </summary>
		private readonly INamedTypeRef baseClass;
        /// <summary>Name of the base class.</summary>
        public INamedTypeRef BaseClass { get { return baseClass; } }

        public PhpMemberAttributes MemberAttributes { get; }

        /// <summary>Implemented interface name indices. </summary>
        public INamedTypeRef[]/*!!*/ ImplementsList { get; }

        /// <summary>
        /// Type parameters.
        /// </summary>
        public TypeSignature TypeSignature { get { return this.GetTypeSignature() ?? TypeSignature.s_empty; } }

        /// <summary>
        /// Member declarations. Partial classes merged to the aggregate has this field <B>null</B>ed.
        /// </summary>
        public IList<TypeMemberDecl> Members { get { return members; } internal set { members = value; } }
        private IList<TypeMemberDecl> members;

        public Text.Span HeadingSpan { get; }

        public Text.Span BodySpan { get; }

        /// <summary>Indicates if type was decorated with partial keyword (Pure mode only).</summary>
        public bool PartialKeyword => TypeDeclData.TryGet(this, out var data) && data.IsPartial;

        /// <summary>
        /// Gets value indicating whether the declaration is conditional.
        /// </summary>
        public bool IsConditional => TypeDeclData.TryGet(this, out var data) && data.IsConditional;

        #endregion

        #region Construction

        public TypeDecl(
            Text.Span span, Text.Span headingSpan,
            bool isConditional, PhpMemberAttributes memberAttributes, bool isPartial,
            FormalTypeParam[]/*!*/ genericParams, INamedTypeRef baseClass,
            INamedTypeRef[]/*!*/ implementsList, TypeMemberDecl[]/*!*/ elements, Text.Span bodySpan)
            : base(span)
        {
            Debug.Assert(genericParams != null && implementsList != null && elements != null);
            Debug.Assert((memberAttributes & PhpMemberAttributes.Trait) == 0 || (memberAttributes & PhpMemberAttributes.Interface) == 0, "Interface cannot be a trait");

            this.baseClass = baseClass;
            this.MemberAttributes = memberAttributes;
            this.ImplementsList = implementsList.AsArray();
            this.members = elements.AsArray();
            this.HeadingSpan = headingSpan;
            this.BodySpan = bodySpan;

            if (isConditional)
            {
                TypeDeclData.GetOrAdd(this).IsConditional = true;
            }

            if (isPartial)
            {
                TypeDeclData.GetOrAdd(this).IsPartial = true;
            }

            if (genericParams != null && genericParams.Length != 0)
            {
                this.SetTypeSignature(new TypeSignature(genericParams));
            }
        }

        #endregion

        /// <summary>
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
    }

    /// <summary>
    /// Represents a class or an interface declaration.
    /// </summary>
    public sealed class NamedTypeDecl : TypeDecl
    {
        #region Properties

        /// <summary>
        /// Name of the class.
        /// </summary>
        public override NameRef Name { get; }

        /// <summary>
        /// Remember containinng ns, frequent op.
        /// </summary>
        private NamespaceDecl _ContainingNamespace;

        /// <summary>
        /// Gets the <see cref="NamespaceDecl"/> containining this type, or <c>null</c> reference if the type is declared in a global namespace.
        /// </summary>
        public new NamespaceDecl ContainingNamespace => _ContainingNamespace ?? (_ContainingNamespace = base.ContainingNamespace);

        /// <summary>
        /// Fully qualified name.
        /// </summary>
        public override QualifiedName QualifiedName
        {
            get
            {
                var ns = this.ContainingNamespace;
                if (ns != null)
                {
                    var ns_fqn = ns.QualifiedName.QualifiedName.Namespaces;
                    if (ns_fqn != null && ns_fqn.Length != 0)
                    {
                        return new QualifiedName(Name.Name, ns_fqn, true);
                    }
                }
                
                //
                return new QualifiedName(Name.Name, Syntax.Name.EmptyNames, true);
            }
        }

        #endregion

        #region Construction

        public NamedTypeDecl(
            Text.Span span, Text.Span headingSpan, bool isConditional, PhpMemberAttributes memberAttributes, bool isPartial,
            NameRef className, FormalTypeParam[]/*!*/ genericParams, INamedTypeRef baseClass,
            INamedTypeRef[]/*!*/ implementsList, TypeMemberDecl[]/*!*/ elements, Text.Span bodySpan)
            : base(span, headingSpan, isConditional,
                  memberAttributes, isPartial, genericParams, baseClass, implementsList, elements, bodySpan)
        {
            Debug.Assert(genericParams != null && implementsList != null && elements != null);
            Debug.Assert((memberAttributes & PhpMemberAttributes.Trait) == 0 || (memberAttributes & PhpMemberAttributes.Interface) == 0, "Interface cannot be a trait");

            this.Name = className;
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitNamedTypeDecl(this);
        }
    }

    /// <summary>
    /// Represents a class or an interface declaration.
    /// </summary>
    public sealed class AnonymousTypeDecl : TypeDecl
    {
        #region Properties

        /// <summary>
        /// The anonymous class name as it is in PHP - "class@anonymous\0{FILENAME}{BUFFER_POINTER,X8}".
        /// Does not have span (its span is invalid).
        /// </summary>
        public override NameRef Name => new NameRef(Text.Span.Invalid, GeneratePhpLikeName(this.ContainingSourceUnit, this.Span));

        /// <summary>
        /// Fully qualified name. Does not contain containing namespace.
        /// </summary>
        public override QualifiedName QualifiedName => new QualifiedName(Name, Syntax.Name.EmptyNames, true);

        #endregion

        #region Construction

        public AnonymousTypeDecl(
            Text.Span span, Text.Span headingSpan,
            bool isConditional, PhpMemberAttributes memberAttributes, bool isPartial,
            FormalTypeParam[]/*!*/ genericParams, INamedTypeRef baseClass,
            INamedTypeRef[]/*!*/ implementsList, TypeMemberDecl[]/*!*/ elements, Text.Span bodySpan)
            : base(span, headingSpan, isConditional,
                  memberAttributes, isPartial, genericParams, baseClass, implementsList, elements, bodySpan)
        {
        }

        #endregion

        /// <summary>
        /// Create the type name of an anomynous class as it is in PHP. See <c>__CLASS__</c> for a reference.
        /// </summary>
        /// <param name="unit">The containing source unit. Can be <c>null</c> if unknown.</param>
        /// <param name="span">Position of the class declaration within source unit.</param>
        /// <returns>An anonymous class name.</returns>
        private static string GeneratePhpLikeName(SourceUnit unit, Text.Span span)
        {
            // see zend_compile.c
            // sprintf(ZSTR_VAL(result), "class@anonymous%c%s%s", '\0', ZSTR_VAL(filename), char_pos_buf);

            var fname = unit?.FilePath ?? "<unknown>";  // TODO: (unit is EvalSourceUnit) => $"{fname}({line_number}) : eval()'d code"

            // TEMPLATE: class@anonymous{FILENAME}{BUFFER_POINTER,X8}
            return $"{Syntax.Name.AnonymousClassName.Value}\0{fname}{span.Start.ToString("X8")}";
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitAnonymousTypeDecl(this);
        }
    }

    #endregion

    #region TypeMemberDecl

    /// <summary>
    /// Represents a member declaration.
    /// </summary>
    public abstract class TypeMemberDecl : LangElement
    {
        /// <summary>
        /// Gets the type member modifiers.
        /// </summary>
        public PhpMemberAttributes Modifiers { get; }

        protected TypeMemberDecl(Text.Span span, PhpMemberAttributes modifiers)
            : base(span)
        {
            this.Modifiers = modifiers;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Represents a method declaration.
    /// </summary>
    public sealed class MethodDecl : TypeMemberDecl, INamedFunctionDeclaration
    {
        #region Nested class: BaseCtorParamsHolder

        /// <summary>
        /// Legacy thing (CLR); remember parameters passed to base() ctor.
        /// </summary>
        sealed class BaseCtorParamsHolder
        {
            /// <summary>
            /// Parameters passed to base() construct.
            /// <!--see cref="LanguageFeatures.Clr"/-->
            /// </summary>
            public ActualParam[] BaseCtorParams;
        }

        #endregion

        /// <summary>
        /// Name of the method.
        /// </summary>
        public NameRef Name { get; }

        /// <summary>
        /// Method signature containing all the parameters and parentheses.
        /// </summary>
        public Signature Signature { get; }

        /// <summary>
        /// Type parameter signatture for generics.
        /// </summary>
        public TypeSignature TypeSignature { get { return this.GetTypeSignature() ?? TypeSignature.s_empty; } }

        ILangElement IFunctionDeclaration.Body => Body;

        /// <summary>
        /// Method content.
        /// </summary>
        public BlockStmt Body { get; set; }

        /// <summary>
        /// Parameters used to initialize the base class.
        /// Not used.
        /// </summary>
        public ActualParam[] BaseCtorParams => this.TryGetProperty<BaseCtorParamsHolder>(out var holder) ? holder.BaseCtorParams : EmptyArray<ActualParam>.Instance;

        /// <summary>
        /// Span of the entire signature.
        /// </summary>
        public Text.Span ParametersSpan { get { return Signature.Span; } }

        /// <summary>
        /// Span of the entire method header.
        /// </summary>
        public Text.Span HeadingSpan
        {
            get
            {
                if (Span.IsValid)
                {
                    var endspan = ReturnType != null ? ReturnType.Span : Signature.Span;
                    if (endspan.IsValid)
                    {
                        return Text.Span.FromBounds(Span.Start, endspan.End);
                    }
                }

                //
                return Text.Span.Invalid;
            }
        }

        /// <summary>
        /// Return type hint, optional.
        /// </summary>
        public TypeRef ReturnType { get; }

        #region Construction

        /// <summary>
        /// Create new method declaration.
        /// </summary>
        /// <param name="span">Entire span.</param>
        /// <param name="name">Method name.</param>
        /// <param name="aliasReturn"><c>true</c> if method returns alias, <c>false</c> otherwise.</param>
        /// <param name="formalParams">Parameters.</param>
        /// <param name="paramsSpan">Span of the parameters including parentheses.</param>
        /// <param name="genericParams">Generic parameters.</param>
        /// <param name="body">Method content.</param>
        /// <param name="modifiers">Method modifiers, visibility etc.</param>
        /// <param name="baseCtorParams">Parameters for the base class constructor.</param>
        /// <param name="returnType">Return type hint, optional.</param>
        public MethodDecl(Text.Span span,
            NameRef name, bool aliasReturn, FormalParam[]/*!*/ formalParams, Text.Span paramsSpan,
            FormalTypeParam[]/*!*/ genericParams, BlockStmt body,
            PhpMemberAttributes modifiers, ActualParam[] baseCtorParams, TypeRef returnType)
            : base(span, modifiers)
        {
            Debug.Assert(genericParams != null && formalParams != null);

            this.Name = name;
            this.Signature = new Signature(aliasReturn, formalParams, paramsSpan);
            this.Body = body;
            this.ReturnType = returnType;

            if (genericParams != null && genericParams.Length != 0)
            {
                this.SetTypeSignature(new TypeSignature(genericParams));
            }

            if (baseCtorParams != null && baseCtorParams.Length != 0)
            {
                this.SetProperty(new BaseCtorParamsHolder { BaseCtorParams = baseCtorParams.AsArray(), });
            }
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitMethodDecl(this);
        }

        /// <summary>
        /// <see cref="IDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
    }

    #endregion

    #region Fields

    /// <summary>
    /// Represents a field multi-declaration.
    /// </summary>
    /// <remarks>
    /// Is derived from LangElement because we need position to report field_in_interface error.
    /// Else we would have to test ClassType in every FieldDecl and not only in FieldDeclList
    /// </remarks>
    public sealed class FieldDeclList : TypeMemberDecl
    {
        /// <summary>List of fields in this list</summary>
        public IReadOnlyList<FieldDecl> Fields/*!*/ { get; }

        /// <summary>Optional, fields type.</summary>
        public TypeRef Type { get; }

        public FieldDeclList(Text.Span span, PhpMemberAttributes modifiers, IReadOnlyList<FieldDecl>/*!*/ fields, TypeRef type)
            : base(span, modifiers)
        {
            Debug.Assert(fields != null);
            this.Fields = fields ?? throw new ArgumentNullException(nameof(fields));
            this.Type = type;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitFieldDeclList(this);
        }

        /// <summary>
        /// <see cref="IDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
    }

    /// <summary>
    /// Represents a field declaration.
    /// </summary>
    public sealed class FieldDecl : LangElement, IReadOnlyList<FieldDecl>
    {
        /// <summary>
        /// Gets a name of the field.
        /// </summary>
        public VariableName Name { get { return name; } }
        private VariableName name;

        /// <summary>
        /// Span of the property name.
        /// </summary>
        public Text.Span NameSpan => new Text.Span(Span.Start, name.Value.Length + 1);

        /// <summary>
        /// Initial value of the field represented by compile time evaluated expression.
        /// After analysis represented by Literal or ConstantUse or ArrayEx with constant parameters.
        /// Can be null.
        /// </summary>
        private Expression initializer;
        /// <summary>
        /// Initial value of the field represented by compile time evaluated expression.
        /// After analysis represented by Literal or ConstantUse or ArrayEx with constant parameters.
        /// Can be null.
        /// </summary>
        public Expression Initializer { get { return initializer; } internal set { initializer = value; } }

        /// <summary>
        /// Determines whether the field has an initializer.
        /// </summary>
        public bool HasInitVal { get { return initializer != null; } }

        public FieldDecl(Text.Span span, string/*!*/ name, Expression initVal)
            : base(span)
        {
            this.name = new VariableName(name);
            this.initializer = initVal;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitFieldDecl(this);
        }

        /// <summary>
        /// <see cref="IDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }

        #region IReadOnlyList<FieldDecl> // works as a list as well, so we don't need to allocate new array with just one element

        IEnumerator<FieldDecl> IEnumerable<FieldDecl>.GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<FieldDecl>)this).GetEnumerator();

        int IReadOnlyCollection<FieldDecl>.Count => 1;

        FieldDecl IReadOnlyList<FieldDecl>.this[int index] => index == 0 ? this : throw new ArgumentOutOfRangeException(nameof(index));

        #endregion
    }

    #endregion

    #region Class constants

    /// <summary>
    /// Represents a class constant declaration.
    /// </summary>
    public sealed class ConstDeclList : TypeMemberDecl
    {
        /// <summary>List of constants in this list</summary>
        public IList<ClassConstantDecl>/*!*/ Constants { get; }

        /// <summary>Optional type of constants.</summary>
        public TypeRef Type { get; }

        public ConstDeclList(Text.Span span, PhpMemberAttributes modifiers, IList<ClassConstantDecl>/*!*/ constants, TypeRef type)
            : base(span, modifiers)
        {
            this.Constants = constants ?? throw new ArgumentNullException(nameof(constants));
            this.Type = type;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitConstDeclList(this);
        }

        /// <summary>
        /// <see cref="IDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
    }

    public sealed class ClassConstantDecl : ConstantDecl
    {
        public ClassConstantDecl(Text.Span span, string/*!*/ name, Text.Span namePos, Expression/*!*/ initializer)
            : base(span, name, namePos, initializer)
        {
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitClassConstantDecl(this);
        }

        /// <summary>
        /// <see cref="IDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
    }

    #endregion

    #region Enum case

    /// <summary>
    /// Represents the enum case declaration.
    /// </summary>
    public sealed class EnumCaseDecl : TypeMemberDecl
    {
        /// <summary>
        /// Optional. The case value.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// The case name.
        /// Enum names are case-insensitive.
        /// </summary>
        public NameRef Name { get; }

        public EnumCaseDecl(Text.Span span, string name, Text.Span nameSpan, Expression expression)
            : base(span, PhpMemberAttributes.Public | PhpMemberAttributes.Static)
        {
            this.Expression = expression;
            this.Name = new NameRef(nameSpan, name);
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitEnumCaseDecl(this);
        }
    }

    #endregion

    #region Traits

    #region TraitAdaptationBlock

    /// <summary>
    /// Block statement.
    /// </summary>
    public sealed class TraitAdaptationBlock : LangElement
    {
        private readonly TraitsUse.TraitAdaptation[]/*!*/_adaptations;
        /// <summary>Statements in block</summary>
        public TraitsUse.TraitAdaptation[]/*!*/ Adaptations { get { return _adaptations; } }

        public TraitAdaptationBlock(Text.Span span, IList<TraitsUse.TraitAdaptation>/*!*/body)
            : base(span)
        {
            Debug.Assert(body != null);
            _adaptations = body.AsArray();
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitTraitAdaptationBlock(this);
        }
    }

    #endregion

    /// <summary>
    /// Represents class traits usage.
    /// </summary>
    public sealed class TraitsUse : TypeMemberDecl
    {
        #region TraitAdaptation, TraitAdaptationPrecedence, TraitAdaptationAlias

        public abstract class TraitAdaptation : LangElement
        {
            /// <summary>
            /// Name of existing trait member. Its qualified name is optional.
            /// </summary>
            public Tuple<TypeRef, NameRef> TraitMemberName { get; private set; }

            public TraitAdaptation(Text.Span span, Tuple<TypeRef, NameRef> traitMemberName)
                : base(span)
            {
                this.TraitMemberName = traitMemberName;
            }
        }

        /// <summary>
        /// Trait usage adaptation specifying a member which will be preferred over specified ambiguities.
        /// </summary>
        public sealed class TraitAdaptationPrecedence : TraitAdaptation
        {
            /// <summary>
            /// List of types which member <see cref="TraitAdaptation.TraitMemberName"/>.<c>Item2</c> will be ignored.
            /// </summary>
            public TypeRef[]/*!*/IgnoredTypes { get; private set; }

            public TraitAdaptationPrecedence(Text.Span span, Tuple<TypeRef, NameRef> traitMemberName, IList<TypeRef>/*!*/ignoredTypes)
                : base(span, traitMemberName)
            {
                this.IgnoredTypes = ignoredTypes.AsArray();
            }

            public override void VisitMe(TreeVisitor visitor)
            {
                visitor.VisitTraitAdaptationPrecedence(this);
            }
        }

        /// <summary>
        /// Trait usage adaptation which aliases a trait member.
        /// </summary>
        public sealed class TraitAdaptationAlias : TraitAdaptation
        {
            /// <summary>
            /// Optionally new member visibility attributes.
            /// </summary>
            public PhpMemberAttributes? NewModifier { get; private set; }

            /// <summary>
            /// Optionally new member name. Can be <c>NameRef.Invalid</c>.
            /// </summary>
            public NameRef NewName { get; private set; }

            public TraitAdaptationAlias(Text.Span span, Tuple<TypeRef, NameRef>/*!*/oldname, NameRef newname, PhpMemberAttributes? newmodifier)
                : base(span, oldname)
            {
                if (oldname == null)
                    throw new ArgumentNullException("oldname");

                this.NewName = newname;
                this.NewModifier = newmodifier;
            }

            public override void VisitMe(TreeVisitor visitor)
            {
                visitor.VisitTraitAdaptationAlias(this);
            }
        }

        #endregion

        /// <summary>
        /// List of trait types to be used.
        /// </summary>
        public TypeRef[]/*!*/TraitsList { get { return traitsList; } }
        private readonly TypeRef[]/*!*/traitsList;

        /// <summary>
        /// List of trait adaptations modifying names of trait members. Can be <c>null</c> reference.
        /// </summary>
        public TraitAdaptationBlock TraitAdaptationBlock { get { return traitAdaptationBlock; } }
        private readonly TraitAdaptationBlock traitAdaptationBlock;

        /// <summary>
        /// Position where traits list ends.
        /// </summary>
        public int HeadingEndPosition => traitsList.Last().Span.End;

        public TraitsUse(Text.Span span, IList<TypeRef>/*!*/traitsList, TraitAdaptationBlock traitAdaptationBlock)
            : base(span, PhpMemberAttributes.Public)
        {
            if (traitsList == null)
                throw new ArgumentNullException("traitsList");
            Debug.Assert(traitsList.All(t => t is INamedTypeRef));
            this.traitsList = traitsList.AsArray();
            this.traitAdaptationBlock = traitAdaptationBlock;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitTraitsUse(this);
        }
    }

    #endregion

    #region Property Hooks

    /// <summary>
    /// Represents a property with hooks.
    /// </summary>
    public sealed class PropertyDecl : TypeMemberDecl
    {
        /// <summary>List of property hooks in this declaration.</summary>
        public IReadOnlyList<PropertyHookDecl> PropertyHooks/*!*/ { get; }

        /// <summary>Optional, property type.</summary>
        public TypeRef Type { get; }

        public VariableNameRef Name { get; }

        public IExpression InitialValue { get; }

        public PropertyDecl(Text.Span span, VariableNameRef name, PhpMemberAttributes modifiers, IReadOnlyList<PropertyHookDecl>/*!*/ hooks, TypeRef propertyType, IExpression initialValue)
            : base(span, modifiers)
        {
            Debug.Assert(hooks != null);
            this.PropertyHooks = hooks ?? throw new ArgumentNullException(nameof(hooks));
            this.Type = propertyType;
            this.Name = name;
            this.InitialValue = initialValue;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitPropertyDecl(this);
        }

        /// <summary>
        /// <see cref="IDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get => this.GetPHPDoc();
            set => this.SetPHPDoc(value);
        }
    }

    public class PropertyHookDecl : LangElement, INamedFunctionDeclaration
    {
        public PropertyHookDecl(
            Span span,
            PhpMemberAttributes memberAttributes,
            NameRef name,
            Signature signature,
            ILangElement body
            ) : base(span)
        {
            this.Name = name;
            this.Modifiers = memberAttributes;
            this.Signature = signature;
            this.Body = body;
        }

        /// <summary>
        /// Gets the type hook modifiers.
        /// </summary>
        public PhpMemberAttributes Modifiers { get; }

        public NameRef Name { get; }

        public Signature Signature { get; }

        /// <summary>
        /// Always <c>null</c>.
        /// Property type is specified using <see cref="PropertyDecl.Type"/>.
        /// </summary>
        public TypeRef ReturnType => null;

        public ILangElement Body { get; }

        public override void VisitMe(TreeVisitor visitor) => visitor.VisitPropertyHookDecl(this);

        /// <summary>
        /// <see cref="IDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get => this.GetPHPDoc();
            set => this.SetPHPDoc(value);
        }
    }


    #endregion
}
