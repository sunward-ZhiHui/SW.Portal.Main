using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllTempSalesPackInformationQuery : PagedRequest, IRequest<List<TempSalesPackInformationReportModel>>
    {
    }
    public class GetAllTempSalesPackInformationSyncQuery : PagedRequest, IRequest<TempSalesPackInformationReportModel>
    {
        public TempSalesPackInformationReportModel TempSalesPackInformationReportModel { get; set; }
        public GetAllTempSalesPackInformationSyncQuery(TempSalesPackInformationReportModel SearchString)
        {
            this.TempSalesPackInformationReportModel = SearchString;
        }
    }
    public class GetTempSalesPackInformationFactor : PagedRequest, IRequest<List<TempSalesPackInformationFactor>>
    {
        public long? Id { get; set; }
        public GetTempSalesPackInformationFactor(long? id)
        {
            this.Id = id;
        }
    }
    public class TempSalesPackInformationFactorQuery : PagedRequest, IRequest<TempSalesPackInformationFactor>
    {
        public TempSalesPackInformationFactor TempSalesPackInformationFactor { get; private set; }
        public TempSalesPackInformationFactorQuery(TempSalesPackInformationFactor tempSalesPackInformationFactor)
        {
            this.TempSalesPackInformationFactor = tempSalesPackInformationFactor;
        }
    }
    public class DeleteTempSalesPackInformationFactorQuery : PagedRequest, IRequest<TempSalesPackInformationFactor>
    {
        public long? Id { get; set; }
        public DeleteTempSalesPackInformationFactorQuery(long? id)
        {
            this.Id = id;
        }
    }
}
