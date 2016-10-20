﻿// Copyright(c) DEVSENSE s.r.o.
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
using System.Diagnostics;
using System.Text;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

namespace Devsense.PHP.Syntax.Ast.Serialization
{
    public class JsonNodeWriter : INodeWriter
    {
        StringBuilder _builder = new StringBuilder();
        string _indent = string.Empty;
        bool _first = true;

        void Indent() { _indent += "  "; _first = true; }
        void DeIndent() { if (_indent.Length >= 2) _indent = _indent.Remove(_indent.Length - 2); _first = false; }

        void SerializeValue(string nodeType, string nodeValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(nodeType));
            if (!_first)
                _builder.Append(",");
            else _first = false;
            _builder.AppendFormat("\n{0}\"{1}\" : \"{2}\"", _indent, nodeType, nodeValue);
        }

        void SerializeProperties(NodeObj[] properties)
        {
            if (properties != null)
                foreach (var property in properties)
                    if (!string.IsNullOrEmpty(property.NodeType))
                    {
                        Debug.Assert(property.NodeValue == null || property.NodeProperties == null);
                        if (property.NodeValue != null)
                            SerializeValue(property.NodeType, property.NodeValue);
                        else
                        {
                            Debug.Assert(property.NodeProperties != null);
                            Serialize(property.NodeType, property.NodeProperties);
                        }
                    }
        }

        //LangElement Reconstruct(KeyValuePair<string, JToken> node)
        //{
        //    Debug.Assert(node.Value.Type.ToString() == "Object");
        //    LangElement element = (LangElement)Activator.CreateInstance(Type.GetType(node.Key));
        //    foreach (var item in (JObject)node.Value)
        //    {

        //        if(item.Value.Type.ToString() != "Object")
        //        {
        //            FieldInfo property = element.GetType().GetRuntimeField(item.Key);
        //            if (property != null)
        //            {
        //                property.SetValue(element, token.Value.ToString());
        //            }
        //        }
        //        else
        //        {

        //        }
        //    }
        //    return element;
        //}

        override public string ToString()
        {
            return _builder.ToString().TrimStart();// string.Format("{{\n{0}\n}}", _builder.ToString());
        }

        public void EndSerialize(params NodeObj[] properties)
        {
            SerializeProperties(properties);
            DeIndent();
            _builder.AppendFormat("\n{0}}}", _indent);
        }

        public void Serialize(string nodeType, params NodeObj[] properties)
        {
            Debug.Assert(!string.IsNullOrEmpty(nodeType));
            StartSerialize(nodeType, properties);
            EndSerialize();
        }

        public void StartSerialize(string nodeType, params NodeObj[] properties)
        {
            Debug.Assert(!string.IsNullOrEmpty(nodeType));
            if (!_first)
                _builder.Append(",");
            else _first = false;
            _builder.AppendFormat("\n{0}\"{1}\" : {{", _indent, nodeType);
            Indent();
            SerializeProperties(properties);
        }

        //public LangElement Deserialize(string data, INodesFactory<LangElement, Span> factory)
        //{
        //    Debug.Assert(factory != null);
        //    _astFactory = factory;
        //    JObject root = JObject.Parse(data);
        //    Debug.Assert(root.Count == 1);
        //    return Reconstruct(root.GetEnumerator().Current);

        //    //    if(token.Key == "StringLiteral")
        //    //    {

        //    //    }
        //    //    else
        //    //    //change the display Content of the parent
        //    //    parent.Text = token.Key.ToString();
        //    //    //create the child node
        //    //    TreeNode child = new TreeNode();
        //    //    child.Text = token.Key.ToString();
        //    //    //check if the value is of type obj recall the method
        //    //    if (token.Value.Type.ToString() == "Object")
        //    //    {
        //    //        // child.Text = token.Key.ToString();
        //    //        //create a new JObject using the the Token.value
        //    //        JObject o = (JObject)token.Value;
        //    //        //recall the method
        //    //        child = Json2Tree(o);
        //    //        //add the child to the parentNode
        //    //        parent.Nodes.Add(child);
        //    //    }
        //    //    //if type is of array
        //    //    else if (token.Value.Type.ToString() == "Array")
        //    //    {
        //    //        int ix = -1;
        //    //        //  child.Text = token.Key.ToString();
        //    //        //loop though the array
        //    //        foreach (var itm in token.Value)
        //    //        {
        //    //            //check if value is an Array of objects
        //    //            if (itm.Type.ToString() == "Object")
        //    //            {
        //    //                TreeNode objTN = new TreeNode();
        //    //                //child.Text = token.Key.ToString();
        //    //                //call back the method
        //    //                ix++;

        //    //                JObject o = (JObject)itm;
        //    //                objTN = Json2Tree(o);
        //    //                objTN.Text = token.Key.ToString() + "[" + ix + "]";
        //    //                child.Nodes.Add(objTN);
        //    //                //parent.Nodes.Add(child);
        //    //            }
        //    //            //regular array string, int, etc
        //    //            else if (itm.Type.ToString() == "Array")
        //    //            {
        //    //                ix++;
        //    //                TreeNode dataArray = new TreeNode();
        //    //                foreach (var data in itm)
        //    //                {
        //    //                    dataArray.Text = token.Key.ToString() + "[" + ix + "]";
        //    //                    dataArray.Nodes.Add(data.ToString());
        //    //                }
        //    //                child.Nodes.Add(dataArray);
        //    //            }

        //    //            else
        //    //            {
        //    //                child.Nodes.Add(itm.ToString());
        //    //            }
        //    //        }
        //    //        parent.Nodes.Add(child);
        //    //    }
        //    //    else
        //    //    {
        //    //        //if token.Value is not nested
        //    //        // child.Text = token.Key.ToString();
        //    //        //change the value into N/A if value == null or an empty string 
        //    //        if (token.Value.ToString() == string.Empty)
        //    //            child.Nodes.Add("N/A");
        //    //        else
        //    //            child.Nodes.Add(token.Value.ToString());
        //    //        parent.Nodes.Add(child);
        //    //    }
        //    //}
        //    return null;
        //}
    }
}
