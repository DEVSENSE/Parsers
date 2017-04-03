using Devsense.PHP.Syntax.Ast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax
{
    #region TreeContext
    
    /// <summary>
    /// Provides information about current node context.
    /// </summary>
    public class TreeContext
    {
        readonly TreeContext _parentContext;
        readonly AstNode _scope;

        /// <summary>
        /// Creates a nested context.
        /// </summary>
        public virtual TreeContext NextContext(AstNode scope /* ... */)
        {
            return new TreeContext(scope, this);
        }

        /// <summary>
        /// Constructs an initial tree context.
        /// </summary>
        public TreeContext(AstNode scope)
            :this(scope, null)
        {
        }

        /// <summary>
        /// Constructs nested tree context.
        /// </summary>
        /// <param name="scope">Current tree scope.</param>
        /// <param name="parent">Parent context, cannot be <c>null</c>.</param>
        protected TreeContext(AstNode scope, TreeContext parent)
        {
            if (scope == null) throw new ArgumentNullException(nameof(scope));

            _parentContext = parent;
            _scope = scope;
        }

        /// <summary>
        /// Number of nested scopes.
        /// </summary>
        public virtual int Level => (_parentContext != null) ? (1 + _parentContext.Level) : 1;

        /// <summary>
        /// Gets reference to the current scope.
        /// </summary>
        public AstNode Scope => _scope;

        /// <summary>
        /// Gets previous scope context.
        /// </summary>
        public virtual TreeContext ParentContext => _parentContext;

        // TODO: last HTML code
    }

    #endregion

    public class TreeContextVisitor : TreeVisitor
    {
        #region ScopeHelper

        /// <summary>
        /// Helper object that nests <see cref="TreeContextVisitor._context"/> in its constructor and un-nests in its <see cref="IDisposable.Dispose"/>.
        /// To be used within <c>use</c> block.
        /// </summary>
        protected struct ScopeHelper : IDisposable
        {
            readonly TreeContextVisitor _visitor;

            public ScopeHelper(TreeContextVisitor visitor, AstNode scope)
            {
                Debug.Assert(visitor != null);
                Debug.Assert(scope != null);

                _visitor = visitor;
                visitor._context = visitor._context.NextContext(scope);
            }

            public void Dispose()
            {
                Debug.Assert(_visitor != null);
                Debug.Assert(_visitor._context != null);

                _visitor._context = _visitor._context.ParentContext;
            }
        }

        #endregion

        /// <summary>
        /// Current visitor context.
        /// </summary>
        protected TreeContext Context => _context;
        private TreeContext _context;

        public TreeContextVisitor(TreeContext initialContext)
        {
            if (initialContext == null) throw new ArgumentNullException(nameof(initialContext));

            _context = initialContext;
        }

        public override void VisitGlobalCode(GlobalCode x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitGlobalCode(x);
            }
        }

        public override void VisitTypeDecl(TypeDecl x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitTypeDecl(x);
            }
        }

        public override void VisitMethodDecl(MethodDecl x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitMethodDecl(x);
            }
        }

        public override void VisitFunctionDecl(FunctionDecl x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitFunctionDecl(x);
            }
        }

        public override void VisitLambdaFunctionExpr(LambdaFunctionExpr x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitLambdaFunctionExpr(x);
            }
        }

        public override void VisitNamespaceDecl(NamespaceDecl x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitNamespaceDecl(x);
            }
        }

        public override void VisitBlockStmt(BlockStmt x)
        {

            using (new ScopeHelper(this, x))
            {
                base.VisitBlockStmt(x);
            }
        }

        public override void VisitConditionalStmt(ConditionalStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitConditionalStmt(x);
            }
        }

        public override void VisitForeachStmt(ForeachStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitForeachStmt(x);
            }
        }

        public override void VisitForStmt(ForStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitForStmt(x);
            }
        }

        public override void VisitWhileStmt(WhileStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitWhileStmt(x);
            }
        }

        public override void VisitTryStmt(TryStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitTryStmt(x);
            }
        }

        public override void VisitCatchItem(CatchItem x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitCatchItem(x);
            }
        }

        public override void VisitFinallyItem(FinallyItem x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitFinallyItem(x);
            }
        }
    }
}
