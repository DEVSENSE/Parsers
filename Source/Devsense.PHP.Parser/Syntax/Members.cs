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

namespace Devsense.PHP.Syntax
{
    #region PhpMemberAttributes

    [Flags]
    public enum PhpMemberAttributes : short
    {
        None = 0,

        Public = 0,
        Private = 1,
        Protected = 2,

        /// <summary>
        /// The member is read-only.
        /// </summary>
        ReadOnly = 4,

        NamespacePrivate = Private,

        Static = 8,
        AppStatic = Static | 16,
        Abstract = 32,
        Final = 64,

        /// <summary>
        /// The type is an interface.
        /// </summary>
        Interface = 128,

        /// <summary>
        /// The type is a trait.
        /// </summary>
        Trait = 256,

        /// <summary>
        /// The member is a constructor.
        /// </summary>
        Constructor = 512,

        /// <summary>
        /// The member is "enum".
        /// </summary>
        Enum = 1024,

        /// <summary>
        /// The member needs to be activated before it can be resolved.
        /// TODO: useful when analysis checks whether there are any imported conditional types/functions.
        /// TODO: add the first conditional to the AC, ignore the others. Add the flag handling to Resolve* and to analyzer.
        /// </summary>
        InactiveConditional = 1024 << 1,

        PublicSet = 1024 << 2,
        PrivateSet = 1024 << 3,
        ProtectedSet = 1024 << 4,

        TypeMask = Interface | Trait | Enum /*| Class = 0*/,
        StaticMask = Static | AppStatic,
        VisibilityMask = Public | Private | Protected | NamespacePrivate,
        SetVisibilityMask = PublicSet | PrivateSet | ProtectedSet,
        SpecialMembersMask = Constructor
    }

    #endregion

    /// <summary>
    /// <see cref="PhpMemberAttributes"/> extension methods.
    /// </summary>
    public static class PhpMemberAttributesExtensions
    {
        /// <summary>Gets value indicating a trait declaration.</summary>
        public static bool IsTrait(this PhpMemberAttributes attrs) => (attrs & PhpMemberAttributes.Trait) != 0;

        /// <summary>Gets value indicating an interface declaration.</summary>
        public static bool IsInterface(this PhpMemberAttributes attrs) => (attrs & PhpMemberAttributes.Interface) != 0;

        /// <summary>Gets value indicating an interface declaration.</summary>
        public static bool IsEnum(this PhpMemberAttributes attrs) => (attrs & PhpMemberAttributes.Enum) != 0;

        /// <summary>Gets value indicating the member is read-only.</summary>
        public static bool IsReadOnly(this PhpMemberAttributes attrs) => (attrs & PhpMemberAttributes.ReadOnly) != 0;
    }
}
