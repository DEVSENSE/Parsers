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

using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax
{
    #region VariableNameRef

    /// <summary>
    /// Represents a variable name and its position within AST.
    /// </summary>
    [DebuggerDisplay("{_span,nq} {_name}")]
    public struct VariableNameRef : IEquatable<VariableName>, IEquatable<string>
    {
        /// <summary>
        /// Position of the name.
        /// </summary>
        public Span Span => HasValue ? new Span(_span_start, Name.Value.Length + 1) : Span.Invalid;

        /// <summary>
        /// Variable name.
        /// </summary>
        public VariableName Name { get; }

        /// <summary>
        /// Position of the <c>$variable</c> in source code.
        /// </summary>
        private readonly int _span_start;

        /// <summary>
        /// Gets value indicating the value is not empty.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(Name.Value);

        public VariableNameRef(Span span, string name)
            : this(span.Start, new VariableName(name))
        {
        }

        public VariableNameRef(Span span, VariableName name)
            : this(span.Start, name)
        {
        }

        public VariableNameRef(int start, VariableName name)
        {
            _span_start = start;
            this.Name = name;
        }

        public override string ToString() => this.Name.ToString();

        public static implicit operator VariableName(VariableNameRef self) => self.Name;

        public override bool Equals(object obj) =>
            obj is VariableName name ? Equals(name) :
            obj is VariableNameRef nameref ? Equals(nameref) :
            obj is string str ? Equals(str) :
            false;

        public override int GetHashCode() => this.Name.GetHashCode();

        #region IEquatable<VariableName>

        public bool Equals(VariableName other) => this.Name.Equals(other);

        public static bool operator ==(VariableNameRef name, VariableName other) => name.Equals(other);

        public static bool operator !=(VariableNameRef name, VariableName other) => !name.Equals(other);

        #endregion

        #region IEquatable<string>

        public bool Equals(string other) => this.Name.Equals(other);

        public static bool operator ==(VariableNameRef name, string other) => name.Equals(other);

        public static bool operator !=(VariableNameRef name, string other) => !name.Equals(other);

        #endregion
    }

    #endregion

    #region NameRef

    /// <summary>
    /// Represents a variable name and its position.
    /// </summary>
    [DebuggerDisplay("{_span,nq} {_name}")]
    public struct NameRef
    {
        private readonly Span _span;
        private readonly Name _name;

        /// <summary>
        /// Position of the name.
        /// </summary>
        public Span Span => _span;

        /// <summary>
        /// Variable name.
        /// </summary>
        public Name Name => _name;

        /// <summary>
        /// Gets value indicating the name is not empty.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(_name.Value);

        /// <summary>
        /// An empty name.
        /// </summary>
        public static NameRef Invalid => new NameRef(Span.Invalid, Name.EmptyBaseName);

        public NameRef(Span span, string name)
            : this(span, new Name(name))
        {
        }

        public NameRef(Span span, Name name)
        {
            _span = span;
            _name = name;
        }

        /// <summary>
        /// Gets <see cref="Name"/> as <see cref="string"/>.
        /// </summary>
        public override string ToString() => _name.ToString();

        public static implicit operator Name(NameRef self) => self.Name;
    }

    #endregion

    #region QualifiedNameRef

    /// <summary>
    /// Represents a qualified name and its position.
    /// </summary>
    [DebuggerDisplay("{_span,nq} {_name}")]
    public struct QualifiedNameRef
    {
        private readonly Span _span;
        private readonly QualifiedName _name;

        /// <summary>
        /// Position of the qualified name.
        /// </summary>
        public Span Span => _span;

        /// <summary>
        /// Qualified name.
        /// </summary>
        public QualifiedName QualifiedName => _name;

        /// <summary>
        /// Gets value indicating the qualified name is not empty.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(_name.Name.Value) || (_name.Namespaces != null && _name.Namespaces.Length != 0);

        internal static QualifiedNameRef FromTypeRef(TypeRef tref)
        {
            if (tref == null)
            {
                return QualifiedNameRef.Invalid;
            }
            var qname = tref.QualifiedName;
            if (qname.HasValue)
                return new QualifiedNameRef(tref.Span, qname.Value);
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// Empty name.
        /// </summary>
        public static QualifiedNameRef Invalid => new QualifiedNameRef(Span.Invalid, Syntax.Name.EmptyBaseName, Syntax.Name.EmptyNames);

        public QualifiedNameRef(Span span, Name name, Name[] namespaces, bool fullyQualified = false)
            : this(span, new QualifiedName(name, namespaces, fullyQualified))
        {
        }

        public QualifiedNameRef(Span span, QualifiedName name)
        {
            _span = span;
            _name = name;
        }

        /// <summary>
        /// Gets <see cref="QualifiedName"/> as <see cref="string"/>.
        /// </summary>
        public override string ToString() => _name.ToString();

        public static implicit operator QualifiedName(QualifiedNameRef self) => self.QualifiedName;
    }

    #endregion
}
