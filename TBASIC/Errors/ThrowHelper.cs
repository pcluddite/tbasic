using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Errors
{
    internal class ThrowHelper
    {
        public static Exception UndefinedObject(string name)
        {
            return new KeyNotFoundException("'" + name + "' does not exist in the current context");
        }

        public static Exception AlreadyDefined(string name, string type, string newType)
        {
            return new InvalidCastException(string.Format("An object '{0}' has been defined as a {1} and cannot be redefined as a {2}", name, type, newType));
        }

        public static Exception ConstantChange()
        {
            return new InvalidOperationException("Cannot redefine a constant");
        }

        public static Exception ContextCleared()
        {
            return new InvalidOperationException("Context fell out of scope and was disposed");
        }

        public static Exception InvalidOperator(char opr)
        {
            return new InvalidOperatorException(opr.ToString());
        }

        public static Exception InvalidVariableName(string name)
        {
            return new ScriptParsingException(string.Format("Cannot define variable with name '{0}'", name));
        }

        public static Exception InvalidOperatorDeclaration(object opr)
        {
            return new InvalidOperatorException(string.Format("Invalid operator in declaration '{0}', expected '='", opr), prependGeneric: false);
        }

        public static Exception ArraysCannotBeConstant()
        {
            return new ScriptParsingException("Arrays cannot be defined as constants");
        }

        public static Exception ExpectedToken(string token)
        {
            return new ExpectedTokenExceptiopn(token);
        }

        public static Exception NoOpeningStatement(string str)
        {
            return new ScriptParsingException(string.Format("Cannot find opening statement for '{0}'", str));
        }

        public static Exception NoCondition()
        {
            return new ScriptParsingException("Expected condition");
        }

        public static Exception UnterminatedBlock(string name)
        {
            return new EndOfCodeException("Unterminated '" + name + "' block");
        }

        public static Exception InvalidExpression(string expr, string expected)
        {
            return new ScriptParsingException(string.Format("Invalid expression '{0}': '{1}' expected", expr, expected));
        }

        public static Exception NoIndexSpecified()
        {
            return new FormatException("At least one index was expected between braces");
        }

        public static Exception IndexUnavailable(string _sName)
        {
            return new InvalidOperationException(string.Format("Object '{0}' cannot be indexed", _sName));
        }

        public static Exception IndexOutOfRange(string _sName, int index)
        {
            return new UnauthorizedAccessException(string.Format("Index '{0}' of object '{1}' is out of range", index, _sName));
        }

        public static Exception InvalidExpression(string expr)
        {
            return new ScriptParsingException("Invalid expression: '" + expr + "'");
        }
    }
}
