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
    public class ProductionOutputQueryRepository : QueryRepository<ViewProductionOutput>, IProductionOutputQueryRepository
    {
        public ProductionOutputQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ViewProductionOutput>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM view_ProductionOutput";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewProductionOutput>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
