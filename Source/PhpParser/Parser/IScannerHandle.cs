using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhpParser.Parser
{
    public interface IScannerHandler
    {
        /// <summary>
        /// Called by <see cref="Scanner"/> when new token is obtained from lexer.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="buffer">Internal text buffer.</param>
        /// <param name="tokenStart">Position within <paramref name="buffer"/> where the token text starts.</param>
        /// <param name="tokenLength">Length of the token text.</param>
        void OnNextToken(Tokens token, char[] buffer, int tokenStart, int tokenLength);
    }
}
