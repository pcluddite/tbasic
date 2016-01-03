using System;
using Tbasic.Runtime;

namespace Tbasic {
    internal class ElseBlock : CodeBlock {

        internal ElseBlock(LineCollection lines) {
            Body = lines;
            Header = lines[0];
            Footer = lines[lines.Count - 1];
        }

        public override void Execute(Executer exec) {
            throw new NotImplementedException();
        }
    }
}
