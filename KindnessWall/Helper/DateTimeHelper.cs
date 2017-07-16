using AutoMapper;
using System;
using System.Globalization;

namespace KindnessWall.Helper
{
    public static class DateTimeHelper
    {
        public static string GetDateFromShamsiDateTime(this string shamsiDateTime)
        {
            return shamsiDateTime.Substring(0, 10);
        }

        public static string GetTimeFromShamsiDateTime(this string shamsiDateTime)
        {
            return shamsiDateTime.Substring(11, 5);
        }
    }

    public class DateTimeToPersianDateTimeConverter : ITypeConverter<DateTime, string>
    {
        private readonly string _separator;
        private readonly bool _includeHourMinute;

        public DateTimeToPersianDateTimeConverter(string separator = "/", bool includeHourMinute = true)
        {
            _separator = separator;
            _includeHourMinute = includeHourMinute;
        }

        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            return ToShamsiDateTime(source);
        }


        private string ToShamsiDateTime(DateTime info)
        {
            var year = info.Year;
            var month = info.Month;
            var day = info.Day;
            var persianCalendar = new PersianCalendar();
            var pYear = persianCalendar.GetYear(new DateTime(year, month, day, new GregorianCalendar()));
            var pMonth = persianCalendar.GetMonth(new DateTime(year, month, day, new GregorianCalendar()));
            var pDay = persianCalendar.GetDayOfMonth(new DateTime(year, month, day, new GregorianCalendar()));
            return _includeHourMinute ?
                string.Format("{0}{1}{2}{1}{3} {4}:{5}", pYear, _separator, pMonth.ToString("00", CultureInfo.InvariantCulture), pDay.ToString("00", CultureInfo.InvariantCulture), info.Hour.ToString("00"), info.Minute.ToString("00"))
                : string.Format("{0}{1}{2}{1}{3}", pYear, _separator, pMonth.ToString("00", CultureInfo.InvariantCulture), pDay.ToString("00", CultureInfo.InvariantCulture));
        }


    }



}