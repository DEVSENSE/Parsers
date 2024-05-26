using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Devsense.PHP.Text;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using UnitTests.TestImplementation;
using Devsense.PHP.Syntax;
using Devsense.PHP.Ast.DocBlock;

namespace UnitTests
{
    [TestClass]
    public class NamesTest
    {
        [TestMethod]
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
                Assert.IsNotNull(fqn);
                var parsed = QualifiedName.Parse(fqn, false);
                Assert.AreEqual(fqn, parsed.ToString());
            }
        }
    }
}
