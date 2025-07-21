using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    
    public class SPCDataFinishedProdctQueryRepository : DbConnector, ISPCDataFinishedProdctQueryRepository
    {
        public SPCDataFinishedProdctQueryRepository(IConfiguration configuration)
           : base(configuration)
        {

        }

        public async Task<bool> DeleteAsync(long ID)
        {
            try
            {
                var query = "DELETE FROM SPCDataFinishedProdct WHERE ID  = @ID";

                using (var connection = CreateConnection())
                {
                    var rowsAffected = await connection.ExecuteAsync(query, new { ID = ID });
                    return rowsAffected > 0;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<SPCDataFinishedProdct>> GetAllAsync()
        {
            try
            {                
                var query = @"select  * from SPCDataFinishedProdct";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<SPCDataFinishedProdct>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<SPCDataFinishedProdct> InsertAsync(SPCDataFinishedProdct SPCDataFinishedProdct)
        {
            try
            {
                var query = @"INSERT INTO SPCDataFinishedProdct(BatchNo,Value,Remarks,AddedByUserID,AddedDate,SessionID)
                             VALUES ( @BatchNo, @Value,@Remarks,@AddedByUserID,@AddedDate, @SessionID);
                             SELECT CAST(SCOPE_IDENTITY() as int) ";

                using (var connection = CreateConnection())
                {
                    SPCDataFinishedProdct.ID = await connection.QuerySingleAsync<long>(query, SPCDataFinishedProdct);
                    return SPCDataFinishedProdct;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<SPCDataFinishedProdct> UpdateAsync(SPCDataFinishedProdct dataItem, SPCDataFinishedProdct changedItem)
        {
            try
            {
                var query = @"UPDATE SPCDataFinishedProdct SET
                        BatchNo = @BatchNo,
                        Value = @Value,
                        Remarks = @Remarks,
                        ModifiedByUserID = @ModifiedByUserID,
                        ModifiedDate = @ModifiedDate,
                        SessionID = @SessionID
                      WHERE ID = @ID";

                var parameters = new
                {
                    ID = dataItem.ID,
                    BatchNo = changedItem.BatchNo,
                    Value = changedItem.Value,
                    Remarks= changedItem.Remarks,
                    ModifiedByUserID = changedItem.ModifiedByUserID,
                    ModifiedDate = changedItem.ModifiedDate,
                    SessionID = changedItem.SessionId
                };

                using var connection = CreateConnection();
                await connection.ExecuteAsync(query, parameters);
                return changedItem;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
