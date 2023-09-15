using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command
{
    public class CreateAttributeDetailsCommand : IRequest<AttributeDetailsResponse>
    {
        public int AttributeDetailID { get; set; }
        public int AttributeID { get; set; }
      
        public string Code { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool Disabled { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public CreateAttributeDetailsCommand()
        {

        }
    }
   
}
