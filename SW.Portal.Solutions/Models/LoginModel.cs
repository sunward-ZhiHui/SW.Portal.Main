namespace SW.Portal.Solutions.Models
{
    public class LoginModel
    {
        public string? loginId { get; set; }
        public string? password { get; set; }
    }
    public class FbOutputCartonsModels
    {
        public long FbOutputCartonID { get; set; }

    }
    public class DeviceTkenModel
    {
        public string? LoginId { get; set; }
        public string? DeviceType { get; set; }
        public string? TokenID { get; set; }
    }
    public class NotificationAllModel
    {
        public List<string>? tokens { get; set; } = new List<string>();
        public string? titles { get; set; }
        public string? message { get; set; }
        public string? housturl { get; set; }
    }
    public class UploadModel
    {
        public Guid SessionId { get; set; }
        public long? addedByUserId { get; set; }
        public IFormFile File { get; set; }
        public string? NewFilename { get; set; }
        public IFormCollection Files { get; set; }
    }
}
