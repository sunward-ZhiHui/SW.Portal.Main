using Application.Queries.Base;
using Core.Entities;
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
        public string SearchString { get; set; }

    }

    public class CreateAttributeHeader : AttributeHeader, IRequest<long>
    {
    }
    public class EditAttributeHeader : AttributeHeader, IRequest<long>
    {
        public AttributeHeader AttributeHeaderItem { get; set; }
        public EditAttributeHeader(AttributeHeader AttributeHeaderItem)
        {
            this.AttributeHeaderItem = AttributeHeaderItem;
        }
    }

    public class DeleteAttributeHeader : PagedRequest, IRequest<long>
    {
        public long ID { get; set; }
        public DeleteAttributeHeader(long ID)
        {
            this.ID = ID;
        }
    }
}
