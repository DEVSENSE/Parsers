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
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Devsense.PHP.Text;
using Devsense.PHP.Utilities;

namespace Devsense.PHP.Syntax
{
    // 
    //  Identifier            Representation
    // --------------------------------------------------------------------
    //  variable, field       VariableName     (case-sensitive)
    //  class constant        VariableName     (case-sensitive)
    //  namespace constant    QualifiedName    (case-sensitive)
    //  method                Name             (case-insensitive)
    //  class, function       QualifiedName    (case-insensitive)
    //  primitive type        PrimitiveTypeName(case-insensitive)
    //  namespace component   Name             (case-sensitive?)
    //  label                 VariableName     (case-sensitive?)
    //

    #region Name

    /// <summary>
    /// Case-insensitive culture-sensitive (TODO ???) simple name in Unicode C normal form.
    /// Used for names of methods and namespace components.
    /// </summary>
    [DebuggerNonUserCode]
    public readonly struct Name : IEquatable<Name>, IEquatable<string>
    {
        public string/*!*/ Value => this.value;
        private readonly string/*!*/ value;
        private readonly int hashCode;

        public static StringComparer Comparer => StringComparer.OrdinalIgnoreCase;

        #region Special Names

        public static readonly Name[] EmptyNames = EmptyArray<Name>.Instance;
        public static readonly Name EmptyBaseName = new Name("");
        public static readonly Name SelfClassName = new Name("self");
        public static readonly Name StaticClassName = new Name("static");
        public static readonly Name ParentClassName = new Name("parent");
        public static readonly Name AutoloadName = new Name("__autoload");
        public static readonly Name ClrCtorName = new Name(".ctor");
        public static readonly Name ClrInvokeName = new Name("Invoke"); // delegate Invoke method
        public static readonly Name AppStaticName = new Name("AppStatic");
        public static readonly Name AppStaticAttributeName = new Name("AppStaticAttribute");
        public static readonly Name ExportName = new Name("Export");
        public static readonly Name ExportAttributeName = new Name("ExportAttribute");
        public static readonly Name DllImportAttributeName = new Name("DllImportAttribute");
        public static readonly Name DllImportName = new Name("DllImport");
        public static readonly Name OutAttributeName = new Name("OutAttribute");
        public static readonly Name OutName = new Name("Out");
        public static readonly Name DeclareHelperName = new Name("<Declare>");
        public static readonly Name LambdaFunctionName = new Name("<Lambda>");
        public static readonly Name ClosureFunctionName = new Name("{closure}");
        public static readonly Name AnonymousClassName = new Name("class@anonymous");

        #region SpecialMethodNames

        /// <summary>
        /// Contains special (or &quot;magic&quot;) method names.
        /// </summary>
        public static class SpecialMethodNames
        {
            /// <summary>Constructor.</summary>
            public static readonly Name Construct = new Name("__construct");

            /// <summary>Destructor.</summary>
            public static readonly Name Destruct = new Name("__destruct");

            /// <summary>Invoked when cloning instances.</summary>
            public static readonly Name Clone = new Name("__clone");

            /// <summary>Invoked when casting to string.</summary>
            public static readonly Name Tostring = new Name("__tostring");

            /// <summary>Invoked when serializing instances.</summary>
            public static readonly Name Sleep = new Name("__sleep");

            /// <summary>Invoked when deserializing instanced.</summary>
            public static readonly Name Wakeup = new Name("__wakeup");

            /// <summary>Invoked when an unknown field is read.</summary>
            public static readonly Name Get = new Name("__get");

            /// <summary>Invoked when an unknown field is written.</summary>
            public static readonly Name Set = new Name("__set");

            /// <summary>Invoked when an unknown method is called.</summary>
            public static readonly Name Call = new Name("__call");

            /// <summary>Invoked when an object is called like a function.</summary>
            public static readonly Name Invoke = new Name("__invoke");

            /// <summary>Invoked when an unknown method is called statically.</summary>
            public static readonly Name CallStatic = new Name("__callStatic");

            /// <summary>Invoked when an unknown field is unset.</summary>
            public static readonly Name Unset = new Name("__unset");

            /// <summary>Invoked when an unknown field is tested for being set.</summary>
            public static readonly Name Isset = new Name("__isset");

            /// <summary>Invoked when object is being serialized.</summary>
            public static readonly Name Serialize = new Name("__serialize");

            /// <summary>Invoked when object is being unserialized.</summary>
            public static readonly Name Unserialize = new Name("__unserialize");
        };

        #endregion

        /// <summary>
        /// Name suffix of attribute class name.
        /// </summary>
        internal const string AttributeNameSuffix = "Attribute";

        public bool IsCloneName
        {
            get { return this.Equals(SpecialMethodNames.Clone); }
        }

        public bool IsConstructName
        {
            get { return this.Equals(SpecialMethodNames.Construct); }
        }

        public bool IsDestructName
        {
            get { return this.Equals(SpecialMethodNames.Destruct); }
        }

        public bool IsCallName
        {
            get { return this.Equals(SpecialMethodNames.Call); }
        }

        public bool IsCallStaticName
        {
            get { return this.Equals(SpecialMethodNames.CallStatic); }
        }

        public bool IsToStringName
        {
            get { return this.Equals(SpecialMethodNames.Tostring); }
        }

        public bool IsParentClassName
        {
            get { return this.Equals(Name.ParentClassName); }
        }

        public bool IsSelfClassName
        {
            get { return this.Equals(Name.SelfClassName); }
        }

        public bool IsStaticClassName
        {
            get { return this.Equals(Name.StaticClassName); }
        }

        public bool IsReservedClassName
        {
            get { return IsParentClassName || IsSelfClassName || IsStaticClassName; }
        }

        /// <summary>
        /// <c>true</c> if the name was generated for the 
        /// <see cref="Devsense.PHP.Syntax.Ast.AnonymousTypeDecl"/>, 
        /// <c>false</c> otherwise.
        /// </summary>
        public bool IsGenerated
        {
            get { return value.StartsWith(AnonymousClassName.Value); }
        }

        #endregion

        /// <summary>
        /// Creates a name. 
        /// </summary>
        /// <param name="value">The name shouldn't be <B>null</B>.</param>
        public Name(string/*!*/ value)
        {
            Debug.Assert(value != null);
            this.value = value;
            this.hashCode = Comparer.GetHashCode(value);
        }

        #region Utils

        /// <summary>
        /// Separator of class name and its static field in a form of <c>CLASS::MEMBER</c>.
        /// </summary>
        public const string ClassMemberSeparator = "::";

        /// <summary>
        /// Splits the <paramref name="value"/> into class name and member name if it is double-colon separated.
        /// </summary>
        /// <param name="value">Full name.</param>
        /// <param name="className">Will contain the class name fragment if the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>. Otherwise <c>null</c>.</param>
        /// <param name="memberName">Will contain the member name fragment if the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>. Otherwise it contains original <paramref name="value"/>.</param>
        /// <returns>True iff the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.</returns>
        public static bool IsClassMemberSyntax(string/*!*/value, out string className, out string memberName)
        {
            Debug.Assert(value != null);
            //Debug.Assert(QualifiedName.Separator.ToString() == ":::" && !value.Contains(QualifiedName.Separator.ToString())); // be aware of deprecated namespace syntax

            int separator;
            if ((separator = value.IndexOf(':')) >= 0 &&    // value.Contains( ':' )
                (separator = System.Globalization.CultureInfo.InvariantCulture.CompareInfo.IndexOf(value, ClassMemberSeparator, separator, value.Length - separator, System.Globalization.CompareOptions.Ordinal)) > 0) // value.Contains( "::" )
            {
                className = value.Remove(separator);
                memberName = value.Substring(separator + ClassMemberSeparator.Length);
                return true;
            }
            else
            {
                className = null;
                memberName = value;
                return false;
            }
        }

        /// <summary>
        /// Determines if given <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.
        /// </summary>
        /// <param name="value">Full name.</param>
        /// <returns>True iff the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.</returns>
        public static bool IsClassMemberSyntax(string value)
        {
            return value != null && value.Contains(ClassMemberSeparator);
        }

        #endregion

        #region Basic Overrides

        public override bool Equals(object obj) =>
            obj is Name name ? Equals(name) :
            false;

        public override int GetHashCode() => this.hashCode;

        public override string ToString() => this.value;

        #endregion

        #region IEquatable<Name> Members

        public bool Equals(Name other) => this.GetHashCode() == other.GetHashCode() && Equals(other.Value);

        public static bool operator ==(Name name, Name other)
        {
            return name.Equals(other);
        }

        public static bool operator !=(Name name, Name other)
        {
            return !name.Equals(other);
        }

        #endregion

        #region IEquatable<string> Members

        public bool Equals(string other) => Comparer.Equals(value, other);

        #endregion
    }

