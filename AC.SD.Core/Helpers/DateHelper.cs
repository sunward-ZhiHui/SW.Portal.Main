using System;
using System.Globalization;

namespace AC.SD.Core.Helpers
{
    public static class DateHelper
    {
        public static int GetWeekNumber(DateTime selectedDate)
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            var calendar = cultureInfo.Calendar;

            return calendar.GetWeekOfYear(
                selectedDate,
                cultureInfo.DateTimeFormat.CalendarWeekRule,
                cultureInfo.DateTimeFormat.FirstDayOfWeek
            );
        }
    }
}
