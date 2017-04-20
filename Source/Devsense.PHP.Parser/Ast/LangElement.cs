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
using Devsense.PHP.Text;
using System.Collections.Generic;

namespace Devsense.PHP.Syntax.Ast
{
    public interface IAstNode
    {

    }

    /// <summary>
    /// Base class for all AST nodes.
    /// </summary>
    public abstract class AstNode : IAstNode, IPropertyCollection
    {
        #region Fields & Properties

        /// <summary>
        /// Contains properties of this <see cref="AstNode"/>.
        /// </summary>
        private PropertyCollection _properties;

        /// <summary>
        /// Gets property collection associated with this node.
        /// </summary>
        public IPropertyCollection Properties { get { return (IPropertyCollection)this; } }

        #endregion

        #region IPropertyCollection

        void IPropertyCollection.SetProperty(object key, object value)
        {
            lock (this)
            {
                _properties.SetProperty(key, value);
            }
        }

        object IPropertyCollection.GetProperty(object key)
        {
            lock (this)
            {
                return _properties.GetProperty(key);
            }
        }

        public void SetProperty<T>(T value)
        {
            lock (this)
            {
                _properties.SetProperty<T>(value);
            }
        }

        public T GetProperty<T>()
        {
            lock (this)
            {
                return _properties.GetProperty<T>();
            }
        }

        bool IPropertyCollection.TryGetProperty(object key, out object value)
        {
            lock (this)
            {
                return _properties.TryGetProperty(key, out value);
            }
        }

        bool IPropertyCollection.TryGetProperty<T>(out T value)
        {
            lock (this)
            {
                return _properties.TryGetProperty<T>(out value);
            }
        }

        bool IPropertyCollection.RemoveProperty(object key)
        {
            lock (this)
            {
                return _properties.RemoveProperty(key);
            }
        }

        bool IPropertyCollection.RemoveProperty<T>()
        {
            lock (this)
            {
                return _properties.RemoveProperty<T>();
            }
        }

        void IPropertyCollection.ClearProperties()
        {
            lock (this)
            {
                _properties.ClearProperties();
            }
        }

        object IPropertyCollection.this[object key]
        {
            get
            {
                return ((IPropertyCollection)this).GetProperty(key);
            }
            set
            {
                ((IPropertyCollection)this).SetProperty(key, value);
            }
        }

        #endregion
    }

    /// <summary>
	/// Base class for all AST nodes representing PHP language Elements - statements and expressions.
	/// </summary>
	public abstract class LangElement : AstNode
    {
        /// <summary>
        /// Immutable empty list of <see cref="LangElement"/>.
        /// </summary>
        internal static readonly List<LangElement>/*!*/EmptyList = new List<LangElement>();

        #region ContainingElement

        /// <summary>
        /// Gets the parent symbol in the AST hierarchy.
        /// </summary>
        public virtual LangElement ContainingElement { get; set; }

        /// <summary>
        /// Gets reference to containing namespace scope or <c>null</c> in case of global namespace.
        /// </summary>
        public NamespaceDecl ContainingNamespace => LookupContainingElement<NamespaceDecl>();

        /// <summary>
        /// Gets reference to containing type declaration.
        /// </summary>
        public TypeDecl ContainingType => ContainingElement.LookupContainingElement<TypeDecl>();

        /// <summary>
        /// Gets reference to containing source unit.
        /// </summary>
        public virtual SourceUnit ContainingSourceUnit => LookupContainingElement<GlobalCode>()?.ContainingSourceUnit;

        /// <summary>
        /// Iterates through containing elements to find closest element of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of element to look for.</typeparam>
        /// <returns>Reference to element of type <typeparamref name="T"/> or <c>null</c> is not containing.</returns>
        protected T LookupContainingElement<T>() where T : LangElement
        {
            for (var x = this; x != null; x = x.ContainingElement)
            {
                if (x is T)
                {
                    return (T)x;
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Position of element in source file.
        /// </summary>
        public Span Span { get; protected set; }
		
		/// <summary>
        /// Initialize the LangElement.
        /// </summary>
        /// <param name="span">The position of the LangElement in the source code.</param>
		protected LangElement(Span span)
		{
			this.Span = span;
		}

        /// <summary>
        /// In derived classes, calls Visit* on the given visitor object.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void VisitMe(TreeVisitor/*!*/visitor);
	}

    #region Scope

    public struct Scope
    {
        public int Start { get { return start; } }
        private int start;

        public static readonly Scope Invalid = new Scope(-1);
        public static readonly Scope Global = new Scope(0);
        public static readonly Scope Ignore = new Scope(Int32.MaxValue);

        public bool IsGlobal
        {
            get
            {
                return start == 0;
            }
        }

        public bool IsValid
        {
            get
            {
                return start >= 0;
            }
        }

        public Scope(int start)
        {
            this.start = start;
        }

        public void Increment()
        {
            start++;
        }

        public void Decrement()
        {
            start--;
        }

        public override string ToString()
        {
            return start.ToString();
        }
    }

    #endregion
}