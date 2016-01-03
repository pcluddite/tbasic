using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Tbasic.Runtime;

namespace Tbasic.Libraries {
    internal class RuntimeLib : Library {

        public RuntimeLib() {
            Add("SIZE", SizeOf);
            Add("LEN", SizeOf);
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

        private void ToChar(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            try {
                stackFrame.Data = stackFrame.Get<char>(1);
            }
            catch(InvalidCastException) {
                stackFrame.Status = 1;
            }
        }

        private void ToString(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            try {
                stackFrame.Data = stackFrame.Get<string>(1);
            }
            catch (InvalidCastException) {
                stackFrame.Status = 1;
            }
        }

        private void ToBool(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            try {
                stackFrame.Data = stackFrame.Get<bool>(1);
            }
            catch (InvalidCastException) {
                stackFrame.Status = 1;
            }
        }

        private void ToDouble(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            try {
                stackFrame.Data = stackFrame.Get<double>(1);
            }
            catch (InvalidCastException) {
                stackFrame.Status = 1;
            }
        }

        private void ToInt(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            try {
                stackFrame.Data = stackFrame.Get<int>(1);
            }
            catch (InvalidCastException) {
                stackFrame.Status = 1;
            }
        }

        private void ToByte(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            try {
                stackFrame.Data = stackFrame.Get<byte>(1);
            }
            catch (InvalidCastException) {
                stackFrame.Status = 1;
            }
        }

        private void SizeOf(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
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
                stackFrame.Status = 1;
            }
            stackFrame.Data = len;
        }

        private void IsInt(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get(1) is int;
        }

        private void IsString(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
	        stackFrame.Data =  stackFrame.Get(1) is string;
        }

        private void IsBool(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get(1) is bool;
        }
        
        private void IsDouble(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get(1) is byte;
        }
        
        private void IsDefined(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            string name = stackFrame.Get<string>(1);
            ObjectContext context = stackFrame.Context.FindContext(name);
            stackFrame.Data = context != null;
        }
        
        private void IsByte(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get(1) is byte;
        }
    }
}