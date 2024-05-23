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

namespace Devsense.PHP.Syntax.Ast
{
    public interface ILiteral : IExpression
    {

    }

    #region Literal

    /// <summary>
    /// Base class for literals.
    /// </summary>
    public abstract class Literal : Expression, ILiteral, IArrayItem
    {
        /// <summary>
        /// Gets internal value of literal.
        /// </summary>
        internal abstract object ValueObj { get; }

        /// <summary>
        /// Optional. Literal source code text.
        /// </summary>
        public string SourceText { get; set; }

        protected Literal(Text.Span span)
            : base(span)
        {
        }

        static NumberLiteralFlags DetermineNumberLiteralFlags(ReadOnlySpan<char> sourceText)
        {
            var flags = NumberLiteralFlags.Default;

            if (!sourceText.IsEmpty)
            {
                if (sourceText.Length > 1 && sourceText[0] == '0')
                {
                    // 0b
                    if (sourceText[1] == 'b') flags |= NumberLiteralFlags.Binary;
                    // 0x
                    else if (sourceText[1] == 'x') flags |= NumberLiteralFlags.Hexadecimal;
                    // 0o
                    else if (sourceText[1] == 'o') flags |= NumberLiteralFlags.OctalExplicit;
                    // 0...
                    else if (sourceText[1] != '_') flags |= NumberLiteralFlags.Octal; // 0123 -> octal format
                }

                if (sourceText.IndexOfAny('e', 'E') > 0) flags |= NumberLiteralFlags.HasExp;
                if (sourceText.IndexOf('_') >= 0) flags |= NumberLiteralFlags.HasUnderscores;
            }

            return flags;
        }

        /// <summary>
        /// Creates <see cref="Literal"/> instance for given <paramref name="value"/>.
        /// </summary>
        public static Literal Create(Text.Span span, long value, ReadOnlySpan<char> sourceText = default)
        {
            return new LongIntLiteral(span, value, DetermineNumberLiteralFlags(sourceText));
        }

        /// <summary>
        /// Creates <see cref="Literal"/> instance for given <paramref name="value"/>.
        /// </summary>
        public static Literal Create(Text.Span span, double value, ReadOnlySpan<char> sourceText = default)
        {
            return new DoubleLiteral(span, value, DetermineNumberLiteralFlags(sourceText) | NumberLiteralFlags.FloatingPointNumber);
        }

        /// <summary>
        /// Creates <see cref="Literal"/> instance for given <paramref name="value"/>.
        /// </summary>
        /// <param name="span">Element span.</param>
        /// <param name="value">Value of the literal.</param>
        /// <param name="sourceText">Optional original literal source text to determine additional features.</param>
        /// <returns>Instance of the <see cref="Literal"/> containing <paramref name="value"/>.</returns>
        public static Literal Create(Text.Span span, object value, ReadOnlySpan<char> sourceText = default)
        {
            if (ReferenceEquals(value, null))
            {
                return new NullLiteral(span);
            }

            if (value is long l)
            {
                return Create(span, l, sourceText);
            }

            if (value is int i)
            {
                return Create(span, (long)i, sourceText);
            }

            if (value is double d)
            {
                return Create(span, d, sourceText);
            }

            if (value is bool b)
            {
                return new BoolLiteral(span, b);
            }

            if (value is string str)
            {
                return new StringLiteral(span, str);
            }

            if (value is IStringLiteralValue strvalue)
            {
                if (strvalue.Contains8bitText)
                {
                    return new StringLiteral(span, strvalue);
                }

                return new StringLiteral(span, strvalue.ToString());
            }

            if (value is byte[] barr)
            {
                return new BinaryStringLiteral(span, barr);
            }

            //
            throw new ArgumentException();
        }

        #region IArrayItem // to be as a nested array item itself, without encapsulating to Item class

        bool IArrayItem.IsByRef => false;
        bool IArrayItem.IsSpreadItem => false;
        IExpression IArrayItem.Index => null;
        IExpression IArrayItem.Value => this;

        #endregion
    }

    #endregion

    #region NumberLiteralFlags

    [Flags]
    public enum NumberLiteralFlags
    {
        Default = 0, // decimal plain number

