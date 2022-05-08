using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Ast.DocBlock
{
    /// <summary>
    /// Helper documentary comments methods.
    /// </summary>
    public static class DocBlockExtensions
    {
        /// <summary>
        /// Returns first <see cref="IDocEntry"/> of type <typeparamref name="T"/>, or <c>null</c> if there is no such entry.
        /// </summary>
        /// <typeparam name="T">Type of entry.</typeparam>
        /// <param name="docblock">Block of entries.</param>
        public static T GetElementOfType<T>(this IDocBlock docblock) where T : IDocEntry
        {
            foreach (var entry in docblock)
            {
                if (entry is T t)
                {
                    return t;
                }
            }

            return default(T);
        }
    }
}
