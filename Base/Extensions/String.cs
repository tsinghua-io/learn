using System;
using System.Text.RegularExpressions;

namespace LearnTsinghua.Extensions
{
    public static class StringExtension
    {
        public static bool FuzzyMatch(this string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                return false;

            var pos = 0;
            foreach (var ch in source)
            {
                var index = target.IndexOf(ch, pos);
                if (index < 0)
                    return false;
                pos = index + 1;
            }
            return true;
        }

        public static string Oneliner(this string str)
        {
            return Regex.Replace(str ?? "", @"\s+", " ");
        }
    }
}
