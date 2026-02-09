using Devsense.PHP.Syntax.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Syntax.Ast.Serialization;
using Xunit;

namespace UnitTests
{
    public class JsonSerializerTests
    {
        [Fact]
        public void ToStringTest()
        {
            INodeWriter serializer = new JsonNodeWriter();
            Assert.Equal(string.Empty, serializer.ToString());
            serializer.Serialize("hello", null);
            Assert.Equal("\"hello\" : {\n}", serializer.ToString());
        }

        [Fact]
        public void EndSerializeTest()
        {
            INodeWriter serializer = new JsonNodeWriter();
            serializer.EndSerialize(null);
            Assert.Equal("}", serializer.ToString());

            serializer = new JsonNodeWriter();
            Assert.Equal(string.Empty, serializer.ToString());
            serializer.EndSerialize(new NodeObj("hello", "world"));
            Assert.Equal("\"hello\" : \"world\"\n}", serializer.ToString());
        }

        [Fact]
        public void SerializeTest()
        {
            INodeWriter serializer = new JsonNodeWriter();
            serializer.Serialize("hello", null);
            Assert.Equal("\"hello\" : {\n}", serializer.ToString());

            serializer = new JsonNodeWriter();
            serializer.Serialize("hello", new NodeObj("hello", "world"));
            Assert.Equal("\"hello\" : {\n  \"hello\" : \"world\"\n}", serializer.ToString());
            serializer.Serialize("world", new NodeObj("hello", new NodeObj("hello", "world")));
            Assert.Equal("\"hello\" : {\n  \"hello\" : \"world\"\n},\n\"world\" : {\n  \"hello\" : {\n    \"hello\" : \"world\"\n  }\n}", serializer.ToString());
        }

        [Fact]
        public void StartSerializeTest()
        {
            INodeWriter serializer = new JsonNodeWriter();
            serializer.StartSerialize("hello", null);
            Assert.Equal("\"hello\" : {", serializer.ToString());

            serializer = new JsonNodeWriter();
            serializer.StartSerialize("hello", new NodeObj("hello", "world"));
            Assert.Equal("\"hello\" : {\n  \"hello\" : \"world\"", serializer.ToString());
            serializer.StartSerialize("world", new NodeObj("hello", "world"));
            Assert.Equal("\"hello\" : {\n  \"hello\" : \"world\",\n  \"world\" : {\n    \"hello\" : \"world\"", serializer.ToString());
        }

        //[Fact]
        //public void DeserializeTest()
        //{
        //    ISerializer serializer = new JsonSerializer();
        //    LangElement ast = serializer.Deserialize("{\n\"GlobalCode\" : {\n  \"EchoStmt\" : {\n  }\n}\n}", new TestNodesFactory(null));
        //    Assert.True(ast is GlobalCode);
        //    GlobalCode code = (GlobalCode)ast;
        //    Assert.NotNull(code);
        //    Assert.Equal(1, code.Statements.Length);
        //    Assert.IsFalse(string.IsNullOrEmpty(((StringLiteral)((EchoStmt)code.Statements[0]).Parameters[0]).Value));
        //    Assert.True(((StringLiteral)((EchoStmt)code.Statements[0]).Parameters[0]).Value.Contains("https://github.com/php/php-src/tree/master/Zend/tests"));
        //}
    }
}
