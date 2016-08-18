using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Devsense.PHP.Text;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Errors;

namespace Devsense.PHP.Syntax
{
    internal enum ContextType { Class, Function, Constant }

    public partial class Parser
    {
        LanguageFeatures _features;
        ITokenProvider<SemanticValueType, Span> _lexer;
        INodesFactory<LangElement, Span> _astFactory;
        Scope _currentScope;
        NamespaceDecl _currentNamespace = null;
        List<NamingContext> _context = new List<NamingContext>();
        NamingContext _namingContext { get { return _context.Last(); } }
        ContextType _contextType = ContextType.Class;

        /// <summary>
        /// The root of AST.
        /// </summary>
        private LangElement _astRoot;

        protected sealed override int EofToken
        {
            get { return (int)Tokens.END; }
        }

        protected sealed override int ErrorToken
        {
            get { return (int)Tokens.T_ERROR; }
        }

        protected override Span CombinePositions(Span first, Span last)
        {
            if (last.IsValid)
            {
                if (first.IsValid)
                    return Span.Combine(first, last);
                else
                    return last;
            }
            else
                return first;
        }

        //$$ = AddToList<LangElement>($1, $3);
        //$$ = new List<LangElement>() { (LangElement)$1 };

        public LangElement Parse(
                ITokenProvider<SemanticValueType, Span> lexer, INodesFactory<LangElement, Span> astFactory,
                LanguageFeatures features, int positionShift = 0)
        {
            // initialization:
            this._features = features;
            this._lexer = new CompliantLexer(lexer);
            this._astFactory = astFactory;
            //InitializeFields();

            this._currentScope = new Scope(1); // starts assigning scopes from 2 (1 is reserved for prepended inclusion)

            base.Scanner = this._lexer;
            bool accept = base.Parse();
            Debug.Assert(accept, "Parser rejected the source code.");

            LangElement result = _astRoot;

            // clean and let GC collect unused AST and other stuff:
            //ClearFields();

            return result;
        }

        void SetNamingContext(string nameSpace)
        {
            _context.Add(new NamingContext(nameSpace, 1));
        }

        void ResetNamingContext()
        {
            _context.RemoveLast();
        }

        void AssignNamingContext()
        {
            if (_currentNamespace != null)
            {
                Debug.Assert(_context.Count == 2);
                Debug.Assert(_currentNamespace.Naming == _namingContext);
                ResetNamingContext();
            }
        }

        void AssignStatements(List<LangElement> statements)
        {
            Debug.Assert(statements.All(s => s == null || s is Statement), "Code contains an invalid statement.");
            if (statements.Count > 0 &&
                    statements.Any(s => s is NamespaceDecl && ((NamespaceDecl)s).IsSimpleSyntax) &&
                    statements.Any(s => s is NamespaceDecl && !((NamespaceDecl)s).IsSimpleSyntax))
                _astFactory.Error(Span.Combine(statements.First(s => s != null).Span, statements.Last(s => s != null).Span),
                    new ErrorInfo(9999, "Cannot mix bracketed namespace declarations with unbracketed namespace declarations", ErrorSeverity.Error),
                    null);
            List<LangElement> namespaces = new List<LangElement>();
            int i = 0;
            while ((i = statements.FindLastIndex(s => s is NamespaceDecl && ((NamespaceDecl)s).IsSimpleSyntax)) != -1)
            {
                int count = statements.Count - i;
                namespaces.Add(statements[i]);
                // add all the subsequent statements except the NamespaceDecl itself
                ((NamespaceDecl)statements[i]).Statements = statements.GetRange(i + 1, count - 1).Select(s => (Statement)s).ToList();
                statements.RemoveRange(i, count);
            }
            namespaces.Reverse(); // keep the original order
            statements.AddRange(namespaces);
        }

        private void AddAlias(Tuple<List<string>, string> alias)
        {
            AddAlias(alias, _contextType);
        }

        private void AddAlias(Tuple<List<string>, string> alias, ContextType contextType)
        {
            string aliasName = string.IsNullOrEmpty(alias.Item2) ? alias.Item1.Last() : alias.Item2;
            switch (contextType)
            {
                case ContextType.Class:
                    _namingContext.AddAlias(aliasName, new QualifiedName(alias.Item1, true, true));
                    break;
                case ContextType.Function:
                    _namingContext.AddFunctionAlias(aliasName, new QualifiedName(alias.Item1, true, true));
                    break;
                case ContextType.Constant:
                    _namingContext.AddConstantAlias(aliasName, new QualifiedName(alias.Item1, true, true));
                    break;
            }
        }

        private void AddAlias(List<string> prefix, Tuple<List<string>, string> alias)
        {
            AddAlias(new Tuple<List<string>, string>((List<string>)JoinLists(prefix, alias.Item1), alias.Item2));
        }

        private void AddAlias(List<string> prefix, Tuple<List<string>, string, ContextType> alias)
        {
            AddAlias(new Tuple<List<string>, string>((List<string>)JoinLists(prefix, alias.Item1), alias.Item2), alias.Item3);
        }

        private IList<T> AddToList<T>(IList<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        private IList<T> AddToList<T>(object list, object item)
        {
            return AddToList((List<T>)list, (T)item);
        }

        private IList<T> JoinLists<T>(IList<T> first, IList<T> second)
        {
            return first.Concat(second).ToList();
        }

        private IList<T> JoinLists<T>(object first, object second)
        {
            return JoinLists((List<T>)first, (List<T>)second);
        }

        private Tuple<T1, T2, T3> JoinTuples<T1, T2, T3>(Tuple<T1, T2> first, T3 second)
        {
            return new Tuple<T1, T2, T3>(first.Item1, first.Item2, second);
        }

        private LangElement StatementsToBlock(Span span, List<LangElement> statements, Tokens endToken)
        {
            return _astFactory.ColonBlock(span, statements, endToken);
        }

        private LangElement StatementsToBlock(Span span, object statements, Tokens endToken)
        {
            Debug.Assert(statements is List<LangElement>); 
            return StatementsToBlock(span, (List<LangElement>)statements, endToken);
        }

        private LangElement CreateProperty(Span span, LangElement objectExpr, object name)
        {
            if (name is Name)
                return _astFactory.Variable(span, new VariableName(((Name)name).Value), objectExpr);
            else
                return _astFactory.Variable(span, (LangElement)name, objectExpr);
        }

        private LangElement CreateStaticProperty(Span span, TypeRef objectName, Span objectNamePos, object name)
        {
            if (name is DirectVarUse)
                return _astFactory.Variable(span, ((DirectVarUse)name).VarName, objectName);
            else
                return _astFactory.Variable(span, ((IndirectVarUse)name).VarNameEx, objectName);
        }

        private LangElement CreateStaticProperty(Span span, LangElement objectExpr, Span objectNamePos, object name)
        {
            if (name is DirectVarUse)
                return _astFactory.Variable(span, ((DirectVarUse)name).VarName, (TypeRef)_astFactory.TypeReference(objectNamePos, objectExpr, null));
            else
                return _astFactory.Variable(span, ((IndirectVarUse)name).VarNameEx, (TypeRef)_astFactory.TypeReference(objectNamePos, objectExpr, null));
        }

        private List<T> RightTrimList<T>(List<T> list)
        {
            T elem;
            if (list.Count > 0 && (elem = list.Last()) == null)
                list.Remove(elem);
            return list;
        }

        private Span CombineSpans(params Span[] spans)
        {
            return new Span(spans.Min(s => s.Start), spans.Max(s => s.End));
        }

        void ResetDocBlock() => Scanner.DocBlock = null;
    }
}
