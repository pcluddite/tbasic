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
using Tbasic.Runtime;
using Tbasic.Libraries;

namespace Tbasic {
    internal class FuncBlock : CodeBlock {

        public StackFrame Template { get; private set; }

        public FuncBlock(int index, LineCollection code) {
            LoadFromCollection(
                code.ParseBlock(
                    index,
                    c => c.Name.Equals("FUNCTION", StringComparison.OrdinalIgnoreCase),
                    c => c.Text.Equals("END FUNCTION", StringComparison.OrdinalIgnoreCase)
                ));
            Template = new StackFrame(null);
            Template.SetAll(ParseFunction(Header.Text.Substring(Header.Name.Length)));
        }

        public TBasicFunction CreateDelegate() {
            return new TBasicFunction(Execute);
        }

        public void Execute(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(Template.Count);

            Executer exec = stackFrame.StackExecuter;
            exec.Context = exec.Context.CreateSubContext();
            
            for (int index = 1; index < Template.Count; index++) {
                exec.Context.SetVariable(Template.Get<string>(index), stackFrame.Get(index));
            }
            exec.Context.SetCommand("return", Return);
            exec.Context.SetFunction("SetStatus", SetStatus);

            stackFrame = exec.Execute(Body);
            exec.HonorBreak();
            exec.Context = exec.Context.Collect();
        }
        
        private void Return(ref StackFrame stackFrame) {
            if (stackFrame.Count < 2) {
                stackFrame.AssertArgs(2);
            }
            Evaluator e = new Evaluator(
                stackFrame.Text.Substring(stackFrame.Name.Length),
                stackFrame.StackExecuter);
            stackFrame.Data = e.Evaluate();
            stackFrame.StackExecuter.RequestBreak();
        }

        private void SetStatus(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Status = stackFrame.Get<int>(1);
        }

        public override void Execute(Executer exec) {
            throw new NotImplementedException();
        }

        private object[] ParseFunction(string text) {
            Line codeLine = new Line(-1, text);
            text = codeLine.Text; // it's trimmed
            List<object> result = new List<object>();
            result.Add(codeLine.Name);

            int c_index = codeLine.Name.Length; // Start at the end of the name
            int expected = 0;
            int last = c_index;

            for (; c_index < text.Length; c_index++) {
                char cur = text[c_index];
                switch (cur) {
                    case ' ': // ignore spaces
                        continue;
                    case '\'':
                    case '\"': {
                            c_index = Evaluator.IndexString(text, c_index);
                        }
                        break;
                    case '(':
                        expected++;
                        break;
                    case ')':
                        expected--;
                        break;
                }

                if ((expected == 1 && cur == ',') ||
                     expected == 0) { // The commas in between other parentheses are not ours.
                    string param = text.Substring(last + 1, c_index - last - 1).Trim();
                    if (!param.Equals("")) {
                        result.Add(param); // From the last comma to this one. That's a parameter.
                    }
                    last = c_index;
                    if (expected == 0) { // fin
                        return result.ToArray();
                    }
                }
            }
            throw new FormatException("unterminated function");
        }
    }
}
