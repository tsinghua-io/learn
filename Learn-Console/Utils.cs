using System;
using System.Collections.Generic;

namespace LearnTsinghua.Terminal
{
    public static class Utils
    {
        public static void WriteWithKeywords(string s, IList<string> keywords)
        {
            var pos = 0;
            while (pos < s.Length)
            {
                var keywordPos = int.MaxValue;
                var keyword = "";

                foreach (var word in keywords)
                {
                    if (string.IsNullOrEmpty(word))
                        continue;
                    var index = s.IndexOf(word, pos);
                    if (index >= 0 && index < keywordPos)
                    {
                        keywordPos = index;
                        keyword = word;
                    }
                }

                if (keyword == "")
                {
                    Console.Write(s.Substring(pos));
                    return;
                }

                Console.Write(s.Substring(pos, keywordPos - pos));
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.Write(keyword);
                Console.ResetColor();
                pos = keywordPos + keyword.Length;
            }
        }
    }
}
