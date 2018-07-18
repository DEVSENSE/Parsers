// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.


namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// A token category.
    /// </summary>
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

    /// <summary>
    /// Extension methods for <see cref="Tokens"/>.
    /// </summary>
    public static class TokensExtension
    {
        private static bool IsInString(Lexer.LexicalStates state)
        {
            switch (state)
            {
                case Lexer.LexicalStates.ST_DOUBLE_QUOTES:
                case Lexer.LexicalStates.ST_BACKQUOTE:
                case Lexer.LexicalStates.ST_HEREDOC:
                case Lexer.LexicalStates.ST_IN_HEREDOC:
                case Lexer.LexicalStates.ST_IN_STRING:
                case Lexer.LexicalStates.ST_IN_SHELL:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsInString(Lexer lexer)
        {
            if (lexer.CurrentLexicalState == Lexer.LexicalStates.ST_LOOKING_FOR_PROPERTY)
            {
                return IsInString(lexer.PreviousLexicalState);
            }
            else
            {
                return IsInString(lexer.CurrentLexicalState);
            }
        }

        /// <summary>
        /// Gets interface|trait or class token.
        /// </summary>
        public static Tokens AsTypeKeywordToken(this Ast.TypeDecl tdecl)
        {
            switch (tdecl.MemberAttributes & (PhpMemberAttributes.Trait | PhpMemberAttributes.Interface))
            {
                case PhpMemberAttributes.Interface: return Tokens.T_INTERFACE;
                case PhpMemberAttributes.Trait: return Tokens.T_TRAIT;
                default: return Tokens.T_CLASS;
            }
        }

        public static Tokens AsToken(this PhpMemberAttributes attributes)
        {
            switch (attributes)
            {
                case PhpMemberAttributes.Public:
                    return Tokens.T_PUBLIC;
                case PhpMemberAttributes.Private:
                    return Tokens.T_PRIVATE;
                case PhpMemberAttributes.Protected:
                    return Tokens.T_PROTECTED;
                case PhpMemberAttributes.Static:
                    return Tokens.T_STATIC;
                case PhpMemberAttributes.Abstract:
                    return Tokens.T_ABSTRACT;
                case PhpMemberAttributes.Final:
                    return Tokens.T_FINAL;
                case PhpMemberAttributes.Interface:
                    return Tokens.T_INTERFACE;
                case PhpMemberAttributes.Trait:
                    return Tokens.T_TRAIT;
                default:
                    return Tokens.T_ERROR;
            }
        }
        public static PhpMemberAttributes ToModifier(this Tokens token)
        {
            switch (token)
            {
                case Tokens.T_PUBLIC:
                    return PhpMemberAttributes.Public;
                case Tokens.T_PRIVATE:
                    return PhpMemberAttributes.Private;
                case Tokens.T_PROTECTED:
                    return PhpMemberAttributes.Protected;
                case Tokens.T_STATIC:
                    return PhpMemberAttributes.Static;
                case Tokens.T_ABSTRACT:
                    return PhpMemberAttributes.Abstract;
                case Tokens.T_FINAL:
                    return PhpMemberAttributes.Final;
                case Tokens.T_INTERFACE:
                    return PhpMemberAttributes.Interface;
                case Tokens.T_TRAIT:
                    return PhpMemberAttributes.Trait;
                default:
                    return PhpMemberAttributes.None;
            }
        }

        /// <summary>
        /// Gets category of a token in given lexical context.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="lexer">Current lexer.</param>
        /// <returns>A token category.</returns>
        public static TokenCategory GetTokenCategory(this Tokens token, Lexer lexer)
        {
            switch (token)
            {
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
                case Tokens.T_COALESCE:             // ??
                case Tokens.T_SPACESHIP:            // <=>
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
                    return IsInString(lexer) ? TokenCategory.String : TokenCategory.Delimiter;

                case Tokens.T_RBRACE:                       // }
                    if (IsInString(lexer))
                        // we are in string:
                        return TokenCategory.StringCode;
                    else
                        // part of script:
                        return TokenCategory.Delimiter;

                case Tokens.T_STRING:                       // identifier
                    return IsInString(lexer) ? TokenCategory.String : TokenCategory.Identifier;

                case Tokens.T_DOLLAR:                       // isolated '$'
                case Tokens.T_OBJECT_OPERATOR:              // ->
                    return IsInString(lexer) ? TokenCategory.StringCode : TokenCategory.Operator;

                case Tokens.T_VARIABLE:                     // identifier
                    return IsInString(lexer) ? TokenCategory.StringCode : TokenCategory.Variable;

                #endregion

                default:
                    return TokenCategory.Unknown;
            }
        }
    }
}
