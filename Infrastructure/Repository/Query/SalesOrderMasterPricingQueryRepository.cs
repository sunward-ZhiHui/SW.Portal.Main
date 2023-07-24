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

namespace Infrastructure.Repository.Query
{
    public class SalesOrderMasterPricingQueryRepository : QueryRepository<View_SalesOrderMasterPricing>, ISalesOrderMasterPricingQueryRepository
    {
        private readonly ISalesOrderMasterPricingLineSellingMethodQueryRepository _salesOrderMasterPricingLineSellingMethodQueryRepository;
        public SalesOrderMasterPricingQueryRepository(IConfiguration configuration, ISalesOrderMasterPricingLineSellingMethodQueryRepository salesOrderMasterPricingLineSellingMethodQueryRepository)
            : base(configuration)
        {
            _salesOrderMasterPricingLineSellingMethodQueryRepository = salesOrderMasterPricingLineSellingMethodQueryRepository;
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
                        connection.Open();
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
                    var results = await _salesOrderMasterPricingLineSellingMethodQueryRepository.GetAllAsync();
                    var result = (await connection.QueryAsync<View_SalesOrderMasterPricingLineByItem>(query, parameters)).ToList();
                    if (result.Count > 0)
                    {
                        result.ForEach(s =>
                        {
                            s.SalesOrderMasterPricingLineSellingMethods = results.Where(w => w.SalesOrderMasterPricingLineId == s.SalesOrderMasterPricingLineId).ToList();
                            view_SalesOrderMasterPricingLineByItems.Add(s);
                        });
                    }
                    return view_SalesOrderMasterPricingLineByItems;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTypeForSellingMethod(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty)
        {
            SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();
            try
            {
                var query = @"SELECT 
                    t1.SalesOrderMasterPricingLineSellingMethodID,
                    t1.TierFromQty,
                    t1.TierToQty,
                    t1.TierPrice,
                    t1.BounsQty,
                    t1.BounsFocQty,
                    t1.SalesOrderMasterPricingLineID,
                    t2.ItemID,
                    t2.SellingMethodID,
                    t2.SellingPrice,
                    t3.CompanyID,
                    t3.PriceValidaityFrom,
                    t3.PriceValidaityTo,
                    t4.Value as SellingMethod,
                    t3.MasterType
                    FROM 
                    SalesOrderMasterPricingLineSellingMethod t1 
                    JOIN SalesOrderMasterPricingLine t2 ON t2.SalesOrderMasterPricingLineID=t1.SalesOrderMasterPricingLineID
                    JOIN SalesOrderMasterPricing t3 ON t3.SalesOrderMasterPricingID=t2.SalesOrderMasterPricingID
                    JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID=t2.SellingMethodID
                    WHERE t3.CompanyID=@CompanyId AND t3.MasterType='SpecificItemMasterPrice' 
                    AND t2.ItemID=@ItemId AND t2.SellingMethodId=@SellingMethodId 
                    AND @Date between t3.PriceValidaityFrom and t3.PriceValidaityTo
                    AND @Qty between t1.TierFromQty and t1.TierToQty
                    order by t1.TierFromQty asc";

                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("Qty", Qty, DbType.Decimal);
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                    if (result != null)
                    {
                        salesOrderMasterPricingFromSalesModel = result;
                    }
                    else
                    {
                        salesOrderMasterPricingFromSalesModel = await GetPricingTypeForSellingForMasterPriceNoLimitToQtyMethodSpecific(CompanyId, DateFrom, SellingMethodId, ItemId, Qty);
                    }
                }
                return salesOrderMasterPricingFromSalesModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTypeForSellingForMasterPriceNoLimitToQtyMethodSpecific(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty)
        {
            try
            {
                SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();
                var query = @"SELECT 
                    t1.SalesOrderMasterPricingLineSellingMethodID,
                    t1.TierFromQty,
                    t1.TierToQty,
                    t1.TierPrice,
                    t1.BounsQty,
                    t1.BounsFocQty,
                    t1.SalesOrderMasterPricingLineID,
                    t2.ItemID,
                    t2.SellingMethodID,
                    t2.SellingPrice,
                    t3.CompanyID,
                    t3.PriceValidaityFrom,
                    t3.PriceValidaityTo,
                    t4.Value as SellingMethod,
                    t3.MasterType
                    FROM 
                    SalesOrderMasterPricingLineSellingMethod t1 
                    JOIN SalesOrderMasterPricingLine t2 ON t2.SalesOrderMasterPricingLineID=t1.SalesOrderMasterPricingLineID
                    JOIN SalesOrderMasterPricing t3 ON t3.SalesOrderMasterPricingID=t2.SalesOrderMasterPricingID
                    JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID=t2.SellingMethodID
                    WHERE t3.CompanyID=@CompanyId AND t3.MasterType='SpecificItemMasterPrice' 
                    AND t2.ItemID=@ItemId AND t2.SellingMethodId=@SellingMethodId 
                    AND @Date between t3.PriceValidaityFrom and t3.PriceValidaityTo
                    AND @Qty >= TierFromQty and TierToQty is null
                    order by t1.TierFromQty asc";

                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("Qty", Qty, DbType.Decimal);
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                    if (result != null)
                    {
                        salesOrderMasterPricingFromSalesModel = result;
                    }
                    else
                    {
                        salesOrderMasterPricingFromSalesModel = await GetPricingTypeForSellingForNoSpecific(CompanyId, DateFrom, SellingMethodId, ItemId,Qty);
                    }
                    return salesOrderMasterPricingFromSalesModel;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTypeForSellingForNoSpecific(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty)
        {
            try
            {
                SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();
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
                    WHERE t2.CompanyID=@CompanyId AND t2.MasterType='SpecificItemMasterPrice' 
                    AND t1.ItemID=@ItemId AND t1.SellingMethodId=@SellingMethodId 
                    AND @Date between t2.PriceValidaityFrom and t2.PriceValidaityTo
                    order by t1.SellingPrice asc";

                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    var result= await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                    if(result != null)
                    {
                        salesOrderMasterPricingFromSalesModel = result;
                    }
                    else
                    {
                        salesOrderMasterPricingFromSalesModel = await GetPricingTypeForSellingMethodSMasterPrice(CompanyId, DateFrom, SellingMethodId, ItemId,Qty);
                    }
                    return salesOrderMasterPricingFromSalesModel;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTypeForSellingMethodSMasterPrice(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty)
        {
            SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();
            try
            {
                var query = @"SELECT 
                    t1.SalesOrderMasterPricingLineSellingMethodID,
                    t1.TierFromQty,
                    t1.TierToQty,
                    t1.TierPrice,
                    t1.BounsQty,
                    t1.BounsFocQty,
                    t1.SalesOrderMasterPricingLineID,
                    t2.ItemID,
                    t2.SellingMethodID,
                    t2.SellingPrice,
                    t3.CompanyID,
                    t3.PriceValidaityFrom,
                    t3.PriceValidaityTo,
                    t4.Value as SellingMethod,
                    t3.MasterType
                    FROM 
                    SalesOrderMasterPricingLineSellingMethod t1 
                    JOIN SalesOrderMasterPricingLine t2 ON t2.SalesOrderMasterPricingLineID=t1.SalesOrderMasterPricingLineID
                    JOIN SalesOrderMasterPricing t3 ON t3.SalesOrderMasterPricingID=t2.SalesOrderMasterPricingID
                    JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID=t2.SellingMethodID
                    WHERE t3.CompanyID=@CompanyId AND t3.MasterType='MasterPrice' 
                    AND t2.ItemID=@ItemId AND t2.SellingMethodId=@SellingMethodId 
                    AND @Date between t3.PriceValidaityFrom and t3.PriceValidaityTo
                    AND @Qty between t1.TierFromQty and t1.TierToQty
                    order by t1.TierFromQty asc";

                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("Qty", Qty, DbType.Decimal);
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                    if (result != null)
                    {
                        salesOrderMasterPricingFromSalesModel = result;
                    }
                    else
                    {
                        salesOrderMasterPricingFromSalesModel = await GetPricingTypeForSellingForMasterPriceNoLimitToQtyMethodMasterPrice(CompanyId, DateFrom, SellingMethodId, ItemId, Qty);
                    }
                }
                return salesOrderMasterPricingFromSalesModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTypeForSellingForMasterPriceNoLimitToQtyMethodMasterPrice(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId, decimal? Qty)
        {
            try
            {
                SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();
                var query = @"SELECT 
                    t1.SalesOrderMasterPricingLineSellingMethodID,
                    t1.TierFromQty,
                    t1.TierToQty,
                    t1.TierPrice,
                    t1.BounsQty,
                    t1.BounsFocQty,
                    t1.SalesOrderMasterPricingLineID,
                    t2.ItemID,
                    t2.SellingMethodID,
                    t2.SellingPrice,
                    t3.CompanyID,
                    t3.PriceValidaityFrom,
                    t3.PriceValidaityTo,
                    t4.Value as SellingMethod,
                    t3.MasterType
                    FROM 
                    SalesOrderMasterPricingLineSellingMethod t1 
                    JOIN SalesOrderMasterPricingLine t2 ON t2.SalesOrderMasterPricingLineID=t1.SalesOrderMasterPricingLineID
                    JOIN SalesOrderMasterPricing t3 ON t3.SalesOrderMasterPricingID=t2.SalesOrderMasterPricingID
                    JOIN ApplicationMasterDetail t4 ON t4.ApplicationMasterDetailID=t2.SellingMethodID
                    WHERE t3.CompanyID=@CompanyId AND t3.MasterType='MasterPrice' 
                    AND t2.ItemID=@ItemId AND t2.SellingMethodId=@SellingMethodId 
                    AND @Date between t3.PriceValidaityFrom and t3.PriceValidaityTo
                    AND @Qty >= TierFromQty and TierToQty is null
                    order by t1.TierFromQty asc";

                var parameters = new DynamicParameters();
                parameters.Add("CompanyId", CompanyId);
                parameters.Add("Date", DateFrom, DbType.Date);
                parameters.Add("ItemId", ItemId, DbType.Int64);
                parameters.Add("SellingMethodId", SellingMethodId, DbType.Int64);
                parameters.Add("Qty", Qty, DbType.Decimal);
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingFromSalesModel>(query, parameters);
                    if (result != null)
                    {
                        salesOrderMasterPricingFromSalesModel = result;
                    }
                    else
                    {
                        salesOrderMasterPricingFromSalesModel = await GetPricingTypeForSellingForMasterPriceNoPricingTierMasterPrice(CompanyId, DateFrom, SellingMethodId, ItemId, Qty);
                    }
                    return salesOrderMasterPricingFromSalesModel;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> GetPricingTypeForSellingForMasterPriceNoPricingTierMasterPrice(long? CompanyId, DateTime? DateFrom, long? SellingMethodId, long? ItemId,decimal? Qty)
        {
            try
            {
                SalesOrderMasterPricingFromSalesModel salesOrderMasterPricingFromSalesModel = new SalesOrderMasterPricingFromSalesModel();
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
                    WHERE t2.CompanyID=@CompanyId AND t2.MasterType='MasterPrice' 
                    AND t1.ItemID=@ItemId AND t1.SellingMethodId=@SellingMethodId 
                    AND @Date between t2.PriceValidaityFrom and t2.PriceValidaityTo
                    order by t1.SellingPrice asc";

                var parameters = new DynamicParameters();
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
    }
}
