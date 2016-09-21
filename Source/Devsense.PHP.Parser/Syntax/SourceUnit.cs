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
using System.Text;
using System.IO;

using Devsense.PHP.Text;
using Devsense.PHP.Syntax.Ast;

namespace Devsense.PHP.Syntax
{
    #region SourceUnit

    /// <summary>
    /// Represents single source document.
    /// </summary>
    public abstract class SourceUnit : ILineBreaks, IPropertyCollection
    {
        #region Fields & Properties

        /// <summary>
        /// Source file containing the unit. For evals, it can be even a non-php source file.
        /// Used for emitting debug information and error reporting.
        /// </summary>
        public string/*!*/ FilePath { get { return _filePath; } }
        readonly string/*!*/ _filePath;

        public GlobalCode Ast { get { return (GlobalCode)ast; } }
        protected AstNode ast;

        /// <summary>
        /// Set of object properties.
        /// </summary>
        private PropertyCollection innerProps;

        /// <summary>
        /// Gets line breaks for this source unit.
        /// </summary>
        /// <remarks>Line breaks are used to resolve line and column number from given position.</remarks>
        public ILineBreaks/*!*/LineBreaks { get { return (ILineBreaks)this; } }

        /// <summary>
        /// Line breaks managed internally.
        /// </summary>
        protected ILineBreaks _innerLineBreaks;

        /// <summary>
        /// Naming context defining aliases.
        /// </summary>
        public NamingContext/*!*/ Naming
        {
            get { return this._naming; }
            internal set
            {
                if (value == null) throw new ArgumentNullException();
                this._naming = value;
            }
        }
        private NamingContext/*!*/_naming;

        /// <summary>
        /// Current namespace (in case we are compiling through eval from within namespace).
        /// </summary>
        public QualifiedName? CurrentNamespace { get { return this._naming.CurrentNamespace; } }
        
        public List<QualifiedName>/*!*/ImportedNamespaces { get { return importedNamespaces; } }
        private readonly List<QualifiedName>/*!*/importedNamespaces = new List<QualifiedName>();
        public bool HasImportedNamespaces { get { return this.importedNamespaces != null && this.importedNamespaces.Count != 0; } }

        /// <summary>
        /// Encoding of the file or the containing file.
        /// </summary>
        public Encoding/*!*/ Encoding { get { return _encoding; } }
        protected readonly Encoding/*!*/ _encoding;

        /// <summary>
        /// Gets value indicating whether we are in pure mode.
        /// </summary>
        public virtual bool IsPure { get { return false; } }

        /// <summary>
        /// Gets value indicating whether we are processing transient unit.
        /// </summary>
        public virtual bool IsTransient { get { return false; } }

        #endregion

        #region Construction

        public SourceUnit(string/*!*/ filePath, Encoding/*!*/ encoding, ILineBreaks/*!*/lineBreaks)
        {
            Debug.Assert(filePath != null && encoding != null);
            Debug.Assert(lineBreaks != null);

            _filePath = filePath;
            _encoding = encoding;
            _innerLineBreaks = lineBreaks;
            _naming = new NamingContext(null);
        }

        #endregion

        #region Abstract Methods

        public abstract void Parse(INodesFactory<LangElement, Span> factory, Errors.IErrorSink<Span> errors);

        public abstract void Close();

        public abstract string GetSourceCode(Span span);

        #endregion

        #region ILineBreaks Members

        int ILineBreaks.Count
        {
            get { return this._innerLineBreaks.Count; }
        }

        int ILineBreaks.TextLength
        {
            get { return this._innerLineBreaks.TextLength; }
        }

        int ILineBreaks.EndOfLineBreak(int index)
        {
            return this._innerLineBreaks.EndOfLineBreak(index);
        }

        public virtual int GetLineFromPosition(int position)
        {
            return this._innerLineBreaks.GetLineFromPosition(position);
        }

        public virtual void GetLineColumnFromPosition(int position, out int line, out int column)
        {
            this._innerLineBreaks.GetLineColumnFromPosition(position, out line, out column);
        }

        #endregion

        #region IPropertyCollection Members

        void IPropertyCollection.SetProperty(object key, object value)
        {
            innerProps.SetProperty(key, value);
        }

        void IPropertyCollection.SetProperty<T>(T value)
        {
            innerProps.SetProperty<T>(value);
        }

