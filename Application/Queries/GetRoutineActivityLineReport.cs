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
    public class GetRoutineActivityLineReport : PagedRequest, IRequest<List<View_RoutineActivityAppLineReport>>
    {
    }
    public class GetAllRoutineActivityAppLineFilterReport : View_RoutineActivityAppLineReport, IRequest<List<View_RoutineActivityAppLineReport>>
    {
        public long? CompanyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public GetAllRoutineActivityAppLineFilterReport(long? CompanyId, DateTime FromDate, DateTime ToDate)
        {
            this.CompanyId = CompanyId;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
        }
    }
   
}
