﻿using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax
{
    static class DocBlockExtent
    {
        /// <summary>
        /// Determines whether this block is above given element.
        /// </summary>
        public static bool IsAbove(this IDocBlock docblock, ILangElement element) =>
            element != null && Math.Max(docblock.Extent.End, docblock.Span.End) <= element.Span.Start;

        /// <summary>
        /// Determines whether this block is below given element or element is <c>null</c>.
        /// </summary>
        public static bool IsBelowOrNull(this IDocBlock docblock, ILangElement element) =>
            element == null || Math.Max(docblock.Extent.End, docblock.Span.End) > element.Span.End;
    }
    
    /// <summary>
    /// Helper class containing list of DOC comments during tokenization.
    /// Provides searching for DOC comment above given position.
    /// </summary>
    class DocCommentContainer
    {
        #region Fields & Properties

        /// <summary>
        /// Ordered list of DOC comments.
        /// </summary>
        readonly List<IDocBlock> _doclist = new List<IDocBlock>();

        /// <summary>
        /// Extent of included DOC comments span.
        /// </summary>
        public Span Extent => (_doclist != null && _doclist.Count != 0)
            ? Span.FromBounds(_doclist[0].Extent.Start, _doclist.Last().Extent.End)
            : Span.Invalid;

        #endregion

        /// <summary>
        /// Inserts DOC block into the list.
        /// </summary>
        public IDocBlock/*!*/Append(IDocBlock/*!*/phpdoc, Tokens previous, Span previousSpan)
        {
            Debug.Assert(phpdoc != null);
            Debug.Assert(
                _doclist.Count == 0 ||
                _doclist.Last().Extent.Start < phpdoc.Span.Start,
                "Blocks have to be appended in order."
            );

            if (previous == Tokens.T_WHITESPACE)
            {
                phpdoc.Extent = Span.FromBounds(previousSpan.Start, phpdoc.Extent.End);
            }
            
            _doclist.Add(phpdoc);

            //
            return phpdoc;
        }

        /// <summary>
        /// Finds DOC comment above given position, removes it from the internal list and returns its reference.
        /// </summary>
        public bool TryReleaseBlock(int position, out IDocBlock phpdoc)
        {
            var index = this.FindIndex(position - 1);
            return TryReleaseIndexedBlock(index, out phpdoc);
        }

        /// <summary>
        /// Finds DOC comment inside given position, removes it from the internal list and returns its reference.
        /// </summary>
        public bool TryReleaseBlock(Span position, out IDocBlock phpdoc)
        {
            var index = FindFirstIn(_doclist, position);
            return TryReleaseIndexedBlock(index, out phpdoc);
        }

        /// <summary>
        /// Finds DOC comment above given position, removes it from the internal list and returns its reference.
        /// </summary>
        public bool TryReleaseIndexedBlock(int index, out IDocBlock phpdoc)
        {
            if (index >= 0 && index < _doclist.Count)
            {
                var list = _doclist;
                phpdoc = list[index];
                list.RemoveAt(index);

                //
                return true;
            }

            //
            phpdoc = null;
            return false;
        }

        /// <summary>
        /// Finds DOC comment at given position and annotates statement with it.
        /// </summary>
        public void Annotate(LangElement element)
        {
            if (TryReleaseBlock(element.Span.Start, out var phpdoc))
            {
                element.SetPHPDocNoLock(phpdoc);
            }
        }

        /// <summary>
        /// Finds DOC comment intersecting given span and annotates statement with it.
        /// </summary>
        public void AnnotateSpan(LangElement element)
        {
            if (TryReleaseBlock(Span.FromBounds(element.Span.Start - 1, element.Span.End), out var phpdoc))
            {
                element.SetPHPDocNoLock(phpdoc);
            }
        }

        /// <summary>
        /// Finds DOC comment at given position and annotates statement with it.
        /// </summary>
        public void AnnotateMember(LangElement element)
        {
            if (LastDocBlock != null && LastDocBlock.Span.End > element.Span.Start &&
                TryReleaseBlock(Span.Combine(element.Span, LastDocBlock.Span), out var phpdoc))
            {
                element.SetPHPDocNoLock(phpdoc);
            }
        }

        /// <summary>
        /// Merges DOC comments into the list of statements.
        /// </summary>
        /// <param name="extent">Span of code block containing <paramref name="stmts"/>.</param>
        /// <param name="stmts">List of statements to be merged with overlapping DOC comments.</param>
        /// <param name="factory">Factory used to create PHPDoc statements.</param>
        public void Merge(Text.Span extent, List<LangElement>/*!*/stmts, INodesFactory<LangElement, Span> factory)
        {
            Debug.Assert(extent.IsValid);
            Debug.Assert(stmts != null);

            if (this.Extent.OverlapsWith(extent))
            {
                var list = _doclist;
                int insertAt = 0;
                int count = 0;

                IDocBlock doc;
                int indexFrom = FindFirstIn(list, extent);

                for (var index = indexFrom; index < list.Count && (doc = list[index]).Extent.OverlapsWith(extent); index++)
                {
                    // skip stmts fully above {doc}
                    while (insertAt < stmts.Count && doc.IsBelowOrNull(stmts[insertAt]))
                    {
                        insertAt++;
                    }

                    // insert {doc} into {stmts}
                    if (insertAt == stmts.Count || doc.IsAbove(stmts[insertAt]))
                    {
                        stmts.Insert(insertAt, factory.PHPDoc(doc.Span, doc));
                        insertAt++;
                    }

                    //
                    count++;
                }

                //
                if (count != 0)
                {
                    list.RemoveRange(indexFrom, count);
                }
            }
        }

        /// <summary>
        /// Finds index of DOC comment at given position.
        /// </summary>
        private int FindIndex(int position)
        {
            if (this.Extent.Contains(position))
            {
                Debug.Assert(_doclist != null);
                int index = FindIndex(_doclist, position);
                if (index >= 0 && _doclist[index].Extent.Contains(position))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Binary search.
        /// </summary>
        private static int FindIndex(List<IDocBlock>/*!*/list, int position)
        {
            int a = 0, b = list.Count - 1;
            while (a <= b)
            {
                int x = (a + b) >> 1;
                var doc = list[x];
                if (doc.Extent.Contains(position))
                    return x;

                if (position > doc.Extent.Start)
                    a = x + 1;
                else
                    b = x - 1;
            }

            return -1;
        }

        /// <summary>
        /// Gets lowest index of DOC comment that intersects given span. Returns count of items if nothing was found.
        /// </summary>
        private static int FindFirstIn(List<IDocBlock>/*!*/list, Span span)
        {
            Debug.Assert(span.IsValid);
            Debug.Assert(list != null);

            int result = list.Count;
            int a = 0, b = result - 1;
            while (a <= b)
            {
                // modified binary search to find the lowest index of element within {span}

                int x = (a + b) >> 1;
                var doc = list[x];
                if (doc.Extent.Start >= span.End)
                {
                    b = x - 1;
                }
                else if (doc.Extent.End <= span.Start)
                {
                    a = x + 1;
                }
                else
                {
                    result = x;
                    b = x - 1;  // and try lower
                }
            }

            //
            return result;
        }

        public IDocBlock LastDocBlock => _doclist.Count > 0 ? _doclist.Last() : null;
    }
}
