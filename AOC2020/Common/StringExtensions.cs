using System;

namespace AOC2020.Common
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var length = text.IndexOf(search, StringComparison.Ordinal);
            return length < 0 ? text : text.Substring(0, length) + replace + text.Substring(length + search.Length);
        }
    }
}