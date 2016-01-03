using System;
using Tbasic.Runtime;

namespace Tbasic {
    internal class ForBlock : CodeBlock {
        public ForBlock(int index, LineCollection code) {
            LoadFromCollection(
                code.ParseBlock(
                    index,
                    c => c.Name.Equals("FOR", StringComparison.OrdinalIgnoreCase),
                    c => c.Name.Equals("NEXT", StringComparison.OrdinalIgnoreCase)
                ));
        }

        public override void Execute(Executer exec) {
            throw new NotImplementedException();
        }
    }
}
