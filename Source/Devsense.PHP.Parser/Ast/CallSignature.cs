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
    #region ActualParam

    /// <summary>
    /// Represents a single argument passed to <see cref="FunctionCall"/>.
    /// </summary>
    public sealed class ActualParam : LangElement
    {
        [Flags]
        public enum Flags : byte
        {
            Default = 0,
            IsByRef = 1,
            IsUnpack = 2,
        }

        public Expression Expression { get; }

        /// <summary>
        /// Flags describing use of the parameter.
        /// </summary>
        readonly Flags _flags;

        /// <summary>
        /// Gets value indicating whether the parameter is prefixed by <c>&amp;</c> character.
        /// </summary>
        public bool Ampersand => (_flags & Flags.IsByRef) != 0;

        /// <summary>
        /// Gets value indicating whether the parameter is passed with <c>...</c> prefix and so it has to be unpacked before passing to the function call.
        /// </summary>
        public bool IsUnpack => (_flags & Flags.IsUnpack) != 0;

        public ActualParam(Text.Span p, Expression param)
            : this(p, param, Flags.Default)
        { }

        public ActualParam(Text.Span p, Expression param, Flags flags)
            : base(p)
        {
            Expression = param ?? throw new ArgumentNullException(nameof(param));
            _flags = flags;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitActualParam(this);
        }
    }

    #endregion

    #region NamedActualParam

    [Obsolete("This is not used and will be removed.")]
    public sealed class NamedActualParam : LangElement
    {
        public Expression/*!*/ Expression { get { return expression; } }
        internal Expression/*!*/ expression;

        public VariableName Name { get { return name; } }
        private VariableName name;

        public NamedActualParam(Text.Span span, string name, Expression/*!*/ expression)
            : base(span)
        {
            this.name = new VariableName(name);
            this.expression = expression;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            throw new NotSupportedException();
            //visitor.VisitNamedActualParam(this);
        }
    }

    #endregion

    #region CallSignature

    public struct CallSignature
    {
        public static CallSignature Empty => new CallSignature(ArrayUtils.Empty<ActualParam>(), Text.Span.Invalid);

        /// <summary>
        /// Gets value indicating the signature is empty.
        /// </summary>
        public bool IsEmpty => Parameters == null || Parameters.Length == 0;

        /// <summary>
        /// List of actual parameters (<see cref="ActualParam"/> nodes).
        /// </summary>	
        public ActualParam[]/*!*/ Parameters { get; }

        /// <summary>
        /// Signature position including the parentheses.
        /// </summary>
        public Text.Span Position { get; set; }

        ///// <summary>
        ///// List of generic parameters.
        ///// </summary>
        //public TypeRef[]/*!*/ GenericParams
        //{
        //    get
        //    {
        //        return this.GetProperty<TypeRef[]>() ?? EmptyArray<TypeRef>.Instance;
        //    }
        //    set
        //    {
        //        if (value != null && value.Length != 0)
        //        {
        //            this.SetProperty<TypeRef[]>(value);
        //        }
        //        else
        //        {
        //            this.Properties.RemoveProperty<TypeRef[]>();
        //        }
        //    }
        //}

        /// <summary>
        /// Initialize new instance of <see cref="CallSignature"/>.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="position">Signature position.</param>
        public CallSignature(IList<ActualParam> parameters, Text.Span position)
        {
            this.Parameters = (parameters ?? throw new ArgumentNullException(nameof(parameters))).AsArray();
            //this.GenericParams = (genericParams != null && genericParams.Count != 0) ? genericParams.AsArray() : null;
            this.Position = position;
        }
    }

    #endregion
}
