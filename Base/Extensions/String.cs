using System;

namespace LearnTsinghua.Extensions
{
    public static class StringExtension
    {
        public static bool FuzzyMatch(this string source, string target)
        {
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
    }
}
