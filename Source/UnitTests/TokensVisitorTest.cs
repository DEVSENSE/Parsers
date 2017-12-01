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
    [DeploymentItem("TestData.csv")]
    public class TokensVisitorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void TokensVisitorTest()
        {
            string path = (string)TestContext.DataRow["files"];
            string testcontent = File.ReadAllText(path);

            string[] testparts = testcontent.Split(new string[] { "<<<TEST>>>" }, StringSplitOptions.RemoveEmptyEntries);

            if (testparts.Length >= 2)
            {
                testcontent = testparts[0];
                if (path.Contains("functions1.phpt"))
                {
                    return; // TODO - too slow test 
                }
            }

            var original = testcontent;
            var sourceUnit = new TestSourceUnit(original, path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Php71Set);
            var factory = new BasicNodesFactory(sourceUnit);
            var errors = new TestErrorSink();


            GlobalCode ast = null;

            using (StringReader source_reader = new StringReader(original))
            {
                sourceUnit.Parse(factory, errors, new TestErrorRecovery());
                if(errors.Count != 0)
                {
                    return;// TODO - handle errors
                }
                ast = sourceUnit.Ast;
            }

            var newlineLength = original.Contains("\r\n");
            var provider = SourceTokenProviderFactory.CreateProvider(sourceUnit.SourceLexer.AllTokens, original);
            var whitespace = new WhiteSpaceCommentComposer(provider);
            var composer = new TestComposer(whitespace);
            var visitor = new TokenVisitor(new TreeContext(ast), composer, provider);
            visitor.VisitElement(ast);
            var code = whitespace.Code;

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
            Assert.AreEqual(tokens.Length, whitespace.Processed.Count);
            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.AreEqual(tokens[i].Token, whitespace.Processed[i].Token);
                Assert.AreEqual(tokens[i].Span, whitespace.Processed[i].Span);
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

            public CollectionLexer SourceLexer => _lexer;

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
            List<ISourceToken> _tokens = new List<ISourceToken>();

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

            public List<ISourceToken> AllTokens => _tokens;

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
                    _tokens.Add(new SourceToken(token, TokenPosition));

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


        class WhiteSpaceCommentComposer : ITokenComposer
        {
            public StringBuilder Code { get { ProcessWhitespaces(new Span(int.MaxValue, 0)); return _builder; } }
            private StringBuilder _builder = new StringBuilder();

            public List<ISourceToken> Processed => _processed;
            private List<ISourceToken> _processed = new List<ISourceToken>();

            private Span _previous = Span.Invalid;
            private ISourceTokenProvider _tokens;

            public WhiteSpaceCommentComposer(ISourceTokenProvider tokens)
            {
                _tokens = tokens;
            }

            private void ProcessToken(Tokens token, string text, Span position)
            {
                var start = position.StartOrInvalid;
                var end = start + text.Length;
                if (_builder.Length <= end)
                {
                    _builder.Append(' ', end - _builder.Length);
                }
                if (start >= 0 && text.Length >= 0)
                {
                    _builder.Replace(start, text.Length, text);
                    _processed.Add(new SourceToken(token, position));
                }
            }

            public void ConsumeToken(Tokens token, string text, Span position)
            {
                ProcessWhitespaces(position);
                if (token != Tokens.T_SEMI || _tokens.GetTokenAt(position, Tokens.T_SEMI, null) != null) // TODO - last element without semicolon
                {
                    ProcessToken(token, _tokens.GetTokenText(new SourceToken(token, position), string.Empty), position);
                }
            }

            private void ProcessWhitespaces(Span position)
            {
                if (position.IsValid && (!_previous.IsValid || _previous.End <= position.Start))
                {
                    var whitespaceSpan = Span.FromBounds(_previous.IsValid ? _previous.End : 0, position.Start);
                    var tokens = _tokens.GetTokens(whitespaceSpan, t => t.Token == Tokens.T_WHITESPACE || t.Token == Tokens.T_COMMENT ||
                    t.Token == Tokens.T_DOC_COMMENT || t.Token == Tokens.T_OPEN_TAG || t.Token == Tokens.T_CLOSE_TAG, Enumerable.Empty<ISourceToken>());
                    if (tokens != null)
                    {
                        foreach (var item in tokens)
                        {
                            ProcessToken(item.Token, _tokens.GetTokenText(item, string.Empty), item.Span);
                        }
                    }
                    _previous = position;
                }
            }
        }

        class TestComposer : ITokenComposer
        {

            private ITokenComposer _composer;

            public TestComposer(ITokenComposer composer)
            {
                _composer = composer;
            }

            public void ConsumeToken(Tokens token, string text, Span position)
            {
                _composer.ConsumeToken(token, text, position);
            }

            protected void ConsumeToken(Tokens token, Span position) => ConsumeToken(token, TokenFacts.GetTokenText(token), position);
        }
    }
}
