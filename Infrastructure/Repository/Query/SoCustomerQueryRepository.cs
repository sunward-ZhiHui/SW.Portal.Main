using Core.Entities;
using Core.Entities.Views;
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

namespace Infrastructure.Repository.Query
{
    public class SoCustomerQueryRepository : QueryRepository<SoCustomer>, ISoCustomerQueryRepository
    {
        public SoCustomerQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<SoCustomer>> GetAllAsync()
        {
            try
            {
                var query = "select  t1.*,t2.PlantCode as CompanyName from SoCustomer t1 LEFT JOIN Plant t2 ON t1.CompanyID=t2.PlantID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<SoCustomer>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<SoCustomer>> GetListByTypeAsync(string Type)
        {
            try
            {
                string newList = string.Join(",", Type.Split(',').Select(x => string.Format("'{0}'", x)).ToList());
                var query = "select  t1.*,t2.PlantCode as CompanyName from SoCustomer t1 LEFT JOIN Plant t2 ON t1.CompanyID=t2.PlantID where t1.type in(" + newList + ")";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<SoCustomer>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<SoCustomer> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM SoCustomer WHERE SoCustomerID = @SoCustomerID";
                var parameters = new DynamicParameters();
                parameters.Add("SoCustomerID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<SoCustomer>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}
