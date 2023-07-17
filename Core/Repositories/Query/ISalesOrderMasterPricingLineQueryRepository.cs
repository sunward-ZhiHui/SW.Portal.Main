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
    public interface ISalesOrderMasterPricingLineQueryRepository : IQueryRepository<View_SalesOrderMasterPricingLine>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<View_SalesOrderMasterPricingLine>> GetAllAsync(long? Id);
        Task<View_SalesOrderMasterPricingLine> GetByIdAsync(long? Id);
    }
}
