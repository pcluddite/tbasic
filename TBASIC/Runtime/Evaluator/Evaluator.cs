﻿/**
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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Tbasic.Runtime
{
    /// <summary>
    /// This class will evaluate boolean and mathmatical expressions
    /// </summary>
    internal class Evaluator : IExpression
    {
        #region Internal Structures and Classes

        /// <summary>
        /// Value and index of a Regex Match.
        /// </summary>
        internal class MatchInfo {

            public int Index { get; private set; }
            public string Value { get; set; }
            public int Length { get { return Value.Length; } }

            public MatchInfo(Match m) {
                Index = m.Index;
                Value = m.Value;
            }
        }

        #endregion

        #region Private Members

        private LinkedList<object> _expressionlist = new LinkedList<object>();
        private string _expression = "";
        private bool _bParsed;

        #endregion

        #region Construction

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Evaluator(Executer exec) {
            CurrentExecution = exec;
        }

        /// <summary>
        /// Constructor with string
        /// </summary>
        /// <param name="expression">string of the Expression to evaluate</param>
        /// <param name="exec">the current context</param>
        public Evaluator(string expression, Executer exec) {
            CurrentExecution = exec;
            Expression = expression;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the expression to be evaluated.
        /// </summary>
        public string Expression
        {
            get { return _expression; }
            set
            {
                _expression = value.Trim();
                _bParsed = false;
                _expressionlist.Clear();
            }
        }

        public ObjectContext CurrentContext {
            get {
                return CurrentExecution.Context;
            }
        }

        public Executer CurrentExecution { get;  set; }

        public bool Reparse {
            private get {
                return _bParsed;
            }
            set {
                if (_bParsed && value) {
                    _expressionlist.Clear();
                    _bParsed = !value;
                }
            }
        }

        #endregion

        #region Methods

        #region Evaluate
        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns>object of the expression return value</returns>
        public object Evaluate()
        {
            if ("" + Expression == "") {
                return 0;
            }

            return ExecuteEvaluation();
        }

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns>bool value of the evaluated expression</returns>
        public bool EvaluateBool() {
            return Convert.ToBoolean(Evaluate(), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns>integer value of the evaluated expression</returns>
        public int EvaluateInt()
        { return Convert.ToInt32(Evaluate(), CultureInfo.CurrentCulture); }

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns>double value of the evaluated expression</returns>
        public double EvaluateDouble()
        { return Convert.ToDouble(Evaluate(), CultureInfo.CurrentCulture); }

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns>decimal value of the evaluated expression</returns>
        public decimal EvaluateDecimal() { return Convert.ToDecimal(Evaluate(), CultureInfo.CurrentCulture); }

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns>long value of the evaluated expression</returns>
        public long EvaluateLong()
        { return Convert.ToInt64(Evaluate(), CultureInfo.CurrentCulture); }
        
        #endregion

        /// <summary>
        /// gets a string representation of this expression
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return Expression; }

        /// <summary>
        /// Sorts the mathmatical operations to be executed
        /// </summary>
        private object ExecuteEvaluation()
        {
            //Break Expression Apart into List
            if (!_bParsed) {
                for (int x = 0; x < Expression.Length; x = NextToken(x)) ;
            }
            _bParsed = true;

            //Perform Operations
            return EvaluateList();
        }

        /// <summary>
        /// This will search the expression for the next token (operand, operator, etc)
        /// </summary>
        /// <param name="nIdx">Start Position of Search</param>
        /// <returns>First character index after token.</returns>
        //[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private int NextToken(int nIdx) {
            MatchInfo mRet = null;
            int nRet = nIdx;
            object val = null;

            //Check for preceeding white space from last token index
            Match m = DefinedRegex.WhiteSpace.Match(Expression, nIdx);
            if (m.Success && m.Index == nIdx) {
                return nIdx + m.Length;
            }

            //Check Parenthesis
            m = DefinedRegex.Parenthesis.Match(Expression, nIdx);
            if (m.Success) {
                mRet = new MatchInfo(m);
            }
            

            //Check String
            m = DefinedRegex.String.Match(Expression, nIdx);
            if (m.Success && (mRet == null || m.Index < mRet.Index)) {
                mRet = new MatchInfo(m);
                string str_parsed;
                ReadString(m.Value, 0, out str_parsed);
                val = str_parsed;
            }

            //Check Function
            if (mRet == null || mRet.Index > nIdx) {
                m = DefinedRegex.Function.Match(Expression, nIdx);
                if (m.Success && (mRet == null || m.Index < mRet.Index)) {
                    mRet = new MatchInfo(m);
                    Function func = new Function(
                        Expression.Substring(mRet.Index, mRet.Length),
                        CurrentExecution // share the wealth
                    );
                    func.Parse();
                    mRet.Value = func.Expression;
                    val = func;
                }
            }

            //Check null
            if (mRet == null || mRet.Index > nIdx) {
                m = DefinedRegex.Null.Match(Expression, nIdx);
                if (m.Success && (mRet == null || m.Index < mRet.Index)) {
                    mRet = new MatchInfo(m);
                }
            }

            //Check Variable
            if (mRet == null || mRet.Index > nIdx) {
                m = DefinedRegex.Variable.Match(Expression, nIdx);
                if (m.Success && (mRet == null || m.Index < mRet.Index)) {
                    mRet = new MatchInfo(m);
                    Variable v = new Variable(m.Value, CurrentExecution);
                    mRet.Value = v.Expression;
                    val = v;
                }
            }

            //Check Unary Operator
            if (mRet == null || mRet.Index > nIdx) {
                m = DefinedRegex.UnaryOp.Match(Expression, nIdx);
                if (m.Success && (mRet == null || m.Index < mRet.Index)) {
                    mRet = new MatchInfo(m);
                    val = new UnaryOperator(m.Value);
                }
            }

            //Check Hexadecimal
            if (mRet == null || mRet.Index > nIdx) {
                m = DefinedRegex.Hexadecimal.Match(Expression, nIdx);
                if (m.Success && (mRet == null || m.Index < mRet.Index)) {
                    mRet = new MatchInfo(m);
                    val = Convert.ToInt32(m.Value, 16);
                }
            }

            //Check Boolean
            if (mRet == null || mRet.Index > nIdx) {
                m = DefinedRegex.Boolean.Match(Expression, nIdx);
                if (m.Success && (mRet == null || m.Index < mRet.Index)) {
                    mRet = new MatchInfo(m); 
                    val = bool.Parse(m.Value);
                }
            }

            //Check Numeric
            if (mRet == null || mRet.Index > nIdx) {
                m = DefinedRegex.Numeric.Match(Expression, nIdx);
                if (m.Success && (mRet == null || m.Index < mRet.Index)) {
                    while (m.Success && ("" + m.Value == "")) {
                        m = m.NextMatch();
                    }
                    if (m.Success) {
                        mRet = new MatchInfo(m);
                        val = Variable.DuckType(double.Parse(m.Value, CultureInfo.CurrentCulture));
                    }
                }
            }

            //Check Binary Operator
            if (mRet == null || mRet.Index > nIdx) {
                m = DefinedRegex.BinaryOp.Match(Expression, nIdx);
                if (m.Success && (mRet == null || m.Index < mRet.Index)) { 
                    mRet = new MatchInfo(m);
                    val = new BinaryOperator(m.Value); 
                }
            }

            if (mRet == null) {
                throw new ArgumentException("invalid expression: '" + Expression + "'");
            }

            if (mRet.Index != nIdx) {
                throw new ArgumentException(
                    "invalid token in expression: '" + Expression.Substring(nIdx, mRet.Index - nIdx).Trim() + "'"
                );
            }

            if (mRet.Value == "(") {

                nRet = IndexGroup(Expression, mRet.Index) + 1;

                Evaluator expr = new Evaluator(
                    Expression.Substring(mRet.Index + 1, nRet - mRet.Index - 2),
                    CurrentExecution // share the wealth
                );
                _expressionlist.AddLast(expr);
            }
            else {
                nRet = mRet.Index + mRet.Length;
                _expressionlist.AddLast(val);
            }

            return nRet;
        }

        /// <summary>
        /// Traverses the list to perform operations on items according to operator precedence
        /// </summary>
        /// <returns>final evaluated expression of Expression string</returns>
        private object EvaluateList()
        {
            LinkedList<object> list = new LinkedList<object>(_expressionlist);

            //Do the unary operators first

            LinkedListNode<object> x = list.First;
            for (; x != null; x = x.Next) {
                UnaryOperator op = x.Value as UnaryOperator;
                if (op != null) {
                    x.Value = PerformUnaryOp(op, x.Next.Value);
                    list.Remove(x.Next);
                }
            }
            
            //Get the queued binary operations
            BinaryOpQueue opqueue = new BinaryOpQueue(list);

            StringBuilder sb = new StringBuilder();
            x = list.First.Next; // second element
            for (; x != null; x = x.Next.Next) {
                BinaryOperator op = x.Value as BinaryOperator;
                if (op == null) {
                    sb.AppendFormat(
                        "\n{0} [?] {1}",
                        (x.Previous.Value is string) ? "\"" + x.Previous.Value + "\"" : x.Previous.Value,
                        (x.Value is string) ? "\"" + x.Value + "\"" : x.Value
                    );
                    x = x.Previous;
                }
                else {
                    if (x.Next == null) {
                        throw new ArgumentException(
                            "expression cannot end in a binary operation: [" + x.Value + "]"
                        );
                    }
                }
            }
            if (sb.Length > 0) {
                throw new ArgumentException("missing binary operator: " + sb);
            }

            var nodePair = opqueue.Dequeue();
            while (nodePair != null) {
                nodePair.Node.Previous.Value = PerformBinaryOp(
                    nodePair.Operator,
                    nodePair.Node.Previous.Value,
                    nodePair.Node.Next.Value
                    );
                list.Remove(nodePair.Node.Next);
                list.Remove(nodePair.Node);
                nodePair = opqueue.Dequeue();
            }
            IExpression expr = list.First.Value as IExpression;
            if (expr == null) {
                return list.First.Value;
            }
            else {
                return expr.Evaluate();
            }
        }

        #endregion

        #region static methods
        
        /// <summary>
        /// Static version of the Expression Evaluator
        /// </summary>
        /// <param name="expressionString">expression to be evaluated</param>
        /// <param name="exec">the current execution</param>
        /// <returns></returns>
        public static object Evaluate(string expressionString, Executer exec) {
            Evaluator expression = new Evaluator(expressionString, exec);
            return expression.Evaluate();
        }

        /// <summary>
        /// This routine will actually execute an operation and return its value
        /// </summary>
        /// <param name="op">Operator Information</param>
        /// <param name="v1">left operand</param>
        /// <param name="v2">right operand</param>
        /// <returns>v1 (op) v2</returns>
        private static object PerformBinaryOp(BinaryOperator op, object v1, object v2) {
            IExpression tv = v1 as IExpression;
            if (tv != null) {
                v1 = tv.Evaluate();
            }

            switch (op.OperatorString) { // short circuit evaluation 1/6/16
                case "AND":
                    if (Convert.ToBoolean(v1, CultureInfo.CurrentCulture)) {
                        tv = v2 as IExpression;
                        if (tv != null) {
                            v2 = tv.Evaluate();
                        }
                        if (Convert.ToBoolean(v2, CultureInfo.CurrentCulture)) {
                            return true;
                        }
                    }
                    return false;
                case "OR":
                    if (Convert.ToBoolean(v1, CultureInfo.CurrentCulture)) {
                        return true;
                    }
                    else {
                        tv = v2 as IExpression;
                        if (tv != null) {
                            v2 = tv.Evaluate();
                        }
                        if (Convert.ToBoolean(v2, CultureInfo.CurrentCulture)) {
                            return true;
                        }
                    }
                    return false;
            }

            tv = v2 as IExpression;
            if (tv != null) {
                v2 = tv.Evaluate();
            }

            switch (op.OperatorString) {
                case "*": return (Convert.ToDouble(v1, CultureInfo.CurrentCulture) *
                                  Convert.ToDouble(v2, CultureInfo.CurrentCulture));
                case "/": {
                    double d1 = Convert.ToDouble(v1, CultureInfo.CurrentCulture);
                    double d2 = Convert.ToDouble(v2, CultureInfo.CurrentCulture);
                    if (d2 == 0) {
                        throw new DivideByZeroException();
                    }
                    return d1 / d2;
                }
                case "MOD": return (Convert.ToInt64(v1, CultureInfo.CurrentCulture) %
                                  Convert.ToInt64(v2, CultureInfo.CurrentCulture));
                case "<<": return (Convert.ToInt64(v1, CultureInfo.CurrentCulture) <<
                                   Convert.ToInt32(v2, CultureInfo.CurrentCulture));
                case ">>": return (Convert.ToInt64(v1, CultureInfo.CurrentCulture) >>
                                   Convert.ToInt32(v2, CultureInfo.CurrentCulture));
                case "+":
                case "-":
                case "<":
                case "<=":
                case "=<":
                case ">":
                case ">=":
                case "=>":
                case "==":
                case "=":
                case "<>":
                case "!=": return DoSpecialOperator(op, v1, v2);
                case "&": return (Convert.ToUInt64(v1, CultureInfo.CurrentCulture) &
                                  Convert.ToUInt64(v2, CultureInfo.CurrentCulture));
                case "^": return (Convert.ToUInt64(v1, CultureInfo.CurrentCulture) ^
                                  Convert.ToUInt64(v2, CultureInfo.CurrentCulture));
                case "|": return (Convert.ToUInt64(v1, CultureInfo.CurrentCulture) |
                                  Convert.ToUInt64(v2, CultureInfo.CurrentCulture));
            }
            throw new ArgumentException("binary operator " + op.OperatorString + " not defined.");
        }

        private static object DoSpecialOperator(BinaryOperator op, object v1, object v2) {
            string str1 = v1 as string,
                   str2 = v2 as string;
            if (str1 == null && str2 == null) {
                try {
                    double f1 = Convert.ToDouble(v1, CultureInfo.CurrentCulture),
                           f2 = Convert.ToDouble(v2, CultureInfo.CurrentCulture);
                    switch (op.OperatorString) {
                        case "+": return f1 + f2;
                        case "-": return f1 - f2;
                        case "<": return f1 < f2;
                        case "=<":
                        case "<=": return f1 <= f2;
                        case ">": return f1 > f2;
                        case "=>":
                        case ">=": return f1 >= f2;
                        case "==":
                        case "=": return f1 == f2;
                        case "<>":
                        case "!=": return f1 != f2;
                    }
                }
                catch (InvalidCastException) {
                }
            }
            else {
                if (str1 == null) {
                    str1 = ConvertToString(v1).ToString();
                }
                if (str2 == null) {
                    str2 = ConvertToString(v2).ToString();
                }
                switch (op.OperatorString) {
                    case "+":
                        return str1 + str2;
                    case "==":
                        return str1 == str2;
                    case "=":
                        return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase); // = is case insensitive 1/2/16
                    case "<>":
                    case "!=":
                        return str1 != str2;
                }
            }
            throw new FormatException(
                string.Format(
                "operator '{0}' cannot be applied to objects of type '{1}' and '{2}'",
                op.OperatorString, GetTypeName(v1.GetType()), GetTypeName(v2.GetType())
                ));
        }

        private static object ConvertToString(object obj) {
            if (obj == null) {
                return "";
            }
            else if (obj is string) {
                return FormatString(obj);
            }
            else if (obj.GetType().IsArray) {
                StringBuilder sb = new StringBuilder("{ ");
                object[] _aObj = (object[])obj;
                if (_aObj.Length > 0) {
                    for (int i = 0; i < _aObj.Length - 1; i++) {
                        sb.AppendFormat("{0}, ", ConvertToString(_aObj[i]));
                    }
                    sb.AppendFormat("{0} ", ConvertToString(_aObj[_aObj.Length - 1]));
                }
                sb.Append("}");
                return sb.ToString();
            }
            return obj;
        }

        private static object FormatString(object o) {
            string str = o as string;
            if (str == null) {
                return o;
            }
            else {
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < str.Length; index++) {
                    char c = str[index];
                    switch (c) {
                         case '\n': sb.Append("\\n"); break;
                         case '\r': sb.Append("\\r"); break;
                         case '\\': sb.Append("\\\\"); break;
                         case '\b': sb.Append("\\b"); break;
                         case '\t': sb.Append("\\t"); break;
                         case '\f': sb.Append("\\f"); break;
                         case '\"': sb.Append("\\\""); break;
                         case '\'': sb.Append("\\'"); break;
                        default:
                             if (c < ' ') {
                                 sb.Append("\\u");
                                 sb.Append(Convert.ToString((int)c, 16).PadLeft(4, '0'));
                             }
                             else {
                                 sb.Append(c);
                             }
                             break;
                    }
                }
                return "\"" + sb + "\"";
            }
        }

        private static string GetTypeName(Type t) {
            if (t.IsArray) {
                return "object array";
            }
            else {
                return t.Name.ToLower();
            }
        }

        private static object PerformUnaryOp(UnaryOperator op, object v) {
            IExpression tempv = v as IExpression;
            if (tempv != null) {
                v = tempv.Evaluate();
            }

            switch (op.OperatorString.ToUpper()) {
                case "+": return (Convert.ToDouble(v, CultureInfo.CurrentCulture));
                case "-": return (-Convert.ToDouble(v, CultureInfo.CurrentCulture));
                case "NOT ": return (!Convert.ToBoolean(v, CultureInfo.CurrentCulture));
                case "~": return (~Convert.ToUInt64(v, CultureInfo.CurrentCulture));
            }
            throw new ArgumentException("unary operator '" + op.OperatorString + "' not defined.");
        }

        /// <summary>
        /// Indexes a string without parsing it (i.e. retreives the index of the terminating quote)
        /// </summary>
        /// <param name="str_full">the full string containing the string to be checked</param>
        /// <param name="index">the index in the full string of the first quote of the string to be indexed</param>
        /// <returns>the index of the terminating quote  in the full string</returns>
        internal static int IndexString(string str_full, int index) {
            char quote = str_full[index]; // The first character should be the quote

            index++; // We used the first character

            for (; index < str_full.Length; index++) {
                char cur = str_full[index];
                switch (cur) {
                    case '\n':
                    case '\r':
                        throw new FormatException("unterminated string");
                    case '\\':
                        index++;
                        cur = str_full[index];
                        if (index >= str_full.Length) {
                            throw new FormatException("unterminated escape sequence");
                        }
                        switch (cur) {
                            case 'b':
                            case 't':
                            case 'n':
                            case 'f':
                            case 'r':
                            case '"':
                            case '\\':
                            case '\'': break; // you're golden
                            case 'u':
                                index += 4;
                                if (index >= str_full.Length) {
                                    throw new FormatException("Unterminated escape sequence. Expected four digit hex to follow '\\u'.");
                                }
                                break;
                            default:
                                throw new FormatException("unknown escape sequence: \\" + cur);
                        }
                        break;
                    default:
                        if (cur == quote) { // We be dun
                            return index;
                        }
                        break;
                }
            }
            throw new FormatException("unterminated string");
        }

        /// <summary>
        /// Indexes and builds a string, parsing any escape sequences properly
        /// </summary>
        /// <param name="str_full">the full string containing the string to be parsed</param>
        /// <param name="index">the index in the full string of the first quote of the string to be indexed</param>
        /// <param name="s_parsed">the parsed string (without quotes and escape sequences replaced)</param>
        /// <returns>the index of the terminating quote  in the full string</returns>
        internal static int ReadString(string str_full, int index, out string s_parsed) {
            char quote = str_full[index]; // The first character should be the quote

            index++; // We used the first character

            StringBuilder sb = new StringBuilder();
            for (; index < str_full.Length; index++) {
                char cur = str_full[index];
                switch (cur) {
                    case '\n':
                    case '\r':
                        throw new FormatException("unterminated string");
                    case '\\':
                        index++;
                        if (index >= str_full.Length) {
                            throw new FormatException("unterminated escape sequence");
                        }
                        cur = str_full[index];
                        switch (cur) {
                            case 'b': sb.Append('\b'); break;
                            case 't': sb.Append('\t'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'r': sb.Append('\r'); break;
                            case '\\': sb.Append('\\'); break;
                            case '"': sb.Append('"'); break;
                            case '\'': sb.Append('\''); break;
                            case 'u':
                                index += 4;
                                if (index >= str_full.Length) {
                                    throw new FormatException("Unterminated escape sequence. Expected four digit hex to follow '\\u'.");
                                }
                                sb.Append((char)ushort.Parse(str_full.Substring(index - 3, 4), NumberStyles.HexNumber));
                                break;
                            default:
                                throw new FormatException("unknown escape sequence: \\" + cur);
                        }
                        break;
                    default:
                        if (cur == quote) { // We be dun
                            s_parsed = sb.ToString();
                            return index;
                        }
                        else {
                            sb.Append(cur);
                        }
                        break;
                }
            }
            throw new FormatException("unterminated string");
        }

        /// <summary>
        /// Indexes a group without evaluating its operators
        /// </summary>
        /// <param name="s_full">the full string containing the group to be indexed</param>
        /// <param name="firstIndex">the index in the full string of the character of the group to be indexed</param>
        /// <returns>the index of the terminating grouping character in the full string</returns>
        internal static int IndexGroup(string s_full, int firstIndex) {
            char c_open = s_full[firstIndex]; // The first character should be the grouping character (i.e. '(' or '[')
            char c_close = c_open == '(' ? ')' : ']';

            int expected = 1; // We are expecting one closing character
            int c_index = firstIndex + 1; // We used the first character
            for (; c_index < s_full.Length; c_index++) {
                char cur = s_full[c_index];
                switch (cur) {
                    case ' ': // ignore spaces
                        continue;
                    case '\'':
                    case '\"': {
                            c_index = IndexString(s_full, c_index);
                        }
                        break;
                    case '(':
                    case '[':
                        if (cur == c_open) {
                            expected++;
                        }
                        else {
                            c_index = IndexGroup(s_full, c_index);
                        }
                        break;
                    default:
                        if (cur == c_close) {
                            expected--;
                        }
                        break;
                }
                if (expected == 0) {
                    return c_index;
                }
            }

            throw new FormatException("unterminated group");
        }

        internal static int ReadGroup(string s_full, int c_index, Executer _curExec, out IList<object> _oParams) {
            List<object> result = new List<object>();
            char c_open = s_full[c_index];
            char c_close = c_open == '(' ? ')' : ']';
            int expected = 0;
            int last = c_index;

            for (; c_index < s_full.Length; c_index++) {
                char cur = s_full[c_index];
                switch (cur) {
                    case ' ': // ignore spaces
                        continue;
                    case '\'':
                    case '\"': {
                            c_index = IndexString(s_full, c_index);
                        }
                        break;
                    case '(':
                    case '[':
                        if (cur == c_open) {
                            expected++;
                        }
                        else {
                            c_index = IndexGroup(s_full, c_index);
                        }
                        break;
                    default:
                        if (cur == c_close) {
                            expected--;
                        }
                        break;
                }

                if ((expected == 1 && cur == ',') // The commas in between other parentheses are not ours.
                    || expected == 0) {
                    string _param = s_full.Substring(last + 1, c_index - last - 1).Trim();
                    if (!_param.Equals("")) {
                        Evaluator expr = new Evaluator(_param, _curExec);
                        result.Add(expr.Evaluate());
                    }
                    last = c_index;
                    if (expected == 0) { // fin
                        _oParams = result;
                        return last;
                    }
                }
            }

            throw new FormatException("unterminated group");
        }

        internal static bool TryParse<T>(object input, out T result) {
            try {
                result = (T)input;
                return true;
            }
            catch (InvalidCastException) {
            }
            IConvertible convertible = input as IConvertible;
            if (convertible == null) {
                result = default(T);
                return false;
            }
            else {
                try {
                    result = (T)convertible.ToType(typeof(T), CultureInfo.CurrentCulture);
                    return true;
                }
                catch {
                    result = default(T);
                    return false;
                }
            }
        }

        #endregion
    }
}