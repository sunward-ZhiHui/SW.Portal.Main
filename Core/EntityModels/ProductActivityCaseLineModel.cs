using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ProductActivityCaseLineModel : BaseModel
    {
        public long ProductActivityCaseLineId { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public string NameOfTemplate { get; set; }
        public string Subject { get; set; }
        public string Link { get; set; }
        public string LocationName { get; set; }
        public string Naming { get; set; }
        public bool? IsAutoNumbering { get; set; }
        public long? TemplateProfileId { get; set; }
        public string DocumentNo { get; set; }
        public long? ProdActivityCategoryId { get; set; }
        public long? ProdActivityActionId { get; set; }
        public string TemplateProfileName { get; set; }
        public string ProdActivityCategory { get; set; }
        public string ProdActivityAction { get; set; }
        public string AutoNumbering { get; set; }
        public long? LocationToSaveId { get; set; }
        public long? ProdActivityCategoryChildId { get; set; }
        public long? ProdActivityActionChildId { get; set; }
        public string ManufacturingProcessChilds { get; set; }
        public string TopicId { get; set; }
    }
}
