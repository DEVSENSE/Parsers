using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Utilities
{
    public interface IChildrenProvider
    {
        ReadOnlySpan<ILangElement> GetChildren(ILangElement element);
    }

    public static class SyntaxHelpers
    {
        static readonly ObjectPool<IChildrenProvider> s_visitorPool = new ObjectPool<IChildrenProvider>(
            () => new GetChildrenVisitor(),
            (obj) => ((GetChildrenVisitor)obj).Free()
        );

        public static IChildrenProvider GetChildrenProvider() => s_visitorPool.Allocate();

        public static void ReturnChildrenProvider(IChildrenProvider obj) => s_visitorPool.Free(obj);

        /// <summary>
        /// Collects element child elements.
        /// </summary>
        sealed class GetChildrenVisitor : TreeVisitor, IChildrenProvider
        {
            /// <summary>
            /// Buffer.
            /// </summary>
            ILangElement[] _children = EmptyArray<ILangElement>.Instance;

            int _childrenCount = 0;
            int _childrenUsed = 0;

            public void Free()
            {
                if (_children.Length > 128 && _children.Length > _childrenUsed * 4)
                {
                    _children = EmptyArray<ILangElement>.Instance;
                }
                else
                {
                    Array.Clear(_children, 0, _childrenUsed);
                }

                _childrenCount = 0;
                _childrenUsed = 0;
            }

            void AddChild(ILangElement element)
            {
                if (element != null)
                {
                    if (_childrenCount == _children.Length)
                    {
                        Array.Resize(ref _children, (_children.Length + 2) * 2);
                    }

                    _children[_childrenCount++] = element;
                    _childrenUsed = Math.Max(_childrenUsed, _childrenCount); // remember how much elements we've used so we can clear them
                }
            }

            /// <summary>
            /// Gets list of direct child nodes.
            /// Cannot be <c>null</c>.
            /// </summary>
            public ReadOnlySpan<ILangElement> GetChildren(ILangElement element)
            {
                _childrenCount = 0;

                if (element != null)
                {
                    element.VisitMe(this);
                }

                return _children.AsSpan(0, _childrenCount);
            }

            //protected override void VisitList(INamedTypeRef[] items)
            //{
            //    if (items != null)
            //    {
            //        for (int i = 0; i < items.Length; i++)
            //        {
            //            AddChild(items[i]);
            //        }
            //    }
            //}

            public override void VisitElement(ILangElement element)
            {
                AddChild(element);

                // do not visit the child
            }
        }

        /// <summary>
        /// Gets enumeration of all elements in the subtree including the root.
        /// </summary>
        public static IEnumerable<ILangElement> EnumerateElements(this ILangElement root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            var childvisitor = SyntaxHelpers.GetChildrenProvider();
            var stack = StackObjectPool<ILangElement>.Allocate();
            stack.Push(root);

            //
            while (stack.TryPop(out var element))
            {
                yield return element;

                var children = childvisitor.GetChildren(element);

                foreach (var child in children)
                {
                    stack.Push(child);
                }
            }

            //
            StackObjectPool<ILangElement>.Free(stack);
            SyntaxHelpers.ReturnChildrenProvider(childvisitor);
        }
    }
}
