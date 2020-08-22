using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Syntax.Ast
{
    #region IMatchEx, IMatchArm

    /// <summary>
    /// Match expression.
    /// </summary>
    public interface IMatchEx : IExpression
    {
        /// <summary>
        /// The match value.
        /// </summary>
        IExpression MatchValue { get; }

        /// <summary>
        /// The match arms.
        /// </summary>
        IList<IMatchArm> MatchItems { get; }
    }

    /// <summary>
    /// Match expression branch (arm).
    /// </summary>
    public interface IMatchArm : ILangElement
    {
        /// <summary>
        /// List of conditions (order matters) to compare with <see cref="IMatchEx.MatchValue"/>.
        /// In case the list is empty, this arm represents the <c>default</c> branch.
        /// </summary>
        IList<IExpression> ConditionList { get; }

        /// <summary>
        /// Expression to be evaluated in case one if the conditions are met.
        /// </summary>
        IExpression Expression { get; }
    }

    #endregion

    /// <summary>
    /// Match expression.
    /// </summary>
    public sealed class MatchEx : Expression, IMatchEx
    {
        public MatchEx(Span span, Expression value, IList<MatchArm> arms) : base(span)
        {
            MatchValue = value ?? throw new ArgumentNullException(nameof(value));
            MatchItems = arms.AsArray();
        }

        public override Operations Operation => Operations.Match;

        /// <summary>
        /// The match value.
        /// </summary>
        public Expression MatchValue { get; }

        IExpression IMatchEx.MatchValue => MatchValue;

        /// <summary>
        /// The match arms.
        /// </summary>
        public MatchArm[] MatchItems { get; }

        IList<IMatchArm> IMatchEx.MatchItems => MatchItems;

        public override void VisitMe(TreeVisitor visitor) => visitor.VisitMatchEx(this);
    }

    /// <summary>
    /// Match expression branch (arm).
    /// </summary>
    public sealed class MatchArm : LangElement, IMatchArm
    {
        public MatchArm(Span span, IList<Expression> condition, Expression expression) : base(span)
        {
            this.ConditionList = condition.AsArray();
            this.Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public Expression[] ConditionList { get; }

        IList<IExpression> IMatchArm.ConditionList => ConditionList;

        /// <summary>
        /// Expression to be evaluated in case one if the conditions are met.
        /// </summary>
        public Expression Expression { get; }

        IExpression IMatchArm.Expression => Expression;

        public override void VisitMe(TreeVisitor visitor) => visitor.VisitMatchItem(this);
    }
}
