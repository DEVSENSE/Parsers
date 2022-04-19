using System;
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
        public void DataProviderTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * @dataProvider provideTrimData
 */");

            Assert.IsNotNull(phpdoc.GetElement<PHPDocBlock.DataProviderTag>());
            Assert.AreEqual(phpdoc.GetElement<PHPDocBlock.DataProviderTag>().FunctionName.Name.Value, "provideTrimData");
        }

        [TestMethod]
        public void AnnotationTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * @Annotation
 */");

            Assert.IsNotNull(phpdoc.GetElement<PHPDocBlock.AnnotationTag>());
        }

        [TestMethod]
        public void EmptyDeprecatedTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * @deprecated
 */");

            Assert.IsNotNull(phpdoc.GetElement<PHPDocBlock.DeprecatedTag>());
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

        [TestMethod]
        public void MultitypeTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * @param $a (int|double) The value.
 * @param $b (int|double)[] The value.
 * @param $c int|double[] The value.
 * @param $d int|array<float|string|null> The value.
 * @param $e int|array<array-key, float|string|null> The value.
 * @param non-empty-string   $pattern
 * @param array<int|string, list<array{string|null, int}>> $matches Set by method
 * @param list<array{string, int<0, max>}> $matches Set by method
 */");

            var ps = phpdoc.Params;

            foreach (var p in ps)
            {
                Assert.IsFalse(string.IsNullOrEmpty(p.TypeNames), $"Type in '{p}' was not parsed.");
            }
        }


        [TestMethod]
        public void CallableSyntaxTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * @param $a (Closure(): \Generator<TKey, TValue, mixed, void>) The value.
 * @param $b (callable(): mixed) The value.
 * @param callable(): mixed $c Used in \LazyOption.php.
 */");

            var ps = phpdoc.Params;

            foreach (var p in ps)
            {
                Assert.IsFalse(string.IsNullOrEmpty(p.TypeNames), $"Type in '{p}' was not parsed.");
            }
        }

        [TestMethod]
        public void MethodVarArgTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * @method static int alphanum(string ...$x)
 * @method static int alphanum2(... $x)
 * @method $this alphanum3(string ...$x)
 */");

            Assert.AreEqual(3, phpdoc.Elements.Length);

            var method = phpdoc.GetElement<PHPDocBlock.MethodTag>();

            Assert.IsNotNull(method);
            Assert.IsTrue(method.Parameters[0].IsVariadic);
            Assert.AreEqual(method.Parameters[0].Name.Name.Value, "x");
        }
    }
}
