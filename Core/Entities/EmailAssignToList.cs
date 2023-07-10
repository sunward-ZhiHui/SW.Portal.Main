using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailAssignToList
    {        
        public long UserID { get; set; }
        public long? TopicId { get; set; }
        public string? FirstName { get; set; }
        [NotMapped]
        public string? DesignationName { get; set; }
        [NotMapped]
        public string? CompanyName { get; set;}
        [NotMapped]
        public string? LastName { get; set; }
        [NotMapped]
        public string? NickName { get; set; }

    }
}