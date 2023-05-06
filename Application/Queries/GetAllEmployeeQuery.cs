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
    public class GetAllEmployeeQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllEmployeeListQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public string SearchString { get; set; }
    }

    public class GetEmployeeResetPasswordQuery : ViewEmployee,IRequest<ViewEmployee>
    {
        public long LoginID { get; set; }
    }


}
