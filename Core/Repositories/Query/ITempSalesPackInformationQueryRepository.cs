using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ITempSalesPackInformationQueryRepository : IQueryRepository<TempSalesPackInformationReportModel>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<TempSalesPackInformationReportModel>> GetTempSalesPackInformationReport();
        Task<TempSalesPackInformationReportModel> GetTempSalesPackInformationReportSync(TempSalesPackInformationReportModel tempSalesPackInformationReportModel);
        Task<IReadOnlyList<TempSalesPackInformationFactor>> GetTempSalesPackInformationFactor(long? Id);
        Task<TempSalesPackInformationFactor> InsertTempSalesPackInformationFactor(TempSalesPackInformationFactor value);
        Task<TempSalesPackInformationFactor> DeleteTempSalesPackInformationFactor(long? Id);
        
    }
}
