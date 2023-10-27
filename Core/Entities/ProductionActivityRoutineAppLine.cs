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
    public class ProductionActivityRoutineAppLine : BaseEntity
    {
        [Key]
        public long ProductionActivityRoutineAppLineId { get; set; }
        public long? ProductionActivityRoutineAppId { get; set; }
        public long? CompanyID { get; set; }

        public string? ProdOrderNo { get; set; }
        public string? Comment { get; set; }
      
        public long? LocationID { get; set; }
        public string ActionDropdown { get; set; }
        public long? ProdActivityActionId { get; set; }
        public long? ProdActivityCategoryId { get; set; }
        public long? ManufacturingProcessId { get; set; }
        public bool? IsTemplateUpload { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? ProductActivityCaseLineId { get; set; }
        public long? NavprodOrderLineId { get; set; }
       
        public bool? QaCheck { get; set; }
        public bool? IsOthersOptions { get; set; }
        public long? ProdActivityResultId { get; set; }
        public long? ManufacturingProcessChildId { get; set; }
        public long? ProdActivityCategoryChildId { get; set; }
        public long? ProdActivityActionChildD { get; set; }
        public string TopicId { get; set; }
        public long? QaCheckUserId { get; set; }
        public DateTime? QaCheckDate { get; set; }
        public long? LocationId { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public long? VisaMasterId { get; set; }
        public long? RoutineStatusId { get; set; }
        public byte[] CommentImage { get; set; }
        public string CommentImageType { get; set; }

        [NotMapped]
        public string ProdActivityResult { get; set; }
        [NotMapped]
        public string ManufacturingProcessChild { get; set; }

        [NotMapped]
        public string ProdActivityCategoryChild { get; set; }
        [NotMapped]
        public string ProdActivityActionChild { get; set; }
        [NotMapped]
        public string FilePath { get; set; }
        [NotMapped]

        public string LocationName { get; set; }
        [NotMapped]

        public List<long> ResponsibilityUsers { get; set; }
        [NotMapped]
        public string VisaMaster { get; set; }
        [NotMapped]

        public string RoutineStatus { get; set; }
        [NotMapped]

        public bool? IsEmailCreated { get; set; } = false;
        [NotMapped]

        public bool? IsActionPermission { get; set; } = true;
        public ProductionActivityPermission ProductActivityPermissionData { get; set; }
    }
}
