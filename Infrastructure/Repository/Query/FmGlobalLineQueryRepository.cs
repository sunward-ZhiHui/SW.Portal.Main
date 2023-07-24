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
    public class FmGlobalLineQueryRepository : QueryRepository<ViewFmglobalLine>, IFmGlobalLineQueryRepository
    {
        public FmGlobalLineQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewFmglobalLine>> GetAllAsync(Int64 id)
        {
            try
            {
                var query = "select  * from ViewFmglobalLine WHERE FmglobalId = @FmglobalId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalId", id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewFmglobalLine>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewFmglobalLine> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM FmglobalLine  WHERE FmglobalLineId = @FmglobalLineId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalLineId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewFmglobalLine>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewFmglobalLine> GetByPalletNoAsync(string PalletNoYear)
        {
            try
            {
                var query = "SELECT * FROM FmglobalLine WHERE PalletNoYear =@PalletNoYear order by PalletNoAuto desc";
                var parameters = new DynamicParameters();
                parameters.Add("PalletNoYear", PalletNoYear);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewFmglobalLine>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}


