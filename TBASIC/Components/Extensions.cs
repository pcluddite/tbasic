using System;

namespace Tbasic
{
    internal static class Extensions
    {
        public static bool EqualsIgnoreCase(this string initial, string other)
        {
            return initial.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(this string initial, string other)
        {
            return initial.EndsWith(other, StringComparison.OrdinalIgnoreCase);
        }
    }
}
