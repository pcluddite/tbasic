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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Tbasic.Parsing;
using Tbasic.Components;
using Tbasic.Operators;

namespace Tbasic.Runtime
{
    /// <summary>
    /// This class will evaluate boolean and mathmatical expressions
    /// </summary>
    internal partial class Evaluator : IExpression
    {

        #region Private Members

        private LinkedList<object> _expressionlist = new LinkedList<object>();
        private StringSegment _expression = StringSegment.Empty;
        private bool _bParsed;

        #endregion

        #region Construction

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Evaluator(Executer exec)
        {
            CurrentExecution = exec;
        }

        /// <summary>
        /// Constructor with string
        /// </summary>
        /// <param name="expression">string of the Expression to evaluate</param>
        /// <param name="exec">the current context</param>
        public Evaluator(StringSegment expression, Executer exec)
        {
            CurrentExecution = exec;
            Expression = expression;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the expression to be evaluated. This value is trimmed.
        /// </summary>
        public StringSegment Expression
        {
            get { return _expression; }
            set {
                _expression = value.Trim();
                _bParsed = false;
                _expressionlist.Clear();
            }
        }

        public ObjectContext CurrentContext
        {
            get {
                return CurrentExecution.Context;
            }
        }

        public Executer CurrentExecution { get; set; }

        public bool Reparse
        {
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
            if (StringSegment.IsNullOrEmpty(Expression)) {
                return 0;
            }

            return Variable.ConvertToObject(ExecuteEvaluation());
        }

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns>bool value of the evaluated expression</returns>
        public bool EvaluateBool()
        {
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
        public override string ToString() { return Expression.ToString(); }

        /// <summary>
        /// Sorts the mathmatical operations to be executed
        /// </summary>
        private object ExecuteEvaluation()
        {
            //Break Expression Apart into List
            if (!_bParsed) {
                Scanner scanner = new Scanner(_expression);
                while (!scanner.EndOfStream)
                    NextToken(scanner);
            }
            _bParsed = true;

            //Perform Operations
            return EvaluateList();
        }

        /// <summary>
        /// This will search the expression for the next token (operand, operator, etc)
        /// </summary>
        /// <param name="scanner"></param>
        /// <returns>First character index after token.</returns>
        private int NextToken(Scanner scanner)
        {
            //Check for preceeding white space from last token index
            scanner.SkipWhiteSpace();

            int startIndex = scanner.IntPosition;

            //Check Parenthesis
            if (scanner.Next("(")) {
                return AddObjectToExprList("(", startIndex, scanner);
            }

            //Check String
            string str_parsed;
            if (scanner.NextString(out str_parsed)) {
                return AddObjectToExprList(str_parsed, startIndex, scanner);
            }

            //Check Unary Operator
            UnaryOperator unaryOp;
            if (scanner.NextUnaryOp(CurrentContext._unaryOps, _expressionlist.Last?.Value, out unaryOp)) {
                return AddObjectToExprList(unaryOp, startIndex, scanner);
            }

            //Check Function
            Function func;
            if (scanner.NextFunction(CurrentExecution, out func)) {
                return AddObjectToExprList(func, startIndex, scanner);
            }

            //Check null
            if (scanner.Next("null")) {
                return AddObjectToExprList("null", startIndex, scanner);
            }

            //Check Variable
            Variable variable;
            if (scanner.NextVariable(CurrentExecution, out variable)) {
                return AddObjectToExprList(variable, startIndex, scanner);
            }

            //Check Hexadecimal
            int hex;
            if (scanner.NextHexadecimal(out hex)) {
                return AddObjectToExprList(hex, startIndex, scanner);
            }

            //Check Boolean
            bool b;
            if (scanner.NextBool(out b)) {
                return AddObjectToExprList(b, startIndex, scanner);
            }

            //Check Numeric
            Number num;
            if (scanner.NextPositiveNumber(out num)) {
                return AddObjectToExprList(num.ToObject(), startIndex, scanner);
            }

            //Check Binary Operator
            BinaryOperator binOp;
            if (scanner.NextBinaryOp(CurrentContext._binaryOps, out binOp)) {
                return AddObjectToExprList(binOp, startIndex, scanner);
            }

            // Couldn't be parsed

            if (CurrentContext.FindFunctionContext(_expression.ToString()) == null) {
                throw new ArgumentException("Invalid expression '" + _expression + "'");
            }
            else {
                throw new FormatException("Poorly formed function call");
            }

            /*if (mRet.Index != nIdx) {
                throw new ArgumentException(
                    "Invalid token in expression '" + expr.Substring(nIdx, mRet.Index - nIdx).Trim() + "'"
                );
            }*/
        }

        private int AddObjectToExprList(object val, int startIndex, Scanner scanner)
        {
            if (Equals(val, "(")) {

                scanner.IntPosition = GroupParser.IndexGroup(_expression, startIndex) + 1;

                Evaluator eval = new Evaluator(
                    _expression.Subsegment(startIndex + 1, scanner.IntPosition - startIndex - 2),
                    CurrentExecution // share the wealth
                );
                _expressionlist.AddLast(eval);
            }
            else {
                _expressionlist.AddLast(val);
            }

            return scanner.IntPosition;
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
                UnaryOperator? op = x.Value as UnaryOperator?;
                if (op != null) {
                    x.Value = PerformUnaryOp(op.Value, x.Previous?.Value, x.Next.Value);
                    list.Remove(x.Next);
                }
            }

            //Get the queued binary operations
            BinaryOpQueue opqueue = new BinaryOpQueue(list);

            StringBuilder sb = new StringBuilder();
            x = list.First.Next; // second element
            for (; x != null; x = x.Next.Next) {
                BinaryOperator? op = x.Value as BinaryOperator?;
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
                            "Expression cannot end in a binary operation: [" + x.Value + "]"
                        );
                    }
                }
            }
            if (sb.Length > 0) {
                throw new ArgumentException("Missing binary operator: " + sb);
            }

