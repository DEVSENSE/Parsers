using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Represents attribute.
    /// </summary>
    public interface IAttributeElement : ILangElement
    {
        /// <summary>
        /// Class reference.
        /// </summary>
        TypeRef ClassRef { get; }

        /// <summary>
        /// Gets arguments.
        /// </summary>
        CallSignature CallSignature { get; }
    }

    /// <summary>
    /// Attribute group.
    /// </summary>
    public interface IAttributeGroup : ILangElement
    {
        /// <summary>
        /// Gets attributes within the group.
        /// </summary>
        IAttributeElement[] Attributes { get; }
    }

    /// <summary>
    /// Represents attribute.
    /// </summary>
    public class AttributeElement : LangElement, IAttributeElement
    {
        public AttributeElement(Span span, TypeRef classref, CallSignature callsignature)
            : base(span)
        {
            this.ClassRef = classref;
            this.CallSignature = callsignature;
        }

        /// <summary>
        /// Attribute class reference.
        /// </summary>
        public TypeRef ClassRef { get; }

        /// <summary>
        /// Attribute arguments.
        /// </summary>
        public CallSignature CallSignature { get; }

        /// <summary>
        /// Visits the corresponding visitor method.
        /// </summary>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitAttribute(this);
        }
    }

    /// <summary>
    /// Attribute group.
    /// </summary>
    public class AttributeGroup : LangElement, IAttributeGroup
    {
        public AttributeGroup(Span span, IList<IAttributeElement> attributes)
            : base(span)
        {
            this.Attributes = attributes.AsArray();
        }

        /// <inheritdoc/>
        public IAttributeElement[] Attributes { get; }

        /// <inheritdoc/>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitAttributeGroup(this);
        }
    }
}
