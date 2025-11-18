using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Helper
{
    public class Helper
    {
        public static string FormatDateTime(DateTime? dateTime)
        {
            // Define your desired date format here
            if (dateTime != null && dateTime != DateTime.MinValue)

            {
                return dateTime?.ToString("dd-MMM-yyyy hh.mm tt");
            }
            return string.Empty;
        }
        public static string FormatNewDateTime(DateTime? dateTime)
        {
            // Define your desired date format here
            return dateTime?.ToString("MMM dd, yyyy • hh:mm tt");
        }
        public static string FormatTime(DateTime? dateTime)
        {
            // Define your desired date format here
            return dateTime?.ToString("hh.mm tt");
        }
        public static string FormatDate(DateTime? dateTime)
        {
            // Define your desired date format here
            if (dateTime.HasValue && dateTime != DateTime.MinValue)
            {
                // Define your desired date format here
                return dateTime.Value.ToString("dd-MMM-yyyy");
            }
            else
            {
                // Handle the case when the dateTime is null
                return string.Empty; // or any other appropriate handling
            }
        }
        public static string FormatMonthYear(DateTime dateTime)
        {
            // Define your desired date format here
            return dateTime.ToString("MMM-yyyy");
        }
    }
}
