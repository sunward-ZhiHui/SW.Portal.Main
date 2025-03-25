namespace AC.SD.Core.Services
{
    public class OutlookEmailSettings
    {
        public string SmtpServer { get; set; }
        public string ImapServer { get; set; }
        public int ImapPort { get; set; }
        public int SmtpPort { get; set; }
    }
}
