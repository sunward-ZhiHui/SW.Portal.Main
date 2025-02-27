using Core.Entities.Base;
using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserMultiple:BaseEntity
    {
        [Key]
        public long UserMultipleID { get; set; }
       
        public long? AppointmentID { get; set; }
        public long? UserID { get; set; }
        public bool IsAccepted { get; set; } = false;
        public bool IsReminder { get; set; }=false;
         
    }
}
