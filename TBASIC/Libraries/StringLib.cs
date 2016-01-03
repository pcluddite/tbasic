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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tbasic.Libraries {
    internal class StringLib : Library {
        public StringLib() {
            Add("StrContains", StringContains);
            Add("StrIndexOf", StringIndexOf);
            Add("StrLastIndexOf", StringLastIndexOf);
            Add("StrUpper", StringUpper);
            Add("StrCompare", StringCompare);
            Add("StrLower", StringLower);
            Add("StrLeft", StringLeft);
            Add("StrRight", StringRight);
            Add("StrTrim", Trim);
            Add("StrTrimStart", TrimStart);
            Add("StrTrimEnd", TrimEnd);
            Add("StrSplit", StringSplit);
            Add("StrToChars", ToCharArray);
            Add("CharsToStr", CharsToString);
            Add("Substring", Substring); 
        }

        private void CharsToString(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            StringBuilder hanz = new StringBuilder();
            foreach (char c in stackFrame.Get<char[]>(1)) {
                hanz.Append(c);
            }
            stackFrame.Data = hanz.ToString();
        }

        private void ToCharArray(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get<string>(1).ToCharArray();
        }

        private void StringSplit(ref StackFrame stackFrame) {
            stackFrame.Assert(3);
            stackFrame.Data = Regex.Split(stackFrame.Get(1).ToString(), stackFrame.Get(2).ToString());
        }

        private void Trim(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get(1).ToString().Trim();
        }

        private void TrimStart(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get(1).ToString().TrimStart();
        }

        private void TrimEnd(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get(1).ToString().TrimEnd();
        }

        private void StringContains(ref StackFrame stackFrame) {
            stackFrame.Assert(3);
            stackFrame.Data = stackFrame.Get<string>(1).Contains(stackFrame.Get<string>(2));
        }

        private void StringCompare(ref StackFrame stackFrame)
        {
            stackFrame.Assert(3);
            stackFrame.Data = stackFrame.Get<string>(1).CompareTo(stackFrame.Get<string>(2));
        }

        private void StringIndexOf(ref StackFrame stackFrame) {
            if (stackFrame.Count == 3) {
                stackFrame.Add(0);
            }
            if (stackFrame.Count == 4) {
                stackFrame.Add(stackFrame.Get<string>(1).Length);
            }
            stackFrame.Assert(5);
            if (stackFrame.Get(2) is char) {
                stackFrame.Data = stackFrame.Get<string>(1).IndexOf(stackFrame.Get<char>(2), stackFrame.Get<int>(3), stackFrame.Get<int>(4));
            }
            else {
                stackFrame.Data = stackFrame.Get<string>(1).IndexOf(stackFrame.Get<string>(2), stackFrame.Get<int>(3), stackFrame.Get<int>(4));
            }
        }

        private void StringLastIndexOf(ref StackFrame stackFrame) {
            if (stackFrame.Count == 3) {
                stackFrame.Add(0);
            }
            if (stackFrame.Count == 4) {
                stackFrame.Add(stackFrame.Get<string>(1).Length);
            }
            stackFrame.Assert(5);
            if (stackFrame.Get(2) is char) {
                stackFrame.Data = stackFrame.Get<string>(1).LastIndexOf(stackFrame.Get<char>(2), stackFrame.Get<int>(3), stackFrame.Get<int>(4));
            }
            else {
                stackFrame.Data = stackFrame.Get<string>(1).LastIndexOf(stackFrame.Get<string>(2), stackFrame.Get<int>(3), stackFrame.Get<int>(4));
            }
        }

        private void StringUpper(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get<string>(1).ToUpper();
        }

        private void StringLower(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get<string>(1).ToLower();
        }

        private void StringLeft(ref StackFrame stackFrame) {
            stackFrame.Assert(3);
            stackFrame.Data = stackFrame.Get<string>(1).Substring(stackFrame.Get<int>(2));
        }

        private void StringRight(ref StackFrame stackFrame) {
            stackFrame.Assert(3);
            stackFrame.Data = stackFrame.Get<string>(1).Remove(stackFrame.Get<int>(2));
        }

        private void Substring(ref StackFrame stackFrame) {
            if (stackFrame.Count == 3) {
                stackFrame.Data = stackFrame.Get<string>(1).Substring(
                                    stackFrame.Get<int>(2)
                                    );
            }
            else {
                stackFrame.Assert(4);
                stackFrame.Data = stackFrame.Get<string>(1).Substring(
                                    stackFrame.Get<int>(2), stackFrame.Get<int>(3)
                                    );
            }
        }
    }
}
