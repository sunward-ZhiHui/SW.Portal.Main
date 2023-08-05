using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using NAV;
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
        public async Task<IReadOnlyList<ViewFmglobalLine>> GetAllbyIdsAsync(Int64 id)
        {
            try
            {
                var query = "select  * from FMGlobalLine WHERE FmglobalId = @FmglobalId";
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
        public async Task<FmglobalLine> GetFMGlobalLineFromIDAsync(long? FmglobalLineId)
        {
            try
            {
                var query = "select  * from FMGlobalLine WHERE FmglobalLineId=" + FmglobalLineId;
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<FmglobalLine>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewFmglobalLine>> GetAllByLocationFromAsync(Int64 id)
        {
            try
            {
                var query = "select  t2.* from FMGlobalMove t1 JOIN FMGlobalLine t2 ON t1.FMGlobalLineID=t2.FMGlobalLineID WHERE   t1.IsHandQty=1 AND t1.LocationToID=@LocationFromID";
                var parameters = new DynamicParameters();
                parameters.Add("LocationFromID", id, DbType.Int64);
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
        public async Task<FmglobalMove> FMGlobalMoveAynsc(long? FmglobalLineId)
        {
            try
            {
                var query = "select  * from FMGlobalMove WHERE IsHandQty=0 AND  TransactionQty=1 AND FmglobalLineId=" + FmglobalLineId;
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<FmglobalMove>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<FmglobalMove> GetFMGlobalMoveExits(long? LocationID, long? LocationToID, long? FmglobalId, int? IsHandQty, int? TransactionQty)
        {
            try
            {
                var query = "select  * from FMGlobalMove WHERE IsHandQty=@IsHandQty AND LocationToID=@LocationToID AND  LocationID=@LocationID AND TransactionQty=@TransactionQty AND FmglobalLinePreviousId=@FmglobalLinePreviousId";
                var parameterss = new DynamicParameters();
                parameterss.Add("LocationID", LocationID);
                parameterss.Add("LocationToID", LocationToID);
                parameterss.Add("FmglobalLinePreviousId", FmglobalId);
                parameterss.Add("IsHandQty", IsHandQty);
                parameterss.Add("TransactionQty", TransactionQty);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<FmglobalMove>(query, parameterss));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<FmglobalMove> GetFMGlobalMoveCheckExits(long? LocationID, long? LocationToID, long? FmglobalId, long? FmglobalLineId, int? IsHandQty, int? TransactionQty)
        {
            try
            {
                var query = "select  * from FMGlobalMove WHERE IsHandQty=@IsHandQty AND LocationToID=@LocationToID AND  LocationID=@LocationID AND TransactionQty=@TransactionQty AND  FmglobalLineId=@FmglobalLineId AND FmglobalId=@FmglobalId";
                var parameterss = new DynamicParameters();
                parameterss.Add("LocationID", LocationID);
                parameterss.Add("LocationToID", LocationToID);
                parameterss.Add("FmglobalId", FmglobalId);
                parameterss.Add("FmglobalLineId", FmglobalLineId);
                parameterss.Add("IsHandQty", IsHandQty);
                parameterss.Add("TransactionQty", TransactionQty);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<FmglobalMove>(query, parameterss));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long?> UpdatePreviosMoveQty(long? Id, long? LocationID, long? ModifiedByUserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    long lastInsertedRecordId = 0;
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var result = await FMGlobalMoveAynsc(Id);
                            if (result != null)
                            {
                                var querys = "UPDATE FmglobalMove SET TransactionQty=@TransactionQty,IsHandQty = @IsHandQty WHERE  FmglobalLineId = @FmglobalLinePreviousId";
                                var parameterss = new DynamicParameters();
                                parameterss.Add("IsHandQty", 0);
                                parameterss.Add("TransactionQty", 0);
                                parameterss.Add("FmglobalLinePreviousId", result.FmglobalLinePreviousId);
                                await connection.QuerySingleOrDefaultAsync<long>(querys, parameterss, transaction);


                                var parameters = new DynamicParameters();
                                parameters.Add("FmglobalMoveId", result.FmglobalMoveId);
                                parameters.Add("IsHandQty", 1);
                                parameters.Add("TransactionQty", 0);
                                parameters.Add("ModifiedByUserId", ModifiedByUserId);
                                parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                                var query = "UPDATE FmglobalMove SET ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId,TransactionQty=@TransactionQty,IsHandQty = @IsHandQty WHERE  FmglobalMoveId = @FmglobalMoveId";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                                lastInsertedRecordId = 1;
                                transaction.Commit();
                            }
                            return lastInsertedRecordId;

                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewFmglobalLine>> GetFmGlobalLineByPalletEntryNoAsync(long? CompanyId)
        {
            try
            {
                var query = "select  t1.* from FMGlobalLine t1 JOIN FMGlobal t2 ON t1.FmglobalId=t2.FmglobalId WHERE t2.CompanyID=" + CompanyId;
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


