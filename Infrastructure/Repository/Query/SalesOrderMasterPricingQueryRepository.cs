using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.EntityModel;
using Core.Entities.Views;
using Core.Entities;
using Core.EntityModels;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Text.RegularExpressions;

namespace Infrastructure.Repository.Query
{
    public class SalesOrderMasterPricingQueryRepository : QueryRepository<View_SalesOrderMasterPricing>, ISalesOrderMasterPricingQueryRepository
    {
        public SalesOrderMasterPricingQueryRepository(IConfiguration configuration)
            : base(configuration)
        {
        }
        public async Task<IReadOnlyList<View_SalesOrderMasterPricing>> GetAllByMasterTypeAsync(string MasterType)
        {
            try
            {
                var query = "select  * from View_SalesOrderMasterPricing where MasterType=@MasterType";
                var parameters = new DynamicParameters();
                parameters.Add("MasterType", MasterType);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_SalesOrderMasterPricing>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_SalesOrderMasterPricing> GetByIdAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM View_SalesOrderMasterPricing WHERE SalesOrderMasterPricingId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SalesOrderMasterPricing>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_SalesOrderMasterPricing> GetBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM View_SalesOrderMasterPricing WHERE SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SalesOrderMasterPricing>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_SalesOrderMasterPricing> GetCheckPriceValidaityDateAsync(SalesOrderMasterPricing items)
        {
            try
            {
                var query = "select  * from SalesOrderMasterPricing WHERE CompanyId=@CompanyId and MasterType=@MasterType and @PriceValidaityFrom between PriceValidaityFrom and PriceValidaityTo";
                var parameters = new DynamicParameters();
                parameters.Add("MasterType", items.MasterType);
                parameters.Add("CompanyId", items.CompanyId);
                parameters.Add("PriceValidaityFrom", items.PriceValidaityFrom, DbType.DateTime);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SalesOrderMasterPricing>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertSalesOrderMasterPricingLineAsync(SalesOrderMasterPricing items)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SalesOrderMasterPricingId", items.SalesOrderMasterPricingId);
                        parameters.Add("CompanyId", items.CompanyId);
                        parameters.Add("AddedByUserId", items.AddedByUserId);
                        parameters.Add("StatusCodeId", items.StatusCodeId);                      
                        var result = await connection.ExecuteAsync("sp_Ins_SalesOrderMasterPricingline", parameters, commandType: CommandType.StoredProcedure);
                        return result;
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
        public async Task<IReadOnlyList<View_SalesOrderMasterPricingLineByItem>> GetSalesOrderLineByItemAsync(long? CompanyId, DateTime? FromDate, long? SellingMethodId, long? ItemId)
        {
            try
            {
                var view_SalesOrderMasterPricingLineByItems = new List<View_SalesOrderMasterPricingLineByItem>();
                var parameters = new DynamicParameters();
                var query = "SELECT * FROM View_SalesOrderMasterPricingLineByItem WHERE ItemId=@ItemId AND SellingMethodId=@SellingMethodId and CompanyId = @CompanyId and @FromDate between PriceValidaityFrom and PriceValidaityTo;";
                parameters.Add("CompanyId", CompanyId, DbType.Int64);
                parameters.Add("FromDate", FromDate, DbType.Date);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_SalesOrderMasterPricingLineByItem>(query, parameters)).ToList();

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTypeForSellingMethod(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty, string SellingMethod)
        {
            SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();
            if (SellingMethod.ToLower() == "tier")
            {
                var result = await GetPricingTierDuartionCompare(CompanyId, DateFrom, SellingMethodId, ItemId, Qty, "SpecificItemMasterPrice");
                if (result != null)
                {
                    salesOrderMasterPricingFromSalesModel = result;
                }
                else
                {
                    var resultMaster = await GetPricingTierDuartionCompare(CompanyId, DateFrom, SellingMethodId, ItemId, Qty, "MasterPrice");
                    if (resultMaster != null)
                    {
                        salesOrderMasterPricingFromSalesModel = resultMaster;
                    }
                    else
                    {
                        var ItemSellingPrice = await GetPricingNoTierOrBonus(CompanyId, DateFrom, SellingMethodId, ItemId, "SpecificItemMasterPrice");
                        if (ItemSellingPrice != null)
                        {
                            salesOrderMasterPricingFromSalesModel = ItemSellingPrice;
                        }
                        else
                        {
                            salesOrderMasterPricingFromSalesModel = await GetPricingNoTierOrBonus(CompanyId, DateFrom, SellingMethodId, ItemId, "MasterPrice");
                        }
                    }
                }
            }
            else
            {
                var resultMaster = await GetPricingBonusDuartionCompare(CompanyId, DateFrom, SellingMethodId, ItemId, Qty, "MasterPrice");
                if (resultMaster != null)
                {
                    salesOrderMasterPricingFromSalesModel = resultMaster;
                }
                else
                {
                    var ItemSellingPrice = await GetPricingNoTierOrBonus(CompanyId, DateFrom, SellingMethodId, ItemId, "SpecificItemMasterPrice");
                    if (ItemSellingPrice != null)
                    {
                        salesOrderMasterPricingFromSalesModel = ItemSellingPrice;
                    }
                    else
                    {
                        salesOrderMasterPricingFromSalesModel = await GetPricingNoTierOrBonus(CompanyId, DateFrom, SellingMethodId, ItemId, "MasterPrice");
                    }
                }
            }
            return salesOrderMasterPricingFromSalesModel;
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingNoTierOrBonus(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, string MasterType)
        {

            try
            {
                var query = @"SELECT 
                    t1.SalesOrderMasterPricingLineID,
                    t1.ItemID,
                    t1.SellingMethodID,
                    t1.SellingPrice,
                    t2.CompanyID,
                    t2.PriceValidaityFrom,
                    t2.PriceValidaityTo,
                    t3.Value as SellingMethod,
                    t2.MasterType
                    FROM 
                    SalesOrderMasterPricingLine t1 
                    JOIN SalesOrderMasterPricing t2 ON t2.SalesOrderMasterPricingID=t1.SalesOrderMasterPricingID
                    JOIN ApplicationMasterDetail t3 ON t3.ApplicationMasterDetailID=t1.SellingMethodID
                    WHERE t2.CompanyID=@CompanyId AND t2.MasterType=@MasterType 
                    AND t1.ItemID=@ItemId AND t1.SellingMethodId=@SellingMethodId 
                    AND @Date between t2.PriceValidaityFrom and t2.PriceValidaityTo
                    order by t1.SellingPrice asc";

                var parameters = new DynamicParameters();
                parameters.Add("MasterType", MasterType);
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTierDuartionCompare(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty, string MasterType)
        {
            try
            {

                var query = "SELECT TT.* FROM(SELECT t1.SalesOrderMasterPricingLineID,t1.TierFromQty AS TierNewFromQty,MIN(t5.TierFromQty) - 1 AS TierNewToQty," +
                    "MAX(t1.SalesOrderMasterPricingLineSellingMethodID) as SalesOrderMasterPricingLineSellingMethodID," +
                    "MAX(t1.TierFromQty) as TierFromQty,MAX(t1.TierToQty) as TierToQty," +
                    "MAX(t1.TierPrice) as TierPrice,MAX(t1.BounsQty) as BounsQty," +
                    "MAX(t1.BounsFocQty) as BounsFocQty,MAX(t2.ItemID) as ItemID," +
                    "MAX(t2.SellingMethodID) as SellingMethodID," +
                    "MAX(t2.SellingPrice) as SellingPrice," +
                    "MAX(t3.CompanyID) as CompanyID," +
                    "MAX(t3.PriceValidaityFrom) as PriceValidaityFrom," +
                    "MAX(t3.PriceValidaityTo) as PriceValidaityTo,MAX(t4.Value) as SellingMethod," +
                    "MAX(t3.MasterType) as MasterType FROM SalesOrderMasterPricingLineSellingMethod t1 " +
                    "JOIN SalesOrderMasterPricingLine t2 ON t2.SalesOrderMasterPricingLineID = t1.SalesOrderMasterPricingLineID " +
                    "JOIN SalesOrderMasterPricing t3 ON t3.SalesOrderMasterPricingID = t2.SalesOrderMasterPricingID " +
                    "JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID = t2.SellingMethodID " +
                    "LEFT JOIN SalesOrderMasterPricingLineSellingMethod t5 ON t5.TierFromQty > t1.TierFromQty AND t1.SalesOrderMasterPricingLineID = t5.SalesOrderMasterPricingLineID " +
                    "WHERE t3.CompanyID = @CompanyId " +
                    "AND t3.MasterType =@MasterType " +
                    "AND t2.ItemID = @ItemID " +
                    "AND t2.SellingMethodId = @SellingMethodId " +
                    "AND @Date between t3.PriceValidaityFrom and t3.PriceValidaityTo " +
                    "GROUP BY t1.TierFromQty, t1.SalesOrderMasterPricingLineID) TT WHERE @Qty between TT.TierFromQty and TT.TierNewToQty ORDER BY TT.SalesOrderMasterPricingLineID, TT.TierFromQty";
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("MasterType", MasterType);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("Qty", Qty, DbType.Decimal);
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        return await GetPricingTierToQtyIsNull(CompanyId, DateFrom, SellingMethodId, ItemId, Qty, MasterType);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTierToQtyIsNull(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty, string MasterType)
        {
            try
            {
                SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();

                var query = "SELECT TT.* FROM(SELECT t1.SalesOrderMasterPricingLineID,t1.TierFromQty AS TierNewFromQty,MIN(t5.TierFromQty) - 1 AS TierNewToQty," +
                    "MAX(t1.SalesOrderMasterPricingLineSellingMethodID) as SalesOrderMasterPricingLineSellingMethodID," +
                    "MAX(t1.TierFromQty) as TierFromQty,MAX(t1.TierToQty) as TierToQty," +
                    "MAX(t1.TierPrice) as TierPrice,MAX(t1.BounsQty) as BounsQty," +
                    "MAX(t1.BounsFocQty) as BounsFocQty,MAX(t2.ItemID) as ItemID," +
                    "MAX(t2.SellingMethodID) as SellingMethodID," +
                    "MAX(t2.SellingPrice) as SellingPrice," +
                    "MAX(t3.CompanyID) as CompanyID," +
                    "MAX(t3.PriceValidaityFrom) as PriceValidaityFrom," +
                    "MAX(t3.PriceValidaityTo) as PriceValidaityTo,MAX(t4.Value) as SellingMethod," +
                    "MAX(t3.MasterType) as MasterType FROM SalesOrderMasterPricingLineSellingMethod t1 " +
                    "JOIN SalesOrderMasterPricingLine t2 ON t2.SalesOrderMasterPricingLineID = t1.SalesOrderMasterPricingLineID " +
                    "JOIN SalesOrderMasterPricing t3 ON t3.SalesOrderMasterPricingID = t2.SalesOrderMasterPricingID " +
                    "JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID = t2.SellingMethodID " +
                    "LEFT JOIN SalesOrderMasterPricingLineSellingMethod t5 ON t5.TierFromQty > t1.TierFromQty AND t1.SalesOrderMasterPricingLineID = t5.SalesOrderMasterPricingLineID " +
                    "WHERE t3.CompanyID = @CompanyId " +
                    "AND t3.MasterType =@MasterType " +
                    "AND t2.ItemID = @ItemID " +
                    "AND t2.SellingMethodId = @SellingMethodId " +
                    "AND @Date between t3.PriceValidaityFrom and t3.PriceValidaityTo " +
                    "GROUP BY t1.TierFromQty, t1.SalesOrderMasterPricingLineID) TT WHERE @Qty>=TT.TierNewFromQty AND TT.TierNewToQty is null  ORDER BY TT.SalesOrderMasterPricingLineID, TT.TierFromQty";
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("MasterType", MasterType);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("Qty", Qty, DbType.Decimal);
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }



        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingBonusDuartionCompare(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty, string MasterType)
        {
            try
            {

                var query = "SELECT TT.* FROM(SELECT t1.SalesOrderMasterPricingLineID,t1.BounsQty,MIN(t5.BounsQty) - 1 AS BounsToQty," +
                    "MAX(t1.SalesOrderMasterPricingLineSellingMethodID) as SalesOrderMasterPricingLineSellingMethodID," +
                     "MAX(t1.TierFromQty) as TierFromQty,MAX(t1.TierToQty) as TierToQty," +
                    "MAX(t1.TierPrice) as TierPrice," +
                    "MAX(t1.BounsFocQty) as BounsFocQty,MAX(t2.ItemID) as ItemID," +
                    "MAX(t2.SellingMethodID) as SellingMethodID," +
                    "MAX(t2.SellingPrice) as SellingPrice," +
                    "MAX(t3.CompanyID) as CompanyID," +
                    "MAX(t3.PriceValidaityFrom) as PriceValidaityFrom," +
                    "MAX(t3.PriceValidaityTo) as PriceValidaityTo,MAX(t4.Value) as SellingMethod," +
                    "MAX(t3.MasterType) as MasterType FROM SalesOrderMasterPricingLineSellingMethod t1 " +
                    "JOIN SalesOrderMasterPricingLine t2 ON t2.SalesOrderMasterPricingLineID = t1.SalesOrderMasterPricingLineID " +
                    "JOIN SalesOrderMasterPricing t3 ON t3.SalesOrderMasterPricingID = t2.SalesOrderMasterPricingID " +
                    "JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID = t2.SellingMethodID " +
                    "LEFT JOIN SalesOrderMasterPricingLineSellingMethod t5 ON t5.BounsQty > t1.BounsQty AND t1.SalesOrderMasterPricingLineID = t5.SalesOrderMasterPricingLineID " +
                    "WHERE t3.CompanyID = @CompanyId " +
                    "AND t3.MasterType =@MasterType " +
                    "AND t2.ItemID = @ItemID " +
                    "AND t2.SellingMethodId = @SellingMethodId " +
                    "AND @Date between t3.PriceValidaityFrom and t3.PriceValidaityTo " +
                    "GROUP BY t1.BounsQty, t1.SalesOrderMasterPricingLineID) TT WHERE @Qty between TT.BounsQty and TT.BounsToQty ORDER BY TT.SalesOrderMasterPricingLineID, TT.BounsQty";
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("MasterType", MasterType);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("Qty", Qty, DbType.Decimal);
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        return await GetPricingBonusToQtyIsNull(CompanyId, DateFrom, SellingMethodId, ItemId, Qty, MasterType);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingBonusToQtyIsNull(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty, string MasterType)
        {
            try
            {
                SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();

                var query = "SELECT TT.* FROM(SELECT t1.SalesOrderMasterPricingLineID,t1.BounsQty,MIN(t5.BounsQty) - 1 AS BounsToQty," +
                    "MAX(t1.SalesOrderMasterPricingLineSellingMethodID) as SalesOrderMasterPricingLineSellingMethodID," +
                    "MAX(t1.TierFromQty) as TierFromQty,MAX(t1.TierToQty) as TierToQty," +
                    "MAX(t1.TierPrice) as TierPrice," +
                    "MAX(t1.BounsFocQty) as BounsFocQty,MAX(t2.ItemID) as ItemID," +
                    "MAX(t2.SellingMethodID) as SellingMethodID," +
                    "MAX(t2.SellingPrice) as SellingPrice," +
                    "MAX(t3.CompanyID) as CompanyID," +
                    "MAX(t3.PriceValidaityFrom) as PriceValidaityFrom," +
                    "MAX(t3.PriceValidaityTo) as PriceValidaityTo,MAX(t4.Value) as SellingMethod," +
                    "MAX(t3.MasterType) as MasterType FROM SalesOrderMasterPricingLineSellingMethod t1 " +
                    "JOIN SalesOrderMasterPricingLine t2 ON t2.SalesOrderMasterPricingLineID = t1.SalesOrderMasterPricingLineID " +
                    "JOIN SalesOrderMasterPricing t3 ON t3.SalesOrderMasterPricingID = t2.SalesOrderMasterPricingID " +
                    "JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID = t2.SellingMethodID " +
                    "LEFT JOIN SalesOrderMasterPricingLineSellingMethod t5 ON t5.BounsQty > t1.BounsQty AND t1.SalesOrderMasterPricingLineID = t5.SalesOrderMasterPricingLineID " +
                    "WHERE t3.CompanyID = @CompanyId " +
                    "AND t3.MasterType =@MasterType " +
                    "AND t2.ItemID = @ItemID " +
                    "AND t2.SellingMethodId = @SellingMethodId " +
                    "AND @Date between t3.PriceValidaityFrom and t3.PriceValidaityTo " +
                    "GROUP BY t1.BounsQty, t1.SalesOrderMasterPricingLineID) TT WHERE @Qty>=TT.BounsQty AND TT.BounsToQty is null  ORDER BY TT.SalesOrderMasterPricingLineID, TT.BounsQty";
                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("MasterType", MasterType);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("Qty", Qty, DbType.Decimal);
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
