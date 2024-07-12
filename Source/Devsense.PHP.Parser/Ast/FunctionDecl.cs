// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using Devsense.PHP.Ast.DocBlock;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// A function/method/lambda declaration.
    /// </summary>
    public interface IFunctionDeclaration : ILangElement
    {
        /// <summary>
        /// Function signature.
        /// </summary>
        Signature Signature { get; }

        /// <summary>
        /// Return type if present.
        /// </summary>
        TypeRef ReturnType { get; }

        /// <summary>
        /// Function body. Is either a block statement (function, method, closure) or an expression (arrow func).
        /// </summary>
        ILangElement Body { get; }
    }

    /// <summary>
    /// A function with name (global function or method).
    /// </summary>
    public interface INamedFunctionDeclaration : IFunctionDeclaration
    {
        NameRef Name { get; }
    }

    #region FormalParam

    /// <summary>
    /// Represents a formal parameter definition.
    /// </summary>
    public class FormalParam : LangElement
    {
        [Flags]
        public enum Flags : byte
        {
            Default = 0,
            IsByRef = 1,
            IsOut = 2,
            IsVariadic = 4,

            IsConstructorPropertyPublic = 8,
            IsConstructorPropertyPrivate = 16,
            IsConstructorPropertyProtected = 32,
            IsConstructorPropertyReadOnly = 64,
            IsConstructorPropertyMask = IsConstructorPropertyPublic | IsConstructorPropertyPrivate | IsConstructorPropertyProtected | IsConstructorPropertyReadOnly,
        }

        /// <summary>
        /// Flags describing the parameter.
        /// </summary>
        protected virtual Flags FlagsValue => 0;

        /// <summary>
        /// Name of the argument.
        /// </summary>
        public virtual VariableNameRef Name => new VariableNameRef(this.Span, this.VariableName);

        /// <summary>
        /// Whether the parameter is &amp;-modified.
        /// </summary>
        public bool PassedByRef => (FlagsValue & Flags.IsByRef) != 0;

        /// <summary>
        /// Whether the parameter is an out-parameter. Set by applying the [Out] attribute.
        /// </summary>
        public bool IsOut => (FlagsValue & Flags.IsOut) != 0;

        /// <summary>
        /// Gets value indicating whether the parameter is variadic and so passed parameters will be packed into the array as passed as one parameter.
        /// </summary>
        public bool IsVariadic => (FlagsValue & Flags.IsVariadic) != 0;

        /// <summary>
        /// Gets value indicating the parameter is a constructor property (PHP8).
        /// </summary>
        public virtual bool IsConstructorProperty => false;

        /// <summary>
        /// In case the parameter is <see cref="IsConstructorProperty"/>, gets the member visibility.
        /// </summary>
        public virtual PhpMemberAttributes ConstructorPropertyFlags => 0;

        protected VariableName VariableName { get; }

        /// <summary>
        /// Initial value expression. Can be <B>null</B>.
        /// </summary>
        public virtual Expression InitValue => null;

        /// <summary>
        /// Either <see cref="TypeRef"/> or <B>null</B>.
        /// </summary>
        public virtual TypeRef TypeHint => null;

        /// <summary>
        /// In case of constructor property with property hooks, gets list of hooks.
        /// </summary>
        public virtual PropertyHookDecl[] PropertyHooks => null;

        #region Construction

        public static FormalParam Create(VariableNameRef name)
        {
            return new FormalParam(name.Span, name.Name);
        }

        public static FormalParam Create(
            Text.Span span, VariableNameRef name,
            TypeRef typeHint = null, Flags flags = 0,
            Expression initValue = null,
            PhpMemberAttributes constructorPropertyVisibility = 0,
            PropertyHookDecl[] property_hooks = null
        )
        {
            if (span == name.Span && typeHint == null && flags == Flags.Default && initValue == null && constructorPropertyVisibility == 0 && property_hooks == null)
            {
                return Create(name);
            }
            else
            {
                return new FormalParamEx(span, name, typeHint, flags, initValue,
                    constructorPropertyVisibility: constructorPropertyVisibility,
                    property_hooks: property_hooks
                );
            }
        }

        public FormalParam(Text.Span span, VariableName name) : base(span)
        {
            this.VariableName = name;
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitFormalParam(this);
        }
    }

    internal class FormalParamEx : FormalParam
    {
        protected override Flags FlagsValue { get; }

        Text.Span VariableNameSpan { get; }

        public override VariableNameRef Name => new VariableNameRef(VariableNameSpan, VariableName);

        public override TypeRef TypeHint { get; }

        public override Expression InitValue { get; }

        public override bool IsConstructorProperty => (FlagsValue & Flags.IsConstructorPropertyMask) != 0;

        /// <summary>
        /// In case the parameter is <see cref="IsConstructorProperty"/>, gets the member visibility.
        /// </summary>
        public override PhpMemberAttributes ConstructorPropertyFlags
        {
            get
            {
                var result = (PhpMemberAttributes)0;

                if ((FlagsValue & Flags.IsConstructorPropertyPublic) != 0) result |= PhpMemberAttributes.Public; // 0
                if ((FlagsValue & Flags.IsConstructorPropertyPrivate) != 0) result |= PhpMemberAttributes.Private;
                if ((FlagsValue & Flags.IsConstructorPropertyProtected) != 0) result |= PhpMemberAttributes.Protected;
                if ((FlagsValue & Flags.IsConstructorPropertyReadOnly) != 0) result |= PhpMemberAttributes.ReadOnly;

                return result;
            }
        }

        public override PropertyHookDecl[] PropertyHooks => this.GetProperty<PropertyHookDecl[]>();

        public FormalParamEx(
            Text.Span span, VariableNameRef name,
            TypeRef typeHint, Flags flags,
            Expression initValue,
            PhpMemberAttributes constructorPropertyVisibility,
            PropertyHookDecl[] property_hooks
        ) : base(span, name.Name)
        {
            this.VariableNameSpan = name.Span;
            this.TypeHint = typeHint;
            this.InitValue = initValue;
            this.FlagsValue = flags;

            if (constructorPropertyVisibility != 0)
            {
                Debug.Assert((constructorPropertyVisibility & PhpMemberAttributes.Constructor) != 0);

                this.FlagsValue |= (constructorPropertyVisibility & PhpMemberAttributes.VisibilityMask) switch
                {
                    PhpMemberAttributes.Private => Flags.IsConstructorPropertyPrivate,
                    PhpMemberAttributes.Protected => Flags.IsConstructorPropertyProtected,
                    //PhpMemberAttributes.Public => Flags.IsConstructorPropertyPublic,
                    _ => Flags.IsConstructorPropertyPublic,
                };

                if (constructorPropertyVisibility.IsReadOnly())
                {
                    this.FlagsValue |= Flags.IsConstructorPropertyReadOnly;
                }
            }

            if (property_hooks != null)
            {
                this.SetProperty<PropertyHookDecl[]>(property_hooks);
            }
        }
    }

    #endregion

    #region Signature

    public struct Signature
    {
        public bool AliasReturn { get { return aliasReturn; } }
        private readonly bool aliasReturn;

        public FormalParam[]/*!*/ FormalParams { get { return formalParams; } }
        private readonly FormalParam[]/*!*/ formalParams;

        /// <summary>
        /// Signature position including the parentheses.
        /// </summary>
        public Text.Span Span { get { return _span; } }
        private Text.Span _span;

        public Signature(bool aliasReturn, FormalParam[]/*!*/ formalParams, Text.Span position)
        {
            this.aliasReturn = aliasReturn;
            this.formalParams = formalParams ?? EmptyArray<FormalParam>.Instance;
            _span = position;
        }
    }

    #endregion

    #region FunctionDecl

    /// <summary>
    /// Represents a function declaration.
    /// </summary>
    public sealed class FunctionDecl : Statement, INamedFunctionDeclaration
    {
        internal override bool IsDeclaration { get { return true; } }

        ILangElement IFunctionDeclaration.Body => this.Body;

        public NameRef Name { get { return name; } }
        private readonly NameRef name;

        public Signature Signature { get { return signature; } }
        private readonly Signature signature;

        public TypeSignature TypeSignature => this.GetTypeSignature();

        public BlockStmt/*!*/ Body { get; set; }

        /// <summary>
        /// Gets value indicating whether the function is declared conditionally.
        /// </summary>
        public bool IsConditional { get; internal set; }

        /// <summary>
        /// Gets function declaration attributes.
        /// </summary>
        public PhpMemberAttributes MemberAttributes { get; private set; }

        public Text.Span ParametersSpan { get { return Signature.Span; } }

        /// <summary>
        /// Span of the entire method header.
        /// </summary>
        public Text.Span HeadingSpan
        {
            get
            {
                if (Span.IsValid)
                {
                    var endspan = ReturnType != null ? ReturnType.Span : Signature.Span;
                    if (endspan.IsValid)
                    {
                        return Text.Span.FromBounds(Span.Start, endspan.End);
                    }
                }

                //
                return Text.Span.Invalid;
            }
        }

        public TypeRef ReturnType { get; }

        #region Construction

        public FunctionDecl(
            Text.Span span,
            bool isConditional, PhpMemberAttributes memberAttributes, NameRef/*!*/ name,
            bool aliasReturn, FormalParam[]/*!*/ formalParams, Text.Span paramsSpan, FormalTypeParam[]/*!*/ genericParams,
            BlockStmt body, TypeRef returnType)
            : base(span)
        {
            Debug.Assert(formalParams != null && body != null);

            this.name = name;
            this.signature = new Signature(aliasReturn, formalParams, paramsSpan);
            this.Body = body;
            this.IsConditional = isConditional;
            this.MemberAttributes = memberAttributes;
            this.ReturnType = returnType;

            if (genericParams != null && genericParams.Length != 0)
            {
                this.SetTypeSignature(new TypeSignature(genericParams));
            }
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitFunctionDecl(this);
        }

        /// <summary>
        /// <see cref="IDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public IDocBlock PHPDoc
        {
            get => this.GetPHPDoc();
            set => this.SetPHPDoc(value);
        }
    }

    #endregion
}
