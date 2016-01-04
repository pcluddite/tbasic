/**
 *  TBASIC 2.0
 *  Copyright (C) 2016 Timothy Baxendale
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

namespace Tbasic {

    internal class ScriptException : Exception {
        public int Line { get; private set; }
        public string Name { get; private set; }
        public ScriptException(int line, string name, Exception e)
            : base(GetMessage(e), e) {
            Line = line;
            Name = name;
        }

        private static string GetMessage(Exception e) {
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

        public static NullReferenceException UndefinedObject(string name) {
            return new NullReferenceException("'" + name + "' does not exist in the current context");
        }

        public static AccessViolationException AlreadyDefined(string name, string type, string newType) {
            return new AccessViolationException(
                string.Format("an object '{0}' has been defined as a {1} and cannot be redefined as a {2}", name, type, newType));
        }

        public static AccessViolationException ConstantChange() {
            return new AccessViolationException("cannot redefine a constant");
        }

        public static ObjectDisposedException ContextCleared() {
            return new ObjectDisposedException("ObjectContext", "context fell out of scope and was disposed");
        }

        public static ArgumentException InvalidOperator(char opr) {
            return new ArgumentException(string.Format("invalid operator '{0}'", opr));
        }

        public static ArgumentException InvalidVariableName(string name) {
            return new ArgumentException(string.Format("cannot define variable '{0}'. Name is invalid.", name));
        }

        public static ArgumentException InvalidOperatorDeclaration(object opr) {
            return new ArgumentException(string.Format("invalid operator in declaration '{0}', expected '='", opr));
        }

        public static ArgumentException NoOpeningStatement(string str) {
            return new ArgumentException(string.Format("cannot find opening statement for '{0}'", str));
        }

        public static ArgumentException NoCondition() {
            return new ArgumentException("expected condition");
        }

        public static EndOfCodeException UnterminatedBlock(int line, string name) {
            return new EndOfCodeException(line, "unterminated '" + name + "' block");
        }

        public static ArgumentException InvalidExpression(string expr, string expected) {
            return new ArgumentException(string.Format("invalid expression '{0}': '{1}' expected", expr, expected));
        }

        public static FormatException NoIndexSpecified() {
            return new FormatException("at least one index was expected between braces");
        }

        public static FormatException IndexUnavailable(string _sName) {
            return new FormatException(string.Format("object '{0}' cannot be indexed", _sName));
        }

        public static IndexOutOfRangeException IndexOutOfRange(string _sName, int index) {
            return new IndexOutOfRangeException(string.Format("index {0} of object {1} is out of range", index, _sName));
        }
    }
}
