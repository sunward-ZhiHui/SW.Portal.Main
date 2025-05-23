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
using Core.EntityModels;

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
                var query = "select t1.*,t2.PlantCode,t2.Description as PlantDescription from NAVItems t1 LEFT JOIN Plant t2 ON t2.PlantID=t1.CompanyId";

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

        public async Task<IReadOnlyList<RawMatItemList>> GetRawMatItemListByTypeList(string? Type, long? CompanyId)
        {
            try
            {
                List<RawMatItemList> rawMatItemLists = new List<RawMatItemList>();
                if (!string.IsNullOrEmpty(Type))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Type", Type, DbType.String);
                    parameters.Add("CompanyId", CompanyId);
                    var query = "select  *,CONCAT(ItemNo,'|',Description) as ItemNoDescription from RawMatItemList where CompanyId=@CompanyId AND Type=@Type";

                    using (var connection = CreateConnection())
                    {
                        rawMatItemLists = (await connection.QueryAsync<RawMatItemList>(query, parameters)).ToList();
                    }
                }
                return rawMatItemLists;
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
                                var itemBatchExits = itemBatch.Where(f => f.CompanyId == CompanyId && f.LotNo == s.LotNo && f.BatchNo == s.BatchNo && f.ItemId == Exits.ItemId && f.LocationCode == s.LocationCode).Count();
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
        public async Task<Navitems> GetNavItemServicesList(long? CompanyId, long? UserId)
        {
            try
            {
                Navitems itemBatchInfo = new Navitems();
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                var itemLists = await _salesOrderService.GetNavItemsAdd(plantData);
                if (itemLists != null && itemLists.Count() > 0)
                {
                    await SyncAndGetItems(itemLists, plantData.PlantCode, plantData.PlantID, UserId);
                }
                return itemBatchInfo;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task SyncAndGetItems(List<Navitems> items, string company, long? companyId, long? UserId)
        {

            List<string> itemsNos = items.Select(s => s.No.Trim().ToLower()).ToList();
            var navItemsData = await GetNavItemItemNosAsync(companyId);
            if (items != null && items.Count() > 0)
            {
                foreach (var item in items)
                {
                    var exits = navItemsData.FirstOrDefault(f => f.No.Trim().ToLower() == item.No.Trim().ToLower());
                    if (exits != null)
                    {
                        item.ItemId = exits.ItemId;
                        await InsertOrUpdateNavItems(item, companyId, UserId);
                    }
                    else
                    {
                        await InsertOrUpdateNavItems(item, companyId, UserId);
                    }
                }
            }

        }
        public async Task<long> InsertOrUpdateNavItems(Navitems navitems, long? companyId, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    if (navitems.ItemId > 0)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ItemId", navitems.ItemId);
                        parameters.Add("No", navitems.No, DbType.String);
                        parameters.Add("RelatedItemNo", navitems.RelatedItemNo, DbType.String);
                        parameters.Add("PurchaseUom", navitems.PurchaseUom, DbType.String);
                        parameters.Add("ShelfLife", navitems.ShelfLife, DbType.String);
                        parameters.Add("AddedByUserId", navitems.AddedByUserId == null ? UserId : navitems.AddedByUserId);
                        parameters.Add("ModifiedByUserId", navitems.ModifiedByUserId == null ? UserId : navitems.ModifiedByUserId);
                        parameters.Add("AddedDate", navitems.AddedDate == null ? DateTime.Now : navitems.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", navitems.ModifiedDate == null ? DateTime.Now : navitems.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeId", navitems.StatusCodeId);
                        var query1 = "Update  NavItems SET No=@No,RelatedItemNo=@RelatedItemNo,ModifiedDate=@ModifiedDate,AddedDate=@AddedDate,ModifiedByUserId=@ModifiedByUserId,AddedByUserId=@AddedByUserId,ShelfLife=@ShelfLife," +
                            "PurchaseUom=@PurchaseUom,StatusCodeId=@StatusCodeId  WHERE ItemId =@ItemId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);
                        return navitems.ItemId;
                    }
                    else
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ItemId", navitems.ItemId);
                        parameters.Add("No", navitems.No, DbType.String);
                        parameters.Add("RelatedItemNo", navitems.RelatedItemNo, DbType.String);
                        parameters.Add("Description", navitems.Description, DbType.String);
                        parameters.Add("Description2", navitems.Description2, DbType.String);
                        parameters.Add("ItemType", navitems.ItemType, DbType.String);
                        parameters.Add("StatusCodeId", navitems.StatusCodeId);
                        parameters.Add("Inventory", navitems.Inventory);
                        parameters.Add("InternalRef", navitems.InternalRef, DbType.String);
                        parameters.Add("ItemRegistration", navitems.ItemRegistration, DbType.String);
                        parameters.Add("ExpirationCalculation", navitems.ExpirationCalculation, DbType.String);
                        parameters.Add("BatchNos", navitems.BatchNos, DbType.String);
                        parameters.Add("ProductionRecipeNo", navitems.ProductionRecipeNo, DbType.String);
                        parameters.Add("Qcenabled", navitems.Qcenabled);
                        parameters.Add("ProductionBomno", navitems.ProductionBomno, DbType.String);
                        parameters.Add("RoutingNo", navitems.RoutingNo, DbType.String);
                        parameters.Add("BaseUnitofMeasure", navitems.BaseUnitofMeasure, DbType.String);
                        parameters.Add("UnitCost", navitems.UnitCost);
                        parameters.Add("UnitPrice", navitems.UnitPrice);
                        parameters.Add("VendorNo", navitems.VendorNo, DbType.String);
                        parameters.Add("ItemCategoryCode", navitems.ItemCategoryCode, DbType.String);
                        parameters.Add("ItemTrackingCode", navitems.ItemTrackingCode, DbType.String);
                        parameters.Add("Qclocation", navitems.Qclocation, DbType.String);
                        parameters.Add("CompanyId", companyId);
                        parameters.Add("PurchaseUom", navitems.PurchaseUom, DbType.String);
                        parameters.Add("ShelfLife", navitems.ShelfLife, DbType.String);
                        parameters.Add("AddedByUserId", navitems.AddedByUserId == null ? UserId : navitems.AddedByUserId);
                        parameters.Add("ModifiedByUserId", navitems.ModifiedByUserId == null ? UserId : navitems.ModifiedByUserId);
                        parameters.Add("AddedDate", navitems.AddedDate == null ? DateTime.Now : navitems.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", navitems.ModifiedDate == null ? DateTime.Now : navitems.ModifiedDate, DbType.DateTime);
                        var query = "INSERT INTO [NavItems](ModifiedDate,AddedDate,ModifiedByUserId,AddedByUserId,ShelfLife,PurchaseUom,CompanyId,Qclocation,ItemTrackingCode,ItemCategoryCode,VendorNo,UnitPrice,UnitCost,BaseUnitofMeasure,RoutingNo,ProductionBomno,No,RelatedItemNo,Description,Description2,ItemType,StatusCodeId,Inventory,InternalRef,ItemRegistration,ExpirationCalculation,BatchNos,ProductionRecipeNo,Qcenabled) OUTPUT INSERTED.ItemId VALUES " +
                            "(@ModifiedDate,@AddedDate,@ModifiedByUserId,@AddedByUserId,@ShelfLife,@PurchaseUom,@CompanyId,@Qclocation,@ItemTrackingCode,@ItemCategoryCode,@VendorNo,@UnitPrice,@UnitCost,@BaseUnitofMeasure,@RoutingNo,@ProductionBomno,@No,@RelatedItemNo,@Description,@Description2,@ItemType,@StatusCodeId,@Inventory,@InternalRef,@ItemRegistration,@ExpirationCalculation,@BatchNos,@ProductionRecipeNo,@Qcenabled)";
                        var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        return lastInsertedRecordId;

                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
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
                var navItemsData = await GetNavItemItemNosAsync(CompanyId);
                if (plantData != null)
                {
                    List<Navitems> navItemsDatas = navItemsData != null && navItemsData.Count() > 0 ? navItemsData.ToList() : new List<Navitems>();
                    List<FinishedProdOrderLine> FinishedProdOrderLineDatas = FinishedProdOrderLineData != null ? FinishedProdOrderLineData.ToList() : new List<FinishedProdOrderLine>();
                    var lst = await _salesOrderService.FinishedProdOrderLineAsync(plantData.NavCompanyName, plantData.PlantID, FinishedProdOrderLineDatas, navItemsDatas);
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
                    parameters.Add("ItemId", finishedProdOrderLine.ItemId);
                    parameters.Add("OptStatus", finishedProdOrderLine.OptStatus, DbType.String);
                    var lastInsertedRecordId = finishedProdOrderLine.FinishedProdOrderLineId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  FinishedProdOrderLine SET OptStatus=@OptStatus,ItemId=@ItemId,ItemNo=@ItemNo,Status=@Status,CompanyId=@CompanyId,ProdOrderNo=@ProdOrderNo,BatchNo=@BatchNo,Description=@Description,Description2=@Description2,ReplanRefNo=@ReplanRefNo,OrderLineNo=@OrderLineNo," +
                            "StartingDate=@StartingDate,ManufacturingDate=@ManufacturingDate,ExpirationDate=@ExpirationDate,ProductCode=@ProductCode,ProductName=@ProductName  WHERE FinishedProdOrderLineId =@FinishedProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);

                    }
                    else
                    {
                        var query = "INSERT INTO [FinishedProdOrderLine](BatchNo,OptStatus,ItemId,ItemNo,CompanyId,Status,ProdOrderNo,Description,Description2,ReplanRefNo,OrderLineNo,StartingDate,ManufacturingDate,ExpirationDate,ProductCode,ProductName) OUTPUT INSERTED.FinishedProdOrderLineId VALUES " +
                            "(@BatchNo,@OptStatus,@ItemId,@ItemNo,@CompanyId,@Status,@ProdOrderNo,@Description,@Description2,@ReplanRefNo,@OrderLineNo,@StartingDate,@ManufacturingDate,@ExpirationDate,@ProductCode,@ProductName)";
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
        public async Task<IReadOnlyList<FinishedProdOrderLine>> DeleteFinishedProdOrderLine(long? CompanyId)
        {
            List<FinishedProdOrderLine> ItemBatchInfos = new List<FinishedProdOrderLine>();
            try
            {
                var query = "select tt2.*,(right(tt2.ProductName, len(tt2.ProductName) - charindex(',', tt2.ProductName))) as ProductCode from (select tt1.*,\r\nProductName = STUFF((\r\n          SELECT ',' + CAST(md.FinishedProdOrderLineID AS VARCHAR(MAX))\r\n          FROM FinishedProdOrderLine md\r\n          WHERE md.CompanyID=tt1.CompanyID AND md.ProdOrderNo=tt1.ProdOrderNo AND md.ReplanRefNo=tt1.ReplanRefNo AND md.ItemNo=tt1.ItemNo\r\n\t\t  Order by md.FinishedProdOrderLineID asc\r\n          FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')\r\nfrom (select t1.ProdOrderNo,t1.ReplanRefNo,t1.ItemNo,t1.CompanyID,count(t1.ProdOrderNo) Counts from FinishedProdOrderLine t1 where t1.CompanyID=" + CompanyId + " group by t1.ProdOrderNo,t1.ReplanRefNo,t1.ItemNo,t1.CompanyID)tt1 where Counts>1 )tt2 order by tt2.Counts desc;";
                query += "\r\nselect tt2.*,\r\n(CONCAT('DELETE FROM ItemBatchInfo WHERE ItemBatchId IN(',right(tt2.ProductName, len(tt2.ProductName) - charindex(',', tt2.ProductName)),')')) as ProductCode from(select tt1.* ,\r\nProductName = STUFF((\r\n          SELECT ',' + CAST(md.ItemBatchId AS VARCHAR(MAX))\r\n          FROM ItemBatchInfo md\r\n          WHERE md.CompanyID=tt1.CompanyID AND md.BatchNo=tt1.BatchNo AND md.CompanyId=tt1.CompanyId AND md.LocationCode=tt1.LocationCode AND md.ItemId=tt1.ItemId\r\n\t\t  Order by md.ItemBatchId asc\r\n          FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')\r\nfrom \r\n(select t1.BatchNo,t1.CompanyId,t1.LocationCode,t1.ItemId,COUNT(t1.ItemId) as Counts from ItemBatchInfo t1 where CAST(t1.AddedDate as Date)>='2024-09-05' AND t1.CompanyId=2 group by t1.BatchNo,t1.CompanyId,t1.LocationCode,t1.ItemId)tt1 where tt1.Counts>1)tt2 order by tt2.Counts desc";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<FinishedProdOrderLine>(query)).ToList();
                    ItemBatchInfos = result != null ? result : new List<FinishedProdOrderLine>();
                }
                if (ItemBatchInfos != null && ItemBatchInfos.Count() > 0)
                {
                    var query1 = string.Empty;
                    ItemBatchInfos.ForEach(s =>
                    {
                        query1 += "\n\r";
                    });
                }
                return ItemBatchInfos;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<NavprodOrderLine>> GetNavprodOrderLineAsync(long? CompanyId)
        {
            List<NavprodOrderLine> NavprodOrderLineList = new List<NavprodOrderLine>();
            try
            {
                var query = "select  * from NavprodOrderLine where  CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<NavprodOrderLine>(query)).ToList();
                    NavprodOrderLineList = result != null ? result : new List<NavprodOrderLine>();
                }
                return NavprodOrderLineList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        /*public async Task<NavprodOrderLine> GetNavprodOrderLineList(long? CompanyId)
        {
            try
            {
                NavprodOrderLine finishedProdOrderLine = new NavprodOrderLine();
                var NavprodOrderLineData = await GetNavprodOrderLineAsync(CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    List<NavprodOrderLine> NavprodOrderLineDatas = NavprodOrderLineData != null ? NavprodOrderLineData.ToList() : new List<NavprodOrderLine>();
                    var lst = await _salesOrderService.GetNAVProdOrderLine(plantData.NavCompanyName, plantData.PlantID, NavprodOrderLineDatas);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            await InsertNavprodOrderLine(s);
                        }
                    }
                }
                finishedProdOrderLine.NavprodOrderLineId = 1;
                return finishedProdOrderLine;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }*/
        /*public async Task<long> InsertNavprodOrderLine(NavprodOrderLine finishedProdOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("NAVProdOrderLineId", finishedProdOrderLine.NavprodOrderLineId);
                    parameters.Add("Status", finishedProdOrderLine.Status, DbType.String);
                    parameters.Add("ProdOrderNo", finishedProdOrderLine.ProdOrderNo, DbType.String);
                    parameters.Add("OrderLineNo", finishedProdOrderLine.OrderLineNo);
                    parameters.Add("ItemNo", finishedProdOrderLine.ItemNo, DbType.String);
                    parameters.Add("Description", finishedProdOrderLine.Description, DbType.String);
                    parameters.Add("Description1", finishedProdOrderLine.Description1, DbType.String);
                    parameters.Add("CompletionDate", finishedProdOrderLine.CompletionDate, DbType.DateTime);
                    parameters.Add("RemainingQuantity", finishedProdOrderLine.RemainingQuantity);
                    parameters.Add("UnitofMeasureCode", finishedProdOrderLine.UnitofMeasureCode, DbType.String);
                    parameters.Add("LastSyncDate", finishedProdOrderLine.LastSyncDate, DbType.DateTime);
                    parameters.Add("BatchNo", finishedProdOrderLine.BatchNo, DbType.String);
                    parameters.Add("LastSyncUserId", finishedProdOrderLine.LastSyncUserId);
                    parameters.Add("ReplanRefNo", finishedProdOrderLine.RePlanRefNo, DbType.String);
                    parameters.Add("StartDate", finishedProdOrderLine.StartDate, DbType.DateTime);
                    parameters.Add("OutputQty", finishedProdOrderLine.OutputQty);
                    parameters.Add("CompanyId", finishedProdOrderLine.CompanyId);
                    var lastInsertedRecordId = finishedProdOrderLine.NavprodOrderLineId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  NavprodOrderLine SET Status=@Status,ProdOrderNo=@ProdOrderNo,OrderLineNo=@OrderLineNo,ItemNo=@ItemNo,Description=@Description,Description1=@Description1,CompletionDate=@CompletionDate," +
                            "RemainingQuantity=@RemainingQuantity,UnitofMeasureCode=@UnitofMeasureCode,LastSyncDate=@LastSyncDate,BatchNo=@BatchNo," +
                            "LastSyncUserId=@LastSyncUserId,ReplanRefNo=@ReplanRefNo,StartDate=@StartDate,OutputQty=@OutputQty,CompanyId=@CompanyId  WHERE NAVProdOrderLineId =@NAVProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [NavprodOrderLine](Status,ProdOrderNo,OrderLineNo,ItemNo,Description,Description1,CompletionDate,RemainingQuantity,UnitofMeasureCode,LastSyncDate,BatchNo,LastSyncUserId,ReplanRefNo,StartDate,OutputQty,CompanyId) " +
                            "OUTPUT INSERTED.NAVProdOrderLineId VALUES " +
                            "(@Status,@ProdOrderNo,@OrderLineNo,@ItemNo,@Description,@Description1,@CompletionDate,@RemainingQuantity,@UnitofMeasureCode,@LastSyncDate,@BatchNo,@LastSyncUserId,@ReplanRefNo,@StartDate,@OutputQty,@CompanyId)";
                        lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                    }
                    return lastInsertedRecordId;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }*/
        public async Task<NavprodOrderLine> GetNavprodOrderLineList(long? CompanyId)
        {
            try
            {
                NavprodOrderLine finishedProdOrderLine = new NavprodOrderLine();
                var NavprodOrderLineData = await GetNavprodOrderLineAsync(CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    List<NavprodOrderLine> NavprodOrderLineDatas = NavprodOrderLineData != null ? NavprodOrderLineData.ToList() : new List<NavprodOrderLine>();
                    var lst = await _salesOrderService.GetFinishedProdOrderLineToNAVProdOrderLine(plantData.NavCompanyName, plantData.PlantID, NavprodOrderLineDatas);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            await InsertNavprodOrderLine(s);
                        }
                    }
                }
                finishedProdOrderLine.NavprodOrderLineId = 1;
                return finishedProdOrderLine;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertNavprodOrderLine(NavprodOrderLine finishedProdOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("NAVProdOrderLineId", finishedProdOrderLine.NavprodOrderLineId);
                    parameters.Add("Status", finishedProdOrderLine.Status, DbType.String);
                    parameters.Add("ProdOrderNo", finishedProdOrderLine.ProdOrderNo, DbType.String);
                    parameters.Add("OrderLineNo", finishedProdOrderLine.OrderLineNo);
                    parameters.Add("ItemNo", finishedProdOrderLine.ItemNo, DbType.String);
                    parameters.Add("Description", finishedProdOrderLine.Description, DbType.String);
                    parameters.Add("Description1", finishedProdOrderLine.Description1, DbType.String);
                    parameters.Add("CompletionDate", finishedProdOrderLine.CompletionDate, DbType.DateTime);
                    parameters.Add("RemainingQuantity", finishedProdOrderLine.RemainingQuantity);
                    parameters.Add("UnitofMeasureCode", finishedProdOrderLine.UnitofMeasureCode, DbType.String);
                    parameters.Add("LastSyncDate", finishedProdOrderLine.LastSyncDate, DbType.DateTime);
                    parameters.Add("BatchNo", finishedProdOrderLine.BatchNo, DbType.String);
                    parameters.Add("LastSyncUserId", finishedProdOrderLine.LastSyncUserId);
                    parameters.Add("ReplanRefNo", finishedProdOrderLine.RePlanRefNo, DbType.String);
                    parameters.Add("StartDate", finishedProdOrderLine.StartDate, DbType.DateTime);
                    parameters.Add("OutputQty", finishedProdOrderLine.OutputQty);
                    parameters.Add("CompanyId", finishedProdOrderLine.CompanyId);
                    var lastInsertedRecordId = finishedProdOrderLine.NavprodOrderLineId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  NavprodOrderLine SET Status=@Status,ProdOrderNo=@ProdOrderNo,OrderLineNo=@OrderLineNo,ItemNo=@ItemNo,Description=@Description,Description1=@Description1," +
                            "UnitofMeasureCode=@UnitofMeasureCode,LastSyncDate=@LastSyncDate,BatchNo=@BatchNo," +
                            "LastSyncUserId=@LastSyncUserId,ReplanRefNo=@ReplanRefNo,StartDate=@StartDate,CompanyId=@CompanyId  WHERE NAVProdOrderLineId =@NAVProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [NavprodOrderLine](Status,ProdOrderNo,OrderLineNo,ItemNo,Description,Description1,UnitofMeasureCode,LastSyncDate,BatchNo,LastSyncUserId,ReplanRefNo,StartDate,CompanyId) " +
                            "OUTPUT INSERTED.NAVProdOrderLineId VALUES " +
                            "(@Status,@ProdOrderNo,@OrderLineNo,@ItemNo,@Description,@Description1,@UnitofMeasureCode,@LastSyncDate,@BatchNo,@LastSyncUserId,@ReplanRefNo,@StartDate,@CompanyId)";
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
        public async Task<IReadOnlyList<NavprodOrderLine>> GetNavprodOrderLineListAsync()
        {
            List<NavprodOrderLine> NavprodOrderLineList = new List<NavprodOrderLine>();
            try
            {
                var query = "select t1.*,t2.PlantCode as CompanyName from NAVProdOrderLine t1 \r\nLEFT JOIN Plant t2 ON t2.PlantID=t1.CompanyID";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<NavprodOrderLine>(query)).ToList();
                    NavprodOrderLineList = result != null ? result : new List<NavprodOrderLine>();
                }
                return NavprodOrderLineList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<NavprodOrderLine> InsertOrUpdateNavprodOrderLine(NavprodOrderLine finishedProdOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("NAVProdOrderLineId", finishedProdOrderLine.NavprodOrderLineId);
                    parameters.Add("Status", finishedProdOrderLine.Status, DbType.String);
                    parameters.Add("ProdOrderNo", finishedProdOrderLine.ProdOrderNo, DbType.String);
                    parameters.Add("OrderLineNo", finishedProdOrderLine.OrderLineNo);
                    parameters.Add("ItemNo", finishedProdOrderLine.ItemNo, DbType.String);
                    parameters.Add("Description", finishedProdOrderLine.Description, DbType.String);
                    parameters.Add("Description1", finishedProdOrderLine.Description1, DbType.String);
                    parameters.Add("CompletionDate", finishedProdOrderLine.CompletionDate, DbType.DateTime);
                    parameters.Add("RemainingQuantity", finishedProdOrderLine.RemainingQuantity);
                    parameters.Add("UnitofMeasureCode", finishedProdOrderLine.UnitofMeasureCode, DbType.String);
                    parameters.Add("LastSyncDate", finishedProdOrderLine.LastSyncDate, DbType.DateTime);
                    parameters.Add("BatchNo", finishedProdOrderLine.BatchNo, DbType.String);
                    parameters.Add("LastSyncUserId", finishedProdOrderLine.LastSyncUserId);
                    parameters.Add("ReplanRefNo", finishedProdOrderLine.RePlanRefNo, DbType.String);
                    parameters.Add("StartDate", finishedProdOrderLine.StartDate, DbType.DateTime);
                    parameters.Add("OutputQty", finishedProdOrderLine.OutputQty);
                    parameters.Add("CompanyId", finishedProdOrderLine.CompanyId);
                    var lastInsertedRecordId = finishedProdOrderLine.NavprodOrderLineId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  NavprodOrderLine SET Status=@Status,ProdOrderNo=@ProdOrderNo,OrderLineNo=@OrderLineNo,ItemNo=@ItemNo,Description=@Description,Description1=@Description1," +
                            "UnitofMeasureCode=@UnitofMeasureCode,LastSyncDate=@LastSyncDate,BatchNo=@BatchNo," +
                            "LastSyncUserId=@LastSyncUserId,ReplanRefNo=@ReplanRefNo,StartDate=@StartDate,CompanyId=@CompanyId  WHERE NAVProdOrderLineId =@NAVProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [NavprodOrderLine](Status,ProdOrderNo,OrderLineNo,ItemNo,Description,Description1,UnitofMeasureCode,LastSyncDate,BatchNo,LastSyncUserId,ReplanRefNo,StartDate,CompanyId) " +
                            "OUTPUT INSERTED.NAVProdOrderLineId VALUES " +
                            "(@Status,@ProdOrderNo,@OrderLineNo,@ItemNo,@Description,@Description1,@UnitofMeasureCode,@LastSyncDate,@BatchNo,@LastSyncUserId,@ReplanRefNo,@StartDate,@CompanyId)";
                        lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        finishedProdOrderLine.NavprodOrderLineId = lastInsertedRecordId;
                    }
                    return finishedProdOrderLine;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<NavprodOrderLine> DeleteNavprodOrderLine(NavprodOrderLine navprodOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("NavprodOrderLineId", navprodOrderLine.NavprodOrderLineId);
                        var query = "DELETE FROM NavprodOrderLine WHERE NavprodOrderLineId = @NavprodOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return navprodOrderLine;
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


        public async Task<IReadOnlyList<FinishedProdOrderLineOptStatus>> GetFinishedProdOrderLineOptStatus()
        {
            try
            {
                var query = "select  * from View_FinishedProdOrderLineOptStatus";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<FinishedProdOrderLineOptStatus>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<FinishedProdOrderLine>> GeFinishedProdOrderLineListAsync()
        {
            List<FinishedProdOrderLine> NavprodOrderLineList = new List<FinishedProdOrderLine>();
            try
            {

                var query = "select t1.*,t2.PlantCode as CompanyCode from FinishedProdOrderLine t1 \r\nLEFT JOIN Plant t2 ON t1.CompanyID=t2.PlantID\r\nLEFT JOIN NAVItems t3 ON t1.ItemID=t3.ItemId";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<FinishedProdOrderLine>(query)).ToList();
                    NavprodOrderLineList = result != null ? result : new List<FinishedProdOrderLine>();
                }
                return NavprodOrderLineList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<FinishedProdOrderLine> InsertOrUpdateFinishedProdOrderLine(FinishedProdOrderLine finishedProdOrderLine)
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
                    parameters.Add("ItemId", finishedProdOrderLine.ItemId);
                    parameters.Add("OptStatus", finishedProdOrderLine.OptStatus, DbType.String);
                    var lastInsertedRecordId = finishedProdOrderLine.FinishedProdOrderLineId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  FinishedProdOrderLine SET OptStatus=@OptStatus,ItemId=@ItemId,ItemNo=@ItemNo,Status=@Status,CompanyId=@CompanyId,ProdOrderNo=@ProdOrderNo,BatchNo=@BatchNo,Description=@Description,Description2=@Description2,ReplanRefNo=@ReplanRefNo,OrderLineNo=@OrderLineNo," +
                            "StartingDate=@StartingDate,ManufacturingDate=@ManufacturingDate,ExpirationDate=@ExpirationDate,ProductCode=@ProductCode,ProductName=@ProductName  WHERE FinishedProdOrderLineId =@FinishedProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);

                    }
                    else
                    {
                        var query = "INSERT INTO [FinishedProdOrderLine](BatchNo,OptStatus,ItemId,ItemNo,CompanyId,Status,ProdOrderNo,Description,Description2,ReplanRefNo,OrderLineNo,StartingDate,ManufacturingDate,ExpirationDate,ProductCode,ProductName) OUTPUT INSERTED.FinishedProdOrderLineId VALUES " +
                            "(@BatchNo,@OptStatus,@ItemId,@ItemNo,@CompanyId,@Status,@ProdOrderNo,@Description,@Description2,@ReplanRefNo,@OrderLineNo,@StartingDate,@ManufacturingDate,@ExpirationDate,@ProductCode,@ProductName)";
                        lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        finishedProdOrderLine.FinishedProdOrderLineId = lastInsertedRecordId;
                    }
                    return finishedProdOrderLine;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<FinishedProdOrderLine> DeleteFinishedProdOrderLine(FinishedProdOrderLine navprodOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("FinishedProdOrderLineId", navprodOrderLine.FinishedProdOrderLineId);
                        var query = "DELETE FROM FinishedProdOrderLine WHERE FinishedProdOrderLineId = @FinishedProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return navprodOrderLine;
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
        public async Task<IReadOnlyList<SoCustomer>> GetNavVendorAsync(long? CompanyId)
        {
            List<SoCustomer> NavprodOrderLineList = new List<SoCustomer>();
            try
            {
                var query = "select  * from SoCustomer where  Type='Vendor' AND  CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<SoCustomer>(query)).ToList();
                    NavprodOrderLineList = result != null ? result : new List<SoCustomer>();
                }
                return NavprodOrderLineList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SoCustomer> GetNavVendorList(long? CompanyId)
        {
            try
            {
                SoCustomer navVendor = new SoCustomer();
                var VendorDataData = await GetNavVendorAsync(CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    List<SoCustomer> VendorDataDatas = VendorDataData != null ? VendorDataData.ToList() : new List<SoCustomer>();
                    var lst = await _salesOrderService.NavVendorAsync(plantData.NavCompanyName, plantData.PlantID, VendorDataDatas);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            await InsertOrUpdateNavVendor(s);
                        }
                    }
                }
                navVendor.SoCustomerId = 1;
                return navVendor;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertOrUpdateNavVendor(SoCustomer finishedProdOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("SoCustomerId", finishedProdOrderLine.SoCustomerId);
                    parameters.Add("ShipCode", finishedProdOrderLine.ShipCode, DbType.String);
                    parameters.Add("CustomerName", finishedProdOrderLine.CustomerName, DbType.String);
                    parameters.Add("Address1", finishedProdOrderLine.Address1, DbType.String);
                    parameters.Add("Address2", finishedProdOrderLine.Address2, DbType.String);
                    parameters.Add("PostCode", finishedProdOrderLine.PostCode, DbType.String);
                    parameters.Add("City", finishedProdOrderLine.City, DbType.String);
                    parameters.Add("StateCode", finishedProdOrderLine.StateCode, DbType.String);
                    parameters.Add("Type", finishedProdOrderLine.Type, DbType.String);
                    parameters.Add("CompanyId", finishedProdOrderLine.CompanyId);
                    var lastInsertedRecordId = finishedProdOrderLine.SoCustomerId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  SoCustomer SET ShipCode=@ShipCode,CustomerName=@CustomerName,Address1=@Address1,Address2=@Address2,PostCode=@PostCode,City=@City," +
                            "StateCode=@StateCode,Type=@Type,CompanyId=@CompanyId  WHERE SoCustomerId =@SoCustomerId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [SoCustomer](ShipCode,CustomerName,Address1,Address2,PostCode,City,StateCode,Type,CompanyId) " +
                            "OUTPUT INSERTED.SoCustomerId VALUES " +
                            "(@ShipCode,@CustomerName,@Address1,@Address2,@PostCode,@City,@StateCode,@Type,@CompanyId)";
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

        public async Task<IReadOnlyList<RawMatPurch>> GetRawMatPurchAsync(long? CompanyId)
        {
            List<RawMatPurch> NavprodOrderLineList = new List<RawMatPurch>();
            try
            {
                var query = "select  * from RawMatPurch where CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<RawMatPurch>(query)).ToList();
                    NavprodOrderLineList = result != null ? result : new List<RawMatPurch>();
                }
                return NavprodOrderLineList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<RawMatPurch> GetRawMatPurchList(long? CompanyId)
        {
            try
            {
                RawMatPurch rawMatPurch = new RawMatPurch();
                var rawMatPurchData = await GetRawMatPurchAsync(CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    var navItemsData = await GetNavItemItemNosAsync(CompanyId);
                    List<Navitems> navItemsDatas = navItemsData != null && navItemsData.Count() > 0 ? navItemsData.ToList() : new List<Navitems>();
                    List<RawMatPurch> RawMatPurchDatas = rawMatPurchData != null ? rawMatPurchData.ToList() : new List<RawMatPurch>();
                    var lst = await _salesOrderService.GetRawMatPurchAsync(plantData.NavCompanyName, plantData.PlantID, RawMatPurchDatas, navItemsDatas);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            await InsertOrUpdateRawMatPurch(s);
                        }
                    }
                }
                rawMatPurch.RawMatPurchId = 1;
                return rawMatPurch;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertOrUpdateRawMatPurch(RawMatPurch finishedProdOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("RawMatPurchId", finishedProdOrderLine.RawMatPurchId);
                    parameters.Add("ItemNo", finishedProdOrderLine.ItemNo, DbType.String);
                    parameters.Add("BatchNo", finishedProdOrderLine.BatchNo, DbType.String);
                    parameters.Add("Description", finishedProdOrderLine.Description, DbType.String);
                    parameters.Add("Description2", finishedProdOrderLine.Description2, DbType.String);
                    parameters.Add("Quantity", finishedProdOrderLine.Quantity);
                    parameters.Add("ExpirationDate", finishedProdOrderLine.ExpirationDate, DbType.DateTime);
                    parameters.Add("ManufacturingDate", finishedProdOrderLine.ManufacturingDate, DbType.DateTime);
                    parameters.Add("UnitOfMeasureCode", finishedProdOrderLine.UnitOfMeasureCode, DbType.String);
                    parameters.Add("ItemId", finishedProdOrderLine.ItemId);
                    parameters.Add("CompanyId", finishedProdOrderLine.CompanyId);
                    parameters.Add("QcRefNo", finishedProdOrderLine.QcRefNo, DbType.String);
                    var lastInsertedRecordId = finishedProdOrderLine.RawMatPurchId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  RawMatPurch SET QcRefNo=@QcRefNo,BatchNo=@BatchNo,ItemNo=@ItemNo,Description=@Description,Description2=@Description2,Quantity=@Quantity,ExpirationDate=@ExpirationDate,ManufacturingDate=@ManufacturingDate," +
                            "UnitOfMeasureCode=@UnitOfMeasureCode,ItemId=@ItemId,CompanyId=@CompanyId  WHERE RawMatPurchId =@RawMatPurchId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [RawMatPurch](QcRefNo,BatchNo,ItemNo,Description,Description2,Quantity,ExpirationDate,ManufacturingDate,UnitOfMeasureCode,ItemId,CompanyId) " +
                            "OUTPUT INSERTED.RawMatPurchId VALUES " +
                            "(@QcRefNo,@BatchNo,@ItemNo,@Description,@Description2,@Quantity,@ExpirationDate,@ManufacturingDate,@UnitOfMeasureCode,@ItemId,@CompanyId)";
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
        public async Task<IReadOnlyList<ReleaseProdOrderLine>> GeReleaseProdOrderLineAsync(long? CompanyId)
        {
            List<ReleaseProdOrderLine> ReleaseProdOrderLineList = new List<ReleaseProdOrderLine>();
            try
            {
                var query = "select  * from ReleaseProdOrderLine where CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<ReleaseProdOrderLine>(query)).ToList();
                    ReleaseProdOrderLineList = result != null ? result : new List<ReleaseProdOrderLine>();
                }
                return ReleaseProdOrderLineList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ReleaseProdOrderLine> GetReleaseProdOrderLineList(long? CompanyId)
        {
            try
            {
                ReleaseProdOrderLine _releaseProdOrderLine = new ReleaseProdOrderLine();
                var _releaseProdOrderLineData = await GeReleaseProdOrderLineAsync(CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    var navItemsData = await GetNavItemItemNosAsync(CompanyId);
                    List<Navitems> navItemsDatas = navItemsData != null && navItemsData.Count() > 0 ? navItemsData.ToList() : new List<Navitems>();
                    List<ReleaseProdOrderLine> _releaseProdOrderLineDatas = _releaseProdOrderLineData != null ? _releaseProdOrderLineData.ToList() : new List<ReleaseProdOrderLine>();
                    var lst = await _salesOrderService.ReleaseProdOrderLineAsync(plantData.NavCompanyName, plantData.PlantID, _releaseProdOrderLineDatas, navItemsDatas);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            await InsertOrUpdateReleaseProdOrderLine(s);
                        }
                    }
                }
                _releaseProdOrderLine.ReleaseProdOrderLineId = 1;
                return _releaseProdOrderLine;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertOrUpdateReleaseProdOrderLine(ReleaseProdOrderLine finishedProdOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ReleaseProdOrderLineId", finishedProdOrderLine.ReleaseProdOrderLineId);
                    parameters.Add("ItemNo", finishedProdOrderLine.ItemNo, DbType.String);
                    parameters.Add("Description", finishedProdOrderLine.Description, DbType.String);
                    parameters.Add("Description2", finishedProdOrderLine.Description2, DbType.String);
                    parameters.Add("BatchNo", finishedProdOrderLine.BatchNo, DbType.String);
                    parameters.Add("BatchSize", finishedProdOrderLine.BatchSize, DbType.String);
                    parameters.Add("Quantity", finishedProdOrderLine.Quantity);
                    parameters.Add("LocationCode", finishedProdOrderLine.LocationCode, DbType.String);
                    parameters.Add("CompletionDate", finishedProdOrderLine.CompletionDate, DbType.DateTime);
                    parameters.Add("MachineCenterCode", finishedProdOrderLine.MachineCenterCode, DbType.String);
                    parameters.Add("ProdOrderNo", finishedProdOrderLine.ProdOrderNo, DbType.String);
                    parameters.Add("Remarks", finishedProdOrderLine.Remarks, DbType.String);
                    parameters.Add("ReplanRefNo", finishedProdOrderLine.ReplanRefNo, DbType.String);
                    parameters.Add("Promised", finishedProdOrderLine.Promised);
                    parameters.Add("Status", finishedProdOrderLine.Status, DbType.String);
                    parameters.Add("SubStatus", finishedProdOrderLine.SubStatus, DbType.String);
                    parameters.Add("StartingDate", finishedProdOrderLine.StartingDate, DbType.DateTime);
                    parameters.Add("UnitOfMeasureCode", finishedProdOrderLine.UnitOfMeasureCode, DbType.String);
                    parameters.Add("ItemId", finishedProdOrderLine.ItemId);
                    parameters.Add("CompanyId", finishedProdOrderLine.CompanyId);
                    parameters.Add("PrePrintedStartDate", finishedProdOrderLine.PrePrintedStartDate, DbType.DateTime);
                    parameters.Add("ProduceExactQuantity", finishedProdOrderLine.ProduceExactQuantity);
                    var lastInsertedRecordId = finishedProdOrderLine.ReleaseProdOrderLineId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  ReleaseProdOrderLine SET ProduceExactQuantity=@ProduceExactQuantity, PrePrintedStartDate=@PrePrintedStartDate,SubStatus=@SubStatus,Status=@Status,Promised=@Promised,ReplanRefNo=@ReplanRefNo,Remarks=@Remarks,ProdOrderNo=@ProdOrderNo,MachineCenterCode=@MachineCenterCode, LocationCode=@LocationCode, BatchSize=@BatchSize,BatchNo=@BatchNo,ItemNo=@ItemNo,Description=@Description,Description2=@Description2,Quantity=@Quantity,CompletionDate=@CompletionDate,StartingDate=@StartingDate," +
                            "UnitOfMeasureCode=@UnitOfMeasureCode,ItemId=@ItemId,CompanyId=@CompanyId  WHERE ReleaseProdOrderLineId =@ReleaseProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [ReleaseProdOrderLine](ProduceExactQuantity,PrePrintedStartDate,SubStatus,Status,Promised,ReplanRefNo,Remarks,ProdOrderNo,MachineCenterCode,LocationCode,BatchSize,BatchNo,ItemNo,Description,Description2,Quantity,CompletionDate,StartingDate,UnitOfMeasureCode,ItemId,CompanyId) " +
                            "OUTPUT INSERTED.ReleaseProdOrderLineId VALUES " +
                            "(@ProduceExactQuantity,@PrePrintedStartDate,@SubStatus,@Status,@Promised,@ReplanRefNo,@Remarks,@ProdOrderNo,@MachineCenterCode,@LocationCode,@BatchSize,@BatchNo,@ItemNo,@Description,@Description2,@Quantity,@CompletionDate,@StartingDate,@UnitOfMeasureCode,@ItemId,@CompanyId)";
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


        public async Task<IReadOnlyList<AllProdOrderLine>> GeAllProdOrderLineAsync(long? CompanyId)
        {
            List<AllProdOrderLine> ReleaseProdOrderLineList = new List<AllProdOrderLine>();
            try
            {
                var query = "select  * from AllProdOrderLine where CompanyId= " + CompanyId;

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<AllProdOrderLine>(query)).ToList();
                    ReleaseProdOrderLineList = result != null ? result : new List<AllProdOrderLine>();
                }
                return ReleaseProdOrderLineList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<AllProdOrderLine> GetAllProdOrderLineList(long? CompanyId)
        {
            try
            {
                AllProdOrderLine _releaseProdOrderLine = new AllProdOrderLine();
                var _releaseProdOrderLineData = await GeAllProdOrderLineAsync(CompanyId);
                var plantData = await _plantQueryRepository.GetByIdAsync(CompanyId.GetValueOrDefault(0));
                if (plantData != null)
                {
                    var navItemsData = await GetNavItemItemNosAsync(CompanyId);
                    List<Navitems> navItemsDatas = navItemsData != null && navItemsData.Count() > 0 ? navItemsData.ToList() : new List<Navitems>();
                    List<AllProdOrderLine> _releaseProdOrderLineDatas = _releaseProdOrderLineData != null ? _releaseProdOrderLineData.ToList() : new List<AllProdOrderLine>();
                    var lst = await _salesOrderService.AllProdOrderLineAsync(plantData.NavCompanyName, plantData.PlantID, _releaseProdOrderLineDatas, navItemsDatas);
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var s in lst)
                        {
                            await InsertOrUpdateAllProdOrderLine(s);
                        }
                    }
                }
                _releaseProdOrderLine.AllProdOrderLineId = 1;
                return _releaseProdOrderLine;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertOrUpdateAllProdOrderLine(AllProdOrderLine finishedProdOrderLine)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("AllProdOrderLineId", finishedProdOrderLine.AllProdOrderLineId);
                    parameters.Add("ItemNo", finishedProdOrderLine.ItemNo, DbType.String);
                    parameters.Add("Description", finishedProdOrderLine.Description, DbType.String);
                    parameters.Add("Description2", finishedProdOrderLine.Description2, DbType.String);
                    parameters.Add("BatchNo", finishedProdOrderLine.BatchNo, DbType.String);
                    parameters.Add("BatchSize", finishedProdOrderLine.BatchSize, DbType.String);
                    parameters.Add("Quantity", finishedProdOrderLine.Quantity);
                    parameters.Add("LocationCode", finishedProdOrderLine.LocationCode, DbType.String);
                    parameters.Add("CompletionDate", finishedProdOrderLine.CompletionDate, DbType.DateTime);
                    parameters.Add("MachineCenterCode", finishedProdOrderLine.MachineCenterCode, DbType.String);
                    parameters.Add("ProdOrderNo", finishedProdOrderLine.ProdOrderNo, DbType.String);
                    parameters.Add("Remarks", finishedProdOrderLine.Remarks, DbType.String);
                    parameters.Add("ReplanRefNo", finishedProdOrderLine.ReplanRefNo, DbType.String);
                    parameters.Add("Promised", finishedProdOrderLine.Promised);
                    parameters.Add("Status", finishedProdOrderLine.Status, DbType.String);
                    parameters.Add("SubStatus", finishedProdOrderLine.SubStatus, DbType.String);
                    parameters.Add("StartingDate", finishedProdOrderLine.StartingDate, DbType.DateTime);
                    parameters.Add("UnitOfMeasureCode", finishedProdOrderLine.UnitOfMeasureCode, DbType.String);
                    parameters.Add("ItemId", finishedProdOrderLine.ItemId);
                    parameters.Add("CompanyId", finishedProdOrderLine.CompanyId);
                    parameters.Add("PrePrintedStartDate", finishedProdOrderLine.PrePrintedStartDate, DbType.DateTime);
                    parameters.Add("ProduceExactQuantity", finishedProdOrderLine.ProduceExactQuantity);
                    var lastInsertedRecordId = finishedProdOrderLine.AllProdOrderLineId;
                    if (lastInsertedRecordId > 0)
                    {
                        var query1 = "Update  AllProdOrderLine SET ProduceExactQuantity=@ProduceExactQuantity, PrePrintedStartDate=@PrePrintedStartDate,SubStatus=@SubStatus,Status=@Status,Promised=@Promised,ReplanRefNo=@ReplanRefNo,Remarks=@Remarks,ProdOrderNo=@ProdOrderNo,MachineCenterCode=@MachineCenterCode, LocationCode=@LocationCode, BatchSize=@BatchSize,BatchNo=@BatchNo,ItemNo=@ItemNo,Description=@Description,Description2=@Description2,Quantity=@Quantity,CompletionDate=@CompletionDate,StartingDate=@StartingDate," +
                            "UnitOfMeasureCode=@UnitOfMeasureCode,ItemId=@ItemId,CompanyId=@CompanyId  WHERE AllProdOrderLineId =@AllProdOrderLineId;";
                        var rowsAffected = await connection.ExecuteAsync(query1, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [AllProdOrderLine](ProduceExactQuantity,PrePrintedStartDate,SubStatus,Status,Promised,ReplanRefNo,Remarks,ProdOrderNo,MachineCenterCode,LocationCode,BatchSize,BatchNo,ItemNo,Description,Description2,Quantity,CompletionDate,StartingDate,UnitOfMeasureCode,ItemId,CompanyId) " +
                            "OUTPUT INSERTED.AllProdOrderLineId VALUES " +
                            "(@ProduceExactQuantity,@PrePrintedStartDate,@SubStatus,@Status,@Promised,@ReplanRefNo,@Remarks,@ProdOrderNo,@MachineCenterCode,@LocationCode,@BatchSize,@BatchNo,@ItemNo,@Description,@Description2,@Quantity,@CompletionDate,@StartingDate,@UnitOfMeasureCode,@ItemId,@CompanyId)";
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
