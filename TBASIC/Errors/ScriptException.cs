/**
 *  TBASIC
 *  Copyright (C) 2013-2016 Timothy Baxendale
 *  
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *  
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *  
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 *  USA
 **/
using System;
using System.Text;

namespace Tbasic
{
    /// <summary>
    /// Represents a generic scripting error
    /// </summary>
    public class ScriptException : Exception
    {
        /// <summary>
        /// The line at which the error occoured
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// The function or command that caused the error
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Constructs a new ScriptException
        /// </summary>
        /// <param name="line">the line number at which the error occoured</param>
        /// <param name="name">the function or command that caused the error</param>
        /// <param name="e">the exception that occoured</param>
        public ScriptException(int line, string name, Exception e)
            : base(string.Format("An error occoured at '{0}' on line {1}", name, line) + GetMessage(e), e)
        {
            Line = line;
            Name = name;
        }
        
        private static string GetMessage(Exception e)
        {
            StringBuilder msg = new StringBuilder();
            while (e is ScriptException) {
                ScriptException ex = (ScriptException)e;
                msg.AppendFormat("\tat '{0}' on line {1}\n", ex.Name, ex.Line);
                e = ex.InnerException;
            }
            msg.Append("\nDetail:\n");
            msg.AppendFormat("{0}", e.Message);
            return msg.ToString();
        }

        internal static NullReferenceException UndefinedObject(string name)
        {
            return new NullReferenceException("'" + name + "' does not exist in the current context");
        }

        internal static AccessViolationException AlreadyDefined(string name, string type, string newType)
        {
            return new AccessViolationException(
                string.Format("an object '{0}' has been defined as a {1} and cannot be redefined as a {2}", name, type, newType));
        }

        internal static AccessViolationException ConstantChange()
        {
            return new AccessViolationException("cannot redefine a constant");
        }

        internal static ObjectDisposedException ContextCleared()
        {
            return new ObjectDisposedException("ObjectContext", "context fell out of scope and was disposed");
        }

        internal static ArgumentException InvalidOperator(char opr)
        {
            return new ArgumentException(string.Format("invalid operator '{0}'", opr));
        }

        internal static ArgumentException InvalidVariableName(string name)
        {
            return new ArgumentException(string.Format("cannot define variable '{0}'. Name is invalid.", name));
        }

        internal static ArgumentException InvalidOperatorDeclaration(object opr)
        {
            return new ArgumentException(string.Format("invalid operator in declaration '{0}', expected '='", opr));
        }

        internal static ArgumentException NoOpeningStatement(string str)
        {
            return new ArgumentException(string.Format("cannot find opening statement for '{0}'", str));
        }

        internal static ArgumentException NoCondition()
        {
            return new ArgumentException("expected condition");
        }

        internal static EndOfCodeException UnterminatedBlock(int line, string name)
        {
            return new EndOfCodeException(line, "unterminated '" + name + "' block");
        }

        internal static ArgumentException InvalidExpression(string expr, string expected)
        {
            return new ArgumentException(string.Format("invalid expression '{0}': '{1}' expected", expr, expected));
        }

        internal static FormatException NoIndexSpecified()
        {
            return new FormatException("at least one index was expected between braces");
        }

        internal static FormatException IndexUnavailable(string _sName)
        {
            return new FormatException(string.Format("object '{0}' cannot be indexed", _sName));
        }

        internal static IndexOutOfRangeException IndexOutOfRange(string _sName, int index)
        {
            return new IndexOutOfRangeException(string.Format("index {0} of object {1} is out of range", index, _sName));
        }
    }
}