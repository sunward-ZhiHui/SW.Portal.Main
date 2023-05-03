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
    public class LevelMasterQueryRepository : QueryRepository<ViewLevel>, ILevelMasterQueryRepository
    {
        public LevelMasterQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewLevel>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_Level";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewLevel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewLevel> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_Level WHERE LevelId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewLevel>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
