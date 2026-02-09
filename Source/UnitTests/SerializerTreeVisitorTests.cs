using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Syntax.Ast.Serialization;
using Xunit;

namespace UnitTests
{
    public class SerializerTreeVisitorTests
    {
        [Fact]
        public void SerializerTreeVisitorVisitStringLiteralTest()
        {
            var serializer = new JsonNodeWriter();
            TreeSerializer visitor = new TreeSerializer(serializer);
            visitor.VisitStringLiteral(StringLiteral.Create(new Span(0, 10), "hello world"));
            Assert.Equal("\"StringLiteral\" : {\n  \"Span\" : {\n    \"start\" : \"0\",\n    \"end\" : \"10\"\n  },\n  \"Value\" : \"hello world\"\n}", serializer.ToString());
        }
    }
}
