using System;
using System.Collections.Generic;
using LearnTsinghua.Models;

namespace LearnTsinghua.Terminal
{
    public static class Utils
    {
        public static void WriteWithHighlights(string s)
        {
            var me = Me.Get();
            var keywords = new List<string>{ me.Name, me.Id };

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
                    break;
                }

                Console.ResetColor();
                Console.Write(s.Substring(pos, keywordPos - pos));
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.Write(keyword);
                pos = keywordPos + keyword.Length;
            }
            Console.ResetColor();
        }

        public static void WriteFileSize(int size)
        {
            var K = 1024;
            var M = K * K;
            var G = K * M;
            
            string unit;

            if (size < 1000)
            {
                unit = "B";
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else if (size < 1000 * K)
            {
                size = (int)Math.Round((double)size / (double)K);
                unit = "K";
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
            else if (size < 1000 * M)
            {
                size = (int)Math.Round((double)size / (double)M);
                unit = "M";
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            }
            else
            {
                size = (int)Math.Round((double)size / (double)G);
                unit = "G";
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }

            Console.Write("{0,3}", size);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(unit);
            Console.ResetColor();
        }
    }
}
