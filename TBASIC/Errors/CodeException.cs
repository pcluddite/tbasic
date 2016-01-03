using System;

namespace Tbasic {
    /// <summary>
    /// Handles exceptions due to code formatting
    /// </summary>
    public class CodeException : FormatException {
        internal CodeException(int line)
            : this(line, "Line could not be parsed. Check syntax.") {
        }

        internal CodeException(int line, string msg)
            : base("An error occoured at line " + line + ":\n" + msg) {
        }
    }
}
