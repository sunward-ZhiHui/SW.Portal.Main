using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class ConsumptionQueryRepository : QueryRepository<ViewConsumptionEntry>, IConsumptionQueryRepository
    {
        public ConsumptionQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ViewConsumptionEntry>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM view_ConsumptionEntry";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewConsumptionEntry>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewConsumptionLines>> GetAllLineAsync()
        {
            try
            {
                
                var query = "SELECT * FROM view_AppConsumptionsLines";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewConsumptionLines>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
   
}
