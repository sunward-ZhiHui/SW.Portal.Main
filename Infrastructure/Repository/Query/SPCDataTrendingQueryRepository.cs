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
    
    public class SPCDataTrendingQueryRepository : DbConnector, ISPCDataTrendingQueryRepository
    {
        public SPCDataTrendingQueryRepository(IConfiguration configuration)
           : base(configuration)
        {

        }

        public async Task<bool> DeleteAsync(long ID)
        {
            try
            {
                var query = "DELETE FROM SPCDataTrending WHERE ID  = @ID";

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

        public async Task<IReadOnlyList<SPCDataTrending>> GetAllAsync()
        {
            try
            {                
                var query = @"select  * from SPCDataTrending";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<SPCDataTrending>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<SPCDataTrending> InsertAsync(SPCDataTrending SPCDataTrending)
        {
            try
            {
                var query = @"INSERT INTO SPCDataTrending(QCRefNo,Assay,AddedByUserID,AddedDate,SessionID)
                             VALUES ( @QCRefNo, @Assay,@AddedByUserID,@AddedDate, @SessionID);
                             SELECT CAST(SCOPE_IDENTITY() as int) ";

                using (var connection = CreateConnection())
                {
                    SPCDataTrending.ID = await connection.QuerySingleAsync<long>(query, SPCDataTrending);
                    return SPCDataTrending;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<SPCDataTrending> UpdateAsync(SPCDataTrending dataItem, SPCDataTrending changedItem)
        {
            try
            {
                var query = @"UPDATE SPCDataTrending SET
                        QCRefNo = @QCRefNo,
                        Assay = @Assay,
                        ModifiedByUserID = @ModifiedByUserID,
                        ModifiedDate = @ModifiedDate,
                        SessionID = @SessionID
                      WHERE ID = @ID";

                var parameters = new
                {
                    ID = dataItem.ID,
                    QCRefNo = changedItem.QCRefNo,
                    Assay = changedItem.Assay,
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
