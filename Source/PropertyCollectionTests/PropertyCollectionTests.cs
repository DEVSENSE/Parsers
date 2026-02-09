using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Xunit;

namespace PropertyCollectionTests
{
    public partial class PropertyCollectionTests
    {
        [Fact]
        public void PropertyOfTypeTest()
        {
            var collection = new PropertyCollection();
            var phpdoc = new PHPDocBlock("", Span.Invalid);

            collection.SetProperty(phpdoc.GetType(), phpdoc);
            Assert.NotNull(collection.GetPropertyOfType<IDocBlock>());

            collection.SetProperty(typeof(string), "string");
            Assert.NotNull(collection.GetPropertyOfType<IDocBlock>());
            Assert.NotNull(collection.GetPropertyOfType<string>());

            collection.SetProperty(typeof(int), 123);
            Assert.NotNull(collection.GetPropertyOfType<IDocBlock>());
            Assert.NotNull(collection.GetPropertyOfType<string>());
        }
    }
}
