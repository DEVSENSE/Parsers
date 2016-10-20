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

namespace Devsense.PHP.Syntax.Ast.Serialization
{
    public struct NodeObj
    {
        public static readonly NodeObj Empty = new NodeObj() { NodeType = null, NodeValue = null };

        public NodeObj(string nodeType, string nodeValue) { NodeType = nodeType; NodeValue = nodeValue; NodeProperties = null; }
        public NodeObj(string nodeType, params NodeObj[] nodeProperties) { NodeType = nodeType; NodeValue = null; NodeProperties = nodeProperties; }

        public string NodeType;
        public string NodeValue;
        public NodeObj[] NodeProperties;
    }

    public interface INodeWriter
    {
        void Serialize(string nodeType, params NodeObj[] properties);
        void StartSerialize(string nodeType, params NodeObj[] properties);
        void EndSerialize(params NodeObj[] properties);
    }
}
