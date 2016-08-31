using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
	#region FormalTypeParam

	public sealed class FormalTypeParam : LangElement
	{
		public Name Name { get { return name; } }
		private readonly Name name;

		/// <summary>
		/// Either <see cref="PrimitiveTypeName"/>, <see cref="GenericQualifiedName"/>, or <B>null</B>.
		/// </summary>
		public object DefaultType { get { return defaultType; } }
		private readonly object defaultType;

        /// <summary>
        /// Gets collection of CLR attributes annotating this statement.
        /// </summary>
        public CustomAttributes Attributes
        {
            get { return this.GetCustomAttributes(); }
            set { this.SetCustomAttributes(value); }
        }

		/// <summary>
        /// Singleton instance of an empty <see cref="List&lt;FormalTypeParam&gt;"/>.
        /// </summary>
        public static readonly List<FormalTypeParam>/*!*/EmptyList = new List<FormalTypeParam>();

		#region Construction

		public FormalTypeParam(Text.Span span, Name name, object defaultType, List<CustomAttribute> attributes)
            : base(span)
		{
            Debug.Assert(defaultType == null || defaultType is PrimitiveTypeName || defaultType is GenericQualifiedName);

			this.name = name;
			this.defaultType = defaultType;

			if (attributes != null && attributes.Count != 0)
                this.Attributes = new CustomAttributes(attributes);
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

	public struct TypeSignature
	{
		internal FormalTypeParam[]/*!!*/ TypeParams { get { return typeParams; } }
		private readonly FormalTypeParam[]/*!!*/ typeParams;

		#region Construction

		public TypeSignature(IList<FormalTypeParam>/*!!*/ typeParams)
		{
			Debug.Assert(typeParams != null);
			this.typeParams = typeParams.AsArray();
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
        #region Properties

        internal override bool IsDeclaration { get { return true; } }

        /// <summary>
        /// Type name or empty name in case of anonymous type.
        /// </summary>
        public abstract NameRef Name { get; }

        /// <summary>
		/// Name of the base class.
		/// </summary>
		private readonly QualifiedNameRef baseClass;
        /// <summary>Name of the base class.</summary>
        public QualifiedNameRef BaseClass { get { return baseClass; } }

        public PhpMemberAttributes MemberAttributes { get; private set; }

        /// <summary>Implemented interface name indices. </summary>
        public QualifiedNameRef[]/*!!*/ ImplementsList { get; private set; }

        /// <summary>
        /// Type parameters.
        /// </summary>
        public TypeSignature TypeSignature { get { return typeSignature; } }
        internal readonly TypeSignature typeSignature;

        /// <summary>
        /// Member declarations. Partial classes merged to the aggregate has this field <B>null</B>ed.
        /// </summary>
        public List<TypeMemberDecl> Members { get { return members; } internal set { members = value; } }
        private List<TypeMemberDecl> members;

        /// <summary>
        /// Gets collection of CLR attributes annotating this statement.
        /// </summary>
        public CustomAttributes Attributes
        {
            get { return this.GetCustomAttributes(); }
            set { this.SetCustomAttributes(value); }
        }

        public Text.Span HeadingSpan { get { return headingSpan; } }
        private Text.Span headingSpan;

        public Text.Span BodySpan { get { return bodySpan; } }
        private Text.Span bodySpan;

        /// <summary>Indicates if type was decorated with partial keyword (Pure mode only).</summary>
        public bool PartialKeyword { get { return partialKeyword; } }
        /// <summary>Contains value of the <see cref="PartialKeyword"/> property</summary>
        private bool partialKeyword;

        internal Scope Scope { get; private set; }

        /// <summary>
        /// Gets value indicating whether the declaration is conditional.
        /// </summary>
        public bool IsConditional { get; internal set; }

        #endregion

        #region Construction

        public TypeDecl(
            Text.Span span, Text.Span headingSpan,
            bool isConditional, Scope scope, PhpMemberAttributes memberAttributes, bool isPartial,
            List<FormalTypeParam>/*!*/ genericParams, QualifiedNameRef baseClass,
            List<QualifiedNameRef>/*!*/ implementsList, List<TypeMemberDecl>/*!*/ elements, Text.Span bodySpan,
            List<CustomAttribute> attributes)
            : base(span)
        {
            Debug.Assert(genericParams != null && implementsList != null && elements != null);
            Debug.Assert((memberAttributes & PhpMemberAttributes.Trait) == 0 || (memberAttributes & PhpMemberAttributes.Interface) == 0, "Interface cannot be a trait");
            
            this.typeSignature = new TypeSignature(genericParams);
            this.baseClass = baseClass;
            this.MemberAttributes = memberAttributes;
            this.Scope = scope;
            this.IsConditional = isConditional;
            this.ImplementsList = implementsList.AsArray();
            this.members = elements;
            this.members.TrimExcess();

            if (attributes != null && attributes.Count != 0)
                this.Attributes = new CustomAttributes(attributes);
            this.headingSpan = headingSpan;
            this.bodySpan = bodySpan;
            this.partialKeyword = isPartial;
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitTypeDecl(this);
        }

        /// <summary>
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
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
		public override NameRef Name { get { return name; } }
		private readonly NameRef name;
        
		#endregion

		#region Construction

		public NamedTypeDecl(
            Text.Span span, Text.Span headingSpan, bool isConditional, Scope scope, PhpMemberAttributes memberAttributes, bool isPartial,
            NameRef className, List<FormalTypeParam>/*!*/ genericParams, QualifiedNameRef baseClass,
            List<QualifiedNameRef>/*!*/ implementsList, List<TypeMemberDecl>/*!*/ elements, Text.Span bodySpan,

            List<CustomAttribute> attributes)
            : base(span, headingSpan, isConditional, 
                  scope, memberAttributes, isPartial, genericParams, baseClass, implementsList, elements, bodySpan, attributes)
		{
			Debug.Assert(genericParams != null && implementsList != null && elements != null);
            Debug.Assert((memberAttributes & PhpMemberAttributes.Trait) == 0 || (memberAttributes & PhpMemberAttributes.Interface) == 0, "Interface cannot be a trait");

			this.name = className;
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
        /// Always empty.
        /// </summary>
        public override NameRef Name => NameRef.Invalid;

        #endregion

        #region Construction

        public AnonymousTypeDecl(
            Text.Span span, Text.Span headingSpan,
            bool isConditional, Scope scope, PhpMemberAttributes memberAttributes, bool isPartial,
            List<FormalTypeParam>/*!*/ genericParams, QualifiedNameRef baseClass,
            List<QualifiedNameRef>/*!*/ implementsList, List<TypeMemberDecl>/*!*/ elements, Text.Span bodySpan,
            List<CustomAttribute> attributes)
            : base(span, headingSpan, isConditional,
                  scope, memberAttributes, isPartial, genericParams, baseClass, implementsList, elements, bodySpan, attributes)
        {
        }

        #endregion

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
        public PhpMemberAttributes Modifiers { get { return modifiers; } }
		protected PhpMemberAttributes modifiers;

        /// <summary>
        /// Gets collection of CLR attributes annotating this statement.
        /// </summary>
        public CustomAttributes Attributes
        {
            get { return this.GetCustomAttributes(); }
            set { this.SetCustomAttributes(value); }
        }

        protected TypeMemberDecl(Text.Span span, List<CustomAttribute> attributes)
            : base(span)
		{
            if (attributes != null && attributes.Count != 0)
			    this.Attributes = new CustomAttributes(attributes);
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// Represents a method declaration.
	/// </summary>
    public sealed class MethodDecl : TypeMemberDecl
	{
		/// <summary>
		/// Name of the method.
		/// </summary>
		public NameRef Name { get { return name; } }
		private readonly NameRef name;

		public Signature Signature { get { return signature; } }
		private readonly Signature signature;

		public TypeSignature TypeSignature { get { return typeSignature; } }
		private readonly TypeSignature typeSignature;

        public BlockStmt Body { get { return body; } internal set { body = value; } }
        private BlockStmt body;

        public ActualParam[] BaseCtorParams { get { return baseCtorParams; } internal set { baseCtorParams = value; } }
		private ActualParam[] baseCtorParams;

        public Text.Span ParametersSpan { get { return parametersSpan; } }
        private Text.Span parametersSpan;

        public Text.Span HeadingSpan => Text.Span.FromBounds(Span.Start, ((returnType != null) ? returnType.Span : ParametersSpan).End);

        public TypeRef ReturnType { get { return returnType; } }
        private TypeRef returnType;

        #region Construction

        public MethodDecl(Text.Span span, 
			NameRef name, bool aliasReturn, IList<FormalParam>/*!*/ formalParams, Text.Span paramsSpan,
            IList<FormalTypeParam>/*!*/ genericParams,  BlockStmt body,
            PhpMemberAttributes modifiers, IList<ActualParam> baseCtorParams, 
			List<CustomAttribute> attributes, TypeRef returnType)
            : base(span, attributes)
        {
            Debug.Assert(genericParams != null && formalParams != null);

            this.modifiers = modifiers;
            this.name = name;
            this.signature = new Signature(aliasReturn, formalParams);
            this.typeSignature = new TypeSignature(genericParams);
            this.body = body;
            this.baseCtorParams = (baseCtorParams != null) ? baseCtorParams.AsArray() : null;
            this.parametersSpan = paramsSpan;
            this.returnType = returnType;
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
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
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
	/// Else we would have to test ClassType in every FieldDecl and not only in FildDeclList
	/// </remarks>
	public sealed class FieldDeclList : TypeMemberDecl
	{
		private readonly List<FieldDecl>/*!*/ fields;
        /// <summary>List of fields in this list</summary>
        public List<FieldDecl> Fields/*!*/ { get { return fields; } }

		public FieldDeclList(Text.Span span, PhpMemberAttributes modifiers, List<FieldDecl>/*!*/ fields,
			List<CustomAttribute> attributes)
            : base(span, attributes)
		{
			Debug.Assert(fields != null);

			this.modifiers = modifiers;
			this.fields = fields;
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
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
	}

	/// <summary>
	/// Represents a field declaration.
	/// </summary>
	public sealed class FieldDecl : LangElement
	{
		/// <summary>
		/// Gets a name of the field.
		/// </summary>
		public VariableName Name { get { return name; } }
		private VariableName name;

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
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
    }

	#endregion

	#region Class constants

	/// <summary>
	/// Represents a class constant declaration.
	/// </summary>
	public sealed class ConstDeclList : TypeMemberDecl
	{
		/// <summary>List of constants in this list</summary>
        public List<ClassConstantDecl>/*!*/ Constants { get { return constants; } }
        private readonly List<ClassConstantDecl>/*!*/ constants;
        
		public ConstDeclList(Text.Span span, List<ClassConstantDecl>/*!*/ constants, List<CustomAttribute> attributes)
            : base(span, attributes)
		{
			Debug.Assert(constants != null);

			this.constants = constants;

			//class constants never have modifiers
			modifiers = PhpMemberAttributes.Public;
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
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
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
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
    }

	#endregion

    #region Traits

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
            public Tuple<QualifiedNameRef, NameRef> TraitMemberName { get; private set; }

            public TraitAdaptation(Text.Span span, Tuple<QualifiedNameRef, NameRef> traitMemberName)
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
            public List<QualifiedNameRef>/*!*/IgnoredTypes { get; private set; }

            public TraitAdaptationPrecedence(Text.Span span, Tuple<QualifiedNameRef, NameRef> traitMemberName, List<QualifiedNameRef>/*!*/ignoredTypes)
                : base(span, traitMemberName)
            {
                this.IgnoredTypes = ignoredTypes;
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

            public TraitAdaptationAlias(Text.Span span, Tuple<QualifiedNameRef, NameRef>/*!*/oldname, NameRef newname, PhpMemberAttributes? newmodifier)
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
        public List<QualifiedNameRef>/*!*/TraitsList { get { return traitsList; } }
        private readonly List<QualifiedNameRef>/*!*/traitsList;

        /// <summary>
        /// List of trait adaptations modifying names of trait members. Can be <c>null</c> reference.
        /// </summary>
        public List<TraitAdaptation> TraitAdaptationList { get { return traitAdaptationList; } }
        private readonly List<TraitAdaptation> traitAdaptationList;

        /// <summary>
        /// Gets a value indicating whether there is a block (even empty) of trait adaptations.
        /// </summary>
        public bool HasTraitAdaptationBlock { get { return this.traitAdaptationList != null; } }

        /// <summary>
        /// Position where traits list ends.
        /// </summary>
        public int HeadingEndPosition { get { return headingEndPosition; } }
        private readonly int headingEndPosition;

        public TraitsUse(Text.Span span, int headingEndPosition, List<QualifiedNameRef>/*!*/traitsList, List<TraitAdaptation> traitAdaptationList)
            : base(span, null)
        {
            if (traitsList == null)
                throw new ArgumentNullException("traitsList");

            this.traitsList = traitsList;
            this.traitAdaptationList = traitAdaptationList;
            this.headingEndPosition = headingEndPosition;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitTraitsUse(this);
        }
    }

    #endregion
}
