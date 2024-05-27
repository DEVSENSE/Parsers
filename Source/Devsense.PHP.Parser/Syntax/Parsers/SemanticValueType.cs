using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Devsense.PHP.Syntax
{
    struct StringPair
    {
        public string Text;
        public string SourceCode;

        public StringPair(string text, string sourcecode)
        {
            this.Text = text;
            this.SourceCode = sourcecode;
        }
    }

    struct AnonymousClass
    {
        public Span Span;
        public TypeRef TypeRef;
        public List<ActualParam> ActualParams;

        public AnonymousClass(TypeRef tref, List<ActualParam> @params, Span span)
        {
            Span = span;
            TypeRef = tref;
            ActualParams = @params;
        }
    }

    class ContextAlias
    {
        public Span Span;
        public QualifiedNameRef QualifiedName;
        public NameRef Name;
        public AliasKind Kind;

        public ContextAlias(Span span, QualifiedNameRef qualifiedName, NameRef name, AliasKind kind)
        {
            Span = span;
            QualifiedName = qualifiedName;
            Name = name;
            Kind = kind;
        }
    }

    class CompleteAlias
    {
        public QualifiedNameRef QualifiedName;
        public NameRef Name;

        public CompleteAlias(QualifiedNameRef qualifiedName, NameRef name)
        {
            QualifiedName = qualifiedName;
            Name = name;
        }
    }

    struct IfStatement
    {
        public readonly Span Span;
        public readonly LangElement Condition;
        public readonly Span ConditionSpan;
        public readonly LangElement Body;

        public IfStatement(Span span, LangElement condition, Span condSpan, LangElement body)
        {
            Span = span;
            Condition = condition;
            ConditionSpan = condSpan;
            Body = body;
        }
    }

    /// <summary>
    /// Parser information for the <c>switch</c> statement.
    /// </summary>
    class SwitchObject
    {
        public List<LangElement> CaseList { get; } = new List<LangElement>();

        public Tokens ClosingToken { get; set; } = Tokens.T_RBRACE;

        public Span ClosingTokenSpan { get; set; } = Span.Invalid;

        public SwitchObject WithClosingToken(Tokens closing, Span closingSpan)
        {
            this.ClosingToken = closing;
            this.ClosingTokenSpan = closingSpan;
            return this;
        }
    }

    /// <summary>
    /// Compressed parser value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public partial struct SemanticValueType
    {
        [StructLayout(LayoutKind.Sequential)]
        struct ReferenceTypes
        {
            [Flags]
            enum TypesFlags : byte
            {
                IsFullyQualifiedName = 1,
            }

            public object Object1;
            public object Object2;
            byte _flags;

            public ReferenceTypes(object o) : this(o, null) { }

            public ReferenceTypes(object o1, object o2) : this(o1, o2, 0) { }

            public ReferenceTypes(object o1, object o2, byte flags)
            {
                Object1 = o1;
                Object2 = o2;
                _flags = flags;
            }

            public static implicit operator QualifiedName(ReferenceTypes r) => new QualifiedName(
                new Name((string)r.Object1),
                (Name[])r.Object2,
                (r._flags & (byte)TypesFlags.IsFullyQualifiedName) != 0
            );

            public static implicit operator ReferenceTypes(QualifiedName qn) => new ReferenceTypes(
                qn.Name.Value,
                qn.Namespaces,
                qn.IsFullyQualifiedName ? (byte)TypesFlags.IsFullyQualifiedName : (byte)0
            );

            public ActualParam AsActualParam(int start) => new ActualParam(Object1, start, (byte)_flags);

            public ArrayItem AsArrayItem() => ArrayItem.CreateFromCompressed(Object1, Object2, _flags);
        }

        // 1. value types:

        [FieldOffset(0)]
        double _double; // 8

        [FieldOffset(0)]
        long _long; // 8

        [FieldOffset(0)]
        Span _span; // 8

        // 2 referece types at the end
        // let .NET allign the struct according to sizeof(object)

        [FieldOffset(8)]
        ReferenceTypes _objs; // ...

        //
        // Properties:
        //

        public bool Bool { get => _long != 0; set => _long = value ? 1 : 0; }
        public long Long { get => _long; set => _long = value; }
        public double Double { get => _double; set => _double = value; }
        public int Integer { get => (int)_long; set => _long = value; }
        public object Object { get => _objs.Object1; set => _objs = new ReferenceTypes(value); }
        public string String { get => (string)Object; set => Object = value; }
        public AliasKind Kind { get => (AliasKind)_long; set => _long = (long)value; }
        /// <summary>The original token.</summary>
        public Tokens Token { get => (Tokens)_long; set => _long = (long)value; }
        /// <summary>Token that encapsulates the string literal.</summary>
        public Tokens QuoteToken { get => Token; set => Token = value; }

        public QualifiedName QualifiedName { get => _objs; set => _objs = value; }
        public QualifiedNameRef QualifiedNameReference { get => new QualifiedNameRef(_span, QualifiedName); set { _span = value.Span; this.QualifiedName = value.QualifiedName; } }

        public TypeRef TypeReference { get { return (TypeRef)Object; } set { Object = value; } }
        public List<TypeRef> TypeRefList { get { return (List<TypeRef>)Object; } set { Object = value; } }
        public LangElement Node { get { return (LangElement)Object; } set { Object = value; } }
        public List<LangElement> NodeList { get { return (List<LangElement>)Object; } set { Object = value; } }
        public ActualParam Param
        {
            get => _objs.AsActualParam(_span.Start);
            set
            {
                value.Compress(out var o, out var start, out var flags);
                _objs = new ReferenceTypes(o, null, flags);
                _span = new Span(start, 0);
            }
        }
        public List<ActualParam> ParamList { get { return (List<ActualParam>)Object; } set { Object = value; } }
        internal AnonymousClass AnonymousClass
        {
            get => new AnonymousClass((TypeRef)_objs.Object1, (List<ActualParam>)_objs.Object2, _span);
            set
            {
                _objs = new ReferenceTypes(value.TypeRef, value.ActualParams);
                _span = value.Span;
            }
        }
        internal SwitchObject SwitchObject { get { return (SwitchObject)Object; } set { Object = value; } }
        internal StringPair Strings { get => new StringPair((string)_objs.Object1, (string)_objs.Object2); set => _objs = new ReferenceTypes(value.Text, value.SourceCode); }
        public List<string> StringList { get { return (List<string>)Object; } set { Object = value; } }
        internal CompleteAlias Alias { get { return (CompleteAlias)Object; } set { Object = value; } }
        internal List<CompleteAlias> AliasList { get { return (List<CompleteAlias>)Object; } set { Object = value; } }
        internal ContextAlias ContextAlias { get { return (ContextAlias)Object; } set { Object = value; } }
        internal List<ContextAlias> ContextAliasList { get { return (List<ContextAlias>)Object; } set { Object = value; } }
        public FormalParam FormalParam { get { return (FormalParam)Object; } set { Object = value; } }
        public List<FormalParam> FormalParamList { get { return (List<FormalParam>)Object; } set { Object = value; } }
        public ArrayItem Item
        {
            get => _objs.AsArrayItem();
            set
            {
                value.Compress(out var o1, out var o2, out var flags);
                _objs = new ReferenceTypes(o1, o2, flags);
            }
        }
        public IList<ArrayItem> ItemList { get { return (IList<ArrayItem>)Object; } set { Object = value; } }
        internal List<IfStatement> IfItemList { get { return (List<IfStatement>)Object; } set { Object = value; } }
        public ForeachVar ForeachVar { get { return (ForeachVar)Object; } set { Object = value; } }
        public UseBase Use { get { return (UseBase)Object; } set { Object = value; } }
        public List<UseBase> UseList { get { return (List<UseBase>)Object; } set { Object = value; } }
        public Lexer.HereDocTokenValue HereDocValue { get { return (Lexer.HereDocTokenValue)Object; } set { Object = value; } }
    }
}
