using System;
using System.Linq;
using System.Collections.Generic;
using Tbasic.Runtime;

namespace Tbasic {
    internal class WhileBlock : CodeBlock {

        public WhileBlock(int index, LineCollection code) {
            LoadFromCollection(
                code.ParseBlock(
                    index,
                    c => c.Name.Equals("WHILE", StringComparison.OrdinalIgnoreCase),
                    c => c.Text.Equals("WEND", StringComparison.OrdinalIgnoreCase)
                ));
        }

        public override void Execute(Executer exec) {
            StackFrame parameters = new StackFrame(exec, Header.Text);

            if (parameters.Count < 2) {
                throw ScriptException.NoCondition();
            }

            string condition = Header.Text.Substring(Header.Text.IndexOf(' '));

            Evaluator eval = new Evaluator(condition, exec);

            while (eval.EvaluateBool()) {
                exec.Execute(Body);
                if (exec.BreakRequest) {
                    exec.HonorBreak();
                    break;
                }
                eval.Reparse = true;
            }
        }
    }
}
