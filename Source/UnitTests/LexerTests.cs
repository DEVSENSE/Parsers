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

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("TestData.csv")]
    [DeploymentItem(@"..\..\Tokens.php")]
    public class LexerTests
    {
        public TestContext TestContext { get; set; }

        Tokens _token = Tokens.END;

        void handler(Tokens token, char[] buffer, int tokenStart, int tokenLength)
        {
            _token = token;
        }

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
            SourceUnit sourceUnit = new CodeSourceUnit(File.ReadAllText(path), path, new System.Text.ASCIIEncoding(), Lexer.LexicalStates.INITIAL);
            ITokenProvider<SemanticValueType, Span> lexer = new Lexer(new StreamReader(path), sourceUnit, new TestErrorSink(), 
                LanguageFeatures.ShortOpenTags, 0, Lexer.LexicalStates.INITIAL);
            Assert.AreNotEqual(null, lexer);
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void LexerGetNextTokenTest()
        {
            string path = (string)TestContext.DataRow["files"];

            TestErrorSink errorSink = new TestErrorSink();
            SourceUnit sourceUnit = new CodeSourceUnit(File.ReadAllText(path), path, new System.Text.ASCIIEncoding(), Lexer.LexicalStates.INITIAL);
            Lexer lexer = new Lexer(new StreamReader(path), sourceUnit, errorSink, 
                LanguageFeatures.ShortOpenTags, 0, Lexer.LexicalStates.INITIAL);
            lexer.NextTokenEvent += handler;

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
                Assert.AreEqual(int.Parse(expectedToken[0]), (int)token);
                Assert.AreEqual(token, _token);
                if (token == Tokens.T_VARIABLE || token == Tokens.T_STRING || token == Tokens.T_END_HEREDOC)
                {
                    Assert.AreEqual(expectedToken[2].TrimStart('$'), lexer.TokenValue.Object.ToString());
                }
                if (token == Tokens.T_DNUMBER)
                {
                    Assert.AreEqual(double.Parse(expectedToken[2]), lexer.TokenValue.Double);
                }
                if (token == Tokens.T_LNUMBER)
                {
                    Assert.AreEqual(int.Parse(expectedToken[2]), lexer.TokenValue.Integer);
                }
                //lexer.RestoreCompressedState(lexer.GetCompressedState());
            }
            Assert.AreEqual(0, errorSink.Errors.Count);
        }
    }
}