    #endregion

    #region VariableName

    /// <summary>
    /// Case-sensitive simple name in Unicode C normal form.
    /// Used for names of variables and constants.
    /// </summary>
    [DebuggerNonUserCode]
    public readonly struct VariableName : IEquatable<VariableName>, IEquatable<string>
    {
        public string Value => this.value;

        readonly string value;

        public static StringComparison Comparison => StringComparison.Ordinal;

        #region Special Names

        public static readonly VariableName ThisVariableName = new VariableName("this");

        #region Autoglobals

        public const string EnvName = "_ENV";
        public const string ServerName = "_SERVER";
        public const string GlobalsName = "GLOBALS";
        public const string RequestName = "_REQUEST";
        public const string GetName = "_GET";
        public const string PostName = "_POST";
        public const string CookieName = "_COOKIE";
        public const string HttpRawPostDataName = "HTTP_RAW_POST_DATA";
        public const string FilesName = "_FILES";
        public const string SessionName = "_SESSION";

        static readonly HashSet<VariableName> s_AutoGlobals = new HashSet<VariableName>()
        {
            new VariableName(EnvName),
            new VariableName(ServerName),
            new VariableName(GlobalsName),
            new VariableName(RequestName),
            new VariableName(GetName),
            new VariableName(PostName),
            new VariableName(CookieName),
            new VariableName(HttpRawPostDataName),
            new VariableName(FilesName),
            new VariableName(SessionName),
        };

        /// <summary>
        /// Enumeration of autoglobal variables.
        /// </summary>
        public static IEnumerable<VariableName> AutoGlobals => s_AutoGlobals;

        #endregion

        /// <summary>
        /// Gets value indicating this object represents the special variable name <c>$this</c>.
        /// </summary>
        public bool IsThisVariableName => ThisVariableName.Equals(this.value);

        #region IsAutoGlobal

        /// <summary>
        /// Gets value indicting whether the name represents an auto-global variable.
        /// </summary>
        public bool IsAutoGlobal => IsAutoGlobalVariableName(this);

        /// <summary>
        /// Checks whether a specified name is the name of an auto-global variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Whether <paramref name="name"/> is auto-global.</returns>
        public static bool IsAutoGlobalVariableName(VariableName name)
        {
            if (!string.IsNullOrEmpty(name.value))
            {
                switch (name.value[0])
                {
                    case '_':
                        return s_AutoGlobals.Contains(name);
                    case 'G':
                        return name.Equals(GlobalsName);
                    case 'H':
                        return name.Equals(HttpRawPostDataName);
                }

            }

            return false;
        }

        /// <summary>
        /// Checks whether a specified name is the name of an auto-global variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Whether <paramref name="name"/> is auto-global.</returns>
        public static bool IsAutoGlobalVariableName(string name) => IsAutoGlobalVariableName(new VariableName(name));

        #endregion

        #endregion

        /// <summary>
		/// Creates a name. 
		/// </summary>
		/// <param name="value">The name, cannot be <B>null</B> nor empty.</param>
		public VariableName(string/*!*/ value)
        {
            Debug.Assert(value != null);
            // TODO (missing from Mono): this.value = value.Normalize();

            this.value = value;
        }

        #region Basic Overrides

        public override bool Equals(object obj) =>
            obj is string str ? Equals(str) :
            obj is VariableName name ? Equals(name) :
            false;

        public override int GetHashCode() => StringComparer.InvariantCulture.GetHashCode(value);

        public override string ToString() => this.value;

        #endregion

        #region IEquatable<VariableName> Members

        public bool Equals(VariableName other) => string.Equals(this.value, other.value, Comparison);

        public static bool operator ==(VariableName name, VariableName other) => name.Equals(other);

        public static bool operator !=(VariableName name, VariableName other) => !name.Equals(other);

        #endregion

        #region IEquatable<string> Members

        public bool Equals(string other) => string.Equals(value, other, Comparison);

        public static bool operator ==(VariableName name, string str) => name.Equals(str);

        public static bool operator !=(VariableName name, string str) => !name.Equals(str);

        #endregion

        #region IEquatable<ReadOnlySpan<char>>

        public bool Equals(ReadOnlySpan<char> other) => value.AsSpan().Equals(other, Comparison);

        public static bool operator ==(VariableName name, ReadOnlySpan<char> str) => name.Equals(str);

        public static bool operator !=(VariableName name, ReadOnlySpan<char> str) => !name.Equals(str);

        #endregion
    }

