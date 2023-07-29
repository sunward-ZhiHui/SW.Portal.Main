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
    public interface ISalesOrderMasterPricingQueryRepository : IQueryRepository<View_SalesOrderMasterPricing>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<View_SalesOrderMasterPricing>> GetAllByMasterTypeAsync(string MasterType);
        Task<View_SalesOrderMasterPricing> GetByIdAsync(long? Id);
        Task<View_SalesOrderMasterPricing> GetBySessionIdAsync(Guid? SessionId);
        Task<View_SalesOrderMasterPricing> GetCheckPriceValidaityDateAsync(SalesOrderMasterPricing view_SalesOrderMasterPricing);
        Task<long> InsertSalesOrderMasterPricingLineAsync(SalesOrderMasterPricing salesOrderMasterPricing);
        Task<IReadOnlyList<View_SalesOrderMasterPricingLineByItem>> GetSalesOrderLineByItemAsync(long? CompanyId, DateTime? FromDate, long? SellingMethodId, long? ItemId);
        Task<SalesOrderMasterPricingFromSalesModel> GetPricingTypeForSellingMethod(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId,decimal? Qty, string SellingMethod);

    }
}
