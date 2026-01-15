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

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Direct variable use - a variable or a field accessed by an identifier.
    /// </summary>
    public abstract class DirectVarUse : SimpleVarUse
    {
        protected int _span_start = -1;

        public override Span Span
        {
            get => this.NameSpan;
            protected set => _span_start = value.IsValid ? value.Start : -1;
        }

        public abstract Span NameSpan { get; }

        sealed class LocalDirectVarUse : DirectVarUse
        {
            public override Expression IsMemberOf => null;

            public override Span NameSpan => _span_start < 0 ? Span.Invalid : new Span(_span_start, 1 + VarName.Value.Length);

            public LocalDirectVarUse(Text.Span span, VariableName varName)
                : base(span, varName)
            {
            }
        }

        sealed class MemberDirectVarUse : DirectVarUse
        {
            public override Expression IsMemberOf { get; }

            public override Span NameSpan => _span_start < 0 ? Span.Invalid : new Span(_span_start, VarName.Value.Length);

            public MemberDirectVarUse(Text.Span span, VariableName varName, Expression isMemberOf)
                : base(span, varName)
            {
                this.IsMemberOf = isMemberOf;
            }
        }

        public static DirectVarUse Create(Text.Span span, VariableName varName) => new LocalDirectVarUse(span, varName);

        public static DirectVarUse Create(Text.Span span, VariableName varName, Expression isMemberOf) => isMemberOf != null
            ? new MemberDirectVarUse(span, varName, isMemberOf)
            : Create(span, varName)
            ;

        public override Operations Operation => Operations.DirectVarUse;

        public VariableName VarName { get; set; }

        protected DirectVarUse(Text.Span span, VariableName varName)
            : base(span)
        {
            this.VarName = varName;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDirectVarUse(this);
        }
    }
}
