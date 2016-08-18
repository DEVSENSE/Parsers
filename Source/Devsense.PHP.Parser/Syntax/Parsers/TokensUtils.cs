using System;

using Devsense.PHP.Syntax;
using System.Diagnostics;

namespace Devsense.PHP.Parser.Syntax.Parsers
{
    public enum TokenCategory
    {
        Comment,
        Delimiter,
        Identifier,
        Keyword,
        LineComment,
        Number,
        Operator,
        String,
        Text,
        Unknown,
        WhiteSpace,

        Variable,
        ScriptTags,
        StringCode,
        Html
    }

    internal static class TokensUtils
    {
        public static TokenCategory GetTokenCategory(this Tokens token, Lexer.LexicalStates CurrentLexicalState)
        {
            bool inString = CurrentLexicalState == Lexer.LexicalStates.ST_DOUBLE_QUOTES || 
                CurrentLexicalState == Lexer.LexicalStates.ST_BACKQUOTE || 
                CurrentLexicalState == Lexer.LexicalStates.ST_HEREDOC;
            switch (token)
            {
                case Tokens.EOF:
                case Tokens.T_ERROR:
                    return TokenCategory.Unknown;

                #region Special Keywords

                case Tokens.T_GOTO:
                case Tokens.T_TRY:
                case Tokens.T_CATCH:
                case Tokens.T_FINALLY:
                case Tokens.T_THROW:
                case Tokens.T_INTERFACE:
                case Tokens.T_IMPLEMENTS:
                case Tokens.T_CLONE:
                case Tokens.T_ABSTRACT:
                case Tokens.T_FINAL:
                case Tokens.T_PRIVATE:
                case Tokens.T_PROTECTED:
                case Tokens.T_PUBLIC:
                case Tokens.T_INSTANCEOF:
                case Tokens.T_NAMESPACE:
                case Tokens.T_USE:
                    return TokenCategory.Keyword;

                #endregion

                #region Basic Keywords

                case Tokens.T_REQUIRE_ONCE:
                case Tokens.T_REQUIRE:
                case Tokens.T_EVAL:
                case Tokens.T_INCLUDE_ONCE:
                case Tokens.T_INCLUDE:
                case Tokens.T_LOGICAL_OR:           // or
                case Tokens.T_LOGICAL_XOR:          // xor
                case Tokens.T_LOGICAL_AND:          // and
                case Tokens.T_PRINT:
                case Tokens.T_NEW:
                case Tokens.T_EXIT:
                case Tokens.T_IF:
                case Tokens.T_ELSEIF:
                case Tokens.T_ELSE:
                case Tokens.T_ENDIF:
                case Tokens.T_ECHO:
                case Tokens.T_DO:
                case Tokens.T_WHILE:
                case Tokens.T_ENDWHILE:
                case Tokens.T_FOR:
                case Tokens.T_ENDFOR:
                case Tokens.T_FOREACH:
                case Tokens.T_ENDFOREACH:
                case Tokens.T_AS:
                case Tokens.T_SWITCH:
                case Tokens.T_ENDSWITCH:
                case Tokens.T_CASE:
                case Tokens.T_DEFAULT:
                case Tokens.T_BREAK:
                case Tokens.T_CONTINUE:
                case Tokens.T_FUNCTION:
                case Tokens.T_CONST:
                case Tokens.T_RETURN:
                case Tokens.T_YIELD:
                case Tokens.T_GLOBAL:
                case Tokens.T_STATIC:
                case Tokens.T_VAR:
                case Tokens.T_UNSET:
                case Tokens.T_ISSET:
                case Tokens.T_EMPTY:
                case Tokens.T_CLASS:
                case Tokens.T_TRAIT:
                case Tokens.T_INSTEADOF:
                case Tokens.T_EXTENDS:
                case Tokens.T_LIST:
                case Tokens.T_ARRAY:
                case Tokens.T_CLASS_C:              // __CLASS__
                case Tokens.T_TRAIT_C:              // __TRAIT__
                case Tokens.T_METHOD_C:             // __METHOD__
                case Tokens.T_FUNC_C:               // __FUNCTION__
                case Tokens.T_FILE:                 // __FILE__
                case Tokens.T_LINE:                 // __LINE__
                case Tokens.T_DIR:                  // __DIR__
                case Tokens.T_CALLABLE:             // callable
                    return TokenCategory.Keyword;

                #endregion

                #region Operators

                case Tokens.T_UNSET_CAST:           // (unset)
                case Tokens.T_BOOL_CAST:            // (bool)
                case Tokens.T_OBJECT_CAST:          // (object)
                case Tokens.T_ARRAY_CAST:           // (array)
                case Tokens.T_STRING_CAST:          // (string)
                case Tokens.T_DOUBLE_CAST:          // (double)
                case Tokens.T_INT_CAST:             // (int)
                case Tokens.T_AT:                   // @
                case Tokens.T_QUESTION:             // ?
                case Tokens.T_LT:                   // <
                case Tokens.T_GT:                   // >
                case Tokens.T_PERCENT:              // %
                case Tokens.T_EXCLAM:               // !
                case Tokens.T_TILDE:                // ~
                case Tokens.T_EQ:                   // =
                case Tokens.T_SLASH:                // /
                case Tokens.T_CARET:                // ^
                case Tokens.T_AMP:                  // &
                case Tokens.T_PLUS:                 // +
                case Tokens.T_MINUS:                // -
                case Tokens.T_PIPE:                 // |
                case Tokens.T_MUL:                  // *
                case Tokens.T_POW:                  // **
                case Tokens.T_DOT:                  // .
                case Tokens.T_SR_EQUAL:             // >>=
                case Tokens.T_SL_EQUAL:             // <<=
                case Tokens.T_XOR_EQUAL:            // ^=
                case Tokens.T_OR_EQUAL:             // |=
                case Tokens.T_AND_EQUAL:            // &=
                case Tokens.T_MOD_EQUAL:            // %=
                case Tokens.T_CONCAT_EQUAL:         // .=
                case Tokens.T_DIV_EQUAL:            // /=
                case Tokens.T_MUL_EQUAL:            // *=
                case Tokens.T_POW_EQUAL:            // **=
                case Tokens.T_MINUS_EQUAL:          // -=
                case Tokens.T_PLUS_EQUAL:           // +=
                case Tokens.T_BOOLEAN_OR:           // ||      
                case Tokens.T_BOOLEAN_AND:          // &&
                case Tokens.T_IS_NOT_IDENTICAL:     // !==
                case Tokens.T_IS_IDENTICAL:         // ===
                case Tokens.T_IS_NOT_EQUAL:         // !=
                case Tokens.T_IS_EQUAL:             // ==
                case Tokens.T_IS_GREATER_OR_EQUAL:  // >=
                case Tokens.T_IS_SMALLER_OR_EQUAL:  // <=
                case Tokens.T_SR:                   // >>
                case Tokens.T_SL:                   // <<
                case Tokens.T_DEC:                  // --
                case Tokens.T_INC:                  // ++
                case Tokens.T_DOUBLE_COLON:         // ::
                case Tokens.T_COLON:                // :
                case Tokens.T_DOUBLE_ARROW:         // =>
                case Tokens.T_ELLIPSIS:             // ...
                    return TokenCategory.Operator;

                #endregion

                #region Others

                case Tokens.T_LPAREN:                       // (
                case Tokens.T_RPAREN:                       // )
                case Tokens.T_SEMI:                         // ;
                case Tokens.T_COMMA:                        // ,
                case Tokens.T_NS_SEPARATOR:                 // \
                    return TokenCategory.Delimiter;

                //case Tokens.T_NAMESPACE_NAME:               // namespace name
                case Tokens.T_STRING_VARNAME:               // identifier following encapsulated "${"
                    return TokenCategory.Identifier;

                case Tokens.T_DNUMBER:                      // double (or overflown integer) out of string 
                case Tokens.T_LNUMBER:                      // integer (or hex integer) out of string
                    return TokenCategory.Number;

                case Tokens.T_DOUBLE_QUOTES:                // "
                case Tokens.T_BACKQUOTE:                    // `
                case Tokens.T_START_HEREDOC:                // <<<XXX
                case Tokens.T_END_HEREDOC:                  // XXX
                case Tokens.T_ENCAPSED_AND_WHITESPACE:      // character(s) in string
                case Tokens.T_CONSTANT_ENCAPSED_STRING:     // quoted string not containing '$' 
                case Tokens.T_NUM_STRING:                   // number in string
                    return TokenCategory.String;

                case Tokens.T_DOLLAR_OPEN_CURLY_BRACES:     // "${" in string - starts non-string code
                case Tokens.T_CURLY_OPEN:                   // "{$" in string
                    return TokenCategory.StringCode;

                case Tokens.T_WHITESPACE:
                    return TokenCategory.WhiteSpace;

                case Tokens.T_COMMENT:
                case Tokens.T_DOC_COMMENT:
                    return TokenCategory.Comment;

                case Tokens.T_OPEN_TAG:
                case Tokens.T_OPEN_TAG_WITH_ECHO:
                case Tokens.T_CLOSE_TAG:
                    return TokenCategory.ScriptTags;

                case Tokens.T_INLINE_HTML:
                    return TokenCategory.Html;

                #endregion

                #region Tokens with Ambiguous Category

                case Tokens.T_LBRACKET:                     // [
                case Tokens.T_RBRACKET:                     // ]
                case Tokens.T_LBRACE:                       // {
                    return (inString) ? TokenCategory.String : TokenCategory.Delimiter;

                case Tokens.T_RBRACE:                       // }
                    if (inString)
                        // we are in string:
                        return TokenCategory.String;
                    else
                        // part of script:
                        return TokenCategory.Delimiter;

                case Tokens.T_STRING:                       // identifier
                    return (inString) ? TokenCategory.String : TokenCategory.Identifier;

                case Tokens.T_DOLLAR:                       // isolated '$'
                case Tokens.T_OBJECT_OPERATOR:              // ->
                    return (inString) ? TokenCategory.StringCode : TokenCategory.Operator;

                case Tokens.T_VARIABLE:                     // identifier
                    return (inString) ? TokenCategory.StringCode : TokenCategory.Variable;

                #endregion

                default:
                    return TokenCategory.Unknown;
            }
        }
    }
}
