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

namespace Devsense.PHP.Syntax.Ast
{
    #region WhileStmt

    /// <summary>
    /// Represents a while-loop statement.
    /// </summary>
    public sealed class WhileStmt : Statement
    {
        public enum Type { While, Do };

        /// <summary>Type of statement</summary>
        public Type LoopType { get { return type; } }
        private Type type;

        /// <summary>
        /// Condition or a <B>null</B> reference for unbounded loop.
        /// </summary>
        public Expression CondExpr { get { return condExpr; } internal set { condExpr = value; } }
        private Expression condExpr;

        /// <summary>
        /// Position of the header parentheses.
        /// </summary>
        public Text.Span ConditionPosition { get { return _conditionPosition; } }
        private Text.Span _conditionPosition;

        /// <summary>Body of loop</summary>
        public Statement/*!*/ Body { get { return body; } internal set { body = value; } }
        private Statement/*!*/ body;

        /// <summary>
        /// Position of the 'function' keyword.
        /// </summary>
        public int WhilePosition
        {
            get { return Span.Start + _whileOffset; }
            set { _whileOffset = (short)(value - Span.Start); }
        }
        private short _whileOffset = 0;

        public WhileStmt(Text.Span span, Type type, Expression/*!*/ condExpr, Text.Span conditionSpan, Statement/*!*/ body)
            : base(span)
        {
            Debug.Assert(condExpr != null && body != null);

            this.type = type;
            this.condExpr = condExpr;
            this.body = body;
            this._conditionPosition = conditionSpan;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitWhileStmt(this);
        }
    }

    #endregion

    #region ForStmt

    /// <summary>
    /// Represents a for-loop statement.
    /// </summary>
    public sealed class ForStmt : Statement
    {
        private readonly Expression[]/*!*/ initExList;
        private readonly Expression[]/*!*/ condExList;
        private readonly Expression[]/*!*/ actionExList;
        private Statement/*!*/ body;

        /// <summary>List of expressions used for initialization</summary>
        public IList<Expression> /*!*/ InitExList { get { return initExList; } }
        /// <summary>List of expressions used as condition</summary>
        public IList<Expression> /*!*/ CondExList { get { return condExList; } }
        /// <summary>List of expressions used to incrent iterator</summary>
        public IList<Expression> /*!*/ ActionExList { get { return actionExList; } }
        /// <summary>Body of statement</summary>
        public Statement/*!*/ Body { get { return body; } internal set { body = value; } }

        /// <summary>
        /// Position of the header parentheses.
        /// </summary>
        public Text.Span ConditionPosition { get { return _conditionPosition; } }
        private Text.Span _conditionPosition;

        public ForStmt(Text.Span p,
            IList<Expression>/*!*/ initExList, IList<Expression>/*!*/ condExList, IList<Expression>/*!*/ actionExList,
            Text.Span conditionSpan,Statement/*!*/ body)
            : base(p)
        {
            Debug.Assert(initExList != null && condExList != null && actionExList != null && body != null);

            this.initExList = initExList.AsArray();
            this.condExList = condExList.AsArray();
            this.actionExList = actionExList.AsArray();
            this.body = body;
            this._conditionPosition = conditionSpan;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitForStmt(this);
        }
    }
    #endregion

    #region ForeachStmt

    /// <summary>
    /// Represents a foreach-loop statement.
    /// </summary>
    public sealed class ForeachVar : AstNode
    {
        /// <summary>
        /// Whether the variable is aliased.
        /// </summary>
        public bool Alias { get { return _alias; } set { _alias = value; } }
        private bool _alias;

        /// <summary>
        /// The variable itself. Can be <c>null</c> reference if <see cref="ListEx"/> is represented instead.
        /// </summary>
        public VariableUse Variable { get { return _target as VariableUse; } }

        /// <summary>
        /// PHP list expression. Can be <c>null</c> reference if <see cref="VariableUse"/> is represented instead.
        /// </summary>
        public ListEx List { get { return _target as ListEx; } }

        /// <summary>
        /// PHP array expression. Can be <c>null</c> reference if <see cref="VariableUse"/> is represented instead.
        /// </summary>
        public ArrayEx Array { get { return _target as ArrayEx; } }

        /// <summary>
        /// Inner expression representing <see cref="Variable"/> or <see cref="List"/>.
        /// </summary>
        public VarLikeConstructUse/*!*/Target => _target;
        private readonly VarLikeConstructUse/*!*/_target;

        /// <summary>
        /// Position of foreach variable.
        /// </summary>
        public Text.Span Span => _target.Span;

        public ForeachVar(VariableUse variable, bool alias)
        {
            _target = variable;
            _alias = alias;
        }

        /// <summary>
        /// Initializes instance of <see cref="ForeachVar"/> representing PHP list expression.
        /// </summary>
        /// <param name="list"></param>
        public ForeachVar(ListEx/*!*/list)
        {
            Debug.Assert(list != null);

            _target = list;
            _alias = false;
        }

        /// <summary>
        /// Initializes instance of <see cref="ForeachVar"/> representing PHP list expression.
        /// </summary>
        /// <param name="array"></param>
        public ForeachVar(ArrayEx/*!*/array)
        {
            Debug.Assert(array != null);

            _target = array;
            _alias = false;
        }
    }

    /// <summary>
    /// Represents a foreach statement.
    /// </summary>
    public class ForeachStmt : Statement
    {
        private Expression/*!*/ enumeree;
        /// <summary>Array to enumerate through</summary>
        public Expression /*!*/Enumeree { get { return enumeree; } }
        private ForeachVar keyVariable;
        /// <summary>Variable to store key in (can be null)</summary>
        public ForeachVar KeyVariable { get { return keyVariable; } }
        private ForeachVar/*!*/ valueVariable;
        /// <summary>Variable to store value in</summary>
        public ForeachVar /*!*/ ValueVariable { get { return valueVariable; } }
        private Statement/*!*/ body;
        /// <summary>Body - statement in loop</summary>
        public Statement/*!*/ Body { get { return body; } internal set { body = value; } }

        /// <summary>
        /// Position of the header parentheses.
        /// </summary>
        public Text.Span ConditionPosition { get { return _conditionPosition; } }
        private Text.Span _conditionPosition;

        public ForeachStmt(Text.Span span, Expression/*!*/ enumeree, ForeachVar key, ForeachVar/*!*/ value,
          Text.Span conditionSpan, Statement/*!*/ body)
            : base(span)
        {
            Debug.Assert(enumeree != null && value != null && body != null);

            this.enumeree = enumeree;
            this.keyVariable = key;
            this.valueVariable = value;
            this.body = body;
            this._conditionPosition = conditionSpan;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitForeachStmt(this);
        }
    }

    #endregion
}
