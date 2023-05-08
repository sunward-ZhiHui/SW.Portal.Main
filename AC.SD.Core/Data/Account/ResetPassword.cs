using System.ComponentModel.DataAnnotations;

namespace AC.SD.Core.Data.Account
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserID { get; set; }
    }
}
