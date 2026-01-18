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
    }
}
