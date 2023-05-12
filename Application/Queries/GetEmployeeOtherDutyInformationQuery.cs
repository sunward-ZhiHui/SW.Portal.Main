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
    public class GetEmployeeOtherDutyInformationQuery : PagedRequest, IRequest<List<View_EmployeeOtherDutyInformation>>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public GetEmployeeOtherDutyInformationQuery(long? Id)
        {
            this.Id = Id;
        }
    }


}