    #endregion

    #region QualifiedName

    /// <summary>
    /// Case-insensitive culture-sensitive (TODO ???) qualified name in Unicode C normal form.
    /// </summary>
    [DebuggerNonUserCode]
    public struct QualifiedName : IEquatable<QualifiedName>
    {
        #region Special names

        public static readonly QualifiedName Assert = new QualifiedName(new Name("assert"), Name.EmptyNames);
        public static readonly QualifiedName Error = new QualifiedName(new Name("<error>"), Name.EmptyNames);
        public static readonly QualifiedName Lambda = new QualifiedName(new Name("Lambda"), Name.EmptyNames);
        public static readonly QualifiedName Null = new QualifiedName(new Name("null"), Name.EmptyNames);
        public static readonly QualifiedName True = new QualifiedName(new Name("true"), Name.EmptyNames);
        public static readonly QualifiedName False = new QualifiedName(new Name("false"), Name.EmptyNames);
        public static readonly QualifiedName Array = new QualifiedName(new Name("array"), Name.EmptyNames);
        public static readonly QualifiedName Object = new QualifiedName(new Name("object"), Name.EmptyNames);
        public static readonly QualifiedName Mixed = new QualifiedName(new Name("mixed"), Name.EmptyNames);
        public static readonly QualifiedName Never = new QualifiedName(new Name("never"), Name.EmptyNames);
        public static readonly QualifiedName Int = new QualifiedName(new Name("int"), Name.EmptyNames);
        public static readonly QualifiedName Integer = new QualifiedName(new Name("integer"), Name.EmptyNames);
        public static readonly QualifiedName LongInteger = new QualifiedName(new Name("int64"), Name.EmptyNames);
        public static readonly QualifiedName String = new QualifiedName(new Name("string"), Name.EmptyNames);
        public static readonly QualifiedName Boolean = new QualifiedName(new Name("boolean"), Name.EmptyNames);
        public static readonly QualifiedName Bool = new QualifiedName(new Name("bool"), Name.EmptyNames);
        public static readonly QualifiedName Double = new QualifiedName(new Name("double"), Name.EmptyNames);
        public static readonly QualifiedName Float = new QualifiedName(new Name("float"), Name.EmptyNames);
        public static readonly QualifiedName Resource = new QualifiedName(new Name("resource"), Name.EmptyNames);
        public static readonly QualifiedName Callable = new QualifiedName(new Name("callable"), Name.EmptyNames);
        public static readonly QualifiedName Void = new QualifiedName(new Name("void"), Name.EmptyNames);
        public static readonly QualifiedName Iterable = new QualifiedName(new Name("iterable"), Name.EmptyNames);

