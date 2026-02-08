using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("ParserTestData.csv")]
    public class ParserTests
    {
        [TestMethod]
        public void ErrorRecoveryTest()
        {
            var codes = new[] {
@"<?php $x = 123",
@"<?php
if (true) {
",
@"<?php
$x->a = 'john'
$x->b = 'wick';
"
            };

            foreach (var code in codes)
            {
                var sourceUnit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Basic);
                var factory = new BasicNodesFactory(sourceUnit);
                var errors = new TestErrorSink();

                sourceUnit.Parse(factory, errors);

                Assert.IsNotNull(sourceUnit.Ast);
                Assert.IsTrue(errors.Count != 0);
                Assert.IsTrue(errors.Errors.Any(e => e.Error == FatalErrors.SyntaxError));
            }
        }

        [TestMethod]
        public void SimpleParseTest()
        {
            var codes = new[] {
@"<?php
class X {
    function static() { }
}",
@"<?php
echo $x->prop;",
@"<?php
class enum extends A {
}",
@"<?php
A::E->foo(); // dereferencable class const
",
@"<?php

class X {
    public function __construct(
        private readonly T $t, // private readonly
    ) {
    }
}",
@"<?php

do { } while (false);",
@"<?php
$fn = function () use ($a,): int {};",
@"<?php use X\enum;",
@"<?php use X\Enum as BaseEnum;",
@"<?php
function test(){
    global $test;
	$test = 1;
}",
@"<?php $r[] = 1;",
// PHP 8.4 new without parenthesis
@"<?php
//echo new A->prop;  // unexpected token ->
//echo new A->foo(); // unexpected token ->
//var_dump(new A::$prop); // unexpected token ::
//echo new A[0];  // unexpected token [
//echo new A::C; // unexpected identifier C
",
@"<?php
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
",
            };

            foreach (var code in codes)
            {
                var sourceUnit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Basic);
                var factory = new BasicNodesFactory(sourceUnit);
                var errors = new TestErrorSink();

                sourceUnit.Parse(factory, errors);

                Assert.IsNotNull(sourceUnit.Ast);
            }
        }

        public TestContext TestContext { get; set; }
        public const string Errors = "ERRORS:";
        public const string Pattern = @"\s*" + Errors + @"\s*(?<Number>\d*(, \d*)*)\s*(?<JSON>.*)";
        private Regex _errorRegex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.Singleline);

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\ParserTestData.csv", "ParserTestData#csv", DataAccessMethod.Sequential)]
        public void ParserParseTest()
        {
            string path = (string)TestContext.DataRow["files"];
            string testcontent = File.ReadAllText(path);

            string[] testparts = testcontent.Split(new string[] { "<<<TEST>>>" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(testparts.Length >= 2);

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
                Assert.AreEqual(1, matches.Count, path);
                Assert.AreEqual(knownErrors.Length, errors.Count, path);
                for (int i = 0; i < knownErrors.Length; i++)
                {
                    Assert.IsTrue(int.TryParse(knownErrors[i], out var errorid), path);
                    Assert.AreEqual(errorid, errors.Errors[i].Error.Id, path);
                    Assert.IsNotNull(errors.Errors[i].ToString());
                }
                testparts[1] = matches[0].Groups["JSON"].Value;
            }
            else
            {
                Assert.AreEqual(0, errors.Count, errors.Count != 0 ? $"{errors.Errors[0].ToString()} in {path}" : null);
            }

            Assert.IsNotNull(sourceUnit.Ast);

            // check all functions are in AST actually
            foreach (var m in factory.Methods)
            {
                Assert.IsNotNull(m.ContainingElement); // parent was resolved
                Assert.AreEqual(sourceUnit, m.ContainingSourceUnit);
            }

            foreach (var f in factory.Functions)
            {
                Assert.IsNotNull(f.ContainingElement); // parent was resolved
                Assert.AreEqual(sourceUnit, f.ContainingSourceUnit);
            }

            //

            var serializer = new JsonNodeWriter();
            TreeSerializer visitor = new TreeSerializer(serializer);
            sourceUnit.Ast.VisitMe(visitor);

            Regex rgx = new Regex(@"""Span""[^}]*},?\s*\n?"); // omit Span for more compact testing (position must be verified separately)
            string expected = rgx.Replace(testparts[1].Trim().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);
            string actual = rgx.Replace(serializer.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty), string.Empty);

            if (testparts[1].Trim() != "<<<IGNORE>>>")
            {
                // IMPORTANT - Uncomment to regenerate test data
                //File.WriteAllText(path, testparts[0] + "\n<<<TEST>>>\n" + rgx.Replace(serializer.ToString(), string.Empty));
                //Assert.AreEqual(expected, actual, path);
            }

            // check every node has a parent
            var parentChecker = new ContainingElementCheck();
            parentChecker.VisitGlobalCode(sourceUnit.Ast);

            // check nodes have correct span corresponding to correct source text
            var spanChecker = new NameSpanCheck(testparts[0]);
            spanChecker.VisitGlobalCode(sourceUnit.Ast);
        }

        [TestMethod]
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
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
        public void SingleByteStringTest()
        {
            var codes = new[] {
                @"<?php echo ""\x99\x7a\x7b"";",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
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
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
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
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
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

        [TestMethod]
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
                            Assert.IsNotNull(p.GetPropertyOfType<IDocBlock>());
                        }
                    }
                }
            }
        }

        [TestMethod]
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
                    Assert.IsNotNull(tdecl.PHPDoc);

                    foreach (var m in tdecl.Members.OfType<MethodDecl>())
                    {
                        Assert.IsNotNull(m.PHPDoc);
                    }
                }
            }
        }

        [TestMethod]
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
                        Assert.IsNotNull(m.GetPropertyOfType<IDocBlock>());
                    }
                }
            }
        }

        [TestMethod]
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
                    Assert.IsNotNull(func.ContainingElement); // random check // unnecessary for this test

                    foreach (var p in func.Signature.FormalParams)
                    {
                        if (p != null)
                        {
                            Assert.IsTrue(p.TryGetProperty<IDocBlock>(out var phpdoc));
                        }
                    }
                }
            }
        }

        [TestMethod]
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

        [TestMethod]
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

                Assert.AreEqual(0, errors.Count);
            }
        }
        
        [TestMethod]
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

                Assert.AreEqual(0, errors.Count);
            }
        }

        [TestMethod]
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

                Assert.AreEqual(0, errors.Count);
            }
        }

        [TestMethod]
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

                Assert.AreEqual(0, errors.Count);
            }
        }

        [TestMethod]
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

                Assert.IsNotNull(unit.Ast);
                Assert.IsInstanceOfType(unit.Ast.Statements[0], typeof(NamedTypeDecl));
            }
        }

        [TestMethod]
        public void AliasesTest()
        {
            var codes = new[] {
                @"<?php use A\{X,Y,};", // <-- trailing comma
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
        public void StringLiteralTest()
        {
            var codes = new[] {
                @"<?php echo ""\\test"";",
            };

            foreach (var code in codes)
            {
                var unit = new CodeSourceUnit(code, "dummy.php", Encoding.UTF8);
                unit.Parse(new BasicNodesFactory(unit), null);
                Assert.IsNotNull(unit.Ast);
            }
        }

        [TestMethod]
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

                Assert.IsNotNull(unit.Ast);
                Assert.AreEqual(1, errors.Count);
            }
        }

        [TestMethod]
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

                Assert.IsNotNull(unit.Ast);
                Assert.AreEqual(0, errors.Count);
            }
        }

        [TestMethod]
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

                Assert.IsNotNull(unit.Ast);
                Assert.AreEqual(0, errors.Count);

                var anonymousTypeDecl = unit.Ast.Statements
                    .OfType<ExpressionStmt>()
                    .Select(s => s.Expression)
                    .OfType<NewEx>()
                    .Select(n => n.ClassNameRef)
                    .OfType<AnonymousTypeRef>()
                    .Select(a => a.TypeDeclaration)
                    .Single()
                    ;

                Assert.AreNotEqual(anonymousTypeDecl.GetAttributes().Count, 0);
            }
        }

        [TestMethod]
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

                Assert.IsNotNull(unit.Ast);
                Assert.AreEqual(0, errors.Count);

                var rvals = unit.Ast.Statements
                    .OfType<ExpressionStmt>()
                    .Select(s => s.Expression)
                    .OfType<IAssignmentEx>()
                    .Select(n => n.RValue)
                    ;
            }
        }

        [TestMethod]
        public void BinaryOpTest()
        {
            var errors = new TestErrorSink();
            var unit = new CodeSourceUnit(@"<?php

$a = true and false;
$b = true && false;

", "dummy.php", Encoding.UTF8);
            unit.Parse(new BasicNodesFactory(unit), errors);

            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
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

            Assert.AreEqual(0, errors.Count);
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
                    Assert.IsNotNull(element.ContainingElement);
                    base.VisitElement(element);
                }
            }

            public override void VisitAnonymousTypeRef(AnonymousTypeRef x)
            {
                Assert.IsNotNull(x.ContainingElement);
                base.VisitAnonymousTypeRef(x);
            }

            public override void VisitClassTypeRef(ClassTypeRef x)
            {
                Assert.IsNotNull(x.ContainingElement);
                base.VisitClassTypeRef(x);
            }

            public override void VisitTranslatedTypeRef(TranslatedTypeRef x)
            {
                Assert.IsNotNull(x.ContainingElement);
                base.VisitTranslatedTypeRef(x);
            }

            public override void VisitPrimitiveTypeRef(PrimitiveTypeRef x)
            {
                Assert.IsNotNull(x.ContainingElement);
                base.VisitPrimitiveTypeRef(x);
            }

            public override void VisitReservedTypeRef(ReservedTypeRef x)
            {
                Assert.IsNotNull(x.ContainingElement);
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
                    Assert.IsTrue(element.Span.IsValid);
                    Assert.IsTrue(element is PHPDocBlock || inclusion.Last().Contains(element.Span));
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
                        Assert.IsNotNull(((NamespaceDecl)element).Body);
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
                Assert.AreEqual(original.TrimEnd(), element.Value.TrimEnd()); // TODO: FIX: HereDoc string literal span includes the closing newline
            }

            void CheckFunctionDecl(FunctionDecl func)
            {
                Assert.IsTrue(func.Span.Contains(func.Name.Span));
                Assert.IsTrue(func.Span.Contains(func.HeadingSpan));
                Assert.IsTrue(func.Span.Contains(func.ParametersSpan));
                Assert.IsTrue(func.Span.Contains(func.Body.Span));
                Assert.IsTrue(func.HeadingSpan.Contains(func.Name.Span));
                Assert.IsTrue(func.HeadingSpan.Contains(func.ParametersSpan));
                Assert.IsTrue(func.HeadingSpan.End <= func.Body.Span.Start);
                Assert.IsTrue(func.Name.Span.End <= func.ParametersSpan.Start);
                foreach (var param in func.Signature.FormalParams)
                    Assert.IsTrue(param.Span.Contains(param.Name.Span));
            }

            void CheckMethodDecl(MethodDecl method)
            {
                Assert.IsTrue(method.Span.Contains(method.Name.Span));
                Assert.IsTrue(method.Span.Contains(method.HeadingSpan));
                Assert.IsTrue(method.Span.Contains(method.ParametersSpan));
                Assert.IsTrue(method.HeadingSpan.Contains(method.Name.Span));
                Assert.IsTrue(method.HeadingSpan.Contains(method.ParametersSpan));
                if (method.Body != null)
                {
                    Assert.IsTrue(method.Span.Contains(method.Body.Span));
                    Assert.IsTrue(method.HeadingSpan.End <= method.Body.Span.Start);
                }
                Assert.IsTrue(method.Name.Span.End <= method.ParametersSpan.Start);
            }

            void CheckLambdaDecl(LambdaFunctionExpr lambda)
            {
                Assert.IsTrue(lambda.Span.Contains(lambda.HeadingSpan));
                Assert.IsTrue(lambda.Span.Contains(lambda.ParametersSpan));
                Assert.IsTrue(lambda.Span.Contains(lambda.Body.Span));
                Assert.IsTrue(lambda.HeadingSpan.Contains(lambda.ParametersSpan));
                Assert.IsTrue(lambda.HeadingSpan.End <= lambda.Body.Span.Start);
            }

            void CheckTypeDecl(TypeDecl type)
            {
                Assert.IsTrue(type.Name.HasValue);
                Assert.IsTrue(type is AnonymousTypeDecl || type.Span.Contains(type.Name.Span));
                Assert.IsTrue(type.Span.Contains(type.HeadingSpan));
                foreach (var member in type.Members)
                    Assert.IsTrue(type.Span.Contains(member.Span));
                Assert.IsTrue(type is AnonymousTypeDecl || type.HeadingSpan.Contains(type.Name.Span));
                foreach (var implements in type.ImplementsList)
                {
                    Assert.IsTrue(type.HeadingSpan.Contains(implements.Span));
                    Assert.IsTrue(type.Span.Contains(implements.Span));
                }
                if (type.Members.Count > 0)
                    Assert.IsTrue(type.HeadingSpan.End <= type.Members.Min(a => a.Span.Start));
                if (type.ImplementsList.Length > 0)
                    Assert.IsTrue(type.Name.Span.End <= type.ImplementsList.Min(a => a.Span.Start));
                if (type.BaseClass != null)
                {
                    Assert.IsTrue(type.Span.Contains(type.BaseClass.Span));
                    Assert.IsTrue(type.HeadingSpan.Contains(type.BaseClass.Span));
                    Assert.IsTrue(type.Name.Span.End <= type.BaseClass.Span.Start);
                }
            }
        }
    }
}
