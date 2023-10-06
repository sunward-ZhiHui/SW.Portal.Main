using FirebaseAdmin.Messaging;

namespace SW.Portal.Solutions.Models
{
    public class PostItem
    {
        public NotificationMessageBody? notification { get; set; }
        public string? title { get; set; }
        public string? body { get; set; }
        public object? data { get; set; }
        //public List<string> registration_ids { get; set; } = new List<string>();


        
        //public Dictionary<string, string> data { get; set; }
        public List<string> registration_ids { get; set; }
        public FcmOptions? fcm_options { get; set; }
    }   
}
