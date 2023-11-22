using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.CustomValidations;

namespace Core.EntityModels
{
    public class ActivityEmailTopicsModel
    {
        public long ActivityEmailTopicID { get; set; }
        [Required(ErrorMessage = "Manufacturing Process is required")]
        public long? ManufacturingProcessId { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public long? CategoryActionId { get; set; }
        public long? ActionId { get; set; }
        [Required(ErrorMessage = "Subject Name is required")]
        public string SubjectName { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? DocumentSessionId { get; set; }
        public Guid? EmailTopicSessionId { get; set; }
        public string ActivityType { get; set; }
        public long FromId { get; set; }
        public string ToIds { get; set; }
        public string CcIds { get; set; }
        public string Tags { get; set; }
        public string? Comment { get; set; }
        public long? AddedByUserID { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string ManufacturingProcess { get; set; }
        [NotMapped]
        public string CategoryAction { get; set; }
        [NotMapped]
        public string ActionName { get; set; }
        public string BackURL { get; set; }
        public bool? IsDraft { get; set; }
        [Required(ErrorMessage = "To User is required")]
        [ProductionActivityToEmailCustomValidation]
        public IEnumerable<long?>? ToId { get; set; } = null;
        [Required(ErrorMessage = "CC User is required")]
        [ProductionActivityCcEmailCustomValidation]
        public IEnumerable<long?>? CcId { get; set; } = null;
        public long? ActivityMasterId { get; set; }
        public int? StatusCodeId { get; set; }
    }
}
