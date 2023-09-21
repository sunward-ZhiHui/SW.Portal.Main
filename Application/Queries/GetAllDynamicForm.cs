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
    public class CreateDynamicForm : DynamicForm, IRequest<long>
    {

    }
    
}
