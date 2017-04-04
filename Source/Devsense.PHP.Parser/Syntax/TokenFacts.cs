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

                case Tokens.T_LOGICAL_XOR: return "XOR";
                case Tokens.T_LOGICAL_OR: return "OR";
                case Tokens.T_LOGICAL_AND: return "AND";
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

                case Tokens.T_GLOBAL: return "global";
                case Tokens.T_STATIC: return "static";
                case Tokens.T_FINAL: return "final";
                case Tokens.T_FUNCTION: return "function";
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

                case Tokens.T_HALT_COMPILER: return "__halt_compiler";

                default: throw new ArgumentException();
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

                default:
                    throw new ArgumentException();
            }
        }
    }
}
