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

using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Represents an <c>echo</c> statement.
    /// </summary>
    public abstract class EchoStmt : Statement
    {
        #region ParametersEnumerable    // TODO: NETSTANDARD2.1+ // ReadOnlySpan<IExpression>

        public struct ParametersEnumerable // : IReadOnlyList<IExpression>
        {
            readonly IExpression _single;

            readonly IExpression[] _array;

            public ParametersEnumerable(IExpression single)
            {
                _single = single;
                _array = null;
            }

            public ParametersEnumerable(IExpression[] array)
            {
                _single = null;
                _array = array;
            }

            public IExpression this[int index]
            {
                get
                {
                    if (_single != null)
                    {
                        if (index == 0)
                            return _single;
                    }
                    else if (_array != null)
                    {
                        return _array[index];
                    }

                    throw new ArgumentOutOfRangeException();
                }
            }

            public int Count => _single != null ? 1 : _array != null ? _array.Length : 0;

            public ParametersEnumerator GetEnumerator() => new ParametersEnumerator(this);

            public void CopyTo(Span<IExpression> span)
            {
                if (_single != null)
                {
                    span[0] = _single;
                }
                else if (_array != null)
                {
                    _array.CopyTo(span);
                }
            }
        }

        public ref struct ParametersEnumerator // : IEnumerator<IExpression>
        {
            readonly ParametersEnumerable _parameters;

            int _index;

            public ParametersEnumerator(ParametersEnumerable parameters)
            {
                _parameters = parameters;
                _index = -1;
            }

            public IExpression Current => _index >= 0 ? _parameters[_index] : null;

            public bool MoveNext()
            {
                return ++_index < _parameters.Count;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        #endregion

        #region SimpleEcho

        /// <summary>
        /// Most common form of `echo`.
        /// </summary>
        sealed class SimpleEcho : EchoStmt
        {
            readonly IExpression _expr;

            public SimpleEcho(Span span, IExpression expr) : base(span)
            {
                _expr = expr;
            }

            public override bool IsHtmlCode => false;

            public override ParametersEnumerable Parameters => new ParametersEnumerable(_expr);
        }

        #endregion

        #region MultiEcho

        sealed class MultiEcho : EchoStmt
        {
            readonly IExpression[] _array;

            public MultiEcho(Span span, IExpression[] exprs)
                : base(span)
            {
                _array = exprs;
            }

            public override bool IsHtmlCode => false;

            public override ParametersEnumerable Parameters => new ParametersEnumerable(_array);
        }

        #endregion

        #region HtmlCode

        sealed class HtmlCode : EchoStmt
        {
            readonly StringLiteral _htmlcode;

            public HtmlCode(Span span, string htmlCode) : base(span)
            {
                _htmlcode = StringLiteral.Create(span, htmlCode);
            }

            public override bool IsHtmlCode => true;

            public override ParametersEnumerable Parameters => new ParametersEnumerable(_htmlcode);
        }

        #endregion

        /// <summary>Collection of parameters - Expressions.</summary>
        public abstract ParametersEnumerable Parameters { get; }

        /// <summary>
        /// Gets value indicating whether this <see cref="EchoStmt"/> represents HTML code.
        /// </summary>
        public abstract bool IsHtmlCode { get; }

        protected EchoStmt(Text.Span span)
            : base(span)
        {
        }

        public static EchoStmt CreateEcho(Text.Span span, IExpression expr) => new SimpleEcho(span, expr);

        public static EchoStmt CreateEcho(Text.Span span, IExpression[] exprs) => new MultiEcho(span, exprs);

        public static EchoStmt CreateHtml(Text.Span span, string htmlCode) => new HtmlCode(span, htmlCode);

        ///// <summary>
        ///// Initializes new echo statement as a representation of HTML code.
        ///// </summary>
        //public EchoStmt(Text.Span span, string htmlCode)
        //    : base(span)
        //{
        //    this.parameters = new Expression[] { StringLiteral.Create(span, htmlCode) };
        //    this.isHtmlCode = true;
        //}

        // not used anymore
        internal override bool SkipInPureGlobalCode()
        {
            return
                Parameters.Count == 1 &&
                Parameters[0] is StringLiteral literal &&
                StringUtils.IsWhitespace(literal.Value)
                ;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitEchoStmt(this);
        }
    }
}