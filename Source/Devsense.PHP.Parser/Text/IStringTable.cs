using System;

namespace Devsense.PHP.Text
{
    /// <summary>
    /// Common interface for string interning.
    /// </summary>
    public interface IStringTable
    {
        /// <summary>
        /// Get cached string instance.
        /// </summary>
        string GetOrAdd(ReadOnlySpan<char> text);
    }
}
