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
using System.Collections.Generic;

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
            var errors = new ErrorSink();

            GlobalCode ast = null;

            Parser parser = new Parser();
            using (StringReader source_reader = new StringReader(testparts[0]))
            {
                sourceUnit.Parse(factory, errors);
                ast = sourceUnit.Ast;
            }
            Assert.AreEqual(0, errors.Count);

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

            // check every node has a parent
            var checker = new ContainingElementCheck();
            checker.VisitGlobalCode(ast);
        }

        sealed class ErrorSink : IErrorSink<Span>
        {
            public class ErrorInstance
            {
                public Span Span;
                public ErrorInfo Error;
                public string[] Args;
            }

            public readonly List<ErrorInstance> Errors = new List<ErrorInstance>();

            public int Count => this.Errors.Count;

            public void Error(Span span, ErrorInfo info, params string[] argsOpt)
            {
                Errors.Add(new ErrorInstance()
                {
                    Span = span,
                    Error = info,
                    Args = argsOpt,
                });
            }
        }

        /// <summary>
        /// Helper visitor checking every node has a containing element.
        /// </summary>
        sealed class ContainingElementCheck : TreeVisitor
        {
            public override void VisitElement(LangElement element)
            {
                if (element != null)
                {
                    Assert.IsNotNull(element.ContainingElement);

                    base.VisitElement(element);
                }
            }
        }
    }
}
