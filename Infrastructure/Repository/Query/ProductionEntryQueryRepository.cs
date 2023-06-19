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
    public class ProductionEntryQueryRepository : QueryRepository<ViewProductionEntry>, IProductionEntryQueryRepository
    {
        public ProductionEntryQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ViewProductionEntry>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM view_ProductionEntry";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewProductionEntry>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

     
    }
}

