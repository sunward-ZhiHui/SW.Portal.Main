using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public static class DateFormatHelper
    {
        public static string FormatDateTime(DateTime dateTime)
        {
            // Define your desired date format here
            return dateTime.ToString("dd-MMM-yyyy HH:MM:ss");
        }
    }
}
