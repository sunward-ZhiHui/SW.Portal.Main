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

}
