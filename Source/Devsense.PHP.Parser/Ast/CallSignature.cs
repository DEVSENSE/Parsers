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
using NameTuple = System.Tuple<Devsense.PHP.Syntax.VariableNameRef, Devsense.PHP.Syntax.Ast.Expression>;

namespace Devsense.PHP.Syntax.Ast
{
    #region ActualParam

    /// <summary>
    /// Represents a single argument passed to <see cref="FunctionCall"/>.
    /// </summary>
    public struct ActualParam : ITreeNode
    {
        /// <summary>
        /// <see cref="ArrayUtils.Empty{ActualParam}()"/>.
        /// </summary>
        public static ActualParam[] EmptyArray => ArrayUtils.Empty<ActualParam>();

        [Flags]
        public enum Flags : byte
        {
            Default = 0,
            IsByRef = 1,
            IsUnpack = 2,

            /// <summary>
            /// Flag annotating the "..." special argument making making the containing call converted to a closure.
            /// Introduced in PHP 8.1: https://wiki.php.net/rfc/first_class_callable_syntax
            /// </summary>
            IsCallableConvert = 4,
        }

        /// <summary>
        /// Either <see cref="Expression"/> or <see cref="Tuple{VariableNameRef, Expression}"/>.
        /// </summary>
        object _obj;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression
        {
            get
            {
                if (_obj is Expression expr) return expr;
                if (_obj is NameTuple t) return t.Item2;
                Debug.Assert(_obj == null);
                return null;
            }
        }

        /// <summary>
        /// Named argument, if specified.
        /// </summary>
        public VariableNameRef? Name
        {
            get
            {
                if (_obj is NameTuple t) return t.Item1;
                return null;
            }
        }

        /// <summary>
        /// Flags describing use of the parameter.
        /// </summary>
        readonly Flags _flags;

        /// <summary>
        /// The parameter span.
        /// </summary>
        public Span Span
        {
            get
            {
                if (_spanStart >= 0)
                {
                    if (Expression != null && Expression.Span.IsValid)
                    {
                        return Span.FromBounds(_spanStart, Expression.Span.End);
                    }

                    if (IsCallableConvert)
                    {
                        return new Span(_spanStart, 3); // "..."
                    }
                }
                return Span.Invalid;
            }
        }
        readonly int _spanStart;

        /// <summary>
        /// Gets value indicating the parameter is not empty (<see cref="Expression"/> is not a <c>null</c> reference).
        /// </summary>
        public bool Exists => Expression != null;

        /// <summary>
        /// Gets value indicating whether the parameter is prefixed by <c>&amp;</c> character.
        /// </summary>
        public bool Ampersand => (_flags & Flags.IsByRef) != 0;

        /// <summary>
        /// Gets value indicating whether the parameter is passed with <c>...</c> prefix and so it has to be unpacked before passing to the function call.
        /// </summary>
        public bool IsUnpack => (_flags & Flags.IsUnpack) != 0;

        /// <summary>
        /// Flag annotating the "..." special argument making making the containing call converted to a closure.
        /// </summary>
        public bool IsCallableConvert => (_flags & Flags.IsCallableConvert) != 0;

        /// <summary>
        /// Gets value indicating it's a named argument.
        /// </summary>
        public bool IsNamedArgument => Name.HasValue;

        internal void Compress(out object obj1, out int start, out byte flags)
        {
            obj1 = _obj;
            start = _spanStart;
            flags = (byte)_flags;
        }

        internal ActualParam(object obj, int start, byte flags)
        {
            _obj = obj;
            _spanStart = start;
            _flags = (Flags)flags;
        }

        public ActualParam(Span p, Expression param)
            : this(p, param, Flags.Default, default)
        { }

        public ActualParam(Span p, Expression param, Flags flags, VariableNameRef? nameOpt = default)
        {
            _flags = flags;
            _spanStart = p.Start;
            _obj = nameOpt.HasValue
                ? (object)new NameTuple(nameOpt.Value, param)
                : param;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitActualParam(this);
        }
    }

    #endregion

    #region CallSignature

    public struct CallSignature
    {
        /// <summary>
        /// Empty non-existent signature.
        /// </summary>
        public static CallSignature Empty => new CallSignature(ArrayUtils.Empty<ActualParam>(), Text.Span.Invalid);

        /// <summary>
        /// Creates a special signature that is treated like a conversion of the containing call to a Closure.
        /// Introduced in PHP 8.1: https://wiki.php.net/rfc/first_class_callable_syntax
        /// </summary>
        /// <param name="span">Span of the ellipsis '...'.</param>
        /// <returns>Single item list with parameter denoting it's a callable convert signature.</returns>
        public static ActualParam[] CreateCallableConvert(Text.Span span) => new ActualParam[] { new ActualParam(span, null, ActualParam.Flags.IsCallableConvert) };

        /// <summary>
        /// Gets value indicating the object is treated like a conversion of the containing call to a Closure.
        /// Introduced in PHP 8.1: https://wiki.php.net/rfc/first_class_callable_syntax
        /// </summary>
        public bool IsCallableConvert => Parameters != null && Parameters.Length == 1 && Parameters[0].IsCallableConvert;

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
        public Text.Span Span { get; set; }

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

        ///// <summary>
        ///// Initialize new instance of <see cref="CallSignature"/>.
        ///// </summary>
        ///// <param name="parameters">List of parameters.</param>
        ///// <param name="span">Signature position.</param>
        //public CallSignature(IList<ActualParam> parameters, Text.Span span) : this(parameters.AsArray(), span)
        //{
        //}

        /// <summary>
        /// Initialize new instance of <see cref="CallSignature"/>.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="span">Signature position.</param>
        public CallSignature(ActualParam[] parameters, Text.Span span)
        {
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.Span = span;
        }
    }

    #endregion
}
