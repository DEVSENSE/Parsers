﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Devsense.PHP.Syntax.Ast;
using System.Text.RegularExpressions;
using Devsense.PHP.Syntax.Ast.Serialization;
using Devsense.PHP.Syntax;
using System.Text;
using System.Linq;
using Devsense.PHP.Errors;
using Devsense.PHP.Text;
using System.Collections.Generic;
using UnitTests.TestImplementation;

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("ParserTestData.csv")]
    public class ParserTests
    {
        public TestContext TestContext { get; set; }
        public const string Errors = "ERRORS:";
        const string Pattern = @"\s*" + Errors + @"\s*(?<Number>\d*)\s*(?<JSON>.*)";
        private Regex ErrorRegex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.Singleline);

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\ParserTestData.csv", "ParserTestData#csv", DataAccessMethod.Sequential)]
        public void ParserParseTest()
        {
            string path = (string)TestContext.DataRow["files"];
            string testcontent = File.ReadAllText(path);

            string[] testparts = testcontent.Split(new string[] { "<<<TEST>>>" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(testparts.Length >= 2);

            var sourceUnit = new CodeSourceUnit(testparts[0], path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Php71Set);
            var factory = new BasicNodesFactory(sourceUnit);
            var errors = new TestErrorSink();

            GlobalCode ast = null;

            Parser parser = new Parser();
            using (StringReader source_reader = new StringReader(testparts[0]))
            {
                sourceUnit.Parse(factory, errors);
                ast = sourceUnit.Ast;
            }
            if (testparts[1].TrimStart().StartsWith(Errors))
            {
                var matches = ErrorRegex.Matches(testparts[1]);
                Assert.AreEqual(1, matches.Count, path);
                Assert.AreEqual(1, errors.Count, path);
                int errorid = 0;
                Assert.IsTrue(int.TryParse(matches[0].Groups["Number"].Value, out errorid), path);
                Assert.AreEqual(errorid, errors.Errors.Single().Error.Id, path);
                testparts[1] = matches[0].Groups["JSON"].Value;
            }
            else
            {
                Assert.AreEqual(0, errors.Count, path);
            }

            var serializer = new JsonNodeWriter();
            TreeSerializer visitor = new TreeSerializer(serializer);
            ast.VisitMe(visitor);

            Regex rgx = new Regex(@"""Span""[^}]*},?\s*\n?"); // omit Span for more compact testing (position must be verified separately)
            string expected = rgx.Replace(testparts[1].Trim().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);
            string actual = rgx.Replace(serializer.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);

            if (testparts[1].Trim() != "<<<IGNORE>>>")
            {
                // IMPORTANT - Uncomment to regenerate test data
                //File.WriteAllText(path, testparts[0] + "\n<<<TEST>>>\n" + rgx.Replace(serializer.ToString(), string.Empty));
                Assert.AreEqual(expected, actual, path);
            }

            // check every node has a parent
            var parentChecker = new ContainingElementCheck();
            parentChecker.VisitGlobalCode(ast);

            // check every node has a parent
            var spanChecker = new NameSpanCheck();
            spanChecker.VisitGlobalCode(ast);
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

        /// <summary>
        /// Helper visitor checking every node has a containing element.
        /// </summary>
        sealed class NameSpanCheck : TreeVisitor
        {
            List<Span> inclusion = new List<Span>() { new Span(0, int.MaxValue) };

            public override void VisitElement(LangElement element)
            {
                if (element != null)
                {
                    Assert.IsTrue(element.Span.IsValid);
                    Assert.IsTrue(element is PHPDocBlock || inclusion.Last().Contains(element.Span));
                    if (element is FunctionDecl)
                        CheckFunctionDecl((FunctionDecl)element);
                    else if (element is MethodDecl)
                        CheckMethodDecl((MethodDecl)element);
                    else if (element is LambdaFunctionExpr)
                        CheckLambdaDecl((LambdaFunctionExpr)element);
                    else if (element is TypeDecl)
                        CheckTypeDecl((TypeDecl)element);

                    if (element is NamespaceDecl && ((NamespaceDecl)element).IsSimpleSyntax)
                        inclusion.Add(Span.Combine(element.Span, ((NamespaceDecl)element).Body.Span));
                    else if (element is VarLikeConstructUse && ((VarLikeConstructUse)element).IsMemberOf != null)
                        inclusion.Add(Span.Combine(((VarLikeConstructUse)element).IsMemberOf.Span, element.Span));
                    else
                        inclusion.Add(element.Span);
                    base.VisitElement(element);
                    inclusion.RemoveAt(inclusion.Count - 1);
                }
            }

            void CheckFunctionDecl(FunctionDecl func)
            {
                Assert.IsTrue(func.Span.Contains(func.Name.Span));
                Assert.IsTrue(func.Span.Contains(func.HeadingSpan));
                Assert.IsTrue(func.Span.Contains(func.ParametersSpan));
                Assert.IsTrue(func.Span.Contains(func.Body.Span));
                Assert.IsTrue(func.HeadingSpan.Contains(func.Name.Span));
                Assert.IsTrue(func.HeadingSpan.Contains(func.ParametersSpan));
                Assert.IsTrue(func.HeadingSpan.End <= func.Body.Span.Start);
                Assert.IsTrue(func.Name.Span.End <= func.ParametersSpan.Start);
                foreach (var param in func.Signature.FormalParams)
                    Assert.IsTrue(param.Span.Contains(param.Name.Span));
            }

            void CheckMethodDecl(MethodDecl method)
            {
                Assert.IsTrue(method.Span.Contains(method.Name.Span));
                Assert.IsTrue(method.Span.Contains(method.HeadingSpan));
                Assert.IsTrue(method.Span.Contains(method.ParametersSpan));
                Assert.IsTrue(method.HeadingSpan.Contains(method.Name.Span));
                Assert.IsTrue(method.HeadingSpan.Contains(method.ParametersSpan));
                if (method.Body != null)
                {
                    Assert.IsTrue(method.Span.Contains(method.Body.Span));
                    Assert.IsTrue(method.HeadingSpan.End <= method.Body.Span.Start);
                }
                Assert.IsTrue(method.Name.Span.End <= method.ParametersSpan.Start);
            }

            void CheckLambdaDecl(LambdaFunctionExpr lambda)
            {
                Assert.IsTrue(lambda.Span.Contains(lambda.HeadingSpan));
                Assert.IsTrue(lambda.Span.Contains(lambda.ParametersSpan));
                Assert.IsTrue(lambda.Span.Contains(lambda.Body.Span));
                Assert.IsTrue(lambda.HeadingSpan.Contains(lambda.ParametersSpan));
                Assert.IsTrue(lambda.HeadingSpan.End <= lambda.Body.Span.Start);
            }

            void CheckTypeDecl(TypeDecl type)
            {
                Assert.IsTrue(type.Name.HasValue);
                Assert.IsTrue(type is AnonymousTypeDecl || type.Span.Contains(type.Name.Span));
                Assert.IsTrue(type.Span.Contains(type.HeadingSpan));
                foreach (var member in type.Members)
                    Assert.IsTrue(type.Span.Contains(member.Span));
                Assert.IsTrue(type is AnonymousTypeDecl || type.HeadingSpan.Contains(type.Name.Span));
                foreach (var implements in type.ImplementsList)
                {
                    Assert.IsTrue(type.HeadingSpan.Contains(implements.Span));
                    Assert.IsTrue(type.Span.Contains(implements.Span));
                }
                if (type.Members.Count > 0)
                    Assert.IsTrue(type.HeadingSpan.End <= type.Members.Min(a => a.Span.Start));
                if (type.ImplementsList.Length > 0)
                    Assert.IsTrue(type.Name.Span.End <= type.ImplementsList.Min(a => a.Span.Start));
                if (type.BaseClass != null)
                {
                    Assert.IsTrue(type.Span.Contains(type.BaseClass.Span));
                    Assert.IsTrue(type.HeadingSpan.Contains(type.BaseClass.Span));
                    Assert.IsTrue(type.Name.Span.End <= type.BaseClass.Span.Start);
                }
            }
        }
    }
}
