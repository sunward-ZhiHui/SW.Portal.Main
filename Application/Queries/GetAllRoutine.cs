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
    public class GetAllRoutine : PagedRequest, IRequest<List<ProductionActivityRoutineAppLine>>
    {
        public class GetAllRoutineList : PagedRequest, IRequest<ProductionActivityRoutineAppLine>
        {
            public Guid? ID { get; set; }
            public long? DynamicFormDataId { get; set; }
            public GetAllRoutineList(Guid? ID, long? dynamicFormDataId)
            {
                this.ID = ID;
                this.DynamicFormDataId = dynamicFormDataId;
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
}
