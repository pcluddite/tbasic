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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Tbasic.Runtime;
using Tbasic.Errors;

namespace Tbasic.Libraries
{
    internal class RuntimeLib : Library
    {
        public RuntimeLib()
        {
            Add("Size", SizeOf);
            Add("Len", SizeOf);
            Add("IsStr", IsString);
            Add("IsInt", IsInt);
            Add("IsDouble", IsDouble);
            Add("IsBool", IsBool);
            Add("IsDefined", IsDefined);
            Add("IsByte", IsByte);
            Add("Str", ToString);
            Add("Double", ToDouble);
            Add("Int", ToInt);
            Add("Bool", ToBool);
            Add("Byte", ToByte);
            Add("Char", ToChar);
            AddLibrary(new StringLib());
            AddLibrary(new ArrayLib());
        }

        private void ToChar(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            try {
                stackFrame.Data = stackFrame.Get<char>(1);
            }
            catch (InvalidCastException) {
                throw new CustomException(ErrorClient.BadRequest, "Parameter cannot be char");
            }
        }

        private void ToString(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            try {
                stackFrame.Data = stackFrame.Get<string>(1);
            }
            catch (InvalidCastException) {
                throw new CustomException(ErrorClient.BadRequest, "Parameter cannot be string");
            }
        }

        private void ToBool(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            try {
                stackFrame.Data = stackFrame.Get<bool>(1);
            }
            catch (InvalidCastException) {
                throw new CustomException(ErrorClient.BadRequest, "Parameter cannot be bool");
            }
        }

        private void ToDouble(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            try {
                stackFrame.Data = stackFrame.Get<double>(1);
            }
            catch (InvalidCastException) {
                throw new CustomException(ErrorClient.BadRequest, "Parameter cannot be double");
            }
        }

        private void ToInt(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            try {
                stackFrame.Data = stackFrame.Get<int>(1);
            }
            catch (InvalidCastException) {
                throw new CustomException(ErrorClient.BadRequest, "Parameter cannot be int");
            }
        }

        private void ToByte(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            try {
                stackFrame.Data = stackFrame.Get<byte>(1);
            }
            catch (InvalidCastException) {
                throw new CustomException(ErrorClient.BadRequest, "Parameter cannot be byte");
            }
        }

        private void SizeOf(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            object obj = stackFrame.Get(1);
            int len = -1;
            if (obj == null) {
                len = 0;
            }
            else if (obj is string) {
                len = obj.ToString().Length;
            }
            else if (obj is int) {
                len = sizeof(int);
            }
            else if (obj is double) {
                len = sizeof(double);
            }
            else if (obj is bool) {
                len = sizeof(bool);
            }
            else if (obj.GetType().IsArray) {
                len = ((object[])obj).Length;
            }
            else {
                throw new CustomException(ErrorClient.BadRequest, "Object size cannot be determined");
            }
            stackFrame.Data = len;
        }

        private void IsInt(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            stackFrame.Data = stackFrame.Get(1) is int;
        }

        private void IsString(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            stackFrame.Data = stackFrame.Get(1) is string;
        }

        private void IsBool(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            stackFrame.Data = stackFrame.Get(1) is bool;
        }

        private void IsDouble(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            stackFrame.Data = stackFrame.Get(1) is byte;
        }

        private void IsDefined(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            string name = stackFrame.Get<string>(1);
            ObjectContext context = stackFrame.Context.FindContext(name);
            stackFrame.Data = context != null;
        }

        private void IsByte(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            stackFrame.Data = stackFrame.Get(1) is byte;
        }
    }
}