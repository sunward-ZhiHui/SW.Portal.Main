using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Views;

namespace Infrastructure.Repository.Query
{
    public class ApplicationMasterDetailQueryRepository : QueryRepository<View_ApplicationMasterDetail>, IApplicationMasterDetailQueryRepository
    {
        public ApplicationMasterDetailQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ApplicationMasterDetail>> GetAllAsync()
        {
            try
            {
                var query = "select * from ApplicationMasterDetail";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterDetail>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<View_ApplicationMasterDetail>> GetApplicationMasterByCode(long? Id)
        {
            try
            {
                var query = "select * from view_ApplicationMasterDetail WHERE ApplicationMasterCodeID =" + "'" + Id + "'";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_ApplicationMasterDetail>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_ApplicationMasterDetail> GetByIdAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM view_ApplicationMasterDetail WHERE ApplicationMasterDetailID = @ApplicationMasterDetailID";
                var parameters = new DynamicParameters();
                parameters.Add("ApplicationMasterDetailID", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_ApplicationMasterDetail>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationMaster> GetByApplicationMasterCodeAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationMaster order by ApplicationMasterCodeId desc";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationMaster>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationMaster> InsertApplicationMaster(ApplicationMaster value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ApplicationMasterId", value.ApplicationMasterId);
                        parameters.Add("ApplicationMasterName", value.ApplicationMasterName, DbType.String);
                        parameters.Add("ApplicationMasterDescription", value.ApplicationMasterDescription, DbType.String);
                        if (value.ApplicationMasterId > 0)
                        {
                            var query = "UPDATE ApplicationMaster SET ApplicationMasterName=@ApplicationMasterName,ApplicationMasterDescription = @ApplicationMasterDescription " +
                                "WHERE ApplicationMasterId = @ApplicationMasterId";
                            await connection.ExecuteAsync(query, parameters);
                        }
                        else
                        {
                            var checkLink = await GetByApplicationMasterCodeAsync();
                            long? ApplicationMasterCodeId = 100;
                            if (checkLink != null && checkLink.ApplicationMasterCodeId > 0)
                            {
                                ApplicationMasterCodeId = (long)checkLink.ApplicationMasterCodeId + 1;
                            }

                            parameters.Add("ApplicationMasterCodeId", ApplicationMasterCodeId);
                            var query = "INSERT INTO [ApplicationMaster](ApplicationMasterName,ApplicationMasterDescription,ApplicationMasterCodeId) OUTPUT INSERTED.ApplicationMasterId VALUES " +
                                "(@ApplicationMasterName,@ApplicationMasterDescription,@ApplicationMasterCodeId)";

                            value.ApplicationMasterId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }

                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
