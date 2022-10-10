using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreTutorial.Data
{
    public static class AppHash
    {
        public static string HashPassword(string input)
        {
            string ps = string.Empty;
            MD5 hash = MD5.Create();
            byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            ps = builder.ToString();

            return ps;
        }

        public static string StringToBase64(string text)
        {
            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }

        public static string Base64ToString(string base64Text)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64Text);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string TimeAgo(DateTime postDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - postDate.Ticks);
            double total = Math.Abs(ts.TotalSeconds);

            if (total < 1 * MINUTE)
            {
                if (ts.Seconds == 1)
                {
                    return "منذ لحظة مضت";
                }
                else if (ts.Seconds < 11)
                {
                    return "منذ " + ts.Seconds + " ثواني مضت ";
                }
                else
                {
                    return "منذ " + ts.Seconds + " ثانية مضت ";
                }
            }

            if (total < 2 * MINUTE)
                return "منذ دقيقتين مضت";

            if (total < 60 * MINUTE)
                return "منذ " + ts.Minutes + " دقيقة مضت ";

            if (total < 90 * MINUTE)
                return "منذ ساعة مضت";

            if (total < 24 * HOUR)
                return "منذ " + ts.Hours + " ساعة مضت ";

            if (total < 48 * HOUR)
                return "منذ البارحة";

            if (total < 30 * DAY)
                return "منذ " + ts.Days + " يوم مضي ";

            if (total < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "منذ شهر مضي" : "منذ " + months + " شهر مضي ";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "منذ سنة مضت" : "منذ " + years + " سنة مضت ";
            }
        }
    }
}
