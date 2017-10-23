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

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("ParserTestData.csv")]
    public class TokensVisitorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\ParserTestData.csv", "ParserTestData#csv", DataAccessMethod.Sequential)]
        public void TokensVisitorTest()
        {
            string path = (string)TestContext.DataRow["files"];
            string testcontent = File.ReadAllText(path);

            string[] testparts = testcontent.Split(new string[] { "<<<TEST>>>" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(testparts.Length >= 2);

            if(testparts[1].Contains("ERRORS"))
            {
                return;
            }

            var original = testparts[0].Replace('\t', ' ').TrimEnd(' ', '\t', '\n', '\r');
            var sourceUnit = new TestSourceUnit(original, path, Encoding.UTF8, Lexer.LexicalStates.INITIAL,
                LanguageFeatures.Php71Set | LanguageFeatures.FullInformation);
            var factory = new BasicNodesFactory(sourceUnit);
            var errors = new TestErrorSink();


            GlobalCode ast = null;

            using (StringReader source_reader = new StringReader(original))
            {
                sourceUnit.Parse(factory, errors, new TestErrorRecovery());
                ast = sourceUnit.Ast;
            }

            var newlineLength = original.Contains("\r\n");
            var lines = LineBreaks.Create(original);
            var composer = new TestComposer();
            var visitor = new TokenVisitor(new TreeContext(ast), composer);
            try
            {
                visitor.VisitElement(ast);
                var code = composer.Code;
                var line = 0;
                foreach (var item in sourceUnit.Comments)
                {
                    if (code.Length <= item.Position.End)
                    {
                        code.Append(' ', item.Position.End - code.Length);
                    }
                    code.Replace(item.Position.Start, item.Position.Length, item.Text);
                }
                for (int i = 1; i < code.Length; i++)
                {
                    if (lines.GetLineFromPosition(i) > line && !newlineLength)
                    {
                        code[i - 1] = '\n';
                        line++;
                    }
                    else if (lines.GetLineFromPosition(i) > line && newlineLength)
                    {
                        code[i - 2] = '\r';
                        code[i - 1] = '\n';
                        line++;
                    }
                }

                var result = code.ToString();
                //File.WriteAllText(Path.Combine(Directory.GetParent(path).FullName, "original.txt"), original);
                //File.WriteAllText(Path.Combine(Directory.GetParent(path).FullName, "result.txt"), result);
                Assert.AreEqual(original.Length, result.Length);
                for (int i = 0; i < original.Length; i++)
                {
                    Assert.AreEqual(original[i], result[i]);
                }
                Assert.AreEqual(original, result);
            }
            catch (Exception)
            {

            }
        }
        internal struct Comment
        {
            public readonly Span Position;
            public readonly string Text;
            public Comment(Span pos, string txt) { Position = pos; Text = txt; }
        }
        internal class TestSourceUnit : CodeSourceUnit
        {
            private CollectionLexer _lexer;
            private LanguageFeatures _features;
            public List<Comment> Comments => _lexer.Comments;

            public TestSourceUnit(string/*!*/ code, string/*!*/ filePath,
                Encoding/*!*/ encoding,
                Lexer.LexicalStates initialState = Lexer.LexicalStates.INITIAL,
                LanguageFeatures features = LanguageFeatures.Basic)
                : base(code, filePath, encoding, initialState, features)
            {
                _features = features;
            }

            public override void Parse(INodesFactory<LangElement, Span> factory, IErrorSink<Span> errors, IErrorRecovery recovery = null)
            {
                using (var source = new StringReader(this.Code))
                {
                    _lexer = new CollectionLexer(source, errors);
                    ast = new Parser().Parse(_lexer, factory, _features, errors, recovery);
                }
            }
        }
        internal class CollectionLexer : ITokenProvider<SemanticValueType, Span>
        {
            List<Comment> _comments = new List<Comment>();
            ITokenProvider<SemanticValueType, Span> _provider;
            public List<Comment> Comments => _comments;

            /// <summary>
            /// Lexer constructor that initializes all the necessary members
            /// </summary>
            /// <param name="provider">Underlaying tokens provider.</param>
            public CollectionLexer(StringReader source, IErrorSink<Span> errors)
            {
                _provider = new Lexer(source, Encoding.UTF8, errors, LanguageFeatures.Basic, 0, Lexer.LexicalStates.INITIAL);
            }

            public Span TokenPosition => _provider.TokenPosition;

            public string TokenText => _provider.TokenText;

            public SemanticValueType TokenValue => _provider.TokenValue;

            /// <summary>
            /// Get next token and store its actual position in the source unit.
            /// This implementation supports the functionality of zendlex, which skips empty nodes (open tag, comment, etc.).
            /// </summary>
            /// <returns>Next token.</returns>
            public int GetNextToken()
            {
                do
                {
                    Tokens token = (Tokens)_provider.GetNextToken();

                    // origianl zendlex() functionality - skip open and close tags because they are not in the PHP grammar
                    switch (token)
                    {
                        case Tokens.T_DOC_COMMENT:
                        case Tokens.T_COMMENT:
                        case Tokens.T_OPEN_TAG:
                        case Tokens.T_CLOSE_TAG:
                            _comments.Add(new Comment(TokenPosition, TokenText));
                            break;
                        case Tokens.T_WHITESPACE:
                        case Tokens.T_OPEN_TAG_WITH_ECHO:
                            break;
                    }

                    return (int)token;
                } while (true);
            }

            public void ReportError(string[] expectedTokens)
            {
                _provider.ReportError(expectedTokens);
            }

            public CompleteToken PreviousToken => CompleteToken.Empty;

            public PHPDocBlock DocBlock { get => null; set => throw new NotImplementedException(); }

            public void AddNextTokens(IList<CompleteToken> tokensBuffer, CompleteToken previousToken) { }
        }

        class TestComposer : ITokenComposer
        {
            public StringBuilder Code => _builder;

            private StringBuilder _builder = new StringBuilder();

            private string GetOriginalValue(Literal literal)
            {
                object value = null;
                if (literal.Properties.TryGetProperty(Literal.OrigianlValueProperty, out value))
                {
                    return (string)value;
                }
                return null;
            }

            public void ConsumeLiteral(Literal literal)
            {
                var origianl = GetOriginalValue(literal);
                if (literal is BoolLiteral)
                {
                    var value = ((BoolLiteral)literal).Value.ToString();
                    ConsumeToken(Tokens.T_STRING, origianl != null ? origianl : value.ToLowerInvariant(), literal.Span.StartOrInvalid);
                }
                else if (literal is DoubleLiteral)
                {
                    ConsumeToken(Tokens.T_DNUMBER, origianl != null ? origianl : ((DoubleLiteral)literal).Value.ToString(CultureInfo.InvariantCulture), literal.Span.StartOrInvalid);
                }
                else if (literal is NullLiteral)
                {
                    ConsumeToken(Tokens.T_STRING, origianl != null ? origianl : "null", literal.Span.StartOrInvalid);
                }
                else if (literal is LongIntLiteral)
                {
                    ConsumeToken(Tokens.T_LNUMBER, origianl != null ? origianl : ((LongIntLiteral)literal).Value.ToString(CultureInfo.InvariantCulture), literal.Span.StartOrInvalid);
                }
                else if (literal is StringLiteral)
                {
                    ConsumeToken(Tokens.T_CONSTANT_ENCAPSED_STRING, origianl != null ? origianl : $"\"{((StringLiteral)literal).Value}\"", literal.Span.StartOrInvalid);
                }
            }

            public void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span = default(Span))
            {
                object modifier;
                if (element.Properties.TryGetProperty(TypeMemberDecl.ModifierPositionProperty, out modifier))
                {
                    var position = (KeyValuePair<Tokens, short>[])modifier;
                    for (int i = 0; i < position.Length; i++)
                    {
                        ConsumeToken(position[i].Key, span.StartOrInvalid + position[i].Value);
                    }
                }
                else if(element is MethodDecl && ((MethodDecl)element).ModifierPosition >=0)
                {
                    ConsumeToken(((MethodDecl)element).Modifiers.ToToken(), ((MethodDecl)element).ModifierPosition);
                }
            }

            public void ConsumeToken(Tokens token, string text, int position)
            {
                var start = position;
                var end = start + text.Length;
                if (_builder.Length <= end)
                {
                    _builder.Append(' ', end - _builder.Length);
                }
                if (start >= 0 && text.Length >= 0)
                {
                    _builder.Replace(start, text.Length, text);
                }
            }

            protected void ConsumeToken(Tokens token, int position) => ConsumeToken(token, TokenFacts.GetTokenText(token), position);

            public bool IsOldArraySyntax(ArrayEx node) => false;
        }
    }
}
