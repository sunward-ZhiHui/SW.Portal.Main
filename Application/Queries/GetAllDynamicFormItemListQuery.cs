using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllDynamicFormItemListQuery : PagedRequest, IRequest<List<DynamicFormItem>>
    {
        public string SearchString { get; set; }
    }
    public class CreateDynamicFormItemList : DynamicFormItem, IRequest<long>
    {
    }
    public class EditDynamicFormItemList : DynamicFormItem, IRequest<long>
    {
    }
    public class DeleteDynamicFormItemListt : DynamicFormItem, IRequest<long>
    {
        public long ID { get; set; }

        public DeleteDynamicFormItemListt(long Id)
        {
            this.ID = Id;
        }
    }

    public class EditDynamicFormItemLine : DynamicFormItemLine, IRequest<long>
    {

    }
    public class CreateDynamicFormItemLine : DynamicFormItemLine, IRequest<long>
    {

    }
    public class DeleteDynamicFormItemLine : DynamicFormItemLine, IRequest<long>
    {
        public long DynamicFormItemLineID { get; set; }

        public DeleteDynamicFormItemLine(long DynamicFormItemLineID)
        {
            this.DynamicFormItemLineID = DynamicFormItemLineID;
        }
    }
    public class GetAllDynamicItemFormLineList : PagedRequest, IRequest<List<DynamicFormItemLine>>
    {
        public long DynamicFormItemID { get; set; }

        public GetAllDynamicItemFormLineList(long DynamicFormItemID)
        {
            this.DynamicFormItemID = DynamicFormItemID;
        }
    }
    public class GetAllDynamicFormDropdownListQuery : PagedRequest, IRequest<List<DynamicForm>>
    {
        public string SearchString { get; set; }
    }
    public class GetDynamicFormItemMasterList : PagedRequest, IRequest<List<DynamicFormItem>>
    {
        public long DynamicFormItemID { get; set; }

        public GetDynamicFormItemMasterList(long DynamicFormItemID)
        {
            this.DynamicFormItemID = DynamicFormItemID;
        }
    }

    public class GetDynamicFormItemLineDropdoenList : PagedRequest, IRequest<DropDownOptionsGridListModel>
    {
     
       public List<long?> DynamicFormId { get;set; }

        public long? Userid { get; set; }
        public GetDynamicFormItemLineDropdoenList(List<long?> DynamicFormId, long? userid)
        {
            this.DynamicFormId = DynamicFormId;
            this.Userid = userid;
        }
    }


    public class GetDynamicFormItemBySession : PagedRequest, IRequest<DynamicFormItem>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetDynamicFormItemBySession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
}
