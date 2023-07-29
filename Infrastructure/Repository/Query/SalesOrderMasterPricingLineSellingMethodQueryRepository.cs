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
    public class SalesOrderMasterPricingLineSellingMethodQueryRepository : QueryRepository<SalesOrderMasterPricingLineSellingMethod>, ISalesOrderMasterPricingLineSellingMethodQueryRepository
    {
        public SalesOrderMasterPricingLineSellingMethodQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<SalesOrderMasterPricingLineSellingMethod>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM SalesOrderMasterPricingLineSellingMethod";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<SalesOrderMasterPricingLineSellingMethod>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<SalesOrderMasterPricingLineSellingMethod> GetByIdAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM SalesOrderMasterPricingLineSellingMethod WHERE SalesOrderMasterPricingLineSellingMethodId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingLineSellingMethod>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<SalesOrderMasterPricingLineSellingMethod>> GetAllSalesOrderMasterPricingLineAsync(long? Id, string SellingMethodName)
        {
            try
            {
                List<SalesOrderMasterPricingLineSellingMethod> salesOrderMasterPricingLineSellingMethods = new List<SalesOrderMasterPricingLineSellingMethod>();
                var query = "SELECT * FROM SalesOrderMasterPricingLineSellingMethod WHERE SalesOrderMasterPricingLineID = @Id";
                if (SellingMethodName != null)
                {
                    if (SellingMethodName == "tier")
                    {
                        query = "SELECT MAX(t1.SalesOrderMasterPricingLineSellingMethodID) as SalesOrderMasterPricingLineSellingMethodID," +
                            "t1.SalesOrderMasterPricingLineID, " +
                            "t1.TierFromQty, " +
                            "MIN(t2.TierFromQty)-1 AS TierToQty, " +
                            "MAX(t1.TierPrice) AS TierPrice FROM SalesOrderMasterPricingLineSellingMethod t1 " +
                            "LEFT JOIN SalesOrderMasterPricingLineSellingMethod t2 ON t2.TierFromQty > t1.TierFromQty " +
                            "AND t1.SalesOrderMasterPricingLineID = t2.SalesOrderMasterPricingLineID  " +
                            "LEFT JOIN SalesOrderMasterPricingLineSellingMethod t3 ON t2.BounsQty > t1.BounsQty " +
                            "AND t1.SalesOrderMasterPricingLineID = t2.SalesOrderMasterPricingLineID " +
                            "WHERE t1.SalesOrderMasterPricingLineID=@ID GROUP BY t1.TierFromQty,t1.SalesOrderMasterPricingLineID " +
                            "ORDER BY t1.SalesOrderMasterPricingLineID, t1.TierFromQty";
                    }
                    if (SellingMethodName == "bonus")
                    {
                        query = "SELECT MAX(t1.SalesOrderMasterPricingLineSellingMethodID) as SalesOrderMasterPricingLineSellingMethodID," +
                            "t1.SalesOrderMasterPricingLineID, " +
                            "t1.BounsQty, " +
                            "MIN(t2.BounsQty)-1 AS BounsToQty, " +
                            "MAX(t1.BounsFocQty) AS BounsFocQty, " +
                            "MAX(t1.BounsPrice) AS BounsPrice FROM SalesOrderMasterPricingLineSellingMethod t1 " +
                            "LEFT JOIN SalesOrderMasterPricingLineSellingMethod t2 ON t2.BounsQty > t1.BounsQty " +
                            "AND t1.SalesOrderMasterPricingLineID = t2.SalesOrderMasterPricingLineID  " +
                            "LEFT JOIN SalesOrderMasterPricingLineSellingMethod t3 ON t2.BounsQty > t1.BounsQty " +
                            "AND t1.SalesOrderMasterPricingLineID = t2.SalesOrderMasterPricingLineID " +
                            "WHERE t1.SalesOrderMasterPricingLineID=@ID GROUP BY t1.BounsQty,t1.SalesOrderMasterPricingLineID " +
                            "ORDER BY t1.SalesOrderMasterPricingLineID, t1.BounsQty";
                    }
                    var parameters = new DynamicParameters();
                    parameters.Add("Id", Id, DbType.Int64);

                    using (var connection = CreateConnection())
                    {
                        salesOrderMasterPricingLineSellingMethods = (await connection.QueryAsync<SalesOrderMasterPricingLineSellingMethod>(query, parameters)).ToList();
                    }
                }
                return salesOrderMasterPricingLineSellingMethods;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