            BinOpNodePair nodePair;
            while (opqueue.Dequeue(out nodePair)) {
                nodePair.Node.Previous.Value = PerformBinaryOp(
                    nodePair.Operator,
                    nodePair.Node.Previous.Value,
                    nodePair.Node.Next.Value
                    );
                list.Remove(nodePair.Node.Next);
                list.Remove(nodePair.Node);
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
        public static object Evaluate(StringSegment expressionString, Executer exec)
        {
            Evaluator expression = new Evaluator(expressionString, exec);
            return expression.Evaluate();
        }

        internal static bool TryParse<T>(object input, out T result)
        {
            try {
                result = (T)input;
                return true;
            }
            catch (InvalidCastException) {
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
        }

        public static object PerformUnaryOp(UnaryOperator op, object left, object right)
        {
            object operand = op.Side == UnaryOperator.OperandSide.Left ? left : right;
            IExpression tempv = operand as IExpression;
            if (tempv != null) {
                operand = tempv.Evaluate();
            }

            try {
                return op.ExecuteOperator(operand);
            }
            catch(InvalidCastException) when (operand is IOperator) {
                throw new ArgumentException("Unary operand cannot be " + operand.GetType().Name);
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is ArgumentException || ex is OverflowException) {
                throw new ArgumentException("Unary operator '" + op.OperatorString + "' not defined.");
            }
        }

        /// <summary>
        /// This routine will actually execute an operation and return its value
        /// </summary>
        /// <param name="op">Operator Information</param>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>v1 (op) v2</returns>
        public static object PerformBinaryOp(BinaryOperator op, object left, object right)
        {
            IExpression tv = left as IExpression;
            if (tv != null) {
                left = tv.Evaluate();
            }

            try {
                switch (op.OperatorString) { // short circuit evaluation 1/6/16
                    case "AND":
                        if (Convert.ToBoolean(left, CultureInfo.CurrentCulture)) {
                            tv = right as IExpression;
                            if (tv != null) {
                                right = tv.Evaluate();
                            }
                            if (Convert.ToBoolean(right, CultureInfo.CurrentCulture)) {
                                return true;
                            }
                        }
                        return false;
                    case "OR":
                        if (Convert.ToBoolean(left, CultureInfo.CurrentCulture)) {
                            return true;
                        }
                        else {
                            tv = right as IExpression;
                            if (tv != null) {
                                right = tv.Evaluate();
                            }
                            if (Convert.ToBoolean(right, CultureInfo.CurrentCulture)) {
                                return true;
                            }
                        }
                        return false;
                }

                tv = right as IExpression;
                if (tv != null) {
                    right = tv.Evaluate();
                }
                return op.ExecuteOperator(left, right);
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is ArgumentException || ex is OverflowException) {
                throw new FormatException(string.Format(
                        "Operator '{0}' cannot be applied to objects of type '{1}' and '{2}'",
                        op.OperatorString, GetTypeName(right), GetTypeName(left)
                    ));
            }
        }

        public static string GetTypeName(object value)
        {
            Type t = value.GetType();
            if (t.IsArray) {
                return "object array";
            }
            else {
                return t.Name.ToLower();
            }
        }

        public static string ConvertToString(object obj)
        {
            if (obj == null) {
                return "";
            }
            string str_obj = obj as string;
            if (str_obj != null) {
                return FormatString(str_obj);
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
            return obj.ToString();
        }

        private static string FormatString(string str)
        {
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
                            sb.Append(Convert.ToString(c, 16).PadLeft(4, '0'));
                        }
                        else {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return "\"" + sb + "\"";
        }

        #endregion
    }
}
