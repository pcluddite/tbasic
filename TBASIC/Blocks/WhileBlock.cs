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
