﻿// Copyright(c) DEVSENSE s.r.o.
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
using System.Linq;

using Devsense.PHP.Syntax;
using Devsense.PHP.Utilities;

namespace Devsense.PHP.Text
{
    #region ILineBreaks

    /// <summary>
    /// Manages information about line breaks in the document.
    /// </summary>
    public interface ILineBreaks
    {
        /// <summary>
        /// Gets amount of line breaks.
        /// </summary>
        /// <remarks>Lines count equals <see cref="Count"/> + <c>1</c>.</remarks>
        int Count { get; }

        /// <summary>
        /// Gets length of document.
        /// </summary>
        int TextLength { get; }

        /// <summary>
        /// Gets position of <paramref name="index"/>-th line end, including its break characters.
        /// </summary>
        /// <param name="index">Index of te line.</param>
        /// <returns>Position of the line end.</returns>
        int EndOfLineBreak(int index);

        /// <summary>
        /// Resolve the line at given position.
        /// </summary>
        /// <param name="position">Index of character within the document.</param>
        /// <param name="line">Line number, zero-based.</param>
        /// <returns>Value indicating the line was resolved, i.e. the <paramref name="position"/> is in valid range.</returns>
        bool TryGetLineAtPosition(int position, out int line);

        // /// <summary>
        // /// Gets line number from <paramref name="position"/> within document.
        // /// </summary>
        // /// <param name="position">Position within document.</param>
        // /// <returns>Line number.</returns>
        // /// <exception cref="ArgumentOutOfRangeException">In case <paramref name="position"/> is out of line number range.</exception>
        // int GetLineFromPosition(int position);
        //
        // /// <summary>
        // /// Gets line and column from position number.
        // /// </summary>
        // /// <param name="position">Position with the document.</param>
        // /// <param name="line">Line number.</param>
        // /// <param name="column">Column nummber.</param>
        // void GetLineColumnFromPosition(int position, out int line, out int column);
    }

    #endregion

    #region LineBreaks

    public abstract class LineBreaks : ILineBreaks
    {
        #region ILineBreaks Members

        public abstract int Count { get; }

        public abstract int EndOfLineBreak(int index);

        public int TextLength
        {
            get { return _textLength; }
        }

        public bool TryGetLineAtPosition(int position, out int line)
        {
            if (position < 0 || position > this.TextLength)
            {
                // invalid
                line = 0;
                return false;
            }

            if (position < this.TextLength)
            {
                // binary search line
                int a = 0;
                int b = this.Count;
                while (a < b)
                {
                    int x = (a + b) / 2;
                    if (position < this.EndOfLineBreak(x))
                        b = x;
                    else
                        a = x + 1;
                }

                line = a;
            }
            else
            {
                // last line
                line = this.LinesCount - 1;
            }

            return true;
        }

        #endregion

        protected int _textLength;

        protected LineBreaks(int textLength)
        {
            _textLength = textLength;
        }

        public static LineBreaks/*!*/Create(string text)
        {
            var lineends = ListObjectPool<int>.Allocate();
            CalculateLineEnds(lineends, text);

            var linebreaks = Create(text, lineends);

            ListObjectPool<int>.Free(lineends);

            //
            return linebreaks;
        }

        public static LineBreaks/*!*/Create(string text, List<int>/*!*/lineEnds)
        {
            if (text == null) throw new ArgumentNullException();
            return Create(text.Length, lineEnds);
        }

        internal static LineBreaks/*!*/Create(int textLength, List<int>/*!*/lineEnds)
        {
            if (textLength < 0) throw new ArgumentException();
            if (lineEnds == null) throw new ArgumentNullException();

            if (lineEnds.Count == 0 || lineEnds.Last() <= ushort.MaxValue)
            {
                return new ShortLineBreaks(textLength, lineEnds);
            }
            else
            {
                return new IntLineBreaks(textLength, lineEnds);
            }
        }

        /// <summary>
        /// Amount of lines in the document.
        /// </summary>
        public int LinesCount { get { return this.Count + 1; } }

