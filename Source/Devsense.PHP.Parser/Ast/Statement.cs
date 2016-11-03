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
    #region Statement

    /// <summary>
    /// Abstract base class representing all statements elements of PHP source file.
    /// </summary>
    public abstract class Statement : LangElement
    {
        protected Statement(Text.Span span)
            : base(span)
        {
        }

        /// <summary>
        /// Whether the statement is a declaration statement (class, function, namespace, const).
        /// </summary>
        internal virtual bool IsDeclaration { get { return false; } }

        internal virtual bool SkipInPureGlobalCode() { return false; }
    }

    #endregion

    #region BlockStmt

    /// <summary>
    /// Block statement.
    /// </summary>
    public sealed class BlockStmt : Statement
    {
        private readonly Statement[]/*!*/_statements;
        /// <summary>Statements in block</summary>
        public Statement[]/*!*/ Statements { get { return _statements; } }

        public BlockStmt(Text.Span span, IList<Statement>/*!*/body)
            : base(span)
        {
            Debug.Assert(body != null);
            _statements = body.AsArray();
        }

        /// <summary>
        /// Extend actual block span.
        /// Used for the colon blocks in the alternate notation if/else.
        /// </summary>
        /// <param name="newSpan">New span, which must contain the old span.</param>
        internal void ExtendSpan(Text.Span newSpan)
        {
            Debug.Assert(newSpan.Contains(Span));
            Span = newSpan;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitBlockStmt(this);
        }
    }

    #endregion

    #region ExpressionStmt

    /// <summary>
    /// Expression statement.
    /// </summary>
    public sealed class ExpressionStmt : Statement
    {
        /// <summary>Expression that repesents this statement</summary>
        public Expression/*!*/ Expression { get { return expression; } internal set { expression = value; } }
        private Expression/*!*/ expression;

        public ExpressionStmt(Text.Span span, Expression/*!*/ expression)
            : base(span)
        {
            Debug.Assert(expression != null);
            this.expression = expression;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitExpressionStmt(this);
        }
    }

    #endregion

    #region EmptyStmt

    /// <summary>
    /// Empty statement.
    /// </summary>
    public sealed class EmptyStmt : Statement
    {
        public static readonly EmptyStmt Unreachable = new EmptyStmt(Text.Span.Invalid);
        public static readonly EmptyStmt Skipped = new EmptyStmt(Text.Span.Invalid);
        public static readonly EmptyStmt PartialMergeResiduum = new EmptyStmt(Text.Span.Invalid);

        internal override bool SkipInPureGlobalCode()
        {
            return true;
        }

        public EmptyStmt(Text.Span p) : base(p) { }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitEmptyStmt(this);
        }
    }

    #endregion

    #region PHPDocStmt

    /// <summary>
    /// Empty statement containing PHPDoc block.
    /// </summary>
    public sealed class PHPDocStmt : Statement
    {
        public PHPDocBlock/*!*/PHPDoc { get { return _phpdoc; } }
        private readonly PHPDocBlock _phpdoc;

        internal override bool SkipInPureGlobalCode() { return true; }

        public PHPDocStmt(PHPDocBlock/*!*/phpdoc) : base(phpdoc.Span)
        {
            Debug.Assert(phpdoc != null);
            _phpdoc = phpdoc;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitPHPDocStmt(this);
        }
    }

    #endregion

    #region UnsetStmt

    /// <summary>
    /// Represents an <c>unset</c> statement.
    /// </summary>
    public sealed class UnsetStmt : Statement
    {
        /// <summary>List of variables to be unset</summary>
        public List<VariableUse> /*!*/VarList { get { return varList; } }
        private readonly List<VariableUse>/*!*/ varList;

        public UnsetStmt(Text.Span p, List<VariableUse>/*!*/ varList)
            : base(p)
        {
            Debug.Assert(varList != null);
            this.varList = varList;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitUnsetStmt(this);
        }
    }

    #endregion

    #region GlobalStmt

    /// <summary>
    /// Represents a <c>global</c> statement.
    /// </summary>
    public sealed class GlobalStmt : Statement
    {
        public List<SimpleVarUse>/*!*/ VarList { get { return varList; } }
        private List<SimpleVarUse>/*!*/ varList;

        public GlobalStmt(Text.Span p, List<SimpleVarUse>/*!*/ varList)
            : base(p)
        {
            Debug.Assert(varList != null);
            this.varList = varList;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitGlobalStmt(this);
        }
    }

    #endregion

    #region StaticStmt

    /// <summary>
    /// Represents a <c>static</c> statement.
    /// </summary>
    public sealed class StaticStmt : Statement
    {
        /// <summary>List of static variables</summary>
        public List<StaticVarDecl>/*!*/ StVarList { get { return stVarList; } }
        private List<StaticVarDecl>/*!*/ stVarList;

        public StaticStmt(Text.Span p, List<StaticVarDecl>/*!*/ stVarList)
            : base(p)
        {
            Debug.Assert(stVarList != null);
            this.stVarList = stVarList;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitStaticStmt(this);
        }
    }

    /// <summary>
    /// Helper class. No error or warning can be caused by declaring variable as static.
    /// </summary>
    /// <remarks>
    /// Even this is ok:
    /// 
    /// function f()
    ///	{
    ///   global $a;
    ///   static $a = 1;
    /// }
    /// 
    /// That's why we dont'need to know Position => is not child of LangElement
    /// </remarks>
    public class StaticVarDecl : LangElement
    {
        /// <summary>Static variable being declared</summary>
        public VariableName/*!*/ Variable { get { return variable; } }
        private VariableName/*!*/ variable;

        /// <summary>
        /// Span of the static variable name.
        /// </summary>
        public Text.Span NameSpan => new Text.Span(Span.Start, variable.Value.Length + 1);

        /// <summary>Expression used to initialize static variable</summary>
        public Expression Initializer { get { return initializer; } internal set { initializer = value; } }
        private Expression initializer;
        
        public StaticVarDecl(Text.Span span, VariableName variableName, Expression initializer)
            : base(span)
        {
            Debug.Assert(variableName != null);

            this.variable = variableName;
            this.initializer = initializer;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitStaticVarDecl(this);
        }
    }

    #endregion

    #region DeclareStmt

    public sealed class DeclareStmt : Statement
    {
        /// <summary>
        /// Inner statement.
        /// </summary>
        public Statement Statement { get { return this.stmt; } }
        private readonly Statement/*!*/stmt;

        public DeclareStmt(Text.Span p, Statement statement)
            : base(p)
        {
            this.stmt = statement;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDeclareStmt(this);
        }
    }

    #endregion

    #region UseStatement

    public abstract class UseBase
    {
        public Span Span => _span;
        private readonly Span _span;

        public UseBase(Span span)
        {
            Debug.Assert(span.IsValid);
            _span = span;
        }
    }

    /// <summary>
    /// Represents a simple alias within use statement in form <c>QNAME as NAME</c> or just <c>QNAME</c>.
    /// </summary>
    public sealed class SimpleUse : UseBase
    {
        /// <summary>
        /// Span of <see cref="Alias"/>,
        /// can be <c>invalid</c>.
        /// </summary>
        public Span AliasSpan => _aliasSpan;
        private readonly Span _aliasSpan;

        /// <summary>
        /// Span of associated qualified name.
        /// </summary>
        public Span NameSpan => _nameSpan;
        private readonly Span _nameSpan;

        /// <summary>
        /// The alias name.
        /// </summary>
        public Alias Alias => _alias;
        private readonly Alias _alias;

        /// <summary>
        /// The qualified name.
        /// </summary>
        public QualifiedName QualifiedName => _qualifiedName;
        private readonly QualifiedName _qualifiedName;

        public SimpleUse(Span aliasSpan, Span qnameSpan, Alias alias, QualifiedName qualifiedName)
            : base(aliasSpan.IsValid ? Span.Combine(qnameSpan, aliasSpan) : qnameSpan)
        {
            Debug.Assert(qnameSpan.IsValid);

            _aliasSpan = aliasSpan;
            _nameSpan = qnameSpan;
            _alias = alias;
            _qualifiedName = qualifiedName;
        }
    }

    /// <summary>
    /// Represents a grouped using.
    /// </summary>
    public sealed class GroupUse : UseBase
    {
        public QualifiedNameRef Prefix => _prefix;
        private readonly QualifiedNameRef _prefix;

        public SimpleUse[] Uses => _uses;
        private readonly SimpleUse[] _uses;

        public GroupUse(Span span, QualifiedNameRef prefix, List<SimpleUse> uses)
            : base(span)
        {
            Debug.Assert(span.IsValid);
            Debug.Assert(uses != null);

            _prefix = prefix;
            _uses = uses.ToArray();
        }
    }

    /// <summary>
    /// Represents <c>use</c> statement within the source code.
    /// </summary>
    public class UseStatement : Statement
    {
        /// <summary>
        /// The kind of the use statement.
        /// </summary>
        public AliasKind Kind => _kind;
        private readonly AliasKind _kind;

        /// <summary>
        /// Collection of actual uses within the statement.
        /// </summary>
        public UseBase[] Uses => _uses;
        private readonly UseBase[] _uses;

        public UseStatement(Span span, List<UseBase> uses, AliasKind kind)
            : base(span)
        {
            Debug.Assert(uses != null);

            _kind = kind;
            _uses = uses.ToArray();
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitUseStatement(this);
        }
    }

    #endregion
}
