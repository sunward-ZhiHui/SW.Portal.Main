using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Helper
{
    public  class Helper
    {
        public static string FormatDateTime(DateTime? dateTime)
        {
            // Define your desired date format here
            return dateTime?.ToString("dd-MMM-yyyy hh.mm tt");
        }
        public static string FormatDate(DateTime? dateTime)
        {
            // Define your desired date format here
            if (dateTime.HasValue)
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
