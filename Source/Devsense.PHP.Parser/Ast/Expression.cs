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
using Devsense.PHP.Text;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
    #region enum Operations

    public enum Operations
    {
        // unary ops:
        Plus,
        Minus,
        LogicNegation,
        BitNegation,
        AtSign,
        Print,
        Clone,
        Match,

        // casts:
        BoolCast,
        Int8Cast,
        Int16Cast,
        Int32Cast,
        Int64Cast,
        UInt8Cast,
        UInt16Cast,
        UInt32Cast,
        UInt64Cast,
        DoubleCast,
        FloatCast,
        DecimalCast,
        StringCast,
        BinaryCast,
        UnicodeCast,
        ObjectCast,
        ArrayCast,
        UnsetCast,

        // binary ops:
        Xor, Or, And,
        BitOr, BitXor, BitAnd,
        Equal, NotEqual,
        Identical, NotIdentical,
        LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual,
        ShiftLeft, ShiftRight,
        Add, Sub, Mul, Div, Mod, Pow,
        Concat, Spaceship, Coalesce,

        // n-ary ops:
        ConcatN,
        List,
        Conditional,

        // assignments:
        AssignRef,
        AssignValue,
        AssignAdd,
        AssignSub,
        AssignMul,
        AssignPow,
        AssignDiv,
        AssignMod,
        AssignAnd,
        AssignOr,
        AssignXor,
        AssignShiftLeft,
        AssignShiftRight,
        AssignAppend,
        AssignPrepend,
        AssignCoalesce,

        // constants, variables, fields, items:
        GlobalConstUse,
        ClassConstUse,
        PseudoConstUse,
        DirectVarUse,
        IndirectVarUse,
        DirectStaticFieldUse,
        IndirectStaticFieldUse,
        ItemUse,

        // literals:
        NullLiteral,
        BoolLiteral,
        IntLiteral,
        LongIntLiteral,
        DoubleLiteral,
        StringLiteral,
        BinaryStringLiteral,

        // routine calls:
        DirectCall,
        IndirectCall,
        DirectStaticCall,
        IndirectStaticCall,

        // instances:
        New,
        Array,
        InstanceOf,
        TypeOf,

        // built-in functions:
        Inclusion,
        Isset,
        Empty,
        Eval,

        // others:
        Exit,
        ShellCommand,
        IncDec,
        Yield,
        Throw,
        Parenthesis,

        // lambda function:
        Closure,
        ArrowFunc,
    }

    #endregion

    /// <summary>
    /// An expression.
    /// </summary>
    public interface IExpression : ILangElement
    {
        /// <summary>
        /// Operation represented by the expression
        /// </summary>
        Operations Operation { get; }
    }

    #region Expression

    /// <summary>
    /// Abstract base class for expressions.
    /// </summary>
    public abstract class Expression : LangElement, IExpression
    {
        /// <summary>
        /// Operation represented by the expression
        /// </summary>
        public abstract Operations Operation { get; }

        protected Expression(Text.Span span) : base(span) { }

        /// <summary>
        /// Compressed type information used by eventual type analysis.
        /// </summary>
        public ulong TypeInfoValue { get; set; }

        /// <summary>
        /// Whether the expression is allowed to be passed by reference to a routine.
        /// </summary>
        internal virtual bool AllowsPassByReference { get { return false; } }
    }

    #endregion

    #region ConstantDecl

    public abstract class ConstantDecl : LangElement
    {
        /// <summary>
        /// Constant name.
        /// </summary>
        public VariableName Name { get; } // NOTE: Variable actually
        
        public Span NameSpan { get; }

        /// <summary>
        /// Initial value of the constant.
        /// </summary>
        public Expression/*!*/ Initializer { get { return initializer; } internal set { initializer = value; } }
        private Expression/*!*/ initializer;

        /// <summary>
        /// Constant construtor
        /// </summary>
        /// <param name="span">Declaration span.</param>
        /// <param name="name">Constant name.</param>
        /// <param name="namePos">Position of the name.</param>
        /// <param name="initializer">Initial value of the ocnstant.</param>
        public ConstantDecl(Text.Span span, string/*!*/ name, Text.Span namePos, Expression/*!*/ initializer)
            : base(span)
        {
            Debug.Assert(initializer != null);
            this.Name = new VariableName(name);
            this.NameSpan = namePos;
            this.initializer = initializer;
        }
    }

    #endregion

    #region VarLikeConstructUse

    /// <summary>
    /// Common abstract base class representing all constructs that behave like a variable (L-value).
    /// </summary>
    public abstract class VarLikeConstructUse : Expression
    {
        public abstract Expression IsMemberOf { get; }

        sealed class NullSafeOperationFlag
        {
            public static bool HasFlag(AstNode node) => node.TryGetProperty<NullSafeOperationFlag>(out _);

            public static void SetFlag(AstNode node, bool value)
            {
                if (value)
                {
                    node.SetProperty(Instance);
                }
                else
                {
                    ((IPropertyCollection)node).RemoveProperty<NullSafeOperationFlag>();
                }
            }

            static readonly NullSafeOperationFlag Instance = new NullSafeOperationFlag();

            private NullSafeOperationFlag() { }
        }

        /// <summary>
        /// Gets value indicating the object operation is null-safe (<c>?-&gt;</c>).
        /// </summary>
        public bool IsNullSafeObjectOperation
        {
            get => IsMemberOf != null && NullSafeOperationFlag.HasFlag(this);
            internal set => NullSafeOperationFlag.SetFlag(this, value);
        }

        internal override bool AllowsPassByReference { get { return true; } }

        protected VarLikeConstructUse(Text.Span p) : base(p) { }
    }

    #endregion

    #region EncapsedExpression

    /// <summary>
    /// Expression representing an enclosed expression in parenthesis, braces or quotes.
    /// </summary>
    public abstract class EncapsedExpression : Expression
    {
        #region ParenthesisExpression

        /// <summary>
        /// Expression representing a parenthesis enclosed expression.
        /// </summary>
        public sealed class ParenthesisExpression : EncapsedExpression
        {
            public override Tokens OpenToken => Tokens.T_LPAREN;

            public override Tokens CloseToken => Tokens.T_RPAREN;

            public ParenthesisExpression(Span span, Expression expression) : base(span, expression)
            {
                _expression = expression;
            }
        }

        #endregion

        #region BracesExpression

        /// <summary>
        /// Expression representing a parenthesis enclosed expression.
        /// </summary>
        public sealed class BracesExpression : EncapsedExpression
        {
            public override Tokens OpenToken => Tokens.T_LBRACE;

            public override Tokens CloseToken => Tokens.T_RBRACE;

            public BracesExpression(Span span, Expression expression) : base(span, expression)
            {
                _expression = expression;
            }
        }

        #endregion

        #region DollarBracesExpression

        /// <summary>
        /// Expression representing a parenthesis enclosed expression.
        /// </summary>
        public sealed class DollarBracesExpression : EncapsedExpression
        {
            public override Tokens OpenToken => Tokens.T_DOLLAR_OPEN_CURLY_BRACES;

            public override Tokens CloseToken => Tokens.T_RBRACE;

            public DollarBracesExpression(Span span, Expression expression) : base(span, expression)
            {
                _expression = expression;
            }
        }

        #endregion

        #region StringEncapsedExpression

        /// <summary>
        /// Expression representing a concat expression enclosed in quotes or labels.
        /// </summary>
        public abstract class StringEncapsedExpression : EncapsedExpression
        {
            public abstract string OpenLabel { get; }

            public abstract string CloseLabel { get; }

            public StringEncapsedExpression(Span span, Expression expression) : base(span, expression) { }
        }

        #endregion

        #region SingleQuotedExpression

        /// <summary>
        /// Expression representing a single-quote enclosed expression.
        /// </summary>
        public sealed class SingleQuotedExpression : StringEncapsedExpression
        {
            public override Tokens OpenToken => Tokens.T_SINGLE_QUOTES;

            public override Tokens CloseToken => Tokens.T_SINGLE_QUOTES;

            public override string OpenLabel => "'";

            public override string CloseLabel => "'";

            public SingleQuotedExpression(Span span, Expression expression) : base(span, expression)
            {
                _expression = expression;
            }
        }

        #endregion

        #region DoubleQuotedExpression

        /// <summary>
        /// Expression representing a double-quote enclosed expression.
        /// </summary>
        public sealed class DoubleQuotedExpression : StringEncapsedExpression
        {
            public override Tokens OpenToken => Tokens.T_DOUBLE_QUOTES;

            public override Tokens CloseToken => Tokens.T_DOUBLE_QUOTES;

            public override string OpenLabel => @"""";

            public override string CloseLabel => @"""";

            public DoubleQuotedExpression(Span span, Expression expression) : base(span, expression)
            {
                _expression = expression;
            }
        }

        #endregion

        #region BackQuotedExpression

        /// <summary>
        /// Expression representing a back-ticks enclosed expression.
        /// </summary>
        public sealed class BackQuotedExpression : StringEncapsedExpression
        {
            public override Tokens OpenToken => Tokens.T_BACKQUOTE;

            public override Tokens CloseToken => Tokens.T_BACKQUOTE;

            public override string OpenLabel => "`";

            public override string CloseLabel => "`";

            public BackQuotedExpression(Span span, Expression expression) : base(span, expression)
            {
                _expression = expression;
            }
        }

        #endregion

        #region NowDocExpression

        /// <summary>
        /// Expression representing a nowdoc expression.
        /// </summary>
        public sealed class NowDocExpression : StringEncapsedExpression
        {
            public override Tokens OpenToken => Tokens.T_START_HEREDOC;

            public override Tokens CloseToken => Tokens.T_END_HEREDOC;

            public override string OpenLabel => $"<<<'{Label}'\r\n";

            public override string CloseLabel => Label;

            /// <summary>
            /// NOWDOC label
            /// </summary>
            public string Label { get; }

            public NowDocExpression(Span span, Expression expression, string label)
                : base(span, expression)
            {
                this.Label = label;
                _expression = expression;
            }
        }

        #endregion

        #region HereDocExpression

        /// <summary>
        /// Expression representing a heredoc expression.
        /// </summary>
        public sealed class HereDocExpression : StringEncapsedExpression
        {
            public override Tokens OpenToken => Tokens.T_START_HEREDOC;

            public override Tokens CloseToken => Tokens.T_END_HEREDOC;

            public override string OpenLabel => "<<<" + QuotedLabel + "\r\n";

            public override string CloseLabel => Label;

            /// <summary>
            /// NOWDOC label
            /// </summary>
            public string Label { get; }

            string QuotedLabel => _quoted ? ("\"" + Label + "\"") : Label;
            readonly bool _quoted;

            public HereDocExpression(Span span, Expression expression, string label, bool quoted)
                : base(span, expression)
            {
                this.Label = label;
                _expression = expression;
                _quoted = quoted;
            }
        }

        #endregion

        public abstract Tokens OpenToken { get; }

        public abstract Tokens CloseToken { get; }

        public Expression Expression => _expression;

        public override Operations Operation => Operations.Parenthesis;

        internal override bool AllowsPassByReference => _expression.AllowsPassByReference;

        protected Expression _expression;

        public EncapsedExpression(Span span, Expression expression) : base(span)
        {
            Debug.Assert(expression != null);
            _expression = expression;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitEncapsedExpression(this);
        }
    }

    #endregion
}