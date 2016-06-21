using System;
using System.Collections.Generic;
using Tbasic.Operators;
using Tbasic.Components;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Tbasic.Runtime
{
    internal partial class Evaluator
    {
        private Dictionary<string, BinaryOperator> binaryOps = new Dictionary<string, BinaryOperator>(StringComparer.OrdinalIgnoreCase);
        
        public void LoadStandardOperators()
        {
            binaryOps.Add("*",   new BinaryOperator("*",   0, Multiply));
            binaryOps.Add("/",   new BinaryOperator("/",   0, Divide));
            binaryOps.Add("MOD", new BinaryOperator("MOD", 0, Modulo));
            binaryOps.Add("+",   new BinaryOperator("+",   1, Add));
            binaryOps.Add("-",   new BinaryOperator("-",   1, Subtract));
            binaryOps.Add(">>",  new BinaryOperator(">>",  2, ShiftRight));
            binaryOps.Add("<<",  new BinaryOperator("<<",  2, ShiftLeft));
            binaryOps.Add("<",   new BinaryOperator("<",   3, LessThan));
            binaryOps.Add("=<",  new BinaryOperator("=<",  3, LessThanOrEqual));
            binaryOps.Add("<=",  new BinaryOperator("<=",  3, LessThanOrEqual));
            binaryOps.Add(">",   new BinaryOperator(">",   3, GreaterThan));
            binaryOps.Add("=>",  new BinaryOperator("=>",  3, GreaterThanOrEqual));
            binaryOps.Add(">=",  new BinaryOperator(">=",  3, GreaterThanOrEqual));
            binaryOps.Add("==",  new BinaryOperator("==",  4, EqualTo));
            binaryOps.Add("=",   new BinaryOperator("=",   4, EqualTo));
            binaryOps.Add("<>",  new BinaryOperator("<>",  4, NotEqualTo));
            binaryOps.Add("!=",  new BinaryOperator("!=",  4, NotEqualTo));
            binaryOps.Add("&",   new BinaryOperator("&",   5, BitAnd));
            binaryOps.Add("^",   new BinaryOperator("^",   6, BitXor));
            binaryOps.Add("|",   new BinaryOperator("|",   7, BitOr));
            binaryOps.Add("AND", new BinaryOperator("AND", 8, NotImplemented)); // These are special cases that are evaluated with short circuit evalutaion 6/20/16
            binaryOps.Add("OR",  new BinaryOperator("OR",  9, NotImplemented));
        }

        /// <summary>
        /// This method gets the precedence of a binary operator
        /// </summary>
        /// <param name="strOp"></param>
        /// <returns></returns>
        public int OperatorPrecedence(string strOp)
        {    
            return binaryOps[strOp].Precedence;
        }

        /// <summary>
        /// Gets a binary operator if it exists, throws an ArgumentException otherwise
        /// </summary>
        /// <param name="strOp"></param>
        /// <returns></returns>
        public BinaryOperator GetBinaryOperator(string strOp)
        {
            BinaryOperator op;
            if (binaryOps.TryGetValue(strOp, out op)) {
                return op;
            }
            else {
                throw new ArgumentException("operator '" + strOp + "' not defined.");
            }
        }

        private MatchInfo CheckBinaryOp(string expr, int index, out BinaryOperator foundOp)
        {
            int foundIndex = int.MaxValue;
            string foundStr = null;
            foundOp = default(BinaryOperator);
            foreach (var op in binaryOps) {
                string opStr = op.Value.OperatorString;
                int foundAt = expr.IndexOf(opStr, index, StringComparison.OrdinalIgnoreCase);
                if (foundAt > -1 && foundAt < foundIndex) {
                    foundOp = op.Value;
                    foundIndex = foundAt;
                    foundStr = opStr;
                }
            }
            if (foundIndex == -1) {
                return null;
            }
            else {
                return new MatchInfo(Match.Empty, foundIndex, new StringSegment(expr, foundIndex, foundStr.Length));
            }
        }

        private static object Multiply(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) *
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object Divide(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) /
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object Modulo(object left, object right)
        {
            return Convert.ToInt64(left, CultureInfo.CurrentCulture) %
                   Convert.ToInt64(right, CultureInfo.CurrentCulture);
        }

        private static object Add(object left, object right)
        {
            string str1 = left as string,
                   str2 = right as string;
            if (str1 != null || str2 != null)
                return StringAdd(left, right, str1, str2);
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) +
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static string StringAdd(object left, object right, string str1, string str2)
        {
            InitializeStrings(left, right, ref str1, ref str2);
            return str1 + str2;
        }

        private static object Subtract(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) -
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object LessThan(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) <
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object LessThanOrEqual(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) <=
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object GreaterThan(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) >
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object GreaterThanOrEqual(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) >=
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object EqualTo(object left, object right)
        {
            string str1 = left as string,
                   str2 = right as string;
            if (str1 != null || str2 != null)
                return StrEquals(left, right, str1, str2);
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) ==
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static bool StrEquals(object left, object right, string str1, string str2)
        {
            InitializeStrings(left, right, ref str1, ref str2);
            return str1 == str2;
        }

        private static void InitializeStrings(object left, object right, ref string str1, ref string str2)
        {
            if (str1 == null)
                str1 = ConvertToString(left);
            if (str2 == null)
                str2 = ConvertToString(right);
        }

        private static object NotEqualTo(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) !=
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static bool StrNotEqualTo(object left, object right, string str1, string str2)
        {
            InitializeStrings(left, right, ref str1, ref str2);
            return str1 != str2;
        }

        private static object ShiftLeft(object left, object right)
        {
            return Convert.ToInt64(left, CultureInfo.CurrentCulture) <<
                   Convert.ToInt32(right, CultureInfo.CurrentCulture);
        }

        private static object ShiftRight(object left, object right)
        {
            return Convert.ToInt64(left, CultureInfo.CurrentCulture) >>
                   Convert.ToInt32(right, CultureInfo.CurrentCulture);
        }

        private static object BitAnd(object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.CurrentCulture) &
                   Convert.ToUInt64(right, CultureInfo.CurrentCulture);
        }

        private static object BitXor(object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.CurrentCulture) ^
                   Convert.ToUInt64(right, CultureInfo.CurrentCulture);
        }

        private static object BitOr(object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.CurrentCulture) |
                   Convert.ToUInt64(right, CultureInfo.CurrentCulture);
        }

        private static object NotImplemented(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This routine will actually execute an operation and return its value
        /// </summary>
        /// <param name="op">Operator Information</param>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>v1 (op) v2</returns>
        private static object PerformBinaryOp(BinaryOperator op, object left, object right)
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

        private static string GetTypeName(object value)
        {
            Type t = value.GetType();
            if (t.IsArray) {
                return "object array";
            }
            else {
                return t.Name.ToLower();
            }
        }

        private static string ConvertToString(object obj)
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
    }
}