        /// <summary>
        /// Gets value indicating the <see cref="QualifiedName"/> does not specify the <see cref="Namespaces"/> part.
        /// </summary>
        public bool IsSimpleName => Namespaces == null || Namespaces.Length == 0;

        /// <summary>
        /// Gets value indicating whether this name represents a primitive type.
        /// </summary>
        public bool IsPrimitiveTypeName
        {
            get
            {
                return IsSimpleName && (
                        Equals(Int) ||      // PHP 7.0
                        Equals(Float) ||    // PHP 7.0
                        Equals(String) ||   // PHP 7.0
                        Equals(Bool) ||     // PHP 7.0
                        Equals(Array) ||
                        Equals(Callable) ||
                        Equals(Void) ||     // PHP 7.1
                        Equals(Iterable) || // PHP 7.1
                        Equals(Object) ||   // PHP 7.2
                        Equals(Mixed) ||    // PHP 8.0
                        Equals(Never) ||    // PHP 8.1
                        false
                        );
            }
        }

        public bool IsParentClassName
        {
            get { return IsSimpleName && name == Name.ParentClassName; }
        }

        public bool IsSelfClassName
        {
            get { return IsSimpleName && name == Name.SelfClassName; }
        }

        public bool IsStaticClassName
        {
            get { return IsSimpleName && name == Name.StaticClassName; }
        }

        public bool IsReservedClassName
        {
            get { return this.IsSimpleName && this.name.IsReservedClassName; }
        }

        public bool IsAutoloadName
        {
            get { return IsSimpleName && name == Name.AutoloadName; }
        }

        public bool IsAppStaticAttributeName
        {
            get { return IsSimpleName && (name == Name.AppStaticName || name == Name.AppStaticAttributeName); }
        }

        public bool IsExportAttributeName
        {
            get { return IsSimpleName && (name == Name.ExportName || name == Name.ExportAttributeName); }
        }

        public bool IsDllImportAttributeName
        {
            get { return IsSimpleName && (name == Name.DllImportName || name == Name.DllImportAttributeName); }
        }

        public bool IsOutAttributeName
        {
            get { return IsSimpleName && (name == Name.OutName || name == Name.OutAttributeName); }
        }

        #endregion

        public const char Separator = '\\';

        #region Properties

        /// <summary>
		/// The outer most namespace is the first in the array.
		/// </summary>
		public Name[]/*!*/ Namespaces { get { return namespaces; } set { namespaces = value; } }
        private Name[]/*!*/ namespaces;

        /// <summary>
        /// Base name. Contains the empty string for namespaces.
        /// </summary>
        public Name Name { get { return name; } set { name = value; } }
        private Name name;

        /// <summary>
        /// <c>True</c> if this represents fully qualified name (absolute namespace).
        /// </summary>
        public bool IsFullyQualifiedName { get { return isFullyQualifiedName; } internal set { isFullyQualifiedName = value; } }
        private bool isFullyQualifiedName;

        #endregion

        #region Construction

