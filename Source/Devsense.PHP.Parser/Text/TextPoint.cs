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

namespace Devsense.PHP.Text
{
    /// <summary>
    /// Represents position within text.
    /// </summary>
    public struct TextPoint : IComparable<TextPoint>, IEquatable<TextPoint>
    {
        #region Fields

        private readonly ILineBreaks _lineBreaks;
        private readonly int _position;

        #endregion

        #region Construction

        public TextPoint(ILineBreaks lineBreaks, int position)
        {
            //if (lineBreaks == null)
            //    throw new ArgumentNullException("lineBreaks");
            //if (position > lineBreaks.TextLength)
            //    throw new ArgumentException("position");

            _lineBreaks = lineBreaks;
            _position = position;
        }

        #endregion

        #region Properties

        public ILineBreaks LineBreaks
        {
            get
            {
                return _lineBreaks;
            }
        }

        public int Position
        {
            get
            {
                return _position;
            }
        }

        public int Line
        {
            get
            {
                return _lineBreaks.GetLineFromPosition(_position);
            }
        }

        public int Column
        {
            get
            {
                int line, column;
                _lineBreaks.GetLineColumnFromPosition(_position, out line, out column);
                return column;
            }
        }

        #endregion

        #region Methods

        public static implicit operator int(TextPoint point)
        {
            return point.Position;
        }

        public override int GetHashCode()
        {
            return _position.GetHashCode() ^ _lineBreaks.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TextPoint)
                return this.Equals((TextPoint)obj);

            return false;
        }

        public TextPoint Add(int offset)
        {
            return new TextPoint(this.LineBreaks, _position + offset);
        }

        public TextPoint Subtract(int offset)
        {
            return this.Add(-offset);
        }

        public static TextPoint operator -(TextPoint point, int offset)
        {
            return point.Add(-offset);
        }

        public static int operator -(TextPoint start, TextPoint other)
        {
            if (start.LineBreaks != other.LineBreaks)
                throw new ArgumentException();
            
            return start.Position - other.Position;
        }

        public static bool operator ==(TextPoint left, TextPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextPoint left, TextPoint right)
        {
            return !(left == right);
        }

        public static TextPoint operator +(TextPoint point, int offset)
        {
            return point.Add(offset);
        }

        public static bool operator >(TextPoint left, TextPoint right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(TextPoint left, TextPoint right)
        {
            return left.CompareTo(right) < 0;
        }

        #endregion

        #region IComparable<TextPoint>

        public int CompareTo(TextPoint other)
        {
            if (this.LineBreaks != other.LineBreaks)
                throw new ArgumentException();

            return _position.CompareTo(other._position);
        }

        #endregion

        #region IEquatable<TextPoint> Members

        public bool Equals(TextPoint other)
        {
            return other.LineBreaks == this.LineBreaks && other.Position == this.Position;
        }

        #endregion
    }
}
