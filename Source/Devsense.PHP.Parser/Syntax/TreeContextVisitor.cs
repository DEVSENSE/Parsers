using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Syntax.Visitor;
using Devsense.PHP.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax.Visitor
{
    #region Dummy Nodes

    /// <summary>
    /// Dummy AST node representing a function header.
    /// </summary>
    internal sealed class DummyRoutineHeader : AstNode
    {
        public DummyRoutineHeader(AstNode decl, Span span) : base()
        {
            this.RoutineDecl = decl ?? throw new ArgumentNullException(nameof(decl));
            this.Span = span;
        }

        /// <summary>
        /// Original function or method declaration. (either <see cref="MethodDecl"/> or <see cref="FunctionDecl"/> or <see cref="LambdaFunctionExpr"/>).
        /// </summary>
        public AstNode RoutineDecl { get; private set; }

        /// <summary>
        /// Optional span of the header.
        /// </summary>
        public Span Span { get; private set; }
    }

    /// <summary>
    /// Represents <see cref="BlockStmt"/> when inside its closing tokens ({, }, :, endif, ...).
    /// </summary>
    public sealed class DummyInsideBlockStmt : AstNode
    {
        /// <summary>
        /// The original block statement.
        /// </summary>
        public ILangElement OriginalBlock { get; }

        public DummyInsideBlockStmt(ILangElement originalBlock)
        {
            this.OriginalBlock = originalBlock ?? throw new ArgumentNullException(nameof(originalBlock));
        }
    }

    #endregion

    #region ITreeContext

    /// <summary>
    /// Provides current AST context to the <see cref="ITokenComposer"/>.
    /// </summary>
    public interface ITreeContext
    {
        /// <summary>
        /// Path from the current node to the root.
        /// </summary>
        IEnumerable<AstNode> Scope { get; }

        /// <summary>
        /// Provided <see cref="ILineBreaks"/> instance which corresponds to provided <see cref="Span"/>s.
        /// </summary>
        ILineBreaks LineBreaks { get; }
    }

    #endregion

    #region TreeContext

    /// <summary>
    /// Provides information about current node context.
    /// </summary>
    public class TreeContext : ITreeContext
    {
        readonly GlobalCode _ast;
        readonly Stack<AstNode> _scope = new Stack<AstNode>();

        /// <summary>
        /// Path from the current node to the root.
        /// </summary>
        public IEnumerable<AstNode> Scope => _scope;

        /// <summary>
        /// Provided <see cref="ILineBreaks"/> instance which corresponds to provided <see cref="Span"/>s.
        /// Can be <c>null</c>.
        /// </summary>
        public ILineBreaks LineBreaks => _ast?.ContainingSourceUnit?.LineBreaks;

        /// <summary>
        /// Constructs the object.
        /// </summary>
        public TreeContext(GlobalCode ast = null)
        {
            _ast = ast;
        }

        /// <summary>
        /// Enter the scope.
        /// </summary>
        public virtual void EnterScope(AstNode node)
        {
            _scope.Push(node);
        }

        /// <summary>
        /// Leave the scope. Must corespond to <see cref="EnterScope"/>
        /// </summary>
        public virtual void LeaveScope(AstNode node)
        {
            var pop = _scope.Pop();
            Debug.Assert(pop == node);
        }
    }

    #endregion

    #region ITreeContextExtension

    /// <summary>
    /// <see cref="ITreeContextExtention"/> extension methods.
    /// </summary>
    public static class ITreeContextExtention
    {
        /// <summary>
        /// Gets the first scope of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Node type. Of type <see cref="AstNode"/>.</typeparam>
        /// <param name="context">Visitor context.</param>
        /// <returns>Node of type <typeparamref name="T"/> or <c>null</c> if the context does not have such node.</returns>
        public static T Get<T>(this ITreeContext context) where T : AstNode => context.Scope.OfType<T>().FirstOrDefault();

        /// <summary>
        /// Gets the first scope of type <typeparamref name="T"/> and matching given predicate.
        /// </summary>
        /// <typeparam name="T">Node type. Of type <see cref="AstNode"/>.</typeparam>
        /// <param name="context">Visitor context.</param>
        /// <param name="predicate">Additional predicate to filter scopes.</param>
        /// <returns>Node of type <typeparamref name="T"/> or <c>null</c> if the context does not have such scopes.</returns>
        public static T Get<T>(this ITreeContext context, Func<T, bool> predicate) where T : AstNode => context.Scope.OfType<T>().FirstOrDefault(predicate);

        /// <summary>
        /// Determines if the context is in given scopes.
        /// </summary>
        public static bool IsIn<T>(this ITreeContext context) where T : AstNode => Get<T>(context) != null;

        /// <summary>
        /// Determines if the context is in given scopes.
        /// </summary>
        public static bool IsIn<T>(this ITreeContext context, Func<T, bool> predicate) where T : AstNode => Get<T>(context, predicate) != null;

        /// <summary>
        /// Gets value indicating the context is in a routine header (either function, method, lambda).
        /// </summary>
        public static bool IsInRoutineHeader(this ITreeContext context, out Span span)
        {
            var header = Get<DummyRoutineHeader>(context);
            if (header != null)
            {
                span = header.Span;
                return true;
            }
            else
            {
                span = Span.Invalid;
                return false;
            }
        }

        /// <summary>
        /// Gets value indicating the state is at an opening or closing tokens of a block statememnt.
        /// </summary>
        public static bool IsAtBlockBounds(this ITreeContext context)
        {
            foreach (var x in context.Scope)
            {
                if (x is DummyInsideBlockStmt)
                {
                    break; // we are inside
                }

                if (x is IBlockStatement || x is SwitchStmt || x is SwitchItem || x is TypeDecl || x is TraitAdaptationBlock)
                {
                    return true; // we are in block but didn't reach the inside -> boundaries
                }
            }

            return false;
        }

        /// <summary>
        /// Counts scopes matching given predicate.
        /// </summary>
        public static int Count(this ITreeContext context, Func<AstNode, bool> predicate) => context.Count(predicate);

        ///// <summary>
        ///// Counts scopes that causes an indentation by default.
        ///// Gets raugh level of indentation at current context state (blocks, type declaration).
        ///// </summary>
        //public static int CountIndent(this ITreeContext context) => Count(context, s_indentnodes);

        //readonly static Func<AstNode, bool> s_indentnodes = new Func<AstNode, bool>( // sample indent function, should be replaced by actual implementation
        //    node => (node is DummyInsideBlockStmt db && !(db.OriginalBlock is SimpleBlockStmt)) || node is TypeDecl);
    }

    #endregion
}

