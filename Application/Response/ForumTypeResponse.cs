using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class ForumTypeResponse
    {
        public long ID { get; set; }
        public long IndexId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? RowIndex { get; set; }


    }
}
