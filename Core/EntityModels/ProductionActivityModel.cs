using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Base;

namespace Core.EntityModels
{
    public class ProductionActivityModel
    {
        [Key]
        public long ProductionActivityAppID { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyID { get; set; }
        [Required(ErrorMessage = "ProdorderNo is Required")]
        public string? ProdOrderNo { get; set; }
        public string? Comment { get; set; }
        public string? TopicID { get; set; }
        public long? LocationID { get; set; }
        [NotMapped]
        public long? ICTMasterID { get; set; }
        [NotMapped]
        public string? SiteName { get; set; }
        [NotMapped]
        public string? ZoneName { get; set; }

        [NotMapped]
        public string? DeropdownName { get; set; }
        [NotMapped]
        public long? NavprodOrderLineId { get; set; }
        [NotMapped]
      
        public string? RePlanRefNo { get; set; }
        [NotMapped]
        public string? BatchNo { get; set; }
        [NotMapped]
        public string? Description { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
        [Key]
        public long ProductionActivityAppLineID { get; set; }
        public long? PAApplineProductionActivityAppID { get; set; }
        public string ActionDropdown { get; set; }
        public long? ProdActivityActionID { get; set; }
        public long? ProdActivityCategoryID { get; set; }
        public long? ManufacturingProcessID { get; set; }
        public bool IsTemplateUpload { get; set; }
       
        public long? ProductActivityCaseLineID { get; set; }
        [Required(ErrorMessage = "ProdorderNo is Required")]
        public string AppLineNavprodOrderLineID { get; set; }
        public bool QaCheck { get; set; }
        public bool IsOthersOptions { get; set; }
        public long? ProdActivityResultID { get; set; }
        [Required(ErrorMessage = "Process is Required")]
        public long? ManufacturingProcessChildID { get; set; }
        [Required(ErrorMessage = "Category is Required")]
        public long? ProdActivityCategoryChildID { get; set; }
        public long? ProdActivityActionChildD { get; set; }
        public string PAApplineTopicID { get; set; }
        public long? QaCheckUserID { get; set; }
        public DateTime? QaCheckDate { get; set; }
        public long? PAApplineLocationID { get; set; }
        public long? ProductActivityCaseID { get; set; }
        public long? ActivityMasterID { get; set; }
        public long? ActivityStatusID { get; set; }
        public string CommentImage { get; set; }
        public string CommentImageType { get; set; }
        [NotMapped]
       
        public string? Process { get; set; }
        [NotMapped]
        public string? Result { get; set; }
        [NotMapped]
        
        public string? Category { get; set; }
        [NotMapped]
        public string? Action { get; set; }

        public DateTime applineAddedDate { get; set; }
        public long? applineModifiedByUserID { get; set; }
        public DateTime? applineModifiedDate { get; set; }
        public Guid? applineSessionId { get; set; }
        public long? applineAddedByUserID { get; set; }
        public int? applineStatusCodeID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}