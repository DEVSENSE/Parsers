﻿// Copyright(c) DEVSENSE s.r.o.
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

namespace Devsense.PHP.Syntax
{
    public partial class Parser
    {
        CompliantLexer _lexer;
        INodesFactory<LangElement, Span> _astFactory;
        IErrorSink<Span> _errors;
        Scope _currentScope;
        bool isConditional => !_currentScope.IsGlobal;
        NamespaceDecl _currentNamespace = null;
        readonly Stack<NamingContext> _context = new Stack<NamingContext>();
        NamingContext namingContext => _context.Peek();
        AliasKind _contextType = AliasKind.Type;
        LanguageFeatures _languageFeatures;

        /// <summary>
        /// The root of AST.
        /// </summary>
        private LangElement _astRoot;

        /// <summary>
        /// Gets parser error sink. Cannot be <c>null</c>.
        /// </summary>
        public IErrorSink<Span> ErrorSink => _errors;

        LangElement NullLangElement => null;

        Stack<ClassContext> ClassContexts = new Stack<ClassContext>();

        private struct ClassContext
        {
            public QualifiedName Name;
            public TypeRef Base;
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
                int positionShift = 0)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            if (astFactory == null)
                throw new ArgumentNullException(nameof(astFactory));

            // initialization:
            _languageFeatures = language;
            _lexer = new CompliantLexer(lexer);
            _astFactory = astFactory;
            _errors = errors ?? new EmptyErrorSink<Span>();
            //InitializeFields();

            _currentScope = new Scope(0);

            base.Scanner = _lexer;
            bool accept = base.Parse();

            LangElement result = _astRoot;

            // clean and let GC collect unused AST and other stuff:
            //ClearFields();

            return result;
        }

        void SetNamingContext(List<string> ns)
        {
            var qname = (ns != null && ns.Count != 0)
                ? (QualifiedName?)new QualifiedName(ns, false, true)
                : null;

            _context.Push(new NamingContext(qname));
        }

        void ResetNamingContext()
        {
            _context.Pop();
        }

