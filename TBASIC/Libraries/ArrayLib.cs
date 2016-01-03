using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Libraries {
    internal class ArrayLib : Library {

        public ArrayLib() {
            Add("ArrayContains", ArrayContains);
            Add("ArrayIndexOf", ArrayIndexOf);
            Add("ArrayLastIndexOf", ArrayLastIndexOf);
            //Add("ArrayResize", ArrayResize);
        }

        private void ArrayContains(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
            stackFrame.Data = stackFrame.Get<object[]>(1).Contains(stackFrame.Get(2));
        }

        private void ArrayIndexOf(ref StackFrame stackFrame) {
            object[] arr = stackFrame.Get<object[]>(1);
            if (stackFrame.Count == 3) {
                stackFrame.Add(0);
            }
            if (stackFrame.Count == 4) {
                stackFrame.Add(arr.Length);
            }
            stackFrame.Assert(5);
            object o = stackFrame.Get(2);
            int i = stackFrame.Get<int>(3);
            int count = stackFrame.Get<int>(5);
            for (; i < arr.Length && i < count; i++) {
                if (arr[i] == o) {
                    stackFrame.Data = i;
                    return;
                }
            }
            stackFrame.Data = -1;
        }

        private void ArrayLastIndexOf(ref StackFrame stackFrame) {
            object[] arr = stackFrame.Get<object[]>(1);
            if (stackFrame.Count == 3) {
                stackFrame.Add(0);
            }
            if (stackFrame.Count == 4) {
                stackFrame.Add(arr.Length);
            }
            stackFrame.Assert(5);
            int i = stackFrame.Get<int>(3);
            object o = stackFrame.Get(2);
            int count = stackFrame.Get<int>(5);
            for (; i >= 0 && i > count; i--) {
                if (arr[i] == o) {
                    stackFrame.Data = i;
                    return;
                }
            }
            stackFrame.Data = -1;
        }
    }
}
