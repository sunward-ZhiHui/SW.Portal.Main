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

namespace Infrastructure.Repository.Query
{
    public class SalesOrderMasterPricingLineQueryRepository : QueryRepository<View_SalesOrderMasterPricingLine>, ISalesOrderMasterPricingLineQueryRepository
    {
        private readonly ISalesOrderMasterPricingLineSellingMethodQueryRepository _salesOrderMasterPricingLineSellingMethodQueryRepository;
        public SalesOrderMasterPricingLineQueryRepository(IConfiguration configuration, ISalesOrderMasterPricingLineSellingMethodQueryRepository salesOrderMasterPricingLineSellingMethodQueryRepository)
            : base(configuration)
        {
            _salesOrderMasterPricingLineSellingMethodQueryRepository = salesOrderMasterPricingLineSellingMethodQueryRepository;
        }
        public async Task<View_SalesOrderMasterPricingLine> GetByIdAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM View_SalesOrderMasterPricingLine WHERE SalesOrderMasterPricingLineId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SalesOrderMasterPricingLine>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_SalesOrderMasterPricingLine>> GetAllAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM View_SalesOrderMasterPricingLine WHERE SalesOrderMasterPricingId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_SalesOrderMasterPricingLine>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_SalesOrderMasterPricingLineByItem>> GetSalesOrderMasterPricingLineByItemAsync(long? CompanyId, DateTime? PriceValidaityFrom, DateTime? PriceValidaityTo, long? ItemId)
        {
            try
            {
                var view_SalesOrderMasterPricingLineByItems = new List<View_SalesOrderMasterPricingLineByItem>();
                var parameters = new DynamicParameters();
                var query = "SELECT * FROM View_SalesOrderMasterPricingLineByItem WHERE  MasterType='MasterPrice'and CompanyId = @CompanyId and PriceValidaityFrom>=@PriceValidaityFrom and PriceValidaityTo<=@PriceValidaityTo";
                parameters.Add("CompanyId", CompanyId, DbType.Int64);
                parameters.Add("PriceValidaityFrom", PriceValidaityFrom, DbType.Date);
                parameters.Add("PriceValidaityTo", PriceValidaityTo, DbType.Date);
                if (ItemId != null)
                {
                    query = "SELECT * FROM View_SalesOrderMasterPricingLineByItem WHERE ItemId=@ItemId and MasterType='MasterPrice'and CompanyId = @CompanyId and PriceValidaityFrom>=@PriceValidaityFrom and PriceValidaityTo<=@PriceValidaityTo";
                    parameters.Add("ItemId", ItemId, DbType.Int64);
                }
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
    }
}
