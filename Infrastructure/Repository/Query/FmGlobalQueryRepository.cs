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
    public class FmGlobalQueryRepository : QueryRepository<ViewFmglobal>, IFmGlobalQueryRepository
    {
        public FmGlobalQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewFmglobal>> GetAllAsync()
        {
            try
            {
                var query = "select  * from View_Fmglobal";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewFmglobal>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewFmglobal> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM View_Fmglobal  WHERE FmglobalId = @FmglobalId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewFmglobal>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewFmglobal> GetBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM View_Fmglobal  WHERE SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewFmglobal>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}


