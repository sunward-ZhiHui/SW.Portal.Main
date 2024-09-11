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
using static iTextSharp.text.pdf.AcroFields;
using System.ComponentModel.Design;
using Google.Protobuf.Collections;

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

                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;

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
        public async Task<IReadOnlyList<ItemBatchInfo>> GetNavItemBatchNoByItemByAllAsync()
        {
            try
            {
                var query = "select t1.*,t3.PlantCode,t3.Description as PlantDescription,t2.No as ItemNo,t2.Description as ItemDescription,t2.Description2 as ItemDescription2 from ItemBatchInfo t1 \r\nJOIN Plant t3 ON t3.PlantId=t1.CompanyId JOIN NAVItems t2 ON t1.ItemId=t2.ItemId;";

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
        public async Task<ItemBatchInfo> InsertOrUpdateBatchInfo(ItemBatchInfo itemBatchInfo)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ItemBatchId", itemBatchInfo.ItemBatchId);
                    parameters.Add("ItemId", itemBatchInfo.ItemId);
                    parameters.Add("CompanyId", itemBatchInfo.CompanyId);
                    parameters.Add("LocationCode", itemBatchInfo.LocationCode, DbType.String);
                    parameters.Add("BatchNo", itemBatchInfo.BatchNo, DbType.String);
                    parameters.Add("LotNo", itemBatchInfo.LotNo, DbType.String);
                    parameters.Add("ExpiryDate", itemBatchInfo.ExpiryDate == DateTime.MinValue ? null : itemBatchInfo.ExpiryDate, DbType.DateTime);
                    parameters.Add("ManufacturingDate", itemBatchInfo.ManufacturingDate == DateTime.MinValue ? null : itemBatchInfo.ManufacturingDate, DbType.DateTime);
                    parameters.Add("QuantityOnHand", itemBatchInfo.QuantityOnHand, DbType.Decimal);
                    parameters.Add("NavQuantity", itemBatchInfo.NavQuantity, DbType.Decimal);
                    parameters.Add("IssueQuantity", itemBatchInfo.BalanceQuantity, DbType.Decimal);
                    parameters.Add("BalanceQuantity", itemBatchInfo.BalanceQuantity, DbType.Decimal);
                    parameters.Add("AddedByUserId", itemBatchInfo.AddedByUserId);
                    parameters.Add("ModifiedByUserId", itemBatchInfo.ModifiedByUserId);
                    parameters.Add("StatusCodeId", 1);
                    parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                    parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                    var lastInsertedRecordId = itemBatchInfo.ItemBatchId;
                    if (itemBatchInfo.ItemBatchId > 0)
                    {
                        var query = "Update  ItemBatchInfo SET ItemId=@ItemId,CompanyId=@CompanyId,LocationCode=@LocationCode,BatchNo=@BatchNo,LotNo=@LotNo,ExpiryDate=@ExpiryDate,ManufacturingDate=@ManufacturingDate,QuantityOnHand=@QuantityOnHand,NavQuantity=@NavQuantity,IssueQuantity=@IssueQuantity,BalanceQuantity=@BalanceQuantity,ModifiedByUserId=@ModifiedByUserId,ModifiedDate=@ModifiedDate WHERE ItemBatchId = @ItemBatchId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [ItemBatchInfo](ItemId,CompanyId,LocationCode,BatchNo,LotNo,ExpiryDate,ManufacturingDate,QuantityOnHand,NavQuantity,IssueQuantity,BalanceQuantity,StatusCodeId,AddedDate,AddedByUserId,ModifiedByUserId,ModifiedDate) OUTPUT INSERTED.ItemBatchId VALUES " +
                            "(@ItemId,@CompanyId,@LocationCode,@BatchNo,@LotNo,@ExpiryDate,@ManufacturingDate,@QuantityOnHand,@NavQuantity,@IssueQuantity,@BalanceQuantity,@StatusCodeId,@AddedDate,@AddedByUserId,@ModifiedByUserId,@ModifiedDate)";

                        itemBatchInfo.ItemBatchId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                    }
                    return itemBatchInfo;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ItemBatchInfo> DeleteItemBatchInfo(ItemBatchInfo itemBatchInfo)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ItemBatchId", itemBatchInfo.ItemBatchId);

                        var query = "DELETE  FROM ItemBatchInfo WHERE ItemBatchId = @ItemBatchId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return itemBatchInfo;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
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
        public async Task<IReadOnlyList<ItemBatchInfo>> GetNavItemBatchNoByCompanyIdAsync(long? CompanyId)
        {
            List<ItemBatchInfo> ItemBatchInfos = new List<ItemBatchInfo>();
            try
            {
                var query = "select  * from ItemBatchInfo where  CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<ItemBatchInfo>(query)).ToList();
                    ItemBatchInfos = result != null ? result : new List<ItemBatchInfo>();
                }
                return ItemBatchInfos;
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
                    var parameters = new DynamicParameters();
                    parameters.Add("ItemId", ItemId);
                    parameters.Add("CompanyId", CompanyId);
                    parameters.Add("LocationCode", itemBatchInfo.LocationCode);
                    parameters.Add("BatchNo", itemBatchInfo.BatchNo);
                    parameters.Add("LotNo", itemBatchInfo.LotNo);
                    parameters.Add("ExpiryDate", itemBatchInfo.ExpiryDate == DateTime.MinValue ? null : itemBatchInfo.ExpiryDate, DbType.DateTime);
                    parameters.Add("ManufacturingDate", itemBatchInfo.ManufacturingDate == DateTime.MinValue ? null : itemBatchInfo.ManufacturingDate, DbType.DateTime);
                    parameters.Add("QuantityOnHand", itemBatchInfo.QuantityOnHand, DbType.Decimal);
                    parameters.Add("NavQuantity", itemBatchInfo.NavQuantity, DbType.Decimal);
                    parameters.Add("IssueQuantity", itemBatchInfo.BalanceQuantity, DbType.Decimal);
                    parameters.Add("BalanceQuantity", itemBatchInfo.BalanceQuantity, DbType.Decimal);
                    parameters.Add("StatusCodeId", 1);
                    parameters.Add("AddedDate", DateTime.Now);

                    var query = "INSERT INTO [ItemBatchInfo](ItemId,CompanyId,LocationCode,BatchNo,LotNo,ExpiryDate,ManufacturingDate,QuantityOnHand,NavQuantity,IssueQuantity,BalanceQuantity,StatusCodeId,AddedDate) OUTPUT INSERTED.ItemBatchId VALUES " +
                        "(@ItemId,@CompanyId,@LocationCode,@BatchNo,@LotNo,@ExpiryDate,@ManufacturingDate,@QuantityOnHand,@NavQuantity,@IssueQuantity,@BalanceQuantity,@StatusCodeId,@AddedDate)";

                    var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                    return lastInsertedRecordId;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ItemBatchInfo> GetNavItemBatchInfo(long? CompanyId)
        {
            try
            {
                ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                var Navlists = await GetByCompanyAsyncList(CompanyId);
                var itemBatch = await GetNavItemBatchNoByCompanyIdAsync(CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    var lst = await _salesOrderService.NavItemBatchAsync(plantData.NavCompanyName);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            var Exits = Navlists.Where(w => w.No == s.ItemNo).FirstOrDefault();
                            if (Exits != null)
                            {
                                var itemBatchExits = itemBatch.Where(f => f.CompanyId == CompanyId && f.BatchNo == s.BatchNo && f.ItemId == Exits.ItemId && f.LocationCode == s.LocationCode).Count();
                                if (itemBatchExits > 0)
                                {

                                }
                                else
                                {
                                    await InsertBatchInfo(s, plantData.CompanyID, Exits.ItemId);
                                }
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
        public async Task<IReadOnlyList<Navitems>> GetNavItemItemNosAsync(long? CompanyId)
        {
            List<Navitems> ItemBatchInfos = new List<Navitems>();
            try
            {
                var query = "select  * from Navitems where  CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<Navitems>(query)).ToList();
                    ItemBatchInfos = result != null ? result : new List<Navitems>();
                }
                return ItemBatchInfos;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Navitems> GetNavItemServicesList(long? CompanyId)
        {
            try
            {
                Navitems itemBatchInfo = new Navitems();
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                var itemLists = await _salesOrderService.GetNavItemsAdd(plantData);
                if (itemLists != null && itemLists.Count() > 0)
                {
                    await SyncAndGetItems(itemLists, plantData.PlantCode, plantData.PlantID);
                }
                return itemBatchInfo;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task SyncAndGetItems(List<Navitems> items, string company, long? companyId)
        {

            List<string> itemsNos = items.Select(s => s.No.Trim().ToLower()).ToList();
            var navItemsData = await GetNavItemItemNosAsync(companyId);
            List<Navitems> existingNavitems = navItemsData.Where(n => itemsNos.Contains(n.No.Trim().ToLower()) && n.CompanyId == companyId).ToList();
            List<string> existingNos = existingNavitems.Select(n => n.No.Trim().ToLower()).ToList();
            List<Navitems> existingItems = items.Where(i => existingNos.Contains(i.No.Trim().ToLower())).ToList();
            List<Navitems> newItemLists = items.Where(i => !existingNos.Contains(i.No.Trim().ToLower())).ToList();
            List<Navitems> navItems = GenerateNewItems(newItemLists, company, companyId);
            List<Navitems> updatedNavItems = UpdateNavItems(existingNavitems, existingItems, company);

            List<Navitems> updatenavitemModelList = new List<Navitems>();


            if (updatedNavItems.Count > 0)
            {
                // SqlBulkUpload objUpdate = new SqlBulkUpload(_context, _config);
                // objUpdate.BulkUpdateData(updatedNavItemslist, "ItemId", new List<string>() { "AddedByUser" }, "Navitems");
            }

            //SqlBulkUpload objInsert = new SqlBulkUpload(_context, _config);
            // await objInsert.BulkInsertAsync(objnavItems, "Navitems");
        }
        private List<Navitems> GenerateNewItems(List<Navitems> newItemLists, string company, long? companyId)
        {
            List<Navitems> navitems = new List<Navitems>();
            newItemLists.ForEach(item =>
            {
                if (item.No == "FP-PP-CRM-088")
                {

                }
                Navitems newNavItem = new Navitems
                {
                    No = item.No,
                    RelatedItemNo = item.RelatedItemNo,
                    Description = item.Description,
                    Description2 = item.Description2,
                    ItemType = item.ItemType,
                    StatusCodeId = item.StatusCodeId,
                    Inventory = item.Inventory,
                    InternalRef = item.InternalRef,
                    ItemRegistration = item.ItemRegistration,
                    ExpirationCalculation = item.ExpirationCalculation,
                    BatchNos = item.BatchNos,
                    ProductionRecipeNo = item.ProductionRecipeNo,
                    Qcenabled = item.Qcenabled,
                    ProductionBomno = item.ProductionBomno,
                    RoutingNo = item.RoutingNo,
                    BaseUnitofMeasure = item.BaseUnitofMeasure,
                    UnitCost = item.UnitCost,
                    UnitPrice = item.UnitPrice,
                    VendorNo = item.VendorNo,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemTrackingCode = item.ItemTrackingCode,
                    Qclocation = item.Qclocation,
                    Company = company,
                    CompanyId = companyId,
                    PurchaseUom = item.PurchaseUom,
                    ShelfLife = item.ShelfLife,
                };
                navitems.Add(newNavItem);
            });
            return navitems;
        }

        private List<Navitems> UpdateNavItems(List<Navitems> existingNavitems, List<Navitems> existingItems, string company)
        {
            existingNavitems.ForEach(e =>
            {
                var item = existingItems.FirstOrDefault(i => i.No.Trim().ToLower() == e.No.Trim().ToLower());
                e.No = item.No;
                e.RelatedItemNo = item.RelatedItemNo;
                e.ShelfLife = item.ShelfLife;
                e.StatusCodeId = item.StatusCodeId;
                e.PurchaseUom = item.PurchaseUom;
            });
            return existingNavitems;
        }
        public async Task<IReadOnlyList<FinishedProdOrderLine>> GetFinishedProdOrderLineByCompanyIdAsync(long? CompanyId)
        {
            List<FinishedProdOrderLine> ItemBatchInfos = new List<FinishedProdOrderLine>();
            try
            {
                var query = "select  * from FinishedProdOrderLine where  CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<FinishedProdOrderLine>(query)).ToList();
                    ItemBatchInfos = result != null ? result : new List<FinishedProdOrderLine>();
                }
                return ItemBatchInfos;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<FinishedProdOrderLine> GetFinishedProdOrderLineList(long? CompanyId)
        {
            try
            {
                FinishedProdOrderLine finishedProdOrderLine = new FinishedProdOrderLine();
                var FinishedProdOrderLineData = await GetFinishedProdOrderLineByCompanyIdAsync(CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    List<FinishedProdOrderLine> FinishedProdOrderLineDatas = FinishedProdOrderLineData != null ? FinishedProdOrderLineData.ToList() : new List<FinishedProdOrderLine>();
                    var lst = await _salesOrderService.FinishedProdOrderLineAsync(plantData.NavCompanyName, plantData.PlantID, FinishedProdOrderLineDatas);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            await InsertFinishedProdOrderLine(s);
                        }
                    }
                }
                finishedProdOrderLine.FinishedProdOrderLineId = 1;
                return finishedProdOrderLine;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertFinishedProdOrderLine(FinishedProdOrderLine finishedProdOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("FinishedProdOrderLineId", finishedProdOrderLine.FinishedProdOrderLineId);
                    parameters.Add("ItemNo", finishedProdOrderLine.ItemNo, DbType.String);
                    parameters.Add("Status", finishedProdOrderLine.Status, DbType.String);
                    parameters.Add("CompanyId", finishedProdOrderLine.CompanyId);
                    parameters.Add("ProdOrderNo", finishedProdOrderLine.ProdOrderNo, DbType.String);
                    parameters.Add("BatchNo", finishedProdOrderLine.BatchNo, DbType.String);
                    parameters.Add("Description", finishedProdOrderLine.Description, DbType.String);
                    parameters.Add("Description2", finishedProdOrderLine.Description2, DbType.String);
                    parameters.Add("ReplanRefNo", finishedProdOrderLine.ReplanRefNo, DbType.String);
                    parameters.Add("OrderLineNo", finishedProdOrderLine.OrderLineNo);
                    parameters.Add("StartingDate", finishedProdOrderLine.StartingDate, DbType.DateTime);
                    parameters.Add("ManufacturingDate", finishedProdOrderLine.ManufacturingDate, DbType.DateTime);
                    parameters.Add("ExpirationDate", finishedProdOrderLine.ExpirationDate, DbType.DateTime);
                    parameters.Add("ProductCode", finishedProdOrderLine.ProductCode, DbType.String);
                    parameters.Add("ProductName", finishedProdOrderLine.ProductName, DbType.String);
                    var lastInsertedRecordId = finishedProdOrderLine.FinishedProdOrderLineId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  FinishedProdOrderLine SET ItemNo=@ItemNo,Status=@Status,CompanyId=@CompanyId,ProdOrderNo=@ProdOrderNo,BatchNo=@BatchNo,Description=@Description,Description2=@Description2,ReplanRefNo=@ReplanRefNo,OrderLineNo=@OrderLineNo," +
                            "StartingDate=@StartingDate,ManufacturingDate=@ManufacturingDate,ExpirationDate=@ExpirationDate,ProductCode=@ProductCode,ProductName=@ProductName  WHERE FinishedProdOrderLineId =@FinishedProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);

                    }
                    else
                    {
                        var query = "INSERT INTO [FinishedProdOrderLine](ItemNo,CompanyId,Status,ProdOrderNo,Description,Description2,ReplanRefNo,OrderLineNo,StartingDate,ManufacturingDate,ExpirationDate,ProductCode,ProductName) OUTPUT INSERTED.FinishedProdOrderLineId VALUES " +
                            "(@ItemNo,@CompanyId,@Status,@ProdOrderNo,@Description,@Description2,@ReplanRefNo,@OrderLineNo,@StartingDate,@ManufacturingDate,@ExpirationDate,@ProductCode,@ProductName)";
                        lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                    }
                    return lastInsertedRecordId;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
