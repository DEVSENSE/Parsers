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
