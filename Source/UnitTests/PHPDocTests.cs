﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Devsense.PHP.Text;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using UnitTests.TestImplementation;
using Devsense.PHP.Syntax;

namespace UnitTests
{
    [TestClass]
    public class PHPDocTests
    {
        static PHPDocBlock NewPHPDoc(string code)
        {
            code = code.Trim();
            return new PHPDocBlock(code, new Span(0, code.Length));
        }

        [TestMethod]
        public void SummaryTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * Summary.
 */");

            Assert.AreEqual(phpdoc.Summary, "Summary.");
        }

        [TestMethod]
        public void ReturnsTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * Summary.
 * @return int The return value.
 */");

            Assert.AreEqual(phpdoc.Returns.TypeNames, "int");
            Assert.AreEqual(phpdoc.Returns.Description, "The return value.");
        }
    }
}