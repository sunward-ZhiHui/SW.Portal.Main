using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class ForumTypeResponse : BaseEntity
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }      
    }
}
