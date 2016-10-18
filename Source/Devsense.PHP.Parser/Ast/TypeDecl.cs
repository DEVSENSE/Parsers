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

        /// <summary>
        /// Gets value indicating whether the declaration is conditional.
        /// </summary>
        public bool IsConditional { get; internal set; }

        #endregion

        #region Construction

        public TypeDecl(
            Text.Span span, Text.Span headingSpan,
            bool isConditional, PhpMemberAttributes memberAttributes, bool isPartial,
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
            Text.Span span, Text.Span headingSpan, bool isConditional, PhpMemberAttributes memberAttributes, bool isPartial,
            NameRef className, List<FormalTypeParam>/*!*/ genericParams, QualifiedNameRef baseClass,
            List<QualifiedNameRef>/*!*/ implementsList, List<TypeMemberDecl>/*!*/ elements, Text.Span bodySpan,

            List<CustomAttribute> attributes)
            : base(span, headingSpan, isConditional, 
                  memberAttributes, isPartial, genericParams, baseClass, implementsList, elements, bodySpan, attributes)
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
        /// The anonymous class name as it is in PHP - "class@anonymous\0{FILENAME}{BUFFER_POINTER,X8}".
        /// Does not have span (its span is invalid).
        /// </summary>
        public override NameRef Name => new NameRef(Text.Span.Invalid, GeneratePhpLikeName(this.ContainingSourceUnit, this.Span));

        #endregion

        #region Construction

        public AnonymousTypeDecl(
            Text.Span span, Text.Span headingSpan,
            bool isConditional, PhpMemberAttributes memberAttributes, bool isPartial,
            List<FormalTypeParam>/*!*/ genericParams, QualifiedNameRef baseClass,
            List<QualifiedNameRef>/*!*/ implementsList, List<TypeMemberDecl>/*!*/ elements, Text.Span bodySpan,
            List<CustomAttribute> attributes)
            : base(span, headingSpan, isConditional,
                  memberAttributes, isPartial, genericParams, baseClass, implementsList, elements, bodySpan, attributes)
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
            return $"class@anonymous\0{fname}{span.Start.ToString("X8")}";
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
        
		public ConstDeclList(Text.Span span, PhpMemberAttributes modifiers, List<ClassConstantDecl>/*!*/ constants, List<CustomAttribute> attributes)
            : base(span, attributes)
		{
			Debug.Assert(constants != null);

            this.modifiers = modifiers;
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
        public TraitAdaptationBlock TraitAdaptationBlock { get { return traitAdaptationBlock; } }
        private readonly TraitAdaptationBlock traitAdaptationBlock;

        /// <summary>
        /// Position where traits list ends.
        /// </summary>
        public int HeadingEndPosition => traitsList.Last().Span.End;

        public TraitsUse(Text.Span span, List<QualifiedNameRef>/*!*/traitsList, TraitAdaptationBlock traitAdaptationBlock)
            : base(span, null)
        {
            if (traitsList == null)
                throw new ArgumentNullException("traitsList");

            this.traitsList = traitsList;
            this.traitAdaptationBlock = traitAdaptationBlock;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitTraitsUse(this);
        }
    }

    #endregion
}
