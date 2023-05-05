using System.ComponentModel.DataAnnotations;

namespace AC.SD.Core.Data.Account
{
    public class Logins
    {
        [Required]
        public string LoginID { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
    