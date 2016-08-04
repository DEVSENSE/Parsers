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
            this._lexer = lexer;
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
            List<LangElement> namespaces = new List<LangElement>();
            int i = 0;
            while((i = statements.FindLastIndex(s => s is NamespaceDecl && ((NamespaceDecl)s).IsSimpleSyntax)) != -1)
            {
                int count = statements.Count - i;
                namespaces.Add(statements[i]);
                // add all the subsequent statements except the NamespaceDecl itself
                ((NamespaceDecl)statements[i]).Statements = statements.GetRange(i+1, count-1).Select(s => (Statement)s).ToList();
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

        private LangElement StatementsToBlock(Span span, List<LangElement> statements)
        {
            if (statements.Count > 1)
                return _astFactory.Block(span, statements);
            else return statements.First();
        }

        private LangElement StatementsToBlock(Span span, object statements)
        {
            Debug.Assert(statements is List<LangElement>);
            return StatementsToBlock(span, (List<LangElement>)statements);
        }

        private LangElement StatementBlock(Span span, object statements)
        {
            return (statements is Statement) ? (LangElement)statements : StatementsToBlock(span, statements);
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

        enum _zend_sup
        {
            ZEND_TYPE_nullABLE,
            ZEND_PARAM_REF,
            ZEND_PARAM_VARIADIC,
            ZEND_INCLUDE,
            ZEND_INCLUDE_ONCE,
            ZEND_EVAL,
            ZEND_REQUIRE,
            ZEND_REQUIRE_ONCE,
            ZEND_NOP,
            ZEND_ADD,
            ZEND_SUB,
            ZEND_MUL,
            ZEND_DIV,
            ZEND_MOD,
            ZEND_SL,
            ZEND_SR,
            ZEND_CONCAT,
            ZEND_BW_OR,
            ZEND_BW_AND,
            ZEND_BW_XOR,
            ZEND_BW_NOT,
            ZEND_BOOL_NOT,
            ZEND_BOOL_XOR,
            ZEND_IS_IDENTICAL,
            ZEND_IS_NOT_IDENTICAL,
            ZEND_IS_EQUAL,
            ZEND_IS_NOT_EQUAL,
            ZEND_IS_SMALLER,
            ZEND_IS_SMALLER_OR_EQUAL,
            ZEND_CAST,
            ZEND_QM_ASSIGN,
            ZEND_ASSIGN_ADD,
            ZEND_ASSIGN_SUB,
            ZEND_ASSIGN_MUL,
            ZEND_ASSIGN_DIV,
            ZEND_ASSIGN_MOD,
            ZEND_ASSIGN_SL,
            ZEND_ASSIGN_SR,
            ZEND_ASSIGN_CONCAT,
            ZEND_ASSIGN_BW_OR,
            ZEND_ASSIGN_BW_AND,
            ZEND_ASSIGN_BW_XOR,
            ZEND_PRE_INC,
            ZEND_PRE_DEC,
            ZEND_POST_INC,
            ZEND_POST_DEC,
            ZEND_ASSIGN,
            ZEND_ASSIGN_REF,
            ZEND_ECHO,
            ZEND_GENERATOR_CREATE,
            ZEND_JMP,
            ZEND_JMPZ,
            ZEND_JMPNZ,
            ZEND_JMPZNZ,
            ZEND_JMPZ_EX,
            ZEND_JMPNZ_EX,
            ZEND_CASE,
            ZEND_SEND_VAR_NO_REF_EX,
            ZEND_BOOL,
            ZEND_FAST_CONCAT,
            ZEND_ROPE_INIT,
            ZEND_ROPE_ADD,
            ZEND_ROPE_END,
            ZEND_BEGIN_SILENCE,
            ZEND_END_SILENCE,
            ZEND_INIT_FCALL_BY_NAME,
            ZEND_DO_FCALL,
            ZEND_INIT_FCALL,
            ZEND_RETURN,
            ZEND_RECV,
            ZEND_RECV_INIT,
            ZEND_SEND_VAL,
            ZEND_SEND_VAR_EX,
            ZEND_SEND_REF,
            ZEND_NEW,
            ZEND_INIT_NS_FCALL_BY_NAME,
            ZEND_FREE,
            ZEND_INIT_ARRAY,
            ZEND_ADD_ARRAY_ELEMENT,
            ZEND_INCLUDE_OR_EVAL,
            ZEND_UNSET_VAR,
            ZEND_UNSET_DIM,
            ZEND_UNSET_OBJ,
            ZEND_FE_RESET_R,
            ZEND_FE_FETCH_R,
            ZEND_EXIT,
            ZEND_FETCH_R,
            ZEND_FETCH_DIM_R,
            ZEND_FETCH_OBJ_R,
            ZEND_FETCH_W,
            ZEND_FETCH_DIM_W,
            ZEND_FETCH_OBJ_W,
            ZEND_FETCH_RW,
            ZEND_FETCH_DIM_RW,
            ZEND_FETCH_OBJ_RW,
            ZEND_FETCH_IS,
            ZEND_FETCH_DIM_IS,
            ZEND_FETCH_OBJ_IS,
            ZEND_FETCH_FUNC_ARG,
            ZEND_FETCH_DIM_FUNC_ARG,
            ZEND_FETCH_OBJ_FUNC_ARG,
            ZEND_FETCH_UNSET,
            ZEND_FETCH_DIM_UNSET,
            ZEND_FETCH_OBJ_UNSET,
            ZEND_FETCH_LIST,
            ZEND_FETCH_CONSTANT,
            ZEND_EXT_STMT,
            ZEND_EXT_FCALL_BEGIN,
            ZEND_EXT_FCALL_END,
            ZEND_EXT_NOP,
            ZEND_TICKS,
            ZEND_SEND_VAR_NO_REF,
            ZEND_CATCH,
            ZEND_THROW,
            ZEND_FETCH_CLASS,
            ZEND_CLONE,
            ZEND_RETURN_BY_REF,
            ZEND_INIT_METHOD_CALL,
            ZEND_INIT_STATIC_METHOD_CALL,
            ZEND_ISSET_ISEMPTY_VAR,
            ZEND_ISSET_ISEMPTY_DIM_OBJ,
            ZEND_SEND_VAL_EX,
            ZEND_SEND_VAR,
            ZEND_INIT_USER_CALL,
            ZEND_SEND_ARRAY,
            ZEND_SEND_USER,
            ZEND_STRLEN,
            ZEND_DEFINED,
            ZEND_TYPE_CHECK,
            ZEND_VERIFY_RETURN_TYPE,
            ZEND_FE_RESET_RW,
            ZEND_FE_FETCH_RW,
            ZEND_FE_FREE,
            ZEND_INIT_DYNAMIC_CALL,
            ZEND_DO_ICALL,
            ZEND_DO_UCALL,
            ZEND_DO_FCALL_BY_NAME,
            ZEND_PRE_INC_OBJ,
            ZEND_PRE_DEC_OBJ,
            ZEND_POST_INC_OBJ,
            ZEND_POST_DEC_OBJ,
            ZEND_ASSIGN_OBJ,
            ZEND_OP_DATA,
            ZEND_INSTANCEOF,
            ZEND_DECLARE_CLASS,
            ZEND_DECLARE_INHERITED_CLASS,
            ZEND_DECLARE_FUNCTION,
            ZEND_YIELD_FROM,
            ZEND_DECLARE_CONST,
            ZEND_ADD_INTERFACE,
            ZEND_DECLARE_INHERITED_CLASS_DELAYED,
            ZEND_VERIFY_ABSTRACT_CLASS,
            ZEND_ASSIGN_DIM,
            ZEND_ISSET_ISEMPTY_PROP_OBJ,
            ZEND_HANDLE_EXCEPTION,
            ZEND_USER_OPCODE,
            ZEND_ASSERT_CHECK,
            ZEND_JMP_SET,
            ZEND_DECLARE_LAMBDA_FUNCTION,
            ZEND_ADD_TRAIT,
            ZEND_BIND_TRAITS,
            ZEND_SEPARATE,
            ZEND_FETCH_CLASS_NAME,
            ZEND_CALL_TRAMPOLINE,
            ZEND_DISCARD_EXCEPTION,
            ZEND_YIELD,
            ZEND_GENERATOR_RETURN,
            ZEND_FAST_CALL,
            ZEND_FAST_RET,
            ZEND_RECV_VARIADIC,
            ZEND_SEND_UNPACK,
            ZEND_POW,
            ZEND_ASSIGN_POW,
            ZEND_BIND_GLOBAL,
            ZEND_COALESCE,
            ZEND_SPACESHIP,
            ZEND_DECLARE_ANON_CLASS,
            ZEND_DECLARE_ANON_INHERITED_CLASS,
            ZEND_FETCH_STATIC_PROP_R,
            ZEND_FETCH_STATIC_PROP_W,
            ZEND_FETCH_STATIC_PROP_RW,
            ZEND_FETCH_STATIC_PROP_IS,
            ZEND_FETCH_STATIC_PROP_FUNC_ARG,
            ZEND_FETCH_STATIC_PROP_UNSET,
            ZEND_UNSET_STATIC_PROP,
            ZEND_ISSET_ISEMPTY_STATIC_PROP,
            ZEND_FETCH_CLASS_CONSTANT,
            ZEND_BIND_LEXICAL,
            ZEND_BIND_STATIC,
            ZEND_FETCH_THIS,
            ZEND_ISSET_ISEMPTY_THIS,
            ZEND_ACC_STATIC,
            ZEND_ACC_ABSTRACT,
            ZEND_ACC_FINAL,
            ZEND_ACC_IMPLEMENTED_ABSTRACT,
            ZEND_ACC_IMPLICIT_ABSTRACT_CLASS,
            ZEND_ACC_EXPLICIT_ABSTRACT_CLASS,
            ZEND_ACC_INTERFACE,
            ZEND_ACC_TRAIT,
            ZEND_ACC_ANON_CLASS,
            ZEND_ACC_ANON_BOUND,
            ZEND_ACC_PUBLIC,
            ZEND_ACC_PROTECTED,
            ZEND_ACC_PRIVATE,
            ZEND_ACC_PPP_MASK,
            ZEND_ACC_CHANGED,
            ZEND_ACC_IMPLICIT_PUBLIC,
            ZEND_ACC_CTOR,
            ZEND_ACC_DTOR,
            ZEND_ACC_CLONE,
            ZEND_ACC_USER_ARG_INFO,
            ZEND_ACC_ALLOW_STATIC,
            ZEND_ACC_SHADOW,
            ZEND_ACC_DEPRECATED,
            ZEND_ACC_IMPLEMENT_INTERFACES,
            ZEND_ACC_IMPLEMENT_TRAITS,
            ZEND_ACC_CONSTANTS_UPDATED,
            ZEND_HAS_STATIC_IN_METHODS,
            ZEND_ACC_CLOSURE,
            ZEND_ACC_GENERATOR,
            ZEND_ACC_NO_RT_ARENA,
            ZEND_ACC_CALL_VIA_TRAMPOLINE,
            ZEND_ACC_CALL_VIA_HANDLER,
            ZEND_ACC_NEVER_CACHE,
            ZEND_ACC_VARIADIC,
            ZEND_ACC_RETURN_REFERENCE,
            ZEND_ACC_DONE_PASS_TWO,
            ZEND_ACC_USE_GUARDS,
            ZEND_ACC_HAS_TYPE_HINTS,
            ZEND_ACC_HAS_FINALLY_BLOCK,
            ZEND_ACC_ARENA_ALLOCATED,
            ZEND_ACC_HAS_RETURN_TYPE,
            ZEND_ACC_STRICT_TYPES,
            IS_UNDEF,
            IS_NULL,
            IS_FALSE,
            IS_TRUE,
            IS_LONG,
            IS_DOUBLE,
            IS_STRING,
            IS_ARRAY,
            IS_OBJECT,
            IS_RESOURCE,
            IS_REFERENCE,
            IS_CONSTANT,
            IS_CONSTANT_AST,
            _IS_BOOL,
            IS_CALLABLE,
            IS_VOID,
            IS_INDIRECT,
            IS_PTR,
            _IS_ERROR,
            T_LINE,
            T_FILE,
            T_DIR,
            T_TRAIT_C,
            T_METHOD_C,
            T_FUNC_C,
            T_NS_C,
            T_CLASS_C
        };

        private void RESET_DOC_COMMENT()
        {
            // reades and deletes the actual doc block
            string doc = _lexer.DocBlock;
        }

        private object zend_ast_create(params object[] abc)
        {
            // TODO implement
            return null;
        }

        private object zend_ast_create_decl(params object[] abc)
        {
            // TODO implement
            return null;
        }

        private long CG(params object[] abc)
        {
            // TODO implement
            return 0;
        }

        private object zend_ast_create_ex(params object[] abc)
        {
            // TODO implement
            return 0;
        }

        private object zend_lex_tstring(params object[] abc)
        {
            // TODO implement
            return 0;
        }

        private object zend_ast_get_str(params object[] abc)
        {
            // TODO implement
            return 0;
        }

        private object zend_ast_create_zval(params object[] abc)
        {
            // TODO implement
            return 0;
        }

        private object zend_ast_list_rtrim(params object[] abc)
        {
            // TODO implement
            return 0;
        }

        long zend_lineno = 0; // TODO implement
    }
}
