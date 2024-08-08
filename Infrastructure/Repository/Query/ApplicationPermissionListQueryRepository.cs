using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using Google.Api.Gax.ResourceNames;
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

        public async Task<long> Delete(long id, long permissionid)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    parameters.Add("permissionid", permissionid);
                    var query = string.Empty;

                    query += "Delete from  ApplicationRolePermission WHERE PermissionID=@permissionid;";
                    query += "Delete from  ApplicationPermission WHERE PermissionID=@id;";
                    
                    //var query = "DELETE  FROM ApplicationPermission WHERE PermissionID = @id";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;

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
                var parameters = new DynamicParameters();
              
                var query = "SELECT * FROM ApplicationPermission WHERE ParentID=60400";

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
                    var checkLink = await GetApplicationPermissionTop1Async();
                    long? permissionid = 0;
                    string nextPermissionLevel = "A01";

                    var allPermissions = await GetAllAsync();
                    var nextpermission = allPermissions.OrderByDescending(p => p.PermissionOrder).FirstOrDefault();

                    if (checkLink != null && checkLink.PermissionID > 0)
                    {
                        permissionid = (long)checkLink.PermissionID + 1;
                        
                        if(nextpermission != null)
                        {
                            if (!string.IsNullOrEmpty(nextpermission.PermissionOrder))
                            {
                                int currentLevelNumber = int.Parse(nextpermission.PermissionOrder.Substring(1));
                                nextPermissionLevel = "A" + (currentLevelNumber + 1).ToString();
                            }
                        }                        

                    }

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("PermissionID", permissionid);                        
                        parameters.Add("PermissionURL", applicationPermission.PermissionURL, DbType.String);
                        parameters.Add("PermissionName", applicationPermission.PermissionName, DbType.String);
                        parameters.Add("PermissionLevel", nextPermissionLevel, DbType.String);

                        var query = "INSERT INTO ApplicationPermission(PermissionID,PermissionURL,PermissionName,ParentID,PermissionLevel,PermissionOrder,IsDisplay,IsHeader,IsNewPortal,Component,Name,IsCmsApp,IsMobile,IsPermissionURL) VALUES (@PermissionID,@PermissionURL,@PermissionName,60400,1,@PermissionLevel,1,1,1,'PortalUrl','PortalUrl',1,0,1)";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);                            

                        return rowsAffected;
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

        async Task<long> IApplicationPermissionListQueryRepository.Update(ApplicationPermission applicationPermission)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("PermissionID", applicationPermission.PermissionID, DbType.Int64);
                        parameters.Add("PermissionURL", applicationPermission.PermissionURL, DbType.String);
                        parameters.Add("PermissionName", applicationPermission.PermissionName, DbType.String);                           

                        var query = "UPDATE ApplicationPermission SET PermissionURL = @PermissionURL ,PermissionName = @PermissionName WHERE PermissionID = @PermissionID";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
                            
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

        private async Task<ApplicationPermission> GetApplicationPermissionTop1Async()
        {
            try
            {
                var query = "SELECT TOP 1 * FROM ApplicationPermission ORDER BY PermissionID DESC;";

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


