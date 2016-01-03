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
