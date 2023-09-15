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

namespace Infrastructure.Repository.Query
{
    public class DivisionQueryRepository : QueryRepository<ViewDivision>, IDivisionQueryRepository
    {
        public DivisionQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewDivision>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_Division";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewDivision>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewDivision> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_Division WHERE divisionId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewDivision>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewDivision>> GetDivisionByCompanyAsync(long? companyId)
        {
            try
            {
                var query = "SELECT * FROM view_Division WHERE companyId = @companyId";
                var parameters = new DynamicParameters();
                parameters.Add("companyId", companyId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewDivision>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
