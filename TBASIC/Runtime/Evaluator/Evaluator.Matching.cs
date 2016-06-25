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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tbasic.Components;
using Tbasic.Operators;
using Tbasic.Parsing;
using System.Globalization;

namespace Tbasic.Runtime
{
    internal partial class Evaluator
    {
        /// <summary>
        /// A cray hack for a fully customizable version of the regex Match class
        /// </summary>
        internal struct MatchInfo // hope to remove the day we don't use regex 6/15/16
        {
            public int Index { get; private set; }
            public StringSegment Value { get; private set; }
            public bool Success { get; private set; }

            public int Length
            {
                get {
                    return Value.Length;
                }
            }

            public Match RealMatch { get; private set; }

            public MatchInfo(Match m, int idx, StringSegment val, bool success = true)
            {
                Index = idx;
                Value = val;
                Success = success;
                RealMatch = m;
            }

            public MatchInfo(Match m)
            {
                RealMatch = m;
                if (m == null) {
                    Index = -1;
                    Value = null;
                    Success = false;
                }
                else {
                    Index = m.Index;
                    Value = new StringSegment(m.Value);
                    Success = m.Success;
                }
            }

            public Match NextMatch()
            {
                return RealMatch.NextMatch();
            }

            public static implicit operator MatchInfo(Match m)
            {
                return new MatchInfo(m);
            }
        }

        /// <summary>
        /// Matches a string that is not surrounded by quotes and does not have TBASIC escape sequences
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static MatchInfo MatchUnformattedString(StringSegment expr, string search, int start)
        {
            int index = expr.IndexOf(search, start, StringComparison.OrdinalIgnoreCase);
            if (index > -1) {
                return new MatchInfo(Match.Empty, index, expr.Subsegment(index, search.Length));
            }
            return default(MatchInfo);
        }

        /// <summary>
        /// Matches a string that is surrounded by quotes and may contain TBASIC escape sequences
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="start"></param>
        /// <param name="unformatted"></param>
        /// <returns></returns>
        public static MatchInfo MatchFormattedString(StringSegment expr, int start, out string unformatted)
        {
            if (expr[start] != '\"' && expr[start] != '\'') {
                unformatted = null;
                return null;
            }
            int end = GroupParser.ReadString(expr, start, out unformatted);
            return new MatchInfo(Match.Empty, start, expr.Subsegment(start, end - start + 1));
        }

        private static MatchInfo MatchNumeric(StringSegment expr, int start)
        {
            int endIndex = FindConsecutiveDigits(expr, start);
            if (endIndex > start) {
                if (endIndex < expr.Length && expr[endIndex] == '.') {
                    endIndex = FindConsecutiveDigits(expr, endIndex + 1);
                }
                if (endIndex < expr.Length && (expr[endIndex] == 'e' || expr[endIndex] == 'E')) {
                    if (expr[++endIndex] == '-')
                        ++endIndex;
                    endIndex = FindConsecutiveDigits(expr, endIndex);
                }
            }
            else {
                return null;
            }

            if (endIndex < expr.Length) {
                return new MatchInfo(Match.Empty, start, expr.Subsegment(start, endIndex));
            }
            else {
                return new MatchInfo(Match.Empty, start, expr.Subsegment(start));
            }
        }

        private static int FindConsecutiveDigits(StringSegment expr, int start)
        {
            int index = start;
            for (; index < expr.Length; ++index) {
                if (!char.IsDigit(expr[index])) {
                    return index;
                }
            }
            return index;
        }

        private MatchInfo MatchBinaryOp(StringSegment expr, int index, out BinaryOperator foundOp)
        {
            return MatchOperator(expr, index, CurrentContext._binaryOps, out foundOp);
        }

        private MatchInfo MatchUnaryOp(StringSegment expr, int index, object last, out UnaryOperator foundOp)
        {
            if (last != null && !(last is BinaryOperator)) {
                foundOp = default(UnaryOperator);
                return null;
            }
            return MatchOperator(expr, index, CurrentContext._unaryOps, out foundOp);
        }

        private static MatchInfo MatchOperator<T>(StringSegment expr, int index, IDictionary<string, T> ops, out T foundOp) where T : IOperator
        {
            int foundIndex = int.MaxValue;
            string foundStr = null;
            foundOp = default(T);
            foreach (var op in ops) {
                string opStr = op.Value.OperatorString;
                int foundAt = expr.IndexOf(opStr, index, StringComparison.OrdinalIgnoreCase);
                if (foundAt > -1 && foundAt < foundIndex) {
                    foundOp = op.Value;
                    foundIndex = foundAt;
                    foundStr = opStr;
                }
            }
            if (foundIndex == int.MaxValue) {
                return null;
            }
            else {
                return new MatchInfo(Match.Empty, foundIndex, expr.Subsegment(foundIndex, foundStr.Length));
            }
        }
    }
}
