using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Devsense.PHP.Syntax.Ast;
using System.Text.RegularExpressions;
using Devsense.PHP.Syntax.Ast.Serialization;
using Devsense.PHP.Syntax;
using System.Text;
using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("ParserTestData.csv")]
    public class ParserTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\ParserTestData.csv", "ParserTestData#csv", DataAccessMethod.Sequential)]
        public void ParserParseTest()
        {
            string path = (string)TestContext.DataRow["files"];
            string testcontent = File.ReadAllText(path);

            string[] testparts = testcontent.Split(new string[] { "<<<TEST>>>" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(testparts.Length >= 2);

            var sourceUnit = new CodeSourceUnit(testparts[0], path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Basic);
            var factory = new BasicNodesFactory(sourceUnit);

            GlobalCode ast = null;
            
            Parser parser = new Parser();
            using (StringReader source_reader = new StringReader(testparts[0]))
            {
                sourceUnit.Parse(factory, (IErrorSink<Span>)factory);
                ast = sourceUnit.Ast;
                Assert.AreEqual(0, factory.Errors.Count);
            }
            Assert.AreEqual(0, factory.Errors.Count);

            var serializer = new JsonNodeWriter();
            TreeSerializer visitor = new TreeSerializer(serializer);
            ast.VisitMe(visitor);

            Regex rgx = new Regex("\"Span\"[^}]*},?"); // omit Span for more compact testing (position must be verified separately)
            // Regex rgx = new Regex(""); // for testing position
            string expected = rgx.Replace(testparts[1].Trim().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);
            string actual = rgx.Replace(serializer.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);

            //Assert.AreEqual(expected.Length, actual.Length);
            //for (int i = 0; i < expected.Length; i++)
            //    Assert.AreEqual(expected[i], actual[i], "difference at " + i.ToString());
            Assert.AreEqual(expected, actual);
        }
    }
}
