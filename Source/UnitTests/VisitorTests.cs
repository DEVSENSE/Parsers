﻿using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.TestImplementation;

namespace UnitTests
{
    [TestClass]
    [DeploymentItem("ParserTestData.csv")]
    public class VisitorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\ParserTestData.csv", "ParserTestData#csv", DataAccessMethod.Sequential)]
        public void VisitorVisitTests()
        {
            string path = (string)TestContext.DataRow["files"];
            string testcontent = File.ReadAllText(path);

            string[] testparts = testcontent.Split(new string[] { "<<<TEST>>>" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(testparts.Length >= 2);

            var sourceUnit = new CodeSourceUnit(testparts[0], path, Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Basic);
            var factory = new AstCounterFactory(sourceUnit);
            var errors = new TestErrorSink();

            bool expectErrors = testparts[1].TrimStart().StartsWith(ParserTests.Errors);

            GlobalCode ast = null;

            Parser parser = new Parser();
            using (StringReader source_reader = new StringReader(testparts[0]))
            {
                sourceUnit.Parse(factory, errors);
                ast = sourceUnit.Ast;
            }
            if (expectErrors)
            {
                Assert.AreEqual(1, errors.Count, path);
            }
            else
            {
                Assert.AreEqual(0, errors.Count, path);

                // check every node has a parent
                var checker = new TreeVisitorCheck();
                checker.VisitElement(ast);
                Assert.AreEqual(factory.CreatedElements.Count, checker.VisitedElements.Count, path);
                Assert.AreEqual(factory.ItemCount, checker.ItemCount, path);
                Assert.AreEqual(factory.ForeachVarCount, checker.ForeachVarCount, path);
            }
        }

        class LangElementComparer : IEqualityComparer<LangElement>
        {
            public static readonly LangElementComparer Singleton = new LangElementComparer();

            public bool Equals(LangElement x, LangElement y) => x.Span.Equals(y.Span) && x.GetType().Equals(y.GetType());

            public int GetHashCode(LangElement obj) => obj.Span.GetHashCode() | obj.GetType().GetHashCode();
        }

        /// <summary>
        /// Helper visitor checking every node has a containing element.
        /// </summary>
        sealed class TreeVisitorCheck : TreeVisitor
        {

            int _itemCount = 0;
            public int ItemCount => _itemCount;

            int _foreachVarCount = 0;
            public int ForeachVarCount => _foreachVarCount;

            HashSet<LangElement> _visitedElements = new HashSet<LangElement>(LangElementComparer.Singleton);
            public HashSet<LangElement> VisitedElements => _visitedElements;

            public override void VisitElement(LangElement element)
            {
                // TODO - PHPDocBlock is not created by Lexer and CompliantLexer, without the factory
                if (element != null && !(element is PHPDocBlock))
                {
                    Assert.IsTrue(_visitedElements.Add(element));
                    base.VisitElement(element);
                }
            }

            public override void VisitArrayItem(Item item)
            {
                if (item != null)
                    _itemCount++;
                base.VisitArrayItem(item);
            }

            public override void VisitForeachVar(ForeachVar x)
            {
                if (x != null)
                    _foreachVarCount++;
                base.VisitForeachVar(x);
            }

            public override void VisitTranslatedTypeRef(TranslatedTypeRef x)
            {
                // TODO visit original type reference
                VisitElement(x.OriginalType);
                base.VisitTranslatedTypeRef(x);
            }

            public override void VisitTypeDecl(TypeDecl x)
            {
                // TODO - base class and interfaces are not visited, they are converted to qualified name
                VisitElement((TypeRef)x.BaseClass);
                foreach (var item in x.ImplementsList)
                    VisitElement((TypeRef)item);
                base.VisitTypeDecl(x);
            }
        }

        sealed class AstCounterFactory : BasicNodesFactory
        {
            int _itemCount = 0;
            public int ItemCount => _itemCount;

            int _foreachVarCount = 0;
            public int ForeachVarCount => _foreachVarCount;

            HashSet<LangElement> _createdElements = new HashSet<LangElement>(LangElementComparer.Singleton);

            public HashSet<LangElement> CreatedElements => _createdElements;

            public AstCounterFactory(SourceUnit sourceUnit) : base(sourceUnit)
            {
            }

            LangElement CountLE(LangElement element)
            {
                if (element == null) return element;
                Assert.IsTrue(_createdElements.Add(element));
                return element;
            }

            TypeRef CountTR(TypeRef element)
            {
                Assert.IsTrue(_createdElements.Add(element));
                return element;
            }

            Item CountI(Item i)
            {
                _itemCount++;
                return i;
            }

            ForeachVar CountFV(ForeachVar i)
            {
                _foreachVarCount++;
                return i;
            }

            public override ActualParam ActualParameter(Span span, LangElement expr, ActualParam.Flags flags)
                => (ActualParam)CountLE(base.ActualParameter(span, expr, flags)); // TODO - replace definitions by interfaces

            public override TypeRef AnonymousTypeReference(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, IEnumerable<FormalTypeParam> typeParamsOpt, INamedTypeRef baseClassOpt, IEnumerable<INamedTypeRef> implements, IEnumerable<LangElement> members, Span blockSpan)
            {
                // TODO - AnonymousTypeRef internaly creates AnonymousTypeDecl
                var reference = CountTR(base.AnonymousTypeReference(span, headingSpan, conditional, attributes, typeParamsOpt, baseClassOpt, implements, members, blockSpan));
                _createdElements.Add(((AnonymousTypeRef)reference).TypeDeclaration);
                return reference;
            }

            public override LangElement ArrayItem(Span span, LangElement expression, LangElement indexOpt)
                => CountLE(base.ArrayItem(span, expression, indexOpt));

            public override Item ArrayItemRef(Span span, LangElement indexOpt, LangElement variable)
                => CountI(base.ArrayItemRef(span, indexOpt, variable));

            public override Item ArrayItemValue(Span span, LangElement indexOpt, LangElement valueExpr)
                => CountI(base.ArrayItemValue(span, indexOpt, valueExpr));

            public override LangElement Assignment(Span span, LangElement target, LangElement value, Operations assignOp)
                => CountLE(base.Assignment(span, target, value, assignOp));

            public override LangElement BinaryOperation(Span span, Operations operation, LangElement leftExpression, LangElement rightExpression)
                 => CountLE(base.BinaryOperation(span, operation, leftExpression, rightExpression));

            public override LangElement Block(Span span, IEnumerable<LangElement> statements)
                 => CountLE(base.Block(span, statements));

            public override LangElement BlockComment(Span span, string content)
                 => CountLE(base.BlockComment(span, content));

            public override LangElement Call(Span span, LangElement nameExpr, CallSignature signature, TypeRef typeRef)
                 => CountLE(base.Call(span, nameExpr, signature, typeRef));

            public override LangElement Call(Span span, LangElement nameExpr, CallSignature signature, LangElement memberOfOpt)
                 => CountLE(base.Call(span, nameExpr, signature, memberOfOpt));

            public override LangElement Call(Span span, Name name, Span nameSpan, CallSignature signature, TypeRef typeRef)
                 => CountLE(base.Call(span, name, nameSpan, signature, typeRef));

            public override LangElement Call(Span span, TranslatedQualifiedName name, CallSignature signature, LangElement memberOfOpt)
                 => CountLE(base.Call(span, name, signature, memberOfOpt));

            public override LangElement Case(Span span, LangElement valueOpt, LangElement block)
            {
                // TODO - block is ignored and statements are passed to the 
                _createdElements.Remove(block);
                return CountLE(base.Case(span, valueOpt, block));
            }

            public override LangElement Catch(Span span, TypeRef typeOpt, DirectVarUse variable, LangElement block)
                 => CountLE(base.Catch(span, typeOpt, variable, block));

            public override LangElement ClassConstDecl(Span span, VariableName name, Span nameSpan, LangElement initializer)
                 => CountLE(base.ClassConstDecl(span, name, nameSpan, initializer));

            public override LangElement ClassConstUse(Span span, TypeRef tref, Name name, Span nameSpan)
                 => CountLE(base.ClassConstUse(span, tref, name, nameSpan));

            public override LangElement ColonBlock(Span span, IEnumerable<LangElement> statements, Tokens endToken)
                 => base.ColonBlock(span, statements, endToken); // TODO - ColonBlock calls Block

            public override LangElement Concat(Span span, IEnumerable<LangElement> expressions)
                 => CountLE(base.Concat(span, expressions));

            public override LangElement ConditionalEx(Span span, LangElement condExpr, LangElement trueExpr, LangElement falseExpr)
                 => CountLE(base.ConditionalEx(span, condExpr, trueExpr, falseExpr));

            public override LangElement ConstUse(Span span, TranslatedQualifiedName name)
                 => CountLE(base.ConstUse(span, name));

            public override LangElement Declare(Span span, IEnumerable<LangElement> decls, LangElement statementOpt)
            {
                // TODO - declarations are ignored
                if (decls != null)
                    foreach (var item in decls)
                    {
                        CreatedElements.Remove(item);
                        CreatedElements.Remove(((ConstantDecl)item).Initializer);
                    }
                return CountLE(base.Declare(span, decls, statementOpt));
            }

            public override LangElement DeclList(Span span, PhpMemberAttributes attributes, IEnumerable<LangElement> decls)
                 => CountLE(base.DeclList(span, attributes, decls));

            public override LangElement Do(Span span, LangElement body, LangElement cond)
                 => CountLE(base.Do(span, body, cond));

            public override LangElement Echo(Span span, IEnumerable<LangElement> parameters)
                 => CountLE(base.Echo(span, parameters));

            public override LangElement Empty(Span span, LangElement code)
                 => CountLE(base.Empty(span, code));

            public override LangElement EmptyStmt(Span span)
                 => CountLE(base.EmptyStmt(span));

            public override LangElement Eval(Span span, LangElement code)
                 => CountLE(base.Eval(span, code));

            public override LangElement Exit(Span span, LangElement statusOpt)
                 => CountLE(base.Exit(span, statusOpt));

            public override LangElement ExpressionStmt(Span span, LangElement expression)
                 => CountLE(base.ExpressionStmt(span, expression));

            public override LangElement FieldDecl(Span span, VariableName name, LangElement initializerOpt)
                 => CountLE(base.FieldDecl(span, name, initializerOpt));

            public override LangElement Finally(Span span, LangElement block)
                 => CountLE(base.Finally(span, block));

            public override LangElement For(Span span, IEnumerable<LangElement> init, IEnumerable<LangElement> cond, IEnumerable<LangElement> action, LangElement body)
                 => CountLE(base.For(span, init, cond, action, body));

            public override LangElement Foreach(Span span, LangElement enumeree, ForeachVar keyOpt, ForeachVar value, LangElement body)
                 => CountLE(base.Foreach(span, enumeree, keyOpt, value, body));

            public override ForeachVar ForeachVariable(Span span, LangElement variable, bool alias = false)
                 => CountFV(base.ForeachVariable(span, variable, alias));

            public override LangElement Function(Span span, bool conditional, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, LangElement body)
                => CountLE(base.Function(span, conditional, aliasReturn, attributes, returnType, name, nameSpan, typeParamsOpt, formalParams, formalParamsSpan, body));

            public override TypeRef GenericTypeReference(Span span, LangElement className, List<TypeRef> genericParams)
                 => CountTR(base.GenericTypeReference(span, className, genericParams));

            public override LangElement Global(Span span, List<LangElement> variables)
                 => CountLE(base.Global(span, variables));

            public override LangElement GlobalCode(Span span, IEnumerable<LangElement> statements, NamingContext context)
                 => CountLE(base.GlobalCode(span, statements, context));

            public override LangElement GlobalConstDecl(Span span, bool conditional, VariableName name, Span nameSpan, LangElement initializer)
                 => CountLE(base.GlobalConstDecl(span, conditional, name, nameSpan, initializer));

            public override LangElement Goto(Span span, string label, Span labelSpan)
                 => CountLE(base.Goto(span, label, labelSpan));

            public override LangElement HaltCompiler(Span span)
                 => CountLE(base.HaltCompiler(span));

            public override LangElement If(Span span, LangElement cond, LangElement body, LangElement elseOpt)
            {
                // TODO - else or elseif is passed as an IfStmt that is discarded and only its conditions used
                if (elseOpt != null)
                    _createdElements.Remove(elseOpt);
                return CountLE(base.If(span, cond, body, elseOpt));
            }

            public override LangElement Inclusion(Span span, bool conditional, InclusionTypes type, LangElement fileNameExpression)
                 => CountLE(base.Inclusion(span, conditional, type, fileNameExpression));

            public override LangElement IncrementDecrement(Span span, LangElement refexpression, bool inc, bool post)
                 => CountLE(base.IncrementDecrement(span, refexpression, inc, post));

            public override LangElement InlineHtml(Span span, string html)
            {
                // TODO - Inline html internali creates a string literal
                var htm = CountLE(base.InlineHtml(span, html));
                _createdElements.Add(((EchoStmt)htm).Parameters[0]);
                return htm;
            }

            public override LangElement InstanceOf(Span span, LangElement expression, TypeRef typeRef)
                 => CountLE(base.InstanceOf(span, expression, typeRef));

            public override LangElement Isset(Span span, IEnumerable<LangElement> variables)
                 => CountLE(base.Isset(span, variables));

            public override LangElement Jump(Span span, JumpStmt.Types type, LangElement exprOpt)
                 => CountLE(base.Jump(span, type, exprOpt));

            public override LangElement Label(Span span, string label, Span labelSpan)
                 => CountLE(base.Label(span, label, labelSpan));

            public override LangElement Lambda(Span span, Span headingSpan, bool aliasReturn, TypeRef returnType, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, IEnumerable<FormalParam> lexicalVars, LangElement body)
                 => CountLE(base.Lambda(span, headingSpan, aliasReturn, returnType, formalParams, formalParamsSpan, lexicalVars, body));

            public override LangElement LineComment(Span span, string content)
                 => CountLE(base.LineComment(span, content));

            public override LangElement List(Span span, IEnumerable<Item> targets)
                 => CountLE(base.List(span, targets));

            public override LangElement Literal(Span span, object value)
                 => CountLE(base.Literal(span, value));

            public override LangElement Method(Span span, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Span returnTypeSpan, string name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, IEnumerable<ActualParam> baseCtorParams, LangElement body)
                 => CountLE(base.Method(span, aliasReturn, attributes, returnType, returnTypeSpan, name, nameSpan, typeParamsOpt, formalParams, formalParamsSpan, baseCtorParams, body));

            public override LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, NamingContext context)
                 => CountLE(base.Namespace(span, name, nameSpan, context));

            public override LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, LangElement block, NamingContext context)
                 => CountLE(base.Namespace(span, name, nameSpan, block, context));

            public override LangElement New(Span span, TypeRef classNameRef, IEnumerable<ActualParam> argsOpt)
                 => CountLE(base.New(span, classNameRef, argsOpt));

            public override LangElement NewArray(Span span, IEnumerable<Item> itemsOpt)
                 => CountLE(base.NewArray(span, itemsOpt));

            public override TypeRef NullableTypeReference(Span span, LangElement className)
                 => CountTR(base.NullableTypeReference(span, className));

            public override FormalParam Parameter(Span span, string name, Span nameSpan, TypeRef typeOpt, FormalParam.Flags flags, Expression initValue)
                 => (FormalParam)CountLE(base.Parameter(span, name, nameSpan, typeOpt, flags, initValue)); // TODO - replace definitions by interfaces

            public override LangElement ParenthesisExpression(Span span, LangElement expression)
                 => base.ParenthesisExpression(span, expression); // TODO - expression is returned, no new expression is created

            public override LangElement PHPDoc(Span span, LangElement content)
                 => CountLE(base.PHPDoc(span, content));

            public override LangElement PseudoConstUse(Span span, PseudoConstUse.Types type)
                 => CountLE(base.PseudoConstUse(span, type));

            public override LangElement Shell(Span span, LangElement command)
                 => CountLE(base.Shell(span, command));

            public override LangElement SimpleBlock(Span span, IEnumerable<LangElement> statements)
                 => base.SimpleBlock(span, statements); // TODO - SimpleBlock calls Block

            public override LangElement Static(Span span, IEnumerable<LangElement> staticVariables)
                 => CountLE(base.Static(span, staticVariables));

            public override LangElement StaticVarDecl(Span span, VariableName name, LangElement initializerOpt)
                 => CountLE(base.StaticVarDecl(span, name, initializerOpt));

            public override LangElement Switch(Span span, LangElement value, List<LangElement> block)
                 => CountLE(base.Switch(span, value, block));

            public override LangElement Throw(Span span, LangElement expression)
                 => CountLE(base.Throw(span, expression));

            public override LangElement TraitAdaptationAlias(Span span, Tuple<QualifiedNameRef, NameRef> name, NameRef identifierOpt, PhpMemberAttributes? attributeOpt)
                 => CountLE(base.TraitAdaptationAlias(span, name, identifierOpt, attributeOpt));

            public override LangElement TraitAdaptationBlock(Span span, IEnumerable<LangElement> adaptations)
                 => CountLE(base.TraitAdaptationBlock(span, adaptations));

            public override LangElement TraitAdaptationPrecedence(Span span, Tuple<QualifiedNameRef, NameRef> name, IEnumerable<QualifiedNameRef> precedences)
                 => CountLE(base.TraitAdaptationPrecedence(span, name, precedences));

            public override LangElement TraitUse(Span span, IEnumerable<QualifiedNameRef> traits, LangElement adaptationsBlock)
                 => CountLE(base.TraitUse(span, traits, adaptationsBlock));

            public override LangElement TryCatch(Span span, LangElement body, IEnumerable<LangElement> catches, LangElement finallyBlockOpt)
                 => CountLE(base.TryCatch(span, body, catches, finallyBlockOpt));

            public override LangElement Type(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, INamedTypeRef baseClassOpt, IEnumerable<INamedTypeRef> implements, IEnumerable<LangElement> members, Span bodySpan)
            {
                var imp = implements != null ? implements.ToList() : null;
                return CountLE(base.Type(span, headingSpan, conditional, attributes, name, nameSpan, typeParamsOpt, baseClassOpt, imp, members, bodySpan));
            }

            public override TypeRef TypeReference(Span span, IEnumerable<LangElement> classes)
            {
                // TODO - returns only the first type if only one is present
                var reference = base.TypeReference(span, classes);
                if (reference is MultipleTypeRef)
                    return CountTR(reference);
                else return reference;
            }

            public override TypeRef PrimitiveTypeReference(Span span, PrimitiveTypeRef.PrimitiveType tname)
                => CountTR(base.PrimitiveTypeReference(span, tname));

            public override TypeRef AliasedTypeReference(Span span, QualifiedName className, TypeRef origianType)
                => CountTR(base.AliasedTypeReference(span, className, origianType));

            public override TypeRef ReservedTypeReference(Span span, ReservedTypeRef.ReservedType typeName)
                => CountTR(base.ReservedTypeReference(span, typeName));

            public override TypeRef TypeReference(Span span, LangElement varName)
                => CountTR(base.TypeReference(span, varName));

            public override TypeRef TypeReference(Span span, QualifiedName className)
                 => CountTR(base.TypeReference(span, className));

            public override LangElement UnaryOperation(Span span, Operations operation, LangElement expression)
                 => CountLE(base.UnaryOperation(span, operation, expression));

            public override LangElement Unset(Span span, IEnumerable<LangElement> variables)
                 => CountLE(base.Unset(span, variables));

            public override LangElement Variable(Span span, string name, TypeRef typeRef)
            {
                // TODO - variable used for the property is discarded
                Assert.AreEqual(1, _createdElements.RemoveWhere(e => !typeRef.Span.Contains(e.Span) && span.Contains(e.Span)));
                return CountLE(base.Variable(span, name, typeRef));
            }

            public override LangElement Variable(Span span, LangElement nameExpr, LangElement memberOfOpt)
                 => CountLE(base.Variable(span, nameExpr, memberOfOpt));

            public override LangElement Variable(Span span, LangElement nameExpr, TypeRef typeRef)
            {
                // TODO - variable used for the property is discarded
                Assert.AreEqual(1, _createdElements.RemoveWhere(e => e is IndirectVarUse && e.Span.Contains(nameExpr.Span)));
                return CountLE(base.Variable(span, nameExpr, typeRef));
            }

            public override LangElement Variable(Span span, string name, LangElement memberOfOpt)
                 => CountLE(base.Variable(span, name, memberOfOpt));

            public override LangElement While(Span span, LangElement cond, LangElement body)
                 => CountLE(base.While(span, cond, body));

            public override LangElement Yield(Span span, LangElement keyOpt, LangElement valueOpt)
                 => CountLE(base.Yield(span, keyOpt, valueOpt));

            public override LangElement YieldFrom(Span span, LangElement fromExpr)
                 => CountLE(base.YieldFrom(span, fromExpr));

            public override LangElement Use(Span span, IEnumerable<UseBase> uses, AliasKind kind)
                 => CountLE(base.Use(span, uses, kind));
        }
    }
}
