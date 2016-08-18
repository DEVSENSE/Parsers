using Microsoft.VisualStudio.TestTools.UnitTesting;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Syntax.Ast.Serialization;

namespace UnitTests
{
    [TestClass]
    public class SerializerTreeVisitorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void SerializerTreeVisitorVisitStringLiteralTest()
        {
            var serializer = new JsonNodeWriter();
            TreeSerializer visitor = new TreeSerializer(serializer);
            visitor.VisitStringLiteral(new StringLiteral(new Span(0, 10), "hello world"));
            Assert.AreEqual("\"StringLiteral\" : {\n  \"Span\" : {\n    \"start\" : \"0\",\n    \"end\" : \"10\"\n  },\n  \"Value\" : \"hello world\"\n}", serializer.ToString());
        }
    }
}
