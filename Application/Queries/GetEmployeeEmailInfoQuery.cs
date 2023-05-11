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
    public class GetEmployeeEmailInfoQuery : PagedRequest, IRequest<List<View_EmployeeEmailInfo>>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public GetEmployeeEmailInfoQuery(long? Id)
        {
            this.Id = Id;
        }
    }


}
