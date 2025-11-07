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
using System.Diagnostics;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// A binary expression.
    /// </summary>
    public interface IBinaryExpression : IExpression
    {
        /// <summary>
        /// Left operand.
        /// </summary>
        IExpression Left { get; set; }

        /// <summary>
        /// Right operand.
        /// </summary>
        IExpression Right { get; set; }
    }

    /// <summary>
    /// Binary expression.
    /// </summary>
    public abstract class BinaryEx : Expression, IBinaryExpression
    {
        #region IBinaryExpression

        IExpression IBinaryExpression.Left { get => LeftExpr; set => LeftExpr = (Expression)value; }

        IExpression IBinaryExpression.Right { get => RightExpr; set => RightExpr = (Expression)value; }

        #endregion

        #region Fields & Properties

        public Expression/*!*/ LeftExpr { get { return leftExpr; } internal set { Debug.Assert(value != null); leftExpr = value; } }
        private Expression/*!*/ leftExpr;

        public Expression/*!*/ RightExpr { get { return rightExpr; } internal set { Debug.Assert(value != null); rightExpr = value; } }
        private Expression/*!*/ rightExpr;

        public abstract Tokens Token { get; }

        #endregion

        #region Specialized

        sealed class CommonBinaryEx : BinaryEx
        {
            public override Operations Operation => TokenToBinaryOperation(Token);
            public override Tokens Token => (Tokens)_token;

            readonly ushort _token;

            public CommonBinaryEx(Span span, Tokens token, Expression/*!*/ leftExpr, Expression/*!*/ rightExpr)
                : base(span, leftExpr, rightExpr)
            {
                _token = checked((ushort)token);
            }
        }

        sealed class ConcatBinaryEx : BinaryEx
        {
            public override Operations Operation => Operations.Concat;
            public override Tokens Token => Tokens.T_DOT;
            public ConcatBinaryEx(Span span, Expression/*!*/ leftExpr, Expression/*!*/ rightExpr) : base(span, leftExpr, rightExpr) { }
        }

        sealed class IsEqualBinaryEx : BinaryEx
        {
            public override Operations Operation => Operations.Equal;
            public override Tokens Token => Tokens.T_IS_EQUAL;
            public IsEqualBinaryEx(Span span, Expression/*!*/ leftExpr, Expression/*!*/ rightExpr) : base(span, leftExpr, rightExpr) { }
        }

        #endregion

        #region Construction

        public static BinaryEx Create(Span span, Tokens token, Expression/*!*/ leftExpr, Expression/*!*/ rightExpr)
        {
            Debug.Assert(TryTokenToBinaryOperation(token, out _));

            switch (token)
            {
                case Tokens.T_DOT: // .
                    return new ConcatBinaryEx(span, leftExpr, rightExpr);
                case Tokens.T_IS_EQUAL: // ==
                    return new IsEqualBinaryEx(span, leftExpr, rightExpr);
                default:
                    return new CommonBinaryEx(span, token, leftExpr, rightExpr);
            }
        }

        private BinaryEx(Span span, Expression/*!*/ leftExpr, Expression/*!*/ rightExpr)
            : base(span)
        {
            Debug.Assert(leftExpr != rightExpr, "LeftExpr == RightExpr");

            this.LeftExpr = leftExpr;
            this.RightExpr = rightExpr;
        }

        #endregion

        public static bool TryTokenToBinaryOperation(Tokens token, out Operations op) => (op = TokenToBinaryOperation(token)) != 0;

        /// <summary>
        /// Gets <see cref="Operations"/> corresponding to given <see cref="Tokens"/>.
        /// Returns <c>0</c> if token does not correspond to any binary operation.
        /// </summary>
        public static Operations TokenToBinaryOperation(Tokens token)
        {
            switch (token)
            {
                case Tokens.T_BOOLEAN_AND:
                case Tokens.T_LOGICAL_AND: return Operations.And;
                case Tokens.T_BOOLEAN_OR:
                case Tokens.T_LOGICAL_OR: return Operations.Or;
                case Tokens.T_LOGICAL_XOR: return Operations.Xor;
                case Tokens.T_PIPE: return Operations.BitOr;
                case Tokens.T_CARET: return Operations.BitXor;
                case Tokens.T_AMPERSAND_NOT_FOLLOWED_BY_VAR_OR_VARARG: return Operations.BitAnd;
                case Tokens.T_AMPERSAND_FOLLOWED_BY_VAR_OR_VARARG: return Operations.BitAnd;
                case Tokens.T_DOT: return Operations.Concat;
                case Tokens.T_PLUS: return Operations.Add;
                case Tokens.T_MINUS: return Operations.Sub;
                case Tokens.T_MUL: return Operations.Mul;
                case Tokens.T_POW: return Operations.Pow;
                case Tokens.T_SLASH: return Operations.Div;
                case Tokens.T_PERCENT: return Operations.Mod;
                case Tokens.T_SL: return Operations.ShiftLeft;
                case Tokens.T_SR: return Operations.ShiftRight;
                case Tokens.T_IS_IDENTICAL: return Operations.Identical;
                case Tokens.T_IS_NOT_IDENTICAL: return Operations.NotIdentical;
                case Tokens.T_IS_EQUAL: return Operations.Equal;
                case Tokens.T_IS_NOT_EQUAL: return Operations.NotEqual;
                case Tokens.T_PIPE_OPERATOR: return Operations.Pipe;
                case Tokens.T_LT: return Operations.LessThan;
                case Tokens.T_IS_SMALLER_OR_EQUAL: return Operations.LessThanOrEqual;
                case Tokens.T_GT: return Operations.GreaterThan;
                case Tokens.T_IS_GREATER_OR_EQUAL: return Operations.GreaterThanOrEqual;
                case Tokens.T_SPACESHIP: return Operations.Spaceship;
                case Tokens.T_COALESCE: return Operations.Coalesce;

                default: return 0;
            }
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitBinaryEx(this);
        }
    }
}