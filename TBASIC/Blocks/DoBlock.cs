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
