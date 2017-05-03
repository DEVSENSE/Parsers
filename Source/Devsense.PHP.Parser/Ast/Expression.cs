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
        Parenthesis,

        // lambda function:
        Closure,
    }

    #endregion

    #region Expression

    /// <summary>
    /// Abstract base class for expressions.
    /// </summary>
    public abstract class Expression : LangElement
    {
        /// <summary>
        /// Immutable empty list of <see cref="Expression"/>.
        /// </summary>
        internal new static readonly List<Expression>/*!*/EmptyList = new List<Expression>();

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

        /// <summary>
        /// Whether to mark sequence point when the expression appears in an expression statement.
        /// </summary>
        internal virtual bool DoMarkSequencePoint { get { return true; } }
    }

    #endregion

    #region ConstantDecl

    public abstract class ConstantDecl : LangElement
    {
        public VariableNameRef Name { get { return name; } }
        protected VariableNameRef name;

        public Expression/*!*/ Initializer { get { return initializer; } internal set { initializer = value; } }
        private Expression/*!*/ initializer;

        public ConstantDecl(Text.Span span, string/*!*/ name, Text.Span namePos, Expression/*!*/ initializer)
            : base(span)
        {
            this.name = new VariableNameRef(namePos, name);
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
        public VarLikeConstructUse IsMemberOf { get { return isMemberOf; } set { isMemberOf = value; } }
        protected VarLikeConstructUse isMemberOf;

        internal override bool AllowsPassByReference { get { return true; } }

        protected VarLikeConstructUse(Text.Span p) : base(p) { }
    }

    #endregion

    #region ParenthesisExpression

    /// <summary>
    /// Expression representing a parenthesis enclosed expression.
    /// </summary>
    public class ParenthesisExpression : Expression
    {
        public Expression Expression => _expression;

        public override Operations Operation => Operations.Parenthesis;

        protected Expression _expression;

        public ParenthesisExpression(Span span, Expression expression) : base(span)
        {
            _expression = expression;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitParenthesisExpression(this);
        }
    }

    #endregion
}