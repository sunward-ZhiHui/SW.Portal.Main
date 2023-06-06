using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Helper
{
    public  class Helper
    {
        public static string FormatDateTime(DateTime dateTime)
        {
            // Define your desired date format here
            return dateTime.ToString("dd-MMM-yyyy hh.mm tt");
        }
        public static string FormatDate(DateTime dateTime)
        {
            // Define your desired date format here
            return dateTime.ToString("dd-MMM-yyyy");
        }
    }
}
