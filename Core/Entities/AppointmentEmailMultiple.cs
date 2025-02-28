using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppointmentEmailMultiple:BaseEntity
    {
        [Key]
        public long AppointmentEmailMultipleID { get; set; }
        public long? AppointmentID { get; set; }
        public long? ConversationId { get; set; }
       

    }
}