        /// <summary>
        /// Gets list of line ends position.
        /// </summary>
        /// <param name="list">Where to put the position of line ends.</param>
        /// <param name="text">Document text.</param>
        /// <returns>List of line ends position.</returns>
        private static void CalculateLineEnds(List<int> list, string text)
        {
            if (text != null)
            {
                int offset = 0;
                while (offset < text.Length)
                {
                    int idx = TextUtils.IndexOfLineBreak(text.AsSpan(offset), out var eol_length);
                    if (idx >= 0)
                    {
                        offset = offset + idx + eol_length;
                        list.Add(offset);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    #endregion

    #region ShortLineBreaks

    /// <summary>
    /// Optimized generalization of <see cref="LineBreaks"/> using <see cref="ushort"/> internally.
    /// </summary>
    internal sealed class ShortLineBreaks : LineBreaks
    {
        readonly ushort[]/*!*/_lineEnds;

        public ShortLineBreaks(int textLength, List<int> lineEnds)
            : base(textLength)
        {
            var count = lineEnds.Count;
            if (count == 0)
            {
                _lineEnds = ArrayUtils.EmptyUShorts;
            }
            else
            {
                var lineends = _lineEnds = new ushort[count];
                for (int i = 0; i < lineends.Length; i++)
                {
                    lineends[i] = unchecked((ushort)lineEnds[i]);
                }
            }
        }

        public override int Count => _lineEnds.Length;

        public override int EndOfLineBreak(int index) => (int)_lineEnds[index];
    }

    #endregion

    #region IntLineBreaks

    /// <summary>
    /// Generalization of <see cref="LineBreaks"/> using <see cref="int"/> internally.
    /// </summary>
    internal sealed class IntLineBreaks : LineBreaks
    {
        private readonly int[]/*!*/_lineEnds;

        public IntLineBreaks(int textLength, List<int> lineEnds)
            : base(textLength)
        {
            var count = lineEnds.Count;
            if (count == 0)
            {
                _lineEnds = ArrayUtils.EmptyIntegers;
            }
            else
            {
                _lineEnds = lineEnds.ToArray();
            }
        }

        public override int Count => _lineEnds.Length;

        public override int EndOfLineBreak(int index) => _lineEnds[index];
    }

    #endregion

    #region ExpandableLineBreaks

    /// <summary>
    /// Generalization of <see cref="LineBreaks"/> using <see cref="List{T}"/> internally.
    /// </summary>
    internal sealed class ExpandableLineBreaks : LineBreaks
    {
        private readonly List<int>/*!*/_lineEnds = new List<int>();

        public ExpandableLineBreaks()
            : base(0)
        {
        }

        public override int Count
        {
            get { return _lineEnds.Count; }
        }

        public override int EndOfLineBreak(int index)
        {
            return (int)_lineEnds[index];
        }

        /// <summary>
        /// Adds line breaks from <paramref name="text"/>[from...from+length].
        /// </summary>
        public void Expand(char[] text, int from, int length)
        {
            var source = text.AsSpan(from, length);
            var offset = _textLength;
            while (source.IsEmpty == false)
            {
                var idx = TextUtils.IndexOfLineBreak(source, out var eol_length);
                if (idx < 0)
                {
                    break;
                }

                _lineEnds.Add(offset + idx);

                source = source.Slice(idx + eol_length);
                offset += idx + eol_length;
            }

            //
            _textLength += length;
        }

        public LineBreaks /*!*/ ToImmutable()
        {
            return LineBreaks.Create(_textLength, _lineEnds);
        }
    }

    #endregion

    //#region VirtualLineBreaks

    ///// <summary>
    ///// <see cref="ILineBreaks"/> implementation which is collecting line break information subsequently
    ///// and provides ability to shift resulting line and column.
    ///// </summary>
    //internal sealed class VirtualLineBreaks : ILineBreaks
    //{
    //    private readonly int lineShift, columnShift;
    //    private LineBreaks/*!*/lineBreaks;
    //    private ExpandableLineBreaks ExpandableLineBreaks { get { return (ExpandableLineBreaks)lineBreaks; } }

    //    public VirtualLineBreaks(LineBreaks lineBreaks, int lineShift, int columnShift)
    //    {
    //        this.lineShift = lineShift;
    //        this.columnShift = columnShift;
    //        this.lineBreaks = lineBreaks;
    //    }

    //    public VirtualLineBreaks(int lineShift, int columnShift)
    //        : this(new ExpandableLineBreaks(), lineShift, columnShift)
    //    {
    //    }

    //    /// <summary>
    //    /// Updates <see cref="TextLength"/> and line breaks with an additional piece of text.
    //    /// </summary>
    //    public void Expand(char[] text, int from, int length)
    //    {
    //        if (IsFinalized)
    //            throw new InvalidOperationException();

    //        this.ExpandableLineBreaks.Expand(text, from, length);
    //    }

    //    /// <summary>
    //    /// Compresses internal storage of line breaks and does not allow to expand any more.
    //    /// </summary>
    //    public ILineBreaks Finalize()
    //    {
    //        if (!IsFinalized)
    //            lineBreaks = this.ExpandableLineBreaks.Finalize();

    //        if (lineShift == 0 && columnShift == 0)
    //            return lineBreaks;
    //        else
    //            return this;
    //    }

    //    public bool IsFinalized { get { return !(lineBreaks is ExpandableLineBreaks); } }

    //    #region ILineBreaks Members

    //    public int Count
    //    {
    //        get { return lineBreaks.Count; }
    //    }

    //    public int TextLength
    //    {
    //        get { return lineBreaks.TextLength; }
    //    }

    //    public int EndOfLineBreak(int index)
    //    {
    //        return lineBreaks.EndOfLineBreak(index);
    //    }

    //    public int GetLineFromPosition(int position)
    //    {
    //        return lineBreaks.GetLineFromPosition(position) + lineShift;
    //    }

    //    public void GetLineColumnFromPosition(int position, out int line, out int column)
    //    {
    //        lineBreaks.GetLineColumnFromPosition(position, out line, out column);

    //        if (line == 0) column += columnShift;
    //        line += lineShift;
    //    }

    //    #endregion
    //}

    //#endregion
}