        ///// <summary>
        ///// Creates a qualified name with or w/o a base name. 
        ///// </summary>
        //internal QualifiedName(string/*!*/ qualifiedName, bool hasBaseName)
        //{
        //    Debug.Assert(qualifiedName != null);
        //    QualifiedName qn = Parse(qualifiedName, 0, qualifiedName.Length, hasBaseName);
        //    this.name = qn.name;
        //    this.namespaces = qn.namespaces;
        //    this.isFullyQualifiedName = qn.IsFullyQualifiedName;
        //}

        internal QualifiedName(IList<string>/*!*/ names, bool hasBaseName, bool fullyQualified)
        {
            Debug.Assert(names != null && names.Count > 0);

            //
            if (hasBaseName)
            {
                name = new Name(names[names.Count - 1]);
                namespaces = new Name[names.Count - 1];
            }
            else
            {
                name = Name.EmptyBaseName;
                namespaces = new Name[names.Count];
            }

            for (int i = 0; i < namespaces.Length; i++)
            {
                namespaces[i] = new Name(names[i]);
            }

            //
            isFullyQualifiedName = fullyQualified;
        }

        public QualifiedName(Name name)
            : this(name, Name.EmptyNames, false)
        {
        }

        public QualifiedName(Name name, Name[]/*!*/ namespaces)
            : this(name, namespaces, false)
        {
        }

        public QualifiedName(Name name, Name[]/*!*/ namespaces, bool fullyQualified)
        {
            if (namespaces == null)
                throw new ArgumentNullException(nameof(namespaces));

            this.name = name;
            this.namespaces = namespaces;
            this.isFullyQualifiedName = fullyQualified;
        }

        internal QualifiedName(Name name, QualifiedName namespaceName)
        {
            Debug.Assert(namespaceName.name.Value == "");

            this.name = name;
            this.namespaces = namespaceName.Namespaces;
            this.isFullyQualifiedName = namespaceName.IsFullyQualifiedName;
        }

        internal QualifiedName(QualifiedName name, QualifiedName namespaceName)
        {
            Debug.Assert(namespaceName.name.Value == "");

            this.name = name.name;

            if (name.IsSimpleName)
            {
                this.namespaces = namespaceName.Namespaces;
            }
            else // used for nested types
            {
                this.namespaces = ArrayUtils.Concat(namespaceName.namespaces, name.namespaces);
            }

            this.isFullyQualifiedName = namespaceName.IsFullyQualifiedName;
        }

        /// <summary>
        /// Make QualifiedName from the string like AAA\BBB\XXX
        /// </summary>
        /// <returns>Qualified name.</returns>
        public static QualifiedName Parse(string name, bool fullyQualified)
        {
            return Parse((name ?? string.Empty).AsSpan(), fullyQualified);
        }

        public static QualifiedName Parse(ReadOnlySpan<char> name, bool fullyQualified)
        {
            name = name.Trim();

            if (name.Length == 0)
            {
                return new QualifiedName(Name.EmptyBaseName);
            }

            // fully qualified
            if (name[0] == Separator)
            {
                name = name.Slice(1);
                fullyQualified = true;
            }

            // parse name
            Name[] namespaces;

            int lastNameStart = name.LastIndexOf(Separator) + 1;
            if (lastNameStart == 0)
            {
                // no namespaces
                namespaces = Name.EmptyNames;
            }
            else
            {
                var namespacesList = ListObjectPool<Name>.Allocate();

                int sep;
                while ((sep = name.IndexOf(Separator)) >= 0)
                {
                    if (sep > 0)
                    {
                        namespacesList.Add(new Name(name.Slice(0, sep).ToString()));
                    }

                    name = name.Slice(sep + 1);
                }

                //
                namespaces = ListObjectPool<Name>.GetArrayAndFree(namespacesList);
            }

            // create QualifiedName
            return new QualifiedName(new Name(name.ToString()), namespaces, fullyQualified);
        }

        /// <summary>
        /// Translates <see cref="QualifiedName"/> according to given naming.
        /// </summary>
        public static bool TryTranslateAlias(QualifiedName qname, AliasKind kind, NamingContext naming, out QualifiedName translated)
        {
            if (naming != null)
            {
                return TryTranslateAlias(qname, kind, naming.Aliases, naming.CurrentNamespace, out translated);
            }
            translated = qname;
            return false;
        }

        /// <summary>
        /// Translates <see cref="QualifiedName"/> according to given naming.
        /// </summary>
        public static bool TryTranslateAlias(QualifiedName qname, NamingContext naming, out QualifiedName translated) =>
            TryTranslateAlias(qname, AliasKind.Type, naming, out translated);

