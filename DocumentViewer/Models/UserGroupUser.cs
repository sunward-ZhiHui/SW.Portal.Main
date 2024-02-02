using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Models
{
    public class UserGroupUser
    {
        [Key]
        public int UserGroupUserID { get; set; }
        public long UserID { get; set; }
        public long UserGroupID { get; set; }        
    }
}
