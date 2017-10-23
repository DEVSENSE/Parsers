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
using System.Linq;

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
        NamespacePrivate = Private,

        Static = 4,
        AppStatic = Static | 8,
        Abstract = 16,
        Final = 32,

        /// <summary>
        /// The type is an interface.
        /// </summary>
        Interface = 64,

        /// <summary>
        /// The type is a trait.
        /// </summary>
        Trait = 128,

        /// <summary>
        /// The member is a constructor.
        /// </summary>
        Constructor = 256,

        /// <summary>
        /// The member is imported type, function or global constant with ambiguous fully qualified name.
        /// </summary>
        Ambiguous = 1024,

        /// <summary>
        /// The member needs to be activated before it can be resolved.
        /// TODO: useful when analysis checks whether there are any imported conditional types/functions.
        /// TODO: add the first conditional to the AC, ignore the others. Add the flag handling to Resolve* and to analyzer.
        /// </summary>
        InactiveConditional = 2048,

        StaticMask = Static | AppStatic,
        VisibilityMask = Public | Private | Protected | NamespacePrivate,
        SpecialMembersMask = Constructor,
        PartialMerged = Abstract | Final
    }

    #endregion

    #region ModifierPosition

    /// <summary>
    /// Structure used only in the parser to collect complete 
    /// information about the type membermodifiers.
    /// </summary>
    internal struct ModifierPosition
    {
        private PhpMemberAttributes _combinedModifiers;
        private List<KeyValuePair<Tokens, int>> _modifiers;

        public ModifierPosition(Tokens modifier, int position)
        {
            _modifiers = new List<KeyValuePair<Tokens, int>>()
            { new KeyValuePair<Tokens, int>(modifier, position) };
            _combinedModifiers = modifier.ToModifier();
        }

        public void AddModifier(ModifierPosition modifier)
        {
            if (_modifiers == null)
            {
                _modifiers = new List<KeyValuePair<Tokens, int>>();
            }
            _modifiers.AddRange(modifier._modifiers);
            for (int i = 0; i < modifier._modifiers.Count; i++)
            {
                _combinedModifiers |= modifier._modifiers[i].Key.ToModifier();
            }
        }

        public void AddModifier(Tokens modifier, int position)
        {
            if (_modifiers == null)
            {
                _modifiers = new List<KeyValuePair<Tokens, int>>();
            }
            _modifiers.Add(new KeyValuePair<Tokens, int>(modifier, position));
            _combinedModifiers |= modifier.ToModifier();
        }

        public bool IsNone => _modifiers == null || _modifiers.Count == 0;

        public PhpMemberAttributes CombinedModifier => _combinedModifiers;

        public KeyValuePair<Tokens, short>[] Modifiers(int start) => _modifiers == null ? null :
            _modifiers.Select(p => new KeyValuePair<Tokens, short>(p.Key, (short)(p.Value - start))).ToArray();
    }

    #endregion
}
