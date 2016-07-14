using PHP.Core.Text;
using PHP.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhpParser.Parser
{
    /// <summary>
    /// Sink for comment tokens and tokens not handled in parser.
    /// These tokens are ignored by tokenizer, so they are not available in resulting AST.
    /// By providing this interface as a part of <see cref="IReductionsSink"/> implementation, implementers may handle additional language elements at token level.
    /// </summary>
    public interface ICommentsSink
    {
        void OnLineComment(Lexer/*!*/scanner, TextSpan span);
        void OnComment(Lexer/*!*/scanner, TextSpan span);
        void OnPhpDocComment(Lexer/*!*/scanner, PHPDocBlock phpDocBlock);

        void OnOpenTag(Lexer/*!*/scanner, TextSpan span);
        void OnCloseTag(Lexer/*!*/scanner, TextSpan span);
    }
}
