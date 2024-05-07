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
using Core.EntityModels;

namespace Infrastructure.Repository.Query
{
    public class ApplicationMasterParentQueryRepository : QueryRepository<ApplicationMasterParent>, IApplicationMasterParentQueryRepository
    {
        public ApplicationMasterParentQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ApplicationMasterParent>> GetAllAsync()
        {
            try
            {
                var query = "select * from ApplicationMasterParent";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterParent>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ApplicationMasterParent>> GetAllByParentAsync()
        {
            try
            {
                var query = "select * from ApplicationMasterParent where ParentID is null";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterParent>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationMasterParent> GetByApplicationMasterParentCodeAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationMasterParent order by ApplicationMasterParentCodeId desc";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationMasterParent>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationMasterParent> InsertApplicationMasterParent(ApplicationMasterParent value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var checkLink = await GetByApplicationMasterParentCodeAsync();
                        var parameters = new DynamicParameters();
                        parameters.Add("ApplicationMasterParentId", value.ApplicationMasterParentId);
                        parameters.Add("ApplicationMasterParentCodeId", value.ApplicationMasterParentCodeId);
                        parameters.Add("ApplicationMasterName", value.ApplicationMasterName, DbType.String);
                        parameters.Add("Description", value.Description, DbType.String);
                        if (value.ApplicationMasterParentId > 0)
                        {
                            var query = "UPDATE ApplicationMasterParent SET ApplicationMasterName=@ApplicationMasterName,Description = @Description " +
                                "WHERE ApplicationMasterParentId = @ApplicationMasterParentId";
                            await connection.ExecuteAsync(query, parameters);
                            if (!string.IsNullOrEmpty(value.ApplicationMasterName2))
                            {
                                var parameters2 = new DynamicParameters();
                                parameters2.Add("ApplicationMasterName", value.ApplicationMasterName2, DbType.String);
                                parameters2.Add("Description", value.Description2, DbType.String);
                                parameters2.Add("ParentId", value.ApplicationMasterParentCodeId);
                                if (value.ApplicationMasterParentCodeId2 > 0)
                                {
                                    parameters2.Add("ApplicationMasterParentCodeId", value.ApplicationMasterParentCodeId2);
                                    var query22 = "UPDATE ApplicationMasterParent SET ApplicationMasterName=@ApplicationMasterName,Description = @Description " +
                                    "WHERE ApplicationMasterParentCodeId = @ApplicationMasterParentCodeId";
                                    await connection.ExecuteAsync(query22, parameters2);
                                    if (value.ApplicationMasterParentCodeId3 > 0)
                                    {
                                        var parameters3 = new DynamicParameters();
                                        parameters3.Add("ApplicationMasterParentCodeId", value.ApplicationMasterParentCodeId3);
                                        parameters3.Add("ApplicationMasterName", value.ApplicationMasterName3, DbType.String);
                                        parameters3.Add("Description", value.Description3, DbType.String);
                                        var query33 = "UPDATE ApplicationMasterParent SET ApplicationMasterName=@ApplicationMasterName,Description = @Description WHERE ApplicationMasterParentCodeId = @ApplicationMasterParentCodeId";
                                        await connection.ExecuteAsync(query33, parameters3);
                                    }
                                    else
                                    {
                                        long? ApplicationMasterParentCodeId3 = 101;
                                        if (checkLink != null && checkLink.ApplicationMasterParentCodeId > 0)
                                        {
                                            ApplicationMasterParentCodeId3 = (long)checkLink.ApplicationMasterParentCodeId + 1;
                                        }
                                        if (!string.IsNullOrEmpty(value.ApplicationMasterName3))
                                        {
                                            var parameters3 = new DynamicParameters();
                                            parameters3.Add("ApplicationMasterParentCodeId", ApplicationMasterParentCodeId3);
                                            parameters3.Add("ApplicationMasterName", value.ApplicationMasterName3, DbType.String);
                                            parameters3.Add("Description", value.Description3, DbType.String);
                                            parameters3.Add("ParentId", value.ApplicationMasterParentCodeId2);
                                            var query3 = "INSERT INTO [ApplicationMasterParent](ApplicationMasterName,Description,ApplicationMasterParentCodeId,ParentId) OUTPUT INSERTED.ApplicationMasterParentId VALUES " +
                                            "(@ApplicationMasterName,@Description,@ApplicationMasterParentCodeId,@ParentId)";
                                            var ApplicationMasterParentId3 = await connection.QuerySingleOrDefaultAsync<long>(query3, parameters3);
                                        }
                                    }
                                }
                                else
                                {
                                    long? ApplicationMasterParentCodeId2 = 101;
                                    if (checkLink != null && checkLink.ApplicationMasterParentCodeId > 0)
                                    {
                                        ApplicationMasterParentCodeId2 = (long)checkLink.ApplicationMasterParentCodeId + 1;
                                    }
                                    parameters2.Add("ApplicationMasterParentCodeId", ApplicationMasterParentCodeId2);
                                    var query2 = "INSERT INTO [ApplicationMasterParent](ApplicationMasterName,Description,ApplicationMasterParentCodeId,ParentId) OUTPUT INSERTED.ApplicationMasterParentId VALUES " +
                                    "(@ApplicationMasterName,@Description,@ApplicationMasterParentCodeId,@ParentId)";
                                    var ApplicationMasterParentId2 = await connection.QuerySingleOrDefaultAsync<long>(query2, parameters2);

                                    if (!string.IsNullOrEmpty(value.ApplicationMasterName3))
                                    {
                                        var ApplicationMasterParentCodeId3 = ApplicationMasterParentCodeId2 + 1;
                                        var parameters3 = new DynamicParameters();
                                        parameters3.Add("ApplicationMasterParentCodeId", ApplicationMasterParentCodeId3);
                                        parameters3.Add("ApplicationMasterName", value.ApplicationMasterName3, DbType.String);
                                        parameters3.Add("Description", value.Description3, DbType.String);
                                        parameters3.Add("ParentId", ApplicationMasterParentCodeId2);
                                        var query3 = "INSERT INTO [ApplicationMasterParent](ApplicationMasterName,Description,ApplicationMasterParentCodeId,ParentId) OUTPUT INSERTED.ApplicationMasterParentId VALUES " +
                                        "(@ApplicationMasterName,@Description,@ApplicationMasterParentCodeId,@ParentId)";
                                        var ApplicationMasterParentId3 = await connection.QuerySingleOrDefaultAsync<long>(query3, parameters3);
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                            long? ApplicationMasterParentCodeId = 101;
                            if (checkLink != null && checkLink.ApplicationMasterParentCodeId > 0)
                            {
                                ApplicationMasterParentCodeId = (long)checkLink.ApplicationMasterParentCodeId + 1;
                            }

                            parameters.Add("ApplicationMasterParentCodeId", ApplicationMasterParentCodeId);
                            var query = "INSERT INTO [ApplicationMasterParent](ApplicationMasterName,Description,ApplicationMasterParentCodeId) OUTPUT INSERTED.ApplicationMasterParentId VALUES " +
                                "(@ApplicationMasterName,@Description,@ApplicationMasterParentCodeId)";
                            value.ApplicationMasterParentId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            if (!string.IsNullOrEmpty(value.ApplicationMasterName2))
                            {
                                var ApplicationMasterParentCodeId2 = ApplicationMasterParentCodeId + 1;
                                var parameters2 = new DynamicParameters();
                                parameters2.Add("ApplicationMasterParentCodeId", ApplicationMasterParentCodeId2);
                                parameters2.Add("ApplicationMasterName", value.ApplicationMasterName2, DbType.String);
                                parameters2.Add("Description", value.Description2, DbType.String);
                                parameters2.Add("ParentId", ApplicationMasterParentCodeId);
                                var query2 = "INSERT INTO [ApplicationMasterParent](ApplicationMasterName,Description,ApplicationMasterParentCodeId,ParentId) OUTPUT INSERTED.ApplicationMasterParentId VALUES " +
                                "(@ApplicationMasterName,@Description,@ApplicationMasterParentCodeId,@ParentId)";
                                var ApplicationMasterParentId2 = await connection.QuerySingleOrDefaultAsync<long>(query2, parameters2);

                                if (!string.IsNullOrEmpty(value.ApplicationMasterName3))
                                {
                                    var ApplicationMasterParentCodeId3 = ApplicationMasterParentCodeId2 + 1;
                                    var parameters3 = new DynamicParameters();
                                    parameters3.Add("ApplicationMasterParentCodeId", ApplicationMasterParentCodeId3);
                                    parameters3.Add("ApplicationMasterName", value.ApplicationMasterName3, DbType.String);
                                    parameters3.Add("Description", value.Description3, DbType.String);
                                    parameters3.Add("ParentId", ApplicationMasterParentCodeId2);
                                    var query3 = "INSERT INTO [ApplicationMasterParent](ApplicationMasterName,Description,ApplicationMasterParentCodeId,ParentId) OUTPUT INSERTED.ApplicationMasterParentId VALUES " +
                                    "(@ApplicationMasterName,@Description,@ApplicationMasterParentCodeId,@ParentId)";
                                    var ApplicationMasterParentId3 = await connection.QuerySingleOrDefaultAsync<long>(query3, parameters3);
                                }
                            }
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