        object IPropertyCollection.GetProperty(object key)
        {
            return innerProps.GetProperty(key);
        }

        T IPropertyCollection.GetProperty<T>()
        {
            return innerProps.GetProperty<T>();
        }

        bool IPropertyCollection.TryGetProperty(object key, out object value)
        {
            return innerProps.TryGetProperty(key, out value);
        }

        bool IPropertyCollection.TryGetProperty<T>(out T value)
        {
            return innerProps.TryGetProperty<T>(out value);
        }

        bool IPropertyCollection.RemoveProperty(object key)
        {
            return innerProps.RemoveProperty(key);
        }

        bool IPropertyCollection.RemoveProperty<T>()
        {
            return innerProps.RemoveProperty<T>();
        }

        void IPropertyCollection.ClearProperties()
        {
            innerProps.ClearProperties();
        }

        object IPropertyCollection.this[object key]
        {
            get
            {
                return innerProps[key];
            }
            set
            {
                innerProps[key] = value;
            }
        }

        #endregion
    }

    #endregion

    #region CodeSourceUnit

    /// <summary>
    /// Source unit from string representation of code.
    /// </summary>
    public class CodeSourceUnit : SourceUnit
    {
        #region Fields & Properties

        public string/*!*/ Code { get { return code; } }
        private readonly string/*!*/ code;

        /// <summary>
        /// Initial state of source code parser. Used by <see cref="Parse"/>.
        /// </summary>
        private readonly Lexer.LexicalStates initialState;

        public Lexer.LexicalStates/*!*/ InitialState { get { return initialState; } }

        readonly LanguageFeatures features;

        #endregion

        #region SourceUnit

        public CodeSourceUnit(string/*!*/ code, string/*!*/ filePath,
            Encoding/*!*/ encoding,
            Lexer.LexicalStates initialState = Lexer.LexicalStates.INITIAL,
            LanguageFeatures features = LanguageFeatures.Basic)
            : base(filePath, encoding, Text.LineBreaks.Create(code))
        {
            this.code = code;
            this.initialState = initialState;
            this.features = features;
        }

        public override void Parse(INodesFactory<LangElement, Span> factory, Errors.IErrorSink<Span> errors)
        {
            using (var source = new StringReader(this.Code))
            {
                var lexer = new Lexer(source, Encoding.UTF8, errors, features, 0, initialState);
                ast = new Parser().Parse(lexer, factory, errors);
            }
        }

        /// <summary>
        /// Initializes <c>Ast</c> with empty <see cref="GlobalCode"/>.
        /// </summary>
        internal void SetEmptyAst()
        {
            this.ast = new GlobalCode(new Span(), new List<Statement>(), this);
        }

        public override string GetSourceCode(Span span)
        {
            return span.GetText(code);
        }

        public override void Close()
        {

        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates source unit and parses given <paramref name="code"/>.
        /// </summary>
        /// <param name="code">Source code to be parsed.</param>
        /// <param name="filePath">Source file used for error reporting.</param>
        /// <param name="factory">Nodes factory and error sink.</param>
        /// <param name="errors">Error sink. Can be <c>null</c>.</param>
        /// <param name="features">Optional. Language features.</param>
        /// <param name="initialState">
        /// Optional. Initial parser state.
        /// This allows e.g. to parse PHP code without encapsulating the code into opening and closing tags.</param>
        /// <returns></returns>
        public static SourceUnit/*!*/ParseCode(string code, string filePath,
            INodesFactory<LangElement, Span> factory = null,
            Errors.IErrorSink<Span> errors = null,
            LanguageFeatures features = LanguageFeatures.Basic,
            Lexer.LexicalStates initialState = Lexer.LexicalStates.INITIAL)
        {
            var unit = new CodeSourceUnit(code, filePath, Encoding.UTF8, initialState, features);

            if (factory == null)
            {
                factory = new BasicNodesFactory(unit);
            }

            if (errors == null)
            {
                errors = (factory as Errors.IErrorSink<Span>) ?? new EmptyErrorSink<Span>();
            }

            var lexer = new Lexer(new StringReader(code), Encoding.UTF8, errors, features, 0, initialState);

            unit.Parse(factory, errors);
            unit.Close();

            //
            return unit;
        }

        #endregion
        
    }

    #endregion
}
