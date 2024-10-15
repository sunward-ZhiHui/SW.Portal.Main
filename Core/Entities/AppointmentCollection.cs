using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppointmentCollection
    {
        public static List<Appointment> GetAppointments()
        {
            var dataSource = new List<Appointment>()
            {
                new Appointment
                {
                    Caption = "Install New Router in Dev Room",
                    Label = 6,
                    Status = 4,
                   Location = "Location",
                  Description = "Description",
                 
                },
            };
            return dataSource;
        }
    }
}