using System;
using System.IO;
using Devsense.PHP.Text;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Devsense.PHP.Syntax;
using Devsense.PHP.Ast.DocBlock;
using Xunit;

namespace UnitTests
{
    public class PHPDocTests
    {
        static IDocBlock NewPHPDoc(string code)
        {
            code = code.Trim();
            return DefaultDocBlockFactory.Instance.CreateDocBlock(new Span(0, code.Length), code);
        }

        [Fact]
        public void SummaryTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * Summary.
 */");

            Assert.True(phpdoc.HasSummary(out var summary));
            Assert.Equal("Summary.", summary);
        }

        [Fact]
        public void EntriesTest()
        {
            var phpdoc = NewPHPDoc(@"
/**
 * Summary.
 * @entry1
 * @entry2 text
 *         text
 * @entry3 text
 *         text
 */");

            Assert.True(phpdoc.HasSummary(out var summary));
            Assert.Equal("Summary.", summary);

            int count = 0;
            foreach (var entry in phpdoc)
            {
                count ++;
            }

            Assert.Equal(3, count);
        }

        //        [Fact]
        //        public void DataProviderTest()
        //        {
        //            var phpdoc = NewPHPDoc(@"
        ///**
        // * @dataProvider provideTrimData
        // */");

        //            Assert.NotNull(phpdoc.GetElement<PHPDocBlock.DataProviderTag>());
        //            Assert.Equal(phpdoc.GetElement<PHPDocBlock.DataProviderTag>().FunctionName.Name.Value, "provideTrimData");
        //        }

        //        [Fact]
        //        public void AnnotationTest()
        //        {
        //            var phpdoc = NewPHPDoc(@"
        ///**
        // * @Annotation
        // */");

        //            Assert.NotNull(phpdoc.GetElement<PHPDocBlock.AnnotationTag>());
        //        }

        //        [Fact]
        //        public void EmptyDeprecatedTest()
        //        {
        //            var phpdoc = NewPHPDoc(@"
        ///**
        // * @deprecated
        // */");

        //            Assert.NotNull(phpdoc.GetElement<PHPDocBlock.DeprecatedTag>());
        //        }

        //        [Fact]
        //        public void ReturnsTest()
        //        {
        //            var phpdoc = NewPHPDoc(@"
        ///**
        // * Summary.
        // * @return int The return value.
        // */");

        //            Assert.Equal(phpdoc.Returns.TypeNames, "int");
        //            Assert.Equal(phpdoc.Returns.Description, "The return value.");
        //        }

        //        [Fact]
        //        public void MultitypeTest()
        //        {
        //            var phpdoc = NewPHPDoc(@"
        ///**
        // * @param $a (int|double) The value.
        // * @param $b (int|double)[] The value.
        // * @param $c int|double[] The value.
        // * @param $d int|array<float|string|null> The value.
        // * @param $e int|array<array-key, float|string|null> The value.
        // * @param non-empty-string   $pattern
        // * @param array<int|string, list<array{string|null, int}>> $matches Set by method
        // * @param list<array{string, int<0, max>}> $matches Set by method
        // */");

        //            var ps = phpdoc.Params;

        //            foreach (var p in ps)
        //            {
        //                Assert.IsFalse(string.IsNullOrEmpty(p.TypeNames), $"Type in '{p}' was not parsed.");
        //            }
        //        }


        //        [Fact]
        //        public void CallableSyntaxTest()
        //        {
        //            var phpdoc = NewPHPDoc(@"
        ///**
        // * @param $a (Closure(): \Generator<TKey, TValue, mixed, void>) The value.
        // * @param $b (callable(): mixed) The value.
        // * @param TFirstDefault|(\Closure(): TFirstDefault) $c.
        // * @param callable(): mixed $c Used in \LazyOption.php.
        // */");

        //            var ps = phpdoc.Params;

        //            foreach (var p in ps)
        //            {
        //                Assert.IsFalse(string.IsNullOrEmpty(p.TypeNames), $"Type in '{p}' was not parsed.");
        //            }
        //        }

        //        [Fact]
        //        public void MethodVarArgTest()
        //        {
        //            var phpdoc = NewPHPDoc(@"
        ///**
        // * @method static int alphanum(string ...$x)
        // * @method static int alphanum2(... $x)
        // * @method $this alphanum3(string ...$x)
        // */");

        //            Assert.Equal(3, phpdoc.Elements.Length);

        //            var method = phpdoc.GetElement<PHPDocBlock.MethodTag>();

        //            Assert.NotNull(method);
        //            Assert.True(method.Parameters[0].IsVariadic);
        //            Assert.Equal(method.Parameters[0].Name.Name.Value, "x");
        //        }
    }
}
