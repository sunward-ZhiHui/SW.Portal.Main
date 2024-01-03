namespace SW.Portal.Solutions.Models
{
    public class FcmNotificationSetting
    {
        public string SenderId { get; set; }
        public string ServerKey { get; set; }
    }
    public class RequestBodyOptions
    {
        public int? MinRequestBodyDataRate { get; set; } = 1024;
}
}
