using System;
using System.Text.RegularExpressions;
using AngleSharp.Parser.Html;

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

        public static string RemoveTags(this string str)
        {
            var doc = new HtmlParser().Parse(str);
            return doc.DocumentElement.TextContent.Trim();
        }

        public static string Oneliner(this string str)
        {
            return Regex.Replace(str ?? "", @"\s+", " ");
        }

        public static string WrapWithCss(this string html, string cssHref)
        {
            return string.Format(
                "<!DOCTYPE html>" +
                "<html>" +
                "<head><link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\"></head>" +
                "<body>{1}</body>" +
                "</html>", cssHref, html);
        }

        public static void ParseSemesterId(this string semesterId, out string year, out string semester)
        {
            year = "#";
            semester = semesterId;
            if (semesterId.Length == 11)
            {
                year = semesterId.Substring(0, 9);
                var season = semesterId.Substring(10);
                switch (season)
                {
                    case "1":
                        semester = "秋季学期";
                        break;
                    case "2":
                        semester = "春季学期";
                        break;
                    case "3":
                        semester = "夏季学期";
                        break;
                }
            }
        }

        public static string SemesterString(this string semesterId)
        {
            string year, semester;
            semesterId.ParseSemesterId(out year, out semester);
            return year + semester;
        }
    }
}
