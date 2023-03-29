using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Utilities
{
    internal static class StringInterns
    {
        public static string TryIntern(char[] buffer, int start, int length)
        {
            // PERF: Whether interning or not, there are some frequently occurring easy cases we can pick off easily.
            switch (length)
            {
                case 0:
                    return string.Empty;

                case 1:
                    switch (buffer[start])
                    {
                        case ' ': return " ";
                        case '\n': return "\n";
                        case ';': return ";";
                        case ',': return ",";
                        case '(': return "(";
                        case ')': return ")";
                        case '[': return "[";
                        case ']': return "]";
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
                    }
                    break;

                case 2:
                    if (buffer[start] == '\r' && buffer[start + 1] == '\n') return "\r\n";
                    //if (buffer[start] == '/' && buffer[start + 1] == '/') return "//";
                    break;
            }

            //
            return null;
        }
    }
}
