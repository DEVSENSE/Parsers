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
    public readonly struct ActualParam : ITreeNode
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
            /// Flags annotating a parameter placeholder.
            /// PHP 8.6.
            /// </summary>
            PlaceholderArg = 4,
            /// <summary>
            /// Flag annotating the "..." special argument making making the containing call converted to a closure.
            /// Introduced in PHP 8.1: https://wiki.php.net/rfc/first_class_callable_syntax
            /// </summary>
            PlaceholderVariadic = 8,

            /// <summary>
            /// Mask indicating a placeholder argument.
            /// </summary>
            IsPlaceholder = PlaceholderArg | PlaceholderVariadic,

            IsError = 16,
        }

        /// <summary>
        /// Either <see cref="Expression"/> or <see cref="Tuple{VariableNameRef, Expression}"/>.
        /// </summary>
        readonly object _obj;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression
        {
            get
            {
                if (_obj is Expression expr) return expr;
                if (_obj is NameTuple t) return t.Item2;
                return null;
            }
        }

        /// <summary>
        /// Named argument, if specified.
        /// </summary>
        public VariableNameRef Name
        {
            get
            {
                if (_obj is NameTuple t) return t.Item1;
                return default(VariableNameRef);
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
                var name = this.Name;

                var from = name.HasValue ? name.Span.Start : _start;
                if (from >= 0)
                {
                    var expression = this.Expression;
                    var end = from;

                    if (expression != null)
                    {
                        // <_start><expr>
                        // <_start>...<expr>
                        end = expression.Span.End;
                    }
                    else if (PlaceholderFlag == Flags.PlaceholderArg)
                    {
                        // <_start>?
                        end = _start + 1;
                    }
                    else if (PlaceholderFlag == Flags.PlaceholderVariadic)
                    {
                        // <_start>...
                        end = _start + 3;
                    }
                    else if (name.HasValue)
                    {
                        // name   :<_start>
                        end = Math.Max(_start, name.Span.End + 1);
                    }

                    //
                    return Span.FromBounds(from, end);
                }
                return Span.Invalid;
            }
        }

        /// <summary>
        /// Starting index of the range.
        /// </summary>
        /// <remarks>Set in the constructor and immutable thereafter.</remarks>
        readonly int _start;

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

        public Flags PlaceholderFlag => _flags & Flags.IsPlaceholder;

        /// <summary>
        /// Gets value indicating it's a named argument.
        /// </summary>
        public bool IsNamedArgument => Name.HasValue;

        internal void Compress(out object obj1, out int start, out byte flags)
        {
            obj1 = _obj;
            start = _start;
            flags = (byte)_flags;
        }

        internal ActualParam(object obj, int start, byte flags)
        {
            _obj = obj;
            _start = start;
            _flags = (Flags)flags;
        }

        public ActualParam(Span p, Expression param)
            : this(p, param, Flags.Default, default)
        { }

        public ActualParam(Span p, Expression param, Flags flags, VariableNameRef nameOpt = default)
        {
            _flags = flags;
            _start = p.Start;
            _obj = nameOpt.HasValue
                ? (object)new NameTuple(nameOpt, param)
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

        public override string ToString()
        {
            var name = this.Name;
            var prefix = name.HasValue ? $"{name.Name.Value}:" : string.Empty;

            var expression = this.Expression;
            var str =
                expression != null ? $"<{expression.GetType().Name}>" :
                PlaceholderFlag == Flags.PlaceholderArg ? "?" :
                PlaceholderFlag == Flags.PlaceholderVariadic ? "..." :
                string.Empty
                ;

            if (Ampersand) str = "&" + str;
            if (IsUnpack) str = "..." + str;

            //
            return prefix + str;
        }
    }

    #endregion

    #region CallSignature

    public readonly struct CallSignature
    {
        /// <summary>
        /// Empty non-existent signature.
        /// </summary>
        public static CallSignature Empty => new CallSignature(ActualParam.EmptyArray, Text.Span.Invalid);

        /// <summary>
        /// Gets value indicating the object is treated like a conversion of the containing call to a Closure.
        /// Introduced in PHP 8.1: https://wiki.php.net/rfc/first_class_callable_syntax
        /// </summary>
        public bool IsCallableConvert => (_packed_span_length & _packed_callableconvert_flag) != 0;

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
        public Text.Span Span
        {
            get
            {
                var span_length = _span_length;
                return span_length >= 0 ? new Text.Span(_span_start, span_length) : Text.Span.Invalid;
            }
        }

        readonly int _span_start;

        int _span_length => _packed_span_length & _packed_span_length_mask;

        readonly int _packed_span_length;

        const int _packed_callableconvert_flag = 1 << 29;

        const int _packed_span_length_mask = ~_packed_callableconvert_flag;

        /// <summary>
        /// Initialize new instance of <see cref="CallSignature"/>.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="span">Signature position.</param>
        public CallSignature(ActualParam[] parameters, Text.Span span)
        {
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            _span_start = span.Start;
            _packed_span_length = span.Length & _packed_span_length_mask;

            // flip the sign on the _packed_span_start (even it's 0) if IsPlaceholder(parameters)
            if (parameters.HavePlaceholder())
            {
                _packed_span_length |= _packed_callableconvert_flag;
            }

            Debug.Assert(this.Span.Equals(span));
        }
    }

    #endregion

    public static class CallSignatureExtensions
    {
        /// <summary>
        /// Flag annotating the "..." or "?" special argument making making the containing call converted to a closure.
        /// </summary>
        public static bool IsPlaceholder(this ActualParam p) => p.PlaceholderFlag != 0;

        public static bool HavePlaceholder(this ActualParam[] ps)
        {
            if (ps != null)
            {
                for (int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].IsPlaceholder()) return true;
                }
            }
            return false;
        }
    }
}
