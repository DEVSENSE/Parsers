using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhpParser.Parser;
using System.IO;
using PHP.Core.Text;
using PHP.Syntax;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("TestData.csv")]
    [DeploymentItem(@"..\..\Tokens.php")]
    public class LexerTests
    {
        public static readonly string FilesPath = "TestData";

        public TestContext TestContext { get; set; }

        private string ParseByPhp(string path)
        {
            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = @"..\..\..\..\Tools\PHP v7.0\php.exe";
            process.StartInfo.Arguments = "-f tokens.php " + path;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();// Waits here for the process to exit.
            return process.StandardOutput.ReadToEnd();
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void LexerConstructorTest()
        {
            string path = (string)TestContext.DataRow["files"];
            SourceUnit sourceUnit = new CodeSourceUnit(File.ReadAllText(path), path, new System.Text.ASCIIEncoding(), Lexer.LexicalStates.INITIAL);
            PhpParser.Parser.ITokenProvider<SemanticValueType, Span> lexer = new Lexer(new StreamReader(path), sourceUnit);
            Assert.AreNotEqual(null, lexer);
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData.csv", "TestData#csv", DataAccessMethod.Sequential)]
        public void LexerGetNextTokenTest()
        {
            string path = (string)TestContext.DataRow["files"];
            SourceUnit sourceUnit = new CodeSourceUnit(File.ReadAllText(path), path, new System.Text.ASCIIEncoding(), Lexer.LexicalStates.INITIAL);
            PhpParser.Parser.ITokenProvider<SemanticValueType, Span> lexer = new Lexer(new StreamReader(path), sourceUnit);

            string parsed = ParseByPhp(path);
            string[] expectedTokens = parsed.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<Tokens> l = new List<Tokens>();
            Tokens t = Tokens.END;
            while ((t = (Tokens)lexer.GetNextToken()) != Tokens.END)
            {
                l.Add(t);
            }

            foreach (var expectedToken in expectedTokens)
            {
                string[] expected = expectedToken.Split('-');
                Tokens token = (Tokens)lexer.GetNextToken();
                Assert.AreEqual(expected[1], token.ToString());
            }
        }
    }
}
