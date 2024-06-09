using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PropertyCollectionTests
{
    [TestClass]
    public partial class PropertyCollectionTests
    {
        [TestMethod]
        public void PropertyOfTypeTest()
        {
            var collection = new PropertyCollection();
            var phpdoc = new PHPDocBlock("", Span.Invalid);

            collection.SetProperty(phpdoc.GetType(), phpdoc);
            Assert.IsNotNull(collection.GetPropertyOfType<IDocBlock>());

            collection.SetProperty(typeof(string), "string");
            Assert.IsNotNull(collection.GetPropertyOfType<IDocBlock>());
            Assert.IsNotNull(collection.GetPropertyOfType<string>());

            collection.SetProperty(typeof(int), 123);
            Assert.IsNotNull(collection.GetPropertyOfType<IDocBlock>());
            Assert.IsNotNull(collection.GetPropertyOfType<string>());
        }
    }
}
