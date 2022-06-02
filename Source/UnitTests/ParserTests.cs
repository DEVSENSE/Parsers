using System;
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
        [TestMethod]
        public void SimpleParseTest()
        {
            var codes = new[] {
@"<?php
class X {
    function static() { }
}",
@"<?php
class enum extends A {
}",
@"<?php
A::E->foo(); // dereferencable class const
",
            };

            foreach (var code in codes)
            {
                var sourceUnit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Basic);
                var factory = new BasicNodesFactory(sourceUnit);
                var errors = new TestErrorSink();

                sourceUnit.Parse(factory, errors, new TestErrorRecovery());

                Assert.IsNotNull(sourceUnit.Ast);
            }
        }

        public TestContext TestContext { get; set; }
        public const string Errors = "ERRORS:";
        public const string Pattern = @"\s*" + Errors + @"\s*(?<Number>\d*(, \d*)*)\s*(?<JSON>.*)";
        private Regex _errorRegex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.Singleline);

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
            var errors = new TestErrorSink();

            //
            sourceUnit.Parse(factory, errors, new TestErrorRecovery());

            //
            if (testparts[1].TrimStart().StartsWith(Errors))
            {
                var matches = _errorRegex.Matches(testparts[1]);
                var knownErrors = matches[0].Groups["Number"].Value.Split(',');
                Assert.AreEqual(1, matches.Count, path);
                Assert.AreEqual(knownErrors.Length, errors.Count, path);
                int errorid = 0;
                for (int i = 0; i < knownErrors.Length; i++)
                {
                    Assert.IsTrue(int.TryParse(knownErrors[i], out errorid), path);
                    Assert.AreEqual(errorid, errors.Errors[i].Error.Id, path);
                    Assert.IsNotNull(errors.Errors[i].ToString());
                }
                testparts[1] = matches[0].Groups["JSON"].Value;
            }
            else
            {
                Assert.AreEqual(0, errors.Count, path);
            }

            Assert.IsNotNull(sourceUnit.Ast);

            var serializer = new JsonNodeWriter();
            TreeSerializer visitor = new TreeSerializer(serializer);
            sourceUnit.Ast.VisitMe(visitor);

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
            parentChecker.VisitGlobalCode(sourceUnit.Ast);

            // check nodes have correct span corresponding to correct source text
            var spanChecker = new NameSpanCheck(testparts[0]);
            spanChecker.VisitGlobalCode(sourceUnit.Ast);
        }

        [TestMethod]
        public void ArrowFuncTest()
        {
            var codes = new[] {
                @"<?php $foo = fn() => 1;",
                @"<?php $foo = static fn() => 1;"
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
        public void SingleByteStringTest()
        {
            var codes = new[] {
                @"<?php echo ""\x99\x7a\x7b"";",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
        public void ReturnTypeTest()
        {
            var codes = new[] {
                @"<?php class X { function foo(): static { } }",
                @"<?php class X { function foo(): never { } }",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
        public void HeredocTest()
        {
            var codes = new[] {
               @"<?php
$x = <<<XXX

  /**
   * text
   */

  XXX;",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
        public void AttributesTest()
        {
            var codes = new[] {
                @"<?php #[ClassName(1,2,3)]class X { }",
                @"<?php #[ClassName]class X { }",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
            }
        }

        [TestMethod]
        public void PhpDocAttributesTest()
        {
            var codes = new[] {
                @"<?php
/** phpdoc */
#[ClassName(1,2,3)]
class X {
    /** phpdoc */
    #[ClassName]
    function foo() { }
}",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);

                foreach (var tdecl in unit.Ast.TraverseNamedTypeDeclarations())
                {
                    Assert.IsNotNull(tdecl.PHPDoc);

                    foreach (var m in tdecl.Members.OfType<MethodDecl>())
                    {
                        Assert.IsNotNull(m.PHPDoc);
                    }
                }
            }
        }

        [TestMethod]
        public void NamedArgTest()
        {
            var codes = new[] {
                @"<?php array_fill(start_index: 0, num: 100, value: 50);",
                @"<?php implode(separator: '.', array: []);"
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
            }
        }

        [TestMethod]
        public void MatchTest()
        {
            var codes = new[] {
                @"<?php echo match($x) { 0 => 'hello', 1, 2, 3 => 'world', default => '!', };",
            };

            foreach (var code in codes)
            {
                var errors = new TestErrorSink();
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, features: LanguageFeatures.Php80Set);
                unit.Parse(new BasicNodesFactory(unit), errors);

                Assert.AreEqual(0, errors.Count);
            }
        }

        [TestMethod]
        public void InstanceOfTest()
        {
            var codes = new[] {
                @"<?php echo $x instanceof ((string)$y);",
                @"<?php echo $x instanceof $y[0];",
            };

            foreach (var code in codes)
            {
                var errors = new TestErrorSink();
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, features: LanguageFeatures.Php80Set);
                unit.Parse(new BasicNodesFactory(unit), errors);

                Assert.AreEqual(0, errors.Count);
            }
        }

        [TestMethod]
        public void EnumerationTest()
        {
            var codes = new[] {
                @"<?php enum Suit { case Hearts; case Diamonds; case Clubs; case Spades; }",
                @"<?php enum Suit: int { case Hearts = 1; case Diamonds = 2; case Clubs = 3; case Spades = 4; }",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, features: LanguageFeatures.Php81Set);
                unit.Parse(new BasicNodesFactory(unit), null);

                Assert.IsNotNull(unit.Ast);
                Assert.IsInstanceOfType(unit.Ast.Statements[0], typeof(NamedTypeDecl));
            }
        }

        [TestMethod]
        public void AliasesTest()
        {
            var codes = new[] {
                @"<?php use A\{X,Y,};", // <-- trailing comma
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
        public void StringLiteralTest()
        {
            var codes = new[] {
                @"<?php echo ""\\test"";",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        /// <summary>
        /// Helper visitor checking every node has a containing element.
        /// </summary>
        sealed class ContainingElementCheck : TreeVisitor
        {
            public override void VisitElement(ILangElement element)
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
            string _originalText;

            public NameSpanCheck(string text)
            {
                _originalText = text;
            }

            List<Span> inclusion = new List<Span>() { new Span(0, int.MaxValue) };

            public override void VisitElement(ILangElement element)
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
                    else if (element is StringLiteral)
                        CheckStringValue((StringLiteral)element);

                    if (element is NamespaceDecl && ((NamespaceDecl)element).IsSimpleSyntax)
                        inclusion.Add(Span.Combine(element.Span, ((NamespaceDecl)element).Body.Span));
                    else if (element is VarLikeConstructUse && ((VarLikeConstructUse)element).IsMemberOf != null)
                        inclusion.Add(Span.Combine(((VarLikeConstructUse)element).IsMemberOf.Span, element.Span));
                    else
                        inclusion.Add(element.Span);
                    base.VisitElement(element);
                    inclusion.RemoveAt(inclusion.Count - 1);
                    if (element is NamespaceDecl)
                    {
                        Assert.IsNotNull(((NamespaceDecl)element).Body);
                    }
                }
            }

            private void CheckStringValue(StringLiteral element)
            {
                // NOTE: this is not correct !!!

                int start = element.ContainingElement is EncapsedExpression.HereDocExpression ||
                    element.ContainingElement is EncapsedExpression.NowDocExpression ||
                    element.ContainingElement is EchoStmt echo && echo.IsHtmlCode ||
                    element.ContainingElement is ItemUse && _originalText[element.Span.Start] != '\''
                    && _originalText[element.Span.Start] != '"' || element.ContainingElement is ConcatEx ? 0 : 1;
                var original = _originalText.Substring(element.Span.Start + start, element.Span.Length - start - start);
                if (element.ContainingElement is EncapsedExpression.HereDocExpression)
                {
                    var matches = Regex.Matches(original, "\\n(\\s*)");
                    if (matches.Count != 0)
                    {
                        string prefix = original;
                        for (int i = 0; i < matches.Count; i++)
                        {
                            if (prefix.StartsWith(matches[i].Groups[1].Value))
                            {
                                prefix = matches[i].Groups[1].Value;
                            }
                            else if (!matches[i].Groups[1].Value.StartsWith(prefix))
                            {
                                prefix = string.Empty;
                            }
                        }
                        original = original.Substring(prefix.Length).Replace("\n" + prefix, "\n");
                    }
                }
                if (start == 1 && _originalText[element.Span.Start] == '\'')
                    original = original.Replace("\\'", "'").Replace("\\\\", "\\");
                else
                    original = original.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\\\", "\\")
                        .Replace("\\\"", "\"").Replace("\\t", "\t").Replace("\\v", "\v").Replace("\\\\$", "\\$")
                        .Replace("\\e", "\u001b").Replace("\\f", "\f").Replace("\\75", "=").Replace("\\`", "`");
                Assert.AreEqual(original.TrimEnd(), element.Value.TrimEnd()); // TODO: FIX: HereDoc string literal span includes the closing newline
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
