using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHP.Core.AST;
using PhpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.TestImplementation;

namespace UnitTests
{
    [TestClass]
    public class JsonSerializerTests
    {
        [TestMethod]
        public void ToStringTest()
        {
            ISerializer serializer = new JsonSerializer();
            Assert.AreEqual(string.Empty, serializer.ToString());
            serializer.Serialize("hello", null);
            Assert.AreEqual("\"hello\" : {\n}", serializer.ToString());
        }

        [TestMethod]
        public void EndSerializeTest()
        {
            ISerializer serializer = new JsonSerializer();
            serializer.EndSerialize(null);
            Assert.AreEqual("}", serializer.ToString());

            serializer = new JsonSerializer();
            Assert.AreEqual(string.Empty, serializer.ToString());
            serializer.EndSerialize(new NodeObj("hello", "world"));
            Assert.AreEqual("\"hello\" : \"world\"\n}", serializer.ToString());
        }

        [TestMethod]
        public void SerializeTest()
        {
            ISerializer serializer = new JsonSerializer();
            serializer.Serialize("hello", null);
            Assert.AreEqual("\"hello\" : {\n}", serializer.ToString());

            serializer = new JsonSerializer();
            serializer.Serialize("hello", new NodeObj("hello", "world"));
            Assert.AreEqual("\"hello\" : {\n  \"hello\" : \"world\"\n}", serializer.ToString());
            serializer.Serialize("world", new NodeObj("hello", new NodeObj("hello", "world")));
            Assert.AreEqual("\"hello\" : {\n  \"hello\" : \"world\"\n},\n\"world\" : {\n  \"hello\" : {\n    \"hello\" : \"world\"\n  }\n}", serializer.ToString());
        }

        [TestMethod]
        public void StartSerializeTest()
        {
            ISerializer serializer = new JsonSerializer();
            serializer.StartSerialize("hello", null);
            Assert.AreEqual("\"hello\" : {", serializer.ToString());

            serializer = new JsonSerializer();
            serializer.StartSerialize("hello", new NodeObj("hello", "world"));
            Assert.AreEqual("\"hello\" : {\n  \"hello\" : \"world\"", serializer.ToString());
            serializer.StartSerialize("world", new NodeObj("hello", "world"));
            Assert.AreEqual("\"hello\" : {\n  \"hello\" : \"world\",\n  \"world\" : {\n    \"hello\" : \"world\"", serializer.ToString());
        }

        //[TestMethod]
        //public void DeserializeTest()
        //{
        //    ISerializer serializer = new JsonSerializer();
        //    LangElement ast = serializer.Deserialize("{\n\"GlobalCode\" : {\n  \"EchoStmt\" : {\n  }\n}\n}", new TestNodesFactory(null));
        //    Assert.IsTrue(ast is GlobalCode);
        //    GlobalCode code = (GlobalCode)ast;
        //    Assert.IsNotNull(code);
        //    Assert.AreEqual(1, code.Statements.Length);
        //    Assert.IsFalse(string.IsNullOrEmpty(((StringLiteral)((EchoStmt)code.Statements[0]).Parameters[0]).Value));
        //    Assert.IsTrue(((StringLiteral)((EchoStmt)code.Statements[0]).Parameters[0]).Value.Contains("https://github.com/php/php-src/tree/master/Zend/tests"));
        //}
    }
}
