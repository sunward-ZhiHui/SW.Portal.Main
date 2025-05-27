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
    public interface ISalesDeliverOrderQueryRepository
    {
        Task<IReadOnlyList<NavpostedShipment>> GetAllByAsync(NavpostedShipment navpostedShipment);
        Task<NavpostedShipment> InsertOrUpdateSalesDeliverOrder(NavpostedShipment navpostedShipment);
        Task<NavpostedShipment> GetSevrviceNavpostedShipment(NavpostedShipment navpostedShipment);
    }
}
