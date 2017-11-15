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
        Basic = Php72Set,

        /// <summary>
        /// Allows using short open tags in the script.
        /// </summary>
        ShortOpenTags = 1,

        /// <summary>
        /// Allows using short open tags in the script.
        /// </summary>
        Php54Set = 16,

        /// <summary>
        /// PHP 5.5 feature set.
        /// </summary>
        Php55Set = Php54Set | 32,

        /// <summary>
        /// PHP 5.6 feature set.
        /// </summary>
        Php56Set = Php55Set | 64,

        /// <summary>
        /// PHP 7.0 feature set.
        /// </summary>
        Php70Set = Php56Set | 128,

        /// <summary>
        /// PHP 7.1 feature set.
        /// </summary>
        Php71Set = Php70Set | 256,

        /// <summary>
        /// PHP 7.2 feature set.
        /// </summary>
        Php72Set = Php71Set | 512,
    }

    public static class LanguageFeaturesExtensions
    {
        public static bool HasFeature(this LanguageFeatures value, LanguageFeatures feature) => (value & feature) == feature;
    }

    #endregion
}
