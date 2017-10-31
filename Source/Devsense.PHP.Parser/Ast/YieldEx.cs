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

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Represents a generator creating expression. Either <c>YieldEx</c> or <c>YieldFromEx</c>.
    /// </summary>
    public interface IYieldLikeEx
    {

    }

    /// <summary>
    /// Represents <c>yield</c> expression for the support for PHP Generator.
    /// </summary>
    public sealed class YieldEx : Expression, IYieldLikeEx
    {
        #region Fields & Properties

        public override Operations Operation { get { return Operations.Yield; } }

        /// <summary>
        /// Represents the key expression in case of <c>yield key =&gt; value</c> form.
        /// Can be a <c>null</c> reference in case of key is not provided.
        /// </summary>
        public Expression KeyExpr { get { return _keyEx; } }

        /// <summary>
        /// Represents the value expression in case of <c>yield key =&gt; value</c> or <c>yield value</c> forms.
        /// Can be a <c>null</c> reference in case of yield is used in read context. (see Generator::send()).
        /// </summary>
        public Expression ValueExpr { get { return _valueEx; } }

        /// <summary>
        /// <c>yield</c> parameters.
        /// </summary>
        private Expression _keyEx, _valueEx;

        /// <summary>
        /// Position of the arrow operator.
        /// </summary>
        public int OperatorPosition
        {
            get { return _operatorOffset < 0 ? -1 : Span.Start + _operatorOffset; }
            set { _operatorOffset = value < 0 ? (short)-1 : (short)(value - Span.Start); }
        }
        private short _operatorOffset = -1;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes new instance of <see cref="YieldEx"/>.
        /// </summary>
        public YieldEx(Text.Span span)
            : this(span, null, null)
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="YieldEx"/>.
        /// </summary>
        public YieldEx(Text.Span span, Expression keyEx, Expression valueEx)
            : base(span)
        {
            if (keyEx != null && valueEx == null) throw new ArgumentException();

            _keyEx = keyEx;
            _valueEx = valueEx;
        }

        #endregion

        #region LangElement

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitYieldEx(this);
        }

        #endregion
    }

    /// <summary>
    /// Represents <c>yield from</c> expression for the support for PHP Generator.
    /// </summary>
    public sealed class YieldFromEx : Expression, IYieldLikeEx
    {
        #region Fields & Properties

        public override Operations Operation { get { return Operations.Yield; } }

        /// <summary>
        /// Represents the value expression in case of <c>yield key =&gt; value</c> or <c>yield value</c> forms.
        /// Can be a <c>null</c> reference in case of yield is used in read context. (see Generator::send()).
        /// </summary>
        public Expression ValueExpr { get { return _valueEx; } }

        /// <summary>
        /// <c>yield</c> parameters.
        /// </summary>
        private Expression _valueEx;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes new instance of <see cref="YieldEx"/>.
        /// </summary>
        public YieldFromEx(Text.Span span, Expression valueEx)
            : base(span)
        {
            if (valueEx == null) throw new ArgumentException();
            
            _valueEx = valueEx;
        }

        #endregion

        #region LangElement

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitYieldFromEx(this);
        }

        #endregion
    }
}
