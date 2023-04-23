using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Utilities
{
    internal static class StringInterns
    {
        public static string TryIntern(char[] buffer, int start, int length)
        {
            var span = buffer.AsSpan(start, length);

            // PERF: Whether interning or not, there are some frequently occurring easy cases we can pick off easily.
            switch (span.Length)
            {
                case 0:
                    return string.Empty;

                case 1:
                    switch (span[0])
                    {
                        case ' ': return " ";
                        case '\n': return "\n";
                        case '\r': return "\r";
                        case ';': return ";";
                        case ',': return ",";
                        case '(': return "(";
                        case ')': return ")";
                        case '[': return "[";
                        case ']': return "]";
                        case '{': return "{";
                        case '}': return "}";
                        case '<': return "<";
                        case '>': return ">";
                        case '/': return "/";
                        case '\\': return "\\";
                        case ':': return ":";
                        case 'a': return "a";
                        case 'b': return "b";
                        case 'c': return "c";
                        case 'i': return "i";
                        case 'x': return "x";
                        case 'y': return "y";
                        case '.': return ".";
                        case '=': return "=";
                        case '0': return "0";
                        case '1': return "1";
                    }
                    break;

                case 2:
                    if (span[0] == '\r' && span[1] == '\n') return "\r\n";
                    //if (span[0] == '/' && span[1] == '/') return "//";
                    if (span[0] == 'f' && span[1] == 'n') return "fn";
                    if (span[0] == '-' && span[1] == '>') return "->";
                    break;

                // most frequent keywords

                case 3:
                    switch (span[0])
                    {
                        case 'u':
                            if (span[1] == 's' && span[2] == 'e') return "use";
                            break;
                        case 'i':
                            if (span[1] == 'n' && span[2] == 't') return "int";
                            break;
                    }
                    break;

                case 4:
                    switch (span[0])
                    {
                        case 'c':
                            if (MemoryExtensions.Equals(span, "case".AsSpan(), StringComparison.Ordinal)) return "case";
                            break;
                        case 'e':
                            if (MemoryExtensions.Equals(span, "enum".AsSpan(), StringComparison.Ordinal)) return "enum";
                            if (MemoryExtensions.Equals(span, "echo".AsSpan(), StringComparison.Ordinal)) return "echo";
                            break;
                        case 'n':
                            if (MemoryExtensions.Equals(span, "null".AsSpan(), StringComparison.Ordinal)) return "null";
                            break;
                    }
                    break;

                case 5:
                    switch (span[0])
                    {
                        case 'c':
                            if (MemoryExtensions.Equals(span, "class".AsSpan(), StringComparison.Ordinal)) return "class";
                            break;
                    }
                    break;

                case 6:
                    switch (span[0])
                    {
                        case 's':
                            if (MemoryExtensions.Equals(span, "static".AsSpan(), StringComparison.Ordinal)) return "static";
                            break;
                        case 'p':
                            if (MemoryExtensions.Equals(span, "public".AsSpan(), StringComparison.Ordinal)) return "public";
                            break;
                    }
                    break;

                case 8:
                    switch (span[0])
                    {
                        case 'f':
                            if (MemoryExtensions.Equals(span, "function".AsSpan(), StringComparison.Ordinal)) return "function";
                            break;
                    }
                    break;
            }

            //
            return null;
        }
    }
}
