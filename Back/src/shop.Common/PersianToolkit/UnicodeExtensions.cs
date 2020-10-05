using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace shop.Common.PersianToolkit
{
    public static class UnicodeExtensions
    {
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormKC);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string CleanUnderLines(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            const char chr1600 = (char)1600; //ـ=1600
            const char chr8204 = (char)8204; //‌=8204

            return text.Replace(chr1600.ToString(), "")
                       .Replace(chr8204.ToString(), "");
        }

        public static string RemovePunctuation(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ?
                string.Empty :
                new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
        }

        public static string ToTime(this string time, bool IsEnd = false)
        {
            if (IsEnd)
            {
                // value;
                double outTime = 0.0;
                var res = double.TryParse(time, out outTime);
                if (res)
                {
                    outTime = outTime + 0.15;
                    var p1 = (int)outTime;
                    var p2 = outTime - p1;

                    if (p2 > 0.45)
                    {
                        p1++;
                        p2 = 0.0;
                    }

                    time = (p1 + Math.Round(p2, 2)).ToString();
                }
            }

            var ht = time.Split('.');
            var hour = ht.First();
            var minute = ht.Length > 1 ? ht.Last() : "00";
            if (ht.First().Length == 1)
            {
                hour = "0" + ht.First();
            }
            if (ht.Length > 1 && ht.Last().Length == 1)
            {
                minute = ht.Last() + "0";
            }
            return hour + ":" + minute;
        }
        public static string GetHour(this string time)
        {
            var ht = time.Split('.');
            var hour = ht.First();
            if (ht.First().Length == 1)
            {
                hour = "0" + ht.First();
            }
            return hour;
        }
        public static string GetMin(this string time)
        {
            var ht = time.Split('.');
            var hour = ht.First();
            var minute = ht.Length > 1 ? ht.Last() : "00";

            if (ht.Length > 1 && ht.Last().Length == 1)
            {
                minute = ht.Last() + "0";
            }
            return minute;
        }

        public static DateTime GetDateTime(this DateTime Date, double hour)
        {
            Date = Date.Date;
            var h = (int)hour;
            var m = (hour - (int)hour) * 100;
            if (hour >= 24)
            {
                Date = Date.AddDays(1);
                Date = Date.AddMinutes((int)m);
            }
            else
            {
                Date = Date.AddMinutes(m);
                Date = Date.AddHours(h);
            }

            return Date;
        }
        public static string GetEnumDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }
        public static string ToEnglishNumber(string input)
        {
            try
            {
                string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

                for (int j = 0; j < persian.Length; j++)
                    input = input.Replace(persian[j], j.ToString());

                return input;
            }
            catch
            {
                return null;
            }
        }
    }
}