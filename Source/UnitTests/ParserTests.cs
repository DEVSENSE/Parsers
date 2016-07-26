using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PHP.Syntax;
using System.IO;
using PhpParser.Parser;
using PHP.Core.AST;
using UnitTests.TestImplementation;
using PhpParser;

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

            using (StringReader source_reader = new StringReader(sourceTest[0]))
            {
                TestNodesFactory astFactory = new TestNodesFactory(sourceUnit);
                Lexer lexer = new Lexer(source_reader, sourceUnit, astFactory, LanguageFeatures.ShortOpenTags);
                Parser parser = new Parser();
                ast = (GlobalCode)parser.Parse(lexer, astFactory, LanguageFeatures.ShortOpenTags);
                Assert.AreEqual(1, astFactory.Errors.Count);
            }

            //Assert.IsNotNull(ast);
            //Assert.AreEqual(1, ast.Statements.Length);
            //Assert.IsFalse(string.IsNullOrEmpty(((StringLiteral)((EchoStmt)ast.Statements[0]).Parameters[0]).Value));
            //Assert.IsTrue(((StringLiteral)((EchoStmt)ast.Statements[0]).Parameters[0]).Value.Contains("https://github.com/php/php-src/tree/master/Zend/tests"));

            var serializer = new JsonSerializer();
            SerializerTreeVisitor visitor = new SerializerTreeVisitor(serializer);
            ast.VisitMe(visitor);
            string expected = sourceTest[1].Trim().Replace("\r", "").Replace("\n", "").Replace(" ", "");
            string actual = serializer.ToString().Replace("\r", "").Replace("\n", "").Replace(" ", "");
            Assert.AreEqual(expected.Length, actual.Length);
            //for (int i = 0; i < expected.Length; i++)
            //    Assert.AreEqual(expected[i], actual[i], "difference at " + i.ToString());
            Assert.AreEqual(expected, actual);
        }
    }
}
