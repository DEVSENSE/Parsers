using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PHP.Core.Text;
using PHP.Core.AST;
using PHP.Syntax;
using System.IO;
using System.Diagnostics;

namespace PhpParser.Parser
{

    public partial class Parser
    {
        LanguageFeatures _features;
        ITokenProvider<SemanticValueType, Span> _lexer;
        INodesFactory<LangElement, Span> _astFactory;
        Scope _currentScope;
        NamingContext _context;

        /// <summary>
        /// The root of AST.
        /// </summary>
        private LangElement _astRoot;

        protected sealed override int EofToken
        {
            get { return (int)Tokens.END; }
        }

        protected sealed override int ErrorToken
        {
            get { return (int)Tokens.T_ERROR; }
        }

        protected override Span CombinePositions(Span first, Span last)
        {
            if (last.IsValid)
            {
                if (first.IsValid)
                    return Span.Combine(first, last);
                else
                    return last;
            }
            else
                return first;
        }

        public LangElement Parse(
            ITokenProvider<SemanticValueType, Span> lexer, INodesFactory<LangElement, Span> astFactory,
            LanguageFeatures features, int positionShift = 0)
        {
            // initialization:
            this._features = features;
            this._lexer = lexer;
            this._astFactory = astFactory;
            //InitializeFields();

            this._currentScope = new Scope(1); // starts assigning scopes from 2 (1 is reserved for prepended inclusion)

            base.Scanner = this._lexer;
            base.Parse();

            LangElement result = _astRoot;

            // clean and let GC collect unused AST and other stuff:
            //ClearFields();

            return result;
        }
    }
}
