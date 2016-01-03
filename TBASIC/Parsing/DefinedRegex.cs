using System.Text.RegularExpressions;

namespace Tbasic.Runtime
{
    /// <summary>
    /// This class is internal to the library, houses different regular expression objects
    /// </summary>
    internal class DefinedRegex
    {
        private const string c_strNumeric       = @"(?:[0-9]+)?(?:\.[0-9]+)?(?:E-?[0-9]+)?(?=\b)";
        private const string c_strHex           = @"0x([0-9a-fA-F]+)";
        private const string c_strBool          = @"true|false";
        private const string c_strFunction      = @"([a-zA-Z][a-zA-Z0-9]*)\s*\((.*)\)";
        private const string c_strVariable      = @"(([a-zA-Z][a-zA-Z0-9]*)\$|\@([a-zA-Z][a-zA-Z0-9]*))(\s*\[(.*)\])?";
        private const string c_strString        = @"\""((\\"")|[^""])*\""|\'((\\')|[^'])*\'";
        private const string c_strNull          = @"null";

        private const string c_strUnaryOp       = @"(?:\+|-|NOT |~)(?=\w|\()";
        private const string c_strBinaryOp      = @"<<|>>|\+|-|\*|/|MOD|AND|OR|&|\||\^|==|!=|<>|>=|=>|<=|=<|=|<|>";
        private const string c_strWhiteSpace    = @"\s+";

        internal static Regex Numeric = new Regex(
            c_strNumeric,
            RegexOptions.Compiled
        );

        internal static Regex Hexadecimal = new Regex(
            c_strHex,
            RegexOptions.Compiled
        );

        internal static Regex Boolean = new Regex(
            c_strBool,
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        internal static Regex UnaryOp = new Regex(
            @"(?<=(?:" + c_strBinaryOp + @")\s*|\A)(?:" + c_strUnaryOp + @")",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        internal static Regex BinaryOp = new Regex(
            @"(?<!(?:" + c_strBinaryOp + @")\s*|^\A)(?:" + c_strBinaryOp + @")",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        internal static Regex Parenthesis = new Regex(
            @"\(",
            RegexOptions.Compiled
        );

        internal static Regex Function = new Regex(
            c_strFunction,
            RegexOptions.Compiled
        );

        internal static Regex Variable = new Regex(
            c_strVariable,
            RegexOptions.Compiled
        );

        internal static Regex String = new Regex(
            c_strString,
            RegexOptions.Compiled
        );

        internal static Regex Null = new Regex(
            c_strNull,
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        internal static Regex WhiteSpace = new Regex(
            c_strWhiteSpace,
            RegexOptions.Compiled
        );

        static DefinedRegex() { }

    }
}
