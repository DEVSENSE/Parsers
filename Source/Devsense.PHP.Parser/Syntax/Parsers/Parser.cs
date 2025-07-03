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
using System.Linq;

using Devsense.PHP.Text;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Errors;
using System.Threading;
using Devsense.PHP.Utilities;

namespace Devsense.PHP.Syntax
{
    public partial class Parser
    {
        IParserTokenProvider<SemanticValueType, Span> _lexer;
        INodesFactory<LangElement, Span> _astFactory;
        IErrorSink<Span> _errors;
        IErrorRecovery _errorRecovery;
        Scope _currentScope;
        bool isConditional => !_currentScope.IsGlobal;
        NamespaceDecl _currentNamespace = null;
        Stack<NamingContext> _namingContext;
        NamingContext namingContext => _namingContext.Peek();
        AliasKind _contextType = AliasKind.Type;
        LanguageFeatures _languageFeatures;

        int _recoveryCount = 0;
        const int _recoveryLimit = 100;

        /// <summary>
        /// The root of AST.
        /// </summary>
        private LangElement _astRoot;

        /// <summary>
        /// Gets parser error sink. Cannot be <c>null</c>.
        /// </summary>
        public IErrorSink<Span> ErrorSink => _errors;

        /// <summary><c>(LangElement)null</c></summary>
        static LangElement NullLangElement => null;

        /// <summary>
        /// Stack of class contexts used to resolve reserved type names and to determine whether we are inside or outside the class.
        /// </summary>
        Stack<ClassContext> ClassContexts { get { return _classContexts ?? (_classContexts = StackObjectPool<ClassContext>.Allocate()); } }
        Stack<ClassContext> _classContexts = null;

        bool IsInClassContext { get { return _classContexts != null && _classContexts.Count != 0; } }

        struct ClassContext
        {
            public QualifiedName Name;
            public TypeRef Base;
            public PhpMemberAttributes Attributes;
        }

        protected sealed override int EofToken
        {
            get { return (int)Tokens.END; }
        }

        protected sealed override int ErrorToken
        {
            get { return (int)Tokens.T_ERROR; }
        }

        protected override Span InvalidPosition => Span.Invalid;

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
                ITokenProvider<SemanticValueType, Span> lexer,
                INodesFactory<LangElement, Span> astFactory,
                LanguageFeatures language,
                IErrorSink<Span> errors = null,
                IErrorRecovery errorRecovery = null,
                int positionShift = 0)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            // initialization:

            _languageFeatures = language;
            _lexer = new CompliantLexer(lexer, language);
            _astFactory = astFactory ?? throw new ArgumentNullException(nameof(astFactory));
            _errors = errors ?? new EmptyErrorSink<Span>();
            _namingContext = StackObjectPool<NamingContext>.Allocate();

            if (errorRecovery != null && errorRecovery != EmptyErrorRecovery.Instance)
            {
                _lexer = new BufferedLexer(_lexer);
                _errorRecovery = errorRecovery;
            }
            else
            {
                _errorRecovery = EmptyErrorRecovery.Instance;
            }
            //InitializeFields();

            _currentScope = new Scope(0);
            _recoveryCount = 0;

            this.value_stack.Clear();
            base.Scanner = _lexer;
            bool accept = base.Parse();

            // cleanup
            if (_classContexts != null)
            {
                StackObjectPool<ClassContext>.Free(_classContexts);
                _classContexts = null;
            }
            if (_namingContext != null)
            {
                StackObjectPool<NamingContext>.Free(_namingContext);
                _namingContext = null;
            }

