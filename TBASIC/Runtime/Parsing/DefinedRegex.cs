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
        private const string c_strVariable      = @"(([a-zA-Z_][a-zA-Z0-9_]*)\$|\@([a-zA-Z_][a-zA-Z0-9_]*))(\s*\[(.*)\])?";
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
