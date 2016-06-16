using System;

namespace Tbasic
{
    internal static class Extensions
    {
        private const StringComparison ComparisonType = StringComparison.OrdinalIgnoreCase;

        public static bool EqualsIgnoreCase(this string initial, string other)
        {
            return initial.Equals(other, ComparisonType);
        }

        public static bool EndsWithIgnoreCase(this string initial, string other)
        {
            return initial.EndsWith(other, ComparisonType);
        }

        public static int IndexOfIgnoreCase(this string initial, string other, int start)
        {
            return initial.IndexOf(other, start, ComparisonType);
        }

        public static int SkipWhiteSpace(this string str, int start = 0)
        {
            for(int index = start; index < str.Length; ++index) {
                if (!char.IsWhiteSpace(str[index])) {
                    return index;
                }
            }
            return -1;
        }
    }
}
