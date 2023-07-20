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
    public interface ISalesOrderMasterPricingLineSellingMethodQueryRepository : IQueryRepository<SalesOrderMasterPricingLineSellingMethod>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<SalesOrderMasterPricingLineSellingMethod>> GetAllAsync();
        Task<SalesOrderMasterPricingLineSellingMethod> GetByIdAsync(long? Id);
        Task<IReadOnlyList<SalesOrderMasterPricingLineSellingMethod>> GetAllSalesOrderMasterPricingLineAsync(long? Id);

    }
}
