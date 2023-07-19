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
    public class GetAllProductionActivityAppLineReport : PagedRequest, IRequest<List<view_ProductionActivityAppLineReport>>
    {
    }
    public class GetAllProductionActivityAppLineFilterReport : view_ProductionActivityAppLineReport, IRequest<List<view_ProductionActivityAppLineReport>>
    {
        public long? CompanyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get;  set; }
        public GetAllProductionActivityAppLineFilterReport(long? CompanyId,DateTime FromDate, DateTime ToDate)
        {
            this.CompanyId = CompanyId;
            this.Date = FromDate;
           this.Date = ToDate;
        }
    }
    
}
