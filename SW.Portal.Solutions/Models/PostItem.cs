namespace SW.Portal.Solutions.Models
{
    public class PostItem
    {
        public NotificationMessageBody notification;
        public string title { get; set; }
        public string body { get; set; }
        public object data { get; set; }
        public List<string> registration_ids { get; set; } = new List<string>();
    }   
}
