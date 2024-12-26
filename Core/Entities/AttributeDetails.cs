using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Entities.CustomValidations.AttributeCustomValidation;

namespace Core.Entities
{
    public class AttributeDetails : BaseEntity
    {
        [Key]
        public long AttributeDetailID { get; set; }
        [Required(ErrorMessage = "Value is Required")]
        [AttributeValueCustomValidation]
        public string? AttributeDetailName { get; set; }
        public long? AttributeID { get; set; }
        public string? Description { get; set; }

        public bool Disabled { get; set; } = false;
        //[NotMapped]
        //public string? ModifiedBy { get; set; }
        //[NotMapped]
        //public string AddedBy { get; set; }
        public int? FormUsedCount { get; set; }
        [NotMapped]
        public object? DataSourceDetails { get; set; }
        [NotMapped]
        public string? DropDownTypeId { get; set; }
        public long? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public List<AttributeHeader> SubAttributeHeaders { get; set; } = new List<AttributeHeader>();
        public long? ApplicationMasterId { get; set; }
        public string? ApplicationMasterName { get; set; }
        public long? ApplicationMasterCodeId { get; set; }
        public long? ApplicationMasterParentCodeId { get; set; }
        public long? ParentId { get; set; }
        public List<long?> ApplicationMasterParentSectionIds { get; set; } = new List<long?>();
        public string? ParentName { get; set; }
        public string? SiteName { get; set; }
        public string? ZoneName { get; set; }
        public string? LocationName { get; set; }
        public string? AreaName { get; set; }
        public string? AttributeDetailNameId { get; set; }
        public string? DesignationName { get; set; }
        public string? Description2 { get; set; }
        public string? ReplanRefNo { get; set; }
        public string? StartingDate { get; set; }
        public string? BatchNo { get; set; }
        public string? ManufacturingDate { get; set; }
        public string? ExpirationDate { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? OptStatus { get; set; }
        public string? NameList { get; set; }
        public string? BatchSize { get; set; }
        public string? ProdOrderNo { get; set; }
        public string? UnitOfMeasureCode { get; set; }
        public string? SubStatus { get; set; }
        public string? Status { get; set; }
        public string? FullName { get; set; }

    }
    public class DataSourceAttributeDetails
    {
        public List<AttributeDetails> AllAttributeDetails { get; set; } = new List<AttributeDetails>();
    }
    public class AttributeFilterDetailsList
    {
        public List<AttributeFilterDetails> AttributeFilterDetails { get; set; } = new List<AttributeFilterDetails>();
    }
    public class AttributeFilterDetails
    {
        public string? AttributeName { get; set; }
        public string? DataSourceName { get; set; }
        public List<AttributeDetails> AttributeDetails { get; set; } = new List<AttributeDetails>();
        public List<DynamicFormFilterBy> DynamicFormFilterBy { get; set; } = new List<DynamicFormFilterBy>();
        public object Data { get; set; } = new object();
    }
}