        /// <summary>
        /// Translates <see cref="QualifiedName"/> according to given naming.
        /// </summary>
        public static QualifiedName TranslateAlias(QualifiedName qname, AliasKind kind, Dictionary<Alias, QualifiedName> aliases, QualifiedName? currentNamespace)
        {
            TryTranslateAlias(qname, kind, aliases, currentNamespace, out var translated);
            return translated;
        }

        /// <summary>
        /// Builds <see cref="QualifiedName"/> with first element aliased if posible.
        /// </summary>
        /// <param name="qname">Qualified name to translate.</param>
        /// <param name="kind">Type of the translated alias.</param>
        /// <param name="aliases">Enumeration of aliases.</param>
        /// <param name="currentNamespace">Current namespace to be prepended if no alias is found.</param>
        /// <param name="translated">Qualified name that has been tralated according to given naming context.</param>
        /// <returns>Indication if the name has been translated or not.</returns>
        public static bool TryTranslateAlias(QualifiedName qname, AliasKind kind, Dictionary<Alias, QualifiedName> aliases, QualifiedName? currentNamespace, out QualifiedName translated)
        {
            if (!qname.IsFullyQualifiedName)
            {
                // get first part of the qualified name:
                string first = qname.IsSimpleName ? qname.Name.Value : qname.Namespaces[0].Value;

                // return the alias if found:
                if (aliases != null && aliases.TryGetValue(new Alias(first, kind), out var alias))
                {
                    if (qname.IsSimpleName)
                    {
                        translated = alias;
                        translated.IsFullyQualifiedName = true;
                    }
                    else
                    {
                        // [ alias.namespaces, alias.name, qname.namespaces+1 ]
                        Name[] names = new Name[qname.namespaces.Length + alias.namespaces.Length];
                        for (int i = 0; i < alias.namespaces.Length; ++i) names[i] = alias.namespaces[i];
                        names[alias.namespaces.Length] = alias.name;
                        for (int j = 1; j < qname.namespaces.Length; ++j) names[alias.namespaces.Length + j] = qname.namespaces[j];

                        translated = new QualifiedName(qname.name, names) { IsFullyQualifiedName = true };
                    }
                    return true;
                }
                else
                {
                    if (currentNamespace.HasValue)
                    {
                        Debug.Assert(string.IsNullOrEmpty(currentNamespace.Value.Name.Value));
                        translated = new QualifiedName(qname, currentNamespace.Value) { IsFullyQualifiedName = true };
                        return true;
                    }
                    else
                    {
                        translated = new QualifiedName(qname.Name, qname.Namespaces) { IsFullyQualifiedName = true };
                        return false;
                    }
                }
            }
            translated = qname;
            return false;
        }

        /// <summary>
        /// Convert namespaces + name into list of strings.
        /// </summary>
        /// <returns>String List of namespaces (additionaly with <see cref="Name"/> component if it is not empty).</returns>
        internal List<string>/*!*/ToStringList()
        {
            List<string> list = new List<string>(this.Namespaces.Select(x => x.Value));

            if (!string.IsNullOrEmpty(this.Name.Value))
                list.Add(this.Name.Value);

            return list;
        }

        /// <summary>
        /// Gets instance of <see cref="QualifiedName"/> with <see cref="QualifiedName.isFullyQualifiedName"/> set.
        /// </summary>
        public QualifiedName WithFullyQualified(bool fullyQualified)
        {
            if (fullyQualified == this.isFullyQualifiedName)
            {
                return this;
            }
            else
            {
                return new QualifiedName(this.name, this.namespaces, fullyQualified);
            }
        }

        #endregion

        #region Basic Overrides

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(QualifiedName) && this.Equals((QualifiedName)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = name.GetHashCode();
                for (int i = 0; i < namespaces.Length; i++)
                    result ^= namespaces[i].GetHashCode() << (i & 0x0f);

                return result;
            }
        }

        /// <summary>
        /// Return the namespace PHP name in form "A\B\C", not ending with <see cref="Separator"/>.
        /// </summary>
        public string NamespacePhpName
        {
            get
            {
                var ns = this.namespaces;
                switch (ns.Length)
                {
                    case 0: return string.Empty;

                    case 1: return ns[0].Value;

                    case 2: return ns[0].Value + "\\" + ns[1].Value;

                    default:
                        var result = StringUtils.GetStringBuilder();

                        result.Append(ns[0].Value);

                        for (int i = 1; i < ns.Length; i++)
                        {
                            result.Append(Separator);
                            result.Append(ns[i].Value);
                        }

                        return StringUtils.ReturnStringBuilder(result);
                }
            }
        }

        public string ToString(Name? memberName, bool instance)
        {
            var result = StringUtils.GetStringBuilder();
            for (int i = 0; i < namespaces.Length; i++)
            {
                result.Append(namespaces[i]);
                result.Append(Separator);
            }
            result.Append(Name);
            if (memberName.HasValue)
            {
                result.Append(instance ? "->" : "::");
                result.Append(memberName.Value.ToString());
            }

            return StringUtils.ReturnStringBuilder(result);
        }

