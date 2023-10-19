using Core.Entities.Base;
using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DocumentDmsShare : BaseEntity
    {
        [Key]
        public long DocumentDmsShareId { get; set; }
        public long? DocumentId { get; set; }
        public Guid? DocSessionId { get; set; }
        public bool IsExpiry { get; set; } = false;
        [FileProfileTypeCustomValidation]
        public DateTime? ExpiryDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? Description { get; set; }
        [NotMapped]
        public string? FileName { get; set; }

    }
}
