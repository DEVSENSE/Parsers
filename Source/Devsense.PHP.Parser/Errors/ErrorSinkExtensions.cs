using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Errors
{
    internal static class ErrorSinkExtensions
    {
        public static void SyntaxError<TSpan>(this IErrorSink<TSpan> errors, string unexpected_token, TSpan unexpected_token_span, string[] expected_tokens)
        {
            if (unexpected_token.Length != 0)
            {
                errors.Error(
                    unexpected_token_span,
                    FatalErrors.SyntaxError,
                    string.Format(Strings.unexpected_token, unexpected_token)
                );
            }
            else
            {
                // EOF
                errors.Error(
                    unexpected_token_span,
                    FatalErrors.SyntaxError,
                    Strings.unexpected_eof
                );
            }
        }
    }
}
