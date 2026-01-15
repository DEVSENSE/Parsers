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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Default implementation of documentary comment element.
    /// </summary>
    [DebuggerDisplay("{PHPDocPreview,nq} {Span}")]
    public sealed class PHPDocBlock : LangElementEntireSpan, IDocBlock
    {
        #region Properties

        /// <summary>
        /// Source documentary comment text, as it is in source code.
        /// Used for lazy initialization.
        /// </summary>
        string _source;

        IDocEntry _lazyEntries;
        
        /// <summary>
        /// Extent span.
        /// </summary>
        public Span Extent { get; set; }

        public IDocEntry Entries
        {
            get
            {
                EnsureParsed();

                return _lazyEntries;
            }
        }

        /// <summary>
        /// Gets the enumeration of <see cref="IDocEntry"/> list.
        /// </summary>
        public DocBlockEntriesEnumerator<IDocEntry> GetEnumerator() => new DocBlockEntriesEnumerator<IDocEntry>(Entries);

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes new instance of <see cref="PHPDocBlock"/>.
        /// </summary>
        /// <param name="doccomment">PHPDoc token content.</param>
        /// <param name="span">Position of the comment in the source code.</param>
        public PHPDocBlock(string doccomment, Span span)
            : base(span)
        {
            _source = doccomment;

            this.Extent = span;
        }

        private void EnsureParsed()
        {
            var source = _source;
            if (source != null && _lazyEntries == null)
            {
                if (ParseNoLock(source, this.Span.Start, out var head, out var summary))
                {
                    if (Interlocked.CompareExchange(ref _lazyEntries, head, null) == null)
                    {
                        _summary = summary;
                    }
                }

                //
                Interlocked.Exchange(ref _source, null);
            }
        }

        /// <summary>
        /// Simple parse of given <paramref name="doccomment"/> into a list of <see cref="IDocEntry"/> instances.
        /// </summary>
        /// <param name="doccomment">Content of the PHPDoc token.</param>
        /// <param name="offset">Start position of <paramref name="doccomment"/> within the source code.</param>
        /// <param name="head">Gets the linked list of entries.</param>
        /// <param name="summary">Gets the documentary comment summary.</param>
        private static bool ParseNoLock(string/*!*/doccomment, int offset, out IDocEntry head, out string summary)
        {
            head = null;
            summary = string.Empty;

            if (string.IsNullOrEmpty(doccomment))
            {
                return false;
            }

            Debug.Assert(doccomment.StartsWith("/**"), "unexpected doc comment prefix");
            Debug.Assert(doccomment.EndsWith("*/"), "unexpected doc comment suffix");

            var summarybld = StringUtils.GetStringBuilder(); // all the text up to the first phpdoc tag will be accumulated as a summary
            CommonDocEntry current = null;

            int index = 0;
            while (index < doccomment.Length)
            {
                // consume line
                int eol = index;
                int linebreak = 0;
                for (; eol < doccomment.Length; eol++)
                {
                    if ((linebreak = TextUtils.LengthOfLineBreak(doccomment, eol)) != 0)
                    {
                        break;
                    }
                }

                // skip the leading whitespace and /**
                while (index < eol && (char.IsWhiteSpace(doccomment, index) || doccomment[index] == '*' || (index == 0 && doccomment[index] == '/'))) index++;

                // line := [index .. eol + linebreak]
                var line = doccomment.AsSpan(index, eol - index);

                // trim ending
                if (eol >= doccomment.Length - 2 && line.EndsWith("*/".AsSpan())) line = line.Slice(0, line.Length - 2); // */
                else if (eol >= doccomment.Length - 1 && line.Length != 0 && line[line.Length - 1] == '/')
                {
                    if (line.Length == 1) break; // this was the last line and it was just */
                    line = line.Slice(0, line.Length - 1);
                }
                line = line.TrimEnd(); // whitespaces

                // 
                if (line.Length != 0 && line[0] == '@') // phpdoc tag
                {
                    // create new entry
                    var entry = new CommonDocEntry()
                    {
                        Span = Span.FromBounds(offset + index, offset + index + line.Length),
                        Next = null,
                        Content = line.ToString(),
                    };

                    // update linked list
                    if (current != null)
                    {
                        current.Next = entry;
                    }
                    else
                    {
                        head = entry;
                    }

                    current = entry;
                }
                else if (current != null)
                {
                    // append to the current
                    current.Span = Span.FromBounds(current.Span.Start, offset + index + line.Length);
                    current.Content = current.Content + Environment.NewLine + line.ToString();
                }
                else
                {
                    // append to the summry
                    if (summarybld.Length != 0)
                    {
                        summarybld.AppendLine();
                    }

                    if (summarybld.Length != 0 || line.Length != 0)
                    {
                        summarybld.AppendSpan(line);
                    }
                }

                // 
                index = eol + linebreak;
            }

            //
            summary = StringUtils.ReturnStringBuilder(summarybld);

            return true;
        }

        #endregion

        #region Helper access methods

        public T GetElement<T>() where T : IDocEntry => DocBlockExtensions.GetElementOfType<T>(this);

        public string Summary
        {
            get
            {
                EnsureParsed();
                return _summary;
            }
        }

        string _summary; // lazily initialized

        /// <summary>
        /// Reconstructs PHPDoc block from parsed elements, including comment tags.
        /// </summary>
        public string PHPDocPreview
        {
            get
            {
                var result = StringUtils.GetStringBuilder();
                result.AppendLine("/**");

                // TODO: +summary

                foreach (var element in this)
                {
                    var str = element.ToString();
                    if (str == null) continue;

                    foreach (var line in str.Split('\n'))
                    {
                        result.Append(" * ");
                        result.AppendLine(line);
                    }

                }
                result.Append(" */");

                return StringUtils.ReturnStringBuilder(result);
            }
        }
        
        #endregion

        /// <summary>
        /// Returns summary of PHPDoc.
        /// </summary>
        public override string ToString() => this.Summary;

        public override void VisitMe(TreeVisitor visitor) => visitor.VisitPHPDocBlock(this);

        bool IDocBlock.HasSummary() => !string.IsNullOrEmpty(this.Summary);

        bool IDocBlock.HasSummary(out string summary) => !string.IsNullOrEmpty(summary = this.Summary);
    }

    internal static class PHPDocBlockHelper
    {
        /// <summary>
        /// Gets <see cref="PHPDocBlock"/> associated with <paramref name="properties"/>.
        /// </summary>
        public static IDocBlock GetPHPDoc(this IPropertyCollection/*!*/properties) => properties.GetPropertyOfType<IDocBlock>();

        /// <summary>
        /// Sets <see cref="PHPDocBlock"/> to <see cref="AstNode.Properties"/>.
        /// </summary>
        internal static void SetPHPDocNoLock(this LangElement/*!*/element, IDocBlock phpdoc)
        {
            if (phpdoc != null)
            {
                element.SetPropertyNoLock(
                    phpdoc.GetType(), // optimization: if the key matches type of value, it's stored within single object without alloc
                    phpdoc
                );

                // remember LangElement associated with phpdoc
                phpdoc.ContainingElement = element;
            }
            else
            {
                var existing = GetPHPDoc(element);
                if (existing != null)
                {
                    element.RemovePropertyNoLock(existing.GetType());
                }
            }
        }
        
        /// <summary>
        /// Sets <see cref="PHPDocBlock"/> to <paramref name="properties"/>.
        /// </summary>
        public static void SetPHPDoc(this IPropertyCollection/*!*/properties, IDocBlock phpdoc)
        {
            if (phpdoc != null)
            {
                properties.SetProperty(phpdoc.GetType(), phpdoc); // optimization // if the key matches type of value, it's stored within single object without alloc

                // remember LangElement associated with phpdoc
                if (properties is ILangElement element)
                {
                    phpdoc.ContainingElement = element;
                }
            }
            else
            {
                var existing = GetPHPDoc(properties);
                if (existing != null)
                {
                    properties.RemoveProperty(existing.GetType());
                }
            }
        }
    }
}
