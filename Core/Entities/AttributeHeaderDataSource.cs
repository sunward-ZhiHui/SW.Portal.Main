using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AttributeHeaderDataSource
    {
        [Key]
        public long HeaderDataSourceId { get; set; }
        public long AttributeHeaderDataSourceId { get; set; }
        
        public string? DisplayName { get; set; }
        public string? DataSourceTable { get; set; }

    }
    public class DynamicFormFilter
    {
        [Key]
        public long DynamicFormFilterId { get; set; }
        public long? DynamicFilterId { get; set; }
        public string? TableName { get; set; }
        public string? DisplayName { get; set; }
    }
    public class DynamicFormFilterBy
    {
        [Key]
        public long DynamicFormFilterById { get; set; }
        public long? DynamicFormFilterId { get; set; }
        public string? FilterDisplayName { get; set; }
        public string? FromFilterFieldName { get; set; }
        public string? FilterTableName { get; set; }
        public string? ToDropDownFieldId { get; set; }
        public string? ToDisplayDropDownName { get; set; }
        public string? ToDisplayDropDownDescription { get; set; }
        public string? FilterDataType { get; set; }
        public string? ApplicationMasterCodeId { get; set; }
        public long? AttributeHeaderDataSourceId { get; set; }
        public string? DisplayName { get; set; }
        public string? DataSourceTable { get; set; }
    }
}
