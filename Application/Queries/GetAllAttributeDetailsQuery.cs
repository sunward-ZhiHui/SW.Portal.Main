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
    public class GetAllAttributeLoadQuery : PagedRequest, IRequest<List<AttributeDetails>>
    {
        public long ID { get; set; }
        public GetAllAttributeLoadQuery(long ID)
        {
            this.ID = ID;
        }
    }
        
    public class GetAllAttributeDetailsQuery : PagedRequest, IRequest<List<AttributeDetails>>
    {
        //public long ID { get; set; }
        //public GetAllAttributeDetailsQuery(long ID)
        //{
        //    this.ID = ID;
        //}
    }
    public class CreateAttributeDetails : AttributeDetails, IRequest<long>
    {
    }
    public class EditAttributeDetails : AttributeDetails, IRequest<long>
    {
        public AttributeDetails AttributeDetailsItem { get; set; }
        public EditAttributeDetails(AttributeDetails AttributeDetailsItem)
        {
            this.AttributeDetailsItem = AttributeDetailsItem;
        }
    }

    public class DeleteAttributeDetails : AttributeDetails, IRequest<long>
    {
        public long AttributeDetailsID { get; set; }
        public DeleteAttributeDetails(long AttributeDetailsID)
        {
            this.AttributeDetailsID = AttributeDetailsID;
        }
    }
}
