using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.TestImplementation;
using Devsense.PHP.Text;
using System.Globalization;
using Devsense.PHP.Errors;
using System.Text.RegularExpressions;
using Devsense.PHP.Syntax.Ast.Serialization;
using Devsense.PHP.Syntax.Visitor;

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("TestData.csv")]
    public class TokensVisitorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void TokensVisitorTest()
        {
            string path = (string)TestContext.DataRow["files"];
            if (path.Contains("functions1.phpt"))
            {
                return; // TODO - too slow test 
            }
            string testcontent = File.ReadAllText(path);
            var original = testcontent;
            var sourceUnit = new TestSourceUnit(original, path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Php71Set);
            var factory = new BasicNodesFactory(sourceUnit);
            var errors = new TestErrorSink();

            GlobalCode ast = null;

            sourceUnit.Parse(factory, errors, new TestErrorRecovery());
            ast = sourceUnit.Ast;
            if (errors.Count != 0)
            {
                return; // AST is null or invalid
            }

            var provider = SourceTokenProviderFactory.CreateProvider(sourceUnit.SourceLexer.AllTokens, original);
            var composer = new WhitespaceComposer(provider);
            var visitor = new TokenVisitor(new TreeContext(ast), composer, provider);
            visitor.VisitElement(ast);
            var code = composer.Code;

            var result = code.ToString();
            //File.WriteAllText(Path.Combine(Directory.GetParent(path).FullName, "original.txt"), original);
            //File.WriteAllText(Path.Combine(Directory.GetParent(path).FullName, "result.txt"), result);
            //Assert.AreEqual(original.Length, result.Length);
            //for (int i = 0; i < original.Length; i++)
            //{
            //    Assert.AreEqual(original[i], result[i]);
            //}
            Assert.AreEqual(original, result);
            var tokens = provider.GetTokens(new Span(0, original.Length)).AsArray();
            Assert.AreEqual(tokens.Length, composer.Processed.Count);
            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.AreEqual(tokens[i].Token, composer.Processed[i].Token);
                Assert.AreEqual(tokens[i].Span, composer.Processed[i].Span);
            }
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void EmptyTokensVisitorTest()
        {
            string path = (string)TestContext.DataRow["files"];
            if (path.Contains("functions1.phpt"))
            {
                return; // TODO - too slow test 
            }
            string testcontent = File.ReadAllText(path);
            var original = testcontent;
            if (original.Contains("namespace\\"))
            {
                return; // TODO - current namespace cannot be decided from AST 
            }

            var sourceUnit = new TestSourceUnit(original, path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Php71Set);
            var factory = new BasicNodesFactory(sourceUnit);
            var errors = new TestErrorSink();
            sourceUnit.Parse(factory, errors, new TestErrorRecovery());
            GlobalCode ast = sourceUnit.Ast;
            if (errors.Count != 0)
            {
                return; // AST is null or invalid
            }

            var provider = SourceTokenProviderFactory.CreateEmptyProvider();
            var composer = new EmptyComposer(provider);
            var visitor = new TokenVisitor(new TreeContext(ast), composer, provider);
            visitor.VisitElement(ast);
            var code = composer.Code.ToString();

            var expectedStr = PrepareString(original);
            var actualStr = PrepareString(code);
            Assert.AreEqual(expectedStr, actualStr);
            var expected = FilterTokens(sourceUnit.SourceLexer.AllTokens);
            var actual = FilterTokens(composer.Processed);
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < Math.Min(expected.Length, actual.Length); i++)
            {
                if (expected[i].Token == Tokens.T_SEMI && actual[i].Token == Tokens.T_CASE)
                {

                }
                if (expected[i].Token == Tokens.T_LOGICAL_OR && actual[i].Token == Tokens.T_BOOLEAN_OR ||
                    expected[i].Token == Tokens.T_LOGICAL_AND && actual[i].Token == Tokens.T_BOOLEAN_AND)
                {
                }
                else
                {
                    Assert.AreEqual(expected[i].Token, actual[i].Token);
                }
            }


            sourceUnit = new TestSourceUnit(code, path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Php71Set);
            sourceUnit.Parse(factory, errors, new TestErrorRecovery());
            var newAst = sourceUnit.Ast;
            Assert.IsNotNull(newAst, "Generated code could not be parsed. (Generated code is not valid)");
            var serializer = new JsonNodeWriter();
            var serializerVisitor = new TreeSerializer(serializer);
            ast.VisitMe(visitor);
            expectedStr = serializer.ToString();
            serializer = new JsonNodeWriter();
            serializerVisitor = new TreeSerializer(serializer);
            newAst.VisitMe(visitor);
            actualStr = serializer.ToString();
            Assert.AreEqual(expectedStr, actualStr);
        }

        private ISourceToken[] FilterTokens(IList<ISourceToken> tokens)
        {
            var filtered = tokens.Where(t =>
            t.Token != Tokens.T_WHITESPACE &&       // whitespaces
            t.Token != Tokens.T_COMMENT &&          // comments
            t.Token != Tokens.T_DOC_COMMENT &&      // comments
            t.Token != Tokens.T_PUBLIC &&           // default public 
            t.Token != Tokens.T_VAR &&              // var replaced by default public
            t.Token != Tokens.END                   // empty end token
            ).AsArray();
            var result = new List<ISourceToken>();
            for (int i = 0; i < filtered.Length; i++)
            {
                if (i + 2 < filtered.Length && filtered[i].Token == Tokens.T_LBRACE &&
                    filtered[i + 1].Token == Tokens.T_SEMI && filtered[i + 2].Token == Tokens.T_CASE)
                {   // leading empty semicolon in switch
                    result.Add(filtered[i]);
                    result.Add(filtered[i + 2]);
                    i += 2;
                }
                else if (i + 2 < filtered.Length && filtered[i].Token == Tokens.T_RPAREN &&
                   filtered[i + 1].Token == Tokens.T_COLON && filtered[i + 2].Token == Tokens.T_CASE)
                {   // switch with colon
                    result.Add(filtered[i]);
                    result.Add(new SourceToken(Tokens.T_LBRACE, Span.Invalid));
                    result.Add(filtered[i + 2]);
                    i += 2;
                }
                else if (i + 3 < filtered.Length && filtered[i].Token == Tokens.T_RPAREN &&
                   filtered[i + 1].Token == Tokens.T_COLON && filtered[i + 2].Token == Tokens.T_SEMI &&
                   filtered[i + 3].Token == Tokens.T_CASE)
                {   // leading empty semicolon in switch with colon
                    result.Add(filtered[i]);
                    result.Add(new SourceToken(Tokens.T_LBRACE, Span.Invalid));
                    result.Add(filtered[i + 3]);
                    i += 3;
                }
                else if (i + 2 < filtered.Length && filtered[i].Token == Tokens.T_EXIT &&
                   filtered[i + 1].Token == Tokens.T_LPAREN && filtered[i + 2].Token == Tokens.T_RPAREN)
                {   // empty exit with parentheses
                    result.Add(filtered[i]);
                    i += 2;
                }
                else if (i + 1 < filtered.Length && filtered[i].Token == Tokens.T_ENDSWITCH &&
                   filtered[i + 1].Token == Tokens.T_SEMI)
                {   // endswitch
                    result.Add(new SourceToken(Tokens.T_RBRACE, Span.Invalid));
                    i++;
                }
                else if (i + 1 < filtered.Length && filtered[i].Token == Tokens.T_SEMI &&
                   filtered[i + 1].Token == Tokens.T_CLOSE_TAG)
                {   // semicolon before close tag
                    result.Add(new SourceToken(Tokens.T_CLOSE_TAG, Span.Invalid));
                    i++;
                }
                else if (i + 1 < filtered.Length && filtered[i].Token == Tokens.T_OPEN_TAG &&
                   filtered[i + 1].Token == Tokens.T_CLOSE_TAG)
                {   // empty php block
                    i++;
                }
                else if (i == filtered.Length - 1 && filtered[i].Token == Tokens.T_OPEN_TAG)
                {   // empty open php block
                    i++;
                }
                else if (i == filtered.Length - 1 && filtered[i].Token == Tokens.T_SEMI)
                {   // closing semicolon and close tag
                    result.Add(new SourceToken(Tokens.T_CLOSE_TAG, Span.Invalid));
                    i++;
                }
                else if (filtered[i].Token == Tokens.T_OPEN_TAG_WITH_ECHO)
                {   // open tag with echo repalced by open tag and echo
                    result.Add(new SourceToken(Tokens.T_OPEN_TAG, Span.Invalid));
                    result.Add(new SourceToken(Tokens.T_ECHO, Span.Invalid));
                }
                else
                {
                    result.Add(filtered[i]);
                }
            }
            if (result.Count != 0 && result.Last().Token == Tokens.T_CLOSE_TAG)
            {
                result.RemoveLast();
            }
            return result.AsArray();
        }

        private string PrepareString(string data)
        {
            return
            Regex.Replace(
                Regex.Replace(
                    Regex.Replace(
                        Regex.Replace(
                            Regex.Replace(
                                Regex.Replace(
                                    Regex.Replace(
                                        Regex.Replace(
                                            Regex.Replace(
                                                data,
                                            @"/\*+[^/]*", "*"). // block comments
                                            Replace("*/", ""),  // block comments end
                                        @"\*$", ""),            // block comment before EOF
                                    @"//.*\n", "\n"),           // line comments
                                @"//.*$", ""),                  // line comments before EOF
                            @"\s+", ""),                        // whitespaces
                        @"\?>$", ";"),                          // terminal close tag
                    @";+$", ""),                                // terminal semicolon
                @"switch([^:{};>]*):", "switch$1{"),            // switch {} vs switch : endswitch; 
            @"<\?php\s*$", "").                                 // empty opened php block
            Replace("public", "").                              // default public
            Replace("die(", "exit(").                           // die vs exit
            Replace("die;", "exit;").                           // die vs exit without parameters
            Replace("exit();", "exit;").                        // exit without parameters
            Replace("||", "or").                                // boolean vs logical or
            Replace("&&", "and").                               // boolean vs logical and
            Replace(";?>", "?>").                               // semicolon before closetag
            Replace("{;case", "{case").                         // leading empty semicolon in switch
            Replace("<?php?>", "").                             // empty php block
            Replace("var$", "$").                               // var modifier replaced by default public
            Replace("<?=", "<?phpecho").                        // open tag with echo
            Replace("endswitch;", "}").                         // endswitch vs }
            Replace("endswitch?", "}?");                        // endswitch vs } before close tag
        }
    }
}
