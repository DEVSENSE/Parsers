using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Token facts.
    /// </summary>
    public static class TokenFacts
    {
        /// <summary>
        /// Map of semireserved tokens source text to their original <see cref="Tokens"/> value.
        /// </summary>
        internal readonly static Dictionary<string, Tokens> s_reservedNameToToken = new Dictionary<string, Tokens>(StringComparer.OrdinalIgnoreCase)
        {
            //semi_reserved:
            {"include", Tokens.T_INCLUDE}, {"include_once", Tokens.T_INCLUDE_ONCE}, {"eval", Tokens.T_EVAL},
            {"REQUIRE", Tokens.T_REQUIRE}, {"REQUIRE_ONCE", Tokens.T_REQUIRE_ONCE},
            {"OR", Tokens.T_LOGICAL_OR}, {"XOR", Tokens.T_LOGICAL_XOR}, {"AND", Tokens.T_LOGICAL_AND},
            {"INSTANCEOF", Tokens.T_INSTANCEOF}, {"NEW", Tokens.T_NEW}, {"CLONE", Tokens.T_CLONE}, {"EXIT", Tokens.T_EXIT},
            {"IF", Tokens.T_IF}, {"ELSEIF", Tokens.T_ELSEIF}, {"ELSE", Tokens.T_ELSE}, {"ENDIF", Tokens.T_ENDIF},
            {"ECHO", Tokens.T_ECHO}, { "PRINT", Tokens.T_PRINT},
            {"DO", Tokens.T_DO}, {"WHILE", Tokens.T_WHILE}, {"ENDWHILE", Tokens.T_ENDWHILE},
            {"FOR", Tokens.T_FOR}, {"ENDFOR", Tokens.T_ENDFOR}, {"FOREACH", Tokens.T_FOREACH}, {"ENDFOREACH", Tokens.T_ENDFOREACH},
            {"DECLARE", Tokens.T_DECLARE}, {"ENDDECLARE", Tokens.T_ENDDECLARE},
            {"AS", Tokens.T_AS}, {"TRY", Tokens.T_TRY}, {"CATCH", Tokens.T_CATCH}, {"FINALLY", Tokens.T_FINALLY},
            {"THROW", Tokens.T_THROW},
            {"USE", Tokens.T_USE}, {"INSTEADOF", Tokens.T_INSTEADOF},
            {"GLOBAL", Tokens.T_GLOBAL}, {"VAR", Tokens.T_VAR}, {"UNSET", Tokens.T_UNSET}, {"ISSET", Tokens.T_ISSET},
            {"EMPTY", Tokens.T_EMPTY}, {"CONTINUE", Tokens.T_CONTINUE}, {"BREAK", Tokens.T_BREAK}, {"GOTO", Tokens.T_GOTO},
            {"FUNCTION", Tokens.T_FUNCTION}, {"CONST", Tokens.T_CONST}, {"RETURN", Tokens.T_RETURN},
            {"YIELD", Tokens.T_YIELD},
            {"SWITCH", Tokens.T_SWITCH}, {"ENDSWITCH", Tokens.T_ENDSWITCH}, {"CASE", Tokens.T_CASE}, {"DEFAULT", Tokens.T_DEFAULT}, {"MATCH", Tokens.T_MATCH},
            {"ARRAY", Tokens.T_ARRAY}, { "LIST", Tokens.T_LIST}, {"CALLABLE", Tokens.T_CALLABLE},
            { "EXTENDS", Tokens.T_EXTENDS}, {"IMPLEMENTS", Tokens.T_IMPLEMENTS}, {"NAMESPACE", Tokens.T_NAMESPACE},
            {"TRAIT", Tokens.T_TRAIT}, {"INTERFACE", Tokens.T_INTERFACE}, {"CLASS", Tokens.T_CLASS},
            {"__CLASS__", Tokens.T_CLASS_C}, {"__TRAIT__", Tokens.T_TRAIT_C}, {"__FUNCTION__", Tokens.T_FUNC_C}, {"__METHOD__", Tokens.T_METHOD_C}, {"__LINE__", Tokens.T_LINE}, {"__FILE__", Tokens.T_FILE}, {"__DIR__", Tokens.T_DIR}, {"__NAMESPACE__", Tokens.T_NS_C},
            {"STATIC", Tokens.T_STATIC}, {"ABSTRACT", Tokens.T_ABSTRACT}, {"FINAL", Tokens.T_FINAL}, {"PRIVATE", Tokens.T_PRIVATE}, {"PROTECTED", Tokens.T_PROTECTED}, {"PUBLIC", Tokens.T_PUBLIC},
        };

        /// <summary>
        /// Gets textual value of given token.
        /// </summary>
        public static string GetTokenText(Tokens t)
        {
            switch (t)
            {
                case Tokens.T_COMMA: return ",";
                case Tokens.T_SEMI: return ";";
                case Tokens.T_COLON: return ":";

                case Tokens.T_LPAREN: return "(";
                case Tokens.T_RPAREN: return ")";
                case Tokens.T_LBRACE: return "{";
                case Tokens.T_RBRACE: return "}";
                case Tokens.T_LBRACKET: return "[";
                case Tokens.T_RBRACKET: return "]";

                case Tokens.T_LOGICAL_XOR: return "xor";
                case Tokens.T_LOGICAL_OR: return "or";
                case Tokens.T_LOGICAL_AND: return "and";
                case Tokens.T_BOOLEAN_AND: return "&&";
                case Tokens.T_BOOLEAN_OR: return "||";
                case Tokens.T_PIPE: return "|";
                case Tokens.T_CARET: return "^";
                case Tokens.T_ELLIPSIS: return "...";
                case Tokens.T_AMP: return "&";
                case Tokens.T_DOUBLE_COLON: return "::";
                case Tokens.T_OBJECT_OPERATOR: return "->";
                case Tokens.T_DOUBLE_ARROW: return "=>";

                case Tokens.T_IS_EQUAL: return "==";
                case Tokens.T_IS_NOT_EQUAL: return "!=";
                case Tokens.T_COALESCE: return "??";
                case Tokens.T_INSTANCEOF: return "instanceof";

                case Tokens.T_GLOBAL: return "global";
                case Tokens.T_STATIC: return "static";
                case Tokens.T_FINAL: return "final";
                case Tokens.T_ABSTRACT: return "abstract";
                case Tokens.T_FUNCTION: return "function";
                case Tokens.T_FN: return "fn";
                case Tokens.T_CONST: return "const";
                case Tokens.T_VAR: return "var";
                case Tokens.T_PRIVATE: return "private";
                case Tokens.T_PROTECTED: return "protected";
                case Tokens.T_PUBLIC: return "public";

                case Tokens.T_EXTENDS: return "extends";
                case Tokens.T_IMPLEMENTS: return "implements";

                case Tokens.T_CLASS_C: return "__CLASS__";
                case Tokens.T_TRAIT_C: return "__TRAIT__";
                case Tokens.T_NS_C: return "__NAMESPACE__";
                case Tokens.T_FUNC_C: return "__FUNCTION__";
                case Tokens.T_METHOD_C: return "__METHOD__";
                case Tokens.T_FILE: return "__FILE__";
                case Tokens.T_LINE: return "__LINE__";
                case Tokens.T_DIR: return "__DIR__";

                case Tokens.T_YIELD: return "yield";
                case Tokens.T_YIELD_FROM: return "yield from";
                case Tokens.T_GOTO: return "goto";
                case Tokens.T_DEFAULT: return "default";
                case Tokens.T_CASE: return "case";
                case Tokens.T_SWITCH: return "switch";
                case Tokens.T_MATCH: return "match";
                case Tokens.T_ENDSWITCH: return "endswitch";
                case Tokens.T_BREAK: return "break";
                case Tokens.T_CONTINUE: return "continue";
                case Tokens.T_RETURN: return "return";

                case Tokens.T_EXCLAM: return "!";
                case Tokens.T_PLUS: return "+";
                case Tokens.T_MINUS: return "-";
                case Tokens.T_MUL: return "*";
                case Tokens.T_SLASH: return "/";
                case Tokens.T_PERCENT: return "%";
                case Tokens.T_POW: return "**";
                case Tokens.T_DOT: return ".";
                case Tokens.T_SPACESHIP: return "<=>";
                case Tokens.T_IS_GREATER_OR_EQUAL: return ">=";
                case Tokens.T_IS_SMALLER_OR_EQUAL: return "<=";
                case Tokens.T_LT: return "<";
                case Tokens.T_EQ: return "=";
                case Tokens.T_GT: return ">";

                case Tokens.T_IS_IDENTICAL: return "===";
                case Tokens.T_IS_NOT_IDENTICAL: return "!==";

                case Tokens.T_BOOL_CAST: return "(bool)";
                case Tokens.T_INT_CAST: return "(int)";
                case Tokens.T_DOUBLE_CAST: return "(double)";
                case Tokens.T_STRING_CAST: return "(string)";
                case Tokens.T_OBJECT_CAST: return "(object)";
                case Tokens.T_ARRAY_CAST: return "(array)";
                case Tokens.T_UNSET_CAST: return "(unset)";

                case Tokens.T_IF: return "if";
                case Tokens.T_ELSEIF: return "elseif";
                case Tokens.T_ELSE: return "else";
                case Tokens.T_ENDIF: return "endif";

                case Tokens.T_DO: return "do";
                case Tokens.T_WHILE: return "while";
                case Tokens.T_ENDWHILE: return "endwhile";
                case Tokens.T_FOR: return "for";
                case Tokens.T_ENDFOR: return "endfor";
                case Tokens.T_FOREACH: return "foreach";
                case Tokens.T_ENDFOREACH: return "endforeach";
                case Tokens.T_DECLARE: return "declare";
                case Tokens.T_ENDDECLARE: return "enddeclare";
                case Tokens.T_AS: return "as";
                case Tokens.T_THROW: return "throw";
                case Tokens.T_NEW: return "new";
                case Tokens.T_ISSET: return "isset";
                case Tokens.T_UNSET: return "unset";
                case Tokens.T_USE: return "use";

                case Tokens.T_HALT_COMPILER: return "__halt_compiler";
                case Tokens.T_PRINT: return "print";
                case Tokens.T_INTERFACE: return "interface";
                case Tokens.T_TRAIT: return "trait";
                case Tokens.T_SL: return "<<";
                case Tokens.T_SR: return ">>";
                case Tokens.T_TILDE: return "~";
                case Tokens.T_CLONE: return "clone";
                case Tokens.T_AT: return "@";
                case Tokens.T_NAMESPACE: return "namespace";

                case Tokens.T_CURLY_OPEN: return "{$";
                case Tokens.T_DOLLAR: return "$";
                case Tokens.T_DOLLAR_OPEN_CURLY_BRACES: return "${";

                case Tokens.T_PLUS_EQUAL: return "+=";
                case Tokens.T_MINUS_EQUAL: return "-=";
                case Tokens.T_MUL_EQUAL: return "*=";
                case Tokens.T_POW_EQUAL: return "**=";
                case Tokens.T_DIV_EQUAL: return "/=";
                case Tokens.T_MOD_EQUAL: return "%=";
                case Tokens.T_AND_EQUAL: return "&=";
                case Tokens.T_OR_EQUAL: return "|=";
                case Tokens.T_XOR_EQUAL: return "^=";
                case Tokens.T_COALESCE_EQUAL: return "??=";
                case Tokens.T_CONCAT_EQUAL: return ".=";
                case Tokens.T_SL_EQUAL: return "<<=";
                case Tokens.T_SR_EQUAL: return ">>=";

                case Tokens.T_ARRAY: return "array";
                case Tokens.T_LIST: return "list";
                case Tokens.T_CATCH: return "catch";
                case Tokens.T_FINALLY: return "finally";
                case Tokens.T_QUESTION: return "?";

                case Tokens.T_ECHO: return "echo";
                case Tokens.T_EVAL: return "eval";
                case Tokens.T_EXIT: return "exit";
                case Tokens.T_EMPTY: return "empty";

                case Tokens.T_INCLUDE: return "include";
                case Tokens.T_REQUIRE: return "require";
                case Tokens.T_INCLUDE_ONCE: return "include_once";
                case Tokens.T_REQUIRE_ONCE: return "require_once";
                case Tokens.T_NS_SEPARATOR: return "\\";
                case Tokens.T_CALLABLE: return "callable";
                case Tokens.T_TRY: return "try";
                case Tokens.T_CLASS: return "class";
                case Tokens.T_BACKQUOTE: return "`";
                case Tokens.T_SINGLE_QUOTES: return "'";
                case Tokens.T_DOUBLE_QUOTES: return "\"";
                case Tokens.T_INSTEADOF: return "insteadof";
                case Tokens.T_OPEN_TAG_WITH_ECHO: return "<?=";

                default: throw new ArgumentException(t.ToString());
            }
        }

        /// <summary>
        /// Gets token corresponding to given pseudo constant <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Pseudo constant type.</param>
        /// <returns>Corresponding token.</returns>
        public static Tokens GetPseudoConstUseToken(PseudoConstUse.Types type)
        {
            switch (type)
            {
                case PseudoConstUse.Types.Class: return (Tokens.T_CLASS_C);
                case PseudoConstUse.Types.Trait: return (Tokens.T_TRAIT_C);
                case PseudoConstUse.Types.Namespace: return (Tokens.T_NS_C);
                case PseudoConstUse.Types.Function: return (Tokens.T_FUNC_C);
                case PseudoConstUse.Types.Method: return (Tokens.T_METHOD_C);
                case PseudoConstUse.Types.File: return (Tokens.T_FILE);
                case PseudoConstUse.Types.Line: return (Tokens.T_LINE);
                case PseudoConstUse.Types.Dir: return (Tokens.T_DIR);
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets token corresponding to given unary or binary operation.
        /// </summary>
        public static Tokens GetOperationToken(Operations op)
        {
            switch (op)
            {
                // unary:
                case Operations.Plus: return Tokens.T_PLUS;
                case Operations.Minus: return Tokens.T_MINUS;
                case Operations.LogicNegation: return Tokens.T_EXCLAM;
                case Operations.BitNegation: return Tokens.T_TILDE;
                case Operations.AtSign: return Tokens.T_AT;
                case Operations.Print: return Tokens.T_PRINT;
                case Operations.Clone: return Tokens.T_CLONE;

                // casts:
                case Operations.BoolCast: return Tokens.T_BOOL_CAST;
                case Operations.Int8Cast:
                case Operations.Int16Cast:
                case Operations.Int32Cast:
                case Operations.Int64Cast:
                case Operations.UInt8Cast:
                case Operations.UInt16Cast:
                case Operations.UInt32Cast:
                case Operations.UInt64Cast: return Tokens.T_INT_CAST;
                case Operations.DoubleCast:
                case Operations.DecimalCast:
                case Operations.FloatCast: return Tokens.T_DOUBLE_CAST;
                case Operations.BinaryCast:
                case Operations.UnicodeCast:
                case Operations.StringCast: return Tokens.T_STRING_CAST;
                case Operations.ObjectCast: return Tokens.T_OBJECT_CAST;
                case Operations.ArrayCast: return Tokens.T_ARRAY_CAST;
                case Operations.UnsetCast: return Tokens.T_UNSET_CAST;


                // binary:
                case Operations.Xor: return Tokens.T_LOGICAL_XOR;
                case Operations.Or: return Tokens.T_BOOLEAN_OR;
                case Operations.And: return Tokens.T_BOOLEAN_AND;
                case Operations.BitOr: return Tokens.T_PIPE;
                case Operations.BitXor: return Tokens.T_CARET;
                case Operations.BitAnd: return Tokens.T_AMP;
                case Operations.Equal: return Tokens.T_IS_EQUAL;
                case Operations.NotEqual: return Tokens.T_IS_NOT_EQUAL;
                case Operations.Identical: return Tokens.T_IS_IDENTICAL;
                case Operations.NotIdentical: return Tokens.T_IS_NOT_IDENTICAL;
                case Operations.LessThan: return Tokens.T_LT;
                case Operations.GreaterThan: return Tokens.T_GT;
                case Operations.LessThanOrEqual: return Tokens.T_IS_SMALLER_OR_EQUAL;
                case Operations.GreaterThanOrEqual: return Tokens.T_IS_GREATER_OR_EQUAL;
                case Operations.ShiftLeft: return Tokens.T_SL;
                case Operations.ShiftRight: return Tokens.T_SR;
                case Operations.Add: return Tokens.T_PLUS;
                case Operations.Sub: return Tokens.T_MINUS;
                case Operations.Mul: return Tokens.T_MUL;
                case Operations.Div: return Tokens.T_SLASH;
                case Operations.Mod: return Tokens.T_PERCENT;
                case Operations.Pow: return Tokens.T_POW;
                case Operations.Concat: return Tokens.T_DOT;
                case Operations.Spaceship: return Tokens.T_SPACESHIP;
                case Operations.Coalesce: return Tokens.T_COALESCE;

                // assignment
                case Operations.AssignAdd: return Tokens.T_PLUS_EQUAL;
                case Operations.AssignSub: return Tokens.T_MINUS_EQUAL;
                case Operations.AssignMul: return Tokens.T_MUL_EQUAL;
                case Operations.AssignPow: return Tokens.T_POW_EQUAL;
                case Operations.AssignDiv: return Tokens.T_DIV_EQUAL;
                case Operations.AssignMod: return Tokens.T_MOD_EQUAL;
                case Operations.AssignAnd: return Tokens.T_AND_EQUAL;
                case Operations.AssignOr: return Tokens.T_OR_EQUAL;
                case Operations.AssignXor: return Tokens.T_XOR_EQUAL;
                case Operations.AssignCoalesce: return Tokens.T_COALESCE_EQUAL;
                case Operations.AssignShiftLeft: return Tokens.T_SL_EQUAL;
                case Operations.AssignShiftRight: return Tokens.T_SR_EQUAL;
                case Operations.AssignAppend: return Tokens.T_CONCAT_EQUAL;

                case Operations.AssignValue:
                case Operations.AssignRef:
                    return Tokens.T_EQ;

                // constructs:
                case Operations.Array: return Tokens.T_ARRAY;
                case Operations.List: return Tokens.T_LIST;

                case Operations.Match: return Tokens.T_MATCH;

                //
                default:
                    throw new ArgumentException();
            }
        }
    }
}
