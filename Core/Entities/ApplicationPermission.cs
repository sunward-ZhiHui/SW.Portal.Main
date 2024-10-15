using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationPermission
    {
        [Key]
        public long PermissionID { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string PermissionName { get; set; }
        public string PermissionCode { get; set; }
        public long? ParentID { get; set; }        
        public int PermissionLevel { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Icon { get; set; }
        public string MenuId { get; set; }
        //[Required(ErrorMessage = "Page URL is Required")]
        public string PermissionURL { get; set; }
        public string PermissionGroup { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string PermissionOrder { get; set; }
        public bool IsDisplay { get; set; }
        public long AppplicationPermissionID { get; set; }
        public string Component { get; set; }
        public string Name { get; set; }
        public string ScreenID { get; set; }
        public bool IsHeader { get; set; }
        public bool IsNewPortal { get; set; }
        public bool IsMobile { get; set; }
        public bool IsProductionApp { get; set; }
        public bool IsCmsApp { get; set; }
        [NotMapped]
        public bool Checked { get; set; } = false;
        public bool IsPermissionURL { get; set; }
        public Guid UniqueSessionID { get; set; }

    }
}
