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
            stackFrame.Assert(Template.Count);

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
                stackFrame.Assert(2);
            }
            Evaluator e = new Evaluator(
                stackFrame.Text.Substring(stackFrame.Name.Length),
                stackFrame.StackExecuter);
            stackFrame.Data = e.Evaluate();
            stackFrame.StackExecuter.RequestBreak();
        }

        private void SetStatus(ref StackFrame stackFrame) {
            stackFrame.Assert(2);
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
