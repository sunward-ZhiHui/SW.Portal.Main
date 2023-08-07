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
                var query = "select  * from view_FMGlobalLineItem WHERE FmglobalLineId = @FmglobalLineId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalLineId", id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewFmglobalLineItem>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewFmglobalLineItem>> GetPalletMovementListingdAsync()
        {
            try
            {
                var query = "select  * from view_FMGlobalLineItem";
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
        public async Task<IReadOnlyList<View_FMGlobalMovePackingList>> GetPalletMovementListingReportdAsync()
        {
            try
            {
                var query = "select  * from View_FMGlobalMovePackingList";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_FMGlobalMovePackingList>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewFmglobalLineItem>> GetAllByIdAsync(Int64 id)
        {
            try
            {
                var query = "select  * from FMGlobalLineItem WHERE FmglobalLineId = @FmglobalLineId";
                var parameters = new DynamicParameters();
                parameters.Add("FmglobalLineId", id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewFmglobalLineItem>(query, parameters)).ToList();
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
        public async Task<ViewFmglobalLineItem> GetBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM FmglobalLineItem  WHERE SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);

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


