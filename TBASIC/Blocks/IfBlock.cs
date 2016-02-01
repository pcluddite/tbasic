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
using Tbasic.Runtime;

namespace Tbasic {
    internal class IfBlock : CodeBlock {
        
        public ElseBlock ElseBlock { get; private set; }

        public bool HasElseBlock {
            get {
                return ElseBlock != null;
            }
        }

        public override int Length {
            get {
                if (HasElseBlock) {
                    return base.Length + ElseBlock.Length + 1; // There's an ELSE keyword too
                }
                else {
                    return base.Length;
                }
            }
        }

        public IfBlock(int index, LineCollection fullCode) {
            ElseBlock = null;
            Header = fullCode[index];

            LineCollection ifLines = new LineCollection();
            LineCollection elseLines = new LineCollection();

            int expected_endif = 1; // How many 'END IF' statements are expected
            index++;

            bool isElse = false; // Whether this should be added to the else block

            for (; index < fullCode.Count; index++) {
                Line cur = fullCode[index];
                cur.CurrentBlock = this;
                if (cur.Name.Equals("IF", StringComparison.OrdinalIgnoreCase)) {
                    expected_endif++;
                }

                if (expected_endif > 0) {
                    if (expected_endif == 1 && cur.Name.Equals("ELSE", StringComparison.OrdinalIgnoreCase)) { // we are now in an else block
                        isElse = true;
                        continue; // We don't need to add the word 'ELSE'
                    }
                }

                if (cur.Text.Equals("END IF", StringComparison.OrdinalIgnoreCase)) {
                    expected_endif--;
                }

                if (expected_endif == 0) {
                    if (elseLines.Count > 0) {
                        ElseBlock = new ElseBlock(elseLines);
                    }
                    Footer = cur;
                    Body = ifLines;
                    return;
                }

                if (isElse) {
                    elseLines.Add(cur);
                }
                else {
                    ifLines.Add(cur);
                }
            }

            throw ScriptException.UnterminatedBlock(fullCode[index].LineNumber, Header.VisibleName);
        }

        public override void Execute(Executer exec) {
            if (!Header.Text.EndsWith(" then", StringComparison.OrdinalIgnoreCase)) {
                throw new ArgumentException("expected 'THEN'");
            }

            Evaluator eval = new Evaluator(Header.Text
                .Substring(Header.Text.IndexOf(' ') + 1) // Get rid of the IF
                .Remove(Header.Text.LastIndexOf(' ') - 2), // Get rid of the THEN
                exec
            );

            if (eval.EvaluateBool()) {
                exec.Execute(Body);
            }
            else if (HasElseBlock) {
                exec.Execute(ElseBlock.Body);
            }
        }
    }
}
