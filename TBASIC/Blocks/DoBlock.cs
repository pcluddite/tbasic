using System;
using Tbasic.Runtime;

namespace Tbasic {
    internal class DoBlock : CodeBlock {
        public DoBlock(int index, LineCollection code) {
            LoadFromCollection(
                code.ParseBlock(
                    index,
                    c => c.Name.Equals("DO", StringComparison.OrdinalIgnoreCase),
                    c => c.Text.Equals("LOOP", StringComparison.OrdinalIgnoreCase)
                ));
        }

        public override void Execute(Executer exec) {

            StackFrame parameters = new StackFrame(exec, Header.Text);

            if (parameters.Count < 3) {
                throw ScriptException.NoCondition();
            }

            string condition = Header.Text.Substring(Header.Text.Substring(3).IndexOf(' ') + 3);

            if (parameters.Get<string>(1).Equals("UNTIL", StringComparison.OrdinalIgnoreCase)) {
                condition = string.Format("NOT ({0})", condition); // Until means inverted
            }
            else if (parameters.Get<string>(1).Equals("WHILE", StringComparison.OrdinalIgnoreCase)) {
                // don't do anything, you're golden
            }
            else {
                throw new FormatException("expected 'UNTIL' or 'WHILE'");
            }

            Evaluator eval = new Evaluator(condition, exec);

            do {
                exec.Execute(Body);
                if (exec.BreakRequest) {
                    exec.HonorBreak();
                    break;
                }
                eval.Reparse = true;
            }
            while (eval.EvaluateBool());
        }
    }
}
