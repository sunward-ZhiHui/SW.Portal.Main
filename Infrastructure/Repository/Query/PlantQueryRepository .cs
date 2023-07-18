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
    public class PlantQueryRepository : QueryRepository<ViewPlants>, IPlantQueryRepository
    {
        public PlantQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewPlants>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_Plants";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewPlants>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewPlants>> GetAllByNavCompanyAsync()
        {
            try
            {
                var query = "select  * from view_Plants where NavCompanyName is not null";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewPlants>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewPlants> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_Plants WHERE plantId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewPlants>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