        FloatingPointNumber = 1, // float number

        Binary = 2, // binary format, i.e. 0b100
        Octal = 4, // octal format, i.e. 0123
        OctalExplicit = Octal | 8, // new octal format, 0o123
        Hexadecimal = 16, // hexadecimal format, i.e. 0x123

        HasUnderscores = 32, // contains PHP7.4 underscores, i.e. 1_234
        HasExp = 64, // in form of 1e2
    }

    #endregion

    #region IntLiteral

    ///// <summary>
    ///// Integer literal.
    ///// </summary>
    //   public sealed class IntLiteral : Literal
    //{
    //       public override Operations Operation { get { return Operations.IntLiteral; } }

    //       /// <summary>
    //       /// Gets internal value of literal.
    //       /// </summary>
    //       internal override object ValueObj { get { return this.value; } }

    //	/// <summary>
    //	/// Gets a value of the literal.
    //	/// </summary>
    //       public int Value { get { return value; } }
    //       private int value;

    //	/// <summary>
    //	/// Initializes a new instance of the IntLiteral class.
    //	/// </summary>
    //	public IntLiteral(Text.Span span, int value)
    //           : base(span)
    //	{
    //		this.value = value;
    //	}

    //	/// <summary>
    //       /// Call the right Visit* method on the given Visitor object.
    //       /// </summary>
    //       /// <param name="visitor">Visitor to be called.</param>
    //       public override void VisitMe(TreeVisitor visitor)
    //       {
    //           visitor.VisitIntLiteral(this);
    //       }
    //}

    #endregion

    #region LongIntLiteral

    /// <summary>
    /// Integer literal.
    /// </summary>
    public sealed class LongIntLiteral : Literal
    {
        /// <summary>
        /// Original number format.
        /// </summary>
        public NumberLiteralFlags Flags { get; }

        public override Operations Operation => Operations.LongIntLiteral;

        /// <summary>
        /// Gets internal value of literal.
        /// </summary>
        internal override object ValueObj => this.Value;

        /// <summary>
        /// Gets a value of the literal.
        /// </summary>
        public long Value { get; }

        /// <summary>
        /// Initializes a new instance of the IntLiteral class.
        /// </summary>
        public LongIntLiteral(Text.Span span, long value, NumberLiteralFlags flags)
            : base(span)
        {
            this.Value = value;
            this.Flags = flags;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitLongIntLiteral(this);
        }
    }

    #endregion

    #region DoubleLiteral

    /// <summary>
    /// Double literal.
    /// </summary>
    public sealed class DoubleLiteral : Literal
    {
        public NumberLiteralFlags Flags { get; }

        public override Operations Operation { get { return Operations.DoubleLiteral; } }

        /// <summary>
        /// Gets internal value of literal.
        /// </summary>
        internal override object ValueObj => this.Value;

        /// <summary>
        /// Gets a value of the literal.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Initializes a new instance of the DoubleLiteral class.
        /// </summary>
        /// <param name="p">A position.</param>
        /// <param name="value">A double value to be stored in node.</param>
        /// <param name="flags">Literal format.</param>
        public DoubleLiteral(Text.Span p, double value, NumberLiteralFlags flags)
            : base(p)
        {
            this.Value = value;
            this.Flags = flags;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDoubleLiteral(this);
        }
    }

    #endregion

    #region IStringLiteralValue

    public interface IStringLiteralValue
    {
        /// <summary>
        /// Gets value indicating the string contains one or more 8bit characters that should not be encoded into UTF-16.
        /// </summary>
        bool Contains8bitText { get; }

        /// <summary>
        /// Gets the <see cref="System.String"/> representation of the value, if possible.
        /// </summary>
        string ToString();

        /// <summary>
        /// Gets the 8bit representation of the value, if possible.
        /// </summary>
        byte[] ToBytes();

        /// <summary>
        /// Gets enumeration of underlying chunks of <see cref="System.String"/> or <see cref="byte"/>[].
        /// </summary>
        IEnumerable<object> EnumerateChunks();
    }

    #endregion

    #region StringLiteral

    /// <summary>
    /// String literal.
    /// </summary>
    public sealed class StringLiteral : Literal, IStringLiteralValue
    {
        public override Operations Operation { get { return Operations.StringLiteral; } }

