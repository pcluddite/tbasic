
namespace Tbasic {
    /// <summary>
    /// An exception pertaining to reaching the end of the code
    /// </summary>
    public class EndOfCodeException : CodeException {

        internal EndOfCodeException(int line, string msg) :
            base(line, msg) {
        }

        internal EndOfCodeException(int line)
            : base(line, "End of code was not expected.") {
        }
    }
}
