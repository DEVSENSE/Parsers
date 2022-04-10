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
    #region GlobalCode

    /// <summary>
    /// Represents a container for global statements.
    /// </summary>
    public interface IGlobalCode : ILangElement, IBlockStatement
    {

    }

    /// <summary>
    /// Represents a container for global statements.
    /// </summary>
    /// <remarks>
    /// PHP source file can contain global code definition which is represented in AST 
    /// by GlobalCode node. Finally, it is emitted into Main() method of concrete PHPPage 
    /// class. The sample code below illustrates a part of PHP global code
    /// </remarks>
    public sealed class GlobalCode : LangElement, IGlobalCode
    {
        #region IGlobalCode

        IReadOnlyCollection<IStatement> IBlockStatement.Statements => statements;

        Tokens IBlockStatement.OpeningToken => 0;

        Tokens IBlockStatement.ClosingToken => 0;

        #endregion

        /// <summary>
        /// Array of nodes representing statements in PHP global code
        /// </summary>
        public Statement[]/*!*/ Statements { get { return statements; } internal set { statements = value; } }
        private Statement[]/*!*/ statements;

        private readonly SourceUnit/*!*/ sourceUnit;

        /// <summary>
        /// Represented source unit.
        /// </summary>
        public override SourceUnit ContainingSourceUnit => sourceUnit;

        public override TypeDecl ContainingType => null;

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
        public NamingContext/*!*/ Naming
        {
            get { return this.naming; }
            internal /* friend Parser */ set { this.naming = value; }
        }
        private NamingContext naming = null;

        public bool IsAnonymous { get { return this.isAnonymous; } }
        private readonly bool isAnonymous;

        public BlockStmt/*!*/ Body { get { return body; } internal set { body = value; } }
        private BlockStmt/*!*/ body = null;

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
        /// Declared constants.
        /// </summary>
        public IList<GlobalConstantDecl>/*!*/ Constants { get; }

        /// <summary>
        /// Span of the entire declaration.
        /// </summary>
        public Text.Span EntireDeclarationSpan
        {
            get { return this.Span; }
        }

        public GlobalConstDeclList(Text.Span span, IList<GlobalConstantDecl>/*!*/ constants)
            : base(span)
        {
            this.Constants = constants ?? throw new ArgumentNullException(nameof(constants));
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

        public GlobalConstantDecl(Text.Span span, bool isConditional,
            string/*!*/ name, Text.Span namePos, Expression/*!*/ initializer)
            : base(span, name, namePos, initializer)
        {
            this.IsConditional = IsConditional;
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
