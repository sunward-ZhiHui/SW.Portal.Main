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
    public class GetAllDynamicForm:PagedRequest, IRequest<List<DynamicForm>>
    {

    }
    public class GetAllDynamicFormLst : PagedRequest, IRequest<DynamicForm>
    {
        public Guid ID { get; set; }
        public GetAllDynamicFormLst(Guid ID)
        {
            this.ID = ID;
        }
    }
    public class CreateDynamicForm : DynamicForm, IRequest<long>
    {

    }
    public class EditDynamicForm : DynamicForm, IRequest<long>
    {

    }
    public class DeleteDynamicForm : DynamicForm, IRequest<long>
    {
        public long ID { get; set; }
        public DeleteDynamicForm(long ID)
        {
            this.ID = ID;
        }
    }
    
}
