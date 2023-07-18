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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
    }
}
