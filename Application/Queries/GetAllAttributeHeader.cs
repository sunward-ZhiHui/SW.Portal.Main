using Application.Queries.Base;
using Core.Entities;
using Core.Repositories.Query;
using DevExpress.XtraReports.Design;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllAttributeHeader : PagedRequest, IRequest<List<DynamicForm>>
    {


    }
    public class GetAllAttributeNameHeader : PagedRequest, IRequest<List<AttributeHeader>>
    {


    }
    public class GetAllAttributeNameNotInDynamicForm : PagedRequest, IRequest<List<AttributeHeader>>
    {
        public long? AttributeID { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public GetAllAttributeNameNotInDynamicForm(long? dynamicFormSectionId, long? attributeID)
        {
            this.DynamicFormSectionId = dynamicFormSectionId;
            this.AttributeID = attributeID;
        }
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
        public DeleteAttributeHeader(long attributeID)
        {
            this.AttributeID = attributeID;
        }
    }

    public class GetAllAttributeNameList : PagedRequest, IRequest<AttributeHeaderListModel>
    {

        public DynamicForm DynamicForm { get; set; }
        public GetAllAttributeNameList(DynamicForm dynamicForm)
        {
            this.DynamicForm = dynamicForm;
        }
    }
}
