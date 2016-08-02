using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHP.Syntax;
using System.IO;
using PhpParser.Parser;
using PHP.Core.AST;
using PhpParser;
using System.Text.RegularExpressions;

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
            SourceUnit sourceUnit = new CodeSourceUnit(File.ReadAllText(path), path, new System.Text.ASCIIEncoding(), Lexer.LexicalStates.INITIAL);

            GlobalCode ast = null;
            string source = File.ReadAllText(path);
            string[] sourceTest = source.Split(new string[] { "<<<TEST>>>" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(sourceTest.Length >= 2);

            BasicNodesFactory astFactory = new BasicNodesFactory(sourceUnit);
            Parser parser = new Parser();
            using (StringReader source_reader = new StringReader(sourceTest[0]))
            {
                Lexer lexer = new CompliantLexer(source_reader, sourceUnit, astFactory, LanguageFeatures.ShortOpenTags);
                ast = (GlobalCode)parser.Parse(lexer, astFactory, LanguageFeatures.ShortOpenTags);
                Assert.AreEqual(0, astFactory.Errors.Count);
            }
            Assert.AreEqual(0, astFactory.Errors.Count);

            var serializer = new JsonSerializer();
            SerializerTreeVisitor visitor = new SerializerTreeVisitor(serializer);
            ast.VisitMe(visitor);

            Regex rgx = new Regex("\"Span\"[^}]*},?"); // omit Span for more compact testing (position must be verified separately)
            // Regex rgx = new Regex(""); // for testing position
            string expected = rgx.Replace(sourceTest[1].Trim().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);
            string actual = rgx.Replace(serializer.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);

            //Assert.AreEqual(expected.Length, actual.Length);
            //for (int i = 0; i < expected.Length; i++)
            //    Assert.AreEqual(expected[i], actual[i], "difference at " + i.ToString());
            Assert.AreEqual(expected, actual);
        }
    }
}
