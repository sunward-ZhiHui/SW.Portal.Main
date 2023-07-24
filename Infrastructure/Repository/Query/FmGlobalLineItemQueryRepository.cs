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
    public class FmGlobalLineItemQueryRepository : QueryRepository<ViewFmglobalLineItem>, IFmGlobalLineItemQueryRepository
    {
        public FmGlobalLineItemQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewFmglobalLineItem>> GetAllAsync(Int64 id)
        {
            try
            {
                var query = "select  * from ViewFmglobalLineItem WHERE FmglobalLineId = @FmglobalLineId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalId", id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewFmglobalLineItem>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewFmglobalLineItem> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM FmglobalLineItem  WHERE FmglobalLineItemId = @FmglobalLineItemId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalLineItemId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewFmglobalLineItem>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}