        void AssignNamingContext()
        {
            if (_currentNamespace != null)
            {
                Debug.Assert(_context.Count == 2);
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
                var ns = stmt as NamespaceDecl;
                if (ns != null)
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
                                ns.Body = (BlockStmt)_astFactory.SimpleBlock(new Span(ns.Span.End, 0), EmptyArray<Statement>.Instance);
                            }
                        }
                    }
                    else
                    {
                        hasNsBracket = true;
                    }

                    if (hasNsSimple && hasNsBracket)
                    {
                        this.ErrorSink.Error(ns.QualifiedName.Span, FatalErrors.MixedNamespacedeclarations);
                    }
                }
                else
                {
                    hasStmt = true;
                }
            }
        }

        private SimpleUse AddAlias(Tuple<QualifiedNameRef, NameRef> alias)
        {
            return AddAlias(alias, _contextType);
        }

        private SimpleUse AddAlias(Tuple<QualifiedNameRef, NameRef> alias, AliasKind contextType)
        {
            var aliasName = alias.Item2.HasValue
                ? alias.Item2
                : new NameRef(Span.Invalid, alias.Item1.QualifiedName.Name);

            bool added = false;
            switch (contextType)
            {
                case AliasKind.Type:
                    added = namingContext.AddAlias(aliasName, alias.Item1);
                    break;
                case AliasKind.Function:
                    added = namingContext.AddFunctionAlias(aliasName, alias.Item1);
                    break;
                case AliasKind.Constant:
                    added = namingContext.AddConstantAlias(aliasName, alias.Item1);
                    break;
            }
            if (!added)
            {
                this.ErrorSink.Error(aliasName.Span.IsValid ? aliasName.Span : alias.Item1.Span, FatalErrors.AliasAlreadyInUse,
                    alias.Item1.QualifiedName.ToString(), aliasName.Name.ToString());
            }
            return new SimpleUse(aliasName.Span, alias.Item1.Span, new Alias(aliasName, contextType), alias.Item1);
        }

        private GroupUse AddAliases(Span span, List<string> prefix, Span prefixSpan, List<Tuple<QualifiedNameRef, NameRef>> aliases)
        {
            List<SimpleUse> uses = new List<SimpleUse>();
            var prefixNamespaces = prefix.Select(p => new Name(p));
            foreach (var alias in aliases)
            {
                Name[] namespaces = prefixNamespaces.Concat(alias.Item1.QualifiedName.Namespaces).ToArray();
                uses.Add(AddAlias(new Tuple<QualifiedNameRef, NameRef>(
                    new QualifiedNameRef(alias.Item1.Span, alias.Item1.QualifiedName.Name, namespaces), alias.Item2)));
            }
            return new GroupUse(span, new QualifiedNameRef(prefixSpan, Name.EmptyBaseName, prefixNamespaces.ToArray()), uses);
        }

        private GroupUse AddAliases(Span span, List<string> prefix, Span prefixSpan, List<Tuple<QualifiedNameRef, NameRef, AliasKind>> aliases)
        {
            List<SimpleUse> uses = new List<SimpleUse>();
            var prefixNamespaces = prefix.Select(p => new Name(p));
            foreach (var alias in aliases)
            {
                Name[] namespaces = prefixNamespaces.Concat(alias.Item1.QualifiedName.Namespaces).ToArray();
                uses.Add(AddAlias(new Tuple<QualifiedNameRef, NameRef>(
                    new QualifiedNameRef(alias.Item1.Span, alias.Item1.QualifiedName.Name, namespaces), alias.Item2), alias.Item3));
            }
            return new GroupUse(span, new QualifiedNameRef(prefixSpan, Name.EmptyBaseName, prefixNamespaces.ToArray()), uses);
        }

        void PushClassContext(string name, TypeRef type)
        {
            ClassContexts.Push(new ClassContext()
            {
                Name = new QualifiedName(new Name(name), namingContext.CurrentNamespace.HasValue ? namingContext.CurrentNamespace.Value.Namespaces : Name.EmptyNames),
                Base = type
            });
        }

        void PopClassContext()
        {
            ClassContexts.Pop();
        }

        private List<T> AddToList<T>(List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        private Tuple<T1, T2, T3> JoinTuples<T1, T2, T3>(Tuple<T1, T2> first, T3 second)
        {
            return new Tuple<T1, T2, T3>(first.Item1, first.Item2, second);
        }

        private LangElement StatementsToBlock(Span span, List<LangElement> statements, Tokens endToken)
        {
            _lexer.DocBlockList.Merge(span, statements, _astFactory);
            return _astFactory.ColonBlock(span, statements, endToken);
        }

        private LangElement StatementsToBlock(Span span, Span endSpan, object statements, Tokens endToken)
        {
            Debug.Assert(statements is List<LangElement>);
            var statemenList = (List<LangElement>)statements;
            return StatementsToBlock(Span.FromBounds(span.Start, endSpan.Start), statemenList, endToken);
        }

        void RebuildLast(object condList, Span end, Tokens token)
        {
            var ifList = ((List<Tuple<Span, LangElement, LangElement>>)condList);
            var block = ifList.Last();
            ((BlockStmt)block.Item3).ExtendSpan(Span.FromBounds(block.Item3.Span.Start, end.Start));
            ifList.Remove(block);
            ifList.Add(new Tuple<Span, LangElement, LangElement>(Span.FromBounds(block.Item1.Start, end.Start), block.Item2, block.Item3));
        }

        private LangElement CreateProperty(Span span, LangElement objectExpr, object name)
        {
            if (name is Name)
                return _astFactory.Variable(span, ((Name)name).Value, objectExpr);
            else
                return _astFactory.Variable(span, (LangElement)name, objectExpr);
        }

        private LangElement CreateStaticProperty(Span span, TypeRef objectName, Span objectNamePos, object name)
        {
            if (name is DirectVarUse)
                return _astFactory.Variable(span, ((DirectVarUse)name).VarName.Value, objectName);
            else
                return _astFactory.Variable(span, ((IndirectVarUse)name).VarNameEx, objectName);
        }

        private LangElement CreateStaticProperty(Span span, LangElement objectExpr, Span objectNamePos, object name) =>
            CreateStaticProperty(span, _astFactory.TypeReference(objectNamePos, objectExpr), objectNamePos, name);

        private List<T> RightTrimList<T>(List<T> list)
        {
            T elem;
            if (list.Count > 0 && (elem = list.Last()) == null)
                list.Remove(elem);
            return list;
        }

        private Span CombineSpans(params Span[] spans)
        {
            var validSpans = spans.Where(s => s.IsValid);
            return Span.FromBounds(validSpans.Min(s => s.Start), validSpans.Max(s => s.End));
        }

        public static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP56PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>() {
            { QualifiedName.Array, PrimitiveTypeRef.PrimitiveType.array },
            { QualifiedName.Callable, PrimitiveTypeRef.PrimitiveType.callable }
        };
        public static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP70PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>() {
            { QualifiedName.Int, PrimitiveTypeRef.PrimitiveType.@int },
            { QualifiedName.Float, PrimitiveTypeRef.PrimitiveType.@float },
            { QualifiedName.String, PrimitiveTypeRef.PrimitiveType.@string },
            { QualifiedName.Bool, PrimitiveTypeRef.PrimitiveType.@bool }
        };
        public static readonly Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType> PHP71PrimitiveTypes = new Dictionary<QualifiedName, PrimitiveTypeRef.PrimitiveType>() {
            { QualifiedName.Void, PrimitiveTypeRef.PrimitiveType.@void },
            { QualifiedName.Iterable, PrimitiveTypeRef.PrimitiveType.iterable }
        };

        ReservedTypeRef.ReservedType _reservedTypeStatic => ReservedTypeRef.ReservedType.@static;

        TypeRef CreateTypeRef(Span span, QualifiedNameRef tname)
        {
            var qname = tname.QualifiedName;
            // primitive type name ?
            if (qname.IsSimpleName)
            {
                ReservedTypeRef.ReservedType reserved;
                PrimitiveTypeRef.PrimitiveType primitive;
                if ((_languageFeatures.HasFeature(LanguageFeatures.Php71Set)) && PHP71PrimitiveTypes.TryGetValue(qname, out primitive))
                {
                    return _astFactory.PrimitiveTypeReference(span, primitive);
                }
                if ((_languageFeatures.HasFeature(LanguageFeatures.Php70Set)) && PHP70PrimitiveTypes.TryGetValue(qname, out primitive))
                {
                    return _astFactory.PrimitiveTypeReference(span, primitive);
                }
                if ((_languageFeatures.HasFeature(LanguageFeatures.Php56Set)) && PHP56PrimitiveTypes.TryGetValue(qname, out primitive))
                {
                    return _astFactory.PrimitiveTypeReference(span, primitive);
                }
                if (ReservedTypeRef.ReservedTypes.TryGetValue(qname.Name, out reserved))
                {
                    var reservedRef = _astFactory.ReservedTypeReference(span, reserved);
                    if (ClassContexts.Count != 0)
                    {
                        var context = ClassContexts.Peek();
                        switch (reserved)
                        {
                            case ReservedTypeRef.ReservedType.parent:
                                if (context.Base == null)
                                {
                                    this.ErrorSink.Error(span, FatalErrors.ParentAccessedInParentlessClass);
                                    return reservedRef;
                                }
                                return _astFactory.AliasedTypeReference(span, context.Base.QualifiedName.Value, reservedRef);
                            case ReservedTypeRef.ReservedType.self:
                                return _astFactory.AliasedTypeReference(span, context.Name, reservedRef);
                            default:
                                throw new ArgumentException();
                        }
                    }
                    return reservedRef;
                }
            }

            // direct type reference
            QualifiedName translated;
            return (TryTranslateAny(tname, out translated)) ?
                _astFactory.AliasedTypeReference(span, translated, _astFactory.TypeReference(span, tname)) :
                _astFactory.TypeReference(span, tname);
        }

        List<TypeRef> TypeRefListFromQNRList(List<QualifiedNameRef> nrefList)
        {
            TypeRef[] types = new TypeRef[nrefList.Count];
            for (int i = 0; i < types.Length; i++)
                types[i] = CreateTypeRef(nrefList[i].Span, nrefList[i]);
            return types.ToList();
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

        VariableUse CreateIssetVar(LangElement node)
        {
            if (node is VariableUse)
            {
                return node as VariableUse;
            }
            _errors.Error(node.Span, FatalErrors.CheckVarUseFault);
            return null;
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
            if (tref is MultipleTypeRef) return new MultipleTypeRef(tref.Span, ((MultipleTypeRef)tref).MultipleTypes.Select(Translate).ToList());
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
            else return TranslateFallbackQualifiedName(nref, AliasKind.Constant);
        }

        #endregion

        private TranslatedQualifiedName TranslateFallbackQualifiedName(QualifiedNameRef qname, AliasKind kind)
        {
            // aliasing
            QualifiedName tmp;
            if (qname.QualifiedName.IsSimpleName && this.namingContext.Aliases != null &&
                this.namingContext.Aliases.TryGetValue(new Alias(qname.QualifiedName.Name, kind), out tmp))
            {
                return new TranslatedQualifiedName(tmp, qname.Span, qname, null);
            }

            //
            QualifiedName translatedQName;
            bool translated = TranslateNamespace(qname, out translatedQName);
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
            QualifiedName name;
            TryTranslateAny(qname, out name);
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

        /// <summary>
        /// Combine spans if they are valid.
        /// </summary>
        /// <param name="a">First span.</param>
        /// <param name="b">Second span.</param>
        /// <returns>Combined span.</returns>
        private Span CombineSpans(Span a, Span b) => a.IsValid ? (b.IsValid ? Span.Combine(a, b) : a) : b;

        /// <summary>
        /// Associates givcen <paramref name="target"/> refering to instance of <see cref="PHPDocBlock"/> to a target which must be an instance of <see cref="IPropertyCollection"/>.
        /// </summary>
        /// <param name="target"><see cref="IPropertyCollection"/> instance. Must not be <c>null</c>.</param>
        void SetDoc(object target)
        {
            Debug.Assert(target != null);
            Debug.Assert(target is IPropertyCollection);
            _lexer.DocBlockList.Annotate((LangElement)target);
        }

        /// <summary>
        /// Associates given <paramref name="target"/> refering to instance of <see cref="PHPDocBlock"/> to a target which must be an instance of <see cref="IPropertyCollection"/>.
        /// </summary>
        /// <param name="target"><see cref="IPropertyCollection"/> instance. Must not be <c>null</c>.</param>
        void SetMemberDoc(object target)
        {
            Debug.Assert(target != null);
            Debug.Assert(target is IPropertyCollection);
            _lexer.DocBlockList.AnnotateMember((LangElement)target);
        }

        /// <summary>
        /// Creates a <see cref="BlockStmt"/> statement from a list of statements.
        /// Unassigned PHPDoc comments are merged to the statements as <see cref="PHPDocStmt"/>.
        /// </summary>
        /// <param name="span">Span of the entire block.</param>
        /// <param name="statements">List of statements in the block.</param>
        /// <returns>Complete block statement.</returns>
        BlockStmt CreateBlock(Span span, List<LangElement> statements)
        {
            Debug.Assert(statements.All(s => s != null));
            _lexer.DocBlockList.Merge(span, statements, _astFactory);
            return (BlockStmt)_astFactory.Block(span, statements);
        }

        /// <summary>
        /// Creates a <see cref="BlockStmt"/> statement from a list of statements in a case or default.
        /// Unassigned PHPDoc comments are merged to the statements as <see cref="PHPDocStmt"/>, PHPDoc comments after the last statement are ignored.
        /// </summary>
        /// <param name="separatorSpan">Span of the separator ':'.</param>
        /// <param name="statements">List of statements in the block.</param>
        /// <returns>Complete block statement.</returns>
        BlockStmt CreateCaseBlock(Span separatorSpan, List<LangElement> statements)
        {
            if (statements.Count == 0)
                return (BlockStmt)_astFactory.Block(separatorSpan, statements);
            Span bodySpan = Span.Combine(separatorSpan, statements.Last().Span);
            _lexer.DocBlockList.Merge(bodySpan, statements, _astFactory);
            return (BlockStmt)_astFactory.Block(bodySpan, statements);
        }

        /// <summary>
        /// Merges current namespace with the name. 
        /// Returns a qualified name based only on the suffix, if the namespace is <c>null</c>.
        /// </summary>
        /// <param name="currentNamespace">Current namespace name, can be <c>null</c>.</param>
        /// <param name="suffix">Rest of the name, after current namespace.</param>
        /// <returns>Complete qualified name</returns>
        QualifiedName MergeWithCurrentNamespace(QualifiedName? currentNamespace, IList<string> suffix)
        {
            if (!currentNamespace.HasValue)
                return new QualifiedName((List<string>)suffix, true, true);

            QualifiedName currentNS = currentNamespace.Value;
            Name[] namespaces = new Name[currentNS.Namespaces.Length + suffix.Count - 1];
            currentNS.Namespaces.CopyTo(namespaces, 0);
            for (int i = currentNS.Namespaces.Length; i < namespaces.Length; i++)
                namespaces[i] = new Name(suffix[i - currentNS.Namespaces.Length]); // loop otimization
            return new QualifiedName(new Name(suffix.Last()), namespaces, true);
        }
    }
}