        /// <summary>
        /// Gets internal value of literal.
        /// </summary>
        internal override object ValueObj { get { return this.Value; } }

        /// <summary>
        /// Underlying value, either <see cref="System.String"/> or <see cref="IStringLiteralValue"/>.
        /// </summary>
        readonly object _RawValue;

        /// <summary>
        /// A value of the literal as <see cref="System.String"/>.
        /// </summary>
        public string Value => _RawValue switch
        {
            string str => str,
            IStringLiteralValue value => value.ToString(),
            _ => throw new InvalidOperationException(),
        };

        /// <summary>
        /// Initializes a new instance of the StringLiteral class.
        /// </summary>
        public StringLiteral(Text.Span span, string value)
            : base(span)
        {
            _RawValue = value ?? throw new ArgumentNullException(nameof(value));
        }

        internal StringLiteral(Text.Span span, IStringLiteralValue value)
            : base(span)
        {
            _RawValue = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitStringLiteral(this);
        }

        #region IStringLiteralValue

        bool IStringLiteralValue.Contains8bitText => _RawValue is IStringLiteralValue value && value.Contains8bitText;

        byte[] IStringLiteralValue.ToBytes() => _RawValue is IStringLiteralValue value ? value.ToBytes() : throw new NotSupportedException();

        string IStringLiteralValue.ToString() => Value;

        /// <summary>
        /// Gets enumeration of underlying chunks of <see cref="System.String"/> or <see cref="byte"/>[].
        /// </summary>
        IEnumerable<object> IStringLiteralValue.EnumerateChunks() => _RawValue is IStringLiteralValue value
            ? value.EnumerateChunks()
            : new object[] { Value };

        #endregion
    }

    #endregion

    #region BinaryStringLiteral

    /// <summary>
    /// String literal.
    /// </summary>
    public sealed class BinaryStringLiteral : Literal, IStringLiteralValue
    {
        public override Operations Operation { get { return Operations.BinaryStringLiteral; } }

        /// <summary>
        /// Gets internal value of literal.
        /// </summary>
        internal override object ValueObj { get { return this.Value; } }

        /// <summary>
        /// A value of the literal.
        /// </summary>
        public byte[] Value { get; }

        /// <summary>
        /// Initializes a new instance of the StringLiteral class.
        /// </summary>
        public BinaryStringLiteral(Text.Span span, byte[]/*!*/ value)
            : base(span)
        {
            this.Value = value;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitBinaryStringLiteral(this);
        }

        #region IStringLiteralValue

        bool IStringLiteralValue.Contains8bitText => true;

        byte[] IStringLiteralValue.ToBytes() => Value;

        string IStringLiteralValue.ToString() => throw new NotSupportedException();

        /// <summary>
        /// Gets enumeration of underlying chunks of <see cref="System.String"/> or <see cref="byte"/>[].
        /// </summary>
        IEnumerable<object> IStringLiteralValue.EnumerateChunks() => new object[] { Value };

        #endregion
    }

    #endregion

    #region BoolLiteral

    /// <summary>
    /// Boolean literal.
    /// </summary>
    public sealed class BoolLiteral : Literal
    {
        public override Operations Operation { get { return Operations.BoolLiteral; } }

        /// <summary>
        /// Gets internal value of literal.
        /// </summary>
        internal override object ValueObj { get { return this.value; } }

        /// <summary>
        /// Gets a value of the literal.
        /// </summary>
        public bool Value { get { return value; } }
        private bool value;

        public BoolLiteral(Text.Span span, bool value)
            : base(span)
        {
            this.value = value;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitBoolLiteral(this);
        }
    }

    #endregion

    #region NullLiteral

    /// <summary>
    /// Null literal.
    /// </summary>
    public sealed class NullLiteral : Literal
    {
        public override Operations Operation { get { return Operations.NullLiteral; } }

        /// <summary>
        /// Gets internal value of literal.
        /// </summary>
        internal override object ValueObj { get { return null; } }

        public NullLiteral(Text.Span span)
            : base(span)
        {
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitNullLiteral(this);
        }
    }

    #endregion
}