            //
            return _astRoot;
        }

        public void Clear()
        {
            base.Initialize();

            _astRoot = null;
            _lazyPrimitiveTypes = null;
            _lexer = null;
            _astFactory = null;
            _currentNamespace = null;
            _contextType = AliasKind.Type;
            _namingContext = null;
            _classContexts = null;
            _errors = null;
            _errorRecovery = null;
        }

        void SetNamingContext(List<string> ns)
        {
            var qname = (ns != null && ns.Count != 0)
                ? (QualifiedName?)new QualifiedName(ns, false, true)
                : null;

            _namingContext.Push(new NamingContext(qname));
        }

        void ResetNamingContext()
        {
            _namingContext.Pop();
        }

        void AssignNamingContext()
        {
            if (_currentNamespace != null)
            {
                Debug.Assert(_namingContext.Count == 2);
                Debug.Assert(_currentNamespace.Naming == namingContext);
                ResetNamingContext();
            }
        }

        void AssignStatements(List<LangElement> statements)
        {
            Debug.Assert(statements.All(s => s != null && s is Statement), "Code contains an invalid statement.");

            bool hasNsSimple = false, hasNsBracket = false, hasStmt = false;

            for (int i = 0; i < statements.Count; i++)
            {
                var stmt = (Statement)statements[i];
                if (stmt is NamespaceDecl ns)
                {
                    if (hasStmt)
                    {
                        // TODO: Error, all statements must be in a namespace
                    }

                    if (ns.IsSimpleSyntax)
                    {
                        hasNsSimple = true;

                        if (i + 1 < statements.Count)
                        {
                            // find next namespace declaration
                            var next_ns_index = statements.FindIndex(i + 1, x => x is NamespaceDecl);
                            if (next_ns_index < 0)
                            {
                                next_ns_index = statements.Count;
                            }

                            // copy following statements into the namespace declaration
                            var count = next_ns_index - i - 1;
                            if (count != 0)
                            {
                                var newcontaining = new Statement[count];
                                for (int j = 0; j < count; j++)
                                {
                                    newcontaining[j] = (Statement)statements[j + i + 1];
                                }

                                statements.RemoveRange(i + 1, count);
                                Span newSpan = newcontaining.Length > 0 ? CombineSpans(newcontaining.First().Span, newcontaining.Last().Span) : new Span(ns.Span.End, 0);
                                ns.Body = (BlockStmt)_astFactory.SimpleBlock(newSpan, newcontaining);
                            }
                            else
                            {
                                ns.Body = EmptyNamespaceBody(ns.Span);
                            }
                        }
                        else
                        {
                            ns.Body = EmptyNamespaceBody(ns.Span);
                        }
                    }
                    else
                    {
                        hasNsBracket = true;
                    }

                    if (hasNsSimple && hasNsBracket)
                    {
                        var span = ns.QualifiedName.Span;
                        if (span.IsValid == false)
                        {
                            span = new Span(ns.Span.Start, "namespace".Length);
                        }

                        this.ErrorSink.Error(ns.QualifiedName.Span, FatalErrors.MixedNamespaceDeclarations);
                    }
                }
                else
                {
                    hasStmt = true;
                }
            }
        }

        private BlockStmt EmptyNamespaceBody(Span position)
        {
            return (BlockStmt)_astFactory.SimpleBlock(new Span(position.End, 0), EmptyArray<Statement>.Instance);
        }

        private SimpleUse AddAlias(CompleteAlias alias)
        {
            return AddAlias(new ContextAlias(
                CombineSpans(alias.QualifiedName.Span, alias.Name.Span),
                alias.QualifiedName, alias.Name,
                _contextType
            ));
        }

        private SimpleUse AddAlias(ContextAlias alias)
        {
            var aliasName = alias.Name.HasValue
                ? alias.Name
                : new NameRef(Span.Invalid, alias.QualifiedName.QualifiedName.Name);

            bool added = false;
            switch (alias.Kind)
            {
                case AliasKind.Type:
                    added = namingContext.AddAlias(aliasName, alias.QualifiedName);
                    break;
                case AliasKind.Function:
                    added = namingContext.AddFunctionAlias(aliasName, alias.QualifiedName);
                    break;
                case AliasKind.Constant:
                    added = namingContext.AddConstantAlias(aliasName, alias.QualifiedName);
                    break;
            }
            if (!added)
            {
                this.ErrorSink.Error(aliasName.Span.IsValid ? aliasName.Span : alias.QualifiedName.Span, FatalErrors.AliasAlreadyInUse,
                    alias.QualifiedName.QualifiedName.ToString(), aliasName.Name.ToString());
            }
            return new SimpleUse(alias.Span, alias.Name.Span, alias.QualifiedName.Span, new Alias(aliasName, alias.Kind), alias.QualifiedName);
        }

        static Name[] SelectNames(List<string> prefix)
        {
            Debug.Assert(prefix != null);

            if (prefix.Count == 0)
            {
                return EmptyArray<Name>.Instance;
            }

            var name = new Name[prefix.Count];

            for (int i = 0; i < prefix.Count; i++)
            {
                name[i] = new Name(prefix[i]);
            }

            return name;
        }

        private GroupUse AddAliases(Span span, List<string> prefix, Span prefixSpan, IList<CompleteAlias> aliases, bool isFullyQualified)
        {
            var uses = ListObjectPool<SimpleUse>.Allocate();
            var prefixNamespaces = SelectNames(prefix);

            for (int i = 0; i < aliases.Count; i++)
            {
                var alias = aliases[i];
                if (alias == null)
                {
                    // trailing comma
                    continue;
                }

                var namespaces = ArrayUtils.Concat(prefixNamespaces, alias.QualifiedName.QualifiedName.Namespaces);
                uses.Add(
                    AddAlias(new CompleteAlias(
                        new QualifiedNameRef(alias.QualifiedName.Span, alias.QualifiedName.QualifiedName.Name, namespaces),
                        alias.Name
                    ))
                );
            }

            return new GroupUse(
                span,
                new QualifiedNameRef(prefixSpan, Name.EmptyBaseName, prefixNamespaces, isFullyQualified),
                ListObjectPool<SimpleUse>.GetArrayAndFree(uses)
            );
        }

        private GroupUse AddAliases(Span span, List<string> prefix, Span prefixSpan, IList<ContextAlias> aliases, bool isFullyQualified)
        {
            var uses = ListObjectPool<SimpleUse>.Allocate();
            var prefixNamespaces = SelectNames(prefix);

            for (int i = 0; i < aliases.Count; i++)
            {
                var alias = aliases[i];
                if (alias == null)
                {
                    // trailing comma
                    continue;
                }

                var namespaces = ArrayUtils.Concat(prefixNamespaces, alias.QualifiedName.QualifiedName.Namespaces);
                uses.Add(
                    AddAlias(new ContextAlias(
                        alias.Span,
                        new QualifiedNameRef(alias.QualifiedName.Span, alias.QualifiedName.QualifiedName.Name, namespaces),
                        alias.Name,
                        alias.Kind
                    ))
                );
            }

            return new GroupUse(
                span,
                new QualifiedNameRef(prefixSpan, Name.EmptyBaseName, prefixNamespaces, isFullyQualified),
                ListObjectPool<SimpleUse>.GetArrayAndFree(uses)
            );
        }

        void PushClassContext(string name, TypeRef baseType, PhpMemberAttributes attrs)
        {
            ClassContexts.Push(new ClassContext()
            {
                Name = new QualifiedName(new Name(name), namingContext.CurrentNamespace.HasValue ? namingContext.CurrentNamespace.Value.Namespaces : Name.EmptyNames),
                Base = baseType,
                Attributes = attrs,
            });
        }

        void PushAnonymousClassContext(TypeRef baseType)
        {
            ClassContexts.Push(new ClassContext()
            {
                Name = new QualifiedName(Name.AnonymousClassName),
                Base = baseType,
                Attributes = PhpMemberAttributes.None,
            });
        }

        void PopClassContext()
        {
            ClassContexts.Pop();
        }

        /// <summary>Allocate pooled instance of <see cref="List{T}"/>.</summary>
        /// <remarks>Responsibility of caller to return the instance to the pool.</remarks>
        static List<T> NewList<T>() => ListObjectPool<T>.Allocate();

        static List<T> NewList<T>(T item1)
        {
            var list = ListObjectPool<T>.Allocate();
            list.Add(item1);
            return list;
        }

        static List<T> NewList<T>(T item1, T item2)
        {
            var list = ListObjectPool<T>.Allocate();
            list.Add(item1);
            list.Add(item2);
            return list;
        }

        /// <summary>Returns the instance to the pool.</summary>
        static TTarget[] GetArrayAndFree<TSource, TTarget>(List<TSource> list)
        {
            var array = list.CastToArray<TTarget>();
            ListObjectPool<TSource>.Free(list);
            return array;
        }

        static T[] GetArrayAndFree<T>(List<T> list) => GetArrayAndFree<T, T>(list);

        static T[] GetArrayAndFreeList<T>(IList<T> list)
        {
            T[] arr = list.AsArray();

            if (list is List<T> obj)
            {
                FreeList(obj);
            }

            return arr;
        }

        static void FreeList<T>(List<T> list) => ListObjectPool<T>.Free(list);

        /// <summary>Alias to <see cref="GetArrayAndFree{LangElement, Statement}"/></summary>
        static Statement[] FreeStatements(List<LangElement> list) => GetArrayAndFree<LangElement, Statement>(list);

        protected static TElement FinalizeAttributes<TElement>(TElement node, List<LangElement> attributes) where TElement : IPropertyCollection
        {
            if (attributes != null && attributes.Count != 0)
            {
                node.SetAttributes(
                    GetArrayAndFree<LangElement, IAttributeGroup>(attributes)
                );
            }

            return node;
        }

        private static List<T> AddNotNull<T>(List<T> list, T item) => item != null ? Add(list, item) : list;

        private static List<T> Add<T>(List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        /// <summary>Alias to <see cref="Add"/></summary>
        private static List<T> AddToList<T>(List<T> list, T item) => Add(list, item);

        private static IList<T> AddToList<T>(IList<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        private static ContextAlias JoinTuples(Span span, CompleteAlias alias, AliasKind kind)
        {
            return new ContextAlias(span, alias.QualifiedName, alias.Name, kind);
        }

        IfStmt CreateIfStatement(List<IfStatement> ifStatements, IfStatement elseStatement)
        {
            ifStatements.Add(elseStatement);
            return CreateIfStatement(ifStatements);
        }

        IfStmt CreateIfStatement(List<IfStatement> ifStatements)
        {
            Debug.Assert(ifStatements != null);
            Debug.Assert(ifStatements.Count != 0);

            ConditionalStmt conditions = null;

            // create list of branches
            for (int i = ifStatements.Count - 1; i >= 0; i--)
            {
                var block = ifStatements[i];
                if (block.Body is List<LangElement> stmts) // colon block
                {
                    Debug.Assert(block.ClosingToken != 0);
                    Debug.Assert(block.Span.IsValid);
                    // List<LangElement> -> FinalizeBlock(@3, @5, $4, Tokens.T_ENDIF)
                    block = block.With(FinalizeBlock(block.Span, stmts, block.ClosingToken));
                }

                conditions = new ConditionalStmt(
                    block.Span,
                    (Expression)block.Condition,
                    block.ConditionSpan,
                    (Statement)block.Body,
                    conditions
                );
            }

            FreeList(ifStatements);

            Debug.Assert(conditions != null);
            return (IfStmt)_astFactory.If(conditions);
        }

        static List<IfStatement> FinishColonIfStatement(List<IfStatement> condList, Span end, Tokens token)
        {
            var block = condList.Last();

            condList[condList.Count - 1] = block
                .With(token) // update closing token
                .With(Span.FromBounds(block.Span.Start, end.Start)) // update block span
                ;

            //
            return condList;
        }

        private LangElement CreateProperty(Span span, LangElement objectExpr, object name)
        {
            var parent = VerifyMemberOf(objectExpr);

            if (name is string namestr)
            {
                return _astFactory.Variable(span, namestr, parent, parent == null);
            }
            else
            {
                Debug.Assert(name is LangElement);
                return _astFactory.Variable(span, (LangElement)name, parent);
            }
        }

        private LangElement CreateStaticProperty(Span span, TypeRef objectName, object name)
        {
            if (name is DirectVarUse)
                return _astFactory.Variable(span, ((DirectVarUse)name).VarName.Value, objectName);
            else
                return _astFactory.Variable(span, ((IndirectVarUse)name).VarNameEx, objectName);
        }

        private LangElement CreateStaticProperty(Span span, LangElement objectExpr, Span objectNamePos, object name) =>
            CreateStaticProperty(span, _astFactory.TypeReference(objectNamePos, objectExpr), name);

        private static IList<T> AddTrailingComma<T>(IList<T> list, bool addComma) where T : class
        {
            if (addComma)
            {
                list.Add(null);
            }
            return list;
        }

        private static IList<ActualParam> AddTrailingComma(IList<ActualParam> list, bool addComma)
        {
            if (addComma)
            {
                list.Add(default(ActualParam));
            }

            return list;
        }

        static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP56PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>(2) {
            { QualifiedName.Array, PrimitiveTypeRef.PrimitiveType.array },
            { QualifiedName.Callable, PrimitiveTypeRef.PrimitiveType.callable },
        };
        static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP70PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>(4) {
            { QualifiedName.Int, PrimitiveTypeRef.PrimitiveType.@int },
            { QualifiedName.Float, PrimitiveTypeRef.PrimitiveType.@float },
            { QualifiedName.String, PrimitiveTypeRef.PrimitiveType.@string },
            { QualifiedName.Bool, PrimitiveTypeRef.PrimitiveType.@bool },
        };
        static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP71PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>(2) {
            { QualifiedName.Void, PrimitiveTypeRef.PrimitiveType.@void },
            { QualifiedName.Iterable, PrimitiveTypeRef.PrimitiveType.iterable },
        };
        static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP72PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>(1) {
            { QualifiedName.Object, PrimitiveTypeRef.PrimitiveType.@object },
        };
        static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP80PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>(1) {
            { QualifiedName.Mixed, PrimitiveTypeRef.PrimitiveType.mixed },
        };
        static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP81PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>(1) {
            { QualifiedName.Never, PrimitiveTypeRef.PrimitiveType.never },
        };
        static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP82PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>(3) {
            { QualifiedName.True, PrimitiveTypeRef.PrimitiveType.@true },
            { QualifiedName.False, PrimitiveTypeRef.PrimitiveType.@false },
            { QualifiedName.Null, PrimitiveTypeRef.PrimitiveType.@null },
        };

        /// <summary>
        /// Gets map of primitive types for current language features.
        /// </summary>
        Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PrimitiveTypes
        {
            get
            {
                if (_lazyPrimitiveTypes == null)
                {
                    // constructs map of primitive types for current language features:

                    var dict = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>(14);

                    if (_languageFeatures.HasFeature(LanguageFeatures.Php82Set))
                    {
                        dict.Add(PHP82PrimitiveTypes);
                    }
                    if (_languageFeatures.HasFeature(LanguageFeatures.Php81Set))
                    {
                        dict.Add(PHP81PrimitiveTypes);
                    }
                    if (_languageFeatures.HasFeature(LanguageFeatures.Php80Set))
                    {
                        dict.Add(PHP80PrimitiveTypes);
                    }
                    if (_languageFeatures.HasFeature(LanguageFeatures.Php72Set))
                    {
                        dict.Add(PHP72PrimitiveTypes);
                    }
                    if (_languageFeatures.HasFeature(LanguageFeatures.Php71Set))
                    {
                        dict.Add(PHP71PrimitiveTypes);
                    }
                    if (_languageFeatures.HasFeature(LanguageFeatures.Php70Set))
                    {
                        dict.Add(PHP70PrimitiveTypes);
                    }
                    if (_languageFeatures.HasFeature(LanguageFeatures.Php56Set))
                    {
                        dict.Add(PHP56PrimitiveTypes);
                    }

                    //
                    Interlocked.CompareExchange(ref _lazyPrimitiveTypes, dict, null);
                }

                return _lazyPrimitiveTypes;
            }
        }
        Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> _lazyPrimitiveTypes;

        static ReservedTypeRef.ReservedType _reservedTypeStatic => ReservedTypeRef.ReservedType.@static;

        /// <summary>
        /// Creates type reference from a given qualified name.
        /// The given name will be translated using current naming context.
        /// </summary>
        public TypeRef CreateTypeRef(QualifiedNameRef tname, bool allowPrimitiveTypes = false)
        {
            var qname = tname.QualifiedName;
            var span = tname.Span;

            // primitive type name ?
            if (qname.IsSimpleName && !qname.IsFullyQualifiedName)
            {
                if (ReservedTypeRef.ReservedTypes.TryGetValue(qname.Name, out ReservedTypeRef.ReservedType reserved))
                {
                    var reservedRef = _astFactory.ReservedTypeReference(span, reserved);
                    if (IsInClassContext)
                    {
                        var context = ClassContexts.Peek();
                        switch (reserved)
                        {
                            case ReservedTypeRef.ReservedType.parent:
                                if (context.Base != null)
                                {
                                    reservedRef = _astFactory.AliasedTypeReference(span, context.Base.QualifiedName.Value, reservedRef);
                                }
                                else if (context.Attributes.IsTrait())
                                {
                                    // keep unbound
                                    // {parent} refers to actual class where the trait is used
                                }
                                else
                                {
                                    // NOTE: allowed on closures
                                    //this.ErrorSink.Error(span, FatalErrors.ParentAccessedInParentlessClass);
                                }
                                break;

                            case ReservedTypeRef.ReservedType.self:

                                if (context.Attributes.IsTrait())
                                {
                                    // keep unbound
                                    // {self} refers to actual class where the trait is used
                                }
                                else if (context.Name.Name == Name.AnonymousClassName)
                                {
                                    // we don't translate {self} in the context of an anonymous type name
                                    // since the translated name is platform specific and may differ from how it is handled by caller
                                }
                                else
                                {
                                    reservedRef = _astFactory.AliasedTypeReference(span, context.Name, reservedRef);
                                }
                                break;

                            case ReservedTypeRef.ReservedType.@static:
                                // keep unbound
                                break;

                            default:
                                throw new ArgumentException();
                        }
                    }
                    else
                    {
                        // TODO: Error: self|parent|static used outside a class context
                        // NOTE: allowed in global code
                    }

                    return reservedRef;
                }

                if (allowPrimitiveTypes && PrimitiveTypes.TryGetValue(qname, out PrimitiveTypeRef.PrimitiveType primitive))
                {
                    return _astFactory.PrimitiveTypeReference(span, primitive);
                }
            }

            // direct type reference
            return CreateNamedTypeRef(span, tname);
        }

        private TypeRef CreateNamedTypeRef(Span span, QualifiedName tname)
        {
            return TryTranslateAny(tname, out var translated) ?
                _astFactory.AliasedTypeReference(span, translated, _astFactory.TypeReference(span, tname)) :
                _astFactory.TypeReference(span, tname);
        }

        LangElement CreateIncDec(Span span, LangElement variable, bool inc, bool post)
        {
            if (variable is VariableUse)
            {
                return _astFactory.IncrementDecrement(span, variable, inc, post);
            }
            _errors.Error(variable.Span, FatalErrors.CheckVarUseFault);
            return variable;
        }

        static LangElement AdjustNullSafeOperator(LangElement element, Tokens objectOperator)
        {
            if (objectOperator == Tokens.T_NULLSAFE_OBJECT_OPERATOR)
            {
                Debug.Assert(element is VarLikeConstructUse);
                ((VarLikeConstructUse)element).IsNullSafeObjectOperation = true;
            }
            return element;
        }

        /// <summary>
        /// Traverse through the expression (either <see cref="IStringLiteralValue"/> or concat of <see cref="IStringLiteralValue"/>s),
        /// and checks and removes the leading whitespace indentation.
        /// </summary>
        /// <param name="element">Either <see cref="IStringLiteralValue"/> or <see cref="IConcatEx"/> with <see cref="IStringLiteralValue"/>s.</param>
        /// <param name="heredoc">Heredoc information.</param>
        /// <param name="_">Ignored.</param>
        LangElement RemoveHereDocIndentation(LangElement element, Lexer.HereDocTokenValue heredoc, bool _)
        {
            // CONSIDER: do it properly in lexer ...

            var indentation = heredoc.Indentation.AsSpan();

            if (indentation.IsEmpty)
            {
                return element;
            }

            if (!indentation.IsWhiteSpace())
            {
                throw new InvalidOperationException($"heredoc.Indentation is expected to be a whitespace!");
            }

            //
            var expressions = element is ConcatEx concat
                ? concat.Expressions
                : new[] { element }
                ;

            var eol = true;
            var errorreported = false;
            var current_indentation = ReadOnlySpan<char>.Empty;

            for (int i = 0; i < expressions.Length; i++)
            {
                var expr = expressions[i];
                if (expr is IStringLiteralValue str) // {StringLiteral}
                {
                    // if (str.Contains8bitText) // TODO, use str.EnumerateChunks() : byte[]|string

                    var content = str.ToString();
                    var result = StringUtils.GetStringBuilder(content.Length);

                    foreach (var lineSpan in TextUtils.EnumerateLines(content, true))
                    {
                        var line = content.AsSpan(lineSpan);
                        var waseol = eol;

                        eol = TextUtils.EndsWithEol(line);

                        if (waseol)
                        {
                            if (line.StartsWith(indentation, StringComparison.Ordinal))
                            {
                                current_indentation = indentation;
                                result.Append(content, lineSpan.Start + indentation.Length, lineSpan.Length - indentation.Length);
                                continue;
                            }
                            else if (line.IsWhiteSpace())
                            {
                                if (!eol)
                                {
                                    current_indentation = line;
                                }

                                // allowed, empty line
                                // add the line break from the line (ignore ' ' and '\t' from the beginning)
                                var nlspan = lineSpan;
                                while (nlspan.Length > 0 && content[nlspan.Start].IsTabOrSpace())
                                    nlspan = nlspan.Slice(1);

                                result.Append(content, nlspan.Start, nlspan.Length);
                                continue;
                            }
                            else
                            {
                                current_indentation = ReadOnlySpan<char>.Empty;

                                if (!errorreported)
                                {
                                    errorreported = true; // report error just once
                                    _errors.Error(expr.Span, FatalErrors.HeredocIndentError);
                                }
                            }
                        }

                        result.Append(content, lineSpan.Start, lineSpan.Length);
                    }

                    if (eol)
                    {
                        current_indentation = ReadOnlySpan<char>.Empty;
                    }

                    expressions[i] = StringLiteral.Create(expr.Span, StringUtils.ReturnStringBuilder(result));
                }
                else if (expr is VarLikeConstructUse)
                {
                    eol = false;

                    // check current_indentatiom
                    if (!current_indentation.StartsWith(indentation, StringComparison.Ordinal) && !errorreported)
                    {
                        errorreported = true; // report error just once
                        _errors.Error(expr.Span, FatalErrors.HeredocIndentError);
                    }
                }
            }

            //
            return expressions.Length == 1 ? expressions[0] : element;
        }

        #region Aliasing

        /// <summary>
        /// Translates given type reference according to current <see cref="namingContext"/>.
        /// </summary>
        /// <param name="tref"></param>
        /// <returns></returns>
        TypeRef Translate(TypeRef/*!*/tref)
        {
            Debug.Assert(tref != null);

            if (tref is ClassTypeRef) return new ClassTypeRef(tref.Span, TranslateAny(((ClassTypeRef)tref).ClassName));
            if (tref is NullableTypeRef) return new NullableTypeRef(tref.Span, Translate(((NullableTypeRef)tref).TargetType));
            if (tref is MultipleTypeRef mtref) return new MultipleTypeRef(tref.Span, mtref.MultipleTypes.Select(Translate));
            if (tref is GenericTypeRef) throw new NotImplementedException();
            // PrimitiveTypeRef is not translated
            // IndirectTypeRef is not translated
            return tref;
        }

        TranslatedQualifiedName TranslateQNRFunction(QualifiedNameRef nref) => TranslateFallbackQualifiedName(nref, AliasKind.Function);

        private TranslatedQualifiedName TranslateQNRConstant(QualifiedNameRef nref)
        {
            var qname = nref.QualifiedName;
            if (qname.IsSimpleName && (qname == QualifiedName.Null || qname == QualifiedName.True || qname == QualifiedName.False))
            {
                // special exit_scope consts
                qname.IsFullyQualifiedName = true;
                return new TranslatedQualifiedName(qname, nref.Span, qname, null);
            }
            else
            {
                return TranslateFallbackQualifiedName(nref, AliasKind.Constant);
            }
        }

        #endregion

        private TranslatedQualifiedName TranslateFallbackQualifiedName(QualifiedNameRef qname, AliasKind kind)
        {
            if (qname.QualifiedName.IsFullyQualifiedName)
            {
                return new TranslatedQualifiedName(qname.QualifiedName, qname.Span);
            }

            // quick aliasing
            if (qname.QualifiedName.IsSimpleName &&
                this.namingContext.Aliases != null &&
                this.namingContext.Aliases.TryGetValue(new Alias(qname.QualifiedName.Name, kind), out var tmp))
            {
                return new TranslatedQualifiedName(tmp, qname.Span, qname, null);
            }

            //
            var translated = TranslateNamespace(qname, out var translatedQName);
            if (!translatedQName.IsFullyQualifiedName && translatedQName.IsSimpleName && !IsInGlobalNamespace)
            {
                // "\foo"
                var fallbackQName = new QualifiedName(translatedQName.Name) { IsFullyQualifiedName = true };
                // "namespace\foo"
                translatedQName = new QualifiedName(translatedQName.Name, namingContext.CurrentNamespace.Value.Namespaces) { IsFullyQualifiedName = true };
                return new TranslatedQualifiedName(translatedQName, qname.Span, qname, fallbackQName);
            }
            else
            {
                translatedQName.IsFullyQualifiedName = true;  // just ensure
                return new TranslatedQualifiedName(translatedQName, qname.Span, qname, null);
            }
        }

        private bool TranslateNamespace(QualifiedName qname, out QualifiedName translated)
        {
            if (qname.IsFullyQualifiedName || qname.IsSimpleName)
            {
                translated = qname;
                return false;
            }
            else
            {
                return TryTranslateAlias(qname, out translated);
            }
        }

        private bool TryTranslateAlias(QualifiedName qname, out QualifiedName translated)
        {
            Debug.Assert(!qname.IsFullyQualifiedName);
            Debug.Assert(!qname.IsPrimitiveTypeName);
            Debug.Assert(!qname.IsReservedClassName);

            // do not use current namespace, if there are imported namespace ... will be resolved later
            return QualifiedName.TryTranslateAlias(qname, AliasKind.Type, this.namingContext.Aliases,
                (IsInGlobalNamespace) ? (QualifiedName?)null : namingContext.CurrentNamespace.Value, out translated);
        }

        private QualifiedName TranslateAny(QualifiedName qname)
        {
            TryTranslateAny(qname, out var name);
            return name;
        }

        private bool TryTranslateAny(QualifiedName qname, out QualifiedName translated)
        {
            if (qname.IsFullyQualifiedName ||   // already translated
                qname.IsReservedClassName ||    // special names (self, parent, static)
                qname.IsPrimitiveTypeName)      // primitive type name
            {
                translated = qname;
                return false;
            }

            // return the alias if found:
            return TryTranslateAlias(qname, out translated);
        }

        private bool IsInGlobalNamespace => !namingContext.CurrentNamespace.HasValue || namingContext.CurrentNamespace.Value.Namespaces.Length == 0;

        /// <summary>Combine spans if they are valid.</summary>
        private static Span CombineSpans(Span a, Span b) => a.IsValid ? (b.IsValid ? Span.Combine(a, b) : a) : b;

        /// <summary>Combine spans if they are valid.</summary>
        private static Span CombineSpans(Span a, Span b, Span c) => CombineSpans(a, CombineSpans(b, c));

        private static Span CombineSpans(Span a, Span b, Span c, Span d, Span e) => CombineSpans(a, CombineSpans(b, CombineSpans(c, CombineSpans(d, e))));

        private static Span CombineSpans(params Span[] spans)
        {
            int min = -1, max = -1;
            for (int i = 0; i < spans.Length; i++)
            {
                var span = spans[i];
                if (span.IsValid)
                {
                    min = (min < 0) | (min > span.Start) ? span.Start : min;
                    max = (max < 0) | (max < span.End) ? span.End : max;
                }
            }

            Debug.Assert(min <= max);

            return min >= 0 ? Span.FromBounds(min, max) : Span.Invalid;
        }

        private long AddModifier(long a, long b, Span bSpan) => (long)AddModifier((PhpMemberAttributes)a, (PhpMemberAttributes)b, bSpan);

        private PhpMemberAttributes AddModifier(PhpMemberAttributes a, PhpMemberAttributes b, Span bSpan)
        {
            // check there is no duplicit modifier
            if ((a & b) != 0)
            {
                _errors.Error(bSpan, Errors.Errors.MultipleVisibilityModifiers);
            }

            return a | b;
        }

        /// <summary>
        /// Associates given <paramref name="target"/> referring to instance of <see cref="PHPDocBlock"/> to a target which must be an instance of <see cref="IPropertyCollection"/>.
        /// </summary>
        /// <param name="target"><see cref="IPropertyCollection"/> instance. Must not be <c>null</c>.</param>
        void SetDoc(object target)
        {
            Debug.Assert(target != null);
            Debug.Assert(target is IPropertyCollection);
            _lexer.DocCommentList.Annotate((LangElement)target);
        }

        /// <summary>
        /// Associates given <paramref name="target"/> referring to instance of <see cref="PHPDocBlock"/> to a target which must be an instance of <see cref="IPropertyCollection"/>.
        /// </summary>
        /// <param name="target"><see cref="IPropertyCollection"/> instance. Must not be <c>null</c>.</param>
        void SetDocSpan(object target)
        {
            Debug.Assert(target != null);
            Debug.Assert(target is IPropertyCollection);
            _lexer.DocCommentList.AnnotateSpan((LangElement)target);
        }

        /// <summary>
        /// Associates given <paramref name="target"/> refering to instance of <see cref="PHPDocBlock"/> to a target which must be an instance of <see cref="IPropertyCollection"/>.
        /// </summary>
        /// <param name="target"><see cref="IPropertyCollection"/> instance. Must not be <c>null</c>.</param>
        void SetMemberDoc(object target)
        {
            Debug.Assert(target != null);
            Debug.Assert(target is IPropertyCollection);
            _lexer.DocCommentList.AnnotateMember((LangElement)target);
        }

        /// <summary>
        /// Unassigned PHPDoc comments are merged to the statements as <see cref="PHPDocStmt"/>.
        /// </summary>
        /// <param name="span">Statements span.</param>
        /// <param name="statements">Statements in block.</param>
        List<LangElement> MergeDoc(Span span, List<LangElement> statements)
        {
            Debug.Assert(statements.All(s => s != null));
            _lexer.DocCommentList.Merge(span, statements, _astFactory);
            return statements;
        }

        /// <summary>
        /// Assocates enum type with its backing type.
        /// </summary>
        /// <param name="target"><see cref="TypeDecl"/> representing enum.</param>
        /// <param name="backingType">Enum's backing type (<see cref="TypeRef"/>), or null.</param>
        static void SetEnumBackingType(LangElement target, LangElement backingType)
        {
            Debug.Assert(target is TypeDecl);
            Debug.Assert(((TypeDecl)target).MemberAttributes.IsEnum());

            if (backingType is TypeRef tref)
            {
                ((TypeDecl)target).SetEnumBackingType(tref);
            }
        }

        /// <remarks>Returns <paramref name="statements"/> to the pool.</remarks>
        private LangElement FinalizeBlock(Span span, List<LangElement> statements)
        {
            MergeDoc(span, statements);

            return _astFactory.Block(span, FreeStatements(statements));
        }

        /// <remarks>Returns <paramref name="statements"/> to the pool.</remarks>
        private LangElement FinalizeBlock(Span span, List<LangElement> statements, Tokens endToken)
        {
            MergeDoc(span, statements);

            return _astFactory.ColonBlock(span, FreeStatements(statements), endToken);
        }

        private LangElement FinalizeBlock(Span span, Span endSpan, List<LangElement> statements, Tokens endToken)
        {
            return FinalizeBlock(Span.FromBounds(span.Start, endSpan.Start), statements, endToken);
        }

        static CallSignature FinalizeCallSignature(Span span, IList<ActualParam> parameters)
        {
            Debug.Assert(parameters is List<ActualParam> || parameters is ActualParam[]);

            var arr = parameters is List<ActualParam> list
                ? GetArrayAndFree(list)
                : parameters.AsArray();

            return new CallSignature(arr, span);
        }

        /// <summary>
        /// Creates a <see cref="BlockStmt"/> statement from a list of statements in a case or default.
        /// Unassigned PHPDoc comments are merged to the statements as <see cref="PHPDocStmt"/>, PHPDoc comments after the last statement are ignored.
        /// </summary>
        /// <param name="separatorSpan">Span of the separator ':'.</param>
        /// <param name="statements">List of statements in the block.</param>
        /// <returns>Complete block statement.</returns>
        /// <remarks>Returns <paramref name="statements"/> to the pool.</remarks>
        BlockStmt FinalizeCaseBlock(Span separatorSpan, List<LangElement> statements)
        {
            Span bodySpan;

            if (statements.Count == 0)
            {
                bodySpan = separatorSpan;
            }
            else
            {
                bodySpan = Span.Combine(separatorSpan, statements.Last().Span);
                MergeDoc(bodySpan, statements);
            }

            return (BlockStmt)_astFactory.Block(bodySpan, FreeStatements(statements));
        }

        /// <summary>
        /// Merges current namespace with the name. 
        /// Returns a qualified name based only on the suffix, if the namespace is <c>null</c>.
        /// </summary>
        /// <param name="currentNamespace">Current namespace name, can be <c>null</c>.</param>
        /// <param name="suffix">Rest of the name, after current namespace.</param>
        /// <returns>Complete qualified name</returns>
        static QualifiedName MergeWithCurrentNamespace(QualifiedName? currentNamespace, List<string> suffix)
        {
            if (!currentNamespace.HasValue)
                return new QualifiedName(suffix, true, true);

            QualifiedName currentNS = currentNamespace.Value;
            Name[] namespaces = new Name[currentNS.Namespaces.Length + suffix.Count - 1];
            currentNS.Namespaces.CopyTo(namespaces, 0);
            for (int i = currentNS.Namespaces.Length; i < namespaces.Length; i++)
            {
                namespaces[i] = new Name(suffix[i - currentNS.Namespaces.Length]);
            }

            return new QualifiedName(new Name(suffix.Last()), namespaces, true);
        }

        Expression VerifyMemberOf(LangElement memberOf)
        {
            //if (memberOf != null && !(memberOf is VarLikeConstructUse))
            //{
            //    _errors.Error(memberOf.Span, FatalErrors.CheckVarUseFault);
            //    return null;
            //}
            Debug.Assert(memberOf is Expression);
            return (Expression)memberOf;
        }

        INamedTypeRef[] FinalizeNamedTypeRefArray(IList<TypeRef> types)
        {
            INamedTypeRef[] arr;

            if (types.Count != 0)
            {
                arr = new INamedTypeRef[types.Count];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = ConvertToNamedTypeRef(types[i]);
                }
            }
            else
            {
                arr = EmptyArray<INamedTypeRef>.Instance;
            }

            //
            if (types is List<TypeRef> list)
            {
                FreeList(list);
            }

            return arr;
        }

        /// <summary>
        /// Ensure that hte <see cref="TypeRef"/> implements <see cref="INamedTypeRef"/> without crashing.
        /// If the <paramref name="type"/> already implements <see cref="INamedTypeRef"/> then it is just returned.
        /// If the <paramref name="type"/> does not implement <see cref="INamedTypeRef"/> then a copy is created with the same name that does and an error is reported.
        /// </summary>
        /// <param name="type">Original <see cref="TypeRef"/>.</param>
        /// <returns>Representation of the <paramref name="type"/> that implements <see cref="INamedTypeRef"/>.</returns>
        INamedTypeRef ConvertToNamedTypeRef(TypeRef type)
        {
            if (type == null) return null;

            if (type is INamedTypeRef named) return named;

            var name = type.QualifiedName.HasValue ? type.QualifiedName.Value : new QualifiedName();
            _errors.Error(type.Span, Errors.Errors.NonClassExtended, name.ToString());
            return (INamedTypeRef)CreateNamedTypeRef(type.Span, name);
        }

        INamedTypeRef[] ConvertToNamedTypeRef(IList<TypeRef> types)
        {
            if (types == null || types.Count == 0)
            {
                return EmptyArray<INamedTypeRef>.Instance;
            }

            var result = new INamedTypeRef[types.Count];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ConvertToNamedTypeRef(types[i]);
            }

            return result;
        }

        /// <summary>
        /// Error recovery implementation.
        /// Logic is provided by the caler as an implementation of the <see cref="IErrorRecovery"/> interface.
        /// The error recovery object gets an instance of <see cref="ILexerState"/> that represents current lexer state that the <see cref="IErrorRecovery"/> can modify.
        /// </summary>
        /// <param name="token">Current token.</param>
        /// <param name="state">Current parser state.</param>
        /// <returns><c>True</c> if the error recovery succeeded, <c>False</c> otherwise.</returns>
        protected override bool ErrorRecovery(int token, int state)
        {
            if (_recoveryCount == 0)
            {
                ReportError();
            }

            if (_recoveryCount++ < _recoveryLimit)
            {
                var next = new CompleteToken((Tokens)token, _lexer.TokenValue, _lexer.TokenPosition, _lexer.TokenText);
                LexerState lexerState = new LexerState(state, states[state].parser_table, _lexer.PreviousToken, next, _lexer);

                bool recovering = _errorRecovery.TryRecover(lexerState);
                if (recovering)
                {
                    _lexer.AddNextTokens(lexerState.TokensBuffer, lexerState.PreviousToken);
                    SetNextState(0, lexerState.CurrentState);
                    return true;
                }
            }
            return false;
        }
    }
}
