using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using PHP.Syntax;

namespace PHP.Core.AST
{
    #region LambdaFunctionDecl

    /// <summary>
    /// Represents a function declaration.
    /// </summary>
    public sealed class LambdaFunctionExpr : Expression, IHasSourceUnit
    {
        /// <summary>
        /// Gets namespace containing this lambda expression. Can be <c>null</c>.
        /// </summary>
        public NamespaceDecl Namespace { get { return ns; } }
        private readonly NamespaceDecl ns;

        public override Operations Operation
        {
            get { return Operations.Closure; }
        }

        /// <summary>
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }

        public Signature Signature { get { return signature; } }
        private readonly Signature signature;

        /// <summary>
        /// Parameters specified within <c>use</c> 
        /// </summary>
        public List<FormalParam> UseParams { get { return useParams; } }
        private readonly List<FormalParam> useParams;

        //private readonly TypeSignature typeSignature;
        public BlockStmt/*!*/ Body { get { return body; } }
        private readonly BlockStmt/*!*/ body;

        public Text.Span HeadingSpan { get { return headingSpan; } }
        private Text.Span headingSpan;

        public Text.Span ParametersSpan { get { return parametersSpan; } }
        private Text.Span parametersSpan;

        /// <summary>
        /// Gets the source file <see cref="SourceUnit"/>. Cannot be <c>null</c>.
        /// </summary>
        public SourceUnit/*!*/SourceUnit { get { return this.sourceUnit; } }
        private readonly SourceUnit/*!*/sourceUnit;

        public TypeRef ReturnType { get { return returnType; } }
        private TypeRef returnType;

        #region Construction

        public LambdaFunctionExpr(SourceUnit/*!*/ sourceUnit,
            Text.Span span, Text.Span headingSpan,
            Scope scope, NamespaceDecl ns, bool aliasReturn, List<FormalParam>/*!*/ formalParams, 
            Text.Span paramSpan, List<FormalParam> useParams,
            BlockStmt/*!*/ body, TypeRef returnType)
            : base(span)
        {
            Debug.Assert(formalParams != null && body != null);
            Debug.Assert(sourceUnit != null);

            this.sourceUnit = sourceUnit;
            
            this.ns = ns;
            this.signature = new Signature(aliasReturn, formalParams);
            this.useParams = useParams;
            //this.typeSignature = new TypeSignature(genericParams);
            //this.attributes = new CustomAttributes(attributes);
            this.body = body;
            this.headingSpan = headingSpan;
            this.parametersSpan = paramSpan;
            this.returnType = returnType;
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitLambdaFunctionExpr(this);
        }
    }

    #endregion
}