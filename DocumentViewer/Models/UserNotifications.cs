using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Models
{
    public class UserNotifications
    {
        [Key]
        public long UserNotificationId { get; set; }
        public long UserId { get; set; }
        public string? DeviceType { get; set; }
        public string? TokenID { get; set; }
    }
}
