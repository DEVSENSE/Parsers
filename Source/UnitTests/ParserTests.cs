using System;
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
using Devsense.PHP.Ast.DocBlock;
using System.Diagnostics;
using Xunit;

namespace UnitTests
{
    public class ParserTests
    {
        [Theory]
        [InlineData(@"<?php $x = 123", @"<?php $x = 123;")]
        [InlineData(@"<?php
if (true) {
",
            @"<?php
if (true) {
}")]
        [InlineData(@"<?php
$x->a = 'john'
$x->b = 'wick';
",
            @"<?php
$x->a = 'john';
$x->b = 'wick';
")]
        [InlineData(@"<?php
function f() {
    $x = 1 + ;
    if ($x > 0) {
        echo 'ok';
    }
    return $x;
}", null, 1)]
        [InlineData(@"<?php
function foo() {
    $a = 1
    if ($a > 0) {
        echo $a;
    }
    return $a;
}",
            @"<?php
function foo() {
    $a = 1;
    if ($a > 0) {
        echo $a;
    }
    return $a;
}")]
        [InlineData(@"<?php
class Test {
    public function run() {
        echo 'start'
        while ($this->cond) {
            work();
        }
    }
}")]
        [InlineData(@"<?php

if ($a) {
    echo ""a"";
    echo ""b"";

echo ""outside"";
")]
        [InlineData(@"<?php
function test() {
    if (true) {
        echo ""A"";
    else {
        echo ""B"";
    }
    echo ""C"";
}")]
        [InlineData(@"<?php
echo $a->;
echo $a->b;")]
        [InlineData(@"<?php
echo $a->
echo $a->b;")]
        //[InlineData(@"<?php
        //function test() {
        //    foo(
        //        bar(1, 2),
        //        baz(3, 4)
        //    // missing closing ')'
        //    if ($x) {
        //        doSomething();
        //    }
        //    echo ""done"";
        //}")]
        public void ErrorRecoveryTest(string code, string expectedcode = null, int tokensDiscarded = 0)
        {
            var sourceUnit = new CodeSourceUnit(code, "dummy.php");
            var errors = new TestErrorSink();

            sourceUnit.Parse(new BasicNodesFactory(sourceUnit), errors);

            Assert.NotNull(sourceUnit.Ast);
            Assert.True(errors.Count != 0);
            Assert.Contains(errors.Errors, e => e.Error == FatalErrors.SyntaxError);
            Assert.Equal(tokensDiscarded, sourceUnit.TokensDiscarded);

            // compare with healthy AST
            if (expectedcode != null)
            {
                var expected = SerializeNode(CodeSourceUnit.ParseCode(expectedcode, "dummy.php").Ast, false);
                var actual = SerializeNode(sourceUnit.Ast, false);

                Assert.Equal(expected, actual);
            }
        }

        [Theory]
        [InlineData(@"<?php
class X {
    function static() { }
}")]
        [InlineData(@"<?php
echo $x->prop;")]
        [InlineData(@"<?php
class enum extends A {
}")]
        [InlineData(@"<?php
A::E->foo(); // dereferencable class const
")]
        [InlineData(@"<?php

class X {
    public function __construct(
        private readonly T $t, // private readonly
    ) {
    }
}")]
        [InlineData(@"<?php

do { } while (false);")]
        [InlineData(@"<?php
$fn = function () use ($a,): int {};")]
        [InlineData(@"<?php use X\enum;")]
        [InlineData(@"<?php use X\Enum as BaseEnum;")]
        [InlineData(@"<?php
function test(){
    global $test;
	$test = 1;
}")]
        [InlineData(@"<?php $r[] = 1;")]
        // PHP 8.4 new without parenthesis
        [InlineData(@"<?php
//echo new A->prop;  // unexpected token ->
//echo new A->foo(); // unexpected token ->
//var_dump(new A::$prop); // unexpected token ::
//echo new A[0];  // unexpected token [
//echo new A::C; // unexpected identifier C
")]
        [InlineData(@"<?php
echo new A()::C;
echo new A()::{'C'};
echo new $class()::C;
echo new $class()::{'C'};
echo new (trim(' A '))()::C;
echo new (trim(' A '))()::{'C'};

echo new A()->property;
echo new $class()->property;
echo new (trim(' A '))()->property;

echo new A()::$staticProperty;
echo new $class()::$staticProperty;
echo new (trim(' A '))()::$staticProperty;

new A()();
new $class()();
new (trim(' A '))()();

new A()->method();
new $class()->method();
new (trim(' A '))()->method();

new A()::staticMethod();
new $class()::staticMethod();
new (trim(' A '))()::staticMethod();

new A()['key'];
new $class()['key'];
new (trim(' A '))()['key'];

isset(new A()['key']);
isset(new $class()['key']);
isset(new (trim(' A '))()['key']);
")]
        public void SimpleParseTest(string code)
        {
            var sourceUnit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Basic);
            var factory = new BasicNodesFactory(sourceUnit);
            var errors = new TestErrorSink();

            sourceUnit.Parse(factory, errors);

            Assert.NotNull(sourceUnit.Ast);
        }

        public const string Errors = "ERRORS:";
        public const string Pattern = @"\s*" + Errors + @"\s*(?<Number>\d*(, \d*)*)\s*(?<JSON>.*)";
        private Regex _errorRegex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.Singleline);

        public static IEnumerable<object[]> TestData_Parser => Directory
            .EnumerateFiles("TestData\\parser", "*.php")
            .Select(path => new object[] { path })
            ;

        [Theory]
        [MemberData(nameof(TestData_Parser))]
        public void ParserParseTest(string path)
        {
            string testcontent = File.ReadAllText(path);

            string[] testparts = testcontent.Split(new string[] { "<<<TEST>>>" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.True(testparts.Length >= 2);

            var sourceUnit = new CodeSourceUnit(testparts[0], path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Basic);
            var errors = new TestErrorSink();
            var factory = new TestNodeFactory(sourceUnit, errors);

            //
            sourceUnit.Parse(factory, errors);

            //
            if (testparts[1].TrimStart().StartsWith(Errors))
            {
                var matches = _errorRegex.Matches(testparts[1]);
                var knownErrors = matches[0].Groups["Number"].Value.Split(',');
                Assert.Single(matches);
                Assert.Equal(knownErrors.Length, errors.Count);
                for (int i = 0; i < knownErrors.Length; i++)
                {
                    Assert.True(int.TryParse(knownErrors[i], out var errorid), path);
                    Assert.Equal(errorid, errors.Errors[i].Error.Id);
                    Assert.NotNull(errors.Errors[i].ToString());
                }
                testparts[1] = matches[0].Groups["JSON"].Value;
            }
            else
            {
                Assert.Empty(errors.Errors);
            }

            Assert.NotNull(sourceUnit.Ast);

            // check all functions are in AST actually
            foreach (var m in factory.Methods)
            {
                Assert.NotNull(m.ContainingElement); // parent was resolved
                Assert.Equal(sourceUnit, m.ContainingSourceUnit);
            }

            foreach (var f in factory.Functions)
            {
                Assert.NotNull(f.ContainingElement); // parent was resolved
                Assert.Equal(sourceUnit, f.ContainingSourceUnit);
            }

            //

            var serialized = SerializeNode(sourceUnit.Ast, true);

            Regex rgx = new Regex(@"""Span""[^}]*},?\s*\n?"); // omit Span for more compact testing (position must be verified separately)
            string expected = rgx.Replace(testparts[1].Trim().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);
            string actual = rgx.Replace(serialized.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);

            if (testparts[1].Trim() != "<<<IGNORE>>>")
            {
                // IMPORTANT - Uncomment to regenerate test data
                //File.WriteAllText(path, testparts[0] + "\n<<<TEST>>>\n" + rgx.Replace(serializer.ToString(), string.Empty));
                //Assert.Equal(expected, actual, path);
            }

            // check every node has a parent
            var parentChecker = new ContainingElementCheck();
            parentChecker.VisitGlobalCode(sourceUnit.Ast);

            // check nodes have correct span corresponding to correct source text
            var spanChecker = new NameSpanCheck(testparts[0]);
            spanChecker.VisitGlobalCode(sourceUnit.Ast);
        }

        [Fact]
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
                Assert.NotNull(unit.Ast);
            }
        }

        [Fact]
        public void SingleByteStringTest()
        {
            var codes = new[] {
                @"<?php echo ""\x99\x7a\x7b"";",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.NotNull(unit.Ast);
            }
        }

        [Fact]
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
                Assert.NotNull(unit.Ast);
            }
        }

        [Fact]
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
                Assert.NotNull(unit.Ast);
            }
        }

        [Fact]
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

        [Fact]
        public void FormalParamDocBlockTest()
        {
            var codes = new[] {
                @"<?php
class X {
    function __construct(
        /** @var int */
        public $p1
        ) { }
}",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);

                foreach (var tdecl in unit.Ast.TraverseNamedTypeDeclarations())
                {
                    foreach (var m in tdecl.Members.OfType<MethodDecl>())
                    {
                        foreach (var p in m.Signature.FormalParams)
                        {
                            Assert.NotNull(p.GetPropertyOfType<IDocBlock>());
                        }
                    }
                }
            }
        }

        [Fact]
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
                    Assert.NotNull(tdecl.PHPDoc);

                    foreach (var m in tdecl.Members.OfType<MethodDecl>())
                    {
                        Assert.NotNull(m.PHPDoc);
                    }
                }
            }
        }

        [Fact]
        public void PhpDocTraitUseTest()
        {
            var codes = new[] {
                @"<?php
class X {
  /** @use T<int> */
  use T;
}",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);

                foreach (var tdecl in unit.Ast.TraverseNamedTypeDeclarations())
                {
                    foreach (var m in tdecl.Members.OfType<TraitsUse>())
                    {
                        Assert.NotNull(m.GetPropertyOfType<IDocBlock>());
                    }
                }
            }
        }

        [Fact]
        public void PhpDocParameterTest()
        {
            var codes = new[] {
                @"<?php
function foo(
    /** summary */$p,
    int /** summary */ $q,
    /** summary */ [AnAttribute] $r
) {}
",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                var factory = new TestNodeFactory(unit, errors: null);
                unit.Parse(factory, null);

                foreach (var func in factory.Functions)
                {
                    Assert.NotNull(func.ContainingElement); // random check // unnecessary for this test

                    foreach (var p in func.Signature.FormalParams)
                    {
                        if (p != null)
                        {
                            Assert.True(p.TryGetProperty<IDocBlock>(out var phpdoc));
                        }
                    }
                }
            }
        }

        [Fact]
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

        [Fact]
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

                Assert.Equal(0, errors.Count);
            }
        }

        [Fact]
        public void AsymmetricVisibilityTest()
        {
            var codes = new[] {
                // https://github.com/DEVSENSE/phptools-docs/issues/728
                @"<?php
class Person {
    public private(set) ?int $age = null;
}
",
            };

            foreach (var code in codes)
            {
                var errors = new TestErrorSink();
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, features: LanguageFeatures.Php84Set);
                unit.Parse(new BasicNodesFactory(unit), errors);

                Assert.Equal(0, errors.Count);
            }
        }

        [Fact]
        public void TypedClassConstTest()
        {
            var codes = new[] {
                @"<?php
class X {
  const int A = 1, B = 2;
}
",
            };

            foreach (var code in codes)
            {
                var errors = new TestErrorSink();
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, features: LanguageFeatures.Basic);
                unit.Parse(new BasicNodesFactory(unit), errors);

                Assert.Equal(0, errors.Count);
            }
        }

        [Fact]
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

                Assert.Equal(0, errors.Count);
            }
        }

        [Fact]
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

                Assert.NotNull(unit.Ast);
                Assert.IsAssignableFrom<NamedTypeDecl>(unit.Ast.Statements[0]);
            }
        }

        [Fact]
        public void AliasesTest()
        {
            var codes = new[] {
                @"<?php use A\{X,Y,};", // <-- trailing comma
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.NotNull(unit.Ast);
            }
        }

        [Fact]
        public void StringLiteralTest()
        {
            var codes = new[] {
                @"<?php echo ""\\test"";",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.NotNull(unit.Ast);
            }
        }

        [Fact]
        public void HeredocIndentTest()
        {
            var codes = new[] {
               @"<?php
$x = ""hello"";

    echo <<<FOO
  $x   // error: wrong indentation
    FOO;",
                              @"<?php
$x = ""hello"";

    echo <<<FOO
$x   // error: wrong indentation
    FOO;",
            };

            foreach (var code in codes)
            {
                var errors = new TestErrorSink();
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), errors);

                Assert.NotNull(unit.Ast);
                Assert.Equal(1, errors.Count);
            }
        }

        [Fact]
        public void PropertyHooksTest()
        {
            var codes = new[] {
                @"<?php
class X {
    public int $runs = 0 {
        set {
            $this->runs = $value;
        }
    }
}",
                @"<?php
class X {
    public string $fullName {
        get => $this->first . ' ' . $this->last;
        set {
        }
    }
}",
                @"<?php
class X {
    function __construct(
        public string $fullName {
            get => $this->first . ' ' . $this->last;
            set {
            }
        }
    )
    {
    }
}",
            };

            foreach (var code in codes)
            {
                var errors = new TestErrorSink();
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), errors);

                Assert.NotNull(unit.Ast);
                Assert.Equal(0, errors.Count);
            }
        }

        [Fact]
        public void AnonymousClassTest()
        {
            var codes = new[]
            {
                @"<?php
new #[Attribuute] class {};"
            };

            foreach (var code in codes)
            {
                var errors = new TestErrorSink();
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), errors);

                Assert.NotNull(unit.Ast);
                Assert.Equal(0, errors.Count);

                var anonymousTypeDecl = unit.Ast.Statements
                    .OfType<ExpressionStmt>()
                    .Select(s => s.Expression)
                    .OfType<NewEx>()
                    .Select(n => n.ClassNameRef)
                    .OfType<AnonymousTypeRef>()
                    .Select(a => a.TypeDeclaration)
                    .Single()
                    ;

                Assert.NotEmpty(anonymousTypeDecl.GetAttributes());
            }
        }

        [Fact]
        public void CloneTest()
        {
            var codes = new[]
            {
                @"<?php
$o = clone $x;
$o = clone($x);
$o = clone($x,);
$o = clone($x,[]);
$o = clone(...);

$o = \clone($x);
$o = \clone($x,);
$o = \clone($x,[]);
$o = \clone(...);
"
            };

            foreach (var code in codes)
            {
                var errors = new TestErrorSink();
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), errors);

                Assert.NotNull(unit.Ast);
                Assert.Equal(0, errors.Count);

                var rvals = unit.Ast.Statements
                    .OfType<ExpressionStmt>()
                    .Select(s => s.Expression)
                    .OfType<IAssignmentEx>()
                    .Select(n => n.RValue)
                    ;
            }
        }

        [Fact]
        public void BinaryOpTest()
        {
            var errors = new TestErrorSink();
            var unit = new CodeSourceUnit(@"<?php

$a = true and false;
$b = true && false;

", "dummy.php", Encoding.UTF8);
            unit.Parse(new BasicNodesFactory(unit), errors);

            Assert.Equal(0, errors.Count);
        }

        [Fact]
        public void PublicPrivateNamespaceName()
        {
            var errors = new TestErrorSink();
            var unit = new CodeSourceUnit(@"<?php
echo Private\Foo::class;
echo strlen(Private\Foo::class);
echo strlen(namespace\Private\Foo::class);
echo strlen(\Private\Foo::class);
", "dummy.php", Encoding.UTF8);
            unit.Parse(new BasicNodesFactory(unit), errors);

            Assert.Equal(0, errors.Count);
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
                    Assert.NotNull(element.ContainingElement);
                    base.VisitElement(element);
                }
            }

            public override void VisitAnonymousTypeRef(AnonymousTypeRef x)
            {
                Assert.NotNull(x.ContainingElement);
                base.VisitAnonymousTypeRef(x);
            }

            public override void VisitClassTypeRef(ClassTypeRef x)
            {
                Assert.NotNull(x.ContainingElement);
                base.VisitClassTypeRef(x);
            }

            public override void VisitTranslatedTypeRef(TranslatedTypeRef x)
            {
                Assert.NotNull(x.ContainingElement);
                base.VisitTranslatedTypeRef(x);
            }

            public override void VisitPrimitiveTypeRef(PrimitiveTypeRef x)
            {
                Assert.NotNull(x.ContainingElement);
                base.VisitPrimitiveTypeRef(x);
            }

            public override void VisitReservedTypeRef(ReservedTypeRef x)
            {
                Assert.NotNull(x.ContainingElement);
                base.VisitReservedTypeRef(x);
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
                    Assert.True(element.Span.IsValid);
                    Assert.True(element is PHPDocBlock || inclusion.Last().Contains(element.Span));
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
                        Assert.NotNull(((NamespaceDecl)element).Body);
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
                Assert.Equal(original.TrimEnd(), element.Value.TrimEnd()); // TODO: FIX: HereDoc string literal span includes the closing newline
            }

            void CheckFunctionDecl(FunctionDecl func)
            {
                Assert.True(func.Span.Contains(func.Name.Span));
                Assert.True(func.Span.Contains(func.HeadingSpan));
                Assert.True(func.Span.Contains(func.ParametersSpan));
                Assert.True(func.Span.Contains(func.Body.Span));
                Assert.True(func.HeadingSpan.Contains(func.Name.Span));
                Assert.True(func.HeadingSpan.Contains(func.ParametersSpan));
                Assert.True(func.HeadingSpan.End <= func.Body.Span.Start);
                Assert.True(func.Name.Span.End <= func.ParametersSpan.Start);
                foreach (var param in func.Signature.FormalParams)
                    Assert.True(param.Span.Contains(param.Name.Span));
            }

            void CheckMethodDecl(MethodDecl method)
            {
                Assert.True(method.Span.Contains(method.Name.Span));
                Assert.True(method.Span.Contains(method.HeadingSpan));
                Assert.True(method.Span.Contains(method.ParametersSpan));
                Assert.True(method.HeadingSpan.Contains(method.Name.Span));
                Assert.True(method.HeadingSpan.Contains(method.ParametersSpan));
                if (method.Body != null)
                {
                    Assert.True(method.Span.Contains(method.Body.Span));
                    Assert.True(method.HeadingSpan.End <= method.Body.Span.Start);
                }
                Assert.True(method.Name.Span.End <= method.ParametersSpan.Start);
            }

            void CheckLambdaDecl(LambdaFunctionExpr lambda)
            {
                Assert.True(lambda.Span.Contains(lambda.HeadingSpan));
                Assert.True(lambda.Span.Contains(lambda.ParametersSpan));
                Assert.True(lambda.Span.Contains(lambda.Body.Span));
                Assert.True(lambda.HeadingSpan.Contains(lambda.ParametersSpan));
                Assert.True(lambda.HeadingSpan.End <= lambda.Body.Span.Start);
            }

            void CheckTypeDecl(TypeDecl type)
            {
                Assert.True(type.Name.HasValue);
                Assert.True(type is AnonymousTypeDecl || type.Span.Contains(type.Name.Span));
                Assert.True(type.Span.Contains(type.HeadingSpan));
                foreach (var member in type.Members)
                    Assert.True(type.Span.Contains(member.Span));
                Assert.True(type is AnonymousTypeDecl || type.HeadingSpan.Contains(type.Name.Span));
                foreach (var implements in type.ImplementsList)
                {
                    Assert.True(type.HeadingSpan.Contains(implements.Span));
                    Assert.True(type.Span.Contains(implements.Span));
                }
                if (type.Members.Count > 0)
                    Assert.True(type.HeadingSpan.End <= type.Members.Min(a => a.Span.Start));
                if (type.ImplementsList.Length > 0)
                    Assert.True(type.Name.Span.End <= type.ImplementsList.Min(a => a.Span.Start));
                if (type.BaseClass != null)
                {
                    Assert.True(type.Span.Contains(type.BaseClass.Span));
                    Assert.True(type.HeadingSpan.Contains(type.BaseClass.Span));
                    Assert.True(type.Name.Span.End <= type.BaseClass.Span.Start);
                }
            }
        }

        static string SerializeNode(GlobalCode ast, bool _serialize_span)
        {
            var serializer = new JsonNodeWriter();
            TreeSerializer visitor = new TreeSerializer(serializer, serialize_span: _serialize_span);
            visitor.VisitGlobalCode(ast);

            return serializer.ToString();
        }
    }
}