        public override string ToString()
        {
            var ns = this.namespaces;
            if (ns == null || ns.Length == 0)
            {
                return this.Name.Value ?? string.Empty;
            }
            else
            {
                var result = StringUtils.GetStringBuilder(ns.Length * 8);
                for (int i = 0; i < ns.Length; i++)
                {
                    result.Append(ns[i]);
                    result.Append(Separator);
                }
                result.Append(this.Name.Value);
                return StringUtils.ReturnStringBuilder(result);
            }
        }

        #endregion

        #region IEquatable<QualifiedName> Members

        public bool Equals(QualifiedName other)
        {
            if (!this.name.Equals(other.name) || this.namespaces.Length != other.namespaces.Length) return false;

            for (int i = 0; i < namespaces.Length; i++)
            {
                if (!this.namespaces[i].Equals(other.namespaces[i]))
                    return false;
            }

            return true;
        }

        public static bool operator ==(QualifiedName name, QualifiedName other)
        {
            return name.Equals(other);
        }

        public static bool operator !=(QualifiedName name, QualifiedName other)
        {
            return !name.Equals(other);
        }

        #endregion
    }

    internal class ConstantQualifiedNameComparer : IEqualityComparer<QualifiedName>
    {
        public static readonly ConstantQualifiedNameComparer Singleton = new ConstantQualifiedNameComparer();

        public bool Equals(QualifiedName x, QualifiedName y)
        {
            return x.Equals(y) && string.Equals(x.Name.Value, y.Name.Value, StringComparison.Ordinal);   // case sensitive comparison of names
        }

        public int GetHashCode(QualifiedName obj)
        {
            return obj.GetHashCode();
        }
    }

    #endregion

    #region GenericQualifiedName

    /// <summary>
    /// Case-insensitive culture-sensitive (TODO ???) qualified name in Unicode C normal form
    /// with associated list of generic qualified names.
    /// </summary>
    public struct GenericQualifiedName
    {
        /// <summary>
        /// Empty GenericQualifiedName array.
        /// </summary>
        public static GenericQualifiedName[] EmptyGenericQualifiedNames => EmptyArray<GenericQualifiedName>.Instance;

        /// <summary>
        /// Qualified name without generics.
        /// </summary>
		public QualifiedName QualifiedName { get; }

        /// <summary>
        /// Array of generic type arguments.
        /// </summary>
        public GenericQualifiedName[]/*!!*/ GenericParams { get; }

        /// <summary>
        /// Gets value indicating whether the name has generic type parameters.
        /// </summary>
        public bool IsGeneric => GenericParams != null && GenericParams.Length != 0;

        public GenericQualifiedName(QualifiedName qualifiedName, GenericQualifiedName[] genericParams)
        {
            this.QualifiedName = qualifiedName;
            this.GenericParams = genericParams ?? EmptyGenericQualifiedNames;
        }

        public GenericQualifiedName(QualifiedName qualifiedName)
        {
            this.QualifiedName = qualifiedName;
            this.GenericParams = EmptyGenericQualifiedNames;
        }
    }

    #endregion

    #region NamingContext

    /// <summary>
    /// Type of alias.
    /// </summary>
    public enum AliasKind
    {
        /// <summary>
        /// Data type or namespace alias.
        /// </summary>
        Type,
        /// <summary>
        /// Function alias.
        /// </summary>
        Function,
        /// <summary>
        /// Constant alias.
        /// </summary>
        Constant
    }

    /// <summary>
    /// Alias structure, contains both the name and type.
    /// Represents the key to <see cref="NamingContext"/>.
    /// </summary>
    public struct Alias
    {
        /// <summary>
        /// Alias name.
        /// </summary>
        public readonly Name Name;

        /// <summary>
        /// Type of alias, a type/namespace, function or constant.
        /// </summary>
        public readonly AliasKind Kind;

        /// <summary>
        /// Creates new alias.
        /// </summary>
        /// <param name="name">Alias name.</param>
        /// <param name="kind">Alias type.</param>
        public Alias(Name name, AliasKind kind)
        {
            Name = name;
            Kind = kind;
        }

        /// <summary>
        /// Creates new alias.
        /// </summary>
        /// <param name="name">Alias name.</param>
        /// <param name="kind">Alias type.</param>
        public Alias(string name, AliasKind kind)
            : this(new Name(name), kind)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [DebuggerNonUserCode]
    public sealed class NamingContext
    {
        sealed class AliasComparer : IEqualityComparer<Alias>
        {
            public static readonly AliasComparer Singleton = new AliasComparer();

            public bool Equals(Alias x, Alias y) => x.Kind == y.Kind &&
                (x.Kind == AliasKind.Constant ?
                    x.Name.Value.Equals(y.Name.Value, StringComparison.Ordinal) :
                    x.Name.Equals(y.Name));

            public int GetHashCode(Alias obj) =>
                obj.Kind == AliasKind.Constant ?
                    StringComparer.Ordinal.GetHashCode(obj.Name.Value) :
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name.Value);
        }

        #region Fields & Properties

        /// <summary>
        /// Current namespace.
        /// </summary>
        public readonly QualifiedName? CurrentNamespace;

        /// <summary>
        /// PHP aliases. Can be null.
        /// </summary>
        public Dictionary<Alias, QualifiedName> Aliases => _aliases;
        private Dictionary<Alias, QualifiedName> _aliases;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes new instance of <see cref="NamingContext"/>
        /// </summary>
        public NamingContext(QualifiedName? currentNamespace)
        {
            Debug.Assert(!currentNamespace.HasValue || string.IsNullOrEmpty(currentNamespace.Value.Name.Value), "Valid namespace QualifiedName has no base name.");

            this.CurrentNamespace = currentNamespace;
        }

        Dictionary<Alias, QualifiedName> EnsureAliases()
        {
            var dict = _aliases;
            if (dict == null)
            {
                _aliases = dict = new Dictionary<Alias, QualifiedName>(AliasComparer.Singleton);
            }

            return dict;
        }

        /// <summary>
        /// Gets qualified name matching given alias.
        /// </summary>
        public bool TryGetAlias(Name name, AliasKind kind, out QualifiedName qname)
        {
            var dict = _aliases;
            if (dict != null)
            {
                return dict.TryGetValue(new Alias(name, kind), out qname);
            }
            else
            {
                qname = default(QualifiedName);
                return false;
            }
        }

        private bool AddAlias(Name name, AliasKind kind, QualifiedName qname)
        {
            Debug.Assert(!string.IsNullOrEmpty(name.Value));

            var dict = EnsureAliases();
            var count = dict.Count;
            var alias = new Alias(name, kind);

            //
            dict[alias] = qname;

            //
            return count != dict.Count;  // item was added
        }

        /// <summary>
        /// Add an alias into the <see cref="Aliases"/>.
        /// </summary>
        /// <param name="alias">Alias name.</param>
        /// <param name="qualifiedName">Aliased namespace. Not starting with <see cref="QualifiedName.Separator"/>.</param>
        /// <remarks>Used when constructing naming context at runtime.</remarks>
        public bool AddAlias(Name alias, QualifiedName qualifiedName)
        {
            Debug.Assert(string.IsNullOrEmpty(qualifiedName.NamespacePhpName) || qualifiedName.NamespacePhpName[0] != QualifiedName.Separator);   // not starting with separator

            return AddAlias(alias, AliasKind.Type, qualifiedName);
        }

        /// <summary>
        /// Adds a function alias into the context.
        /// </summary>
        public bool AddFunctionAlias(Name alias, QualifiedName qname)
        {
            return AddAlias(alias, AliasKind.Function, qname);
        }

        /// <summary>
        /// Adds a constant into the context.
        /// </summary>
        public bool AddConstantAlias(Name alias, QualifiedName qname)
        {
            return AddAlias(alias, AliasKind.Constant, qname);
        }

        #endregion
    }

    #endregion

    #region TranslatedQualifiedName

    /// <summary>
    /// Ecapsulates name of a global constant use or a global function call according to PHP semantics.
    /// </summary>
    /// <remarks>The qualified name can be translated according to current naming context or it can have a fallback.</remarks>
    public struct TranslatedQualifiedName
    {
        readonly QualifiedNameRef _name;
        readonly QualifiedName _originalName;
        readonly QualifiedName? _fallbackName;

        /// <summary>
        /// Translated qualified name.
        /// </summary>
        public QualifiedNameRef Name => _name;

        /// <summary>
        /// Original qualified name, can be equal to <see cref="Name"/>.
        /// Always a valid name.
        /// </summary>
        public QualifiedName OriginalName => _originalName;

        /// <summary>
        /// Optional. A second name to be used in case <see cref="Name"/> is not defined.
        /// </summary>
        public QualifiedName? FallbackName => _fallbackName;

        /// <summary>
        /// Span of the element within the source code.
        /// </summary>
        public Span Span => _name.Span;

        public TranslatedQualifiedName(QualifiedName name, Span nameSpan, QualifiedName originalName, QualifiedName? nameFallback)
        {
            _name = new QualifiedNameRef(nameSpan, name);
            _originalName = originalName;
            _fallbackName = nameFallback;
        }

        public TranslatedQualifiedName(QualifiedName name, Span nameSpan) : this(name, nameSpan, name, null) { }
    }

    #endregion
}
