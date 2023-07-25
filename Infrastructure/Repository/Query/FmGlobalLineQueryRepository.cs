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
                var query = "select  * from view_FMGlobalLine WHERE FmglobalId = @FmglobalId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalId", id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewFmglobalLine>(query, parameters)).ToList();
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
                var query = "SELECT * FROM view_FMGlobalLine  WHERE FmglobalLineId = @FmglobalLineId";
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
        public async Task<ViewFmglobalLine> GetBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM view_FMGlobalLine  WHERE SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);

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
        public async Task<ViewFmglobalLine> GetByPalletNoAsync(string PalletNoYear, long? CompanyId)
        {
            try
            {
                var query = "SELECT * FROM FmglobalLine t1 JOIN Fmglobal t2 ON t1.FmglobalId=t2.FmglobalId  WHERE t2.CompanyId=@CompanyId AND t1.PalletNoYear=@PalletNoYear order by t1.PalletNoAuto desc";
                var parameters = new DynamicParameters();
                parameters.Add("PalletNoYear", PalletNoYear);
                parameters.Add("CompanyId", CompanyId);
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


