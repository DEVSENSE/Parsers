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
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Structuralized representation of PHPDoc DocBlock.
    /// </summary>
    /// <remarks>define() statements, functions, classes, class methods, and class vars, include() statements, and global variables can all be documented.
    /// See http://en.wikipedia.org/wiki/PHPDoc for specifications.</remarks>
    [DebuggerDisplay("{PHPDocPreview,nq} {Span}")]
    public sealed class PHPDocBlock : LangElement
    {
        #region Nested classes: Element

        public abstract class Element
        {
            #region Constants

            const string PHPDocStart = "/**";

            const string PHPDocEnd = "*/";

            /// <summary>
            /// Every PHPDoc line not starting with this character is ignored.
            /// </summary>
            private const char PHPDocFirstChar = '*';

            /// <summary>
            /// Every PHPDoc tag starts with this character.
            /// </summary>
            private const char PHPDocTagChar = '@';

            /// <summary>
            /// String representing new line between PHPDoc comment lines.
            /// </summary>
            protected const string NewLineString = "\n";

            #endregion

            #region Properties

            /// <summary>
            /// Element position within the source code.
            /// </summary>
            public Span Span { get; internal set; }

            #endregion

            #region Tags

            /// <summary>
            /// Tag elements initialized using reflection.
            /// </summary>
            private static readonly Dictionary<string, Func<string, string, Element>>/*!!*/s_elementFactories;

            static Element()
            {
                // initialize dictionary of known tags and their factories:
                s_elementFactories = new Dictionary<string, Func<string, string, Element>>(36, StringComparer.OrdinalIgnoreCase);
                var types = typeof(PHPDocBlock).GetTypeInfo().DeclaredNestedTypes.Where(t => t.IsNestedPublic && !t.IsAbstract);
                var eltype = typeof(Element).GetTypeInfo();
                foreach (var t in types)
                {
                    if (t.IsSealed && eltype.IsAssignableFrom(t))
                    {
                        // add to the dictionary according to its Name:
                        var fld = t.GetDeclaredField("Name");
                        if (fld != null)
                        {
                            var factory = CreateElementFactory(t);
                            s_elementFactories.Add(TagNameHelper(fld), factory);
                        }
                        else
                        {
                            var f1 = t.GetDeclaredField("Name1");
                            var f2 = t.GetDeclaredField("Name2");
                            var f3 = t.GetDeclaredField("Name3");

                            if (f1 != null && f2 != null)
                            {
                                var factory = CreateElementFactory(t);
                                s_elementFactories.Add(TagNameHelper(f1), factory);
                                s_elementFactories.Add(TagNameHelper(f2), factory);

                                if (f3 != null)
                                    s_elementFactories.Add(TagNameHelper(f3), factory);
                            }
                            else
                            {
                                // only these Elements do not represent a tag:
                                Debug.Assert(
                                    t.Name == typeof(ShortDescriptionElement).Name ||
                                    t.Name == typeof(LongDescriptionElement).Name ||
                                    t.Name == typeof(UnknownTextTag).Name);
                            }
                        }
                    }
                }

                // ensure we have some tags:
                Debug.Assert(s_elementFactories.ContainsKey("@param"));
                Debug.Assert(s_elementFactories.ContainsKey("@ignore"));
                Debug.Assert(s_elementFactories.ContainsKey("@var"));
                // ...
            }

            private static Func<string, string, Element>/*!*/CreateElementFactory(TypeInfo/*!*/elementType)
            {
                Debug.Assert(elementType != null && typeof(Element).GetTypeInfo().IsAssignableFrom(elementType));

                var ctors = elementType.DeclaredConstructors.ToArray();
                Debug.Assert(ctors != null && ctors.Length == 1);
                var ctor = ctors[0];

                var args = ctor.GetParameters();
                Debug.Assert(args != null && args.Length <= 2);

                // create function that creates the Element 't':
                if (args.Length == 0)
                {
                    return (tagName, line) => (Element)ctor.Invoke(null);
                }
                else if (args.Length == 1)
                {
                    Debug.Assert(args[0].Name == "line");
                    return (tagName, line) => (Element)ctor.Invoke(new object[] { line });
                }
                else
                {
                    Debug.Assert(args[0].Name == "tagName");
                    Debug.Assert(args[1].Name == "line");
                    return (tagName, line) => (Element)ctor.Invoke(new object[] { tagName, line });
                }
            }

            /// <summary>
            /// Reads value of given field, assuming it is string constant, which value starts with <see cref="PHPDocTagChar"/>.
            /// </summary>
            private static string TagNameHelper(System.Reflection.FieldInfo fld)
            {
                Debug.Assert(fld != null);

                var tagname = fld.GetValue(null) as string;

                Debug.Assert(!string.IsNullOrEmpty(tagname));
                Debug.Assert(tagname[0] == PHPDocTagChar);

                return tagname;
            }

            private static KeyValuePair<string, Func<string, string, Element>> FindTagInfo(ReadOnlySpan<char>/*!*/line)
            {
                Debug.Assert(line.Length != 0);
                Debug.Assert(line[0] == PHPDocTagChar);

                int endIndex = 1;
                char c;
                while (endIndex < line.Length && !char.IsWhiteSpace(c = line[endIndex]) && c != ':' && c != '(' && c != ';' && c != '.')
                    endIndex++;

                var tagName = ((endIndex < line.Length) ? line.Slice(0, endIndex) : line).ToString();

                if (s_elementFactories.TryGetValue(tagName, out var tmp))
                    return new KeyValuePair<string, Func<string, string, Element>>(tagName, tmp);
                else
                    return new KeyValuePair<string, Func<string, string, Element>>(tagName, (_name, _line) => new UnknownTextTag(_name, _line));
            }

            #endregion

            #region Parsing

            /// <summary>
            /// Prepares given <paramref name="line"/>.
            /// 
            /// If the line creates new PHPDoc element, new <see cref="Element"/>
            /// is instantiated and returned in <paramref name="next"/>.
            /// </summary>
            /// <param name="line">Line to parse. Cannot be <c>null</c> reference.</param>
            /// <param name="next">Outputs new element that will follow current element. Parsing will continue using this element.</param>
            /// <param name="lineIndex">Index of the line within PHPDoc token.</param>
            /// <param name="lineOffset">Gets count of characters of which <paramref name="line"/> was moved.</param>
            /// <returns>If the line can be parsed, method returns <c>true</c>.</returns>
            internal static bool TryParseLine(ref ReadOnlySpan<char>/*!*/line, out Element next, int lineIndex, out int lineOffset)
            {
                next = null;
                lineOffset = 0;

                int from = 0;
                int to = line.Length;

                // we shouldn't, but we allow first line to contain text after the /** sequence:
                if (lineIndex == 0 && line.StartsWith(PHPDocStart.AsSpan(), StringComparison.Ordinal))
                {
                    // skip /** sequence
                    from = PHPDocStart.Length;
                }
                else
                {
                    // skip leading whitespaces
                    while (from < line.Length && char.IsWhiteSpace(line[from]))
                    {
                        from++;
                    }

                    if (from < line.Length && line[from] == PHPDocFirstChar)
                    {
                        from++;   // skip '*'
                    }
                    else
                    {
                        // invalid PHPDoc line (not starting with '*'):
                        return false;
                    }
                }

                // trim leading whitespaces
                int leading_ws = 0; // number of leading whitespaces
                while (from < line.Length && char.IsWhiteSpace(line[from]))
                {
                    // skip whitespaces
                    from++;
                    leading_ws++;
                }

                // trim ending whitespaces
                while (to > from && char.IsWhiteSpace(line[to - 1]))
                {
                    to--;
                }

                // trim ending */

                line = line.Slice(from, to - from);
                lineOffset = from;

                //line = line.Replace("{@*}", "*/");

                // check "*/" at the end
                if (line.Length == 1 && line[0] == '/')
                    return false;   // empty line

                if (line.EndsWith(PHPDocEnd.AsSpan(), StringComparison.Ordinal))
                {
                    // trim "*/" from the end
                    line = line.Slice(0, line.Length - PHPDocEnd.Length);
                }

                // TODO: any whitespace sequence is converted into single space, but only outside <pre> and {} blocks
                // TODO: handle "{@tag ...}" for @link, @see etc...

                // check tags:
                if (leading_ws <= 2) // (1+2) or more leading whitespaces means it is a nested element // TODO: we need a regular PHPDoc parser ...
                {
                    next = CreateElement(line);
                }

                // 
                return true;
            }

            /// <summary>
            /// Parses given <paramref name="line"/> and updates current content.
            /// </summary>
            /// <param name="line">Line to parse. Line is trimmed and does not start with '*'. Cannot be <c>null</c> reference.</param>
            /// <param name="next">Next element to continue parsing with.</param>
            internal abstract void ParseLine(ReadOnlySpan<char>/*!*/line, out Element next);

            /// <summary>
            /// Reads tag at the beginning of line and tries to create corresponding <see cref="Element"/> instance.
            /// </summary>
            /// <param name="line">PHPDoc comment line. Assuming the line starts with a PHPDoc tag. Otherwise, or if tag is not recognized, <c>null</c> is returned..</param>
            private static Element CreateElement(ReadOnlySpan<char>/*!*/line)
            {
                Debug.Assert(line != null);

                if (line.Length == 0 || line[0] != PHPDocTagChar)
                    return null;

                // try to match known tags:
                var tagInfo = FindTagInfo(line);
                if (tagInfo.Key != null)
                {
                    Debug.Assert(tagInfo.Value != null);

                    // initialize new tag element
                    return tagInfo.Value(tagInfo.Key, line.ToString());
                }

                // unrecognized tag:
                return null;
            }

            /// <summary>
            /// Returns <c>true</c> if current element does not contain any information and can be ignored.
            /// </summary>
            internal virtual bool IsEmpty { get { return false; } }

            /// <summary>
            /// Called when parsing of this element ended.
            /// </summary>
            internal virtual void OnEndParsing() { }

            #endregion

            #region Helpers

            protected static ReadOnlySpan<char> NextWord(string/*!*/text, ref int index)
            {
                return NextWord(text.AsSpan(), ref index);
            }

            protected static ReadOnlySpan<char> NextWord(ReadOnlySpan<char>/*!*/text, ref int index)
            {
                // skip whitespaces:
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;

                // read word:
                int startIndex = index;
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                // cut off the word:
                if (startIndex < index)
                    return text.Slice(startIndex, index - startIndex);
                else
                    return ReadOnlySpan<char>.Empty;
            }

            #endregion
        }

        /// <summary>
        /// Short description.
        /// </summary>
        public sealed class ShortDescriptionElement : Element
        {
            /// <summary>
            /// Character defining the end of PHPDoc short description.
            /// </summary>
            private const char EndChar = '.';

            public string Text { get; private set; }

            public ShortDescriptionElement()
            {

            }

            internal override void ParseLine(ReadOnlySpan<char>/*!*/line, out Element next)
            {
                next = null;

                // ignore first line of length 0 (empty space after /**)
                if (this.Text == null && line.IsWhiteSpace())
                    return;

                // Short Description can be followed by Long Description.
                // It can be only 3 lines long, otherwise only the first line is taken
                // It is terminated by empty line or a dot.

                if (this.Text != null && (this.Text.LastCharacter() == (int)EndChar))
                {
                    next = new LongDescriptionElement(line.ToString());
                }
                else if (line.Length == 0)
                {
                    next = new LongDescriptionElement(null);
                }
                else if (this.Text.CharsCount('\n') >= 2)
                {
                    // short description has already 3 lines,
                    // only first line is taken, the rest is for LongDescriptionElement
                    int firstLineEndIndex = this.Text.IndexOf('\n');
                    Debug.Assert(firstLineEndIndex != -1);

                    next = new LongDescriptionElement(this.Text.Substring(firstLineEndIndex + 1) + NewLineString + line.ToString());
                    this.Text = this.Text.Remove(firstLineEndIndex);
                }
                else
                {
                    this.Text = (this.Text != null) ? (this.Text + NewLineString + line.ToString()) : line.ToString();
                }
            }

            internal override bool IsEmpty { get { return string.IsNullOrWhiteSpace(this.Text); } }

            internal override void OnEndParsing()
            {
                base.OnEndParsing();
                if (this.Text != null)
                    this.Text = this.Text.Trim();
            }

            public override string ToString()
            {
                return this.Text ?? string.Empty;
            }
        }

        /// <summary>
        /// Long description.
        /// </summary>
        public sealed class LongDescriptionElement : Element
        {
            public string Text { get; private set; }

            public LongDescriptionElement(string initialText)
            {
                this.Text = string.IsNullOrWhiteSpace(initialText) ? null : initialText;
            }

            internal override void ParseLine(ReadOnlySpan<char> line, out Element next)
            {
                // Long Description can only be followed by PHPDoc tag (handled in TryParseLine)

                next = null;
                var newtext = line.ToString();
                this.Text = (this.Text != null) ? (this.Text + NewLineString + newtext) : newtext;
            }

            internal override bool IsEmpty { get { return string.IsNullOrWhiteSpace(this.Text); } }

            internal override void OnEndParsing()
            {
                base.OnEndParsing();
                if (this.Text != null)
                    this.Text = this.Text.Trim();
            }

            public override string ToString()
            {
                return this.Text ?? string.Empty;
            }
        }

        public abstract class EmptyTag : Element
        {
            internal override void ParseLine(ReadOnlySpan<char> line, out Element next)
            {
                next = null;
                // ignored
            }
        }

        /// <summary>
        /// Documents an abstract class, class variable or method.
        /// </summary>
        public sealed class AbstractTag : EmptyTag
        {
            public const string Name = "@abstract";

            public override string ToString()
            {
                return Name;
            }
        }

        /// <summary>
        /// Documents access control for an element. @access private indicates that documentation of element be prevented.
        /// </summary>
        public sealed class AccessTag : Element
        {
            public const string Name1 = "@access";
            public const string Name2 = "@private";

            private const string IsPublic = "public";
            private const string IsPrivate = "private";
            private const string IsProtected = "protected";

            /// <summary>
            /// Resolved access modifier. (public, private or protected)
            /// </summary>
            public PhpMemberAttributes Access { get { return attributes & PhpMemberAttributes.VisibilityMask; } }
            private readonly PhpMemberAttributes attributes;

            private string AccessString
            {
                get
                {
                    switch (Access)
                    {
                        case PhpMemberAttributes.Private: return IsPrivate;
                        case PhpMemberAttributes.Protected: return IsProtected;
                        default: return IsPublic;
                    }
                }
            }

            public AccessTag(string/*!*/line)
            {
                if (line.StartsWith(Name1))
                {
                    if (line.Length > Name1.Length)
                    {
                        var access = line.AsSpan(Name1.Length + 1).Trim().ToString().ToLowerInvariant();

                        // public, private or protected
                        switch (access)
                        {
                            case IsPublic: attributes = PhpMemberAttributes.Public; break;
                            case IsPrivate: attributes = PhpMemberAttributes.Private; break;
                            case IsProtected: attributes = PhpMemberAttributes.Protected; break;
                            default:
                                Debug.WriteLine("Unexpected access modifier in PHPDoc @access tag, line:" + line);
                                break;
                        }
                    }
                    else
                    {
                        attributes = PhpMemberAttributes.Public;
                    }
                }
                else if (line.StartsWith(Name2))
                {
                    attributes = PhpMemberAttributes.Private;
                }
                else
                {
                    Debug.Assert(false, "Unexpected " + line);
                }
            }

            internal override void ParseLine(ReadOnlySpan<char> line, out Element next)
            {
                next = null;
                // ignored
            }

            public override string ToString()
            {
                return Name1 + " " + AccessString;
            }
        }

        public abstract class SingleLineTag : Element
        {
            protected readonly string text;

            internal SingleLineTag(string/*!*/tagName, string/*!*/line)
            {
                Debug.Assert(line.StartsWith(tagName));

                if (line.Length > tagName.Length)
                {
                    this.text = line.AsSpan(tagName.Length).Trim().ToString();
                }
            }

            internal override void ParseLine(ReadOnlySpan<char> line, out Element next)
            {
                next = null;
                // other lines are ignored
            }

            internal override bool IsEmpty
            {
                get
                {
                    return string.IsNullOrWhiteSpace(text);
                }
            }
        }

        /// <summary>
        /// Documents the author of the current element.
        /// </summary>
        public sealed class AuthorTag : SingleLineTag
        {
            public const string Name = "@author";

            /// <summary>
            /// author name &lt;author@email&gt;
            /// </summary>
            public string Author { get { return text; } }

            public AuthorTag(string/*!*/line)
                : base(Name, line)
            {

            }

            public override string ToString()
            {
                return Name + " " + Author;
            }
        }

        /// <summary>
        /// Documents copyright information.
        /// </summary>
        public sealed class CopyrightTag : SingleLineTag
        {
            public const string Name = "@copyright";

            /// <summary>
            /// name date
            /// </summary>
            public string Copyright { get { return text; } }

            public CopyrightTag(string/*!*/line)
                : base(Name, line)
            {

            }

            public override string ToString()
            {
                return Name + " " + Copyright;
            }
        }

        /// <summary>
        /// Documents a method as deprecated.
        /// </summary>
        public sealed class DeprecatedTag : SingleLineTag
        {
            public const string Name1 = "@deprecated";
            public const string Name2 = "@deprec";

            /// <summary>
            /// version
            /// </summary>
            public string Version { get { return text; } }

            public DeprecatedTag(string tagName, string/*!*/line)
                : base(tagName, line)
            {

            }

            public override string ToString()
            {
                return Name1 + " " + Version;
            }

            internal override bool IsEmpty
            {
                get
                {
                    // "@deprecated" itself is significant, even without additional text
                    return false;
                }
            }
        }

        /// <summary>
        /// Annotates an annotation class.
        /// </summary>
        public sealed class AnnotationTag : SingleLineTag
        {
            public const string Name = "@Annotation";

            public AnnotationTag(string tagName, string/*!*/line)
                : base(tagName, line)
            {

            }

            public override string ToString()
            {
                return Name;
            }

            internal override bool IsEmpty
            {
                get
                {
                    // "@Annotation" itself is significant, even without additional text
                    return false;
                }
            }
        }

        /// <summary>
        /// Documents the location of an external saved example file.
        /// </summary>
        public sealed class ExampleTag : SingleLineTag
        {
            public const string Name = "@example";

            /// <summary>
            /// /path/to/example
            /// </summary>
            public string Example { get { return text; } }

            public ExampleTag(string/*!*/line)
                : base(Name, line)
            {

            }

            public override string ToString()
            {
                return Name + " " + Example;
            }
        }

        /// <summary>
        /// Documents an exception thrown by a method.
        /// </summary>
        public sealed class ExceptionTag : TypeVarDescTag
        {
            public const string Name1 = "@exception";
            public const string Name2 = "@throws";

            /// <summary>
            /// version
            /// </summary>
            public string Exception { get { return this.TypeNames; } }

            public ExceptionTag(string tagName, string/*!*/line)
                : base(tagName, line, false)
            {

            }

            public override string ToString()
            {
                return Name2 + " " + this.Exception;
            }
        }

        /// <summary>
        /// Documents any tag in a form of "type [$varname] [multilined-description]".
        /// </summary>
        public abstract class TypeVarDescTag : Element
        {
            /// <summary>
            /// Character separating type names within <see cref="TypeNames"/> property.
            /// </summary>
            public const char TypeNamesSeparator = '|';

            /// <summary>
            /// Character separating intersected type names.
            /// </summary>
            public const char TypeNamesIntersectionSeparator = '&';

            /// <summary>
            /// Optional. Type names separated by '|'.
            /// </summary>
            public string TypeNames
            {
                get
                {
                    var names = _typeNames;
                    if (names == null || names.Length == 0)
                        return null;

                    if (names.Length == 1)
                        return names[0];

                    return string.Join(TypeNamesSeparator.ToString(), names);
                }
            }

            /// <summary>
            /// Position of the <see cref="TypeNames"/> information.
            /// </summary>
            public Span TypeNamesSpan
            {
                get
                {
                    var positions = _typeNamesPos;
                    if (positions == null || positions.Length == 0)
                        return Span.Invalid;
                    var names = _typeNames;

                    var offset = this.Span.Start;
                    return Span.FromBounds(offset + positions[0], offset + positions[positions.Length - 1] + names[names.Length - 1].Length);
                }
            }

            /// <summary>
            /// Array of type names. Cannot be <c>null</c>. Can be an empty array.
            /// </summary>
            public string[]/*!!*/TypeNamesArray { get { return _typeNames; } }
            protected string[]/*!!*/_typeNames;

            /// <summary>
            /// Array of type names span within the source code.
            /// </summary>
            public Span[]/*!*/TypeNameSpans
            {
                get
                {
                    var positions = _typeNamesPos;
                    var names = _typeNames;
                    Debug.Assert(names.Length == positions.Length);
                    Span[] spans = new Span[positions.Length];
                    var offset = this.Span.Start;
                    for (int i = 0; i < spans.Length; i++)
                        spans[i] = new Span(offset + positions[i], names[i].Length);

                    return spans;
                }
            }
            protected int[]/*!!*/_typeNamesPos;

            /// <summary>
            /// Optional. Variable name, starts with '$'.
            /// </summary>
            public readonly string VariableName;

            /// <summary>
            /// Starting column of the <see cref="VariableName"/> within the element.
            /// </summary>
            private readonly int _variableNameOffset = -1;

            /// <summary>
            /// Position of the <see cref="VariableName"/> information.
            /// </summary>
            public Span VariableNameSpan
            {
                get
                {
                    if (this._variableNameOffset < 0)
                        return Span.Invalid;

                    Debug.Assert(this.VariableName != null);
                    return new Span(this.Span.Start + this._variableNameOffset, this.VariableName.Length);
                }
            }

            /// <summary>
            /// Optional. Element description.
            /// </summary>
            public string Description { get; protected set; }

            protected TypeVarDescTag(string/*!*/tagName, string/*!*/line, bool allowVariableName)
            {
                Debug.Assert(line.StartsWith(tagName));

                // [type] [$varname] [type] [description]

                int index = tagName.Length; // current index within line

                // try to find [type]
                TryReadTypeName(line, ref index, out _typeNames, out _typeNamesPos);

                if (allowVariableName)
                {
                    // try to find [$varname]
                    if (TryReadVariableName(line, ref index, out this.VariableName, out this._variableNameOffset))
                    {
                        // try to find [type] if it was not found yet, user may specified it after variable name
                        if (_typeNames == null || _typeNames.Length == 0)
                        {
                            TryReadTypeName(line, ref index, out _typeNames, out _typeNamesPos);
                        }
                    }
                }

                if (index < line.Length)
                {
                    this.Description = line.AsSpan(index).TrimStart().ToString();
                }
            }

            #region Helpers

            static ReadOnlySpan<char> NextTypeName(string text, ref int index)
            {
                // trim leading whitespace
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                {
                    index++;
                }

                int start = index;

                // TNAME1<A, B>|TNAME2
                int nested = 0;
                int rel = 0;
                int end = start;

                for (; end < text.Length; end++)
                {
                    rel++;
                    var c = text[end];

                    if (char.IsLetter(c) || c == '_' || c == QualifiedName.Separator) continue;
                    if (c == TypeNamesSeparator || c == TypeNamesIntersectionSeparator) { rel = 0; continue; }
                    if (rel == 1 && (c == '?')) continue;
                    if (rel > 1 && char.IsNumber(c)) continue;

                    if (c == '<' || c == '[') { nested++; rel = 0; continue; }
                    if ((c == '>' || c == ']') && nested > 0) { nested--; continue; }
                    if ((c == ' ' || c == '-' || c == ',') && nested > 0) continue; // allowed in generics // <int, int> or <array-key, int>

                    // not valid char
                    break;
                }

                //
                index = end;
                return text.AsSpan(start, end - start);
            }

            /// <summary>
            /// Tries to recognize a type name starting at given <paramref name="index"/>.
            /// </summary>
            /// <param name="text">Source text.</param>
            /// <param name="index">Index within <paramref name="text"/> to start read.</param>
            /// <param name="typenames">Resulting type name(s) separated by <c>|</c>.</param>
            /// <param name="typenamesPos">Type names span or invalid span.</param>
            /// <returns>Whether the type name was parsed.</returns>
            internal static bool TryReadTypeName(string/*!*/text, ref int index, out string[] typenames, out int[] typenamesPos)
            {
                // [type]

                var typenameend = index;
                var typename = NextTypeName(text, ref typenameend);
                if (typename.Length != 0)
                {
                    List<int> positions = new List<int>(1);
                    List<string> names = new List<string>(1);

                    int typenameOffset = typenameend - typename.Length;
                    index = typenameend;

                    while (typename.Length != 0)
                    {
                        int sep; // index of next separator | [0..typename.length]
                        int nested = 0; // level of <> or [] nesting
                        for (sep = 0; sep < typename.Length; sep++)
                        {
                            var ch = typename[sep];
                            if (ch == TypeNamesSeparator || ch == TypeNamesIntersectionSeparator)
                            {
                                if (nested == 0) break;
                            }
                            else if (ch == '<' || ch == '[')
                            {
                                nested++;
                            }
                            else if (ch == '>' || ch == ']')
                            {
                                nested--;
                            }
                        }

                        // name: [0..sep)
                        var name = typename.Slice(0, sep);
                        if (name.Length != 0)
                        {
                            names.Add(ToString(name));
                            positions.Add(typenameOffset);
                        }

                        typename = sep < typename.Length ? typename.Slice(sep + 1) : ReadOnlySpan<char>.Empty;
                        typenameOffset += sep + 1;
                    }

                    // [type1|type2] or [type3]
                    var orend = typenameend;
                    var maybeor = NextWord(text, ref orend);
                    if (maybeor.Equals("or".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        var nextend = orend;
                        var next = NextWord(text, ref nextend);
                        if (IsTypeName(next) && next.IndexOf(TypeNamesSeparator) == -1)
                        {
                            index = nextend;
                            names.Add(ToString(next));
                            positions.Add(nextend - next.Length);
                        }
                    }

                    typenames = ToArray(names);
                    typenamesPos = positions.ToArray();
                    return true;
                }

                //
                typenames = EmptyArray<string>.Instance;
                typenamesPos = EmptyArray<int>.Instance;
                return false;
            }

            private static string[] ToArray(List<string> names)
            {
                if (names.Count == 1)
                {
                    if (names[0] == CommonTypeNames.Void) return CommonTypeNames.VoidArray;
                    if (names[0] == CommonTypeNames.Mixed) return CommonTypeNames.MixedArray;
                    if (names[0] == CommonTypeNames.Never) return CommonTypeNames.NeverArray;
                }

                return names.ToArray();
            }

            private static string ToString(ReadOnlySpan<char> span)
            {
                if (span.Equals(CommonTypeNames.Void.AsSpan(), StringComparison.Ordinal)) return CommonTypeNames.Void;
                if (span.Equals(CommonTypeNames.Mixed.AsSpan(), StringComparison.Ordinal)) return CommonTypeNames.Mixed;
                if (span.Equals(CommonTypeNames.Never.AsSpan(), StringComparison.Ordinal)) return CommonTypeNames.Never;
                if (span.Equals(CommonTypeNames.Bool.AsSpan(), StringComparison.Ordinal)) return CommonTypeNames.Bool;

                return span.ToString();
            }

            /// <summary>
            /// tries to read a variable name starting at given <paramref name="index"/>.
            /// </summary>
            /// <param name="text">Source text.</param>
            /// <param name="index">Index within <paramref name="text"/> to start read.</param>
            /// <param name="variableName">Result variable name.</param>
            /// <param name="variableNameOffset">Variable name start index within text.</param>
            /// <returns>Whether the variable name was parsed.</returns>
            private static bool TryReadVariableName(string/*!*/text, ref int index, out string variableName, out int variableNameOffset)
            {
                var wordend = index;
                var word = NextWord(text, ref wordend);
                if (word.Length != 0 && word[0] == '$')
                {
                    index = wordend;
                    variableName = word.ToString();
                    variableNameOffset = wordend - word.Length;
                    return true;
                }

                variableName = null;
                variableNameOffset = -1;
                return false;
            }

            /// <summary>
            /// Checks whether given <paramref name="str"/> may be a type name.
            /// </summary>
            /// <param name="str">String to check.</param>
            /// <returns>Whether given string may be a PHP type name.</returns>
            internal static bool IsTypeName(ReadOnlySpan<char> str)
            {
                if (str.IsEmpty)
                {
                    return false;
                }

                if (str[0] != '_' && !char.IsLetter(str[0]) && str[0] != QualifiedName.Separator && str[0] != '?')
                {
                    return false;
                }

                const string allowedChars = @"_[]<>|&\";

                for (int i = 1; i < str.Length; i++)
                {
                    var c = str[i];
                    if (!char.IsLetterOrDigit(c) && allowedChars.IndexOf(c) < 0)
                    {
                        return false;
                    }
                }

                // ok
                return true;
            }

            #endregion

            internal override void ParseLine(ReadOnlySpan<char> line, out Element next)
            {
                next = null;

                // add the line into description:
                Description = string.IsNullOrWhiteSpace(Description) ? line.ToString() : (Description + NewLineString + line.ToString());
            }

            internal override void OnEndParsing()
            {
                base.OnEndParsing();

                if (string.IsNullOrWhiteSpace(this.Description))
                    this.Description = null;
                else
                    this.Description = this.Description.Trim();

                // TODO: compress TypeNames, VariableName, Description
            }

            internal override bool IsEmpty
            {
                get
                {
                    return _typeNames.Length == 0 && string.IsNullOrEmpty(this.VariableName) && string.IsNullOrWhiteSpace(this.Description);
                }
            }
        }

        public abstract class MethodRefTag : Element
        {
            public NameRef FunctionName => new NameRef(
                new Span(this.Span.Start + _functionNamePos, _functionName.Length),
                _functionName);

            string _functionName;
            int _functionNamePos;

            protected MethodRefTag(string tagName, string line)
            {
                int index = tagName.Length;

                // skip whitespaces:
                while (index < line.Length && char.IsWhiteSpace(line[index]))
                    index++;

                _functionNamePos = index;
                _functionName = NextWord(line, ref index).ToString();
            }

            internal override void ParseLine(ReadOnlySpan<char> line, out Element next)
            {
                next = null;
            }

            internal override bool IsEmpty => string.IsNullOrEmpty(_functionName);
        }

        public sealed class DataProviderTag : MethodRefTag
        {
            public const string Name = "@dataProvider";

            public DataProviderTag(string line)
                : base(Name, line)
            {
            }
        }

        /// <summary>
        /// Documents a global variable or its use in a function or method.
        /// @global	type $globalvarname
        /// </summary>
        public sealed class GlobalTag : TypeVarDescTag
        {
            public const string Name = "@global";

            public GlobalTag(string/*!*/line)
                : base(Name, line, true)
            {
            }

            public override string ToString()
            {
                string result = Name;

                var type = this.TypeNames;
                if (type != null)
                    result += " " + type;

                var varname = this.VariableName;
                if (varname != null)
                    result += " " + varname;

                return result;
            }
        }

        /// <summary>
        /// Prevents the documentation of an element.
        /// </summary>
        public sealed class IgnoreTag : EmptyTag
        {
            public const string Name = "@ignore";

            public override string ToString()
            {
                return Name;
            }
        }

        public abstract class TextTag : Element
        {
            /// <summary>
            /// Tag text information.
            /// </summary>
            public string Text { get; private set; }

            public TextTag(string/*!*/tagName, string/*!*/line)
            {
                Debug.Assert(line.StartsWith(tagName));
                int index = tagName.Length;

                if (index < line.Length)
                {
                    var c = line[index];
                    if (c == ':' || c == '(' || c == ';' || c == '.') index++;
                }

                // trim leading whitespaces
                while (index < line.Length && char.IsWhiteSpace(line[index]))
                    index++;

                this.Text = (index < line.Length) ? line.Substring(index) : string.Empty;
            }

            internal override void ParseLine(ReadOnlySpan<char> line, out Element next)
            {
                next = null;
                this.Text = string.IsNullOrEmpty(this.Text) ? line.ToString() : (this.Text + NewLineString + line.ToString());
            }

            internal override void OnEndParsing()
            {
                base.OnEndParsing();

                if (string.IsNullOrWhiteSpace(this.Text))
                    this.Text = string.Empty;
                else
                    this.Text = this.Text.Trim();
            }
        }

        /// <summary>
        /// Represents an unknown PHPDoc tag followed by text.
        /// </summary>
        public sealed class UnknownTextTag : TextTag
        {
            /// <summary>
            /// Tag name.
            /// </summary>
            public string TagName { get; private set; }

            internal UnknownTextTag(string tagName, string/*!*/line)
                : base(tagName, line)
            {
                this.TagName = tagName;
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(Text) ? (TagName) : (TagName + " " + Text);
            }
        }

        /// <summary>
        /// Private information for advanced developers.
        /// </summary>
        public sealed class InternalTag : TextTag
        {
            public const string Name = "@internal";

            public InternalTag(string/*!*/line)
                : base(Name, line)
            {
            }

            public override string ToString()
            {
                return Name + " " + Text;
            }
        }

        ///// <summary>
        ///// URL information.
        ///// </summary>
        //public sealed class LinkTag : SingleLineTag
        //{
        //    public const string Name = "@link";

        //    /// <summary>
        //    /// URL
        //    /// </summary>
        //    public string Url { get { return this.text; } }

        //    public LinkTag(string/*!*/line)
        //        :base(Name, line)
        //    {

        //    }
        //}

        /// <summary>
        /// Specifies an alias for a variable. For example, $GLOBALS['myvariable'] becomes $myvariable.
        /// </summary>
        public sealed class NameTag : SingleLineTag
        {
            public const string Name = "@name";

            /// <summary>
            /// Variable name. Empty string or a name starting with '$' character.
            /// </summary>
            public string VariableName { get { return string.IsNullOrEmpty(this.text) ? string.Empty : ((this.text[0] == '$') ? this.text : ('$' + this.text)); } }

            public NameTag(string/*!*/line)
                : base(Name, line)
            {

            }
        }

        /// <summary>
        /// phpdoc.de compatibility "phpDocumentor tags".
        /// </summary>
        public sealed class MagicTag : EmptyTag
        {
            public const string Name = "@magic";

            public override string ToString()
            {
                return Name;
            }
        }

        /// <summary>
        /// Documents a group of related classes and functions.
        /// </summary>
        public sealed class PackageTag : SingleLineTag
        {
            public const string Name = "@package";

            /// <summary>
            /// Name of the package.
            /// </summary>
            public string PackageName { get { return this.text; } }

            public PackageTag(string/*!*/line)
                : base(Name, line)
            {

            }
        }

        /// <summary>
        /// Documents a group of related classes and functions within a package.
        /// </summary>
        public sealed class SubPackageTag : SingleLineTag
        {
            public const string Name = "@subpackage";

            /// <summary>
            /// Name of the sub-package.
            /// </summary>
            public string SubPackageName { get { return this.text; } }

            public SubPackageTag(string/*!*/line)
                : base(Name, line)
            {

            }
        }

        /// <summary>
        /// Documents a parameter.
        /// @param type [$varname] description
        /// </summary>
        public sealed class ParamTag : TypeVarDescTag
        {
            public const string Name1 = "@param";
            public const string Name2 = "@psalm-param";

            public ParamTag(string tagName, string/*!*/line)
                : base(tagName, line, true)
            {
            }

            public override string ToString()
            {
                var result = StringUtils.GetStringBuilder();

                result.Append(Name1);

                if (this.TypeNames != null)
                {
                    result.Append(' ');
                    result.Append(this.TypeNames);
                }
                if (this.VariableName != null)
                {
                    result.Append(' ');
                    result.Append(this.VariableName);
                }
                if (this.Description != null)
                {
                    result.Append(' ');
                    result.Append(this.Description);
                }
                //
                return StringUtils.ReturnStringBuilder(result);
            }
        }

        /// <summary>
        /// Documents function return value. This tag should not be used for constructors or methods defined with a void return type
        /// @return type [description]
        /// </summary>
        public sealed class ReturnTag : TypeVarDescTag
        {
            public const string Name = "@return";

            public ReturnTag(string/*!*/line)
                : base(Name, line, false)
            {
            }

            public override string ToString()
            {
                return Name + " " + TypeNames + NewLineString + Description;
            }
        }

        /// <summary>
        /// Documents function return value.
        /// @psalm-return type [description]
        /// </summary>
        public sealed class PsalmReturnTag : TypeVarDescTag
        {
            public const string Name = "@psalm-return";

            public PsalmReturnTag(string/*!*/line)
                : base(Name, line, false)
            {
            }

            public override string ToString()
            {
                return Name + " " + TypeNames + NewLineString + Description;
            }
        }

        /// <summary>
        /// Documents an association to any element (global variable, include, page, class, function, define, method, variable).
        /// </summary>
        public sealed class SeeTag : SingleLineTag
        {
            public const string Name = "@see";

            /// <summary>
            /// element
            /// </summary>
            public string ElementName { get { return this.text; } }

            public SeeTag(string/*!*/line)
                : base(Name, line)
            {

            }
        }

        /// <summary>
        /// Documents when a method was added to a class.
        /// </summary>
        public sealed class SinceTag : SingleLineTag
        {
            public const string Name = "@since";

            /// <summary>
            /// version
            /// </summary>
            public string Version { get { return this.text; } }

            public SinceTag(string/*!*/line)
                : base(Name, line)
            {

            }
        }

        /// <summary>
        /// Documents a static class or method.
        /// </summary>
        public sealed class StaticTag : EmptyTag
        {
            public const string Name = "@static";

            public override string ToString()
            {
                return Name;
            }
        }

        /// <summary>
        /// Documents a static variable's use in a function or class.
        /// </summary>
        public sealed class StaticVarTag : TypeVarDescTag
        {
            public const string Name = "@staticvar";

            public StaticVarTag(string/*!*/line)
                : base(Name, line, false)
            {

            }

            public override string ToString()
            {
                return Name + " " + this.TypeNames;
            }
        }

        /// <summary>
        /// Documents things that need to be done to the code at a later date.
        /// </summary>
        public sealed class TodoTag : TextTag
        {
            public const string Name = "@todo";

            public TodoTag(string/*!*/line)
                : base(Name, line)
            {
            }

            public override string ToString()
            {
                return Name + " " + Text;
            }
        }

        /// <summary>
        /// Documents a link to an external source.
        /// </summary>
        public sealed class LinkTag : TextTag
        {
            public const string Name = "@link";

            public LinkTag(string/*!*/line)
                : base(Name, line)
            {
            }

            public override string ToString()
            {
                return Name + " " + Text;
            }
        }

        /// <summary>
        /// Documents a license information.
        /// </summary>
        public sealed class LicenseTag : TextTag
        {
            public const string Name = "@license";

            public LicenseTag(string/*!*/line)
                : base(Name, line)
            {
            }

            public override string ToString()
            {
                return Name + " " + Text;
            }
        }

        public sealed class VarTag : TypeVarDescTag
        {
            public const string Name = "@var";

            public VarTag(string/*!*/line)
                : base(Name, line, true)
            {
            }

            public override string ToString()
            {
                return Name + " " + this.TypeNames;
            }
        }

        public sealed class MixinTag : TypeVarDescTag
        {
            public const string Name = "@mixin";

            public MixinTag(string/*!*/line)
                : base(Name, line, false)
            {
            }
        }

        /// <summary>
        /// Dynamic property description within a class.
        /// </summary>
        public sealed class PropertyTag : TypeVarDescTag
        {
            public const string Name1 = "@property";
            public const string Name2 = "@property-read";
            public const string Name3 = "@property-write";

            public PropertyTag(string tagName, string/*!*/line)
                : base(tagName, line, true)
            {
            }

            public override string ToString()
            {
                return Name1 + " " + this.TypeNames;
            }
        }

        /// <summary>
        /// Dynamic method description within a class.
        /// </summary>
        public sealed class MethodTag : Element
        {
            public const string Name = "@method";

            /// <summary>Static keyword.</summary>
            const string StaticModifierString = "static";

            enum MethodFlags : byte
            {
                IsStatic = 1,
                ReturnsStatic = 2,
            }

            private MethodFlags _flags;

            /// <summary>
            /// Optional. Type names separated by '|'.
            /// </summary>
            public string TypeNames
            {
                get
                {
                    var names = _typeNames;
                    if (names == null || names.Length == 0)
                        return null;

                    return string.Join(TypeVarDescTag.TypeNamesSeparator.ToString(), names);
                }
            }

            /// <summary>
            /// Array of type names. Cannot be <c>null</c>. Can be an empty array.
            /// </summary>
            public string[]/*!!*/TypeNamesArray { get { return _typeNames; } }
            private readonly string[]/*!*/_typeNames;

            /// <summary>
            /// Array of type names span within the source code.
            /// </summary>
            public Span[]/*!*/TypeNameSpans
            {
                get
                {
                    var positions = _typeNamesPos;
                    var names = _typeNames;
                    Debug.Assert(names.Length == positions.Length);
                    Span[] spans = new Span[positions.Length];
                    var offset = this.Span.Start;
                    for (int i = 0; i < spans.Length; i++)
                        spans[i] = new Span(offset + positions[i], names[i].Length);

                    return spans;
                }
            }
            private readonly int[]/*!*/_typeNamesPos;

            /// <summary>
            /// Array of method parameters;
            /// </summary>
            public FormalParam[]/*!*/Parameters
            {
                get
                {
                    if (_parameters == null)
                    {
                        return EmptyArray<FormalParam>.Instance;
                    }

                    return _parameters;
                }
            }
            readonly FormalParam[]/*!*/_parameters;

            /// <summary>
            /// Method name.
            /// </summary>
            public readonly string MethodName;

            /// <summary>
            /// Whether the method was declared with `static` keyword.
            /// </summary>
            public bool IsStatic => (_flags & MethodFlags.IsStatic) != 0;

            /// <summary>
            /// Whether the method was declared to return itself (returns $this, declared as returning static)
            /// </summary>
            public bool ReturnsStatic => (_flags & MethodFlags.ReturnsStatic) != 0;

            /// <summary>
            /// Span within the source code of the method name.
            /// </summary>
            public Span MethodNameSpan
            {
                get
                {
                    var pos = _methodNamePos;
                    if (pos < 0)
                        return Span.Invalid;
                    Debug.Assert(MethodName != null);
                    return new Span(pos + this.Span.Start, this.MethodName.Length);
                }
            }
            private readonly int _methodNamePos;

            /// <summary>
            /// Optional. Element description.
            /// </summary>
            public string Description { get; private set; }

            public MethodTag(string/*!*/tagName, string/*!*/line)
            {
                Debug.Assert(line.StartsWith(tagName));

                _methodNamePos = -1;

                // [static] [type] [name()] [name(params ...)] [static] [description]

                int index = tagName.Length; // current index within line
                int descStart = index;  // start of description, moved when [type] or [name] found

                // try read `static`
                if (TryReadStatic(line, ref index))
                {
                    _flags |= MethodFlags.IsStatic;
                }

                // try to find [type]
                if (!TypeVarDescTag.TryReadTypeName(line, ref index, out _typeNames, out _typeNamesPos))
                {
                    // or [$this]
                    if (TryReadThis(line, ref index))
                    {
                        _flags |= MethodFlags.ReturnsStatic;
                    }
                }

                descStart = index;
                var word = NextWord(line, ref index);

                // [name()]
                if (word.Length != 0 && word.EndsWith("()".AsSpan(), StringComparison.Ordinal))
                {
                    this.MethodName = word.Slice(0, word.Length - 2).ToString();
                    _methodNamePos = index - word.Length;
                    descStart = index;
                    word = NextWord(line, ref index);
                }

                // [name(params ...)]
                while (descStart < line.Length && char.IsWhiteSpace(line[descStart]))
                    descStart++;    // skip whitespaces

                int nameStart = descStart;
                int paramsFrom = -1;
                // skip [name]
                while (descStart < line.Length && char.IsLetterOrDigit(line[descStart]))
                    descStart++;

                // parse parameters
                if (descStart < line.Length && line[descStart] == '(')
                {
                    paramsFrom = descStart;
                    if (nameStart < paramsFrom)
                    {
                        if (this.MethodName == null)
                            this.MethodName = line.Substring(nameStart, paramsFrom - nameStart);
                        _methodNamePos = nameStart;
                    }
                }
                else
                {
                    descStart = nameStart;
                }

                if (string.IsNullOrEmpty(this.MethodName))
                    return;

                if (paramsFrom > 0 && paramsFrom < line.Length && line[paramsFrom] == '(')
                {
                    // "name(" found
                    int paramsEnd = line.IndexOf(')', paramsFrom);
                    if (paramsEnd > 0)
                    {
                        descStart = paramsEnd + 1;
                        var offset = paramsFrom + 1;
                        var paramsDecl = line.AsSpan(offset, paramsEnd - offset);
                        var ps = new List<FormalParam>();

                        while (paramsDecl.Length != 0 && !paramsDecl.IsWhiteSpace())
                        {
                            var comma = paramsDecl.IndexOf(',');
                            if (comma < 0) comma = paramsDecl.Length;

                            // parse parameter
                            ps.Add(ParseParam(paramsDecl.Slice(0, comma), offset));

                            // move next
                            paramsDecl = comma < paramsDecl.Length ? paramsDecl.Slice(comma + 1) : ReadOnlySpan<char>.Empty;
                            offset += comma + 1;
                        }

                        if (ps.Count != 0)
                        {
                            _parameters = ps.ToArray();
                        }
                    }
                }

                // [static] after the parameters?
                if (!IsStatic && TryReadStatic(line, ref descStart))
                {
                    _flags |= MethodFlags.IsStatic;
                }

                if (descStart < line.Length)
                    this.Description = line.AsSpan(descStart).TrimStart().ToString();
            }

            private static bool TryReadStatic(string line, ref int index)
            {
                int index2 = index;

                var word = NextWord(line, ref index2);
                if (word.Equals(StaticModifierString.AsSpan(), StringComparison.Ordinal))
                {
                    index = index2;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private static bool TryReadThis(string line, ref int index)
            {
                int index2 = index;

                var word = NextWord(line, ref index2);
                if (word.Equals("$this".AsSpan(), StringComparison.Ordinal))
                {
                    index = index2;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Parses parameter description in a form of [type][$name][=initializer].
            /// </summary>
            /// <param name="paramDecl"></param>
            /// <param name="offset">Span offset.</param>
            /// <returns></returns>
            private static FormalParam/*!*/ParseParam(ReadOnlySpan<char>/*!*/paramDecl, int offset)
            {
                var span = new Span(offset, paramDecl.Length);

                TypeRef typehint = null;
                string paramname = string.Empty;
                Span paramnamespan = Span.Invalid;
                Expression initExpr = null;
                var flags = FormalParam.Flags.Default;

                // Trim [=initializer] off
                int eqIndex = paramDecl.IndexOf('=');
                if (eqIndex >= 0)
                {
                    initExpr = TryParseInitValue(Span.Invalid, paramDecl.Slice(eqIndex + 1).Trim());
                    paramDecl = paramDecl.Slice(0, eqIndex);
                }

                int i = 0;
                var word = NextWord(paramDecl, ref i);
                if (word.Length != 0)
                {
                    // [type]
                    if (word.Length != 0 && word[0] != '$' && word[0] != '&' && word[0] != '.')
                    {
                        typehint = TypeRef.FromString(new Span(offset + i - word.Length, word.Length), word.ToString());    // TODO: naming
                        word = NextWord(paramDecl, ref i);
                    }

                    // [...]
                    if (word.StartsWith("...".AsSpan(), StringComparison.Ordinal))
                    {
                        flags |= FormalParam.Flags.IsVariadic;

                        word = word.Slice(3).Trim();
                        if (word.Length == 0)
                        {
                            word = NextWord(paramDecl, ref i);
                        }
                    }

                    // [&]
                    if (word.Length != 0 && word[0] == '&')
                    {
                        flags |= FormalParam.Flags.IsByRef;

                        word = word.Slice(1).TrimStart();
                        if (word.Length == 0)
                        {
                            word = NextWord(paramDecl, ref i);
                        }
                    }

                    // [$name]
                    if (word.Length != 0 && word[0] == '$')
                    {
                        var wordstart = i - word.Length;

                        eqIndex = word.IndexOf('=');
                        if (eqIndex >= 0) word = word.Slice(0, eqIndex);

                        if (word.Length != 0)
                        {
                            paramnamespan = new Span(offset + wordstart, word.Length);

                            if (word[0] == '$' || word[0] == '&') word = word.Slice(1);

                            paramname = word.Trim().ToString();
                        }
                    }
                }

                return new FormalParam(span, paramname, paramnamespan, typehint, flags, initExpr)
                {
                    //ContainingElement = ...,
                };
            }

            /// <summary>
            /// Parses a default parameter value from a string.
            /// </summary>
            /// <param name="span">Value position within source code.</param>
            /// <param name="rvalue">Parameter initializer as it is in PHPDoc string.</param>
            /// <returns>Optional parsed expression. Returns <c>null</c> is value could not be parsed or is empty.</returns>
            private static Expression TryParseInitValue(Span span, ReadOnlySpan<char> rvalue)
            {
                if (rvalue.Length == 0)
                {
                    return null;
                }

                var value = rvalue.ToString(); // TODO: NETSTANDARD2.1

                if (value == "[]" || value == "array()")
                {
                    return ArrayEx.CreateArray(span, EmptyArray<Item>.Instance, value == "[]");
                }
                else if (long.TryParse(value, out var l))
                {
                    return new LongIntLiteral(span, l);
                }
                else if (double.TryParse(value, out var d))
                {
                    return new DoubleLiteral(span, d);
                }
                else if (bool.TryParse(value, out var b))
                {
                    return new BoolLiteral(span, b);
                }
                else if (value == "\"\"" || value == "''")
                {
                    return new StringLiteral(span, string.Empty);
                }
                else if (value.Length >= 2 && value[0] == value[value.Length - 1] && (value[0] == '"' || value[0] == '\''))
                {
                    return new StringLiteral(span, value.Substring(1, value.Length - 2)); // TODO: Lexer.ProcessString
                }
                else if ((char.IsLetter(value[0]) || value[0] == '_'))
                {
                    return new GlobalConstUse(span, new TranslatedQualifiedName(QualifiedName.Parse(value, false), span));
                }
                else
                {
                    Debug.WriteLine("PHPDoc @method tag parameter init value {0} was not handled.", value);
                }

                return null;
            }

            internal override void ParseLine(ReadOnlySpan<char> line, out Element next)
            {
                next = null;

                // add the line into description:
                Description = string.IsNullOrWhiteSpace(Description) ? line.ToString() : (Description + NewLineString + line.ToString());
            }

            internal override void OnEndParsing()
            {
                base.OnEndParsing();

                if (string.IsNullOrWhiteSpace(this.Description))
                    this.Description = null;
                else
                    this.Description = this.Description.Trim();
            }

            internal override bool IsEmpty
            {
                get
                {
                    return _typeNames.Length == 0 && string.IsNullOrEmpty(this.MethodName) && string.IsNullOrWhiteSpace(this.Description);
                }
            }

            public override string ToString()
            {
                return Name + " " + this.MethodName + "()\n" + this.Description;
            }
        }

        public sealed class VersionTag : SingleLineTag
        {
            public const string Name = "@version";

            public string Version { get { return this.text; } }

            public VersionTag(string/*!*/line)
                : base(Name, line)
            {
            }

            public override string ToString()
            {
                return Name + " " + this.Version;
            }
        }

        public sealed class TestTag : EmptyTag
        {
            public const string Name = "@test";

            public TestTag(string/*!*/line)
                : base()
            {
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public sealed class GroupTag : SingleLineTag
        {
            public const string Name = "@group";

            public string Group { get { return this.text; } }

            public GroupTag(string/*!*/line)
                : base(Name, line)
            {
            }

            public override string ToString()
            {
                return Name + " " + this.Group;
            }
        }

        public sealed class TemplateTag : TypeVarDescTag
        {
            public const string Name1 = "@template";
            public const string Name2 = "@psalm-template";

            /// <summary>
            /// The template identifier.
            /// </summary>
            public NameRef Identifier => string.IsNullOrEmpty(_identifier)
                ? default(NameRef)
                : new NameRef(new Span(this.Span.Start + _identifierStart, _identifier.Length), _identifier);

            readonly string _identifier;
            readonly int _identifierStart;

            public TemplateTag(string tagName, string/*!*/line)
                : base(tagName, tagName/*do not consume the following type here*/, false) // empty line
            {
                // @template {Identifier} of {TypeNames}
                Debug.Assert(line.StartsWith(tagName));

                int index = tagName.Length; // current index within line

                var identifier = NextWord(line, ref index);

                _identifier = identifier.ToString();
                _identifierStart = index - identifier.Length;
                
                var of = NextWord(line, ref index);
                if (of.Equals("of".AsSpan(), StringComparison.Ordinal))
                {
                    TryReadTypeName(line, ref index, out _typeNames, out _typeNamesPos);
                }

                if (index < line.Length)
                {
                    this.Description = line.AsSpan(index).TrimStart().ToString();
                }
            }

            internal override bool IsEmpty => string.IsNullOrEmpty(_identifier);

            public override string ToString()
            {
                var sb = StringUtils.GetStringBuilder();

                sb.Append(Name);
                
                if (_identifier.Length != 0)
                {
                    sb.Append(' ');
                    sb.Append(_identifier);
                }

                if (_typeNames.Length != 0)
                {
                    sb.Append(" of ");
                    sb.Append(this.TypeNames);
                }

                if (!string.IsNullOrWhiteSpace(Description))
                {
                    sb.Append(' ');
                    sb.Append(Description);
                }

                //
                return StringUtils.ReturnStringBuilder(sb);
            }
        }

        #endregion

        #region Nested struct: CommonTypeNames

        struct CommonTypeNames
        {
            public static string Bool => "bool";
            public static string Void => "void";
            public static string Mixed => "mixed";
            public static string Never => "never";

            public static readonly string[] VoidArray = new[] { Void };
            public static readonly string[] MixedArray = new[] { Mixed };
            public static readonly string[] NeverArray = new[] { Never };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Original PHPDoc text, including comment tags.
        /// </summary>
        /// <remarks>Used internally for lazy initialization.</remarks>
        string _docCommentString;

        /// <summary>
        /// Parsed data. Lazily initialized.
        /// </summary>
        private Element[] _elements;

        /// <summary>
        /// Elements within the PHPDoc block. Some elements may be ignored due to missing information.
        /// Cannot be <c>null</c> reference.
        /// </summary>
        public Element[]/*!*/Elements
        {
            get
            {
                if (_elements == null)
                {
                    var phpdocstring = _docCommentString;
                    if (phpdocstring != null)
                    {
                        Interlocked.CompareExchange(ref _elements, ParseNoLock(phpdocstring, this.Span.Start), null);
                        _docCommentString = null;
                    }

                    Debug.Assert(_elements != null);

                    // dispose the string,
                    // not needed anymore
                }

                return _elements;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes new instance of <see cref="PHPDocBlock"/>.
        /// </summary>
        /// <param name="doccomment">PHPDoc token content.</param>
        /// <param name="span">Position of the comment in the source code.</param>
        public PHPDocBlock(string doccomment, Span span) // TODO: NETSTANDARD21 ReadOnlySpan<char>
            : base(span)
        {
            this._docCommentString = doccomment;
        }

        /// <summary>
        /// Parses given <paramref name="doccomment"/> into a list of <see cref="Element"/> instances.
        /// </summary>
        /// <param name="doccomment">Content of the PHPDoc token.</param>
        /// <param name="offset">Start position of <paramref name="doccomment"/> within the source code.</param>
        private static Element[]/*!*/ParseNoLock(string/*!*/doccomment, int offset)
        {
            if (string.IsNullOrEmpty(doccomment))
            {
                return EmptyArray<Element>.Instance;
            }

            // initialize line endings information
            var/*!*/lineBreaks = LineBreaks.Create(doccomment);

            //
            var result = new List<Element>();

            Element/*!*/current = new ShortDescriptionElement();
            current.Span = Span.Invalid;

            for (int lineIndex = 0; lineIndex < lineBreaks.LinesCount; lineIndex++)
            {
                var lineSpan = lineBreaks.GetLineSpan(lineIndex);
                var line = doccomment.AsSpan(lineSpan.Start, lineSpan.Length);

                if (Element.TryParseLine(ref line, out var tmp, lineIndex, out var lineOffset))    // validate the line, process tags
                {
                    Debug.Assert(line != null);

                    if (tmp == null)    // no new element created
                    {
                        // pass the line into the current element
                        current.ParseLine(line, out tmp);

                        // update position of the element
                        if (current.Span.IsValid == false)      // ShortDescriptionElement has not initialized Span
                        {
                            if (!current.IsEmpty)   // initialize Start iff element has some text
                                current.Span = new Span(offset + lineSpan.Start + lineOffset, line.Length);
                        }
                        else                                    // other elements has to update their end position
                        {
                            if (tmp != null)
                                current.Span = Span.FromBounds(current.Span.Start, offset + lineSpan.Start + lineOffset + line.Length);   // update its end position                        
                        }
                    }

                    if (tmp != null)    // new element created, it is already initialized with the current line
                    {
                        if (!current.IsEmpty)
                        {
                            current.OnEndParsing();
                            result.Add(current);
                        }

                        tmp.Span = new Span(offset + lineSpan.Start + lineOffset, line.Length);
                        current = tmp;  // it is current element from now
                    }
                }
            }

            // add the last found element
            if (!current.IsEmpty)
            {
                current.OnEndParsing();
                result.Add(current);
            }

            //
            return result.Count != 0 ? result.ToArray() : EmptyArray<Element>.Instance;
        }

        #endregion

        #region Helper access methods

        public T GetElement<T>() where T : Element
        {
            var elements = this.Elements;
            for (int i = 0; i < elements.Length; i++)
                if (elements[i] is T e)
                    return e;

            return null;
        }

        /// <summary>
        /// Enumerate all the '@param' tags.
        /// </summary>
        public IEnumerable<ParamTag> Params
        {
            get
            {
                return this.Elements.OfType<ParamTag>();
            }
        }

        /// <summary>
        /// Gets '@return' tag or <c>null</c>.
        /// </summary>
        public ReturnTag Returns
        {
            get
            {
                return GetElement<ReturnTag>();
            }
        }

        /// <summary>
        /// Whether the PHPDoc block contains '@ignore' tag.
        /// </summary>
        public bool IsIgnored
        {
            get
            {
                return GetElement<IgnoreTag>() != null;
            }
        }

        /// <summary>
        /// Gets short description or <c>null</c>.
        /// </summary>
        public string ShortDescription
        {
            get
            {
                return GetElement<ShortDescriptionElement>()?.Text;
            }
        }

        /// <summary>
        /// Gets long description or <c>null</c>.
        /// </summary>
        public string LongDescription
        {
            get
            {
                return GetElement<LongDescriptionElement>()?.Text;
            }
        }

        /// <summary>
        /// Gets whole description, as a concatenation of <see cref="ShortDescription"/> and <see cref="LongDescription"/>.
        /// </summary>
        public string Summary
        {
            get
            {
                var shortdesc = ShortDescription;
                var longdesc = LongDescription;

                if (shortdesc != null || longdesc != null)
                {
                    if (string.IsNullOrEmpty(shortdesc))
                        return longdesc;

                    if (string.IsNullOrEmpty(longdesc))
                        return shortdesc;

                    return shortdesc + "\n" + longdesc;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets '@access' value or 'Public' if no such tag is found.
        /// </summary>
        public PhpMemberAttributes Access
        {
            get
            {
                var access = GetElement<AccessTag>();
                return (access != null) ? access.Access : PhpMemberAttributes.Public;
            }
        }

        /// <summary>
        /// Reconstructs PHPDoc block from parsed elements, including comment tags.
        /// </summary>
        public string PHPDocPreview
        {
            get
            {
                var result = StringUtils.GetStringBuilder();
                result.AppendLine("/**");

                foreach (var element in this.Elements)
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

        #region ToString

        /// <summary>
        /// Returns summary of PHPDoc.
        /// </summary>
        public override string ToString()
        {
            return this.Summary;
        }

        #endregion

        #region LangElement

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitPHPDocBlock(this);
        }

        #endregion
    }

    internal static class PHPDocBlockHelper
    {
        /// <summary>
        /// Gets <see cref="PHPDocBlock"/> associated with <paramref name="properties"/>.
        /// </summary>
        public static PHPDocBlock GetPHPDoc(this IPropertyCollection/*!*/properties)
        {
            return properties.GetProperty<PHPDocBlock>();
        }

        /// <summary>
        /// Sets <see cref="PHPDocBlock"/> to <paramref name="properties"/>.
        /// </summary>
        public static void SetPHPDoc(this IPropertyCollection/*!*/properties, PHPDocBlock phpdoc)
        {
            if (phpdoc != null)
            {
                properties.SetProperty<PHPDocBlock>(phpdoc);

                // remember LangElement associated with phpdoc
                var element = properties as LangElement;
                if (element != null)
                    phpdoc.SetProperty<LangElement>(element);
            }
            else
            {
                properties.RemoveProperty<PHPDocBlock>();
            }
        }
    }
}
