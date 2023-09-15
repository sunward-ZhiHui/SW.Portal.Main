using Application.Queries.Base;
using Core.Entities;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllAttributeHeader : PagedRequest, IRequest<List<AttributeHeader>>
    {
       

    }

    public class CreateAttributeHeader : AttributeHeader, IRequest<long>
    {
    }
    public class EditAttributeHeader : AttributeHeader, IRequest<long>
    {
        public AttributeHeader AttributeDetailsItem { get; set; }
        public EditAttributeHeader(AttributeHeader AttributeDetailsItem)
        {
            this.AttributeDetailsItem = AttributeDetailsItem;
        }
    }

    public class DeleteAttributeHeader : PagedRequest, IRequest<long>
    {
        public long AttributeID { get; set; }
        public DeleteAttributeHeader(long AttributeID)
        {
            this.AttributeID = AttributeID;
        }
    }
}
