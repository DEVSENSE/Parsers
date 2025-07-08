using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Devsense.PHP.Utilities
{
    public static class StringInterns
    {
        public static string WithIntern(ReadOnlySpan<char> span) => TryIntern(span) ?? span.ToString();

        public static string TryIntern(ReadOnlySpan<char> span)
        {
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
                        case '?': return "?";
                        case '@': return "@";
                        case ';': return ";";
                        case ',': return ",";
                        case '-': return "-";
                        case '+': return "+";
                        case '_': return "_";
                        case '~': return "~";
                        case '|': return "|";
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
                        case '\'': return "'";
                        case '"': return "\"";
                        case 'A': return "A";
                        case 'B': return "B";
                        case 'C': return "C";
                        case 'a': return "a";
                        case 'b': return "b";
                        case 'c': return "c";
                        case 'd': return "d";
                        case 'e': return "e";
                        case 'f': return "f";
                        case 'g': return "g";
                        case 'h': return "h";
                        case 'i': return "i";
                        case 'n': return "n";
                        case 'm': return "m";
                        case 's': return "s";
                        case 'u': return "u";
                        case 'x': return "x";
                        case 'y': return "y";
                        case 'z': return "z";
                        case '.': return ".";
                        case '=': return "=";
                        case '0': return "0";
                        case '1': return "1";
                        case '2': return "2";
                        case '3': return "3";
                        case '4': return "4";
                        case '5': return "5";
                        case '6': return "6";
                        case '7': return "7";
                        case '8': return "8";
                    }
                    break;

                case 2:
                    switch (span[0])
                    {
                        case '\r': if (span[1] == '\n') return "\r\n"; break;
                        case '/': if (span[1] == '/') return "//"; break;
                        case 'f': if (span[1] == 'n') return "fn"; break;
                        case '-': if (span[1] == '>') return "->"; break;
                        case '\"': if (span[1] == '\"') return "\"\""; break;
                        case '\'': if (span[1] == '\'') return "''"; break;
                        case '[': if (span[1] == ']') return "[]"; break;
                    }
                    break;

                // most frequent keywords

                case 3:
                    switch (span[0])
                    {
                        case 'i':
                            if (span[1] == 'n' && span[2] == 't') return "int";
                            break;
                        case 'k':
                            if (span[1] == 'e' && span[2] == 'y') return "key";
                            break;
                        case 'u':
                            if (span[1] == 's' && span[2] == 'e') return "use";
                            break;
                        case '/':
                            if (span[1] == '.' && span[2] == '.') return "/..";
                            break;
                        case '[':
                            if (span[1] == '?' && span[2] == ']') return "[?]";
                            break;
                    }
                    break;

                case 4:
                    switch (span[0])
                    {
                        case 'b':
                            if (MemoryExtensions.Equals(span, "bool".AsSpan(), StringComparison.Ordinal)) return "bool";
                            break;
                        case 'c':
                            if (MemoryExtensions.Equals(span, "case".AsSpan(), StringComparison.Ordinal)) return "case";
                            break;
                        case 'e':
                            if (MemoryExtensions.Equals(span, "enum".AsSpan(), StringComparison.Ordinal)) return "enum";
                            if (MemoryExtensions.Equals(span, "echo".AsSpan(), StringComparison.Ordinal)) return "echo";
                            break;
                        case 'n':
                            if (MemoryExtensions.Equals(span, "null".AsSpan(), StringComparison.Ordinal)) return "null";
                            if (MemoryExtensions.Equals(span, "name".AsSpan(), StringComparison.Ordinal)) return "name";
                            break;
                        case 't':
                            if (MemoryExtensions.Equals(span, "type".AsSpan(), StringComparison.Ordinal)) return "type";
                            if (MemoryExtensions.Equals(span, "text".AsSpan(), StringComparison.Ordinal)) return "text";
                            if (MemoryExtensions.Equals(span, "true".AsSpan(), StringComparison.Ordinal)) return "true";
                            if (MemoryExtensions.Equals(span, "this".AsSpan(), StringComparison.Ordinal)) return "this";
                            break;
                        case 'v':
                            if (MemoryExtensions.Equals(span, "void".AsSpan(), StringComparison.Ordinal)) return "void";
                            break;
                    }
                    break;

                case 5:
                    switch (span[0])
                    {
                        case 'U':
                            if (MemoryExtensions.Equals(span, "UTF-8".AsSpan(), StringComparison.Ordinal)) return "UTF-8";
                            break;
                        case 'a':
                            if (MemoryExtensions.Equals(span, "array".AsSpan(), StringComparison.Ordinal)) return "array";
                            break;
                        case 'c':
                            if (MemoryExtensions.Equals(span, "class".AsSpan(), StringComparison.Ordinal)) return "class";
                            break;
                        case 'f':
                            if (MemoryExtensions.Equals(span, "false".AsSpan(), StringComparison.Ordinal)) return "false";
                            if (MemoryExtensions.Equals(span, "float".AsSpan(), StringComparison.Ordinal)) return "float";
                            break;
                        case 'm':
                            if (MemoryExtensions.Equals(span, "mixed".AsSpan(), StringComparison.Ordinal)) return "mixed";
                            break;
                        case 'v':
                            if (MemoryExtensions.Equals(span, "value".AsSpan(), StringComparison.Ordinal)) return "value";
                            break;
                    }
                    break;

                case 6:
                    switch (span[0])
                    {
                        case 's':
                            if (MemoryExtensions.Equals(span, "static".AsSpan(), StringComparison.Ordinal)) return "static";
                            if (MemoryExtensions.Equals(span, "string".AsSpan(), StringComparison.Ordinal)) return "string";
                            break;
                        case 'p':
                            if (MemoryExtensions.Equals(span, "public".AsSpan(), StringComparison.Ordinal)) return "public";
                            break;
                    }
                    break;

                case 8:
                    switch (span[0])
                    {
                        case 'c':
                            if (MemoryExtensions.Equals(span, "callable".AsSpan(), StringComparison.Ordinal)) return "callable";
                            break;
                        case 'f':
                            if (MemoryExtensions.Equals(span, "function".AsSpan(), StringComparison.Ordinal)) return "function";
                            break;
                        case 's':
                            if (MemoryExtensions.Equals(span, "stdClass".AsSpan(), StringComparison.Ordinal)) return "stdClass";
                            break;
                    }
                    break;
            }

            //
            return null;
        }
    }
}
