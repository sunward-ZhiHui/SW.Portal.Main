using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailParticipant : BaseEntity
    {       
        [Key]
        public long ID { get; set; }
        public long? TopicId { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserCode { get; set; }
        public string? PList { get; set; }
        public bool IsEnabled { get; set; } = true;
        [NotMapped]
        public long? RowIndex { get; set; }
        [NotMapped]
        public string? SubjectName { get; set; }
        [NotMapped]
        public String? DesignationName { get; set; }
        [NotMapped]
        public String? CompanyName { get; set; }
        [NotMapped]
        public String? Name { get; set; }
        [NotMapped]
        public String? NickName { get; set; }
    }    
}
