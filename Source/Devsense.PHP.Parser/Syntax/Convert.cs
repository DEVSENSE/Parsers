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
    public static class Convert
    {
        /// <summary>
        /// Converts a character to a digit.
        /// </summary>
        /// <param name="c">The character [0-9A-Za-z].</param>
        /// <returns>The digit represented by the character or <see cref="Int32.MaxValue"/> 
        /// on non-alpha-numeric characters.</returns>
        public static int AlphaNumericToDigit(char c)
        {
            if (c >= '0' && c <= '9')
                return (int)(c - '0');

            if (c >= 'a' && c <= 'z')
                return (int)(c - 'a') + 10;

            if (c >= 'A' && c <= 'Z')
                return (int)(c - 'A') + 10;

            return Int32.MaxValue;
        }

        /// <summary>
        /// Converts a character to a digit.
        /// </summary>
        /// <param name="c">The character [0-9].</param>
        /// <returns>The digit represented by the character or <see cref="Int32.MaxValue"/> 
        /// on non-numeric characters.</returns>
        public static int NumericToDigit(char c)
        {
            if (c >= '0' && c <= '9')
                return (int)(c - '0');

            return Int32.MaxValue;
        }
    }
}
