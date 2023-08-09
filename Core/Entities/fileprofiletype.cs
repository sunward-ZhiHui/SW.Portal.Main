using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Fileprofiletype
    {
        public long FileProfileTypeId { get; set; }
        public long ProfileId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsExpiryDate { get; set; }
        public bool? IsAllowMobileUpload { get; set; }
        public bool? IsDocumentAccess { get; set; }
        public int? ShelfLifeDuration { get; set; }
        public int? ShelfLifeDurationId { get; set; }
        public string Hints { get; set; }
        public bool? IsEnableCreateTask { get; set; }
        public string ProfileTypeInfo { get; set; }
        public bool? IsCreateByYear { get; set; }
        public bool? IsCreateByMonth { get; set; }
        public bool? IsHidden { get; set; }
        public string ProfileInfo { get; set; }
        public bool? IsTemplateCaseNo { get; set; }
        public long? TemplateTestCaseId { get; set; }
        [NotMapped]
        public string? Label { get; set; }
        public List<Fileprofiletype> Children { get; set; } = new List<Fileprofiletype>();
    }
}
