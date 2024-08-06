using Application.Queries;
using Core.Entities;
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
    public class ApplicationPermissionListQueryRepository : QueryRepository<ApplicationPermission>, IApplicationPermissionListQueryRepository
    {
        public ApplicationPermissionListQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<long> Delete(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("PermissionID", id);

                            var query = "DELETE  FROM ApplicationPermission WHERE PermissionID = @PermissionID";


                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
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

        public async Task<IReadOnlyList<ApplicationPermission>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationPermission";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationPermission>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(ApplicationPermission applicationPermission)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var checkLink = await GetByApplicationMasterCodeAsync();
                        long? permissionid = 0;
                        if (checkLink != null && checkLink.PermissionID > 0)
                        {
                            permissionid = (long)checkLink.PermissionID + 1;
                        }
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("PermissionID", permissionid);
                        
                            parameters.Add("PermissionURL", applicationPermission.PermissionURL, DbType.String);
                            parameters.Add("PermissionName", applicationPermission.PermissionName, DbType.String);

                            parameters.Add("PortalUrl", applicationPermission.Name, DbType.String);


                            var query = "INSERT INTO ApplicationPermission(PermissionURL,PermissionName,Name,PermissionID) VALUES (@PermissionURL,@PermissionName,@PortalUrl,@PermissionID)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
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

        async Task<long> IApplicationPermissionListQueryRepository.Update(ApplicationPermission applicationPermission)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {

                            var parameters = new DynamicParameters();

                            parameters.Add("PermissionID", applicationPermission.PermissionID, DbType.Int64);
                            parameters.Add("PermissionURL", applicationPermission.PermissionURL, DbType.String);
                            parameters.Add("PermissionName", applicationPermission.PermissionName, DbType.String);


                            var query = "UPDATE ApplicationPermission SET PermissionURL = @PermissionURL ,PermissionName = @PermissionName  WHERE PermissionID = @PermissionID";


                                var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                                transaction.Commit();

                                return rowsAffected;
                            
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

        public async Task<ApplicationPermission> GetByApplicationMasterCodeAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationPermission ORDER BY PermissionID DESC;";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationPermission>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}


