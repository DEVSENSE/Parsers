using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Extended <see cref="ITokenProvider{ValueType, PositionType}"/> for parser.
    /// Adds functionality for error recovery and PHP Doc management.
    /// </summary>
    /// <typeparam name="ValueType">Token value.</typeparam>
    /// <typeparam name="PositionType">Token span.</typeparam>
    internal interface IParserTokenProvider<ValueType, PositionType> : ITokenProvider<ValueType, PositionType>
    {
        /// <summary>
        /// List of all PHP Docs analyzed by lexer so far.
        /// </summary>
        DocCommentContainer DocCommentList { get; }

        /// <summary>
        /// The token returned by lexer before the current one. Default is <see cref="CompleteToken.Empty"/>.
        /// </summary>
        CompleteToken PreviousToken { get; }

        /// <summary>
        /// Add new tokens into the token stream. 
        /// Method may be empty, but error recovery will not work.
        /// </summary>
        /// <param name="tokensBuffer">List of tokens that will be inserted before the next token parsed from the source code.</param>
        /// <param name="previousToken">Token that will be considered the last one just before thebuffer is inserted.</param>
        void AddNextTokens(IList<CompleteToken> tokensBuffer, CompleteToken previousToken);
    }
}
