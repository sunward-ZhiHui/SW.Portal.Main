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
        public long AttributeHeaderDataSourceId { get; set; }
        public string? DisplayName { get; set; }
        public string? DataSourceTable { get; set; }

    }
}
