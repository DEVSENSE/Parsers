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

using PHP.Core.AST;
using PHP.Core.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhpParser
{
    public struct NodeObj
    {
        public NodeObj(string nodeType) { NodeType = nodeType; NodeValue = null; NodeProperties = null; }
        public NodeObj(string nodeType, string nodeValue) { NodeType = nodeType; NodeValue = nodeValue; NodeProperties = null; }
        public NodeObj(string nodeType, params NodeObj[] nodeProperties) { NodeType = nodeType; NodeValue = null; NodeProperties = nodeProperties; }

        public string NodeType;
        public string NodeValue;
        public NodeObj[] NodeProperties;
    }

    public interface ISerializer
    {
        void Serialize(string nodeType, params NodeObj[] properties);
        void StartSerialize(string nodeType, params NodeObj[] properties);
        void EndSerialize(params NodeObj[] properties);
    }
}
