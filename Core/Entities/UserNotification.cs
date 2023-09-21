using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserNotification
    {
        [Key]
        public long UserNotificationId { get; set; }
        public long UserId { get; set; }
        public string DeviceType { get; set; }
        public string TokenID { get; set; }
        
    }
}
