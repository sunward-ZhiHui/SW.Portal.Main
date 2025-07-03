using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Infrastructure.Repository.Query
{
    public class PostSalesOrderQueryRepository : QueryRepository<PostSalesOrder>, IPostSalesOrderQueryRepository
    {
        public PostSalesOrderQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<PostSalesOrder>> GetAllAsync()
        {
            try
            {
                var query = "select  * from PostSalesOrder";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<PostSalesOrder>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<string> UpdateStockBalanceData(string query)
        {
            try
            {
                var querys = string.Empty;
                using (var connection = CreateConnection())
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(query))
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                            if (!string.IsNullOrEmpty(query))
                            {
                                await connection.ExecuteAsync(query, parameters);
                            }
                        }
                        return query;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SotckBalanceItemsList> GetSotckBalanceItemsListAsync(StockBalanceSearch searchModel)
        {
            try
            {
                SotckBalanceItemsList sotckBalanceItemsList = new SotckBalanceItemsList();
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", searchModel.CompanyId, DbType.Int64);
                var query = "select  * from Plant;";
                query += "select * from Navitems where companyid=@CompanyId and no like 'FP-%';";
                query += "select * from NavitemStockBalance;";
                query += "select * from Navcustomer where companyid=@CompanyId;";
                query += "select * from DistStockBalanceKiv;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    sotckBalanceItemsList.PlantData = results.Read<Plant>().ToList();
                    sotckBalanceItemsList.NavitemsData = results.Read<Navitems>().ToList();
                    sotckBalanceItemsList.NavitemStockBalance = results.Read<NavitemStockBalance>().ToList();
                    sotckBalanceItemsList.Navcustomer = results.Read<Navcustomer>().ToList();
                    sotckBalanceItemsList.DistStockBalanceKiv = results.Read<DistStockBalanceKiv>().ToList();
                }
                return sotckBalanceItemsList;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<PostSalesOrder> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM PostSalesOrder  WHERE PostSalesOrderID = @PostSalesOrderId";
                var parameters = new DynamicParameters();
                parameters.Add("PostSalesOrderId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<PostSalesOrder>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<string> InsertRawMatItemList(IEnumerable<RawMatItemList> rawMatItemList)
        {
            var successCount = 0;

            foreach (var rawMatItem in rawMatItemList)
            {
                var validation = await GetRawMatItemValidatation(rawMatItem.CompanyId, rawMatItem.ItemNo, rawMatItem.Type);

                if (validation == null)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ItemNo", rawMatItem.ItemNo);
                    parameters.Add("Description", rawMatItem.Description);
                    parameters.Add("Description2", rawMatItem.Description2);
                    parameters.Add("Inventory", rawMatItem.Inventory);
                    parameters.Add("InternalRef", rawMatItem.InternalRef);
                    parameters.Add("ItemRegistration", rawMatItem.ItemRegistration);
                    parameters.Add("BatchNos", rawMatItem.BatchNos);
                    parameters.Add("PSOItemNo", rawMatItem.PSOItemNo);
                    parameters.Add("ProductionRecipeNo", rawMatItem.ProductionRecipeNo);
                    parameters.Add("SafetyLeadTime", rawMatItem.SafetyLeadTime);
                    parameters.Add("ProductionBOMNo", rawMatItem.ProductionBOMNo);
                    parameters.Add("RoutingNo", rawMatItem.RoutingNo);
                    parameters.Add("BaseUnitofMeasure", rawMatItem.BaseUnitofMeasure);
                    parameters.Add("StandardCost", rawMatItem.StandardCost);
                    parameters.Add("UnitCost", rawMatItem.UnitCost);
                    parameters.Add("LastDirectCost", rawMatItem.LastDirectCost);
                    parameters.Add("ItemCategoryCode", rawMatItem.ItemCategoryCode);
                    parameters.Add("ProductGroupCode", rawMatItem.ProductGroupCode);
                    parameters.Add("CompanyId", rawMatItem.CompanyId);
                    parameters.Add("Type", rawMatItem.Type);

                    var query = "INSERT INTO RawMatItemList (ItemNo, Description, Description2,Inventory,InternalRef,ItemRegistration,BatchNos,PSOItemNo,ProductionRecipeNo,SafetyLeadTime,ProductionBOMNo,RoutingNo,BaseUnitofMeasure,StandardCost,UnitCost,LastDirectCost,ItemCategoryCode,ProductGroupCode,CompanyId,Type) VALUES (@ItemNo,@Description, @Description2,@Inventory,@InternalRef,@ItemRegistration,@BatchNos,@PSOItemNo,@ProductionRecipeNo,@SafetyLeadTime,@ProductionBOMNo,@RoutingNo,@BaseUnitofMeasure,@StandardCost,@UnitCost,@LastDirectCost,@ItemCategoryCode,@ProductGroupCode,@CompanyId,@Type)";

                    using (var connection = CreateConnection())
                    {
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);

                        if (rowsAffected > 0)
                            successCount++;
                    }
                }
            }

            if (successCount > 0)
                return $"Successfully added {successCount} items.";
            else
                return "All items already added or failed to insert.";
        }

        public async Task<RawMatItemList> GetRawMatItemValidatation(long? companyid, string? itemno, string? type)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("companyid", companyid);
                parameters.Add("itemno", itemno);
                parameters.Add("type", type);

                var query = "SELECT * FROM RawMatItemList WHERE CompanyId = @companyid AND Type = @type AND ItemNo = @itemno";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<RawMatItemList>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<NavitemStockBalance> InsertOrUpdateNavitemStockBalance(List<NavitemStockBalance> rawMatItemList)
        {
            NavitemStockBalance navitemStockBalance = new NavitemStockBalance();
            foreach (var rawMatItem in rawMatItemList)
            {
                await InsertOrUpdateNavitemStockBalances(rawMatItem);
            }

            return navitemStockBalance;
        }
        public async Task<NavitemStockBalance> InsertOrUpdateNavitemStockBalances(NavitemStockBalance itemBatchInfo)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("NavStockBalanceId", itemBatchInfo.NavStockBalanceId);
                    parameters.Add("ItemId", itemBatchInfo.ItemId);
                    parameters.Add("Quantity", itemBatchInfo.Quantity, DbType.Decimal);
                    parameters.Add("GlobalQty", itemBatchInfo.GlobalQty, DbType.Decimal);
                    parameters.Add("ReworkQty", itemBatchInfo.ReworkQty, DbType.Decimal);
                    parameters.Add("Wipqty", itemBatchInfo.Wipqty, DbType.Decimal);
                    parameters.Add("Kivqty", itemBatchInfo.Kivqty, DbType.Decimal);
                    parameters.Add("RejectQuantity", itemBatchInfo.RejectQuantity, DbType.Decimal);
                    parameters.Add("StockBalWeek", itemBatchInfo.StockBalWeek);
                    parameters.Add("StockBalMonth", itemBatchInfo.StockBalMonth == DateTime.MinValue ? null : itemBatchInfo.StockBalMonth, DbType.DateTime);
                    parameters.Add("Supply1ProcessQty", itemBatchInfo.Supply1ProcessQty);
                    parameters.Add("SupplyWipqty", itemBatchInfo.SupplyWipqty);
                    parameters.Add("NotStartInvQty", itemBatchInfo.NotStartInvQty, DbType.Decimal);
                    parameters.Add("AddedByUserId", itemBatchInfo.AddedByUserId);
                    parameters.Add("ModifiedByUserId", itemBatchInfo.ModifiedByUserId);
                    parameters.Add("StatusCodeId", 1);
                    parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                    parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                    var lastInsertedRecordId = itemBatchInfo.NavStockBalanceId;
                    if (itemBatchInfo.NavStockBalanceId > 0)
                    {
                        var query = "Update  NavitemStockBalance SET StatusCodeId=@StatusCodeId,ItemId=@ItemId,Quantity=@Quantity,GlobalQty=@GlobalQty,ReworkQty=@ReworkQty,Wipqty=@Wipqty,Kivqty=@Kivqty,RejectQuantity=@RejectQuantity,StockBalWeek=@StockBalWeek," +
                            "StockBalMonth=@StockBalMonth,Supply1ProcessQty=@Supply1ProcessQty,SupplyWipqty=@SupplyWipqty,NotStartInvQty=@NotStartInvQty,ModifiedByUserId=@ModifiedByUserId,ModifiedDate=@ModifiedDate WHERE NavStockBalanceId = @NavStockBalanceId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [NavitemStockBalance](ItemId,Quantity,GlobalQty,ReworkQty,Wipqty,Kivqty,RejectQuantity,StockBalWeek," +
                            "StockBalMonth,Supply1ProcessQty,SupplyWipqty,NotStartInvQty,StatusCodeId,AddedDate,AddedByUserId,ModifiedByUserId,ModifiedDate) OUTPUT INSERTED.NavStockBalanceId VALUES " +
                            "(@ItemId,@Quantity,@GlobalQty,@ReworkQty,@Wipqty,@Kivqty,@RejectQuantity,@StockBalWeek,@StockBalMonth,@Supply1ProcessQty,@SupplyWipqty,@NotStartInvQty,@StatusCodeId,@AddedDate,@AddedByUserId,@ModifiedByUserId,@ModifiedDate)";

                        itemBatchInfo.NavStockBalanceId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                    }
                    return itemBatchInfo;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DistStockBalanceKiv> InsertOrUpdateDistStockBalanceKiv(List<DistStockBalanceKiv> rawMatItemList)
        {
            DistStockBalanceKiv navitemStockBalance = new DistStockBalanceKiv();
            foreach (var rawMatItem in rawMatItemList)
            {
                await InsertOrUpdateDistStockBalanceKivs(rawMatItem);
            }

            return navitemStockBalance;
        }
        public async Task<DistStockBalanceKiv> InsertOrUpdateDistStockBalanceKivs(DistStockBalanceKiv itemBatchInfo)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    var parameters = new DynamicParameters();
                    parameters.Add("DistKivId", itemBatchInfo.DistKivId);
                    parameters.Add("ItemId", itemBatchInfo.ItemId);
                    parameters.Add("CustomerId", itemBatchInfo.CustomerId);
                    parameters.Add("Quantity", itemBatchInfo.Quantity, DbType.Decimal);
                    parameters.Add("ItemNo", itemBatchInfo.ItemNo, DbType.String);
                    parameters.Add("CompanyId", itemBatchInfo.CompanyId);
                    parameters.Add("CustomerNo", itemBatchInfo.CustomerNo);
                    parameters.Add("StockBalWeek", itemBatchInfo.StockBalWeek);
                    parameters.Add("StockBalMonth", itemBatchInfo.StockBalMonth == DateTime.MinValue ? null : itemBatchInfo.StockBalMonth, DbType.DateTime);
                    var lastInsertedRecordId = itemBatchInfo.DistKivId;
                    if (itemBatchInfo.DistKivId > 0)
                    {
                        var query = "Update  DistStockBalanceKiv SET ItemId=@ItemId,Quantity=@Quantity," +
                            "CustomerId=@CustomerId,ItemNo=@ItemNo,CompanyId=@CompanyId,CustomerNo=@CustomerNo,StockBalWeek=@StockBalWeek," +
                            "StockBalMonth=@StockBalMonth WHERE DistKivId = @DistKivId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO [DistStockBalanceKiv](ItemId,Quantity,CustomerId,ItemNo,CompanyId,CustomerNo,StockBalWeek," +
                            "StockBalMonth) OUTPUT INSERTED.DistKivId VALUES " +
                            "(@ItemId,@Quantity,@CustomerId,@ItemNo,@CompanyId,@CustomerNo,@StockBalWeek,@StockBalMonth)";

                        itemBatchInfo.DistKivId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                    }
                    return itemBatchInfo;

                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}

