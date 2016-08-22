using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
    #region GlobalCode

    /// <summary>
    /// Represents a container for global statements.
    /// </summary>
    /// <remarks>
    /// PHP source file can contain global code definition which is represented in AST 
    /// by GlobalCode node. Finally, it is emitted into Main() method of concrete PHPPage 
    /// class. The sample code below illustrates a part of PHP global code
    /// </remarks>
    public sealed class GlobalCode : LangElement, IHasSourceUnit
    {
        /// <summary>
        /// Array of nodes representing statements in PHP global code
        /// </summary>
        public Statement[]/*!*/ Statements { get { return statements; } internal set { statements = value; } }
        private Statement[]/*!*/ statements;

        /// <summary>
        /// Represented source unit.
        /// </summary>
        public SourceUnit/*!*/ SourceUnit { get { return sourceUnit; } }
        private readonly SourceUnit/*!*/ sourceUnit;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GlobalCode class.
        /// </summary>
        public GlobalCode(Text.Span span, IList<Statement>/*!*/ statements, SourceUnit/*!*/ sourceUnit) : base(span)
        {
            Debug.Assert(statements != null && sourceUnit != null);

            this.sourceUnit = sourceUnit;
            this.statements = statements.AsArray();
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitGlobalCode(this);
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

    #region NamespaceDecl

    public sealed class NamespaceDecl : Statement
    {
        internal override bool IsDeclaration { get { return true; } }

        /// <summary>
        /// Whether the namespace was declared using PHP simple syntax.
        /// </summary>
        public readonly bool IsSimpleSyntax;

        public QualifiedNameRef QualifiedName { get { return this.qualifiedName; } }
        private QualifiedNameRef qualifiedName;

        /// <summary>
        /// Naming context defining aliases.
        /// </summary>
        public NamingContext/*!*/ Naming {
            get { return this.naming; }
            internal /* friend Parser */ set { this.naming = value; }
        }
        private NamingContext naming = null;

        public bool IsAnonymous { get { return this.isAnonymous; } }
        private readonly bool isAnonymous;

        public IList<Statement>/*!*/ Statements
        {
            get { return this.statements; }
            internal /* friend Parser */ set { this.statements = value; }
        }
        private IList<Statement>/*!*/ statements = null;

        #region Construction

        public NamespaceDecl(Text.Span p)
            : this(p, QualifiedNameRef.Invalid, false)
        {
            this.isAnonymous = true;
            this.qualifiedName = new QualifiedNameRef(Text.Span.Invalid, Name.EmptyBaseName, Name.EmptyNames);
            this.IsSimpleSyntax = false;
        }

        public NamespaceDecl(Text.Span p, QualifiedNameRef name, bool simpleSyntax)
            : base(p)
        {
            this.isAnonymous = false;
            this.qualifiedName = name;
            this.IsSimpleSyntax = simpleSyntax;
        }

        /// <summary>
        /// Finish parsing of namespace, complete its position.
        /// </summary>
        /// <param name="p"></param>
        internal void UpdatePosition(Text.Span p)
        {
            this.Span = p;
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitNamespaceDecl(this);
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

    #region GlobalConstDeclList, GlobalConstantDecl

    public sealed class GlobalConstDeclList : Statement
    {
        /// <summary>
        /// Gets collection of CLR attributes annotating this statement.
        /// </summary>
        public CustomAttributes Attributes
        {
            get { return this.GetCustomAttributes(); }
            set { this.SetCustomAttributes(value); }
        }

        public List<GlobalConstantDecl>/*!*/ Constants { get { return constants; } }
        private readonly List<GlobalConstantDecl>/*!*/ constants;

        public Text.Span EntireDeclarationSpan
        {
            get { return this.Span; }
        }

        public GlobalConstDeclList(Text.Span span, List<GlobalConstantDecl>/*!*/ constants, List<CustomAttribute> attributes)
            : base(span)
        {
            Debug.Assert(constants != null);

            this.constants = constants;
            if (attributes != null && attributes.Count != 0)
                this.Attributes = new CustomAttributes(attributes);
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitGlobalConstDeclList(this);
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

    public sealed class GlobalConstantDecl : ConstantDecl
    {
        /// <summary>
        /// Gets value indicating whether this global constant is declared conditionally.
        /// </summary>
        public bool IsConditional { get; private set; }

        /// <summary>
        /// Source unit.
        /// </summary>
        internal SourceUnit SourceUnit { get; private set; }

        public GlobalConstantDecl(SourceUnit/*!*/ sourceUnit, Text.Span span, bool isConditional, 
            string/*!*/ name, Expression/*!*/ initializer)
            : base(span, name, initializer)
        {
            this.IsConditional = IsConditional;
            this.SourceUnit = sourceUnit;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitGlobalConstantDecl(this);
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

}
