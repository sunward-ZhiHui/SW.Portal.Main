
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface INavItemsQueryRepository : IQueryRepository<View_NavItems>
    {
        Task<IReadOnlyList<View_NavItems>> GetAllAsync();
        Task<IReadOnlyList<View_NavItems>> GetAsyncList();
        Task<View_NavItems> GetByItemSerialNoAsync(string ItemSerialNo);
        Task<long> Update(View_NavItems todolist);
        Task<View_NavItems> GetByItemSerialNoExitsAsync(View_NavItems ItemSerialNo);
        Task<IReadOnlyList<View_NavItems>> GetByCompanyBySerailNoAsyncList(long? CompanyId);
        Task<IReadOnlyList<View_NavItems>> GetByCompanyAsyncList(long? CompanyId);
        Task<IReadOnlyList<View_NavItems>> GetByItemSerialNoNotNullAsync();
        Task<IReadOnlyList<ItemBatchInfo>> GetNavItemBatchNoByItemIdAsync(long? ItemId, long? CompanyId);
        Task<IReadOnlyList<NavProductionInformation>> GetNavProductionInformation(long? ItemId);
        Task<IReadOnlyList<View_NavCrossReference>> GetNavCrossReference(long? ItemId);
        Task<ItemBatchInfo> GetSyncBatchInfo(string ItemNo, long? CompanyId, long? ItemId);
        Task<ItemBatchInfo> GetNavItemBatchInfo(long? CompanyId);
        Task<IReadOnlyList<Navitems>> GetNavItemItemNosAsync(long? CompanyId);
        Task<Navitems> GetNavItemServicesList(long? CompanyId,long? UserId);
        Task<FinishedProdOrderLine> GetFinishedProdOrderLineList(long? CompanyId);
        Task<IReadOnlyList<ItemBatchInfo>> GetNavItemBatchNoByItemByAllAsync();
        Task<ItemBatchInfo> InsertOrUpdateBatchInfo(ItemBatchInfo itemBatchInfo);
        Task<ItemBatchInfo> DeleteItemBatchInfo(ItemBatchInfo itemBatchInfo);
        Task<NavprodOrderLine> GetNavprodOrderLineList(long? CompanyId);
        Task<IReadOnlyList<NavprodOrderLine>> GetNavprodOrderLineListAsync();
        Task<NavprodOrderLine> DeleteNavprodOrderLine(NavprodOrderLine navprodOrderLine);
        Task<NavprodOrderLine> InsertOrUpdateNavprodOrderLine(NavprodOrderLine navprodOrderLine);

        Task<FinishedProdOrderLine> InsertOrUpdateFinishedProdOrderLine(FinishedProdOrderLine finishedProdOrderLine);
        Task<FinishedProdOrderLine> DeleteFinishedProdOrderLine(FinishedProdOrderLine finishedProdOrderLine);
        Task<IReadOnlyList<FinishedProdOrderLine>> GeFinishedProdOrderLineListAsync();
        Task<IReadOnlyList<FinishedProdOrderLineOptStatus>> GetFinishedProdOrderLineOptStatus();
    }
}
