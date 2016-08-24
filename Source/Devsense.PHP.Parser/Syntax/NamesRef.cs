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
    public struct VariableNameRef
    {
        private readonly Span _span;
        private readonly VariableName _name;

        /// <summary>
        /// Position of the name.
        /// </summary>
        public Span Span => _span;

        /// <summary>
        /// Variable name.
        /// </summary>
        public VariableName Name => _name;

        public VariableNameRef(Span span, string name)
            : this(span, new VariableName(name))
        {
        }

        public VariableNameRef(Span span, VariableName name)
        {
            _span = span;
            _name = name;
        }

        public override string ToString() => _name.ToString();

        public static implicit operator VariableName(VariableNameRef self) => self.Name;
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
        /// Position of the name.
        /// </summary>
        public Span Span => _span;

        /// <summary>
        /// Variable name.
        /// </summary>
        public QualifiedName QualifiedName => _name;

        /// <summary>
        /// Gets value indicating the qualified name is not empty.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(_name.Name.Value) || (_name.Namespaces != null && _name.Namespaces.Length != 0);

        internal static QualifiedNameRef FromTypeRef(TypeRef tref)
        {
            if (tref is DirectTypeRef)
            {
                return new QualifiedNameRef(tref.Span, ((DirectTypeRef)tref).ClassName);
            }
            else if (tref == null)
            {
                return QualifiedNameRef.Invalid;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Empty name.
        /// </summary>
        public static QualifiedNameRef Invalid => new QualifiedNameRef(Span.Invalid, Syntax.Name.EmptyBaseName, Syntax.Name.EmptyNames);

        public QualifiedNameRef(Span span, Name name, Name[] namespaces)
            : this(span, new QualifiedName(name, namespaces))
        {
        }

        public QualifiedNameRef(Span span, QualifiedName name)
        {
            _span = span;
            _name = name;
        }

        public override string ToString() => _name.ToString();

        public static implicit operator QualifiedName(QualifiedNameRef self) => self.QualifiedName;
    }

    #endregion
}
