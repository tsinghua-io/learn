using System;

namespace LearnTsinghua.Extensions
{
    public static class DateTimeExtension
    {
        public static string DaysSince(this DateTime time)
        {
            var diff = DateTime.Now - time;
            if (diff.Days == 0)
                return "今天";
            else if (diff.Days == 1)
                return "昨天";
            else
                return diff.Days + "天前";
        }
    }
}

