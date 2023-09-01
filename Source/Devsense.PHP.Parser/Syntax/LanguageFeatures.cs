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
    #region Language Features Enum

    /// <summary>
    /// PHP language features supported by Phalanger.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [Flags]
    public enum LanguageFeatures
    {
        /// <summary>
        /// Basic features - always present.
        /// </summary>
        Basic = Php82Set,

        /// <summary>
        /// Allows using short open tags in the script.
        /// </summary>
        ShortOpenTags = 1,

        /// <summary>
        /// Allows using short open tags in the script.
        /// </summary>
        Php54Set = 1 << 4,

        /// <summary>
        /// PHP 5.5 feature set.
        /// </summary>
        Php55Set = Php54Set | (1 << 5),

        /// <summary>
        /// PHP 5.6 feature set.
        /// </summary>
        Php56Set = Php55Set | (1 << 6),

        /// <summary>
        /// PHP 7.0 feature set.
        /// </summary>
        Php70Set = Php56Set | (1 << 7),

        /// <summary>
        /// PHP 7.1 feature set.
        /// </summary>
        Php71Set = Php70Set | (1 << 8),

        /// <summary>
        /// PHP 7.2 feature set.
        /// </summary>
        Php72Set = Php71Set | (1 << 9),

        /// <summary>
        /// PHP 7.3 feature set.
        /// </summary>
        Php73Set = Php72Set | (1 << 10),

        /// <summary>
        /// PHP 7.4 feature set.
        /// </summary>
        Php74Set = Php73Set | (1 << 11),

        /// <summary>
        /// PHP 8.0 feature set.
        /// </summary>
        Php80Set = Php74Set | (1 << 12),

        /// <summary>
        /// PHP 8.1 feature set.
        /// </summary>
        Php81Set = Php80Set | (1 << 13),

        /// <summary>
        /// PHP 8.2 feature set.
        /// </summary>
        Php82Set = Php81Set | (1 << 14),

        /// <summary>
        /// PHP 8.3 feature set.
        /// </summary>
        Php83Set = Php82Set | (1 << 15),
    }

    public static class LanguageFeaturesExtensions
    {
        public static bool HasFeature(this LanguageFeatures value, LanguageFeatures feature) => (value & feature) == feature;
    }

    #endregion
}
