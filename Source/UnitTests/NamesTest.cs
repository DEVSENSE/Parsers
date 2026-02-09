using System;
using System.IO;
using Devsense.PHP.Text;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using UnitTests.TestImplementation;
using Devsense.PHP.Syntax;
using Devsense.PHP.Ast.DocBlock;
using Xunit;

namespace UnitTests
{
    public class NamesTest
    {
        [Fact]
        public void TestQualifiedNameToString()
        {
            var names = new string[]
            {
                "",
                "a",
                "a\\b",
                "a\\b\\c",
            };

            foreach (var fqn in names)
            {
                Assert.NotNull(fqn);
                var parsed = QualifiedName.Parse(fqn, false);
                Assert.Equal(fqn, parsed.ToString());
            }
        }
    }
}
