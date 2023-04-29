using Application.Queries.Base;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllDepartmentQuery : PagedRequest, IRequest<List<ViewDepartment>>
    {
        public string SearchString { get; set; }
    }
}