namespace Devsense.PHP.Syntax
{
    public class TreeContextVisitor : TreeVisitor
    {
        #region ScopeHelper

        /// <summary>
        /// Helper object that nests <see cref="TreeContextVisitor.Context"/> in its constructor and un-nests in its <see cref="IDisposable.Dispose"/>.
        /// To be used within <c>use</c> block.
        /// </summary>
        internal struct ScopeHelper : IDisposable
        {
            readonly TreeContextVisitor _visitor;
            readonly AstNode _node;

            public ScopeHelper(TreeContextVisitor visitor, AstNode scope)
            {
                Debug.Assert(visitor != null);

                _visitor = visitor;
                _node = scope ?? throw new ArgumentNullException(nameof(scope));

                visitor.Context.EnterScope(scope);
            }

            public void Dispose()
            {
                _visitor.Context.LeaveScope(_node);
            }
        }

        #endregion

        /// <summary>
        /// Current visitor context.
        /// </summary>
        protected TreeContext Context { get; private set; }

        protected virtual bool IsScopeElement(LangElement element)
        {
            return
                element is IStatement ||
                element is GlobalCode ||
                // element is TypeDecl || // handled in TokenVisitor
                element is TraitsUse.TraitAdaptation ||
                element is MethodDecl ||
                element is LambdaFunctionExpr ||
                element is CatchItem ||
                element is FieldDeclList ||
                element is FinallyItem;
        }

        public TreeContextVisitor(TreeContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override void VisitElement(LangElement element)
        {
            if (IsScopeElement(element))
            {
                using (new ScopeHelper(this, element))
                {
                    base.VisitElement(element);
                }
            }
            else
            {
                base.VisitElement(element);
            }
        }

        public override void VisitConditionalStmt(ConditionalStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                base.VisitConditionalStmt(x);
            }
        }
    }
}
