using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PHP.Core.Text;
using PHP.Core.AST;
using PHP.Syntax;
using System.IO;
using System.Diagnostics;

namespace PhpParser.Parser
{
    internal enum ContextType { Class, Function, Constant }

    public partial class Parser
    {
        LanguageFeatures _features;
        ITokenProvider<SemanticValueType, Span> _lexer;
        INodesFactory<LangElement, Span> _astFactory;
        Scope _currentScope;
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

        public LangElement Parse(
            ITokenProvider<SemanticValueType, Span> lexer, INodesFactory<LangElement, Span> astFactory,
            LanguageFeatures features, int positionShift = 0)
        {
            // initialization:
            this._features = features;
            this._lexer = lexer;
            this._astFactory = astFactory;
            //InitializeFields();

            this._currentScope = new Scope(1); // starts assigning scopes from 2 (1 is reserved for prepended inclusion)

            base.Scanner = this._lexer;
            base.Parse();

            LangElement result = _astRoot;

            // clean and let GC collect unused AST and other stuff:
            //ClearFields();

            return result;
        }

        private void RESET_DOC_COMMENT()
        {
            // TODO implement
        }

        private LangElement zend_ast_create(params object[] abc)
        {
            // TODO implement
            return null;
        }

        private LangElement zend_ast_create_decl(params object[] abc)
        {
            // TODO implement
            return null;
        }

        private LangElement zend_handle_encoding_declaration(params object[] abc)
        {
            // TODO implement
            return null;
        }

        private LangElement zend_ast_create_list(params object[] abc)
        {
            // TODO implement
            return null;
        }

        private LangElement zend_ast_list_add(params object[] abc)
        {
            // TODO implement
            return null;
        }

        private long CG(params object[] abc)
        {
            // TODO implement
            return 0;
        }

        private void SetNamingContext(string nameSpace)
        {
            _context.Add(new NamingContext(nameSpace, 1));
        }

        private void ResetNamingContext()
        {
            _context.RemoveLast();
        }

        private void AddAlias(Tuple<List<string>, string> alias)
        {
            AddAlias(alias, _contextType);
        }

        private void AddAlias(Tuple<List<string>, string> alias, ContextType contextType)
        {
            switch (contextType)
            {
                case ContextType.Class:
                    _namingContext.AddAlias(alias.Item2, new QualifiedName(alias.Item1, true, true));
                    break;
                case ContextType.Function:
                    _namingContext.AddFunctionAlias(alias.Item2, new QualifiedName(alias.Item1, true, true));
                    break;
                case ContextType.Constant:
                    _namingContext.AddConstantAlias(alias.Item2, new QualifiedName(alias.Item1, true, true));
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

        enum _zend_ast_kind
        {
            /* special nodes */
            ZEND_AST_ZVAL,
            ZEND_AST_ZNODE,

            /* declaration nodes */
            ZEND_AST_FUNC_DECL,
            ZEND_AST_CLOSURE,
            ZEND_AST_METHOD,
            ZEND_AST_CLASS,

            /* list nodes */
            ZEND_AST_ARG_LIST,
            ZEND_AST_ARRAY,
            ZEND_AST_ENCAPS_LIST,
            ZEND_AST_EXPR_LIST,
            ZEND_AST_STMT_LIST,
            ZEND_AST_IF,
            ZEND_AST_SWITCH_LIST,
            ZEND_AST_CATCH_LIST,
            ZEND_AST_PARAM_LIST,
            ZEND_AST_CLOSURE_USES,
            ZEND_AST_PROP_DECL,
            ZEND_AST_CONST_DECL,
            ZEND_AST_CLASS_CONST_DECL,
            ZEND_AST_NAME_LIST,
            ZEND_AST_TRAIT_ADAPTATIONS,
            ZEND_AST_USE,

            /* 0 child nodes */
            ZEND_AST_MAGIC_CONST,
            ZEND_AST_TYPE,

            /* 1 child node */
            ZEND_AST_VAR,
            ZEND_AST_CONST,
            ZEND_AST_UNPACK,
            ZEND_AST_UNARY_PLUS,
            ZEND_AST_UNARY_MINUS,
            ZEND_AST_CAST,
            ZEND_AST_EMPTY,
            ZEND_AST_ISSET,
            ZEND_AST_SILENCE,
            ZEND_AST_SHELL_EXEC,
            ZEND_AST_CLONE,
            ZEND_AST_EXIT,
            ZEND_AST_PRINT,
            ZEND_AST_INCLUDE_OR_EVAL,
            ZEND_AST_UNARY_OP,
            ZEND_AST_PRE_INC,
            ZEND_AST_PRE_DEC,
            ZEND_AST_POST_INC,
            ZEND_AST_POST_DEC,
            ZEND_AST_YIELD_FROM,

            ZEND_AST_GLOBAL,
            ZEND_AST_UNSET,
            ZEND_AST_RETURN,
            ZEND_AST_LABEL,
            ZEND_AST_REF,
            ZEND_AST_HALT_COMPILER,
            ZEND_AST_ECHO,
            ZEND_AST_THROW,
            ZEND_AST_GOTO,
            ZEND_AST_BREAK,
            ZEND_AST_CONTINUE,

            /* 2 child nodes */
            ZEND_AST_DIM,
            ZEND_AST_PROP,
            ZEND_AST_STATIC_PROP,
            ZEND_AST_CALL,
            ZEND_AST_CLASS_CONST,
            ZEND_AST_ASSIGN,
            ZEND_AST_ASSIGN_REF,
            ZEND_AST_ASSIGN_OP,
            ZEND_AST_BINARY_OP,
            ZEND_AST_GREATER,
            ZEND_AST_GREATER_EQUAL,
            ZEND_AST_AND,
            ZEND_AST_OR,
            ZEND_AST_ARRAY_ELEM,
            ZEND_AST_NEW,
            ZEND_AST_INSTANCEOF,
            ZEND_AST_YIELD,
            ZEND_AST_COALESCE,

            ZEND_AST_STATIC,
            ZEND_AST_WHILE,
            ZEND_AST_DO_WHILE,
            ZEND_AST_IF_ELEM,
            ZEND_AST_SWITCH,
            ZEND_AST_SWITCH_CASE,
            ZEND_AST_DECLARE,
            ZEND_AST_USE_TRAIT,
            ZEND_AST_TRAIT_PRECEDENCE,
            ZEND_AST_METHOD_REFERENCE,
            ZEND_AST_NAMESPACE,
            ZEND_AST_USE_ELEM,
            ZEND_AST_TRAIT_ALIAS,
            ZEND_AST_GROUP_USE,

            /* 3 child nodes */
            ZEND_AST_METHOD_CALL,
            ZEND_AST_STATIC_CALL,
            ZEND_AST_CONDITIONAL,

            ZEND_AST_TRY,
            ZEND_AST_CATCH,
            ZEND_AST_PARAM,
            ZEND_AST_PROP_ELEM,
            ZEND_AST_CONST_ELEM,

            /* 4 child nodes */
            ZEND_AST_FOR,
            ZEND_AST_FOREACH,
        };
    }
}
