using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.EntityModel;
using Core.Entities.Views;
using Core.Entities;
using Azure.Core;
using static Duende.IdentityServer.Models.IdentityResources;

namespace Infrastructure.Repository.Query
{
    public class NavItemsQueryRepository : QueryRepository<View_NavItems>, INavItemsQueryRepository
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IPlantQueryRepository _plantQueryRepository;
        public NavItemsQueryRepository(IConfiguration configuration, ISalesOrderService salesOrderService, IPlantQueryRepository plantQueryRepository)
            : base(configuration)
        {
            _salesOrderService = salesOrderService;
            _plantQueryRepository = plantQueryRepository;
        }
        public async Task<IReadOnlyList<View_NavItems>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_NavItems";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_NavItems>> GetAsyncList()
        {
            try
            {
                var query = "select  * from NAVItems";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_NavItems>> GetByItemSerialNoNotNullAsync()
        {
            try
            {
                var query = "select  * from NAVItems where ItemSerialNo is not null ";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_NavItems>> GetByCompanyBySerailNoAsyncList(long? CompanyId)
        {
            try
            {
                var query = "select  * from NAVItems where ItemSerialNo is not null and CompanyId=" + CompanyId;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<View_NavItems>> GetByCompanyAsyncList(long? CompanyId)
        {
            try
            {
                var query = "select  * from NAVItems where CompanyId=" + CompanyId;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavItems>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Update(View_NavItems todolist)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ItemSerialNo", todolist.ItemSerialNo);
                            parameters.Add("ItemId", todolist.ItemId, DbType.Int64);
                            parameters.Add("ModifiedDate", todolist.ModifiedDate, DbType.DateTime);
                            parameters.Add("ModifiedByUserId", todolist.ModifiedByUserId, DbType.Int64);
                            parameters.Add("UomId", todolist.UomId, DbType.Int64);
                            parameters.Add("SupplyToId", todolist.SupplyToId, DbType.Int64);
                            parameters.Add("PackSizeId", todolist.PackSizeId, DbType.Int64);
                            parameters.Add("CompanyId", todolist.CompanyId, DbType.Int64);
                            var query = " UPDATE Navitems SET ItemSerialNo = @ItemSerialNo,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId,UomId=@UomId,SupplyToId=@SupplyToId,PackSizeId=@PackSizeId,CompanyId=@CompanyId WHERE ItemId = @ItemId";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_NavItems> GetByItemSerialNoExitsAsync(View_NavItems ItemSerialNo)
        {
            try
            {
                var query = "SELECT * FROM view_NavItems WHERE ItemSerialNo =" + "'" + ItemSerialNo.ItemSerialNo + "'";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_NavItems>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_NavItems> GetByItemSerialNoAsync(string ItemSerialNo)
        {
            try
            {
                var query = "SELECT * FROM view_NavItems WHERE  ItemSerialNo =@ItemSerialNo";
                var parameters = new DynamicParameters();
                parameters.Add("ItemSerialNo", ItemSerialNo, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_NavItems>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ItemBatchInfo>> GetNavItemBatchNoByItemIdAsync(long? ItemId, long? CompanyId)
        {
            try
            {
                var query = "select  * from ItemBatchInfo where  CompanyId= " + CompanyId + " AND ItemId=" + ItemId;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ItemBatchInfo>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_NavCrossReference>> GetNavCrossReference(long? ItemId)
        {
            try
            {
                var query = "select  * from View_NavCrossReference where  ItemId=" + ItemId;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_NavCrossReference>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<NavProductionInformation>> GetNavProductionInformation(long? ItemId)
        {
            try
            {
                var query = "select  * from NavProductionInformation where  ItemId=" + ItemId;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavProductionInformation>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertBatchInfo(ItemBatchInfo itemBatchInfo, long? CompanyId, long? ItemId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ItemId", ItemId);
                            parameters.Add("CompanyId", CompanyId);
                            parameters.Add("LocationCode", itemBatchInfo.LocationCode);
                            parameters.Add("BatchNo", itemBatchInfo.BatchNo);
                            parameters.Add("LotNo", itemBatchInfo.LotNo);
                            parameters.Add("ExpiryDate", itemBatchInfo.ExpiryDate, DbType.Date);
                            parameters.Add("ManufacturingDate", itemBatchInfo.ManufacturingDate, DbType.Date);
                            parameters.Add("QuantityOnHand", itemBatchInfo.QuantityOnHand, DbType.Decimal);
                            parameters.Add("NavQuantity", itemBatchInfo.NavQuantity, DbType.Decimal);
                            parameters.Add("IssueQuantity", itemBatchInfo.BalanceQuantity, DbType.Decimal);
                            parameters.Add("BalanceQuantity", itemBatchInfo.BalanceQuantity, DbType.Decimal);
                            parameters.Add("StatusCodeId", 1);
                            parameters.Add("AddedDate", DateTime.Now);

                            var query = "INSERT INTO [ItemBatchInfo](ItemId,CompanyId,LocationCode,BatchNo,LotNo,ExpiryDate,ManufacturingDate,QuantityOnHand,NavQuantity,IssueQuantity,BalanceQuantity,StatusCodeId,AddedDate) OUTPUT INSERTED.ItemBatchId VALUES " +
                                "(@ItemId,@CompanyId,@LocationCode,@BatchNo,@LotNo,@ExpiryDate,@ManufacturingDate,@QuantityOnHand,@NavQuantity,@IssueQuantity,@BalanceQuantity,@StatusCodeId,@AddedDate)";

                            var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            transaction.Commit();

                            return lastInsertedRecordId;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ItemBatchInfo> GetSyncBatchInfo(string ItemNo, long? CompanyId, long? ItemId)
        {
            try
            {
                ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                var lists = await GetNavItemBatchNoByItemIdAsync(ItemId, CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    var lst = await _salesOrderService.SyncBatchAsync(plantData.NavCompanyName, ItemNo);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            var Exits = lists.Where(w => w.BatchNo == s.BatchNo).Count();
                            if (Exits == 0)
                            {
                                await InsertBatchInfo(s, plantData.CompanyID, ItemId); 
                            }
                        }
                    }
                }
                itemBatchInfo.ItemBatchId = 1;
                return itemBatchInfo;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
