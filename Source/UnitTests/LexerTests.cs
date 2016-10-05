using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Devsense.PHP.Text;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnitTests.TestImplementation;
using Devsense.PHP.Syntax;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("TestData.csv")]
    [DeploymentItem(@"..\..\Tokens.php")]
    public class LexerTests
    {
        public TestContext TestContext { get; set; }

        private string ParseByPhp(string path)
        {
            Process process = new Process();
            StringBuilder output = new StringBuilder();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = @"..\..\..\..\Tools\PHP v7.0\php.exe";
            process.StartInfo.Arguments = "-f tokens.php " + path;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            while (!process.HasExited)
                output.Append(process.StandardOutput.ReadToEnd());
            process.WaitForExit();// Waits here for the process to exit.
            return output.ToString();
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void LexerConstructorTest()
        {
            string path = (string)TestContext.DataRow["files"];
            SourceUnit sourceUnit = new CodeSourceUnit(File.ReadAllText(path), path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Basic);
            ITokenProvider<SemanticValueType, Span> lexer = new Lexer(new StreamReader(path), Encoding.UTF8, new TestErrorSink(),
                LanguageFeatures.ShortOpenTags, 0, Lexer.LexicalStates.INITIAL);
            Assert.AreNotEqual(null, lexer);
        }

        bool Increment(int[] word, int n)
        {
            for (int i = 0; i < word.Length; i++)
            {
                word[i] = (word[i] + 1) % n;
                if (word[i] != 0)
                    return true;
            }
            return false;
        }

        void ToArray(int[] word, char[] text, char[] chars)
        {
            for (int i = 0; i < word.Length; i++)
                text[i] = chars[word[i]];
        }

        [TestMethod]
        public void LexerStringsTest()
        {
            TestErrorSink errorSink = new TestErrorSink();
            Lexer lexer = new Lexer(new StringReader("\"\""), Encoding.UTF8, errorSink,
                LanguageFeatures.ShortOpenTags, 0, Lexer.LexicalStates.INITIAL);

            var chars = new[] { '$', '{', 'n', '\0', '\r', '\n', ' ', '\'', '\\', 'x', 'c', '"', '`', '8', '0', '/', '*', '?', '>' };
            int[] word = new int[5];
            char[] text = new char[word.Length];

            var states = new Lexer.LexicalStates[] { Lexer.LexicalStates.ST_DOUBLE_QUOTES, Lexer.LexicalStates.ST_SINGLE_QUOTES,
                Lexer.LexicalStates.ST_BACKQUOTE, Lexer.LexicalStates.ST_HEREDOC, Lexer.LexicalStates.ST_NOWDOC };

            foreach (var state in states)
                while (Increment(word, chars.Length))
                {
                    ToArray(word, text, chars);
                    string line = new string(text);
                    lexer.Initialize(new StringReader(line), state, true, 0);
                    Tokens token = Tokens.EOF;
                    while ((token = lexer.GetNextToken()) != Tokens.EOF)
                    {
                        Assert.IsTrue(lexer.TokenSpan.IsValid, line);
                        Assert.IsTrue(lexer.TokenSpan.Length > 0, line + " - " + state.ToString() + " - " + lexer.TokenSpan.Start.ToString());
                    }
                }
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void LexerGetNextTokenByLineTest()
        {
            string path = (string)TestContext.DataRow["files"];

            TestErrorSink errorSink = new TestErrorSink();
            Lexer lexer = new Lexer(new StreamReader(path), Encoding.UTF8, errorSink,
                LanguageFeatures.ShortOpenTags, 0, Lexer.LexicalStates.INITIAL);

            Lexer.LexicalStates previousState = Lexer.LexicalStates.INITIAL;
            foreach (var line in File.ReadAllLines(path))
            {
                lexer.Initialize(new StringReader(line + Environment.NewLine), previousState, true, 0);

                while (lexer.GetNextToken() != Tokens.EOF)
                {
                    Assert.IsTrue(lexer.TokenSpan.IsValid);
                }
                previousState = lexer.CurrentLexicalState;
            }
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void LexerGetNextTokenTest()
        {
            string path = (string)TestContext.DataRow["files"];

            TestErrorSink errorSink = new TestErrorSink();
            Lexer lexer = new Lexer(new StreamReader(path), Encoding.UTF8, errorSink,
                LanguageFeatures.ShortOpenTags, 0, Lexer.LexicalStates.INITIAL);

            string parsed = ParseByPhp(path);
            parsed = parsed.Substring(0, parsed.LastIndexOf('-'));
            parsed = Regex.Replace(parsed.Replace("\r", " ").Replace("\n", " "), @"\s+", " ");
            int i = 0;
            string[][] expectedTokens = (
                from s in parsed.Split('-')
                let num = i++
                group s by num / 3 into g
                select g.ToArray()
                ).ToArray();

            //List<KeyValuePair<Tokens, SemanticValueType>> l = new List<KeyValuePair<Tokens, SemanticValueType>>();
            //Tokens t = Tokens.END;
            //while ((t = (Tokens)lexer.GetNextToken()) != Tokens.END)
            //{
            //    l.Add(new KeyValuePair<Tokens, SemanticValueType>(t, lexer.TokenValue));
            //}

            foreach (var expectedToken in expectedTokens)
            {
                Tokens token = (Tokens)lexer.GetNextToken();
                Assert.AreEqual(int.Parse(expectedToken[0]), (int)token, path);
                if (token == Tokens.T_VARIABLE || token == Tokens.T_STRING || token == Tokens.T_END_HEREDOC)
                {
                    Assert.AreEqual(expectedToken[2].TrimStart('$'), lexer.TokenValue.Object.ToString());
                }
                if (token == Tokens.T_DNUMBER)
                {
                    Assert.AreEqual(double.Parse(expectedToken[2], System.Globalization.NumberFormatInfo.InvariantInfo), lexer.TokenValue.Double);
                }
                if (token == Tokens.T_LNUMBER)
                {
                    Assert.AreEqual(int.Parse(expectedToken[2]), lexer.TokenValue.Integer);
                }
                //lexer.RestoreCompressedState(lexer.GetCompressedState());
            }
            while (Tokens.EOF != lexer.GetNextToken()) ;
            Assert.AreEqual(Tokens.EOF, lexer.GetNextToken(), path);
            Assert.AreEqual(Tokens.EOF, lexer.GetNextToken(), path);
            Assert.AreEqual(Tokens.EOF, lexer.GetNextToken(), path);
            Assert.AreEqual(0, errorSink.Errors.Count);
        }
    }
}
