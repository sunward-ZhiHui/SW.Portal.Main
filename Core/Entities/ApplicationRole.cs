using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationRole : BaseEntity
    {
        [Key]
        public long RoleID { get; set; }
        public string? RoleName { get; set; }
        public string? RoleDescription { get; set; }
    }
}
