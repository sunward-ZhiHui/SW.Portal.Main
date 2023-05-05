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
    public class LayOutPlanTypeQueryRepository : QueryRepository<ViewLayOutPlanType>,ILayOutPlanTypeQueryRepository
    {
        public LayOutPlanTypeQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewLayOutPlanType>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_LayOutPlanType";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewLayOutPlanType>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewLayOutPlanType> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_LayOutPlanType WHERE  LayoutPlanTypeId  = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewLayOutPlanType>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
   
}
